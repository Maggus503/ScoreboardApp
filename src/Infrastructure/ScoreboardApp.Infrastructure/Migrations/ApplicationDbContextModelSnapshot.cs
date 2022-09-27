﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ScoreboardApp.Infrastructure.Persistence;

#nullable disable

namespace ScoreboardApp.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.CompletionHabit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<Guid>("HabitTrackerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("HabitTrackerId");

                    b.ToTable("CompletionHabits");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.CompletionHabitEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Completion")
                        .HasColumnType("bit");

                    b.Property<DateTime>("EntryDate")
                        .HasColumnType("date");

                    b.Property<Guid>("HabitId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("HabitId");

                    b.ToTable("CompletionHabitEntries");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.EffortHabit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("AverageGoal")
                        .HasColumnType("float");

                    b.Property<string>("Description")
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<Guid>("HabitTrackerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Subtype")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Unit")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("HabitTrackerId");

                    b.ToTable("EffortHabits");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.EffortHabitEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Effort")
                        .HasColumnType("float");

                    b.Property<DateTime>("EntryDate")
                        .HasColumnType("date");

                    b.Property<Guid>("HabitId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("SessionGoal")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("HabitId");

                    b.ToTable("EffortHabitEntries");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.HabitTracker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("HabitTrackers");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.CompletionHabit", b =>
                {
                    b.HasOne("ScoreboardApp.Domain.Entities.HabitTracker", "HabitTracker")
                        .WithMany("CompletionHabits")
                        .HasForeignKey("HabitTrackerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HabitTracker");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.CompletionHabitEntry", b =>
                {
                    b.HasOne("ScoreboardApp.Domain.Entities.CompletionHabit", "Habit")
                        .WithMany("HabitEntries")
                        .HasForeignKey("HabitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Habit");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.EffortHabit", b =>
                {
                    b.HasOne("ScoreboardApp.Domain.Entities.HabitTracker", "HabitTracker")
                        .WithMany("EffortHabits")
                        .HasForeignKey("HabitTrackerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HabitTracker");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.EffortHabitEntry", b =>
                {
                    b.HasOne("ScoreboardApp.Domain.Entities.EffortHabit", "Habit")
                        .WithMany("HabitEntries")
                        .HasForeignKey("HabitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Habit");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.CompletionHabit", b =>
                {
                    b.Navigation("HabitEntries");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.EffortHabit", b =>
                {
                    b.Navigation("HabitEntries");
                });

            modelBuilder.Entity("ScoreboardApp.Domain.Entities.HabitTracker", b =>
                {
                    b.Navigation("CompletionHabits");

                    b.Navigation("EffortHabits");
                });
#pragma warning restore 612, 618
        }
    }
}