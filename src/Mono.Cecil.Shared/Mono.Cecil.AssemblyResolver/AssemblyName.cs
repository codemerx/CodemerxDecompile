namespace Mono.Cecil.AssemblyResolver
{
    using System;
    using System.Linq;
    using System.IO;

    public class AssemblyName
    {
        public AssemblyName(string name, string fullName, Version version, byte[] publicKey, string defaultDir)
            : this(name, fullName, version, publicKey)
        {
            this.DefaultDir = defaultDir;
        }

        public AssemblyName(string name, string fullName, Version version, byte[] publicKey)
        {
            this.Name = name;
            this.FullName = fullName;
            this.Version = version;
            this.PublicKeyToken = publicKey;
        }

        public string DefaultDir { get; private set; }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public byte[] PublicKeyToken { get; private set; }

        public string FullName { get; private set; }

        public TargetArchitecture TargetArchitecture { get; set; }

        public bool HasDefaultDir
        {
        	get
            {
                return !string.IsNullOrEmpty(this.DefaultDir) 
                    && Directory.Exists(this.DefaultDir);
            }
        }

        internal string ParentDirectory(TargetPlatform runtime)
        {
            if (runtime == TargetPlatform.CLR_4)
            {
                return string.Format("v4.0_{0}__{1}", Version.ToString(), PublicKeyTokenAsString());
            }
            else if (runtime == TargetPlatform.CLR_2_3)
            {
                return string.Format("{0}__{1}", Version.ToString(), PublicKeyTokenAsString());
            }
            return string.Empty;
        }

        private string PublicKeyTokenAsString()
        {
            return PublicKeyToken != null
                                                 ? string.Join(string.Empty, PublicKeyToken.Select(b => b.ToString("x2")).ToArray())
                                                 : string.Empty;
        }

        internal string[] SupportedVersions(TargetPlatform runtime)
        {
            switch (runtime)
            {
                case TargetPlatform.CLR_2_3:
                case TargetPlatform.CLR_4:
                    return SystemInformation.CLR_GAC_VERSIONS;

                case TargetPlatform.WindowsPhone:
                    return SystemInformation.WINDOWS_PHONE_VERSIONS;

                case TargetPlatform.WindowsCE:
                    return SystemInformation.WINDOWS_CE_VERSIONS;

                case TargetPlatform.Silverlight:
                    return SystemInformation.SILVERLIGHT_VERSIONS;

				case TargetPlatform.NetCore:
					return SystemInformation.NETCORE_VERSIONS;

				default:
                    return new string[0];
            }
        }
    }
}
