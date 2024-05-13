using Microsoft.Build.Framework;

namespace GitPackage
{
    public class GitPackageTaskItem
    {
        private readonly ITaskItem _item;
        public GitPackageTaskItem(ITaskItem item)
        {
            _item = item;
        }

        public string Include => _item.ItemSpec;

        public string Version => _item.GetMetadata(nameof(Version));

        public string Path => _item.GetMetadata(nameof(Path));

        public string Filter => _item.GetMetadata(nameof(Filter));

        public string[] AsLines()
        {
            return new[]
            {
                $"{nameof(Include)} = {Include}",
                $"{nameof(Version)} = {Version}",
                $"{nameof(Path)} = {Path}",
                $"{nameof(Filter)} = {Filter}"
            };
        }
    }
}