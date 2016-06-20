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
        double k = 200;
        double alpha,t;
        bool back;
        DispatcherTimer timer = new DispatcherTimer();
        Vector3 Light = new Vector3(-20, 20, -200);
        Vector3 p1 = new Vector3(-100, 50, 1),
            p2 = new Vector3(100, -50, -30),
            p3 = new Vector3(0, 0, 20),
            p4 = new Vector3(-10, 30, 180);

        public MainWindow()
        {
            InitializeComponent();
            p = new Parallelepiped()
            {
                Center = new Vector3(1, 1, 1),
                Size = new Vector3(80, 80, 80),
                Turn = new Vector3(0, 0, 0)
            };

          /*  List<ShellExt.Point> list = new List<ShellExt.Point>();
            list.Add(new ShellExt.Point(0, 0));
            list.Add(new ShellExt.Point(100, 0));
            list.Add(new ShellExt.Point(100, 100));
            list.Add(new ShellExt.Point(0, 100));

            Shell.Instance.ShadingPolygon(list, new List<double>() { 1, 0, 1, 0 }, Colors.White);*/

            //TurnY_ValueChanged(null, null);
            //TurnX_ValueChanged(null, null);
            //CenterZ_ValueChanged(null, null);
            //Draw();
            BtnAnimate_Click(null, null);
            //Close();
        }



        void Draw()
        {
            Shell.Instance.Clear();

            p.Shading(Shell.Instance, k, Light);

            Shell.Instance.DrawCircle((int)Light.PerspectiveProection(k).x, (int)Light.PerspectiveProection(k).y, 5, Colors.White);
        }

        private void TurnX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
          //  p.Turn.x = TurnX.Value;
            Draw();
            }
             catch (Exception) { }
        }

        private void TurnY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
          //  p.Turn.y = TurnY.Value;
            Draw();
                 }
            catch (Exception) { }
        }

        private void TurnZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             try
            {
            if (timer.IsEnabled) return;
          //  p.Turn.z = TurnZ.Value;
            Draw();
                 }
            catch (Exception) { }
        }



        private void CenterX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
              //  p.Center.x = CenterX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
             //   p.Center.y = CenterY.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void CenterZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
            //    p.Center.z = CenterZ.Value;
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
            //    p.Size.x = SizeX.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void SizeY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
            //    p.Size.y = SizeY.Value;
                Draw();
            }
            catch (Exception) { }
        }

        private void SizeZ_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (timer.IsEnabled) return;
            //    p.Size.z = SizeZ.Value;
                Draw();
            }
            catch (Exception) { }
        }

       

        private void Turn(object sender, EventArgs e)
        {
            alpha = 100;
            t += 0.005 * (back ? -1 : 1);
            if (t > 1)
                back = true;
            if (t < 0)
                back = false;


            p.Center = new Vector3()
            {
                x = Spline.BezierFunc(new List<double>() { p1.x, p3.x, p4.x, p2.x }, t),
                y = Spline.BezierFunc(new List<double>() { p1.y, p3.y, p4.y, p2.y }, t),
                z = Spline.BezierFunc(new List<double>() { p1.z, p3.z, p4.z, p2.z }, t)
            };
            


            p.Turn.x += alpha * Math.Abs(p.Center.x / p.Center.Length);
            p.Turn.y += alpha * Math.Abs(p.Center.y / p.Center.Length);
            p.Turn.z += alpha * Math.Abs(p.Center.z / p.Center.Length);
            Draw();
            p.Turn.x -= alpha * Math.Abs(p.Center.x / p.Center.Length);
            p.Turn.y -= alpha * Math.Abs(p.Center.y / p.Center.Length);
            p.Turn.z -= alpha * Math.Abs(p.Center.z / p.Center.Length);
        }

        private void BtnAnimate_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Tick += new EventHandler(Turn);
                timer.Interval = new TimeSpan(1000);
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
