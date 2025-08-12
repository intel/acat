using System;
using System.Collections.Generic;

namespace ACAT.Core.Utility.TypeLoader
{
    public interface ITypeLoader<TInterface>
    {
        IReadOnlyDictionary<Guid, Type> LoadedTypes { get; }
        void LoadFromAssembly(string assemblyPath);
        void LoadFromAssemblies(IEnumerable<string> assemblyPaths);
    }
}
