namespace Mono.Cecil.AssemblyResolver
{
    public class AssemblyStrongNameExtended
    {
        private AssemblyStrongNameExtended()
        {
            this.StrongName = string.Empty;
            this.Architecture = TargetArchitecture.AnyCPU;
            this.Special = SpecialTypeAssembly.None;
        }

        public AssemblyStrongNameExtended(string strongName, TargetArchitecture architecture, SpecialTypeAssembly special)
        {
            this.StrongName = strongName;
            this.Architecture = architecture;
            this.Special = special;
        }

        public string StrongName { get; set; }

        public TargetArchitecture Architecture { get; set; }

        public SpecialTypeAssembly Special { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            AssemblyStrongNameExtended other = obj as AssemblyStrongNameExtended;

            return this.Equals(other);
        }

        public static bool operator ==(AssemblyStrongNameExtended first, AssemblyStrongNameExtended second)
        {
            if (((object)first == null) || ((object)second == null))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(AssemblyStrongNameExtended first, AssemblyStrongNameExtended second)
        {
            return !first.Equals(second);
        }

        public bool Equals(AssemblyStrongNameExtended other)
        {
            if (other == null)
            {
                return false;
            }

            return this.StrongName == other.StrongName && this.Architecture == other.Architecture && this.Special == other.Special;
        }

        public override int GetHashCode()
        {
            return this.StrongName.GetHashCode();
        }
    }
}
