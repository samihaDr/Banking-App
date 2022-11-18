using System.Windows;
using System.Windows.Controls;

namespace PRBD_Framework {
    public partial class ErrorMessages : UserControl {
        public ErrorMessages() {
            InitializeComponent();
        }

        public FrameworkElement MyTarget {
            get { return (FrameworkElement)GetValue(MyTargetProperty); }
            set { SetValue(MyTargetProperty, value); }
        }

        public static readonly DependencyProperty MyTargetProperty =
            DependencyProperty.Register("MyTarget", typeof(FrameworkElement),
              typeof(ErrorMessages), new PropertyMetadata(default(FrameworkElement)));
    }
}
