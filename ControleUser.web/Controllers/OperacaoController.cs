using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class OperacaoController : Controller
    {
        // GET: Operacao
        [Authorize]
        public ActionResult EntradaEstoque()
        {
            return View();
        }
        [Authorize]
        public ActionResult SaidaEstoque()
        {
            return View();
        }
        [Authorize]
        public ActionResult LanPerdaProduto()
        {
            return View();
        }
        [Authorize]
        public ActionResult Inventario()
        {
            return View();
        }
    }
}