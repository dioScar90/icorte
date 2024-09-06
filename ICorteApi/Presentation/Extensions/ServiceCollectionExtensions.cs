using FluentValidation;
using ICorteApi.Application.Interfaces;
using ICorteApi.Application.Services;
using ICorteApi.Application.Validators;
using ICorteApi.Domain.Entities;
using ICorteApi.Domain.Enums;
using ICorteApi.Domain.Errors;
using ICorteApi.Domain.Interfaces;
using ICorteApi.Infraestructure.Context;
using ICorteApi.Infraestructure.Interfaces;
using ICorteApi.Infraestructure.Repositories;
using ICorteApi.Presentation.Exceptions;
using ICorteApi.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace ICorteApi.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IBarberShopRepository, BarberShopRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<IRecurringScheduleRepository, RecurringScheduleRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ISpecialScheduleRepository, SpecialScheduleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IBarberShopService, BarberShopService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IRecurringScheduleService, RecurringScheduleService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ISpecialScheduleService, SpecialScheduleService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    public static IServiceCollection AddErrors(this IServiceCollection services)
    {
        services.AddScoped<IAddressErrors, AddressErrors>();
        services.AddScoped<IAppointmentErrors, AppointmentErrors>();
        services.AddScoped<IPaymentErrors, PaymentErrors>();
        services.AddScoped<IProfileErrors, ProfileErrors>();
        services.AddScoped<IBarberShopErrors, BarberShopErrors>();
        services.AddScoped<IImageErrors, ImageErrors>();
        services.AddScoped<IMessageErrors, MessageErrors>();
        services.AddScoped<IRecurringScheduleErrors, RecurringScheduleErrors>();
        services.AddScoped<IReportErrors, ReportErrors>();
        services.AddScoped<IServiceErrors, ServiceErrors>();
        services.AddScoped<ISpecialScheduleErrors, SpecialScheduleErrors>();
        services.AddScoped<IUserErrors, UserErrors>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AddressDtoCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<AddressDtoUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<AppointmentDtoCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<AppointmentDtoUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<PaymentDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<BarberShopDtoCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<BarberShopDtoUpdateValidator>();
        services.AddValidatorsFromAssemblyContaining<MessageDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RecurringScheduleDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ReportDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ServiceDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<SpecialScheduleDtoRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserDtoLoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserDtoRegisterRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserDtoChangeEmailRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserDtoChangePasswordRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UserDtoChangePhoneNumberRequestValidator>();

        return services;
    }

    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        // After .NET 8 we can use IExceptionHandler interface
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // // Se estiver usando .NET 8 ou superior
        // services.AddExceptionHandler<GlobalExceptionHandler>();

        // // Adiciona suporte para detalhes de problemas HTTP
        // services.AddProblemDetails(options =>
        // {
        //     options.CustomizeProblemDetails = (con)
        //     options.IncludeExceptionDetailInProblemDetails = (context, exception) =>
        //         context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        //     options.Map<UnauthorizedAccessException>(ex => new ProblemDetails
        //     {
        //         Status = StatusCodes.Status403Forbidden,
        //         Title = "Forbidden",
        //         Detail = ex.Message
        //     });

        //     options.Map<NotFoundException>(ex => new ProblemDetails
        //     {
        //         Status = StatusCodes.Status404NotFound,
        //         Title = "Not Found",
        //         Detail = ex.Message
        //     });

        //     // Adicione mapeamentos personalizados conforme necessário
        // });

        return services;
    }

    public static IServiceCollection AddAuthorizationRules(this IServiceCollection services)
    {
        // Configuração de autenticação e autorização
        // After .NET 8 it's not necessary to use `AddAuthentication` here.
        // The use of `AddAuthorization` can be converted to the new `AddAuthorizationBuilder`.
        // https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0025?view=aspnetcore-8.0
        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(PolicyUserRole.AdminOnly), policy =>
                policy.RequireRole(
                    nameof(UserRole.Admin)))
            .AddPolicy(nameof(PolicyUserRole.BarberShopOrHigh), policy =>
                policy.RequireRole(
                    nameof(UserRole.BarberShop), nameof(UserRole.Admin)))
            .AddPolicy(nameof(PolicyUserRole.ClientOnly), policy =>
                policy.RequireRole(
                    nameof(UserRole.Client), nameof(UserRole.Admin)))
            .AddPolicy(nameof(PolicyUserRole.ClientOrHigh), policy =>
                policy.RequireRole(
                    nameof(UserRole.Client), nameof(UserRole.BarberShop), nameof(UserRole.Admin)))
            .AddPolicy(nameof(PolicyUserRole.FreeIfAuthenticated), policy =>
                policy.RequireRole(
                    nameof(UserRole.Guest), nameof(UserRole.Client), nameof(UserRole.BarberShop), nameof(UserRole.Admin)));

        return services;
    }

    public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
    {
        services.AddIdentity<User, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                // Define o tempo de bloqueio da conta de um usuário após várias tentativas de login fracassadas.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; // Ajustado para exigir e-mails únicos

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()

            // This `AddDefaultUI` above is necessary to not display
            // `No Registered Service for IEmailSender` error message
            // after run the application by `dotnet run`
            .AddDefaultUI()

            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddCookieConfiguration(this IServiceCollection services)
    {
        // Configuração de autenticação por cookies
        services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnSigningIn = context =>
                {
                    var currentToken = SessionTokenManager.GetCurrentToken();
                    context.Properties.Items["SessionToken"] = currentToken;
                    return Task.CompletedTask;
                };

                options.Events.OnValidatePrincipal = context =>
                {
                    if (context.Properties.Items.TryGetValue("SessionToken", out var sessionToken))
                    {
                        if (sessionToken != SessionTokenManager.GetCurrentToken())
                        {
                            context.RejectPrincipal(); // Rejeitar o principal se o token não corresponder
                        }
                    }
                    return Task.CompletedTask;
                };

                options.Cookie.HttpOnly = true;

                options.LoginPath = "/auth/login";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                options.LogoutPath = "/auth/logout";

                options.AccessDeniedPath = "/auth/access-denied";
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };

                // Útil para prolongar a sessão ativa se o usuário estiver ativo.
                options.SlidingExpiration = true;

                // Garante que os cookies sejam enviados apenas em conexões HTTPS, o que é ótimo para segurança.
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                // Define o tempo de vida do cookie de autenticação, ou seja, quanto tempo o cookie permanece válido antes de expirar.
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            });

        return services;
    }

    public static IServiceCollection AddCustomDataProtection(this IServiceCollection services)
    {
        _ = services.AddDataProtection()
            .SetApplicationName("ICorteApi")
            .PersistKeysToFileSystem(new DirectoryInfo(@"./keys")) // Local onde as chaves são armazenadas
            .ProtectKeysWithDpapi(); // Proteção adicional para as chaves

        return services;
    }

    public static IServiceCollection AddAntiCsrfConfiguration(this IServiceCollection services)
    {
        // Configuração de proteção contra ataques de Cross-Site Request Forgery (CSRF).
        services.AddAntiforgery(options =>
        {
            /*
            Frontend:
                Ao enviar uma requisição que pode alterar dados (como uma requisição POST), o token CSRF
                deve ser incluído no cabeçalho da requisição com o nome "X-CSRF-TOKEN". Isso é geralmente
                feito em requisições AJAX, onde você pode configurar o cliente HTTP (por exemplo, Axios,
                Fetch API) para incluir o token no cabeçalho.
            Backend:
                No servidor, o ASP.NET Core verifica a presença do token no cabeçalho "X-CSRF-TOKEN" e
                valida seu valor. Se o token estiver ausente ou for inválido, a requisição é rejeitada,
                protegendo a aplicação contra ataques CSRF.
            */
            options.HeaderName = "X-CSRF-TOKEN";
        });

        return services;
    }
}
