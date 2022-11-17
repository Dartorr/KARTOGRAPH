using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using System.Windows.Threading;

namespace kurso4_1try
{
    public partial class MainWindow : Window
    {
        public void lock_army_buttons()
        {
            lock_rect_army.Height = splitter.Margin.Top;

            RP_Icon_Rect.IsEnabled = false;
            SV_Icon_Rect.IsEnabled = false;
            BMP_Icon_Rect.IsEnabled = false;
            A_Icon_Rect.IsEnabled = false;
            RG_Icon_Rect.IsEnabled = false;
        }
        public void unlock_army_buttons()
        {
            lock_rect_army.Height = 0;

            RP_Icon_Rect.IsEnabled = true;
            SV_Icon_Rect.IsEnabled = true;
            BMP_Icon_Rect.IsEnabled = true;
            A_Icon_Rect.IsEnabled = true;
            RG_Icon_Rect.IsEnabled = true;
        }
        public void lock_info_buttons()
        {
            lock_rect_info.Height = this.Height;

            show_char_button.IsEnabled = false;


            //replace_current_pos.IsEnabled = false;

        }


        public void unlock_info_buttons()
        {
            lock_rect_info.Height = 0;

            show_char_button.IsEnabled = true;


            // replace_current_pos.IsEnabled = true;

        }
        private void or1_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //чистим второй комбобокс
            or2_combobox.Items.Clear();

            //добавляем все ориентиры, кроме уже выбранного
            List<string> k = (from T
                     in orientier_CCs
                              where T.or_name.Text != or1_combobox.SelectedItem
                              select T.or_name.Text).ToList();

            foreach (string t in k) or2_combobox.Items.Add(t);

            or2_combobox.IsEnabled = true;

        }
        private void or2_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            apply_sec.IsEnabled = true;
        }
        private void PM_ON(bool s)
        {
            PM_Ind.Visibility = Visibility.Visible;
            if (s)
            {
                place_rect.VerticalAlignment = VerticalAlignment.Top;
                place_rect.Height = this.Height / 3 * 2;
            }
            else
            {
                place_rect.VerticalAlignment = VerticalAlignment.Bottom;
                place_rect.Height = this.Height / 3;
            }
        }
        private void PM_OFF()
        {
            PM_Ind.Visibility = Visibility.Collapsed;
            place_rect.Height = 0;

        }
        private void RP_Icon_Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PM_OFF();
            PM_ON(false);
            PM_Ind.Visibility = Visibility.Visible;
            end_fortification.Visibility = Visibility.Hidden;
            placed_item_type = 1;

            placedSoldiersTypes[0] = true;

        }
        private void RG_Icon_Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PM_OFF();
            PM_ON(false);
            PM_Ind.Visibility = Visibility.Visible;
            end_fortification.Visibility = Visibility.Hidden;
            placed_item_type = 3;

            placedSoldiersTypes[1] = true;
        }
        private void SV_Icon_Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PM_OFF();
            PM_ON(false);
            PM_Ind.Visibility = Visibility.Visible;
            end_fortification.Visibility = Visibility.Hidden;
            placed_item_type = 2;

            placedSoldiersTypes[2] = true;

        }
        private void BMP_Icon_Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PM_OFF();
            PM_ON(false);
            PM_Ind.Visibility = Visibility.Visible;
            end_fortification.Visibility = Visibility.Hidden;
            placed_item_type = 4;

            placedSoldiersTypes[3] = true;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //лочим установку юнитов и их настройку до установки 2ух ориентиров
            lock_army_buttons();
            lock_info_buttons();

            //чистим записи, оставшиеся после прошлого сеанса
            using (SoldiersDb db = new SoldiersDb())
            {
                foreach (var t in db.Soldier)
                    db.Soldier.Remove(t);
                foreach (var t in db.Position)
                    db.Position.Remove(t);
                db.SaveChanges();
            }
        }
        private static double ToDegrees(double Angle) => Angle * 180 / Math.PI;
        private static double GetAngle(Point A, Point B)
        {
            // Получим косинус угла по формуле
            double cos = Math.Round((B.Y - A.Y) / (B.X - A.X), 9);
            // Вернем arccos полученного значения (в радианах!)
            return Math.Atan(cos);
        }
        public void createLine(Position tPos)
        {

            Line line1 = new Line();

            line1.X1 = (double)tPos.X;
            line1.Y1 = (double)tPos.Y;

            line1.X2 = (double)tPos.first_orientier_X;
            line1.Y2 = (double)tPos.first_orientier_Y;

            line1.Name = "line_" + lines_count.ToString();

            line1.Stroke = Brushes.Red;
            line1.StrokeThickness = 1;

            Main_Canvas.Children.Add(line1);
            line1.RegisterName(line1.Name, line1);

            lines_count++;

            Line line2 = new Line();

            line2.X1 = (double)tPos.X;
            line2.Y1 = (double)tPos.Y;

            line2.X2 = (double)tPos.second_orientier_X;
            line2.Y2 = (double)tPos.second_orientier_Y;

            line2.Name = "line_" + lines_count.ToString();

            line2.Stroke = Brushes.Red;
            line2.StrokeThickness = 1;

            Main_Canvas.Children.Add(line2);
            line2.RegisterName(line2.Name, line2);

            lines_count++;

            soldier_id_to_lines.Add(
                (selected_item_id,
                (line1.Name),
                (line2.Name))
                );
        }

        public void rotate_to_ors(int k, Point OR_1, Point OR_2)
        {

            Image img = (Image)Main_Canvas.FindName("img_ts_" + k.ToString());
            Point A = new Point(img.Margin.Left + img.Width / 2, img.Margin.Top + img.Width);
            RotateTransform oleg;

            Point average = new Point((OR_1.X + OR_2.X) / 2, (OR_1.Y + OR_2.Y) / 2);

            oleg = new RotateTransform();

            oleg.CenterX = 0;
            oleg.Angle = 0;
            oleg.CenterY = 0;

            if (A.X > average.X)
                oleg.Angle += ToDegrees(GetAngle(A, average));
            else
            {
                oleg.Angle = -180;
                oleg.CenterX = 0;

                oleg.CenterY = 0;
                oleg.Angle += ToDegrees(GetAngle(A, average));
            }

            img.RenderTransform = oleg;
        }

        private void fort_type_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fort_Add_Button.IsEnabled = true;
        }

        public double GetDistation(Point A, Position B)
        {
            double dist = Math.Sqrt(Math.Pow((double)B.X - A.X, 2) + Math.Pow((double)B.Y - A.Y, 2));
            return dist;
        }
    }
}
