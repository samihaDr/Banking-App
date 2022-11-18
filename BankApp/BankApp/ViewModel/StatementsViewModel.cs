using BankApp.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel {
    public class StatementsViewModel : ViewModelCommon {

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand CheckAllBtn { get; set; }
        public ICommand UnCheckAllBtn { get; set; }
        public ICommand CategoryChoosen { get; set; }

        List<string> PeriodItem = new List<string>() { "One Day", "One Week", "Two Weeks", "Four Weeks", "One Year", "All" };

        private bool _futureTransactionsSelected;

        public bool FutureTransactionsSelected {
            get => _futureTransactionsSelected;
            set => SetProperty(ref _futureTransactionsSelected, value, OnRefreshData);
        }
        private bool _pastTransactionsSelected;

        public bool PastTransactionsSelected {
            get => _pastTransactionsSelected;
            set => SetProperty(ref _pastTransactionsSelected, value, OnRefreshData);
        }

        private bool _refusedTransactionsSelected;

        public bool RefusedTransactionsSelected {
            get => _refusedTransactionsSelected;
            set => SetProperty(ref _refusedTransactionsSelected, value, OnRefreshData);
        }

        private InternalAccount _internalAccount;
        public InternalAccount InternalAccount {
            get => _internalAccount;
            set => SetProperty(ref _internalAccount, value);
        }
        private Transfer _transfer;
        public Transfer Transfer {
            get => _transfer;
            set => SetProperty(ref _transfer, value);
        }
        private ObservableCollection<Transfer> _transfers;
        public ObservableCollection<Transfer> Transfers {
            get => _transfers;
            set => SetProperty(ref _transfers, value);
        }

        private List<Transfer> _transfersList;
        public List<Transfer> TransfersList {
            get => _transfersList;
            set => SetProperty(ref _transfersList, value);
        }

        private ObservableCollection<string> _comboPeriod;
        public ObservableCollection<string> ComboPeriod {
            get => _comboPeriod;
            set => SetProperty(ref _comboPeriod, value);
        }

        private string _itemPeriodSelected;
        public string ItemPeriodSelected {
            get => _itemPeriodSelected;
            set => SetProperty(ref _itemPeriodSelected, value, OnRefreshData);
        }
        private ObservableCollectionFast<CheckedCategory> _checkCategoryList;
        public ObservableCollectionFast<CheckedCategory> CheckCategoryList {
            get => _checkCategoryList;
            set => SetProperty(ref _checkCategoryList, value, RaisePropertyChanged);
        }
        private ObservableCollection<Category> _categoryList;
        public ObservableCollection<Category> CategoryList {
            get => _categoryList;
            set => SetProperty(ref _categoryList, value);
        }

        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        private int ConvertStringItemPeriodToInt(String PeriodSelected) {
            int res = 0;
            switch (PeriodSelected) {
                case "One Day":
                    res = 1;
                    break;

                case "One Week":
                    res = 7;
                    break;

                case "Two Weeks":
                    res = 14;
                    break;

                case "Four Weeks":
                    res = 28;
                    break;

                case "One Year":
                    res = 365;
                    break;

                default:
                    break;
            }
            return res;
        }

        private DateTime CalculatePeriod(string selected) {
            int period = ConvertStringItemPeriodToInt(selected);
            DateTime limitDate = App.CurrentDate.AddDays(-period);
            return limitDate;
        }
        protected override void OnRefreshData() {
            Account.GetBalance();
            FilteredStatements();
            CheckCategoryList = new ObservableCollectionFast<CheckedCategory>(GetCheckedCategories());
            
        }

        public void Init(InternalAccount ia) {
            InternalAccount = ia;
            ItemPeriodSelected = ComboPeriod[ComboPeriod.Count - 1];
            System.Console.WriteLine("InternalAccount : " + ia.Iban);
            PastTransactionsSelected = true;
            RaisePropertyChanged();
        }

        public StatementsViewModel() : base() {

            CategoryList = new ObservableCollection<Category>(Category.GetAllCategory());
            ComboPeriod = new ObservableCollection<string>(PeriodItem);
            CheckCategoryList = new ObservableCollectionFast<CheckedCategory>(CheckedCategory.CheckedCategoriesList());
            UnCheckAllBtn = new RelayCommand(() => { UnCheckAll(); OnRefreshData(); });
            CheckAllBtn = new RelayCommand(() => { CheckAll(); OnRefreshData(); });

            Console.WriteLine("List items => " + ComboPeriod.Count());
            CategoryChoosen = new RelayCommand<Transfer>((tr) => { Context.SaveChanges(); });
            Register(App.Messages.MSG_TRANSFER_SAVED, () => OnRefreshData());
            Register<DateTime>(App.Messages.MSG_CHANGE_CURRENT_DATE, (date) => OnRefreshData());

        }
        private void CheckAll() {
            foreach (var cat in CheckCategoryList) {
                cat.IsChecked = true;
            }
        }
        private void UnCheckAll() {
            foreach (var cat in CheckCategoryList) {
                cat.IsChecked = false;
            }
        }
        private ObservableCollection<CheckedCategory> GetCheckedCategories() {
            return CheckCategoryList;
        }
        private void FilteredStatements() {
            Console.WriteLine("ItemPeriod => " + ItemPeriodSelected);
            Console.WriteLine("ConvertStringToInt => " + ConvertStringItemPeriodToInt(ItemPeriodSelected));
            Console.WriteLine("CalculatePeriod => " + CalculatePeriod(ItemPeriodSelected));

            List<Transfer> TransfersList = string.IsNullOrEmpty(Filter) ? InternalAccount.GetTransfersByAccount(InternalAccount) : InternalAccount.GetFilteredTransaction(Filter, InternalAccount);
            var TransactionsFilteredList = new List<Transfer>();
            if (PastTransactionsSelected) { 
                TransactionsFilteredList = TransfersList.Where(tr => tr.IsAcceptedTransaction).ToList();
            if (RefusedTransactionsSelected)
                TransactionsFilteredList = TransfersList;
            if (ItemPeriodSelected != "All")

                TransactionsFilteredList = TransfersList.Where(tr => tr.IsAcceptedTransaction && (tr.ApplyDate > CalculatePeriod(ItemPeriodSelected) && tr.ApplyDate <= App.CurrentDate)).ToList();
            if (FutureTransactionsSelected)
                TransactionsFilteredList = InternalAccount.GetFutureTransctionsByAccount(InternalAccount).Concat(TransactionsFilteredList).ToList();

        } else if (FutureTransactionsSelected) {
                TransactionsFilteredList = InternalAccount.GetFutureTransctionsByAccount(InternalAccount).ToList();
        RefusedTransactionsSelected = false;
            } else {
                RefusedTransactionsSelected = false;
            }

Transfers = new ObservableCollection<Transfer>(TransactionsFilteredList);

        }
    }
}
