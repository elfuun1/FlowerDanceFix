using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FlowerDanceFix
{
    public class ModEntry : Mod
    {
        private static ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            Config = this.Helper.ReadConfig<ModConfig>();
            Helper.Events.GameLoop.GameLaunched += onLaunched;

            // Initialize Patches
            EventPatched.Initialize(Monitor, Config);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_FDF))
            );

            Monitor.Log("Flower Dance Fix started using Harmony.", LogLevel.Alert);
        }
        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            /*
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api is null) // Doesn't have Generic Mod Config Menu installed
            {
                return;
            }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.RegisterLabel(ModManifest, "Flower Dance Fix", "By elfuun");
            api.RegisterParagraph(ModManifest, "Removes some of the hard-coded details of the flower dance festival main event, allowing for altered vanilla NPCs and custom NPCs to participate. Contains configurable options for dance pair generation. Only ''datable'' NPCs are considered valid for selection.");
            api.RegisterSimpleOption(ModManifest, "Maximum Dance Pairs (WIP)", "(WIP) Changes number of pairs dancing ***NOT TESTED FOR MORE THAN 6 PAIRS***", () => config.MaxDancePairs, (int val) => config.MaxDancePairs = val);
            api.RegisterSimpleOption(ModManifest, "NPCs Have Random Partners", "Pairs of NPCs dancing are arranged at random", () => config.NPCsHaveRandomPartners, (bool val) => config.NPCsHaveRandomPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Non-Hetero Pairs (WIP)", "(WIP) Pairs of NPCs dancing are random pairs of random genders- will require additional sprites", () => config.ForceHeteroPartners, (bool val) => config.ForceHeteroPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Non-Binary Dancers (WIP)", "(WIP) Allows NPCs that are not male or female (ie secret or unknown; gender = 2)- will require additional sprites", () => config.AllowNonBinaryPartners, (bool val) => config.AllowNonBinaryPartners = val);
            api.RegisterSimpleOption(ModManifest, "Allow Tourist Dancers", "Allows NPCs living outside the valley (ie they do not live in town; homeRegion != 2)", () => config.AllowTouristPartners, (bool val) => config.AllowTouristPartners = val);
            api.RegisterSimpleOption(ModManifest, "NPC Blacklist", "Configureable blacklist of NPCs preventing them from dancing- enclose NPC base name in quotes, deliniate forward slash, with no spaces", () => config.DancerBlackList, (string val) => config.DancerBlackList = val);
            */
        }
    }
}
