using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStore.Persistence;
using MusicStore.Repositories.Abstractions;
using MusicStore.Repositories.Implementations;
using MusicStore.Services.Abstractions;
using MusicStore.Services.Implementations;
using MusicStore.Services.Profiles;

var builder = WebApplication.CreateBuilder(args);

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

//Configuring identity security policies
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
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

app.MapControllers();

app.Run();
