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
using Microsoft.Xna.Framework.Graphics;
using Harmony;

namespace FlowerDanceFix
{
    public class EventPatched
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

        public static void setUpFestivalMainEvent_FDF(StardewValley.Event __instance)
        {
            {

                if (Monitor is null || __instance is null || !__instance.isSpecificFestival("spring24"))
                {
                    return;
                }
                try
                {
                    /*
                    //Reflection to access protected Game1.Multiplayer
                    Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    */
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
                    List<NPC> usesFemaleFDF = new List<NPC>();
                    List<NPC> usesMaleFDF = new List<NPC>();

                    //Populates "leftoverGender" lists with all datable NPCs of each respective gender for selection, configurable switch for nonbinary characters
                    foreach (NPC character in charList)
                    {

                        if (character.datable.Equals(true) && !character.age.Equals(2))
                        {
                            int intgender = character.gender;
                            Monitor.Log(character.name + "'s gender is evaluated as " + intgender, LogLevel.Trace);

                            if (Config.MixGenderLines.Equals(false))
                            {
                                switch (intgender)
                                {
                                    //Adds male NPCs to leftoverMales, notes if custom FDF sprites exist
                                    case 0:
                                        leftoverMales.Add(character.name);
                                        Monitor.Log("Successfully added " + character.name + " to leftoverMales dancer pool.", LogLevel.Trace);
                                        if (Config.UseCustomFDFSprites.Equals(true))
                                        {
                                            if (HasFDFSprites(character).Equals(true))
                                            {
                                                usesMaleFDF.Add(character);
                                            }
                                            else
                                            {
                                                Monitor.Log("Could not locate custom FDF sprites for " + character.name + "- will revert to using vanilla sprite sheet.", LogLevel.Debug);
                                            }
                                        }
                                        break;

                                    //Adds female NPCs to leftoverFemales, notes if custom FDF sprites exist
                                    case 1:
                                        leftoverFemales.Add(character.name);
                                        Monitor.Log("Successfully added " + character.name + " to leftoverFemales dancer pool.", LogLevel.Trace);
                                        if (Config.UseCustomFDFSprites.Equals(true))
                                        {
                                            if (HasFDFSprites(character).Equals(true))
                                            {
                                                usesFemaleFDF.Add(character);
                                            }
                                            else
                                            {
                                                Monitor.Log("Could not locate custom FDF sprites for " + character.name + "- will revert to using vanilla sprite sheet.", LogLevel.Debug);
                                            }
                                        }
                                        break;

                                    case 2:
                                        if (Config.AllowNonBinaryPartners.Equals(true))
                                        {
                                            //Fails to add non-binary NPC to leftoverGender lists if custom sprites do not exist
                                            if (HasFDFSprites(character).Equals(false))
                                            {
                                                Monitor.Log("Failed to add non-binary NPC " + character.name + " to a leftoverGender dancer pool because no FDF sprites exist for that NPC.", LogLevel.Debug);
                                                break;
                                            }

                                            //Randomize placement of non-binary NPC in leftoverGender lists
                                            else
                                            {
                                                int g = rnd.Next(1);
                                                switch (g)
                                                {
                                                    case 0:
                                                        leftoverMales.Add(character.name);
                                                        Monitor.Log("Successfully added non-binary NPC" + character.name + " randomly to leftoverMales dancer pool.", LogLevel.Trace);
                                                        usesMaleFDF.Add(character);
                                                        break;

                                                    case 1:
                                                        leftoverFemales.Add(character.name);
                                                        Monitor.Log("Successfully added non-binary NPC" + character.name + " randomly to leftoverFemales dancer pool.", LogLevel.Trace);
                                                        usesFemaleFDF.Add(character);
                                                        break;
                                                }
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Monitor.Log("Failed to add non-binary NPC " + character.name + " to a leftoverGender dancer pool due to Config- AllowNonBinaryPartners = false.", LogLevel.Debug);
                                            break;
                                        }
                                }
                            }
                            else if (Config.MixGenderLines.Equals(true))
                            {
                                //Uses default placement in leftoverGender lists if custom FDF sprites do not exist
                                if (HasFDFSprites(character).Equals(false))
                                {
                                    switch (intgender)
                                    {
                                        case 0:
                                            leftoverMales.Add(character.name);
                                            Monitor.Log("Successfully added " + character.name + " to leftoverMales dancer pool. Could not randomize leftoverGender selection due to missing custom FDF sprites.", LogLevel.Debug);
                                            break;
                                        case 1:
                                            leftoverFemales.Add(character.name);
                                            Monitor.Log("Successfully added " + character.name + " to leftoverFemales dancer pool. Could not randomize leftoverGender selection due to missing custom FDF sprites.", LogLevel.Debug);
                                            break;
                                        case 2:
                                            Monitor.Log("Failed to add non-binary NPC " + character.name + " to a leftoverGender dancer pool because no FDF sprites exist for that NPC.", LogLevel.Debug);
                                            break;
                                    }
                                }
                                else
                                //Randomizes placement of all NPCs with custom FDF sprites between leftoverGender lists
                                {
                                    int g = rnd.Next(1);
                                    switch (g)
                                    {
                                        case 0:
                                            leftoverMales.Add(character.name);
                                            Monitor.Log("Successfully added " + character.name + " randomly to leftoverMales dancer pool.", LogLevel.Trace);
                                            usesMaleFDF.Add(character);
                                            break;

                                        case 1:
                                            leftoverFemales.Add(character.name);
                                            Monitor.Log("Successfully added " + character.name + " randomly to leftoverFemales dancer pool.", LogLevel.Trace);
                                            usesFemaleFDF.Add(character);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        Monitor.Log("Finished adding NPCs to leftoverGender dancer pools.", LogLevel.Debug);

                        List<string> tourists = new List<string>();

                        //Prevents selection of tourist datable characters based on config
                        if (Config.AllowTouristPartners.Equals(false))
                        {
                            if (character.datable.Equals(true) && !character.age.Equals(2) && !character.homeRegion.Equals(2))
                            {
                                tourists.Add(character.name);

                                int intgender = character.gender;
                                switch (intgender)
                                {
                                    case 0:
                                        leftoverMales.Remove(character.name);
                                        Monitor.Log("Successfully removed " + character.name + " from leftoverMales dancer pool. Configurable by AllowTouristPartners.", LogLevel.Trace);
                                        if (usesMaleFDF.Contains(character))
                                        {
                                            usesMaleFDF.Remove(character);
                                        }
                                        break;

                                    case 1:
                                        leftoverFemales.Remove(character.name);
                                        Monitor.Log("Successfully removed " + character.name + " from leftoverFemales dancer pool. Configurable by AllowTouristPartners.", LogLevel.Trace);
                                        if (usesFemaleFDF.Contains(character))
                                        {
                                            usesFemaleFDF.Remove(character);
                                        }
                                        break;
                                    case 2:
                                        if (Config.AllowNonBinaryPartners.Equals(true))
                                        {
                                            if (leftoverFemales.Contains(character.name))
                                            {
                                                leftoverFemales.Remove(character.name);
                                                Monitor.Log("Successfully removed non-binary NPC " + character.name + " from leftoverFemales dancer pool. Configurable by AllowTouristPartners and AllowNonBinaryPartners.", LogLevel.Trace);
                                                if (usesFemaleFDF.Contains(character))
                                                {
                                                    usesFemaleFDF.Remove(character);
                                                }
                                            }
                                            else
                                            {
                                                leftoverMales.Remove(character.name);
                                                Monitor.Log("Successfully removed non-binary NPC " + character.name + " from leftoverMales dancer pool. Configurable by AllowTouristPartners and AllowNonBinaryPartners.", LogLevel.Trace);
                                                if (usesMaleFDF.Contains(character))
                                                {
                                                    usesMaleFDF.Remove(character);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Monitor.Log("Failed to removed non-binary NPC " + character.name + " from leftoverGender dancer pool. Configurable by AllowTouristPartners and AllowNonBinaryPartners. If you are seeing this, elfuun made a big mistake somewhere. Please send her a bug report!", LogLevel.Alert);
                                        }
                                        break;
                                }
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
                                    Monitor.Log("Successfully removed blacklisted NPC " + i + " from dancer pool.", LogLevel.Trace);
                                    if (usesMaleFDF.Contains(character))
                                    {
                                        usesMaleFDF.Remove(character);
                                    }
                                }

                                IEnumerable<string> toRemoveFemale = blackList.Intersect(leftoverFemales);
                                foreach (string j in toRemoveFemale)
                                {
                                    leftoverFemales.Remove(j);
                                    blackList.Remove(j);
                                    Monitor.Log("Successfully removed blacklisted NPC " + j + " from dancer pool.", LogLevel.Trace);
                                    if (usesFemaleFDF.Contains(character))
                                    {
                                        usesFemaleFDF.Remove(character);
                                    }
                                }

                                //Logs blacklisting activity to monitor
                                if (!blackList.Any())
                                {
                                    Monitor.Log("Successfully removed all blacklisted NPCs from dancer pool.", LogLevel.Trace);
                                }
                                else
                                {
                                    string blackListError = string.Join(", ", blackList);
                                    Monitor.Log("Failed to remove the following blacklisted NPCs from dancer pool: " + blackListError + ". Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);

                                    if (Config.AllowTouristPartners.Equals(false))
                                    {
                                        IEnumerable<string> blTouristError = blackList.Intersect(tourists);
                                        Monitor.Log("Failed to remove the following blacklisted NPCs from dancer pool: " + blTouristError + " because those NPCs were already excluded due to ''Tourist'' status, due to config- AllowTouristPartners = true", LogLevel.Trace);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Monitor.Log("Flower Dance Fix failed to parse dancer blacklist. Please check that NPCs are referenced by key, and seperated by a single forward slash.", LogLevel.Debug);
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

                        /*
                        if (multiplayer.isDisconnecting(f2) || f2.dancePartner.Value == null)
                        {
                            continue;
                        }
                        */

                        if (f2.dancePartner.GetGender() == 1)
                        {
                            females.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                leftoverFemales.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            males.Add(new NetDancePartner(f2));

                            Monitor.Log("Made a pair of farmer" + f2 + " and NPC " + f2.dancePartner + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        if (f2.dancePartner.GetGender() == 0)
                        {
                            males.Add(f2.dancePartner);
                            if (f2.dancePartner.IsVillager())
                            {
                                leftoverMales.Remove(f2.dancePartner.TryGetVillager().Name);
                            }
                            females.Add(new NetDancePartner(f2));

                            Monitor.Log("Made a pair of farmer" + f2 + " and NPC " + f2.dancePartner + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        if (f2.dancePartner.IsFarmer())
                        {
                            farmers.Remove(f2.dancePartner.TryGetFarmer());

                            Monitor.Log("Made a pair of farmer" + f2 + " and farmer " + f2.dancePartner.TryGetFarmer() + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                        }
                        else
                        {
                            Monitor.Log("Did not add farmer " + f2 + " to NetDancePairs because they did not have a partner", LogLevel.Trace);
                        }
                    }

                    //Generates NPC-NPC pairs
                    do
                    {
                        int rF = rnd.Next(leftoverFemales.Count);
                        string female = leftoverFemales[rF];

                        //Random pair generation- config moderated
                        if (Config.NPCsHaveRandomPartners.Equals(true))
                        {
                            try
                            {
                                int r = rnd.Next(leftoverMales.Count);
                                string randomMale = leftoverMales[r];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Randomly made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }
                            catch (Exception)
                            {
                                Monitor.Log("Failed to fill NetDancePartner with random MF pairs.", LogLevel.Debug);
                                break;
                            }
                        }
                        //"Love Interest" pair generation, followed by random pair generation for any remainders
                        else
                        {
                            string loveInterestMale = Utility.getLoveInterest(female);
                            if (leftoverMales.Contains(Utility.getLoveInterest(female)) || checkCustomLoveInterestIsMutual(female).Equals(true))
                            {
                                if (Utility.getLoveInterest(female).Equals(null) && checkCustomLoveInterestIsMutual(female).Equals(true))
                                {
                                    females.Add(new NetDancePartner(female));
                                    males.Add(new NetDancePartner(loveInterestMale));
                                    leftoverMales.Remove(loveInterestMale);
                                    leftoverFemales.Remove(female);
                                    Monitor.Log("Used custom ''Love Interest'' method to make a pair with " + female + " and " + loveInterestMale + " and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                                }
                                Monitor.Log("Used ''Love Interest'' method to make a pair with " + female + " and " + loveInterestMale + " and successfully entered pair into NetDancePartner.", LogLevel.Trace);
                            }
                            else
                            {
                                int rM = rnd.Next(leftoverMales.Count);
                                string randomMale = leftoverMales[rM];

                                females.Add(new NetDancePartner(female));
                                males.Add(new NetDancePartner(randomMale));

                                leftoverFemales.Remove(female);
                                leftoverMales.Remove(randomMale);

                                Monitor.Log("Randomly made a pair with " + female + " and " + randomMale + " and successfully entered pair into NetDancePartner", LogLevel.Trace);
                            }
                        }
                    }
                    while ((females.Count < Config.MaxDancePairs) && leftoverFemales.Any() && leftoverMales.Any());

                    if (!leftoverFemales.Any())
                    {
                        string unselectedLOFemales = String.Join(", ", leftoverFemales);
                        Monitor.Log("After pair generation, leftoverFemales contains the following NPCs not selected for dance: " + unselectedLOFemales, LogLevel.Trace);
                    }

                    if (!leftoverMales.Any())
                    {
                        string unselectedLOMales = String.Join(", ", leftoverMales);
                        Monitor.Log("After pair generation, leftoverMales contains the following NPCs not selected for dance: " + unselectedLOMales, LogLevel.Trace);
                    }
                    {
                        //Generates spring24.json "mainEvent" value

                        string rawFestivalData = __instance.GetFestivalDataForYear("mainEvent");


                        string eventSetup = "pause 500 / playMusic none / pause 500 / globalFade / viewport - 1000 - 1000/loadActors MainEvent";

                        //build farmers statement
                        StringBuilder warpFarmers = new StringBuilder();


                        int i = 1;
                        do
                        {
                            string female2 = ((!females[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(females[i - 1].TryGetFarmer())) : females[i - 1].TryGetVillager().Name);
                            string male = ((!males[i - 1].IsVillager()) ? ("farmer" + Utility.getFarmerNumberFromFarmer(males[i - 1].TryGetFarmer())) : males[i - 1].TryGetVillager().Name);

                            rawFestivalData = rawFestivalData.Replace("Girl" + (i), female2);


                            rawFestivalData = rawFestivalData.Replace("Guy" + (i), male);
                            i++;
                        }
                        while (i <= Config.MaxDancePairs && i <= females.Count());


                        //Set up regex for farmer sprite version
                        Regex regex = new Regex("showFrame (?<farmerName>farmer\\d) 44");
                        Regex showFrameGirl = new Regex("showFrame (?<farmerName>farmer\\d) 40");
                        Regex animation1Guy = new Regex("animate (?<farmerName>farmer\\d) false true 600 44 45");
                        Regex animation1Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 43 41 43 42");
                        Regex animation2Guy = new Regex("animate (?<farmerName>farmer\\d) false true 300 46 47");
                        Regex animation2Girl = new Regex("animate (?<farmerName>farmer\\d) false true 600 46 47");

                        //Set up regex for FDF sprite version
                        Regex showFrameGuyFDF = new Regex("showFrame () 44");
                        Regex showFrameGirlFDF = new Regex("showFrame () 40");
                        Regex animationGirl1FDF = new Regex("animate () false true 600 43 41 43 42");
                        Regex animationGuy1FDF = new Regex("animate () false true 600 44 45");
                        Regex animationGirl2FDF = new Regex("animate () false true 600 46 47");
                        Regex animationGuy2FDF = new Regex("animate () false true 300 46 47");

                        //Replaces base with farmer sprites
                        rawFestivalData = regex.Replace(rawFestivalData, "showFrame $1 12/faceDirection $1 0");
                        rawFestivalData = showFrameGirl.Replace(rawFestivalData, "showFrame $1 0/faceDirection $1 2");
                        rawFestivalData = animation1Guy.Replace(rawFestivalData, "animate $1 false true 600 12 13 12 14");
                        rawFestivalData = animation1Girl.Replace(rawFestivalData, "animate $1 false true 596 4 0");
                        rawFestivalData = animation2Guy.Replace(rawFestivalData, "animate $1 false true 150 12 13 12 14");
                        rawFestivalData = animation2Girl.Replace(rawFestivalData, "animate $1 false true 600 0 3");

                        //Replaces base with FDF sprites
                        foreach (NPC character in usesFemaleFDF)
                        {
                            rawFestivalData = showFrameGirlFDF.Replace(rawFestivalData, "showFrame $1 0");
                            rawFestivalData = animationGirl1FDF.Replace(rawFestivalData, "animate $1 2 0 2 1");
                            rawFestivalData = animationGirl2FDF.Replace(rawFestivalData, "animate $1 6 7");
                        }
                        foreach (NPC character in usesMaleFDF)
                        {
                            rawFestivalData = showFrameGuyFDF.Replace(rawFestivalData, "showFrame $1 8");
                            rawFestivalData = animationGuy1FDF.Replace(rawFestivalData, "animate $1 8 9 8 9");
                            rawFestivalData = animationGuy2FDF.Replace(rawFestivalData, "animate $1 10 11 10 11");
                        }
                        string[] newCommands = (__instance.eventCommands = rawFestivalData.Split('/'));



                    }

                }

                catch (Exception ex)
                {
                    Monitor.Log($"Failed in {nameof(setUpFestivalMainEvent_FDF)}:\n{ex}", LogLevel.Error);
                }
            }
        }

        //Arugh gotta do all kinds of sting builder stuff
        public static bool HasFDFSprites(NPC character)
        {
            try
            {
                Game1.temporaryContent.Load<Texture2D>("assets\\" + NPC.getTextureNameForCharacter(character.name) + "_FDF");
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void FDFSpriteHandler(NPC character)
        {
            try
            {
                character.Sprite.LoadTexture("assets\\" + NPC.getTextureNameForCharacter(character.name) + "_FDF");
                Monitor.Log("Successfully loaded FDF sprite for " + character.name + ".", LogLevel.Trace);
            }
            catch (Exception)
            {
                character.Sprite.LoadTexture("Characters\\" + NPC.getTextureNameForCharacter(character.name));
                Monitor.Log("Failed to load FDF sprite for " + character.name + ". Reloading vanilla sprite.", LogLevel.Debug);

            }
        }

        public static bool checkCustomLoveInterestIsMutual(string character)
        {
            NPC Target = Game1.getCharacterFromName(character);
            NPC TargetLoveInterest = Game1.getCharacterFromName(Target.loveInterest);

            if (TargetLoveInterest.name.Equals(Target.loveInterest) && Target.name.Equals(TargetLoveInterest.loveInterest))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool checkVanillaLoveInterestIsMutual(string character)
        {
            string TargetLoveInterest = Utility.getLoveInterest(character);

            if (TargetLoveInterest.Equals(null) || Utility.getLoveInterest(TargetLoveInterest).Equals(null))
            {
                return false;
            }
            else if (character.Equals(Utility.getLoveInterest(TargetLoveInterest)) && TargetLoveInterest.Equals(Utility.getLoveInterest(character)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool checkForGayRelationship(string character)
        {
            NPC Target = Game1.getCharacterFromName(character);
            string TargetLIString = Utility.getLoveInterest(character);
            NPC TargetLoveInterest = Game1.getCharacterFromName(TargetLIString);


            if (Target.gender.Equals(TargetLoveInterest.gender))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
    /*
     *  ▄▄
     * ██▀█▄▄▄
     *  ███
     *   █
     *  ███▄
     * ██████▄
     * ███████
     * ███████
     * ▀█████▀
     *   ▀█▀
     *█▀▀▀▀
     *▀▀▀
     *Aw man! That freakin' duck is taking up space in my code!
     */
}

