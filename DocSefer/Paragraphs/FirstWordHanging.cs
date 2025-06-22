using DocSefer.Helpers;
using Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.Linq;
using WpfLib;

namespace DocSefer.Paragraphs
{
    public class FirstWordHanging : PargaraphsBase
    {
        public void Apply(List<Style> styles, int minLineCount)
        {
            Remove();
            var selectionRange = Vsto.Application.Selection.Range;

            using (new UndoRecordHelper("עיצוב חלון"))
            {
                PrepareFootnotes(selectionRange);
                var paragraphs = ValidParagraphs(Vsto.Selection.Range, styles, minLineCount);
                counter = 0;
                foreach (var paragraph in paragraphs)
                {
                    if (counter++ >= MaxSafeIterations)
                    {
                        counter = 0;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    Range paraRange = paragraph.Range;
                    paraRange.Collapse();
                    paraRange.MoveUntil(" ");
                    paraRange.Move();
                    float firstWordX = (float)paraRange.Information[WdInformation.wdHorizontalPositionRelativeToTextBoundary];

                    paraRange.Select();

                    Vsto.Selection.EndKey();
                    Vsto.Selection.Text = "\v" + " ";
                    Vsto.Selection.Collapse(WdCollapseDirection.wdCollapseEnd);
                    float insertX = (float)Vsto.Selection.Information[WdInformation.wdHorizontalPositionRelativeToTextBoundary];
                    Vsto.Selection.Previous().Font.Spacing = insertX - firstWordX;
                }
            }
        }

        public void DoubleWindow(List<Style> styles, int minLineCount)
        {
            Remove();

            var selectionRange = Vsto.Application.Selection.Range;

            using (new UndoRecordHelper("עיצוב חלון"))
            {
                PrepareFootnotes(selectionRange);
                var paragraphs = ValidParagraphs(selectionRange, styles, minLineCount);
                counter = 0;
                foreach (var paragraph in paragraphs)
                {
                    if (counter++ >= MaxSafeIterations)
                    {
                        counter = 0;
                        System.Windows.Forms.Application.DoEvents();
                    }
                    Range paraRange = paragraph.Range;
                    paraRange.Collapse();
                    paraRange.MoveUntil(" ");
                    paraRange.Move();
                    float firstWordX = (float)paraRange.Information[WdInformation.wdHorizontalPositionRelativeToTextBoundary];

                    paraRange.Select();

                    var selection = Vsto.Selection;
                    selection.EndKey();
                    selection.Text = "\v" + " ";
                    selection.Collapse();
                    float insertX = (float)selection.Information[WdInformation.wdHorizontalPositionRelativeToTextBoundary];
                    selection.Previous().Font.Spacing = insertX - firstWordX;

                    selection.EndKey();
                    selection.Text = "\v" + " ";
                    selection.Collapse(WdCollapseDirection.wdCollapseEnd);
                    selection.Previous().Font.Spacing = insertX - firstWordX;
                }
            }
        }

        public void Remove(Range targetRange = null)
        {
            if (targetRange == null)
                targetRange = Vsto.Selection.Range;

            using (new UndoRecordHelper("הסרת עיצוב חלון"))
            {
                var find = targetRange.Find;
                find.Text = "\v" + " ";
                find.Replacement.Text = "";
                find.Wrap = WdFindWrap.wdFindStop;
                find.Execute(Replace: WdReplace.wdReplaceAll);
            }
        }
    }
}
