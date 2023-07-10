using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Inventario;
using Newtonsoft.Json;

namespace SistemaBI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<VentaPorSucursal> VentasPorSucursal { get; set; }
        public List<VentaPorCategoria> VentasPorCategoria { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            VentasPorSucursal = new List<VentaPorSucursal>();
            VentasPorCategoria = new List<VentaPorCategoria>();
        }

        public async Task OnGetAsync()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5138");
            var client = new InventarioService.InventarioServiceClient(channel);

            try
            {
                var mensajeReply = await client.RecibirMensajeInventarioAsync(new SolicitudTextoPlano { });
                var datosJson = mensajeReply.Respuesta; // Suponiendo que el JSON se encuentra en la propiedad "Respuesta" del mensaje
                Console.Write(datosJson);

                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(datosJson);
                if (jsonObject.ContainsKey("ventasPorSucursal"))
                {
                    VentasPorSucursal = JsonConvert.DeserializeObject<List<VentaPorSucursal>>(jsonObject["ventasPorSucursal"].ToString());
                }
                if (jsonObject.ContainsKey("ventasPorCategoria"))
                {
                    VentasPorCategoria = JsonConvert.DeserializeObject<List<VentaPorCategoria>>(jsonObject["ventasPorCategoria"].ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ha ocurrido un error: " + ex.Message);
            }
        }
    }

    public class VentaPorSucursal
    {
        public string Sucursal { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
    }

    public class VentaPorCategoria
    {
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
    }
}
