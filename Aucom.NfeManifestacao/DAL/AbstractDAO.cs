using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.Data;

namespace Scire.NFeManifestacao.DAL
{
    public abstract class AbstractDAO<T> : Scire.NFeManifestacao.DAL.IAbstractDAO<T> where T : EntityObject
    {
        private DAL.ScireNfeEntities Contexto;
        private EntityConnection Conexao;
        private ConexaoBD conecta = new ConexaoBD();
        private const string nomeconexao = "Scire";

        public DAL.ScireNfeEntities MeuContexto
        {
            get
            {
                return this.Contexto;
            }
            set
            {
                this.Contexto = value;
            }
        }

        public EntityConnection MinhaConexao
        {
            get
            {
                return this.Conexao;
            }
        }

        public System.Data.Objects.ObjectSet<T> MeuObjetSet
        {
            get
            {
                this.Contexto = new DAL.ScireNfeEntities(MinhaConexao);
                return this.Contexto.CreateObjectSet<T>();
            }
        }

        #region Construtores
        /// <summary>
        /// Construtor básico da classe
        /// </summary>
        public AbstractDAO()
        {
            //this.Conexao = conecta.GetConexao(nomeconexao);

            this.Conexao = new EntityConnection((new DAL.ScireNfeEntities()).Connection.ConnectionString);
        }

        #endregion

        #region CRUD
        public virtual int Insert(T entity)
        {
            int iRet = 0;   

            using (this.Contexto = new DAL.ScireNfeEntities(MinhaConexao))
            {
                this.Contexto.AddObject(entity.GetType().Name, entity);
                iRet = this.Contexto.SaveChanges();
            }
            return iRet;
        }

        public virtual int Update(T entity)
        {
            int iRet = 0;
            using (this.Contexto = new DAL.ScireNfeEntities(MinhaConexao))
            {
                this.Contexto.Attach(entity);
                this.Contexto.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                iRet = this.Contexto.SaveChanges();
            }
            return iRet;
        }

        public virtual int Delete(T entity)
        {
            int iRet = 0;
            using (this.Contexto = new DAL.ScireNfeEntities(MinhaConexao))
            {
                this.Contexto.Attach(entity);
                this.Contexto.DeleteObject(entity);
                iRet = this.Contexto.SaveChanges();
            }
            return iRet;
        }

        public abstract void GetEntidade(ref T entity);

        public virtual List<T> GetAll()
        {
            using (this.Contexto = new DAL.ScireNfeEntities(MinhaConexao))
            {
                IQueryable<T> pesquisa = this.Contexto.CreateObjectSet<T>().AsQueryable<T>();
                return pesquisa.ToList();
            }
        }
        #endregion
    }
}
