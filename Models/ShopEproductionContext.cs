using Microsoft.EntityFrameworkCore;

namespace ShopEProduction.Models;

public partial class ShopEProductionContext : DbContext
{
    public ShopEProductionContext()
    {
    }

    public ShopEProductionContext(DbContextOptions<ShopEProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=QuanNT18;Database=ShopEProduction;User Id=sa;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CATEGORI__3214EC27A741D1B2");

            entity.ToTable("CATEGORIES");

            entity.HasIndex(e => e.CategoryName, "UQ__CATEGORI__9374460FB98C84A1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CATEGORY_NAME");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCTS__3214EC27188733DB");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.ProductCategory).HasColumnName("PRODUCT_CATEGORY");
            entity.Property(e => e.SoldNumber)
                .HasDefaultValue(0)
                .HasColumnName("SOLD_NUMBER");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.ProductCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PRODUCT_CATEGORY");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCT___3214EC27DF570A44");

            entity.ToTable("PRODUCT_DETAILS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DetailDesc)
                .HasMaxLength(255)
                .HasColumnName("DETAIL_DESC");
            entity.Property(e => e.ProductId).HasColumnName("PRODUCT_ID");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PRODUCT_D__PRODU__534D60F1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROLES__3214EC2729055C86");

            entity.ToTable("ROLES");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3214EC27F7D68D50");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__161CF7246F6A11E7").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__USERS__B15BE12E0E612F1C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
            entity.Property(e => e.UserCreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("USER_CREATE_AT");
            entity.Property(e => e.UserImage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USER_IMAGE");
            entity.Property(e => e.UserPoint).HasColumnName("USER_POINT");
            entity.Property(e => e.UserRoleId).HasColumnName("USER_ROLE_ID");
            entity.Property(e => e.UserStatus).HasColumnName("USER_STATUS");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .HasConstraintName("FK__USERS__USER_ROLE__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
