﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyEzBank.Repositories.Entities;
using MoneyEzBank.Repositories.Repositories.Implements;
using MoneyEzBank.Repositories.Repositories.Interfaces;
using MoneyEzBank.Repositories.UnitOfWork;
using MoneyEzBank.Services.Mappers;
using MoneyEzBank.Services.Services.Implements;
using MoneyEzBank.Services.Services.Interfaces;
using System.Text;

namespace MoneyEzBank.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            // config swagger

            #region config swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoneyEzBank API", Version = "v.1.0" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token!",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            #endregion

            // config authentication

            #region config authen

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion

            // config CORS

            #region config CORS

            services.AddCors(options =>
            {
                options.AddPolicy("app-cors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithExposedHeaders("X-Pagination")
                        .AllowAnyMethod();
                    });
            });

            #endregion

            services.AddHttpClient("WebhookClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(
                    builder.Configuration.GetValue<int>("WebhookSettings:Timeout"));
            });

            return services;
        }

        public static IServiceCollection AddInfractstructure(this IServiceCollection services, IConfiguration config)
        {

            #region config services

            // config UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(MapperConfig).Assembly);

            // config claim service
            services.AddScoped<IClaimsService, ClaimsService>();

            // config user service
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<IWebhookConfigRepository, WebhookConfigRepository>();

            #endregion

            #region config database

            // config database

            services.AddDbContext<MoneyEzBankContext>(options =>
            {
                //options.UseSqlServer(config.GetConnectionString("MoneyEzBankLocal"));
                options.UseSqlServer(config.GetConnectionString("MoneyEzBankVps"));
            });

            #endregion

            return services;
        }
    }
}
