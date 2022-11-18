using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRBD_Framework;


namespace BankApp.Model {
    public abstract class Account : EntityBase<BankContext> {
        public Account() { }
        public Account(string iban, string description, double floorAccount) {
            Iban = iban;
            Description = description;
            FloorAccount = floorAccount;
        }
        [Key]
        public int AccountId { get; set; }
        [Required]
        public string Iban { get; set; }
        
        public string Description { get; set; }
        [InverseProperty(nameof(Transfer.DebitAccount))]
        public virtual ICollection<Transfer> DebitAccounts { get; set; } = new HashSet<Transfer>();

        [InverseProperty(nameof(Transfer.CreditAccount))]
        public virtual ICollection<Transfer> CreditAccounts { get; set; } = new HashSet<Transfer>();
        [NotMapped]
        public double Balance { get; set; }
        public double FloorAccount { get; set; }
        /// <summary>
        /// Obtenir tous les comptes
        /// 
        /// </summary>
        /// <returns></returns>
        public static IQueryable<Account> GetAllAccounts() {
            var otherAccounts = from account in Context.Accounts
                                select account;
            Console.WriteLine("GetAllAccounts" + otherAccounts.Count());
            return otherAccounts.Distinct();
        }

        /// <summary>
        /// Requete qui filtre sur tous les autres comptes internes et externes 
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public static IQueryable<Account> GetOhersAccountsFiltered(string Filter, User user) {
            var allAccounts = GetAllAccountsExceptOurs(user).Where(acc => (acc.Iban.Contains(Filter) || acc.Description.Contains(Filter)));
            return allAccounts;

        }
        /// <summary>
        /// filtre sur les comptes du client
        /// </summary>
        /// <param name="Filter"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IQueryable<Account> GetMyAccountsFiltered(string Filter, User user, Account account) {
            var myAccounts = ClientInternalAccount.GetAllAccountsByClientExceptFromAccount(user, account).Where(acc => (acc.Iban.Contains(Filter) || acc.Description.Contains(Filter)));
            return myAccounts;
        }

        /// <summary>
        /// Requete qui renvoie tous les autres comptes sauf lceux de User connecté
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IQueryable<Account> GetAllAccountsExceptOurs(User user) {
            IQueryable<Account> query = null;
            var query1 = ClientInternalAccount.GetAccountsByClient(user).ToList();
            var query2 = GetAllAccounts().ToList();
            query = query2.AsQueryable().Except(query1);

            return query;
        }

        public static void GetBalance() {
            var AllTransfers = Context.Transfers.OrderBy(tr => tr.ApplyDate).ToList();
            var AllInternalAccounts = Context.InternalAccounts;

            foreach (var ai in AllInternalAccounts) {
                ai.Balance = 0.0;
            }

            foreach (var tr in AllTransfers) {
                if (tr.ApplyDate <= App.CurrentDate) {
                    if (tr.DebitAccount is InternalAccount && tr.CreditAccount is InternalAccount && tr.DebitAccount.Balance - tr.Amount >= tr.DebitAccount.FloorAccount) {
                        tr.IsAcceptedTransaction = true;
                        tr.transferColor = "Green";
                        tr.DebitAccount.Balance -= tr.Amount;
                        tr.CreditAccount.Balance += tr.Amount;
                    } else if (tr.DebitAccount is ExternalAccount) {
                        tr.CreditAccount.Balance += tr.Amount;
                    } else if (tr.CreditAccount is ExternalAccount) {
                        tr.DebitAccount.Balance -= tr.Amount;
                    } else {
                        tr.IsAcceptedTransaction = false;
                        tr.transferColor = "Red";
                    }
                }else {
                    tr.transferColor = "#428af5";
                }
            }
            foreach (var ai in AllInternalAccounts) {
                Console.WriteLine("nouvelle balance: " + ai.Balance);
            }
        }
    }


}
