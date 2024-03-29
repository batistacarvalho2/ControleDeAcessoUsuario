﻿using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CadUsuarioController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 100;
        private const string _senhaPadrao = "{$123,$321}";

        [AllowAnonymous]
        public ActionResult Index()
        {
            var usuarioLogado = ObtenhaUsuarioPornome(HttpContext.User.Identity.Name);

            ViewBag.ListaPerfil = PerfilModel.RecuperarListaAtivos();
            ViewBag.ListaCargo = CargoModel.RecuperarListaCargo();
            ViewBag.SenhaPadrao = _senhaPadrao;

            var lista = UsuarioModel.RecuperarLista(usuarioLogado);

            return View(lista);
        }

        [HttpPost]
        [AllowAnonymous]
        private UsuarioModel ObtenhaUsuarioPornome(string name)
        {
            return UsuarioModel.RecuperarPeloNome(name);
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
                    if (model.Senha == "")
                    {
                        model.Senha = _senhaPadrao;
                    }
                    if (model.Id == 0)
                    {
                        if (!model.ValidaLogin(model))
                        {
                            ModelState.AddModelError("Login", "Login já cadastrado!");
                            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
                        }

                        if (!model.ValidaEmail(model))
                        {
                            return Json(new { StatusCode = 400, Data = model, ErrorMessage = "Usuário/Email já cadastrado!" });
                        }
                    }
                    if (!model.Salvar(model))
                    {
                        return Json(new { StatusCode = 400, Data = model, ErrorMessage = "Erro ao cadastrar/atualizar os dados do Usuário" });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}