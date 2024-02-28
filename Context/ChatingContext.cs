using System;
using System.Collections.Generic;
using ChatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace New.Namespace;

public partial class ChatingContext : DbContext
{
    public ChatingContext()
    {
    }

    public ChatingContext(DbContextOptions<ChatingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
////#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=(Local);Database=CHATING;Integrated Security=True;Persist Security Info=False;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ACCOUNT__3214EC27EA76A363");

            entity.ToTable("ACCOUNT");

            entity.HasIndex(e => e.Emaill, "UQ__ACCOUNT__A6C3B5B180A5791E").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__ACCOUNT__B15BE12E8F6255DC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");
            entity.Property(e => e.Datebirth)
                .HasColumnType("datetime")
                .HasColumnName("DATEBIRTH");
            entity.Property(e => e.DeleteAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("DELETE_AT");
            entity.Property(e => e.Emaill)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMAILL");
            entity.Property(e => e.Password)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Roles)
                .HasMaxLength(50)
                .HasColumnName("ROLES");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MESSAGE__3214EC27EE241C88");

            entity.ToTable("MESSAGE");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");
            entity.Property(e => e.Friendusername)
                .HasMaxLength(100)
                .HasColumnName("FRIENDUSERNAME");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RELATIONSHIP");

            entity.Property(e => e.LevelFriend)
                .HasMaxLength(300)
                .HasColumnName("LEVEL_FRIEND");
            entity.Property(e => e.User1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER1");
            entity.Property(e => e.User2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER2");

            entity.HasOne(d => d.User1Navigation).WithMany()
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.User1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RELATIONSHIP_USER1_FK");

            entity.HasOne(d => d.User2Navigation).WithMany()
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.User2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RELATIONSHIP_USER2_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
