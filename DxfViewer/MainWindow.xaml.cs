using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace DxfViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int COMMAND_LINE_DXF_CONFIG = 1;

        private CanvasManager canvasManager;

        private const double DX = 10;
        private const double DY = 10;
        private const double DZOOMIN = 1.1;

        public MainWindow()
        {
            InitializeComponent();

            canvasManager = new CanvasManager(this.displayCanvas);
        }

        private void menuFile_New(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("New");
        }

        private void menuFile_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                canvasManager.OpenFile(openFileDialog.FileName);
            }
            else
            {
                MessageBox.Show("There was an error!");
            }
        }

        private void menuFile_Close(object sender, RoutedEventArgs e)
        {
            canvasManager.CloseFile();
        }

        private void menuFile_Exit(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Exit");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {

                case Key.Up:
                    canvasManager.Pan(0, -DY);
                    break;

                case Key.Down:
                    canvasManager.Pan(0, DY);
                    break;

                case Key.Right:
                    canvasManager.Pan(DX, 0);
                    break;

                case Key.Left:
                    canvasManager.Pan(-DX, 0);
                    break;
                /*
                                case Key.PageUp:
                                    canvasManager.Zoom(DZOOMIN);
                                    break;

                                case Key.PageDown:
                                    canvasManager.Zoom(1 / DZOOMIN);
                                    break;
                */

                case Key.Home:
                    canvasManager.FitToScreen();
                    break;
            }
        }

        private void panel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //mouse wheel gives 120 - we need to make a better handling of this, to allow for other mouse wheels
            double zoomFactor = Math.Pow(DZOOMIN, e.Delta / 120);
            Point center = e.GetPosition(displayCanvas);
            canvasManager.Zoom(zoomFactor, center);

            e.Handled = true;
        }

        private bool dragInProgress = false;
        private Point lastPoint;

        private void panel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragInProgress = true;
            lastPoint = e.GetPosition(panel);
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragInProgress)
            {
                Console.Out.WriteLine("Old: " + lastPoint.X + "," + lastPoint.Y);
                Point currentPoint = e.GetPosition(panel);
                Console.Out.WriteLine("New: " + currentPoint.X + "," + currentPoint.Y);
                canvasManager.Pan(currentPoint.X - lastPoint.X, currentPoint.Y - lastPoint.Y);
                lastPoint = currentPoint;
            }
        }

        private void panel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragInProgress = false;
        }

        private void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            dragInProgress = false;
        }
    }
}
