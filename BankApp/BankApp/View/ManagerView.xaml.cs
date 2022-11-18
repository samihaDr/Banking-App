using BankApp.Model;
using PRBD_Framework;
using System;
namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ManagerView : UserControlBase {
        public ManagerView() {
            InitializeComponent();

           // Register<Client>(App.Messages.MSG_NEW_CLIENT, client => DoDisplayClient(client, true));
        }

        //private void DoDisplayClient(Client client, bool isNew) {
        //    if(client != null) {

        //    }
        //}
    }

}
