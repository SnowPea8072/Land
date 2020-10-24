using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using static Land.Land;

namespace Land
{
    internal class Function
    {
        /// <summary>
        /// 格式化 JSON 字符串
        /// </summary>
        /// <param name="str">需要格式化的字符串</param>
        /// <returns>格式化的 JSON 字符串</returns>
        public static string JsonFomat(string str)
        {
            var tr = new StringReader(str) as TextReader;
            var serializer = new JsonSerializer();
            var jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                var textWriter = new StringWriter();
                var jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            return str;
        }

        /// <summary>
        /// YAML 转 JSON
        /// </summary>
        /// <param name="path">转换的文件</param>
        /// <returns>转换后的 JSON 字符串</returns>
        public static string ToJson(string path)
        {
            var reader = new StreamReader(path);
            var deserializer = new DeserializerBuilder().Build();
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();
            return serializer.Serialize(deserializer.Deserialize(reader));
        }

        /// <summary>
        /// 读取 data.json 文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>领地信息</returns>
        public static JArray Land_Reload(string path)
        {
            if (File.Exists(dataFile))
            {
                try { return JArray.Parse(File.ReadAllText(path)); }
                catch { Console.WriteLine("Land >> 配置文件 land.json 读取失败！"); }
            }
            else
            {
                initData();
                Console.WriteLine("Land >> 未找到配置文件 land.json，正在为您生成！");
            }
            return null;
        }

        /// <summary>
        /// 求最大值
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>最大值</returns>
        public static int max(int x, int y) => x > y ? x : y;

        /// <summary>
        /// 求最小值
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>最小值</returns>
        public static int min(int x, int y) => x < y ? x : y;

        /// <summary>
        /// 判断玩家是否为OP
        /// </summary>
        /// <param name="xuid">玩家 xuid</param>
        /// <returns>玩家是否为OP</returns>
        public static bool Admin(string xuid) => JArray.Parse(File.ReadAllText(opFile)).First(l => l.Value<string>("xuid") == xuid).Value<string>("permission") == "operator" ? true : false;

        /// <summary>
        /// 获取玩家的 uuid
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <returns>玩家 uuid</returns>
        public static string getUuid(string name) => JArray.Parse(mcapi.getOnLinePlayers()).First(l => l.Value<string>("playername") == name).Value<string>("uuid");

        /// <summary>
        /// 获取玩家的 xuid
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <returns>玩家 xuid</returns>
        public static string getXuid(string name) => JArray.Parse(mcapi.getOnLinePlayers()).First(l => l.Value<string>("playername") == name).Value<string>("xuid");
        
        /// <summary>
        /// 向玩家发送信息
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <param name="name">发送的消息</param>
        /// <returns>发送是否成功</returns>
        public static bool tellraw(string name, string text) => mcapi.runcmd("tellraw \"" + name + "\" {\"rawtext\":[{\"text\":\"" + text + "\"}]}");
    }
}
