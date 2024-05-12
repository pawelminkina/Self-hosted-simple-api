using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using SimpleAPI.Extensions;
using SimpleAPI.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

Log.Logger = CreateSerilogLogger(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetValue<string>("DatabaseConnectionString"),
            npgsqlOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
            });
    },
    ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope
);

builder.Logging.AddSerilog(Log.Logger);
var app = builder.Build();

app.MigrateDbContext<ApplicationDbContext>();
// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();


Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogStashUrl"];
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", "SimpleAPI")
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(requestUri: (string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl), queueLimitBytes: null)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}