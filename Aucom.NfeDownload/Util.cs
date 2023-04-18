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
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Scire.NfeDownload
{
    [ClassInterface(ClassInterfaceType.None)]
    public class UtilBO : Scire.NfeDownload.IUtilBO
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
                if (cert.HasPrivateKey && cert.NotAfter >= DateTime.Now && cert.NotBefore <= DateTime.Now && cert.FriendlyName.Length >= 23)
                {
                    if (cert.FriendlyName.Substring(cert.FriendlyName.Length - 14, 14) == CNPJEmitente || cert.SubjectName.Name.ToUpper().Contains(CNPJEmitente))
                        Certificado = cert;
                    else if (CNPJEmitente.Trim() == "56385834000117" && cert.FriendlyName.Substring(0, 23) == "EUCLIDES RENATO GARBUIO")
                        Certificado = cert;

                    else if (cert.SubjectName.Name.ToUpper().Contains(razao) && cert.NotAfter >= DateTime.Now && cert.NotBefore <= DateTime.Now)
                        Certificado = cert;
                }
            }
            lStore.Close();

            return Certificado;
        }

        public static XmlDocument Assinatura(XmlNode noxml, string sURI, string sTagAssinatura, X509Certificate2 CertificadoValido)
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
            //documento.DocumentElement["MDFe"].AppendChild(xmlSignature);
            documento.DocumentElement.AppendChild(xmlSignature);
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

        public static string UF_IBGE(string UF)
        {
            IDictionary<String, String> UFIbge = new Dictionary<String, String>();
            UFIbge.Add("AC", "12"); //     AC'") ; //     Acre');
            UFIbge.Add("AL", "27"); //     AL'") ; //     Alagoas');
            UFIbge.Add("AM", "13"); //     AM'") ; //     Amazonas');
            UFIbge.Add("AP", "16"); //     AP'") ; //     Amapá');
            UFIbge.Add("BA", "29"); //     BA'") ; //     Bahia');
            UFIbge.Add("CE", "23"); //     CE'") ; //     Ceará');
            UFIbge.Add("DF", "53"); //     DF'") ; //     Distrito Federal');
            UFIbge.Add("ES", "32"); //     ES'") ; //     Espírito Santo');
            UFIbge.Add("GO", "52"); //     GO'") ; //     Goiás');
            UFIbge.Add("MA", "21"); //     MA'") ; //     Maranhão');
            UFIbge.Add("MG", "31"); //     MG'") ; //     Minas Gerais');
            UFIbge.Add("MS", "50"); //     MS'") ; //     Mato Grosso do Sul');
            UFIbge.Add("MT", "51"); //     MT'") ; //     Mato Grosso');
            UFIbge.Add("PA", "15"); //     PA'") ; //     Pará');
            UFIbge.Add("PB", "25"); //     PB'") ; //     Paraíba');
            UFIbge.Add("PE", "26"); //     PE'") ; //     Pernambuco');
            UFIbge.Add("PI", "22"); //     PI'") ; //     Piauí');
            UFIbge.Add("PR", "41"); //     PR'") ; //     Paraná');
            UFIbge.Add("RJ", "33"); //     RJ'") ; //     Rio de Janeiro');
            UFIbge.Add("RN", "24"); //     RN'") ; //     Rio Grande do Norte');
            UFIbge.Add("RO", "11"); //     RO'") ; //     Rondônia');
            UFIbge.Add("RR", "14"); //     RR'") ; //     Roraima');
            UFIbge.Add("RS", "43"); //     RS'") ; //     Rio Grande do Sul');
            UFIbge.Add("SC", "42"); //     SC'") ; //     Santa Catarina');
            UFIbge.Add("SE", "28"); //     SE'") ; //     Sergipe');
            UFIbge.Add("SP", "35"); //     SP'") ; //     São Paulo');
            UFIbge.Add("TO", "17"); //     TO'") ; //     Tocantis');

            return UFIbge[UF];
        }

        public TCodUfIBGE EventoUF_IBGE(string UF)
        {
            IDictionary<String, TCodUfIBGE> UFIbge = new Dictionary<String, TCodUfIBGE>();
            UFIbge.Add("AC", TCodUfIBGE.Item12); //     AC'") ; //     Acre');
            UFIbge.Add("AL", TCodUfIBGE.Item27); //     AL'") ; //     Alagoas');
            UFIbge.Add("AM", TCodUfIBGE.Item13); //     AM'") ; //     Amazonas');
            UFIbge.Add("AP", TCodUfIBGE.Item16); //     AP'") ; //     Amapá');
            UFIbge.Add("BA", TCodUfIBGE.Item29); //     BA'") ; //     Bahia');
            UFIbge.Add("CE", TCodUfIBGE.Item23); //     CE'") ; //     Ceará');
            UFIbge.Add("DF", TCodUfIBGE.Item53); //     DF'") ; //     Distrito Federal');
            UFIbge.Add("ES", TCodUfIBGE.Item32); //     ES'") ; //     Espírito Santo');
            UFIbge.Add("GO", TCodUfIBGE.Item52); //     GO'") ; //     Goiás');
            UFIbge.Add("MA", TCodUfIBGE.Item21); //     MA'") ; //     Maranhão');
            UFIbge.Add("MG", TCodUfIBGE.Item31); //     MG'") ; //     Minas Gerais');
            UFIbge.Add("MS", TCodUfIBGE.Item50); //     MS'") ; //     Mato Grosso do Sul');
            UFIbge.Add("MT", TCodUfIBGE.Item51); //     MT'") ; //     Mato Grosso');
            UFIbge.Add("PA", TCodUfIBGE.Item15); //     PA'") ; //     Pará');
            UFIbge.Add("PB", TCodUfIBGE.Item25); //     PB'") ; //     Paraíba');
            UFIbge.Add("PE", TCodUfIBGE.Item26); //     PE'") ; //     Pernambuco');
            UFIbge.Add("PI", TCodUfIBGE.Item22); //     PI'") ; //     Piauí');
            UFIbge.Add("PR", TCodUfIBGE.Item41); //     PR'") ; //     Paraná');
            UFIbge.Add("RJ", TCodUfIBGE.Item33); //     RJ'") ; //     Rio de Janeiro');
            UFIbge.Add("RN", TCodUfIBGE.Item24); //     RN'") ; //     Rio Grande do Norte');
            UFIbge.Add("RO", TCodUfIBGE.Item11); //     RO'") ; //     Rondônia');
            UFIbge.Add("RR", TCodUfIBGE.Item14); //     RR'") ; //     Roraima');
            UFIbge.Add("RS", TCodUfIBGE.Item43); //     RS'") ; //     Rio Grande do Sul');
            UFIbge.Add("SC", TCodUfIBGE.Item42); //     SC'") ; //     Santa Catarina');
            UFIbge.Add("SE", TCodUfIBGE.Item28); //     SE'") ; //     Sergipe');
            UFIbge.Add("SP", TCodUfIBGE.Item35); //     SP'") ; //     São Paulo');
            UFIbge.Add("TO", TCodUfIBGE.Item17); //     TO'") ; //     Tocantis');

            return UFIbge[UF];
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
