using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace Scire.NFeManifestacao.DAL
{
    public class ConexaoBD
    {
        private Sapiens.Library.CriptografiaSimetrica cript = new Sapiens.Library.CriptografiaSimetrica();
        private DataTable dtConexao = new DataTable("Scire.Conexao");
        private string arquivoBD = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\Scire.Conexao.xml";

        public ConexaoBD()
        {
            dtConexao.Columns.Add("nome", System.Type.GetType("System.String"));
            dtConexao.Columns.Add("cript", System.Type.GetType("System.Boolean"));
            dtConexao.Columns.Add("Host", System.Type.GetType("System.String"));
            dtConexao.Columns.Add("Port", System.Type.GetType("System.String"));
            dtConexao.Columns.Add("Database", System.Type.GetType("System.String"));
            dtConexao.Columns.Add("UserName", System.Type.GetType("System.String"));
            dtConexao.Columns.Add("Password", System.Type.GetType("System.String"));

            if (!System.IO.File.Exists(arquivoBD))
            {
                DataRow row = dtConexao.NewRow();
                row["nome"] = "Scire";
                row["cript"] = "false";
                row["Host"] = "192.168.1.216";
                row["Port"] = "3306";
                row["Database"] = "jalovi";
                row["UserName"] = "root";
                row["Password"] = "123";
                dtConexao.Rows.Add(row);
               

                dtConexao.WriteXml(arquivoBD);
            }


            dtConexao.Rows.Clear();
            dtConexao.ReadXml(arquivoBD);

            foreach (DataRow row1 in dtConexao.Rows)
            {
                if ((bool)row1["cript"] == false)
                {
                    row1["cript"] = true;
                    row1["Host"] = cript.Encrypt(row1["Host"].ToString());
                    row1["Port"] = cript.Encrypt(row1["Port"].ToString());
                    row1["Database"] = cript.Encrypt(row1["Database"].ToString());
                    row1["UserName"] = cript.Encrypt(row1["UserName"].ToString());
                    row1["Password"] = cript.Encrypt(row1["Password"].ToString());
                }
            }
            dtConexao.WriteXml(arquivoBD);

            dtConexao.Rows.Clear();
        }

        public Boolean TestaConexao(string servidor, string porta, string usuario, string senha, string banco)
        {
            MySqlConnectionStringBuilder conexao = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
            conexao.Server = servidor;
            conexao.Port = uint.Parse(porta);
            conexao.UserID = usuario;
            conexao.Password = senha;
            conexao.Database = banco;

            MySqlConnection conectabd = new MySql.Data.MySqlClient.MySqlConnection(conexao.GetConnectionString(true));

            try
            {
                conectabd.Open();

                if (conectabd.State == ConnectionState.Open)
                {
                    conectabd.Close();
                    return true;

                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public EntityConnection GetConexao(string nomeconexao)
        {
            string conexao = string.Empty;

            dtConexao.Rows.Clear();
            dtConexao.ReadXml(arquivoBD);

            foreach (DataRow row1 in dtConexao.Rows)
            {
                if (row1["nome"].ToString() == nomeconexao)
                {
                    conexao = buildConnectionString(cript.Decrypt(row1["Host"].ToString())
                        , int.Parse(cript.Decrypt(row1["Port"].ToString()))
                        , cript.Decrypt(row1["UserName"].ToString())
                        , cript.Decrypt(row1["Password"].ToString())
                        , cript.Decrypt(row1["Database"].ToString()), nomeconexao);

                }
            }

            EntityConnection conn = new EntityConnection(conexao);


            return conn;
        }

        public MySqlConnection GetConexaoMysql(string nomeconexao)
        {

            MySql.Data.MySqlClient.MySqlConnectionStringBuilder conexao = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

            dtConexao.Rows.Clear();
            dtConexao.ReadXml(arquivoBD);

            foreach (DataRow row1 in dtConexao.Rows)
            {
                if (row1["nome"].ToString() == nomeconexao)
                {
                    conexao.Server = cript.Decrypt(row1["Host"].ToString());
                    conexao.Port = uint.Parse(cript.Decrypt(row1["Port"].ToString()));
                    conexao.UserID = cript.Decrypt(row1["UserName"].ToString());
                    conexao.Password = cript.Decrypt(row1["Password"].ToString());
                    conexao.Database = cript.Decrypt(row1["Database"].ToString());
                }
            }

            MySqlConnection conn = new MySqlConnection(conexao.GetConnectionString(true));

            return conn;
        }

        private static string buildConnectionString(string host, int porta, string user, string pwd, string banco, string nomeconexao)
        {
            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder();
            entityConnectionStringBuilder.Provider = "MySql.Data.MySqlClient";
            if (nomeconexao == "Scire")
            {
                entityConnectionStringBuilder.Metadata = "res://*/DAL.ManifestacaoModel.csdl|res://*/DAL.ManifestacaoModel.ssdl|res://*/DAL.ManifestacaoModel.msl";
            }
            else
            {
                entityConnectionStringBuilder.Metadata = "res://*/DAL.ModelLojas.csdl|res://*/DAL.ModelLojas.ssdl|res://*/DAL.ModelLojas.msl";
            }

            string cnstring = string.Format(
                "host={0};Port={1};username={2};password={3};database={4};persistsecurityinfo=True;allowzerodatetime=True;convertzerodatetime=True"
                , host, porta, user, pwd, banco);

            entityConnectionStringBuilder.ProviderConnectionString = cnstring;

            return entityConnectionStringBuilder.ToString();
        }
    }
}
