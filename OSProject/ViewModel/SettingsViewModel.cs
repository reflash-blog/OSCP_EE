using System.Windows.Input;
using IniReader;
using OSProject.View;

namespace OSProject.ViewModel
{
    class SettingsViewModel:ViewModelBase
    {
        private const string Section = "settings";
        private const string Key = "language";
        readonly IniFile IniFile = new IniFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\settings.ini");
        public SettingsViewModel()
        {
            AvailableLanguages = new[] {"Русский", "English"};
            SelectedLanguage = IniFile.IniReadValue(Section, Key, string.Empty);
        }
        public string SelectedLanguage { get; set; }
        public string[] AvailableLanguages { get; set; }

        #region SetCommand
        RelayCommand _setCommand = null;
        public ICommand SetCommand
        {
            get
            {
                if (_setCommand == null)
                {
                    _setCommand = new RelayCommand((p) => OnSet(p), (p) => CanSet(p));
                }

                return _setCommand;
            }
        }

        private bool CanSet(object parameter)
        {
            return SelectedLanguage!=string.Empty;
        }

        private void OnSet(object parameter)
        {
            Set(parameter as SettingsWindow);
        }

        public void Set(SettingsWindow window)
        {
            IniFile.IniWriteValue(Section, Key, SelectedLanguage);
            window.Close();
        }

        #endregion
    }
}
