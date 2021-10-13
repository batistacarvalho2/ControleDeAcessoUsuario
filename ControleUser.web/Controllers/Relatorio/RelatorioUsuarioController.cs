using ControleUser.web.Models;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class RelatorioUsuarioController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var lista = RelatorioUsuarioModel.RecuperarLista();
            //return new ViewAsPdf("~/Views/RelatorioUsuario/Index.cshtml",lista);
            return View(lista);
        }
    }
}