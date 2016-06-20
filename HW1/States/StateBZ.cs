using Graph;
using ShellExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace HW1.States
{
    class StateBZ : State
    {

        List<ShellExt.Point> list = new List<ShellExt.Point>();

        public StateBZ(Button BTNEnter)
        {
            BTNEnter.Visibility = System.Windows.Visibility.Collapsed;
        }


        public override void onPressClearBtn()
        {
            list = new List<Point>();
            Shell.Instance.Clear();
        }

        public override void onPressPixel(int x, int y)
        {
            list.Add(new ShellExt.Point() { x = x, y = y });
            Shell.Instance.Clear();
            foreach (ShellExt.Point p in list)
                Shell.Instance.DrawCircle((int)p.x, (int)p.y, 5, Colors.Red);
            Shell.Instance.DrawPolyLine(Spline.GetBezierCurve(list));
        }

    }
}
