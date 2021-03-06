﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DxfLib;
using DxfLib.Data;
using DxfLib.Query;
using System.Windows;
using System.Windows.Media;

namespace DxfViewer
{

    public class DxfData
    {
        #region properties
        private DxfObject docObject;
        private Point viewMin;
        private Point viewMax;
        private List<String> layers = new List<String>();
        private List<Shape> geomList = new List<Shape>();
        private Dictionary<String, DxfObject> blockMap = new Dictionary<string, DxfObject>();

        //working variables for multe element entities
        private Path activePath = null;
        private PathFigure activePathFigure = null;
        private bool activePathStarted = false;

        private HashSet<String> unsupportedTypes = new HashSet<string>();

        private static double LINE_THICKNESS = 1;
        private static int POLYLINE_CLOSED = 1;

        #endregion

        #region Public Methods
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
            get { return viewMin.X;}
        }

        public double ViewY
        {
            get { return viewMin.Y; }
        }

        public double ViewWidth
        {
            get { return viewMax.X - viewMin.X; }
        }

        public double ViewHeight
        {
            get { return viewMax.Y - viewMin.Y; }
        }

        public List<String> Layers
        {
            get { return layers; }
        }
        #endregion

        //=================================
        // Private Methods
        //=================================

        #region Constructor
        private DxfData(DxfObject docObject)
        {
            this.docObject = docObject;

            //read the layers
            List<dynamic> sectionList = GetList.FromDxf(this.docObject, Has.Code("SECTION"));

            DxfObject headerSection = GetObject.FromList(sectionList, Has.Object(Has.Both(Has.Code("SECTION"), Has.Property("2", "HEADER"))));
            DxfObject tablesSection = GetObject.FromList(sectionList, Has.Object(Has.Both(Has.Code("SECTION"), Has.Property("2", "TABLES"))));
            DxfObject blocksSection = GetObject.FromList(sectionList, Has.Object(Has.Both(Has.Code("SECTION"), Has.Property("2", "BLOCKS"))));
            DxfObject entitiesSection = GetObject.FromList(sectionList, Has.Object(Has.Both(Has.Code("SECTION"), Has.Property("2", "ENTITIES"))));

            //read the dimensions
            DxfObject headBody = GetObject.FromDxf(headerSection, Has.Code("list"));
            DxfObject extentsMin = GetObject.FromDxf(headBody, Has.Code("$EXTMIN"));
            if (extentsMin != null)
            {
                extentsMin.DebugPrint(Console.Out, 0);
                Point? nullableViewMin = this.GetPoint(extentsMin, "10", "20");
                if (nullableViewMin.HasValue)
                {
                    viewMin = nullableViewMin.Value;
                }
            }

            DxfObject extentsMax = GetObject.FromDxf(headBody, Has.Code("$EXTMAX"));
            if (extentsMax != null)
            {
                extentsMax.DebugPrint(Console.Out, 0);
                Point? nullableViewMax = this.GetPoint(extentsMax, "10", "20");
                if (nullableViewMax.HasValue)
                {
                    viewMax = nullableViewMax.Value;
                }
            }
            //read the tables
            DxfObject tablesBody = GetObject.FromDxf(tablesSection, Has.Code("list"));

            //read the layers
            DxfObject layerTable = GetObject.FromDxf(tablesBody, Has.Object(Has.Property("2", "LAYER")));
            DxfObject layersListObject = GetObject.FromDxf(layerTable, Has.Code("list"));
            foreach (DxfObject layer in layersListObject.DataList)
            {
                DxfEntry layerNameEntry = GetObject.FromDxf(layer, Has.Code("2"));
                Console.WriteLine("Layer: " + layerNameEntry.Value);
                layers.Add(layerNameEntry.Value);
            }

            //save the blocks
            DxfObject blocksBody = GetObject.FromDxf(blocksSection,Has.Code("list"));
            foreach(DxfObject block in blocksBody.DataList)
            {
                string blockName = GetBlockName(block);
                blockName = blockName.ToUpper();
                blockMap.Add(blockName, block);
            }

            //convert the entities
            DxfObject entitiesBody = GetObject.FromDxf(entitiesSection, Has.Code("list"));
            Transform baseTransform = Transform.Identity;

            foreach(DxfObject entity in entitiesBody.DataList)
            {
                ProcessEntity(entity,baseTransform,geomList);
            }

            //Write any unsupported types:
            StringBuilder sb = new StringBuilder();
            foreach(string type in unsupportedTypes)
            {
                sb.Append(type);
                sb.Append("; ");
            }
            MessageBox.Show(sb.ToString(), "Unsupported Entity Types");

        }
        #endregion

        #region DxfObject Accessors
        private string GetBlockName(DxfObject block)
        {
            //get the header from the block section
            DxfObject header = block.DataList[0];
            //read the entry with code 2
            return header.GetStringValue("2");
        }

        private DxfObject GetBlock(string blockName)
        {
            //case insensitive?
            blockName = blockName.ToUpper();

            if (!blockMap.ContainsKey(blockName))
            {
                return null;
            }

            return blockMap[blockName];
        }

        private List<dynamic> GetBlockEntities(DxfObject block)
        {
            //get the body is the list section
            DxfObject blockBody = block.GetEntry("list");
            if(blockBody != null)
            {
                return blockBody.DataList;
            }
            else
            {
                return null;
            }
        }
        
        private string GetInsertBlockName(DxfObject insert)
        {
            return insert.GetStringValue("2");
        }

        private Point? GetPoint(DxfObject dxfObject, string codeX, string codeY, int nth = 0)
        {
            double? x = dxfObject.GetDoubleValue(codeX,nth);
            double? y = dxfObject.GetDoubleValue(codeY,nth);
            if ((x.HasValue) && (y.HasValue))
            {
                return new Point(x.Value,y.Value);
            }
            else
            {
                return null;
            }
        }

        private Transform GetInsertTransform(DxfObject insert)
        {
            TransformGroup t = new TransformGroup();

            //optional scale
            Point? scaleXY = GetPoint(insert, "41", "42");
            ScaleTransform st;
            if (scaleXY.HasValue)
            {
                st = new ScaleTransform();
                st.ScaleX = scaleXY.Value.X;
                st.ScaleY = scaleXY.Value.Y;
                t.Children.Add(st);
            }

            //optional rotate
            double? angle = insert.GetDoubleValue("50");
            RotateTransform rt;
            if(angle.HasValue)
            {
                rt = new RotateTransform();
                rt.Angle = angle.Value;
                t.Children.Add(rt);
            }
  

            //translate required
            Point? xy = GetPoint(insert, "10", "20");
            TranslateTransform tt = new TranslateTransform();
            tt.X = xy.Value.X;
            tt.Y = xy.Value.Y;
            t.Children.Add(tt);

            return t;
        }

        private Transform GetBlockBasePointTransform(DxfObject block)
        {
            DxfObject blockHeader = block.DataList[0];

            //translate required
            Point? xy = GetPoint(blockHeader, "10", "20");
            if (xy.HasValue)
            {
                TranslateTransform tt = new TranslateTransform();
                tt.X = xy.Value.X;
                tt.Y = xy.Value.Y;
                return tt;
            }
            else
            {
                return Transform.Identity;
            }
        }
        #endregion

        #region Entity Processing
        private void ProcessEntity(DxfObject entity, Transform transform, List<Shape> geomList)
        {
            string type = entity.Code;
        
            switch(type)
            {
                case ("LINE"):
                    ProcessLine(entity, transform, geomList);
                    break;

                case ("INSERT"):
                    ProcessInsert(entity, transform, geomList);
                    break;

                case ("POLYLINE"):
                    ProcessPolyLine(entity, transform, geomList);
                    break;

                case ("LWPOLYLINE"):
                    ProcessLWPolyLine(entity, transform, geomList);
                    break;

                case ("VERTEX"):
                    ProcessVertex(entity, transform, geomList);
                    break;

                case ("SEQEND"):
                    ProcessSeqEnd(entity, transform, geomList);
                    break;

                case ("CIRCLE"):
                    ProcessCircle(entity, transform, geomList);
                    break;

//                case ("ARC"):
//                    ProcessArc(entity, transform, geomList);
//                    break;


                default:
                    //record unsupported types
                    unsupportedTypes.Add(type);
                    break;   
            }
        }

        private void ProcessLine(DxfObject dxfLine, Transform transform, List<Shape> geomList)
        {
            Point? start = GetPoint(dxfLine,"10", "20");
            Point? end = GetPoint(dxfLine,"11", "21");
            if(start.HasValue && end.HasValue)
            {
                Point startPoint = transform.Transform(start.Value);
                Point endPoint = transform.Transform(end.Value);
                LineGeometry line = new LineGeometry(startPoint,endPoint);
                Path path = new Path();
                path.Data = line;
                path.Stroke = Brushes.Black;
                path.StrokeThickness = LINE_THICKNESS;
                geomList.Add(path);

            }
            else
            {
                Console.WriteLine("Invalid Line!");
                return;
            }
        }

        private void ProcessInsert(DxfObject dxfInsert, Transform transform, List<Shape> geomList)
        {
            string blockName = GetInsertBlockName(dxfInsert);
            DxfObject block = GetBlock(blockName);
            if (block == null)
            {
                Console.WriteLine("Block not found: " + blockName);
                return;
            }

            List<dynamic> blockEntities = GetBlockEntities(block);

            if (blockEntities != null)
            {
                TransformGroup tg = new TransformGroup();               

                //get the base transform from the block definition
                Transform blockBaseTransform = this.GetBlockBasePointTransform(block);
                tg.Children.Add(blockBaseTransform);

                // need to add offset from entity and subtract origin from block
                Transform insertTransform = this.GetInsertTransform(dxfInsert);
                tg.Children.Add(insertTransform);

                //add the requested input transform
                tg.Children.Add(transform);

                List<Shape> blockGeomList = new List<Shape>();
                foreach (dynamic entity in blockEntities)
                {
                    ProcessEntity((DxfObject)entity, tg, blockGeomList);
                }

                geomList.AddRange(blockGeomList);
            }

        }

        private void ProcessPolyLine(DxfObject dxfLine, Transform transform, List<Shape> geomList)
        {

            bool isClosed = false;

            int? flags = dxfLine.GetIntValue("70");
            if(flags.HasValue)
            {
                if( (flags & POLYLINE_CLOSED) != 0)
                {
                    isClosed = true;
                }
            }

            activePath = new Path();
            PathGeometry pathGeom = new PathGeometry();
            activePath.Data = pathGeom;
            activePathFigure = new PathFigure();
            pathGeom.Figures.Add(activePathFigure);

            activePathFigure.IsClosed = isClosed;
            activePathStarted = false;
        }

        private void ProcessVertex(DxfObject dxfVertex, Transform transform, List<Shape> geomList)
        {
            if(activePath == null)
            {
                Console.WriteLine("Vertex found with no active path!");
                return;
            }

            Point? point = GetPoint(dxfVertex, "10", "20");
            if (point.HasValue)
            {
                Point transformedPoint = transform.Transform(point.Value);
                if (activePathStarted) 
                {
                    LineSegment lineSegment = new LineSegment();
                    lineSegment.Point = transformedPoint;
                    activePathFigure.Segments.Add(lineSegment);
                }
                else
                {
                    activePathFigure.StartPoint = transformedPoint;
                    activePathStarted = true;
                }
            }
            else
            {
                Console.WriteLine("Missing point in vertex!");
            }
        }

        private void ProcessSeqEnd(DxfObject dxfObject, Transform transform, List<Shape> geomList)
        {
            if (activePath != null)
            {
                activePath.Stroke = Brushes.Black;
                activePath.StrokeThickness = LINE_THICKNESS;
                geomList.Add(activePath);

                activePath = null;
                activePathFigure = null;
                activePathStarted = false;
            }
            else
            {
                Console.WriteLine("ENDSEQ found while not in a known sequence.");
            }
        }

        private void ProcessLWPolyLine(DxfObject dxfLWPolyLine, Transform transform, List<Shape> geomList)
        {

            bool isClosed = false;

            //read flags
            int? flags = dxfLWPolyLine.GetIntValue("70");
            if (flags.HasValue)
            {
                if ((flags & POLYLINE_CLOSED) != 0)
                {
                    isClosed = true;
                }
            }

            //read vertices count
            int? numVertices = dxfLWPolyLine.GetIntValue("90");
            if(!numVertices.HasValue)
            {
                Console.WriteLine("Missing numer of vertices in lwpolyline!");
                return;
            }

            //create path
            Path path = new Path();
            PathGeometry pathGeom = new PathGeometry();
            path.Data = pathGeom;
            PathFigure pathFigure = new PathFigure();
            pathGeom.Figures.Add(pathFigure);

            pathFigure.IsClosed = isClosed;
            bool pathStarted = false;


            //read the points
            for (int nth = 0; nth < numVertices; nth++)
            {
                Point? point = GetPoint(dxfLWPolyLine, "10", "20", nth);
                if (point.HasValue)
                {
                    Point transformedPoint = transform.Transform(point.Value);
                    if (pathStarted)
                    {
                        LineSegment lineSegment = new LineSegment();
                        lineSegment.Point = transformedPoint;
                        pathFigure.Segments.Add(lineSegment);
                    }
                    else
                    {
                        pathFigure.StartPoint = transformedPoint;
                        pathStarted = true;
                    }
                }
                else
                {
                    Console.WriteLine("missing vertices in LWPolyline!");
                    break;
                }
            }

            path.Stroke = Brushes.Black;
            path.StrokeThickness = LINE_THICKNESS;
            geomList.Add(path);

        }

        private void ProcessCircle(DxfObject dxfCircle, Transform transform, List<Shape> geomList)
        {
            Point? center = GetPoint(dxfCircle, "10", "20");
            double? radius = dxfCircle.GetDoubleValue("40");
            if (center.HasValue && radius.HasValue)
            {
                EllipseGeometry circle = new EllipseGeometry(center.Value,radius.Value,radius.Value,transform);
                Path path = new Path();
                path.Data = circle;
                path.Stroke = Brushes.Black;
                path.StrokeThickness = LINE_THICKNESS;
                geomList.Add(path);

            }
            else
            {
                Console.WriteLine("Invalid Line!");
                return;
            }
        }

        private void ProcessArc(DxfObject dxfArc, Transform transform, List<Shape> geomList)
        {
            /*            Point? center = GetPoint(dxfArc, "10", "20");
                        double? radius = dxfArc.GetDoubleValue("40");
                        double? startAngle = dxfArc.GetDoubleValue("50");
                        double? endAngle = dxfArc.GetDoubleValue("51");
                        if (center.HasValue && radius.HasValue && startAngle.HasValue && endAngle.HasValue)
                        {
                            PathFigure pathFigure = new PathFigure();
                            pathFigure.StartPoint = center.Value; //DETERMINE THIS!!
                            ArcSegment segment = new ArcSegment(); // GET ARGUMENTS
                            pathFigure.Segments.Add(segment);
                            PathGeometry pathGeometry = new PathGeometry();
                            pathGeometry.Figures.Add(pathFigure);

                            Path path = new Path();
                            path.Data = pathGeometry;
                            path.Stroke = Brushes.Black;
                            path.StrokeThickness = LINE_THICKNESS;
                            geomList.Add(path);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Line!");
                            return;
                        }*/
        }
        #endregion

        #region extra stuff
        /*
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

        } */
        #endregion
    }


}

