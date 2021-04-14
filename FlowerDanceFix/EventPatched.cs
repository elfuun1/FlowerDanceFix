using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace FlowerDanceFix
{
    public class EventPatched
    {
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        public static void setUpFestivalMainEvent_FDF(StardewValley.Event __instance)
        {
            {
                Monitor.Log("setUpFestivalMainEvent_FDF loads", LogLevel.Debug);

                if (Monitor is null || __instance is null || !__instance.isSpecificFestival("spring24"))
                {
                    return;
                }

                try
                {
                    //Reflection to access protected Game1.Multiplayer
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

                    //Populates "leftoverGender" lists with all datable NPCs of each respective gender for selection, configurable switch for nonbinary characters
                    foreach (NPC character in charList)
                    {
                        if (character.datable.Equals(true))
                        {
                            int intgender = character.Gender;
                            switch (intgender)
                            {
                                case 0:
                                    leftoverMales.Add(character.Name);
                                    break;

                                case 1:
                                    leftoverFemales.Add(character.Name);
                                    break;

                                case 2:
                                    if (Config.AllowNonBinaryPartners.Equals(true))
                                    {
                                        try
                                        {
                                            //check to see if nonbinary dance partner has custom FDF sprites
                                            //load custom FDF sprites if available

                                            //add nonbinary partner to random leftoverGender list

                                            // int g = rnd.Next(1);
                                            // if (g == 0)
                                            // {
                                            //     leftoverMales.Add(character.Name);
                                            // }
                                            // else
                                            // {
                                            //     leftoverFemales.Add(character.Name);
                                            // }

                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            Monitor.Log("Flower Dance Fix cannot find custom FDF sprites for " + character.name + " and cannot add that NPC to dancer pools.", LogLevel.Debug);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Monitor.Log("Flower Dance Fix cannot add " + character.Name + " beacause that NPC does not have a valid gender- set configuration setting ''AllowNonBinaryPartners'' to true to add to dancer pools.", LogLevel.Debug);
                                        break;
                                    }
                            }
                            continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //Removes blacklisted datables from "leftoverGender" lists based on config
                    if (!String.IsNullOrEmpty(Config.DancerBlackList))
                    {
                        try
                        {
                            List<string> blackList = new List<string>(Config.DancerBlackList.Split('/'));

                            IEnumerable<string> toRemoveMale = blackList.Intersect(leftoverMales);
                            foreach (string i in toRemoveMale)
                            {
                                leftoverMales.Remove(i);
                                blackList.Remove(i);
                            }

                            IEnumerable<string> toRemoveFemale = blackList.Intersect(leftoverFemales);
                            foreach (string j in toRemoveFemale)
                            {
                                leftoverFemales.Remove(j);
                                blackList.Remove(j);
                            }

                            //Logs blacklisting activity to monitor
                            if (!blackList.Any())
                            {
                                Monitor.Log("Flower Dance Fix has successfully removed all blacklisted NPCs from dancer pool.");
                            }
                            else
                            {
                                string blackListError = string.Join(", ", blackList);
                                Monitor.Log("Flower Dance Fix failed to remove the following blacklisted NPCs from dancer pool:" + blackListError + ". Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log("Flower Dance Fix failed to parse dancer blacklist. Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
                        }
                    }

                    //Prevents selection of tourist datable characters based on config
                    if (Config.AllowTouristPartners.Equals(false))
                    {
                        foreach (NPC character in charList)
                        {
                            if (character.gender.Equals(0) && character.datable.Equals(true) && !character.homeRegion.Equals(2))
                            {
                                leftoverMales.Remove(character.Name);
                            }
                            else if (character.gender.Equals(1) && character.datable.Equals(true) && !character.homeRegion.Equals(2))
                            {
                                leftoverMales.Remove(character.Name);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    //Adds farmer-farmer and farmer-NPC pairs to dancelist- vanilla code
                    List<Farmer> farmers = (from f in Game1.getOnlineFarmers()
                                            orderby f.UniqueMultiplayerID
                                            select f).ToList();
                    while (farmers.Count > 0)
                    {
                        Farmer f2 = farmers[0];
                        farmers.RemoveAt(0);
                       /* if (multiplayer.isDisconnecting(f2) || f2.dancePartner.Value == null)
                        {
                            continue;
                        } */
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
                    while (females.Count < Config.MaxDancePairs && !leftoverFemales.Any() && !leftoverMales.Any())
                    {
                        //Random pair generation- config moderated
                        if (Config.NPCsHaveRandomPartners.Equals(true))
                        {
                            try
                            {
                                int r = rnd.Next(leftoverMales.Count);

                                string female = leftoverFemales.Last();
                                string randomMale = leftoverMales[r];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Flower Dance Fix made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }
                            catch (Exception)
                            {
                                Monitor.Log("Flower Dance Fix failed to fill NetDancePartner with random MF pairs.", LogLevel.Debug);
                            }
                        }
                        //"Love Interest" pair generation, followed by random pair generation for any remainders
                        else
                        {
                            string female = leftoverFemales.Last();
                            if (leftoverMales.Contains(Utility.getLoveInterest(female)))
                            {
                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(Utility.getLoveInterest(female)));
                                leftoverMales.Remove(Utility.getLoveInterest(female));
                                leftoverFemales.Remove(female);
                                
                                Monitor.Log("Flower Dance Fix used ''Love Interest'' method to pair" + female + " and " + Utility.getLoveInterest(female) + " and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                            }
                            else
                            {
                                int r = rnd.Next(leftoverMales.Count);
                                string randomMale = leftoverMales[r];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Flower Dance Fix made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }

                            Monitor.Log("Flower Dance Fix used vanilla method to fill NetDancePartner with pairs.", LogLevel.Debug);
                        }
                    }
                    //Generates spring24.json "mainEvent" value

                    string rawFestivalData = __instance.GetFestivalDataForYear("mainEvent");
                    for (int i = 1; i <= Config.MaxDancePairs && i <= females.Count(); i++)
                    {
                        string female2 = ((!females[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(females[i - 1].TryGetFarmer())) : females[i - 1].TryGetVillager().Name);
                        string male = ((!males[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(males[i - 1].TryGetFarmer())) : males[i - 1].TryGetVillager().Name);
                        rawFestivalData = rawFestivalData.Replace("Girl" + (i), female2);
                        rawFestivalData = rawFestivalData.Replace("Guy" + (i), male);
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

                catch (Exception ex)
                {
                    Monitor.Log($"Failed in {nameof(setUpFestivalMainEvent_FDF)}:\n{ex}", LogLevel.Error);
                }

            }
        }
    }
}
