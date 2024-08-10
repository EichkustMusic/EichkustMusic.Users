using static EichkustMusic.Users.Infrastructure.Persistance.PersistanceDependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPersistace(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();