using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace ShinyResetApp {
    class Settings {
        const string SETTINGS_FILE = "settings.ini";

        [IniPath("ResetCountFile", "Files")]
        public string ResetCountFile { get; set; } = "shiny reset attempts.txt";

        [IniPath("ResetAverageFile", "Files")]
        public string ResetAverageFile { get; set; } = "shiny reset avg.txt";

        [IniPath("Resets", "Stats")]
        public ulong Resets { get; set; } = 0;

        [IniPath("Shinies", "Stats")]
        public ulong Shinies { get; set; } = 0;




        public static Settings Load(string? path) {
            if (path == null) {
                path = SETTINGS_FILE;
            } else if (!File.Exists(path)) {
                path = SETTINGS_FILE;
            }
            
            if (!File.Exists(path)) {
                return new Settings(); //return default settings
            }

            try {
                IniData data = new FileIniDataParser().ReadFile(path, Encoding.UTF8);
                SectionDataCollection sections = data.Sections;
                Settings settings = new Settings();
                Type sType = settings.GetType();
                PropertyInfo[] pInfos = sType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo pInfo in pInfos) {
                    IniPathAttribute? attr = pInfo.GetCustomAttribute<IniPathAttribute>();
                    if (attr == null) {
                        continue;
                    }

                    string? tValue = null;
                    object? value = null;
                    KeyDataCollection? sect = null;
                    if (attr.Header != null) {
                        if (!sections.ContainsSection(attr.Header)) {
                            continue;
                        }

                        sect = sections[attr.Header];

                    } else {
                        sect = data.Global;
                    }

                    if (!sect.ContainsKey(attr.Key)) {
                        continue;
                    }

                    tValue = sect[attr.Key];

                    if (attr.Loader != null) {
                        value = attr.Loader(tValue);
                    } else {
                        try {
                            //try block so we don't really need to care about TryParse and conditionals.
                            switch (Type.GetTypeCode(pInfo.PropertyType)) {
                            case TypeCode.Object:
                            case TypeCode.Empty:
                            case TypeCode.DBNull:
                                continue; //can't handle these...
                            }

                            value = Type.GetTypeCode(pInfo.PropertyType) switch {
                                TypeCode.Boolean => bool.Parse(tValue),
                                TypeCode.Byte => byte.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Char => char.Parse(tValue),
                                TypeCode.DateTime => DateTime.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Decimal => decimal.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Double => double.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Int16 => short.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Int32 => int.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Int64 => long.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.SByte => sbyte.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.Single => float.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.String => tValue,
                                TypeCode.UInt16 => ushort.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.UInt32 => uint.Parse(tValue, CultureInfo.InvariantCulture),
                                TypeCode.UInt64 => ulong.Parse(tValue, CultureInfo.InvariantCulture),
                                _ => tValue
                            };
                        } catch {
                            continue;
                        }
                    }

                    pInfo.SetValue(settings, value);
                }

                return settings;
            } catch {
                return new Settings();
            }
        }

        public void Save(string? path) {
            if (path == null) {
                path = SETTINGS_FILE;
            }

            IniData data = new IniData();
            List<string> headers = new List<string>();
            SectionDataCollection sections = data.Sections;

            Type sType = this.GetType();
            PropertyInfo[] pInfos = sType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo pInfo in pInfos) {
                IniPathAttribute? attr = pInfo.GetCustomAttribute<IniPathAttribute>();
                if (attr == null) {
                    continue;
                }

                string? tValue = null;
                object? value = pInfo.GetValue(this);
                KeyDataCollection? sect = null;
                if (attr.Header != null) {
                    if (!sections.ContainsSection(attr.Header)) {
                        if (!sections.AddSection(attr.Header)) {
                            throw new Exception(string.Format(Resources.CantCreateSectionErrorText, attr.Header));
                        }
                    }

                    sect = sections[attr.Header];

                } else {
                    sect = data.Global;
                }

                if (attr.Saver != null) {
                    tValue = attr.Saver(value);
                } else {
                    switch (Type.GetTypeCode(pInfo.PropertyType)) {
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        continue; //can't handle these...
                    }

                    if (tValue != null) {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        tValue = Type.GetTypeCode(pInfo.PropertyType) switch {
                            TypeCode.Byte => ((byte)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.DateTime => ((DateTime)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Decimal => ((decimal)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Double => ((double)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Int16 => ((short)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Int32 => ((int)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Int64 => ((long)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.SByte => ((sbyte)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.Single => ((float)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.UInt16 => ((ushort)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.UInt32 => ((uint)value).ToString(CultureInfo.InvariantCulture),
                            TypeCode.UInt64 => ((ulong)value).ToString(CultureInfo.InvariantCulture),
                            _ => value?.ToString()
                        };
#pragma warning restore CS8605 // Unboxing a possibly null value.
                    }
                }

                if (!sect.AddKey(attr.Key, tValue)) {
                    throw new Exception(string.Format(Resources.CantCreateKeyErrorText, attr.Key));
                }
            }

            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(path, data, Encoding.UTF8);
        }
    }
}
