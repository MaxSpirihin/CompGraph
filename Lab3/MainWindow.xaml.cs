using _3D;
using Graph;
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
using System.Windows.Threading;
using ShellExt;

namespace Lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Parallelepiped p;
        double k = 100;
        double alpha;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            p = new Parallelepiped()
            {
                Center = new Vector3(0, 0, 0),
                Size = new Vector3(100, 100, 100),
                Turn = new Vector3(0, 0, 0)
            };
            TurnY_ValueChanged(null, null);
            TurnX_ValueChanged(null, null);
            CenterZ_ValueChanged(null, null);
            Draw();


        }

        private void TurnX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
            p.Turn.x = TurnX.Value;
            Draw();
            }
             catch (Exception) { }
        }

        private void TurnY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
            p.Turn.y = TurnY.Value;
            Draw();
                 }
            catch (Exception) { }
        }

        private void TurnZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
            p.Turn.z = TurnZ.Value;
            Draw();
                 }
            catch (Exception) { }
        }

        void Draw()
        {
            Shell.Instance.Clear();
            if (IsPerspective.IsChecked.Value)
                p.DrawPerspective(Shell.Instance, k, DelFringes.IsChecked.Value);
           else
                p.DrawParallel(Shell.Instance, DelFringes.IsChecked.Value);
            


        }

        private void K_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                k = K.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Center.x = CenterX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Center.y = CenterY.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Center.z = CenterZ.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void IsPerspective_Checked(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) return;
            Draw();
        }

        private void SizeX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Size.x = SizeX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void SizeY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Size.y = SizeY.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void SizeZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
                p.Size.z = SizeZ.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void ALPHA_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer.IsEnabled) return;
            alpha = ALPHA.Value;
            p.Turn.x += alpha * Nx.Value;
            p.Turn.y += alpha * Ny.Value;
            p.Turn.z += alpha * Nz.Value;
            Draw();
            p.Turn.x -= alpha * Nx.Value;
            p.Turn.y -= alpha * Ny.Value;
            p.Turn.z -= alpha * Nz.Value;
        }


        private void Turn(object sender, EventArgs e)
        {
            alpha += 1;
            p.Turn.x += alpha * Nx.Value;
            p.Turn.y += alpha * Ny.Value;
            p.Turn.z += alpha * Nz.Value;
            Draw();
            p.Turn.x -= alpha * Nx.Value;
            p.Turn.y -= alpha * Ny.Value;
            p.Turn.z -= alpha * Nz.Value;
        }

        private void BtnAnimate_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Tick += new EventHandler(Turn);
                timer.Interval = new TimeSpan(5000);
                timer.Start();
            }
            else
            {
                timer.Stop();
                TurnX_ValueChanged(null,null);
                TurnY_ValueChanged(null, null);
                TurnZ_ValueChanged(null, null);
                CenterX_ValueChanged(null, null);
                CenterY_ValueChanged(null, null);
                CenterZ_ValueChanged(null, null);
                SizeX_ValueChanged(null, null);
                SizeY_ValueChanged(null, null);
                SizeZ_ValueChanged(null, null);
            }
        }

    }
}
