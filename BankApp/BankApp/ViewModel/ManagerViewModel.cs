using BankApp.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel {
    public class ManagerViewModel : ViewModelCommon {
        public ICommand NewClient { get; set; }
        public ICommand SaveBtn { get; set; }
        public ICommand CancelBtn { get; set; }
        public ICommand DeleteBtn { get; set; }
        public ICommand SelectedAgencyCmb{ get; set; }
        public ManagerClientViewModel ManagerClientData { get; set; } = new ManagerClientViewModel();
       
            
        private Agency _selectedAgency;
        public Agency SelectedAgency {
            get => _selectedAgency;
            set => SetProperty(ref _selectedAgency, value, () => OnRefreshData());
        }

        private Client _clientConsulted;
        public Client ClientConsulted {
            get => _clientConsulted;
            set => SetProperty(ref _clientConsulted, value,() => { ManagerClientData.ClientConsulted = value; RaisePropertyChanged(); });
        }
        private Agency _agency;
        public Agency Agency {
            get => _agency;
            set => SetProperty(ref _agency, value);
        }
        private ObservableCollection<Agency> _agencies;
        public ObservableCollection<Agency> Agencies {
            get => _agencies;
            set => SetProperty(ref _agencies, value);
        }

        private ObservableCollection<Client> _clientsList;
        public ObservableCollection<Client> ClientsList {
            get => _clientsList;
            set => SetProperty(ref _clientsList, value);
        }

        protected override void OnRefreshData() {
           
            IQueryable<Client> ClientsListByAgency = Agency.GetClientsByAgency(SelectedAgency);
            this.ClientsList = new ObservableCollection<Client>(ClientsListByAgency);

            SelectedAgencyCmb = new RelayCommand<Agency>(agency => {
                Agency.GetClientsByAgency(agency);
            });
            
            Register<Agency>(App.Messages.MSG_AGENCY_CHANGED, agency => {
                SelectedAgency = agency;
            });
            RaisePropertyChanged();
        }

        public ManagerViewModel() : base() {
            
            IQueryable<Agency> ListOfAgencies = Manager.GetAgenciesByManager(CurrentUser);
            this.Agencies = new ObservableCollection<Agency>(ListOfAgencies);

           // NewClient = new RelayCommand(() => { NotifyColleagues(App.Messages.MSG_NEW_CLIENT, new Client()); });
            SaveBtn = new RelayCommand(SaveAction,CanSaveAction);
            CancelBtn = new RelayCommand(CancelAction,CanCancelAction);
            DeleteBtn = new RelayCommand(Delete);

            
        }

        private void Delete() {
            throw new NotImplementedException();
        }

        private bool CanCancelAction() {
            //Console.WriteLine("Je suis dans le CanCAncelAction");
            return true;
        }

        private bool CanSaveAction() {
            return Validate();
           
        }
            
        public override void CancelAction() {
            ClearErrors();
            //NotifyColleagues(App.Messages.MSG_CLOSE_TAB,FromSelectedAccount);
        }

        public override void SaveAction() {
          
            Context.SaveChanges();
            
        }
       
    }

}
