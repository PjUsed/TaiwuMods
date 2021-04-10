using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;
using YanLib.ModHelper;

namespace UseStorageBook
{
    [BepInPlugin(ModId, ModName, ModVersion)]
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]
    [BepInDependency(YanLib.YanLib.GUID)]
    public class UseStorageBook : BaseUnityPlugin
    {
        public static ModHelper ModHelper { get; private set; }
        public static ManualLogSource ModLogger { get; private set; }
        public static Settings Settings { get; private set; }
        public static bool IsEnable => Settings.Enabled.Value;

        public const string ModId = "TaiwuMod.plugins.UseStorageBook";
        public const string ModName = "使用仓库中的书";
        public const string ModVersion = "1.4.0";

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Settings = new Settings();
            Settings.Init(Config);
            new Harmony(ModId).PatchAll();
            ModHelper = new ModHelper(ModId, $"{ModName}-{ModVersion}");
            ModLogger = base.Logger;

            var container = new BoxAutoSizeModelGameObject
            {
                Name = $"{ModId}.Container"
            };
            container.Group.Direction = Direction.Vertical;
            container.Group.Spacing = 5;
            container.SizeFitter.VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            // 容器高度，文本宽度，全选宽度
            const float containerHeight = 60, labelWidth = 400, selectAllWidth = 120;

            #region Mod启用禁用开关

            var enableContainer = new Container
            {
                Name = $"{ModId}-Enable",
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
                        Name = "{ModId}-Enable-Label",
                        Text = "Mod开关",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                    new TaiwuToggle
                    {
                        Name = $"{ModId}-Enable-Toggle",
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

            #region 背包/仓库设置

            var sourceContainer = new Container()
            {
                Name = $"{ModId}-Source",
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4
                },
                Children =
                {
                    new TaiwuLabel
                    {
                        Name = "{ModId}-Source-Label",
                        Text = "背包/仓库",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    }
                }
            };
            sourceContainer.Children.AddRange(Settings.Source.Value
                .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                {
                    Name = $"{ModId}-Source-Toggle-{index}",
                    Text = Settings.BookSource[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.SourceSet(index, value)
                }));

            container.Children.Add(sourceContainer);

            #endregion 背包/仓库设置

            #region 阅读进度设置

            var statusContainer = new Container()
            {
                Name = $"{ModId}-Status",
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = "{ModId}-Status-Label",
                        Text = "阅读进度",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    },
                }
            };
            statusContainer.Children.AddRange(Settings.Status.Value
                .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                {
                    Name = $"{ModId}-Status-Toggle-{index}",
                    Text = Settings.ReadStatus[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.StatusSet(index, value)
                }));

            container.Children.Add(statusContainer);

            #endregion 阅读进度设置

            #region 真传/手抄设置

            var typeContainer = new Container()
            {
                Name = $"{ModId}-Type-Content",
                Element = { PreferredSize = { 0, containerHeight } },
                Group =
                {
                    Direction = Direction.Horizontal,
                    Spacing = 4
                },
                Children =
                {
                    new TaiwuLabel()
                    {
                        Name = "{ModId}-Type-Label",
                        Text = "真传/手抄",
                        Element = { PreferredSize = { labelWidth, 0 } },
                        UseBoldFont = true,
                        UseOutline = true,
                    }
                }
            };
            typeContainer.Children.AddRange(Settings.Type.Value
                .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                {
                    Name = $"{ModId}-Type-Toggle-{index}",
                    Text = Settings.BookType[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.TypeSet(index, value)
                }));

            container.Children.Add(typeContainer);

            #endregion 真传/手抄设置

            #region 品级设置

            var levelContainer = new BoxAutoSizeModelGameObject()
            {
                Name = $"{ModId}-Level-Container",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 5
                },
                SizeFitter = { VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize },
                Children =
                {
                    new Container
                    {
                        Name = $"{ModId}-Level-Title",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children =
                        {
                            new TaiwuLabel
                            {
                                Name = "{ModId}-Level-Label",
                                Text = "品级",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = $"{ModId}-Level-Switch",
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.Level.Value.All(i => i),
                                onValueChanged = BookLevelSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{ModId}-Level-Content",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children = Settings.Level.Value
                            .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                            {
                                Name = $"{ModId}-Level-Toggle-{index}",
                                Text = Settings.BookLevel[index],
                                FontColor = Color.white,
                                isOn = val,
                                onValueChanged = BookLevelSelectionChanged
                            })
                            .ToList()
                    }
                }
            };

            container.Children.Add(levelContainer);

            #endregion 品级设置

            #region 功法类型设置

            var gongfaContainer = new BoxAutoSizeModelGameObject()
            {
                Name = $"{ModId}-GongFa-Container",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 5
                },
                SizeFitter = { VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize },
                Children =
                {
                    new Container
                    {
                        Name = $"{ModId}-GongFa-Title",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children =
                        {
                            new TaiwuLabel
                            {
                                Name = "{ModId}-GongFa-Label",
                                Text = "功法类型",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = $"{ModId}-GongFa-Switch",
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.GongFa.Value.All(i => i),
                                onValueChanged = BookGongFaSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{ModId}-GongFa-Content",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children = Settings.GongFa.Value
                            .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                            {
                                Name = $"{ModId}-GongFa-Toggle-{index}",
                                Text = Settings.BookGongFa[index],
                                FontColor = Color.white,
                                isOn = val,
                                onValueChanged = BookGongFaSelectionChanged
                            })
                            .ToList()
                    }
                }
            };

            container.Children.Add(gongfaContainer);

            #endregion 功法类型设置

            #region 功法门派设置

            var sectToggles = Settings.Sect.Value
                .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                {
                    Name = $"{ModId}-Sect-Toggle-{index}",
                    Text = Settings.BookSect[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = BookSectSelectionChanged
                });

            var sectContainer = new BoxAutoSizeModelGameObject()
            {
                Name = $"{ModId}-Sect-Container",
                Group =
                {
                    Direction = Direction.Vertical,
                    Spacing = 5
                },
                SizeFitter = { VerticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize },
                Children =
                {
                    new Container
                    {
                        Name = $"{ModId}-Sect-Title",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children =
                        {
                            new TaiwuLabel
                            {
                                Name = "{ModId}-Sect-Label",
                                Text = "门派",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = $"{ModId}-Sect-Switch",
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.Sect.Value.All(i => i),
                                onValueChanged = BookSectSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{ModId}-Sect-Content",
                        Element = { PreferredSize = { 0, containerHeight * 2 } },
                        Group =
                        {
                            Direction = Direction.Vertical,
                            Spacing = 2
                        },
                        Children =
                        {
                            new Container
                            {
                                Name = $"{ModId}-Sect-Content-Group1",
                                Element = { PreferredSize = { 0, containerHeight } },
                                Group =
                                {
                                    Direction = Direction.Horizontal,
                                    Spacing = 4
                                },
                                Children = sectToggles
                                    .Take(sectToggles.Count() / 2)
                                    .ToList()
                            },
                            new Container
                            {
                                Name = $"{ModId}-Sect-Content-Group2",
                                Element = { PreferredSize = { 0, containerHeight } },
                                Group =
                                {
                                    Direction = Direction.Horizontal,
                                    Spacing = 4
                                },
                                Children = sectToggles
                                    .Skip(sectToggles.Count() / 2)
                                    .ToList()
                            }
                        }
                    }
                }
            };

            container.Children.Add(sectContainer);

            #endregion 功法门派设置

            ModHelper.SettingUI = container;
        }

        #region 品级

        /// <summary>
        /// 书本品级全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookLevelSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var child in toggle.Parent.Parent.Children.Find(c => c.Name == $"{ModId}-Level-Content")?.Children)
            {
                if (child is TaiwuToggle t)
                {
                    t.onValueChanged -= BookLevelSelectionChanged;
                    t.isOn = value;
                    t.onValueChanged += BookLevelSelectionChanged;
                }
            }
            Settings.LevelSetAll(value);
        }

        /// <summary>
        /// 书本品级选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookLevelSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('-').Last());
            Settings.LevelSet(index, value);
            var el = toggle.Parent.Parent.Children
                .Find(c => c.Name == $"{ModId}-Level-Title")
                .Children.Find(c => c.Name == $"{ModId}-Level-Switch");
            if (el is TaiwuToggle t)
            {
                t.onValueChanged -= BookLevelSelectAllChanged;
                t.isOn = Settings.Level.Value.All(i => i);
                t.onValueChanged += BookLevelSelectAllChanged;
            }
        }

        #endregion

        #region 功法类型

        /// <summary>
        /// 功法类型全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookGongFaSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var child in toggle.Parent.Parent.Children.Find(c => c.Name == $"{ModId}-GongFa-Content")?.Children)
            {
                if (child is TaiwuToggle t)
                {
                    t.onValueChanged -= BookGongFaSelectionChanged;
                    t.isOn = value;
                    t.onValueChanged += BookGongFaSelectionChanged;
                }
            }
            Settings.GongFaSetAll(value);
        }

        /// <summary>
        /// 功法类型选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookGongFaSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('-').Last());
            Settings.GongFaSet(index, value);
            var el = toggle.Parent.Parent.Children
                .Find(c => c.Name == $"{ModId}-GongFa-Title")
                .Children.Find(c => c.Name == $"{ModId}-GongFa-Switch");
            if (el is TaiwuToggle t)
            {
                t.onValueChanged -= BookGongFaSelectAllChanged;
                t.isOn = Settings.GongFa.Value.All(i => i);
                t.onValueChanged += BookGongFaSelectAllChanged;
            }
        }

        #endregion

        #region 门派

        /// <summary>
        /// 门派全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookSectSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var child in toggle.Parent.Parent.Children
                .Find(c => c.Name == $"{ModId}-Sect-Content")?.Children
                .SelectMany(c => c.Children))
            {
                if (child is TaiwuToggle t)
                {
                    t.onValueChanged -= BookSectSelectionChanged;
                    t.isOn = value;
                    t.onValueChanged += BookSectSelectionChanged;
                }
            }
            Settings.SectSetAll(value);
        }

        /// <summary>
        /// 门派选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void BookSectSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('-').Last());
            Settings.SectSet(index, value);
            var el = toggle.Parent.Parent.Parent.Children
                .Find(c => c.Name == $"{ModId}-Sect-Title")
                .Children.Find(c => c.Name == $"{ModId}-Sect-Switch");
            if (el is TaiwuToggle t)
            {
                t.onValueChanged -= BookSectSelectAllChanged;
                t.isOn = Settings.Sect.Value.All(i => i);
                t.onValueChanged += BookSectSelectAllChanged;
            }
        }

        #endregion
    }
}
