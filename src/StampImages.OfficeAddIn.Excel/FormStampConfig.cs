
using StampImages.Core;
using StampImages.OfficeAddIn.Excel.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
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


            ddlFontFamily.DataSource = new InstalledFontCollection().Families;
            ddlFontFamily.ValueMember = "Name";
            ddlFontFamily.DisplayMember = "Name";
            dtMiddle.CustomFormat = "yyyy/MM/dd";
            dtMiddle.Format = DateTimePickerFormat.Custom;

            // 設定ロード
            ThreeAreaCircularStamp stamp = (ThreeAreaCircularStamp)ConfigService.Load(typeof(ThreeAreaCircularStamp));

            if (stamp != null)
            {

                if (stamp.TopText == null)
                {
                    stamp.TopText = new StampText()
                    {
                        Size = 10
                    };
                }
                if (stamp.MiddleText == null)
                {
                    stamp.MiddleText = new StampText()
                    {
                        Size = 10
                    };
                }
                if (stamp.BottomText == null)
                {
                    stamp.BottomText = new StampText()
                    {
                        Size = 10
                    };
                }

                txtTop.Text = stamp.TopText.Value;
                try
                {
                    dtMiddle.Value = string.IsNullOrEmpty(stamp.MiddleText.Value) ? DateTime.Today : DateTime.Parse(stamp.MiddleText.Value);
                }
                catch (Exception ignore)
                {

                }

                txtBottom.Text = stamp.BottomText.Value;

                numTopSize.Value = (decimal)Math.Max(stamp.TopText.Size, 1);
                numMiddleSize.Value = (decimal)Math.Max(stamp.MiddleText.Size, 1);
                numBottomSize.Value = (decimal)Math.Max(stamp.BottomText.Size, 1);

                ddlFontFamily.SelectedValue = stamp.TopText.FontFamily.Name;

                btnColor.BackColor = stamp.Color;

                chkEdgeType.Checked = stamp.EdgeType == StampEdgeType.Double;

                chkFill.Checked = stamp.IsFillColor;

                UpdateStampImagePreview();
            }
            else
            {
                dtMiddle.Value = DateTime.Today;
                ddlFontFamily.SelectedValue = "MS UI Gothic";
                btnColor.BackColor = Color.Red;
            }





            txtTop.TextChanged += OnPropertyChanged;
            dtMiddle.ValueChanged += OnPropertyChanged;
            txtBottom.TextChanged += OnPropertyChanged;

            numTopSize.TextChanged += OnPropertyChanged;
            numMiddleSize.TextChanged += OnPropertyChanged;
            numBottomSize.TextChanged += OnPropertyChanged;


            chkEdgeType.CheckedChanged += OnPropertyChanged;
            chkFill.CheckedChanged += OnPropertyChanged;

            ddlFontFamily.SelectedIndexChanged += OnPropertyChanged;

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
        /// クリアボタンクリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ConfigService.Save(new ThreeAreaCircularStamp());

            txtTop.Text = "";
            dtMiddle.Value = DateTime.Today;
            txtBottom.Text = "";

            numTopSize.Value = 10;
            numMiddleSize.Value = 10;
            numBottomSize.Value = 10;


            chkEdgeType.Checked = false;
            chkFill.Checked = false;


            ddlFontFamily.SelectedValue = "MS UI Gothic";
            btnColor.BackColor = Color.Red;

            UpdateStampImagePreview();
        }

        /// <summary>
        /// 色指定ボタンクリック時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColor_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                var color = colorDialog1.Color;
                btnColor.BackColor = color;
                UpdateStampImagePreview();


                if (color.GetBrightness() < 0.5)
                {
                    btnColor.ForeColor = Color.White;
                }
                else
                {
                    btnColor.ForeColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// テキスト編集時に実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, EventArgs e)
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

            stamp.Color = btnColor.BackColor;
            stamp.SetFontFamily(new FontFamily("MS UI Gothic"));
            stamp.Size = new Size(100, 100);
            stamp.EdgeWidth = 2;
            stamp.DividerWidth = 2;
            stamp.TopBottomTextOffset = 5;
            stamp.DoubleEdgeOffset = stamp.EdgeWidth + 2;

            stamp.TopText = new StampText
            {
                Value = txtTop.Text,
                Size = ((float)numTopSize.Value)
            };
            stamp.MiddleText = new StampText
            {
                Value = dtMiddle.Value.ToString("yyyy/MM/dd"),
                Size = ((float)numMiddleSize.Value)
            };
            stamp.BottomText = new StampText
            {
                Value = txtBottom.Text,
                Size = ((float)numBottomSize.Value)
            };

            stamp.SetFontFamily(((FontFamily[])ddlFontFamily.DataSource)[ddlFontFamily.SelectedIndex]);

            stamp.EdgeType = chkEdgeType.Checked ? StampEdgeType.Double : StampEdgeType.Single;

            stamp.IsFillColor = chkFill.Checked;




            return stamp;
        }

        /// <summary>
        /// プレビューを更新します。
        /// </summary>
        private void UpdateStampImagePreview()
        {
            if (picStamp.Image != null)
            {
                picStamp.Image.Dispose();
            }
            var stamp = GetStamp();

            using (var factory = new StampImageFactory())
            {
                var bitmap = factory.Create(stamp);
                picStamp.Image = bitmap;
            }

        }


    }
}
