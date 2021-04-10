using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ControleUser.web.Models
{
    public class UnidadeMedidaModel
    {
            public int Id { get; set; }

            [Required(ErrorMessage = "Preencha o nome.")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "Preencha a sigla.")]
            public string Sigla { get; set; }

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
                        comando.CommandText = "SELECT COUNT(*) FROM unidade_medida";
                        ret = Convert.ToInt32(comando.ExecuteScalar());

                    }
                }
                return ret;
            }

            public static List<UnidadeMedidaModel> RecuperarLista(int pagina, int tamPagina)
            {
                var ret = new List<UnidadeMedidaModel>();

                using (var conexao = new NpgsqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new NpgsqlCommand())
                    {
                        var pos = (pagina - 1) * tamPagina;

                        comando.Connection = conexao;
                        comando.CommandText = string.Format("SELECT * FROM unidade_medida ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                        var reader = comando.ExecuteReader();

                        while (reader.Read())
                        {
                            ret.Add(new Models.UnidadeMedidaModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                Sigla = (string)reader["sigla"],
                                Ativo = (bool)reader["ativo"],
                            });
                        }
                    }
                }
                return ret;
            }

            public static UnidadeMedidaModel RecuperarPeloId(int id)
            {
                UnidadeMedidaModel ret = null;

                using (var conexao = new NpgsqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new NpgsqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "select * from unidade_medida where (id =@id)";

                        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                        var reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            ret = new UnidadeMedidaModel
                            {
                                Id = (int)reader["id"],
                                Nome = (string)reader["nome"],
                                Sigla = (string)reader["sigla"],
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
                            comando.CommandText = "delete from unidade_medida where (id = @id)";

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

            private bool IncluirRegistro(UnidadeMedidaModel model, NpgsqlConnection conexao)
            {
                var queryResult = $@"insert into unidade_medida (nome, sigla, ativo) values (@Nome, @Sigla, @Ativo)";
                using (var comando = new NpgsqlCommand(queryResult, conexao))
                {
                    if (model == null)
                    {
                        comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, this.Nome);
                        comando.Parameters.AddWithValue("Sigla", NpgsqlTypes.NpgsqlDbType.Varchar, this.Sigla);
                        comando.Parameters.AddWithValue("Ativo", NpgsqlTypes.NpgsqlDbType.Boolean, this.Ativo);
                        comando.Prepare();

                        comando.ExecuteNonQuery();
                        return true;
                    }
                }

                return false;
            }

            private bool Update(UnidadeMedidaModel model, NpgsqlConnection conexao)
            {
                var queryResult = $@"update unidade_medida set nome = @Nome, sigla = @Sigla, ativo = @Ativo where id = @Id";
                using (var comando = new NpgsqlCommand(queryResult, conexao))
                {
                    comando.Parameters.AddWithValue("Nome", NpgsqlTypes.NpgsqlDbType.Varchar, model.Nome);
                    comando.Parameters.AddWithValue("Sigla", NpgsqlTypes.NpgsqlDbType.Varchar, model.Sigla);
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