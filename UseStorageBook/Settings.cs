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
        /// <param name="config">持久化配置文件</param>
        public void Init(ConfigFile config)
        {
            Enabled = config.Bind(nameof(Settings), nameof(Enabled), true, "是否开启 Mod");
            Source = config.Bind(nameof(Settings), nameof(Source), Enumerable.Repeat(true, BookSource.Length).ToArray(), "背包/仓库");
            Type = config.Bind(nameof(Settings), nameof(Type), Enumerable.Repeat(true, BookType.Length).ToArray(), "真传/手抄");
            Status = config.Bind(nameof(Settings), nameof(Status), Enumerable.Repeat(true, ReadStatus.Length).ToArray(), "阅读进度");
            Level = config.Bind(nameof(Settings), nameof(Level), Enumerable.Repeat(true, BookLevel.Length).ToArray(), "品级");
            GongFa = config.Bind(nameof(Settings), nameof(GongFa), Enumerable.Repeat(true, BookGongFa.Length).ToArray(), "功法");
            Sect = config.Bind(nameof(Settings), nameof(Sect), Enumerable.Repeat(true, BookSect.Length).ToArray(), "门派");
        }

        #endregion 设置初始化方法

        #region 公共方法

        /// <summary>
        /// 设置所有品级
        /// </summary>
        /// <param name="value">值</param>
        public void LevelSetAll(bool value)
        {
            Level.Value = Enumerable.Repeat(value, BookLevel.Length).ToArray();
        }

        /// <summary>
        /// 设置所有功法类型
        /// </summary>
        /// <param name="value">值</param>
        public void GongFaSetAll(bool value)
        {
            GongFa.Value = Enumerable.Repeat(value, BookGongFa.Length).ToArray();
        }

        /// <summary>
        /// 设置所有门派
        /// </summary>
        /// <param name="value">值</param>
        public void SectSetAll(bool value)
        {
            Sect.Value = Enumerable.Repeat(value, BookSect.Length).ToArray();
        }

        /// <summary>
        /// 设置背包/仓库的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void SourceSet(int index, bool value) => SetArraySetting(Source, index, value);

        /// <summary>
        /// 设置真传/手抄的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void TypeSet(int index, bool value) => SetArraySetting(Type, index, value);

        /// <summary>
        /// 设置阅读进度的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void StatusSet(int index, bool value) => SetArraySetting(Status, index, value);

        /// <summary>
        /// 设置品级的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void LevelSet(int index, bool value) => SetArraySetting(Level, index, value);

        /// <summary>
        /// 设置功法类型的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void GongFaSet(int index, bool value) => SetArraySetting(GongFa, index, value);

        /// <summary>
        /// 设置门派的值
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void SectSet(int index, bool value) => SetArraySetting(Sect, index, value);

        #endregion

        #region 私有方法

        /// <summary>
        /// 设置数组设置
        /// <para>配置文件设置为引用类型时，修改内部的值不会触发保存，此处重新给设置赋值</para>
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="entry">数组设置</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        private static void SetArraySetting<T>(ConfigEntry<T[]> entry, int index, T value)
        {
            try
            {
                var array = entry.Value;
                array[index] = value;
                entry.Value = array;
            }
            catch { }
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
