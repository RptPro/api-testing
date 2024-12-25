using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DataBaseWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowJekyllSite", policy =>
                {
                    policy.WithOrigins("https://rptpro.github.io")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Enable Windows Service (Optional, for Windows deployment only)
            builder.Host.UseWindowsService();

            var app = builder.Build();

            // Set dynamic port binding from environment variable or fallback to 5187
            var port = Environment.GetEnvironmentVariable("PORT") ?? "5187"; // Default to 5187 if PORT isn't set
            app.Urls.Add($"http://0.0.0.0:{port}");

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowJekyllSite"); // Apply CORS policy

            // HTTPS redirection (can be omitted or customized if needed)
            if (app.Environment.IsDevelopment()) // Disable in production or modify if required
            {
                app.UseHttpsRedirection();
            }
            app.MapGet("/", () => "Welcome to the API!");
            app.UseRouting();
            app.UseAuthorization();

            // Map controllers (ensure you have valid controllers)
            app.MapControllers();

            // If you need default routing for a controller action, add it here
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Download}/{action=GenerateMultiAndDownloadMdb}/{id?}");

            // Run the application
            app.Run();
        }
    }
}
