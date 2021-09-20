using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    [Authorize(Roles = "Administrador, Gerente")]
    public class CadUsuarioController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 10;
        private const string _senhaPadrao = "{$127,$188}";

        public ActionResult Index()
        {
            ViewBag.ListaPerfil = PerfilModel.RecuperarListaAtivos();
            ViewBag.ListaCargo = CargoModel.RecuperarListaCargo();
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
                    if (model.ValidaLogin(model))
                    {
                        if (model.ValidaEmail(model))
                        {
                            if (!model.Salvar(model))
                            {
                                // return Json(new { StatusCode = 400, Data = model, ErrorMessage = "Erro ao cadastrar/atualizar os dados do Usuário" });
                                return Json(new { StatusCode = 400, Data = model, ErrorMessage = "Erro ao cadastrar/atualizar os dados do Usuário" });

                            }
                        }
                    }
                    else
                    {
                        //return Json(new { StatusCode = 400, Data = model, ErrorMessage = "Usuário/Email já cadastrado!" });
                        resultado = "Usuário/Email já cadastrado!";
                       
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            // return Json(new { StatusCode = 200, Data = model });
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}