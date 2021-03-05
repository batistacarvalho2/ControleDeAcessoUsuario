using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ControleUser.web.Models
{
    public class LoginViewModel
    {
        [Display(Name="Usuário:")]
        public string Usuario { get; set; }

        [Display(Name = "Senha:")]
        public string Senha{ get; set; }

        [Display(Name = "Lembrar Me")]
        public bool LembrarMe { get; set; }

        
    }
}