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

            // customers
            for (int i = 0; i < 100; i++)
            {
                var faker = new Faker("en");
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

            var avatarFaker = new Faker("en");
            var admin1 = new User()
            {
                Id = Guid.NewGuid(),
                Firstname = "Yuanke",
                Lastname = "Miao",
                Email = "yuanke@admin.com",
                Password = hashedYuankePassword,
                Salt = yuankeSalt,
                Avatar = avatarFaker.Person.Avatar,
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
                Avatar = avatarFaker.Person.Avatar,
                Role = UserRole.Admin,
            };
            users.Add(admin1);
            users.Add(admin2);

            return users;
        }

        public static List<Address> GetAddresses(List<User> users)
        {
            var addresses = new List<Address>();

            // giva users random amount addresses, from 0 to 3, to simulate the real life data
            foreach (var user in users)
            {
                int addressAmount = new Random().Next(0, 3);
                for (int i = 0; i < addressAmount; i++)
                {
                    var faker = new Faker("fi");
                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Recipient = faker.Person.FullName,
                        Phone = faker.Person.Phone,
                        Line1 = faker.Address.SecondaryAddress(),
                        Line2 = faker.Address.StreetAddress(),
                        PostalCode = faker.Address.ZipCode(),
                        City = faker.Address.City(),
                        UserId = user.Id,
                    };
                    addresses.Add(address);
                }
            }
            return addresses;
        }


        public static List<Product> GetProducts(List<Category> categories)
        {
            var products = new List<Product>();

            foreach (var category in categories)
            {
                for (int i = 0; i < 20; i++)
                {
                    var faker = new Faker("en");
                    var product = new Product
                    {
                        Id = Guid.NewGuid(),
                        Title = $"{faker.Commerce.ProductAdjective()} {faker.Commerce.Product()} {faker.Random.Word()}",
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

            var images = new List<Image>();
            foreach (var product in products)
            {
                for (int i = 0; i < 5; i++)
                {
                    var faker = new Faker("en");
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

        public static List<Order> GetOrders(List<User> users)
        {
            var orders = new List<Order>();

            foreach (var user in users)
            {
                int orderAmount = new Random().Next(0, 10);
                for (int i = 0; i < orderAmount; i++)
                {
                    var faker = new Faker("fi");
                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Status = faker.PickRandom<OrderStatus>(),
                        ShippingAddress = faker.Address.FullAddress()
                    };
                    orders.Add(order);
                }
            }

            return orders;
        }

        public static List<OrderProduct> GetOrderProducts(List<Order> orders, List<Product> products)
        {
            var orderProducts = new List<OrderProduct>();

            foreach (var order in orders)
            {
                var faker = new Faker("en");
                var randomProducts = faker.Random.ListItems(products, 20);
                int productAmount = faker.Random.Int(1, 20);
                for (int i = 0; i < productAmount; i++)
                {
                    var randomProduct = randomProducts[i];

                    var actualPrice = decimal.Round(randomProduct.Price - randomProduct.Price * randomProduct.DiscountPercentage / 100, 2);
                    var quantity = faker.Random.Int(1, 30);

                    var orderProduct = new OrderProduct
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = randomProduct.Id,
                        Title = randomProduct.Title,
                        Thumbnail = randomProduct.Thumbnail,
                        ActualPrice = actualPrice,
                        Quantity = quantity,
                        TotalPrice = decimal.Round(actualPrice * quantity, 2)
                    };

                    orderProducts.Add(orderProduct);

                }
            }

            return orderProducts;
        }

        public static List<Review> GetReviews(List<User> users, List<Product> products)
        {
            var reviews = new List<Review>();

            foreach (var product in products)
            {
                var faker = new Faker("en");
                int reveiwAmount = faker.Random.Int(0, 100);
                for (int i = 0; i < reveiwAmount; i++)
                {
                    var randomUser = faker.Random.ListItem(users);
                    var review = new Review
                    {
                        Id = Guid.NewGuid(),
                        Rating = faker.Random.Int(1, 5),
                        Content = faker.Lorem.Paragraph(),
                        UserId = randomUser.Id,
                        ProductId = product.Id,
                    };

                    reviews.Add(review);
                }
            }
            return reviews;
        }
    }
}