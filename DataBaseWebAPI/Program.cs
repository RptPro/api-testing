using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
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

            // Enable Windows Service
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

            // HTTPS redirection (can be omitted if using HTTP for testing)
            if (app.Environment.IsDevelopment()) // Disable in production or modify if required
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
