using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace AutoDimCol
{
    internal class CategoryFilter : ISelectionFilter
    {
        public BuiltInCategory ElementCategory { get; set; }
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) { return false; }
            if( (int)elem.Category.BuiltInCategory == (int)ElementCategory)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CategoryFilter(BuiltInCategory elementCategory)
        {
            ElementCategory = elementCategory;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
