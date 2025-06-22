using DocSefer.Helpers;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSefer.Columns
{
    public static class ColumnsHelper
    {
        public static int ColumnBreakPoint(this Range range)
        {
            using (new ScreenFreeze())
            {
                range.Select();
                Selection selection = Vsto.Selection;
                selection.Collapse(WdCollapseDirection.wdCollapseStart);

                int originalPos = (int)selection.Information[WdInformation.wdVerticalPositionRelativeToPage];
                int lastGoodPos = originalPos;

                while (true)
                {
                    selection.MoveDown(WdUnits.wdLine, 1);
                    int currentPos = (int)selection.Information[WdInformation.wdVerticalPositionRelativeToPage];

                    if (currentPos <= lastGoodPos) // Page/column break likely occurred
                        break;

                    lastGoodPos = currentPos;
                }

                return lastGoodPos;
            }
        }
    }
}
