using Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShellExt
{
    public static class Spline
    {
        public static List<Point> GetErmiteSpline(Point P1, Point P2, Point Q1, Point Q2)
        {
            List<Point> result = new List<Point>();
            int n = 100;
            for (double t = 0; t <= 1; t += 1.0 / n)
            {
                result.Add(new Point()
                    {
                        x = ErmiteFunc(P1.x, P2.x, Q1.x, Q2.x, t),
                        y = ErmiteFunc(P1.y, P2.y, Q1.y, Q2.y, t)
                    });
            }

            return result;
        }

         public static List<Point> GetErmiteSpline(List<Point> P,List<Point> Q)
        {
            List<Point> result = new List<Point>();
             for (int i=0;i<P.Count()-1;i++)
                 result.AddRange(GetErmiteSpline(P[i],P[i+1],Q[i],Q[i+1]));
             return result;
        }



        public static List<Point> GetBezierCurve(List<Point> points)
        {
            List<Point> result = new List<Point>();
            int n = 100;
            for (double t = 0; t <= 1 + 0.5/n; t += 1.0 / n)
            {
                result.Add(new Point()
                {
                    x = BezierFunc(points.Select(p=>p.x).ToList(),t),
                    y = BezierFunc(points.Select(p => p.y).ToList(), t)
                });
            }

            return result;
        }

        private static double ErmiteFunc(double p1, double p2, double q1, double q2, double t)
        {
            return (2 * Math.Pow(t, 3) - 3 * Math.Pow(t, 2) + 1) * p1 +
                        (Math.Pow(t, 3) - 2 * Math.Pow(t, 2) + t) * q1 +
                        (-2 * Math.Pow(t, 3) + 3 * Math.Pow(t, 2)) * p2 +
                        (Math.Pow(t, 3) - Math.Pow(t, 2)) * q2;
        }



        public static double BezierFunc(List<double> points, double t)
        {
            double res = 0;
            int m = points.Count();
            for (int i = 0; i < m; i++)
                res += Bernstein(i, m-1, t) * points[i];
            return res;
        }


        private static double Bernstein(int i,int m,double t)
        {
            return Utils.Fact(m) * Utils.Pow(t, i) * Utils.Pow(1 - t, m - i) / (Utils.Fact(i) * Utils.Fact(m - i));
        }


       


        
    }
}
