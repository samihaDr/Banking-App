using PRBD_Framework;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Model {
    public enum Role {
        Holder = 1,
        Representative = 2
    }
    public class ClientInternalAccount : EntityBase<BankContext> {
        public ClientInternalAccount() { } 
        public ClientInternalAccount(Client client, InternalAccount internalAccount, Role role) {
            Client = client;
            InternalAccount = internalAccount;
            Role = role;
        }

        public int ClientId { get; set; }
        public int InternalAccountId { get; set; }
           
        public Role Role { get; set; }

        public virtual Client Client { get; set; }
        public virtual InternalAccount InternalAccount { get; set; }
 
        /// <summary>
        /// Obtenir tous les comptes internes d'un user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IQueryable<ClientInternalAccount> GetInternalAccountsByUser(User user) {
            return Context.ClientInternalAccounts.Where(clientInternalAccount => clientInternalAccount.ClientId == user.UserId);
        }
        /// <summary>
        /// Requete qui renvoie la liste de tous les comptes d'un client
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        
        public static IQueryable<Account> GetAccountsByClient(User user) {
            var allAccountsClient =  from clientInternalAccount in Context.ClientInternalAccounts
                                        where clientInternalAccount.ClientId == user.UserId
                                        select clientInternalAccount.InternalAccount;
            return allAccountsClient.Distinct();
        }
       
        //public static IQueryable<Account> GetOtherAccounts(User user) {
        //    var OtherClientAccounts = from clientInternalAccount in Context.ClientInternalAccounts
        //                                where clientInternalAccount.ClientId != user.UserId 
        //                                select clientInternalAccount.InternalAccount;
        //    return OtherClientAccounts.Distinct();
                                     
        //}
        /// <summary>
        ///Requete qui renvoie la liste de tous les comptes d'un client sauf celui qui est selectionné dans la vue Transfer
        /// </summary>
        /// <param name="user"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IQueryable<Account> GetAllAccountsByClientExceptFromAccount(User user, Account FromAccount) {
            IQueryable<Account> query = null;
            if (FromAccount is SavingAccount ) {
                query = from cia in Context.ClientInternalAccounts
                        where cia.InternalAccountId != FromAccount.AccountId && cia.ClientId == user.UserId && cia.InternalAccount is CurrentAccount
                        select cia.InternalAccount;
            } else if (FromAccount is CurrentAccount) {
                query = from cia in Context.ClientInternalAccounts
                        where cia.InternalAccountId != FromAccount.AccountId && cia.ClientId == user.UserId
                        select cia.InternalAccount;
            }
            return query.Distinct();
        }
        public static IQueryable<ClientInternalAccount> GetFiltered(string Filter, User user) {
            var filtered = from clientInternalAccount in Context.ClientInternalAccounts
                           where clientInternalAccount.ClientId == user.UserId 
                           && (clientInternalAccount.InternalAccount.Iban.Contains(Filter) 
                           || clientInternalAccount.InternalAccount.Description.Contains(Filter))
                           orderby clientInternalAccount.InternalAccount.Iban
                           select clientInternalAccount;
            return filtered.Distinct();
        }
     

    }
}
