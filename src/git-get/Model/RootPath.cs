using System.Diagnostics.CodeAnalysis;

namespace GitGet.Model
{
    /// <summary>
    /// string containing series of '/' delimited path segments.
    /// path segments can contain anything.
    /// <br /> Always starts with /
    /// <br/> Cannot be empty.
    /// <br/> Must contain a sub-path (cannot be '/')
    /// <br /> Trimmed
    /// <br /> auto prefixed with '/'
    /// <br /> trailing '/' removed (if any)
    /// </summary>
    public record RootPath : IDataString
    {
        private readonly string _value;

        private static (string? error, string? value) TryRead(string data)
        {
            data = data.Trim();
            if(data == string.Empty)
                return ("Cannot be empty", null);
            if(data == "/")
                return ("Must contain a sub-path (cannot be '/')", null);

            data = data[0] == '/' ? data : $"/{data}";

            data = data[^1] == '/' ? data[..^1] : data;

            return new(null, data);
        }

        public static RootPath? TryParse(string? value)
        {
            if(value is null) return null;

            var (_, data) = TryRead(value);

            return data is null ? null : new(data, true);
        }

        private RootPath(string value, bool isChecked)
        {
            if(!isChecked) {
                var (error, clean) = TryRead(value);

                if(clean is null)
                    throw new ArgumentException(error, nameof(value));

                value = clean;
            }
            _value = value;
        }

        public RootPath(string value) : this(value, false) { }

        [return: NotNullIfNotNull(nameof(data))]
        public static implicit operator string?(RootPath? data) => data?.ToString();

        public override string ToString() => _value;
    }
}
