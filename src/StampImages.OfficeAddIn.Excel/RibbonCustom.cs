using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;
using StampImages.OfficeAddIn.Excel.Services;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using StampImages.Core;
using Shape = Microsoft.Office.Interop.Excel.Shape;
using System.Drawing;



// TODO:  リボン (XML) アイテムを有効にするには、次の手順に従います。

// 1: 次のコード ブロックを ThisAddin、ThisWorkbook、ThisDocument のいずれかのクラスにコピーします。

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new RibbonCustom();
//  }

// 2. ボタンのクリックなど、ユーザーの操作を処理するためのコールバック メソッドを、このクラスの
//    "リボンのコールバック" 領域に作成します。メモ: このリボンがリボン デザイナーからエクスポートされたものである場合は、
//    イベント ハンドラー内のコードをコールバック メソッドに移動し、リボン拡張機能 (RibbonX) のプログラミング モデルで
//    動作するように、コードを変更します。

// 3. リボン XML ファイルのコントロール タグに、コードで適切なコールバック メソッドを識別するための属性を割り当てます。  

// 詳細については、Visual Studio Tools for Office ヘルプにあるリボン XML のドキュメントを参照してください。


namespace StampImages.OfficeAddIn.Excel
{
    [ComVisible(true)]
    public class RibbonCustom : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;


        /// <summary>
        /// スタンプ設定サービス
        /// </summary>
        ConfigurationService ConfigService { get; set; } = new ConfigurationService();


        /// <summary>
        /// コンストラクター
        /// </summary>
        public RibbonCustom()
        {
        }

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("StampImages.OfficeAddIn.Excel.RibbonCustom.xml");
        }

        /// <summary>
        /// アイコンを取得します。
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public Bitmap GetIconImage(Office.IRibbonControl control)
        {
            return Properties.Resources.icon.ToBitmap();
        }



        /// <summary>
        /// リボンロード時に実行されます。
        /// </summary>
        /// <param name="ribbonUI"></param>
        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }



        /// <summary>
        /// スタンプボタンクリック時に実行します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClickStampButton(Office.IRibbonControl control)
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
                    // セルや図形等でLeft、Topが定義される共通のインターフェースを持たないようなので
                    // dynamicのまま処理して、例外発生時にA1にする
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

            if (selection != null)
            {
                // 以下図形がフォーカスされている場合、サイズを合わせる
                if (selection is Microsoft.Office.Interop.Excel.Rectangle rect)
                {
                    AdjustForSelectionShape(stampShape, rect.Width, rect.Height);
                }
                else if (selection is Microsoft.Office.Interop.Excel.Oval oval)
                {
                    AdjustForSelectionShape(stampShape, oval.Width, oval.Height);
                }
            }


            File.Delete(imagePath);
        }

        /// <summary>
        /// スタンプ画像のサイズと配置位置を調整します。
        /// </summary>
        /// <param name="stampShape"></param>
        /// <param name="selectionWidth"></param>
        /// <param name="selectionHeight"></param>
        private void AdjustForSelectionShape(Shape stampShape, double selectionWidth, double selectionHeight)
        {
            var size = (float)Math.Min(selectionWidth, selectionHeight);
            stampShape.Width = size;
            stampShape.Height = size;

            // 図形中心へ調整
            if (selectionWidth > selectionHeight)
            {
                stampShape.Left += (float)(selectionWidth - stampShape.Width) / 2;
            }
            else if (selectionWidth < selectionHeight)
            {
                stampShape.Top += (float)(selectionHeight - stampShape.Height) / 2;
            }
        }

        /// <summary>
        /// 設定ダイアログランチャークリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClickConfigDialogLauncher(Office.IRibbonControl control)
        {
            var form = new FormStampConfig();
            form.ShowDialog();
        }

        /// <summary>
        /// スタンプ設定ボタンクリック時に実行されます。
        /// </summary>
        /// <param name="control"></param>
        public void OnClickStampConfigButton(Office.IRibbonControl control) => OnClickConfigDialogLauncher(control);

        #region ヘルパー

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }


        #endregion
    }
}
