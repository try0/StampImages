using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Interop.Excel;
using stdole;
using System.Windows.Forms;
using System.Drawing;

namespace StampImages.OfficeAddIn.Excel
{
    public partial class ThisAddIn
    {
        private static readonly string STAMP_MENU_TEXT = "スタンプ貼り付け";

        /// <summary>
        /// コンテキストメニュー
        /// </summary>
        public CommandBar ContextMenu => this.Application.CommandBars["Cell"];

        /// <summary>
        /// アドイン起動時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.SheetBeforeRightClick += OnSheetRightClick;

            // コンテキストメニューへ追加
            var contextMenu = ContextMenu;

            var menuControls = contextMenu.Controls;
            CommandBarButton stampMenuItem =
                (CommandBarButton)menuControls.Add(MsoControlType.msoControlButton, missing, missing, menuControls.Count + 1, true);
            stampMenuItem.BeginGroup = true;
            stampMenuItem.Style = MsoButtonStyle.msoButtonIconAndCaption;
            Image stampImageIcon = Properties.Resources.icon.ToBitmap();
            stampMenuItem.Picture = AxHostForPictureDisp.GetIPictureDisp(stampImageIcon);
            stampMenuItem.Caption = STAMP_MENU_TEXT;
            stampMenuItem.Click += new _CommandBarButtonEvents_ClickEventHandler(stampMenuItemClick);

        }

        /// <summary>
        /// セル右クリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Target"></param>
        /// <param name="Cancel"></param>
        private void OnSheetRightClick(object sender, Range Target, ref bool Cancel)
        {
            // イベント再設定
            var contextMenu = ContextMenu;

            foreach (var ctrl in contextMenu.Controls)
            {
                if (ctrl is CommandBarButton stampMenuItem)
                {
                    if (stampMenuItem.Caption.Equals(STAMP_MENU_TEXT))
                    {
                        stampMenuItem.Click += new _CommandBarButtonEvents_ClickEventHandler(stampMenuItemClick);
                        break;
                    }
                }
            }
        }

        void stampMenuItemClick(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            // リボンへ移譲
            Globals.Ribbons.RibbonMain.btnPasteStamp_Click(null, null);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO で生成されたコード

        /// <summary>
        /// デザイナーのサポートに必要なメソッドです。
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion


        internal class AxHostForPictureDisp : AxHost
        {
            public AxHostForPictureDisp() : base("")
            {
            }

            /// <summary>
            /// <see cref="Image"/>から、<see cref="IPictureDisp"/>を取得します。
            /// </summary>
            /// <param name="Image"></param>
            /// <returns></returns>
            public static IPictureDisp GetIPictureDisp(Image Image)
            {
                if (GetIPictureDispFromPicture(Image) is IPictureDisp disp)
                {
                    return disp;
                }

                return null;
            }
        }
    }
}
