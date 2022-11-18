using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using BankApp.Model;
using System.Windows.Input;

namespace BankApp.ViewModel {

    class AccountChecked : ViewModelCommon {

        private Account _account;
        public Account Account {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private bool _isChecked;
        public bool IsChecked {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value, () => {
                NotifyColleagues(App.Messages.MSG_ACCOUNT_CHECKED);
            });
        }
        public AccountChecked(Account account, bool isChecked = false) {
            Account = account;
            IsChecked = isChecked;
        }

        public AccountChecked(Account account) {
            Account = account;
        }
    }
    class AdminViewModel : ViewModelCommon {

        public ICommand DeleteBtn { get; set; }
        public ICommand CancelBtn { get; set; }

        private ObservableCollectionFast<AccountChecked> _accounts;

        public ObservableCollectionFast<AccountChecked> Accounts {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        private List<Account> _accountsChecked;

        public List<Account> AccountsChecked {
            get => _accountsChecked;
            set => SetProperty(ref _accountsChecked, value);
        }
        protected override void OnRefreshData() {
            Console.WriteLine("Ja suis dans le Refrech ");
            RaisePropertyChanged(nameof(Accounts));
            AddCheckedAccount();
        }

        private ObservableCollectionFast<AccountChecked> GetAccounts() {
            var listOfAccounts = Account.GetAllAccounts().ToList();

            ObservableCollectionFast<AccountChecked> AccountsList = new ObservableCollectionFast<AccountChecked>();
            foreach (var account in listOfAccounts) {
                AccountsList.Add(new AccountChecked(account));
            }
            return AccountsList;
        }
        private void AddCheckedAccount() {
            foreach (var acc in Accounts) {

                if (acc.IsChecked) {
                    AccountsChecked.Add(acc.Account);
                    Console.WriteLine("Account added => " + acc.Account.Iban);
                }
            }
        }

        public AdminViewModel() {
            AccountsChecked = new List<Account>();

            Accounts = new ObservableCollectionFast<AccountChecked>(GetAccounts());

            DeleteBtn = new RelayCommand(DeleteAction, CanDeleteAction);
            CancelBtn = new RelayCommand(CancelAction, CanCancelAction);

            Register(App.Messages.MSG_ACCOUNT_CHECKED, OnRefreshData);

            foreach (var acc in Accounts) {
                Console.WriteLine("Account added dans constructor => " + acc.Account.Iban + "   isCheck   " + acc.IsChecked);
            }
        }

        private bool CanDeleteAction() {
            if (AccountsChecked.Count != 0)
                return true;
            else 
                return false;
        }

        private void DeleteAction() {
            if (AccountsChecked.Count() != 0) {
                foreach (var acc in Accounts) {
                    Console.WriteLine("Account to delete => " + acc.Account.Iban);
                    if (acc.IsChecked) {
                        Console.WriteLine("Account to delete is checked => " + acc.Account.Iban);
                        var transfers = Context.Transfers.Where(tr => tr.DebitAccount.Iban == acc.Account.Iban || tr.CreditAccount.Iban == acc.Account.Iban).ToList();
                        Console.WriteLine("Liste de transfers =>" + transfers.Count());
                        if (transfers.Count() != 0) {
                            foreach (var tr in transfers) {
                                Console.WriteLine("Tr to delete => " + tr.Description);
                                Context.Transfers.Remove(tr);
                                Context.SaveChanges();
                            }
                        }
                        var clientInternalAccounts = Context.ClientInternalAccounts.Where(cia => cia.InternalAccount.Iban == acc.Account.Iban).ToList();
                        Console.WriteLine("Liste CIA => " + clientInternalAccounts.Count());
                        foreach (var cia in clientInternalAccounts) {
                            Context.ClientInternalAccounts.Remove(cia);
                            Context.SaveChanges();
                        }
                        Context.Accounts.Remove(acc.Account);
                        Context.SaveChanges();
                        AccountsChecked.Remove(acc.Account);
                    }
                }
            }
            Accounts = new ObservableCollectionFast<AccountChecked>(GetAccounts());
            RaisePropertyChanged();
        }


        public override void CancelAction() {
            if (AccountsChecked.Count() != 0) {
                foreach (var acc in Accounts) {
                    acc.IsChecked = false;
                    AccountsChecked.Remove(acc.Account);
                }
                Console.WriteLine("Je suis dans le CancelAction");
                Console.WriteLine("ACCOUNTSCHECKED => " + AccountsChecked.Count());
                //OnRefreshData();
            }
            
            //NotifyColleagues(App.Messages.MSG_CLOSE_TAB);
        }
        private bool CanCancelAction() {
            if (AccountsChecked.Count() != 0)
                return true;
            else
                return false;
        }



    }
}
