using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using TestContainers.Core.Builders;
using TestContainers.Core.Containers;
using Xunit;

namespace TestContainers.Tests.ContainerTests
{
    public class GenericContainerFixture : IAsyncLifetime
    {
        public ContainerInspectResponse ContainerInfo => _container.ContainerInspectResponse;
        private readonly Container _container;

        public GenericContainerFixture() => _container = new GenericContainerBuilder<Container>()
            .Begin()
            .WithImage("alpine:latest")
            .WithLabel(("your.custom", "label"))
            .Build();

        public Task InitializeAsync() => _container.Start();

        public Task DisposeAsync() => _container.Stop();
    }

    public class GenericContainerTests : IClassFixture<GenericContainerFixture>
    {
        private readonly ContainerInspectResponse _containerInfo;

        public GenericContainerTests(GenericContainerFixture fixture) => _containerInfo = fixture.ContainerInfo;

        [Fact]
        public void CustomLabelTest()
        {
            var label = _containerInfo.Config.Labels.Single();
            Assert.Equal("your.custom", label.Key);
            Assert.Equal("label", label.Value);
        }

        [Theory]
        [InlineData("alpine:latest", "alpine:latest")]
        [InlineData("alpine", "alpine:latest")]
        [InlineData("alpine:hello", "alpine:hello")]
        [InlineData("test.de:55/alpine:latest", "test.de:55/alpine:latest")]
        [InlineData("test.de:55/alpine", "test.de:55/alpine:latest")]
        public void WithImage_ExtractFromImageAndTag(string path, string tag)
        {
            var container = new GenericContainerBuilder<Container>()
                .Begin()
                .WithImage(path)
                .Build();

            Assert.Equal(tag, container.DockerImageName);
        }

        [Fact]
        public async Task WithExecCommand()
        {
            var container = new GenericContainerBuilder<Container>()
                .Begin()
                .WithImage("alpine:latest")
                .Build();

            await container.Start();

            var execCommand = new[]
            {
                "sh",
                "-c",
                "ls"
            };

            await container.ExecuteCommand(execCommand);
            await container.Stop();
        }
    }
}
