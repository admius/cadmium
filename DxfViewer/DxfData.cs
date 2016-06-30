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
        private double viewX = 0;
        private double viewY = 0;
        private double viewWidth = 500;
        private double viewHeight = 500;
        private List<Shape> geomList = new List<Shape>();
        private Dictionary<String, DxfObject> blockMap = new Dictionary<string, DxfObject>();

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

            //save the blocks
            DxfObject blocksSection = dxfObject.GetValue("Section:BLOCKS");
            DxfObject blocks = blocksSection.GetValue("Blocks Body");
            foreach(DxfObject block in blocks.DataList)
            {
                string blockName = GetBlockName(block);
                blockMap.Add(blockName, block);
            }

            //convert the entities
            DxfObject entitiesSection = dxfObject.GetValue("Section:ENTITIES");
            DxfObject entities = entitiesSection.GetValue("Entities Body");

            Transform baseTransform = Transform.Identity;

            foreach(DxfObject entity in entities.DataList)
            {
                ProcessEntity(entity,baseTransform,geomList);
            }
        }

        #region Entity Accessors
        private string GetBlockName(DxfObject block)
        {
            //get the header from the block section
            DxfObject header = block.DataList[0];
            //read the entry with code 2
            return header.GetValue("2");
        }

        private List<dynamic> GetBlockEntities(DxfObject block)
        {
            //get the body from the block section
            DxfObject blockBody = block.DataList[1];
            

            //@todo - add a constant for the entity name - or better yet, figure out a better way to do this.
            if (blockBody.Key == "Entities Body")
            {
                return blockBody.DataList;
            }
            else
            {
                //no body
                return null;
            }
        }

        private string GetEntityType(DxfObject entity)
        {
            return entity.GetValue("0");
        }
        
        private string GetInsertBlockName(DxfObject insert)
        {
            return insert.GetValue("2");
        } 

        private double? GetDoubleValue(DxfObject dxfobject, string code)
        {
            string value = dxfObject.GetValue(code);
            if (value != null)
            {
                return Double.Parse(value);
            }
            else
            {
                return null;
            }
        }

        private Point? GetPoint(DxfObject dxfObject, string codeX, string codeY)
        {
            double? x = GetDoubleValue(dxfObject, codeX);
            double? y = GetDoubleValue(dxfObject, codeY);
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
            Point? scaleXY = GetPoint(dxfObject, "41", "42");
            ScaleTransform st;
            if (scaleXY.HasValue)
            {
                st = new ScaleTransform();
                st.ScaleX = scaleXY.Value.X;
                st.ScaleY = scaleXY.Value.Y;
                t.Children.Add(st);
            }

            //optional rotate
            double? angle = GetDoubleValue(insert,"50");
            RotateTransform rt;
            if(angle.HasValue)
            {
                rt = new RotateTransform();
                rt.Angle = angle.Value;
                t.Children.Add(rt);
            }
  

            //translate required
            Point? xy = GetPoint(dxfObject, "10", "20");
            TranslateTransform tt = new TranslateTransform();
            tt.X = xy.Value.X;
            tt.Y = xy.Value.Y;
            t.Children.Add(tt);

            return t;
        }

        private Transform GetBlockBasePointTransform(DxfObject block)
        {
            //translate required
            Point? xy = GetPoint(dxfObject, "10", "20");
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

        private void ProcessEntity(DxfObject entity, Transform transform, List<Shape> geomList)
        {
            string type = GetEntityType(entity);
            switch(type)
            {
                case ("LINE"):
                    ProcessLine(entity, transform, geomList);
                    break;

                case ("INSERT"):
                    ProcessInsert(entity, transform, geomList);
                    break;

                //need to add other types!
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
                path.StrokeThickness = 1;
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
            DxfObject block = blockMap[blockName];
            if(block == null)
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
                    ProcessEntity((DxfObject)entity, transform, blockGeomList);
                }
            }

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

