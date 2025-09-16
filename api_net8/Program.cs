global using api.Models;
global using api.Data;
using api.Services.AuthServices;
using api.Services.GeneralAPIServices;
using api.Services.TVFServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using Minio;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using api.UserSync;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json")
                            .Build();
//builder.Services.AddAutoMapper(typeof(GenericMappingProfile)); // Register AutoMapper
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<dbAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbAcountConnection"), o => o.UseCompatibilityLevel(120)));
builder.Services.AddScoped<SqlConnection>(provider =>
{
    var connection = new SqlConnection(builder.Configuration.GetConnectionString("dbAcountConnection"));
    connection.Open();
    return connection;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = """Standard Authorization header using the Bearer scheme. Example: "bearer {token}" """,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"

            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        }
);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(120)));
});

// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(configuration.GetConnectionString("MinioEndpoint"))
            .WithCredentials(configuration.GetConnectionString("MinioAccessKey"), configuration.GetConnectionString("MinioSecretKey"))
            .WithSSL(false).Build());

builder.Services.AddScoped<ITVFServices, TVFServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped(typeof(IAPIService<>), typeof(APIService<>));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
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
builder.Services.AddHttpClient();


builder.Services.AddHttpClient<UserUpdateController>();
builder.Services.AddScoped<UserUpdateController>();
builder.Services.AddHostedService<UserSyncBackgroundService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowCollabora");
app.UseRouting();

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseAuthorization();
app.MapControllers();

app.Run();

