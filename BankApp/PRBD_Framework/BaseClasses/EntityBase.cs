using Microsoft.EntityFrameworkCore;

namespace PRBD_Framework {
    public class EntityBase<C> : ValidatableObjectBase where C : DbContextBase, new() {
        public static C Context => ApplicationRoot.Context<C>();

        public EntityBase() { }

        public bool IsDetached => Context.Entry(this).State == EntityState.Detached;
        public bool IsUnchanged => Context.Entry(this).State == EntityState.Unchanged;
        public bool IsAdded => Context.Entry(this).State == EntityState.Added;
        public bool IsModified => Context.Entry(this).State == EntityState.Modified;
        public bool IsDeleted => Context.Entry(this).State == EntityState.Deleted;

        public void Reload() {
            if (IsModified)
                Context.Entry(this).Reload();
        }
    }
}