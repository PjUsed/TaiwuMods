using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
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
        public const string ModVersion = "1.4.2";

        #region 控件名称设置

        private readonly string _settingUIContainerName = $"{ModId}.Container";
        private readonly string _enableContainerName = $"{ModId}.Enable.Container";
        private readonly string _enableContainerToggleName = $"{ModId}.Enable.Toggle";
        private readonly string _sourceContainerName = $"{ModId}.Source.Container";
        private readonly string _sourceContainerToggleNamePrefix = $"{ModId}.Source.Toggle";
        private readonly string _typeContainerName = $"{ModId}.Type.Container";
        private readonly string _typeContainerToggleNamePrefix = $"{ModId}.Type.Toggle";
        private readonly string _statusContainerName = $"{ModId}.Status.Container";
        private readonly string _statusContainerToggleNamePrefix = $"{ModId}.Status.Toggle";
        private readonly string _levelContainerName = $"{ModId}.Level.Container";
        private readonly string _levelContainerAllToggleName = $"{ModId}.Level.All.Toggle";
        private readonly string _levelContainerToggleNamePrefix = $"{ModId}.Level.Toggle";
        private readonly string _gongfaContainerName = $"{ModId}.GongFa.Container";
        private readonly string _gongfaContainerAllToggleName = $"{ModId}.GongFa.All.Toggle";
        private readonly string _gongfaContainerToggleNamePrefix = $"{ModId}.GongFa.Toggle";
        private readonly string _sectContainerName = $"{ModId}.Sect.Container";
        private readonly string _sectContainerAllToggleName = $"{ModId}.Sect.All.Toggle";
        private readonly string _sectContainerToggleNamePrefix = $"{ModId}.Sect.Toggle";

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
            const float containerHeight = 60, labelWidth = 400, selectAllWidth = 120;

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

            #region 背包/仓库设置

            var sourceContainer = new Container()
            {
                Name = _sourceContainerName,
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
                        Name = $"{_sourceContainerName}.Label",
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
                    Name = $"{_sourceContainerToggleNamePrefix}.{index}",
                    Text = Settings.BookSource[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.SourceSet(index, value)
                }));

            container.Children.Add(sourceContainer);

            #endregion 背包/仓库设置

            #region 真传/手抄设置

            var typeContainer = new Container()
            {
                Name = _typeContainerName,
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
                        Name = $"{_typeContainerName}.Label",
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
                    Name = $"{_typeContainerToggleNamePrefix}.{index}",
                    Text = Settings.BookType[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.TypeSet(index, value)
                }));

            container.Children.Add(typeContainer);

            #endregion 真传/手抄设置

            #region 阅读进度设置

            var statusContainer = new Container()
            {
                Name = _statusContainerName,
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
                        Name = $"{_statusContainerName}.Label",
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
                    Name = $"{_statusContainerToggleNamePrefix}.{index}",
                    Text = Settings.ReadStatus[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = (value, _) => Settings.StatusSet(index, value)
                }));

            container.Children.Add(statusContainer);

            #endregion 阅读进度设置

            #region 品级设置

            var levelContainer = new BoxAutoSizeModelGameObject()
            {
                Name = _levelContainerName,
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
                        Name = $"{_levelContainerName}.Title",
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
                                Name = $"{_levelContainerName}.Title.Label",
                                Text = "品级",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = _levelContainerAllToggleName,
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.Level.Value.All(i => i),
                                onValueChanged = LevelSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{_levelContainerName}.Content",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children = Settings.Level.Value
                            .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                            {
                                Name = $"{_levelContainerToggleNamePrefix}.{index}",
                                Text = Settings.BookLevel[index],
                                FontColor = Color.white,
                                isOn = val,
                                onValueChanged = LevelSelectionChanged
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
                Name = _gongfaContainerName,
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
                        Name = $"{_gongfaContainerName}.Title",
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
                                Name = $"{_gongfaContainerName}.Title.Label",
                                Text = "功法类型",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = _gongfaContainerAllToggleName,
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.GongFa.Value.All(i => i),
                                onValueChanged = GongFaSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{_gongfaContainerName}.Content",
                        Element = { PreferredSize = { 0, containerHeight } },
                        Group =
                        {
                            Direction = Direction.Horizontal,
                            Spacing = 4
                        },
                        Children = Settings.GongFa.Value
                            .Select((val, index) => (ManagedGameObject)new TaiwuToggle
                            {
                                Name = $"{_gongfaContainerToggleNamePrefix}.{index}",
                                Text = Settings.BookGongFa[index],
                                FontColor = Color.white,
                                isOn = val,
                                onValueChanged = GongFaSelectionChanged
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
                    Name = $"{_sectContainerToggleNamePrefix}.{index}",
                    Text = Settings.BookSect[index],
                    FontColor = Color.white,
                    isOn = val,
                    onValueChanged = SectSelectionChanged
                });

            var sectContainer = new BoxAutoSizeModelGameObject()
            {
                Name = _sectContainerName,
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
                        Name = $"{_sectContainerName}.Title",
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
                                Name = $"{_sectContainerName}.Title.Label",
                                Text = "门派",
                                Element = { PreferredSize = { 0, containerHeight } },
                                UseBoldFont = true,
                                UseOutline = true,
                            },
                            new TaiwuToggle
                            {
                                Name = _sectContainerAllToggleName,
                                Text = "全选",
                                Element = { PreferredSize = { selectAllWidth, containerHeight } },
                                FontColor = Color.white,
                                isOn = Settings.Sect.Value.All(i => i),
                                onValueChanged = SectSelectAllChanged
                            }
                        }
                    },
                    new Container
                    {
                        Name = $"{_sectContainerName}.Content",
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
                                Name = $"{_sectContainerName}.Content.Group1",
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
                                Name = $"{_sectContainerName}.Content.Group2",
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

            #region 当设置变更时，同步更新设置界面UI

            Settings.Source.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _sourceContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.Source.Value);
            };

            Settings.Type.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _typeContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.Type.Value);
            };

            Settings.Status.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _statusContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.Status.Value);
            };

            Settings.Level.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _levelContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.Level.Value);
            };

            Settings.GongFa.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _gongfaContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.GongFa.Value);
            };

            Settings.Sect.SettingChanged += (s, e) =>
            {
                var toggles = SearchToggleElements(container, _sectContainerToggleNamePrefix);
                SetTogglesValue(toggles, Settings.Sect.Value);
            };

            #endregion
        }

        #region 品级全选及选中事件方法

        /// <summary>
        /// 书本品级全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void LevelSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var tg in SearchToggleElements(toggle.Parent.Parent, _levelContainerToggleNamePrefix))
            {
                tg.onValueChanged -= LevelSelectionChanged;
                tg.isOn = value;
                tg.onValueChanged += LevelSelectionChanged;
            }
            Settings.LevelSetAll(value);
        }

        /// <summary>
        /// 书本品级选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void LevelSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('.').Last());
            Settings.LevelSet(index, value);
            var tg = SearchToggleElements(toggle.Parent.Parent, _levelContainerAllToggleName).SingleOrDefault();
            if (tg != null)
            {
                tg.onValueChanged -= LevelSelectAllChanged;
                tg.isOn = Settings.Level.Value.All(i => i);
                tg.onValueChanged += LevelSelectAllChanged;
            }
        }

        #endregion

        #region 功法类型全选及选中事件方法

        /// <summary>
        /// 功法类型全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void GongFaSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var tg in SearchToggleElements(toggle.Parent.Parent, _gongfaContainerToggleNamePrefix))
            {
                tg.onValueChanged -= GongFaSelectionChanged;
                tg.isOn = value;
                tg.onValueChanged += GongFaSelectionChanged;
            }
            Settings.GongFaSetAll(value);
        }

        /// <summary>
        /// 功法类型选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void GongFaSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('.').Last());
            Settings.GongFaSet(index, value);
            var tg = SearchToggleElements(toggle.Parent.Parent, _gongfaContainerAllToggleName).SingleOrDefault();
            if (tg != null)
            {
                tg.onValueChanged -= GongFaSelectAllChanged;
                tg.isOn = Settings.GongFa.Value.All(i => i);
                tg.onValueChanged += GongFaSelectAllChanged;
            }
        }

        #endregion

        #region 门派全选及选中事件方法

        /// <summary>
        /// 门派全选
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void SectSelectAllChanged(bool value, Toggle toggle)
        {
            foreach (var tg in SearchToggleElements(toggle.Parent.Parent, _sectContainerToggleNamePrefix))
            {
                tg.onValueChanged -= SectSelectionChanged;
                tg.isOn = value;
                tg.onValueChanged += SectSelectionChanged;
            }
            Settings.SectSetAll(value);
        }

        /// <summary>
        /// 门派选择
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="toggle">开关元素</param>
        private void SectSelectionChanged(bool value, Toggle toggle)
        {
            var index = int.Parse(toggle.Name.Split('.').Last());
            Settings.SectSet(index, value);
            var tg = SearchToggleElements(toggle.Parent.Parent.Parent, _sectContainerAllToggleName).SingleOrDefault();
            if (tg != null)
            {
                tg.onValueChanged -= SectSelectAllChanged;
                tg.isOn = Settings.Sect.Value.All(i => i);
                tg.onValueChanged += SectSelectAllChanged;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 给TaiwuToggle集合赋值
        /// </summary>
        /// <param name="toggles">Toggles</param>
        /// <param name="values">值</param>
        private void SetTogglesValue(IEnumerable<TaiwuToggle> toggles, bool[] values)
        {
            foreach (var toggle in toggles)
            {
                // 获取索引，判断索引超出数组长度，Toggle的值是否发生变化
                if (int.TryParse(toggle.Name.Split('.').Last(), out var index) && index < values.Length && toggle.isOn != values[index])
                {
                    toggle.isOn = values[index];
                }
            }
        }

        /// <summary>
        /// 搜索父级元素下的所有
        /// </summary>
        /// <param name="parent">父级元素</param>
        /// <param name="namePrefix">Toggle控件名前缀</param>
        /// <param name="comparison">比较方式，默认忽略大小写</param>
        /// <returns>符合条件的Toggle集合</returns>
        private IEnumerable<TaiwuToggle> SearchToggleElements(ManagedGameObject parent, string namePrefix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var child in parent.Children)
            {
                if (child is Container container)
                {
                    foreach (var item in SearchToggleElements(container, namePrefix))
                    {
                        yield return item;
                    }
                }
                else if(child is BoxAutoSizeModelGameObject box)
                {
                    foreach (var item in SearchToggleElements(box, namePrefix))
                    {
                        yield return item;
                    }
                }
                else if(child is TaiwuToggle toggle && toggle.Name.StartsWith(namePrefix, comparison))
                {
                    yield return toggle;
                }
            }
        }

        #endregion
    }
}
