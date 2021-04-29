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
	
		//From NPC.cs, ln 3062
		public virtual void wearIslandAttire()
		{
			try
			{
				Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value) + "_Beach");
			}
			catch (ContentLoadException)
			{
				Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value));
			}
			isWearingIslandAttire = true;
			resetPortrait();
		}
		//From NPC.cs, ln 3076
		public virtual void wearNormalClothes()
		{
			Sprite.LoadTexture("Characters\\" + getTextureNameForCharacter(name.Value));
			isWearingIslandAttire = false;
			resetPortrait();
		}
		//From IslandSouth.cs, ln 927
		public static void SetupIslandSchedules()
		{
			//...
			foreach (NPC visitor2 in visitors)
			{
				StringBuilder schedule = new StringBuilder("");
				bool should_dress = HasIslandAttire(visitor2);
				bool had_first_activity = false;
				if (should_dress)
				{
					Point dressing_room2 = GetDressingRoomPoint(visitor2);
					schedule.Append("/a1150 IslandSouth " + dressing_room2.X + " " + dressing_room2.Y + " change_beach");
					had_first_activity = true;
				}

				//...
				
				if (should_dress)
				{
					Point dressing_room = GetDressingRoomPoint(visitor2);
					schedule.Append("/a1730 IslandSouth " + dressing_room.X + " " + dressing_room.Y + " change_normal");
				}
			}
		}
	}

}

