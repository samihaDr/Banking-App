using BankApp.Model;
using PRBD_Framework;
using System;
namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StatementsView : UserControlBase {
        public StatementsView(InternalAccount ia) {
            InitializeComponent();
            vm.Init(ia);
            Register<DateTime>(App.Messages.MSG_CHANGE_CURRENT_DATE, date => {
                Console.WriteLine("STATEMANTSVIEW =>La date a bien été changeée : " + date); 
            });
            
        }

       
    }


}
