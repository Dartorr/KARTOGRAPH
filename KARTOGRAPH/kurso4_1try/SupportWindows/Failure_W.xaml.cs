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
using System.Windows.Shapes;

namespace kurso4_1try
{
    /// <summary>
    /// Логика взаимодействия для Failure_W.xaml
    /// </summary>
    public partial class Failure_W : Window
    {
        public Failure_W()
        {
            InitializeComponent();
        }

        public Failure_W(bool Or, bool[] sold, int CurrentF, int MaxF, int CurrentEnemy, int MaxE)
        {
            InitializeComponent();


            bool[] correct = { false, false };
            int[] degr = { 2, 2 };

            if (!Or)
            {
                orient.Text = "Ориентиры выставлены верно";
                orient.Foreground = Brushes.Green;
                correct[0] = true;
            }
            else
            {
                orient.Text = "Ориентиры установлены в неправильном порядке";
                orient.Foreground = Brushes.Red;
            }

            bool flag = true;
            foreach (bool k in sold) if (!k) flag = false;
            if (flag)
            {
                correct[1] = true;
                forces.Text = "Все силы были выставлены";
                forces.Foreground = Brushes.Green;
            }
            else
            {
                forces.Text = "Не все силы были выставлены";
                forces.Foreground = Brushes.Red;
            }

            if (correct[0] && correct[1])
            {
                correct_common.Text = "5";
                correct_common.Foreground = Brushes.Green;
                degr[0] = 5;
            }
            else if ((!correct[0]) && (!correct[1]))
            {
                correct_common.Text = "2";
                correct_common.Foreground = Brushes.Red;
                degr[0] = 2;
            }
            else
            {
                correct_common.Text = "3";
                correct_common.Foreground = Brushes.Orange;
                degr[0] = 3;
            }

            Killed.Text = (MaxE - CurrentEnemy).ToString();
            lost.Text = (MaxF - CurrentF).ToString();

            if ((MaxF - CurrentF) > (MaxF * 0.2))
            {
                degree_combat.Text = "3";
                degree_combat.Foreground = Brushes.Orange;

                degr[0] = 3;
            }
            else
            {
                degree_combat.Text = "5";
                degree_combat.Foreground = Brushes.Green;
                degr[0] = 5;
            }

            int cd = (Int32)((degr[0] + degr[1]) / 2);
            degree_common.Text = (cd).ToString();
            if (cd < 3) degree_common.Foreground = Brushes.Red;
            else if (cd < 4) degree_common.Foreground = Brushes.Orange;
            else degree_common.Foreground = Brushes.Green;
        }
    }
}
