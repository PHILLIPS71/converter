using Xunit;

namespace Giantnodes.Infrastructure.Tests;

public sealed class EnumerationTests
{
    private record TestEnumeration : Enumeration
    {
        public static readonly TestEnumeration One = new(1, "One");
        public static readonly TestEnumeration Two = new(2, "Two");
        public static readonly TestEnumeration Three = new(3, "Three");

        private TestEnumeration(int id, string name)
            : base(id, name)
        {
        }
    }

    [Fact]
    public void Should_create_enumeration_with_correct_id_and_name()
    {
        // Act
        var enumeration = TestEnumeration.One;

        // Assert
        Assert.Equal(1, enumeration.Id);
        Assert.Equal("One", enumeration.Name);
    }

    [Fact]
    public void Should_return_name_when_converting_to_string()
    {
        // Arrange
        var enumeration = TestEnumeration.One;

        // Act
        var result = enumeration.ToString();

        // Assert
        Assert.Equal(enumeration.Name, result);
    }

    [Fact]
    public void Should_use_id_as_hash_code()
    {
        // Arrange
        var enumeration = TestEnumeration.One;

        // Act
        var result = enumeration.GetHashCode();

        // Assert
        Assert.Equal(enumeration.Id.GetHashCode(), result);
    }

    [Fact]
    public void Should_return_all_enumeration_values()
    {
        // Arrange
        var values = new[] { TestEnumeration.One, TestEnumeration.Two, TestEnumeration.Three };

        // Act
        var result = Enumeration.GetAll<TestEnumeration>().ToArray();

        // Assert
        Assert.Equal(values.Length, result.Length);
        Assert.All(values, value => Assert.Contains(value, result));
    }

    [Fact]
    public void Should_parse_enumeration_using_predicate()
    {
        // Arrange
        var expected = TestEnumeration.Two;

        // Act
        var result = Enumeration.Parse<TestEnumeration>(x => x.Id == 2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_throw_argument_exception_for_invalid_predicate()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Enumeration.Parse<TestEnumeration>(x => x.Id == 999));
    }

    [Fact]
    public void Should_parse_by_name()
    {
        // Arrange & Act
        var result = Enumeration.ParseByValueOrName<TestEnumeration>("One");

        // Assert
        Assert.Equal(TestEnumeration.One, result);
    }

    [Fact]
    public void Should_parse_by_id_value()
    {
        // Arrange & Act
        var result = Enumeration.ParseByValueOrName<TestEnumeration>("2");

        // Assert
        Assert.Equal(TestEnumeration.Two, result);
    }

    [Fact]
    public void Should_throw_argument_exception_for_invalid_value_or_name()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Enumeration.ParseByValueOrName<TestEnumeration>("InvalidValue"));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(4, false)]
    public void Should_try_parse_by_id(int id, bool expected)
    {
        // Arrange & Act
        var result = Enumeration.TryParse(id, out TestEnumeration? enumeration);

        // Assert
        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.NotNull(enumeration);
            Assert.Equal(id, enumeration.Id);
        }
        else
        {
            Assert.Null(enumeration);
        }
    }

    [Theory]
    [InlineData("One", true)]
    [InlineData("InvalidValue", false)]
    public void Should_try_parse_by_name(string name, bool expected)
    {
        // Arrange & Act
        var result = Enumeration.TryParse(name, out TestEnumeration? enumeration);

        // Assert
        Assert.Equal(expected, result);
        if (expected)
        {
            Assert.NotNull(enumeration);
            Assert.Equal(name, enumeration.Name);
        }
        else
        {
            Assert.Null(enumeration);
        }
    }

    [Theory]
    [InlineData("One", true)]
    [InlineData("2", true)]
    [InlineData("InvalidValue", false)]
    public void Should_try_parse_by_value_or_name(string reference, bool expected)
    {
        // Arrange & Act
        var result = Enumeration.TryParseByValueOrName<TestEnumeration>(reference);

        // Assert
        if (expected)
        {
            Assert.NotNull(result);
            Assert.True(result.Name == reference || result.Id.ToString() == reference);
        }
        else
        {
            Assert.Null(result);
        }
    }
}