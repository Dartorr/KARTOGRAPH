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
        private void begin_simulation_MI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dbSoldiersCCount = 0;
                using (SoldiersDb db = new SoldiersDb())
                    dbSoldiersCCount = db.Soldier.ToList().Count;

                if ((!sim_Move_timer.IsEnabled) && (dbSoldiersCCount > 0))
                {
                    Before_Sim_Warning_Window warning = new Before_Sim_Warning_Window();
                    warning.ShowDialog();
                    if ((bool)warning.DialogResult)
                    {
                        using (SoldiersDb db = new SoldiersDb())
                        {
                            tTXes = db.TTX.ToList();
                            Positions = db.Position.ToList();
                            for (int i = 0; i < 2; i++)
                            {
                                List<Position> k = (from t in db.Position select t).ToList();
                                int pos_id = (new Random()).Next(0, k.Count);

                                Soldier soldier = new Soldier();

                                soldier.TTX_ID = 0;
                                soldier.ID = soldiers_temp_list.Count;
                                soldier.Side = true;
                                soldier.Position = pos_id;

                                soldiers_temp_list.Add(soldier);

                                db.Soldier.Add(soldier);
                                db.SaveChanges();
                            }
                        }

                        foreach (Point t in fort_places)
                            foreach (Soldier k in soldiers_temp_list)
                            {
                                if (GetDistation(t, k.Position1) <= 100) k.Defense = 4;
                            }

                        Point A = new Point(212 - 100, 70);
                        Point B = new Point(212, 70);
                        Point C = new Point(212 + 100, 70);

                        Create_Squad(A);
                        Create_Squad(B);
                        Create_Squad(C);

                        EFMaxCount = Enemy_Forces_List.Count;
                        FFMaxCount = soldiers_temp_list.Count;

                        EF_CCount = EFMaxCount;
                        FF_CCount = FFMaxCount;

                        EF_Count_FLevel = (int)(EFMaxCount * 0.7);
                        FF_Count_Flevel = (int)(FFMaxCount * 0.7);

                        sim_Move_timer.Tick += Sim_Move_Tick;
                        sim_Move_timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
                        sim_Move_timer.Start();

                        sim_attack_timer.Tick += Sim_Attack_Tick;
                        sim_attack_timer.Interval = new TimeSpan(0, 0, 0, 1, 200);
                        sim_attack_timer.Start();

                        lock_army_buttons();
                        begin_simulation_MI.IsEnabled = false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла непредвиденная ошибка симуляции. Перезапустите программу");
                this.Close();
            }
        }

        public void Create_Squad(Point Center)
        {
            Point A = new Point(Center.X - 20, Center.Y);
            Point B = new Point(Center.X + 20, Center.Y);
            Point C = new Point(Center.X - 20, Center.Y + 20);
            Point D = new Point(Center.X + 20, Center.Y + 20);
            Point E = new Point(Center.X - 20, Center.Y + 40);
            Point F = new Point(Center.X + 20, Center.Y + 40);

            Create_Soldier(0, A);
            Create_Soldier(0, B);
            Create_Soldier(1, C);
            Create_Soldier(2, D);
            Create_Soldier(3, E);
            Create_Soldier(4, F);
        }
        public void Create_Soldier(int ttx_id, Point place)
        {
            Soldier S1 = new Soldier();
            S1.ID = soldiers_temp_list.Count + Enemy_Forces_List.Count;
            S1.TTX_ID = ttx_id;
            S1.Side = false;

            if (ttx_id != 4)
            {
                Ellipse e_s1 = new Ellipse();

                e_s1.Width = 10;
                e_s1.Height = 10;

                e_s1.Fill = Brushes.Blue;

                e_s1.HorizontalAlignment = HorizontalAlignment.Left;
                e_s1.VerticalAlignment = VerticalAlignment.Top;

                e_s1.Margin = new Thickness(place.X - 5, place.Y - 5, 0, 0);

                e_s1.Name = "enemy_" + S1.ID;
                Main_Canvas.Children.Add(e_s1);
                e_s1.RegisterName(e_s1.Name, e_s1);

                Enemy_Forces_List.Add((S1, place, e_s1.Name));
            }

            else
            {
                Rectangle e_s1 = new Rectangle();

                e_s1.Width = 15;
                e_s1.Height = 40;

                e_s1.Fill = Brushes.Blue;

                e_s1.HorizontalAlignment = HorizontalAlignment.Left;
                e_s1.VerticalAlignment = VerticalAlignment.Top;

                e_s1.Margin = new Thickness(place.X - 7.5, place.Y - 0, 0, 0);

                e_s1.Name = "enemy_" + S1.ID;
                Main_Canvas.Children.Add(e_s1);
                e_s1.RegisterName(e_s1.Name, e_s1);

                Enemy_Forces_List.Add((S1, place, e_s1.Name));
            }

        }
        private void Sim_Move_Tick(object sender, EventArgs e)
        {

            for (int i = 0; i < Enemy_Forces_List.Count; i++)
            {
                move_sim_ticks++;

                TTX tTX = new TTX();
                int idttx = (int)Enemy_Forces_List[i].Item1.TTX_ID;

                tTX = (from t
                       in tTXes
                       where t.ID_TTX == idttx
                       select t)
                       .ToList()[0];

                if (move_sim_ticks < 800)
                    koef += 0.00005;
                if (move_sim_ticks > 6000)
                    koef -= 0.0001;
                if (koef < 0) koef = 0;

                Point t_p = Enemy_Forces_List[i].Item2;
                t_p.Y += (double)tTX.speed * koef;
                (Soldier, Point, string) tmp = (Enemy_Forces_List[i].Item1,
                    t_p,
                    Enemy_Forces_List[i].Item3);
                Enemy_Forces_List[i] = tmp;

                Random xRand = new Random((int)DateTime.Now.Ticks);
                double Dx = (double)(tTX.speed * koef * (xRand.NextDouble() - 0.5) * 2);

                if (tTX.ID_TTX == 4)
                {
                    Rectangle oleg = (Rectangle)Main_Canvas.FindName(tmp.Item3);
                    oleg.Margin = new Thickness(oleg.Margin.Left + Dx,
                        oleg.Margin.Top
                        + (double)tTX.speed * koef,
                        0, 0);
                }
                else
                {
                    Ellipse oleg = (Ellipse)Main_Canvas.FindName(tmp.Item3);
                    oleg.Margin = new Thickness(oleg.Margin.Left + Dx,
                        oleg.Margin.Top
                        + (double)tTX.speed * koef,
                        0, 0);
                }
            }


        }
        private void Sim_Attack_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < soldiers_temp_list.Count; i++)
                {
                    Soldier F_sol = soldiers_temp_list[i];
                    TTX tTX = new TTX();
                    Position t_pos = new Position();

                    tTX = (from tt
                           in tTXes
                           where tt.ID_TTX == F_sol.TTX_ID
                           select tt)
                           .ToList()[0]; //находим ттх нашего солдата
                    t_pos = (from tt
                             in Positions
                             where tt.Position_ID == F_sol.Position
                             select tt)
                             .ToList()[0]; //находим позицию солдата

                    List<Soldier> inAtkRadius = new List<Soldier>();
                    for (int j = 0; j < Enemy_Forces_List.Count; j++)
                    {
                        (Soldier, Point, string) t = Enemy_Forces_List[j];

                        double dist = GetDistation(t.Item2, t_pos);
                        //фильтруем по радиусу атаки основного оружия
                        if (dist < tTX.atk_primary_rad)
                            inAtkRadius.Add(t.Item1);
                    }
                    if (inAtkRadius.Count > 0)
                    {
                        List<Soldier> k = inAtkRadius;
                        if (F_sol.TTX_ID < 3)
                        {
                            k = (from tt in inAtkRadius
                                 where tt.TTX_ID != 4
                                 orderby (from kkk in tTXes
                                          where tt.TTX_ID == kkk.ID_TTX
                                          select kkk).ToList()[0].priority_for_enemy descending
                                 select tt).ToList();
                        }
                        else
                        {
                            k = (from tt in inAtkRadius
                                 orderby (from kkk
                                        in tTXes
                                          where tt.TTX_ID == kkk.ID_TTX
                                          select kkk)
                                        .ToList()[0].priority_for_enemy descending
                                 select tt).ToList();
                        }
                        if (k.Count > 0)
                            Attack(F_sol, k[0]);
                    }

                }
                for (int i = 0; i < Enemy_Forces_List.Count; i++)
                {
                    List<Soldier> inRad = new List<Soldier>();
                    (Soldier, Point, string) t = Enemy_Forces_List[i];
                    TTX tTX = new TTX();

                    tTX = (from tk
                           in tTXes
                           where tk.ID_TTX == t.Item1.TTX_ID
                           select tk)
                           .ToList()[0];

                    for (int j = 0; j < soldiers_temp_list.Count; j++)
                    {
                        Soldier t_s = soldiers_temp_list[j];
                        TTX t_s_ttx = new TTX();
                        Position t_pos = new Position();


                        t_s_ttx = (from tt
                               in tTXes
                                   where tt.ID_TTX == t_s.TTX_ID
                                   select tt)
                               .ToList()[0]; //находим ттх нашего солдата
                        t_pos = (from tt
                                 in Positions
                                 where tt.Position_ID == t_s.Position
                                 select tt)
                                 .ToList()[0]; //находим позицию солдата

                        if (tTX.atk_primary_rad > GetDistation(t.Item2, t_pos))
                        {
                            if (t.Item1.TTX_ID < 3)
                            {
                                if (t_s_ttx.armor == 0)
                                    inRad.Add(t_s);
                            }
                            else inRad.Add(t_s);
                        }

                    }
                    if (inRad.Count > 0)
                    {
                        List<Soldier> ordered_inRad = new List<Soldier>();

                        ordered_inRad = (from k
                                         in inRad
                                         orderby (from kk
                                                  in tTXes
                                                  where kk.ID_TTX == k.TTX_ID
                                                  select kk)
                                                  .ToList()[0]
                                                  .priority_for_enemy descending
                                         select k)
                                         .ToList();

                        if (ordered_inRad.Count > 0)
                            Attack(t.Item1, ordered_inRad[0]);

                    }

                }

                if (isVictory) Victory();
                else if (isFailure) Failure();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Возникла непредвиденная ошибка. Перезапустите программу");
            }
        }
        public void Attack(Soldier soldierA, Soldier soldierB)
        {
            try
            { 
            TTX ATTX = new TTX();
            TTX BTTX = new TTX();

            bool isTargetDead = false;

            ATTX = (from t
                    in tTXes
                    where t.ID_TTX == soldierA.TTX_ID
                    select t)
                    .ToList()[0];
            BTTX = (from t
                    in tTXes
                    where t.ID_TTX == soldierB.TTX_ID
                    select t)
                    .ToList()[0];


            if (soldierB.Defense == null) soldierB.Defense = 0;

            bool flag = false;


            if ((BTTX.armor == 0) || ((bool)ATTX.is_primary_percing_armor))
            {
                for (int i = 0; i < ATTX.burst_length; i++)
                {
                    Random hit_random = new Random((int)DateTime.Now.Ticks);
                    double hitChance = (double)((ATTX.hit_chance_primary / (soldierB.Defense + 1)) * BTTX.signature);

                    if (hit_random.NextDouble() < hitChance)
                    {
                        isTargetDead = true;
                        flag = true;
                    }
                }
            }
            if ((!flag) && (ATTX.atk_secondary_rad != 0))
            {
                Random hit_random = new Random((int)DateTime.Now.Ticks);
                double hitChance = (double)((ATTX.hit_chance_secondary / (soldierB.Defense + 1)) * BTTX.signature);

                if (hit_random.NextDouble() < hitChance)
                {
                    isTargetDead = true;
                    flag = true;
                }
            }

            if (isTargetDead)
                if ((bool)soldierB.Side)
                {

                    soldiers_temp_list.Remove(soldierB);

                    FF_CCount--;
                    F_PB.Value--;
                    if (FF_CCount < FF_Count_Flevel) isFailure = true;
                    flag = true;

                }
                else
                {

                    string enemy_name = "";
                    for (int j = 0; j < Enemy_Forces_List.Count; j++)
                        if (Enemy_Forces_List[j].Item1.ID == soldierB.ID)
                        {
                            enemy_name = Enemy_Forces_List[j].Item3;
                            Enemy_Forces_List.RemoveAt(j);
                        }

                    if (soldierB.TTX_ID == 4)
                    {
                        Rectangle temp = (Rectangle)Main_Canvas.FindName(enemy_name);
                        if (temp != null) temp.Fill = Brushes.Gray;
                    }
                    else
                    {
                        Ellipse temp = (Ellipse)Main_Canvas.FindName(enemy_name);
                        if (temp != null) temp.Fill = Brushes.Gray;
                    }

                    EF_CCount--;
                    E_PB.Value--;
                    if (EF_CCount < FF_Count_Flevel)
                    {
                        isVictory = true;
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла непредвиденная ошибка. Перезапустите программу");
            }
        }
        public void Victory()
        {
            sim_attack_timer.Stop();
            sim_Move_timer.Stop();
            Victory_W victory_W = new Victory_W(isOrientsPlacedCorrect,
                placedSoldiersTypes, FF_CCount,
                FFMaxCount, EF_CCount,
                EFMaxCount);
            victory_W.ShowDialog();
            this.Close();
        }
        public void Failure()
        {
            sim_attack_timer.Stop();
            sim_Move_timer.Stop();
            Failure_W failure = new Failure_W(isOrientsPlacedCorrect,
                placedSoldiersTypes, FF_CCount,
                FFMaxCount, EF_CCount,
                EFMaxCount);
            failure.ShowDialog();
            this.Close();
        }

    }
}
