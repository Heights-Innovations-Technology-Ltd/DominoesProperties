#nullable disable

using Microsoft.EntityFrameworkCore;
using Models.Models;

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
        public virtual DbSet<ApplicationSetting> Applicationsettings { get; set; }
        public virtual DbSet<AuditLog> Auditlogs { get; set; }
        public virtual DbSet<Blogpost> Blogposts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Description> Descriptions { get; set; }
        public virtual DbSet<EmailRetry> Emailretries { get; set; }
        public virtual DbSet<Enquiry> Enquiries { get; set; }
        public virtual DbSet<Investment> Investments { get; set; }
        public virtual DbSet<Newsletter> Newsletters { get; set; }
        public virtual DbSet<OfflineInvestment> OfflineInvestments { get; set; }
        public virtual DbSet<PaystackPayment> Paystackpayments { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PropertyType> PropertyTypes { get; set; }
        public virtual DbSet<Propertyupload> Propertyuploads { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Sharingentry> Sharingentries { get; set; }
        public virtual DbSet<Sharinggroup> Sharinggroups { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<Thirdpartycustomer> Thirdpartycustomers { get; set; }
        public virtual DbSet<Thirdpartyclient> Thirdpartyclients { get; set; }
        public virtual DbSet<Thirdpartyinvestment> ThirdPartyInvestments { get; set; }

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

                entity.ToTable("admin");

                entity.HasIndex(e => e.Email, "admin_Email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.RoleFk, "admin_role_Id_fk");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.RoleFk)
                    .HasColumnName("RoleFK")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.RoleFkNavigation)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.RoleFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("admin_role_Id_fk");
            });

            modelBuilder.Entity<ApplicationSetting>(entity =>
            {
                entity.ToTable("applicationsettings");

                entity.HasIndex(e => e.Id, "applicationsettings_Id_uindex")
                    .IsUnique();

                entity.Property(e => e.SettingName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TestingEmail)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.TestingMode)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("auditlog");

                entity.HasIndex(e => e.Id, "auditlog_Id_uindex")
                    .IsUnique();

                entity.Property(e => e.RequestPayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.ResponsePayload).HasColumnType("varchar(5000)");
            });

            modelBuilder.Entity<Blogpost>(entity =>
            {
                entity.ToTable("blogpost");

                entity.HasIndex(e => e.CreatedBy, "blogpost_admin_null_fk");

                entity.Property(e => e.BlogImage).HasMaxLength(500);

                entity.Property(e => e.BlogTags).HasMaxLength(500);

                entity.Property(e => e.BlogTitle).HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasMaxLength(70);

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UniqueNumber).HasMaxLength(200);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Blogposts)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("blogpost_admin_null_fk");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.HasIndex(e => e.Email, "customer_Email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "customer_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "customer_Phone_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UniqueRef, "customer_UniqueRef_uindex")
                    .IsUnique();

                entity.Property(e => e.AccountNumber).HasMaxLength(10);

                entity.Property(e => e.Address).HasColumnType("varchar(5000)");

                entity.Property(e => e.BankName).HasMaxLength(200);

                entity.Property(e => e.DateRegistered).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsAccountVerified)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsActive)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsSubscribed)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsVerified)
                    .IsRequired()
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
                entity.ToTable("description");

                entity.Property(e => e.AirConditioned)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Basement)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Bathroom).HasDefaultValueSql("'0'");

                entity.Property(e => e.Bedroom).HasDefaultValueSql("'0'");

                entity.Property(e => e.Fireplace)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.FloorLevel).HasDefaultValueSql("'0'");

                entity.Property(e => e.Gym)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LandSize).HasMaxLength(50);

                entity.Property(e => e.Laundry)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Parking)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.PropertyId).HasMaxLength(200);

                entity.Property(e => e.Refrigerator)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.SecurityGuard)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.SwimmingPool)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Toilet).HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<EmailRetry>(entity =>
            {
                entity.ToTable("emailretry");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasColumnName("body");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("category");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("date_created")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Recipient)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("recipient");

                entity.Property(e => e.RecipientName)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("recipient_name");

                entity.Property(e => e.RetryCount).HasColumnName("retry_count");

                entity.Property(e => e.StatusCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("status_code");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("subject");
            });

            modelBuilder.Entity<Enquiry>(entity =>
            {
                entity.ToTable("enquiry");

                entity.HasIndex(e => e.Status, "enquiry_Status_index");

                entity.HasIndex(e => e.ClosedBy, "enquiry_admin_Email_fk");

                entity.Property(e => e.ClosedBy).HasMaxLength(200);

                entity.Property(e => e.CustomerUniqueReference)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.PropertyReference)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'NEW'");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.ClosedByNavigation)
                    .WithMany(p => p.Enquiries)
                    .HasForeignKey(d => d.ClosedBy)
                    .HasConstraintName("enquiry_admin_Email_fk");
            });

            modelBuilder.Entity<Investment>(entity =>
            {
                entity.ToTable("investment");

                entity.HasIndex(e => e.Id, "investment_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Status, "investment_Status_index");

                entity.HasIndex(e => e.CustomerId, "investment_customer_Id_fk");

                entity.HasIndex(e => e.PropertyId, "investment_property_Id_fk");

                entity.HasIndex(e => e.TransactionRef, "investment_transaction_TransactionRef_fk");

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.PaymentType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'PENDING'");

                entity.Property(e => e.TransactionRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.YearlyInterestAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Yield).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investment_customer_Id_fk");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investment_property_Id_fk");

                entity.HasOne(d => d.TransactionRefNavigation)
                    .WithMany(p => p.Investments)
                    .HasForeignKey(d => d.TransactionRef)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("investment_transaction_TransactionRef_fk");
            });

            modelBuilder.Entity<Newsletter>(entity =>
            {
                entity.ToTable("newsletter");

                entity.HasIndex(e => e.Email, "newsletter_Email_uindex")
                    .IsUnique();

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(70);
            });

            modelBuilder.Entity<OfflineInvestment>(entity =>
            {
                entity.ToTable("offline_investment");

                entity.HasIndex(e => e.PaymentRef, "investment_Payment_TransactionRef_fk");

                entity.HasIndex(e => e.Id, "off_investment_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Status, "off_investment_Status_index");

                entity.HasIndex(e => e.CustomerId, "off_investment_customer_Id_fk");

                entity.HasIndex(e => e.PropertyId, "off_investment_property_Id_fk");

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.PaymentRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProofUrl).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'PENDING'");

                entity.Property(e => e.TreatedBy).HasMaxLength(70);

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.OfflineInvestments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("off_investment_customer_Id_fk");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.OfflineInvestments)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("off_investment_property_Id_fk");
            });

            modelBuilder.Entity<PaystackPayment>(entity =>
            {
                entity.ToTable("paystackpayment");

                entity.HasIndex(e => e.Id, "paystackpayment_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.PaystackRef, "paystackpayment_pk")
                    .IsUnique();

                entity.HasIndex(e => e.TransactionRef, "paystackpayment_TransactionRef_uindex");

                entity.Property(e => e.AccessCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Channel).HasMaxLength(10);

                entity.Property(e => e.FromAccount).HasMaxLength(10);

                entity.Property(e => e.Payload).HasColumnType("varchar(5000)");

                entity.Property(e => e.PaymentModule).HasMaxLength(50);

                entity.Property(e => e.PaystackRef).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.ToAccount).HasMaxLength(10);

                entity.Property(e => e.Charges).HasColumnType("decimal(10,2)");

                entity.Property(e => e.TransactionRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(2);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("property");

                entity.HasIndex(e => e.Id, "property_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.UniqueId, "property_UniqueId_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.CreatedBy, "property_admin_Email_fk");

                entity.HasIndex(e => e.Type, "property_property_type_Id_fk");

                entity.Property(e => e.Account).HasMaxLength(10);

                entity.Property(e => e.AllowSharing)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Bank).HasMaxLength(100);

                entity.Property(e => e.ClosingDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.InterestRate).HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Latitude).HasMaxLength(100);

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.Longitude).HasMaxLength(100);

                entity.Property(e => e.MaxUnitPerCustomer).HasDefaultValueSql("'1000'");

                entity.Property(e => e.MinimumSharingPercentage).HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectedGrowth).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'ONGOING_CONSTRUCTION'");

                entity.Property(e => e.Summary).IsRequired();

                entity.Property(e => e.TargetYield).HasColumnType("decimal(18,2)");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

                entity.Property(e => e.UniqueId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");

                entity.Property(e => e.VideoLink).HasMaxLength(500);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("property_admin_Email_fk");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("property_property_type_Id_fk");
            });

            modelBuilder.Entity<PropertyType>(entity =>
            {
                entity.ToTable("property_type");

                entity.HasIndex(e => e.Id, "property_type_Id_uindex")
                    .IsUnique();

                entity.Property(e => e.DateCreated).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Propertyupload>(entity =>
            {
                entity.ToTable("propertyuploads");

                entity.HasIndex(e => e.Id, "propertyuploads_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Url, "propertyuploads_Url_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.AdminEmail, "propertyuploads_admin_Email_fk");

                entity.HasIndex(e => e.PropertyId, "propertyuploads_property_Id_fk");

                entity.Property(e => e.AdminEmail)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.DateUploaded).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ImageName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UploadType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'PICTURE'");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.AdminEmailNavigation)
                    .WithMany(p => p.Propertyuploads)
                    .HasForeignKey(d => d.AdminEmail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertyuploads_admin_Email_fk");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertyUploads)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertyuploads_property_Id_fk");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.CreatedBy).HasMaxLength(200);

                entity.Property(e => e.Page).HasColumnType("varchar(5000)");

                entity.Property(e => e.Privilege).HasColumnType("varchar(5000)");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Sharingentry>(entity =>
            {
                entity.ToTable("sharingentry");

                entity.HasIndex(e => e.CustomerId, "sharingentry_CustomerId_index");

                entity.HasIndex(e => e.GroupId, "sharingentry_GroupId_index");

                entity.HasIndex(e => e.Id, "sharingentry_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.IsClosed, "sharingentry_IsClosed_index");

                entity.HasIndex(e => e.PaymentReference, "sharingentry_PaymentReference_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.GroupId, "sharingentry_sharinggroup_UniqueId_fk");

                entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.GroupId)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.IsClosed)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsReversed)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.PaymentReference).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Sharingentries)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sharingentry_customer_Id_fk");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Sharingentries)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sharingentry_sharinggroup_UniqueId_fk");
            });

            modelBuilder.Entity<Sharinggroup>(entity =>
            {
                entity.HasKey(e => e.UniqueId)
                    .HasName("PRIMARY");

                entity.ToTable("sharinggroup");

                entity.HasIndex(e => e.UniqueId, "sharinggroup_UniqueId_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.PropertyId, "sharinggroup_property_Id_fk");

                entity.Property(e => e.UniqueId).HasMaxLength(32);

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.CustomerUniqueId).HasMaxLength(50);

                entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsClosed)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Sharinggroups)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sharinggroup_property_Id_fk");
            });

            modelBuilder.Entity<Thirdpartyclient>(entity =>
            {
                entity.ToTable("thirdpartyclient");

                entity.HasIndex(e => e.ClientName, "td_client_name_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "td_id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.ClientId, "td_client_id_uindex")
                    .IsUnique();

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ClientId)
                    .IsRequired();

                entity.Property(e => e.ClientName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ApiKey)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Thirdpartycustomer>(entity =>
            {
                entity.ToTable("thirdpartycustomer");

                entity.HasIndex(e => e.Email, "tdc_Email_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "tdc_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Phone, "tdc_Phone_uindex")
                    .IsUnique();

                entity.Property(e => e.DateRegistered).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(14);

                entity.Property(e => e.Channel)
                    .IsRequired();
            });

            modelBuilder.Entity<Thirdpartyinvestment>(entity =>
            {
                entity.ToTable("thirdpartyinvestment");

                entity.HasIndex(e => e.Id, "tp_investment_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.CustomerId, "tp_investment_customer_Id_fk");

                entity.HasIndex(e => e.PropertyId, "tp_investment_property_Id_fk");

                entity.HasIndex(e => e.TransactionRef, "tp_investment_transaction_TransactionRef_fk");

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.PaymentType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TransactionRef)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Thirdpartyinvestments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tp_investment_customer_Id_fk");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Thirdpartyinvestments)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tp_investment_property_Id_fk");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionRef)
                    .HasName("PRIMARY");

                entity.ToTable("transaction");

                entity.HasIndex(e => e.Status, "transaction_Status_index");

                entity.HasIndex(e => e.TransactionRef, "transaction_TransactionRef_index");

                entity.HasIndex(e => e.CustomerId, "transaction_customer_Id_fk");

                entity.Property(e => e.TransactionRef).HasMaxLength(50);

                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Channel).HasMaxLength(10);

                entity.Property(e => e.Module).HasMaxLength(50);

                entity.Property(e => e.RequestPayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.ResponsePayload).HasColumnType("varchar(5000)");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TransactionDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TransactionType).HasMaxLength(10);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("transaction_customer_Id_fk");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("wallet");

                entity.HasIndex(e => e.CustomerId, "wallet_CustomerId_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "wallet_Id_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.WalletNo, "wallet_WalletNo_uindex")
                    .IsUnique();

                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");

                entity.Property(e => e.LastTransactionAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Limit).HasColumnType("decimal(18,2)");

                entity.Property(e => e.WalletNo)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.Wallet)
                    .HasForeignKey<Wallet>(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("wallet_customer_Id_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}