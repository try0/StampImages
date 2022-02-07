using StampImages.OfficeAddIn.Excel.Objects;
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
    public partial class FormStampPositionConfig : Form
    {

        public ConfigurationService ConfigService { get; set; } = new ConfigurationService();


        public FormStampPositionConfig()
        {
            InitializeComponent();

            gvStampPosition.ColumnCount = 3;

            // カラム名を指定
            gvStampPosition.Columns[0].HeaderText = "ObjectName";
            gvStampPosition.Columns[0].Width = 120;
            gvStampPosition.Columns[1].HeaderText = "SheetName";
            gvStampPosition.Columns[1].Width = 120;
            gvStampPosition.Columns[2].HeaderText = "FilePath";

            gvStampPosition.Columns[gvStampPosition.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            List<StampPosition> positions = ConfigService.LoadStampPostionList();

            foreach (var pos in positions)
            {
                gvStampPosition.Rows.Add(pos.ObjectName, pos.SheetName, pos.FilePath);
            }

            for (int i = 0; i < 10; i++)
            {
                gvStampPosition.Rows.Add("", "", "");
            }

        }

        private void btnSavePosition_Click(object sender, EventArgs e)
        {
            List<StampPosition> positions = new List<StampPosition>();

            foreach (DataGridViewRow row in gvStampPosition.Rows)
            {
                var pos = new StampPosition()
                {
                    ObjectName = row.Cells[0].Value?.ToString().Trim(),
                    SheetName = row.Cells[1].Value?.ToString().Trim(),
                    FilePath = row.Cells[2].Value?.ToString().Trim(),
                };

                if (string.IsNullOrEmpty(pos.ObjectName)
                    && string.IsNullOrEmpty(pos.SheetName)
                    && string.IsNullOrEmpty(pos.FilePath))
                {
                    continue;
                }

                positions.Add(pos);
            }

            ConfigService.SaveStampPostionList(positions);

            Close();
        }
    }
}
