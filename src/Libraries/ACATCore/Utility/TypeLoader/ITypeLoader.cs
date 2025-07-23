using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Core.Utility
{
    public interface ITypeLoader<TInterface>
    {
        IReadOnlyDictionary<Guid, Type> LoadedTypes { get; }
        void LoadFromAssembly(string assemblyPath);
        void LoadFromAssemblies(IEnumerable<string> assemblyPaths);
    }
}
