using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MusicStore.API.EndPoint;
using MusicStore.API.EndPoints;
using MusicStore.API.Filters;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Seeders;
using MusicStore.Repositories.Abstractions;
using MusicStore.Repositories.Implementations;
using MusicStore.Services.Abstractions;
using MusicStore.Services.Implementations;
using MusicStore.Services.Profiles;
using Serilog;
using Serilog.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "log.txt");
var logger = new LoggerConfiguration()
    .WriteTo.File(logPath, 
    rollingInterval: RollingInterval.Day,
    restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

try
{
    builder.Logging.AddSerilog(logger);
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENVI"}");
    //Options pattern register
    builder.Services.Configure<AppSettings>(builder.Configuration);

    //CORS
    var corsConfiguration = "MusicStoreCors";
    builder.Services.AddCors(setup =>
    {
        setup.AddPolicy(corsConfiguration, policy =>
        {
            policy.AllowAnyOrigin(); //Que cualquiera pueda consumir el API
            policy.AllowAnyHeader().WithExposedHeaders(new string[] { "TotalRecordsQuantity" });
            policy.AllowAnyMethod();
        });
    });
    // Add services to the container.

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(FilterExceptions));
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            var response = new BaseResponse
            {
                Success = false,
                ErrorMessage = string.Join(", ", errors) //Une los mensajes de error en un solo string
            };

            return new BadRequestObjectResult(response);
        };
    });

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    //builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //Configuring context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    //Registering Services
    builder.Services.AddScoped<IGenreRepository, GenreRepository>();
    builder.Services.AddScoped<IConcertRepository, ConcertRepository>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ISaleRepository, SaleRepository>();

    builder.Services.AddScoped<IConcertService, ConcertService>();
    builder.Services.AddScoped<IGenreService, GenreService>();
    builder.Services.AddScoped<ISaleService, SaleService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    #region Registering Healthchecks
    builder.Services.AddHealthChecks()
        .AddCheck("selfcheck", () => HealthCheckResult.Healthy())
        .AddDbContextCheck<ApplicationDbContext>();
    #endregion

    builder.Services.AddScoped<UserDataSeeder>();
    builder.Services.AddScoped<GenreSeeder>();

    //Configuring identity security policies
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"] ??
            throw new InvalidOperationException("JWT key not configured"));
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
    builder.Services.AddAuthorization();

    builder.Services.AddIdentity<MusicStoreUserIdentity, IdentityRole>(policies =>
        {
            policies.Password.RequireDigit = true;
            policies.Password.RequiredLength = 6;
            policies.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<ConcertProfile>();
        config.AddProfile<GenreProfile>();
        config.AddProfile<SaleProfile>();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(corsConfiguration);
    app.MapReports();
    app.MapHomeEndpoints();
    app.MapControllers();

    //Aplicando migraciones automaticas y seed data
    await ApplyMigrationsAndSeedDataAsync(app);
    //Configuring health checks
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "An unhandled exception occurred during the API initialization.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if(dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    var userDataSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await userDataSeeder.SeedAsync();
    var genreSeeder = scope.ServiceProvider.GetRequiredService<GenreSeeder>();
    await genreSeeder.SeedAsync();
}