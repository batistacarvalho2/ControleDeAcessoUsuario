﻿using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ControleUser.web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public static List<GrupoProdutoModel> RecuperarLista()
        {
            var ret = new List<GrupoProdutoModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from grupo_produto order by nome";
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"],
                        });
                    }
                }
            }
            return ret;
        }

        public static GrupoProdutoModel RecuperarPeloId(int id)
        {
            GrupoProdutoModel ret = null;

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from grupo_produto where (id =@id)";

                    comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
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
                        comando.CommandText = "delete from grupo_produto where (id = @id)";

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

        private bool IncluirRegistro(GrupoProdutoModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"insert into grupo_produto (nome, ativo) values (@Nome, @Ativo)";
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                if (model == null)
                {
                    comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, this.Nome);
                    comando.Parameters.AddWithValue("Ativo", NpgsqlTypes.NpgsqlDbType.Boolean, this.Ativo);
                    comando.Prepare();

                    comando.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        private bool Update(GrupoProdutoModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"update grupo_produto set nome = @Nome, ativo = @Ativo where id = @Id";
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, model.Nome);
                comando.Parameters.AddWithValue("Ativo", NpgsqlTypes.NpgsqlDbType.Boolean, model.Ativo);
                comando.Parameters.AddWithValue("Id", NpgsqlTypes.NpgsqlDbType.Integer, model.Id);
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