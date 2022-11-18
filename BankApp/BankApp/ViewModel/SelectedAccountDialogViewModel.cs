using PRBD_Framework;
using BankApp.Model;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace BankApp.ViewModel {
    public class SelectedAccountDialogViewModel : DialogViewModelBase<User, BankContext> {
        public ICommand Ok { get; set; }
        public ICommand Cancel { get; set; }
        // le compte exclu de la liste de MyAccounts
        private Account _accountToExclude;
        public Account AccountToExclude {
            get => _accountToExclude;
            set => SetProperty(ref _accountToExclude, value);
        }

        private Account _accountSelected;
        public Account AccountSelected {
            get => _accountSelected;
            set => SetProperty(ref _accountSelected, value);
        }
        private ObservableCollection<Account> _otherAccounts;
        public ObservableCollection<Account> OtherAccounts {
            get => _otherAccounts;
            set => SetProperty(ref _otherAccounts, value);
        }

        private ObservableCollection<Account> _myOtherAccounts;
        public ObservableCollection<Account> MyOtherAccounts {
            get => _myOtherAccounts;
            set => SetProperty(ref _myOtherAccounts, value);
        }
        private Account _accountSelectedFromTransfer;
        public Account AccountSelectedFromTransfer {
            get => _accountSelectedFromTransfer;
            set => SetProperty(ref _accountSelectedFromTransfer, value);
        }
        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }
        private ObservableCollection<Account> _filterdAccounts;
        public ObservableCollection<Account> FilteredAccounts {
            get => _filterdAccounts;
            set => SetProperty(ref _filterdAccounts, value);
        }
        public SelectedAccountDialogViewModel() {
            Ok = new RelayCommand<Account>((account) => {
                DialogResult = true;
                NotifyColleagues(App.Messages.MSG_ACCOUNT_SELECTED, account);
                });


            Cancel = new RelayCommand(() => DialogResult = false);

            
            //OnRefreshData();

        }
        protected override void OnRefreshData() {
            //IQueryable<Account> AccountsFilteredList = Account.GetFilteredAccounts(Filter, CurrentUser);
           
            IQueryable<Account> OthersAccountsFilteredList = string.IsNullOrEmpty(Filter) ? Account.GetAllAccountsExceptOurs(CurrentUser) : Account.GetOhersAccountsFiltered(Filter, CurrentUser);
            this.OtherAccounts = new ObservableCollection<Account>(OthersAccountsFilteredList);
            IQueryable<Account> MyAccountsFilteredList = string.IsNullOrEmpty(Filter) ? ClientInternalAccount.GetAllAccountsByClientExceptFromAccount(CurrentUser, AccountSelectedFromTransfer) : Account.GetMyAccountsFiltered(Filter, CurrentUser, AccountSelectedFromTransfer);
            this.MyOtherAccounts = new ObservableCollection<Account>(MyAccountsFilteredList);
        }

        public void Init(Account accountToExclude) {
            AccountSelectedFromTransfer = accountToExclude;
            System.Console.WriteLine( accountToExclude.Iban);
            
            IQueryable<Account> MyOtherAccountsList = ClientInternalAccount.GetAllAccountsByClientExceptFromAccount(CurrentUser, accountToExclude);
            this.MyOtherAccounts = new ObservableCollection<Account>(MyOtherAccountsList);
            if (accountToExclude is CurrentAccount) {
                IQueryable<Account> OtherAccountsList = Account.GetAllAccountsExceptOurs(CurrentUser);
                this.OtherAccounts = new ObservableCollection<Account>(OtherAccountsList);
            }
            else {
                this.OtherAccounts = null;
            }

        }
    }
}
