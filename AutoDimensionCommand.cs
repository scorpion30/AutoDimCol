using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace AutoDimCol
{
    [Transaction(TransactionMode.Manual)]
    public class AutoDimensionCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            ColumnsWindow columnsWindow = new ColumnsWindow();
            columnsWindow.UIDoc = uidoc;
            columnsWindow.Show();

            return Result.Succeeded;
        }
    }
}
