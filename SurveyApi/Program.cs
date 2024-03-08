using Azure.Identity;
using Microsoft.OpenApi.Models;
using SurveyApi;
using SurveyApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "SurveyApi - V1",
            Version = "v1"
        }
    );

    var filePath = Path.Combine(AppContext.BaseDirectory, "SurveyApi.xml");
    c.IncludeXmlComments(filePath);
});

// Access Key Vault to fetch the app configuration resource's connection string
var keyVaultEndpoint = new Uri("https://kv-surveycreds.vault.azure.net/");
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
string connectionString = builder.Configuration["AppConfigConnectionString"]!;

// Authenticate into the App Configuration Resource and bind its Key Vault References
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
});

// Bind result of App Configuration to KeyVaultOptions
builder.Services.Configure<KeyVaultOptions>(builder.Configuration.GetSection("KeyVault"));

// Connection String set in context class
builder.Services.AddDbContext<DbSurveyProjectContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", typeof(Program).Assembly.GetName().Name);
        c.DisplayRequestDuration();
    });

    app.UseDeveloperExceptionPage();
}



app.UseCors(cors => cors
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
            );

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
