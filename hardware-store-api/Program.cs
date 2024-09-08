using hardware_store_api.Services.AddressService;
using hardware_store_api.Services.AuthService;
using hardware_store_api.Services.CityService;
using hardware_store_api.Services.CountryService;
using hardware_store_api.Services.ShopOrderService;
using hardware_store_api.Services.PaymentTypeService;
using hardware_store_api.Services.PaypalService;
using hardware_store_api.Services.ProductService;
using hardware_store_api.Services.ProductTypeService;
using hardware_store_api.Services.RegionService;
using hardware_store_api.Services.ShippingMethodService;
using hardware_store_api.Services.ShoppingCartService;
using hardware_store_api.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using hardware_store_api.Controllers.Hubs;
using hardware_store_api.Services.UserRoleService;
using Microsoft.AspNetCore.Diagnostics;
using hardware_store_api.Middleware;
using hardware_store_api.Services.OrderStatusService;
using hardware_store_api.Services.ShopOrderProductService;
using hardware_store_api.Services.StoreOrderService;
using hardware_store_api.Services.StoreOrderProductService;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
// var config = builder.Configuration;

DotNetEnv.Env.Load();

var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();

/*
var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};*/

builder.Services.AddTransient<ErrorHandlingMiddleware>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaypalService, PaypalService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IPaymentTypeService, PaymentTypeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IShippingMethodService, ShippingMethodService>();
builder.Services.AddScoped<IShopOrderService, ShopOrderService>();
builder.Services.AddScoped<IShopOrderProductService, ShopOrderProductService>();
builder.Services.AddScoped<IStoreOrderService, StoreOrderService>();
builder.Services.AddScoped<IStoreOrderProductService, StoreOrderProductService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();

builder.Services.AddCors(options => 
{ 
    options.AddPolicy("NewPolitics", app => {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }); 
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("NewPolitics");

app.UseAuthentication();

app.UseAuthorization();

//app.UseWebSockets(webSocketOptions);

app.MapControllers();

app.MapHub<MessageHub>("/test_signal");

app.Run();
