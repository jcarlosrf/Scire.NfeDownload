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

namespace Scire.NFeManifestacao
{
    public partial class MainWindow : Window
    {
        TreeViewItem treeItem;

        BLL.DistBO distribui = new BLL.DistBO();
        BLL.ManifestacaoBO manifesta = new BLL.ManifestacaoBO();
        BLL.MdfeBO mdfeBO = new BLL.MdfeBO();
        BLL.Mdfe mdfe;

        Aguarde aguarde = new Aguarde();

        DAL.tipo_evento tipoEvento = new DAL.tipo_evento();
        DAL.nfe nfe;
        DAL.nfe_dados dados;
        DAL.destinatario destinatario = new DAL.destinatario();
        DAL.fornecedor fornecedor;

        int indexDest { get; set; }
        int indexForn { get; set; }

        Sapiens.Library.Log.LogError logErro;

        public MainWindow()
        {
            InitializeComponent();
            manifesta.AtualizaTreeView += new BLL.ManifestacaoBO.AtualizaTreeViewEventHandler(Manifesta_AtualizaTreeView);

            logErro = new Sapiens.Library.Log.LogError(System.Windows.Forms.Application.StartupPath + "\\Log");

            TxtPDataInicio.Text = DateTime.Today.AddDays(-120).ToString("dd/MM/yyyy");
            TxtPDataFinal.Text = DateTime.Today.ToString("dd/MM/yyyy");
            manifesta.dhinicial = (DateTime)TxtPDataInicio.SelectedDate;
            manifesta.dhifnal = (DateTime)TxtPDataFinal.SelectedDate;

            CbxEvento.IsEnabled = false;
            TxtJust.IsEnabled = false;
            BtnManifestar.IsEnabled = false;

            //Properties.Settings.Default.Reset();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                aguarde.Show();
                distribui.BuscarNFeCompleta();
                distribui.BuscarTodasNfe(); 
                manifesta.BuscarNfeFornecedor();
                ImplementaFiltro();
                aguarde.Close();
            }
            catch (Exception exc)
            {
            logErro.Log(exc, true);
                aguarde.Close();
            }
        }

        public void Manifesta_AtualizaTreeView(object sender, EventArgs e)
        {
            try
            {
                treeItem = new TreeViewItem();

                treeItem.Header = manifesta.nfFornecedor[0].razao;

                foreach (BLL.NotaFornecedor item in manifesta.nfFornecedor)
                {
                    treeItem.Items.Add(new TreeViewItem() { Header = item.data + "   " + item.chave });
                }

                TrvFornecedor.Items.Add(treeItem);
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void CbxEvento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (manifesta.listTipoEvento == null)
                    return;

                foreach (DAL.tipo_evento item in manifesta.listTipoEvento)
                {
                    if (CbxEvento.SelectedValue == item.tipo && item.just == true)
                    {
                        TxtJust.IsEnabled = true;
                        BtnManifestar.IsEnabled = false;
                        TxtJust.Focus();
                        return;
                    }
                }
                BtnManifestar.IsEnabled = true;
                TxtJust.IsEnabled = false;
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void TrvFornecedor_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue == null)
                    return;

                nfe = new DAL.nfe();
                dados = new DAL.nfe_dados();
                fornecedor = new DAL.fornecedor();
                destinatario = new DAL.destinatario();
                mdfe = new BLL.Mdfe();

                string chave = ((TreeViewItem)e.NewValue).Header.ToString().Substring(13);
                nfe = manifesta.BuscarNfe(chave);

                if (nfe == null)
                {
                    CbxEvento.IsEnabled = false;
                    BtnManifestar.IsEnabled = false;
                    TxtJust.IsEnabled = false;
                    return;
                }

                dados = manifesta.BuscarDadosByNfe(nfe.id_nfe);

                if (dados == null)
                    return;

                fornecedor = manifesta.BuscarFornecedor(nfe.fornecedor);

                if (fornecedor == null)
                    return;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(dados.xml);
                XmlNode node = doc.Clone();

                XElement element = XElement.Load(new XmlNodeReader(node));

                if (nfe != null)
                {
                    var veriValor = (from valor in element.Elements()
                                     where valor.Name.LocalName.Equals("vNF")
                                     select valor).FirstOrDefault();

                    if (veriValor == null && string.IsNullOrEmpty(nfe.valor_total.ToString()))
                        nfe.valor_total = 0;

                    var procoloco = (from protocolo in element.Elements()
                                     where protocolo.Name.LocalName.Equals("nProt")
                                     select protocolo).FirstOrDefault();

                    //if (procoloco == null)
                    //    return

                    var proto = (from protocolo in element.Descendants()
                                 where protocolo.Name.LocalName.Equals("nProt")
                                 select protocolo).FirstOrDefault().Value;

                    LblChave.Content = nfe.chave;
                    LblCnpjFornecededor.Content = fornecedor.cnpj;
                    LblRazaoFornecedor.Content = fornecedor.razao;
                    LblIE.Content = fornecedor.ie;
                    LblDataEmissao.Content = nfe.data_hora.ToString("dd/MM/yyyy");
                    //TxtRecimento.Content = node.FirstChild["dhRecbto"].InnerText;
                    //TxtTipo.Content = node.FirstChild["tpNF"].InnerText;
                    LblValorTotal.Content = Convert.ToDouble(nfe.valor_total).ToString("C");
                    LblProtocolo.Content = proto;
                    //TxtSituacao.Content = node.FirstChild["cSitNFe"].InnerText;
                    CbxEvento.IsEnabled = true;
                    mdfe.chave = nfe.chave;

                    CbxEvento.Items.Clear();

                    manifesta.ModificarTipoEvento(dados.nfe);
                    ImplementaComboBox();

                    LblCnpjDestinatario.Content = string.Empty;
                    LblRazaoDestinatario.Content = string.Empty;

                    destinatario = manifesta.BuscarDestinatario(nfe.destinatario);
                    if (destinatario != null && destinatario.id_destinatario != 1)
                    {
                        LblCnpjDestinatario.Content = nfe.destinatario;
                        LblRazaoDestinatario.Content = destinatario.razao;
                    }

                    return;
                }
                CbxEvento.IsEnabled = false;
                BtnManifestar.IsEnabled = false;
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void BtnManifestar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DAL.tipo_evento item in manifesta.listTipoEvento)
                {
                    if (CbxEvento.Text == item.tipo)
                        mdfe.eventoSelecionado = item.id_tipo;
                }

                mdfeBO.Manifestar(mdfe.chave, mdfe.justificativa, mdfe.eventoSelecionado.ToString());
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void TxtJust_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtJust.Text))
                {
                    mdfe.justificativa = string.Empty;
                    BtnManifestar.IsEnabled = false;
                }
                else
                {
                    BtnManifestar.IsEnabled = true;
                    mdfe.justificativa = TxtJust.Text;
                }
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void ImplementaComboBox()
        {
            try
            {
                CbxEvento.Items.Insert(0, "Selecione um evento");
                CbxEvento.SelectedIndex = 0;

                if (manifesta.listTipoEvento != null)
                {
                    foreach (DAL.tipo_evento item in manifesta.listTipoEvento)
                    {
                        CbxEvento.Items.Add(item.tipo);
                    }
                }
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                aguarde = new Aguarde();
                aguarde.Show();
                TrvFornecedor.Items.Clear();

                distribui.BuscarNFeCompleta();
                distribui.BuscarTodasNfe();
                manifesta.BuscarNfeFornecedor();
                manifesta.BuscarTipoEvento();
                ImplementaFiltro();
                aguarde.Close();
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
                aguarde.Close();
            }
        }

        #region Filtro
        private void ImplementaFiltro()
        {
            try
            {
                CbxDestinatario.ItemsSource = null;
                CbxDestinatario.Items.Clear();
                manifesta.BuscarTodosDestinatarios();
                CbxDestinatario.ItemsSource = manifesta.listDestinatario;
                CbxDestinatario.SelectedIndex = 0;

                CbxFornecedor.ItemsSource = null;
                CbxFornecedor.Items.Clear();
                manifesta.TodosFornecedor();
                CbxFornecedor.ItemsSource = manifesta.listaFornecedor;
                CbxFornecedor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }

        private void CbxDestinatario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            indexDest = CbxDestinatario.SelectedIndex;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TxtPDataFinal.Text) && !string.IsNullOrEmpty(TxtPDataInicio.Text) &&
                    Convert.ToDateTime(TxtPDataInicio.Text) <= Convert.ToDateTime(TxtPDataFinal.Text))
                {
                    TrvFornecedor.Items.Clear();

                    manifesta.destinatarioCNPJ = CbxDestinatario.SelectedValue.ToString();
                    manifesta.fornecedorCNPJ = CbxFornecedor.SelectedValue.ToString();
                    manifesta.dhinicial = (DateTime)TxtPDataInicio.SelectedDate;
                    manifesta.dhifnal = (DateTime)TxtPDataFinal.SelectedDate;
                    manifesta.BuscarFiltro();
                }
                else
                {
                    MessageBox.Show("Aviso do filtro.");
                }
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }
        #endregion

        private void CbxFornecedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CbxFornecedor.SelectedIndex >= 0)
                    indexForn = CbxFornecedor.SelectedIndex;

            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
        }
    }
}
