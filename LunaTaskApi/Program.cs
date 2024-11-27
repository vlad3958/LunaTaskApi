using LunaTaskApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Data Source=database-1.chau4g8kk1z7.us-east-1.rds.amazonaws.com;Initial Catalog=database-1.chau4g8kk1z7.us-east-1.rds.amazonaws.com;User ID=admin;Password=11111111;Trust Server Certificate=True"));

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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