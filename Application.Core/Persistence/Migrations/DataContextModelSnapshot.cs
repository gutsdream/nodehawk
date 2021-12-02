﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Application.Core.Persistence;

namespace Persistence.Migrations
{
    [DbContext(typeof(Application.Core.Persistence.DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Domain.Entities.AwsDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("SecretKey")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AwsDetails");
                });

            modelBuilder.Entity("Domain.Entities.ConnectionDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Host")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ConnectionDetails");
                });

            modelBuilder.Entity("Domain.Entities.Node", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ConnectionDetailsId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionDetailsId")
                        .IsUnique();

                    b.ToTable("Nodes");
                });

            modelBuilder.Entity("Domain.Entities.Node+Snapshot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("ContainerRunning")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("TEXT");

                    b.Property<int>("SpaceUsedPercentage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("NodeId");

                    b.ToTable("NodeSnapshots");
                });

            modelBuilder.Entity("Domain.Entities.Node", b =>
                {
                    b.HasOne("Domain.Entities.ConnectionDetails", "ConnectionDetails")
                        .WithOne("Node")
                        .HasForeignKey("Domain.Entities.Node", "ConnectionDetailsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ConnectionDetails");
                });

            modelBuilder.Entity("Domain.Entities.Node+Snapshot", b =>
                {
                    b.HasOne("Domain.Entities.Node", "Node")
                        .WithMany("Snapshots")
                        .HasForeignKey("NodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Node");
                });

            modelBuilder.Entity("Domain.Entities.ConnectionDetails", b =>
                {
                    b.Navigation("Node");
                });

            modelBuilder.Entity("Domain.Entities.Node", b =>
                {
                    b.Navigation("Snapshots");
                });
#pragma warning restore 612, 618
        }
    }
}