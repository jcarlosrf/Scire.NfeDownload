using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Scire.NfeDownload.BLL;

namespace Scire.NfeDownload
{
    public partial class frmNfeDownload : Form
    {
        DistBO distribui = new DistBO();

        Sapiens.Library.Log.LogError logErro;

        public frmNfeDownload()
        {
            InitializeComponent();
            distribui.AtualizaStatus += new DistBO.AtualizaStatusEventHandler(distribui_AtualizaStatus);
            TimerEspera.Enabled = true;
            
            if (!System.IO.Directory.Exists(Application.StartupPath + "\\Log"))
                System.IO.Directory.CreateDirectory(Application.StartupPath + "\\Log");

            logErro = new Sapiens.Library.Log.LogError(Application.StartupPath + "\\Log");

            Properties.Settings.Default.Reset();
        }

        void distribui_AtualizaStatus(object sender, EventArgs e)
        {
            BarraProgrsso.Maximum = distribui.iRegTotal;
            BarraProgrsso.Value = distribui.iRegAtual;
            lblData.Text = distribui.DataBusca.ToString("dd/MM/yyyy HH:mm:ss");

            lblNSU.Text = UtilBo.UltimoNSU(2).ToString().PadLeft(15, '0');

            lblNSUSefaz.Text = distribui.NSUSefaz;

            this.Refresh();
        }

        private void TimerEspera_Tick(object sender, EventArgs e)
        {
            try
            {
                LblStatus.Text = "Processando";
                TimerEspera.Enabled = false;
                distribui.BuscarNFe();

                //DAL.dsTRanspTableAdapters.nfe_chavesTableAdapter tanfe = new DAL.dsTRanspTableAdapters.nfe_chavesTableAdapter();
                //DistBO bo = new DistBO();
                //bo.AtualizaStatus += Bo_AtualizaStatus;
                //foreach (DAL.dsTRansp.nfe_chavesRow nfe in tanfe.GetData("0"))
                //{
                //    bo.BuscaCompleta(nfe.chave);

                //    tanfe.UpdateQuery("1", nfe.chave);
                //}
            }
            catch (Exception ex)
            {
                logErro.Log(ex, true);
            }
            finally
            {
                if (distribui.CodRetorno == "656")
                    TimerEspera.Interval = (1000 * 60) * 75; // 1 hora e 15
                else
                    TimerEspera.Interval = Properties.Settings.Default.IntervaloSegundos * 1000;
                TimerEspera.Enabled = true;
                LblStatus.Text = "Aguardando";
            }
        }

        private void lblNSU_Click(object sender, EventArgs e)
        {

        }       

        private void Bo_AtualizaStatus(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}
