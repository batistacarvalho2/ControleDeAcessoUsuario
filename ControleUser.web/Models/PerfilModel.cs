using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ControleUser.web.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }
        public int IdUsuario { get; set; }
        public string UsuarioProprietario { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }


        public static List<PerfilModel> RecuperarLista()
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format(@"select p.id, p.nome, u.nome as usuarioProprietario, p.data_criacao, p.ativo, p.id_usuario, u.nome 
                                                                    from perfil p
                                                                    inner join usuario u
                                                                    on u.id = p.id_usuario");

                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            IdUsuario = (int)reader["id_usuario"],
                            UsuarioProprietario = (string)reader["usuarioProprietario"],
                            DataCriacao = (DateTime)reader["data_criacao"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }
            return ret;
        }

        public static List<PerfilModel> RecuperarListaAtivos()
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("SELECT * FROM perfil where ativo='1' order by nome");
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            IdUsuario = (int)reader["id_usuario"],
                            DataCriacao = (DateTime)reader["data_criacao"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }
            return ret;
        }


        public static PerfilModel RecuperarPeloId(int id)
        {
            PerfilModel ret = null;

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from perfil where (id =@id)";

                    comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            IdUsuario = (int)reader["id_usuario"],
                            DataCriacao = (DateTime)reader["data_criacao"],
                            Ativo = (bool)reader["ativo"],
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
                        comando.CommandText = "delete from perfil where (id = @id)";

                        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }
            return ret;
        }

        public int Salvar()
        {
            var model = RecuperarPeloId(this.Id);

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                if(model == null)
                {
                    return IncluirRegistro(model, conexao) ? 1 : 0;
                }
                else
                {
                    return Update(model, conexao) ? 1 : 0;
                }
            }
        }

        private bool IncluirRegistro(PerfilModel model, NpgsqlConnection conexao)
        {
            var queryResult = @"insert into perfil
                                            (nome, data_criacao, ativo, id_usuario)
                                        values
                                            (@Nome,  @data_criacao, @Ativo, @id_usuario)";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                if (model == null)
                {
                    comando.Parameters.AddWithValue("Nome", NpgsqlDbType.Varchar, this.Nome);
                    comando.Parameters.AddWithValue("id_usuario", NpgsqlDbType.Integer, this.IdUsuario);
                    comando.Parameters.AddWithValue("Data_criacao", NpgsqlDbType.Timestamp, DateTime.Now);
                    comando.Parameters.AddWithValue("Ativo", NpgsqlDbType.Boolean, this.Ativo);
                    comando.Prepare();

                    comando.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        private bool Update(PerfilModel model, NpgsqlConnection conexao)
        {
            var queryResult = @"update perfil set
                                        nome = @nome,
                                        ativo = @ativo,
                                        id_usuario = @id_usuario
                                   where
                                        id = @id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                comando.Parameters.AddWithValue("nome", NpgsqlDbType.Varchar, this.Nome);
                comando.Parameters.AddWithValue("id_usuario", NpgsqlDbType.Integer, this.IdUsuario);
                comando.Parameters.AddWithValue("ativo", NpgsqlDbType.Boolean, this.Ativo);
                comando.Parameters.AddWithValue("id", NpgsqlDbType.Integer, this.Id);
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