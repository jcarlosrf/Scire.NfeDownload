using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aucom.NFeManifestacao.DAL
{
    public class UptabelasDAO : AbstractDAO<uptabelas>
    {
        public enum Operacao
        {
            Insert, Update, Delete
        }

        public override void GetEntidade(ref uptabelas uptab)
        {
            decimal id = uptab.sequencia;

            using (MeuContexto = new manifestaEntities(MinhaConexao))
            {
                uptab = MeuContexto.uptabelas.Where(u => u.sequencia == id).FirstOrDefault();
            }
        }
    }
}
