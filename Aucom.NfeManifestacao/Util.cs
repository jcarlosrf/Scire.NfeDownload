using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Scire.NFeManifestacao.XSDs.Manifestacao;
using Scire.NFeManifestacao.XSDs.Download;

namespace Scire.NFeManifestacao.BLL
{
    [ClassInterface(ClassInterfaceType.None)]
    public class UtilBO : Scire.NFeManifestacao.IUtilBO
    {
        #region XML - Objetos

        /// <summary>
        /// Serializa um objeto das classes do webserver
        /// TRansforma esse objeto em um XML 
        /// </summary>
        /// <param name="objXML">Estância do objeto</param>
        /// <param name="tipo">Tipo "getType"</param>
        /// <returns>XML serializado</returns>
        public static XmlNode SerializaObjeto(object objXML, Type tipo)
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            XmlDocument xmldoc = new XmlDocument();

            XmlSerializer serxml = new XmlSerializer(tipo);
            serxml.Serialize(writer, objXML);
            stream.Position = 0;
            xmldoc.Load(stream);

            return xmldoc;
        }

        /// <summary>
        /// Desserialziar um XML retornado do webservice
        /// TRansforma um xml retorno em objeto serializavel
        /// </summary>
        /// <param name="XmlObjeto">XML retorno</param>
        /// <param name="tipo">tipo do objeto</param>
        /// <returns>Objeto generico que será tipado</returns>
        public static object DesserializarObjeto(XmlNode XmlObjeto, Type tipo)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + XmlObjeto.OuterXml);
            MemoryStream stream1 = new MemoryStream(byteArray);

            XmlSerializer serxml = new XmlSerializer(tipo);

            object objeto1 = serxml.Deserialize(stream1);

            return objeto1;
        }

        /// <summary>
        /// Desserialziar um XML retornado do webservice
        /// TRansforma um xml retorno em objeto serializavel
        /// </summary>
        /// <param name="XmlCompleto">XML completo</param>
        /// <param name="tipo">tipo do objeto</param>
        /// <returns>Objeto generico que será tipado</returns>
        public static object DesserializarObjeto(string XmlCompleto, Type tipo)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(XmlCompleto);
            MemoryStream stream1 = new MemoryStream(byteArray);

            XmlSerializer serxml = new XmlSerializer(tipo);

            object objeto1 = serxml.Deserialize(stream1);

            return objeto1;
        }

        /// <summary>
        /// Converte XML para estrutura de tabelas - DataSet
        /// </summary>
        /// <param name="XmlObjeto"></param>
        /// <returns></returns>
        public static DataSet XMLparaDataSet(XmlNode XmlObjeto)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(XmlObjeto.OuterXml);
            MemoryStream stream1 = new MemoryStream(byteArray);

            DataSet dsNovo = new DataSet();
            dsNovo.ReadXml(stream1);

            return dsNovo;
        }

        /// <summary>
        /// Converte XML para estrutura de tabelas - DataSet
        /// </summary>
        /// <param name="xmlCompleto"></param>
        /// <returns></returns>
        public static DataSet XMLparaDataSet(string xmlCompleto)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(xmlCompleto);
            MemoryStream stream1 = new MemoryStream(byteArray);

            DataSet dsNovo = new DataSet();
            dsNovo.ReadXml(stream1);

            return dsNovo;
        }

        #endregion

        #region Assinaturas - CErtificados
        /// <summary>
        /// Busca certificados validos para emissão da MdFe
        /// </summary>
        /// <returns>DAdos completos do certificado pesquisado</returns>
        public static X509Certificate2 GetCertificado(string CNPJEmitente, string razao)
        {
            X509Certificate2Collection lcerts;
            X509Store lStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            X509Certificate2 Certificado = new X509Certificate2();

            // Abre o Store
            lStore.Open(OpenFlags.MaxAllowed);

            // Lista os certificados
            lcerts = lStore.Certificates;

            foreach (X509Certificate2 cert in lcerts)
            {
                if (cert.HasPrivateKey && cert.NotAfter >= DateTime.Now && cert.NotBefore <= DateTime.Now && cert.FriendlyName.Length >= 14)
                {
                    if (cert.FriendlyName.Substring(cert.FriendlyName.Length - 14, 14) == CNPJEmitente)
                        Certificado = cert;
                    else if (CNPJEmitente.Trim() == "56385834000117" && cert.FriendlyName.Substring(0, 23) == "EUCLIDES RENATO GARBUIO")
                        Certificado = cert;
                }
                else if (cert.SubjectName.Name.ToUpper().Contains(razao) && cert.NotAfter >= DateTime.Now && cert.NotBefore <= DateTime.Now)
                    Certificado = cert;
            }
            lStore.Close();

            return Certificado;
        }

        public static XmlDocument Assinatura(XmlNode noxml, string tag, string sTagAssinatura, X509Certificate2 CertificadoValido)
        {
            XmlDocument documento = new XmlDocument();
            documento.LoadXml(noxml.InnerXml);

            XmlElement xmlInfMDFe = (XmlElement)documento.GetElementsByTagName(sTagAssinatura).Item(0);

            // --------------------- assinando ------------------------------------------------------------------------
            string sId = xmlInfMDFe.Attributes.GetNamedItem("Id").Value;
            SignedXml docXML = new SignedXml(xmlInfMDFe);
            docXML.SigningKey = CertificadoValido.PrivateKey;

            Reference reference = new Reference("#" + sId);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());

            docXML.AddReference(reference);

            // carrega o certificado em KeyInfoX509Data para adicionar a KeyInfo
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(CertificadoValido));
            docXML.KeyInfo = keyInfo;
            docXML.ComputeSignature();

            XmlElement xmlSignature = documento.CreateElement("Signature", "http://www.w3.org/2000/09/xmldsig#");
            XmlElement xmlSignedInfo = docXML.SignedInfo.GetXml();
            XmlElement xmlKeyInfo = docXML.KeyInfo.GetXml();
            XmlElement xmlSignatureValue = documento.CreateElement("SignatureValue", xmlSignature.NamespaceURI);
            string signBase64 = Convert.ToBase64String(docXML.Signature.SignatureValue);
            XmlText text = documento.CreateTextNode(signBase64);
            xmlSignatureValue.AppendChild(text);

            xmlSignature.AppendChild(documento.ImportNode(xmlSignedInfo, true));
            xmlSignature.AppendChild(xmlSignatureValue);
            xmlSignature.AppendChild(documento.ImportNode(xmlKeyInfo, true));


            //documento.DocumentElement["MDFe"].AppendChild(documento.ImportNode(xmlDigitalSignature, true));
            documento.DocumentElement["evento"].AppendChild(xmlSignature);
            //documento.DocumentElement.AppendChild(xmlSignature);
            return documento;
        }

        /// <summary>
        /// Calcula digito verificador com Modulo 11
        /// </summary>
        /// <param name="strText">String com codigo</param>
        /// <returns>Retorna digito</returns>
        public static int DigitoModulo11(string strText)
        {
            int[] intPesos = { 2, 3, 4, 5, 6, 7, 8, 9 };

            int intSoma = 0;
            int intIdx = 0;

            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
                if (intIdx == 8)
                    intIdx = 0;
            }
            int intResto = intSoma % 11;
            int intdigito = 11 - intResto;

            if (intdigito >= 10)
                intdigito = 0;

            return intdigito;
        }

        #endregion

        #region TiposPadroes

        public Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE EventoUF_IBGEDownload(string UF)
        {
            IDictionary<String, Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE> UFIbge = new Dictionary<String, Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE>();
            UFIbge.Add("AC", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item12); //     AC'") ; //     Acre');
            UFIbge.Add("AL", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item27); //     AL'") ; //     Alagoas');
            UFIbge.Add("AM", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item13); //     AM'") ; //     Amazonas');
            UFIbge.Add("AP", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item16); //     AP'") ; //     Amapá');
            UFIbge.Add("BA", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item29); //     BA'") ; //     Bahia');
            UFIbge.Add("CE", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item23); //     CE'") ; //     Ceará');
            UFIbge.Add("DF", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item53); //     DF'") ; //     Distrito Federal');
            UFIbge.Add("ES", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item32); //     ES'") ; //     Espírito Santo');
            UFIbge.Add("GO", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item52); //     GO'") ; //     Goiás');
            UFIbge.Add("MA", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item21); //     MA'") ; //     Maranhão');
            UFIbge.Add("MG", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item31); //     MG'") ; //     Minas Gerais');
            UFIbge.Add("MS", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item50); //     MS'") ; //     Mato Grosso do Sul');
            UFIbge.Add("MT", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item51); //     MT'") ; //     Mato Grosso');
            UFIbge.Add("PA", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item15); //     PA'") ; //     Pará');
            UFIbge.Add("PB", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item25); //     PB'") ; //     Paraíba');
            UFIbge.Add("PE", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item26); //     PE'") ; //     Pernambuco');
            UFIbge.Add("PI", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item22); //     PI'") ; //     Piauí');
            UFIbge.Add("PR", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item41); //     PR'") ; //     Paraná');
            UFIbge.Add("RJ", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item33); //     RJ'") ; //     Rio de Janeiro');
            UFIbge.Add("RN", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item24); //     RN'") ; //     Rio Grande do Norte');
            UFIbge.Add("RO", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item11); //     RO'") ; //     Rondônia');
            UFIbge.Add("RR", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item14); //     RR'") ; //     Roraima');
            UFIbge.Add("RS", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item43); //     RS'") ; //     Rio Grande do Sul');
            UFIbge.Add("SC", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item42); //     SC'") ; //     Santa Catarina');
            UFIbge.Add("SE", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item28); //     SE'") ; //     Sergipe');
            UFIbge.Add("SP", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item35); //     SP'") ; //     São Paulo');
            UFIbge.Add("TO", Scire.NFeManifestacao.XSDs.Download.Envio.TCodUfIBGE.Item17); //     TO'") ; //     Tocantis');

            return UFIbge[UF];
        }

        public TCOrgaoIBGE EventoUF_IBGE(string UF)
        {
            IDictionary<String, TCOrgaoIBGE> UFIbge = new Dictionary<String, TCOrgaoIBGE>();
            UFIbge.Add("AC", TCOrgaoIBGE.Item12); //     AC'") ; //     Acre');
            UFIbge.Add("AL", TCOrgaoIBGE.Item27); //     AL'") ; //     Alagoas');
            UFIbge.Add("AM", TCOrgaoIBGE.Item13); //     AM'") ; //     Amazonas');
            UFIbge.Add("AP", TCOrgaoIBGE.Item16); //     AP'") ; //     Amapá');
            UFIbge.Add("BA", TCOrgaoIBGE.Item29); //     BA'") ; //     Bahia');
            UFIbge.Add("CE", TCOrgaoIBGE.Item23); //     CE'") ; //     Ceará');
            UFIbge.Add("DF", TCOrgaoIBGE.Item53); //     DF'") ; //     Distrito Federal');
            UFIbge.Add("ES", TCOrgaoIBGE.Item32); //     ES'") ; //     Espírito Santo');
            UFIbge.Add("GO", TCOrgaoIBGE.Item52); //     GO'") ; //     Goiás');
            UFIbge.Add("MA", TCOrgaoIBGE.Item21); //     MA'") ; //     Maranhão');
            UFIbge.Add("MG", TCOrgaoIBGE.Item31); //     MG'") ; //     Minas Gerais');
            UFIbge.Add("MS", TCOrgaoIBGE.Item50); //     MS'") ; //     Mato Grosso do Sul');
            UFIbge.Add("MT", TCOrgaoIBGE.Item51); //     MT'") ; //     Mato Grosso');
            UFIbge.Add("PA", TCOrgaoIBGE.Item15); //     PA'") ; //     Pará');
            UFIbge.Add("PB", TCOrgaoIBGE.Item25); //     PB'") ; //     Paraíba');
            UFIbge.Add("PE", TCOrgaoIBGE.Item26); //     PE'") ; //     Pernambuco');
            UFIbge.Add("PI", TCOrgaoIBGE.Item22); //     PI'") ; //     Piauí');
            UFIbge.Add("PR", TCOrgaoIBGE.Item41); //     PR'") ; //     Paraná');
            UFIbge.Add("RJ", TCOrgaoIBGE.Item33); //     RJ'") ; //     Rio de Janeiro');
            UFIbge.Add("RN", TCOrgaoIBGE.Item24); //     RN'") ; //     Rio Grande do Norte');
            UFIbge.Add("RO", TCOrgaoIBGE.Item11); //     RO'") ; //     Rondônia');
            UFIbge.Add("RR", TCOrgaoIBGE.Item14); //     RR'") ; //     Roraima');
            UFIbge.Add("RS", TCOrgaoIBGE.Item43); //     RS'") ; //     Rio Grande do Sul');
            UFIbge.Add("SC", TCOrgaoIBGE.Item42); //     SC'") ; //     Santa Catarina');
            UFIbge.Add("SE", TCOrgaoIBGE.Item28); //     SE'") ; //     Sergipe');
            UFIbge.Add("SP", TCOrgaoIBGE.Item35); //     SP'") ; //     São Paulo');
            UFIbge.Add("TO", TCOrgaoIBGE.Item17); //     TO'") ; //     Tocantis');
            UFIbge.Add("AN", TCOrgaoIBGE.Item91); //     AN'") ; //     Nacional);

            return UFIbge[UF];
        }

        public TEventoInfEventoTpEvento Evento_DescEvento(string evento)
        {
            IDictionary<String, TEventoInfEventoTpEvento> DescEvento = new Dictionary<String, TEventoInfEventoTpEvento>();

            DescEvento.Add("210200", TEventoInfEventoTpEvento.Item210200); //     0'") ; //     Confirmacao da Operacao');
            DescEvento.Add("210210", TEventoInfEventoTpEvento.Item210210); //     1'") ; //     Ciencia da Operacao');
            DescEvento.Add("210220", TEventoInfEventoTpEvento.Item210220); //     2'") ; //     Desconhecimento da Operacao');
            DescEvento.Add("210240", TEventoInfEventoTpEvento.Item210240); //     3'") ; //     Operacao nao Realizada');

            return DescEvento[evento];
        }

        #endregion

        public string PeriodoSemana(DateTime data)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int semana = ciCurr.Calendar.GetWeekOfYear(data, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday);

            return semana.ToString().PadLeft(2, '0');
        }



        public static string PrimeiraPalavra(string frase)
        {
            int tamanho = frase.IndexOf(' ');
            if (tamanho == 0)
                tamanho = frase.Length;
            else
                tamanho = tamanho + 1;

            string palavra = frase.Substring(0, tamanho);

            return palavra;
        }
    }



}
