using TemplateMultTenaBackend.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.ConfigureRepositoryContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlmostHackers Template v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz (http://localhost:5000/)
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
