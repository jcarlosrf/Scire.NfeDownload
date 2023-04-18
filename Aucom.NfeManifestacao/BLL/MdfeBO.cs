using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Scire.NFeManifestacao.XSDs.Manifestacao;

namespace Scire.NFeManifestacao.BLL
{
    public class MdfeBO
    {
        Sapiens.Library.Log.LogError logErro;

        private UtilBO util = new UtilBO();
        private TEnvEvento EnvConf = new TEnvEvento();
        private TRetEnvEvento RetEnvEvento = new TRetEnvEvento();
        private X509Certificate2 CertificadoValido;
        private XmlNode xmlEnvio, xmlRetorno;
        private BLL.ManifestacaoBO manifesta = new ManifestacaoBO();
        private BLL.Evento evento;
        private DAL.nfe nfe;
        private DistBO distro = new DistBO();

        private string cnpj { get; set; }
        private string razao { get; set; }
        private string chave { get; set; }
        private string uf { get; set; }
        private string justifica { get; set; }
        private string descEvento { get; set; }
        private string tag { get; set; }

        private string option {get; set;}
        private string tipoEvento { get; set; }
        private string dataHora { get; set; }

        bool erro;
        public string[] sucesso = { "128", "135", "136", "137", "138", "139", "140" };
        string resposta {get; set;}

        public MdfeBO()
        {
            cnpj = Properties.Settings.Default.Cnpj;
            razao = Properties.Settings.Default.Razao;
            uf = Properties.Settings.Default.Uf;
            CertificadoValido = UtilBO.GetCertificado(cnpj, razao);
            tag = "envEvento";

            logErro = new Sapiens.Library.Log.LogError(System.Windows.Forms.Application.StartupPath + "\\Log");
        }

        public void Manifestar(string chave, string justifica, string descEvento)
        {            
            evento = new Evento();
            nfe = new DAL.nfe();
            resposta = null;
            erro = true;
            this.chave = chave;
            this.justifica = justifica;
            this.descEvento = descEvento;
            

            dataHora = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

            EnvConf = MontaClasse();

            xmlEnvio = UtilBO.SerializaObjeto(EnvConf, typeof(TEnvEvento));
            xmlEnvio.InnerXml = xmlEnvio.InnerXml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            //xmlEnvio.InnerXml = xmlEnvio.InnerXml.Replace("xmlns=\"http://www.portalfiscal.inf.br/nfe\"", "");
            xmlEnvio.InnerXml = xmlEnvio.InnerXml.Replace("evento versao=\"1.00\"", "evento versao=\"1.00\" xmlns=\"http://www.portalfiscal.inf.br/nfe\"");

            xmlEnvio = UtilBO.Assinatura(xmlEnvio, tag, "infEvento", this.CertificadoValido);

            RecepcaoEventoServ.NFeRecepcaoEvento4SoapClient client = new RecepcaoEventoServ.NFeRecepcaoEvento4SoapClient();

            client.ClientCredentials.ClientCertificate.Certificate = CertificadoValido;


            xmlRetorno = client.nfeRecepcaoEventoNF(xmlEnvio);

            XElement element = XElement.Load(new XmlNodeReader(xmlRetorno));

            resposta = (from n in element.Descendants()
                        where n.Name.LocalName.Equals("cStat")
                           select n).FirstOrDefault().Value;

            foreach (string vetor in sucesso)
            {
                if (resposta == vetor)
                    erro = false;
            }

            if (erro)
            {
                string motivo = (from n in element.Descendants()
                                 where n.Name.LocalName.Equals("xMotivo")
                            select n).FirstOrDefault().Value;
                logErro.Log(motivo.ToString(), true);
                return;
            }

            manifesta.InseriDados(element, tipoEvento, dataHora);
            distro.BuscarByChave(chave);
        }

        public TEnvEvento MontaClasse()
        {
            tipoEvento = string.Empty;
            TEnvEvento oEnvio = new TEnvEvento();
            oEnvio.versao = "1.00";
            oEnvio.idLote = "0";

            TEvento Evento = new TEvento();
            Evento.versao = "1.00";
            Evento.infEvento = new TEventoInfEvento();

            TEventoInfEventoDetEvento DetEvento = new TEventoInfEventoDetEvento();
            DetEvento.versao = TEventoInfEventoDetEventoVersao.Item100;
            DetEvento.descEvento = TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;

            TEventoInfEvento EventoInf = new TEventoInfEvento();
            EventoInf.cOrgao = TCOrgaoIBGE.Item91;
            EventoInf.tpAmb = TAmb.Item1; //Homologação
            EventoInf.ItemElementName = ItemChoiceType.CNPJ;
            EventoInf.Item = cnpj;
            EventoInf.chNFe = chave;
            EventoInf.dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            EventoInf.tpEvento = util.Evento_DescEvento(descEvento);
            EventoInf.nSeqEvento = "1";
            EventoInf.verEvento = "1.00";
            EventoInf.detEvento = DetEvento;
            EventoInf.Id = string.Format("ID{0}{1}{2}", util.Evento_DescEvento(descEvento).ToString().Substring(4, 6), chave, "01");

            Evento.infEvento = EventoInf;

            tipoEvento = util.Evento_DescEvento(descEvento).ToString();
            if (tipoEvento == "Item210210")
            {
                tipoEvento = "Ciencia";
            }
            else if (tipoEvento == "Item210200")
            {
                tipoEvento = "Confirma";
            }
            else if (tipoEvento == "Item210220")
            {
                tipoEvento = "Desconhece";
            }
            else if (tipoEvento == "Item210240")
            {
                tipoEvento = "NaoRealiza";
            }

            oEnvio.evento = new TEvento[1];
            oEnvio.evento[0] = Evento;


            return oEnvio;
        }
    }
}