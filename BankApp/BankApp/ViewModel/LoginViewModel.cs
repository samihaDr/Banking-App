using System;
using System.Linq;
using PRBD_Framework;
using BankApp.Model;
using System.Windows.Input;
using System.ComponentModel.DataAnnotations;

namespace BankApp.ViewModel {
    public class LoginViewModel : ViewModelCommon {
        public ICommand LoginCommand { get; set; }

        private string _email;
        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }

        private string _password;
        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }
        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => { return _email != null && _password != null && !HasErrors; });
        }

        private void LoginAction() {
            if (Validate()) {
                User user = User.GetUserByEmail(Email);
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }

        protected override void OnRefreshData() {
            
        }
        public override bool Validate() {
            ClearErrors();

            User user = User.GetUserByEmail(Email);

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (!validMail(Email))
                AddError(nameof(Email), "Email not valid");
            else if ( user == null)
                AddError(nameof(Email), "does not exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (user != null && user.Password != Password)
                    AddError(nameof(Password), "wrong password");
            }
            return !HasErrors;
        }
       
        private static bool validMail(string address) {
            EmailAddressAttribute e = new();
            if (e.IsValid(address))
                return true;
            else
                return false;
        }

       

    }
}
