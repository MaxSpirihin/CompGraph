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
    class StateERM : State
    {
        List<Point> START;
        List<Point> END;
        Point Pcur, Qcur;

        public StateERM(Button BTNEnter)
        {
            START = new List<Point>();
            END = new List<Point>();
            BTNEnter.Content = "Build Curve";
        }


        public override void onPressMainBtn()
        {
            Draw(false);
            List<Point> Q = new List<Point>();
            for (int i = 0; i < START.Count(); i++)
                Q.Add(new Point() { x = END[i].x - START[i].x, y = END[i].y - START[i].y });
            Shell.Instance.DrawPolyLine(Spline.GetErmiteSpline(START, Q));
        }

        public override void onPressClearBtn()
        {
            START = new List<Point>();
            END = new List<Point>();
            Draw(false);
        }


        public override void onPressPixel(int x, int y)
        {
            Pcur = new Point() { x = x, y = y };
        }

        public override void onMove(int x, int y)
        {
            Qcur = new Point() { x = x, y = y };
            Draw();
        }

        public override void onOut(int x, int y)
        {
            if (Qcur != null && Qcur != Pcur)
            {
                START.Add(Pcur);
                END.Add(Qcur);
                Pcur = Qcur = null;
                Draw(false);
            }
        }

        private void Draw(bool DrawCur = true)
        {
            Shell.Instance.Clear();

            for (int i = 0; i < START.Count(); i++)
                Shell.Instance.DrawArrow(START[i], END[i], Colors.Blue);

            if (DrawCur)
                Shell.Instance.DrawArrow(Pcur, Qcur, Colors.Green);
        }


    }
}
