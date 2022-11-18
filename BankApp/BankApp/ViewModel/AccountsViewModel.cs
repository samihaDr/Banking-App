using BankApp.Model;
using PRBD_Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel {
    class AccountsViewModel : ViewModelCommon {

        private ObservableCollection<ClientInternalAccount> _accounts;
        public ObservableCollection<ClientInternalAccount> Accounts {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }
       
        private bool _checkingSelected;
        public bool CheckingSelected {
            get => _checkingSelected;
            set => SetProperty(ref _checkingSelected, value, OnRefreshData);
        }

        private bool _savingSelected;
        public bool SavingSelected {
            get => _savingSelected;
            set => SetProperty(ref _savingSelected, value, OnRefreshData);
        }

        private bool _allSelected;
        public bool AllSelected {
            get => _allSelected;
            set => SetProperty(ref _allSelected, value, OnRefreshData);
        }

        public ICommand ClearFilter { get; set; }
        public ICommand Transfer { get; set; }
        public ICommand Statements { get; set; }


        public AccountsViewModel() : base() {

            Accounts = new ObservableCollection<ClientInternalAccount>(ClientInternalAccount.GetInternalAccountsByUser(CurrentUser));
            Console.WriteLine("user connecté : " + CurrentUser.NameClient);

            ClearFilter = new RelayCommand(() => Filter = ""); 

            Transfer = new RelayCommand<Account>((account)=> {
                NotifyColleagues(App.Messages.MSG_NEW_TRANSFER, account);
            });

            Statements = new RelayCommand<InternalAccount>(ia => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_STATEMENTS, ia);
            });

            AllSelected = true;

            Register<DateTime>(App.Messages.MSG_CHANGE_CURRENT_DATE, date => OnRefreshData());
            Register(App.Messages.MSG_TRANSFER_SAVED, () => OnRefreshData());
        }

        protected override void OnRefreshData() {
            //Calcule le solde a chaqur Refresh
            Account.GetBalance();

            //On  ne peut pas avoir 2 boutons checked
            if ((_checkingSelected && _savingSelected) ||
                (_checkingSelected && _allSelected) ||
                (_savingSelected && _allSelected))
                return;

            IQueryable<ClientInternalAccount> Accounts = string.IsNullOrEmpty(Filter) ? ClientInternalAccount.GetInternalAccountsByUser(CurrentUser) : ClientInternalAccount.GetFiltered(Filter, CurrentUser);
            var filteredAccounts = from cci in Accounts
                                   where
                                   CheckingSelected && cci.InternalAccount.AccountType == AccountType.Current ||
                                   SavingSelected && cci.InternalAccount.AccountType == AccountType.Saving ||
                                   AllSelected
                                   select cci;

            this.Accounts = new ObservableCollection<ClientInternalAccount>(filteredAccounts);

        }

      
    }

}
