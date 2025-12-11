using ERPContable.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptions =>
{
        // 2. Aquí forzamos las opciones de conexión:
        // Desactivar explícitamente el cifrado y confiar en el certificado del servidor
        sqlServerOptions.EnableRetryOnFailure(); // Buena práctica

        // **ESTO ES LO CRUCIAL**
        // A veces el driver necesita ambos, TrustServerCertificate y Encrypt=False, explícitamente
        // Nota: Estas opciones deben ser especificadas *dentro* de la cadena de conexión, 
        // pero podemos intentar forzar el comportamiento si la cadena no funciona.

        // Dado que la cadena de conexión sigue fallando, vamos a probar una inyección de lógica:
        // Intentaremos agregar una opción más limpia en la cadena:
        // sqlServerOptions.UseConnectionRetry(5); // Versiones más recientes
}));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
