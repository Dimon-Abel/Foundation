using Orleans;
using System.Threading.Tasks;

namespace Grains.Interfaces
{
    public interface IBase : IGrainWithGuidKey
    {
        Task<string> Return(string returnStr);
        Task Initialization();
    }
}
