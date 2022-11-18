using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class Transfer : EntityBase<BankContext> {
        public Transfer(User currentUser) { }
        public Transfer() { }
        public Transfer(Account debitAccount, Account creditAccount, User user, double amount, string description,DateTime creationDate, Category category = null, DateTime? effectiveDate = null) {
            User = user;
            Amount = amount;
            DebitAccount = debitAccount;
            CreationDate = creationDate;
            CreditAccount = creditAccount;
            Description = description;
            EffectiveDate = effectiveDate;
            Category = category;
            ApplyDate = EffectiveDate ?? creationDate;
        }
        [Key]
        public int TransferId { get; set; }
        [Required]

        private double _amount;
        public double Amount {
            get { return _amount; }
            set { _amount = value; }
        }
        [NotMapped]
        public double Balance { get; set; }
        [NotMapped]
        public string transferColor { get; set; }
        [NotMapped]
        public Boolean IsAcceptedTransaction {
            get; set;
        }
        //[Required]
        public string Description { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public DateTime CreationDate { get; set; }
        //[Required]
        public DateTime ApplyDate { get; set; }
        public virtual User User { get; set; }

        public virtual Category Category { get; set; }

        //[Required]
        [InverseProperty(nameof(Account.DebitAccounts))]
        public virtual Account DebitAccount { get; set; }

        [Required]
        [InverseProperty(nameof(Account.CreditAccounts))]
        public virtual Account CreditAccount { get; set; }
    }
}
