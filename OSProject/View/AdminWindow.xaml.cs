using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Framework.UI.Controls;
using Microsoft.Win32;
using OSProject.Controller.DataBase;
using OSProject.Model.Structures;
using OSProject.ViewModel;

namespace OSProject.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AdminWindow
    {
        public AdminWindow()
        {
            InitializeComponent();
            this.DataContext = new AdminViewModel();
        }

        private void AdminWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            using (var userRepository = new UserRepository())
            {

                foreach (var user in userRepository.GetUsers())
                {
                    AddUserWizard(user);
                }
            }
            UpdateBindings();

            Notify(
                "Уведомление",
                "Режим администратора загружен",
                new TimeSpan(30000000));
        }

        

        private void aboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Notify(
                "О программе",
                "Программа по операционным системам",
                new TimeSpan(300000000));
        }

        private void AddUserButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as AdminViewModel;
            if(context == null) return;
            if (!CanExecute())
            {
                Notify("Программа", "Все поля должны быть заполнены верно", new TimeSpan(30000000));
                return;
            }
            var userInfo = new UserInfo
            {
                Email = context.Email,
                Date = context.Date.Date.ToString("g"),
                Name = context.Name,
                SurName = context.SurName,
                UserType = context.UserType
            };
            var user = new User { login = context.Login, password = CypherPassword(context.Password),userInfo = userInfo};
            using (var userRepository = new UserRepository())
            {
                var result = userRepository.AddUser(user);
                if (result)
                {
                    Notify("База данных", "Пользователь " + user.login + " успешно добавлен", new TimeSpan(300000000));
                    AddUserWizard(user);
                    UpdateBindings();
                }
                else
                {
                    Notify("База данных", "Ошибка добавления пользователя " + user.login, new TimeSpan(300000000));  
                }
            }
            
        }



        
        #region Extensions

        private void AddUserWizard(User user)
        {
            var stackpanel = new StackPanel();
            var itemsControl = new ItemsControl();

            stackpanel.Children.Add(new TextBlock { Text = user.userInfo.SurName + " " + user.userInfo.Name, FontSize = 16, FontWeight = FontWeights.SemiBold });
            stackpanel.Children.Add(new Separator());

            var itemStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            itemStackPanel.Children.Add(new TextBlock { Text = "Логин:    ", FontWeight = FontWeights.SemiBold, Width = 100 });
            itemStackPanel.Children.Add(new TextBlock { Name = "Login",Text = user.login });
            itemsControl.Items.Add(itemStackPanel);
            itemStackPanel = new StackPanel {Orientation = Orientation.Horizontal};
            itemStackPanel.Children.Add(new TextBlock { Text = "E-mail:    ", FontWeight = FontWeights.SemiBold, Width = 100 });
            itemStackPanel.Children.Add(new TextBlock { Text = user.userInfo.Email });
            itemsControl.Items.Add(itemStackPanel);
            itemStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            itemStackPanel.Children.Add(new TextBlock { Text = "Тип:    ", FontWeight = FontWeights.SemiBold, Width = 100 });
            itemStackPanel.Children.Add(new TextBlock { Text = user.userInfo.UserType });
            itemsControl.Items.Add(itemStackPanel);
            itemStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            itemStackPanel.Children.Add(new TextBlock { Text = "Дата рождения:    ", FontWeight = FontWeights.SemiBold,Width = 100});
            itemStackPanel.Children.Add(new TextBlock { Text = user.userInfo.Date });
            itemsControl.Items.Add(itemStackPanel);
            stackpanel.Children.Add(itemsControl);
            var button = new Button{Content = "Удалить",Width = 200,Height = 30,HorizontalAlignment = HorizontalAlignment.Right};
            button.Click += DeleteUser;
            stackpanel.Children.Add(button);
            var item = new WizardItem
            {
                Content = stackpanel,
                ContentTemplate = null,
                Description = "Дата рождения " + user.userInfo.Date,
                Icon = this.FindResource("User1Geometry"),
                Id = user.login,
                ParentId = user.userInfo.UserType,
                Title = user.userInfo.SurName + " " + user.userInfo.Name
            };
            this.Wizard.Items.Add(item);

        }

        private void DeleteUser(object sender,
            EventArgs e)
        {
            var button = sender as Control;
            if (button == null) return;
            var stackPanel = button.Parent;
            var textBlock = stackPanel.FindVisualChild<TextBlock>("Login");
            using (var userRepository = new UserRepository())
            {
                var user = userRepository.GetUser(textBlock.Text);
                var result = userRepository.RemoveUser(user);
                if (result)
                {
                    Notify("База данных", "Пользователь " + user.login + " успешно удален", new TimeSpan(300000000));
                }
                else
                {
                    Notify("База данных", "Ошибка удаления пользователя " + textBlock.Text, new TimeSpan(300000000));   // User is null
                }
            }
        }

        private bool CanExecute()
        {
            var context = this.DataContext as AdminViewModel;
            return context != null && IsValid(this);
        }

        private bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
                LogicalTreeHelper.GetChildren(obj)
                .OfType<DependencyObject>()
                .All(IsValid);
        }

        private string CypherPassword(string password)
        {
            var hklm = Registry.CurrentUser.OpenSubKey("SOFTWARE\\OS", true);
            if (hklm == null) return "";
            //kQW4Ggp9ZxzBAXNGcZ3eBoAhExrTxgAKuSjUmF7MLJYv5LINgiNNdQcIGaNa9nu
            var key = (string)hklm.GetValue("Key");
            return XorWithKey(HashPasswordSha256(password), key);
        }
        private string XorWithKey(string password, string key)
        {
            var charAArray = password.ToCharArray();
            var charBArray = key.ToCharArray();
            var result = new char[password.Length];
            var len = 0;

            // Set length to be the length of the shorter string
            if (password.Length > key.Length)
                len = key.Length - 1;
            else
                len = password.Length - 1;

            for (var i = 0; i < len; i++)
            {
                result[i] = (char) (charAArray[i] ^ charBArray[i]);
            }

            return new string(result);
        }
        private string HashPasswordSha256(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + String.Format("{0:x2}", x));
        }

        private void Notify(string title, string message, TimeSpan timeTicks)
        {
            NotifyBox.Show(
                (DrawingImage)this.FindResource("FolderDrawingImage"),
                title,
                message,
                timeTicks);
        }

        private void UpdateBindings()
        {
            this.DataContext = new AdminViewModel();
        }
        #endregion

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsWindow();
            settingsDialog.ShowDialog();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.Show();
            Close();
        }
    }
}
