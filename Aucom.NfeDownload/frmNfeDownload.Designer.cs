namespace Scire.NfeDownload
{
    partial class frmNfeDownload
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNfeDownload));
            this.TimerEspera = new System.Windows.Forms.Timer(this.components);
            this.BarraProgrsso = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNSUSefaz = new System.Windows.Forms.Label();
            this.lblNSU = new System.Windows.Forms.Label();
            this.lblData = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LblFuncao = new System.Windows.Forms.Label();
            this.PictureScire = new System.Windows.Forms.PictureBox();
            this.PictureNFe = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureScire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureNFe)).BeginInit();
            this.SuspendLayout();
            // 
            // TimerEspera
            // 
            this.TimerEspera.Interval = 5000;
            this.TimerEspera.Tick += new System.EventHandler(this.TimerEspera_Tick);
            // 
            // BarraProgrsso
            // 
            this.BarraProgrsso.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BarraProgrsso.Location = new System.Drawing.Point(0, 208);
            this.BarraProgrsso.Name = "BarraProgrsso";
            this.BarraProgrsso.Size = new System.Drawing.Size(603, 23);
            this.BarraProgrsso.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Última Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Último NSU Buscado:";
            // 
            // lblNSUSefaz
            // 
            this.lblNSUSefaz.AutoSize = true;
            this.lblNSUSefaz.Location = new System.Drawing.Point(477, 171);
            this.lblNSUSefaz.Name = "lblNSUSefaz";
            this.lblNSUSefaz.Size = new System.Drawing.Size(36, 13);
            this.lblNSUSefaz.TabIndex = 6;
            this.lblNSUSefaz.Text = "[NSU]";
            // 
            // lblNSU
            // 
            this.lblNSU.AutoSize = true;
            this.lblNSU.Location = new System.Drawing.Point(145, 171);
            this.lblNSU.Name = "lblNSU";
            this.lblNSU.Size = new System.Drawing.Size(36, 13);
            this.lblNSU.TabIndex = 3;
            this.lblNSU.Text = "[NSU]";
            this.lblNSU.Click += new System.EventHandler(this.lblNSU_Click);
            // 
            // lblData
            // 
            this.lblData.AutoSize = true;
            this.lblData.Location = new System.Drawing.Point(145, 186);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(36, 13);
            this.lblData.TabIndex = 2;
            this.lblData.Text = "[Data]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(361, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Último NSU SEFAZ:";
            // 
            // LblFuncao
            // 
            this.LblFuncao.AutoSize = true;
            this.LblFuncao.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblFuncao.Location = new System.Drawing.Point(200, 31);
            this.LblFuncao.Name = "LblFuncao";
            this.LblFuncao.Size = new System.Drawing.Size(367, 25);
            this.LblFuncao.TabIndex = 10;
            this.LblFuncao.Text = "Download de Documentos Fiscais";
            // 
            // PictureScire
            // 
            this.PictureScire.Image = global::Scire.NfeDownload.Properties.Resources.logoScire_transp;
            this.PictureScire.Location = new System.Drawing.Point(392, 83);
            this.PictureScire.Name = "PictureScire";
            this.PictureScire.Size = new System.Drawing.Size(211, 78);
            this.PictureScire.TabIndex = 8;
            this.PictureScire.TabStop = false;
            // 
            // PictureNFe
            // 
            this.PictureNFe.Image = global::Scire.NfeDownload.Properties.Resources.nota_fiscal_eletronica__2_;
            this.PictureNFe.Location = new System.Drawing.Point(0, -9);
            this.PictureNFe.Name = "PictureNFe";
            this.PictureNFe.Size = new System.Drawing.Size(603, 170);
            this.PictureNFe.TabIndex = 9;
            this.PictureNFe.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(361, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Status:";
            // 
            // LblStatus
            // 
            this.LblStatus.AutoSize = true;
            this.LblStatus.Location = new System.Drawing.Point(477, 186);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(43, 13);
            this.LblStatus.TabIndex = 12;
            this.LblStatus.Text = "[Status]";
            // 
            // frmNfeDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 231);
            this.Controls.Add(this.LblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblFuncao);
            this.Controls.Add(this.PictureScire);
            this.Controls.Add(this.PictureNFe);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblNSUSefaz);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblNSU);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.BarraProgrsso);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmNfeDownload";
            this.Text = "Scire.NFeDownload";
            ((System.ComponentModel.ISupportInitialize)(this.PictureScire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureNFe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer TimerEspera;
        private System.Windows.Forms.ProgressBar BarraProgrsso;
        private System.Windows.Forms.PictureBox PictureScire;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNSUSefaz;
        private System.Windows.Forms.Label lblNSU;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox PictureNFe;
        private System.Windows.Forms.Label LblFuncao;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LblStatus;
    }
}