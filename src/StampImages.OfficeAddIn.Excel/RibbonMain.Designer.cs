
namespace StampImages.OfficeAddIn.Excel
{
    partial class RibbonMain : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonMain()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonMain));
            this.TabStampImages = this.Factory.CreateRibbonTab();
            this.groupMain = this.Factory.CreateRibbonGroup();
            this.btnPasteStamp = this.Factory.CreateRibbonButton();
            this.TabStampImages.SuspendLayout();
            this.groupMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabStampImages
            // 
            this.TabStampImages.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.TabStampImages.Groups.Add(this.groupMain);
            this.TabStampImages.Label = "StampImages";
            this.TabStampImages.Name = "TabStampImages";
            // 
            // groupMain
            // 
            this.groupMain.DialogLauncher = ribbonDialogLauncherImpl1;
            this.groupMain.Items.Add(this.btnPasteStamp);
            this.groupMain.Label = "設定";
            this.groupMain.Name = "groupMain";
            // 
            // btnPasteStamp
            // 
            this.btnPasteStamp.Image = ((System.Drawing.Image)(resources.GetObject("btnPasteStamp.Image")));
            this.btnPasteStamp.Label = "　スタンプ　　";
            this.btnPasteStamp.Name = "btnPasteStamp";
            this.btnPasteStamp.ShowImage = true;
            this.btnPasteStamp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnPasteStamp_Click);
            // 
            // RibbonMain
            // 
            this.Name = "RibbonMain";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.TabStampImages);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RibbonMain_Load);
            this.TabStampImages.ResumeLayout(false);
            this.TabStampImages.PerformLayout();
            this.groupMain.ResumeLayout(false);
            this.groupMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab TabStampImages;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupMain;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPasteStamp;
    }

    partial class ThisRibbonCollection
    {
        internal RibbonMain RibbonMain
        {
            get { return this.GetRibbon<RibbonMain>(); }
        }
    }
}
