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

    enum StateEnumCL
    {
        NonEnter,
        FirstEnter,
        SecondEnter,
        ThirdEnter
    }


    class StateCL : State
    {
        Polygon p1;
        Point a, b;
        StateEnumCL state;
        Button BTNEnter;

        public StateCL(Button BTNEnter)
        {
            this.BTNEnter = BTNEnter;
            onPressClearBtn();
        }

        public override void onPressMainBtn()
        {
            switch (state)
            {
                case StateEnumCL.NonEnter:
                    state = StateEnumCL.FirstEnter;
                    Shell.Instance.Clear();
                    p1 = new Polygon();
                    a= b = null;
                    BTNEnter.Content = "Enter Polygon";
                    BTNEnter.IsEnabled = false;
                    break;
                case StateEnumCL.FirstEnter:
                    Shell.Instance.StrokePolygon(p1, Colors.Green);
                    state = StateEnumCL.SecondEnter;
                    BTNEnter.Content = "Enter Line";
                    BTNEnter.IsEnabled = false;
                    break;
                case StateEnumCL.SecondEnter:
                    break;
                case StateEnumCL.ThirdEnter:
                    Point c,d;
                    if (p1.ClipLine(a, b, out c, out d))
                        Shell.Instance.DrawLine((int)c.x, (int)c.y, (int)d.x, (int)d.y, Colors.Red);
                    state = StateEnumCL.NonEnter;
                    BTNEnter.Content = "Start Entering";
                    break;
            }
        }

        public override void onPressClearBtn()
        {
            p1 = new Polygon();
            a = b = null;
            state = StateEnumCL.NonEnter;
            Shell.Instance.Clear();
            BTNEnter.Content = "Start Entering";
        }

        public override void onPressPixel(int x, int y)
        {
            Polygon temp;
            switch (state)
            {
                case StateEnumCL.FirstEnter:
                    temp = (Polygon)p1.Clone();
                    temp.AddPoint(x, y);
                    if (temp.IsSimple && temp.IsConvex)
                    {
                        p1.AddPoint(x, y);
                        Shell.Instance.Clear();
                        Shell.Instance.StrokePolygon(p1, Colors.Green, true);
                    }
                    if (p1.Length > 2) BTNEnter.IsEnabled = true;
                    break;
                case StateEnumCL.SecondEnter:
                    if (a == null)
                        a = new Point() { x = x, y = y };
                    else
                        if (b == null)
                        {
                            b = new Point() { x = x, y = y };
                            Shell.Instance.DrawLine((int)a.x, (int)a.y, (int)b.x, (int)b.y,Colors.Blue);
                            state = StateEnumCL.ThirdEnter;
                            BTNEnter.Content = "Clip";
                            BTNEnter.IsEnabled = true;
                        }
                    break;
            }
        }

    }
}
