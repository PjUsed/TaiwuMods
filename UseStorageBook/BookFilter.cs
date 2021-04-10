using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TaiwuUIKit.GameObjects;
using UnityEngine;
using UnityEngine.UI;
using UnityUIKit.Core;
using UnityUIKit.Core.GameObjects;
using UnityUIKit.GameObjects;

namespace UseStorageBook
{
    internal class BookFilter : MonoBehaviour
    {
		public bool Open { get; private set; }

		public static BookFilter Instance { get; private set; }

        private static GameObject obj;

		private static GameObject canvas;

		private const float designWidth = 1600f;

		private const float designHeight = 900f;

		private Rect _windowRect;

		private GUIStyle _labelStyle;

		private GUILayoutOption[] _toggleStyle;

		private GUIStyle _buttonStyle;

		private bool cursorLock;

		private Settings _settings;

		public static bool Load()
		{
			try
			{
				if (Instance == null)
				{
					obj = new GameObject($"TaiwuMod.plugins.{UseStorageBook.ModId}.{nameof(BookFilter)}", typeof(BookFilter));
					DontDestroyOnLoad(obj);
					UseStorageBook.ModLogger.LogInfo($"Create {nameof(BookFilter)} UI");
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

		public void Start()
		{
			_windowRect = new Rect(0f, 150f, 380f, 0);
		}

		public void OnGUI()
		{
			if (Open)
			{
				PrepareGUIStyle();

                Vector2 vector = new Vector2((float)Screen.width / 1600f, (float)Screen.height / 900f);
                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(vector.x, vector.y, 1f));
                GUI.backgroundColor = Color.black;
                GUI.color = Color.white;
                _windowRect = GUILayout.Window(668, _windowRect, WindowFunc, "", GUILayout.MaxHeight(_windowRect.height), GUILayout.MaxWidth(_windowRect.width));

			}
		}

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

        #endregion

        private void WindowFunc(int windowId)
		{
			if(_settings is null)
				_settings = UseStorageBook.Settings;

			GUILayout.BeginVertical();

			#region 背包/仓库

			GUILayout.BeginHorizontal();
            for (int i = 0; i < Settings.BookSource.Length; i++)
            {
				var state = GUILayout.Toggle(_settings.Source.Value[i], Settings.BookSource[i], _toggleStyle);
				if(_settings.Source.Value[i] != state)
                {
					_settings.Source.Value[i] = state;
					SettingsChanged();
				}
			}
            GUILayout.EndHorizontal();

			#endregion

			#region 真传/手抄

			if (BuildingWindow.instance.studySkillTyp >= 17)
			{
				GUILayout.BeginHorizontal();
				for (int i = 0; i < Settings.BookType.Length; i++)
				{
					var state = GUILayout.Toggle(_settings.Type.Value[i], Settings.BookType[i], _toggleStyle);
					if (_settings.Type.Value[i] != state)
					{
						_settings.Type.Value[i] = state;
						SettingsChanged();
					}
				}
				GUILayout.EndHorizontal();
			}

			#endregion

			#region 阅读进度

			GUILayout.BeginHorizontal();
			for (int i = 0; i < Settings.ReadStatus.Length; i++)
			{
				var state = GUILayout.Toggle(_settings.Status.Value[i], Settings.ReadStatus[i], _toggleStyle);
				if (_settings.Status.Value[i] != state)
				{
					_settings.Status.Value[i] = state;
					SettingsChanged();
				}
			}
			GUILayout.EndHorizontal();

			#endregion

			#region 品级

			if (GUILayout.Button("全部", GUILayout.Width(60)))
            {
				var all = _settings.Level.Value.All(i => i);
				_settings.LevelSetAll(!all);
				SettingsChanged();
			}

			for (int i = 0; i < Settings.BookLevel.Length; i++)
			{
				if(i % 5 == 0)
					GUILayout.BeginHorizontal();
				var state = GUILayout.Toggle(_settings.Level.Value[i], Settings.BookLevel[i], _toggleStyle);
				if (_settings.Level.Value[i] != state)
				{
					_settings.Level.Value[i] = state;
					SettingsChanged();
				}
				if (i % 5 == 4 || i == Settings.BookLevel.Length - 1)
					GUILayout.EndHorizontal();
			}

			#endregion

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
						_settings.GongFa.Value[i] = state;
						SettingsChanged();
					}
					if (i % 5 == 4 || i == Settings.BookGongFa.Length - 1)
						GUILayout.EndHorizontal();
				}
			}

			#endregion

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
						_settings.Sect.Value[i] = state;
						SettingsChanged();
					}
					if (i % 5 == 4 || i == Settings.BookSect.Length - 1)
						GUILayout.EndHorizontal();
				}
			}

			#endregion

			GUILayout.EndVertical();
		}

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
				cursorLock = Cursor.lockState == CursorLockMode.Locked || !Cursor.visible;
				if (cursorLock)
				{
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
			}
			else if (cursorLock)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		private void BlockGameUI(bool open)
		{
			if (open)
			{
				canvas = new GameObject($"TaiwuMod.plugins.{UseStorageBook.ModId}.canvas", typeof(Canvas), typeof(GraphicRaycaster));
				canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
				canvas.GetComponent<Canvas>().sortingOrder = 32767;
				DontDestroyOnLoad(canvas);
				GameObject gameObject = new GameObject("panel", typeof(Image));
				gameObject.transform.SetParent(canvas.transform);
				gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
				gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
				gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(63f / 64f, 83f / 90f);
				gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.95f);
				gameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
				gameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
			}
			else
			{
				Destroy(canvas);
			}
		}

		/// <summary>
		/// 筛选条件变更
		/// </summary>
		private void SettingsChanged()
        {
            //Traverse.Create(BuildingWindow.instance).Method("SetBook").GetValue();
			ReflectionMethods.Invoke(BuildingWindow.instance, "SetBook");
        }
	}
}
