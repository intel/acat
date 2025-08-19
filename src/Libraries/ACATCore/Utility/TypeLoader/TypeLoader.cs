using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ACAT.Core.Utility.TypeLoader
{
    public class TypeLoader<TInterface> : ITypeLoader<TInterface>
        where TInterface : class, IPluginExtension
    {
        private readonly Dictionary<Guid, Type> _typeCache = new();

        public IReadOnlyDictionary<Guid, Type> LoadedTypes => _typeCache;

#if SIGNED_RELEASE
        public static bool IsAssemblyStrongNamed(string assemblyPath)
        {
            try
            {
                var name = AssemblyName.GetAssemblyName(assemblyPath);
                return name.GetPublicKeyToken()?.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static readonly byte[] AllowedPublicKeyToken = { 0x12, 0x34, 0x56, 0x78, 0xAB, 0xCD, 0xEF, 0x01 };

        public static bool IsFromTrustedPublisher(string assemblyPath)
        {
            try
            {
                var name = AssemblyName.GetAssemblyName(assemblyPath);
                var token = name.GetPublicKeyToken();
                return token != null && token.SequenceEqual(AllowedPublicKeyToken);
            }
            catch
            {
                return false;
            }
        }
#endif

        private static bool IsDotNetAssembly(string assemblyPath)
        {
            try
            {
                AssemblyName.GetAssemblyName(assemblyPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void LoadFromAssembly(string assemblyPath, bool firstordefault = true)
        {
            if (!File.Exists(assemblyPath))
                throw new FileNotFoundException("Assembly not found", assemblyPath);

#if SIGNED_RELEASE

            // In signed release builds, we enforce strong-naming and publisher trust checks
            if (!IsFromTrustedPublisher(assemblyPath))
            {
                if (!IsAssemblyStrongNamed(assemblyPath))
                    throw new InvalidOperationException("Assembly is not strong-named.");
            }
#endif
            if (IsDotNetAssembly(assemblyPath))
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                LoadTypesFromAssembly(assembly, firstordefault);
            }
        }


        public void LoadFromAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (var path in assemblyPaths)
            {
                LoadFromAssembly(path);
            }
        }

        public void AddAssemblytoCache(Guid id, Type type)
        {
            if (id == Guid.Empty || type == null)
                throw new ArgumentException("Invalid type or ID");
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException($"Type {type.FullName} does not implement {typeof(TInterface).FullName}");
            _typeCache[id] = type;
        }

        private void LoadTypesFromAssembly(Assembly assembly, bool firstordefault = true)
        {
            try
            {
                Type[] types;
                try
                {
                    // Handle type loading issues gracefully
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Some types couldn't load, fall back to what we can access
                    types = ex.Types.Where(t => t != null).ToArray();
                }
                if (firstordefault)
                {
                    var matchingType = types
                    .Where(type =>
                        type.IsClass && !type.IsAbstract &&
                        typeof(TInterface).IsAssignableFrom(type))
                    .FirstOrDefault(type =>
                    {
                        var attr = ClassDescriptorAttribute.GetDescriptor(type);
                        return attr != null && attr.Id != Guid.Empty;
                    });

                    if (matchingType != null)
                    {
                        var attr = ClassDescriptorAttribute.GetDescriptor(matchingType);
                        //_typeCache[attr.Id] = matchingType;
                        AddAssemblytoCache(attr.Id, matchingType);
                    }
                }
                else
                {
                    var matchingTypes = types
                        .Where(type =>
                            type.IsClass &&
                            !type.IsAbstract &&
                            typeof(TInterface).IsAssignableFrom(type));
                            //&&
                            //ClassDescriptorAttribute.GetDescriptor(type) is { Id: var id } && id != Guid.Empty);

                    if (matchingTypes != null)
                    {
                        foreach (var matchingType in matchingTypes)
                        {
                            //var attr = ClassDescriptorAttribute.GetDescriptor(matchingType);
                            if (matchingType.GUID != null)
                            {
                                AddAssemblytoCache(matchingType.GUID, matchingType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or skip if instantiation fails
                Log.Exception($"Failed to create instance of {assembly.FullName}: {ex.Message}");
            }
        }
    }
}
