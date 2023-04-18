using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aucom.NFeManifestacao.DAL
{
    public class RespostaEventoDAO : AbstractDAO<resposta_evento>
    {
        public override void GetEntidade(ref resposta_evento entity)
        {
            int id = entity.id_resposta;

            using (MeuContexto = new manifestaEntities(MinhaConexao))
            {
                entity = MeuContexto.resposta_evento.Where(r => r.id_resposta == id).FirstOrDefault();
            }
        }
    }
}
