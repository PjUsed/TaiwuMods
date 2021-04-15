using HarmonyLib;
using StorageCheck.Models;
using UnityEngine.UI;

namespace StorageCheck.HarmonyPatchs
{
    [HarmonyPatch(typeof(WindowManage), "ShowItemMassage")]
	public static class WindowManage_ShowItemMassage_Patch
	{
		/// <summary>
		/// 后置方法，在WindowManage.ShowItemMassage之后执行
		/// </summary>
		/// <param name="___baseWeaponMassage">WindowManage实例的baseWeaponMassage字段</param>
		/// <param name="___informationMassage">WindowManage实例的informationMassage字段</param>
		/// <param name="itemId">原方法参数</param>
		private static void Postfix(ref string ___baseWeaponMassage, ref Text ___informationMassage, int itemId)
		{
			string text = ___baseWeaponMassage;
			var itemInfo = ItemInfo.Get(itemId);

			// 记录是否需要换行
			var flag = false;
			if (StorageCheck.Settings.CheckBag.Value)
			{
				text += itemInfo.BagAvailableUseTimes > 0
					? DateFile.instance.SetColoer(20008, $"\n 背包数量: {itemInfo.BagCount}  总耐久: {itemInfo.BagAvailableUseTimes}/{itemInfo.BagTotalUseTimes}", false)
					: DateFile.instance.SetColoer(20008, $"\n 背包数量: {itemInfo.BagCount} ", false);
				flag = true;
			}
			if (StorageCheck.Settings.CheckWarehouse.Value)
			{
				text += itemInfo.WarehouseAvailableUseTimes > 0
					? DateFile.instance.SetColoer(20008, $"\n 仓库数量: {itemInfo.WarehouseCount}  总耐久: {itemInfo.WarehouseAvailableUseTimes}/{itemInfo.WarehouseTotalUseTimes}", false)
					: DateFile.instance.SetColoer(20008, $"\n 仓库数量: {itemInfo.WarehouseCount} ", false);
				flag = true;
			}

            if (flag)
            {
				text += "\n\n";
            }

			___baseWeaponMassage = text;
			___informationMassage.text = text;
		}
	}
}
