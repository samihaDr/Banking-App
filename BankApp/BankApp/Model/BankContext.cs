
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;
using System;

namespace BankApp.Model {
    public class BankContext : DbContextBase {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=bankApp")
                .UseLazyLoadingProxies(true);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClientInternalAccount>()
                .HasKey(cCpIn => new { cCpIn.ClientId, cCpIn.InternalAccountId });

            modelBuilder.Entity<ClientInternalAccount>()
                .HasOne(cCpIn => cCpIn.Client)
                .WithMany(c => c.Accounts)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<ClientInternalAccount>()
                .HasOne(cCpIn => cCpIn.InternalAccount)
                .WithMany(cpInterne => cpInterne.Accounts)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<InternalAccount> InternalAccounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<ClientInternalAccount> ClientInternalAccounts { get; set; }

        public void SeedData() {
            Database.BeginTransaction();
            var agency1 = new Agency("Agency1");
            var agency2 = new Agency("Agency2");
            var agency3 = new Agency("Agency3");
            var bruno = new Manager("Bruno", "Lacroix", "bruno@test.com", "bruno");
            var benoit = new Manager("Benoît", "Penelle","ben@test.com", "ben");
            var admin = new Admin("Admin", "Istrator", "admin@test.com", "admin");
            var bob = new Client("Bob", "Marley", "bob@test.com", "bob", agency1);
            var caro = new Client("Caroline", "de Monaco", "caro@test.com", "caro", agency1);
            var louise = new Client("Louise", "TheCross", "louise@test.com", "louise", agency2);
            var jules = new Client("Jules", "TheCross","jules@test.com", "jules", agency2);
            Users.AddRange(bruno, benoit, admin, bob, caro, louise, jules);
            bruno.Agencies.Add(agency3);
            benoit.Agencies.Add(agency1);
            benoit.Agencies.Add(agency2);
            var currentA = new CurrentAccount("BE02 9999 1017 8207", "AAA", -50);
            var currentB = new CurrentAccount("BE14 9996 1669 4306", "BBB", -10);
            var currentD = new CurrentAccount("BE55 9999 6717 9982", "DDD", -100);
            var savingC = new SavingAccount("BE71 9991 5987 4787", "CCC");
            var externalE = new ExternalAccount("BE23 0081 6870 0358", "EEE");
            var cia1 = new ClientInternalAccount(bob, currentA, Role.Holder);
            var cia2 = new ClientInternalAccount(bob, currentB, Role.Holder);
            var cia3 = new ClientInternalAccount(bob, savingC, Role.Representative);
            var cia4 = new ClientInternalAccount(caro, currentA, Role.Representative);
            var cia5 = new ClientInternalAccount(caro, currentD, Role.Holder);
            var cia6 = new ClientInternalAccount(caro, savingC, Role.Holder);
            ClientInternalAccounts.AddRange(cia1, cia2, cia3, cia4, cia5, cia6);
            var cat1 = new Category("Category1");
            var cat2 = new Category("Category2");
            var cat3 = new Category("Category3");
            var cat4 = new Category("Category4");
            var cat5 = new Category("Category5");
            Categories.AddRange(cat1, cat2, cat3, cat4, cat5);
            var transfer1 = new Transfer(currentA, currentB, benoit, 15, "Tx #014", new DateTime(2022, 01, 15), null);
            var transfer2 = new Transfer(externalE, savingC, null, 5, "Tx #007", new DateTime(2022, 01, 04), null, new DateTime(2022, 01, 08));
            var transfer3 = new Transfer(currentB, savingC, bob, 35, "Tx #012", new DateTime(2022, 01, 12), null, new DateTime(2022, 01, 14));
            var transfer4 = new Transfer(savingC, externalE, bob, 10, "Tx #009", new DateTime(2022, 01, 07), null, new DateTime(2022, 01, 11));
            var transfer5 = new Transfer(currentB, currentA, bob, 20, "Tx #006", new DateTime(2022, 01, 03), null, new DateTime(2022, 01, 07));
            var transfer6 = new Transfer(currentA, currentB, bob, 50, "Tx #005",  new DateTime(2022, 01, 02), null, new DateTime(2022, 01, 04));
            var transfer7 = new Transfer(currentA, savingC, caro, 5, "Tx #002",  new DateTime(2022, 01, 01), null, new DateTime(2022, 01, 05));
            var transfer8 = new Transfer(currentA, currentB, caro, 35, "Tx #003", new DateTime(2022, 01, 01), null, new DateTime(2022, 01, 09));
            var transfer9 = new Transfer(savingC, currentB, caro, 100, "Tx #008", new DateTime(2022, 01, 06), null);
            var transfer10 = new Transfer(currentA, savingC, caro, 15, "Tx #011", new DateTime(2022, 01, 11), null, new DateTime(2022, 01, 16));
            var transfer11 = new Transfer(currentA, savingC, caro, 100, "Tx #013",  new DateTime(2022, 01, 13), null);
            var transfer12 = new Transfer(currentB, savingC, bob, 15, "Tx #004",  new DateTime(2022, 01, 02), null, new DateTime(2022, 01, 03));
            var transfer13 = new Transfer(savingC, currentA, bob, 15, "Tx #010",  new DateTime(2022, 01, 10), null);
            var transfer14 = new Transfer(currentA, currentB, bob, 10, "Tx #001",  new DateTime(2022, 01, 01), null);
            Transfers.AddRange(transfer1, transfer2, transfer3, transfer4, transfer5, transfer6, transfer7, transfer8,
                transfer9, transfer10, transfer11, transfer12, transfer13, transfer14);
            SaveChanges();
            Database.CommitTransaction();
        }
       
    }


}
