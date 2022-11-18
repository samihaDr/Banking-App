using BankApp.Model;
using PRBD_Framework;
using System.Windows.Input;

namespace BankApp.ViewModel {
    public class ManagerClientViewModel : ViewModelCommon {

       
        public string Firstname {
            get => ClientConsulted?.FirstName;
            set => SetProperty(ClientConsulted.FirstName, value, ClientConsulted, (cli, v) => {
                cli.FirstName = v;
            });
        }
        public string LastName {
            get => ClientConsulted?.LastName;
            set => SetProperty(ClientConsulted.LastName, value, ClientConsulted, (cli, v) => {
                cli.LastName = v;
            });
        }
        public string Email {
            get => ClientConsulted?.Email;
            set => SetProperty(ClientConsulted.Email, value, ClientConsulted, (cli, v) => {
                cli.Email = v;
            });
        }
        public string Password {
            get => ClientConsulted?.Password;
            set => SetProperty(ClientConsulted.Password, value, ClientConsulted, (cli, v) => {
                cli.Password = v;
            });
        }
        private string _confirmPassword;
        public string ConfirmPassword {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
      
        private Client _clientConsulted;
        public Client ClientConsulted {
            get => _clientConsulted;
            set => SetProperty(ref _clientConsulted, value, OnRefreshData);
        }
        
        public ManagerClientViewModel() {
           
        }
        protected override void OnRefreshData() {
            
            RaisePropertyChanged();
        }
       
      
    }

}
