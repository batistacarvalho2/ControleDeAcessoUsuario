using ControleUser.web.Helpers;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleUser.web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

       // [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }
       // [Required(ErrorMessage = "Informe o senha")]
        public string Senha { get; set; }
      //  [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;
           
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                NpgsqlCommand comando = new NpgsqlCommand(string.Format("select count(*) from usuario where login=@login and senha=@senha", login, CriptoHelper.HashMD5(senha)), conexao);

                comando.Parameters.Add("@login",NpgsqlDbType.Varchar).Value = login;
                comando.Parameters.Add("@senha",NpgsqlDbType.Varchar).Value = CriptoHelper.HashMD5(senha);

                try
                {
                    //ret = ((long)comando.ExecuteScalar() > 0);

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Senha = (string)reader["senha"],
                            Nome = (string)reader["nome"]
                        };
                    }
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

