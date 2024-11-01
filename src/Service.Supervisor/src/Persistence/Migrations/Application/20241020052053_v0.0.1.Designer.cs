﻿// <auto-generated />
using System;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Giantnodes.Service.Supervisor.Persistence.Migrations.Application
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241020052053_v0.0.1")]
    partial class v001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.FileSystemEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("concurrency_token");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid?>("FileSystemDirectoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_system_directory_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("FileSystemDirectoryId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Library", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("concurrency_token");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("DirectoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("directory_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("name");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("slug");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_libraries");

                    b.HasIndex("DirectoryId")
                        .HasDatabaseName("ix_libraries_directory_id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_libraries_name");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_libraries_slug");

                    b.ToTable("libraries", "public");
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories.FileSystemDirectory", b =>
                {
                    b.HasBaseType("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.FileSystemEntry");

                    b.ToTable("directories", "public");
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files.FileSystemFile", b =>
                {
                    b.HasBaseType("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.FileSystemEntry");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.ToTable("files", "public");
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.FileSystemEntry", b =>
                {
                    b.HasOne("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories.FileSystemDirectory", null)
                        .WithMany("Entries")
                        .HasForeignKey("FileSystemDirectoryId");
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Library", b =>
                {
                    b.HasOne("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories.FileSystemDirectory", "Directory")
                        .WithMany()
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_libraries_directories_directory_id");

                    b.Navigation("Directory");
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories.FileSystemDirectory", b =>
                {
                    b.OwnsOne("Giantnodes.Service.Supervisor.Domain.Values.PathInfo", "PathInfo", b1 =>
                        {
                            b1.Property<Guid>("FileSystemDirectoryId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<byte[]>("ConcurrencyToken")
                                .IsConcurrencyToken()
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("bytea")
                                .HasColumnName("concurrency_token");

                            b1.Property<string>("DirectoryPath")
                                .HasColumnType("text")
                                .HasColumnName("path_info_directory_path");

                            b1.Property<char>("DirectorySeparatorChar")
                                .HasColumnType("character(1)")
                                .HasColumnName("path_info_directory_separator_char");

                            b1.Property<string>("Extension")
                                .HasColumnType("text")
                                .HasColumnName("path_info_extension");

                            b1.Property<string>("FullName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("path_info_full_name");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("path_info_name");

                            b1.HasKey("FileSystemDirectoryId");

                            b1.ToTable("directories", "public");

                            b1.WithOwner()
                                .HasForeignKey("FileSystemDirectoryId")
                                .HasConstraintName("fk_directories_directories_id");
                        });

                    b.Navigation("PathInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files.FileSystemFile", b =>
                {
                    b.OwnsOne("Giantnodes.Service.Supervisor.Domain.Values.PathInfo", "PathInfo", b1 =>
                        {
                            b1.Property<Guid>("FileSystemFileId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<byte[]>("ConcurrencyToken")
                                .IsConcurrencyToken()
                                .ValueGeneratedOnAddOrUpdate()
                                .HasColumnType("bytea")
                                .HasColumnName("concurrency_token");

                            b1.Property<string>("DirectoryPath")
                                .HasColumnType("text")
                                .HasColumnName("path_info_directory_path");

                            b1.Property<char>("DirectorySeparatorChar")
                                .HasColumnType("character(1)")
                                .HasColumnName("path_info_directory_separator_char");

                            b1.Property<string>("Extension")
                                .HasColumnType("text")
                                .HasColumnName("path_info_extension");

                            b1.Property<string>("FullName")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("path_info_full_name");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("path_info_name");

                            b1.HasKey("FileSystemFileId");

                            b1.ToTable("files", "public");

                            b1.WithOwner()
                                .HasForeignKey("FileSystemFileId")
                                .HasConstraintName("fk_files_files_id");
                        });

                    b.Navigation("PathInfo")
                        .IsRequired();
                });

            modelBuilder.Entity("Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories.FileSystemDirectory", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
