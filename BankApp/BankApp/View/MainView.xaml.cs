using System;
using System.Windows.Controls;
using BankApp.Model;
using PRBD_Framework;

namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowBase {
        public MainView() {
            InitializeComponent();
           
            Register<Account>(App.Messages.MSG_NEW_TRANSFER, 
                (account) => DoDisplayTransfer(account));

            Register<Account>(App.Messages.MSG_FROM_ACCOUNT_CHANGED,
                account => DoRenameTab(string.IsNullOrEmpty(account.DebitAccounts.ToString()) ? "<New Account>" : account.DebitAccounts.ToString()));

            Register<InternalAccount>(App.Messages.MSG_DISPLAY_STATEMENTS,
                ia => DoDisplayStatements(ia));

            //Register<Account>(App.Messages.MSG_CLOSE_TAB,
            //   (account) => DoCloseTab(account));

        }

        private void DoDisplayStatements(InternalAccount ia) {
            if(ia != null) {
                OpenTab(ia.Iban, ia.Iban + "Statement", () => new StatementsView(ia));
            }
        }

        private void DoDisplayTransfer(Account account) {
               OpenTab("<New Transfer>", account.Iban, () => new TransferView(account));
        }
        private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }

        private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
            var tab = tabControl.FindByTag(tag);
            if (tab == null)
                tabControl.Add(createView(), header, tag);
            else
                tabControl.SetFocus(tab);
        }

        private void DoRenameTab(string header) {
            if (tabControl.SelectedItem is TabItem tab) {
                tabControl.RenameTab(tab, header);
                tab.Tag = header;
            }
        }
        //private void DoCloseTab(Account account) {
        //    tabControl.CloseByTag(string.IsNullOrEmpty(account.Iban) ? "<New Transfer>" : account.Iban);
        //}
      

    }
}
