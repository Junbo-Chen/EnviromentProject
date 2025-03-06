using EnviromentProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnviromentProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var connectionString = builder.Configuration.GetValue<string>("DefaultConnection");
            //var connectionString = builder.Configuration["DefaultConnection"];
            var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(connectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("No connection string found in appsettings.json");
            }

            builder.Services.AddTransient<EnvironmentRepository, EnvironmentRepository>(o => new EnvironmentRepository(connectionString));
            builder.Services.AddTransient<ObjectRepository, ObjectRepository>(o=> new ObjectRepository(connectionString));
            builder.Services.AddAuthorization();

            builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
            })
            .AddRoles<IdentityRole>()
            .AddDapperStores(options =>
            {
                options.ConnectionString = connectionString;
            });


            var app = builder.Build();
            app.UseAuthentication();


            app.MapGet("/", () => $"The API is up . Connection string found: : {(sqlConnectionStringFound ? "✔" : "❌")}");


            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapGroup("/account").MapIdentityApi<IdentityUser>();
            app.MapControllers().RequireAuthorization();


            app.Run();
        }
    }
}
