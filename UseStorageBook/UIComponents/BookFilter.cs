using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UseStorageBook.Extensions;

namespace UseStorageBook.UIComponents
{
    internal class BookFilter : MonoBehaviour
    {
        #region Public Properties

        public static BookFilter Instance { get; private set; }
        public bool Open { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private static GameObject _obj;

        private static GameObject _canvas;

        private Rect _windowRect;

        private GUIStyle _labelStyle;

        private GUILayoutOption[] _toggleStyle;

        private GUIStyle _buttonStyle;

        private bool _cursorLock;

        private Settings _settings;

        #endregion Private Fields

        #region 初始化

        /// <summary>
        /// 加载
        /// </summary>
        /// <returns>true为加载成功，false相反</returns>
        public static bool Load()
        {
            try
            {
                if (Instance == null)
                {
                    _obj = new GameObject($"TaiwuMod.plugins.{UseStorageBook.ModId}.{nameof(BookFilter)}", typeof(BookFilter));
                    DontDestroyOnLoad(_obj);
                    UseStorageBook.ModLogger.LogMessage($"Create {nameof(BookFilter)} UI");
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void Awake()
        {
            UseStorageBook.ModLogger.LogDebug($"{nameof(BookFilter)} is awake");
            Instance = this;
            DontDestroyOnLoad(this);
        }

        #endregion 初始化

        #region GUI加载

        public void Start()
        {
            _windowRect = new Rect(0f, 150f, 380f, 0);
        }

        public void OnGUI()
        {
            if (Open)
            {
                PrepareGUIStyle();

                Vector2 vector = new Vector2(Screen.width / 1600f, Screen.height / 900f);
                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(vector.x, vector.y, 1f));
                GUI.backgroundColor = Color.black;
                GUI.color = Color.white;
                _windowRect = GUILayout.Window(668, _windowRect, WindowFunc, "", GUILayout.MaxHeight(_windowRect.height), GUILayout.MaxWidth(_windowRect.width));
            }
        }

        /// <summary>
        /// 窗口设置
        /// <para>该窗口会一直保持刷新（只要显示了就一直被调用）</para>
        /// </summary>
        /// <param name="windowId">窗口Id</param>
        private void WindowFunc(int windowId)
        {
            if (_settings is null)
                _settings = UseStorageBook.Settings;

            GUILayout.BeginVertical();

            #region 背包/仓库

            GUILayout.BeginHorizontal();
            for (int i = 0; i < Settings.BookSource.Length; i++)
            {
                var state = GUILayout.Toggle(_settings.Source.Value[i], Settings.BookSource[i], _toggleStyle);
                if (_settings.Source.Value[i] != state)
                {
                    _settings.SourceSet(i, state);
                    SettingsChanged();
                }
            }
            GUILayout.EndHorizontal();

            #endregion 背包/仓库

            #region 真传/手抄

            if (BuildingWindow.instance.studySkillTyp >= 17)
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < Settings.BookType.Length; i++)
                {
                    var state = GUILayout.Toggle(_settings.Type.Value[i], Settings.BookType[i], _toggleStyle);
                    if (_settings.Type.Value[i] != state)
                    {
                        _settings.TypeSet(i, state);
                        SettingsChanged();
                    }
                }
                GUILayout.EndHorizontal();
            }

            #endregion 真传/手抄

            #region 阅读进度

            GUILayout.BeginHorizontal();
            for (int i = 0; i < Settings.ReadStatus.Length; i++)
            {
                var state = GUILayout.Toggle(_settings.Status.Value[i], Settings.ReadStatus[i], _toggleStyle);
                if (_settings.Status.Value[i] != state)
                {
                    _settings.StatusSet(i, state);
                    SettingsChanged();
                }
            }
            GUILayout.EndHorizontal();

            #endregion 阅读进度

            #region 品级

            if (GUILayout.Button("全部", GUILayout.Width(60)))
            {
                var all = _settings.Level.Value.All(i => i);
                _settings.LevelSetAll(!all);
                SettingsChanged();
            }

            for (int i = 0; i < Settings.BookLevel.Length; i++)
            {
                if (i % 5 == 0)
                    GUILayout.BeginHorizontal();
                var state = GUILayout.Toggle(_settings.Level.Value[i], Settings.BookLevel[i], _toggleStyle);
                if (_settings.Level.Value[i] != state)
                {
                    _settings.LevelSet(i, state);
                    SettingsChanged();
                }
                if (i % 5 == 4 || i == Settings.BookLevel.Length - 1)
                    GUILayout.EndHorizontal();
            }

            #endregion 品级

            #region 功法类型

            if (BuildingWindow.instance.studySkillTyp >= 17)
            {
                if (GUILayout.Button("全部", GUILayout.Width(60)))
                {
                    var all = _settings.GongFa.Value.All(i => i);
                    _settings.GongFaSetAll(!all);
                    SettingsChanged();
                }
                for (int i = 0; i < Settings.BookGongFa.Length; i++)
                {
                    if (i % 5 == 0)
                        GUILayout.BeginHorizontal();
                    var state = GUILayout.Toggle(_settings.GongFa.Value[i], Settings.BookGongFa[i], _toggleStyle);
                    if (_settings.GongFa.Value[i] != state)
                    {
                        _settings.GongFaSet(i, state);
                        SettingsChanged();
                    }
                    if (i % 5 == 4 || i == Settings.BookGongFa.Length - 1)
                        GUILayout.EndHorizontal();
                }
            }

            #endregion 功法类型

            #region 门派

            if (BuildingWindow.instance.studySkillTyp >= 17)
            {
                if (GUILayout.Button("全部", GUILayout.Width(60)))
                {
                    var all = _settings.Sect.Value.All(i => i);
                    _settings.SectSetAll(!all);
                    SettingsChanged();
                }
                for (int i = 0; i < Settings.BookSect.Length; i++)
                {
                    if (i % 5 == 0)
                        GUILayout.BeginHorizontal();
                    var state = GUILayout.Toggle(_settings.Sect.Value[i], Settings.BookSect[i], _toggleStyle);
                    if (_settings.Sect.Value[i] != state)
                    {
                        _settings.SectSet(i, state);
                        SettingsChanged();
                    }
                    if (i % 5 == 4 || i == Settings.BookSect.Length - 1)
                        GUILayout.EndHorizontal();
                }
            }

            #endregion 门派

            GUILayout.EndVertical();
        }

        #endregion GUI加载

        #region 设置UI控件样式

        /// <summary>
        /// 设置UI控件样式
        /// </summary>
        private void PrepareGUIStyle()
        {
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 5, 5, 5),
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                fixedHeight = 40f,
                normal = { textColor = Color.white }
            };

            _toggleStyle = new GUILayoutOption[]
            {
                GUILayout.Width(72),
                GUILayout.Height(30),
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(5, 5, 15, 5),
                fontSize = 16,
                fixedHeight = 40f,
                normal = { textColor = Color.white }
            };
        }

        #endregion 设置UI控件样式

        #region GUI显示及关闭

        /// <summary>
        /// 打开书本过滤
        /// </summary>
        public void ShowMenu()
        {
            ToggleWindow(open: true);
        }

        /// <summary>
        /// 关闭书本过滤
        /// </summary>
        public void CloseMenu()
        {
            ToggleWindow(open: false);
        }

        private void ToggleWindow(bool open)
        {
            Open = open;
            BlockGameUI(Open);
            if (Open)
            {
                _cursorLock = Cursor.lockState == CursorLockMode.Locked || !Cursor.visible;
                if (_cursorLock)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if (_cursorLock)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        /// <summary>
        /// 暂时阻断游戏UI
        /// </summary>
        /// <param name="open">此界面是否打开</param>
        private void BlockGameUI(bool open)
        {
            if (open)
            {
                _canvas = new GameObject($"TaiwuMod.plugins.{UseStorageBook.ModId}.canvas", typeof(Canvas), typeof(GraphicRaycaster));
                _canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.GetComponent<Canvas>().sortingOrder = 32767;
                DontDestroyOnLoad(_canvas);
                GameObject gameObject = new GameObject("panel", typeof(Image));
                gameObject.transform.SetParent(_canvas.transform);
                gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
                gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
                gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(63f / 64f, 83f / 90f);
                gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.95f);
                gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            }
            else
            {
                Destroy(_canvas);
            }
        }

        #endregion GUI显示及关闭

        #region 变更筛选

        /// <summary>
        /// 筛选条件变更
        /// </summary>
        private void SettingsChanged()
        {
            //Traverse.Create(BuildingWindow.instance).Method("SetBook").GetValue();
            ReflectionMethods.Invoke(BuildingWindow.instance, "SetBook");
        }

        #endregion 变更筛选
    }
}
