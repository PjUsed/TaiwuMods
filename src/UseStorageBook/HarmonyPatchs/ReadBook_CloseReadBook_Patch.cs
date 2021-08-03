using HarmonyLib;

namespace UseStorageBook.HarmonyPatchs
{
    [HarmonyPatch(typeof(ReadBook), "CloseReadBook")]
    public static class ReadBook_CloseReadBook_Patch
    {
        /// <summary>
        /// 仓库中的书耐久为0时将其移除
        /// </summary>
        static void Prefix()
        {
            if (!UseStorageBook.IsEnable)
                return;
            var df = DateFile.instance;
            var bookId = BuildingWindow.instance.readBookId;
            if (df.GetActorItems(-999).ContainsKey(bookId))
            {
                var hp = int.Parse(df.GetItemDate(bookId, 901));
                if (hp <= 1)
                {
                    df.LoseItem(-999, bookId, 1, true);
                    UseStorageBook.ModLogger.LogMessage($"仓库书籍(id:{bookId})已销毁");
                }
            }
        }
    }
}
