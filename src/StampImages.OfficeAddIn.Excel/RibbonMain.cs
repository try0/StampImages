using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using StampImages.Core;
using StampImages.OfficeAddIn.Excel.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace StampImages.OfficeAddIn.Excel
{
    /// <summary>
    /// アドインリボン
    /// </summary>
    public partial class RibbonMain
    {
        ConfigurationService ConfigService { get; set; } = new ConfigurationService();

        /// <summary>
        /// リボンロード時に実行します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonMain_Load(object sender, RibbonUIEventArgs e)
        {
            groupMain.DialogLauncherClick += groupMain_DialogLauncherClick;
        }

        /// <summary>
        /// スタンプボタンクリック時に実行します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnPasteStamp_Click(object sender, RibbonControlEventArgs e)
        {
            // 設定ロード
            ThreeAreaCircularStamp stamp = (ThreeAreaCircularStamp)ConfigService.Load(typeof(ThreeAreaCircularStamp));

            if (stamp == null)
            {
                return;
            }
            stamp.MiddleText.Value = DateTime.Today.ToString("yyyy/MM/dd");

            // 一時ディレクトリーへ書き出し
            string tempPath = Path.Combine(Path.GetTempPath(), "tmpshape");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            string imagePath = Path.Combine(tempPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
            using (var factory = new StampImageFactory())
            {
                factory.Save(stamp, imagePath);
            }

            // アクティブシート取得
            Worksheet activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            var rangeA1 = activeSheet.Range["A1"];
            float left = (float)rangeA1.Left;
            float top = (float)rangeA1.Top;

            var selection = Globals.ThisAddIn.Application.Selection;
            if (selection != null)
            {
                try
                {
                    left = (float)selection.Left;
                    top = (float)selection.Top;
                }
                catch (Exception ignore)
                {
                    left = (float)rangeA1.Left;
                    top = (float)rangeA1.Top;
                }

            }


            // スタンプ貼り付け

            float width = 0.0F;
            float height = 0.0F;
            Shape stampShape
                  = activeSheet.Shapes.AddPicture(imagePath, MsoTriState.msoCTrue, MsoTriState.msoCTrue,
                                            left, top, width, height);

            stampShape.ScaleHeight(1.0F, MsoTriState.msoCTrue);
            stampShape.ScaleWidth(1.0F, MsoTriState.msoCTrue);


            File.Delete(imagePath);
        }

        /// <summary>
        /// 設定ダイアログランチャークリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupMain_DialogLauncherClick(object sender, Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs e)
        {
            var form = new FormStampConfig();
            form.ShowDialog();
        }


    }
}
