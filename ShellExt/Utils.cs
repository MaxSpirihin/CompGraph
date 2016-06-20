using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExt
{
    /// <summary>
    /// тип нахождения точки относительно вектора
    /// </summary>
    public enum CLPointType
    {
        LEFT,
        RIGHT,
        BEYOUND,
        BEHIND,
        BETWEEN,
        ORIGIN,
        DESTINATION
    };


    /// <summary>
    /// тип пересечения отрезков
    /// </summary>
    public enum IntersectType
    {
        COLLINEAR,
        PARALLEL,
        SKEW,
        SKEW_CROSS,
        SKEW_NO_CROSS
    };


    public static class Utils
    {
        public static void Swap(ref int a, ref int b)
        {
            int c = b;
            b = a;
            a = c;
        }


        /// <summary>
        /// определить положение точки относительно вектора
        /// </summary>
        /// <param name="x1">начало вектора</param>
        /// <param name="y1">начало вектора</param>
        /// <param name="x2">конец вектора</param>
        /// <param name="y2">конец вектора</param>
        /// <param name="x">точка</param>
        /// <param name="y">точка</param>
        public static CLPointType Classify(double x1, double y1, double x2,
            double y2, double x,double y)
        {
            double ax = x2 - x1, ay = y2 - y1;
            double bx = x - x1, by = y - y1;
            double s = ax * by - bx * ay;
            if (s > 0) return CLPointType.LEFT;
            if (s < 0) return CLPointType.RIGHT;
            if (ax * bx < 0 || ay * by < 0) return CLPointType.BEHIND;
            if (ax * ax + ay * ay < bx * bx + by * by) return CLPointType.BEYOUND;
            if (x1 == x && y1 == y) return CLPointType.ORIGIN;
            if (x2 == x && y2 == y) return CLPointType.DESTINATION;
            return CLPointType.BETWEEN;
        }



        private static IntersectType Intersect(double xa, double ya,
            double xb,double yb,double xc,double yc,
            double xd,double yd,ref double t)
        {
            double nx = yd - yc, ny = xc - xd;
            CLPointType type;
            double denom = nx * (xb - xa) + ny * (yb - ya);
            if (denom == 0)
            {
                type = Classify(xc, yc, xd, yd, xa, ya);
                if (type == CLPointType.LEFT || type == CLPointType.RIGHT)
                    return IntersectType.PARALLEL;
                else
                    return IntersectType.COLLINEAR;
            }
            double num = nx * (xa - xc) + ny * (ya - yc);
            t = -num / denom;
            return IntersectType.SKEW;
        }


        /// <summary>
        /// определить тип пересечения 2-ч отрезков
        /// </summary>
        /// <param name="xa">начало отрезка 1</param>
        /// <param name="ya">начало отрезка 1</param>
        /// <param name="xb">конец отрезка 1</param>
        /// <param name="yb">конец отрезка 1</param>
        /// <param name="xc">начало отрезка 2</param>
        /// <param name="yc">начало отрезка 2</param>
        /// <param name="xd">конец отрезка 2</param>
        /// <param name="yd">конец отрезка 2</param>
        /// <param name="tab">относительная координата пер-ия на 1 отрезке</param>
        /// <param name="tcd">относительная координата пер-ия на 2 отрезке</param>
        /// <returns></returns>
        public static IntersectType Cross(double xa, double ya,
            double xb,double yb,double xc,double yc,
            double xd,double yd,
            ref double tab,ref double tcd)
        {
            IntersectType type = Intersect(xa, ya, xb, yb, xc, yc, xd, yd, ref tab);
            if (type == IntersectType.PARALLEL || type == IntersectType.COLLINEAR)
                return type;

            if (tab < 0 || tab > 1)
                return IntersectType.SKEW_NO_CROSS;

            Intersect(xc, yc, xd, yd, xa, ya, xb, yb, ref tcd);

            if (tcd < 0 || tcd > 1)
                return IntersectType.SKEW_NO_CROSS;

            return IntersectType.SKEW_CROSS;
        }


        /// <summary>
        /// возвращает точку пер-ия отрезков (null, если не пер-ся)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Point Intersect(Point a,Point b,Point c,Point d)
        {
            double tab = 0, tcd = 0;
            if (Utils.Cross(a.x, a.y, b.x, b.y, c.x, c.y, d.x, d.y, ref tab, ref tcd) ==
                IntersectType.SKEW_CROSS)
            return new Point()
            {
                x = (b.x - a.x) * tab + a.x,
                y = (b.y - a.y) * tab + a.y,
            };
            return null;
        }


        /// <summary>
        /// находит первый общий элемент в двух итераторах
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="S"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static T FindCommonElement<T>(IEnumerable<T> S, IEnumerable<T> C)
        {
            T p = default(T);
            foreach (T s in S)
            {
                if ( p != null )
                    if (!p.Equals(default(T)))
                        break;
                foreach (T c in C)
                    if (s.Equals(c))
                    {
                        p = s;
                        break;
                    }
            }
            return p;
        }

        
        /// <summary>
        /// опр-т лежит ли точка p на отрезке s1-s2
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool PointInSegment(Point s1,Point s2, Point p)
        {
            double k = (s2.y - s1.y) / (s2.x - s1.x);
            if (Math.Abs(p.y - k * p.x - s1.x) < 1e-5)
                return false;

            if (p.x < Math.Min(s1.x, s2.x) || p.x > Math.Max(s1.x, s2.x))
                return false;

            if (p.y < Math.Min(s1.y, s2.y) || p.y > Math.Max(s1.y, s2.y))
                return false;

            return true;
        }


        /// <summary>
        /// факториал числа
        /// </summary>
        public static double Fact(int n)
        {
            double res = 1;
            for (int i = 1; i <= n; i++)
            {
                res *= i;
            }
            return res;
        }


        /// <summary>
        /// степень
        /// </summary>
        public static double Pow(double x,double y)
        {
            if (Math.Abs(y) < 1e-5)
                return 1;
            return Math.Pow(x, y);
        }



        public static double Interpolate(Polygon Pol, List<double> tops, Point p)
        {
            //ищем левое
            Point pLeft = new Point(-10000, p.y);
            int iL = -1;
            Point A = null;
            for (int i = 0; i <= Pol.Length; i++)
            {
                A = Utils.Intersect(pLeft, p, Pol.points[i], Pol.points[(i + 1) % Pol.Length]);
                if (A != null)
                {
                    iL = i;
                    break;
                }
            }

            //ищем правое
            Point pRight = new Point(10000, p.y);
            int iR = -1;
            Point B = null;
            for (int i = 0; i <= Pol.Length; i++)
            {
                B = Utils.Intersect( p,pRight, Pol.points[i], Pol.points[(i + 1) % Pol.Length]);
                if (B != null)
                {
                    iR = i;
                    break;
                }
            }

            //считаем в левом
            double y1, y2, I1, I2;
            if (Pol.points[iL].y > Pol.points[(iL + 1) % Pol.Length].y)
            {
                y1 = Pol.points[iL].y;
                y2 = Pol.points[(iL + 1) % Pol.Length].y;
                I1 = tops[iL];
                I2 = tops[(iL + 1) % Pol.Length];
            }
            else
            {
                y2 = Pol.points[iL].y;
                y1 = Pol.points[(iL + 1) % Pol.Length].y;
                I2 = tops[iL];
                I1 = tops[(iL + 1) % Pol.Length];
            }


            double Ia = (y1 != y2) ? 
                I1 * (p.y - y2) / (y1 - y2) + I2 * (y1 - p.y) / (y1 - y2) : (I1 + I2) / 2;



            //считаем в левом
            if (Pol.points[iR].y > Pol.points[(iR + 1) % Pol.Length].y)
            {
                y1 = Pol.points[iR].y;
                y2 = Pol.points[(iR + 1) % Pol.Length].y;
                I1 = tops[iR];
                I2 = tops[(iR + 1) % Pol.Length];
            }
            else
            {
                y2 = Pol.points[iR].y;
                y1 = Pol.points[(iR + 1) % Pol.Length].y;
                I2 = tops[iR];
                I1 = tops[(iR + 1) % Pol.Length];
            }

            double Ib = (y1 != y2) ?
                I1 * (p.y - y2) / (y1 - y2) + I2 * (y1 - p.y) / (y1 - y2) : (I1 + I2) / 2;


            return (B.x !=A.x) ?
                Ia * (B.x - p.x) / (B.x - A.x) + Ib * (p.x - A.x) / (B.x - A.x) : (Ia + Ib) / 2;




        }

        

    }
}
