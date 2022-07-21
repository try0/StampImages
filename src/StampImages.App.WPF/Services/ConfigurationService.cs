using StampImages.App.WPF.Objects;
using StampImages.Core;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace StampImages.App.WPF.Services
{
    /// <summary>
    /// 設定系サービス
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// スタンプデータを保存します
        /// </summary>
        /// <param name="stamp"></param>
        void Save(BaseStamp stamp);

        /// <summary>
        /// 保存してあるスタンプデータをロードします
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        BaseStamp Load(Type type);

        /// <summary>
        /// スタンプデータをシリアライズします
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        string Serialize(BaseStamp stamp);

        /// <summary>
        /// スタンプデータをデシリアライズします
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        T Deserialize<T>(string json, Type type = null) where T : BaseStamp;

        AppConfig LoadAppConfig();

        void SaveAppConfig(AppConfig config);
    }

    /// <summary>
    /// 設定系サービス実装（Json）
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {

        private static readonly string CONFIG_FILE_DIR = Path.Combine(Application.UserAppDataPath, "Config");

        public class ColorJsonConverter : JsonConverter<Color>
        {
            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string[] rgb = reader.GetString().Split(',');
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
                    return StampText.GetDefaultFontFamily();
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

        public void Save(BaseStamp stamp)
        {
            if (!File.Exists(CONFIG_FILE_DIR))
            {
                Directory.CreateDirectory(CONFIG_FILE_DIR);
            }

            var json = Serialize(stamp);

            using (var streamWriter = new StreamWriter(Path.Combine(CONFIG_FILE_DIR, $"{stamp.GetType().Name}.json"), false, Encoding.UTF8))
            {
                streamWriter.WriteLine(json);
                streamWriter.Flush();
            }
        }

        public BaseStamp Load(Type type)
        {
            if (!File.Exists(Path.Combine(CONFIG_FILE_DIR, $"{type.Name}.json")))
            {
                return null;
            }

            using (var streamReader = new StreamReader(Path.Combine(CONFIG_FILE_DIR, $"{type.Name}.json"), Encoding.UTF8))
            {
                string json = streamReader.ReadToEnd();
                return Deserialize<BaseStamp>(json, type);
            }
        }

        public string Serialize(BaseStamp stamp)
        {
            var json = JsonSerializer.Serialize(stamp, stamp.GetType(), GetDefaultOptions());
            return json;
        }

        public T Deserialize<T>(string json, Type type) where T : BaseStamp
        {
            if (type == null)
            {
                type = typeof(T);
            }
            return (T)JsonSerializer.Deserialize(json, type, GetDefaultOptions());
        }

        public string SerializeObject(object obj, Type type)
        {
            var json = JsonSerializer.Serialize(obj, type, GetDefaultOptions());
            return json;
        }

        public T DeserializeObject<T>(string json, Type type)
        {
            if (type == null)
            {
                type = typeof(T);
            }
            return (T)JsonSerializer.Deserialize(json, type, GetDefaultOptions());
        }

        public AppConfig LoadAppConfig()
        {
            if (!File.Exists(Path.Combine(CONFIG_FILE_DIR, $"AppConfig.json")))
            {
                return new AppConfig();
            }

            using (var streamReader = new StreamReader(Path.Combine(CONFIG_FILE_DIR, $"AppConfig.json"), Encoding.UTF8))
            {
                string json = streamReader.ReadToEnd();
                return DeserializeObject<AppConfig>(json, typeof(AppConfig));
            }
        }

        public void SaveAppConfig(AppConfig config)
        {
            if (!File.Exists(CONFIG_FILE_DIR))
            {
                Directory.CreateDirectory(CONFIG_FILE_DIR);
            }

            var json = SerializeObject(config, typeof(AppConfig));

            using (var streamWriter = new StreamWriter(Path.Combine(CONFIG_FILE_DIR, $"AppConfig.json"), false, Encoding.UTF8))
            {
                streamWriter.WriteLine(json);
                streamWriter.Flush();
            }
        }
    }
}
