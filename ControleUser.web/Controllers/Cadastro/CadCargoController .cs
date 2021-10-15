using ControleUser.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleUser.web.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class CadCargoController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
            {
                var lista = CargoModel.RecuperarLista();
                return View(lista);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public JsonResult RecuperarCargo(int id)
            {
                return Json(CargoModel.RecuperarPeloId(id));
            }

            [HttpPost]
            public JsonResult ExcluirCargo(int id)
            {
                return Json(CargoModel.ExcluirPeloId(id));
            }

            [HttpPost]
            public JsonResult SalvarCargo(CargoModel model)
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

        }
}