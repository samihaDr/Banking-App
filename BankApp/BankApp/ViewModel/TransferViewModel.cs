using BankApp.Model;
using PRBD_Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel {
    public class TransferViewModel : ViewModelCommon {
        public ICommand SaveBtn { get; set; }
        public ICommand CancelBtn { get; set; }
        public ICommand SelectedAccountBtn { get; set; }

        private Transfer _transfer;
        public Transfer Transfer {
            get => _transfer;
            set => SetProperty(ref _transfer, value);
        }

        /// <summary>
        /// la liste des comptes du client connecté
        /// </summary>
        private ObservableCollection<Account> _myAccounts;
        public ObservableCollection<Account> MyAccounts {
            get => _myAccounts;
            set => SetProperty(ref _myAccounts, value);
        }

        private ObservableCollection<Account> _myAccountsExceptFromAccount;
        public ObservableCollection<Account> MyAccountsExceptFromAccount {
            get => _myAccountsExceptFromAccount;
            set => SetProperty(ref _myAccountsExceptFromAccount, value);
        }


        private ObservableCollection<Category> _categoryList;
        public ObservableCollection<Category> CategoryList {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }
        /// <summary>
        /// Le compte selectionné dans la fenetre Dialog
        /// </summary>
        private Account _toSelectedAccount;
        public Account ToSelectedAccount {
            get => _toSelectedAccount;
            set => SetProperty(ref _toSelectedAccount, value);
        }

        /// <summary>
        /// le selectionné dans le comboBox
        /// </summary>
        private Account _fromSelectedAccount;
        public Account FromSelectedAccount {
            get => _fromSelectedAccount;
            set => SetProperty(ref _fromSelectedAccount, value);
        }


        private Account _to;
        public Account To {
            get => _to;
            set => SetProperty(ref _to, value, () => Validate());
        }
        private double _amount;
        public double Amount {
            get => _amount;
            set => SetProperty(ref _amount, value, () => Validate());
        }
        private string _description;
        public string Description {
            get => _description;
            set => SetProperty(ref _description, value , () => Validate());
        }
        private DateTime _creationDate = DateTime.Now;
        //public DateTime CreationDate {
        //    get => _creationDate;
        //    set => SetProperty(ref _creationDate, value);
        //}
        private DateTime? _effectDate;
        public DateTime? EffectDate {
            get => _effectDate;
            set => SetProperty(ref _effectDate, value, () => Validate());
        }

        private Category? _category;
        public Category Category {
            get => _category;
            set => SetProperty(ref _category, value);
        }
       
        public override bool Validate() {
            ClearErrors();

            if (ToSelectedAccount == null)
                AddError(nameof(ToSelectedAccount), "required");
            if (Amount == 0)
                AddError(nameof(Amount), "required");
            else if (Amount < 0)
                AddError(nameof(Amount), "The Amount must be positif");
            //Checker si le montant a transferer est superieur au solde
            else if (Amount > (FromSelectedAccount.Balance - FromSelectedAccount.FloorAccount) && EffectDate == null)
                AddError(nameof(Amount), "Maximum allowed transfer is " + (FromSelectedAccount.Balance - FromSelectedAccount.FloorAccount));
            if (EffectDate < App.CurrentDate)
                AddError(nameof(EffectDate), "EffectDate is not valid");
            if (string.IsNullOrEmpty(Description))
                AddError(nameof(Description), "required");
            return !HasErrors;
        }
        public override bool MayLeave => IsSavedAndValid;

        public bool IsSavedAndValid => !HasChanges;

        protected override void OnRefreshData() {
            //base.OnRefreshData();
            
        }


        public void Init(Account account) {
            FromSelectedAccount = account;
            System.Console.WriteLine("FromSelectedAccount : " + FromSelectedAccount.Iban);
            AddComputedProperties(nameof(IsSavedAndValid));
            RaisePropertyChanged();
        }
        public TransferViewModel() : base() {
            //OnRefreshData();
            SaveBtn = new RelayCommand(SaveAction,CanSaveAction);
            CancelBtn = new RelayCommand(CancelAction,CanCancelAction);


            IQueryable<Account> MyAccountsList = ClientInternalAccount.GetAccountsByClient(CurrentUser);
            this.MyAccounts = new ObservableCollection<Account>(MyAccountsList);

            IQueryable<Category> ListOfCategory = Category.GetAllCategory();
            this.CategoryList = new ObservableCollection<Category>(ListOfCategory);

            SelectedAccountBtn = new RelayCommand<Account>(account => {
               App.ShowDialog<SelectedAccountDialogViewModel, User, BankContext>(account);
            });

            Register<Account>(App.Messages.MSG_FROM_ACCOUNT_CHANGED, account => { 
                FromSelectedAccount = account;
                //vider la comboBox de ToSelectedAccount
            });
            Register<Account>(App.Messages.MSG_ACCOUNT_SELECTED, account => { ToSelectedAccount = account; });
          
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
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB,FromSelectedAccount);
            //NotifyColleagues(App.Messages.MSG_CLOSE_TAB);
        }

        public override void SaveAction() {
            Console.WriteLine();
            Console.WriteLine(EffectDate);
            Transfer transfer = new Transfer(
               FromSelectedAccount,
               ToSelectedAccount,
               CurrentUser,
               Amount,
               Description,
               App.CurrentDate,
               Category,
               EffectDate);
            Context.Transfers.Add(transfer);
            Context.SaveChanges();
            //NotifyColleagues(App.Messages.MSG_CLOSE_TAB);
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, FromSelectedAccount);
            NotifyColleagues(App.Messages.MSG_TRANSFER_SAVED);
        }
       
    }

}
