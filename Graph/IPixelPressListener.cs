using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    /// <summary>
    /// интерфейс для подписки на событие нажатия на поле
    /// </summary>
    public interface IPixelPressListener
    {
        /// <summary>
        /// вызывается при нажатии на поле ЛКМ, возвращает коор-ты нажатого пикселя
        /// </summary>
        void onPressPixel(int x, int y);

        /// <summary>
        /// вызывается при движении нажатой ЛКМ
        /// </summary>
        void onMove(int x, int y);


        /// <summary>
        /// вызывается при отпускании ЛКМ
        /// </summary>
        void onOut(int x, int y);
    }
}
