using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.DAL
{
    public class DestinatarioDAO : AbstractDAO<destinatario>
    {
        public override void GetEntidade(ref destinatario entity)
        {
            int id = entity.id_destinatario;
            string cnpj = entity.cnpj;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                entity = MeuContexto.destinatario.Where(d => d.cnpj == cnpj || d.id_destinatario == id).FirstOrDefault();
            }
        }
    }
}
