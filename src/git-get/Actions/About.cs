using System.Reflection;

using GitGet.Model;

namespace GitGet.Actions;

internal class About : IAction
{
    public async Task<int> Run(Args args)
    {
        var assem = Assembly.GetExecutingAssembly();

        var ver = assem.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion!;

        await using var rd = assem.GetManifestResourceStream(GetType(), "About.md")!;
        var txt2 = await new StreamReader(rd).ReadToEndAsync();

        txt2 = txt2
            .Replace("99.0", ver)
            .Replace("https://github.com/Dkowald/kwld.GitPackage", Const.HomeUrl)
            .Replace("https://github.com/Dkowald/kwld.GitPackage.git", Const.HomeRepositoryUrl);

        await Console.Out.WriteAsync(txt2);
        return 0;
    }
}