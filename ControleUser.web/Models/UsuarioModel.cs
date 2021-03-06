using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleUser.web.Models
{
    public class UsuarioModel
    {
        public static bool ValidarUsuario(string login, string senha)
        {
            var ret = false;
            using (var conexao = new SqlConnection())
            {
                //conexao.ConnectionString = @" User ID=postgres;Password=123;Host=localhost;Port=5432;Database=mybase;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;";
                conexao.ConnectionString = "Server=localhost;Port=5432;User Id=postgres;Password=123;Database=mybase;";
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "select count(*) from usuario where login='{0}' and senha='{1}'", login, senha);
                   ret = ((int)comando.ExecuteScalar() > 0);
                }
            }
            return ret;
        }
    }
}

