using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class Category : EntityBase<BankContext> {
        public Category() { }
        [Key]
        public int CategoryId { get; set; }
        public Category(string name) {
            Name = name;
        }
        public override bool Equals(object obj) {
            return CategoryId == (obj as Category)?.CategoryId;
        }

        public override int GetHashCode() {
            return CategoryId.GetHashCode();
        }

        private string _name;
        [Required]
        public string Name {
            get { return _name; }
            set { _name = value.ToLower(); }
        }
        //[InverseProperty(nameof(Virement.Categorie))]
        public virtual ICollection<Transfer> Transfers { get; set; } = new HashSet<Transfer>();

        public static IQueryable<Category> GetAllCategory() {
            return Context.Categories;
        }
        
    }
}
