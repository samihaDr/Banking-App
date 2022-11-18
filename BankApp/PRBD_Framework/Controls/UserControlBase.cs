using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PRBD_Framework {
    public class UserControlBase : UserControl, IDisposable {
        private bool _disposed;

        //public UserControlBase(): base() {
        //    // see: https://stackoverflow.com/a/993454
        //    Console.WriteLine(CultureInfo.CurrentCulture.IetfLanguageTag);
        //    //this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        //}

        public virtual void Dispose() {
            if (!_disposed) {
                //Console.WriteLine("Disposing " + this);
                ApplicationRoot.UnRegister(this);
                _disposed = true;
                (DataContext as IDisposable)?.Dispose();
            }
        }

        public void Register(Enum message, Action callback) {
            ApplicationRoot.Register(this, message, callback);
        }

        public void Register<T>(Enum message, Action<T> callback) {
            ApplicationRoot.Register(this, message, callback);
        }

        public static void NotifyColleagues(Enum message, object parameter) {
            ApplicationRoot.NotifyColleagues(message, parameter);
        }

        public static void NotifyColleagues(Enum message) {
            ApplicationRoot.NotifyColleagues(message);
        }

        public void UnRegister() {
            ApplicationRoot.UnRegister(this);
        }
    }
}
