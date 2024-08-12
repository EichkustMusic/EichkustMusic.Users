using static EichkustMusic.Users.Infrastructure.Persistance.PersistanceDependencyInjection;
using static EichkustMusic.Users.Infrastructure.Identity.IdentityDependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPersistace(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseIdentityServer();

app.Run();