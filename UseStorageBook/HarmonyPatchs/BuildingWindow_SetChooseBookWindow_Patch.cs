using HarmonyLib;
using UseStorageBook.UIComponents;

namespace UseStorageBook.HarmonyPatchs
{
    [HarmonyPatch(typeof(BuildingWindow), "SetChooseBookWindow")]
    public class BuildingWindow_SetChooseBookWindow_Patch
    {
        /// <summary>
        /// 设置并显示筛选界面UI
        /// </summary>
        static void Postfix()
        {
            if (!UseStorageBook.IsEnable)
                return;

            if (BookFilter.Instance is null)
                BookFilter.Load();
            BookFilter.Instance?.ShowMenu();
        }
    }
}
