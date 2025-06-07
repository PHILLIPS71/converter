using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations;

public sealed class PipelineLifecycleSagaMap : SagaClassMap<PipelineLifecycleSagaState>
{
    protected override void Configure(EntityTypeBuilder<PipelineLifecycleSagaState> builder, ModelBuilder model)
    {
    }
}