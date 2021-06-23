using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewModdingAPI;
using Harmony;
using Microsoft.Xna.Framework.Graphics;

namespace FlowerDanceFix
{
    class CustomDance
    {
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }
        public static bool HasFDFSprites(NPC character)
        {
            try
            {
                Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_FDF");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string BuildEventWarpBlock(List<NetDancePartner> females)
        {

            /* There will eventually be some code here straightening out issues between custom spectator animations
             * that involve NPCs you can dance with
             * it'll be messy, ugh I'm not looking forward to it
             */

            int n = females.Count;
            int q = n % 2;

            StringBuilder eventWarpDancer = new StringBuilder();

            //I gave up on using math to get x coordinate, so here's some arrays instead lmao

            switch (q)
            {
                case 0:

                    int counti = 1;

                    int[] even = { 0, 13, 15, 11, 17, 9, 19, 7, 21, 6, 22, 8, 20, 10, 18, 12, 16 };

                    while (counti <= 16 && counti <= n)
                    {
                        eventWarpDancer.Append($"/warp Girl{counti} {even[counti]} 24");
                        eventWarpDancer.Append($"/warp Guy{counti} {even[counti]} 27");
                        counti++;
                    }

                    break;

                case 1:

                    int countj = 1;

                    int[] odd = { 0, 14, 16, 12, 18, 10, 20, 8, 22, 6, 21, 7, 19, 9, 17, 11, 15, 13 };

                    while (countj <= 17 && countj <= n)
                    {
                        eventWarpDancer.Append($"/warp Girl{countj} {odd[countj]} 24");
                        eventWarpDancer.Append($"/warp Guy{countj} {odd[countj]} 27");
                        countj++;
                    }
                    break;
            }
            return eventWarpDancer.ToString();
        }

        public static string BuildShowFrameBlock(List<NetDancePartner> females)
        {

            int n = females.Count();
            int count = 1;

            StringBuilder eventShowFrame = new StringBuilder();

            while (count <= n)
            {
                eventShowFrame.Append($"/showFrame Girl{count} 40");
                eventShowFrame.Append($"/showFrame Guy{count} 44");
                count++;
            }

            return eventShowFrame.ToString();
        }

        public static string BuildAnimateBlock1(List<NetDancePartner> females)
        {

            int n = females.Count();
            int count = 1;

            StringBuilder eventAnimate1 = new StringBuilder();
            eventAnimate1.Append("/beginSimultaneousCommand");
            while (count <= n)
            {
                eventAnimate1.Append($"/animate Girl{count} false true 600 43 41 43 42");
                eventAnimate1.Append($"/animate Guy{count} false true 600 44 45");

                count++;
            }
            eventAnimate1.Append("/endSimultaneousCommand");
            return eventAnimate1.ToString();
        }

        public static string BuildAnimateBlock2(List<NetDancePartner> females)
        {
            int n = females.Count();
            int count = 1;

            StringBuilder eventAnimate2 = new StringBuilder();
            eventAnimate2.Append("/beginSimultaneousCommand");
            while (count <= n)
            {
                eventAnimate2.Append($"/animate Girl{count} false true 600 44 45");
                eventAnimate2.Append($"/animate Guy{count} false true 600 46 47");

                count++;
            }
            eventAnimate2.Append("/endSimultaneousCommand");
            return eventAnimate2.ToString();
        }

        public static string BuildAnimateBlock3(List<NetDancePartner> females)
        {
            int n = females.Count();
            int count = 1;

            StringBuilder eventAnimate3 = new StringBuilder();
            eventAnimate3.Append("/beginSimultaneousCommand");
            while (count <= n)
            {
                eventAnimate3.Append($"/animate Girl{count} false true 600 46 47");
                eventAnimate3.Append($"/animate Guy{count} false true 600 44 45");

                count++;
            }
            eventAnimate3.Append("/endSimultaneousCommand");
            return eventAnimate3.ToString();
        }

        public static string BuildStopAnimationBlock(List<NetDancePartner> females)
        {
            int n = females.Count();
            int count = 1;

            StringBuilder eventStopAnimation = new StringBuilder();

            while (count <= n)
            {
                eventStopAnimation.Append($"/stopAnimation Girl{count} 46");
                eventStopAnimation.Append($"/stopAnimation Guy{count} 44");
                count++;
            }

            return eventStopAnimation.ToString();
        }

        public static string BuildOffsetBlock(List<NetDancePartner> females)
        {
            int n = females.Count();
            int count = 1;

            StringBuilder eventOffset = new StringBuilder();
            eventOffset.Append("/beginSimultaneousCommand");
            while (count <= n)
            {
                eventOffset.Append($"/positionOffset Guy{count} 0 -2");
                count++;
            }
            eventOffset.Append("/endSimultaneousCommand");
            return eventOffset.ToString();
        }

        public static string BuildGiantOffsetBlock(List<NetDancePartner> females)
        {
            string offsetBlock = BuildOffsetBlock(females);

            StringBuilder eventOffsetGiant = new StringBuilder();
            for (int z = 0; z < 28; z++)
            {
                eventOffsetGiant.Append(offsetBlock);
                eventOffsetGiant.Append("/pause 300");
            }
            eventOffsetGiant.Append(offsetBlock);

            return eventOffsetGiant.ToString();
        }
        /*
         public static bool HasFDFSprites(NPC character)
         {
             try
             {
                 Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_FDF");
                 return true;
             }
             catch (Exception)
             {
                 return false;
             }
         }

         public static string BuildEventWarp(List<NetDancePartner> females)
         {
             int n = females.Count;
             int q = n % 2;

             StringBuilder eventWarp = new StringBuilder();

             switch (q)
             {
                 case 0:
                     int e = 13;
                     int count = 1;
                     int osc = 0;
                     while (count < n && count < 8)
                     {
                         eventWarp.Append("/warp Girl" + count + " " + (e + (2 * Math.Pow(-1, osc))));
                         eventWarp.Append("/warp Guy" + count + " " + (e + (((osc + 2) % 2) * 2 * Math.Pow(-1, osc))));
                         count += ((osc + 2) % 2);
                         osc++;

                     }




                     return "even";
                 case 1:
                     int odd = 14; 
                     return "odd";
                 default:
                     return "this isn't a thing that should happen, excuse me???";
             }
         }

         public static string xCoordinateBuilder(int start, int max)
         {

         }

         //Example Code by PathosChild
         public bool CanLoad(IAssetInfo asset)
         {
             return this.GetNpcSprite(asset) != null;
         }

         public T Load<T>(IAssetInfo asset)
         {
             return this.GetNpcSprite(asset);
         }

         private Texture2D GetNpcSprite(IAssetInfo asset)
         {
             var segments = PathUtilities.GetSegments(asset.AssetName);

             bool isNpcSprite =
                typeof(T) == typeof(Texture2D)
                && segments.Length == 2
                && string.Equals(segments[0], "Characters", StringComparison.OrdinalIgnoreCase);
             if (!isNpcSprite)
                 return null;

             FileInfo file = new FileInfo(Path.Combine(this.Helper.DirectoryPath, "assets", $"{segments[1]}.png"));
             return file.Exists
                 ? this.Helper.Content.Load<Texture2D>($"assets/{file.Name}")
                 : null;
         }
         */
    }
}

