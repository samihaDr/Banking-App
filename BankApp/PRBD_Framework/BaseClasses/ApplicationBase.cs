using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PRBD_Framework {
    public enum ApplicationBaseMessages {
        MSG_REFRESH_DATA
    }

    public abstract partial class ApplicationBase<U, C> : ApplicationRoot where U : EntityBase<C> where C : DbContextBase, new() {
        public static C Context => Context<C>();

        public static U CurrentUser { get; protected set; }

        public static void Login(U user) {
            CurrentUser = user;
        }

        public static void Logout() {
            CurrentUser = null;
        }

        public static bool IsLoggedIn { get => CurrentUser != null; }

    }

    public partial class ApplicationRoot : Application {
        protected static readonly Messenger messenger = new();
        protected static readonly Dictionary<object, List<Tuple<Enum, Guid>>> ids = new();

        private int refreshDelay = 24 * 60 * 60;
        protected int RefreshDelay {
            get => refreshDelay;
            set {
                refreshDelay = value;
                timer.Interval = TimeSpan.FromSeconds(value);
            }
        }

        protected DispatcherTimer timer;

        public static string IMAGE_PATH {
            get {
                // vérifie d'abord si on trouve un dossier images à côté de l'exécutable
                var path = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/images");
                if (!Directory.Exists(path)) {
                    // Si ce n'est pas le cas, on feinte en se basant sur le stack trace pour retrouver le path du code source du projet
                    // C'est ce qui sera utilisé quand on est dans VS en mode design.
                    // (voir https://stackoverflow.com/a/20999702)
                    var trace = new StackTrace(true);
                    foreach (var frame in trace.GetFrames()) {
                        path = Path.GetFullPath(Path.GetDirectoryName(frame.GetFileName()) + "/images");
                        if (Directory.Exists(path))
                            return path;
                    }
                    path = null;
                }
                return path;
            }
        }

        public static string GetAbsolutePicturePath(string relativePath) {
            return Path.Combine(IMAGE_PATH, relativePath);
        }

        private static DbContextBase context;

        public static C Context<C>() where C : DbContextBase, new() {
            try {
                if (context == null) {
                    context = new C();
                    context.SaveChangesFailed += ContextSaveChangesFailed;
                }
                return (C)context;
            } catch (Exception) {
                return null;
            }
        }

        private static void ContextSaveChangesFailed(object sender, SaveChangesFailedEventArgs e) {
            string error = e.Exception.Message;
            if (e.Exception is DbUpdateConcurrencyException)
                error = "The data you're trying to save have been changed by someone else.\n\n" +
                    "They will be reverted now to the values stored in the database.";
            MessageBox.Show(error, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ClearContext() {
            context = null;
        }

        protected virtual void OnRefreshData() {
            //Console.WriteLine(GetType().FullName + "." + nameof(OnRefreshData));
        }

        public ApplicationRoot() {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            Register(this, ApplicationBaseMessages.MSG_REFRESH_DATA, () => OnRefreshData());

            timer = new DispatcherTimer(
                TimeSpan.FromSeconds(RefreshDelay),
                DispatcherPriority.ApplicationIdle, (s, e) => {
                    if (Context<DbContextBase>().NeedsRefreshData()) {
                        Console.Write("Refreshing data... ");
                        ClearContext();
                        NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
                        Console.WriteLine("done");
                    }
                },
                Dispatcher
            );
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            if (e.Exception is DbUpdateException) {
                var ex = e.Exception as DbUpdateException;
                //Console.WriteLine(ex.InnerException.StackTrace);
                MessageBox.Show(ex.GetBaseException().Message, "Database Error! Shutting down...", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            } else {
                //Console.WriteLine(e.Exception.StackTrace);
                MessageBox.Show(e.Exception.ToString());
                e.Handled = true;
            }
            Shutdown();
        }

        protected static Type MapViewModelToView(Type viewModelType) {
            var viewTypeName = viewModelType.FullName.Replace("ViewModel", "View");
            return viewModelType.Assembly.GetType(viewTypeName, true);
        }

        public static void NavigateTo<T, U, C>() where T : ViewModelBase<U,C> where U : EntityBase<C> where C : DbContextBase, new() {
            var win = Current.MainWindow;
            Current.MainWindow = (WindowBase)Activator.CreateInstance(MapViewModelToView(typeof(T)));
            Current.MainWindow.Show();
            if (win != null)
                win.Close();
        }

        public static object ShowDialog<T, U, C>(params object[] args) 
            where T : DialogViewModelBase<U, C> where U : EntityBase<C> where C : DbContextBase, new() {
            var frm = (DialogWindowBase)Activator.CreateInstance(MapViewModelToView(typeof(T)), args);
            frm.ShowDialog();
            return frm.GetViewModel<U,C>().DialogResult;
        }

        public static void ChangeCulture(string culture) {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            var oldWindow = Current.MainWindow;
            if (oldWindow != null) {
                var type = oldWindow.GetType();
                Window newWindow = (Window)Activator.CreateInstance(type);
                newWindow.Show();
                Current.MainWindow = newWindow;
                oldWindow.Close();
            }
        }

        public static void Register(object owner, Enum message, Action callback) {
            var id = messenger.Register(message, callback);
            if (!ids.ContainsKey(owner))
                ids[owner] = new List<Tuple<Enum, Guid>>();
            ids[owner].Add(new Tuple<Enum, Guid>(message, id));
        }

        public static void Register<T>(object owner, Enum message, Action<T> callback) {
            var id = messenger.Register<T>(message, callback);
            if (!ids.ContainsKey(owner))
                ids[owner] = new List<Tuple<Enum, Guid>>();
            ids[owner].Add(new Tuple<Enum, Guid>(message, id));
        }

        public static void NotifyColleagues(Enum message, object parameter) {
            messenger.NotifyColleagues(message, parameter);
        }

        public static void NotifyColleagues(Enum message) {
            messenger.NotifyColleagues(message);
        }

        public static void UnRegister(object owner) {
            if (ids.ContainsKey(owner))
                foreach (var tuple in ids[owner])
                    messenger.UnRegister(tuple.Item1, tuple.Item2);
        }
    }
}
