using CrayonCCP.Infrastructure.DbContexts;
using CrayonCCP.Infrastructure.Extensions;

namespace CrayonCCP;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.ConfigureDatabase(builder.Configuration, builder.Environment);
        
        builder.Services.RegisterRepositories();
        builder.Services.RegisterServices();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        await app.MigrateDatabaseAsync();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.RegisterMiddlewares();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}