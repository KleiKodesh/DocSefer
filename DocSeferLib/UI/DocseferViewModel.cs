using DocSeferLib.Columns;
using DocSeferLib.Paragraphs;

namespace DocSeferLib.UI
{
    public class DocseferViewModel
    {
        public ParagraphsViewModel ParagraphsViewModel { get; } = new ParagraphsViewModel();
        public ColumnsViewModel ColumnsViewModel { get; } = new ColumnsViewModel();

        public DocseferViewModel() { }

    }
}
