using System;
using System.IO;
using Newtonsoft.Json.Linq;
using static Land.Function;
using static Land.Land;

namespace Land
{
    internal class Language
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        public static string title { get; set; }
        
        /// <summary>
        /// 菜单内容
        /// </summary>
        public static string content { get; set; }

        public static class Open
        {
            /// <summary>
            /// 进入领地选择模式
            /// </summary>
            public static string select { get; set; }

            /// <summary>
            /// 退出领地选择模式
            /// </summary>
            public static string unselect { get; set; }

            /// <summary>
            /// 购买领地
            /// </summary>
            public static string buy { get; set; }

            /// <summary>
            /// 领地出售
            /// </summary>
            public static string sell { get; set; }

            /// <summary>
            /// 传送到我的领地
            /// </summary>
            public static string go { get; set; }

            /// <summary>
            /// 查询脚下领地信息
            /// </summary>
            public static string search { get; set; }

            /// <summary>
            /// 分享你拥有的领地
            /// </summary>
            public static string share { get; set; }

            /// <summary>
            /// 领地权限设置
            /// </summary>
            public static string settings { get; set; }
        }

        public static class Do
        {
            /// <summary>
            /// 已进入领地模式
            /// </summary>
            public static string select { get; set; }
            
            /// <summary>
            /// 已退出领地模式
            /// </summary>
            public static string unselect { get; set; }
            
            /// <summary>
            /// 设置点 A
            /// </summary>
            public static string setA { get; set; }
            
            /// <summary>
            /// 设置点 B
            /// </summary>
            public static string setB { get; set; }

            /// <summary>
            /// A 点设置成功
            /// </summary>
            public static string successA { get; set; }

            /// <summary>
            /// B 点设置成功
            /// </summary>
            public static string successB { get; set; }
        }

        public static class Buy
        {
            /// <summary>
            /// 领地名重复
            /// </summary>
            public static string repeat { get; set; }
            
            /// <summary>
            /// 领地位置重合
            /// </summary>
            public static string intersect { get; set; }
            
            /// <summary>
            /// 领地购买
            /// </summary>
            public static string sellY { get; set; }
            
            /// <summary>
            /// 领地未出售
            /// </summary>
            public static string sellN { get; set; }
            
            /// <summary>
            /// 领地创建成功
            /// </summary>
            public static string success { get; set; }
            
            /// <summary>
            /// 领地购买成功
            /// </summary>
            public static string buy { get; set; }

            /// <summary>
            /// 已拥有领地
            /// </summary>
            public static string owner { get; set; }

            /// <summary>
            /// 未知领地
            /// </summary>
            public static string unkown { get; set; }
            
            /// <summary>
            /// 输入领地名称
            /// </summary>
            public static string set { get; set; }
            
            /// <summary>
            /// 是否允许玩家PVP
            /// </summary>
            public static string attack { get; set; }
            
            /// <summary>
            /// 是否允许玩家打开箱子
            /// </summary>
            public static string chest { get; set; }
            
            /// <summary>
            /// 是否允许玩家破坏方块
            /// </summary>
            public static string destroy { get; set; }
            
            /// <summary>
            /// 是否允许玩家放置方块
            /// </summary>
            public static string placed { get; set; }
            
            /// <summary>
            /// 是否允许玩家使用物品
            /// </summary>
            public static string useitem { get; set; }

            /// <summary>
            /// 是否开启防爆
            /// </summary>
            public static string explode { get; set; }
        }

        public static class Sell
        {
            /// <summary>
            /// 输入领地价格
            /// </summary>
            public static string price { get; set; }
            
            /// <summary>
            /// 是否回收领地
            /// </summary>
            public static string whether { get; set; }
            
            /// <summary>
            /// 领地回收成功
            /// </summary>
            public static string recycle { get; set; }
            
            /// <summary>
            /// 领地出售成功
            /// </summary>
            public static string sell { get; set; }

            /// <summary>
            /// 不是领地主人 
            /// </summary>
            public static string isnot { get; set; }

            /// <summary>
            /// 未知领地 
            /// </summary>
            public static string unkown { get; set; }
        }

        public static class Modify
        {
        }
        
        public static class Share
        {
        }
        
        public static class Protect
        {
            /// <summary>
            /// 领地操作权限
            /// </summary>
            public static string CannotUseLand { get; set; }
        }

        public static class Image
        {
            /// <summary>
            /// 进入领地选择模式
            /// </summary>
            public static string select { get; set; }
            
            /// <summary>
            /// 退出领地选择模式
            /// </summary>
            public static string unselect { get; set; }
            
            /// <summary>
            /// 购买领地
            /// </summary>
            public static string buy { get; set; }
            
            /// <summary>
            /// 领地出售
            /// </summary>
            public static string sell { get; set; }
            
            /// <summary>
            /// 传送到我的领地
            /// </summary>
            public static string go { get; set; }
            
            /// <summary>
            /// 查询脚下领地信息
            /// </summary>
            public static string search { get; set; }
            
            /// <summary>
            /// 分享你拥有的领地
            /// </summary>
            public static string share { get; set; }
            
            /// <summary>
            /// 领地权限设置
            /// </summary>
            public static string settings { get; set; }
        }

        public static void reload()
        {
            try
            {
                var language = JObject.Parse(ToJson(languageFile));
                /* var reader = new StreamReader(languageFile);
                var Yaml = new YamlStream();
                Yaml.Load(reader);
                var language = Yaml.Documents[0].RootNode as YamlMappingNode;
                title = (string)language.Children[new YamlScalarNode("title")];
                content = (string)language.Children[new YamlScalarNode("content")];
                Open.select = (string)language.Children[new YamlScalarNode("open.select")];
                Open.unselect = (string)language.Children[new YamlScalarNode("open.unselect")];
                Open.buy = (string)language.Children[new YamlScalarNode("open.buy")];
                Open.sell = (string)language.Children[new YamlScalarNode("open.sell")];
                Open.go = (string)language.Children[new YamlScalarNode("open.go")];
                Open.search = (string)language.Children[new YamlScalarNode("open.search")];
                Open.share = (string)language.Children[new YamlScalarNode("open.share")];
                Open.settings = (string)language.Children[new YamlScalarNode("open.settings")]; */
                title = (string)language["title"];
                content = (string)language["content"];
                Open.select = (string)language["open.select"];
                Open.unselect = (string)language["open.unselect"];
                Open.buy = (string)language["open.buy"];
                Open.sell = (string)language["open.sell"];
                Open.go = (string)language["open.go"];
                Open.search = (string)language["open.search"];
                Open.share = (string)language["open.share"];
                Open.settings = (string)language["open.settings"];
                Do.select = (string)language["do.select"];
                Do.unselect = (string)language["do.unselect"];
                Do.setA = (string)language["do.setA"];
                Do.setB = (string)language["do.setB"];
                Do.successA = (string)language["do.successA"];
                Do.successB = (string)language["do.successB"];
                Buy.repeat = (string)language["buy.repeat"];
                Buy.intersect = (string)language["buy.intersect"];
                Buy.sellY = (string)language["buy.sellY"];
                Buy.sellN = (string)language["buy.sellN"];
                Buy.success = (string)language["buy.success"];
                Buy.buy = (string)language["buy.buy"];
                Buy.owner = (string)language["buy.owner"];
                Buy.unkown = (string)language["buy.unkown"];
                Buy.set = (string)language["buy.set"];
                Buy.attack = (string)language["buy.attack"];
                Buy.chest = (string)language["buy.chest"];
                Buy.destroy = (string)language["buy.destroy"];
                Buy.placed = (string)language["buy.placed"];
                Buy.useitem = (string)language["buy.useitem"];
                Buy.explode = (string)language["buy.explode"];
                Sell.price = (string)language["sell.price"];
                Sell.whether = (string)language["sell.whether"];
                Sell.recycle = (string)language["sell.recycle"];
                Sell.sell = (string)language["sell.sell"];
                Sell.isnot = (string)language["sell.isnot"];
                Sell.unkown = (string)language["sell.unkown"];
                Protect.CannotUseLand = (string)language["protect.CannotUseLand"];
                Image.select = (string)language["image.select"];
                Image.unselect = (string)language["image.unselect"];
                Image.buy = (string)language["image.buy"];
                Image.sell = (string)language["image.sell"];
                Image.go = (string)language["image.go"];
                Image.search = (string)language["image.search"];
                Image.share = (string)language["image.share"];
                Image.settings = (string)language["image.settings"];
                Console.WriteLine("Land >> 配置文件 language.yml 读取成功！");
            }
            catch { Console.WriteLine("Land >> 配置文件 language.yml 读取失败！"); }
        }
    }
}