using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TransactionMicroservices.Clients;
using TransactionMicroservices.IServiceContracts;
using TransactionMicroservices.Services;
using UserMicroservices.Data;
var builder = WebApplication.CreateBuilder(args);

// Read JWT config
var jwtSettings = builder.Configuration.GetSection("JWT");
var secret = jwtSettings["Secret"]; // make sure this key matches appsettings
var keyBytes = Encoding.UTF8.GetBytes(secret);

// Add Authentication (JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        // optional: reduce allowed clock skew
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**************************DB CONNECTION************************************/
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("ApiGateway", client =>
{

    client.BaseAddress = new Uri("https://localhost:7000");
});
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<GatewayClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
