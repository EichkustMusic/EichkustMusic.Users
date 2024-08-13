using static EichkustMusic.Users.Infrastructure.Persistance.PersistanceDependencyInjection;
using static EichkustMusic.Users.Infrastructure.Identity.IdentityDependencyInjection;
using static EichkustMusic.S3.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddPersistace(builder.Configuration);

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddS3(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseIdentityServer();

app.Run();