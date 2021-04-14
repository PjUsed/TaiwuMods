using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace StorageCheck.HarmonyPatchs
{
    [HarmonyPatch(typeof(WindowManage), "ShowBookMassage")]
	public static class WindowManage_ShowBookMassage_Patch
	{
		public static string getBookInfo(WindowManage __instance, int itemId)
		{
			bool flag = false;
			if (ShopSystem.instance.shopWindow.activeInHierarchy || BookShopSystem.instance.shopWindow.activeInHierarchy || Warehouse.instance.warehouseWindow.activeInHierarchy || (ActorMenu.instance.actorMenu.activeInHierarchy && !ActorMenu.instance.isEnemy) || DateFile.instance.actorItemsDate[DateFile.instance.MianActorID()].ContainsKey(itemId))
			{
				flag = true;
			}
			string text = "";
			if (!flag)
			{
				text = $"{__instance.SetMassageTitle(8007, 0, 12, 10002)}{DateFile.instance.massageDate[10][2]}{DateFile.instance.massageDate[8006][4]}\n\n";
			}
			else
			{
				int key = int.Parse(DateFile.instance.GetItemDate(itemId, 32, true, -1));
				int num = int.Parse(DateFile.instance.GetItemDate(itemId, 31, true, -1));
				int[] array = ((num == 17) ? ((!DateFile.instance.gongFaBookPages.ContainsKey(key)) ? new int[10] : DateFile.instance.gongFaBookPages[key]) : ((!DateFile.instance.skillBookPages.ContainsKey(key)) ? new int[10] : DateFile.instance.skillBookPages[key]));
				int[] bookPage = DateFile.instance.GetBookPage(itemId);
				int num2 = int.Parse(DateFile.instance.GetItemDate(itemId, 999, true, -1));
				int num3 = int.Parse(DateFile.instance.GetItemDate(itemId, 32, true, -1));
				int[] array2 = new int[10];
				bool[] array3 = new bool[10];
				bool[] array4 = new bool[10];
				int[] array5 = new int[2]
				{
				DateFile.instance.MianActorID(),
				-999
				};
				foreach (int key2 in array5)
				{
					foreach (int key3 in DateFile.instance.actorItemsDate[key2].Keys)
					{
						if (!(DateFile.instance.GetItemDate(key3, 999, true, -1) == num2.ToString()) && (!StorageCheck.Settings.ShowGoodPage.Value || int.Parse(DateFile.instance.GetItemDate(key3, 32, true, -1)) != num3))
						{
							continue;
						}
						int[] bookPage2 = DateFile.instance.GetBookPage(key3);
						for (int j = 0; j < 10; j++)
						{
							if (bookPage2[j] == 0)
							{
								continue;
							}
							array2[j]++;
							if (StorageCheck.Settings.ShowGoodPage.Value)
							{
								if (int.Parse(DateFile.instance.GetItemDate(key3, 31, true, -1)) == 17 && int.Parse(DateFile.instance.GetItemDate(key3, 35, true, -1)) == 0)
								{
									array3[j] = true;
								}
								else if (int.Parse(DateFile.instance.GetItemDate(key3, 35, true, -1)) == 1)
								{
									array4[j] = true;
								}
							}
						}
					}
				}
				if (bookPage.Length != 0)
				{
					text += "背包功法页已读统计:\n";
					for (int k = 0; k < bookPage.Length; k++)
					{
						string str = "";
						if (Mathf.Abs(array[k]) < 10)
						{
							str = "<color=#00000000>00</color>";
						}
						else if (Mathf.Abs(array[k]) < 100)
						{
							str = "<color=#00000000>0</color>";
						}
						string str2 = str + Mathf.Abs(array[k]) + "%";
						text += string.Format("{0}{1}{2}{3}{4}", DateFile.instance.massageDate[10][2], DateFile.instance.massageDate[8][2].Split('|')[k], (bookPage[k] != 1) ? DateFile.instance.SetColoer(20010, DateFile.instance.massageDate[7010][4].Split('|')[0], false) : DateFile.instance.SetColoer(20004, DateFile.instance.massageDate[7010][4].Split('|')[1], false), (array[k] != 1 && array[k] > -100) ? (DateFile.instance.SetColoer(20002, $"  ({DateFile.instance.massageDate[7009][4].Split('|')[2]})", false) + DateFile.instance.SetColoer(10001, "  " + str2, false)) : (DateFile.instance.SetColoer(20005, $"  ({DateFile.instance.massageDate[7009][4].Split('|')[3]})", false) + DateFile.instance.SetColoer(10001, "  100%", false)), (array2[k] >= 1) ? (DateFile.instance.SetColoer(20004, $"  ○已有{array2[k]}页", false) + (array3[k] ? (DateFile.instance.SetColoer(20004, "真传", false) + (array4[k] ? DateFile.instance.SetColoer(20010, "/手抄", false) : "")) : (array4[k] ? DateFile.instance.SetColoer(20010, "手抄", false) : "")) + "\n") : DateFile.instance.SetColoer(20010, $"  ×无此页\n", false));
					}
					text += "\n";
				}
				if (num == 17)
				{
					int num4 = int.Parse(DateFile.instance.gongFaDate[key][103 + int.Parse(DateFile.instance.GetItemDate(itemId, 35, true, -1))]);
					if (num4 > 0)
					{
						text += $"{__instance.SetMassageTitle(8007, 0, 14, 10002)}{DateFile.instance.massageDate[10][2]}{DateFile.instance.SetColoer(20002, $"{DateFile.instance.massageDate[8006][5].Split('|')[0]}{DateFile.instance.SetColoer(20001 + int.Parse(DateFile.instance.gongFaDate[key][2]), DateFile.instance.gongFaDate[key][0], false)}{DateFile.instance.massageDate[8006][5].Split('|')[1]}{DateFile.instance.gongFaFPowerDate[num4][99]}{DateFile.instance.massageDate[5001][5]}", false)}\n\n";
					}
				}
			}
			return text;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			StorageCheck.ModLogger.LogInfo((object)(" Transpiler init codes: " + list.Count));
			bool flag = true;
			int index = 0;
			if (flag)
			{
				List<CodeInstruction> list2 = new List<CodeInstruction>();
				list2.Add(new CodeInstruction(OpCodes.Ldsfld, (object)typeof(StorageCheck).GetProperty("IsEnable")));
				list2.Add(new CodeInstruction(OpCodes.Ldc_I4_0, (object)null));
				list2.Add(new CodeInstruction(OpCodes.Beq_S, (object)7));
				list2.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
				list2.Add(new CodeInstruction(OpCodes.Ldarg_1, (object)null));
				list2.Add(new CodeInstruction(OpCodes.Call, (object)typeof(WindowManage_ShowBookMassage_Patch).GetMethod("getBookInfo")));
				list2.Add(new CodeInstruction(OpCodes.Ret, (object)null));
				list.InsertRange(index, list2);
			}
			else
			{
				StorageCheck.ModLogger.LogError((object)" game changed ... this mod failed to find code to patch...");
			}
			return list.AsEnumerable();
		}
	}
}
