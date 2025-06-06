﻿// <auto-generated />
using System;
using Giantnodes.Service.Runner.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Giantnodes.Service.Runner.Persistence.Migrations.MassTransit
{
    [DbContext(typeof(MassTransitDbContext))]
    [Migration("20241120063815_v8.3.0")]
    partial class v830
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("masstransit")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.InboxState", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("Consumed")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("consumed");

                    b.Property<Guid>("ConsumerId")
                        .HasColumnType("uuid")
                        .HasColumnName("consumer_id");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("delivered");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration_time");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("last_sequence_number");

                    b.Property<Guid>("LockId")
                        .HasColumnType("uuid")
                        .HasColumnName("lock_id");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.Property<int>("ReceiveCount")
                        .HasColumnType("integer")
                        .HasColumnName("receive_count");

                    b.Property<DateTime>("Received")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("received");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("row_version");

                    b.HasKey("Id")
                        .HasName("pk_inbox_state");

                    b.HasAlternateKey("MessageId", "ConsumerId")
                        .HasName("ak_inbox_state_message_id_consumer_id");

                    b.HasIndex("Delivered")
                        .HasDatabaseName("ix_inbox_state_delivered");

                    b.ToTable("inbox_state", "masstransit");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
                {
                    b.Property<long>("SequenceNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("sequence_number");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SequenceNumber"));

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("body");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("content_type");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid")
                        .HasColumnName("conversation_id");

                    b.Property<Guid?>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("destination_address");

                    b.Property<DateTime?>("EnqueueTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("enqueue_time");

                    b.Property<DateTime?>("ExpirationTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expiration_time");

                    b.Property<string>("FaultAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("fault_address");

                    b.Property<string>("Headers")
                        .HasColumnType("text")
                        .HasColumnName("headers");

                    b.Property<Guid?>("InboxConsumerId")
                        .HasColumnType("uuid")
                        .HasColumnName("inbox_consumer_id");

                    b.Property<Guid?>("InboxMessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("inbox_message_id");

                    b.Property<Guid?>("InitiatorId")
                        .HasColumnType("uuid")
                        .HasColumnName("initiator_id");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.Property<string>("MessageType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_type");

                    b.Property<Guid?>("OutboxId")
                        .HasColumnType("uuid")
                        .HasColumnName("outbox_id");

                    b.Property<string>("Properties")
                        .HasColumnType("text")
                        .HasColumnName("properties");

                    b.Property<Guid?>("RequestId")
                        .HasColumnType("uuid")
                        .HasColumnName("request_id");

                    b.Property<string>("ResponseAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("response_address");

                    b.Property<DateTime>("SentTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("sent_time");

                    b.Property<string>("SourceAddress")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("source_address");

                    b.HasKey("SequenceNumber")
                        .HasName("pk_outbox_message");

                    b.HasIndex("EnqueueTime")
                        .HasDatabaseName("ix_outbox_message_enqueue_time");

                    b.HasIndex("ExpirationTime")
                        .HasDatabaseName("ix_outbox_message_expiration_time");

                    b.HasIndex("OutboxId", "SequenceNumber")
                        .IsUnique()
                        .HasDatabaseName("ix_outbox_message_outbox_id_sequence_number");

                    b.HasIndex("InboxMessageId", "InboxConsumerId", "SequenceNumber")
                        .IsUnique()
                        .HasDatabaseName("ix_outbox_message_inbox_message_id_inbox_consumer_id_sequence_");

                    b.ToTable("outbox_message", "masstransit");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxState", b =>
                {
                    b.Property<Guid>("OutboxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("outbox_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateTime?>("Delivered")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("delivered");

                    b.Property<long?>("LastSequenceNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("last_sequence_number");

                    b.Property<Guid>("LockId")
                        .HasColumnType("uuid")
                        .HasColumnName("lock_id");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("row_version");

                    b.HasKey("OutboxId")
                        .HasName("pk_outbox_state");

                    b.HasIndex("Created")
                        .HasDatabaseName("ix_outbox_state_created");

                    b.ToTable("outbox_state", "masstransit");
                });

            modelBuilder.Entity("MassTransit.JobAttemptSaga", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id");

                    b.Property<int>("CurrentState")
                        .HasColumnType("integer")
                        .HasColumnName("current_state");

                    b.Property<DateTime?>("Faulted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("faulted");

                    b.Property<string>("InstanceAddress")
                        .HasColumnType("text")
                        .HasColumnName("instance_address");

                    b.Property<Guid>("JobId")
                        .HasColumnType("uuid")
                        .HasColumnName("job_id");

                    b.Property<int>("RetryAttempt")
                        .HasColumnType("integer")
                        .HasColumnName("retry_attempt");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("row_version");

                    b.Property<string>("ServiceAddress")
                        .HasColumnType("text")
                        .HasColumnName("service_address");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started");

                    b.Property<Guid?>("StatusCheckTokenId")
                        .HasColumnType("uuid")
                        .HasColumnName("status_check_token_id");

                    b.HasKey("CorrelationId")
                        .HasName("pk_job_attempt_saga");

                    b.HasIndex("JobId", "RetryAttempt")
                        .IsUnique()
                        .HasDatabaseName("ix_job_attempt_saga_job_id_retry_attempt");

                    b.ToTable("job_attempt_saga", "masstransit");
                });

            modelBuilder.Entity("MassTransit.JobSaga", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id");

                    b.Property<Guid>("AttemptId")
                        .HasColumnType("uuid")
                        .HasColumnName("attempt_id");

                    b.Property<DateTime?>("Completed")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("completed");

                    b.Property<string>("CronExpression")
                        .HasColumnType("text")
                        .HasColumnName("cron_expression");

                    b.Property<int>("CurrentState")
                        .HasColumnType("integer")
                        .HasColumnName("current_state");

                    b.Property<TimeSpan?>("Duration")
                        .HasColumnType("interval")
                        .HasColumnName("duration");

                    b.Property<DateTimeOffset?>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<DateTime?>("Faulted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("faulted");

                    b.Property<string>("IncompleteAttempts")
                        .HasColumnType("text")
                        .HasColumnName("incomplete_attempts");

                    b.Property<string>("Job")
                        .HasColumnType("text")
                        .HasColumnName("job");

                    b.Property<string>("JobProperties")
                        .HasColumnType("text")
                        .HasColumnName("job_properties");

                    b.Property<Guid?>("JobRetryDelayToken")
                        .HasColumnType("uuid")
                        .HasColumnName("job_retry_delay_token");

                    b.Property<Guid?>("JobSlotWaitToken")
                        .HasColumnType("uuid")
                        .HasColumnName("job_slot_wait_token");

                    b.Property<string>("JobState")
                        .HasColumnType("text")
                        .HasColumnName("job_state");

                    b.Property<TimeSpan?>("JobTimeout")
                        .HasColumnType("interval")
                        .HasColumnName("job_timeout");

                    b.Property<Guid>("JobTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("job_type_id");

                    b.Property<long?>("LastProgressLimit")
                        .HasColumnType("bigint")
                        .HasColumnName("last_progress_limit");

                    b.Property<long?>("LastProgressSequenceNumber")
                        .HasColumnType("bigint")
                        .HasColumnName("last_progress_sequence_number");

                    b.Property<long?>("LastProgressValue")
                        .HasColumnType("bigint")
                        .HasColumnName("last_progress_value");

                    b.Property<DateTimeOffset?>("NextStartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("next_start_date");

                    b.Property<string>("Reason")
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<int>("RetryAttempt")
                        .HasColumnType("integer")
                        .HasColumnName("retry_attempt");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("row_version");

                    b.Property<string>("ServiceAddress")
                        .HasColumnType("text")
                        .HasColumnName("service_address");

                    b.Property<DateTimeOffset?>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<DateTime?>("Started")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("started");

                    b.Property<DateTime?>("Submitted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("submitted");

                    b.Property<string>("TimeZoneId")
                        .HasColumnType("text")
                        .HasColumnName("time_zone_id");

                    b.HasKey("CorrelationId")
                        .HasName("pk_job_saga");

                    b.ToTable("job_saga", "masstransit");
                });

            modelBuilder.Entity("MassTransit.JobTypeSaga", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id");

                    b.Property<int>("ActiveJobCount")
                        .HasColumnType("integer")
                        .HasColumnName("active_job_count");

                    b.Property<string>("ActiveJobs")
                        .HasColumnType("text")
                        .HasColumnName("active_jobs");

                    b.Property<int>("ConcurrentJobLimit")
                        .HasColumnType("integer")
                        .HasColumnName("concurrent_job_limit");

                    b.Property<int>("CurrentState")
                        .HasColumnType("integer")
                        .HasColumnName("current_state");

                    b.Property<int?>("GlobalConcurrentJobLimit")
                        .HasColumnType("integer")
                        .HasColumnName("global_concurrent_job_limit");

                    b.Property<string>("Instances")
                        .HasColumnType("text")
                        .HasColumnName("instances");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("OverrideJobLimit")
                        .HasColumnType("integer")
                        .HasColumnName("override_job_limit");

                    b.Property<DateTime?>("OverrideLimitExpiration")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("override_limit_expiration");

                    b.Property<string>("Properties")
                        .HasColumnType("text")
                        .HasColumnName("properties");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea")
                        .HasColumnName("row_version");

                    b.HasKey("CorrelationId")
                        .HasName("pk_job_type_saga");

                    b.ToTable("job_type_saga", "masstransit");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
                {
                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.OutboxState", null)
                        .WithMany()
                        .HasForeignKey("OutboxId")
                        .HasConstraintName("fk_outbox_message_outbox_state_outbox_id");

                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.InboxState", null)
                        .WithMany()
                        .HasForeignKey("InboxMessageId", "InboxConsumerId")
                        .HasPrincipalKey("MessageId", "ConsumerId")
                        .HasConstraintName("fk_outbox_message_inbox_state_inbox_message_id_inbox_consumer_");
                });

            modelBuilder.Entity("MassTransit.JobAttemptSaga", b =>
                {
                    b.HasOne("MassTransit.JobSaga", null)
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_job_attempt_saga_job_saga_job_id");
                });
#pragma warning restore 612, 618
        }
    }
}
