using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDimCol
{
    internal class ExternalDimensionsHandler : IExternalEventHandler
    {
        public List<Element> SelectedColumns { get; set; }  

        public double Offset { get; set; }
        public void Execute(UIApplication app)
        {
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;
            try
            {
                using(Transaction te = new Transaction(doc, "Dimensions")) 
                {
                    te.Start();

                   foreach ( var col in SelectedColumns )
                    {
                        var colGeometry = col.GetSymbolGeometry(doc);
                        List<PlanarFace> facesY = new List<PlanarFace>();
                        List<PlanarFace> facesX = new List<PlanarFace>();
                        foreach ( PlanarFace face in colGeometry.Faces)
                        {
                            if(face.FaceNormal.X.Abs ().Approximately (1))
                            {
                                facesY.Add(face);
                            }
                            else if (face.FaceNormal.Y .Abs().Approximately(1))
                            {
                                facesX.Add(face);
                            }
                        }

                        ReferenceArray referenceArrayY = new ReferenceArray();
                        ReferenceArray referenceArrayX = new ReferenceArray();

                        foreach(var face in facesY)
                        {
                            referenceArrayY.Append(face.Reference);
                        }
                        foreach (var face in facesX)
                        {
                            referenceArrayX.Append(face.Reference);
                        }

                        
                        var bBox = col.get_BoundingBox(doc.ActiveView);
                        var maxPt = bBox.Max;
                        var minPt = bBox.Min;

                        var offset = UnitUtils.ConvertToInternalUnits(Offset, UnitTypeId.Millimeters);
                        var lineX = Line.CreateBound(new XYZ(minPt.X, maxPt.Y+offset, maxPt.Z), new XYZ(maxPt.X, maxPt.Y + offset, maxPt.Z));
                        var lineY = Line.CreateBound(new XYZ(maxPt.X + (offset * 6), minPt.Y  , maxPt.Z), new XYZ(maxPt.X + (offset * 6), maxPt.Y  , maxPt.Z));

                        doc.Create.NewDimension(doc.ActiveView, lineX, referenceArrayY);
                        doc.Create.NewDimension(doc.ActiveView, lineY, referenceArrayX);
                    }

                    te.Commit();
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "Dimensions";
        }
    }
}
