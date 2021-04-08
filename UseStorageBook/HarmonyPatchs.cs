using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseStorageBook
{
    [HarmonyPatch(typeof(BuildingWindow), "SetBook")]
    public class BuildingWindow_SetBook_Patch
    {
        
    }

    [HarmonyPatch(typeof(BuildingWindow), "Start")]
    public class BuildingWindow_Start_Patch
    {
        
    }

    [HarmonyPatch(typeof(BuildingWindow), "SetChooseBookWindow")]
    public class BuildingWindow_SetChooseBookWindow_Patch
    {
        
    }

    [HarmonyPatch(typeof(BuildingWindow), "CloseBookWindow")]
    public class BuildingWindow_CloseBookWindow_Patch
    {
        
    }

    /// <summary>
    /// 仓库中的书耐久为0时将其移除
    /// </summary>
    [HarmonyPatch(typeof(ReadBook), "CloseReadBook")]
    public static class ReadBook_CloseReadBook_Patch
    {
        
    }

    /// <summary>
    /// 解决鼠标放在书本上不显示仓库中书上时，不显示仓库中书的阅读状态的BUG
    /// （暂时将书加入背包）
    /// </summary>
    [HarmonyPatch(typeof(WindowManage), "ShowBookMassage")]
    public static class WindowsManage_ShowBookMassage_Patch
    {
        /// <summary>
        /// 记录当前书的id，并将仓库中的书暂时加入背包
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="__state"></param>
        static void Prefix(ref int itemId, ref int __state)
        {
            
        }

        /// <summary>
        /// 将书从背包中移除
        /// </summary>
        /// <param name="__state"></param>
        static void Postfix(ref int __state)
        {
            
        }
    }
}
