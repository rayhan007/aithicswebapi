using aithics.api.Middleware;
using aithics.data;
using aithics.data.Data;
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
builder.Services.AddScoped<ISleepTrackerService, SleepTrackerService>();

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  // ✅ Allow HTTP for local testing
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = env.Jwt_Issuer,   // ✅ Replace with your actual issuer
            ValidAudience = env.Jwt_Audience,   // ✅ Replace with your actual audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(env.Jwt_Key))
        };

        // ✅ Enable Debug Logging for JWT
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ JWT Authentication Failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ JWT Token Validated Successfully");
                return Task.CompletedTask;
            }
        };
    });

// Register Authorization
builder.Services.AddAuthorization();

// Enable Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter("global", partition => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 15, // Max 5 requests per minute
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

// Register Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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
app.UseRouting();  // Enable routing

app.UseAuthentication();  // Apply authentication middleware
app.UseAuthorization();   // Apply authorization middleware

app.UseRateLimiter();  // Apply rate limiting

// Your custom middleware for API permission check
app.UseMiddleware<ApiPermissionMiddleware>();

// Map controllers
app.MapControllers();

app.Run();
