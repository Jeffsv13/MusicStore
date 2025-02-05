using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Seeders;
using MusicStore.Repositories.Abstractions;
using MusicStore.Repositories.Implementations;
using MusicStore.Services.Abstractions;
using MusicStore.Services.Implementations;
using MusicStore.Services.Profiles;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();
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
app.MapControllers();

//Aplicando migraciones automaticas y seed data
await ApplyMigrationsAndSeedDataAsync(app);
app.Run();

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