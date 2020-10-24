using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Collections;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using System.Collections.Generic;
using static Essentials.Function;
using static Land.Language;
using static Land.Function;
using static Land.Form;
using CSR;

namespace Land
{
    internal class Land
    {
        /// <summary>
        /// MC相关API方法
        /// </summary>
        public static MCCSAPI mcapi = null;
        public static JObject config = null;
        private static readonly Dictionary<string, bool> Placed = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> Opened = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> player = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> onselect = new Dictionary<string, bool>();
        private static readonly Dictionary<string, Dictionary<string, int>> PosA = new Dictionary<string, Dictionary<string, int>>();
        private static readonly Dictionary<string, Dictionary<string, int>> PosB = new Dictionary<string, Dictionary<string, int>>();
        public const int timeout = 5 * 1000;
        public const string Version = "Beta v0.0.1";
        public const string Author = "SnowPea8072";
        public const string PluginName = "Land";
        private const string BDSName = "plugins";
        private const string FolderName = "plugins/Land";
        public const string opFile = "permissions.json";
        public const string dataFile = "plugins//Land//land.json";
        public const string configFile = "plugins//Land//config.yml";
        public const string languageFile = "plugins//Land//language.yml";

        #region 初始化文件

        /// <summary>
        /// 初始化文件
        /// </summary>
        private static void initPlugin()
        {
            if (!Directory.Exists(BDSName))
                Directory.CreateDirectory(BDSName);
            if (!Directory.Exists(FolderName))
                Directory.CreateDirectory(FolderName);
            if (!File.Exists(dataFile))
                initData();
            if (!File.Exists(configFile))
                initConfig();
            if (!File.Exists(languageFile))
                initLanguage();
            try { config = JObject.Parse(ToJson(configFile)); }
            catch { Console.WriteLine("Land >> 配置文件 config.yml 读取失败！"); }
            reload();
        }

        /// <summary>
        /// 初始化 land.json 文件
        /// </summary>
        public static void initData()
        {
            var data = new JArray();
            File.WriteAllText(dataFile, data.ToString());
        }

        /// <summary>
        /// 初始化 config.yml 文件
        /// </summary>
        private static void initConfig()
        {
            string[] config = new string[]
            {
                $"version: \"{Version}\"",
                "open: 347",
                "buy: 1.2",
                "sell: 1"
            };
            File.WriteAllLines(configFile, config);
        }

        /// <summary>
        /// 初始化 languege.yml 文件
        /// </summary>
        private static void initLanguage()
        {
            string[] language = new string[]
            {
                "title: \"领地系统\"",
                "content: \"欢迎使用领地系统\"",
                "open.select: \"进入领地选择模式\"",
                "open.unselect: \"退出领地选择模式\"",
                "open.buy: \"购买领地\"",
                "open.sell: \"领地出售\"",
                "open.go: \"传送到我的领地\"",
                "open.search: \"查询脚下领地信息\"",
                "open.share: \"分享你拥有的领地\"",
                "open.settings: \"领地权限设置\"",
                @"do.select: ""已进入领地选择模式：\n§a使用木棍破坏方块以选择 A 点\n使用木棍敲击方块以选择 B 点""",
                "do.unselect: \"§a已成功退出领地选择模式\"",
                "do.setA: \"§c请先设置 A 点\"",
                "do.setB: \"§c请先设置 B 点\"",
                "do.successA: \"§aA 点设置成功\"",
                "do.successB: \"§aB 点设置成功\"",
                "buy.repeat: \"该领地已存在\"",
                "buy.intersect: \"无法与其他领地重合\"",
                @"buy.sellY: ""你确定要购买此领地吗？\n购买所需： {0} 金币""",
                "buy.sellN: \"此领地暂未出售\"",
                "buy.success: \"领地创建成功\"",
                "buy.buy: \"领地购买成功\"",
                "buy.owner: \"你已拥有此领地\"",
                "buy.unkown: \"你脚下没有领地\"",
                "buy.set: \"请输入领地名称：\"",
                "buy.attack: \"是否允许玩家PVP?\"",
                "buy.chest: \"是否允许玩家打开箱子?\"",
                "buy.destroy: \"是否允许玩家破坏方块?\"",
                "buy.placed: \"是否允许玩家放置方块?\"",
                "buy.useitem: \"是否允许玩家使用物品?\"",
                "buy.explode: \"是否开启防爆?\"",
                "sell.price: \"请输入你要出售的价格（出售必填）：\"",
                "sell.whether: \"是否将领地回收?\"",
                "sell.recycle: \"领地回收成功\"",
                "sell.sell: \"领地出售成功\"",
                "sell.isnot: \"你不是该领地的主人\"",
                "sell.unkown: \"你脚下没有领地\"",
                "protect.CannotUseLand: \"§c你没有权限操作该领地\"",
                "image.select: \"textures/ui/icon_import\"",
                "image.unselect: \"textures/ui/recap_glyph_desaturated\"",
                "image.buy: \"textures/ui/icon_blackfriday\"",
                "image.sell: \"textures/ui/icon_trash\"",
                "image.go: \"textures/ui/worldsIcon\"",
                "image.search: \"textures/ui/Feedback\"",
                "image.share: \"textures/ui/switch_accounts\"",
                "image.settings: \"textures/ui/settings_glyph_color_2x\""
            };
            File.WriteAllLines(languageFile, language);
        }

        #endregion

        #region 失败/成功表单信息

        /// <summary>
        /// 玩家放置方块设置
        /// </summary>
        /// <param name="name">玩家名称</param>
        private static void Placed_Do(object name)
        {
            Placed[(string)name] = true;
            Thread.Sleep(300);
            Placed[(string)name] = false;
        }

        /// <summary>
        /// 玩家开箱设置
        /// </summary>
        /// <param name="name">玩家名称</param>
        private static void Opened_Do(object name)
        {
            Opened[(string)name] = true;
            Thread.Sleep(300);
            Opened[(string)name] = false;
        }

        /// <summary>
        /// 发送 返回操作 表单
        /// </summary>
        /// <param name="obj">表单信息</param>
        private static void Land_Do(object obj)
        {
            var Json = JObject.Parse((string)obj);
            sendModalForm(getUuid((string)Json["name"]), title, (string)Json["content"], "返回", "退出", json => { if (json.selected == "true") Land_Open(json.playername); });
        }

        #endregion

        #region 领地操作

        /// <summary>
        /// 打开领地菜单
        /// </summary>
        /// <param name="name">玩家名称</param>
        private static void Land_Open(string name)
        {
            string uuid = getUuid(name);
            sendCustomForm(uuid, new JObject(
                    new JProperty("type", "form"),
                    new JProperty("title", title),
                    new JProperty("content", content),
                    new JProperty("buttons", new JArray(
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.select)
                            )),
                            new JProperty("text", Open.select)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.unselect)
                            )),
                            new JProperty("text", Open.unselect)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.buy)
                            )),
                            new JProperty("text", Open.buy)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.sell)
                            )),
                            new JProperty("text", Open.sell)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.go)
                            )),
                            new JProperty("text", Open.go)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.search)
                            )),
                            new JProperty("text", Open.search)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.share)
                            )),
                            new JProperty("text", Open.share)
                        ),
                        new JObject(
                            new JProperty("image", new JObject(
                                new JProperty("type", "path"),
                                new JProperty("data", Image.settings)
                            )),
                            new JProperty("text", Open.settings)
                        )
                    ))
                ).ToString()
                , json =>
                {
                    switch (int.Parse(json.selected))
                    {
                        case 0:
                            Land_Select(json.playername);
                            break;
                        case 1:
                            Land_Unselect(json.playername);
                            break;
                        case 2:
                            Land_Buy(json.playername, json.dimensionid, json.XYZ);
                            break;
                        case 3:
                            Land_Sell(json.playername, json.dimensionid, json.XYZ);
                            break;
                        case 4:
                            // Land_Go(json.playername);
                            break;
                        case 5:
                            // Land_Search(json.playername);
                            break;
                        case 6:
                            // Land_Share(json.playername);
                            break;
                        case 7:
                            // Land_Settings();
                            break;
                        default:
                            break;
                    }
                }
            );
        }

        #endregion

        #region 领地选择模式操作

        /// <summary>
        /// 进入领地选择模式
        /// </summary>
        /// <param name="name">玩家名称</param>
        private static void Land_Select(string name)
        {
            onselect[name] = true;
            PosA[name] = null;
            PosB[name] = null;
            tellraw(name, Do.select);
        }
        
        /// <summary>
        /// 退出领地选择模式
        /// </summary>
        /// <param name="name">玩家名称</param>
        private static void Land_Unselect(string name)
        {
            onselect[name] = false;
            PosA.Remove(name);
            PosB.Remove(name);
            tellraw(name, Do.unselect);
        }

        #endregion

        #region 领地购买操作

        /// <summary>
        /// 领地购买
        /// </summary>
        /// <param name="name"></param>
        private static void Land_Buy(string name, int dimensionid, Vec3 position)
        {
            string uuid = getUuid(name);
            if (dimensionid != 0)
                return;
            if (onselect[name])
            {
                if (PosA[name] == null)
                {
                    tellraw(name, Do.setA);
                    return;
                }
                if (PosB[name] == null)
                {
                    tellraw(name, Do.setB);
                    return;
                }
                var data = Land_Reload(dataFile);
                int x1 = max(PosA[name]["x"], PosB[name]["x"]);
                int y1 = max(PosA[name]["y"], PosB[name]["y"]);
                int z1 = max(PosA[name]["z"], PosB[name]["z"]);
                int x2 = min(PosA[name]["x"], PosB[name]["x"]);
                int y2 = min(PosA[name]["y"], PosB[name]["y"]);
                int z2 = min(PosA[name]["z"], PosB[name]["z"]);
                foreach (JObject pos in data)
                {
                    bool x = x1 <= (int)pos["position"]["x1"] && x2 >= (int)pos["position"]["x2"];
                    bool y = y1 <= (int)pos["position"]["y1"] && y2 >= (int)pos["position"]["y2"];
                    bool z = z1 <= (int)pos["position"]["z1"] && z2 >= (int)pos["position"]["z2"];
                    if (x && y && z)
                    {
                        var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                        failed.Name = "领地重合线程";
                        failed.Start(new JObject(
                            new JProperty("name", name),
                            new JProperty("content", Buy.intersect)
                        ).ToString());
                        return;
                    }
                }
                int money = Convert.ToInt32((x1 - x2) * (y1 - y2) * (z1 - z2) * (float)config["buy"]);
                sendCustomForm(uuid, new JObject(
                        new JProperty("title", title),
                        new JProperty("type", "custom_form"),
                        new JProperty("content", new JArray(
                            new JObject(
                                new JProperty("text", Buy.set),
                                new JProperty("type", "input"),
                                new JProperty("placeholder", ""),
                                new JProperty("default", "")
                            ),
                            new JObject(
                                new JProperty("text", Buy.attack),
                                new JProperty("type", "toggle"),
                                new JProperty("default", true)
                            ),
                            new JObject(
                                new JProperty("text", Buy.chest),
                                new JProperty("type", "toggle"),
                                new JProperty("default", false)
                            ),
                            new JObject(
                                new JProperty("text", Buy.destroy),
                                new JProperty("type", "toggle"),
                                new JProperty("default", false)
                            ),
                            new JObject(
                                new JProperty("text", Buy.placed),
                                new JProperty("type", "toggle"),
                                new JProperty("default", false)
                            ),
                            new JObject(
                                new JProperty("text", Buy.useitem),
                                new JProperty("type", "toggle"),
                                new JProperty("default", false)
                            ),
                            new JObject(
                                new JProperty("text", Buy.explode),
                                new JProperty("type", "toggle"),
                                new JProperty("default", true)
                            ),
                            new JObject(
                                new JProperty("text", $"本次操作需要： {money} 金币"),
                                new JProperty("type", "label")
                            )
                        ))
                    ).ToString()
                    , json =>
                    {
                        if (json.selected == "null")
                            return;
                        var selected = JsonConvert.DeserializeObject<ArrayList>(json.selected);
                        foreach (JObject land in data)
                            if ((string)land["name"] == (string)selected[0])
                            {
                                var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                                failed.Name = "领地名重复线程";
                                failed.Start(new JObject(
                                    new JProperty("name", json.playername),
                                    new JProperty("content", Buy.repeat)
                                ).ToString());
                                return;
                            }
                        if (ChangeMoney(json.playername, "Deduct", money))
                        {
                            data.Add(new JObject(
                                new JProperty("name", selected[0]),
                                new JProperty("owner", json.playername),
                                new JProperty("attack", selected[1]),
                                new JProperty("chest", selected[2]),
                                new JProperty("destroy", selected[3]),
                                new JProperty("placed", selected[4]),
                                new JProperty("useitem", selected[5]),
                                new JProperty("explode", selected[6]),
                                new JProperty("position", new JObject(
                                    new JProperty("x1", x1),
                                    new JProperty("y1", y1),
                                    new JProperty("z1", z1),
                                    new JProperty("x2", x2),
                                    new JProperty("y2", y2),
                                    new JProperty("z2", z2)
                                )),
                                new JProperty("share", new JArray()),
                                new JProperty("sell", false),
                                new JProperty("price", 0)
                            ));
                            File.WriteAllText(dataFile, JsonFomat(data.ToString()));
                            var success = new Thread(new ParameterizedThreadStart(Land_Do));
                            success.Name = "领地创建成功线程";
                            success.Start(new JObject(
                                new JProperty("name", json.playername),
                                new JProperty("content", string.Format(Buy.success, selected[0]))
                            ).ToString());
                        }
                        else
                        {
                            var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                            failed.Name = "余额不足线程";
                            failed.Start(new JObject(
                                new JProperty("name", json.playername),
                                new JProperty("content", "余额不足！")
                            ).ToString());
                        }
                    }
                );
            }
            else
            {
                bool unkown = true;
                var data = Land_Reload(dataFile);
                foreach (JObject land in data)
                {
                    bool x = position.x <= (int)land["position"]["x1"] && position.x >= (int)land["position"]["x2"];
                    bool y = position.y <= (int)land["position"]["y1"] && position.y >= (int)land["position"]["y2"];
                    bool z = position.z <= (int)land["position"]["z1"] && position.z >= (int)land["position"]["z2"];
                    if (x && y && z)
                    {
                        if ((string)land["owner"] == name)
                        {
                            var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                            failed.Name = "领地无法购买线程";
                            failed.Start(new JObject(
                                new JProperty("name", name),
                                new JProperty("content", Buy.owner)
                            ));
                        }
                        else if ((bool)land["sell"])
                        {
                            sendModalForm(uuid, title, Buy.sellY, "确定", "取消"
                                , json =>
                                {
                                    if (json.selected == "null" || json.selected == "false")
                                        return;
                                    if (ChangeMoney(json.playername, "Deduct", (int)land["price"]))
                                    {
                                        land["owner"] = json.playername;
                                        land["share"] = new JArray();
                                        land["sell"] = false;
                                        land["price"] = 0;
                                        File.WriteAllText(dataFile, JsonFomat(data.ToString()));
                                        var success = new Thread(new ParameterizedThreadStart(Land_Do));
                                        success.Name = "领地购买成功线程";
                                        success.Start(new JObject(
                                            new JProperty("name", json.playername),
                                            new JProperty("content", string.Format(Buy.buy, land["name"]))
                                        ).ToString());
                                    }
                                    else
                                    {
                                        var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                                        failed.Name = "余额不足线程";
                                        failed.Start(new JObject(
                                            new JProperty("name", json.playername),
                                            new JProperty("content", "余额不足！")
                                        ).ToString());
                                    }
                                });
                        }
                        else
                        {
                            var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                            failed.Name = "领地购买失败线程";
                            failed.Start(new JObject(
                                new JProperty("name", name),
                                new JProperty("content", Buy.sellN)
                            ));
                        }
                        unkown = false;
                        break;
                    }
                }
                if (unkown)
                {
                    var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                    failed.Name = "未知领地线程";
                    failed.Start(new JObject(
                        new JProperty("name", name),
                        new JProperty("content", Buy.unkown)
                    ).ToString());
                }
            }
        }

        #endregion

        #region 领地出售

        private static void Land_Sell(string name, int dimensionid, Vec3 position)
        {
            if (dimensionid != 0)
                return;
            bool found = false;
            var data = Land_Reload(dataFile);
            foreach (JObject land in data)
            {
                bool X = position.x <= (int)land["position"]["x1"] && position.x >= (int)land["position"]["x2"];
                bool Y = position.y <= (int)land["position"]["y1"] && position.y >= (int)land["position"]["y2"];
                bool Z = position.z <= (int)land["position"]["z1"] && position.z >= (int)land["position"]["y2"];
                if (X && Y && Z)
                {
                    if ((string)land["owner"] == name)
                    {
                        sendCustomForm(getUuid(name), new JObject(
                                new JProperty("title", title),
                                new JProperty("type", "custom_form"),
                                new JProperty("content", new JArray(
                                    new JObject(
                                        new JProperty("text", Sell.price),
                                        new JProperty("type", "input"),
                                        new JProperty("placeholder", ""),
                                        new JProperty("default", "")
                                    ),
                                    new JObject(
                                        new JProperty("text", Sell.whether),
                                        new JProperty("type", "toggle"),
                                        new JProperty("default", false)
                                    )
                                ))
                            ).ToString()
                            , json =>
                            {
                                if (json.selected == "null")
                                    return;
                                string uuid = getUuid(json.playername);
                                var selected = JsonConvert.DeserializeObject<ArrayList>(json.selected);
                                if ((bool)selected[1])
                                {
                                    int x = (int)land["position"]["x1"] - (int)land["position"]["x2"];
                                    int y = (int)land["position"]["y1"] - (int)land["position"]["y2"];
                                    int z = (int)land["position"]["z1"] - (int)land["position"]["z2"];
                                    int money = Convert.ToInt32(x * y * z * (float)config["sell"]);
                                    ChangeMoney(json.playername, "Add", money);
                                    data.Remove(land);
                                    File.WriteAllText(dataFile, JsonFomat(data.ToString()));
                                    var success = new Thread(new ParameterizedThreadStart(Land_Do));
                                    success.Name = "领地回收成功线程";
                                    success.Start(new JObject(
                                        new JProperty("name", json.playername),
                                        new JProperty("content", Sell.recycle)
                                    ).ToString());
                                }
                                else
                                {
                                    land["sell"] = true;
                                    land["price"] = int.Parse((string)selected[0]);
                                    File.WriteAllText(dataFile, JsonFomat(data.ToString()));
                                    var success = new Thread(new ParameterizedThreadStart(Land_Do));
                                    success.Name = "领地出售成功线程";
                                    success.Start(new JObject(
                                        new JProperty("name", json.playername),
                                        new JProperty("content", Sell.sell)
                                    ).ToString());
                                }
                            });
                        return;
                    }
                    var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                    failed.Name = "不是领地主人线程";
                    failed.Start(new JObject(
                        new JProperty("name", name),
                        new JProperty("content", Sell.isnot)
                    ).ToString());
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                var failed = new Thread(new ParameterizedThreadStart(Land_Do));
                failed.Name = "未知领地线程";
                failed.Start(new JObject(
                    new JProperty("name", name),
                    new JProperty("content", Sell.unkown)
                ).ToString());
            }
        }

        #endregion

        #region 领地保护操作

        /// <summary>
        /// 领地保护
        /// </summary>
        /// <param name="playername">玩家名称</param>
        /// <param name="x">x 坐标</param>
        /// <param name="y">y 坐标</param>
        /// <param name="z">z 坐标</param>
        /// <param name="dimensionid">维度ID</param>
        /// <param name="type">保护类型</param>
        /// <returns>是否拦截事件</returns>
        private static bool Land_Protect(string playername, float x, float y, float z, int dimensionid, string type)
        {
            if (dimensionid != 0)
                return true;
            foreach (JObject data in Land_Reload(dataFile))
            {
                bool X = x <= (int)data["position"]["x1"] && x >= (int)data["position"]["x2"];
                bool Y = y <= (int)data["position"]["y1"] && y >= (int)data["position"]["y2"];
                bool Z = z <= (int)data["position"]["z1"] && z >= (int)data["position"]["y2"];
                if (type == "explode")
                    data[type] = !(bool)data[type];
                if (X && Y && Z)
                {
                    if (!(bool)data[type] && playername != (string)data["owner"])
                    {
                        bool can = true;
                        foreach (string name in data["share"])
                            if (playername == name)
                                return true;
                        switch (type)
                        {
                            case "attack":
                            case "explode":
                                can = false;
                                break;
                            case "useitem":
                                can = !Placed[playername];
                                break;
                            case "placed":
                                can = !Opened[playername];
                                break;
                            default:
                                break;
                        }
                        if (can)
                            tellraw(playername, Protect.CannotUseLand);
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }

        #endregion

        #region 领地选择操作 & 领地保护操作

        /// <summary>
        /// 设置玩家加入游戏监听
        /// </summary>
        private static bool PlayerJoin(Events x)
        {
            var json = BaseEvent.getFrom(x) as LoadNameEvent;
            onselect[json.playername] = false;
            Placed[json.playername] = false;
            player[json.playername] = true;
            return true;
        }

        /// <summary>
        /// 设置玩家使用物品监听
        /// </summary>
        /// <returns>是否拦截物品使用</returns>
        private static bool UseItem(Events x)
        {
            var json = BaseEvent.getFrom(x) as UseItemEvent;
            if (json.itemid == 280 && player[json.playername] && json.dimensionid == 0 && onselect[json.playername])
            {
                player[json.playername] = false;
                Thread.Sleep(300);
                player[json.playername] = true;
                PosB[json.playername] = new Dictionary<string, int>()
                {
                    { "x", json.position.x },
                    { "y", json.position.y },
                    { "z", json.position.z }
                };
                tellraw(json.playername, Do.successB);
                return false;
            }
            return Land_Protect(json.playername, json.XYZ.x, json.XYZ.y, json.XYZ.z, json.dimensionid, "useitem");
        }

        /// <summary>
        /// 设置玩家攻击监听
        /// </summary>
        /// <returns>是否拦截玩家攻击</returns>
        private static bool Attack(Events x)
        {
            var json = BaseEvent.getFrom(x) as AttackEvent;
            if (json.actortype == "entity.player.name")
                return Land_Protect(json.playername, json.XYZ.x, json.XYZ.y, json.XYZ.z, json.dimensionid, "attack");
            return true;
        }
        
        /// <summary>
        /// 设置玩家破坏方块监听
        /// </summary>
        /// <returns>是否拦截玩家破坏方块</returns>
        private static bool DestroyBlock(Events x)
        {
            var json = BaseEvent.getFrom(x) as DestroyBlockEvent;
            if (player[json.playername] && onselect[json.playername] && json.dimensionid == 0)
            {
                player[json.playername] = false;
                Thread.Sleep(300);
                player[json.playername] = true;
                if (mcapi.COMMERCIAL)
                {
                    var selectItem = JObject.Parse(mcapi.getPlayerSelectedItem(getUuid(json.playername)));
                    if ((string)selectItem["selecteditem"]["tv"][2]["cv"]["tv"] == "minecraft:stick")
                    {
                        PosA[json.playername] = new Dictionary<string, int>()
                        {
                            { "x", json.position.x },
                            { "y", json.position.y },
                            { "z", json.position.z }
                        };
                        tellraw(json.playername, Do.successA);
                        return false;
                    }
                }
                else
                    Console.WriteLine("Land >> 请使用商业版BDSNetRunner，否则本插件部分功能无法使用！");
            }
            return Land_Protect(json.playername, json.position.x, json.position.y, json.position.z, json.dimensionid, "destroy");
        }

        /// <summary>
        /// 设置玩家放置方块监听
        /// </summary>
        /// <returns>是否拦截玩家放置方块</returns>
        private static bool PlacedBlock(Events x)
        {
            var json = BaseEvent.getFrom(x) as PlacedBlockEvent;
            var start = new Thread(new ParameterizedThreadStart(Placed_Do));
            start.Name = "玩家放置方块设置线程";
            start.Start(json.playername);
            return Land_Protect(json.playername, json.position.x, json.position.y, json.position.z, json.dimensionid, "placed");
        }
        
        /// <summary>
        /// 设置玩家开箱监听
        /// </summary>
        /// <returns>是否拦截玩家开箱</returns>
        private static bool StartOpenChest(Events x)
        {
            var json = BaseEvent.getFrom(x) as StartOpenChestEvent;
            var start = new Thread(new ParameterizedThreadStart(Opened_Do));
            start.Name = "玩家开箱设置线程";
            start.Start(json.playername);
            return Land_Protect(json.playername, json.position.x, json.position.y, json.position.z, json.dimensionid, "chest");
        }
        
        /* /// <summary>
        /// 设置玩家开桶监听
        /// </summary>
        /// <returns>是否拦截玩家开桶</returns>
        private static bool StartOpenBarrel(Events x)
        {
            var json = BaseEvent.getFrom(x) as StartOpenBarrelEvent;
            return Land_Protect(json.playername, json.position.x, json.position.y, json.position.z, json.dimensionid, Protect.CannotOpenChest);
        } */

        private static bool LevelExplode(Events x)
        {
            var json = BaseEvent.getFrom(x) as LevelExplodeEvent;
            return Land_Protect(null, json.position.x, json.position.y, json.position.z, json.dimensionid, "explode");
        }

        #endregion

        #region 后台命令

        /// <summary>
        /// 后台指令监听
        /// </summary>
        /// <returns>是否拦截后台命令</returns>
        private static bool ServerCmd(Events x)
        {
			var json = BaseEvent.getFrom(x) as ServerCmdEvent;
            switch (json.cmd)
            {
                case "land reload":
                    if (File.Exists(configFile))
                    {
                        try
                        {
                            var reader = new StreamReader(configFile);
                            var deserializer = new DeserializerBuilder().Build();
                            var serializer = new SerializerBuilder()
                                .JsonCompatible()
                                .Build();
                            var str = serializer.Serialize(deserializer.Deserialize(reader));
                            config = JObject.Parse(str);
                            Console.WriteLine("Land >> 配置文件 config.yml 读取成功！");
                        }
                        catch { Console.WriteLine("Land >> 配置文件 config.yml 读取失败！"); }
                    }
                    else
                    {
                        initConfig();
                        Console.WriteLine("Land >> 未找到配置文件 config.yml，正在为您生成！");
                    }
                    if (File.Exists(languageFile))
                        reload();
                    else
                    {
                        initLanguage();
                        Console.WriteLine("Land >> 未找到配置文件 language.yml，正在为您生成！");
                    }
                    return false;
                default:
                    break;
            }
            return true;
        }

        #endregion

        #region 玩家命令

        /// <summary>
        /// 设置玩家命令监听
        /// </summary>
        /// <returns>是否拦截玩家命令</returns>
        private static bool InputCommand(Events x)
        {
            var json = BaseEvent.getFrom(x) as InputCommandEvent;
            switch (json.cmd)
            {
                case "/land":
                    Land_Open(json.playername);
                    return false;
                default:
                    break;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <param name="api">MC相关API方法</param>
        public static void init(MCCSAPI api)
        {
            mcapi = api;

            initPlugin();

            #region 注册命令

            mcapi.setCommandDescribe("land", "打开领地菜单");

            #endregion

            #region 设置事件监听器

            api.addAfterActListener(EventKey.onLoadName, PlayerJoin);
            api.addBeforeActListener(EventKey.onAttack, Attack);
            api.addBeforeActListener(EventKey.onUseItem, UseItem);
            api.addBeforeActListener(EventKey.onServerCmd, ServerCmd);
            api.addBeforeActListener(EventKey.onPlacedBlock, PlacedBlock);
            api.addBeforeActListener(EventKey.onInputCommand, InputCommand);
            api.addBeforeActListener(EventKey.onDestroyBlock, DestroyBlock);
            api.addBeforeActListener(EventKey.onLevelExplode, LevelExplode);
            api.addBeforeActListener(EventKey.onStartOpenChest, StartOpenChest);
            // api.addBeforeActListener(EventKey.onStartOpenBarrel, StartOpenBarrel);

            #endregion
        }
    }
}


namespace CSR
{
    partial class Plugin
    {
        /// <summary>
        /// 静态api对象
        /// </summary>
        public static MCCSAPI api { get; private set; } = null;

        /// <summary>
        /// 插件装载时的事件
        /// </summary>
        public static int onServerStart(string pathandversion)
        {
            string[] pav = pathandversion.Split(',');
            if (pav.Length > 1)
            {
                string path = pav[0];
                string version = pav[1];
                bool commercial = (pav[pav.Length - 1] == "1");
                api = new MCCSAPI(path, version, commercial);
                if (api != null)
                {
                    onStart(api);
                    GC.KeepAlive(api);
                    return 0;
                }
            }
            Console.WriteLine("Load failed.");
            return -1;
        }

        /// <summary>
        /// 通用调用接口
        /// </summary>
        /// <param name="api">MC相关调用方法</param>
        public static void onStart(MCCSAPI api)
        {
            // TODO 此接口为必要实现
            Land.Land.init(api);
            Console.WriteLine("[{0}]{1}插件加载成功！ By：{2}", Land.Land.Version, Land.Land.PluginName, Land.Land.Author);
        }
    }
}
