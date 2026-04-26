using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Variant10.Models;

public partial class ProductContext : DbContext
{
    public ProductContext()
    {
    }

    public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FacturingCompany> FacturingCompanies { get; set; }

    public virtual DbSet<FoodProduct> FoodProducts { get; set; }

    public virtual DbSet<FoodProduction> FoodProductions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=products;Username=postgres; Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FacturingCompany>(entity =>
        {
            entity.HasKey(e => e.firmId).HasName("FacturingCompanies_pkey");

            entity.Property(e => e.firmId)
                .ValueGeneratedNever()
                .HasColumnName("Firm id");
            entity.Property(e => e.adress)
                .HasMaxLength(100)
                .HasColumnName("Adress");
            entity.Property(e => e.directorSurname)
                .HasMaxLength(100)
                .HasColumnName("Director surname");
            entity.Property(e => e.firmName)
                .HasMaxLength(100)
                .HasColumnName("Firm name");
        });

        modelBuilder.Entity<FoodProduct>(entity =>
        {
            entity.HasKey(e => e.id).HasName("FoodProducts_pkey");

            entity.Property(e => e.id)
                .ValueGeneratedNever()
                .HasColumnName("Id");
            entity.Property(e => e.packageType)
                .HasMaxLength(100)
                .HasColumnName("Package type");
            entity.Property(e => e.productGroup)
                .HasMaxLength(100)
                .HasColumnName("Product group");
            entity.Property(e => e.title)
                .HasMaxLength(100)
                .HasColumnName("Title");
        });

        modelBuilder.Entity<FoodProduction>(entity =>
        {
            entity.HasKey(e => new { e.firmId, e.productId, e.productionVolume }).HasName("FoodProduction_pkey");

            entity.ToTable("FoodProduction");

            entity.Property(e => e.productionVolume).HasColumnName("Production volume");
            entity.Property(e => e.productId).HasColumnName("Product id");
            entity.Property(e => e.firmId).HasColumnName("Firm id");

            entity.HasOne(d => d.firmIdNavigation).WithMany(p => p.FoodProductions)
                .HasForeignKey(d => d.firmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK-Firmid-Firmid");

            entity.HasOne(d => d.productIdNavigation).WithMany(p => p.FoodProductions)
                .HasForeignKey(d => d.productId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK-Productid-Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
