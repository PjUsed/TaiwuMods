using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using YanLib.ModHelper;

namespace StorageCheck
{
    [BepInPlugin(ModId, ModName, ModVersion)]
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]
    [BepInDependency(YanLib.YanLib.GUID)]
    public class StorageCheck : BaseUnityPlugin
    {
        public static ModHelper ModHelper { get; private set; }
        public static ManualLogSource ModLogger { get; private set; }
        public static Settings Settings { get; private set; }
        public static bool IsEnable => Settings.Enabled.Value;

        public const string ModId = "TaiwuMod.plugins.StorageCheck";
        public const string ModName = "StorageCheck/库存检查";
        public const string ModVersion = "1.1.0";

        #region 控件名称设置

        private readonly string _settingUIContainerName = $"{ModId}.Container";
        private readonly string _enableContainerName = $"{ModId}.Enable.Container";
        private readonly string _enableContainerToggleName = $"{ModId}.Enable.Toggle";
        private readonly string _showBagContainerName = $"{ModId}.ShowBag.Container";
        private readonly string _showBagContainerToggleName = $"{ModId}.ShowBag.Toggle";
        private readonly string _showWarehouseContainerName = $"{ModId}.ShowWarehouse.Container";
        private readonly string _showWarehouseContainerToggleName = $"{ModId}.ShowWarehouse.Toggle";
        private readonly string _showBookInfoContainerName = $"{ModId}.ShowBookInfo.Container";
        private readonly string _showBookInfoContainerToggleName = $"{ModId}.ShowBookInfo.Toggle";
        private readonly string _showGoodPageContainerName = $"{ModId}.ShowGoodPage.Container";
        private readonly string _showGoodPageContainerToggleName = $"{ModId}.ShowGoodPage.Toggle";

        #endregion

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Settings = new Settings();
            Settings.Init(Config);
            new Harmony(ModId).PatchAll();
            ModHelper = new ModHelper(ModId, $"{ModName}.{ModVersion}");
            ModLogger = base.Logger;

            var container = new BoxAutoSizeModelGameObject
            {
                Name = _settingUIContainerName
            };
            container.Group.Direction = Direction.Vertical;
            container.Group.Spacing = 5;
            container.SizeFitter.VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            // 容器高度，文本宽度，全选宽度
            const float containerHeight = 60, labelWidth = 400;

            #region Mod启用禁用开关

            var enableContainer = new Container
            {
                Name = _enableContainerName,
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4,
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = $"{_enableContainerName}.Label",
                        Text = "Mod开关",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = _enableContainerToggleName,
                        Text = Settings.Enabled.Value ? "开" : "关",
                        FontColor = Color.white,
                        isOn = Settings.Enabled.Value,
                        onValueChanged = (value, sender) =>
                        {
                            Settings.Enabled.Value = value;
                            sender.Text = Settings.Enabled.Value ? "开" : "关";
                            foreach (var child in sender.Parent.Parent.Children)
                            {
                                if (child.Name.StartsWith(ModId) && !child.Name.Contains("Enable"))
                                {
                                    child.SetActive(value);
                                }
                            }
                        }
                    }
                }
            };

            container.Children.Add(enableContainer);

            #endregion Mod启用禁用开关

            #region 是否显示背包开关

            var showBagContainer = new Container
            {
                Name = _showBagContainerName,
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4,
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = $"{_showBagContainerName}.Label",
                        Text = "显示背包",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = _showBagContainerToggleName,
                        Text = "启用",
                        FontColor = Color.white,
                        isOn = Settings.ShowBag.Value,
                        onValueChanged = (value, _) => Settings.ShowBag.Value = value
                    }
                }
            };

            container.Children.Add(enableContainer);

            #endregion Mod启用禁用开关

            #region 是否显示仓库开关

            var showWarehouseContainer = new Container
            {
                Name = _showWarehouseContainerName,
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4,
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = $"{_showWarehouseContainerName}.Label",
                        Text = "显示仓库",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = _showWarehouseContainerToggleName,
                        Text = "启用",
                        FontColor = Color.white,
                        isOn = Settings.ShowWarehouse.Value,
                        onValueChanged = (value, _) => Settings.ShowWarehouse.Value = value
                    }
                }
            };

            container.Children.Add(enableContainer);

            #endregion Mod启用禁用开关

            #region 是否显示书籍信息开关

            var showBookInfoContainer = new Container
            {
                Name = _showBookInfoContainerName,
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4,
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = $"{_showBookInfoContainerName}.Label",
                        Text = "显示书籍信息",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = _showBookInfoContainerToggleName,
                        Text = "启用",
                        FontColor = Color.white,
                        isOn = Settings.ShowBookInfo.Value,
                        onValueChanged = (value, _) => Settings.ShowBookInfo.Value = value
                    }
                }
            };

            container.Children.Add(enableContainer);

            #endregion Mod启用禁用开关

            #region 是否显示页数开关

            var showGoodPageContainer = new Container
            {
                Name = _showGoodPageContainerName,
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4,
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = $"{_showGoodPageContainerName}.Label",
                        Text = "是否显示页数",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = _showGoodPageContainerToggleName,
                        Text = "启用",
                        FontColor = Color.white,
                        isOn = Settings.ShowGoodPage.Value,
                        onValueChanged = (value, _) => Settings.ShowGoodPage.Value = value
                    }
                }
            };

            container.Children.Add(enableContainer);

            #endregion Mod启用禁用开关

            ModHelper.SettingUI = container;
        }
    }
}
