
namespace StampImages.OfficeAddIn.Excel
{
    partial class FormStampConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStampConfig));
            this.btnSave = new System.Windows.Forms.Button();
            this.txtTop = new System.Windows.Forms.TextBox();
            this.txtMiddle = new System.Windows.Forms.TextBox();
            this.txtBottom = new System.Windows.Forms.TextBox();
            this.picStamp = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picStamp)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(292, 198);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtTop
            // 
            this.txtTop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtTop.Location = new System.Drawing.Point(12, 12);
            this.txtTop.MaxLength = 20;
            this.txtTop.Name = "txtTop";
            this.txtTop.Size = new System.Drawing.Size(183, 19);
            this.txtTop.TabIndex = 1;
            this.txtTop.TextChanged += new System.EventHandler(this.txtTop_TextChanged);
            // 
            // txtMiddle
            // 
            this.txtMiddle.Location = new System.Drawing.Point(12, 37);
            this.txtMiddle.Name = "txtMiddle";
            this.txtMiddle.ReadOnly = true;
            this.txtMiddle.Size = new System.Drawing.Size(183, 19);
            this.txtMiddle.TabIndex = 2;
            // 
            // txtBottom
            // 
            this.txtBottom.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtBottom.Location = new System.Drawing.Point(12, 62);
            this.txtBottom.MaxLength = 20;
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.Size = new System.Drawing.Size(183, 19);
            this.txtBottom.TabIndex = 3;
            this.txtBottom.TextChanged += new System.EventHandler(this.txtBottom_TextChanged);
            // 
            // picStamp
            // 
            this.picStamp.BackColor = System.Drawing.Color.White;
            this.picStamp.Location = new System.Drawing.Point(268, 12);
            this.picStamp.Name = "picStamp";
            this.picStamp.Size = new System.Drawing.Size(130, 130);
            this.picStamp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picStamp.TabIndex = 4;
            this.picStamp.TabStop = false;
            // 
            // FormStampConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 233);
            this.Controls.Add(this.picStamp);
            this.Controls.Add(this.txtBottom);
            this.Controls.Add(this.txtMiddle);
            this.Controls.Add(this.txtTop);
            this.Controls.Add(this.btnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStampConfig";
            this.Text = "設定";
            ((System.ComponentModel.ISupportInitialize)(this.picStamp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtTop;
        private System.Windows.Forms.TextBox txtMiddle;
        private System.Windows.Forms.TextBox txtBottom;
        private System.Windows.Forms.PictureBox picStamp;
    }
}