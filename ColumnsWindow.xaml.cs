using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
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
using System.Windows.Shapes;

namespace AutoDimCol
{
    /// <summary>
    /// Interaction logic for ColumnsWindow.xaml
    /// </summary>
    public partial class ColumnsWindow : Window
    {
        ExternalEvent ExEventDimension;
        ExternalDimensionsHandler externalDimensionsHandler = new ExternalDimensionsHandler();





        public UIDocument UIDoc { get; set; }
        List<Element> columns = new List<Element>();
        public ColumnsWindow()
        {
            InitializeComponent();
            ExEventDimension = ExternalEvent.Create(externalDimensionsHandler);
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryFilter categoryFilter = new CategoryFilter(BuiltInCategory.OST_StructuralColumns);
                columns = UIDoc.Selection.PickElementsByRectangle(categoryFilter, "Please select columns").ToList();  
                columnsNo.Text = columns.Count.ToString();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                externalDimensionsHandler.SelectedColumns = columns;
                externalDimensionsHandler.Offset = 100;
                ExEventDimension.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
