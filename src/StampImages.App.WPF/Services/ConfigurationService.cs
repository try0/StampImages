using StampImages.Core;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StampImages.App.WPF.Services
{
    /// <summary>
    /// 設定系サービス
    /// </summary>
    public interface IConfigurationService
    {
        public void Serialize(BaseStamp stamp);
        public BaseStamp Deserialize(Type type);
    }

    /// <summary>
    /// 設定系サービス実装（Json）
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        public class ColorJsonConverter : JsonConverter<Color>
        {
            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string[] rgb = reader.GetString().Split(",");
                if (rgb.Length != 3)
                {
                    return BaseStamp.DEFAULT_STAMP_COLOR;
                }
                return Color.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
            }

            public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"{value.R},{value.G},{value.B}");
            }

        }

        public class FontFamilyJsonConverter : JsonConverter<FontFamily>
        {
            public override FontFamily Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.GetString() == null || reader.GetString().Length == 0)
                {
                    return StampText.DEFAULT_FONT_FAMILY;
                }

                return new FontFamily(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Name);
            }
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public ConfigurationService()
        {
        }

        /// <summary>
        /// 読み書きのデフォルト設定を取得します。
        /// </summary>
        /// <returns></returns>
        private JsonSerializerOptions GetDefaultOptions()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new FontFamilyJsonConverter());
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        public void Serialize(BaseStamp stamp)
        {
            if (!File.Exists("./Config"))
            {
                Directory.CreateDirectory("./Config");
            }


            var json = JsonSerializer.Serialize(stamp, stamp.GetType(), GetDefaultOptions());
            using (var streamWriter = new StreamWriter($"./Config/{stamp.GetType().Name}.json", false, Encoding.UTF8))
            {
                streamWriter.WriteLine(json);
                streamWriter.Flush();
            }

        }

        public BaseStamp Deserialize(Type type)
        {
            if (!File.Exists($"./Config/{type.Name}.json"))
            {
                return null;
            }

            using (var streamReader = new StreamReader($"./Config/{type.Name}.json", Encoding.UTF8))

            {
                string json = streamReader.ReadToEnd();

                return (BaseStamp)JsonSerializer.Deserialize(json, type, GetDefaultOptions());
            }
        }

    }
}
