using ControleUser.web.Helpers;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ControleUser.web.Models
{
    public class RelatorioUsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public int IdCargo { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public int IdPerfil { get; set; }
        public string NomeCargo { get; set; }
        public bool Ativo { get; set; }
        public bool Administrador { get; set; }

        public static RelatorioUsuarioModel ValidarUsuario(string login, string senha)
        {
            RelatorioUsuarioModel ret = null;


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
                        ret = new RelatorioUsuarioModel
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

        public static List<RelatorioUsuarioModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<RelatorioUsuarioModel>();


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
                        ret.Add(new RelatorioUsuarioModel
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

        public static RelatorioUsuarioModel RecuperarPeloId(int id)
        {
            RelatorioUsuarioModel ret = null;

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
                        ret = new RelatorioUsuarioModel
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


    }
}