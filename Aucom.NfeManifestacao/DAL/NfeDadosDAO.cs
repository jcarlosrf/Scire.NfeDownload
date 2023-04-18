using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scire.NFeManifestacao.DAL
{
    class NfeDadosDAO : AbstractDAO<nfe_dados>
    {

        public override void GetEntidade(ref nfe_dados entity)
        {
            int id_nfe_dados = entity.id_nfe_dados;
            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                entity = MeuContexto.nfe_dados.Where(d => d.id_nfe_dados == id_nfe_dados).FirstOrDefault();
            }
        }

        public nfe_dados BuscarDadosByNfe(int nfe)
        {
            nfe_dados dados;
            int id_nfe = nfe;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                dados = (from d in MeuContexto.nfe_dados
                            where d.nfe == id_nfe
                            select d).FirstOrDefault();
            }
            return dados;
        }

        public List<nfe_dados> BuscarCodigo(int id_nfe)
        {
            List<nfe_dados> dados;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {

                dados = (from d in MeuContexto.nfe_dados
                         where d.nfe == id_nfe
                         select d).ToList();
            }
            return dados;
        }

        public nfe_dados BuscaByTipo(nfe_dados ndados)
        {
            nfe_dados dados;
            string tipo = ndados.tipo;
            int nfe = ndados.nfe;

            using (MeuContexto = new ScireNfeEntities(MinhaConexao))
            {
                dados = (from d in MeuContexto.nfe_dados
                         where d.nfe == nfe
                         && d.tipo == tipo
                         select d).FirstOrDefault();
            }

            return dados;
        }
    }
}
