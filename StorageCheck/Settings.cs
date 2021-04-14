using BepInEx.Configuration;

namespace StorageCheck
{
    public class Settings
    {
        #region 公共字段

        /// <summary>
        /// 是否启用
        /// </summary>
        public ConfigEntry<bool> Enabled;

        /// <summary>
        /// 是否显示背包
        /// </summary>
        public ConfigEntry<bool> ShowBag;

        /// <summary>
        /// 是否显示仓库
        /// </summary>
        public ConfigEntry<bool> ShowWarehouse;

        /// <summary>
        /// 是否显示书本信息
        /// </summary>
        public ConfigEntry<bool> ShowBookInfo;

        /// <summary>
        /// 是否显示页数
        /// </summary>
        public ConfigEntry<bool> ShowGoodPage;

        #endregion 公共字段

        #region 设置初始化方法

        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="config">持久化配置文件</param>
        public void Init(ConfigFile config)
        {
            Enabled = config.Bind(nameof(Settings), nameof(Enabled), true, "是否开启 Mod");
            ShowBag = config.Bind(nameof(Settings), nameof(ShowBag), true, "是否显示背包");
            ShowWarehouse = config.Bind(nameof(Settings), nameof(ShowWarehouse), true, "是否显示仓库");
            ShowBookInfo = config.Bind(nameof(Settings), nameof(ShowBookInfo), true, "是否显示书本信息");
            ShowGoodPage = config.Bind(nameof(Settings), nameof(ShowGoodPage), true, "是否显示页数");
        }

        #endregion 设置初始化方法
    }
}
