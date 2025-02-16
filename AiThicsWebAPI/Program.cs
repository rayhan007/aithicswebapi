using aithics.api.Middleware;
using aithics.data;
using aithics.service.Interfaces;
using aithics.service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Database
builder.Services.AddDbContext<AithicsDbContext>(options =>
    options.UseSqlServer(DbSettings.ConnectionString));

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiPermissionService, ApiPermissionService>();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_Key")))

        };
    });

// Enable Rate Limiting (Prevents brute-force attacks)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter("global", partition => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5, // Max 5 requests per minute
            Window = TimeSpan.FromMinutes(1)
        }));
});

// CORS Protection
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("https://localhost:7124")
                        .WithMethods("GET", "POST", "PUT", "DELETE")
                        .WithHeaders("Authorization", "Content-Type"));
});

// Enable Swagger with JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AiThics API",
        Version = "v1",
        Description = "JWT Authenticated API with Role-Based Access Control"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer <your-token>'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

builder.Services.AddControllers(); // Fix: Register MVC Controllers
builder.Services.AddEndpointsApiExplorer();  // Required for minimal API exploration
builder.Services.AddSwaggerGen();  //Registers Swagger Generator
// Register Authentication & Authorization Services
builder.Services.AddAuthentication(); // If using authentication
builder.Services.AddAuthorization();  // Fix for missing authorization services

var app = builder.Build();
// Enable Swagger UI in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable Security Features
app.UseHsts();  // Enforce HTTPS
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

// Enable Middleware & Security Features
app.UseMiddleware<ApiPermissionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
