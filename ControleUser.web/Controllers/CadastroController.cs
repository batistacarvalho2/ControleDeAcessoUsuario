﻿using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    public class CadastroController : Controller
    {
        #region Cadastro Usuário

        private const string _senhaPadrao = "{$127,$188}";

        [Authorize]
        public ActionResult Usuario()
        {        
            ViewBag.SenhaPadrao = _senhaPadrao;
            return View(UsuarioModel.RecuperarLista());
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
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

        #endregion

        #region Cadastro de Produtos
        [Authorize]
        public ActionResult GrupoProduto()
        {
            var lista = GrupoProdutoModel.RecuperarLista();
            ViewBag.QuantMaxLinhasPorPagina = 5;
            ViewBag.PaginaAtual = 1;
            
            var difQuantPaginas = (lista.Count % ViewBag.QuantMaxLinhasPorPagina) > 0? 1 : 0;
            ViewBag.QuantPaginas = (lista.Count / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;   


                return View(lista);
        }

        [HttpPost]
        [Authorize]
        public ActionResult RecuperarGrupoProduto(int id)
        {
            return Json(GrupoProdutoModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [Authorize]
        public ActionResult ExcluirGrupoProduto(int id)
        {
            return Json(GrupoProdutoModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [Authorize]
        public ActionResult SalvarGrupoProduto(GrupoProdutoModel model)
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
        public ActionResult UnidadeMedida()
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

        [Authorize]
        public ActionResult PerfilUsuario()
        {
            return View();
        }

        #endregion
    }
}

