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

        public static List<RelatorioUsuarioModel> RecuperarLista()
        {
            var ret = new List<RelatorioUsuarioModel>();


            using (var conexao = new NpgsqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new NpgsqlCommand())
                {        
                    comando.Connection = conexao;
                    //comando.CommandText = string.Format("SELECT * FROM usuario ORDER BY nome OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", pos > 0 ? pos - 1 : 0, tamPagina);
                    comando.CommandText = string.Format(@"SELECT * FROM usuario
                                                inner join cargo_funcao on usuario.id_cargo = cargo_funcao.id 
                                                ORDER BY usuario.nome");


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
    }
}