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
        //список солдат, взятых из бд
        List<Soldier> soldiers_temp_list = new List<Soldier>();
        //тип размещаемого обьекта
        int placed_item_type = 0;
        int count = 0;
        int lines_count = 0;

        //переменные, связанные с фортификацией
        bool is_fort_stand = false;
        Point fort_place_site = new Point();
        int fort_count = 0;
        List<Point> fort_places = new List<Point>();

        //ориентиры
        List<orientier_CC> orientier_CCs = new List<orientier_CC>();
        //ид выбранного обьекта
        int selected_item_id = 0;
        //список, в котором хранятся линии секторов обсрела, распределенные по id солдата
        List<(int, string, string)> soldier_id_to_lines = new List<(int, string, string)>();

        //списки, нужные для симуляции
        List<(Soldier, Point, string)> Enemy_Forces_List = new List<(Soldier, Point, string)>();
        List<Position> Friendly_Forces_Positions_List = new List<Position>();
        //таймер для тиков в симуляции
        DispatcherTimer sim_Move_timer = new DispatcherTimer();
        DispatcherTimer sim_attack_timer = new DispatcherTimer();
        //значения для контроля победы/поражения
        int EFMaxCount;//противников всего
        int FFMaxCount;//союзников всего
        int EF_CCount;//Противников осталось
        int FF_CCount;//союзников осталось
        int EF_Count_FLevel;//Уровень потерь, после которого противнику засчитывается поражение
        int FF_Count_Flevel;//то же самое, но наоборот

        List<TTX> tTXes = new List<TTX>();//список ттх из бд. Нужен для уменьшения количества обращений
        List<Position> Positions = new List<Position>();//то же самое, но для позиций

        int move_sim_ticks = 0;//количество тиков движений в симуляции
        double koef = 0;//коэф скорости передвижения
        bool isVictory = false;
        bool isFailure = false;

        //средства для предварительной оценки правильности составленной карточки
        bool isOrientsPlacedCorrect = true;
        bool[] placedSoldiersTypes = { false, false, false, false };

    }
}
