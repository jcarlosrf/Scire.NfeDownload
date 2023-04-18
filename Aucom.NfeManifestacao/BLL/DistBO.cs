using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Data.Objects;

using Scire.NFeManifestacao;
using Scire.NFeManifestacao.XSDs.Download.Envio;
using Scire.NFeManifestacao.DAL;

namespace Scire.NFeManifestacao.BLL
{
    class DistBO
    {
        Sapiens.Library.Log.LogError logErro;

        distDFeInt dist = new distDFeInt();
        distDFeInt distNfe = new distDFeInt();
        Scire.NFeManifestacao.XSDs.Download.Resposta.retDistDFeInt ret = new Scire.NFeManifestacao.XSDs.Download.Resposta.retDistDFeInt();
        XmlNode xmlEnvio, xmlRetorno, xmlChave, xmlNota;
        List<XmlNode> resumidas = new List<XmlNode>();

        UtilBO util = new UtilBO();
        List<string> notas = new List<string>();
        X509Certificate2 CertificadoValido;
        distDFeIntDistNSU dnsu = new distDFeIntDistNSU();

        ManifestacaoBO manifesta = new ManifestacaoBO();

        private string uf { get; set; }
        private string cnpj { get; set; }
        private string razao { get; set; }
        private string ultNSU { get; set; }
        private string diretorioNFe { get; set; }
        private string diretorioEvento { get; set; }
        private string nsu { get; set; }
        public string ultimoNsu { get; set; }
        public string UltimoSefaz { get; set; }
        private string DiretorioNFe { get; set; }

        private string dataHora { get; set; }

        public DistBO()
        {
            this.cnpj = Properties.Settings.Default.Cnpj;
            this.razao = Properties.Settings.Default.Razao;
            this.CertificadoValido = UtilBO.GetCertificado(cnpj, razao);
            this.UltimoSefaz = Properties.Settings.Default.UltimoSefaz;
            this.uf = Properties.Settings.Default.Uf;
            diretorioNFe = Properties.Settings.Default.DiretorioNfe;
            diretorioEvento = Properties.Settings.Default.DiretorioNfe;
            nsu = "1";
            ultimoNsu = "1";

            dist.versao = TVerDistDFe.Item101;
            dist.ItemElementName = ItemChoiceType.CNPJ;
            dist.Item = cnpj;
            dist.cUFAutor = util.EventoUF_IBGEDownload(uf);
            dist.tpAmb = TAmb.Item1;
            dist.cUFAutorSpecified = true;

            distNfe.versao = dist.versao;
            distNfe.ItemElementName = dist.ItemElementName;
            distNfe.Item = dist.Item;
            distNfe.cUFAutor = dist.cUFAutor;
            distNfe.tpAmb = TAmb.Item1;
            distNfe.cUFAutorSpecified = true;

            logErro = new Sapiens.Library.Log.LogError(System.Windows.Forms.Application.StartupPath + "\\Log");
        }

        public void MontarObjeto()
        {
            this.cnpj = Properties.Settings.Default.Cnpj;
            this.razao = Properties.Settings.Default.Razao;
            this.CertificadoValido = UtilBO.GetCertificado(cnpj, razao);
            this.uf = Properties.Settings.Default.Uf;
            diretorioNFe = Properties.Settings.Default.DiretorioNfe;

            dist.versao = TVerDistDFe.Item101;
            dist.ItemElementName = ItemChoiceType.CNPJ;
            dist.Item = cnpj;
            dist.cUFAutor = util.EventoUF_IBGEDownload(uf);
            dist.tpAmb = TAmb.Item1;
            dist.cUFAutorSpecified = true;

            distNfe.versao = dist.versao;
            distNfe.ItemElementName = dist.ItemElementName;
            distNfe.Item = dist.Item;
            distNfe.cUFAutor = dist.cUFAutor;
            distNfe.tpAmb = TAmb.Item1;
            distNfe.cUFAutorSpecified = true;
        }

        public void BuscarTodasNfe()
        {
            for (int i = 0; i < int.Parse(ultimoNsu); i++)
            {
                BuscarNfe();
                i = int.Parse(nsu);
                i--;
            }
        }

        public void BuscarNFeCompleta()
        {
            DAL.ScireNfeEntities contexto = new DAL.ScireNfeEntities();
            ObjectResult<stp_NotasUltimoEvento_Result> obj = contexto.stp_NotasUltimoEvento("210", 0);

            foreach (stp_NotasUltimoEvento_Result nota in obj)
            {
                this.BuscarByChave(nota.chave);
            }
        }

        public void BuscarNfe()
        {
            this.ultNSU = Properties.Settings.Default.UltimoNSU.ToString().PadLeft(15, '0');
            dnsu.ultNSU = ultNSU;
            dist.Item1 = dnsu;

            xmlEnvio = UtilBO.SerializaObjeto(dist, typeof(distDFeInt));

            XElement xdoc = XElement.Load(new XmlNodeReader(xmlEnvio));

            NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient client = new NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient();

            client.ClientCredentials.ClientCertificate.Certificate = CertificadoValido;

            XElement xmlRetorno = client.nfeDistDFeInteresse(xdoc);
           
            dataHora = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

            if (VerificarNota(xmlRetorno) == false)
                return;

            nsu = (from n in xmlRetorno.Descendants()
                   where n.Name.LocalName.Equals("ultNSU")
                   select n).FirstOrDefault().Value;

            var doczips = (from n in xmlRetorno.Descendants()
                           where n.Name.LocalName.Equals("docZip")
                           select n).ToList();

            if (doczips.Count == 0)
                return;

            ultimoNsu = (from nota in xmlRetorno.Descendants()
                         where nota.Name.LocalName.Equals("maxNSU")
                         select nota).FirstOrDefault().Value;

            foreach (var doczip in doczips)
            {
                XmlNode xml;
                XmlDocument xmlDoc = new XmlDocument();
                ConvertZip(doczip.Value);
                xmlDoc.LoadXml(ConvertZip(doczip.Value));
                xml = xmlDoc.Clone();

                if (xml == null)
                    return;

                XElement element = XElement.Load(new XmlNodeReader(xml));

                if (xml["resNFe"] != null)
                {
                    #region Inseri Fornecedor e Nfe
                    manifesta.InseriFornecedor(xml);
                    manifesta.InseriNfe(xml, false);
                    #endregion
                }
                else if (xml["nfeProc"] != null)
                {
                    #region Inseri Fornecedor e grava a NFE
                    XElement procuraChave = XElement.Load(new XmlNodeReader(xml));

                    string chave = (from n in element.Descendants()
                                    where n.Name.LocalName.Equals("chNFe")
                                    select n).FirstOrDefault().Value;
                    manifesta.InseriFornecedor(xml);
                    manifesta.InseriDestinatario(procuraChave);
                    manifesta.InseriNfe(xml, true);

                    //if (chave == "35180747640982000140550010000033101013434190")
                        //MessageBox.Show("Parar aqui ");


                    GravarNFe(xml.InnerXml, chave, diretorioNFe);
                    #endregion
                }
            }

            Properties.Settings.Default.UltimoSefaz = ultimoNsu;
            Properties.Settings.Default.UltimoNSU = int.Parse(nsu);
            Properties.Settings.Default.Save();
        }

        public void BuscarByChave(string keyNota)
        {
            distDFeIntConsChNFe key = new distDFeIntConsChNFe();
            key.chNFe = keyNota;
            distNfe.Item1 = key;

            xmlChave = UtilBO.SerializaObjeto(distNfe, typeof(distDFeInt));

            XElement xNota = XElement.Load(new XmlNodeReader(xmlChave));

            NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient client = new NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient();

            client.ClientCredentials.ClientCertificate.Certificate = CertificadoValido;

            XElement xmlNota = client.nfeDistDFeInteresse(xNota);

            dataHora = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

            if (VerificarNota(xmlNota) == false)
                return;

            var lote = (from nota in xmlNota.Elements()
                        where nota.Name.LocalName.Equals("loteDistDFeInt")
                        select nota).ToList();

            if (lote == null)
                return;

            var doc = (from loc in lote.Elements()
                       where loc.Name.LocalName.Equals("docZip")
                       select loc).ToList();

            if (doc.Count == 0)
                return;

            var sdata = (from n in xmlNota.Descendants()
                         where n.Name.LocalName.Equals("dhEmi")
                         select n).FirstOrDefault();

            string proNfe = ConvertZip(doc[0].Value);

            XElement element = XElement.Parse(proNfe);

            manifesta.InseriDestinatario(element);
            manifesta.InseriDados(element, "Nfe", dataHora);

            GravarNFe(element.ToString(), keyNota, diretorioNFe);
        }

        private bool VerificarNota(XElement xmlNota)
        {
            var codigo = (from nota in xmlNota.Descendants()
                          where nota.Name.LocalName.Equals("cStat")
                          select nota).FirstOrDefault().Value;

            if (codigo != "138" && codigo != "137")
            {
                var erro = (from nota in xmlNota.Descendants()
                            where nota.Name.LocalName.Equals("xMotivo")
                            select nota).FirstOrDefault().Value;
                logErro.Log(erro.ToString(), true);
                return false;
            }
            return true;
        }

        private void GravarNFe(string nota, string chave, string diretorio)
        {
            StreamWriter file;
            if (!System.IO.File.Exists(diretorio + "\\NFe" + chave + "-nfe.xml"))
            {
                file = new StreamWriter(diretorio + "\\NFe" + chave + "-nfe.xml", false);
                file.WriteLine(nota);
                file.Close();
            }
        }

        private static string ConvertZip(string crypt)
        {
            byte[] buffer = Convert.FromBase64String(crypt);
            byte[] xmlret = Decompress(buffer);
            return Encoding.UTF8.GetString(xmlret);
        }

        private static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new
            MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}
