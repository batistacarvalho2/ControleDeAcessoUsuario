using ControleUser.web.Helpers;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ControleUser.web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

         [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }

         [Required(ErrorMessage = "Informe o senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o Cargo")]
        public int IdPerfil { get; set; }

        public bool Ativo { get; set; }




        public static UsuarioModel ValidarUsuario(string login, string senha)
        {
            UsuarioModel ret = null;

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                NpgsqlCommand comando = new NpgsqlCommand(string.Format(
                    "select * from usuario where login=@login and senha=@senha", login, CriptoHelper.HashMD5(senha)), conexao);

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
                            Nome = (string)reader["nome"],
                            Email = (string)reader["email"],
                            IdPerfil = (int)reader["id_perfil"],
                            Ativo = (bool)reader["ativo"]

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

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "SELECT COUNT(*) FROM usuario";
                    ret = Convert.ToInt32(comando.ExecuteScalar());

                }
            }
            return ret;
        }

        public static List<UsuarioModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<UsuarioModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format("SELECT * FROM usuario ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Email = (string)reader["email"],
                            Login = (string)reader["login"],
                            IdPerfil = (int)reader["id_perfil"],
                            Ativo = (bool)reader["ativo"]
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
                            Email = (string)reader["email"],
                            Login = (string)reader["login"],
                            IdPerfil = (int)reader["id_perfil"],
                            Ativo = (bool)reader["ativo"]
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

        public bool Salvar(UsuarioModel model) //Apenas inclui os dados no banco!
        {
            if (model.Id == 0)
                return SalvarUsuario();
            else
                return AtualizaUsuario(model);
        }

        private bool SalvarUsuario()
        {
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;              
                var queryResult = $@"insert into usuario (nome, email, login, senha, id_perfil, ativo) values (@nome, @email, @login, @senha, @id_Perfil, @ativo)";
        
                using (var comando = new NpgsqlCommand(queryResult, conexao))
                {
                    conexao.Open();

                    comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, this.Nome);
                    comando.Parameters.AddWithValue("Email", NpgsqlTypes.NpgsqlDbType.Varchar, this.Email);
                    comando.Parameters.AddWithValue("Login", NpgsqlTypes.NpgsqlDbType.Varchar, this.Login);
                    comando.Parameters.AddWithValue("Senha", NpgsqlTypes.NpgsqlDbType.Varchar, CriptoHelper.HashMD5(this.Senha));
                    comando.Parameters.AddWithValue("Id_perfil", NpgsqlTypes.NpgsqlDbType.Integer, this.IdPerfil);
                    comando.Parameters.AddWithValue("Ativo", NpgsqlTypes.NpgsqlDbType.Boolean, this.Ativo);

                    comando.Prepare();

                    var res = comando.ExecuteNonQuery();
                    
                    conexao.Close();

                    if (res > 0)
                        return true;
                    else
                        return false;
                }
            }
        }

        private bool AtualizaUsuario(UsuarioModel model)
        {

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
               
                if (string.IsNullOrEmpty(model.Senha))
                    return AtualizaSemSenha(model, conexao);
                else
                    return AtualizaComSenha(model, conexao);
            }
        }

        private bool AtualizaComSenha(UsuarioModel model, NpgsqlConnection conexao)
        {
            string queryResult = $@"update usuario set 
                                            nome = @nome,
                                            email = @email,
                                            login = @login, 
                                            id_perfil=@id_perfil,
                                            senha = @senha,
                                            ativo = @ativo
                                    where 
                                            id = @id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                conexao.Open();

                comando.Parameters.AddWithValue("nome", NpgsqlTypes.NpgsqlDbType.Varchar, model.Nome);
                comando.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Varchar, model.Email);
                comando.Parameters.AddWithValue("login", NpgsqlTypes.NpgsqlDbType.Varchar, model.Login);
                comando.Parameters.AddWithValue("senha", NpgsqlTypes.NpgsqlDbType.Varchar, CriptoHelper.HashMD5(model.Senha));
                comando.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Integer, model.Id);
                comando.Parameters.AddWithValue("id_perfil", NpgsqlTypes.NpgsqlDbType.Integer, model.IdPerfil);
                comando.Parameters.AddWithValue("ativo", NpgsqlTypes.NpgsqlDbType.Boolean, model.Ativo);

                comando.Prepare();

                var res = comando.ExecuteNonQuery();

                conexao.Close();

                if (res > 0)
                    return true;
                else
                    return false;
            }
        }

        private bool AtualizaSemSenha(UsuarioModel model, NpgsqlConnection conexao)
        {
            string queryResult = $@"update usuario set 
                                            nome=@nome,
                                            email=@email
                                            login=@login,
                                            id_perfil=@id_perfil,
                                            ativo=@ativo

                                    where 
                                            id=@id";
            
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                conexao.Open();

                comando.Parameters.AddWithValue("nome", NpgsqlTypes.NpgsqlDbType.Varchar, model.Nome);
                comando.Parameters.AddWithValue("email", NpgsqlTypes.NpgsqlDbType.Varchar, model.Email);
                comando.Parameters.AddWithValue("login", NpgsqlTypes.NpgsqlDbType.Varchar, model.Login);
                comando.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Integer, model.Id);
                comando.Parameters.AddWithValue("id_perfil", NpgsqlTypes.NpgsqlDbType.Integer, model.IdPerfil);
                comando.Parameters.AddWithValue("ativo", NpgsqlTypes.NpgsqlDbType.Boolean, model.Ativo);

                comando.Prepare();

                var res = comando.ExecuteNonQuery();

                conexao.Close();
                
                if(res > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}

