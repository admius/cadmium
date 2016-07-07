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

            Point initialOut = this.renderMatrix.Transform(canvasPoint);

            this.renderMatrix.Scale(zoomFactor, zoomFactor);

            Point finalOut = this.renderMatrix.Transform(canvasPoint);
            Vector diff = Point.Subtract(finalOut,initialOut);
            this.renderMatrix.Translate(-diff.X,-diff.Y);

            //Point finalOut2 = this.renderMatrix.Transform(canvasPoint);

            UpdateCanvasTransform();
        }

        private void PrintMatrix(Matrix matrix)
        {
            Console.WriteLine("Matrix: " + matrix.M11 + ", " + matrix.M12 + "; " + matrix.M21 + ", " + matrix.M22 + "; Offset: " + matrix.OffsetX + ", " + matrix.OffsetY);
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
