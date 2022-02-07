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
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;



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
        /// スタンプデータを取得します。
        /// </summary>
        /// <returns></returns>
        private ThreeAreaCircularStamp GetStampData()
        {
            // 設定ロード
            ThreeAreaCircularStamp stamp = (ThreeAreaCircularStamp)ConfigService.Load(typeof(ThreeAreaCircularStamp));

            if (stamp == null)
            {
                return null;
            }
            stamp.MiddleText.Value = DateTime.Today.ToString("yyyy/MM/dd");

            return stamp;
        }

        /// <summary>
        /// スタンプボタンクリック時に実行します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClickStampButton(Office.IRibbonControl control)
        {
            var selection = Globals.ThisAddIn.Application.Selection;
            PutStampImage(selection);

        }

        /// <summary>
        /// スタンプ画像を配置します。
        /// </summary>
        /// <param name="selection"></param>
        private void PutStampImage(dynamic selection)
        {
            ThreeAreaCircularStamp stamp = GetStampData();

            if (stamp == null)
            {
                return;
            }
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
                // 以下オブジェクトがフォーカスされている場合、サイズを合わせる

                if (selection is Microsoft.Office.Interop.Excel.Rectangle rect)
                {
                    Debug.WriteLine(rect.Name);
                    AdjustForSelectionObject(stampShape, rect.Width, rect.Height);
                }
                else if (selection is Microsoft.Office.Interop.Excel.Oval oval)
                {
                    AdjustForSelectionObject(stampShape, oval.Width, oval.Height);
                }
                else if (selection is Microsoft.Office.Interop.Excel.Shape shape)
                {
                    AdjustForSelectionObject(stampShape, shape.Width, shape.Height);
                }
                else if (selection is Microsoft.Office.Interop.Excel.Range range)
                {
                    // 選択範囲の大きさと傾斜角から、ある程度の領域が確保されたセルか判定する
                    if (range.Height > 45 && range.Width > 45)
                    {
                        var angleRadian = Math.Atan(range.Height / range.Width);
                        var angleDegree = angleRadian * (180 / Math.PI);
                        if (angleDegree > 27 && angleDegree < 63)
                        {
                            AdjustForSelectionObject(stampShape, range.Width, range.Height);
                        }
                    }
                }
            }


            File.Delete(imagePath);
        }

        /// <summary>
        /// クリップボードに準備ボタンクリック時に実行されます。
        /// </summary>
        /// <param name="control"></param>
        public void OnClickClipBoardButton(Office.IRibbonControl control)
        {

            using (var factory = new StampImageFactory())
            {
                ThreeAreaCircularStamp stamp = GetStampData();
                var stampBitmap = factory.Create(stamp);
                using (var ms = new MemoryStream())
                {
                    stampBitmap.Save(ms, ImageFormat.Png);
                    Clipboard.SetData("PNG", ms);
                }
            }

        }

        /// <summary>
        /// スタンプ配置ボタンクリック時に実行されます。
        /// </summary>
        /// <param name="control"></param>
        public void OnClickPutStampButton(Office.IRibbonControl control)
        {
            Microsoft.Office.Interop.Excel.Application app = Globals.ThisAddIn.Application;
            Workbook book = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet;

            var positions = ConfigService.LoadStampPostionList();

            if (positions.Count == 0)
            {

                OnClickPositionConfigButton(control);

                return;
            }


            foreach (var pos in positions)
            {
                // 指定ファイルパスフィルター
                if (!string.IsNullOrEmpty(pos.FilePath))
                {
                    bool res = book.FullName == pos.FilePath || Regex.IsMatch(book.FullName, pos.FilePath);
                    if (!res)
                    {
                        continue;
                    }
                }

                // シート名フィルター
                if (!string.IsNullOrEmpty(pos.SheetName))
                {
                    bool res = sheet.Name == pos.SheetName || Regex.IsMatch(sheet.Name, pos.SheetName);
                    if (!res)
                    {
                        continue;
                    }
                }

                // スタンプ配置

                // セル
                try
                {

                    Range range = app.Range[pos.ObjectName];
                    if (range == null)
                    {
                        continue;
                    }
                    PutStampImage(range);

                    continue;
                }
                catch (Exception ignore)
                {

                }

                // 図形

                var shape = FindShape(sheet.Shapes, pos.ObjectName);

                if (shape != null)
                {
                    PutStampImage(shape);
                }

            }

        }

        /// <summary>
        /// 指定された名前の図形を取得します。
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Shape FindShape(IEnumerable shapes, string name)
        {

            foreach (Shape shape in shapes)
            {

                if (shape.Type == MsoShapeType.msoGroup && shape.GroupItems.Count != 0)
                {
                    // グループ化されている場合は、子要素も検索する
                    var s = FindShape(shape.GroupItems, name);

                    if (s != null)
                    {
                        return s;
                    }
                }

                if (shape.Name == name)
                {
                    return shape;
                }

            }

            return null;
        }

        /// <summary>
        /// スタンプ画像のサイズと配置位置を調整します。
        /// </summary>
        /// <param name="stampShape"></param>
        /// <param name="selectionWidth"></param>
        /// <param name="selectionHeight"></param>
        private void AdjustForSelectionObject(Shape stampShape, double selectionWidth, double selectionHeight)
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
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        public void OnClickPositionConfigButton(Office.IRibbonControl control)
        {
            var form = new FormStampPositionConfig();
            form.StartPosition = FormStartPosition.CenterParent;
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
