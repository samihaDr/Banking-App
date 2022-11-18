using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class Manager : User {
        public Manager() { }
        public Manager(string firstName, string lastName, string email, string password) : base(firstName, lastName, email, password) {
        }

        public virtual ICollection<Agency> Agencies { get; set; } = new HashSet<Agency>();

        public static IQueryable<Agency> GetAgenciesByManager(User user) {
            var query = Context.Agencies.Where(agency => agency.Manager.UserId == user.UserId);
            return query;
        }

    }
}
