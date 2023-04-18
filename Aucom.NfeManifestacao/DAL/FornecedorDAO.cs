using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.DAL
{
    public class FornecedorDAO : AbstractDAO<fornecedor>
    {
        public override void GetEntidade(ref fornecedor entity)
        {
            string razao = entity.cnpj;
            int id = entity.id_fornecedor;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                entity = MeuContexto.fornecedor.Where(f => f.cnpj == razao || f.id_fornecedor == id).FirstOrDefault();
            }
        }
    }
}
