using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class CurrentAccount : InternalAccount {

        public CurrentAccount() { }

        public CurrentAccount(string iban, string description, double floor) : base(iban, description, floor) {
            
            AccountType = AccountType.Current;
        }
    }
   
}
