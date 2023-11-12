using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Winton.Extensions.Configuration.Consul;

const string appTitle = "Test Website";

const string version = "v1";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
           .AddConsul(
               "appsettings.json",
               options =>
               {
                   options.ConsulConfigurationOptions =
                       cco => { cco.Address = new Uri("http://consul:8500"); };
                   options.Optional = true;
                   options.PollWaitTime = TimeSpan.FromSeconds(5);
                   options.ReloadOnChange = true;
               })
           .AddEnvironmentVariables();

builder.Services.AddSwaggerGen(
        opt => { opt.SwaggerDoc(version, new OpenApiInfo { Title = appTitle, Version = version }); });

builder.Services.AddControllers();

var app = builder.Build();

app.UseDeveloperExceptionPage()
       .UseSwagger()
       .UseSwaggerUI(
           opt =>
           {
               opt.SwaggerEndpoint($"swagger/{version}/swagger.json", appTitle);
               opt.RoutePrefix = string.Empty;
           })
       .UseRouting()
       .UseEndpoints(opt => opt.MapControllers());

app.Run();