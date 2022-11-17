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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Tutorial tut = new Tutorial();
            tut.ShowDialog();
        }



        private void place_rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((placed_item_type < 6) && (orientier_CCs.Count >= 2))//установка юнитов
                {
                    Image img = new Image();

                    Soldier soldier = new Soldier();
                    TTX tempTTX = new TTX();

                    soldier.TTX_ID = placed_item_type;
                    soldier.ID = count;
                    soldier.Side = true;

                    soldiers_temp_list.Add(soldier);

                    img.Height = 15;
                    img.Width = 15;
                    img.Margin = new Thickness(e.GetPosition(this).X - 10, e.GetPosition(this).Y - 10, 0, 0);

                    img.Visibility = Visibility.Visible;
                    img.HorizontalAlignment = HorizontalAlignment.Left;
                    img.VerticalAlignment = VerticalAlignment.Top;

                    img.MouseDown += friend_Hitbox_MouseDown;
                    img.Name = "img_ts_" + count.ToString();
                    img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_" + placed_item_type.ToString() + ".png"));
                    Main_Canvas.Children.Add(img);

                    img.RegisterName("img_ts_" + count.ToString(), img);


                    selected_item_id = count;
                    selected_item_changed();

                    count++;
                    PM_OFF();
                    lock_army_buttons();

                }
                else
                {
                    switch (placed_item_type)
                    {
                        case 10://ориентиры
                            {
                                orientier_CC t_OR = new orientier_CC(orientier_CCs.Count,
                        new Point(e.GetPosition(this).X, e.GetPosition(this).Y));
                                orientier_CCs.Add(t_OR);

                                if (orientier_CCs.Count > 1)
                                    if (t_OR.point_orientire.Y < orientier_CCs[orientier_CCs.Count - 2].point_orientire.Y)
                                        isOrientsPlacedCorrect = false;

                                Main_Canvas.Children.Add(t_OR);

                                PM_OFF();

                                if (orientier_CCs.Count >= 2)
                                {
                                    unlock_army_buttons();
                                    unlock_info_buttons();
                                }
                                RP_Count_All_Textblock_Copy.Text = orientier_CCs.Count.ToString();
                                break;
                            }
                        case 7://фортификации
                            {
                                if (is_fort_stand)
                                {
                                    is_fort_stand = false;
                                    fort_places.Add(fort_place_site);
                                    fort_count++;
                                }
                                else
                                {
                                    is_fort_stand = true;

                                    fort_place_site = e.GetPosition(this);
                                    Image img = new Image();
                                    img.Height = 7;
                                    img.Width = 40;
                                    img.Margin = new Thickness(e.GetPosition(this).X - 20, e.GetPosition(this).Y, 0, 0);

                                    img.Visibility = Visibility.Visible;
                                    img.HorizontalAlignment = HorizontalAlignment.Left;
                                    img.VerticalAlignment = VerticalAlignment.Top;

                                    img.Name = "fort_" + fort_count.ToString();
                                    img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/fort.png"));
                                    Main_Canvas.Children.Add(img);

                                    img.RegisterName("fort_" + fort_count.ToString(), img);

                                }
                                break;
                            }
                        default:
                            break;
                    }

                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Произошла непредвиденная ошибка. Попробуйте перезапустить программу");
                this.Close();
            }

        }

        private void friend_Hitbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //находим id солдата, на которого кликнули
            Image img = (Image)sender;
            string oleg = img.Name;
            oleg = oleg.Replace("img_ts_", "");
            int id = Int32.Parse(oleg);

            //вызываем нужную функцию
            if (soldiers_temp_list[selected_item_id].Position != null)
                selected_item_id = id;
            selected_item_changed();
        }
        public void selected_item_changed()
        {

            Soldier tmp_s = soldiers_temp_list[selected_item_id];
            //проверяем, задана ли позиция
            if (tmp_s.Position == null)
            {
                //если нет - лочим установку юнитов, пока не будет установлена позиция
                lock_army_buttons();
                begin_simulation_MI.IsEnabled = false;
            }
            else
            {
                //иначе анлочим установку юнитов, но лочим изменение позиции
                unlock_army_buttons();
                or1_combobox.IsEnabled = false;
                or2_combobox.IsEnabled = false;
            }

            current_eornot_textbox.Text = "Союзник (РБ)";

            switch (tmp_s.TTX_ID)
            {

                case 0:
                    {
                        current_class_textbox.Text = "Пехота";
                        current_type_textbox.Text = "Стрелок";
                        Selected_Icon_Img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_0.png"));
                        current_portrait.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_0.png"));
                        break;
                    }
                case 1:
                    {
                        current_class_textbox.Text = "Пехота";
                        current_type_textbox.Text = "Пулеметчик";
                        Selected_Icon_Img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_1.png"));
                        current_portrait.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/PKM.png"));
                        break;
                    }
                case 2:
                    {
                        current_class_textbox.Text = "Пехота";
                        current_type_textbox.Text = "Снайпер";
                        Selected_Icon_Img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_2.png"));
                        current_portrait.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/SR.png"));
                        break;
                    }
                case 3:
                    {
                        current_class_textbox.Text = "Пехота";
                        current_type_textbox.Text = "Гранатометчик";
                        Selected_Icon_Img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_3.png"));
                        current_portrait.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/RPG.png"));
                        break;
                    }
                case 4:
                    {
                        current_class_textbox.Text = "Техника";
                        current_type_textbox.Text = "БМП-2";
                        Selected_Icon_Img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/icon_4.png"));
                        current_portrait.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/Resources/BMP.png"));
                        break;
                    }
                default:
                    break;
            }
            if (soldiers_temp_list[selected_item_id].Position == null)
            {
                or1_combobox.IsEnabled = true;
                or1_combobox.Items.Clear();
                foreach (orientier_CC t in orientier_CCs)
                {
                    or1_combobox.Items.Add(t.or_name.Text);
                }
            }
        }
        private void or_ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {

            PM_ON(true);

            placed_item_type = 10;

        }

        private void apply_sec_Click(object sender, RoutedEventArgs e)
        {
            //немного валидации
            switch (placed_item_type)
            {
                case 1:
                    {
                        RP_Count_Textblock.Text = "1";
                        RP_Icon_Rect.MouseDown -= RP_Icon_Rect_MouseDown;
                        RP_Icon_Rect.IsEnabled = false;
                        break;
                    }
                case 3:
                    {
                        RG_Count_Textblock.Text = "1";
                        RG_Icon_Rect.MouseDown -= RG_Icon_Rect_MouseDown;
                        RG_Icon_Rect.IsEnabled = false;
                        break;
                    }
                case 2:
                    {
                        SV_Count_Textblock.Text = "1";
                        SV_Icon_Rect.MouseDown -= SV_Icon_Rect_MouseDown;
                        SV_Icon_Rect.IsEnabled = false;
                        break;
                    }
                case 4:
                    {
                        BMP_Count_Textblock.Text = "1";
                        BMP_Icon_Rect.MouseDown -= BMP_Icon_Rect_MouseDown;
                        BMP_Icon_Rect.IsEnabled = false;
                        break;
                    }

                default:
                    break;
            }

            //находим Image обьект солдата, который будем поворачивать, и ориентиры
            Image img = (Image)Main_Canvas.FindName("img_ts_" + selected_item_id);
            orientier_CC f_or = ((from t
                                  in orientier_CCs
                                  where t.or_name.Text == or1_combobox.SelectedItem
                                  select t).ToList())[0];
            orientier_CC s_or = ((from t
                                  in orientier_CCs
                                  where t.or_name.Text == or2_combobox.SelectedItem
                                  select t).ToList())[0];

            //создаем новую запись в бд
            Position tPos = new Position();

            tPos.Position_ID = selected_item_id;

            tPos.X = (int)img.Margin.Left;
            tPos.Y = (int)img.Margin.Top;

            tPos.first_orientier_X = (int)f_or.point_orientire.X;
            tPos.first_orientier_Y = (int)f_or.point_orientire.Y;

            tPos.second_orientier_X = (int)s_or.point_orientire.X;
            tPos.second_orientier_Y = (int)s_or.point_orientire.Y;

            Point sold_pos = new Point((double)tPos.X, (double)tPos.Y);

            soldiers_temp_list[selected_item_id].Position = tPos.Position_ID;

            using (SoldiersDb db = new SoldiersDb())
            {
                //записываем солдата и его позицию
                db.Soldier.Add(soldiers_temp_list[selected_item_id]);
                db.Position.Add(tPos);
                db.SaveChanges();
            }

            //создаем визуально сектора обстрела, поворачиваем иконку юнита, и анлочим выставление армии
            createLine(tPos);
            createLine(tPos);
            rotate_to_ors(selected_item_id, f_or.point_orientire, s_or.point_orientire);
            unlock_army_buttons();

            or1_combobox.IsEnabled = false;
            or2_combobox.IsEnabled = false;
            apply_sec.IsEnabled = false;
            begin_simulation_MI.IsEnabled = true;
        }

        private void end_fortification_Click(object sender, RoutedEventArgs e)
        {
            PM_OFF();
            placed_item_type = -1;
            end_fortification.Visibility = Visibility.Hidden;
        }
        private void Fort_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            PM_ON(false);
            placed_item_type = 7;
            end_fortification.Visibility = Visibility.Visible;
        }
        private void place_rect_MouseMove(object sender, MouseEventArgs e)
        {
            //функция нужна для поворота окопа вслед за мышкой в режиме установки
            if (is_fort_stand)//если мы устанавливаем фортификацию
            {
                //находим изображение, привязанное к уст.укреплению
                Image image = (Image)Main_Canvas.FindName("fort_" + fort_count);
                double angle;
                //рассчитываем угол между центром изображения и мышкой
                angle = ToDegrees(GetAngle(fort_place_site, e.GetPosition(this))) + 90;
                if (fort_place_site.X > e.GetPosition(this).X) angle += 180;
                //поворачиваем изображение вслез за мышкой
                RotateTransform oleg = new RotateTransform(angle);
                oleg.CenterX = 20;
                oleg.CenterY = 3.5;
                image.RenderTransform = oleg;
            }
        }
        private void tutorial_MI_Click(object sender, RoutedEventArgs e)
        {
            Tutorial tutorial = new Tutorial();
            tutorial.ShowDialog();
        }
        private void show_char_button_Click(object sender, RoutedEventArgs e)
        {
            Soldier temp_sold = soldiers_temp_list[selected_item_id];
            TTX k = new TTX();

            using (SoldiersDb db = new SoldiersDb())
            {
                k = (from t in db.TTX where t.ID_TTX == temp_sold.TTX_ID select t).ToList()[0];
            }

            soldier_game_specifications _Specifications_window = new soldier_game_specifications(k);
            _Specifications_window.Show();
        }

    }
}
