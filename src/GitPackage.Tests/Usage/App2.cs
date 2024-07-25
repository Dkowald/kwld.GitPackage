using GitGet;
using GitGet.Model;
using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class App2 : IClassFixture<App2.State>
{
    public const string Origin = "https://github.com/kubernetes-sigs/nfs-subdir-external-provisioner/";

    public class State
    {
        public TestHost? Host { get; set; }
    }

    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Usage", nameof(App2));

    private readonly StatusFile _nfsCsiYaml;

    private readonly string[] _args;

    private readonly TestHost _host;

    private readonly ILogger _appLogger;

    public App2(State state)
    {
        state.Host ??= new();

        _host = state.Host;

        _appLogger = _host.Get<ILogger>();

        _nfsCsiYaml = new StatusFile(
            _root.GetFolder("Shared"),
            new(Origin),
            new("branch/master"), new("/readme.md,deploy/**/*.yaml"));
        
        _args = [
            _nfsCsiYaml.TargetPath.FullName,
            "--log-level:t"
        ];
    }

    [Ordered, Fact]
    public void Init() 
    {

    }

    [Ordered, Fact]
    public async Task GitGetTag()
    {
        _root.EnsureEmpty();
        _nfsCsiYaml.Version = new("tag/nfs-subdir-external-provisioner-4.0.17");
        _nfsCsiYaml.Commit = null;
        await _nfsCsiYaml.Write(_host.Get<ILogger>());

        await Program.Main(_args);

        await VerifyDirectory(_root.FullName)
            .UseMethodName(nameof(GitGetTag));
    }

    [Ordered, Fact]
    public async Task ResetCommitCausesBranchFetch()
    {
        //remove commit to trigger re-evaluate
        var orgCommit = _nfsCsiYaml.Commit;

        _nfsCsiYaml.Commit = null;
        _nfsCsiYaml.Version = new("branch/master");
        await _nfsCsiYaml.Write(_host.Get<ILogger>());

        await Program.Main(_args);

        var reload = await StatusFile.TryLoad(_appLogger, _nfsCsiYaml.TargetPath);

        Assert.NotNull(reload);
        Assert.False(orgCommit == reload.Commit);
    }
}