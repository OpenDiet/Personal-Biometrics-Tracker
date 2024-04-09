using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalBiometricsTracker.Data;
using PersonalBiometricsTracker.Entities;
using PersonalBiometricsTracker.Services;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration.GetValue<string>("JWTSecret");

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<PersonalBiometricsTrackerDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("BiometricsDb")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWeightService, WeightService>();
builder.Services.AddScoped<IBloodGlucoseService, BloodGlucoseService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Don't use HTTPS redirection in development mode
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
