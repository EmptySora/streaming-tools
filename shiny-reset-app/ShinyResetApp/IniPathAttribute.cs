using System;

namespace ShinyResetApp {
    delegate object? IniValueLoader(string iniValue);
    delegate string IniValueSaver(object? value);

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class IniPathAttribute : Attribute {
        public string Key { get; set; }
        public string? Header { get; set; }
        public IniValueLoader? Loader { get; set; }
        public IniValueSaver? Saver { get; set; }

        public IniPathAttribute(string key, string? header) {
            this.Key = key;
            this.Header = header;
        }
    }
}
