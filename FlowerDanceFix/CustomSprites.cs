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
    class CustomSprites
    {
        public static bool HasFDFSprites(NPC character)
        {
            try
            {
                Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_FDF");
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }

}

