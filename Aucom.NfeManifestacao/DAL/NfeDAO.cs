using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.DAL
{
    public class NfeDAO : AbstractDAO<nfe>
    {
        public override void GetEntidade(ref nfe entity)
        {
            string chave = entity.chave;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                entity = MeuContexto.nfe.Where(n => n.chave == chave).FirstOrDefault();
            }
        }
    }
}
