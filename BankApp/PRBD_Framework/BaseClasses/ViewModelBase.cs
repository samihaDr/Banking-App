namespace PRBD_Framework {
    public abstract class ViewModelBase<U, C> : ObservableBase where U : EntityBase<C> where C : DbContextBase, new() {
        protected C Context { get => ApplicationRoot.Context<C>(); }

        public ViewModelBase() : base() {
            ApplicationRoot.Register(this, ApplicationBaseMessages.MSG_REFRESH_DATA, OnRefreshData);
        }

        protected virtual void OnRefreshData() { }

        public bool HasChanges => Context.ChangeTracker.HasChanges();

        public virtual void SaveAction() { }

        public virtual void CancelAction() { }

        public virtual bool MayLeave => !HasChanges;

        public static U CurrentUser {
            get => ApplicationBase<U, C>.CurrentUser;
        }

        public static bool IsLoggedIn => ApplicationBase<U, C>.IsLoggedIn;

    }
}
