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

namespace FlowerDanceFix
{
    public class ModConfig
    {
        public ModConfig Config;

        public bool ExperimentalFeatures { get; set; } = false;
        
        //WIP- Changes number of pairs dancing- not tested for more than 6 pairs
        public int MaxDancePairs { get; set; } = 6;

        //Pairs of NPCs dancing are random male-female pairs
        public bool NPCsHaveRandomPartners { get; set; } = false;

        //WIP- Pairs of NPCs dancing are random pairs of random genders- will require additional sprites
        public bool ForceHeteroPartners { get; set; } = true;

        //WIP- Code to select datables that are not male of female (ie undefined; gender = 2)- will require additional sprites
        public bool AllowNonBinaryPartners { get; set; } = false;

        //Can select datables living outside the valley (ie not town; homeRegion != 2)
        public bool AllowTouristPartners { get; set; } = false;

        //Configureable blacklist of datables to be removed from genderedList pools- enclose NPC base name in quotes, deliniate forward slash
        public string DancerBlackList { get; set; } = "";

    }
}
