
namespace StampImages.OfficeAddIn.Excel
{
    partial class FormStampPositionConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStampPositionConfig));
            this.btnSavePosition = new System.Windows.Forms.Button();
            this.gvStampPosition = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gvStampPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSavePosition
            // 
            this.btnSavePosition.Location = new System.Drawing.Point(597, 326);
            this.btnSavePosition.Name = "btnSavePosition";
            this.btnSavePosition.Size = new System.Drawing.Size(75, 23);
            this.btnSavePosition.TabIndex = 1;
            this.btnSavePosition.Text = "保存";
            this.btnSavePosition.UseVisualStyleBackColor = true;
            this.btnSavePosition.Click += new System.EventHandler(this.btnSavePosition_Click);
            // 
            // gvStampPosition
            // 
            this.gvStampPosition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvStampPosition.Location = new System.Drawing.Point(12, 12);
            this.gvStampPosition.Name = "gvStampPosition";
            this.gvStampPosition.RowTemplate.Height = 21;
            this.gvStampPosition.Size = new System.Drawing.Size(660, 302);
            this.gvStampPosition.TabIndex = 2;
            // 
            // FormStampPositionConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.gvStampPosition);
            this.Controls.Add(this.btnSavePosition);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStampPositionConfig";
            this.Text = "配置設定";
            ((System.ComponentModel.ISupportInitialize)(this.gvStampPosition)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSavePosition;
        private System.Windows.Forms.DataGridView gvStampPosition;
    }
}