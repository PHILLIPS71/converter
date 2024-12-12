using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Pipelines;

internal sealed class PipelineSagsStateConfiguration : SagaClassMap<PipelineSagsState>
{
    protected override void Configure(EntityTypeBuilder<PipelineSagsState> builder, ModelBuilder model)
    {
        builder
            .HasIndex(p => p.JobId)
            .IsUnique();
    }
}