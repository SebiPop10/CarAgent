using CarAgent_BE.Services; // vom crea ITokenService + InMemoryUserStore
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarAgent_BE.Options;

var builder = WebApplication.CreateBuilder(args);

// === JWT Config ===
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SigningKey"] ?? "DEV_ONLY_TEMP_KEY_CHANGE_ME"));

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// === Servicii provizorii ===
builder.Services.Configure<JwtOptions>(jwtSection);          // record cu Issuer, Audience, etc.
builder.Services.AddSingleton<ITokenService, TokenService>(); // folosește SigningKey + JwtOptions
builder.Services.AddSingleton<IUserStoreLite, InMemoryUserStore>(); // useri în memorie pentru test
builder.Services.AddSingleton<IAuthService, AuthService>();   // coordonează register/login

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
