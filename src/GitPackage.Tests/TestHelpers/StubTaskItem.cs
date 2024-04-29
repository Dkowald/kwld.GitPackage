using System.Collections;

using Microsoft.Build.Framework;

namespace GitPackage.Tests.TestHelpers;

internal class StubTaskItem : ITaskItem
{
    private Dictionary<string, string> _data = new();

    public string? GetMetadata(string metadataName) =>
        _data.GetValueOrDefault(metadataName);

    public void SetMetadata(string metadataName, string metadataValue)
        => _data[metadataName] = metadataValue;

    public void RemoveMetadata(string metadataName)
    {
        if(_data.ContainsKey(metadataName))
            _data.Remove(metadataName);
    }

    public void CopyMetadataTo(ITaskItem destinationItem)
    { throw new NotImplementedException(); }

    public IDictionary CloneCustomMetadata()
    { throw new NotImplementedException(); }

    public string ItemSpec { get; set; } = "spec";

    public ICollection MetadataNames { get; }

    public int MetadataCount { get; }
}