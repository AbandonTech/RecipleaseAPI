using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Reciplease;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RecipleaseContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<RecipleaseContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var validAudience = Environment.GetEnvironmentVariable("ValidAudience");
    var validIssuer = Environment.GetEnvironmentVariable("ValidIssuer");
    var jwtSecretKey = Environment.GetEnvironmentVariable("Secret");

    if (validAudience == null)
        throw new ArgumentNullException(nameof(validAudience), "ValidAudience cannot be null.");
    
    if (validIssuer == null)
        throw new ArgumentNullException(nameof(validIssuer), "ValidIssuer cannot be null.");
    
    if (jwtSecretKey == null)
        throw new ArgumentNullException(nameof(jwtSecretKey), "Secret cannot be null.");
    
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = validAudience,
        ValidIssuer = validIssuer,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
})
.AddGoogle(googleOptions => 
{
    var clientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");

    if (clientId == null)
        throw new ArgumentNullException(nameof(clientId), "Google OAuth Client Id cannot be null.");
    if (clientSecret == null) 
        throw new ArgumentNullException(nameof(clientSecret), "Google OAuth Client Secret cannot be null.");
    
    googleOptions.ClientId = clientId;
    googleOptions.ClientSecret = clientSecret;
});

builder.Services.AddSwaggerGen(options => 
{
    // Add docstrings to Swagger docs.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",  
        Type = SecuritySchemeType.ApiKey,  
        Scheme = "Bearer",  
        BearerFormat = "JWT",  
        In = ParameterLocation.Header,  
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();