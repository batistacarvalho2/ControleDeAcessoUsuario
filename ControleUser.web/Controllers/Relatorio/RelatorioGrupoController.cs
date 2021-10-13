using ControleUser.web.Models;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class RelatorioGrupoController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var lista = RelatorioGrupoModel.RecuperarLista();
            return View(lista);
        }
    }
}