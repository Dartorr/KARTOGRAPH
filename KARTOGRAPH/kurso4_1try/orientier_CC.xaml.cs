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

namespace kurso4_1try
{
    /// <summary>
    /// Логика взаимодействия для orietier_CC.xaml
    /// </summary>
    public partial class orientier_CC : UserControl
    {
        public int orient_id = -1;
        public Point point_orientire = new Point(0, 0);

        public orientier_CC()
        {
            InitializeComponent();
        }

        public orientier_CC(int n, Point tPoint)
        {
            this.Margin = new Thickness(tPoint.X-5, tPoint.Y-5,0,0);

            orient_id = n;
            point_orientire = tPoint;

            InitializeComponent();
            or_name.Text = "Ориентир № " + n;
        }
    }
}
