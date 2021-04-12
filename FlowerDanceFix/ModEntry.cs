using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;


namespace FlowerDanceFix
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            //Initialize Config
            this.Config = this.Helper.ReadConfig<ModConfig>();
            int MaxDancePairs = this.Config.MaxDancePairs;
            bool NPCsHaveRandomPartners = this.Config.NPCsHaveRandomPartners;
            bool ForceHeteroPartners = this.Config.ForceHeteroPartners;
            bool AllowNonbinaryPartners = this.Config.AllowNonBinaryPartners;
            bool AllowTouristPartners = this.Config.AllowTouristPartners;
            string DancerBlackList = this.Config.DancerBlackList;

            //Helper.Events.GameLoop.GameLaunched += onLaunched;

            //Initialize Patches
            EventPatched.Initialize(Monitor);

            EventPatched.SetHelper(helper);

            EventPatched.SetConfig(Config);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                 original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
                 prefix: new HarmonyMethod(typeof(FlowerDanceFix.EventPatched), nameof(FlowerDanceFix.EventPatched.ListFix))
                 );


            Monitor.Log("Flower Dance Fix started using Harmony to patch", LogLevel.Debug);
        }
    }
}
