using Orleans;
using System.Threading.Tasks;

namespace Grains.Interfaces
{
    public interface IBase : IGrainWithIntegerKey
    {
        Task<string> Return(string returnStr);
        Task Initialization();
    }
}
