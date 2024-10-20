using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Interfaces;

public class FileSystemEntryType : InterfaceType<FileSystemEntry>
{
    protected override void Configure(IInterfaceTypeDescriptor<FileSystemEntry> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor
            .Field(p => p.Id)
            .Type<NonNullType<IdType>>();

        descriptor
            .Field(f => f.PathInfo);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }
}