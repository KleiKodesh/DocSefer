using Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfLib;
using WpfLib.ViewModels;

namespace DocSefer.Paragraphs
{
    public class ParagraphsViewModel : ViewModelBase
    {
        public class ActiveStyle : ViewModelBase
        {
            bool _apply;
            public Style Style { get; set; }
            public bool Apply 
            {
                get => _apply;
                set => SetProperty(ref _apply, value);
            }
            public string Name => Style?.NameLocal; // Optional: for UI display
        }

        int _minLineCount = 2;
        ObservableCollection<ActiveStyle> _activeStyles = new ObservableCollection<ActiveStyle>(Vsto.ActiveStyles.Select(s => new ActiveStyle { Style = s, Apply = !(s.NameLocal.ToLower().StartsWith("head") || s.NameLocal.StartsWith("כותר")) }));
        bool _refreshStyles;
        bool? _checkAllStyles;
        public int MinLineCount { get => _minLineCount; set => SetProperty(ref _minLineCount, value); }
        public bool? CheckAllStyles { get => _checkAllStyles; set { if (SetProperty(ref _checkAllStyles, value)) CheckAllChanged(value); } }
        public ObservableCollection<ActiveStyle> ActiveStyles { get => _activeStyles; set => SetProperty(ref _activeStyles, value); } 
        public bool RefreshStyles  {set => RefreshActiveStylesAction(); }
        List<Style> ValidStyles => ActiveStyles.Where(s => s.Apply == true).Select(s => s.Style).ToList();

        // SubClasses
        public CenterLastLine CenterLastLine { get; } = new CenterLastLine();
        public FirstWordStyle FirstWordStyle { get; } = new FirstWordStyle();
        public FirstWordHanging FirstWordHanging { get; } = new FirstWordHanging();

        // Apply Commands
        public RelayCommand ApplyFirstWordHangingCommand => new RelayCommand(() => FirstWordHanging.Apply(ValidStyles, MinLineCount));
        public RelayCommand ApplyDoubleFirstWordHangingCommand => new RelayCommand(() => FirstWordHanging.DoubleWindow(ValidStyles, MinLineCount));
        public RelayCommand ApplyFirstWordStyleCommand => new RelayCommand(() => FirstWordStyle.Apply(ValidStyles, MinLineCount));
        public RelayCommand ApplyCenterLastLineCommand => new RelayCommand(() => CenterLastLine.Apply(ValidStyles, MinLineCount));

        // Remove Commands
        public RelayCommand RemoveFirstWordHangingCommand => new RelayCommand(() => FirstWordHanging.Remove());
        public RelayCommand RemoveFirstWordStyleCommand => new RelayCommand(() => FirstWordStyle.Remove());
        public RelayCommand RemoveCenterLastLineCommand => new RelayCommand(() => CenterLastLine.Remove());

        void RefreshActiveStylesAction()
        {
            var styles = Vsto.ActiveStyles.Select(s => new ActiveStyle { Style = s, Apply = !(s.NameLocal.ToLower().StartsWith("head") || s.NameLocal.StartsWith("כותר")) });
            foreach (Style style in styles)
                if (!ActiveStyles.Any(s => s.Style.NameLocal == style.NameLocal))
                    ActiveStyles.Add(new ActiveStyle
                    {
                        Style = style,
                        Apply = !(style.BuiltIn && (style.NameLocal.ToLower().StartsWith("head") || style.NameLocal.StartsWith("כותר")))
                    });
            CheckAllStyles = ActiveStyles.All(s => s.Apply) ? true : ActiveStyles.All(s => !s.Apply) ? false : (bool?)null;
        }

        void CheckAllChanged(bool? value)
        {
            foreach (var entry in ActiveStyles) entry.Apply = value ?? false;
        }
    }
}
