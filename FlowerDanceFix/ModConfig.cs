﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerDanceFix
{
    public class ModConfig
    {
        //Changes number of pairs dancing- not tested for more than 6 pairs
        public static int MaxDancePairs { get; set; } = 6;

        //Pairs of NPCs dancing are random male-female pairs
        public static bool NPCsHaveRandomPartners { get; set; } = false;

        //TBC- Pairs of NPCs dancing are random pairs of random genders- will require additional sprites
        public static bool ForceHeteroPartners { get; set; } = true;

        //TBC- Code to select datables that are not male of female (ie undefined; gender = 2)
        public static bool AllowNonBinaryPartners { get; set; } = false;
      
        //TBC- Code to select datables living outside the valley (ie not town; homeRegion != 2)
        public static bool AllowTouristPartners { get; set; } = false;

        //TBC- Configureable blacklist of datables to be removed from genderedList pools
    }


}
