using DocSefer.Helpers;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;

public static class Vsto
{
    public static Application Application { get; set; }
    public static UndoRecord UndoRecord => Application?.UndoRecord;
    public static Selection Selection => Application.Selection;
    public static Document ActiveDocument => Application.ActiveDocument;
    public static IEnumerable<Style> ActiveStyles => ActiveDocument?.Styles.Cast<Style>().Where(s => s.InUse);
    
    public static RangePageData RangePageCount(this Range range)
    {
        var actionRange = range.Duplicate;
        int lastPage = (int)actionRange.Information[WdInformation.wdActiveEndPageNumber];
        actionRange.Collapse(WdCollapseDirection.wdCollapseStart);
        int firstPage = (int)actionRange.Information[WdInformation.wdActiveEndPageNumber];
        return new RangePageData { LastPage = lastPage, FirstPage = firstPage };
    }

    public static List<Range> RangeSections(this Range range)
    {
        if (range.Sections.Count < 2)
            return new List<Range> { range.Duplicate };

        List<Range> result = new List<Range>();
        foreach (Section section in range.Sections)
        {
            Range sectionRange = section.Range;
            sectionRange.Start = Math.Max(sectionRange.Start, range.Start);
            sectionRange.End = Math.Min(sectionRange.End, range.End);
            result.Add(sectionRange);
        }

        return result;
    }
}

