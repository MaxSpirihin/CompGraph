using ShellExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _3D
{
    public class Vector3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Vector3() { }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public Point ParallelProection()
        {
            return new Point(x, y);
        }


        public Point PerspectiveProection(double k)
        {
            double r = 1 / k;
            return new Point(x / (r * z + 1), y / (r * z + 1));
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            Vector3 res = new Vector3()
            {
                x = v1.x + v2.x,
                y = v1.y + v2.y,
                z = v1.z + v2.z
            };
            return res;

        }


        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            Vector3 res = new Vector3()
            {
                x = v1.x - v2.x,
                y = v1.y - v2.y,
                z = v1.z - v2.z
            };
            return res;

        }


        public static Vector3 operator *(Vector3 a,Vector3 b)
        {
            return new Vector3(
                a.y*b.z-a.z*b.y,
                b.x*a.z-a.x*b.z,
                a.x*b.y-b.x*a.y           
                );
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }



        public static Vector3 operator *(Vector3 a, double l)
        {
            return new Vector3(a.x * l, a.y * l, a.z * l);
        }


        public static Vector3 operator *(double l,Vector3 a)
        {
            return new Vector3(a.x * l, a.y * l, a.z * l);
        }


        public static Vector3 operator /(Vector3 a, double l)
        {
            return new Vector3(a.x / l, a.y / l, a.z / l);
        }
    }
}
