using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Scire.NFeManifestacao.DAL;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Globalization;
using System.Windows;

namespace Scire.NFeManifestacao.BLL
{
    public class ManifestacaoBO
    {
        Sapiens.Library.Log.LogError logErro;

        FornecedorDAO fornecedorDao = new FornecedorDAO();
        DestinatarioDAO destDAO = new DestinatarioDAO();
        NfeDAO nfeDao = new NfeDAO();
        NfeDadosDAO dadosDao = new NfeDadosDAO();
        TipoEventoDAO tipoEventoDao = new TipoEventoDAO();

        private List<fornecedor> listFornecedor;
        private List<nfe> listNfe;
        public List<tipo_evento> listTipoEvento;
        public List<destinatario> listDestinatario;

        public List<fornecedor> listaFornecedor;
        private List<fornecedor> listaFornecedorDefault;

        private fornecedor fornecedor;
        private destinatario destinatario;
        private nfe nfe;
        private nfe_dados dados;
        private tipo_evento tipoEvento;

        private int codigoTipo { get; set; }
        private string dataHora { get; set; }

        Dados cdados = new Dados();

        public List<BLL.NotaFornecedor> nfFornecedor = new List<NotaFornecedor>();
        public List<BLL.NotaFornecedor> nfFornecedorOriginal = new List<NotaFornecedor>();

        // Delegate
        public delegate void AtualizaTreeViewEventHandler(object sender, System.EventArgs e);
        // Event
        public event AtualizaTreeViewEventHandler AtualizaTreeView;

        #region propriedades para filtros

        private string fornecedor_ { get; set; }
        private string destinatario_ { get; set; }
        private string dhinicial_ { get; set; }
        private string dhifnal_ { get; set; }

        public string fornecedorCNPJ
        {
            get
            {
                if (string.IsNullOrEmpty(this.fornecedor_))
                    return "00000000000000";

                return this.fornecedor_;
            }
            set
            {
                this.fornecedor_ = value;
            }
        }
        public string destinatarioCNPJ
        {
            get
            {
                if (string.IsNullOrEmpty(this.destinatario_))
                    return "00000000000000";

                return this.destinatario_;
            }
            set
            {
                this.destinatario_ = value;
            }
        }
        public DateTime dhinicial
        {
            get
            {
                if (string.IsNullOrEmpty(this.dhinicial_))
                    return DateTime.Today.AddDays(-120);

                return DateTime.Parse(this.dhinicial_);
            }
            set
            {
                this.dhinicial_ = value.ToString("yyyy-MM-dd");
            }
        }
        public DateTime dhifnal
        {
            get
            {
                if (string.IsNullOrEmpty(this.dhifnal_))
                    return DateTime.Today;

                return DateTime.Parse(this.dhifnal_);
            }
            set
            {
                this.dhifnal_ = value.ToString("yyyy-MM-dd");
            }
        }


        #endregion





        public ManifestacaoBO()
        {
            logErro = new Sapiens.Library.Log.LogError(System.Windows.Forms.Application.StartupPath + "\\Log");
        }




        #region Fornecedor

        private void PreparaFornecedor(XmlNode node)
        {
            cdados = new Dados();
            fornecedor = new DAL.fornecedor();

            if (node.FirstChild["CNPJ"] == null)
            {
                XElement xEmitente = XElement.Load(new XmlNodeReader(node));

                var vemitente = (from loc in xEmitente.Descendants()
                                 where loc.Name.LocalName.Equals("emit")
                                 select loc).FirstOrDefault();

                cdados.emitente = (from n in vemitente.Descendants()
                                   where n.Name.LocalName.Equals("CNPJ")
                                   select n).FirstOrDefault().Value;

                cdados.nome = (from n in vemitente.Descendants()
                               where n.Name.LocalName.Equals("xNome")
                               select n).FirstOrDefault().Value;

                cdados.ie = (from n in vemitente.Descendants()
                             where n.Name.LocalName.Equals("IE")
                             select n).FirstOrDefault().Value;

                var emit = (from n in xEmitente.Descendants()
                            where n.Name.LocalName.Equals("emit")
                            select n).FirstOrDefault();

                if (emit == null)
                    return;

                cdados.logradouro = (from n in emit.Descendants()
                                     where n.Name.LocalName.Equals("xLgr")
                                     select n).FirstOrDefault().Value;

                cdados.numero = (from n in emit.Descendants()
                                 where n.Name.LocalName.Equals("nro")
                                 select n).FirstOrDefault().Value;

                cdados.bairro = (from n in emit.Descendants()
                                 where n.Name.LocalName.Equals("xBairro")
                                 select n).FirstOrDefault().Value;

                cdados.municipio = (from n in emit.Descendants()
                                    where n.Name.LocalName.Equals("xMun")
                                    select n).FirstOrDefault().Value;

                cdados.uf = (from n in emit.Descendants()
                             where n.Name.LocalName.Equals("UF")
                             select n).FirstOrDefault().Value;

                cdados.cep = (from n in emit.Descendants()
                              where n.Name.LocalName.Equals("CEP")
                              select n).FirstOrDefault().Value;

                cdados.pais = (from n in emit.Descendants()
                               where n.Name.LocalName.Equals("xPais")
                               select n).FirstOrDefault().Value;

                var fone = (from n in emit.Descendants()
                            where n.Name.LocalName.Equals("fone")
                            select n).FirstOrDefault();

                if (fone == null)
                {
                    cdados.fone = "";
                }
                else
                {
                    cdados.fone = (from n in emit.Descendants()
                                   where n.Name.LocalName.Equals("fone")
                                   select n).FirstOrDefault().Value;
                }

                cdados.ie = (from n in emit.Descendants()
                             where n.Name.LocalName.Equals("IE")
                             select n).FirstOrDefault().Value;

                var comple = (from n in emit.Descendants()
                              where n.Name.LocalName.Equals("xCpl")
                              select n).FirstOrDefault();

                if (comple == null)
                {
                    cdados.complemento = "";
                }
                else
                {
                    cdados.complemento = (from n in emit.Descendants()
                                          where n.Name.LocalName.Equals("xCpl")
                                          select n).FirstOrDefault().Value;
                }

                var im = (from n in xEmitente.Descendants()
                          where n.Name.LocalName.Equals("IM")
                          select n).FirstOrDefault();

                if (im == null)
                {
                    cdados.im = "";
                }
                else
                {
                    cdados.im = (from n in xEmitente.Descendants()
                                 where n.Name.LocalName.Equals("IM")
                                 select n).FirstOrDefault().Value;
                }

                var cnae = (from n in xEmitente.Descendants()
                            where n.Name.LocalName.Equals("CNAE")
                            select n).FirstOrDefault();

                if (cnae == null)
                {
                    cdados.cnae = "";
                }
                else
                {
                    cdados.cnae = (from n in xEmitente.Descendants()
                                   where n.Name.LocalName.Equals("CNAE")
                                   select n).FirstOrDefault().Value;
                }

                var crt = (from n in xEmitente.Descendants()
                           where n.Name.LocalName.Equals("CRT")
                           select n).FirstOrDefault();

                if (crt == null)
                {
                    cdados.crt = "";
                }
                else
                {
                    cdados.crt = (from n in xEmitente.Descendants()
                                  where n.Name.LocalName.Equals("CRT")
                                  select n).FirstOrDefault().Value;
                }
            }
            else
            {
                cdados.emitente = node.FirstChild["CNPJ"].InnerText;
                cdados.ie = node.FirstChild["IE"].InnerText;
                cdados.nome = node.FirstChild["xNome"].InnerText;
            }
        }

        public void InseriFornecedor(XmlNode node)
        {
            PreparaFornecedor(node);
            fornecedor.cnpj = cdados.emitente;

            fornecedorDao.GetEntidade(ref fornecedor);

            if (fornecedor == null)
            {
                fornecedor = new DAL.fornecedor();
                fornecedor.cnpj = cdados.emitente;
                if (cdados.nome != null && cdados.ie != null)
                {
                    fornecedor.razao = cdados.nome;
                    fornecedor.ie = cdados.ie;
                }
                else
                {
                    fornecedor.razao = "";
                    fornecedor.ie = "";
                }

                fornecedor.logradouro = cdados.logradouro;
                fornecedor.numero = cdados.numero;
                fornecedor.complemento = cdados.complemento;
                fornecedor.bairro = cdados.bairro;
                fornecedor.municipio = cdados.municipio;
                fornecedor.uf = cdados.uf;
                fornecedor.cep = cdados.cep;
                fornecedor.pais = cdados.pais;
                fornecedor.fone = cdados.fone;
                fornecedor.ie = cdados.ie;
                fornecedor.im = cdados.im;
                fornecedor.cnae = cdados.cnae;
                fornecedor.crt = cdados.crt;

                if (fornecedorDao.Insert(fornecedor) == 0)
                    return; //Log
            }
            else
            {
                if (string.IsNullOrEmpty(fornecedor.logradouro) || string.IsNullOrEmpty(fornecedor.municipio)
                    || string.IsNullOrEmpty(fornecedor.cep))
                {
                    fornecedor forn = fornecedor;

                    fornecedor.logradouro = cdados.logradouro;
                    fornecedor.numero = cdados.numero;
                    fornecedor.complemento = cdados.complemento;
                    fornecedor.bairro = cdados.bairro;
                    fornecedor.municipio = cdados.municipio;
                    fornecedor.uf = cdados.uf;
                    fornecedor.cep = cdados.cep;
                    fornecedor.pais = cdados.pais;
                    fornecedor.fone = cdados.fone;
                    fornecedor.ie = cdados.ie;
                    fornecedor.im = cdados.im;
                    fornecedor.cnae = cdados.cnae;
                    fornecedor.crt = cdados.crt;

                    if (fornecedorDao.Update(fornecedor) == 0)
                        return;
                }
            }
        }

        public fornecedor BuscarFornecedor(string cnpj)
        {
            fornecedor = new DAL.fornecedor();
            fornecedor.cnpj = cnpj;
            fornecedorDao.GetEntidade(ref fornecedor);
            return fornecedor;
        }

        public fornecedor BuscarFornecedor(int id)
        {
            fornecedor = new DAL.fornecedor();
            fornecedor.id_fornecedor = id;
            fornecedorDao.GetEntidade(ref fornecedor);
            return fornecedor;
        }

        public void TodosFornecedor()
        {
            listaFornecedor = fornecedorDao.GetAll().Where(d => d.cnpj != "00000000000000").OrderBy(f => f.razao).ToList();
            listaFornecedor.Insert(0, new fornecedor { cnpj = "00000000000000", razao = "TODOS" });
        }

        #endregion




        #region FornecedorNFe
        public void BuscarNfeFornecedor()
        {
            listFornecedor = new List<DAL.fornecedor>();
            listNfe = new List<DAL.nfe>();

            listNfe = nfeDao.GetAll();
            listFornecedor = fornecedorDao.GetAll().OrderBy(f => f.razao).ToList();

            if (listFornecedor == null)
                return;

            if (listNfe == null)
                return;

            nfFornecedorOriginal = new List<NotaFornecedor>();

            foreach (DAL.fornecedor item in listFornecedor)
            {
                nfFornecedor = (from nf in listNfe
                                        join fo in listFornecedor on nf.fornecedor equals fo.id_fornecedor
                                        where
                                        (Convert.ToDateTime(nf.data_hora).Date >= this.dhinicial && Convert.ToDateTime(nf.data_hora).Date <= this.dhifnal) &&
                                        fo.cnpj == item.cnpj
                                        orderby fo.razao
                                        select new BLL.NotaFornecedor()
                                        {
                                            chave = nf.chave,
                                            cnpj = fo.cnpj,
                                            razao = fo.razao,
                                            data = nf.data_hora.ToString("dd/MM/yyyy"),
                                            cnpjdestinatario = nf.destinatario
                                        }).ToList();

                if (nfFornecedor.Count > 0)
                {
                    AtualizaTreeView(this, new EventArgs());
                    nfFornecedorOriginal.AddRange(nfFornecedor);
                }
            }
        }
        #endregion

        #region Destinatario
        private void PreparaDestinatario(XElement element)
        {
            destinatario = new destinatario();
            cdados = new Dados();

            var dest = (from nota in element.Descendants()
                        where nota.Name.LocalName.Equals("dest")
                        select nota).FirstOrDefault();

            cdados.cnpj = (from n in dest.Descendants()
                           where n.Name.LocalName.Equals("CNPJ")
                           select n).FirstOrDefault().Value;

            cdados.nome = (from n in dest.Descendants()
                           where n.Name.LocalName.Equals("xNome")
                           select n).FirstOrDefault().Value;

            cdados.logradouro = (from n in dest.Descendants()
                                 where n.Name.LocalName.Equals("xLgr")
                                 select n).FirstOrDefault().Value;

            cdados.numero = (from n in dest.Descendants()
                             where n.Name.LocalName.Equals("nro")
                             select n).FirstOrDefault().Value;

            cdados.bairro = (from n in dest.Descendants()
                             where n.Name.LocalName.Equals("xBairro")
                             select n).FirstOrDefault().Value;

            cdados.municipio = (from n in dest.Descendants()
                                where n.Name.LocalName.Equals("xMun")
                                select n).FirstOrDefault().Value;

            cdados.uf = (from n in dest.Descendants()
                         where n.Name.LocalName.Equals("UF")
                         select n).FirstOrDefault().Value;

            cdados.cep = (from n in dest.Descendants()
                          where n.Name.LocalName.Equals("CEP")
                          select n).FirstOrDefault().Value;

            cdados.pais = (from n in dest.Descendants()
                           where n.Name.LocalName.Equals("xPais")
                           select n).FirstOrDefault().Value;

            var fone = (from n in dest.Descendants()
                        where n.Name.LocalName.Equals("fone")
                        select n).FirstOrDefault();

            if (fone == null)
            {
                cdados.fone = "";
            }
            else
            {
                cdados.fone = (from n in dest.Descendants()
                               where n.Name.LocalName.Equals("fone")
                               select n).FirstOrDefault().Value;
            }

            cdados.ie = (from n in dest.Descendants()
                         where n.Name.LocalName.Equals("IE")
                         select n).FirstOrDefault().Value;

            var comple = (from n in dest.Descendants()
                          where n.Name.LocalName.Equals("xCpl")
                          select n).FirstOrDefault();

            if (comple == null)
            {
                cdados.complemento = "";
            }
            else
            {
                cdados.complemento = (from n in dest.Descendants()
                                      where n.Name.LocalName.Equals("xCpl")
                                      select n).FirstOrDefault().Value;
            }
        }

        public void InseriDestinatario(XElement element)
        {
            PreparaDestinatario(element);

            BuscarDestinatario(cdados.cnpj);

            if (destinatario == null)
            {
                destinatario = new destinatario();

                destinatario.cnpj = cdados.cnpj;
                destinatario.razao = cdados.nome;
                destinatario.logradouro = cdados.logradouro;
                destinatario.numero = cdados.numero;
                destinatario.bairro = cdados.bairro;
                destinatario.municipio = cdados.municipio;
                destinatario.uf = cdados.uf;
                destinatario.cep = cdados.cep;
                destinatario.pais = cdados.pais;
                destinatario.fone = cdados.fone;
                destinatario.ie = cdados.ie;
                destinatario.complemento = cdados.complemento;

                if (destDAO.Insert(destinatario) == 0)
                    return;
            }
            else
            {
                DAL.destinatario desti = destinatario;

                destinatario.logradouro = cdados.logradouro;
                destinatario.numero = cdados.numero;
                destinatario.bairro = cdados.bairro;
                destinatario.municipio = cdados.municipio;
                destinatario.uf = cdados.uf;
                destinatario.cep = cdados.cep;
                destinatario.pais = cdados.pais;
                destinatario.fone = cdados.fone;
                destinatario.ie = cdados.ie;
                destinatario.complemento = cdados.complemento;

                if (destDAO.Update(destinatario) == 0)
                    return;
            }
        }

        public destinatario BuscarDestinatario(string cnpj)
        {
            destinatario = new destinatario();
            destinatario.cnpj = cnpj;
            destDAO.GetEntidade(ref destinatario);
            return destinatario;
        }

        public void BuscarTodosDestinatarios()
        {
            listDestinatario = destDAO.GetAll().Where(d => d.cnpj != "00000000000000").OrderBy(d => d.razao).ToList();
            listDestinatario.Insert(0, new destinatario { cnpj = "00000000000000", razao = "TODOS" });
        }
        #endregion

        #region DadosNfe
        private void PreparaDados(XElement element)
        {
            cdados = new Dados();
            dados = new nfe_dados();

            cdados.protocolo = (from n in element.Descendants()
                                where n.Name.LocalName.Equals("nProt")
                                select n).FirstOrDefault().Value;

            cdados.chave = (from n in element.Descendants()
                            where n.Name.LocalName.Equals("chNFe")
                            select n).FirstOrDefault().Value;

            bool hasCity = element.Elements("City").Any();

            cdados.dataHoraSefaz = (from n in element.Descendants()
                                    where n.Name.LocalName.Equals("dhEmi")
                                    select n).FirstOrDefault().Value;
        }

        public void InseriDados(XElement element, string op, string dataHora)
        {
            PreparaDados(element);
            configuraDados(op);

            BuscarNfe(cdados.chave);

            dados.protocolo = cdados.protocolo;
            dados.nfe = nfe.id_nfe;

            dados = dadosDao.BuscaByTipo(dados);

            if (dados == null)
            {
                dados = new nfe_dados();
                configuraDados(op);
                dados.nfe = nfe.id_nfe;
                dados.protocolo = cdados.protocolo;
                dados.data_sistema = Convert.ToDateTime(dataHora);
                dados.data_sefaz = Convert.ToDateTime(cdados.dataHoraSefaz);

                if (string.IsNullOrEmpty(dados.codigo))
                    dados.codigo = "";

                dados.xml = element.ToString();
                dados.xml = RetirarEspacos(dados.xml);

                if (op == "Nfe")
                    dados.codigo = "210";

                if (dadosDao.Insert(dados) == 0)
                    return; //Log
            }
        }

        public nfe_dados BuscarDados(XmlNode node)
        {
            dados = new nfe_dados();
            BuscarNfe(node);

            if (nfe == null)
                return null; //Log

            dados.nfe = nfe.id_nfe;

            dadosDao.GetEntidade(ref dados);
            return dados;
        }

        public nfe_dados BuscarDadosByNfe(int id_nfe)
        {
            return dados = dadosDao.BuscarDadosByNfe(id_nfe);
        }

        //0 para nenhum código
        //1 para 210
        //2 para 200, 220 ou 240
        //3 para 210 + 200, 220 ou 240

        private void VerificarCodigoById(int id_nfe)
        {
            codigoTipo = 0;
            List<nfe_dados> dados = new List<nfe_dados>();
            dados = dadosDao.BuscarCodigo(id_nfe);

            foreach (nfe_dados item in dados)
            {
                if (string.IsNullOrEmpty(item.codigo))
                {

                }
                else if (item.codigo == "210")
                    codigoTipo += 1;
                else if (item.codigo == "200" || item.codigo == "220" || item.codigo == "240")
                    codigoTipo += 2;
            }
        }

        public void configuraDados(string option)
        {
            dados.operacao = "Manif";
            if (option == "Ciencia")
            {
                dados.tipo = "Ciencia";
                dados.codigo = "210";
            }
            else if (option == "Confirma")
            {
                dados.tipo = "Confirma";
                dados.codigo = "200";
            }
            else if (option == "Desconhece")
            {
                dados.tipo = "Desconhece";
                dados.codigo = "220";
            }
            else if (option == "NaoRealiza")
            {
                dados.tipo = "NaoRealiza";
                dados.codigo = "240";
            }
            else if (option == "Resumo")
            {
                dados.tipo = "Resumo";
                dados.operacao = "Dist";
            }
            else if (option == "Nfe")
            {
                dados.tipo = "Nfe";
                dados.operacao = "Dist";
            }
        }
        #endregion

        #region Nfe
        private void PreparaNfe(XmlNode node)
        {
            cdados = new Dados();
            nfe = new nfe();

            XElement xNfe = XElement.Load(new XmlNodeReader(node));

            cdados.cnpj = (from n in xNfe.Descendants()
                           where n.Name.LocalName.Equals("CNPJ")
                           select n).FirstOrDefault().Value;

            XElement valor = XElement.Load(new XmlNodeReader(node));

            string total = (from n in xNfe.Descendants()
                            where n.Name.LocalName.Equals("vNF")
                            select n).FirstOrDefault().Value;

            cdados.valor = double.Parse(total, CultureInfo.InvariantCulture);


            cdados.cnpj = (from n in xNfe.Descendants()
                           where n.Name.LocalName.Equals("CNPJ")
                           select n).FirstOrDefault().Value;

            cdados.protocolo = (from n in xNfe.Descendants()
                                where n.Name.LocalName.Equals("nProt")
                                select n).FirstOrDefault().Value;

            BuscarFornecedor(cdados.cnpj);

            if (fornecedor == null)
                return; //Log

            cdados.chave = (from n in xNfe.Descendants()
                            where n.Name.LocalName.Equals("chNFe")
                            select n).FirstOrDefault().Value;

            cdados.dataHoraSefaz = (from n in xNfe.Descendants()
                                    where n.Name.LocalName.Equals("dhEmi")
                                    select n).FirstOrDefault().Value;

            var dest = (from nota in xNfe.Descendants()
                        where nota.Name.LocalName.Equals("dest")
                        select nota).FirstOrDefault();

            if (dest == null)
                return;

            cdados.destinatario = (from n in dest.Descendants()
                                   where n.Name.LocalName.Equals("CNPJ")
                                   select n).FirstOrDefault().Value;
        }

        //public void InseriNfe(XmlNode node, bool isNfe, string operacao, string tipo, string codigo)
        public void InseriNfe(XmlNode node, bool isNfe)
        {
            dataHora = DateTime.Now.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

            PreparaNfe(node);
            nfe.chave = cdados.chave;
            nfeDao.GetEntidade(ref nfe);

            if (nfe == null)
            {
                nfe = new DAL.nfe();

                nfe.chave = cdados.chave;
                nfe.fornecedor = fornecedor.id_fornecedor;
                nfe.data_hora = Convert.ToDateTime(cdados.dataHoraSefaz);
                nfe.valor_total = cdados.valor;

                if (isNfe)
                {
                    nfe.xml_nfe = RetirarEspacos(node.InnerXml);
                    nfe.destinatario = cdados.destinatario;
                }
                else
                {
                    nfe.xml_nfe = "";
                    nfe.destinatario = "00000000000000";
                }

                if (nfeDao.Insert(nfe) == 0)
                    return; //log
            }
            else if (nfe.xml_nfe == "" || nfe.destinatario == "")
            {
                nfe.fornecedor = fornecedor.id_fornecedor;
                DAL.nfe nf = nfe;

                //if (node.FirstChild["vNF"] != null)
                //nfe.valor_total = double.Parse(node.FirstChild["vNF"].InnerText, CultureInfo.InvariantCulture);

                if (isNfe && nfe.xml_nfe == "")
                    nfe.xml_nfe = RetirarEspacos(node.InnerXml);

                if (nfe.destinatario == "00000000000000")
                    nfe.destinatario = cdados.destinatario;

                if (nfeDao.Update(nfe) == 0)
                    return; //log
            }

            XElement element = XElement.Load(new XmlNodeReader(node));

            nfe = BuscarNfe(nfe.chave);
            if (!isNfe)
                InseriDados(element, "Resumo", dataHora);
            else
                InseriDados(element, "Nfe", dataHora);
        }

        public nfe BuscarNfe(XmlNode node)
        {
            nfe = new nfe();
            nfe.chave = node.FirstChild["chNFe"].InnerText;
            nfeDao.GetEntidade(ref nfe);
            return nfe;
        }

        public nfe BuscarNfe(string chave)
        {
            nfe = new nfe();
            nfe.chave = chave;
            nfeDao.GetEntidade(ref nfe);
            return nfe;
        }
        #endregion

        #region Tipo Evento
        public void BuscarTipoEvento()
        {
            listTipoEvento = new List<tipo_evento>();
            listTipoEvento = tipoEventoDao.GetAll();
        }

        private void BuscarTipoEventoSemCiencia()
        {
            listTipoEvento = new List<tipo_evento>();
            listTipoEvento = tipoEventoDao.BuscaTipoEvento210();
        }

        private void BuscarTipoEventoUmaOcpcao()
        {
            listTipoEvento = new List<tipo_evento>();

            tipoEvento = new tipo_evento();
            tipoEvento.id_tipo = 210210;

            tipoEventoDao.GetEntidade(ref tipoEvento);

            if (tipoEvento != null)
                listTipoEvento.Add(tipoEvento);
        }
        #endregion

        #region Filtro

        public void BuscarFiltro()
        {

            List<fornecedor> listFornecedorFiltrado = (from f in listFornecedor
                                                       where
                                                       (f.cnpj == this.fornecedorCNPJ || this.fornecedorCNPJ == "00000000000000")
                                                       select f).ToList();

            foreach (DAL.fornecedor item in listFornecedorFiltrado)
            {
                nfFornecedor = (from nf in nfFornecedorOriginal
                                where
                                    (DateTime.Parse(nf.data) >= this.dhinicial && DateTime.Parse(nf.data) <= this.dhifnal)
                                    &&
                                    (nf.cnpj == item.cnpj) 
                                    &&
                                    (nf.cnpjdestinatario == this.destinatarioCNPJ || this.destinatarioCNPJ == "00000000000000")
                                select nf
                                   ).ToList();


                if (nfFornecedor.Count > 0)
                    AtualizaTreeView(this, new EventArgs());
            }   
        }


        #endregion

        public void ModificarTipoEvento(int id_nfe)
        {
            VerificarCodigoById(id_nfe);

            if (codigoTipo == 0)
            {
                BuscarTipoEventoUmaOcpcao();
            }
            else if (codigoTipo == 1)
            {
                BuscarTipoEventoSemCiencia();
            }
            else if (codigoTipo >= 2)
            {
                listTipoEvento = null;
            }
        }

        private string RetirarEspacos(string texto)
        {
            texto = texto.Replace("\r\n", "");

            while (texto.IndexOf("> ") > 0)
            {
                texto = texto.Replace("> ", ">");
            }

            return texto;
        }
    }
}
