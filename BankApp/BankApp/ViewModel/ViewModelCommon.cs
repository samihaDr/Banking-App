using BankApp.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.ViewModel {
    public abstract class ViewModelCommon : ViewModelBase<User, BankContext> {

        public static bool IsManager => App.IsLoggedIn && App.CurrentUser is Manager;
        public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Admin;
        public static bool IsClient => App.IsLoggedIn && App.CurrentUser is Client;
    }
}
