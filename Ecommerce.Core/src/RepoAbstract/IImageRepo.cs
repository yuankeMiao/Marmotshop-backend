
using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IImageRepo
    {
        Task<IEnumerable<Image>> UpdateImagesAsync(Guid productId, IEnumerable<Image> images);
    }
}