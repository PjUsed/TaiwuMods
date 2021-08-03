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
        /// 是否检查背包
        /// </summary>
        public ConfigEntry<bool> CheckBag;

        /// <summary>
        /// 是否检查仓库
        /// </summary>
        public ConfigEntry<bool> CheckWarehouse;

        /// <summary>
        /// 是否显示书籍信息
        /// </summary>
        public ConfigEntry<bool> ShowBookInfo;

        /// <summary>
        /// 是否分别显示真传手抄
        /// </summary>
        public ConfigEntry<bool> ShowBookPage;

        #endregion 公共字段

        #region 设置初始化方法

        /// <summary>
        /// 初始化设置
        /// </summary>
        /// <param name="config">持久化配置文件</param>
        public void Init(ConfigFile config)
        {
            Enabled = config.Bind(nameof(Settings), nameof(Enabled), true, "是否开启 Mod");
            CheckBag = config.Bind(nameof(Settings), nameof(CheckBag), true, "是否检查背包");
            CheckWarehouse = config.Bind(nameof(Settings), nameof(CheckWarehouse), true, "是否检查仓库");
            ShowBookInfo = config.Bind(nameof(Settings), nameof(ShowBookInfo), true, "是否显示书籍信息");
            ShowBookPage = config.Bind(nameof(Settings), nameof(ShowBookPage), true, "是否分别显示真传手抄");
        }

        #endregion 设置初始化方法
    }
}
