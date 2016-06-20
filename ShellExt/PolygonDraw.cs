using Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShellExt
{
    public static class PolyginDraw
    {
        /// <summary>
        /// нарисовать полигон цветом
        /// </summary>
        public static void StrokePolygon(this Shell shell, Polygon polygon, Color color,bool noLast = false)
        {
            for(int i=0;i< (noLast ? polygon.Length-1 : polygon.Length);i++)
            {
                shell.DrawLine((int)polygon.points[i].x, (int)polygon.points[i].y,
                    (int)polygon.points[(i + 1) % polygon.Length].x, (int)polygon.points[(i + 1) % polygon.Length].y, color);
            }
        }


        /// <summary>
        /// нарисовать полигон
        /// </summary>
        public static void StrokePolygon(this Shell shell, Polygon polygon, bool noLast = false)
        {
            shell.StrokePolygon(polygon, Shell.DRAW_COLOR,noLast);
        }


        /// <summary>
        /// залить полигон цветом
        /// </summary>
        /// <param name="UseNZW">использовать non-zero-winding алгоритм</param>
        public static void FillPolygon(this Shell shell, Polygon polygon,Color color,bool UseNZW = false)
        {
            int xmin = Int32.MaxValue, xmax = Int32.MinValue, ymin = Int32.MaxValue, ymax = Int32.MinValue;

            for (int i=0;i<polygon.Length;i++)
            {
                xmin = polygon.points[i].x < xmin ? (int)polygon.points[i].x : xmin;
                ymin = polygon.points[i].y < ymin ? (int)polygon.points[i].y : ymin;
                xmax = polygon.points[i].x > xmax ? (int)polygon.points[i].x : xmax;
                ymax = polygon.points[i].y > ymax ? (int)polygon.points[i].y : ymax;
            }

            for (int x = xmin; x <= xmax; x++)
                for (int y = ymin; y <= ymax; y++)
                    if (UseNZW ? polygon.PInNZWMode(x,y) : polygon.PInEOMode(x, y))
                        shell.SetPixel(x, y, color);
        }



        public static void ShadingPolygon(this Shell shell, List<Point> points, List<double> Intensity,Color color)
        {
            Polygon polygon = new Polygon(points);

            byte r = color.R, g = color.G, b = color.B;

            int xmin = Int32.MaxValue, xmax = Int32.MinValue, ymin = Int32.MaxValue, ymax = Int32.MinValue;

            for (int i = 0; i < polygon.Length; i++)
            {
                xmin = polygon.points[i].x < xmin ? (int)polygon.points[i].x : xmin;
                ymin = polygon.points[i].y < ymin ? (int)polygon.points[i].y : ymin;
                xmax = polygon.points[i].x > xmax ? (int)polygon.points[i].x : xmax;
                ymax = polygon.points[i].y > ymax ? (int)polygon.points[i].y : ymax;
            }

           /*for (int x = xmin; x <= xmax; x++)
                for (int y = ymin; y <= ymax; y++)
                    if (polygon.PInEOMode(x, y))
                    {

                        double c = Utils.Interpolate(polygon, Intensity,new Point(x,y));
                        if (Double.IsNaN(c)) c = 0;
                        shell.SetPixel(x, y, new Color() { A = 255, R = Convert.ToByte(r * c), G = Convert.ToByte(g * c),
                        B = Convert.ToByte(b * c)
                        });
                    }*/


            for (int y = ymin; y <= ymax; y++)
            {
                //ищем левое
            Point pLeft = new Point(-10000, y);
            Point pRight = new Point(10000, y);
            int iL = -1;
            Point A = null;
            for (int i = 0; i <= polygon.Length; i++)
            {
                A = Utils.Intersect(pLeft, pRight, polygon.points[i % polygon.Length], polygon.points[(i + 1) % polygon.Length]);
                if (A != null)
                {
                    iL = i;
                    break;
                }
            }

            //ищем правое
            
            int iR = -1;
            Point B = null;
            for (int i = 0; i <= polygon.Length; i++)
            {
                B = Utils.Intersect(pLeft, pRight, polygon.points[i % polygon.Length], polygon.points[(i + 1) % polygon.Length]);
                if (B != null && !B.Equals(A))
                {
                    iR = i;
                    break;
                }
            }

            if (A == null || B == null) 
                continue;

                if (A.x>B.x)
                {
                    int tempI = iR;
                    iR = iL;
                    iL = tempI;
                    Point pTemp = B;
                    B = A;
                    A = pTemp;
                }


            //считаем в левом
            double y1, y2, I1, I2;
            if (polygon.points[iL].y > polygon.points[(iL + 1) % polygon.Length].y)
            {
                y1 = polygon.points[iL].y;
                y2 = polygon.points[(iL + 1) % polygon.Length].y;
                I1 = Intensity[iL];
                I2 = Intensity[(iL + 1) % polygon.Length];
            }
            else
            {
                y2 = polygon.points[iL].y;
                y1 = polygon.points[(iL + 1) % polygon.Length].y;
                I2 = Intensity[iL];
                I1 = Intensity[(iL + 1) % polygon.Length];
            }


            double Ia = (y1 != y2) ?
                I1 * (y - y2) / (y1 - y2) + I2 * (y1 - y) / (y1 - y2) : (I1 + I2) / 2;



            //считаем в левом
            if (polygon.points[iR].y > polygon.points[(iR + 1) % polygon.Length].y)
            {
                y1 = polygon.points[iR].y;
                y2 = polygon.points[(iR + 1) % polygon.Length].y;
                I1 = Intensity[iR];
                I2 = Intensity[(iR + 1) % polygon.Length];
            }
            else
            {
                y2 = polygon.points[iR].y;
                y1 = polygon.points[(iR + 1) % polygon.Length].y;
                I2 = Intensity[iR];
                I1 = Intensity[(iR + 1) % polygon.Length];
            }

            double Ib = (y1 != y2) ?
                I1 * (y - y2) / (y1 - y2) + I2 * (y1 - y) / (y1 - y2) : (I1 + I2) / 2;



             for (int x = (int)A.x; x < B.x;x++ )
             {
                 double c = (B.x != A.x) ?
                Ia * (B.x - x) / (B.x - A.x) + Ib * (x - A.x) / (B.x - A.x) : (Ia + Ib) / 2;

                 shell.SetPixel(x,y, new Color()
                 {
                     A = 255,
                     R = Convert.ToByte(r * c),
                     G = Convert.ToByte(g * c),
                     B = Convert.ToByte(b * c)
                 });
             }

                 


            }


            
        }



      

        /// <summary>
        /// залить полигон цветом
        /// </summary>
        /// <param name="UseNZW">использовать non-zero-winding алгоритм</param>
        public static void FillPolygon(this Shell shell, Polygon polygon, bool UseNZW = false)
        {
            shell.FillPolygon(polygon, Shell.DRAW_COLOR, UseNZW);
        }

    }
}
