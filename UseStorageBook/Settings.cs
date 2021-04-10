using BepInEx.Configuration;
using System.Linq;

namespace UseStorageBook
{
    /// <summary>
    /// 设置
    /// </summary>
    public class Settings
    {
        #region 静态公共字段

        /// <summary>
        /// 书本来源
        /// </summary>
        public static readonly string[] BookSource = { "背包", "仓库" };

        /// <summary>
        /// 书本类型
        /// </summary>
        public static readonly string[] BookType = { "真传", "手抄" };

        /// <summary>
        /// 阅读进度
        /// </summary>
        public static readonly string[] ReadStatus = { "未读", "已读", "读完" };

        /// <summary>
        /// 书本品级
        /// </summary>
        public static readonly string[] BookLevel = { "九品", "八品", "七品", "六品", "五品", "四品", "三品", "二品", "一品" };

        /// <summary>
        /// 书本功法
        /// </summary>
        public static readonly string[] BookGongFa = { "内功", "轻功", "绝技", "拳掌", "指法", "腿法", "暗器", "剑法", "刀法", "长兵", "奇门", "软兵", "御射", "乐器" };

        /// <summary>
        /// 书本门派
        /// </summary>
        public static readonly string[] BookSect = { "无门派", "少林派", "峨眉派", "百花谷", "武当派", "元山派", "狮相门", "然山派", "璇女派", "铸剑山庄", "空桑派", "无量金刚宗", "五仙教", "界青门", "伏龙坛", "血犼教" };

        #endregion 静态公共字段

        #region 公共字段

        /// <summary>
        /// 是否启用
        /// </summary>
        public ConfigEntry<bool> Enabled;

        /// <summary>
        /// 来源，背包或仓库
        /// </summary>
        public ConfigEntry<bool[]> Source;

        /// <summary>
        /// 阅读进度
        /// </summary>
        public ConfigEntry<bool[]> Status;

        /// <summary>
        /// 类型，真传或手抄
        /// </summary>
        public ConfigEntry<bool[]> Type;

        /// <summary>
        /// 品级，九至一品
        /// </summary>
        public ConfigEntry<bool[]> Level;

        /// <summary>
        /// 功法类型，内功、身法、绝技...
        /// </summary>
        public ConfigEntry<bool[]> GongFa;

        /// <summary>
        /// 门派
        /// </summary>
        public ConfigEntry<bool[]> Sect;

        #endregion 公共字段

        #region 设置初始化方法

        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="Config">持久化配置文件</param>
        public void Init(ConfigFile Config)
        {
            Enabled = Config.Bind(nameof(Settings), nameof(Enabled), true, "是否开启 Mod");
            Source = Config.Bind(nameof(Settings), nameof(Source), Enumerable.Repeat(true, BookSource.Length).ToArray(), "背包/仓库");
            Type = Config.Bind(nameof(Settings), nameof(Type), Enumerable.Repeat(true, BookType.Length).ToArray(), "真传/手抄");
            Status = Config.Bind(nameof(Settings), nameof(Status), Enumerable.Repeat(true, ReadStatus.Length).ToArray(), "阅读进度");
            Level = Config.Bind(nameof(Settings), nameof(Level), Enumerable.Repeat(true, BookLevel.Length).ToArray(), "品级");
            GongFa = Config.Bind(nameof(Settings), nameof(GongFa), Enumerable.Repeat(true, BookGongFa.Length).ToArray(), "功法");
            Sect = Config.Bind(nameof(Settings), nameof(Sect), Enumerable.Repeat(true, BookSect.Length).ToArray(), "门派");
        }

        #endregion 设置初始化方法

        #region 公共方法

        public void LevelSetAll(bool value)
        {
            Level.Value = Enumerable.Repeat(value, BookLevel.Length).ToArray();
        }

        public void GongFaSetAll(bool value)
        {
            GongFa.Value = Enumerable.Repeat(value, BookGongFa.Length).ToArray();
        }

        public void SectSetAll(bool value)
        {
            Sect.Value = Enumerable.Repeat(value, BookSect.Length).ToArray();
        }

        #endregion
    }

    /// <summary>
    /// 设置类型
    /// </summary>
    public enum SettingType
    {
        /// <summary>
        /// 启用/禁用
        /// </summary>
        Enabled,

        /// <summary>
        /// 背包/仓库
        /// </summary>
        Source,

        /// <summary>
        /// 真传/手抄
        /// </summary>
        Type,

        /// <summary>
        /// 阅读进度
        /// </summary>
        Status,

        /// <summary>
        /// 品级
        /// </summary>
        Level,

        /// <summary>
        /// 功法类型
        /// </summary>
        GongFa,

        /// <summary>
        /// 门派
        /// </summary>
        Sect
    }
}
