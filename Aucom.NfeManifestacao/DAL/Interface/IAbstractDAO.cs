using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Scire.NFeManifestacao.DAL
{
    public interface IAbstractDAO<T> where T : System.Data.Objects.DataClasses.EntityObject
    {
        int Delete(T entity);
        System.Collections.Generic.List<T> GetAll();
        void GetEntidade(ref T entity);
        int Insert(T entity);
        ScireNfeEntities MeuContexto { get; set; }
        System.Data.Objects.ObjectSet<T> MeuObjetSet { get; }
        System.Data.EntityClient.EntityConnection MinhaConexao { get; }
        int Update(T entity);
    }
}
