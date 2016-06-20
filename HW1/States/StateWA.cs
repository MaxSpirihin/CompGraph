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

    enum StateEnumWA
    {
        NonEnter,
        FirstEnter,
        SecondEnter,
        ThirdEnter
    }


    class StateWA : State
    {
        Polygon p1, p2;
        StateEnumWA state;
        Button BTNEnter;

        public StateWA(Button BTNEnter)
        {
            this.BTNEnter = BTNEnter;
            onPressClearBtn();
        }

        public override void onPressMainBtn()
        {
            switch (state)
            {
                case StateEnumWA.NonEnter:
                    state = StateEnumWA.FirstEnter;
                    Shell.Instance.Clear();
                    p1 = new Polygon();
                    p2 = new Polygon();
                    BTNEnter.Content = "Enter First";
                    BTNEnter.IsEnabled = false;
                    break;
                case StateEnumWA.FirstEnter:
                    Shell.Instance.StrokePolygon(p1, Colors.Green);
                    state = StateEnumWA.SecondEnter;
                    BTNEnter.Content = "Enter Second";
                    BTNEnter.IsEnabled = false;
                    break;
                case StateEnumWA.SecondEnter:
                    Shell.Instance.StrokePolygon(p2, Colors.Blue);
                    state = StateEnumWA.ThirdEnter;
                    BTNEnter.Content = "Clip";
                    break;
                case StateEnumWA.ThirdEnter:
                    foreach (Polygon p in Polygon.ClipWA(p1, p2))
                        Shell.Instance.StrokePolygon(p, Colors.Red);
                    state = StateEnumWA.NonEnter;
                    BTNEnter.Content = "Start Entering";
                    break;
            }
        }

        public override void onPressClearBtn()
        {
            p1 = new Polygon();
            p2 = new Polygon();
            state = StateEnumWA.NonEnter;
            Shell.Instance.Clear();
            BTNEnter.Content = "Start Entering";
        }

        public override void onPressPixel(int x, int y)
        {
            Polygon temp;
            switch (state)
            {
                case StateEnumWA.FirstEnter:
                    temp = (Polygon)p1.Clone();
                    temp.AddPoint(x, y);
                    if (temp.IsSimple)
                    {
                        p1.AddPoint(x, y);
                        Shell.Instance.Clear();
                        Shell.Instance.StrokePolygon(p1, Colors.Green, true);
                    }
                    if (p1.Length > 2) BTNEnter.IsEnabled = true;
                    break;
                case StateEnumWA.SecondEnter:
                    temp = (Polygon)p2.Clone();
                    temp.AddPoint(x, y);
                    if (temp.IsSimple)
                    {
                        p2.AddPoint(x, y);
                        Shell.Instance.SetPixel(x, y, Colors.White);
                        Shell.Instance.Clear();
                        Shell.Instance.StrokePolygon(p1, Colors.Green);
                        Shell.Instance.StrokePolygon(p2, Colors.Blue, true);
                    }
                    if (p2.Length > 2) BTNEnter.IsEnabled = true;
                    break;
            }
        }

    }
}
