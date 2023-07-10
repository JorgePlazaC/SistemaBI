using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Inventario; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Start the task for receiving messages in the background.
Task.Run(async () =>
{
    // The port number must match the port of the gRPC server.
    using var channel = GrpcChannel.ForAddress("http://localhost:5138");
    var client = new InventarioService.InventarioServiceClient(channel); 

    while (true) // Bucle infinito
    {
        try
        {
            // Llamada a RecibirMensajeInventario
            var mensajeReply = await client.RecibirMensajeInventarioAsync(
                new SolicitudTextoPlano { });
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
