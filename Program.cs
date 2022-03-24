using System.Reflection;

using Reciplease;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<RecipleaseContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Add docstrings to Swagger docs.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication().AddGoogle(googleOptions => 
{
    string? clientId = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_ID");
    string? clientSecret = Environment.GetEnvironmentVariable("GOOGLE_OAUTH_CLIENT_SECRET");

    if (clientId == null){
        throw new ArgumentNullException(paramName: nameof(clientId), message: "Google OAuth Client Id cannot be null.");
    }
    if (clientSecret == null){
         throw new ArgumentNullException(paramName: nameof(clientSecret), message: "Google OAuth Client Secret cannot be null.");
    }

    googleOptions.ClientId = clientId;
    googleOptions.ClientSecret = clientSecret;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

void ConfigureGoogleOAuth()
{
    
}

ConfigureGoogleOAuth();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();