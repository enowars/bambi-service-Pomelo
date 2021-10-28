﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pomelo.Database;

namespace Pomelo.Migrations
{
    [DbContext(typeof(PomeloDbContext))]
    partial class PomeloDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

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

                    b.HasIndex("Department");

                    b.HasIndex("Name");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Pomelo.Models.EmployeeProjectHours", b =>
                {
                    b.Property<long>("EmployeeId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("DeliveredHours")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TotalHours")
                        .HasColumnType("INTEGER");

                    b.HasKey("EmployeeId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("EmployeeProjectHours");
                });

            modelBuilder.Entity("Pomelo.Models.EmployeeProjectWeeklyCapacity", b =>
                {
                    b.Property<long>("EmployeeId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ProjectId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Start")
                        .HasColumnType("TEXT");

                    b.Property<long>("Capacity")
                        .HasColumnType("INTEGER");

                    b.HasKey("EmployeeId", "ProjectId", "Start");

                    b.HasIndex("ProjectId");

                    b.ToTable("EmployeeProjectWeeklyCapacities");
                });

            modelBuilder.Entity("Pomelo.Models.Project", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Begin")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DeliveredHoursTimestamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Department")
                        .IsRequired()
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

            modelBuilder.Entity("Pomelo.Models.EmployeeProjectHours", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Employee")
                        .WithMany("EmployeeProjectHours")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pomelo.Models.Project", "Project")
                        .WithMany("EmployeeProjectHours")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Pomelo.Models.EmployeeProjectWeeklyCapacity", b =>
                {
                    b.HasOne("Pomelo.Models.Employee", "Employee")
                        .WithMany("EmployeeProjectWeeklyCapacities")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pomelo.Models.Project", "Project")
                        .WithMany("EmployeeProjectWeeklyCapacities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
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

            modelBuilder.Entity("Pomelo.Models.Employee", b =>
                {
                    b.Navigation("EmployeeProjectHours");

                    b.Navigation("EmployeeProjectWeeklyCapacities");
                });

            modelBuilder.Entity("Pomelo.Models.Project", b =>
                {
                    b.Navigation("EmployeeProjectHours");

                    b.Navigation("EmployeeProjectWeeklyCapacities");
                });
#pragma warning restore 612, 618
        }
    }
}
