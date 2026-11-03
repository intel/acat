using System.Threading.Tasks;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Optional initialization contract for plugins with asynchronous startup work.
    /// Existing synchronous plugin interfaces remain unchanged.
    /// </summary>
    public interface IAsyncInitializer<in T>
    {
        Task<bool> InitAsync(T argument);
    }
}
