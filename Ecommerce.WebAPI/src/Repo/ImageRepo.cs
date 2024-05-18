
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.WebAPI.src.Database;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Repo
{
    public class ImageRepo : IImageRepo
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Image> _images;

        public ImageRepo(AppDbContext context)
        {
            _context = context;
            _images = _context.Images;
        }
        public async Task<IEnumerable<Image>> UpdateImagesAsync(Guid productId, IEnumerable<Image> images)
        {
            var existingImages = await _images.Where(ei => ei.ProductId == productId).ToListAsync();

            // remove them all
            _images.RemoveRange(existingImages);

            // add new images
            await _images.AddRangeAsync(images);

            await _context.SaveChangesAsync();

            return images;
        }
    }
}