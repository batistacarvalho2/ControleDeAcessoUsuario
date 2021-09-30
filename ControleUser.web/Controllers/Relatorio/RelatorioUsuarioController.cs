using ControleUser.web.Models;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class RelatorioUsuarioController : Controller
    {

        private const int _quantMaxLinhasPorPagina = 10;
        private const string _senhaPadrao = "{$127,$188}";
        // GET: Relatorio
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ListaPerfil = PerfilModel.RecuperarListaAtivos();
            ViewBag.ListaCargo = CargoModel.RecuperarListaCargo();
            ViewBag.SenhaPadrao = _senhaPadrao;
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = RelatorioUsuarioModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = RelatorioUsuarioModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }
        public ActionResult FiltroRelatorio()
        {
            return View();
        }

        public ActionResult ListaGrupoUsuario()
        {
            return View();
        }
    }
}