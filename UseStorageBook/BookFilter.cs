
using UnityEngine;

namespace UseStorageBook
{
    internal class BookFilter : MonoBehaviour
    {
        private static GameObject obj;

        private Rect windowRect;

        public static BookFilter Instance
        {
            get;
            private set;
        }

		public static bool Load()
		{
			try
			{
				if (obj == null)
				{
					obj = new GameObject(nameof(BookFilter), typeof(BookFilter));
                    DontDestroyOnLoad(obj);
				}
			}
			catch (System.Exception)
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
	}
}
