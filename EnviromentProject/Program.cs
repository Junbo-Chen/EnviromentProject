using EnviromentProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnviromentProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(connectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("No connection string found in appsettings.json");
            }

            builder.Services.AddSingleton<DbConnectionHelper>();
            builder.Services.AddSingleton<UserRepository>();
            builder.Services.AddSingleton<EnvironmentRepository>();
            builder.Services.AddSingleton<ObjectRepository>();


            var app = builder.Build();
            app.MapGet("/", () => $"The API is up . Connection string found: : {(sqlConnectionStringFound ? "✔" : "❌")}");


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
