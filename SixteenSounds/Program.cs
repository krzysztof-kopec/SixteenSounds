using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Serwisy
builder.Services.AddControllers();
builder.Services.AddDbContext<SixteenSoundsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. KONFIGURACJA SWAGGERA (To naprawi b³¹d czerwony w terminalu)
builder.Services.AddEndpointsApiExplorer();
// To musi byæ tak rozbudowane, ¿eby Swagger "prze³kn¹³" IFormFile
builder.Services.AddSwaggerGen(); // Czasami wystarczy najprostsza wersja

var app = builder.Build();

// 3. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SixteenSounds API v1"));
}

// Pozwala serwerowi pokazywaæ pliki z folderu wwwroot (potrzebne do muzyki!)
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<SixteenSoundsDbContext>();
    context.Database.Migrate();
}

Console.WriteLine(">>> SERWER STARTUJE...");
app.Run();