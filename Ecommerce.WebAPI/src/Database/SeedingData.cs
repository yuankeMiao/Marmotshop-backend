using Bogus;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.WebAPI.src.Service;

namespace Ecommerce.WebAPI.src.Database
{
    public class SeedingData
    {
        public static List<Category> GetCategories()
        {
            var faker = new Faker("en");
            var categories = new List<Category>();
            List<string> categoryNames = ["Sport", "Clothes", "Games", "Self care", "Books"];

            foreach (var categoryName in categoryNames)
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryName,
                    Image = faker.Image.PicsumUrl(),
                };
                categories.Add(category);
            }

            return categories;
        }
 
        public static List<User> GetUsers()
        {
            var passwordService = new PasswordService();
            var users = new List<User>();
            var faker = new Faker("en");

            // customers
            for (int i = 0; i < 100; i++)
            {
                var rawPsw = faker.Internet.Password();
                var hashedPassword = passwordService.HashPassword(rawPsw, out byte[] salt);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Firstname = faker.Person.FirstName,
                    Lastname = faker.Person.LastName,
                    Email = i.ToString() + faker.Person.Email,
                    Password = hashedPassword,
                    Salt = salt,
                    Avatar = faker.Person.Avatar,
                    Role = UserRole.Customer
                };
                users.Add(user);
            }

            // admins
            var hashedYuankePassword = passwordService.HashPassword("yuanke@123", out byte[] yuankeSalt);
            var hashedLukaPassword = passwordService.HashPassword("yuanke@123", out byte[] lukaSalt);

            var admin1 = new User()
            {
                Id = Guid.NewGuid(),
                Firstname = "Yuanke",
                Lastname = "Miao",
                Email = "yuanke@admin.com",
                Password = hashedYuankePassword,
                Salt = yuankeSalt,
                Avatar = faker.Person.Avatar,
                Role = UserRole.Admin,
            };
            var admin2 = new User()
            {
                Id = Guid.NewGuid(),
                Firstname = "Luka",
                Lastname = "Miao",
                Email = "luka@admin.com",
                Password = hashedLukaPassword,
                Salt = lukaSalt,
                Avatar = faker.Person.Avatar,
                Role = UserRole.Admin,
            };
            users.Add(admin1);
            users.Add(admin2);

            return users;
        }

        public static List<Address> GetAddresses(List<User> users)
        {
            var addresses = new List<Address>();

            // only gave half users address, since it's optional
            // giva users random amount addresses, from 1 to 4, to simulate the real life data
            return addresses;
        }


        public static List<Product> GetProducts(List<Category> categories)
        {
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

  
        public static List<Image> GetImages(List<Product> products)
        {
            var faker = new Faker("en");

            var images = new List<Image>();
            foreach (var product in products)
            {
                for (int i = 0; i < 5; i++)
                {
                    var image = new Image
                    {
                        Id = Guid.NewGuid(),
                        Url = faker.Image.PicsumUrl(),
                        ProductId = product.Id,
                    };
                    images.Add(image);
                }
            }

            return images;
        }
   
    }
}