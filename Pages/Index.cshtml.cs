using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SistemaBI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<VentaPorSucursal> VentasPorSucursal { get; set; }
        public List<VentaPorCategoria> VentasPorCategoria { get; set; }
        public Dictionary<string, int> ResumenVentasPorSucursal { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            VentasPorSucursal = new List<VentaPorSucursal>();
            VentasPorCategoria = new List<VentaPorCategoria>();
            ResumenVentasPorSucursal = new Dictionary<string, int>();
        }

        public async Task OnGetAsync()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5138");
            var client = new InventarioService.InventarioServiceClient(channel);
            try
            {
                var mensajeReply = await client.RecibirMensajeInventarioAsync(new SolicitudTextoPlano { });
                var datosJson = mensajeReply.Respuesta;

                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(datosJson);
                if (jsonObject.ContainsKey("ventasPorSucursal"))
                {
                    VentasPorSucursal = JsonConvert.DeserializeObject<List<VentaPorSucursal>>(jsonObject["ventasPorSucursal"].ToString()) ?? new List<VentaPorSucursal>();
                }
                if (jsonObject.ContainsKey("ventasPorCategoria"))
                {
                    VentasPorCategoria = JsonConvert.DeserializeObject<List<VentaPorCategoria>>(jsonObject["ventasPorCategoria"].ToString()) ?? new List<VentaPorCategoria>();
                }

                foreach (var venta in VentasPorSucursal)
                {
                    if (ResumenVentasPorSucursal.ContainsKey(venta.Sucursal))
                    {
                        ResumenVentasPorSucursal[venta.Sucursal] += venta.Cantidad;
                    }
                    else
                    {
                        ResumenVentasPorSucursal[venta.Sucursal] = venta.Cantidad;
                    }
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
