using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.DAL
{
    public class TipoEventoDAO : AbstractDAO<tipo_evento>
    {
        public override void GetEntidade(ref tipo_evento entity)
        {
            int id = entity.id_tipo;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                entity = MeuContexto.tipo_evento.Where(t => t.id_tipo == id).FirstOrDefault();
            }
        }

        public List<tipo_evento> BuscaTipoEvento210()
        {
            List<tipo_evento> tipoEvento;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                tipoEvento = (from t in MeuContexto.tipo_evento
                              where t.id_tipo != 210210
                              select t).ToList();
            }
            return tipoEvento;
        }
    }
}
