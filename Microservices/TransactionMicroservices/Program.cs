using Microsoft.EntityFrameworkCore;
using TransactionMicroservices.Clients;
using TransactionMicroservices.IServiceContracts;
using TransactionMicroservices.Services;
using UserMicroservices.Data;
var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
