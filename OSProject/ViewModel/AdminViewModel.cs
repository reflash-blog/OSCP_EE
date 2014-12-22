using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Framework.ComponentModel;
using Framework.ComponentModel.Rules;

namespace OSProject.ViewModel
{
    class AdminViewModel : NotifyDataErrorInfo<AdminViewModel>
    {
        public AdminViewModel()
        {
            _date = DateTime.Today;
        }

        static AdminViewModel()
        {
            Rules.Add(new DelegateRule<AdminViewModel>(
                "Login",
                "Логин должен содержать минимум 6 символов",
                x => !string.IsNullOrWhiteSpace(x.Login) && x.Login.Length >= 5));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "Email",
                "E-mail должен соответствовать выражению xxxx@yyyy.zz",
                x => !string.IsNullOrWhiteSpace(x.Email) && Regex.Matches(x.Email,
                    @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                    RegexOptions.IgnoreCase).Count != 0));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "UserType",
                "Тип должен быть выбранным из списка",
                x => !string.IsNullOrWhiteSpace(x.UserType)));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "Name",
                "Имя не должно содержать цифр или пробелов",
                x => !string.IsNullOrWhiteSpace(x.Name) && Regex.Matches(x.Name,
                    @"^[a-zA-Zа-яА-Я]+$",
                    RegexOptions.IgnoreCase).Count != 0));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "SurName",
                "Фамилия не должна содержать цифр или пробелов",
                x => !string.IsNullOrWhiteSpace(x.SurName) && Regex.Matches(x.SurName,
                    @"^[a-zA-Zа-яА-Я]+$",
                    RegexOptions.IgnoreCase).Count != 0));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "Password",
                "Пароль должен быть длиной минимум 4 символа и не должен содержать знаков пробела, а также спецсимволов (^ $ &...)",
                x => !string.IsNullOrWhiteSpace(x.Password) && Regex.Matches(x.Password,
                    @"^[a-zA-Z0-9а-яА-Я]+$",
                    RegexOptions.IgnoreCase).Count != 0 && x.Password.Length >= 4));
            Rules.Add(new DelegateRule<AdminViewModel>(
                "DuplicatePassword",
                "Пароли должны совпадать",
                x => !string.IsNullOrWhiteSpace(x.DuplicatePassword) && x.DuplicatePassword == x.Password));
        }

        public ObservableCollection<string> TypeItems
        {
            get { return new ObservableCollection<string> {"Admin", "User"}; }
        }

        private string _login;
        private string _email;
        private string _userType;
        private string _name;
        private string _surName;
        private DateTime _date;
        private string _password;
        private string _duplicatePassword;

        public string Login
        {
            get { return _login; }
            set { this.SetProperty(ref _login, value); }
        }

        public string Email
        {
            get { return _email; }
            set { this.SetProperty(ref _email, value); }
        }
        public string UserType
        {
            get { return _userType; }
            set { this.SetProperty(ref _userType, value); }
        }
        public string Name
        {
            get { return _name; }
            set { this.SetProperty(ref _name, value); }
        }
        public string SurName
        {
            get { return _surName; }
            set { this.SetProperty(ref _surName, value); }
        }
        public DateTime Date
        {
            get { return _date; }
            set { this.SetProperty(ref _date, value); }
        }
        public string Password
        {
            get { return _password; }
            set { this.SetProperty(ref _password, value); }
        }
        public string DuplicatePassword
        {
            get { return _duplicatePassword; }
            set { this.SetProperty(ref _duplicatePassword, value); }
        }

        #region HelpCommand
        RelayCommand _helpCommand = null;
        public ICommand HelpCommand
        {
            get
            {
                if (_helpCommand == null)
                {
                    _helpCommand = new RelayCommand((p) => OnHelp(p), (p) => CanHelp(p));
                }

                return _helpCommand;
            }
        }

        private bool CanHelp(object parameter)
        {
            return File.Exists(@"help.chm");
        }

        private void OnHelp(object parameter)
        {
            Help();
        }

        public void Help()
        {
            Process.Start(@"help.chm");
        }

        #endregion 
    }
}
