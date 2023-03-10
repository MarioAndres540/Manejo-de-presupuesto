using Manejo_Presupuesto.Models;
using Manejo_Presupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manejo_Presupuesto.Controllers
{
    public class CuentasController: Controller
    {
        private readonly IRepositoriosTiposCuentas repositoriosTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public CuentasController(IRepositoriosTiposCuentas repositoriosTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositoriosTiposCuentas = repositoriosTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]

        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositoriosTiposCuentas.Obtener(usuarioId);
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
            return View(modelo);
        }
    }
}
