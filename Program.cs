using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ai_content_copilot.Models;
using ai_content_copilot.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<AiCopilotSettings>(builder.Configuration.GetSection("AiCopilot"));

builder.Services.AddHttpClient("OllamaClient", (serviceProvider, client) =>
{
    var settings = serviceProvider
        .GetRequiredService<IOptions<AiCopilotSettings>>()
        .Value;
    
    client.BaseAddress = new Uri(settings.OllamaBaseUrl);
});

builder.Services.AddScoped<OllamaService>();
builder.Services.AddScoped<IAiService>(serviceProvider => serviceProvider.GetRequiredService<OllamaService>());


builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();


await app.BootUmbracoAsync();


app.MapControllers();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice(); 
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });



await app.RunAsync();
