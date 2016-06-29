using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DxfLib;
using DxfLib.Data;
using System.Windows;
using System.Windows.Media;

namespace DxfViewer
{
    public class DxfData
    {
        private DxfObject dxfObject;
        private double viewX;
        private double viewY;
        private double viewWidth;
        private double viewHeight;
        private List<Shape> geomList;

        public static DxfData Open(string dxfFile, string configFile)
        {
            DxfReader dxfReader = new DxfReader();
            dxfReader.Init(configFile);

            DxfObject docObject = dxfReader.Open(dxfFile);

            return new DxfData(docObject);
        }

        public List<Shape> GeomList
        {
            get { return geomList; }
        }

        public double ViewX {
            get { return viewX;}
        }

        public double ViewY
        {
            get { return viewY; }
        }

        public double ViewWidth
        {
            get { return viewWidth; }
        }

        public double ViewHeight
        {
            get { return viewHeight; }
        }

        //=================================
        // Private Methods
        //=================================

        private DxfData(DxfObject dxfObject)
        {
            this.dxfObject = dxfObject;
        }


        //we need to use this to configure the dxf objects into geometry
        private Shape getShape(dynamic geom)
        {
            System.Windows.Media.PathGeometry pathGeom = new System.Windows.Media.PathGeometry();
            System.Windows.Media.PathFigure pathFigure = null;

            foreach (dynamic instr in geom)
            {

                int instrType = instr[0];
                switch (instrType)
                {
                    case 0:
                        {
                            //move to
                            if (pathFigure != null)
                            {
                                pathGeom.Figures.Add(pathFigure);
                            }
                            pathFigure = new System.Windows.Media.PathFigure();
                            pathFigure.StartPoint = new Point((double)instr[1], (double)instr[2]);
                            break;
                        }

                    case 1:
                        {
                            //line to
                            Point point = new Point((double)instr[1], (double)instr[2]);
                            System.Windows.Media.PathSegment segment = new System.Windows.Media.LineSegment(point, true);
                            pathFigure.Segments.Add(segment);
                            break;
                        }

                    case 2:
                        {
                            //quad to
                            Point point1 = new Point((double)instr[1], (double)instr[2]);
                            Point point2 = new Point((double)instr[3], (double)instr[4]);
                            System.Windows.Media.PathSegment segment = new System.Windows.Media.QuadraticBezierSegment(point1, point2, true);
                            pathFigure.Segments.Add(segment);
                            break;
                        }

                    case 3:
                        {
                            //cube to
                            Point point1 = new Point((double)instr[1], (double)instr[2]);
                            Point point2 = new Point((double)instr[3], (double)instr[4]);
                            Point point3 = new Point((double)instr[5], (double)instr[6]);
                            System.Windows.Media.PathSegment segment = new System.Windows.Media.BezierSegment(point1, point2, point3, true);
                            pathFigure.Segments.Add(segment);
                            break;
                        }

                    case 4:
                        {
                            //close
                            //figure out how to close!!!
                            break;
                        }
                }

            }

            //add the last segment
            if (pathFigure != null)
            {
                pathGeom.Figures.Add(pathFigure);
            }

            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Data = pathGeom;
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1;

            return path;

        }
    }
}
}
