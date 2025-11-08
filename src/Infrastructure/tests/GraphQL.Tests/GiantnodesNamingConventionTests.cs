using Giantnodes.Infrastructure.GraphQL;
using HotChocolate.Types;
using TestNamespace;
using Xunit;

namespace Giantnodes.Tests.Infrastructure.GraphQL
{
    public sealed class GiantnodesNamingConventionTests
    {
        private readonly GiantnodesNamingConvention _sut;

        public GiantnodesNamingConventionTests()
        {
            _sut = new GiantnodesNamingConvention();
        }

        [Fact]
        public void Should_return_original_type_name_when_type_name_is_not_command()
        {
            // Arrange
            var type = typeof(string);
            const TypeKind kind = TypeKind.Scalar;

            // Act
            var result = _sut.GetTypeName(type, kind);

            // Assert
            Assert.Equal("String", result);
        }

        [Fact]
        public void Should_convert_command_to_input_when_type_name_ends_with_command()
        {
            // Arrange
            var type = typeof(UserRegister.Command);
            const TypeKind kind = TypeKind.InputObject;

            // Act
            var result = _sut.GetTypeName(type, kind);

            // Assert
            Assert.Equal("UserRegisterInput", result);
        }
    }
}

namespace TestNamespace
{
    internal sealed class UserRegister
    {
        public class Command
        {
        }
    }

    internal sealed class OuterClass
    {
        public class InnerCommand
        {
        }
    }
}
