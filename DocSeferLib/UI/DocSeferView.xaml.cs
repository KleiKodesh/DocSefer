using System.Windows.Controls;

namespace DocSeferLib
{
    /// <summary>
    /// Interaction logic for DocSeferLibView2.xaml
    /// </summary>
    public partial class DocSeferLibView : UserControl
    {
        public DocSeferLibView(Microsoft.Office.Interop.Word.Application app)
        {
            Vsto.Application = app;
            InitializeComponent();
        }
    }
}
