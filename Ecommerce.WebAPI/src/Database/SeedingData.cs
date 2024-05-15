using Bogus;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.WebAPI.src.Service;

namespace Ecommerce.WebAPI.src.Database
{
    public class SeedingData
    {
        private static Random random = new Random();
        private static int GetRandomNumber()
        {
            return random.Next(1, 11);
        }
        private static int GetRandomNumberForImage()
        {
            return random.Next(100, 1000);
        }

        private static int RandomNumber1 = GetRandomNumber();
        private static int RandomNumber2 = GetRandomNumber();
        private static int RandomNumber3 = GetRandomNumber();
        private static int RandomNumber4 = GetRandomNumber();
        private static int RandomNumber5 = GetRandomNumber();
        private static int RandomNumber6 = GetRandomNumber();

        #region Categories
        private static Category category1 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronic",
            Image = $"https://picsum.photos/200/?random={RandomNumber1}"
        };
        private static Category category2 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Clothing",
            Image = $"https://picsum.photos/200/?random={RandomNumber2}"
        };
        private static Category category3 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Furniture",
            Image = $"https://picsum.photos/200/?random={RandomNumber3}"
        };
        private static Category category4 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Books",
            Image = $"https://picsum.photos/200/?random={RandomNumber4}"
        };
        private static Category category5 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Toys",
            Image = $"https://picsum.photos/200/?random={RandomNumber5}"
        };
        private static Category category6 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Sports",
            Image = $"https://picsum.photos/200/?random={RandomNumber6}"
        };

        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                category1, category2, category3, category4, category5, category6
            };
        }
        #endregion

        #region Users
        public static List<User> GetUsers()
        {
            var passwordService = new PasswordService();
            var hashedYuankePassword = passwordService.HashPassword("yuanke@123", out byte[] yuankeSalt);
            var hashedCustomerPassword = passwordService.HashPassword("customer@123", out byte[] customerSalt);
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = "Yuanke",
                    Lastname = "Miao",
                    Email = "yuanke@admin.com",
                    Password = hashedYuankePassword,
                    Salt= yuankeSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    Role = UserRole.Admin,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                     Firstname = "John",
                    Lastname = "Doe",
                    Email = "John@customer.com",
                    Password = hashedCustomerPassword,
                    Salt= customerSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    Role = UserRole.Customer,
                }
            };
        }

        #endregion

        #region products
        public static List<Product> GetProducts()
        {
            var categories = GetCategories();
            var products = new List<Product>();

            var faker = new Faker("en");

            foreach (var category in categories)
            {
                for (int i = 0; i < 20; i++)
                {
                    var product = new Product
                    {
                        Id = Guid.NewGuid(),
                        Title = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.Product()} {faker.Random.Word()}", // make sure title is unique
                        Description = faker.Commerce.ProductDescription(),
                        Price = decimal.Parse(faker.Commerce.Price()),
                        DiscountPercentage = faker.Commerce.Random.Int(0, 30),
                        Rating = faker.Commerce.Random.Int(1, 5),
                        Stock = faker.Commerce.Random.Int(0, 200),
                        Brand = faker.Random.Word(),
                        CategoryId = category.Id,
                        Thumbnail = faker.Image.PicsumUrl(),
                    };
                    products.Add(product);
                }
            }
            return products;
        }

        #endregion
    }
}