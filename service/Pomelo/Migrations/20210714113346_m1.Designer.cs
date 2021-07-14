﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.Database;

namespace Pomelo.Migrations
{
    [DbContext(typeof(PomeloDbContext))]
    [Migration("20210714113346_m1")]
    partial class m1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("Pomelo.Models.Employee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Pomelo.Models.Project", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Begin")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("End")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Pomelo.Models.TotalPlanning", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("EmployeeId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PerformedHours")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TotalHours")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ProjectId");

                    b.ToTable("TotalPlannings");
                });

            modelBuilder.Entity("Pomelo.Models.Vacation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("AbsentDays")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CalendarWeek")
                        .HasColumnType("INTEGER");

                    b.Property<long>("EmployeeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Vacations");
                });

            modelBuilder.Entity("Pomelo.Models.WeeklyCapacity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("CalendarWeek")
                        .HasColumnType("INTEGER");

                    b.Property<long>("EmployeeId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Hours")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("ProjectId");

                    b.ToTable("WeeklyCapacities");
                });

            modelBuilder.Entity("Pomelo.Models.Project", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Pomelo.Models.TotalPlanning", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Employee")
                        .WithMany("TotalPlannings")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pomelo.Models.Project", "Project")
                        .WithMany("TotalPlannings")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Pomelo.Models.Vacation", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Employee")
                        .WithMany("Vacations")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Pomelo.Models.WeeklyCapacity", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Employee")
                        .WithMany("WeeklyCapacities")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pomelo.Models.Project", "Project")
                        .WithMany("WeeklyCapacities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Pomelo.Models.Employee", b =>
                {
                    b.Navigation("TotalPlannings");

                    b.Navigation("Vacations");

                    b.Navigation("WeeklyCapacities");
                });

            modelBuilder.Entity("Pomelo.Models.Project", b =>
                {
                    b.Navigation("TotalPlannings");

                    b.Navigation("WeeklyCapacities");
                });
#pragma warning restore 612, 618
        }
    }
}
