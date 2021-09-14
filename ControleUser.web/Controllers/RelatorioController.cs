using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class RelatorioController : Controller
    {
        // GET: Relatorio
        [Authorize]
        public ActionResult ListaUsuario()
        {
            return View();
        }
        [Authorize]
        public ActionResult ListaGrupoUsuario()
        {
            return View();
        }
    }
}