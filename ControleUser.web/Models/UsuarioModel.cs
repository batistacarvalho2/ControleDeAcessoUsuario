using ControleUser.web.Helpers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
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
           
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                NpgsqlCommand comando = new NpgsqlCommand(string.Format("select count(*) from usuario where login='{0}' and senha='{1}'", login, CriptoHelper.HashMD5(senha)), conexao);

                try
                {
                    ret = ((long)comando.ExecuteScalar() > 0);
                 
                }
                catch (NpgsqlException e)
                {
                    Console.WriteLine("Erro " + e);
                }
                conexao.Close();

            }
            return ret;
        }
    }
}

