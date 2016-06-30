using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DxfViewer
{
    class CanvasManager
    {
        private Canvas canvas;
        private ScrollViewer scrollViewer;
        private Matrix renderMatrix;

        private string dxfConfigFileName;
        private DxfData data;

        public CanvasManager(Canvas canvas)
        {
            this.canvas = canvas;
            this.renderMatrix = Matrix.Identity;

            this.canvas.Height = 1000;
            this.canvas.Width = 1000;

            UpdateCanvasTransform();

            string[] args = Environment.GetCommandLineArgs();
            dxfConfigFileName = args[MainWindow.COMMAND_LINE_DXF_CONFIG];
        }

        public void OpenFile(String fileName)
        {
            data = DxfData.Open(fileName,dxfConfigFileName);
            ProcessData();
        }

        public void CloseFile()
        {
            MessageBox.Show("File close requested - nothing done though!");
        }

        public void Zoom(double zoomFactor, Point canvasPoint)
        {
            this.renderMatrix.Translate(-canvasPoint.X, -canvasPoint.Y);
            this.renderMatrix.Scale(zoomFactor, zoomFactor);
            this.renderMatrix.Translate(canvasPoint.X, canvasPoint.Y);
            UpdateCanvasTransform();
        }

        public void Pan(double dx, double dy)
        {
            this.renderMatrix.Translate(dx, dy);
            UpdateCanvasTransform();
        }

        public void FitToScreen()
        {

        }

        private void UpdateCanvasTransform()
        {
            this.canvas.RenderTransform = new MatrixTransform(this.renderMatrix);
            this.canvas.LayoutTransform = new MatrixTransform(this.renderMatrix);
        }

        private void ProcessData()
        {
            //clear the canvas
            canvas.Children.Clear();

            foreach (Shape shape in data.GeomList)
            {
                canvas.Children.Add(shape);     
            }
        }
    }

}
