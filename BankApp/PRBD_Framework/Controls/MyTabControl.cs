using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PRBD_Framework {
    public class MyTabControl : TabControl, IDisposable {
        public TabItem Add(ContentControl content, string header, object tag = null) {

            // refuser l'ajout si le tab courant est dirty
            if (IsTabDirty(SelectedItem as TabItem)) {
                DisplayChangeTabError();
                return null;
            }

            // crée le tab
            var tab = new TabItem() {
                Content = content,
                Header = header,
                Tag = tag ?? header
            };

            if (HasCloseButton) {
                // crée le header du tab avec le bouton de fermeture
                var headerPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                headerPanel.Children.Add(new TextBlock() { Text = header, VerticalAlignment = VerticalAlignment.Center });
                var closeButton = new Button() {
                    Content = "X",
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.Transparent,
                    Foreground = Brushes.Red,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = "Close"
                };
                headerPanel.Children.Add(closeButton);
                tab.Header = headerPanel;

                closeButton.Click += (o, e) => CloseTab(tab);
            }

            // ajoute cet onglet à la liste des onglets existant du TabControl
            Items.Add(tab);
            // exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            SetFocus(tab);
            return tab;
        }

        private void CloseTab(TabItem tab) {
            if (tab == null) return;
            if (tab.Content != null) {
                var vm = (tab.Content as dynamic)?.DataContext;
                if (vm != null && (vm.HasErrors || vm.HasChanges || !vm.MayLeave))
                    MessageBox.Show(Application.Current.MainWindow,
                        "You have unsaved changes and/or errors", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    Items.Remove(tab);
            } else
                Items.Remove(tab);
        }

        public void RenameTab(TabItem tab, string newName) {
            if (tab.Header is StackPanel stackPanel)
                (stackPanel.Children[0] as TextBlock).Text = newName;
            else
                tab.Header = newName;
            tab.Tag = newName;
        }

        private void Dispose(IEnumerable<TabItem> tabs) {
            var tabsToDispose = new List<TabItem>(tabs);
            foreach (TabItem tab in tabsToDispose) {
                if (tab.Content != null) {
                    var content = (tab.Content as ContentControl).Content as FrameworkElement;
                    if (content.DataContext is not null and IDisposable ctx) {
                        ctx.Dispose();
                        content.DataContext = null;
                    }
                    // dispose du UserControlBase lui-même
                    if (tab.Content is UserControlBase uc)
                        uc.Dispose();
                }
            }
        }

        public void Dispose() {
            Dispose(Items.OfType<TabItem>());
        }

        public void CloseByTag(string tag) {
            var tab = FindByTag(tag);
            if (tab != null)
                CloseTab(tab);
        }

        public TabItem FindByTag(string tag) {
            return (from TabItem t in Items where tag == t.Tag?.ToString() select t).FirstOrDefault();
        }

        public TabItem FindByHeader(string header) {
            return (from TabItem t in Items where header == t.Header?.ToString() select t).FirstOrDefault();
        }

        public void SetFocus(TabItem tab) {
            Dispatcher.InvokeAsync(() => {
                tab.Focus();
                SelectedItem = tab;
            });
        }

        private bool InheritsFrom(Type source, Type toCheck) {
            //TODO: improve this check
            if (source.Name == toCheck.Name)
                return true;
            if (source.BaseType != typeof(object))
                return InheritsFrom(source.BaseType, toCheck);
            return false;
        }

        private bool IsTabDirty(TabItem tab) {
            if (tab?.Content is UserControlBase uc &&
                InheritsFrom(uc.DataContext.GetType(), typeof(ViewModelBase<,>))) {
                dynamic vm = uc.DataContext;
                return vm.HasErrors || vm.HasChanges || !vm.MayLeave;
            }
            return false;
        }

        private void DisplayChangeTabError() {
            MessageBox.Show(Application.Current.MainWindow,
                "You have unsaved changes and/or errors", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool selectedTabChanging;

        protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
            // vérifie si le VM associé au UC contenu dans le TabItem n'a pas de changements en cours
            if (selectedTabChanging) return;
            var oldItem = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as TabItem : null;
            if (IsTabDirty(oldItem)) {
                selectedTabChanging = true;
                SelectedItem = oldItem;
                DisplayChangeTabError();
                selectedTabChanging = false;
            }

            // vérifie si au moins un des tabs sélectionnés est visible
            bool visible = false;
            foreach (TabItem tab in e.AddedItems)
                if (tab.Visibility == Visibility.Visible) {
                    visible = true;
                    break;
                }

            if (!visible) {
                // cherche un tab visible
                foreach (TabItem tab in Items)
                    if (tab.Visibility == Visibility.Visible) {
                        SelectedItem = tab;
                        e.Handled = false;
                        break;
                    }
            }

            base.OnSelectionChanged(e);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Remove)
                Dispose(e.OldItems.OfType<TabItem>());
            else if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (TabItem tab in e.NewItems) {
                    tab.MouseDown += (o, e) => {
                        if (e.ChangedButton == MouseButton.Middle &&
                            e.ButtonState == MouseButtonState.Pressed) {
                            CloseTab(tab);
                        }
                    };
                    tab.PreviewKeyDown += (o, e) => {
                        if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl)) {
                            CloseTab(tab);
                        }
                    };
                }
        }

        public bool HasCloseButton {
            get => (bool)GetValue(HasCloseButtonProperty);
            set => SetValue(HasCloseButtonProperty, value);
        }

        public static readonly DependencyProperty HasCloseButtonProperty = DependencyProperty.Register(
            nameof(HasCloseButton), typeof(bool), typeof(MyTabControl), new PropertyMetadata(false));
    }
}
