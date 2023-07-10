using System.Threading.Tasks;
using Grpc.Net.Client;
using Inventario; // Actualiza con el nombre correcto del espacio de nombres gRPC.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Start the task for receiving messages in the background.
Task.Run(async () =>
{
    // The port number must match the port of the gRPC server.
    using var channel = GrpcChannel.ForAddress("http://localhost:5138"); // Asegúrate de que este puerto sea el correcto.
    var client = new InventarioService.InventarioServiceClient(channel); // Actualizado para usar el cliente correcto.

    while (true) // Bucle infinito
    {
        try
        {
            // Llamada a EnviarMensajeWeb
            var mensajeReply = await client.EnviarMensajeWebAsync(
                new MensajeRequest { Mensaje = "Mensaje desde el sistema web" });
            Console.WriteLine("Respuesta del servidor: " + mensajeReply.Respuesta);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ha ocurrido un error: " + ex.Message);
        }

        await Task.Delay(5000); // Espera 5 segundos antes de la próxima solicitud
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
