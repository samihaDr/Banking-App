using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
   public class SavingAccount : InternalAccount {
        public SavingAccount() {

        }
        public SavingAccount(string iban, string description, double floor = 0) : base(iban, description, floor) {
            AccountType = AccountType.Saving;
        }

    }
}
