using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations.Pipelines;

internal sealed class PipelineSagaMap : SagaClassMap<PipelineSagsState>
{
    protected override void Configure(EntityTypeBuilder<PipelineSagsState> builder, ModelBuilder model)
    {
        builder
            .HasIndex(p => p.JobId)
            .IsUnique();

        builder
            .Property(p => p.Definition)
            .HasJsonConversion();
    }
}