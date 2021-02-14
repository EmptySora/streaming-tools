using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WinAudioLevels {
    //serialize via JSON
    public class ApplicationSettings : ICloneable, IEquatable<ApplicationSettings> {
        private static MainForm _main_form;
        public static ApplicationSettings CurrentSettings => _main_form.Settings;
        private const string DEFAULT_SETTINGS_FILE = "settings.json";
        private static readonly Version CURRENT_SETTINGS_FILE_VERSION = Version.Parse(AssemblyDetails.ASSEMBLY_VERSION);
        private static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings() {
            Formatting = Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DefaultValueHandling = DefaultValueHandling.Include,
            FloatFormatHandling = FloatFormatHandling.String,
            FloatParseHandling = FloatParseHandling.Double,
            TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.Default,
            ReferenceLoopHandling = ReferenceLoopHandling.Error,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            MaxDepth = null,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };
        private delegate ApplicationSettings UpgradeDelegate(ApplicationSettings old, bool inPlace = false);
        private static readonly Dictionary<Version, UpgradeDelegate> UPGRADE_OPERATIONS = new Dictionary<Version, UpgradeDelegate>();
        public static ApplicationSettings GetDefaultSettings() {
            return new ApplicationSettings() {
                InnerSettings = new SettingsV0() {
                    Servers = new SettingsV0.WebSocketServerSettings[] {
                        new SettingsV0.WebSocketServerSettings() {
                            Port = 8069,
                            Secure =false,
                            Uri = "localhost"
                        }
                    }
                }
            };
        }
        //key is the version the upgrade brings the settings to.
        //the delegate must: create a new ApplicationSettings object and return the upgraded settings. Version must equal the key.
        //no key can be larger than CURRENT_SETTINGS_FILE_VERSION;
        //YOU MUST ACCESS SETTINGS THROUGH _inner_settings, NOT Settings.
        //You must manually cast _inner_settings to the correct SettingsV# object (which should be easy since each upgrade function
        //should only have one predecessor.)
        #region Methods and crap
        public override int GetHashCode() {
            //figure out how to do this properly.
            //we might gethash of _internal_settings and then add it to hash of Version.
            //HashCode.Combine is not available in .NET4.8
            return this.Version.GetHashCode() ^ this.InnerSettings.GetHashCode();
        }
        /// <summary>
        /// Saves these settings to the specified file, optionally overwriting the old file.
        /// </summary>
        /// <param name="path">The file path leading to where the settings should be saved.</param>
        /// <param name="overwrite">
        /// <c>true</c> to ovewrite the destination file. If <c>false</c>, the application will throw
        /// a <see cref="InvalidOperationException"/> should the file exist.
        /// </param>
        /// <exception cref="InvalidOperationException"><paramref name="path"/> already exists and <paramref name="overwrite"/> is <c>false</c>.</exception>
        public void Save(string path, bool overwrite = true) {
            if (File.Exists(path) && !overwrite) {
                throw new InvalidOperationException("The specified file already exists.");
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(this, JSON_SETTINGS), Encoding.UTF8);
        }
        /// <summary>
        /// Saves these settings to <c>settings.json</c> in the current directory, overwriting it if it already exists.
        /// </summary>
        public void Save() {
            this.Save(DEFAULT_SETTINGS_FILE);
        }

        /// <summary>
        /// Asynchronously saves these settings to the specified file, optionally overwriting the old file.
        /// </summary>
        /// <param name="path">The file path leading to where the settings should be saved.</param>
        /// <param name="overwrite">
        /// <c>true</c> to ovewrite the destination file. If <c>false</c>, the application will throw
        /// a <see cref="InvalidOperationException"/> should the file exist.
        /// </param>
        /// <exception cref="InvalidOperationException"><paramref name="path"/> already exists and <paramref name="overwrite"/> is <c>false</c>.</exception>
        public async Task SaveAsync(string path, bool overwrite = true) {
            ApplicationSettings nsettings = (ApplicationSettings)this.Clone();
            await new Task(async () => {
                //async task because we want to do JsonConvert on a separate thread but asynchronously write to the file
                if (File.Exists(path) && !overwrite) {
                    throw new InvalidOperationException("The specified file already exists.");
                }
                using(FileStream stream = File.OpenWrite(path)) {
                    using(StreamWriter writer = new StreamWriter(stream, Encoding.UTF8)) {
                        await writer.WriteAsync(JsonConvert.SerializeObject(this, JSON_SETTINGS));
                    }
                }
            });
        }
        /// <summary>
        /// Asynchronously saves these settings to <c>settings.json</c> in the current directory, overwriting it if it already exists.
        /// </summary>
        public async Task SaveAsync() {
            await this.SaveAsync(DEFAULT_SETTINGS_FILE);
        }


        /// <summary>
        /// Loads the saved application settings located at the specified file <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file containing the JSON settings.</param>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        public static ApplicationSettings Load(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("The specified file could not be found.");
            }

            //SettingsVersionCheck
            ApplicationSettings ret = JsonConvert.DeserializeObject<ApplicationSettings>(File.ReadAllText(path, Encoding.UTF8), JSON_SETTINGS);
            if (!SettingsVersionCheck(ret)) {
                throw new ApplicationSettingsException("Version check failed!");
            }
            return ret;
        }
        /// <summary>
        /// Loads the saved application settings located at <c>settings.json</c> (in the current directory).
        /// </summary>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        public static ApplicationSettings Load() {
            return Load(DEFAULT_SETTINGS_FILE);
        }

        /// <summary>
        /// Asynchronously loads the saved application settings located at the specified file <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the file containing the JSON settings.</param>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        public static async Task<ApplicationSettings> LoadAsync(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("The specified file could not be found.");
            }
            using(FileStream fStream = File.OpenRead(path)) {
                using(StreamReader reader = new StreamReader(fStream, Encoding.UTF8)) {
                    ApplicationSettings ret = JsonConvert.DeserializeObject<ApplicationSettings>(await reader.ReadToEndAsync(), JSON_SETTINGS);
                    if (!SettingsVersionCheck(ret)) {
                        throw new ApplicationSettingsException("Version check failed!");
                    }
                    return ret;
                }
            }
        }
        /// <summary>
        /// Asynchronously loads the saved application settings located at <c>settings.json</c> (in the current directory).
        /// </summary>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        /// <exception cref="FileNotFoundException">The specified file could not be found.</exception>
        public static async Task<ApplicationSettings> LoadAsync() {
            return await LoadAsync(DEFAULT_SETTINGS_FILE);
        }

        internal static void RegisterMainForm(MainForm form) {
            _main_form = form;
        }


        /// <summary>
        /// <para>Loads the saved application settings located at the specified file <paramref name="path"/>.</para>
        /// <para>If the specified file doesn't exist, the current default settings will be returned instead.</para>
        /// </summary>
        /// <param name="path">The path to the file containing the JSON settings.</param>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        public static ApplicationSettings LoadOrDefault(string path) {
            if (!File.Exists(path)) {
                ApplicationSettings ret = GetDefaultSettings();
                ret.Save();
                return ret;
            } else {
                try {
                    return Load(path);
                } catch {
                    return HandleBadSettingsFile(path);
                }
            }
        }
        private static ApplicationSettings HandleBadSettingsFile(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("Somehow, you managed to delete the settings file between loading it and recreating it. Nice job dingus.");
            }
            string fn = Path.GetFileName(path);
            string d = Path.GetDirectoryName(path);
            int n = 1;
            string newPath = Path.Combine(d, string.Format("{0}.{1}.bak", fn, n));
            while (File.Exists(newPath)) {
                n++;
                newPath = Path.Combine(d, string.Format("{0}.{1}.bak", fn, n));
            }
            File.Move(path, newPath);
            Console.WriteLine("The old settings file has been moved to the following location due to corruption:\n    {0}", newPath);
            ApplicationSettings ret = GetDefaultSettings();
            ret.OldFileWasLost = true;
            ret.OldFileLocation = newPath;
            ret.Save();
            return ret;
        }
        private static async Task<ApplicationSettings> HandleBadSettingsFileAsync(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("Somehow, you managed to delete the settings file between loading it and recreating it. Nice job dingus.");
            }
            string fn = Path.GetFileName(path);
            string d = Path.GetDirectoryName(path);
            int n = 1;
            string newPath = Path.Combine(d, string.Format("{0}.{1}.bak", fn, n));
            while (File.Exists(newPath)) {
                n++;
                newPath = Path.Combine(d, string.Format("{0}.{1}.bak", fn, n));
            }
            File.Move(path, newPath);
            Console.WriteLine("The old settings file has been moved to the following location due to corruption:\n    {0}", newPath);
            ApplicationSettings ret = GetDefaultSettings();
            ret.OldFileWasLost = true;
            ret.OldFileLocation = newPath;
            await ret.SaveAsync();
            return ret;
        }

        /// <summary>
        /// Loads the saved application settings located at <c>settings.json</c> (in the current directory) or returns
        /// the default settings if there is no <c>settings.json</c> file.
        /// </summary>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        public static ApplicationSettings LoadOrDefault() {
            return LoadOrDefault(DEFAULT_SETTINGS_FILE);
        }

        /// <summary>
        /// <para>Asynchronously loads the saved application settings located at the specified file <paramref name="path"/>.</para>
        /// <para>If the specified file doesn't exist, the current default settings will be returned instead.</para>
        /// </summary>
        /// <param name="path">The path to the file containing the JSON settings.</param>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        public static async Task<ApplicationSettings> LoadOrDefaultAsync(string path) {
            if (!File.Exists(path)) {
                ApplicationSettings ret = GetDefaultSettings();
                await ret.SaveAsync();
                return ret;
            } else {
                try {
                    return await LoadAsync(path);
                } catch {
                    return await HandleBadSettingsFileAsync(path);
                }
            }
        }

        /// <summary>
        /// Asynchronously loads the saved application settings located at <c>settings.json</c> (in the current directory) or returns
        /// the default settings if there is no <c>settings.json</c> file.
        /// </summary>
        /// <returns>A new <see cref="ApplicationSettings"/> object containing the loaded settings.</returns>
        /// <remarks>
        /// <para>Be cautious about the settings version. Version 1.0 is not the same as Version 1.1 and so on.</para>
        /// <para>The file must use the UTF-8 encoding.</para>
        /// </remarks>
        public static async Task<ApplicationSettings> LoadOrDefaultAsync() {
            return await LoadOrDefaultAsync(DEFAULT_SETTINGS_FILE);
        }
        public object Clone() {
            return new ApplicationSettings() {
                InternalVersion = this.InternalVersion is null ? null : this.InternalVersion + "",
                InnerSettings = (ISettingsObject)this.InnerSettings.Clone()
            };
        }
        //might remove the above and just clone the object.
        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) {
                return true; //reference compare to see if we're comparing the object to itself.
                //just so we can possibly skimp out on shit.
            }
            if((obj == null) || !(obj is ApplicationSettings settings)) {
                return false;
            }
            
            if (!this.VersionEquals(settings)) {
                return false;
                //KEEP THIS THIS WAY, DO NOT MERGE THE BELOW.
                //IF VERSION DOESN'T MATCH, THERE'S NO POINT CHECKING FOR EQUALITY.
            }
            //objects are the same type. compare them
            return this.InnerSettings == settings.InnerSettings;
        }
        /// <summary>
        /// Determines whether or not two <see cref="ApplicationSettings"/> objects have the same settings.
        /// </summary>
        /// <param name="a">The first <see cref="ApplicationSettings"/> object.</param>
        /// <param name="b">The second <see cref="ApplicationSettings"/> object.</param>
        /// <returns>A boolean value indicating whether or not the two objects have the same settings.</returns>
        public static bool operator ==(ApplicationSettings a, ApplicationSettings b) {
            return a.Equals(b);
        }
        /// <summary>
        /// Determines whether or not two <see cref="ApplicationSettings"/> objects do not have the same settings.
        /// </summary>
        /// <param name="a">The first <see cref="ApplicationSettings"/> object.</param>
        /// <param name="b">The second <see cref="ApplicationSettings"/> object.</param>
        /// <returns>A boolean value indicating whether or not the two objects do not have the same settings.</returns>

        public static bool operator !=(ApplicationSettings a, ApplicationSettings b) {
            return !(a == b);
        }
        /// <summary>
        /// Upgrades the current <see cref="ApplicationSettings"/> file to the newest supported version.
        /// </summary>
        public static ApplicationSettings Upgrade(ApplicationSettings settings) {
            while (true) {
                Version nextUpgrade = null;
                try {
                    nextUpgrade = UPGRADE_OPERATIONS.Keys.Where(v => settings.Version < v).Min();
                } catch {
                    //no upgrades available.
                    return settings;
                }
                settings = UPGRADE_OPERATIONS[nextUpgrade](settings);
            }
        }
        /// <summary>
        /// Detects whether or not the current <see cref="ApplicationSettings"/> needs to be upgraded.
        /// </summary>
        /// <param name="newVersion">The new version, if any, the settings should be upgraded to</param>
        /// <returns>
        /// A boolean value indicating if this <see cref="ApplicationSettings"/> must be upgraded before being used.
        /// </returns>
        public bool IsUpgradeNeeded(out Version newVersion) {
            while (true) {
                Version nextUpgrade = null;
                try { nextUpgrade = UPGRADE_OPERATIONS.Keys.Where(v => this.Version < v).Min(); }
                catch { /* no upgrades available. */ }
                newVersion = nextUpgrade;
                return nextUpgrade != null;
            }
        }
        /// <summary>
        /// Upgrades the current <see cref="ApplicationSettings"/> file to the newest supported version in-place.
        /// </summary>
        public bool Upgrade() {
            bool didAnything = false;
            while (true) {
                Version nextUpgrade = null;
                try {
                    nextUpgrade = UPGRADE_OPERATIONS.Keys.Where(v => this.Version < v).Min();
                } catch {
                    //no upgrades available.
                    return false || didAnything;
                }
                UPGRADE_OPERATIONS[nextUpgrade](this, true);
                didAnything = true;
            }
        }

        /// <summary>
        /// Gets whether or not the versions of two <see cref="ApplicationSettings"/> objects should be treated as equal
        /// despite potentially being separate values.
        /// </summary>
        /// <param name="other">The other <see cref="ApplicationSettings"/> object.</param>
        /// <returns>
        /// <para>
        /// <c>true</c> if this <see cref="ApplicationSettings"/> has a <see cref="Version"/> that is not semantically
        /// different from <paramref name="other"/>'s <see cref="Version"/>.
        /// </para>
        /// <para>
        /// This will return <c>true</c> if, before any settings upgrades, neither <see cref="ApplicationSettings"/> object
        /// must be upgraded to be compatible with the other. (Since <see cref="Version"/> defaults to the application version).
        /// </para>
        /// </returns>
        public bool VersionEquals(ApplicationSettings other) {
            if (other is null) {
                return false;
            }
            if (UPGRADE_OPERATIONS.Count == 0) {
                return true;
            }
            Version selfLast = null;
            Version otherLast = null;
            try { selfLast = UPGRADE_OPERATIONS.Keys.Where(v => this.Version >= v).Max(); } catch { selfLast = this.Version; }
            try { otherLast = UPGRADE_OPERATIONS.Keys.Where(v => other.Version >= v).Max(); } catch { otherLast = this.Version; }
            if (selfLast < UPGRADE_OPERATIONS.Keys.Min()) {
                selfLast = new Version(0, 0, 0, 0);
            }
            if (otherLast < UPGRADE_OPERATIONS.Keys.Min()) {
                otherLast = new Version(0, 0, 0, 0);
            }
            //the above two conditionals are required since the oldest settings versions would end up being themselves.
            //ie: if the first upgrade is at 1.0, and ver a is 0.8 and ver b is 0.9, without the above two,
            //we do 0.8 != 0.9. However, they're both actually the same. Thus, if the lowest version in the dictionary
            //is greater than either, set that version to 0.0.
            return selfLast == otherLast;
        }
        public bool Equals(ApplicationSettings other) {
            return this.Equals((object)other);
        }

        public interface ISettingsObject : ICloneable { }
        public interface ISettingsObject<T> : IEquatable<T>, ISettingsObject
            where T : ISettingsObject<T> {
            //this... actually works...
            //this whole constraint ensures that T is set to the class inheriting from ISettingsObject<T>
            //Technically, if I had A:ISO<B> and B:ISO<A>, that might also work... though, the circular ref might cause issues.
            //the only issue is that there's no way to tell what the T is alone, since the T is literally the class.
            //the whole point of this is to include the proper interfaces with the individual settings objects

            //IE: IT ENSURES: public class MyClass : ISettingsObject<MyClass>
        }
        /// <summary>
        /// Specifies the version of the application this <see cref="ISettingsObject"/> was created in.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class SettingsVersionAttribute : Attribute {
            /// <summary>
            /// Specifies the version of the application this <see cref="ISettingsObject"/> was created in.
            /// </summary>
            /// <param name="version">The textual version, eg: <c>1.0.0.0</c>.</param>
            public SettingsVersionAttribute(string version) {
                this.SettingsVersion = Version.Parse(version);
            }
            /// <summary>
            /// Gets the version of the application this <see cref="ISettingsObject"/> was created in.
            /// </summary>
            public Version SettingsVersion { get; }
            /// <summary>
            /// Gets the <see cref="Type"/> of the <see cref="ISettingsObject"/> that preceeded this one.
            /// Set this to <c>null</c> to indicate no previous settings object.
            /// </summary>
            public Type PreviousSettingsVersion { get; set; }
        }

        [Serializable]
        public class ApplicationSettingsException : Exception {
            public ApplicationSettingsException() { }
            public ApplicationSettingsException(string message) : base(message) { }
            public ApplicationSettingsException(string message, Exception inner) : base(message, inner) { }
            protected ApplicationSettingsException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        private static bool SettingsVersionCheck(ApplicationSettings settings) {
            Version sVersion = settings.Version;
            Type sType = settings.InnerSettings.GetType();
            SettingsVersionAttribute sva = sType.GetCustomAttribute<SettingsVersionAttribute>();
            SettingsVersionAttribute svaPrevious = null;
            Version aVersion = Version.Parse(AssemblyDetails.ASSEMBLY_FILE_VERSION);
            if (sVersion < sva.SettingsVersion) {
                //invalid? eg: SettingsV0 says 0.1.5.0, but we're at 0.1.4.27.
                //I don't think we should care too much.
                return false;
            }
            if (!(sva.PreviousSettingsVersion is null)) {
                svaPrevious = sva.PreviousSettingsVersion.GetCustomAttribute<SettingsVersionAttribute>();
                if (sVersion < svaPrevious.SettingsVersion) {
                    return false;
                }
            }
            if (aVersion < sVersion) {
                return false; //using newer settings file in old app...
            }
            return true;
        }
        #endregion

        #region Actual settings...


        [JsonProperty(PropertyName = "version")]
        private string InternalVersion { get; set; } = CURRENT_SETTINGS_FILE_VERSION.ToString();
        [JsonIgnore]
        public Version Version => Version.Parse(this.InternalVersion);
        [JsonProperty(PropertyName = "@@INSTRUCTIONS@@")]
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS0414 // The field is assigned but its value is never used
        private string _instructions = "Caution while editing this file manually. " +
            "There is a bunch of metadata that can easily be screwed up. IE: If you " +
            "don't know what you're doing, DO NOT EDIT THIS FILE";
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0414 // The field is assigned but its value is never used
        //hopefully... hopefully this will be serialized properly.
        [JsonProperty(PropertyName = "settings")]
        private ISettingsObject InnerSettings { get; set; }
        //the goal is to have a base ISettingsObject which will be internally used.
        //this object will automatically be deserialized into the proper SettingsV# object. (not necessarily same type as Settings
        //we will cast this internally.
        //this is here so ANY SettingsV# object gets serialized or deserialized.
        //This allows us to semi-cleanly allow multiple different versions of settings objects.
        //settings are automatically upgraded internally upon being loaded.
        //the upgrades are done in steps. (so, SettingsV0 turns into SettingsV1 and then into SettingsV2 and so on)
        [JsonIgnore]
        public SettingsV0 Settings {
            get {
                if(this.IsUpgradeNeeded(out _)) {
                    throw new InvalidOperationException("The settings must be upgraded first!");
                }
                return this.InnerSettings as SettingsV0;
            }
        }
        /// <summary>
        /// Gets or sets an object that was created by <see cref="LoadingForm"/> while loading the application.
        /// This object is used to transfer other non-serialized information from the loading site to the application.
        /// This property will not be used in comparisons or cloning operations.
        /// </summary>
        [JsonIgnore]
        public object LoadingObject { get; internal set; }
        /// <summary>
        /// Gets whether or not the actual settings.json was backed-up and removed due to corrpution.
        /// </summary>
        [JsonIgnore]
        public bool OldFileWasLost { get; private set; } = false;
        /// <summary>
        /// Gets the new path to the old settings.json file should the user desire to salvage it.
        /// </summary>
        [JsonIgnore]
        public string OldFileLocation { get; private set; } = null;
        //update the class above with the newest SettingsV object every time a new SettingsV object is introduced.
        #endregion

        #region Settings Objects
        [SettingsVersion("0.1.4.25", PreviousSettingsVersion = null)]
        public class SettingsV0 : ISettingsObject<SettingsV0> {
            #region Methods and crap
            public object Clone() {
                return new SettingsV0() {
                    Servers = this.Servers.Select(a => (WebSocketServerSettings)a.Clone()).ToArray(),
                    Devices = this.Devices.Select(a => (AudioDeviceSettings)a.Clone()).ToArray()
                };
            }
            public override int GetHashCode() {
                //Figure out how to get a proper hash code...
                return base.GetHashCode();
            }

            public bool Equals(SettingsV0 other) {
                return this.Equals((object)other);
            }
            public override bool Equals(object obj) {
                if (ReferenceEquals(this, obj)) {
                    return true; //reference compare to see if we're comparing the object to itself.
                                 //just so we can possibly skimp out on shit.
                }
                if ((obj == null) || !(obj is SettingsV0 settings)) {
                    return false;
                }
                if ((this.Servers is null != settings.Servers is null) 
                    || (this.Devices is null != settings.Devices is null)) {
                    return false;
                }
                if (this.Servers is null && this.Devices is null) {
                    return true;
                }
                //objects are the same type. compare them
                return (this.Servers == null || this.Servers.SequenceEqual(settings.Servers))
                    || (this.Devices == null || this.Devices.SequenceEqual(settings.Devices));
                
                
            }


            /// <summary>
            /// Determines whether or not two <see cref="SettingsV0"/> objects have the same settings.
            /// </summary>
            /// <param name="a">The first <see cref="SettingsV0"/> object.</param>
            /// <param name="b">The second <see cref="SettingsV0"/> object.</param>
            /// <returns>A boolean value indicating whether or not the two objects have the same settings.</returns>
            public static bool operator ==(SettingsV0 a, SettingsV0 b) {
                return a.Equals(b);
            }
            /// <summary>
            /// Determines whether or not two <see cref="SettingsV0"/> objects do not have the same settings.
            /// </summary>
            /// <param name="a">The first <see cref="SettingsV0"/> object.</param>
            /// <param name="b">The second <see cref="SettingsV0"/> object.</param>
            /// <returns>A boolean value indicating whether or not the two objects do not have the same settings.</returns>

            public static bool operator !=(SettingsV0 a, SettingsV0 b) {
                return !(a == b);
            }
            #endregion

            public class WebSocketServerSettings : ISettingsObject<WebSocketServerSettings> {
                #region Methods and crap
                public object Clone() {
                    return new WebSocketServerSettings() {
                        Port = this.Port,
                        Secure = this.Secure,
                        SslEnabledProtocols = this.SslEnabledProtocols,
                        SslRequireClientCertificate = this.SslRequireClientCertificate,
                        SslServerCertificatePath = (this.SslServerCertificatePath is null) ? null : this.SslServerCertificatePath + "",
                        Uri = (this.Uri is null) ? null : this.Uri + "",
                        SslCheckCertificateRevocation = this.SslCheckCertificateRevocation,
                        Enabled = this.Enabled
                    };
                }
                public override int GetHashCode() {
                    //Figure out how to get a proper hash code...
                    return this.Port.GetHashCode()
                        ^ this.Secure.GetHashCode()
                        ^ this.SslEnabledProtocols.GetHashCode()
                        ^ this.SslRequireClientCertificate.GetHashCode()
                        ^ this.SslServerCertificatePath.GetHashCode()
                        ^ this.Uri.GetHashCode()
                        ^ this.SslCheckCertificateRevocation.GetHashCode()
                        ^ this.Enabled.GetHashCode();
                }

                public bool Equals(WebSocketServerSettings other) {
                    return this.Equals((object)other);
                }
                public override bool Equals(object obj) {
                    if (ReferenceEquals(this, obj)) {
                        return true; //reference compare to see if we're comparing the object to itself.
                                     //just so we can possibly skimp out on shit.
                    }
                    if ((obj == null) || !(obj is WebSocketServerSettings settings)) {
                        return false;
                    }
                    //objects are the same type. compare them
                    return this.Port == settings.Port
                        && this.Secure == settings.Secure
                        && this.SslCheckCertificateRevocation == settings.SslCheckCertificateRevocation
                        && this.SslEnabledProtocols == settings.SslEnabledProtocols
                        && this.SslRequireClientCertificate == settings.SslRequireClientCertificate
                        && this.SslServerCertificatePath == settings.SslServerCertificatePath
                        && this.Uri == settings.Uri
                        && this.Enabled == settings.Enabled;
                }


                /// <summary>
                /// Determines whether or not two <see cref="WebSocketServerSettings"/> objects have the same settings.
                /// </summary>
                /// <param name="a">The first <see cref="WebSocketServerSettings"/> object.</param>
                /// <param name="b">The second <see cref="WebSocketServerSettings"/> object.</param>
                /// <returns>A boolean value indicating whether or not the two objects have the same settings.</returns>
                public static bool operator ==(WebSocketServerSettings a, WebSocketServerSettings b) {
                    return a.Equals(b);
                }
                /// <summary>
                /// Determines whether or not two <see cref="WebSocketServerSettings"/> objects do not have the same settings.
                /// </summary>
                /// <param name="a">The first <see cref="WebSocketServerSettings"/> object.</param>
                /// <param name="b">The second <see cref="WebSocketServerSettings"/> object.</param>
                /// <returns>A boolean value indicating whether or not the two objects do not have the same settings.</returns>

                public static bool operator !=(WebSocketServerSettings a, WebSocketServerSettings b) {
                    return !(a == b);
                }
                #endregion

                [JsonProperty(PropertyName = "port")]
                public int Port { get; set; } = 8069;
                [JsonProperty(PropertyName = "secure")]
                public bool Secure { get; set; } = false;
                [JsonProperty(PropertyName = "uri")]
                public string Uri { get; set; } = "localhost";
                [JsonProperty(PropertyName = "sslRequireClientCert")]
                public bool SslRequireClientCertificate { get; set; } = false;
                [JsonProperty(PropertyName = "sslCheckCertRecovation")]
                public bool SslCheckCertificateRevocation { get; set; } = true;
                [JsonProperty(PropertyName = "sslEnabledProtocols")]
                public SslProtocols SslEnabledProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;
                [JsonProperty(PropertyName = "sslServerCertPath")]
                public string SslServerCertificatePath { get; set; } = "domain.crt";
                [JsonProperty(PropertyName = "enabled")]
                public bool Enabled { get; set; } = true;


                [JsonIgnore]
                public string FullUri => string.Format(
                    "ws{0}://{1}:{2}",
                    this.Secure ? "s" : "",
                    this.Uri ?? "localhost",
                    this.Port);

            }

            public class AudioDeviceSettings : ISettingsObject<AudioDeviceSettings> {
                #region Methods and crap
                public object Clone() {
                    return new AudioDeviceSettings() {
                        Capture = this.Capture,
                        DeviceId = this.DeviceId is null ? null : this.DeviceId + "",
                        DisplayName = this.DisplayName is null ? null : this.DisplayName + "",
                        Notes = this.Notes is null ? null : this.Notes + "",
                        ObsName = this.ObsName is null ? null : this.ObsName + "",
                        Type = this.Type
                    };
                }
                public override int GetHashCode() {
                    //Figure out how to get a proper hash code...
                    return this.Capture.GetHashCode()
                        ^ this.DeviceId.GetHashCode()
                        ^ this.DisplayName.GetHashCode()
                        ^ this.Notes.GetHashCode()
                        ^ this.ObsName.GetHashCode()
                        ^ this.Type.GetHashCode();
                }

                public bool Equals(AudioDeviceSettings other) {
                    return this.Equals((object)other);
                }
                public override bool Equals(object obj) {
                    if (ReferenceEquals(this, obj)) {
                        return true; //reference compare to see if we're comparing the object to itself.
                                     //just so we can possibly skimp out on shit.
                    }
                    if ((obj == null) || !(obj is AudioDeviceSettings settings)) {
                        return false;
                    }
                    //objects are the same type. compare them
                    return this.Capture == settings.Capture
                        && this.DeviceId == settings.DeviceId
                        && this.DisplayName == settings.DisplayName
                        && this.Notes == settings.Notes
                        && this.ObsName == settings.ObsName
                        && this.Type == settings.Type;
                }


                /// <summary>
                /// Determines whether or not two <see cref="AudioDeviceSettings"/> objects have the same settings.
                /// </summary>
                /// <param name="a">The first <see cref="AudioDeviceSettings"/> object.</param>
                /// <param name="b">The second <see cref="AudioDeviceSettings"/> object.</param>
                /// <returns>A boolean value indicating whether or not the two objects have the same settings.</returns>
                public static bool operator ==(AudioDeviceSettings a, AudioDeviceSettings b) {
                    return a.Equals(b);
                }
                /// <summary>
                /// Determines whether or not two <see cref="AudioDeviceSettings"/> objects do not have the same settings.
                /// </summary>
                /// <param name="a">The first <see cref="AudioDeviceSettings"/> object.</param>
                /// <param name="b">The second <see cref="AudioDeviceSettings"/> object.</param>
                /// <returns>A boolean value indicating whether or not the two objects do not have the same settings.</returns>

                public static bool operator !=(AudioDeviceSettings a, AudioDeviceSettings b) {
                    return !(a == b);
                }
                #endregion

                /// <summary>
                /// Contains the WASAPI-compliant Id of the device in question. (Not present
                /// for <see cref="DeviceType.ScreenCaptureInput"/> or <see cref="DeviceType.ScreenCaptureOutput"/>.)
                /// </summary>
                [JsonProperty(PropertyName = "id")]
                public string DeviceId { get; set; } = null;
                /// <summary>
                /// Gets or sets the string of characters the OCR will use to detect which OBS audio mixer slider is being
                /// tracked. (Only present for <see cref="DeviceType.ScreenCaptureInput"/> or <see cref="DeviceType.ScreenCaptureOutput"/>.)
                /// </summary>
                [JsonProperty(PropertyName = "obsName")]
                public string ObsName { get; set; } = null;
                /// <summary>
                /// Gets or sets a user supplied string that will be used in the UI to identify this device.
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string DisplayName { get; set; } = null;
                /// <summary>
                /// Gets or sets a user supplied string that can be used to allow the user to add notes to this device.
                /// </summary>
                [JsonProperty(PropertyName = "notes")]
                public string Notes { get; set; } = null;
                /// <summary>
                /// Gets or sets the type of device this is.
                /// The upper two bits of this enumeration will automatically be set by the application.
                /// The lower thirty bits will be set by the user since they're more helpful to the user than the application.
                /// </summary>
                [JsonProperty(PropertyName = "type")]
                public DeviceType Type { get; set; } = DeviceType.GenericInput;
                /// <summary>
                /// Gets or sets whether or not this device should be captured and relayed through the WebSocket server.
                /// </summary>
                [JsonProperty(PropertyName = "capture")]
                public bool Capture { get; set; } = true;


                [Flags]
                public enum DeviceType : uint {
                    GenericInput = 0x00000000,
                    GenericOutput = 0x80000000,
                    DeviceNumberMask = 0x3FFFFFFF,
                    ScreenCaptureInput = GenericInput | 0x40000000,
                    ScreenCaptureOutput = GenericOutput | 0x40000000,
                    Microphone = GenericInput | 1,
                    CaptureCard = ScreenCaptureInput | 2,
                    WebCam = ScreenCaptureInput | 3,
                    Speakers = GenericOutput | 1,
                    Headphones = GenericOutput | 2,
                }
            }


            [JsonProperty(PropertyName = "servers")]
            public WebSocketServerSettings[] Servers { get; set; } = new WebSocketServerSettings[0];

            [JsonProperty(PropertyName = "devices")]
            public AudioDeviceSettings[] Devices { get; set; } = new AudioDeviceSettings[0];

            [JsonProperty(PropertyName = "addDeviceForm.native.enabledColumns")]
            public string[] AddDeviceForm_Native_EnabledColumns { get; set; } = new string[0];

        }
        //settings layout: "." delimited list of components. last component is true setting name.

        #endregion
    }
}
//honestly, I can't wait until I end up having to straight up copy and paste the entire SettingsV0 class
//to create SettingsV1
//The fact that this actually works as intended first try is astonishing to me.