﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using FluentValidation;
using ProjectAPI.DataAccess;
using ProjectAPI.Mapping;
using ProjectAPI.Primitives;
using ProjectAPI.ModelValidation;
using ProjectAPI.BusinessLogic.Extensions;

namespace ProjectAPI
{
    /// <summary>
    /// This class handles the request pipeline and is called right after <see cref="Program"/> is executed.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructs an instance of <see cref="Startup"/> using the specified configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method is used to add services to the container.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCatalogContext(Configuration);

            services.DisableAutomaticValidation();

            services.AddLoggingToFile();
            
            services.AddAutoMapper(typeof(AllMappersProfile));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AuthOptions.ISSUER,
                            ValidateAudience = true,
                            ValidAudience = AuthOptions.AUDIENCE,
                            ValidateLifetime = true,
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                            ValidateIssuerSigningKey = true,
                        };
                    });

            services.AddMediatR(AppDomain.CurrentDomain.Load("ProjectAPI.BusinessLogic"));

            services.AddScoped<IValidator<ProductModel>, ProductModelValidator>();
            services.AddScoped<IValidator<CategoryModel>, CategoryModelValidator>();

            services.AddSwagger(Configuration);

            services.AddControllersWithViews();
        }

        /// <summary>
        ///  This method is used to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">An instance of <see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">An instance of <see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCustomSwagger(Configuration);
            app.UseCustomExceptionMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}