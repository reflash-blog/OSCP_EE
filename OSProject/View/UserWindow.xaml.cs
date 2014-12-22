using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Elysium;
using Framework.UI.Controls;
using IniReader;
using Newtonsoft.Json;
using OSProject.Model.Structures;
using OSProject.ViewModel;

namespace OSProject.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class UserWindow
    {
        public UserWindow()
        {
            InitializeComponent();
            var settings = GetLanguageSettings();
            var context = UserViewModel.InitializeUserWindowViewModel();
            context.Settings = settings;
            this.DataContext = context;
        }

        private Settings GetLanguageSettings()
        {
            var file = new IniFile(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\settings.ini");
            var language = file.IniReadValue("settings", "language", string.Empty);
            var settings = InputObjectFromFile(language == "Русский" ? "rus.config" : "eng.config");
            return settings;
        }

        private void UserWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as UserViewModel;

            if (context != null)
                NotifyBox.Show(
                    (DrawingImage)this.FindResource("FolderDrawingImage"),
                    context.Settings.Notification,
                    context.Settings.NotificationMessage,
                    new TimeSpan(30000000));
        }


        private static Settings InputObjectFromFile(string filename)
        {
            string json;                                                             // Строка JSON
            using (var srd = new StreamReader(new FileStream(filename,               // Вывод в файл
                FileMode.Open, FileAccess.Read)))
            {
                json = srd.ReadToEnd();                                   // Считываем весь файл в строку
                srd.Close();                                                         // Закрываем поток считывания
            }
            return JsonConvert.DeserializeObject<Settings>(json);                   // Возвращаем строку с данными

        }
    }
}
