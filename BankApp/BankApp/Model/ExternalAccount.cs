using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class ExternalAccount : Account {
       public ExternalAccount() { }
       public ExternalAccount(string iban, string description, double floorAccount=0) : base(iban, description, floorAccount) { }
    }
}
