using GitGet;
using GitPackage.Cli;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.Usage;

[TestCaseOrderer(LineOrderedTests.TypeName, LineOrderedTests.AssemName)]
public class App2 : IClassFixture<TestHost>
{
    private readonly IDirectoryInfo _root = Files.AppData.GetFolder("Flow", "App2");

    private readonly GitPackageStatusFile _nfsCsiYaml;

    private readonly string[] _args;

    private readonly TestHost _host;

    private readonly ILogger _appLogger;

    public App2(TestHost state)
    {
        _host = state;

        _appLogger = _host.Get<ILoggerProvider>().CreateLogger("");

        _nfsCsiYaml = new GitPackageStatusFile(_appLogger, _root.GetFolder("CSI/NFSFolders/data")) {
            Origin = "https://github.com/kubernetes-sigs/nfs-subdir-external-provisioner/",
            Filter = new("readme.md;deploy/*.yaml") };
        
        _args = [
            _nfsCsiYaml.BackingFile.Directory!.FullName,
            "--log-level:t"
        ];
    }

    [Ordered, Fact]
    public async Task GitGetTag()
    {
        _root.EnsureEmpty();
        _nfsCsiYaml.Version = new("tag/nfs-subdir-external-provisioner-4.0.17");
        _nfsCsiYaml.Commit = null;
        _nfsCsiYaml.Write();

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
        _nfsCsiYaml.Write();

        await Program.Main(_args);

        var reload = GitPackageStatusFile.Load(_appLogger, _nfsCsiYaml.BackingFile);

        Assert.NotNull(reload);
        Assert.False(orgCommit == reload.Commit);
    }
}