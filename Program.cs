using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using robot_controller_api;
using robot_controller_api.Authentication;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Niko Borrowdale",
            Email = "s2232415244@deakin.edu.au"
        },

    });
});
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", default);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "admin", "ADMIN")); // Add case-insensitive check for "Admin"
    options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "admin", "ADMIN", "User", "user", "USER")); // Add case-insensitive check for "User" and "Admin"
});

// Dependency management
//builder.Services.AddScoped<IPasswordHasher<UserModel>, BCryptPasswordHasher<UserModel>>();
builder.Services.AddScoped<IPasswordHasher<UserModel>, Argon2PasswordHasher<UserModel>>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-outline.css"));
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
