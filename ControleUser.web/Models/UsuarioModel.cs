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

                NpgsqlCommand comando = new NpgsqlCommand(string.Format("select * from usuario where login=@login and senha=@senha", login, CriptoHelper.HashMD5(senha)), conexao);

                comando.Parameters.Add("@login", NpgsqlDbType.Varchar).Value = login;
                comando.Parameters.Add("@senha", NpgsqlDbType.Varchar).Value = CriptoHelper.HashMD5(senha);

                try
                {
                    //ret = ((long)comando.ExecuteScalar() > 0);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
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



        public static List<UsuarioModel> RecuperarLista()
        {
            var ret = new List<UsuarioModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from usuario order by nome";
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"],
                        });
                    }
                }
            }
            return ret;
        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel ret = null;

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from usuario where (id =@id)";

                    comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        };
                    }
                }
            }
            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new NpgsqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new NpgsqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from usuario where (id = @id)";

                        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }
            return ret;
        }

        public int Salvar() //Apenas inclui os dados no banco!
        {
            var model = RecuperarPeloId(this.Id);

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                if (IncluirRegistro(model, conexao) == false)
                {
                    return Update(model, conexao) ? 1 : 0;
                }
                else
                {
                    return 1;
                }

            }
        }

        private bool IncluirRegistro(UsuarioModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"insert into usuario (nome, login, senha) values (@Nome, @login, @senha)";
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                if (model == null)
                {
                    comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, this.Nome);
                    comando.Parameters.AddWithValue("Login", NpgsqlTypes.NpgsqlDbType.Boolean, this.Login);
                    comando.Parameters.AddWithValue("Senha", NpgsqlTypes.NpgsqlDbType.Boolean, CriptoHelper.HashMD5(this.Senha));
                    comando.Prepare();

                    comando.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        private bool Update(UsuarioModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"update usuario set nome = @Nome, login = @Login" +
                (!string.IsNullOrEmpty(this.Senha) ?" ,senha=@Senha": "") + " where id = @Id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                comando.Parameters.AddWithValue("@nome", NpgsqlTypes.NpgsqlDbType.Varchar, this.Nome);
                comando.Parameters.AddWithValue("@login", NpgsqlTypes.NpgsqlDbType.Boolean, this.Login);

                if (!string.IsNullOrEmpty(this.Senha))
                {
                    comando.Parameters.AddWithValue("@senha", NpgsqlTypes.NpgsqlDbType.Boolean, CriptoHelper.HashMD5(this.Senha));
                }
                comando.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Integer, this.Id);
                comando.Prepare();
                
                if (comando.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
    }
}

