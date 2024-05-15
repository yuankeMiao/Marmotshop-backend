using Ecommerce.Core.src.Entity;

namespace Ecommerce.Core.src.RepoAbstract
{
    public interface IImageRepo
    {
        Task<IEnumerable<Image>> GetImagesByProductIdAsync(Guid productId);
        Task<Image> GetImageByIdAsync(Guid imageId);
        Task  UpdateImageUrlAsync(Guid imageId, string newUrl);
    }
}