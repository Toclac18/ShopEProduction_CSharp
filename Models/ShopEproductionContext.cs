﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopEProduction.Models;

public partial class ShopEproductionContext : DbContext
{
    public ShopEproductionContext()
    {
    }

    public ShopEproductionContext(DbContextOptions<ShopEproductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<PurchaseHistory> PurchaseHistories { get; set; }

    public virtual DbSet<PurchaseHistoryDetail> PurchaseHistoryDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WalletHistory> WalletHistories { get; set; }

    public virtual DbSet<WalletHistoryDetail> WalletHistoryDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=QuanNT18;database=ShopEProduction;uid=sa;pwd=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CARTS__3214EC270A1D37E9");

            entity.ToTable("CARTS");

            entity.HasIndex(e => e.UserId, "UQ__CARTS__F3BEEBFE2BCACCB6").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CARTS__USER_ID__17036CC0");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CART_ITE__3214EC2765EE5BC8");

            entity.ToTable("CART_ITEMS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CART_ID");
            entity.Property(e => e.ProductDetailId).HasColumnName("PRODUCT_DETAIL_ID");
            entity.Property(e => e.ProductDetailName)
                .HasMaxLength(255)
                .HasColumnName("PRODUCT_DETAIL_NAME");
            entity.Property(e => e.ProductDetailPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRODUCT_DETAIL_PRICE");
            entity.Property(e => e.ProductId).HasColumnName("PRODUCT_ID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CART_ITEM__CART___2DE6D218");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CART_ITEM__PRODU__2FCF1A8A");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CART_ITEM__PRODU__2EDAF651");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CATEGORI__3214EC27C4AE8295");

            entity.ToTable("CATEGORIES");

            entity.HasIndex(e => e.CategoryName, "UQ__CATEGORI__9374460F75B625C7").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__PRODUCTS__3214EC27CEBFA887");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CATEGORY_ID");
            entity.Property(e => e.CurrentAvailable).HasColumnName("CURRENT_AVAILABLE");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.SoldNumber).HasColumnName("SOLD_NUMBER");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("STATUS");
            entity.Property(e => e.Type).HasColumnName("TYPE");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCTS__CATEGO__5DCAEF64");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCT___3214EC278BFA915D");

            entity.ToTable("PRODUCT_DETAILS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DetailDesc)
                .HasMaxLength(255)
                .HasColumnName("DETAIL_DESC");
            entity.Property(e => e.DetailPrivateDesc)
                .HasMaxLength(255)
                .HasColumnName("DETAIL_PRIVATE_DESC");
            entity.Property(e => e.Duration)
                .HasColumnType("datetime")
                .HasColumnName("DURATION");
            entity.Property(e => e.ExpiredDate)
                .HasColumnType("datetime")
                .HasColumnName("EXPIRED_DATE");
            entity.Property(e => e.IsBoughtFlg)
                .HasDefaultValue(false)
                .HasColumnName("IS_BOUGHT_FLG");
            entity.Property(e => e.IsRentedFlg)
                .HasDefaultValue(false)
                .HasColumnName("IS_RENTED_FLG");
            entity.Property(e => e.Price).HasColumnName("PRICE");
            entity.Property(e => e.ProductId).HasColumnName("PRODUCT_ID");
            entity.Property(e => e.ProductType).HasColumnName("PRODUCT_TYPE");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("datetime")
                .HasColumnName("RELEASE_DATE");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__PRODUCT_D__PRODU__693CA210");
        });

        modelBuilder.Entity<PurchaseHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PURCHASE__3214EC272385F9C3");

            entity.ToTable("PURCHASE_HISTORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CART_ID");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("PURCHASE_DATE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.Cart).WithMany(p => p.PurchaseHistories)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK_PurchaseHistory_Carts");

            entity.HasOne(d => d.User).WithMany(p => p.PurchaseHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PurchaseHistory_Users");
        });

        modelBuilder.Entity<PurchaseHistoryDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PURCHASE__3214EC27E08E37B7");

            entity.ToTable("PURCHASE_HISTORY_DETAILS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.HistoryId).HasColumnName("HISTORY_ID");
            entity.Property(e => e.IsBoughtFlg).HasColumnName("IS_BOUGHT_FLG");
            entity.Property(e => e.IsRentedFlg).HasColumnName("IS_RENTED_FLG");
            entity.Property(e => e.PriceAtPurchase).HasColumnName("PRICE_AT_PURCHASE");
            entity.Property(e => e.ProductDetailId).HasColumnName("PRODUCT_DETAIL_ID");

            entity.HasOne(d => d.History).WithMany(p => p.PurchaseHistoryDetails)
                .HasForeignKey(d => d.HistoryId)
                .HasConstraintName("FK_PurchaseHistoryDetails_PurchaseHistory");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.PurchaseHistoryDetails)
                .HasForeignKey(d => d.ProductDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseHistoryDetails_ProductDetails");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROLES__3214EC278AB87615");

            entity.ToTable("ROLES");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3214EC2765C0367C");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UQ__USERS__161CF7244C624A65").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__USERS__B15BE12ED584591E").IsUnique();

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

        modelBuilder.Entity<WalletHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WALLET_H__3214EC2794D6499A");

            entity.ToTable("WALLET_HISTORY");

            entity.HasIndex(e => e.UserId, "UQ_WalletHistory_UserId").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CurrentBalance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CURRENT_BALANCE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithOne(p => p.WalletHistory)
                .HasForeignKey<WalletHistory>(d => d.UserId)
                .HasConstraintName("FK_WalletHistory_Users");
        });

        modelBuilder.Entity<WalletHistoryDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WALLET_H__3214EC2707BF033F");

            entity.ToTable("WALLET_HISTORY_DETAILS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChangeAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CHANGE_AMOUNT");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.HistoryId).HasColumnName("HISTORY_ID");
            entity.Property(e => e.HistoryType)
                .HasMaxLength(3)
                .HasColumnName("HISTORY_TYPE");
            entity.Property(e => e.PostValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("POST_VALUE");
            entity.Property(e => e.PreValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PRE_VALUE");
            entity.Property(e => e.PurchaseDetailId).HasColumnName("PURCHASE_DETAIL_ID");
            entity.Property(e => e.TimeExecution)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TIME_EXECUTION");

            entity.HasOne(d => d.History).WithMany(p => p.WalletHistoryDetails)
                .HasForeignKey(d => d.HistoryId)
                .HasConstraintName("FK_WALLET_HISTORY");

            entity.HasOne(d => d.PurchaseDetail).WithMany(p => p.WalletHistoryDetails)
                .HasForeignKey(d => d.PurchaseDetailId)
                .HasConstraintName("FK_PURCHASE_HISTORY_DETAILS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
