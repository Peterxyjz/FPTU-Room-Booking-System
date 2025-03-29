using System;
using System.Collections.Generic;
using FPTURoomBooking.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FPTURoomBooking.Repositories.Data;

public partial class FpturoomBookingContext : DbContext
{
    public FpturoomBookingContext()
    {
    }

    public FpturoomBookingContext(DbContextOptions<FpturoomBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomEquipment> RoomEquipments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnectionString"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC07E93CFD87");

            entity.ToTable("Booking", tb =>
                {
                    tb.HasTrigger("TR_Booking_CreateNotification");
                    tb.HasTrigger("TR_Booking_PreventConflict");
                    tb.HasTrigger("TR_Booking_UpdateRoomStatus");
                });

            entity.HasIndex(e => e.RoomId, "IX_Booking_RoomId");

            entity.HasIndex(e => e.StartTime, "IX_Booking_StartTime");

            entity.HasIndex(e => e.Status, "IX_Booking_Status");

            entity.HasIndex(e => e.UserId, "IX_Booking_UserId");

            entity.Property(e => e.CheckInTime).HasColumnType("datetime");
            entity.Property(e => e.CheckOutTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Purpose).HasMaxLength(255);
            entity.Property(e => e.RecurringPattern).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ duyệt");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Room");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Building__3214EC07D40BC5E6");

            entity.ToTable("Building");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Equipmen__3214EC07D40B23BF");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07C018F6DA");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.IsRead, "IX_Notification_IsRead");

            entity.HasIndex(e => e.UserId, "IX_Notification_UserId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07E15EF1E9");

            entity.ToTable("Role");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Room__3214EC075B8F4B60");

            entity.ToTable("Room");

            entity.HasIndex(e => e.BuildingId, "IX_Room_BuildingId");

            entity.HasIndex(e => e.Status, "IX_Room_Status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Sẵn sàng");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Building).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Building");
        });

        modelBuilder.Entity<RoomEquipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoomEqui__3214EC07057325A9");

            entity.ToTable("RoomEquipment");

            entity.HasIndex(e => e.EquipmentId, "IX_RoomEquipment_EquipmentId");

            entity.HasIndex(e => e.RoomId, "IX_RoomEquipment_RoomId");

            entity.HasIndex(e => new { e.RoomId, e.EquipmentId }, "UQ_RoomEquipment").IsUnique();

            entity.Property(e => e.LastMaintenance).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Equipment).WithMany(p => p.RoomEquipments)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomEquipment_Equipment");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomEquipments)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomEquipment_Room");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0704D2A092");

            entity.ToTable("User");

            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E47D67E605").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105347FBA8F46").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
