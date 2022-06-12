using Microsoft.EntityFrameworkCore;
using Models.Models;

#nullable disable

namespace Models.Context
{
    public partial class dominoespropertiesContext : DbContext
    {
        public dominoespropertiesContext()
        {
        }

        public dominoespropertiesContext(DbContextOptions<dominoespropertiesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<Investment> Investments { get; set; }
        public virtual DbSet<PaystackPayment> PaystackPayments { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PropertyType> PropertyTypes { get; set; }
        public virtual DbSet<PropertyUpload> PropertyUploads { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Name=ConnectionStrings:DominoProps_String");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PRIMARY");

                entity.ToTable("Admin");

                entity.HasIndex(e => e.Email, "Admin_Email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.RoleFk, "Admin_Role_Id_fk");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsActive).HasColumnType("bit(1)");

                entity.Property(e => e.IsDeleted).HasColumnType("bit(1)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.RoleFk).HasColumnName("RoleFK");

                entity.HasOne(d => d.RoleFkNavigation)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.RoleFk)
                    .HasConstraintName("Admin_Role_Id_fk");
            });

            modelBuilder.Entity<ApplicationSetting>(entity =>
            {
                entity.Property(e => e.SettingName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TestingEmail).HasMaxLength(200);

                entity.Property(e => e.TestingMode).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog");

                entity.Property(e => e.RequestPayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.ResponsePayload).HasColumnType("varchar(5000)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.Email, "Customer_Email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "Customer_Phone_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UniqueRef, "Customer_UniqueRef_uindex")
                    .IsUnique();

                entity.Property(e => e.AccountNumber).HasMaxLength(10);

                entity.Property(e => e.Address).HasColumnType("varchar(5000)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsAccountVerified)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsActive)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsSubscribed)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsVerified)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PassportUrl).HasMaxLength(500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.UniqueRef)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Description>(entity =>
            {
                entity.ToTable("Description");

                entity.Property(e => e.AirConditioned).HasColumnType("bit(1)");

                entity.Property(e => e.Basement).HasColumnType("bit(1)");

                entity.Property(e => e.Fireplace).HasColumnType("bit(1)");

                entity.Property(e => e.Gym).HasColumnType("bit(1)");

                entity.Property(e => e.LandSize).HasMaxLength(50);

                entity.Property(e => e.Laundry).HasColumnType("bit(1)");

                entity.Property(e => e.Parking).HasColumnType("bit(1)");

                entity.Property(e => e.Refrigerator).HasColumnType("bit(1)");

                entity.Property(e => e.SecurityGuard).HasColumnType("bit(1)");

                entity.Property(e => e.SwimmingPool).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.ToTable("Investment");

                entity.HasIndex(e => e.CustomerId, "Investment_Customer_Id_fk");

                entity.HasIndex(e => e.PropertyId, "Investment_Property_Id_fk");

                entity.HasIndex(e => e.TransactionRef, "Investment_TransactionRef_uindex")
                    .IsUnique();

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.PaymentDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.PaymentType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TransactionRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.YearlyInterestAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Yield).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("Investment_Customer_Id_fk");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("Investment_Property_Id_fk");
            });

            modelBuilder.Entity<PaystackPayment>(entity =>
            {
                entity.ToTable("PaystackPayment");

                entity.HasIndex(e => e.TransactionRef, "PaystackPayment_TransactionRef_uindex")
                    .IsUnique();

                entity.Property(e => e.AccessCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Channel).HasMaxLength(10);

                entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FromAccount).HasMaxLength(10);

                entity.Property(e => e.Payload).HasColumnType("varchar(5000)");

                entity.Property(e => e.PaymentModule)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Status).HasMaxLength(5);

                entity.Property(e => e.ToAccount).HasMaxLength(10);

                entity.Property(e => e.TransactionRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(2);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property");

                entity.HasIndex(e => e.CreatedBy, "Property_Admin_Email_fk");

                entity.HasIndex(e => e.Type, "Property_Property_Type_Id_fk");

                entity.HasIndex(e => e.UniqueId, "Property_UniqueId_uindex")
                    .IsUnique();

                entity.Property(e => e.ClosingDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsDeleted)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Latitude).HasMaxLength(100);

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.Longitude).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectedGrowth).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.TargetYield).HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Property_Admin_Email_fk");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("Property_Property_Type_Id_fk");
            });

            modelBuilder.Entity<PropertyType>(entity =>
            {
                entity.ToTable("Property_Type");

                entity.Property(e => e.DateCreated).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<PropertyUpload>(entity =>
            {
                entity.HasIndex(e => e.PropertyId, "PropertyUploads_Property_Id_fk");

                entity.HasIndex(e => e.Url, "PropertyUploads_Url_uindex")
                    .IsUnique();

                entity.Property(e => e.ImageName).HasMaxLength(100);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertyUploads)
                    .HasForeignKey(d => d.PropertyId)
                    .HasConstraintName("PropertyUploads_Property_Id_fk");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Page).HasColumnType("varchar(5000)");

                entity.Property(e => e.Privilege).HasColumnType("varchar(5000)");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionRef)
                    .HasName("PRIMARY");

                entity.ToTable("Transaction");

                entity.HasIndex(e => e.CustomerId, "Transaction_Customer_Id_fk");

                entity.Property(e => e.TransactionRef).HasMaxLength(50);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Channel).HasMaxLength(10);

                entity.Property(e => e.Module).HasMaxLength(20);

                entity.Property(e => e.RequestPayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.ResponsePayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.TransactionDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TransactionType).HasMaxLength(10);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("Transaction_Customer_Id_fk");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("Wallet");

                entity.HasIndex(e => e.CustomerId, "Wallet_CustomerId_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.WalletNo, "Wallet_WalletNo_uindex")
                    .IsUnique();

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastTransactionAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Limit)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.WalletNo)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.Wallet)
                    .HasForeignKey<Wallet>(d => d.CustomerId)
                    .HasConstraintName("Wallet_Customer_Id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
