using HarmonyLib;
using Verse;

namespace SOS2VGE
{
	public class SOS2VGE_Mod : Mod
	{
		public SOS2VGE_Mod(ModContentPack content) : base(content)
		{
			LongEventHandler.ExecuteWhenFinished(() => new Harmony("alove.SOS2VGE").PatchAll(typeof(SOS2VGE_Mod).Assembly));
		}
	}
}
