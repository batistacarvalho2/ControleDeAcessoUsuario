﻿using Npgsql;
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
            //using (var conexao = new SqlConnection())
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = "server = localhost; user id = postgres; password = 123; database = postgres";
                conexao.Open();
              
                NpgsqlCommand comando = new NpgsqlCommand(string.Format("select count(*) from usuario where login='{0}' and senha='{1}'", login,senha), conexao);

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
