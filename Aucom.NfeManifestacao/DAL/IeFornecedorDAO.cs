using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aucom.NFeManifestacao.DAL
{
    public class IeFornecedorDAO : AbstractDAO<ie_fornecedor>
    {
        public override void GetEntidade(ref ie_fornecedor entity)
        {
            int id = entity.id_ie;

            using (MeuContexto = new manifestaEntities(MinhaConexao))
            {
                entity = MeuContexto.ie_fornecedor.Where(i => i.id_ie == id).FirstOrDefault();
            }
        }
    }
}
