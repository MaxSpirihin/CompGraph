using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExt
{
    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }


        public Point() { }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Point p = (Point)obj;
            return Math.Abs(x - p.x) < 1e-5 && Math.Abs(y - p.y) < 1e-5;
        }
    }
}
