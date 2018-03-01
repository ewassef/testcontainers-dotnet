using System;
using System.Runtime.InteropServices;
using Docker.DotNet;

public class UnixSocketClientProviderStrategy : DockerClientProviderStrategy
{
    string connectionString => Environment.GetEnvironmentVariable("DOCKER_HOST");
    protected override DockerClientConfiguration Config { get; } =
        new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));

    protected override bool IsApplicable() => Utils.IsOSX();

    protected override string GetDescription() =>
        "Docker for windows (via TCP port 2375";

    protected override void Test()
    {
        
    }
}