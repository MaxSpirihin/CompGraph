using Graph;
using HW1.States;
using ShellExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace HW1
{
    

    public partial class MainWindow : Window, IPixelPressListener
    {
        
        State state;

        public MainWindow()
        {
            InitializeComponent();
            Shell.Instance.AddListener(this);
            state = new StateBZ(BTNEnter);

        }

        private void BTNClear_Click(object sender, RoutedEventArgs e)
        {
            state.onPressClearBtn();
        }

        private void BTNEnter_Click(object sender, RoutedEventArgs e)
        {
            state.onPressMainBtn();
        }

        public void onPressPixel(int x, int y)
        {
            state.onPressPixel(x, y);
            
        }


        public void onMove(int x, int y)
        {
            state.onMove(x, y);
        }

        public void onOut(int x, int y)
        {
            state.onOut(x, y);
        }

        private void CMBTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (state == null) return;
            BTNEnter.Visibility = System.Windows.Visibility.Visible;
            BTNEnter.IsEnabled = true;
            BTNEnter.Content = "";

            Shell.Instance.Clear();
            switch (CMBTask.SelectedIndex)
            {
                case 0:
                    state = new StateBZ(BTNEnter);
                    break;
                case 1:
                    state = new StateERM(BTNEnter);
                    break;
                case 2:
                    state = new StateCL(BTNEnter);
                    break;
                case 3:
                    state = new StateWA(BTNEnter);
                    break;
            }

        }
    }
}
