using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PhoneCostApp.Model
{
    public partial class PhoneCostContext : DbContext
    {
        public PhoneCostContext()
        {
        }

        public PhoneCostContext(DbContextOptions<PhoneCostContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Companies { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<PhoneCost> PhoneCosts { get; set; } = null!;

        public virtual DbSet<LogEntry> LogEntries { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=ch-s-0009001.ch.abb.com;Initial Catalog=PhoneCost;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Org1).HasMaxLength(50);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EmployeeName).HasMaxLength(50);
            });

            modelBuilder.Entity<PhoneCost>(entity =>
            {
                entity.ToTable("PhoneCost");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.CustomerCostCenter)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Debtor)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.MobileCalls).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MobileConnection).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ReferencePeriod).HasMaxLength(50);

                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.PhoneCosts)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Company");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.PhoneCosts)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Department");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.PhoneCosts)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_Employee");
            });

            modelBuilder.Entity<LogEntry>(e =>
                         e.ToTable("LogEntry"));

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
