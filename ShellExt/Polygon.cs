using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExt
{


    /// <summary>
    /// тип ребра полигона по отношению к точке
    /// </summary>
    enum EType
    {
        TOUCHING,
        CROSS_LEFT,
        CROSS_RIGHT,
        INESSENTIAL
    }


    public class Polygon : ICloneable
    {

        #region Клиентский интерфейс

        public List<Point> points { get; private set; }
        /// <summary>
        /// кол-во точек полигона
        /// </summary>
        public int Length
        {
            get
            {
                return points.Count();
            }
        }
        /// <summary>
        /// является лм полигон простым
        /// </summary>
        public bool IsSimple
        {
            get
            {
                return _IsSimple();
            }
        }
        /// <summary>
        /// является ли полигон выпуклым
        /// </summary>
        public bool IsConvex
        {
            get
            {
                return _IsConvex();
            }
        }
        /// <summary>
        /// ориентирован ли полигон по часовой стрелке
        /// </summary>
        public bool IsClockWise
        {
            get
            {
                return _IsClockWise();
            }
        }
        public Polygon()
        {
            points = new List<Point>();
        }


        public Polygon(List<int> X, List<int> Y)
        {
            points = new List<Point>();
            for (int i = 0; i < Math.Min(X.Count(), Y.Count()); i++)
            {
                points.Add(new Point() { x = X[i], y = Y[i] });
            }
        }

        public Polygon(List<Point> points)
        {
            this.points = points;
        }

        public void AddPoint(int x, int y)
        {
            points.Add(new Point() { x = x, y = y });
        }


        public void AddPoint(double x, double y)
        {
            points.Add(new Point() { x = x, y = y });
        }

        public void AddPoint(Point p)
        {
            points.Add(p);
        }



        /// <summary>
        /// находится ли точка внутри полигона (even-odd mode)
        /// </summary>
        public bool PInEOMode(double x, double y)
        {
            bool isIn = false;
            for (int i = 0; i < Length; i++)
            {
                switch (EdgeType(points[i].x, points[i].y, points[(i + 1) % Length].x, points[(i + 1) % Length].y, x, y))
                {
                    case EType.TOUCHING:
                        return true;
                    case EType.CROSS_LEFT:
                    case EType.CROSS_RIGHT:
                        isIn = !isIn;
                        break;
                }
            }
            return isIn;
        }

        /// <summary>
        /// находится ли точка внутри полигона (non-zero-winding mode)
        /// </summary>
        public bool PInNZWMode(double x, double y)
        {
            int param = 0;
            for (int i = 0; i < Length; i++)
            {
                switch (EdgeType(points[i].x, points[i].y, points[(i + 1) % Length].x, points[(i + 1) % Length].y, x, y))
                {
                    case EType.TOUCHING:
                        return true;
                    case EType.CROSS_LEFT:
                        param++;
                        break;
                    case EType.CROSS_RIGHT:
                        param--;
                        break;
                }
            }
            return param != 0;
        }




        /// <summary>
        /// переориентировать направление обхода полигона
        /// </summary>
        public void ReOrient()
        {
            points.Reverse();
        }



        public bool IsInside(Polygon pol)
        {
            foreach (Point p in pol.points)
            {
                if (!PInEOMode(p.x, p.y))
                    return false;
            }

            return true;
        }

        public static List<Polygon> ClipWA(Polygon p1, Polygon p2)
        {
            //учитваем движение по часовой стрелке
            if (!p1.IsClockWise) p1.ReOrient();
            if (!p2.IsClockWise) p2.ReOrient();

            if (p1.IsInside(p2))
                return new List<Polygon>() { p2 };
            if (p2.IsInside(p1))
                return new List<Polygon>() { p1 };



            //собираем 2 массива точек
            List<Point> S = GetWAPoints(p1, p2);
            List<Point> C = GetWAPoints(p2, p1);



            List<Polygon> result = new List<Polygon>();


            while (true)
            {
                //ищем общую точку
                Point p = Utils.FindCommonElement<Point>(S, C);
                if (p == null)
                    break;

                bool isFirst = StartWAWithFirst(p1.points, p2.points, p);
                Polygon polygon = new Polygon();
                List<Point> completed = new List<Point>();
                completed.Add(p);

                do
                {
                    polygon.AddPoint(p);

                    if (isFirst)
                        p = S[(S.IndexOf(p) + 1) % S.Count()];
                    else
                        p = C[(C.IndexOf(p) + 1) % C.Count()];

                    if (S.Any(point => point.Equals(p)) &&
                        C.Any(point => point.Equals(p)))
                    {
                        isFirst = !isFirst;
                        completed.Add(p);
                    }

                }
                while (!p.Equals(polygon.points[0]));

                result.Add(polygon);
                foreach (Point po in completed)
                {
                    S.Remove(S.FirstOrDefault(poi => poi.Equals(po)));
                    C.Remove(C.FirstOrDefault(poi => poi.Equals(po)));
                }
            }
            return result;
        }


        public object Clone()
        {
            Polygon result = new Polygon();
            foreach (Point p in points)
                result.AddPoint(p.x, p.y);
            return result;
        }



        /// <summary>
        /// обрезает отрезок полигоном
        /// </summary>
        public bool ClipLine(Point a, Point b, out Point newA, out Point newB)
        {
            double t0 = 0, t1 = 1;
            newA = newB = null;
            Point s = new Point()
            {
                x = b.x - a.x,
                y = b.y - a.y
            };


            for (int i = 0; i < Length; i++)
            {
                Point n = new Point()
                {
                    x = points[(i + 1) % Length].y - points[i].y,
                    y = points[i].x - points[(i + 1) % Length].x
                };

                double dnom = n.x * s.x + n.y * s.y,
                    num = n.x * (a.x - points[i].x) + n.y * (a.y - points[i].y);

                if (dnom != 0)
                {
                    double t = -num / dnom;
                    if (dnom < 0)
                    {
                        if (t > t0)
                            t0 = t;
                    }
                    else
                        if (t < t1)
                            t1 = t;
                }
                else
                {
                    if (num < 0)
                        return false;
                }
            }//endfor

            if (t0<=t1)
            {
                newA = new Point()
                {
                    x = a.x + t0 * (b.x - a.x),
                    y = a.y + t0 * (b.y - a.y)
                };

                newB = new Point()
                {
                    x = a.x + t1 * (b.x - a.x),
                    y = a.y + t1 * (b.y - a.y)
                };
                return true;
            }
                return false;
        }



        #endregion

        #region Приватные методы
        private bool _IsSimple()
        {
            double tab = 0, tcd = 0;
            for (int i = 0; i < Length - 1; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (j == i || j % Length == (i + 1) % Length || i % Length == (j + 1) % Length)
                        continue;

                    if (Utils.Cross(points[i].x, points[i].y, points[(i + 1) % Length].x, points[(i + 1) % Length].y,
                        points[j].x, points[j].y, points[(j + 1) % Length].x, points[(j + 1) % Length].y, ref tab, ref tcd) == IntersectType.SKEW_CROSS)
                        return false;
                }
            }
            return true;
        }


        private bool _IsConvex()
        {
            if (Length < 3)
                return true;

            CLPointType type = Utils.Classify(points[0].x, points[0].y, points[1].x, points[1].y, points[2].x, points[2].y);

            for (int i = 1; i < Length; i++)
                if (Utils.Classify(points[i].x, points[i].y, points[(i + 1) % Length].x, points[(i + 1) % Length].y,
                    points[(i + 2) % Length].x, points[(i + 2) % Length].y) != type)
                    return false;

            return true;
        }



        private bool _IsClockWise()
        {
            return (SignedPolygonArea() > 0);
        }


        /// <summary>
        /// вычисляет площадь полигона
        /// </summary>
        private double SignedPolygonArea()
        {
            int num_points = Length;

            double area = 0;
            for (int i = 0; i < Length; i++)
            {
                area +=
                    (points[(i + 1) % Length].x - points[i].x) *
                    (points[(i + 1) % Length].y + points[i].y) / 2;
            }

            return area;
        }


        private static EType EdgeType(double x0, double y0, double xd, double yd, double xa, double ya)
        {
            switch (Utils.Classify(x0, y0, xd, yd, xa, ya))
            {
                case CLPointType.LEFT:
                    if (ya > y0 && ya <= yd)
                        return EType.CROSS_LEFT;
                    else
                        return EType.INESSENTIAL;
                case CLPointType.RIGHT:
                    if (ya > yd && ya <= y0)
                        return EType.CROSS_RIGHT;
                    else
                        return EType.INESSENTIAL;
                case CLPointType.BETWEEN:
                case CLPointType.ORIGIN:
                case CLPointType.DESTINATION:
                    return EType.TOUCHING;
                default:
                    return EType.INESSENTIAL;
            }
        }


        private static List<Point> GetWAPoints(Polygon S, Polygon C)
        {
            List<Point> result = new List<Point>();//здесь будет набор точек
            for (int i = 0; i < S.Length; i++)
            {
                //формируем массив точек пересечения с текущем ребром
                List<Point> Cross = new List<Point>();
                for (int j = 0; j < C.Length; j++)
                {
                    Point cross = Utils.Intersect(S.points[i], S.points[(i + 1) % S.Length],
                       C.points[j], C.points[(j + 1) % C.Length]);
                    if (cross != null)
                        Cross.Add(cross);
                }

                //сортируем по удаленности от 1 точки
                Point start = S.points[i];
                Cross = Cross.OrderBy(p => (p.x - start.x) * (p.x - start.x) +
                    (p.y - start.y) * (p.y - start.y)).ToList();

                //добавляем начальную точку и точки пересечения
                result.Add(start);
                foreach (Point p in Cross)
                    result.Add(p);
            }
            return result;
        }



        private static bool StartWAWithFirst(List<Point> S, List<Point> C, Point p)
        {
            //определим отрезки
            Point s1 = null, s2 = null;
            for (int i = 0; i < S.Count(); i++)
                if (Utils.PointInSegment(S[i], S[(i + 1) % S.Count()], p))
                {
                    s1 = S[i];
                    s2 = S[(i + 1) % S.Count()];
                    break;
                }
            Point c1 = null, c2 = null;
            for (int i = 0; i < S.Count(); i++)
                if (Utils.PointInSegment(C[i], C[(i + 1) % C.Count()], p))
                {
                    c1 = C[i];
                    c2 = C[(i + 1) % C.Count()];
                    break;
                }


            //первая - слева, вторая справа
            return (Utils.Classify(c1.x, c1.y, c2.x, c2.y, s1.x, s1.y) == CLPointType.LEFT &&
                Utils.Classify(c1.x, c1.y, c2.x, c2.y, s2.x, s2.y) == CLPointType.RIGHT);

        }


        #endregion

    }
}
