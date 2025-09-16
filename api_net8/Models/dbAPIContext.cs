using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata;
using api.Models;
using Minio.DataModel;
using System.Threading;


#nullable disable

namespace api.Models
{
    public partial class dbAPIContext : DbContext
    {
        public dbAPIContext()
        {
        }

        public dbAPIContext(DbContextOptions<dbAPIContext> options)
            : base(options)
        {
        }
        public virtual DbSet<tbChucNang> tbChucNangs { get; set; }
        public virtual DbSet<tbChucNang_Nhom> tbChucNang_Nhoms { get; set; }
        public virtual DbSet<tbUser> tbUsers { get; set; }
        public virtual DbSet<tbSystemUser> tbSystemUsers { get; set; }
        public virtual DbSet<tbNhom> tbNhoms { get; set; }
        public virtual DbSet<tbNguoiDung_Nhom> tbNguoiDung_Nhoms { get; set; }
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
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("dbAcountConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
