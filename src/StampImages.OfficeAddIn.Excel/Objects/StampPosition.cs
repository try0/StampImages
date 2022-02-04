

namespace StampImages.OfficeAddIn.Excel.Objects
{
    /// <summary>
    /// スタンプ配置場所
    /// </summary>
    public class StampPosition
    {
        /// <summary>
        /// Range, ShapeName
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// シート名　正規表現でフィルター
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// ファイル名　正規表現でフィルター
        /// </summary>
        public string FilePath { get; set; }
    }
}
