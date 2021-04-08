using BepInEx;
using HarmonyLib;
using YanLib.ModHelper;

namespace UseStorageBook
{
    [BepInPlugin(ModId, ModName, ModVersion)]
    [BepInProcess("The Scroll Of Taiwu Alpha V1.0.exe")]
    [BepInDependency("")]
    public class UseStorageBook : BaseUnityPlugin
    {
        public static ModHelper ModHelper;

        public const string ModId = "UseStorageBook";

        public const string ModName = "使用仓库中的书";

        public const string ModVersion = "1.4.0";

        public static Settings Settings { get; private set; }

        public void Awake()
        {
            Settings = new Settings();
            Settings.Init(Config);
            new Harmony(ModId).PatchAll();
            ModHelper = new ModHelper(ModId, ModName);
        }
    }
}
