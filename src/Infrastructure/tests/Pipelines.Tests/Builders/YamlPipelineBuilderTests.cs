using ErrorOr;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Giantnodes.Infrastructure.Pipelines.Tests;

public sealed class YamlPipelineBuilderTests
{
    public abstract class Fixture
    {
        internal readonly YamlPipelineBuilder _sut;

        internal readonly FakeLogger<YamlPipelineBuilder> _logger;

        protected Fixture()
        {
            _logger = new FakeLogger<YamlPipelineBuilder>();

            _sut = new YamlPipelineBuilder(_logger);
        }
    }

    public sealed class Build : Fixture
    {
        [Fact]
        public void Should_deserialize_minimal_valid_pipeline()
        {
            // Arrange
            const string yaml = """
                                name: Video Transcoding Pipeline
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("Video Transcoding Pipeline", result.Value.Name);
            Assert.Null(result.Value.Description);
            Assert.Empty(result.Value.Stages);
        }

        [Fact]
        public void Should_deserialize_pipeline_with_description()
        {
            // Arrange
            const string yaml = """
                                name: Video Transcoding Pipeline
                                description: converts video files to multiple output formats
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("Video Transcoding Pipeline", result.Value.Name);
            Assert.Equal("converts video files to multiple output formats", result.Value.Description);
        }

        [Fact]
        public void Should_deserialize_pipeline_with_single_stage()
        {
            // Arrange
            const string yaml = """
                                name: Video Probe Pipeline
                                stages:
                                  probe:
                                    id: probe
                                    name: Probe Video
                                    steps:
                                      - id: probe-media
                                        name: Probe Media File
                                        uses: giantnodes/probe
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Single(result.Value.Stages);
            Assert.True(result.Value.Stages.ContainsKey("probe"));
            Assert.Equal("probe", result.Value.Stages["probe"].Id);
            Assert.Equal("Probe Video", result.Value.Stages["probe"].Name);
        }

        [Fact]
        public void Should_deserialize_pipeline_with_multiple_stages()
        {
            // Arrange
            const string yaml = """
                                name: Video Processing Pipeline
                                stages:
                                  probe:
                                    id: probe
                                    name: Probe Video
                                    steps:
                                      - id: probe-media
                                        name: Probe Media File
                                        uses: giantnodes/probe
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                  thumbnail:
                                    id: thumbnail
                                    name: Generate Thumbnail
                                    steps:
                                      - id: generate-thumbnail
                                        name: Generate Video Thumbnail
                                        uses: giantnodes/thumbnail
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(3, result.Value.Stages.Count);
            Assert.True(result.Value.Stages.ContainsKey("probe"));
            Assert.True(result.Value.Stages.ContainsKey("encode"));
            Assert.True(result.Value.Stages.ContainsKey("thumbnail"));
        }

        [Fact]
        public void Should_deserialize_pipeline_with_stage_dependencies()
        {
            // Arrange
            const string yaml = """
                                name: Video Transcoding Pipeline
                                stages:
                                  probe:
                                    id: probe
                                    name: Probe Media
                                    steps:
                                      - id: probe-media
                                        name: Probe Media File
                                        uses: giantnodes/probe
                                  validate:
                                    id: validate
                                    name: Validate Media
                                    needs:
                                      - probe
                                    steps:
                                      - id: validate-streams
                                        name: Validate Media Streams
                                        uses: giantnodes/validate
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    needs:
                                      - probe
                                      - validate
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Empty(result.Value.Stages["probe"].Needs);
            Assert.Single(result.Value.Stages["validate"].Needs);
            Assert.Contains("probe", result.Value.Stages["validate"].Needs);
            Assert.Equal(2, result.Value.Stages["encode"].Needs.Count);
            Assert.Contains("probe", result.Value.Stages["encode"].Needs);
            Assert.Contains("validate", result.Value.Stages["encode"].Needs);
        }

        [Fact]
        public void Should_deserialize_pipeline_with_stage_steps()
        {
            // Arrange
            const string yaml = """
                                name: Video Transcoding Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    steps:
                                      - id: extract-video
                                        name: Extract Video Stream
                                        uses: giantnodes/extract

                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Single(result.Value.Stages);
            Assert.Equal(2, result.Value.Stages["encode"].Steps.Count);
        }

        [Fact]
        public void Should_deserialize_pipeline_with_step_configuration()
        {
            // Arrange
            const string yaml = """
                                name: Video Encoding Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                        with:
                                          codec: h264
                                          preset: medium
                                          crf: 23
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.False(result.IsError);
            Assert.Single(result.Value.Stages);

            var step = result.Value.Stages["encode"].Steps.First();
            Assert.Equal("encode-h264", step.Id);
            Assert.Equal("Encode to H.264", step.Name);
            Assert.Equal("giantnodes/encode", step.Uses);
            Assert.Equal(3, step.With.Count);
            Assert.Equal("h264", step.With["codec"]);
            Assert.Equal("medium", step.With["preset"]);
            Assert.Equal("23", step.With["crf"]);
        }

        [Fact]
        public void Should_return_error_when_yaml_is_malformed()
        {
            // Arrange
            const string yaml = "not valid yaml at all ][}{";

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Unexpected, result.FirstError.Type);
        }

        [Fact]
        public void Should_return_error_when_yaml_is_empty()
        {
            // Arrange
            var yaml = string.Empty;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
        }

        [Fact]
        public void Should_return_error_when_pipeline_name_is_missing()
        {
            // Arrange
            const string yaml = """
                                description: missing name property
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("pipeline name is required", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_pipeline_description_is_empty()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                description: ''
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("pipeline description cannot be empty", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_stage_name_is_missing()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("stage name is required", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_stage_id_is_empty()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  encode:
                                    id: ''
                                    name: Encode Video
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("stage id cannot be empty", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_stage_has_no_steps()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    name: Encode Video
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("stage must contain at least one step", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_step_uses_is_missing()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("step uses is required", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_multiple_errors_when_multiple_validation_failures()
        {
            // Arrange
            const string yaml = """
                                name: ''
                                description: ''
                                stages:
                                  encode:
                                    id: ''
                                    name: ''
                                    steps:
                                      - id: ''
                                        name: ''
                                        uses: ''
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(7, result.Errors.Count);
            Assert.All(result.Errors, error => Assert.Equal(ErrorType.Validation, error.Type));
        }

        [Fact]
        public void Should_return_error_when_stage_ids_are_not_unique()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  probe:
                                    id: duplicate
                                    name: Probe Video
                                    steps:
                                      - id: probe-media
                                        name: Probe Media File
                                        uses: giantnodes/probe
                                  encode:
                                    id: duplicate
                                    name: Encode Video
                                    steps:
                                      - id: encode-h264
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("stage ids must be unique within the pipeline", result.FirstError.Description);
        }

        [Fact]
        public void Should_return_error_when_step_ids_are_not_unique()
        {
            // Arrange
            const string yaml = """
                                name: Video Pipeline
                                stages:
                                  encode:
                                    id: encode
                                    name: Encode Video
                                    steps:
                                      - id: duplicate
                                        name: Extract Video
                                        uses: giantnodes/extract
                                      - id: duplicate
                                        name: Encode to H.264
                                        uses: giantnodes/encode
                                """;

            // Act
            var result = _sut.Build(yaml);

            // Assert
            Assert.True(result.IsError);
            Assert.Single(result.Errors);
            Assert.Equal(ErrorType.Validation, result.FirstError.Type);
            Assert.Equal("step ids must be unique within the stage", result.FirstError.Description);
        }
    }
}
