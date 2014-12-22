using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using OSProject.Controller.DataBase;
using OSProject.View;

namespace OSProject.ViewModel
{
    class LoginViewModel:ViewModelBase
    {

        public string Login { get; set; }
        public string Password { get; set; }

        #region LoginCommand
        RelayCommand _loginCommand = null;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand((p) => OnLogin(p), (p) => CanLogin(p));
                }

                return _loginCommand;
            }
        }

        private bool CanLogin(object parameter)
        {
            return Login!=string.Empty&&Password!=string.Empty;
        }

        private void OnLogin(object parameter)
        {
            LoginTask(parameter as LoginWindow);
        }

        public void LoginTask(LoginWindow window)
        {
            if(window==null) return;
            using (var userRepository = new UserRepository())
            {
                var user = userRepository.GetUser(Login);
                if (user == null || !user.password.Equals(CypherPassword(Password))) return;
                switch (user.userInfo.UserType)
                {
                    case "Admin":
                        (new AdminWindow()).Show();
                        window.Close();
                        break;
                    case "User":
                        (new UserWindow()).Show();
                        window.Close();
                        break;
                }
            }
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
                result[i] = (char)(charAArray[i] ^ charBArray[i]);
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

        #endregion
    }
}
