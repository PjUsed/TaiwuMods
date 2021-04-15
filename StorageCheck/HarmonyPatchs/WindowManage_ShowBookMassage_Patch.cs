using HarmonyLib;
using StorageCheck.Models;
using UnityEngine;

namespace StorageCheck.HarmonyPatchs
{
    [HarmonyPatch(typeof(WindowManage), "ShowBookMassage")]
	public static class WindowManage_ShowBookMassage_Patch
	{
		/// <summary>
		/// 重写替换WindowManage.ShowBookMassage方法
		/// </summary>
		/// <param name="__instance">WindowManage实例</param>
		/// <param name="__result">原方法返回值</param>
		/// <param name="itemId">原方法参数</param>
		/// <returns></returns>
		static bool Prefix(WindowManage __instance, ref string __result, int itemId)
        {
			var mainActorId = DateFile.instance.MianActorID();
			var itemInfo = ItemInfo.Get(itemId);
			var key = itemInfo.Key;

			string text;
			if (ShopSystem.Exists
				|| BookShopSystem.Exists
				|| Warehouse.instance.warehouseWindow.activeInHierarchy
				|| (ActorMenu.Exists && !ActorMenu.instance.isEnemy)
				|| DateFile.instance.actorItemsDate[mainActorId].ContainsKey(itemId)
				|| int.Parse(DateFile.instance.GetActorDate(mainActorId, 308, applyBonus: false)) == itemId
				|| int.Parse(DateFile.instance.GetActorDate(mainActorId, 309, applyBonus: false)) == itemId
				|| int.Parse(DateFile.instance.GetActorDate(mainActorId, 310, applyBonus: false)) == itemId)
			{
				// 书籍内容部分
				text = __instance.SetMassageTitle(8007, 0, 12);
				for (int i = 0; i < itemInfo.GoodPages.Length; i++)
				{
					// 阅读进度
					string readProgressStr;
					var process = Mathf.Abs(itemInfo.LearnProcess[i]);
					if (process == 1 || process >= 100)
					{
						readProgressStr = DateFile.instance.SetColoer(20005, $"  ({DateFile.instance.massageDate[7009][4].Split('|')[3]})") + DateFile.instance.SetColoer(10001, "  100%");
					}
					else
					{
						readProgressStr = DateFile.instance.SetColoer(20002, $"  ({DateFile.instance.massageDate[7009][4].Split('|')[2]})") + DateFile.instance.SetColoer(10001, $"  <color=#00000000>{(process > 9 ? "0" : "00")}</color>{process}%");
					}

					// 书籍信息
					var bookPageStr = string.Empty;
                    if (StorageCheck.Settings.ShowBookInfo.Value)
                    {
						if(!StorageCheck.Settings.ShowBookPage.Value || itemInfo.ItemType == ItemType.SkillBook)
                        {
							bookPageStr = itemInfo.GoodPages[i] > 0
								? DateFile.instance.SetColoer(20004, $"  ○已有{itemInfo.GoodPages[i]}页", false)
								: DateFile.instance.SetColoer(20010, "  ×无此页", false);
                        }
                        else
                        {
							bookPageStr = itemInfo.GoodPages[i] + itemInfo.BadPages[i] > 0
								? $"{DateFile.instance.SetColoer(20004, $"  ○已有 {itemInfo.GoodPages[i]}真传", false)} {DateFile.instance.SetColoer(20010, $"{itemInfo.BadPages[i]}手抄", false)}"
								: DateFile.instance.SetColoer(20010, "  ×无此页", false);
                        }
                    }

					text += string.Format("{0}{1}{2}{3}{4}",
						__instance.Dit(),
						DateFile.instance.massageDate[8][2].Split('|')[i],
						(itemInfo.BookStatus[i] == 1) ? DateFile.instance.SetColoer(20004, DateFile.instance.massageDate[7010][4].Split('|')[1]) : DateFile.instance.SetColoer(20010, DateFile.instance.massageDate[7010][4].Split('|')[0]),
						readProgressStr,
						bookPageStr);
					text += "\n";
				}

				// 所载心法部分
				text += "\n";
				if (itemInfo.ItemType == ItemType.GongfaBook)
				{
					var temp = int.Parse(DateFile.instance.gongFaDate[key][103 + int.Parse(DateFile.instance.GetItemDate(itemId, 35))]);
					if (temp > 0)
					{
						text += $"{__instance.SetMassageTitle(8007, 0, 14)}{__instance.Dit()}{DateFile.instance.SetColoer(20002, $"{DateFile.instance.massageDate[8006][5].Split('|')[0]}{DateFile.instance.SetColoer(20001 + int.Parse(DateFile.instance.gongFaDate[key][2]), DateFile.instance.gongFaDate[key][0])}{DateFile.instance.massageDate[8006][5].Split('|')[1]}{DateFile.instance.gongFaFPowerDate[temp][99]}{DateFile.instance.massageDate[5001][5]}")}\n\n";
					}
				}
			}
			else
			{
				// 书籍内容，他人物品，无法查看
				text =  $"{__instance.SetMassageTitle(8007, 0, 12)}{__instance.Dit()}{DateFile.instance.massageDate[8006][4]}\n\n";
			}

			__result = text;
			return false;
        }
	}
}
