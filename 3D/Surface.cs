using Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShellExt;

namespace _3D
{
    public static class Surface
    {

        public class Grid
        {
            public List<List<Vector3>> points { get; set; }
            public List<List<Vector3>> second { get; set; }





            public void DrawWithZBuf(Shell shell, double k, bool DrawSecond = false)
            {

                double[] ZBufUp = new double[1000];
                for (int i = 0; i < ZBufUp.Length; i++)
                    ZBufUp[i] = -100000;
                double[] ZBufDown = new double[1000];
                for (int i = 0; i < ZBufDown.Length; i++)
                    ZBufDown[i] = 100000;


                //определяем порядок
                bool IsPlus = (points.First().First().z - k) * (points.First().First().z - k) +
                    (points.First().Last().z - k) * (points.First().Last().z - k) >
                    (points.Last().First().z - k) * (points.Last().First().z - k) +
                    (points.Last().Last().z - k) * (points.Last().Last().z - k);


                for (int j = IsPlus ? 0 : points.Count - 1; IsPlus ? j < points.Count() : j >= 0; j += IsPlus ? 1 : -1)
                {
                    for (int i = 0; i < points[j].Count(); i++)
                    {
                        if (i != points[j].Count - 1)
                            shell.DrawLineBuffer(points[j][i].PerspectiveProection(k),
                                points[j][i + 1].PerspectiveProection(k), ref ZBufUp,ref ZBufDown);

                        if (IsPlus ? j != points.Count - 1 : j != 0)
                            shell.DrawLineBuffer(points[j][i].PerspectiveProection(k),
                                points[IsPlus ? j + 1 : j - 1][i].PerspectiveProection(k), ref ZBufUp, ref ZBufDown,false);
                    }

                }
            }



            public void Draw(Shell shell, double k)
            {
                for (int j = 0; j < points.Count(); j++)
                {
                    for (int i = 0; i < points[j].Count() - 1; i++)
                    {
                        shell.DrawLine(points[j][i].PerspectiveProection(k),
                                points[j][i + 1].PerspectiveProection(k));
                    }
                }


                //второй набор
                if (second != null)
                {
                    for (int j = 0; j < second.Count(); j++)
                    {
                        for (int i = 0; i < second[j].Count() - 1; i++)
                        {
                            shell.DrawLine(second[j][i].PerspectiveProection(k),
                                second[j][i + 1].PerspectiveProection(k));
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < points[0].Count(); j++)
                {
                    for (int i = 0; i < points[0].Count() - 1; i++)
                    {
                        shell.DrawLine(points[i][j].PerspectiveProection(k),
                                points[i+1][j].PerspectiveProection(k));
                    }
                }
                }
            }
        }









        public static Grid GetGrid(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Grid grid = new Grid()
            {
                points = new List<List<Vector3>>(),
                second = new List<List<Vector3>>()
            };


            for (int i = 0; i <= 10; i++)
            {
                Vector3 v1 = new Vector3(p1.x + (p2.x - p1.x) * i / 10, p1.y + (p2.y - p1.y) * i / 10, p1.z + (p2.z - p1.z) * i / 10);
                Vector3 v2 = new Vector3(p3.x + (p4.x - p3.x) * i / 10, p3.y + (p4.y - p3.y) * i / 10, p3.z + (p4.z - p3.z) * i / 10);
                grid.points.Add(new List<Vector3>() { v1, v2 });
            }

            for (int i = 0; i <= 10; i++)
            {
                Vector3 v1 = new Vector3(p1.x + (p3.x - p1.x) * i / 10, p1.y + (p3.y - p1.y) * i / 10, p1.z + (p3.z - p1.z) * i / 10);
                Vector3 v2 = new Vector3(p2.x + (p4.x - p2.x) * i / 10, p2.y + (p4.y - p2.y) * i / 10, p2.z + (p4.z - p2.z) * i / 10);
                grid.second.Add(new List<Vector3>() { v1, v2 });
            }

            return grid;

        }




        public static Grid Pyramid(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Grid grid = new Grid()
            {
                points = new List<List<Vector3>>(),
                second = new List<List<Vector3>>()
            };

            Vector3 Startn = (p1 - p2) * (p2 - p3);
            Startn = Startn / Startn.Length;
            double l = 80;

            for (int i = 0; i <= 10; i++)
            {
                double x = i * 0.1;
                Vector3 n = Startn * (x < 0.5 ? l * x * 2 : l * (1 - x) * 2);
                Vector3 A = p1 + (p2 - p1) * i / 10;
                Vector3 B = p3 + (p4 - p3) * i / 10;
                Vector3 C = (A + B) / 2 + n;
                grid.points.Add(new List<Vector3>() { A, C, B });
            }

            for (int i = 0; i <= 10; i++)
            {
                double x = i * 0.1;
                Vector3 n = Startn * (x < 0.5 ? l * x * 2 : l * (1 - x) * 2);
                Vector3 A = p1 + (p3 - p1) * i / 10;
                Vector3 B = p2 + (p4 - p2) * i / 10;
                Vector3 C = (A + B) / 2 + n;
                grid.second.Add(new List<Vector3>() { A, C, B });


            }
            return grid;
        }





        public static Grid CunsAndBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p12, Vector3 p13,
            Vector3 p24, Vector3 p34,int n = 10)
        {
            Grid grid = new Grid()
            {
                points = new List<List<Vector3>>()
            };


            for (int i = 0; i <= n; i++)
            {
                var points = new List<Vector3>();
                for (int j = 0; j <= n; j++)
                {
                    double u = i * 1.0/n;
                    double v = j * 1.0/n;
                    Vector3 p = Bezier2(p1, p12, p2, u) * (1 - v) + Bezier2(p3, p34, p4, u) * v +
                        Bezier2(p1, p13, p3, v) * (1 - u) + Bezier2(p2, p24, p4, v) * u -
                        p1 * (1 - u) * (1 - v) - p3 * (1 - u) * v - p2 * u * (1 - v) - p4 * u * v;
                    points.Add(p);

                }
                grid.points.Add(points);
            }

            return grid;
        }


        public static Vector3 Bezier2(Vector3 P0,Vector3 P1,Vector3 P2,double t)
        {
            return (1 - t) * (1 - t) * P0 + 2 * t * (1 - t) * P1 + t * t * P2;
        }




    }
}
