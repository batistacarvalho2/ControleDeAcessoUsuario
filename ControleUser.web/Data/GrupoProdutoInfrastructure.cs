using ControleUser.web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ControleUser.web.Data
{
    public class GrupoProdutoInfrastructure : IInfrastructure<GrupoProdutoInfrastructure>
    {
        // Obter a string de conexão global
        // Definir os metodos de Update, Insert, Delete, SelectById, SelectAll
        public Task Add(GrupoProdutoInfrastructure model)
        {
            throw new NotImplementedException();
        }

        public Task Delete(GrupoProdutoInfrastructure model)
        {
            throw new NotImplementedException();
        }

        public Task<GrupoProdutoInfrastructure> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<GrupoProdutoInfrastructure>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(GrupoProdutoInfrastructure model)
        {
            throw new NotImplementedException();
        }
    }
}