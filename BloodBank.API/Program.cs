using BloodBank.Application;
using BloodBank.Core;
using BloodBank.Infrastructure;
using BloodBank.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

await builder.Services
    .AddCore()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<BloodBankDbContextInitializer>();


builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<BloodBankDbContextInitializer>();
    
await initializer.InitializeAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
