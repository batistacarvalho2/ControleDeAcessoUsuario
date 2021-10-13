using ControleUser.web.Helpers;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ControleUser.web.Models
{
    public class RelatorioGrupoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string UsuarioProprietario { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        public static List<RelatorioGrupoModel> RecuperarLista()
        {
            var ret = new List<RelatorioGrupoModel>();


            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format(@"SELECT * FROM perfil 
                                                                    ORDER BY nome");


                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new RelatorioGrupoModel
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
    }
}