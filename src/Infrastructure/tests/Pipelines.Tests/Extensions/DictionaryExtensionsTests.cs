using ErrorOr;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class DictionaryExtensionsTests
{
    public abstract class Fixture
    {
        internal readonly Dictionary<string, object> _dictionary;

        protected Fixture()
        {
            _dictionary = [];
        }
    }

    public sealed class GetOptional_WithoutDefault : Fixture
    {
        [Fact]
        public void Should_return_value_when_key_exists_and_type_matches()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.GetOptional<string>("codec");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("h264", result.Value);
        }

        [Fact]
        public void Should_return_default_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.GetOptional<string>("codec");

            // Assert
            Assert.False(result.IsError);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Should_return_error_when_type_does_not_match()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.GetOptional<int>("codec");

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("value for key 'codec' is not of type Int32", result.FirstError.Description);
        }
    }

    public sealed class GetOptional_WithDefault : Fixture
    {
        [Fact]
        public void Should_return_value_when_key_exists_and_type_matches()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.GetOptional("codec", "h265");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("h264", result.Value);
        }

        [Fact]
        public void Should_return_default_value_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.GetOptional("codec", "h265");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("h265", result.Value);
        }

        [Fact]
        public void Should_return_error_when_type_does_not_match()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.GetOptional("codec", 0);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
        }
    }

    public sealed class GetOptional_WithConverter_WithoutDefault : Fixture
    {
        [Fact]
        public void Should_return_converted_value_when_key_exists()
        {
            // Arrange
            _dictionary["crf"] = "23";

            // Act
            var result = _dictionary.GetOptional<int>("crf", value => int.Parse((string)value));

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(23, result.Value);
        }

        [Fact]
        public void Should_return_default_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.GetOptional<int?>("crf", value => int.Parse((string)value));

            // Assert
            Assert.False(result.IsError);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Should_return_error_when_converter_returns_error()
        {
            // Arrange
            _dictionary["crf"] = "invalid";

            // Act
            var result = _dictionary.GetOptional<int>("crf", value =>
                int.TryParse((string)value, out var parsed)
                    ? parsed
                    : Error.Validation("conversion failed"));

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
        }
    }

    public sealed class GetOptional_WithConverter_WithDefault : Fixture
    {
        [Fact]
        public void Should_return_converted_value_when_key_exists()
        {
            // Arrange
            _dictionary["crf"] = "23";

            // Act
            var result = _dictionary.GetOptional("crf", value => int.Parse((string)value), 0);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(23, result.Value);
        }

        [Fact]
        public void Should_return_default_value_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.GetOptional("crf", value => int.Parse((string)value), 18);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(18, result.Value);
        }

        [Fact]
        public void Should_return_error_when_converter_returns_error()
        {
            // Arrange
            _dictionary["crf"] = "invalid";

            // Act
            var result = _dictionary.GetOptional("crf", value =>
                int.TryParse((string)value, out var parsed)
                    ? parsed
                    : Error.Validation("conversion failed"), 18);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
        }
    }

    public sealed class Get : Fixture
    {
        [Fact]
        public void Should_return_value_when_key_exists_and_type_matches()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.Get<string>("codec");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("h264", result.Value);
        }

        [Fact]
        public void Should_return_error_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.Get<string>("codec");

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
            Assert.Equal("key 'codec' not found in dictionary", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_type_does_not_match()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.Get<int>("codec");

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("value for key 'codec' is not of type Int32", result.FirstError.Description);
        }
    }

    public sealed class TryGet : Fixture
    {
        [Fact]
        public void Should_return_true_and_value_when_key_exists_and_type_matches()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var success = _dictionary.TryGet<string>("codec", out var value);

            // Assert
            Assert.True(success);
            Assert.Equal("h264", value);
        }

        [Fact]
        public void Should_return_false_when_key_does_not_exist()
        {
            // Act
            var success = _dictionary.TryGet<string>("codec", out var value);

            // Assert
            Assert.False(success);
            Assert.Null(value);
        }

        [Fact]
        public void Should_return_false_when_type_does_not_match()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var success = _dictionary.TryGet<int>("codec", out var value);

            // Assert
            Assert.False(success);
            Assert.Equal(0, value);
        }
    }

    public sealed class Has : Fixture
    {
        [Fact]
        public void Should_return_true_when_key_exists()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.Has("codec");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Should_return_false_when_key_does_not_exist()
        {
            // Act
            var result = _dictionary.Has("codec");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Should_return_true_when_value_is_null()
        {
            // Arrange
            _dictionary["codec"] = null!;

            // Act
            var result = _dictionary.Has("codec");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Should_be_case_sensitive()
        {
            // Arrange
            _dictionary["codec"] = "h264";

            // Act
            var result = _dictionary.Has("Codec");

            // Assert
            Assert.False(result);
        }
    }
}
