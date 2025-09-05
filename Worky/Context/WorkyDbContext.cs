using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Worky.Migrations;


namespace Worky.Context;

public partial class WorkyDbContext : IdentityDbContext<Users, Roles, string,
    UserClaims, UserRoles, UserLogins, RoleClaims, UserTokens>
{
    public WorkyDbContext()
    {
    }

    public WorkyDbContext(DbContextOptions<WorkyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Roles> AspNetRoles { get; set; }

    public virtual DbSet<RoleClaims> AspNetRoleClaims { get; set; }

    public virtual DbSet<Users> AspNetUsers { get; set; }

    public virtual DbSet<UserClaims> AspNetUserClaims { get; set; }

    public virtual DbSet<UserLogins> AspNetUserLogins { get; set; }

    public virtual DbSet<UserTokens> AspNetUserTokens { get; set; }

    public virtual DbSet<UserRoles> AspNetUserRoles { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Manager_filter> Manager_filters { get; set; }

    public virtual DbSet<RecruitManager> RecruitManagers { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Resume_filter> Resume_filters { get; set; }

    public virtual DbSet<Tarrif> Tarrifs { get; set; }

    public virtual DbSet<Vacancy> Vacancies { get; set; }

    public virtual DbSet<Vacancy_filter> Vacancy_filters { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    public virtual DbSet<__EFMigrationsHistory> __EFMigrationsHistories { get; set; }

    public virtual DbSet<Company> companies { get; set; }

    public virtual DbSet<TypeOfActivity> typeOfActivities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=Worky;user=mac;password=123",
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.6.2-mariadb"), x => x.UseNetTopologySuite());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.ToTable("AspNetRoles");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id)
                .HasMaxLength(450)
                .IsRequired();

            entity.Property(e => e.Name)
                .HasMaxLength(256);

            entity.Property(e => e.NormalizedName)
                .HasMaxLength(256);

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique();
        });

        modelBuilder.Entity<RoleClaims>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<UserClaims>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
        });

        modelBuilder.Entity<UserTokens>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
        });

        modelBuilder.Entity<UserRoles>(entity =>
        {
            entity.ToTable("AspNetUserRoles");

            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.company).WithMany(p => p.Deals).HasConstraintName("deal_ibfk_2");

            entity.HasOne(d => d.tariff).WithMany(p => p.Deals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("deal_ibfk_1");
        });

        modelBuilder.Entity<Education>(entity => { entity.HasKey(e => e.id).HasName("PRIMARY"); });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.resume).WithMany(p => p.Feedbacks).HasConstraintName("feedback_ibfk_1");

            entity.HasOne(d => d.vacancy).WithMany(p => p.Feedbacks).HasConstraintName("feedback_ibfk_2");
        });

        modelBuilder.Entity<Manager>(entity => { entity.HasKey(e => e.id).HasName("PRIMARY"); });

        modelBuilder.Entity<Manager_filter>(entity =>
        {
            entity.HasKey(e => e.filter_id).HasName("PRIMARY");

            entity.HasOne(d => d.manager).WithMany(p => p.Manager_filters).HasConstraintName("manager_filter_ibfk_1");

            entity.HasOne(d => d.typeOfActivity).WithMany(p => p.Manager_filters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("manager_filter_ibfk_2");
        });

        modelBuilder.Entity<RecruitManager>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasOne(d => d.company).WithMany(p => p.RecruitManagers).HasConstraintName("recruitmanager_ibfk_2");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.income_date).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.education).WithMany(p => p.Resumes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("resume_ibfk_2");

            entity.HasOne(d => d.worker).WithMany(p => p.Resumes).HasConstraintName("worker_ibfk_1");
        });

        modelBuilder.Entity<Resume_filter>(entity =>
        {
            entity.HasKey(e => e.filter_id).HasName("PRIMARY");

            entity.HasOne(d => d.resume).WithMany(p => p.Resume_filters).HasConstraintName("resume_filter_ibfk_1");

            entity.HasOne(d => d.typeOfActivity).WithMany(p => p.Resume_filters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resume_filter_ibfk_2");
        });

        modelBuilder.Entity<Tarrif>(entity => { entity.HasKey(e => e.id).HasName("PRIMARY"); });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.income_date).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.company).WithMany(p => p.Vacancies).HasConstraintName("vacancy_ibfk_1");

            entity.HasOne(d => d.education).WithMany(p => p.Vacancies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vacancy_ibfk_2");
        });

        modelBuilder.Entity<Vacancy_filter>(entity =>
        {
            entity.HasKey(e => e.filter_id).HasName("PRIMARY");

            entity.HasOne(d => d.typeOfActivity).WithMany(p => p.Vacancy_filters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vacancy_filter_ibfk_2");

            entity.HasOne(d => d.vacancy).WithMany(p => p.Vacancy_filters).HasConstraintName("vacancy_filter_ibfk_1");
        });

        modelBuilder.Entity<Worker>(entity => { entity.HasKey(e => e.id).HasName("PRIMARY"); });

        modelBuilder.Entity<__EFMigrationsHistory>(entity => { entity.HasKey(e => e.MigrationId).HasName("PRIMARY"); });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.office_coord, "idx_coord")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 32 })
                .HasAnnotation("MySql:SpatialIndex", true);

            entity.HasIndex(e => e.name, "idx_name").HasAnnotation("MySql:FullTextIndex", true);
        });

        modelBuilder.Entity<TypeOfActivity>(entity => { entity.HasKey(e => e.id).HasName("PRIMARY"); });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}