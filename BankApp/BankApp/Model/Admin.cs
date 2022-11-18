using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class Admin : User {
      public Admin() { }
        public Admin(string lastName, string firstName, string email, string password) : base(lastName, firstName, email, password) {
        }
    }
}
