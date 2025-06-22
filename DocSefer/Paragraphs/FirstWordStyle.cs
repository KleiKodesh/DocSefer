using DocSefer.Helpers;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DocSefer.Paragraphs
{
    public class FirstWordStyle : PargaraphsBase
    {
        string _selectedStyle = "מילה ראשונה";
        public string SelectedStyle 
        {
            get => _selectedStyle; 
            set => SetProperty(ref _selectedStyle, value); 
        }

        public List<string> Styles => Vsto.ActiveDocument.Styles
                .Cast<Style>()
                .Where(s => s.Type == WdStyleType.wdStyleTypeCharacter)
                .Select(s => s.NameLocal)
                .ToList();


        public FirstWordStyle()
        {
            CreateFirstWordStyle();
        }

        public void Apply(List<Style> styles, int minLineCount)
        {
            //Remove();

            var selectionRange = Vsto.Application.Selection.Range;

            using (new UndoRecordHelper("עיצוב מילה ראשונה"))
            {
                PrepareFootnotes(selectionRange);
                var paragraphs = ValidParagraphs(selectionRange, styles, minLineCount);

                foreach (var paragraph in paragraphs)
                {
                    Range paraRange = paragraph.Range;
                    paraRange.Collapse();
                    paraRange.MoveEndUntil(" ");
                    paraRange.Font.Reset();
                    paraRange.set_Style(SelectedStyle);
                    paraRange.Select();
                }
            }
        }

        public void SetFirstWordStyle()
        {
            var listView = new ListView
            {
                Width = 300,
                Height = 400
            };

            foreach (Style style in Vsto.ActiveDocument.Styles.Cast<Style>())
            {
                listView.Items.Add(style.NameLocal);
            }

            var window = new System.Windows.Window
            {
                Style = (System.Windows.Style)new System.Windows.ResourceDictionary { Source = new Uri("pack://application:,,,/WpfLib;component/Dictionaries/ThemedWindowDictionary.xaml") }["ThemedToolWindowStyle"],
                Content = listView,
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            };

            listView.SelectionChanged += (s, _) =>
            {
                if (listView.SelectedItem != null)
                {
                    Interaction.SaveSetting(AppDomain.CurrentDomain.FriendlyName, "Settings", "FirstWordStyle", listView.SelectedItem.ToString());
                    window.Close();
                }
            };

            window.ShowDialog();
        }


        public Style CreateFirstWordStyle()
        {
            string targetStyleName = Interaction.GetSetting(AppDomain.CurrentDomain.FriendlyName, "Settings", "FirstWordStyle", "מילה ראשונה");
            if (string.IsNullOrEmpty(targetStyleName))
                targetStyleName = "מילה ראשונה";

            foreach (Style targetStyle in Vsto.ActiveDocument.Styles)
                if (targetStyle.NameLocal == targetStyleName)
                    return targetStyle;

            Style newStyle = Vsto.ActiveDocument.Styles.Add(targetStyleName, WdStyleType.wdStyleTypeCharacter);
            Font font = newStyle.Font;
            font.Bold = 1;
            font.BoldBi = 1;
            font.Size += 2;
            font.SizeBi += 2;
            //newStyle.QuickStyle = true;

            return newStyle;

            // Optional: Copy style to Normal template (commented out as in original)
            // Application.OrganizerCopy(
            //     Application.ActiveDocument.Name,
            //     Application.NormalTemplate,
            //     targetStyleName,
            //     WdOrganizerObject.wdOrganizerObjectStyles
            // );
        }

        public void Remove(Range targetRange = null)
        {
            if (targetRange == null)
                targetRange = Vsto.Selection.Range;

            using (new UndoRecordHelper("הסרת עיצוב מילה ראשונה"))
            {
                foreach (Paragraph paragraph in targetRange.Paragraphs.Cast<Paragraph>().ToList())
                {
                    Range paraRange = paragraph.Range;
                    if (!paraRange.Text.Contains(" ")) continue;
                    paraRange.Collapse();
                    paraRange.MoveEndUntil(" ");
                    paraRange.MoveEnd();
                    var txt = paraRange.Text;
                    paraRange.Text = "";
                    paraRange.Text = txt;
                    paraRange.Select();
                }
            }
        }
    }
}
