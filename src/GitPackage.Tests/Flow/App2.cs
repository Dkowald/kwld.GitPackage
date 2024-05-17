using GitPackage.Cli;
using GitPackage.Cli.GitCommands;
using GitPackage.Cli.Model;
using GitPackage.Tests.TestHelpers;
using InMemLogger;
using Microsoft.Extensions.Logging;

namespace GitPackage.Tests.Flow;

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
            
        _nfsCsiYaml = new GitPackageStatusFile(_appLogger, _root.GetFolder("CSI/NFSFolders/data"))
        {
            Include = "https://github.com/kubernetes-sigs/nfs-subdir-external-provisioner/",
            Filter = new("readme.md;deploy/*.yaml")
        };

        _args = [
            "-f", _nfsCsiYaml.BackingFile.FullName,
        ];
    }

    [Ordered, Fact]
    public async Task GitGetTag()
    {
        _nfsCsiYaml.Version = new("tag/nfs-subdir-external-provisioner-4.0.17");
        _nfsCsiYaml.Write();

        await Program.Main(_args);

        await VerifyDirectory(_root.FullName);
    }

    [Ordered, Fact]
    public async Task GetGetBranch()
    {
        var status = GitPackageStatusFile.Load(_appLogger, _nfsCsiYaml.BackingFile);
        status.Version = new("branch/master");
        status.Write();

        await Program.Main(_args);
        
        //should be the same, except .gitpackage
        await VerifyDirectory(_root.FullName, 
                include: f => !f.EndsWith(GitPackageStatusFile.StatusFileName))
            .UseMethodName(nameof(GitGetTag));
    }

    [Ordered, Fact]
    public async Task GetHeadForBranch()
    {
        //remove commit to trigger re-evaluate
        _nfsCsiYaml.Commit = null;
        _nfsCsiYaml.Write();

        await Program.Main(_args);

        //will use HEAD from the branch.
        await VerifyDirectory(_root.FullName);
    }
}