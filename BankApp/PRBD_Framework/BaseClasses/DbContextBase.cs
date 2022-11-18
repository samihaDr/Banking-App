using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PRBD_Framework {
    internal class DbHistory {
        [Key] public string TableName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now.ToUniversalTime();
    }

    public class DbContextBase : DbContext {
        internal DbSet<DbHistory> DbHistories { get; set; }
        private static Dictionary<string, DateTime> history = new Dictionary<string, DateTime>();

        public DbContextBase() : base() { }

        public DbContextBase(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            AppDomain.CurrentDomain.SetData("DataDirectory",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            base.OnConfiguring(optionsBuilder);
        }

        public bool NeedsRefreshData() {
            try {
                if (ChangeTracker.HasChanges())
                    return false;
                var histEntries = (from h in DbHistories select h).ToList();
                bool needsRefresh = false;
                foreach (var h in histEntries) {
                    var t = history.ContainsKey(h.TableName) ? history[h.TableName] : new DateTime();
                    Entry(h).Reload();
                    if (h.Timestamp != t) {
                        needsRefresh = true;
                        history[h.TableName] = h.Timestamp;
                    }
                }

                return needsRefresh;
            } catch (Exception) {
                return true;
            }
        }

        private void UpdateHistory() {
            var tables = ((from e in ChangeTracker.Entries()
                           where e.State != EntityState.Unchanged
                           select e.Metadata.GetTableName()).Distinct()).ToList();
            foreach (var table in tables) {
                var hist = DbHistories.Find(table);
                if (hist == null) {
                    hist = new DbHistory { TableName = table };
                    DbHistories.Add(hist);
                }

                hist.Timestamp = DateTime.Now.ToUniversalTime();
                history[table] = hist.Timestamp;
            }
        }

        public override int SaveChanges() {
            int count = -1;
            bool hasTransaction = Database.CurrentTransaction != null;
            try {
                if (ExecuteValidation()) {
                    // s'il n'y a pas de transaction en cours, en créer une
                    if (!hasTransaction)
                        Database.BeginTransaction();
                    // mettre à jour la table historique pour les entités impactées
                    UpdateHistory();
                    count = base.SaveChanges();
                    if (!hasTransaction)
                        Database.CommitTransaction();
                    return count;
                }

                Console.WriteLine("SaveChanges() not successful due to business rules errors");
            } catch (DbUpdateConcurrencyException ex) {
                Console.WriteLine("SaveChanges() not successful due to optimistic locking violation");
                // see: https://docs.microsoft.com/en-us/ef/core/saving/concurrency
                foreach (var entry in ex.Entries) {
                    entry.Reload();
                }

                ApplicationRoot.NotifyColleagues(ApplicationBaseMessages.MSG_REFRESH_DATA);
            }

            if (!hasTransaction)
                Database.RollbackTransaction();
            return count;
        }

        private bool ExecuteValidation() {
            bool hasErrors = false;
            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => (e.State == EntityState.Added) || (e.State == EntityState.Modified)).ToList()) {
                if (entry.Entity is ValidatableObjectBase) {
                    var entity = entry.Entity as ValidatableObjectBase;
                    entity.Validate();
                    if (entity.HasErrors) {
                        Console.WriteLine($"Business rules errors in entity of type {entity.GetType().Name}:\n" +
                                          JsonConvert.SerializeObject(entity.Errors, Formatting.Indented));
                        hasErrors = true;
                    }
                }
            }

            return !hasErrors;
        }

        public int SaveChangesWithIdentityInsert<T>() {
            int ret;
            // see:  https://entityframeworkcore.com/knowledge-base/58847327/set-identity-insert-not-working-when-using-transactionscope-in-entity-framework-core-3
            Database.OpenConnection();
            try {
                SetIdentityInsert<T>(true);
                ret = base.SaveChanges();
                SetIdentityInsert<T>(false);
            } finally {
                Database.CloseConnection();
            }
            return ret;
        }

        private void SetIdentityInsert<T>(bool enable) {
            if (!Database.IsSqlServer() || !HasIdentity<T>()) return;
            IEntityType entityType = Model.FindEntityType(typeof(T));
            string value = enable ? "ON" : "OFF";
            string query = $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}";
            Database.ExecuteSqlRaw(query);
        }

        private bool HasIdentity<T>() {
            var efEntity = Model.FindEntityType(typeof(T));
            var efProperties = efEntity.GetProperties();
            return efProperties.Any(p => {
                return p.IsPrimaryKey() && p.ValueGenerated == ValueGenerated.OnAdd;
            });
        }
    }
}