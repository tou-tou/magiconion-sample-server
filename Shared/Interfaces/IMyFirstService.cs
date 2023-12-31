using MagicOnion;

namespace Shared.Interfaces
{
    // copied from https://github.com/Cysharp/MagicOnion#service-definition-shared-library
    // Defines .NET interface as a Server/Client IDL.
    // The interface is shared between server and client.
    public interface IMyFirstService : IService<IMyFirstService>
    {
        // The return type must be `UnaryResult<T>` or `UnaryResult`.
        UnaryResult<int> SumAsync(int x, int y);
    }
}