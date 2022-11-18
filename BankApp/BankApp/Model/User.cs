using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace BankApp.Model {
    public abstract class User : EntityBase<BankContext> {
        public User() {
        }

        public User(string firstName, string lastName, string email, string Password) {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            this.Password = Password;
        }
        [Key]
        public int UserId { get; set; }

        private string _lastName;
        [Required]
        public string LastName{
            get { return _lastName; }
            set { _lastName = value.ToLower(); }
        }

        private string _firstName;

        public string FirstName {
            get { return _firstName; }
            set { _firstName = value.ToLower(); }
        }

        [Required]
        private string _email;

        public string Email {
            get { return _email; }
            set { _email = value; } 
        }

        private string _password;

       
        [Required]
        public string Password{
            get { return _password; }
            set { _password = value; }// utiliser un pattern MDP
        }
        public virtual ICollection<Transfer> Trasfers { get; set; } = new HashSet<Transfer>();
        public string NameClient => $"{_lastName}  {_firstName}";

        public static User GetUserByEmail(string Email) {
            User query = Context.Users.FirstOrDefault(user => user.Email == Email);
            return query;
        }
    }
}
