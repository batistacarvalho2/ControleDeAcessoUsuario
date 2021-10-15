using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class CadPerfilController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.ListaUsuario = UsuarioModel.RecuperarListaUsuario();
            var lista = PerfilModel.RecuperarLista();
            return View(lista);
        }

        [HttpPost]
        public JsonResult RecuperarPerfil(int id)
        {
            return Json(PerfilModel.RecuperarPeloId(id));
        }

        [HttpPost]
        public JsonResult ExcluirPerfil(int id)
        {
            return Json(PerfilModel.ExcluirPeloId(id));
        }

        [HttpPost]
        public JsonResult SalvarPerfil(PerfilModel model)
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
                catch (Exception e)
                {
                    resultado = "ERRO";
                }
            }
            //objeto anonimo
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }

    }
}