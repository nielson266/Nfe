namespace Nfe
{
    partial class FConfiguracao
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
            this.TmStatus = new System.Windows.Forms.Timer(this.components);
            this.TmInutilizacao = new System.Windows.Forms.Timer(this.components);
            this.TmCancelamento = new System.Windows.Forms.Timer(this.components);
            this.TmNotaFiscal = new System.Windows.Forms.Timer(this.components);
            this.TmCartaEletronica = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUltConsulta = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAmb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtVersao = new System.Windows.Forms.TextBox();
            this.TmConsultaLote = new System.Windows.Forms.Timer(this.components);
            this.TmEntrada = new System.Windows.Forms.Timer(this.components);
            this.TmSemRetorno = new System.Windows.Forms.Timer(this.components);
            this.TmXmlCliente = new System.Windows.Forms.Timer(this.components);
            this.TmEnviarEmailCliente = new System.Windows.Forms.Timer(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.TmConsultaNFeDestinatario = new System.Windows.Forms.Timer(this.components);
            this.TmManifestacao = new System.Windows.Forms.Timer(this.components);
            this.TmDownloadNFe = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TmStatus
            // 
            this.TmStatus.Interval = 120000;
            this.TmStatus.Tick += new System.EventHandler(this.TmStatus_Tick);
            // 
            // TmInutilizacao
            // 
            this.TmInutilizacao.Interval = 120000;
            this.TmInutilizacao.Tick += new System.EventHandler(this.TmInutilizacao_Tick);
            // 
            // TmCancelamento
            // 
            this.TmCancelamento.Interval = 30000;
            this.TmCancelamento.Tick += new System.EventHandler(this.TmCancelamento_Tick);
            // 
            // TmNotaFiscal
            // 
            this.TmNotaFiscal.Interval = 30000;
            this.TmNotaFiscal.Tick += new System.EventHandler(this.TmNotaFiscal_Tick);
            // 
            // TmCartaEletronica
            // 
            this.TmCartaEletronica.Interval = 120000;
            this.TmCartaEletronica.Tick += new System.EventHandler(this.TmCartaEletronica_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 299);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Status";
            // 
            // txtStatus
            // 
            this.txtStatus.Enabled = false;
            this.txtStatus.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatus.Location = new System.Drawing.Point(2, 313);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(33, 18);
            this.txtStatus.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ultima Consulta";
            // 
            // txtUltConsulta
            // 
            this.txtUltConsulta.Enabled = false;
            this.txtUltConsulta.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUltConsulta.Location = new System.Drawing.Point(36, 313);
            this.txtUltConsulta.Name = "txtUltConsulta";
            this.txtUltConsulta.Size = new System.Drawing.Size(120, 18);
            this.txtUltConsulta.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 299);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Mensagem";
            // 
            // txtMsg
            // 
            this.txtMsg.Enabled = false;
            this.txtMsg.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMsg.Location = new System.Drawing.Point(157, 313);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(174, 18);
            this.txtMsg.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(368, 299);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Ambiente";
            // 
            // txtAmb
            // 
            this.txtAmb.Enabled = false;
            this.txtAmb.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAmb.Location = new System.Drawing.Point(371, 313);
            this.txtAmb.Name = "txtAmb";
            this.txtAmb.Size = new System.Drawing.Size(38, 18);
            this.txtAmb.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(331, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Versao";
            // 
            // TxtVersao
            // 
            this.TxtVersao.Enabled = false;
            this.TxtVersao.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtVersao.Location = new System.Drawing.Point(332, 313);
            this.TxtVersao.Name = "TxtVersao";
            this.TxtVersao.Size = new System.Drawing.Size(38, 18);
            this.TxtVersao.TabIndex = 7;
            // 
            // TmConsultaLote
            // 
            this.TmConsultaLote.Interval = 60000;
            this.TmConsultaLote.Tick += new System.EventHandler(this.TmConsultaLote_Tick);
            // 
            // TmEntrada
            // 
            this.TmEntrada.Interval = 20000;
            this.TmEntrada.Tick += new System.EventHandler(this.TmEntrada_Tick);
            // 
            // TmSemRetorno
            // 
            this.TmSemRetorno.Interval = 60000;
            this.TmSemRetorno.Tick += new System.EventHandler(this.TmSemRetorno_Tick);
            // 
            // TmXmlCliente
            // 
            this.TmXmlCliente.Interval = 60000;
            this.TmXmlCliente.Tick += new System.EventHandler(this.TmXmlCliente_Tick);
            // 
            // TmEnviarEmailCliente
            // 
            this.TmEnviarEmailCliente.Interval = 20000;
            this.TmEnviarEmailCliente.Tick += new System.EventHandler(this.TmEnviarEmailCliente_Tick);
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.AutoCheck = false;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(314, 52);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(31, 23);
            this.checkBox1.TabIndex = 8;
            this.checkBox1.Text = "Off";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // TmConsultaNFeDestinatario
            // 
            this.TmConsultaNFeDestinatario.Interval = 300000;
            this.TmConsultaNFeDestinatario.Tick += new System.EventHandler(this.TmConsultaNFeDestinatario_Tick);
            // 
            // TmManifestacao
            // 
            this.TmManifestacao.Interval = 30000;
            this.TmManifestacao.Tick += new System.EventHandler(this.TmManifestacao_Tick);
            // 
            // TmDownloadNFe
            // 
            this.TmDownloadNFe.Enabled = true;
            this.TmDownloadNFe.Interval = 30000;
            this.TmDownloadNFe.Tick += new System.EventHandler(this.TmDownloadNFe_Tick);
            // 
            // FConfiguracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 335);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.txtUltConsulta);
            this.Controls.Add(this.TxtVersao);
            this.Controls.Add(this.txtAmb);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "FConfiguracao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer TmStatus;
        private System.Windows.Forms.Timer TmInutilizacao;
        private System.Windows.Forms.Timer TmCancelamento;
        private System.Windows.Forms.Timer TmNotaFiscal;
        private System.Windows.Forms.Timer TmCartaEletronica;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUltConsulta;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAmb;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtVersao;
        private System.Windows.Forms.Timer TmConsultaLote;
        private System.Windows.Forms.Timer TmEntrada;
        private System.Windows.Forms.Timer TmSemRetorno;
        private System.Windows.Forms.Timer TmXmlCliente;
        private System.Windows.Forms.Timer TmEnviarEmailCliente;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer TmConsultaNFeDestinatario;
        private System.Windows.Forms.Timer TmManifestacao;
        private System.Windows.Forms.Timer TmDownloadNFe;
    }
}