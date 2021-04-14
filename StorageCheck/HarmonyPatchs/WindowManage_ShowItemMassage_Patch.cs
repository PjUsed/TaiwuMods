using GameData;
using HarmonyLib;
using UnityEngine.UI;

namespace StorageCheck.HarmonyPatchs
{
    [HarmonyPatch(typeof(WindowManage), "ShowItemMassage")]
	public static class WindowManage_ShowItemMassage_Patch
	{
		public static int localGetItemNumber(int actorId, int itemId)
		{
			int result = 0;
			if (DateFile.instance.actorItemsDate.ContainsKey(actorId) && DateFile.instance.actorItemsDate[actorId].ContainsKey(itemId))
			{
				return (int.Parse(DateFile.instance.GetItemDate(itemId, 6, true, -1)) == 0) ? 1 : DateFile.instance.actorItemsDate[actorId][itemId];
			}
			return result;
		}

		private static bool getItemNumber(int actorid, int item_id, ref int num, ref int usenum, ref int totalcount)
		{
			num = 0;
			usenum = 0;
			int num2 = int.Parse(DateFile.instance.GetItemDate(item_id, 999, true, -1));
			if (num2 > 0)
			{
				foreach (int key in DateFile.instance.actorItemsDate[actorid].Keys)
				{
					if (DateFile.instance.GetItemDate(key, 999, true, -1) == num2.ToString())
					{
						num += localGetItemNumber(actorid, key);
						usenum += int.Parse((Items.GetItem(key) != null) ? DateFile.instance.GetItemDate(key, 901, true, -1) : DateFile.instance.GetItemDate(key, 902, true, -1));
						totalcount += int.Parse((Items.GetItem(key) != null) ? Items.GetItemProperty(key, 902) : DateFile.instance.GetItemDate(key, 902, true, -1));
					}
				}
			}
			return usenum > 0;
		}

		private static void Postfix(WindowManage __instance, int itemId, ref string ___baseWeaponMassage, ref Text ___informationMassage)
		{
			if (StorageCheck.IsEnable)
			{
				string text = ___baseWeaponMassage;
				if (StorageCheck.Settings.ShowBag.Value)
				{
					int num = 0;
					int usenum = 0;
					int totalcount = 0;
					text = (getItemNumber(DateFile.instance.MianActorID(), itemId, ref num, ref usenum, ref totalcount) ? (text + DateFile.instance.SetColoer(20008, $"\n 背包数量: {num}  总耐久: {usenum}/{totalcount}", false)) : (text + DateFile.instance.SetColoer(20008, $"\n 背包数量: {num} ", false)));
				}
				if (StorageCheck.Settings.ShowWarehouse.Value)
				{
					int num2 = 0;
					int usenum2 = 0;
					int totalcount2 = 0;
					text = (getItemNumber(-999, itemId, ref num2, ref usenum2, ref totalcount2) ? (text + DateFile.instance.SetColoer(20008, $"\n 仓库数量: {num2}  总耐久: {usenum2}/{totalcount2}", false)) : (text + DateFile.instance.SetColoer(20008, $"\n 仓库数量: {num2} ", false)));
					text += "\n\n";
				}
				___baseWeaponMassage = text;
				___informationMassage.text = text;
			}
		}
	}
}
