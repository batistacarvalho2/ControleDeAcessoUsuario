using ControleUser.web.Helpers;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;


namespace ControleUser.web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe o login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Infome o cargo")]
        public int IdCargo { get; set; }

        [Required(ErrorMessage = "Informe o senha")]
        public string Senha { get; set; }       

        [Required(ErrorMessage = "Informe o E-mail")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        [RegularExpression(@"(\w+@\w+\.\w+)(\.\w+)?", ErrorMessage = "E-mail em formato inválido.")]
        public string Email { get; set; }
        public int IdPerfil { get; set; }        
        public string NomeCargo { get; set; }
        public bool Ativo { get; set; }
        public bool Administrador { get; set; }

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
                            IdCargo = (int)reader["id_cargo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Administrador = (bool)reader["administrador"],
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
                    //comando.CommandText = string.Format("SELECT * FROM usuario ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                    comando.CommandText = string.Format(@"SELECT * FROM usuario
                                                inner join cargo_funcao on usuario.id_cargo = cargo_funcao.id 
                                                ORDER BY usuario.nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);


                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Email = (string)reader["email"],
                            Login = (string)reader["login"],
                            IdCargo = (int)reader["id_cargo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Administrador = (bool)reader["administrador"],
                            Ativo = (bool)reader["ativo"],
                            NomeCargo = (string)reader["Cargo"]
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
                            IdCargo = (int)reader["id_cargo"],
                            IdPerfil = (int)reader["id_perfil"],
                            Administrador = (bool)reader["administrador"],
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

        public bool ValidaLogin(UsuarioModel login)
        {
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                NpgsqlCommand comando = new NpgsqlCommand("select login from usuario where login = @login", conexao);

                conexao.Open();
                comando.Parameters.Add("@login", NpgsqlDbType.Varchar).Value = login.Login;

                try
                {
                    var countLogin = comando.ExecuteScalar();
                    return countLogin == null ? true : false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Login já cadastrado " + e);
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public bool ValidaEmail(UsuarioModel email)
        {
            // var id = RecuperarPeloId();
            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                NpgsqlCommand comando = new NpgsqlCommand("select login from usuario where email = @email", conexao);

                conexao.Open();
                comando.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = email.Email;

                try
                {
                    var countEmail = comando.ExecuteScalar();
                    return countEmail == null ? true : false;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Login já cadastrado " + e);
                    throw;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public bool Salvar(UsuarioModel model) //Apenas inclui os dados no banco!
        {
            if (model.Id == 0)
                return SalvarUsuario();
            else
                return AtualizaUsuario(model);
        }

        public int ValidaAdmin(bool admin)
        {
            if (admin == true)
                return 2;
            else
                return 3;
        }


        private bool SalvarUsuario()
        {
            IdPerfil = ValidaAdmin(this.Administrador);

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                var queryResult = $@"insert into usuario
                                                (nome, email, login, id_cargo, senha, id_perfil, administrador, ativo)
                                                values
                                                (@nome, @email, @login, @id_cargo, @senha, @id_Perfil, @administrador, @ativo)";

                using (var comando = new NpgsqlCommand(queryResult, conexao))
                {
                    conexao.Open();

                    comando.Parameters.AddWithValue("Nome", NpgsqlDbType.Varchar, this.Nome);
                    comando.Parameters.AddWithValue("Email", NpgsqlDbType.Varchar, this.Email);
                    comando.Parameters.AddWithValue("Login", NpgsqlDbType.Varchar, this.Login);
                    comando.Parameters.AddWithValue("Id_Cargo", NpgsqlDbType.Integer, this.IdCargo);
                    comando.Parameters.AddWithValue("Senha", NpgsqlDbType.Varchar, CriptoHelper.HashMD5(this.Senha));
                    comando.Parameters.AddWithValue("Id_perfil", NpgsqlDbType.Integer, this.IdPerfil);
                    comando.Parameters.AddWithValue("Administrador", NpgsqlDbType.Boolean, this.Administrador);
                    comando.Parameters.AddWithValue("Ativo", NpgsqlDbType.Boolean, this.Ativo);

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
            IdPerfil = ValidaAdmin(this.Administrador);
            string queryResult = $@"update usuario set 
                                            nome = @nome,
                                            email = @email,
                                            login = @login, 
                                            id_cargo = @id_cargo,                                             
                                            senha = @senha,
                                            administrador = @administrador,
                                            ativo = @ativo,
                                            id_perfil=@id_perfil
                                    where 
                                            id = @id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                conexao.Open();

                comando.Parameters.AddWithValue("nome", NpgsqlDbType.Varchar, this.Nome);
                comando.Parameters.AddWithValue("email", NpgsqlDbType.Varchar, this.Email);
                comando.Parameters.AddWithValue("login", NpgsqlDbType.Varchar, this.Login);
                comando.Parameters.AddWithValue("id_cargo",NpgsqlDbType.Integer, this.IdCargo);
                comando.Parameters.AddWithValue("senha", NpgsqlDbType.Varchar, CriptoHelper.HashMD5(this.Senha));
                comando.Parameters.AddWithValue("id", NpgsqlDbType.Integer, this.Id);
                comando.Parameters.AddWithValue("id_perfil", NpgsqlDbType.Integer, this.IdPerfil);
                comando.Parameters.AddWithValue("administrador",NpgsqlDbType.Boolean, this.Administrador);
                comando.Parameters.AddWithValue("ativo", NpgsqlDbType.Boolean, this.Ativo);

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
            IdPerfil = ValidaAdmin(this.Administrador);
            string queryResult = $@"update usuario set 
                                            nome=@nome,
                                            email=@email,                                            
                                            login=@login,
                                            id_cargo=@id_cargo,
                                            id_perfil=@id_perfil,
                                            administrador = @administrador,
                                            ativo=@ativo

                                    where 
                                            id=@id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                conexao.Open();

                comando.Parameters.AddWithValue("nome", NpgsqlDbType.Varchar, this.Nome);
                comando.Parameters.AddWithValue("email", NpgsqlDbType.Varchar, this.Email);
                comando.Parameters.AddWithValue("login", NpgsqlDbType.Varchar, this.Login);
                comando.Parameters.AddWithValue("id_cargo", NpgsqlDbType.Integer, this.IdCargo);
                comando.Parameters.AddWithValue("id",NpgsqlDbType.Integer, this.Id);
                comando.Parameters.AddWithValue("id_perfil", NpgsqlDbType.Integer, this.IdPerfil);
                comando.Parameters.AddWithValue("administrador",NpgsqlDbType.Boolean, this.Administrador);
                comando.Parameters.AddWithValue("ativo",NpgsqlDbType.Boolean, this.Ativo);

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
}

