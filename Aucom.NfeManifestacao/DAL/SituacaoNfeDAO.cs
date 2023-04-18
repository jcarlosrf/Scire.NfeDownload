using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aucom.NFeManifestacao.DAL
{
    public class SituacaoNfeDAO : AbstractDAO<situacao_nfe>
    {
        UptabelasDAO upDao = new UptabelasDAO();

        public override void GetEntidade(ref situacao_nfe entity)
        {
            int id = entity.id_situacao;
            using (MeuContexto = new manifestaEntities(MinhaConexao))
            {
                entity = MeuContexto.situacao_nfe.Where(s => s.id_situacao == id).FirstOrDefault();  
            }
        }
    }
}
