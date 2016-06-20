using Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShellExt
{

    public enum LineCap
    {
        FLAP,
        ROUND,
        SQUARE
    }



    public static class LineDraw
    {
        private readonly static int[] DASHED = { 10, 10 };


        public static void DrawLineBuffer(this Shell shell, Point p1, Point p2, 
            ref double[] ZbufUp, ref double[] ZbufDown,bool ReWrite = true)
        {
            shell.DrawLineBuffer((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y,Shell.DRAW_COLOR,ref ZbufUp,ref ZbufDown,ReWrite);
        }


        public static void DrawLineBuffer(this Shell shell, int x1, int y1, int x2, int y2, Color color, 
            ref double[] ZbufUp, ref double[] ZbufDown,bool ReWrite = true)
        {

            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1); // Если мы во 2,3,6,7 частях меняем x и y местами
            if (steep)
            {
                Utils.Swap(ref x1, ref y1);
                Utils.Swap(ref x2, ref y2);
            }
            // если надо, меняем начало и конец отрезка местами
            if (x1 > x2)
            {
                Utils.Swap(ref x1, ref x2);
                Utils.Swap(ref y1, ref y2);
            }
            int dx = x2 - x1;
            int dy = Math.Abs(y2 - y1);
            int error = dx / 2;
            int ystep = (y1 < y2) ? 1 : -1;
            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                //shell.SetPixel(steep ? y : x, steep ? x : y, color);

                int realY = steep ? x : y;
                int realX = steep ? y : x;

                int num = 400 + realX;
                if (ZbufUp[num] <= realY)
                {
                    if(ReWrite)
                        ZbufUp[num] = realY;
                    shell.SetPixel(realX, realY, color);
                }


                if (ZbufDown[num] >= realY)
                {
                    if (ReWrite)
                        ZbufDown[num] = realY;
                    shell.SetPixel(realX, realY, color);
                }

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }









        public static void DrawLine(this Shell shell, Point p1, Point p2)
        {
            shell.DrawLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, Shell.DRAW_COLOR);
        }


        public static void DrawLine(this Shell shell, Point p1,Point p2, Color color)
        {
            shell.DrawLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, color);
        }


        /// <summary>
        /// нарисовать линию цветом
        /// </summary>
        public static void DrawLine(this Shell shell, int x1, int y1, int x2, int y2, Color color)
        {

            var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1); // Если мы во 2,3,6,7 частях меняем x и y местами
            if (steep)
            {
                Utils.Swap(ref x1, ref y1); 
                Utils.Swap(ref x2, ref y2);
            }
            // если надо, меняем начало и конец отрезка местами
            if (x1 > x2)
            {
                Utils.Swap(ref x1, ref x2);
                Utils.Swap(ref y1, ref y2);
            }
            int dx = x2 - x1;
            int dy = Math.Abs(y2 - y1);
            int error = dx / 2; 
            int ystep = (y1 < y2) ? 1 : -1; 
            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                shell.SetPixel(steep ? y : x, steep ? x : y, color); 
                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        /// <summary>
        /// нарисовать линию
        /// </summary>
        public static void DrawLine(this Shell shell, int x1, int y1, int x2, int y2)
        {
            shell.DrawLine(x1, y1, x2, y2, Shell.DRAW_COLOR);
        }



        /// <summary>
        /// нарисовать линию 
        /// </summary>
        /// <param name="w">толщина</param>
        /// <param name="color">цвет</param>
        /// <param name="type">тип законцовки</param>
        public static void DrawLine(this Shell shell, int x1, int y1, int x2, int y2, int w, Color color,
            LineCap type = LineCap.FLAP)
        {
            int x1old = x1, x2old = x2, y1old = y1, y2old = y2;

            if (w<3)
            {
                shell.DrawLine(x1, y1, x2, y2, color);
                return;
            }

            double alpha = Math.Atan(((double)(x2 - x1)) / ((double)(y2 - y1)));

            if (y1 <= y2)
            {
                Utils.Swap(ref x1, ref x2);
                Utils.Swap(ref y1, ref y2);
            }

            if (type == LineCap.SQUARE)
            {
                
                x1 += (int)(w * Math.Sin(alpha) / 2);
                y1 += (int)(w * Math.Cos(alpha) / 2);
                x2 -= (int)(w * Math.Sin(alpha) / 2);
                y2 -= (int)(w * Math.Cos(alpha) / 2);
            }

            Polygon pol = new Polygon();
            double dx = w * Math.Cos(alpha) / 2;
            double dy = w * Math.Sin(alpha) / 2;

//            pol.AddPoint(x1 - (int)dx, y1 + (int)dy);


            if (type == LineCap.ROUND)
            {
                int n = w / 2;
                if (n < 5) n=5;
                double al = alpha;
                for (int i = 0; i < n; i++)
                {
                    al += Math.PI / n;
                    pol.AddPoint(x1 - (int)(w * Math.Cos(al) / 2), y1 + (int)(w * Math.Sin(al) / 2));
                }

                for (int i = 0; i < n; i++)
                {
                    pol.AddPoint(x2 - (int)(w * Math.Cos(al) / 2), y2 + (int)(w * Math.Sin(al) / 2));
                    al += Math.PI / n;
                }
            }
            else
            {
                pol.AddPoint(x1 - (int)dx, y1 + (int)dy);
                pol.AddPoint(x1 + (int)dx, y1 - (int)dy);
                pol.AddPoint(x2 + (int)dx, y2 - (int)dy);
                pol.AddPoint(x2 - (int)dx, y2 + (int)dy);
            }
            
            shell.FillPolygon(pol, color);
            //shell.DrawLine(x1old, y1old, x2old, y2old, Colors.Red);
        }


        /// <summary>
        /// нарисовать линию 
        /// </summary>
        /// <param name="w">толщина</param>
        /// <param name="type">тип законцовки</param>
        public static void DrawLine(this Shell shell, int x1, int y1, int x2, int y2, int w,
            LineCap type = LineCap.FLAP)
        {
            shell.DrawLine(x1, y1, x2, y2, w,Shell.DRAW_COLOR, type);
        }





        /// <summary>
        /// нарисовать штриховую линию 
        /// </summary>
        public static void DrawLineDashed(this Shell shell, int x1, int y1, int x2, int y2)
        {
            shell.DrawLineDashed(x1, y1, x2, y2, Shell.DRAW_COLOR);
        }


        /// <summary>
        /// нарисовать штриховую линию цветом
        /// </summary>
        public static void DrawLineDashed(this Shell shell, int x1, int y1, int x2, int y2,Color color)
        {
            foreach (Polygon p in CutToDash(x1, y1, x2, y2))
            {
                shell.DrawLine((int)p.points[0].x, (int)p.points[0].y, (int)p.points[1].x, (int)p.points[1].y, color);
            }
        }

        /// <summary>
        /// нарисовать штриховую линию 
        /// </summary>
        /// <param name="w">толщина</param>
        public static void DrawLineDashed(this Shell shell, int x1, int y1, int x2, int y2,int w, Color color)
        {
            foreach (Polygon p in CutToDash(x1, y1, x2, y2))
            {
                shell.DrawLine((int)p.points[0].x, (int)p.points[0].y, (int)p.points[1].x, (int)p.points[1].y, w);
            }
        }
        
        /// <summary>
        /// нарисовать штриховую линию 
        /// </summary>
        /// <param name="w">толщина</param>
        public static void DrawLineDashed(this Shell shell, int x1, int y1, int x2, int y2, int w)
        {
            shell.DrawLineDashed(x1, y1, x2, y2, w, Shell.DRAW_COLOR);
        }


        private static List<Polygon> CutToDash(int x1, int y1, int x2, int y2)
        {
            List<Polygon> result = new List<Polygon>();

            double len = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            double l = 0;

            while(l-4 < len)
            {
                int X1 = x1+ (int) (l * (x2 - x1) / len);
                int Y1 = y1 + (int)(l * (y2 - y1) / len);
                l += DASHED[0];
                int X2 = x1 + (int)((l < len ? l : len) * (x2 - x1) / len);
                int Y2 = y1 + (int)((l < len ? l : len) * (y2 - y1) / len);
                l += DASHED[1];
                result.Add(new Polygon(new List<int>() { X1, X2 }, new List<int>() { Y1, Y2 }));
            }

            return result;
        }

        public static void DrawPolyLine(this Shell shell, List<Point> points, Color color)
        {
            for (int i = 0; i < (points.Count() - 1); i++)
            {
                shell.DrawLine((int)points[i].x, (int)points[i].y,
                    (int)points[i + 1].x, (int)points[i + 1].y, color);
            }
        }


        public static void DrawPolyLine(this Shell shell, List<Point> points)
        {
            shell.DrawPolyLine(points, Shell.DRAW_COLOR);
        }


        public static void DrawCircle(this Shell shell,int X,int Y,int radius,Color color)
        {
            for (int x = X - radius; x <= X + radius; x++)
                for (int y = Y - radius; y <= Y + radius; y++)
                    if ((x - X) * (x - X) + (y - Y) * (y - Y) <= radius * radius)
                        shell.SetPixel(x, y, color);
        }

        public static void DrawCircle(this Shell shell, int X, int Y, int radius)
        {
            shell.DrawCircle(X, Y, radius, Shell.DRAW_COLOR);
        }


        public static void DrawArrow(this Shell shell, Point start, Point end)
        {
            shell.DrawArrow(start, end, Shell.DRAW_COLOR);
        }


        public static void DrawArrow(this Shell shell, Point start, Point end,Color color)
        {
            shell.DrawLine((int)end.x, (int)end.y, (int)start.x, (int)start.y,color);

            int arrowLength = 7; 
            double dx = end.x - start.x;
            double dy = end.y - start.y;

            double theta = Math.Atan2(dy, dx);

            double rad = 0.5; 
            double x = end.x - arrowLength * Math.Cos(theta + rad);
            double y = end.y - arrowLength * Math.Sin(theta + rad);

            double phi2 = -0.5;
            double x2 = end.x - arrowLength * Math.Cos(theta + phi2);
            double y2 = end.y - arrowLength * Math.Sin(theta + phi2);

            List<int> arrowYs = new List<int>();
            arrowYs.Add((int)end.y);
            arrowYs.Add((int)y);
            arrowYs.Add((int)y2);

            List<int> arrowXs = new List<int>();
            arrowXs.Add((int)end.x);
            arrowXs.Add((int)x);
            arrowXs.Add((int)x2);

            shell.FillPolygon(new Polygon(arrowXs, arrowYs),color);
        }

    }
}
