using ICorteApi.Context;
using ICorteApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
// using ICorteApi.Routes;
using ICorteApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Text;
using ICorteApi.Routes;

namespace ICorteApi;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // var connectionString = Configuration.GetConnectionString("SqliteConnection");

        services.AddDbContext<ICorteContext>(options =>
        {
            // options.UseNpgsql(
            //     connectionString,
            //     assembly => assembly.MigrationsAssembly(typeof(ICorteContext).Assembly.FullName)
            // );
            // options.UseSqlite(
            //     connectionString,
            //     assembly => assembly.MigrationsAssembly(typeof(ICorteContext).Assembly.FullName)
            // );
            options.UseInMemoryDatabase("AppDb");
        });

        services.AddScoped<ICorteContext>();
        
        // services.AddIdentity<User, Role>(options =>
        // {
        //     // Password settings.
        //     options.Password.RequireDigit = true;
        //     options.Password.RequireLowercase = true;
        //     options.Password.RequireNonAlphanumeric = true;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequiredLength = 6;
        //     options.Password.RequiredUniqueChars = 1;

        //     // Lockout settings.
        //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //     options.Lockout.MaxFailedAccessAttempts = 5;
        //     options.Lockout.AllowedForNewUsers = true;

        //     // User settings.
        //     options.User.AllowedUserNameCharacters =
        //     "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        //     options.User.RequireUniqueEmail = false;
        // })
        // .AddEntityFrameworkStores<ICorteContext>()
        // .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // ValidateIssuerSigningKey = true,
                // IssuerSigningKey = new SymmetricSecurityKey(Settings.Settings.GetEncodingKey()),
                // ValidateIssuer = false,
                // ValidateAudience = false,
                
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!))
            };
        });

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddIdentityApiEndpoints<User>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        })
            .AddEntityFrameworkStores<ICorteContext>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BarberShop API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }});
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.DefineCultureLocalization("pt-BR");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            // endpoints.MapIdentityApi<User>();
            endpoints.MapUsersEndpoint();
            endpoints.MapAddressesEndpoint();
            // endpoints.MapUsersEndpoint();
            endpoints.MapGet("/", () => "Hello World!");
        });
    }
}