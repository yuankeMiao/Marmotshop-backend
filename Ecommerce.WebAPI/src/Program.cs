using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.Service.src.Shared;
using Ecommerce.WebAPI.src.AuthorizationPolicy;
using Ecommerce.WebAPI.src.Database;
using Ecommerce.WebAPI.src.Middleware;
using Ecommerce.WebAPI.src.Repo;
using Ecommerce.WebAPI.src.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
      options =>
    {
      options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
      {
        Description = "Bearer token authentication",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
      }
      );

      // swagger would add the token to the request header of routes with [Authorize] attribute
      options.OperationFilter<SecurityRequirementsOperationFilter>();
    }
);

// add all controllers
builder.Services.AddControllers();

// adding db context into project
var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Localhost"));
dataSourceBuilder.MapEnum<UserRole>();
dataSourceBuilder.MapEnum<OrderStatus>();
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<AppDbContext>
(
      options =>
      options
        .UseNpgsql(dataSource)
        .UseSnakeCaseNamingConvention()
// .AddInterceptors(new TimeStampInterceptor())
);

// service registration -> automatically create all instances of dependencies


builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IImageRepo, ImageRepo>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IReviewRepo, ReviewRepo>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddScoped<ExceptionHandlerMiddleware>(serviceProvider =>
{
  var logger = serviceProvider.GetRequiredService<ILogger<ExceptionHandlerMiddleware>>();
  return new ExceptionHandlerMiddleware(next =>
  {
    var requestDelegate = serviceProvider.GetRequiredService<RequestDelegate>();
    return Task.CompletedTask;
  }, logger);
}); // Catching database exception

// Register authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, OwnerAddressHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerReviewHandler>();

// add authentication instructions
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Secrets:JwtKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Secrets:Issuer"],
      };
    }
);

// Add authorization instructions
builder.Services.AddAuthorization(
    policy =>
    {
      policy.AddPolicy("SuperAdmin", policy => policy.RequireClaim(ClaimTypes.Email, "yuanke@admin.com"));
      policy.AddPolicy("AddressOwner", policy => policy.Requirements.Add(new OwnerAddressRequirement()));
      policy.AddPolicy("AdminOrOwnerReview", policy => policy.Requirements.Add(new AdminOrOwnerReviewRequirement()));
    }
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
  options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
  options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseCors(options => options.AllowAnyOrigin());
app.UseHttpsRedirection();
app.MapControllers();

app.Run();