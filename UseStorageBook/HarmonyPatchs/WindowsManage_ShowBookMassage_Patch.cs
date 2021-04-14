using HarmonyLib;

namespace UseStorageBook.HarmonyPatchs
{
    /// <summary>
    /// 解决鼠标放在书本上不显示仓库中书上时，不显示仓库中书的阅读状态的BUG
    /// （暂时将书加入背包）
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "ShowBookMassage", typeof(int))]
    public static class WindowsManage_ShowBookMassage_Patch
    {
        /// <summary>
        /// 记录当前书的id，并将仓库中的书暂时加入背包
        /// </summary>
        /// <param name="itemId">物品Id</param>
        /// <param name="__state">状态，用于储存物品Id</param>
        static void Prefix(ref int itemId, ref int __state)
        {
            if (!UseStorageBook.IsEnable)
                return;
            if (DateFile.instance.actorItemsDate[-999].ContainsKey(itemId))
            {
                DateFile.instance.actorItemsDate[-999].Remove(itemId);
                DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].Add(itemId, 1);
                __state = itemId;
            }
            else
            {
                __state = -1;
            }
        }

        /// <summary>
        /// 将书从背包中移除
        /// </summary>
        /// <param name="__state"></param>
        static void Postfix(ref int __state)
        {
            if (!UseStorageBook.IsEnable)
                return;
            if (__state > 0)
            {
                DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].Remove(__state);
                DateFile.instance.actorItemsDate[-999].Add(__state, 1);
            }
        }
    }
}
