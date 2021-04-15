using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StorageCheck.Models;
using System;
using TaiwuUIKit.GameObjects;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using YanLib.ModHelper;
using static UnityEngine.UI.ContentSizeFitter;

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
        public const string ModVersion = "1.1.1";

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Settings = new Settings();
            Settings.Init(Config);
            var harmony = new Harmony(ModId);
            if (Settings.Enabled.Value)
            {
                harmony.PatchAll();
            }
            ModHelper = new ModHelper(ModId, $"{ModName}.{ModVersion}");
            ModLogger = base.Logger;

            ModHelper.SettingUI = new BoxAutoSizeModelGameObject
            {
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 10,
                    Padding = {0, 10, 0, 0}
                },
                SizeFitter = { VerticalFit = FitMode.PreferredSize },
                Children =
                {
                    // Mod开关
                    new TaiwuToggle
                    {
                        Name = $"{ModId}.Enable.Toggle",
                        Text = Settings.Enabled.Value ? "库存检查 开" : "库存检查 关",
                        Element = { PreferredSize = { 0, 50 } },
                        isOn = Settings.Enabled.Value,
                        onValueChanged = (value, sender) =>
                        {
                            Settings.Enabled.Value = value;
                            sender.Text = value ? "库存检查 开" : "库存检查 关";
                            try
                            {
                                if(value)
                                    harmony.PatchAll();
                                else
                                    harmony.UnpatchAll();
                            }
                            catch (Exception ex)
                            {
                                ModLogger.LogError($"尝试{(value ? "加载" : "卸载")}Harmony补丁时出现错误。{ex.Message}");
                            }
                            foreach (var child in sender.Parent.Children)
                            {
                                if (child.Name != sender.Name)
                                {
                                    child.SetActive(value);
                                }
                            }
                        }
                    },
                    // 选项开关
                    new Container
                    {
    			        Element = { PreferredSize = { 0 , 50 } },
                        Group =
                        {
    				        Spacing = 10,
    				        Direction = Direction.Horizontal,
                        },
    			        DefaultActive = Settings.Enabled.Value,
                        Children =
                        {
                            new TaiwuToggle()
                            {
                                Text = "检查背包",
                                Element = { PreferredSize = { 0, 50 } },
                                TipTitle = "说明",
                                TipContant = "是否统计背包中的物品",
                                isOn = Settings.CheckBag.Value,
                                onValueChanged = (value, _) =>
                                {
                                    Settings.CheckBag.Value = value;
                                    ItemInfo.ResetCurrentItem();
                                }
                            },
                            new TaiwuToggle()
                            {
                                Text = "检查仓库",
                                Element = { PreferredSize = { 0, 50 } },
                                TipTitle = "说明",
                                TipContant = "是否统计仓库中的物品",
                                isOn = Settings.CheckWarehouse.Value,
                                onValueChanged = (value, _) =>
                                {
                                    Settings.CheckWarehouse.Value = value;
                                    ItemInfo.ResetCurrentItem();
                                }
                            },
                            new TaiwuToggle()
                            {
                                Text = "书籍信息",
                                Element = { PreferredSize = { 0, 50 } },
                                TipTitle = "说明",
                                TipContant = "是否显示书籍信息对比",
                                isOn = Settings.ShowBookInfo.Value,
                                onValueChanged = (value, _) =>
                                {
                                    Settings.ShowBookInfo.Value = value;
                                    ItemInfo.ResetCurrentItem();
                                }
                            },
                            new TaiwuToggle()
                            {
                                Text = "真传手抄",
                                Element = { PreferredSize = { 0, 50 } },
                                TipTitle = "说明",
                                TipContant = "是否分别显示真传手抄页数\n关闭则显示书籍已有页数",
                                isOn = Settings.ShowBookInfo.Value,
                                onValueChanged = (value, _) =>
                                {
                                    Settings.ShowBookInfo.Value = value;
                                    ItemInfo.ResetCurrentItem();
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
