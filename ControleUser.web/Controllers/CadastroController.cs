using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class CadastroController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 5;

        #region Cadastro de Produtos

        [Authorize]
        public ActionResult GrupoProduto()
        {
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            var lista = GrupoProdutoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            var quant = GrupoProdutoModel.RecuperarQuantidade();

            var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;


            return View(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult GrupoProdutoPagina(int pagina)
        {
            var lista = GrupoProdutoModel.RecuperarLista(pagina, _quantMaxLinhasPorPagina);
            return Json(lista);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarGrupoProduto(int id)
        {
            return Json(GrupoProdutoModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [Authorize]
        public JsonResult ExcluirGrupoProduto(int id)
        {
            return Json(GrupoProdutoModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [Authorize]
        public JsonResult SalvarGrupoProduto(GrupoProdutoModel model)
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
                    if (model.Salvar() != 0)
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
                    resultado = "ERRO";
                }
            }
            //objeto anonimo
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }

        [Authorize]
        public ActionResult MarcaProduto()
        {
            return View();
        }

        [Authorize]
        public ActionResult LocalProduto()
        {
            return View();
        }

        [Authorize]
        public ActionResult Produto()
        {
            return View();
        }

        [Authorize]
        public ActionResult Pais()
        {
            return View();
        }

        [Authorize]
        public ActionResult Estado()
        {
            return View();
        }

        [Authorize]
        public ActionResult Cidade()
        {
            return View();
        }

        [Authorize]
        public ActionResult Fornecedor()
        {
            return View();
        }

        #endregion
    }
}
