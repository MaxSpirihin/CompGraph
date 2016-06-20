using Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1.States
{
    class State : IPixelPressListener
    {
        public virtual void onPressMainBtn()
        {

        }
        public virtual void onPressClearBtn()
        {

        }


        public virtual void onPressPixel(int x, int y)
        {
        }

        public virtual void onMove(int x, int y)
        {
        }

        public virtual void onOut(int x, int y)
        {
        }
    }
}
