using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public enum AccountType {
        Current,
        Saving,
    }
    public abstract class InternalAccount : Account {
        public InternalAccount() : base() { }
        public InternalAccount(string iban, string decription, double floorAccount) : base(iban, decription, floorAccount) {
        }

        public AccountType AccountType { get; set; }
        public virtual ICollection<ClientInternalAccount> Accounts { get; set; } = new HashSet<ClientInternalAccount>();

        /// <summary>
        /// revoie tous le transfers passés et qui ne sont pas refusés
        /// </summary>
        /// <param name="ia"></param>
        /// <returns></returns>
        public static List<Transfer> GetTransfersByAccount(InternalAccount ia) {
            var query = Context.Transfers.AsEnumerable()
               .Where(tr => (tr.DebitAccount.Iban.Equals(ia.Iban) || tr.CreditAccount.Iban.Equals(ia.Iban))
                 && tr.ApplyDate <= App.CurrentDate).OrderByDescending(tr => tr.ApplyDate).ToList();

            return query;
        }

        /// <summary>
        /// Renvoie les transactions futeres par compte
        /// </summary>
        /// <param name="ia"></param>
        /// <returns></returns>
        public static List<Transfer> GetFutureTransctionsByAccount(InternalAccount ia) {
            var query = Context.Transfers.AsEnumerable()
                .Where(tr => tr.DebitAccount.Iban.Equals(ia.Iban)
                && tr.ApplyDate > App.CurrentDate).OrderByDescending(tr => tr.ApplyDate).ToList();
            Console.WriteLine("FutureTransactions => " + query.Count());
            return query;
        }

        /// <summary>
        /// Methode qui filtre sur le nom,l'Iban,la description .....
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="ia"></param>
        /// <returns></returns>
        public static List<Transfer> GetFilteredTransaction(string filter, InternalAccount ia) {
            var query = Context.Transfers.AsEnumerable()
                .Where(tr => (tr.DebitAccount.Iban.Equals(ia.Iban) || tr.CreditAccount.Iban.Equals(ia.Iban)) && (tr.ApplyDate < App.CurrentDate)
                && (tr.DebitAccount.Iban.Contains(filter)
                || tr.CreditAccount.Iban.Contains(filter)
                || tr.Description.Contains(filter)
                || tr.DebitAccount.Description.Contains(filter)
                || tr.CreditAccount.Description.Contains(filter)
                || (tr.User != null && tr.User.NameClient.Contains(filter))
                || !tr.IsAcceptedTransaction))
                .OrderByDescending(tr => tr.ApplyDate)
                .ToList();

            return query;
        }
    }
}