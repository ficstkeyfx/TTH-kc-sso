global using QLNguoiDung.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using QLNguoiDung.Models;
using QLNguoiDung.Hubs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Minio;
using Minio.DataModel.Args;
using QLNguoiDung.Authentication;
using QLNguoiDung;
using QLNguoiDung.Services.AuthenServices;
using QLNguoiDung.Services.LinkService;
using Microsoft.Extensions.Caching.Memory;

using QLNguoiDung.Services.IApiServices;
using Microsoft.AspNetCore.Http.Features;
using QLNguoiDung.Services.ITVFServices;


var builder = WebApplication.CreateBuilder(args);
IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json")
                            .Build();

// Add services to the container.
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ApiServices>();
builder.Services.AddScoped<LinkService>();
builder.Services.AddScoped<DialogService>();

builder.Services.AddMemoryCache();

builder.Services.AddScoped<AuthenServices>();
builder.Services.AddScoped<IAuthenServices,AuthenServices>();
builder.Services.AddScoped<IApiServices, ApiServices>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<ITVFServices, TVFServices>();
builder.Services.AddScoped<TVFServices>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(configuration.GetConnectionString("MinioEndpoint"))
            .WithCredentials(configuration.GetConnectionString("MinioAccessKey"), configuration.GetConnectionString("MinioSecretKey"))
            .WithSSL(false).Build());

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddDbContext<dbQLNguoiDungContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DBConnection"));

}, contextLifetime: ServiceLifetime.Transient);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(configuration.GetConnectionString("API") ?? "http://apiQLNguoiDung.6pg.org/")
    });
builder.Services.AddRadzenComponents();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = null; 
});

builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = null; 
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCollabora", policy =>
    {
        policy.WithOrigins("http://203.128.246.222:9980") 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});




var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = 1000 * 1024 * 1024; 
    await next.Invoke();
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "frame-ancestors 'self' http://QLNguoiDung.6pg.org http://203.128.246.222:9980;");
    await next();
});


// app.MapPost("/set-cookie", (HttpContext context, Dictionary<string,string> cookies) =>
// {
//     foreach (var cookie in cookies)
//     {
//         context.Response.Cookies.Append(cookie.Key, cookie.Value, new CookieOptions
//         {
//             Expires = DateTimeOffset.UtcNow.AddMinutes(30),
//             HttpOnly = true,
//             Secure = true,
//             SameSite = SameSiteMode.Strict
//         });
//     }
//     return Results.Ok();
// });


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowCollabora");
app.UseRouting();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<ChatHubs>("/chat");
app.Run();
