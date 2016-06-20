using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Graph
{
    /// <summary>
    /// Класс оболочки для рисования. Для обращение используйте статическле поле Index. 
    /// Реализовано по паттерну "Одиночка"
    /// </summary>
    public class Shell
    {
        private static Shell _Instance;
        private MainWindow window;

        public static readonly Color DRAW_COLOR = Colors.White;
        public static readonly Color ClEAR_COLOR = Colors.Black;

        public static Shell Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Shell();
                return _Instance;
            }
        }

    
        private Shell()
        {
            try
            {
                Application app = new Application();
                window = new MainWindow();
                app.Run(window);
            }
            catch(Exception)
            {
                window = new MainWindow();
                window.Show();
            }
        }


        /// <summary>
        /// закрасить пиксел
        /// </summary>
        public void SetPixel(int x, int y)
        {
            window.SetPixel(x,y,DRAW_COLOR);
        }

        /// <summary>
        /// закрасить пиксел цветом
        /// </summary>
        public void SetPixel(int x, int y,Color color)
        {
            window.SetPixel(x, y, color);
        }

        /// <summary>
        /// очистить поле - станет черным
        /// </summary>
        public void Clear()
        {
            window.Clear();
        }

        /// <summary>
        /// подключить слушателя к событию нажатия на поле
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(IPixelPressListener listener)
        {
            window.AddListener(listener);
        }

        /// <summary>
        /// получить цвет пикселя
        /// </summary>
        public Color GetPixel(int x,int y)
        {
            return window.GetPixel(x,y);
        }

    }
}
