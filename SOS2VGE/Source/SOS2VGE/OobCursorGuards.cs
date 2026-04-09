using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SOS2VGE
{
	/// <summary>
	/// Looks up SOS2 Harmony postfix methods after ShipInteriorMod2 has loaded (not at Mod ctor time).
	/// </summary>
	internal static class Sos2HarmonyLookup
	{
		internal static MethodBase TryCanLandOnSos2SpacePostfix()
		{
			var topLevel = TryPostfixOnType("SaveOurShip2.CanLandOnSOS2Space");
			if (topLevel != null)
			{
				return topLevel;
			}
			return TryPostfixOnType("SaveOurShip2.HarmonyPatches+CanLandOnSOS2Space");
		}

		internal static MethodBase TryVfShuttleBayLandingPostfix()
		{
			var topLevel = TryPostfixOnType("SaveOurShip2.VFShuttleBayLanding");
			if (topLevel != null)
			{
				return topLevel;
			}
			return TryPostfixOnType("SaveOurShip2.HarmonyPatches+VFShuttleBayLanding");
		}

		static MethodBase TryPostfixOnType(string fullName)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type type;
				try
				{
					type = assembly.GetType(fullName, throwOnError: false, ignoreCase: false);
				}
				catch
				{
					continue;
				}
				if (type == null)
				{
					continue;
				}
				var postfix = AccessTools.Method(type, "Postfix");
				if (postfix != null)
				{
					return postfix;
				}
			}
			return null;
		}
	}

	/// <summary>
	/// SOS2 shuttle postfix can dereference map/vehicleDef in edge targeter states.
	/// Skip that postfix when either is null to avoid targeter lockup loops.
	/// </summary>
	[HarmonyPatch]
	public static class Guard_SOS2_VFShuttleBayLanding_Postfix
	{
		static MethodBase TargetMethod() => Sos2HarmonyLookup.TryVfShuttleBayLandingPostfix();

		static bool Prepare() => TargetMethod() != null;

		[HarmonyPrefix]
		public static bool Prefix(object vehicleDef, Map map)
		{
			return vehicleDef != null && map != null;
		}
	}

	/// <summary>
	/// Gravship: SOS2's postfix uses GetTerrain before checking bounds; skip it when the cursor is off-map.
	/// </summary>
	[HarmonyPatch]
	public static class Guard_CanLandOnSOS2Space_Postfix
	{
		static MethodBase TargetMethod() => Sos2HarmonyLookup.TryCanLandOnSos2SpacePostfix();

		static bool Prepare() => TargetMethod() != null;

		[HarmonyPrefix]
		public static bool Prefix(IntVec3 cell, Map map)
		{
			return map != null && cell.InBounds(map);
		}
	}

	/// <summary>
	/// Shuttle: VF's IsRoofRestricted hits RoofGrid without an InBounds check; short-circuit before the original runs.
	/// </summary>
	[HarmonyPatch]
	public static class Guard_ExtVehicles_IsRoofRestricted
	{
		static MethodBase TargetMethod()
		{
			var extVehicles = AccessTools.TypeByName("Vehicles.Ext_Vehicles");
			var vehicleDef = AccessTools.TypeByName("Vehicles.VehicleDef");
			if (extVehicles == null || vehicleDef == null)
			{
				return null;
			}
			return AccessTools.Method(extVehicles, "IsRoofRestricted", new[] { vehicleDef, typeof(IntVec3), typeof(Map) });
		}

		static bool Prepare() => TargetMethod() != null;

		[HarmonyPrefix]
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(object vehicleDef, IntVec3 cell, Map map, ref bool __result)
		{
			if (vehicleDef == null || map == null)
			{
				// Returning false here also runs SOS2 postfixes; set false so their first short-circuit exits safely.
				__result = false;
				return false;
			}
			if (!cell.InBounds(map))
			{
				__result = true;
				return false;
			}
			return true;
		}
	}
}
