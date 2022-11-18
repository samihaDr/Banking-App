using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PRBD_Framework {
    public class MyDataGrid : DataGrid {
        public MyDataGrid() {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.SelectedItemsList = this.SelectedItems;
        }

        public IList SelectedItemsList {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(MyDataGrid),
                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
