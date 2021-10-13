using ControleUser.web.Models;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class RelatorioGrupoController : Controller
    {
        [Authorize]
        public ActionResult ListaGrupo()
        {
            var lista = RelatorioGrupoModel.RecuperarLista();
            //return new ViewAsPdf("~/Views/RelatorioUsuario/Index.cshtml",lista);
            return View(lista);
        }
    }
}