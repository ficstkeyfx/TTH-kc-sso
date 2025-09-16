using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata;


#nullable disable

namespace QLNguoiDung.Models
{
    public partial class dbQLNguoiDungContext : DbContext
    {
        public dbQLNguoiDungContext()
        {
        }

        public dbQLNguoiDungContext(DbContextOptions<dbQLNguoiDungContext> options)
            : base(options)
        {
        }

        //Thu thập thông tin làm phần mềm
        public virtual DbSet<tbChucNang> tbChucNangs { get; set; }
        public virtual DbSet<tbChucNang_Nhom> tbChucNang_Nhoms { get; set; }
        public virtual DbSet<tbChucNang_NguoiDung> tbChucNang_NguoiDungs { get; set; }
        public virtual DbSet<tbUser> tbUsers { get; set; }
        public virtual DbSet<tbSystemUser> tbSystemUsers { get; set; }
        public virtual DbSet<tbNhom> tbNhoms { get; set; }
        public virtual DbSet<tbNguoiDung_Nhom> tbNguoiDung_Nhoms { get; set; }
        public virtual DbSet<tbNguoiDung> tbNguoiDungs { get; set; }
        public virtual DbSet<vNguoiDungHeThong> vNguoiDungHeThongs { get; set; }
        public virtual DbSet<vNguoiDungPhanMem> vNguoiDungPhanMems { get; set; }
        // Unable to generate entity type for table 'dbo.Docs' since its primary key could not be scaffolded. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBConnection"));

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbChucNang>(entity =>
            {
                entity.Property(e => e.IdChucNang).ValueGeneratedNever();
            });

            modelBuilder.Entity<tbChucNang_Nhom>(entity =>
            {
                entity.Property(e => e.IdChucNang_Nhom).ValueGeneratedNever();

                entity.HasOne(d => d.IdChucNangNavigation)
                    .WithMany(p => p.tbChucNang_Nhoms)
                    .HasForeignKey(d => d.IdChucNang)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tbChucNang_Nhom_tbChucNang");

            });


            modelBuilder.Entity<tbNhom>(entity =>
            {
                entity.Property(e => e.IdNhom).ValueGeneratedNever();
            });


            modelBuilder.Entity<tbUser>(entity =>
            {
                entity.Property(e => e.IdUser).ValueGeneratedNever();

                entity.Property(e => e.token).IsUnicode(false);
            });

            modelBuilder.HasSequence<int>("dbo.seq_tbVanBanHuongDan", "QLDG");

            modelBuilder.HasSequence<int>("seq_tbChucNang").HasMin(0);
            modelBuilder.HasSequence("seq_tbChucNang_Nhom");
            modelBuilder.HasSequence("seq_tbDuyetNghiPhep");


            modelBuilder.HasSequence<int>("seq_tbDayGioi").StartsAt(4);

            modelBuilder.HasSequence<int>("seq_tbDuyetGiang").StartsAt(2);

            modelBuilder.HasSequence<int>("seq_tbFile");

            modelBuilder.HasSequence<int>("seq_tbGroup")
                .StartsAt(5)
                .HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbGroup_Permission")
                .StartsAt(5)
                .HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbHoiDong").HasMin(0);
            modelBuilder.HasSequence<int>("seq_tbThiHocPhan").HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbHoiDong_File").HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbHoiGiang").HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbKTDayGioi").HasMin(0);

            modelBuilder.HasSequence("seq_tbKTDSDuyetGiang");

            modelBuilder.HasSequence("seq_tbKTDuyetGiang");

            modelBuilder.HasSequence<int>("seq_tbMonHoc", "CTDT");

            modelBuilder.HasSequence("seq_tbNamHoc");

            modelBuilder.HasSequence("seq_tbNhom");

            modelBuilder.HasSequence<int>("seq_tbThanhVienHoiDong").HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbUser")
                .StartsAt(6)
                .HasMin(0);

            modelBuilder.HasSequence<int>("seq_tbUser_Group")
                .StartsAt(6)
                .HasMin(0);
            modelBuilder.HasSequence("seq_tbUser_Nhom");


            modelBuilder.HasSequence("seq_tbVanBanHuongDan");

            modelBuilder.HasSequence<int>("seq_tbXetDanhHieu").HasMin(0);

            modelBuilder.HasSequence("seq_tbThinhGiang");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
