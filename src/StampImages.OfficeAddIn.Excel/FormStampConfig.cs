
using StampImages.Core;
using StampImages.OfficeAddIn.Excel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StampImages.OfficeAddIn.Excel
{
    /// <summary>
    /// 設定フォーム
    /// </summary>
    public partial class FormStampConfig : Form
    {
        ConfigurationService ConfigService { get; set; } = new ConfigurationService();

        /// <summary>
        /// コンストラクター
        /// </summary>
        public FormStampConfig()
        {
            InitializeComponent();

            // 設定ロード
            ThreeAreaCircularStamp stamp = (ThreeAreaCircularStamp)ConfigService.Load(typeof(ThreeAreaCircularStamp));

            if (stamp != null)
            {
                txtTop.Text = stamp.TopText.Value;
                txtMiddle.Text = DateTime.Today.ToString("yyyy/MM/dd");
                txtBottom.Text = stamp.BottomText.Value;

                UpdateStampImagePreview();
            }
        }


        /// <summary>
        /// 保存ボタンクリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            var stamp = GetStamp();
            ConfigService.Save(stamp);


            Close();
        }

        /// <summary>
        /// テキスト編集時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTop_TextChanged(object sender, EventArgs e)
        {
            UpdateStampImagePreview();
        }

        /// <summary>
        /// テキスト編集時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBottom_TextChanged(object sender, EventArgs e)
        {
            UpdateStampImagePreview();
        }

        /// <summary>
        /// 編集内容を考慮したスタンプデータを取得します。
        /// </summary>
        /// <returns></returns>
        private BaseStamp GetStamp()
        {
            ThreeAreaCircularStamp stamp = (ThreeAreaCircularStamp)ConfigService.Load(typeof(ThreeAreaCircularStamp));

            if (stamp == null)
            {
                stamp = new ThreeAreaCircularStamp();
            }

            stamp.Color = Color.Red;
            stamp.SetFontFamily(new FontFamily("MS UI Gothic"));
            stamp.Size = new Size(100, 100);
            stamp.EdgeWidth = 2;
            stamp.DividerWidth = 2;
            stamp.TopBottomTextOffset = 5;

            stamp.TopText = new StampText
            {
                Value = txtTop.Text,
                Size = 10
            };
            stamp.MiddleText = new StampText
            {
                Value = DateTime.Today.ToString("yyyy/MM/dd"),
                Size = 13
            };
            stamp.BottomText = new StampText
            {
                Value = txtBottom.Text,
                Size = 11
            };

            return stamp;
        }

        /// <summary>
        /// プレビューを更新します。
        /// </summary>
        private void UpdateStampImagePreview()
        {
            var stamp = GetStamp();

            using (var factory = new StampImageFactory())
            {
                var bitmap = factory.Create(stamp);
                picStamp.Image = bitmap;
            }

        }
    }
}
