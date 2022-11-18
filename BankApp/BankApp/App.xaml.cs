using System.Windows;
using PRBD_Framework;
using BankApp.Model;
using BankApp.ViewModel;
using System;

namespace BankApp {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ApplicationBase<User, BankContext> {
        public enum Messages {
            MSG_NEW_TRANSFER,
            MSG_DISPLAY_STATEMENTS,
            MSG_DISPLAY_TRANSFER,
            MSG_CLOSE_TAB,
            MSG_LOGIN,
            MSG_LOGOUT,
            MSG_CHANGE_CURRENT_DATE,
            MSG_ACCOUNT_SELECTED,
            MSG_FROM_ACCOUNT_CHANGED,
            MSG_TRANSFER_SAVED,
            MSG_AGENCY_CHANGED,
            MSG_NEW_CLIENT,
            MSG_ACCOUNT_CHECKED,
            MSG_REFRESH_DATA
        }
        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.SeedData();

            ClearContext();

            Register<User>(this, Messages.MSG_LOGIN, member => {
                Login(member);
                NavigateTo<MainViewModel, User, BankContext>();
            });

            Register(this, Messages.MSG_LOGOUT, () => {
                Logout();
                NavigateTo<LoginViewModel, User, BankContext>();
            });
           
            
        }

        private static DateTime _currentDate = DateTime.Now;
        public static DateTime CurrentDate {
            get => _currentDate;
            set {
                _currentDate = value;
                NotifyColleagues(Messages.MSG_CHANGE_CURRENT_DATE, value);
                Account.GetBalance();
            }
        }

        protected override void OnRefreshData() {
            // pour plus tard
            if (CurrentUser?.Email != null)
                CurrentUser = User.GetUserByEmail(CurrentUser.Email);
        }

    }
}
