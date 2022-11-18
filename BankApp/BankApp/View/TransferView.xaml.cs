using BankApp.Model;
using PRBD_Framework;
using System;
namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TransferView : UserControlBase {
        public TransferView(Account account) {
            InitializeComponent();
            vm.Init(account);

            Register<DateTime>(App.Messages.MSG_CHANGE_CURRENT_DATE, date => {

                Console.WriteLine("TRANSFERVIEW => La date a été changée : " + date);
            }); 

        }
      


    }


}

