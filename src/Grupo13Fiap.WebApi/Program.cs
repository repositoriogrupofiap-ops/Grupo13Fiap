using Grupo13Fiap.Api.Extensions;
using Grupo13Fiap.Infrastructure.Extensions;
using Grupo13Fiap.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors();
builder.Services.AddApiProblemDetails();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddAPIDocumentation();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o=>     {
        o.SwaggerEndpoint("/openapi/v1.json", "Grupo13Fiap API");
    });
    await app.Services.SeedDatabaseAsync();
}

app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder
    .SetIsOriginAllowed(orign => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
app.MapControllers();

app.Run();


