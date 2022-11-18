using System.Windows;
using System.Windows.Controls;

namespace PRBD_Framework {
    public class ValidatedField : StackPanel {
        public override void EndInit() {
            var err = new ErrorMessages { MyTarget = (FrameworkElement)Children[0] };
            Children.Add(err);
            base.EndInit();
        }
    }
}
