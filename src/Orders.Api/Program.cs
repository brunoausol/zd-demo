using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orders.Application.DependencyInjection;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.DependencyInjection;
using Orders.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    if (options.MigrateOnStartup)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();
