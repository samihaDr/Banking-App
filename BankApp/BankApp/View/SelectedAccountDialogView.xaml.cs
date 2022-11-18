using BankApp.Model;
using PRBD_Framework;
using System;
namespace BankApp.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectedAccountDialogView : DialogWindowBase {
        public SelectedAccountDialogView(Account account) {
            InitializeComponent();
            vm.Init(account);
        }
        private void DialogWindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // pour ne pas permettre de fermer la fenêtre => obligé de cliquer sur un des 2 boutons
            if (vm.DialogResult == null)
                e.Cancel = true;
        }


    }

}
