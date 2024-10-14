using ICorteApi.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var apiHttpPort = Environment.GetEnvironmentVariable("API_HTTP_PORT");
ArgumentNullException.ThrowIfNullOrEmpty(apiHttpPort, nameof(apiHttpPort));

// Em ambientes de produção, como o Railway, geralmente o proxy (ou plataforma)
// cuida do SSL/HTTPS externamente, e sua aplicação deve escutar apenas na porta 
// TTP (sem configurar o HTTPS manualmente). GPT
// Porém mantenha `app.UseHttpsRedirection()` ativo. GPT
builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(int.Parse(apiHttpPort)));

// builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("AppDb"));

var pgHost = Environment.GetEnvironmentVariable("PG_HOST");
var pgPort = Environment.GetEnvironmentVariable("PG_PORT");
var pgDb = Environment.GetEnvironmentVariable("PG_DATABASE");
var pgUser = Environment.GetEnvironmentVariable("PG_USER");
var pgPass = Environment.GetEnvironmentVariable("PG_PASSWORD");

var connectionString = $"Host={pgHost};Port={pgPort};Database={pgDb};Username={pgUser};Password={pgPass};";

// var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
ArgumentNullException.ThrowIfNullOrEmpty(pgHost, nameof(pgHost));
ArgumentNullException.ThrowIfNullOrEmpty(pgPort, nameof(pgPort));
ArgumentNullException.ThrowIfNullOrEmpty(pgDb, nameof(pgDb));
ArgumentNullException.ThrowIfNullOrEmpty(pgUser, nameof(pgUser));
ArgumentNullException.ThrowIfNullOrEmpty(pgPass, nameof(pgPass));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql( // PostgreSQL
    // options.UseSqlServer( // SLQ Server
    // options.UseSqlite( // SQLite
        // builder.Configuration.GetConnectionString(connectionString),
        connectionString,
        assembly => assembly.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
);

builder.Services.AddHttpContextAccessor();

// Most important applications services
// This order was suggested by Chat GPT
builder.Services
    .AddIdentityConfigurations()
    .AddRepositories()
    .AddServices()
    .AddErrors()
    .AddValidators()
    .AddAuthorizationRules()
    .AddCookieConfiguration()
    // .AddAntiCsrfConfiguration()
    .AddExceptionHandlers()
;

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BarberShop API", Version = "v1" });
    c.ResolveConflictingActions(x => x.First());
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.Environment.WebRootPath = app.Configuration.GetValue<string?>("ImagesPath") ?? "nothing";
}

app.DefineCultureLocalization();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    /* DESCOMENTE ISSO APENAS EM CASO DE NECESSIDADE E DEPOIS COMENTE NOVAMENTE PELO AMOR DE DEUS */
    // await DataSeeder.ClearAllRowsBeforeSeedAsync(serviceProvider);
    /* DESCOMENTE ISSO APENAS EM CASO DE NECESSIDADE E DEPOIS COMENTE NOVAMENTE PELO AMOR DE DEUS */

    await MigrationApplier.ApplyMigration(serviceProvider);

    await RoleSeeder.SeedRoles(serviceProvider);
    await DataSeeder.SeedData(serviceProvider);
}

app.UseRouting();

// This call must be between `UseRouting` and `UseEndpoints`.
// app.UseAntiforgery();

if (!app.Environment.IsDevelopment())
{
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("PRODUCTIOOOOOOOOOOOOON!");
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    app.UseHttpsRedirection();
}

// After .NET 8 it isn't necessary to use `AddAuthentication` or `UseAuthentication`
// when `AddAuthorization` or `UseAuthorization` is also present.
app.UseAuthentication();
app.UseAuthorization();

// Configuring all application endpoints.
app.ConfigureMyEndpoints();

app.UseExceptionHandler("/error");

app.UseStaticFiles();

// Regenera o token de sessão na inicialização
SessionTokenManager.RegenerateToken();

app.Run();
