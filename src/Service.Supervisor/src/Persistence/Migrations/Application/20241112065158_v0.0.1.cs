using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Giantnodes.Service.Supervisor.Persistence.Migrations.Application
{
    /// <inheritdoc />
    public partial class v001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "directories",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    path_info_name = table.Column<string>(type: "text", nullable: false),
                    path_info_full_name = table.Column<string>(type: "text", nullable: false),
                    path_info_container = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_path = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_separator_char = table.Column<char>(type: "character(1)", nullable: false),
                    concurrency_token = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_directories", x => x.id);
                    table.ForeignKey(
                        name: "FK_directories_directories_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "directories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "files",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    path_info_name = table.Column<string>(type: "text", nullable: false),
                    path_info_full_name = table.Column<string>(type: "text", nullable: false),
                    path_info_container = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_path = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_separator_char = table.Column<char>(type: "character(1)", nullable: false),
                    concurrency_token = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_files_directories_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "public",
                        principalTable: "directories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "libraries",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "citext", nullable: false),
                    slug = table.Column<string>(type: "citext", nullable: false),
                    directory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    concurrency_token = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_libraries", x => x.id);
                    table.ForeignKey(
                        name: "fk_libraries_directories_directory_id",
                        column: x => x.directory_id,
                        principalSchema: "public",
                        principalTable: "directories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_directories_parent_id",
                schema: "public",
                table: "directories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_directories_path_info_full_name",
                schema: "public",
                table: "directories",
                column: "path_info_full_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_files_parent_id",
                schema: "public",
                table: "files",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_files_path_info_full_name",
                schema: "public",
                table: "files",
                column: "path_info_full_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_libraries_directory_id",
                schema: "public",
                table: "libraries",
                column: "directory_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_libraries_name",
                schema: "public",
                table: "libraries",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_libraries_slug",
                schema: "public",
                table: "libraries",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files",
                schema: "public");

            migrationBuilder.DropTable(
                name: "libraries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "directories",
                schema: "public");
        }
    }
}
