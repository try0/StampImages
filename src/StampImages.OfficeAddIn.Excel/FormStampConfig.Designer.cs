
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
            this.txtBottom = new System.Windows.Forms.TextBox();
            this.picStamp = new System.Windows.Forms.PictureBox();
            this.numTopSize = new System.Windows.Forms.NumericUpDown();
            this.numMiddleSize = new System.Windows.Forms.NumericUpDown();
            this.numBottomSize = new System.Windows.Forms.NumericUpDown();
            this.ddlFontFamily = new System.Windows.Forms.ComboBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.colorDialog2 = new System.Windows.Forms.ColorDialog();
            this.btnColor = new System.Windows.Forms.Button();
            this.chkEdgeType = new System.Windows.Forms.CheckBox();
            this.chkFill = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.dtMiddle = new System.Windows.Forms.DateTimePicker();
            this.chkToday = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picStamp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTopSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMiddleSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBottomSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(292, 198);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 23);
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
            // 
            // txtBottom
            // 
            this.txtBottom.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtBottom.Location = new System.Drawing.Point(12, 62);
            this.txtBottom.MaxLength = 20;
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.Size = new System.Drawing.Size(183, 19);
            this.txtBottom.TabIndex = 3;
            // 
            // picStamp
            // 
            this.picStamp.BackColor = System.Drawing.Color.White;
            this.picStamp.Location = new System.Drawing.Point(292, 12);
            this.picStamp.Name = "picStamp";
            this.picStamp.Size = new System.Drawing.Size(110, 110);
            this.picStamp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picStamp.TabIndex = 4;
            this.picStamp.TabStop = false;
            // 
            // numTopSize
            // 
            this.numTopSize.Location = new System.Drawing.Point(201, 12);
            this.numTopSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTopSize.Name = "numTopSize";
            this.numTopSize.Size = new System.Drawing.Size(43, 19);
            this.numTopSize.TabIndex = 5;
            this.numTopSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numMiddleSize
            // 
            this.numMiddleSize.Location = new System.Drawing.Point(201, 37);
            this.numMiddleSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMiddleSize.Name = "numMiddleSize";
            this.numMiddleSize.Size = new System.Drawing.Size(43, 19);
            this.numMiddleSize.TabIndex = 6;
            this.numMiddleSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numBottomSize
            // 
            this.numBottomSize.Location = new System.Drawing.Point(201, 62);
            this.numBottomSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBottomSize.Name = "numBottomSize";
            this.numBottomSize.Size = new System.Drawing.Size(43, 19);
            this.numBottomSize.TabIndex = 7;
            this.numBottomSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ddlFontFamily
            // 
            this.ddlFontFamily.FormattingEnabled = true;
            this.ddlFontFamily.Location = new System.Drawing.Point(292, 128);
            this.ddlFontFamily.Name = "ddlFontFamily";
            this.ddlFontFamily.Size = new System.Drawing.Size(110, 20);
            this.ddlFontFamily.TabIndex = 8;
            // 
            // btnColor
            // 
            this.btnColor.Location = new System.Drawing.Point(292, 155);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(110, 23);
            this.btnColor.TabIndex = 9;
            this.btnColor.Text = "カラー";
            this.btnColor.UseVisualStyleBackColor = true;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // chkEdgeType
            // 
            this.chkEdgeType.AutoSize = true;
            this.chkEdgeType.Location = new System.Drawing.Point(12, 106);
            this.chkEdgeType.Name = "chkEdgeType";
            this.chkEdgeType.Size = new System.Drawing.Size(42, 16);
            this.chkEdgeType.TabIndex = 10;
            this.chkEdgeType.Text = "2重";
            this.chkEdgeType.UseVisualStyleBackColor = true;
            // 
            // chkFill
            // 
            this.chkFill.AutoSize = true;
            this.chkFill.Location = new System.Drawing.Point(12, 128);
            this.chkFill.Name = "chkFill";
            this.chkFill.Size = new System.Drawing.Size(72, 16);
            this.chkFill.TabIndex = 13;
            this.chkFill.Text = "塗りつぶし";
            this.chkFill.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(176, 198);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(110, 23);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dtMiddle
            // 
            this.dtMiddle.Location = new System.Drawing.Point(12, 37);
            this.dtMiddle.Name = "dtMiddle";
            this.dtMiddle.Size = new System.Drawing.Size(123, 19);
            this.dtMiddle.TabIndex = 15;
            // 
            // chkToday
            // 
            this.chkToday.AutoSize = true;
            this.chkToday.Location = new System.Drawing.Point(142, 38);
            this.chkToday.Name = "chkToday";
            this.chkToday.Size = new System.Drawing.Size(48, 16);
            this.chkToday.TabIndex = 16;
            this.chkToday.Text = "当日";
            this.chkToday.UseVisualStyleBackColor = true;
            // 
            // FormStampConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 241);
            this.Controls.Add(this.chkToday);
            this.Controls.Add(this.dtMiddle);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chkFill);
            this.Controls.Add(this.chkEdgeType);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.ddlFontFamily);
            this.Controls.Add(this.numBottomSize);
            this.Controls.Add(this.numMiddleSize);
            this.Controls.Add(this.numTopSize);
            this.Controls.Add(this.picStamp);
            this.Controls.Add(this.txtBottom);
            this.Controls.Add(this.txtTop);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStampConfig";
            this.Text = "設定";
            ((System.ComponentModel.ISupportInitialize)(this.picStamp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTopSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMiddleSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBottomSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtTop;
        private System.Windows.Forms.TextBox txtBottom;
        private System.Windows.Forms.PictureBox picStamp;
        private System.Windows.Forms.NumericUpDown numTopSize;
        private System.Windows.Forms.NumericUpDown numMiddleSize;
        private System.Windows.Forms.NumericUpDown numBottomSize;
        private System.Windows.Forms.ComboBox ddlFontFamily;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ColorDialog colorDialog2;
        private System.Windows.Forms.Button btnColor;
        private System.Windows.Forms.CheckBox chkEdgeType;
        private System.Windows.Forms.CheckBox chkFill;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DateTimePicker dtMiddle;
        private System.Windows.Forms.CheckBox chkToday;
    }
}