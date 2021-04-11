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

namespace FlowerDanceFix
{
	public class EventPatched
	{
		private static IMonitor Monitor;

		private static IModHelper Helper;

		public static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}

		public static void SetHelper(IModHelper helper)
        {
			Helper = helper;
        }
		
		public static bool ListFix(StardewValley.Event __instance)
		{
			try
			{
				{
					if (Monitor is null || __instance is null || !__instance.isSpecificFestival("spring24"))

					{
						return true;
					}


					Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

					//Sets up random number generation for later use
					Random rnd = new Random();
					Random rnd2 = new Random();

					//Sets up lists for pair generation
					List<NetDancePartner> females = new List<NetDancePartner>();
					List<NetDancePartner> males = new List<NetDancePartner>();

					List<string> leftoverFemales = new List<string>();
					List<string> leftoverMales = new List<string>();

					List<NPC> charList = new List<NPC>();
					Utility.getAllCharacters(charList);

					//To-do: code that processes non-binary datable characters based on config

					//To-do: code that prevents selection of tourist datable characters based on config
					
					//Populates "leftoverGender" lists with all datable NPCs of each respective gender for selection
					foreach (NPC character in charList)
					{
						if (character.gender.Equals(0) && character.datable.Equals(true))
							{
								leftoverMales.Add(character.Name);
							}
						else if (character.gender.Equals(1) && character.datable.Equals(true))
							{
								leftoverFemales.Add(character.Name);
							}
						else
							{
								continue;
							}
					}
					//To-do: code that removes blacklisted datables from "leftoverGender" lists based on config

					//Adds farmer-farmer and farmer-NPC pairs to dancelist
					List<Farmer> farmers = (from f in Game1.getOnlineFarmers()
											orderby f.UniqueMultiplayerID
											select f).ToList();
					while (farmers.Count > 0)
					{
						Farmer f2 = farmers[0];
						farmers.RemoveAt(0);
						if (multiplayer.isDisconnecting(f2) || f2.dancePartner.Value == null)
						{
							continue;
						}
						if (f2.dancePartner.GetGender() == 1)
						{
							females.Add(f2.dancePartner);
							if (f2.dancePartner.IsVillager())
							{
								leftoverFemales.Remove(f2.dancePartner.TryGetVillager().Name);
							}
							males.Add(new NetDancePartner(f2));
						}
						else
						{
							males.Add(f2.dancePartner);
							if (f2.dancePartner.IsVillager())
							{
								leftoverMales.Remove(f2.dancePartner.TryGetVillager().Name);
							}
							females.Add(new NetDancePartner(f2));
						}
						if (f2.dancePartner.IsFarmer())
						{
							farmers.Remove(f2.dancePartner.TryGetFarmer());
						}
					}
					//Generates NPC-NPC pairs
					while (females.Count < ModConfig.MaxDancePairs)
					{
						//Random pair generation- config moderated
						if (ModConfig.NPCsHaveRandomPartners.Equals(true))
						{

							int r = rnd.Next(leftoverMales.Count);

							string female = leftoverFemales.Last();
							string randomMale = leftoverMales[r];

							females.Add(new NetDancePartner(female));
							males.Add(new NetDancePartner(randomMale));

							leftoverFemales.Remove(female);
							leftoverMales.Remove(randomMale);
						
						}
						//Vanilla pair generation
						else
						{
							string female = leftoverFemales.Last();
							if (leftoverMales.Contains(Utility.getLoveInterest(female)))
							{
								females.Add(new NetDancePartner(female));
								males.Add(new NetDancePartner(Utility.getLoveInterest(female)));
							}
							leftoverFemales.Remove(female);
						}
						//Generates spring24.json "mainEvent" value
						string rawFestivalData = __instance.GetFestivalDataForYear("mainEvent");
						for (int i = 1; i <= ModConfig.MaxDancePairs; i++)
						{
							string female2 = ((!females[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(females[i - 1].TryGetFarmer())) : females[i - 1].TryGetVillager().Name);
							string male = ((!males[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(males[i - 1].TryGetFarmer())) : males[i - 1].TryGetVillager().Name);
							rawFestivalData = rawFestivalData.Replace("Girl" + i, female2);
							rawFestivalData = rawFestivalData.Replace("Guy" + i, male);
						}
						Regex regex = new Regex("showFrame (?<farmerName>farmer\\d) 44");
						Regex showFrameGirl = new Regex("showFrame (?<farmerName>farmer\\d) 40");
						Regex animation1Guy = new Regex("animate (?<farmerName>farmer\\d) false true 600 44 45");
						Regex animation1Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 43 41 43 42");
						Regex animation2Guy = new Regex("animate (?<farmerName>farmer\\d) false true 300 46 47");
						Regex animation2Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 46 47");
						rawFestivalData = regex.Replace(rawFestivalData, "showFrame $1 12/faceDirection $1 0");
						rawFestivalData = showFrameGirl.Replace(rawFestivalData, "showFrame $1 0/faceDirection $1 2");
						rawFestivalData = animation1Guy.Replace(rawFestivalData, "animate $1 false true 600 12 13 12 14");
						rawFestivalData = animation1Girl.Replace(rawFestivalData, "animate $1 false true 596 4 0");
						rawFestivalData = animation2Guy.Replace(rawFestivalData, "animate $1 false true 150 12 13 12 14");
						rawFestivalData = animation2Girl.Replace(rawFestivalData, "animate $1 false true 600 0 3");
						string[] newCommands = (__instance.eventCommands = rawFestivalData.Split('/'));
					}
				}
				return false;
			}

			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(ListFix)}:\n{ex}", LogLevel.Error);
				return true;
			}
		}
	}
}
