using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class Agency : EntityBase<BankContext>{
        public Agency() {
        }

        public Agency(string name) {
            Name = name;
        }
        [Key]
        public int AgencyId { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Client> Clients { get; set; } = new HashSet<Client>();
        public virtual Manager Manager { get; set; }
       
        public static IQueryable<Client> GetClientsByAgency(Agency agency) {
            var query= Context.Clients.Where(client => client.Agency.Name == agency.Name);
            return query;
        }

    }
}
