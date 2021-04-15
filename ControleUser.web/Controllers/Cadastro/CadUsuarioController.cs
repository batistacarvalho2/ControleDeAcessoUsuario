using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class CadUsuarioController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 5;
        private const string _senhaPadrao = "{$127,$188}";

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ListaPerfil = PerfilModel.RecuperarListaAtivos();
            ViewBag.SenhaPadrao = _senhaPadrao;
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = UsuarioModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = GrupoProdutoModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UsuarioPagina(int pagina)
        {
            var lista = UsuarioModel.RecuperarLista(pagina, _quantMaxLinhasPorPagina);
            return Json(lista);
        }

        [HttpPost]
        public ActionResult RecuperarUsuario(int id)
        {
            return Json(UsuarioModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [Authorize]
        public ActionResult ExcluirUsuario(int id)
        {
            return Json(UsuarioModel.ExcluirPeloId(id));
        }

        [HttpPost]
        public ActionResult SalvarUsuario(UsuarioModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {
                    if (model.Senha == _senhaPadrao)
                    {
                        model.Senha = "";

                    }
                    if (model.Salvar(model))
                    {
                        // ok
                    }
                    else
                    {
                        resultado = "ERRO";
                    }

                }
                catch (Exception)
                {
                    throw;
                }
            }
            //objeto anonimo
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }

    }
}