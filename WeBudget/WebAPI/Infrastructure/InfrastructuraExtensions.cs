using Domain.Users.Interfaces;
using FluentValidation;
using Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using System.Reflection;
using System.Text;
using WebAPI.Application.Behaviors;
using WebAPI.Domain.Users;
using WebAPI.Infrastructure.Auth;
using WebAPI.Infrastructure.Common;

namespace WebAPI.Infrastructure
{
    public static class InfrastructuraExtensions
    {
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext(typeof(InfrastructuraExtensions));
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
        {
            services.AddLogger(host,configuration);

            Log.Information("Adding infrastructure services");

            services
                .AddDatabase(configuration)
                .AddIdentityServices(configuration)
                .AddInteractionService();


            services.AddScoped<ITokenGenerator, TokenGenerator>();            

            return services;
        }

        public static IServiceCollection AddLogger(this IServiceCollection services, IHostBuilder host, IConfiguration configuration)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();


            host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("PostgreSql");

            if (connectionString is null)
            {
                Log.Error("Connection string is missing");
                throw new ArgumentNullException("Connection string is missing");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

            //make ping to database to check connection
            using (var context = services.BuildServiceProvider().GetService<ApplicationDbContext>())
            {
                context.Database.OpenConnection();
                context.Database.CloseConnection();
            }

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {


            AuthenticationSettings authSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>() ?? new AuthenticationSettings();
            services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));

            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddDefaultIdentity<User>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;


                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                var key = authSettings.JWT.Key;
                var audience = authSettings.JWT.Audience;
                var issuer = authSettings.JWT.Issuer;

                if (key is null || audience is null || issuer is null)
                {
                    Log.Error("JWT configuration is missing");
                    throw new ArgumentNullException("JWT configuration is missing");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Basic", policy =>
                {
                    policy.AuthenticationSchemes.Add(IdentityConstants.BearerScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(["User", "Admin"]);
                });
                options.AddPolicy("Admin", policy =>
                {
                    policy.AuthenticationSchemes.Add(IdentityConstants.BearerScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(["Admin"]);
                });
            });

            return services;
        }

        public static IServiceCollection AddInteractionService(this IServiceCollection services)
        {

            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(assembly);
            });
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

            services.AddValidatorsFromAssembly(assembly,includeInternalTypes:true);


            return services;
        }
    }
}
