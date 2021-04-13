using ControleUser.web.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleUser.web.Controllers
{
    public class ContaController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            //var achou = (login.Usuario == "joao" && login.Senha == "123");
            var usuario = (UsuarioModel.ValidarUsuario(login.Usuario, login.Senha));

            if (usuario != null)
            {
                // FormsAuthentication.SetAuthCookie(usuario.Nome, login.LembrarMe);
                var tiket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, PerfilModel.RecuperarPeloId(usuario.Id).Nome));
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, tiket);
                Response.Cookies.Add(cookie);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login invalido.");
            }


            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}