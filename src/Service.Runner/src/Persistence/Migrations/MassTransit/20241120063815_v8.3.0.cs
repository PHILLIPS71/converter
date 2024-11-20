﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Giantnodes.Service.Runner.Persistence.Migrations.MassTransit
{
    /// <inheritdoc />
    public partial class v830 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "masstransit");

            migrationBuilder.CreateTable(
                name: "inbox_state",
                schema: "masstransit",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consumer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    received = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    receive_count = table.Column<int>(type: "integer", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    consumed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_state", x => x.id);
                    table.UniqueConstraint("ak_inbox_state_message_id_consumer_id", x => new { x.message_id, x.consumer_id });
                });

            migrationBuilder.CreateTable(
                name: "job_saga",
                schema: "masstransit",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<int>(type: "integer", nullable: false),
                    submitted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    service_address = table.Column<string>(type: "text", nullable: true),
                    job_timeout = table.Column<TimeSpan>(type: "interval", nullable: true),
                    job = table.Column<string>(type: "text", nullable: true),
                    job_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    retry_attempt = table.Column<int>(type: "integer", nullable: false),
                    started = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    faulted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    job_slot_wait_token = table.Column<Guid>(type: "uuid", nullable: true),
                    job_retry_delay_token = table.Column<Guid>(type: "uuid", nullable: true),
                    incomplete_attempts = table.Column<string>(type: "text", nullable: true),
                    last_progress_value = table.Column<long>(type: "bigint", nullable: true),
                    last_progress_limit = table.Column<long>(type: "bigint", nullable: true),
                    last_progress_sequence_number = table.Column<long>(type: "bigint", nullable: true),
                    job_state = table.Column<string>(type: "text", nullable: true),
                    job_properties = table.Column<string>(type: "text", nullable: true),
                    cron_expression = table.Column<string>(type: "text", nullable: true),
                    time_zone_id = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_saga", x => x.correlation_id);
                });

            migrationBuilder.CreateTable(
                name: "job_type_saga",
                schema: "masstransit",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<int>(type: "integer", nullable: false),
                    active_job_count = table.Column<int>(type: "integer", nullable: false),
                    concurrent_job_limit = table.Column<int>(type: "integer", nullable: false),
                    override_job_limit = table.Column<int>(type: "integer", nullable: true),
                    override_limit_expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    active_jobs = table.Column<string>(type: "text", nullable: true),
                    instances = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    global_concurrent_job_limit = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_type_saga", x => x.correlation_id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_state",
                schema: "masstransit",
                columns: table => new
                {
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lock_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_sequence_number = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_state", x => x.outbox_id);
                });

            migrationBuilder.CreateTable(
                name: "job_attempt_saga",
                schema: "masstransit",
                columns: table => new
                {
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_state = table.Column<int>(type: "integer", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    retry_attempt = table.Column<int>(type: "integer", nullable: false),
                    service_address = table.Column<string>(type: "text", nullable: true),
                    instance_address = table.Column<string>(type: "text", nullable: true),
                    started = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    faulted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status_check_token_id = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_attempt_saga", x => x.correlation_id);
                    table.ForeignKey(
                        name: "fk_job_attempt_saga_job_saga_job_id",
                        column: x => x.job_id,
                        principalSchema: "masstransit",
                        principalTable: "job_saga",
                        principalColumn: "correlation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message",
                schema: "masstransit",
                columns: table => new
                {
                    sequence_number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    enqueue_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    sent_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    headers = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    inbox_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    inbox_consumer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    outbox_id = table.Column<Guid>(type: "uuid", nullable: true),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    message_type = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    conversation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    initiator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    destination_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    response_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    fault_address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    expiration_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_message", x => x.sequence_number);
                    table.ForeignKey(
                        name: "fk_outbox_message_inbox_state_inbox_message_id_inbox_consumer_",
                        columns: x => new { x.inbox_message_id, x.inbox_consumer_id },
                        principalSchema: "masstransit",
                        principalTable: "inbox_state",
                        principalColumns: new[] { "message_id", "consumer_id" });
                    table.ForeignKey(
                        name: "fk_outbox_message_outbox_state_outbox_id",
                        column: x => x.outbox_id,
                        principalSchema: "masstransit",
                        principalTable: "outbox_state",
                        principalColumn: "outbox_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_inbox_state_delivered",
                schema: "masstransit",
                table: "inbox_state",
                column: "delivered");

            migrationBuilder.CreateIndex(
                name: "ix_job_attempt_saga_job_id_retry_attempt",
                schema: "masstransit",
                table: "job_attempt_saga",
                columns: new[] { "job_id", "retry_attempt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_enqueue_time",
                schema: "masstransit",
                table: "outbox_message",
                column: "enqueue_time");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_expiration_time",
                schema: "masstransit",
                table: "outbox_message",
                column: "expiration_time");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_inbox_message_id_inbox_consumer_id_sequence_",
                schema: "masstransit",
                table: "outbox_message",
                columns: new[] { "inbox_message_id", "inbox_consumer_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_message_outbox_id_sequence_number",
                schema: "masstransit",
                table: "outbox_message",
                columns: new[] { "outbox_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_state_created",
                schema: "masstransit",
                table: "outbox_state",
                column: "created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_attempt_saga",
                schema: "masstransit");

            migrationBuilder.DropTable(
                name: "job_type_saga",
                schema: "masstransit");

            migrationBuilder.DropTable(
                name: "outbox_message",
                schema: "masstransit");

            migrationBuilder.DropTable(
                name: "job_saga",
                schema: "masstransit");

            migrationBuilder.DropTable(
                name: "inbox_state",
                schema: "masstransit");

            migrationBuilder.DropTable(
                name: "outbox_state",
                schema: "masstransit");
        }
    }
}
