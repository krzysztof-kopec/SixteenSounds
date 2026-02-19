using Microsoft.EntityFrameworkCore;
using SixteenSounds.Data;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Rejestracja us³ug
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Po³¹czenie z baz¹
    builder.Services.AddDbContext<SixteenSoundsDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var app = builder.Build();

    // W³¹czamy Swaggera na sta³e, ¿eby ³atwiej by³o testowaæ
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SixteenSounds API V1");
        c.RoutePrefix = "swagger"; // Swagger bêdzie pod adresem /swagger
    });

    app.UseAuthorization();
    app.MapControllers();

    Console.WriteLine(">>> SERWER STARTUJE...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("!!! B£¥D KRYTYCZNY STARTU:");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}