using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using BankApp.Model;

namespace BankApp.Model {
    public class Client : User{

       [Required]
       public int ClientNum { get; set; }
        public Client() { }
        public Client(string firstName, string lastName, string email, string password, Agency agency) : base(lastName, firstName, email, password) {
            Agency = agency;
        }
        
        
        public virtual Agency Agency { get; set; }
        public virtual ICollection<ClientInternalAccount> Accounts { get; set; } = new HashSet<ClientInternalAccount>();
        
        public string NumClient => $"{ClientNum}";
    }
}

