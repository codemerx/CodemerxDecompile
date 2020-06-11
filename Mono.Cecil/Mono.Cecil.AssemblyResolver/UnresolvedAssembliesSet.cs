using System;
using System.Collections.Generic;

namespace Mono.Cecil.AssemblyResolver
{
    /*Telerik Authorship*/
    public interface IClonableCollection<T> : ICollection<T>
    {
        IClonableCollection<T> Clone();
    }

    /*Telerik Authorship*/
    internal class UnresolvedAssembliesCollection : IClonableCollection<AssemblyStrongNameExtended>
    {
        private static readonly HashSet<AssemblyStrongNameExtended> UnresolvableAssemblies = new HashSet<AssemblyStrongNameExtended>()
        {
            new AssemblyStrongNameExtended("mscorlib, Version=255.255.255.255, Culture=neutral, PublicKeyToken=b77a5c561934e089", TargetArchitecture.AnyCPU, SpecialTypeAssembly.None),
            new AssemblyStrongNameExtended("mscorlib, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", TargetArchitecture.AnyCPU, SpecialTypeAssembly.None)
		};

        /*Telerik Authorship*/
        private readonly HashSet<AssemblyStrongNameExtended> theSet;

        /*Telerik Authorship*/
        public UnresolvedAssembliesCollection()
        {
            this.theSet = new HashSet<AssemblyStrongNameExtended>();
        }

        /*Telerik Authorship*/
        public UnresolvedAssembliesCollection(IEnumerable<AssemblyStrongNameExtended> collection)
        {
            this.theSet = new HashSet<AssemblyStrongNameExtended>(collection);
        }

        /*Telerik Authorship*/
        public bool Contains(AssemblyStrongNameExtended item)
        {
            return UnresolvableAssemblies.Contains(item) || this.theSet.Contains(item);
        }

        /*Telerik Authorship*/
        public IClonableCollection<AssemblyStrongNameExtended> Clone()
        {
            return new UnresolvedAssembliesCollection(this.theSet);
        }

        /*Telerik Authorship*/
        public void Add(AssemblyStrongNameExtended item)
        {
            this.theSet.Add(item);
        }

        public void Clear()
        {
            this.theSet.Clear();
        }

        /*Telerik Authorship*/
        public void CopyTo(AssemblyStrongNameExtended[] array, int arrayIndex)
        {
            this.theSet.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.theSet.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /*Telerik Authorship*/
        public bool Remove(AssemblyStrongNameExtended item)
        {
            return this.theSet.Remove(item);
        }

        /*Telerik Authorship*/
        public IEnumerator<AssemblyStrongNameExtended> GetEnumerator()
        {
            return this.theSet.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.theSet.GetEnumerator();
        }
    }
}
