using API_CAPITAL_MANAGEMENT.Constants;
using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Domain_Services;
using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Repositories;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//1 Db Connection
var dbStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MyAppDbContext>(options => options.UseSqlServer(dbStr));

// 2 Cache
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024 * 4;
    options.UseCaseSensitivePaths = true;
});

//3 Repositories
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IOrganizationRepo, OrganizationRepo>();
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
builder.Services.AddScoped<IMovementRepo, MovementRepo>();

//4 Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

//5 AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//6 Token
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("SecretKey no esta configurada");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
// 7 Swagger with JWT and Documentation XML
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
    var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaArchivo = Path.Combine(AppContext.BaseDirectory, archivoXML);
    options.IncludeXmlComments(rutaArchivo);
});

// 8 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
        builder =>
        {
            builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
