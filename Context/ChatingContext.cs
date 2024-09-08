using System;
using System.Collections.Generic;
using ChatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.Context;

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

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAccount> RoomAccounts { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //=> optionsBuilder.UseSqlServer("Server=(Local);Database=CHATING;Integrated Security=True;Persist Security Info=False;Trust Server Certificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC27E5110524");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__161CF72436614D57").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__B15BE12ED599A3F5").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("LAST_LOGIN");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Roles)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ROLES");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FriendRe__3214EC0722F4A167");

            entity.ToTable("FriendRequest");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Accepted)
                .HasDefaultValue(false)
                .HasColumnName("ACCEPTED");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");
            entity.Property(e => e.User1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER1");
            entity.Property(e => e.User2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER2");

            //entity.HasOne(d => d.User1Navigation).WithMany(p => p.FriendRequestUser1Navigations)
            //    .HasPrincipalKey(p => p.Username)
            //    .HasForeignKey(d => d.User1)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__FriendReq__USER1__43D61337");

            //entity.HasOne(d => d.User2Navigation).WithMany(p => p.FriendRequestUser2Navigations)
            //    .HasPrincipalKey(p => p.Username)
            //    .HasForeignKey(d => d.User2)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__FriendReq__USER2__44CA3770");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC27029A9597");

            entity.ToTable("MESSAGE");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("CONTENT");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("CREATE_AT");
            entity.Property(e => e.DeleteAt)
                .HasColumnType("datetime")
                .HasColumnName("DELETE_AT");
            entity.Property(e => e.Filename)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("FILENAME");
            entity.Property(e => e.RoomId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });
    
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NOTIFICA__3214EC2759190BC5");

            entity.ToTable("NOTIFICATION");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Seen)
                .HasDefaultValue(false)
                .HasColumnName("SEEN");
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("SYSTEM")
                .HasColumnName("SOURCE");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TITLE");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TYPE");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RELATION__3214EC27E7ECF52A");

            entity.ToTable("RELATIONSHIP");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("NORMAL")
                .HasColumnName("TYPE");
            entity.Property(e => e.User1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER1");
            entity.Property(e => e.User2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USER2");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROOM__3214EC2743EF8AC8");

            entity.ToTable("ROOM");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TYPE");
        });

        modelBuilder.Entity<RoomAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROOM_ACC__3214EC278A8A786E");

            entity.ToTable("ROOM_ACCOUNT");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.RoomId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomAccounts)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ROOM_ACCO__ROOM___0A9D95DB");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.RoomAccounts)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.Username)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ROOM_ACCO__USERN__17036CC0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
