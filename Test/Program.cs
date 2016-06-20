using _3D;
using Graph;
using ShellExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Parallelepiped p = new Parallelepiped()
            {
                Center = new Vector3(200, 200, 200),
                Size = new Vector3(100, 200, 400)
            };

            p.DrawParallel(Shell.Instance);

        }
    }
}
