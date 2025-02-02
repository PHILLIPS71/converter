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
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:ltree", ",,");

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
                    scanned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    path_info_name = table.Column<string>(type: "citext", nullable: false),
                    path_info_full_name = table.Column<string>(type: "citext", nullable: false),
                    path_info_full_name_normalized = table.Column<string>(type: "ltree", nullable: false),
                    path_info_container = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_path = table.Column<string>(type: "citext", nullable: true),
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
                name: "pipelines",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "citext", nullable: false),
                    slug = table.Column<string>(type: "citext", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    definition = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    concurrency_token = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pipelines", x => x.id);
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
                    path_info_name = table.Column<string>(type: "citext", nullable: false),
                    path_info_full_name = table.Column<string>(type: "citext", nullable: false),
                    path_info_full_name_normalized = table.Column<string>(type: "ltree", nullable: false),
                    path_info_container = table.Column<string>(type: "text", nullable: true),
                    path_info_directory_path = table.Column<string>(type: "citext", nullable: true),
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
                    is_monitoring = table.Column<bool>(type: "boolean", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "audio_stream",
                schema: "public",
                columns: table => new
                {
                    file_system_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    language = table.Column<string>(type: "text", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    bitrate = table.Column<long>(type: "bigint", nullable: false),
                    sample_rate = table.Column<int>(type: "integer", nullable: false),
                    channels = table.Column<int>(type: "integer", nullable: false),
                    @default = table.Column<bool>(name: "default", type: "boolean", nullable: false),
                    forced = table.Column<bool>(type: "boolean", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    codec = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audio_stream", x => new { x.file_system_file_id, x.id });
                    table.ForeignKey(
                        name: "fk_audio_stream_files_file_system_file_id",
                        column: x => x.file_system_file_id,
                        principalSchema: "public",
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pipeline_executions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pipeline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    definition = table.Column<string>(type: "text", nullable: false),
                    context = table.Column<string>(type: "text", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    failure_reason = table.Column<string>(type: "citext", nullable: true),
                    failure_failed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pipeline_executions", x => x.id);
                    table.ForeignKey(
                        name: "fk_pipeline_executions_files_file_id",
                        column: x => x.file_id,
                        principalSchema: "public",
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pipeline_executions_pipelines_pipeline_id",
                        column: x => x.pipeline_id,
                        principalSchema: "public",
                        principalTable: "pipelines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subtitle_stream",
                schema: "public",
                columns: table => new
                {
                    file_system_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    language = table.Column<string>(type: "text", nullable: true),
                    @default = table.Column<bool>(name: "default", type: "boolean", nullable: false),
                    forced = table.Column<bool>(type: "boolean", nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    codec = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subtitle_stream", x => new { x.file_system_file_id, x.id });
                    table.ForeignKey(
                        name: "fk_subtitle_stream_files_file_system_file_id",
                        column: x => x.file_system_file_id,
                        principalSchema: "public",
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "video_stream",
                schema: "public",
                columns: table => new
                {
                    file_system_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quality_width = table.Column<int>(type: "integer", nullable: false),
                    quality_height = table.Column<int>(type: "integer", nullable: false),
                    quality_aspect_ratio = table.Column<string>(type: "text", nullable: false),
                    quality_resolution = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    bitrate = table.Column<long>(type: "bigint", nullable: false),
                    framerate = table.Column<double>(type: "double precision", nullable: false),
                    pixel_format = table.Column<string>(type: "text", nullable: false),
                    @default = table.Column<bool>(name: "default", type: "boolean", nullable: false),
                    forced = table.Column<bool>(type: "boolean", nullable: false),
                    rotation = table.Column<int>(type: "integer", nullable: true),
                    index = table.Column<int>(type: "integer", nullable: false),
                    codec = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_video_stream", x => new { x.file_system_file_id, x.id });
                    table.ForeignKey(
                        name: "fk_video_stream_files_file_system_file_id",
                        column: x => x.file_system_file_id,
                        principalSchema: "public",
                        principalTable: "files",
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
                name: "ix_directories_path_info_full_name_normalized",
                schema: "public",
                table: "directories",
                column: "path_info_full_name_normalized")
                .Annotation("Npgsql:IndexMethod", "gist");

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
                name: "ix_files_path_info_full_name_normalized",
                schema: "public",
                table: "files",
                column: "path_info_full_name_normalized")
                .Annotation("Npgsql:IndexMethod", "gist");

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

            migrationBuilder.CreateIndex(
                name: "ix_pipeline_executions_file_id",
                schema: "public",
                table: "pipeline_executions",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_pipeline_executions_pipeline_id",
                schema: "public",
                table: "pipeline_executions",
                column: "pipeline_id");

            migrationBuilder.CreateIndex(
                name: "ix_pipelines_name",
                schema: "public",
                table: "pipelines",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pipelines_slug",
                schema: "public",
                table: "pipelines",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audio_stream",
                schema: "public");

            migrationBuilder.DropTable(
                name: "libraries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pipeline_executions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "subtitle_stream",
                schema: "public");

            migrationBuilder.DropTable(
                name: "video_stream",
                schema: "public");

            migrationBuilder.DropTable(
                name: "pipelines",
                schema: "public");

            migrationBuilder.DropTable(
                name: "files",
                schema: "public");

            migrationBuilder.DropTable(
                name: "directories",
                schema: "public");
        }
    }
}
