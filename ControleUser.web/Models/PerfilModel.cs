﻿using Npgsql;
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
        [Required(ErrorMessage = "Preencha o Usuario Proprietario.")]
        public string UsuarioProprietario { get; set; }

        public DateTime DataCriacao { get; set; }


        public bool Ativo { get; set; }

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
                    comando.CommandText = "SELECT COUNT(*) FROM perfil";
                    ret = Convert.ToInt32(comando.ExecuteScalar());

                }
            }
            return ret;
        }

        public static List<PerfilModel> RecuperarLista(int pagina, int tamPagina)
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(@"SELECT * FROM perfil 
                                                                    ORDER BY nome
                                                                    OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            UsuarioProprietario = (string)reader["usuario_proprietario"],
                            DataCriacao = (DateTime)reader["data_criacao"],
                            Ativo = (bool)reader["ativo"],
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
                            UsuarioProprietario = (string)reader["usuario_proprietario"],
                            DataCriacao = (DateTime)reader["data_criacao"],
                            Ativo = (bool)reader["ativo"],
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
                            UsuarioProprietario = (string)reader["usuario_proprietario"],
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
            var queryResult = $@"insert into perfil
                                            (nome, usuario_proprietario, data_criacao, ativo)
                                        values
                                            (@Nome, @usuario_proprietario, @data_criacao, @Ativo)";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                if (model == null)
                {
                    comando.Parameters.AddWithValue("Nome", NpgsqlDbType.Varchar, Nome);
                    comando.Parameters.AddWithValue("Usuario_proprietario", NpgsqlDbType.Varchar, UsuarioProprietario);
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
            var queryResult = $@"update perfil set
                                        nome = @nome,
                                        usuario_proprietario = @usuario_proprietario,
                                        ativo = @ativo
                                   where
                                        id = @id";

            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                comando.Parameters.AddWithValue("nome", NpgsqlDbType.Varchar, model.Nome);
                comando.Parameters.AddWithValue("usuario_proprietario", NpgsqlTypes.NpgsqlDbType.Varchar, model.UsuarioProprietario);
                comando.Parameters.AddWithValue("ativo", NpgsqlTypes.NpgsqlDbType.Boolean, model.Ativo);
                comando.Parameters.AddWithValue("id", NpgsqlTypes.NpgsqlDbType.Integer, model.Id);
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