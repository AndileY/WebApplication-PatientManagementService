﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WpfApp6.Models;

public partial class PMSContext : DbContext
{
    public PMSContext()
    {
    }

    public PMSContext(DbContextOptions<PMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblDisease> TblDisease { get; set; }

    public virtual DbSet<TblDrugs> TblDrugs { get; set; }

    public virtual DbSet<TblLoginTry> TblLoginTry { get; set; }

    public virtual DbSet<TblTreatment> TblTreatment { get; set; }

    public virtual DbSet<TblUser> TblUser { get; set; }

    public virtual DbSet<TblUserType> TblUserType { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=DESKTOP-NCFE678; database=PMS; integrated security=SSPI; persist security info=FALSE; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblDisease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK_Disease");

            entity.Property(e => e.DiseaseName).IsFixedLength();
        });

        modelBuilder.Entity<TblDrugs>(entity =>
        {
            entity.Property(e => e.DrugName).IsFixedLength();
        });

        modelBuilder.Entity<TblTreatment>(entity =>
        {
            entity.HasMany(d => d.Drug).WithMany(p => p.Treatment)
                .UsingEntity<Dictionary<string, object>>(
                    "TblPrescriptions",
                    r => r.HasOne<TblDrugs>().WithMany()
                        .HasForeignKey("DrugId")
                        .HasConstraintName("FK_tblPrescriptions_tblDrugs"),
                    l => l.HasOne<TblTreatment>().WithMany()
                        .HasForeignKey("TreatmentId")
                        .HasConstraintName("FK_tblPrescriptions_tblTreatment"),
                    j =>
                    {
                        j.HasKey("TreatmentId", "DrugId");
                        j.ToTable("tblPrescriptions");
                    });
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.Property(e => e.FirstName).IsFixedLength();
            entity.Property(e => e.LastName).IsFixedLength();
            entity.Property(e => e.RegisterDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.SaltOfPw).IsFixedLength();
            entity.Property(e => e.UserPw).IsFixedLength();

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.TblUser).HasConstraintName("FK_tblUser_tblUser");
        });

        modelBuilder.Entity<TblUserType>(entity =>
        {
            entity.Property(e => e.UserTypeName).IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}