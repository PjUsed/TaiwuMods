using HarmonyLib;
using UseStorageBook.UIComponents;

namespace UseStorageBook.HarmonyPatchs
{
    [HarmonyPatch(typeof(BuildingWindow), "CloseBookWindow")]
    public class BuildingWindow_CloseBookWindow_Patch
    {
        /// <summary>
        /// 关闭筛选界面UI
        /// </summary>
        static void Postfix()
        {
            if (!UseStorageBook.IsEnable)
                return;

            BookFilter.Instance?.CloseMenu();
        }
    }
}
