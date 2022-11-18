using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using BankApp.Model;
using System.Windows.Input;

namespace BankApp.ViewModel {
    //public event Action<Transfer> CloseTab;
    public class MainViewModel : ViewModelCommon {
        public ICommand ReloadDataCommand { get; set; }
        public event Action CloseTab;
        public MainViewModel() : base() {
            ReloadDataCommand = new RelayCommand(() => {
                // refuser un reload s'il y a des changements en cours
                if (Context.ChangeTracker.HasChanges()) return;
                // permet de renouveller le contexte EF  
                App.ClearContext();
                // notifie tout le monde qu'il faut rafraîchir les données
                NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            });
           // Register(App.Messages.MSG_CLOSE_TAB, () => CloseTab?.Invoke());

        }

        public static string Title {
            get => $"My Bank ({CurrentUser?.NameClient})";
        }
        protected override void OnRefreshData() {
        }
    }
}
