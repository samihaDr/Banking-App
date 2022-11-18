using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Model {
    public class CheckedCategory {
        public CheckedCategory() { }

        private Category _category;
        public Category Category {
            get { return _category; }
            set { _category = value; }
        }
        private bool _isChecked;
        public bool IsChecked {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
       
        public CheckedCategory(Category category, bool isChecked) {
            Category = category;
            IsChecked = isChecked;
        }
        public static List<CheckedCategory> CheckedCategoriesList() {
            List<CheckedCategory> checkedCategories = new List<CheckedCategory>();
            checkedCategories.Add(new CheckedCategory(new Category("<No Category>"), true));
            IQueryable<Category> AllCategories = Category.GetAllCategory();
            foreach (var category in AllCategories) {
                checkedCategories.Add(new CheckedCategory(category, true));
            }
            return checkedCategories;
        }
    }


}
