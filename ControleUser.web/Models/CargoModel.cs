using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace ControleUser.web.Models
{
    public class CargoModel
    {
            public int Id { get; set; }
            [Required(ErrorMessage = "Preencha o Cargo.")]
            public string Cargo { get; set; }


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
                        comando.CommandText = "SELECT COUNT(*) FROM cargo_funcao";
                        ret = Convert.ToInt32(comando.ExecuteScalar());

                    }
                }
                return ret;
            }

            public static List<CargoModel> RecuperarLista(int pagina, int tamPagina)
            {
                var ret = new List<CargoModel>();

                using (var conexao = new NpgsqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new NpgsqlCommand())
                    {
                        var pos = (pagina - 1) * tamPagina;

                        comando.Connection = conexao;
                        comando.CommandText = string.Format(@"SELECT * FROM cargo_funcao
                                                                        ORDER BY cargo 
                                                                        OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                        var reader = comando.ExecuteReader();

                        while (reader.Read())
                        {
                            ret.Add(new CargoModel
                            {
                                Id = (int)reader["id"],
                                Cargo= (string)reader["cargo"],                                
                            });
                        }
                    }
                }
                return ret;
            }

        public static List<CargoModel> RecuperarListaCargo()
        {
            var ret = new List<CargoModel>();

            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("SELECT * FROM cargo_funcao  order by cargo");
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new Models.CargoModel
                        {
                            Id = (int)reader["id"],
                            Cargo= (string)reader["cargo"],                            
                        });
                    }
                }
            }
            return ret;
        }

        public static CargoModel RecuperarPeloId(int id)
            {
                CargoModel ret = null;

                using (var conexao = new NpgsqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new NpgsqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "select * from cargo_funcao where (id =@id)";

                        comando.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                        var reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            ret = new CargoModel
                            {
                                Id = (int)reader["id"],
                                Cargo = (string)reader["cargo"],                                
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
                            comando.CommandText = "delete from cargo_funcao where (id = @id)";

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

        private bool IncluirRegistro(CargoModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"insert into cargo_funcao (cargo) values (@Cargo)";
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                if (model == null)
                {
                    comando.Parameters.AddWithValue("Cargo", NpgsqlDbType.Varchar, this.Cargo);
                    comando.Prepare();

                    comando.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        private bool Update(CargoModel model, NpgsqlConnection conexao)
        {
            var queryResult = $@"update cargo_funcao
                                        set cargo = @Cargo 
                                        where 
                                        id = @Id";
            using (var comando = new NpgsqlCommand(queryResult, conexao))
            {
                comando.Parameters.AddWithValue("Cargo", NpgsqlDbType.Varchar, model.Cargo);
                comando.Parameters.AddWithValue("Id", NpgsqlDbType.Integer, model.Id);
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