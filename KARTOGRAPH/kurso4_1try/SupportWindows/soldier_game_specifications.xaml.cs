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
    /// Логика взаимодействия для soldier_game_specifications.xaml
    /// </summary>
    public partial class soldier_game_specifications : Window
    {
        public soldier_game_specifications()
        {
            InitializeComponent();
        }

        public soldier_game_specifications(TTX tTX)
        {
            InitializeComponent();

            switch (tTX.ID_TTX)
            {
                case 0:
                    {
                        type.Text = "Стрелок";
                        break;
                    }
                case 1:
                    {
                        type.Text = "Пулеметчик";
                        break;
                    }
                case 2:
                    {
                        type.Text = "Снайпер";
                        break;
                    }
                case 3:
                    {
                        type.Text = "Гранатометчик";
                        break;
                    }
                case 4:
                    {
                        type.Text = "БМП";
                        break;
                    }
                default:
                    break;
            }

            par.Text = tTX.atk_primary_rad.ToString();
            if (tTX.atk_secondary_rad != null)
            {
                sar.Text = tTX.atk_secondary_rad.ToString();

                acc_sec.Text = tTX.hit_chance_secondary.ToString();
            }
            else
            {
                sar.Text = " --- ";

                acc_sec.Text = " --- ";
            }
            speed.Text = tTX.speed.ToString();
            if ((bool)tTX.is_primary_percing_armor) ipa.Text = "Да";
            else ipa.Text = "Нет";
            bl.Text = tTX.burst_length.ToString();
            syrn.Text=tTX.signature.ToString();
            armor.Text = tTX.armor.ToString();
            enemy_pr.Text = tTX.priority_for_enemy.ToString();
            acc_pr.Text = tTX.hit_chance_primary.ToString();
            acc_sec.Text = tTX.hit_chance_secondary.ToString();
        }
    }
}
