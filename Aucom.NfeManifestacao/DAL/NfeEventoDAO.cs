using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aucom.NFeManifestacao.DAL
{
    public class NfeEventoDAO : AbstractDAO<nfe_evento>
    {
        public override void GetEntidade(ref nfe_evento entity)
        {
            string chave = entity.chave;
            //int tipoEvento = entity.tipo_evento;

            using(MeuContexto = new manifestaEntities(MinhaConexao))
            {
                entity = MeuContexto.nfe_evento.Where(e => e.chave == chave).FirstOrDefault();
            }
        }
    }
}
