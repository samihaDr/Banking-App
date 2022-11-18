using PRBD_Framework;
using System;
namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AccountsView : UserControlBase {
        public AccountsView() {
            InitializeComponent();
            Register<DateTime>(App.Messages.MSG_CHANGE_CURRENT_DATE, date => {
                Console.WriteLine(" ACCOUNTVIEW => La date a été changée : " + date);
            });
        }

       
    }
}
