using Graph;
using ShellExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D
{

    public struct Fringe
    {
        //определяют все ребра, примыкающие к этой грани, обозначая послед. точки
        public int[] points;

        //определяет 2 точки, соединив которые получим нормаль к грани
        public int nStart;
        public int nEnd;


        public bool IsVisibleParallel(Parallelepiped owner)
        {
            Vector3 V = new Vector3(0, 0, 1);
            Vector3 n = (owner.Points[nStart] - owner.Points[nEnd]);//нормаль к грани
            return (n.x * V.x + n.y * V.y + n.z * V.z) > 0;
        }


        public bool IsVisiblePerspective(Parallelepiped owner, Vector3 S)
        {
            Vector3 C = new Vector3();//точка в центре грани
            for (int i = 0; i < 4; i++)
                C = C + owner.Points[points[i]];

            C.x /= 4;
            C.y /= 4;
            C.z /= 4;


            Vector3 V = C - S;
            Vector3 n = (owner.Points[nStart] - owner.Points[nEnd]);//нормаль
            return (n.x * V.x + n.y * V.y + n.z * V.z) > 0;
        }
    }




    public class Parallelepiped
    {
        public Vector3 Center { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 Turn { get; set; }

        public Vector3[] Points
        {
            get
            {
                return GetPoints();
            }
        }

        public bool[,] Relations
        {
            get
            {
                return GetRelations();
            }
        }

        public Fringe[] Fringes
        {
            get
            {
                return GetFringes();
            }
        }


        public Parallelepiped() { }

        public Parallelepiped(Vector3 Center, Vector3 Size, Vector3 Turn)
        {
            this.Center = Center;
            this.Size = Size;
            this.Turn = Turn;
        }


        public void DrawParallel(Shell shell, bool DelFringes = false)
        {
            if (DelFringes)
                Draw(shell, x => x.ParallelProection(), x => x.IsVisibleParallel(this));
            else
                Draw(shell, x => x.ParallelProection(), null);
        }


        public void DrawPerspective(Shell shell, double k, bool DelFringes = false)
        {
            if (DelFringes)
                Draw(shell, x => x.PerspectiveProection(k), x => x.IsVisiblePerspective(this, new Vector3(0, 0, -k)));
            else
                Draw(shell, x => x.PerspectiveProection(k), null);
        }



        private void Draw(Shell shell, Func<Vector3, Point> Proection, Func<Fringe, bool> IsVisible)
        {
            if (IsVisible != null)
            {
                DrawWithout(shell, Proection, IsVisible);
                return;
            }

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (Relations[i, j])
                    {
                        Point p1 = Proection(Points[i]);
                        Point p2 = Proection(Points[j]);
                        shell.DrawLine(p1, p2);
                    }
        }


        private void DrawWithout(Shell shell, Func<Vector3, Point> Proection, Func<Fringe, bool> IsVisible)
        {
            foreach (Fringe fringe in Fringes)
            {
                if (IsVisible(fringe))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Point p1 = Proection(Points[fringe.points[i]]);
                        Point p2 = Proection(Points[fringe.points[(i + 1) % 4]]);
                        shell.DrawLine(p1, p2);
                    }
                }
            }
        }

        public void DrawPerspectiveWithoutFringes(Shell shell, double k)
        {
            foreach (Fringe fringe in Fringes)
            {
                if (fringe.IsVisiblePerspective(this, new Vector3(0, 0, -k)))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Point p1 = Points[fringe.points[i]].PerspectiveProection(k);
                        Point p2 = Points[fringe.points[(i + 1) % 4]].PerspectiveProection(k);
                        shell.DrawLine(p1, p2);
                    }
                }
            }
        }




        public void Shading(Shell shell, double k, Vector3 Light)
        {
            int j = 1;
            foreach (Fringe fringe in Fringes)
            {

                if (fringe.IsVisiblePerspective(this, new Vector3(0, 0, -k)))
                {

                    var p = new List<Point>();
                    var I = new List<double>();
                    for (int i = 0; i < 4; i++)
                    {
                        p.Add(Points[fringe.points[i]].PerspectiveProection(k));
                        double intense = (GetIntensity(Points[fringe.points[i]], Light, new Vector3(0, 0, -k),
                            (Points[fringe.nEnd] - Points[fringe.nStart])));
                        I.Add(intense > 1 ? 1 : intense);
                    }

                    shell.ShadingPolygon(p, I, new System.Windows.Media.Color() { A = 255, R = 255, G = 0, B = 0 });

                }

                j++;
            }
        }


        public double GetIntensity(Vector3 point, Vector3 Light, Vector3 viewer, Vector3 n)
        {
            double I = 0.1;//рассеянная компонента

            double d = (Light - point).Length/5000;
            double Il = 1, Kd = 0.5, Ks = 0.5, m = 50;


            Vector3 L = Light - point;
            Vector3 S = viewer - point;
            n = n / n.Length;
            L = L / L.Length;

            double cosTET = (n.x * L.x + n.y * L.y + n.z * L.z);
            if (cosTET < 0)
                return I;

            I += Il * (Kd * cosTET) / (d + 1); //диффузная компонента


            Vector3 R = 2 * n - L * n.Length / cosTET;
            R = R / R.Length;
            double cosAL = (R.x * L.x + R.y * L.y + R.z * L.z);

            if (cosAL < 0)
                return I;

            I += Il * (Ks * Math.Pow(cosAL,m)) / (d + 1);//зеркальная компонента

            return I;
        }


        private Vector3[] GetPoints()
        {
            Vector3[] result = new Vector3[8];
            //база
            result[0] = new Vector3(Size.x / 2, Size.y / 2, -Size.z / 2);
            result[1] = new Vector3(-Size.x / 2, Size.y / 2, -Size.z / 2);
            result[2] = new Vector3(Size.x / 2, -Size.y / 2, -Size.z / 2);
            result[3] = new Vector3(-Size.x / 2, -Size.y / 2, -Size.z / 2);
            result[4] = new Vector3(Size.x / 2, Size.y / 2, Size.z / 2);
            result[5] = new Vector3(-Size.x / 2, Size.y / 2, Size.z / 2);
            result[6] = new Vector3(Size.x / 2, -Size.y / 2, Size.z / 2);
            result[7] = new Vector3(-Size.x / 2, -Size.y / 2, Size.z / 2);

            //поворот по z
            double alpha = Turn.z * Math.PI / 180;
            foreach (Vector3 v in result)
            {
                Vector3 newV = new Vector3()
                {
                    x = v.x * Math.Cos(alpha) - v.y * Math.Sin(alpha),
                    y = v.x * Math.Sin(alpha) + v.y * Math.Cos(alpha)
                };
                v.x = newV.x;
                v.y = newV.y;
            }

            //поворот по x
            alpha = Turn.x * Math.PI / 180;
            foreach (Vector3 v in result)
            {
                Vector3 newV = new Vector3()
                {
                    y = v.y * Math.Cos(alpha) - v.z * Math.Sin(alpha),
                    z = v.y * Math.Sin(alpha) + v.z * Math.Cos(alpha)
                };
                v.z = newV.z;
                v.y = newV.y;
            }


            //поворот по y
            alpha = Turn.y * Math.PI / 180;
            foreach (Vector3 v in result)
            {
                Vector3 newV = new Vector3()
                {
                    x = v.x * Math.Cos(alpha) - v.z * Math.Sin(alpha),
                    z = v.x * Math.Sin(alpha) + v.z * Math.Cos(alpha)
                };
                v.x = newV.x;
                v.z = newV.z;
            }


            //смещение
            foreach (Vector3 v in result)
            {
                v.x += Center.x;
                v.y += Center.y;
                v.z += Center.z;
            }


            return result;
        }



        private bool[,] GetRelations()
        {
            int[,] resInt = new int[8, 8]
            {
                {0,1,1,0,1,0,0,0},
                {1,0,0,1,0,1,0,0},
                {1,0,0,1,0,0,1,0},
                {0,1,1,0,0,0,0,1},
                {1,0,0,0,0,1,1,0},
                {0,1,0,0,1,0,0,1},
                {0,0,1,0,1,0,0,1},
                {0,0,0,1,0,1,1,0}
            };

            bool[,] res = new bool[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    res[i, j] = Convert.ToBoolean(resInt[i, j]);

            return res;
        }


        private Fringe[] GetFringes()
        {
            return new Fringe[6]
            {
                new Fringe()
                {
                    points=new int[4]{0,2,6,4},nStart = 3,nEnd = 2
                },
                 new Fringe()
                {
                     points=new int[4]{1,3,7,5},nStart = 2,nEnd = 3
                },
                 new Fringe()
                {
                     points=new int[4]{2,3,7,6},nStart = 0,nEnd = 2
                },
                 new Fringe()
                {
                     points=new int[4]{0,4,5,1},nStart = 2,nEnd = 0
                },
                 new Fringe()
                {
                     points=new int[4]{0,1,3,2},nStart = 4,nEnd = 0
                },
                 new Fringe()
                {
                    points=new int[4]{6,7,5,4},nStart = 0,nEnd = 4
                }
            };
        }




    }
}
