using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Xml.Linq;
using Scire.NfeDownload;
using System.Windows.Forms;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace Scire.NfeDownload.BLL
{
    class DistBO
    {
        Sapiens.Library.Log.LogError logErro;

        distDFeInt dist = new distDFeInt();
        distDFeInt distNfe = new distDFeInt();
        XSDs.retDistDFeInt ret = new XSDs.retDistDFeInt();
        XmlNode xmlEnvio, xmlRetorno, xmlChave, xmlNota;
        UtilBO util = new UtilBO();
        List<string> notas = new List<string>();
        X509Certificate2 CertificadoValido;
        distDFeIntDistNSU dnsu = new distDFeIntDistNSU();

        private string uf;
        private string cnpj;
        private string razao;
        private string ultNSU;
        private string DiretorioMDFe;
        private string nsu;

        public DateTime DataBusca;
        public int iRegAtual, iRegTotal;
        public string NSUSefaz;

        public string CodRetorno { get; set; }

        // Delegate
        public delegate void AtualizaStatusEventHandler(object sender, System.EventArgs e);
        // Event
        public event AtualizaStatusEventHandler AtualizaStatus;

        private int VezBusca;

        public DistBO()
        {
            this.cnpj = Properties.Settings.Default.Cnpj;
            this.razao = Properties.Settings.Default.Razao;
            this.CertificadoValido = UtilBO.GetCertificado(cnpj, razao);
            this.uf = Properties.Settings.Default.Uf;
            DiretorioMDFe = Properties.Settings.Default.PastaPadrao;

            dist.versao = TVerDistDFe.Item101;
            dist.ItemElementName = ItemChoiceType.CNPJ;
            dist.Item = cnpj;
            dist.cUFAutor = util.EventoUF_IBGE(uf);
            dist.tpAmb = TAmb.Item1;
            dist.cUFAutorSpecified = true;

            distNfe.versao = dist.versao;
            distNfe.ItemElementName = dist.ItemElementName;
            distNfe.Item = dist.Item;
            distNfe.cUFAutor = dist.cUFAutor;
            distNfe.tpAmb = TAmb.Item1;
            distNfe.cUFAutorSpecified = true;           

            this.VezBusca = 1;
        }

        public void BuscarNFe()
        {
            //this.ultNSU =   Properties.Settings.Default.UltimoNSU.ToString().PadLeft(15, '0');
            this.ultNSU = UtilBo.UltimoNSU(VezBusca).ToString().PadLeft(15, '0');
            VezBusca++;

            dnsu.ultNSU = this.ultNSU;

            dist.Item1 = dnsu;

            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            xmlEnvio = UtilBO.SerializaObjeto(dist, typeof(distDFeInt));

            XElement xdoc = XElement.Load(new XmlNodeReader(xmlEnvio));

            NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient client = new NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient();

            client.ClientCredentials.ClientCertificate.Certificate = CertificadoValido;

            XElement xmlRetorno = client.nfeDistDFeInteresse(xdoc);

            if (VerificarNota(xmlRetorno) == false)
                return;

            nsu = (from nota in xmlRetorno.Elements()
                   where nota.Name.LocalName.Equals("ultNSU")
                   select nota).FirstOrDefault().Value;

            var lote = (from nota in xmlRetorno.Elements()
                        where nota.Name.LocalName.Equals("loteDistDFeInt")
                        select nota).ToList();

            var doc = (from loc in lote.Elements()
                       where loc.Name.LocalName.Equals("docZip")
                       select loc).ToList();

            if (doc.Count == 0)
            {
                return;
            }

            this.iRegAtual = 0;
            this.iRegTotal = doc.Count;

            this.NSUSefaz = (from nota in xmlRetorno.Elements()
                             where nota.Name.LocalName.Equals("maxNSU")
                             select nota).FirstOrDefault().Value;

            foreach (var doczip in doc)
            {
                string nsu1 = doczip.FirstAttribute.Value;
                
                iRegAtual++;
                AtualizaStatus(this, new EventArgs());

                XmlNode xml;
                byte[] buffer = Convert.FromBase64String(doczip.Value);
                byte[] xmlret = Decompress(buffer);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Encoding.UTF8.GetString(xmlret));
                xml = xmlDoc.Clone();

                if (xml == null)
                    return;

                if (xml["chNFe"] != null)
                {
                    string prov = xml["chNFe"].InnerText;

                    BuscaCompleta(prov);

                    Thread.Sleep(10000);
                }                
                else
                {
                    XElement procuraChave = XElement.Load(new XmlNodeReader(xml));

                    var prot = (from pro in procuraChave.Elements()
                                where pro.Name.LocalName.Equals("protNFe")
                                select pro).FirstOrDefault();

                    if (prot != null)
                    {

                        var sdata = (from n in procuraChave.Descendants()
                                     where n.Name.LocalName.Equals("dhEmi")
                                     select n).FirstOrDefault();

                        if (sdata != null)
                        {
                            this.DataBusca = DateTime.Parse(sdata.Value);
                            AtualizaStatus(this, new EventArgs());
                        }

                        var inf = (from inp in prot.Elements()
                                   where inp.Name.LocalName.Equals("infProt")
                                   select inp).FirstOrDefault();

                        var chave = (from ch in inf.Elements()
                                     where ch.Name.LocalName.Equals("chNFe")
                                     select ch).FirstOrDefault().Value;

                        GravarNFe(xml.InnerXml, chave);
                    }
                    else
                    {
                        var teste = (from pro in procuraChave.Elements()
                                     where pro.Name.LocalName.Equals("chNFe")
                                     select pro).FirstOrDefault();

                        if (teste != null)
                        {
                            //grava aviso de emissão de nota
                            string zzchave = teste.Value;
                            GravarNFe(xml.InnerXml, zzchave);
                        }
                    }
                }
                Int64 longnsu = 0;
                Int64.TryParse(nsu1, out longnsu);
                UtilBo.AtualizarNSU(longnsu, this.DataBusca);    

            }                            
            
            AtualizaStatus(this, new EventArgs());
        }

        public void BuscaCompleta(string keyNota)
        {
            distDFeIntConsChNFe key = new distDFeIntConsChNFe();
            key.chNFe = keyNota;
            distNfe.Item1 = key;

            xmlChave = UtilBO.SerializaObjeto(distNfe, typeof(distDFeInt));

            XElement xNota = XElement.Load(new XmlNodeReader(xmlChave));

            NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient client = new NfeDistribuicaoServ.NFeDistribuicaoDFeSoapClient();
            
            client.ClientCredentials.ClientCertificate.Certificate = CertificadoValido;

            XElement xmlNota = client.nfeDistDFeInteresse(xNota);

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

            if (sdata != null)
            {
                this.DataBusca = DateTime.Parse(sdata.Value);
                AtualizaStatus(this, new EventArgs());
            }

            GravarNFe(ConvertZip(doc[0].Value), keyNota);
        }

        private bool VerificarNota(XElement xmlNota)
        {
            logErro = new Sapiens.Library.Log.LogError(Application.StartupPath + "\\Log");

            var codigo = (from nota in xmlNota.Elements()
                          where nota.Name.LocalName.Equals("cStat")
                          select nota).FirstOrDefault().Value;

            this.CodRetorno = codigo;

            if (codigo != "138" && codigo != "137")
            {
                var erro = (from nota in xmlNota.Elements()
                            where nota.Name.LocalName.Equals("xMotivo")
                            select nota).FirstOrDefault().Value;
                logErro.Log(string.Format("{0} - {1}", codigo, erro.ToString()), true, false);
                return false;
            }
            else if (codigo == "137")
            {
                this.ultNSU = "137 - Nenhum documento localizado";
                AtualizaStatus(this, new EventArgs());
                return false;
            }
            return true;
        }

        private void GravarNFe(string nota, string chave)
        {
            StreamWriter file;
            if (!System.IO.File.Exists(DiretorioMDFe + "\\NFe" + chave + "-nfe.xml"))
            {
                file = new StreamWriter(DiretorioMDFe + "\\NFe" + chave + "-nfe.xml", false);
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
