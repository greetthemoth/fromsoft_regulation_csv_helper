using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EldenRingCSVHelper
{
    public class Program
    {
        
        const string VanillaFilesPath = @"C:\CODING\Souls Modding\Elden Ring Modding\ModEngine-2.0.0\mod\Vanilla\CSV\";

        const string DocsFilesPath = @"C:\CODING\Souls Modding\ModdingTools\Docs\";
        //update this periodically by updating the Vanilla project with the games regulation bin.
        //NVM Map studios has a method for doing just press "Upgrade Params" in the new version.
        //You should update the Vanilla Files tho.

        //WIth Yapped
        //...Open Yapped.
        //......exists at CODING\Souls Modding\ModdingTools\Yapped-Rune-Bear-Shortcut
        //...Open the your Elden RIng regulation bin.
        //......File -> Open -> Find and Select "C:\Steam\steamapps\common\ELDEN RING\Game\regulation.bin" 
        //...Change Project to "Vanilla"
        //......Settings -> View General Setting -> Set "Project Name" to "Vanilla" (no quotes)

        //...Apply both Stock and Project Row Names before exporting.
        //......Field Data - Import Stock Row Names. Yes
        //......Field Data - Import Project Row Names. No (OVERRIDE EXISTING NAMES).

        //...Mass export the data to create the new csv files (should override the existing files)
        //......Field Data - Mass Export Data
        //

        //...Additional notes. 
        //...Make sure the CSV is not unfurled when exporting. 
        //...Settings: Export Settings: Unfurl export.


        //With MapStudio
        //...Open Map Studio (make sure its the most recent version updated for the most recent update)
        //......exists at CODING\Souls Modding\ModdingTools\DSMapStudio
        //...Create a new project using a copy of the vinalla regulation bin.
        //...GO to the param editor.
        //...Export each of the params...
        //......click the param.
        //......Select all the lines using ctrl+a
        //......Edit-> Export CSV -> as Fill -> All.
        //......Save the csv in the Vanilla file path.



        //Import names (example "- FP" and "- No FP" from an old file) with map studio.
        //...Have a target file with the prefered names, temporatily remove the header from it.
        //...Edit -> Import CSV -> Set Delmiter to ";" (or the one in target file), then 'From File' -> Name _> Find the File with the names and select it.
        //...Undo the deletion of the header in the temproaty change to the target file.     


        public static ParamFile BonfireWarpParam = new ParamFile(VanillaFilesPath, "BonfireWarpParam.csv");


        //to read
        public static ParamFile EquipParamWeapon = new ParamFile(VanillaFilesPath, "EquipParamWeapon.csv");
        public static ParamFile EquipParamGoods = new ParamFile(VanillaFilesPath, "EquipParamGoods.csv");
        public static ParamFile EquipParamProtector = new ParamFile(VanillaFilesPath, "EquipParamProtector.csv");
        //to write

        //currently updated to: 1.10
        public static ParamFile AtkParam_Pc = new ParamFile(VanillaFilesPath, "AtkParam_Pc.csv");
        public static ParamFile BehaviorParam_PC = new ParamFile(VanillaFilesPath, "BehaviorParam_PC.csv"); //The names of this file should be renamed to the proper FP ones found in 
        public static ParamFile Bullet = new ParamFile(VanillaFilesPath, "Bullet.csv");
        public static ParamFile CalcCorrectGraph = new ParamFile(VanillaFilesPath, "CalcCorrectGraph.csv");
        public static ParamFile ItemLotParam_enemy = new ParamFile(VanillaFilesPath, "ItemLotParam_enemy.csv");
        public static ParamFile ItemLotParam_map = new ParamFile(VanillaFilesPath, "ItemLotParam_map.csv");
        public static ParamFile Magic = new ParamFile(VanillaFilesPath, "Magic.csv");
        public static ParamFile SpEffectParam = new ParamFile(VanillaFilesPath, "SpEffectParam.csv");
        public static ParamFile SwordArtsParam = new ParamFile(VanillaFilesPath, "SwordArtsParam.csv");
        public static ParamFile NpcParam = new ParamFile(VanillaFilesPath, "NpcParam.csv");

        public static ParamFile ShopLineupParam = new ParamFile(VanillaFilesPath, "ShopLineupParam.csv");


        static void Main()
        {
            if (RunSettings.CreateSmithingStoneMods)
            {
                RunOverride_CreateSmithingStoneMod();
            }
            else
            {

                RunSettings.SetToRun();

                weaponArtsCostMoreMana();
                //                                          weaponArtsDamageModify();
                Console.WriteLine("1");
                spellsCostMoreMana();
                Console.WriteLine("2");
                //                                          naturalFpRegen();
                changeMindToBeLinear();
                Console.WriteLine("3");
                reduceMaxManaSystem();
                Console.WriteLine("4");
                manaRegenOnHit();
                Console.WriteLine("5");

                manaGradualRegenOnCast();
                Console.WriteLine("6");

                //                                //makeRuneArchsGive2500()
                //MaloModShopChanges();
                SmithingStoneShopLineupChanges();
                Console.WriteLine("7");

                giveEnemieSmithingStoneDrops(true);
                Console.WriteLine("8");

                replaceOpenWorldSmithingStones();
                Console.WriteLine("9");


                limitWarpPoints();
                Console.WriteLine(10);

                Output();
            }

            if (RunSettings.PrintOnConsole)
                Console.ReadKey();
        }
        static void Output()
        {
            if (!RunSettings.Testing)
            {
                if (RunSettings.Write)
                {
                    if (RunSettings.Write_AllModifiedFiles)
                    {
                        Util.println("_______________________________________");
                        int filesPrinted = ParamFile.WriteModifiedFiles();
                        Util.println("_______________________________________");
                        Util.println("Succesfully printed " + filesPrinted + " files.");
                    }
                    else
                    {
                        RunSettings.ToRun.WriteModifiedFile();
                    }
                }
                else
                if (RunSettings.ToRun != null && RunSettings.PrintFile)
                {

                    if (RunSettings.PrintFile_OnlyModifiedLines)
                        RunSettings.ToRun.PrintModifiedFile();
                    else
                        RunSettings.ToRun.PrintFile();
                }
            }
            else
            {
                //Run Tests
                RunSettings.Tests();
            }
        }

        static void RunOverride_CreateSmithingStoneMod()
        {
            RunSettings.Testing = false;
            RunSettings.Write_OnlyModifiedLines = true;
            RunSettings.RunIfNull = true;
            string exportDirectory = @"C:\CODING OUTPUT\CSV\Individual Options (slower)";
            
            var SmithingDropMultsToWrite = new float[]
            {
                1,
                1.5f,
                2f,
                3f,
                5f,
            };
           


            for (int i = 0; i < SmithingDropMultsToWrite.Length; i++)
            {
                string multString = "x" + SmithingDropMultsToWrite[i];
                string multDirString = @"\StoneDrops\" + multString + @" Stones";
                if (SmithingDropMultsToWrite[i] == 1)
                {
                    multDirString += "(vanilla-like)";
                    multString = "";
                }
                else if (SmithingDropMultsToWrite[i] == 1.5f)
                    multDirString += "(recommended)";
                RunSettings.Write_directory = exportDirectory + multDirString;
                giveEnemieSmithingStoneDrops(false, true, SmithingDropMultsToWrite[i], false);
                ParamFile.WriteModifiedFiles("", "__" + multString +"StoneDrops");
                ParamFile.ResetAll();
            }

            string dropTypeDirString = @"\RuneDrops";
            RunSettings.Write_directory = exportDirectory + dropTypeDirString;
            giveEnemieSmithingStoneDrops(true, false);
            ParamFile.WriteModifiedFiles("", "__" + "RuneDrops");
            ParamFile.ResetAll();
            

            string OTED_DirString = @"\OneTime Equipment Drops (import last)";
            RunSettings.Write_directory = exportDirectory + OTED_DirString;
            giveEnemieSmithingStoneDrops(false, false,1,true);
            ParamFile.WriteModifiedFiles("", "__" + "OTED");
            ParamFile.ResetAll();

            string worldChanges_DirString = @"\World Changes";
            RunSettings.Write_directory = exportDirectory + worldChanges_DirString;
            SmithingStoneShopLineupChanges();
            replaceOpenWorldSmithingStones();
            ParamFile.WriteModifiedFiles("", "__WorldChanges IMPORT - ");
            ParamFile.ResetAll();

            exportDirectory = @"C:\CODING OUTPUT\CSV\Choose Options";
            {
                
                for (int i = 0;i < SmithingDropMultsToWrite.Length; i++)
                {
                    string multString = "x" + SmithingDropMultsToWrite[i];
                    string multDirString = @"\"+multString+@" Stones";
                    if (SmithingDropMultsToWrite[i] == 1)
                    {
                        multDirString += "(vanilla-like)";
                        multString = "";
                    }else if (SmithingDropMultsToWrite[i] == 1.5f)
                        multDirString += "(recommended)";
                    for (bool WorldChanges = false; ; WorldChanges = true)
                    {
                        string worldChanges_string = "";
                        if (WorldChanges)
                            worldChanges_string = "+WC";//" and OneTimeEquipmentDrops";

                        worldChanges_DirString = @"\";
                        if (WorldChanges)
                            worldChanges_DirString += @"World Changes";
                        else
                            worldChanges_DirString += @"No World Changes";

                        for (bool OneTimeEquipmentDrop = false; ; OneTimeEquipmentDrop = true)
                        {
                            string OTED_string = "";
                            if (OneTimeEquipmentDrop)
                                OTED_string = "+OTED";//" and OneTimeEquipmentDrops";

                            OTED_DirString = @"\";
                            if (OneTimeEquipmentDrop)
                                OTED_DirString += @"OneTime Equipment Drops";
                            else
                                OTED_DirString += @"Regular Equipment Drops";

                            for (bool DropRunes = false; ; DropRunes = true)
                            {
                                string dropTypeString = "StDrops";//" StoneDrops";
                                if (DropRunes)
                                    dropTypeString += "+RuDrops";//" and RuneDrops";
                                                                 //else
                                                                 //dropTypeString += "Only";

                                dropTypeDirString = @"\";
                                if (DropRunes)
                                    dropTypeDirString += @"StoneDrops and RuneDrops";
                                else
                                    dropTypeDirString += @"StoneDrops ONLY";

                                RunSettings.Write_directory = exportDirectory + dropTypeDirString + multDirString + OTED_DirString + worldChanges_DirString;
                                giveEnemieSmithingStoneDrops(DropRunes, true, SmithingDropMultsToWrite[i], OneTimeEquipmentDrop);
                                if (WorldChanges)
                                {
                                    SmithingStoneShopLineupChanges();
                                    replaceOpenWorldSmithingStones();
                                }
                                ParamFile.WriteModifiedFiles("", "__" + multString + dropTypeString + OTED_string + worldChanges_string);
                                ParamFile.ResetAll();

                                if (DropRunes == true)
                                    break;
                            }
                            if (OneTimeEquipmentDrop == true)
                                break;
                        }
                        if (WorldChanges == true)
                            break;
                    }

                }
                
            }//old version


        }

        static void limitWarpPoints()
        {
            if (!IsRunningParamFile(new ParamFile[] { BonfireWarpParam }))
                return;

            int[] warpableGraceIds = new int[]
            {
                //Churches
                110003, //[Leyndell, Royal Capital] Lower Capital Church
                61423600, //[Limgrave] Church of Elleh
                61463800, //[Limgrave] Third Church of Marika
                61413500, //[Limgrave] Church of Dragon Communion
                61433400, //[Weeping Peninsula] Church of Pilgrimage
                //61443300, //[Weeping Peninsula] Callu Baptismal Church
                61413300, //[Weeping Peninsula] Fourth Church of Marika
                62364900, //[Bellum Highway] Bellum Church
                62374600, //[Liurnia of the Lakes] Church of Vows
                62374900, //[Bellum Highway] Church of Inhibition
                63435000, //[Capital Outskirts] Minor Erdtree Church
                64464000, //[Caelid] Smoldering Church
                64503800, //[Caelid] Church of the Plague
                65545500, //[Mountaintops of the Giants] First Church of Marika
                65515300, //[Flame Peak] Church of Repose

                //Concentrated Snowfields
                65475800, //[Consecrated Snowfield] Apostate Derelict
                //65485700, //Consecrated Snowfield] Ordina, Liturgical Town

                //Because it makes too much sense
                62334700, //[Liurnia of the Lakes] The Four Belfries

                //Volcano Manor (Because Marika is the GEQ)
                160001, //[Volcano Manor] Temple of Eiglay
                160003, //[Volcano Manor] Prison Town Church

                
                //Rises
                62334000, //[Moonlight Altar] Chelona's Rise
                62345000, //[Liurnia of the Lakes] Ranni's Rise
                62344300, //[Liurnia of the Lakes] Converted Tower

                //Radagonian Exception
                140001, //[Academy of Raya Lucaria] Debate Parlor

                //Raya Lucaria
                140000, //[Academy of Raya Lucaria] Raya Lucaria Grand Library
                //140002, //Academy of Raya Lucaria] Church of the Cuckoo

                //Worship Areas
                64514300, //[Greyoll's Dragonbarrow] Bestial Sanctum
                61433000, //[Weeping Peninsula] Morne Moangrave
                150001, //[Elphael, Brace of the Haligtree] Prayer Room

                //Farum Azula
                130002, //[Crumbling Farum Azula] Dragon Temple Altar
                130000, //[Crumbling Farum Azula] Maliketh, the Black Blade
                130001, //[Crumbling Farum Azula] Dragonlord Placidusax

                /*//Divine Towers (entrances)
                341000, //[Stormhill] Limgrave Tower Bridge
                341101, //[Liurnia of the Lakes] Liurnia Tower Bridge
                110009, //[Leyndell, Royal Capital] Divine Bridge
                110505, //[Leyndell, Ashen Capital] Divine Bridge
                341202, //[Capital Outskirts] Divine Tower of West Altus: Gate
                341400, //[Forbidden Lands] Divine Tower of East Altus: Gate*/
                
                //Divine Towers
                341002, //[Stormhill] Divine Tower of Limgrave
                341102, //[Liurnia of the Lakes] Divine Tower of Liurnia
                341200, //[Capital Outskirts] Divine Tower of West Altus
                341302, //[Greyoll's Dragonbarrow] Divine Tower of Caelid: Center
                341301, //[Greyoll's Dragonbarrow] Divine Tower of Caelid           //(basement)
                341401, //[Forbidden Lands] Divine Tower of East Altus
                341500, //[Greyoll's Dragonbarrow] Isolated Divine Tower

                


            };


            var IsNotWarpableCond = new Condition.IDCheck(warpableGraceIds).IsFalse;

            Lines notWarpableLines = BonfireWarpParam.GetLinesOnCondition(IsNotWarpableCond);

            Util.println(notWarpableLines.lines.Count);

            notWarpableLines.Operate(new SetFieldTo(BonfireWarpParam.GetFieldIndex("dispMask00"),"0"));


        }
        static void giveEnemieSmithingStoneDrops(bool RUNES = false, bool STONES = true, float DROPMULT = 1, bool OneTimeWeaponAndArmorDrops = false, float MATDROPMULT = 1)
        {

            if (!IsRunningParamFile(new ParamFile[]{ ItemLotParam_enemy, NpcParam, ItemLotParam_map}))
                return;

            Lines StoneLines = new Lines(ItemLotParam_enemy);

            const float OneTimeWeaponAndArmorDrops_WeaponDropChanceMult = 1.6f;
            const float OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult = 0.4f;
            const float OneTimeWeaponAndArmorDrops_ArmorDropChanceMult = 2f;
            const float OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult = 0.2f;

            const int maxLotIndex = 8;



            Dictionary<int, float> npcsDocDifficultyDict = new Dictionary<int, float>();
            Dictionary<int, int> npcsIdToSpLevelsDict= new Dictionary<int, int>();
            //misbegotten drop too much.
            //set npc Difficulty Dict using NPC Locations
            const float levelEstimateIntreval = 0.5f;



            List<int> BossOrMiniBossIds = new List<int>();
            BossOrMiniBossIds = new int[]
            {
20300024,// Rennala, Queen of the Full Moon
20310024,// Rennala, Queen of the Full Moon
21000820,// Black Knife Assassin (Boss)
21000830,// Black Knife Assassin (Boss)
21000930,// Black Knife Assassin (Boss)
21001010,// Black Knife Assassin (Limgrave Catacombs Boss)
21002922,// Alecto, Black Knife Ringleader
21100072,// Beast Clergyman (Farum Azula)
21101072,// Maliketh (Farum Azula)
21200056,// Malenia, Blade of Miquella
21202056,// Malenia, Blade of Miquella
21300014,// Margit, the Fell Omen
21300534,// Morgott, the Omen King
21403050,// Fell Twin
21403150,// Fell Twin
21900078,// Radagon of the Golden Order
22000078,// Elden Beast
25000010,// Crucible Knight (Limgrave Evergaol Boss)
25000933,// Crucible Knight Ordovis
25000941,// Crucible Knight (Redmane Boss)
25001066,// Crucible Knight Siluria
25001933,// Crucible Knight Ordovis (Auriza Boss)
30500051,// Commander Niall
30500140,// Commander O'Neil
31000010,// Bell Bearing Hunter (Limgrave)
31000020,// Bell Bearing Hunter
31000033,// Bell Bearing Hunter
31000042,// Bell Bearing Hunter
31000931,// Elemer of the Briar (Shaded Castle)
31500010,// Night's Cavalry (Limgrave Boss)
31500020,// Night's Cavalry
31500042,// Night's Cavalry
31500050,// Night's Cavalry
31500052,// Night's Cavalry (Glaive)
31501012,// Night's Cavalry (Weeping Peninsula Boss)
31501030,// Night's Cavalry
31501040,// Night's Cavalry
31501052,// Night's Cavalry (Flail)
31811024,// Red Wolf of Radagon Sword (Archives)
31811032,// Red Wolf of the Champion
32500033,// Draconic Tree Sentinel (Capital Outskirts)
32510010,// Tree Sentinel (Limgrave)
32510030,// Tree Sentinel (Altus Plateua)
32511030,// Tree Sentinel - Torch (Altus Plateua)
32520054,// Loretta, Knight of the Haligtree
32520921,// Royal Knight Loretta
33000940,// Nox Swordstress (Boss)
33001940,// Nox Monk (Boss)
33500920,// Crystalian (Boss)
33500930,// Crystalian (Ringblade)
33500940,// Putrid Crystalian (Ringblade)
33501920,// Crystalian (Spear)
33501930,// Crystalian (Spear)
33501940,// Putrid Crystalian (Spear)
33502920,// Crystalian (Staff)
33502940,// Putrid Crystalian (Staff)
34000940,// Frenzied Grave Warden Duelist (Boss)
34000952,// Putrid Grave Warden Duelist (Boss)
34001110,// Grave Warden Duelist (Boss)
34001133,// Grave Warden Duelist (Auriza Boss)
34510912,// Scaly Misbegotten (Morne Tunnel Boss)
34600913,// Leonine Misbegotten (Castle Morne Boss)
34600930,// Misbegotten Warrior
34600941,// Misbegotten Warrior
34601952,// Misbegotten Crusader
35500930,// Sanguine Noble (Boss)
35600030,// Godskin Apostle
35600042,// Godskin Apostle
35600172,// Godskin Apostle {Godskin Duo)
35600950,// Godskin Apostle
35600972,// Godskin Apostle (Godskin Duo HP)
35700038,// Godskin Noble (Volcano Manor)
35700172,// Godskin Noble (Godskin Duo)
35700950,// Godskin Noble
36001920,// Onyx Lord (Boss)
36001933,// Onyx Lord (Boss)
36640012,// Cemetery Shade (Weeping Peninsula)
36640040,// Cemetery Shade (Boss)
36640920,// Cemetery Shade (Boss)
37011930,// Perfumer Tricia
37040940,// Battlemage Hugues (Boss)
38000920,// Cleanrot Knight (Boss)
38000940,// Cleanrot Knight (Boss)
38001940,// Cleanrot Knight (Boss)
38100932,// Kindred of Rot (Boss)
38102932,// Kindred of Rot (Boss)
39701910,// Beastman of Farum Azula (Limgrave Cave Boss)
39701942,// Armored Beastman of Farum Azula (Boss)
39702942,// Azula Beastman (Boss)
40200920,// Royal Revenant (Boss)
41200910,// Demi-Human Chief (Duo Boss)
41300032,// Demi-Human Queen Maggie
41301030,// Demi-Human Queen Gilika
41301932,// Demi-Human Queen Margot
41402920,// Spiritcaller Snail
41402950,// Spiritcaller Snail
42600110,// Erdtree Burial Watchdog (Limgrave Catacombs)
42600512,// Erdtree Burial Watchdog (Impalers Catacombs)
42600940,// Erdtree Burial Watchdog (Minor Erdtree Catacombs)
42601920,// Erdtree Burial Watchdog (Cliffbottom Catacombs)
42601940,// Erdtree Burial Watchdog (Minor Erdtree Catacombs)
42602932,// Erdtree Burial Watchdog (Wyndham Catacombs)
42900010,// Bloodhound Knight Darriwil
42900920,// Bloodhound Knight (Boss)
43400910,// Mad Pumpkin Head (Boss)
43400940,// Mad Pumpkin Head (Flail)
43401940,// Mad Pumpkin Head (Hammer)
44700938,// Abductor Virgin (Boss)
44701938,// Abductor Virgin (Boss)
44800912,// Miranda Blossom
44800930,// Miranda the Blighted Bloom
45000010,// Flying Dragon Agheel (Limgrave)
45000042,// Flying Dragon Greyll (Caelid)
45010040,// Decaying Ekzykes (Caelid)
45020920,// Glintstone Dragon Smarag
45021920,// Glintstone Dragon Smarag
45021922,// Glintstone Dragon Adula
45030050,// Borealis the Freezing Fog (Mountaintops)
45102030,// Ancient Dragon Lansseax (Boss)
45110066,// Lichdragon Fortissax
45200072,// Dragonlord Placidusax
45800030,// Wormface (Boss)
46010920,// Carian Knight Bols (Boss)
46030910,// Stonedigger Troll (Boss)
46030930,// Stonedigger Troll (Boss)
46200062,// Astel, Stars of Darkness
46200952,// Astel, Stars of Darkness (Snowfield Mine)
46300912,// Runebear (Boss)
46400007,// Ulcerated Tree Spirit (Boss)
46400032,// Ulcerated Tree Spirit (Boss)
46400942,// Ulcerated Tree Spirit (Boss)
46400950,// Ulcerated Tree Spirit (Boss)

46500262,// Dragonkin Soldier (Lake of Rot)
46500265,// Dragonkin Soldier (Nokron)
46500960,// Dragonkin Soldier of Nokstella Healthbar (Boss)
46600910,// Guardian Golem (Limgrave Boss)
46700065,// Ancestor Spirit (Regal)
46700964,// Ancestor Spirit (Nonregal)
46800032,// Full-Grown Fallingstar Beast (Volcano Top)
46801930,// Fallingstar Beast (Altus Plateau)
46801940,// Fallingstar Beast (Sellia Tunnel)
46900008,// Grafted Scion (Boss)
47100038,// God-Devouring Serpent
47101038,// Rykard- Lord of Blasphemy
47200070,// Godfrey- First Elden Lord
47200134,// Godfrey- First Elden Lord (Phantom)
47210070,// Hoarah Loux
47300040,// Starscourge Radahn
47500014,// Godrick the Grafted (Stormveil Castle)
47500030,// Godefroy the Grafted (Altus)
47600050,// Fire Giant
47601050,// Fire Giant
47700165,// Valiant Gargoyle (Twinblade)
47700250,// Black Blade Kindred (Forbidden Lands)
47701165,// Valiant Gargoyle
47701242,// Black Blade Kindred
48000068,// Mohg, Lord of Blood
48001935,// Mohg- The Omen (Sewers)
48100012,// Erdtree Avatar (Weeping Peninsula Boss)
48100020,// Erdtree Avatar (North Liurnia)
48100150,// Erdtree Avatar (Giants Mountaintops)
48100920,// Erdtree Avatar (South Liurnia)
48110040,// Putrid Erdtree Avatar
48110042,// Putrid Erdtree Avatar
48110052,// Putrid Erdtree Avatar (Snowfield)
48200920,// Omenkiller (Boss)
48200930,// Omenkiller (Boss Village)
49100026,// Magma Wyrm Makar (Boss)
49100032,// Magma Wyrm (Lava Lake Boss)
49100940,// Magma Wyrm (Gael Tunnel Boss)
49110052,// Great Wyrm Theodorix (Boss)
49500010,// Tibia Mariner (Limgrave)
49500020,// Tibia Mariner (Boss)
49500032,// Tibia Mariner (Boss)
49800010,// Death Bird (Limgrave)
49800012,// Death Bird (Weeping Peninsula)
49800020,// Death Bird (Liurnia)
49800033,// Death Bird (Altus)
49801020,// Death Rite Bird
49801040,// Death Rite Bird
49801050,// Death Rite Bird
49801052,// Death Rite Bird
71000012,// Ancient Hero of Zamor (Weeping Peninsula Boss)
71000030,// Ancient Hero of Zamor (Boss)
71000950,// Ancient Hero of Zamor (Boss)
523040050,// Roundtable Knight Vyke
523240070,// Sir Gideon Ofnir, the All-Knowing
523250066,// Sorcerer Rogier
523290066,// Lionel the Lionhearted
523560020,// Adan, Thief of Fire
523610066,// Fia's Champion 1
523760930,// Necromancer Garris
523860000,// Esgar, Priest of Blood
526100965,// Mimic Tear

//Exceptions not natrually included

526100052,  //Stray Mimic Tear (Hidden Path to Haligtree) (rare exception that isnt technically considered boss)
49100038,   //Magma Wyrm (Volcano Manor) (rare exception that isnt technically considered boss) //since its not considered a boss, and drops a dragon heart (like many other enemies) im not going to make it map based drop.
            }.ToList();
            if(BossOrMiniBossIds.Count == 0){   //only runs if list is empty, its not but if we want to upfdate the list, we leave it empty.
                
                Condition isBossExceptionCond;          //unused
                {


                    int[] BossExceptionIDs = new int[]  
                    //list of ids of characters that technically pass our BossConditions and Appropriate Group Conditions but we dont want to be Bosses.
                    {

46500060,// Dragonkin Soldier (Ice Lightning) (Nokstella)
46500160,// Dragonkin Soldier (Ice Lightning)
43113906,// Soldier of Godrick (Boss)   //design choice. should not drop anything PERIOD.

30106051,// Banished Knight (Niall Boss)
30107051,// Banished Knight (Niall Boss)
30008040,// Exile Soldier (Boss)
30208040,// Large Exile Soldier (Boss)
46209152,// Astel- Stars of Darkness (Snowfield Mine Clone)
44910933,// Living Jar
44810912,// Miranda Sprout (Boss)
30800912,// Imp (Boss)
41403930,// Snail (Sage's Cave)
41002910,// Demihuman (Coastal Cave)
41000910,// Demihuman (Coastal Cave)

31600010,// Funeral Steed (Limgrave)
31600012,// Funeral Steed (Weeping Peninsula)
31600020,// Funeral Steed
31600030,// Funeral Steed
31600042,// Funeral Steed
31600040,// Funeral Steed
31600050,// Funeral Steed
31600052,// Funeral Steed

25006020,// Crucible Knight (Boss) (spawned by snail)
25006120,// Crucible Knight (Boss) (spawned by snail)
25007020,// Crucible Knight (Boss) (spawned by snail)
25007120,// Crucible Knight (Boss) (spawned by snail)

35100920,// Catacombs Skeleton (Scimitar) (Boss)    //Black Knife Catacombs
35101920,// Catacombs Skeleton (Grossmesser) (Boss)
35102920,// Catacombs Skeleton (Bow) (Boss)

41650935,// Bloodbane Stray (Boss)

                    };
                    isBossExceptionCond = new Condition.FieldEqualTo(0, Util.ToStrings(BossExceptionIDs));
                }
                //get group after npc id. create an exception that allows boses to sill be allowed if ni group/
                var isBossCondition = new Condition.TRUE().IsFalse;
                {
                    var BossSoulDropCond = new Condition.FieldEqualTo(NpcParam.GetFieldIndex("isSoulGetByBoss"), "1");
                    var BossSpEffectCond = new Condition.FieldEqualTo(NpcParam.GetFieldIndex("spEffectID31"), "4301");
                    var scaledCond = new Condition.HasInName("(Unscaled)").IsFalse;
                    isBossCondition = new Condition.AllOf(
                        new Condition.Either(
                            BossSoulDropCond
                            , BossSpEffectCond
                        ),
                        scaledCond,
                        isBossExceptionCond.IsFalse
                    );
                }
                Line testline = NpcParam.GetLineWithId(21000830);
                //Util.println(1 + " " + isBossCondition.Pass(testline).ToString());

                Condition isDocumentedIDCond;
                Condition isGroupAppropriateCond = new Condition.TRUE();
                const bool EXCLUDE_IN_GROUP = false;
                using (var sr = new StreamReader(@"C:\CODING\Souls Modding\ModdingTools\Docs\NPCLocations.txt"))
                {
                    List<int> isDocumentedIDs = new List<int>();
                    int[] ignoreIsInGroupIDs = new int[]
                    {
                        //watchdog with imps protected
                        //any bos with guards in the boss fight.

                        //or weird exceptions
                        31810020,// Red Wolf of Radagon
                        30500051,// Commander Niall
                        42600512,// Erdtree Burial Watchdog (Impalers Catacombs)
                        42900010,// Bloodhound Knight Darriwil
                        44800912,// Miranda Blossom
                        45800030,// Wormface (Boss)
                        41402920,// Spiritcaller Snail
                        41402950,// Spiritcaller Snail
                        46500060,// Dragonkin Soldier (Ice Lightning) (Nokstella)
                        46500160,// Dragonkin Soldier (Ice Lightning)
                        46500960,// Dragonkin Soldier of Nokstella Healthbar (Boss)
                        523760930,// Necromancer Garris
                        523860000,// Esgar, Priest of Blood
                        21101072,//Malitketh (Farum Azula)

                    };
                    Dictionary<int, List<int>> idToGroups = new Dictionary<int, List<int>>();
                    List<int> inAGroupIDs = new List<int>();
                    string line;
                    string npcRegexPattern = @"npc (\d+)";
                    string groupRegexPattern = @"group (\d+(\,\d+)?)";
                    //Util.println(1);
                    while ((line = sr.ReadLine()) != null)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(line, npcRegexPattern);
                        if (!match.Success)
                        {
                            //Util.println("line:\n" + line + "\nfailed to find npc id.");
                            continue;
                        }
                        var npcID = int.Parse(match.Groups[1].Value);
                        if (!isDocumentedIDs.Contains(npcID))
                            isDocumentedIDs.Add(npcID);
                        if (!EXCLUDE_IN_GROUP)
                            continue;

                        int groupAmount = 0;
                        int[] groups = null;
                        match = System.Text.RegularExpressions.Regex.Match(line, groupRegexPattern);
                        if (match.Success)
                        {
                            groups = Util.ToInts(match.Groups[1].Value.Split(','));
                            groupAmount = groups.Count();
                        }
                        //Util.println(1 + " " + npcID +" "+groupAmount);
                        if (groupAmount == 0)
                            continue;

                        int prevGroupAmount = 0;
                        if (idToGroups.ContainsKey(npcID))
                        {
                            prevGroupAmount = idToGroups[npcID].Count;
                            if (groupAmount < prevGroupAmount)
                                continue;
                            idToGroups[npcID] = groups.ToList();
                        }
                        else
                        {
                            idToGroups.Add(npcID, groups.ToList());
                        }
                    }
                    isDocumentedIDCond = new Condition.FieldEqualTo(0, Util.ToStrings(isDocumentedIDs.ToArray()));
                    if (EXCLUDE_IN_GROUP)
                    {
                        List<int> SharesIDsWithBosses = new List<int>();
                        List<int> ShareAGroupIDs = new List<int>();
                        //Util.println(2 + " " + isBossCondition.Pass(testline).ToString());
                        foreach (int npcId in idToGroups.Keys)
                        {
                            //if (ShareAGroupIDs.Contains(npcId))
                            //    continue;
                            Line npcLine = NpcParam.GetLineWithId(npcId);
                            bool isBoss = isBossCondition.Pass(npcLine);
                            if (!isBoss)
                                continue;
                            bool foundGroup = false;
                            var groups = idToGroups[npcId];
                            foreach (int group in groups)
                            {
                                foreach (int _npcId in idToGroups.Keys)
                                {
                                    if (npcId == _npcId)
                                        continue;
                                    bool _isBoss = isBossCondition.Pass(NpcParam.GetLineWithId(_npcId));
                                    var _groups = idToGroups[_npcId];
                                    foreach (int _group in _groups)
                                    {
                                        if (_group == group)
                                        {
                                            ShareAGroupIDs.Add(npcId);
                                            //.Add(_npcId);
                                            if (_isBoss)
                                                SharesIDsWithBosses.Add(npcId);
                                            //if (isBoss)
                                            //    SharesIDsWithBosses.Add(_npcId);
                                            //foundGroup = true;
                                            break;
                                        }
                                    }
                                    if (foundGroup)
                                        break;
                                }
                                if (foundGroup)
                                    break;
                            }
                        }
                        //Util.println(3 + " " + isBossCondition.Pass(testline).ToString());
                        //Util.println(2);
                        isGroupAppropriateCond = new Condition.FieldEqualTo(0, Util.ToStrings(ignoreIsInGroupIDs.ToArray())).OR(
                            new Condition.FieldEqualTo(0, Util.ToStrings(SharesIDsWithBosses.ToArray())).IsFalse
                            );
                    }
                }
                var isGroupAppropriateBossCondition = new Condition.AllOf(isDocumentedIDCond, isBossCondition, isGroupAppropriateCond);
                //Util.println(4.5 + " isBoss" + isBossCondition.Pass(testline).ToString() + " isDocu" + isDocumentedIDCond.Pass(testline).ToString() + " isGroup" + isGroupAppropriateCond.Pass(testline).ToString());     //WIERD THIS MAKES IT FALSE FOR SOME REASON


                //-------Might be used later. These lines may be used as refrence to make duo bosses drop stuff via the itemLotParam_map drop refrence.
                //var isNonGroupAppropriateBossCondition = new Condition.AllOf(isDocumentedIDCond,isBossCondition,isGroupAppropriateCond.IsFalse);
                //var isNonGroupAppropriateBossCondition = isDocumentedIDCond.AND(isBossCondition.AND(isGroupAppropriateCond.IsFalse));
                //Util.println(4.5 + " isBoss" + isBossCondition.Pass(testline).ToString() + " isDocu" + isDocumentedIDCond.Pass(testline).ToString() + " isGroup" + isGroupAppropriateCond.IsFalse.Pass(testline).ToString());     //WIERD THIS MAKES IT FALSE FOR SOME REASON


                //Util.println(3);
                //((Lines)NpcParam.GetLinesOnCondition(isInAGroupCondition)).PrintIDAndNames();
                //return;

                var lines =(Lines)NpcParam.GetLinesOnCondition(isGroupAppropriateBossCondition);
                //-------print lines debug.
                //lines.PrintIDAndNames();


                //-------comparison debug.
                //var lines2 = (Lines)NpcParam.GetLinesOnCondition(isNonGroupAppropriateBossCondition);
                // LineFunctions.CompareLines(lines, lines2);


                //-------boss condition weirdness check. conclusion: there is weirdness with making falses inside an AND for some reason. idc mocing on.
                /*var isBossCondition1 = scaledCond.AND(
                    new Condition.Either(
                        BossSoulDropCond
                        , BossSpEffectCond
                    ));
                foreach (Line l in NpcParam.lines)
                {
                    if (l == testline)
                    {
                        Util.println("GO HERE");
                        Util.println(5.5 + " " + isBossCondition.Pass(testline).ToString());
                    }
                    Util.println( l._idName +" isGroupAppropriate" +
                                isGroupAppropriateCond.Pass(l).ToString() + " & isDocu" +
                                isDocumentedIDCond.Pass(l).ToString() + " & "+
                                                                    "isBoss" +
                                isBossCondition.Pass(l).ToString() + ">> ((bossSoul" +
                                BossSoulDropCond.Pass(l).ToString() + " || bossSp" +
                                BossSpEffectCond.Pass(l).ToString() + ") & scaled" +
                                scaledCond.Pass(l).ToString() + ") " +
                                                                      "isBossNEW"+
                                isBossCondition1.Pass(l).ToString()
                                );
                }*/

                
                
                BossOrMiniBossIds = lines.GetIntFields(0).ToList();

                //-------if you want to print a bunch of lineIDs to out them into an Array..
                LineFunctions.PrintLineIDsAsToPutInArray(lines);
                Util.println("FOR EFFICIENCY copy and paste the following ids into the array which assigns to BossOrMiniBossIds ." +
                    " Then rerun the program. You can also comment out the return after this debug if you dont care about ineffeicency.");
                return;

            }

            var BossOrMiniBossToItemLotMapDict = new Dictionary<int, int>();
            var BossOrMiniBossToItemLotMapDict2 = new Dictionary<int, int>();

            //Preassign bosses to itemlotmap
            {

                BossOrMiniBossToItemLotMapDict.Add(21300534, 10040);    //Morgott
                BossOrMiniBossToItemLotMapDict.Add(47210070, 10070);    //Hoarah Loux
                BossOrMiniBossToItemLotMapDict.Add(21403050, 10740);    //Fell Twins
                BossOrMiniBossToItemLotMapDict.Add(21403150, 10740);    //Fell Twins
                BossOrMiniBossToItemLotMapDict.Add(71000030, 20080);    //Ancient Hero of Zamor (Sainted Hero's Grave)

                BossOrMiniBossToItemLotMapDict.Add(31500010, 1043370400);    //Night's Cavalry (Repeating Thrust)
                BossOrMiniBossToItemLotMapDict.Add(31500042, 1052410100);    //Night's Cavalry (BHS Drop)
                BossOrMiniBossToItemLotMapDict.Add(31501040, 1049370100);    //Night's Cavalry (Poison Moth Flight Drop)
                BossOrMiniBossToItemLotMapDict.Add(31500052, 1048550710);    //Night's Cavalry (Night's Cavalry Set + Ancient  Drop) Duo
                BossOrMiniBossToItemLotMapDict.Add(31501052, 1048550710);    //Night's Cavalry (Night's Cavalry Set + Ancient  Drop) Duo
                BossOrMiniBossToItemLotMapDict.Add(31500050, 1048510700);    //Night's Cavalry (Phantom Slash Drop)
                BossOrMiniBossToItemLotMapDict.Add(31500020, 1036480400);    //Night's Cavalry (Giant's Hunt + Glaive Drop) //shares id
                BossOrMiniBossToItemLotMapDict2.Add(31500020, 1039430400);    //Night's Cavalry (Ice Spear Drop)    //shares id.
                BossOrMiniBossToItemLotMapDict.Add(31501030, 30335);    //Night's Cavalry (Shared Order Drop)
                BossOrMiniBossToItemLotMapDict.Add(31501012, 30335);    //Night's Cavalry (Barricade Shield + Flail Drop)


                BossOrMiniBossToItemLotMapDict.Add(25000010, 30120);    //crucible Knight (Stormhill) (Tail spell Drop)
                BossOrMiniBossToItemLotMapDict.Add(25000014, 10001295);    //crucible Knight (Stormveil Castle) (Horn spell Drop)
                BossOrMiniBossToItemLotMapDict.Add(25000165, 12020435);    //crucible Knight (Siofra Aquaduct) (Horn Shield Drop)
                BossOrMiniBossToItemLotMapDict.Add(25009138, 16000950);    //tanith's crucible Knight (Volcano Manor) (Horn Shield Drop)

                BossOrMiniBossToItemLotMapDict.Add(41300012, 1043340400);    //Demi-human Queen (Weeping peninsula) (Demi-human queen staff + Crystal Burst drop)
                BossOrMiniBossToItemLotMapDict.Add(41301030, 1038510050);    //Demi-human Queen Gilika (Lux Ruins) (Ritual Sword Talisman staff drop)
                BossOrMiniBossToItemLotMapDict.Add(41301932, 20400);    //Demi-human Queen Margot (Volcano Cave) (Jar Canon staff drop) // could be done auto
                BossOrMiniBossToItemLotMapDict.Add(41300032, 30395);    //Demi-human Queen Maggie (Sorcerer Azur) (Memory Stone drop) // could be done auto
                //btw Gilika drops nothing if your wondering.


                BossOrMiniBossToItemLotMapDict.Add(42700014, 10001085);    //Elder Lion (Stormveil Castle) (SSS+Beastblood+OldFang drop)
                BossOrMiniBossToItemLotMapDict.Add(42700040, 1047380700);    //Elder Lion (Fort Gael) (Lion's Claw AOW drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict2.Add(42700040, 1049370700);    //Elder Lion (Caelid) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict.Add(42700041, 1051360700);    //Elder Lion (Redmane Castle) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict2.Add(42700041, 1051360800);    //Elder Lion (Redmane Castle) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict.Add(42700034, 11000195);    //Elder Lion (Lleyndel) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict.Add(42700030, 11000185);    //Elder Lion (Lleyndel Outskirts) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                BossOrMiniBossToItemLotMapDict.Add(42701051, 1051570800);    //Elder Lion (Castle Sol) (SSS+Beastblood+OldFang drop)
                BossOrMiniBossToItemLotMapDict2.Add(42701051, 1051570810);    //Elder Lion (Castle Sol) (SSS+Beastblood+OldFang drop)
                //wiki has no information on other lion drops, assuming no drops for them.

                BossOrMiniBossToItemLotMapDict.Add(32500072, 13002095);    //Draconic Tree Sentinel (Farum Azula) (Malformed Dragon Set Drop)
                BossOrMiniBossToItemLotMapDict.Add(32500033, 30315);    //Draconic Tree Sentinel (Lleyndel Entrence) (Dragonclaw Shield + Dragon Greatclaw Drop)

                
                BossOrMiniBossToItemLotMapDict.Add(49100032, 16002000);    //Magma Wyrm (Volcano Manor) (Dragon Heart)
                BossOrMiniBossToItemLotMapDict.Add(49100038, 30390);    //Magma Wyrm (Lava Lake Boss) (Dragon Heart) //no sure which is the "real" ItemLotMap lot so ill set it to both.
                BossOrMiniBossToItemLotMapDict2.Add(49100038, 30400);    //Magma Wyrm (Lava Lake Boss) (Dragon Heart) //no sure which is the "real" ItemLotMap lot so ill set it to both.


                BossOrMiniBossToItemLotMapDict.Add(32510030, 1044320410);    // Tree Sentinel (Altus Plateua) (Erdtree Shield Drop)
                BossOrMiniBossToItemLotMapDict.Add(32511030, 1044320410);    // Tree Sentinel - Torch (Altus Plateua) (Erdtree Shield Srop)

                BossOrMiniBossToItemLotMapDict.Add(33001940, 1049390800);   //Nox Monk (Boss) (Nox Flowing Sword Drop)
                BossOrMiniBossToItemLotMapDict.Add(33000940, 1049390800);   //Nox Swordstress (Boss) (Nox Flowing Sword Drop)

                BossOrMiniBossToItemLotMapDict.Add(31000010, 1042380410);    //Bell Bearing Hunter (Limgrave) Bone Peddelers Bell Bearing
                BossOrMiniBossToItemLotMapDict.Add(31000020, 1037460400);    //Bell Bearing Hunter (Church of Vows) Meat Peddlers Bell Bearing
                BossOrMiniBossToItemLotMapDict.Add(31000033, 1043530400);    //Bell Bearing Hunter (Hermit's Shack) Medicine Peddlers Bell Bearing
                BossOrMiniBossToItemLotMapDict.Add(31000042, 1048410800);    //Bell Bearing Hunter (Hermit's Shack) Medicine Peddlers Bell Bearing


                BossOrMiniBossToItemLotMapDict.Add(71000012, 1042330100);    //Ancient Hero of Zamor (Weeping Peninsula Boss) Radagon's Scarseal

                BossOrMiniBossToItemLotMapDict.Add(49801052, 1048570700);    //Death Rite Bird (Ordina, Liturgical Town) Explosive Ghostflame
                BossOrMiniBossToItemLotMapDict.Add(49801040, 1049370110);    //Death Rite Bird (Southern Aeonia Swamp Bank) Death's Poker
                BossOrMiniBossToItemLotMapDict.Add(49801020, 1036450400);    //Death Rite Bird (Liurnia of the Lakes - Gate Town Northwest) Ancient Death Rancor
                BossOrMiniBossToItemLotMapDict.Add(49800020, 1037420400);    //Death Bird (Liurnia of the Lakes - Scenic Isle, Laskyar Ruins) Red-Feathered Branchsword
                BossOrMiniBossToItemLotMapDict.Add(49800033, 1044530300);    //Death Bird (Capital Outskirts - Minor Erdtree) Twinbird Kiteshield
                BossOrMiniBossToItemLotMapDict.Add(49800012, 1044320400);    //Death Bird (Weeping Peninsula - Castle Morne Approach North) Sacrificial Axe
                BossOrMiniBossToItemLotMapDict.Add(49800010, 1042380400);    //Death Bird (Stormhill - Warmaster's Shack, Stormgate) Blue-Feathered Branchsword

                BossOrMiniBossToItemLotMapDict.Add(48100150, 30525);    //Erdtree Avatar (Cerulean Crystal Tear Drop)

                BossOrMiniBossToItemLotMapDict.Add(47700033, 1042510900);    //Gargoyle (Lleyndel Outskirts) (Gargoyle's Great Axe Drop)
                BossOrMiniBossToItemLotMapDict.Add(47700250, 30505);    //Gargoyle (Rold Lift) (G's Black Blades + G's Black Axe Drop)
                BossOrMiniBossToItemLotMapDict.Add(47701242, 30425);    //Gargoyle (Bestial Sanctum) (G's Blackblade + G's Black Halberd Drop)
                BossOrMiniBossToItemLotMapDict.Add(47702034, 11001187);    //Gargoyle (Lleyndel) (Gargoyle's Halberd Drop)
                //BossOrMiniBossToItemLotMapDict.Add(47700070, );    //Gargoyle (Lleyndel Ashen Capital) // we accually want this one to drop since it respawns
                BossOrMiniBossToItemLotMapDict.Add(47701165, 10100);    //Valiant Gargoyle Duo (Siofra River) (Gargoyle's Great Axe Drop)
                BossOrMiniBossToItemLotMapDict.Add(47700165, 10100);    //Valiant Gargoyle Duo (Siofra River) (Gargoyle's Great Axe Drop)



                //47701034 documented in lleyndel, but only ones compared to 47701034 which is documented 3 times in lleyndel. im assuming is unused due to it having a weirld assigned itemLot_enemy unlike 47701034
                //47700034 is tagged fore lleyndel, undocumented
                //47702066 has item lot, undocumented


                BossOrMiniBossToItemLotMapDict.Add(31811032, 20090);    //Red Wolf of the Champion (Floh Drop)
                BossOrMiniBossToItemLotMapDict.Add(25001066, 12030950);    //Crucible Knight Siluria (Siluria Tree Drop)

                BossOrMiniBossToItemLotMapDict.Add(44800912, 20300);    //Miranda the Blighted Bloom
                BossOrMiniBossToItemLotMapDict.Add(48200930, 20410);    //Omenkiller (Boss Village) - duo fight
                BossOrMiniBossToItemLotMapDict.Add(44800930, 20410);    //Miranda the Blighted Bloom - duo fight

                BossOrMiniBossToItemLotMapDict.Add(523290066, 10350);    //Lionel the Lionhearted - Fia's Champions (Fia's Mist)

                BossOrMiniBossToItemLotMapDict.Add(47500030, 1039500100);    //Godefroy the Grafted (Godfrey Icon Drop)
                BossOrMiniBossToItemLotMapDict.Add(47200134, 101100);    //Godfrey, First Elden Lord (Phantom) (Talisman Pouch Drop)

                BossOrMiniBossToItemLotMapDict.Add(35600042, 34110400);    //Godskin Apostle (Divine Tower of Caelid) (Godskin Apostle Set Drop)
                BossOrMiniBossToItemLotMapDict.Add(37040940, 1049390850);  //Battlemage Hughes (Battlemage Hughes Ashes Drop)(Evergoal) (weirdly, the name and doc location points to diffrent things, just trust)
                BossOrMiniBossToItemLotMapDict.Add(40200920, 1034480100);  //Royal Revenant (Frozen Needle Drop)


                BossOrMiniBossToItemLotMapDict.Add(35500930, 1040530010);  //Sanguine Noble (Bloody Helice Drop)

            }

            using (var sr = new StreamReader(@"C:\CODING\Souls Modding\ModdingTools\Docs\NPCLocations.txt"))
            {
                var ALLordered = new Dictionary<int, Dictionary<int, List<Line>>>();
                var ItemLotMapToBossOrMiniBoss = new Dictionary<int, int>();
                var ItemLotMapToHighestMatchScoreDict = new Dictionary<int, int>();
                var ItemLotMapToCurIndexDict = new Dictionary<int, int>();
                var ItemLotMapToAssignedDataDict = new Dictionary<int, string>();
                var BossOrMiniBossToDataDict = new Dictionary<int, string>();

                foreach (var key in BossOrMiniBossToItemLotMapDict.Keys)
                {
                    int mapId = BossOrMiniBossToItemLotMapDict[key];
                    if (ItemLotMapToBossOrMiniBoss.ContainsKey(mapId))
                        continue;
                    ItemLotMapToBossOrMiniBoss.Add(mapId, key);
                    ItemLotMapToCurIndexDict.Add(mapId, -100);
                    ItemLotMapToHighestMatchScoreDict.Add(mapId, int.MaxValue);
                    
                }
                //get all boss _map param lots related map param lots.
                var BossItemLotMapLines = ItemLotParam_map.GetLinesOnCondition(
                    //	ids 10000 - 30600
                    new Condition.FloatBetween(new FloatFieldRef(0), 10000, 40000, true)
                    // ids end with 0 (start of the lot)
                    .AND(new Condition.Either(
                        new Condition.FieldEndsWith(0, "0"),
                        new Condition.FieldEndsWith(0, "5")
                        ))
                    .AND(new Condition.IDCheck(10050).IsFalse) //dont include* 10050;[Enia - 2 Great Runes] Talisman Pouch
                    );



                string line;
                var vanillaLinesList = new List<Line>();
                Dictionary<string[], float> difficultyOfLocationDict = new Dictionary<string[], float>();
                List<string[]> allLocationGroups = new List<string[]>();

                {
                    string[] limgraveLocations =
                         "Stormveil Castle Entrance\nLimgrave\nBridge of Sacrifice\nChapel of Anticipation\nChurch of Dragon Communion\nChurch of Elleh\nCoastal Cave\nDeathtouched Catacombs\nDivine Tower of Limgrave\nDragon-Burnt Ruins\nForlorn Hound Evergaol\nFort Haight\nFringefolk Hero's Grave\nGatefront Ruins\nGroveside Cave\nHighroad Cave\nLimgrave Colosseum\nLimgrave Tunnels\nMistwood\nMistwood Ruins\nMurkwater Catacombs\nMurkwater Cave\nSiofra River Well\nStarfall Crater\nStormfoot Catacombs\nStormgate\nStormhill\nStormhill Evergaol\nStormhill Shack\nStranded Graveyard\nSummonwater Village\nThird Church of Marika\nWarmaster's Shack"
                         .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(limgraveLocations);
                    difficultyOfLocationDict.Add(limgraveLocations, 1);

                    string[] stormveilCastleLocations =
                    {
                    "Stormveil Castle",
                    };
                    allLocationGroups.Add(stormveilCastleLocations);
                    difficultyOfLocationDict.Add(stormveilCastleLocations, 2);

                    string[] castleMorneLocations =
                    {
                    "Castle Morne\nWeeping Peninsula - Castle Morne", //longer on purpose
                    };
                    allLocationGroups.Add(castleMorneLocations);
                    difficultyOfLocationDict.Add(castleMorneLocations, 1.5f);


                    string[] weepingPenninsulaLocation =
                        "Weeping Peninsula\nAiling Village\nBridge of Sacrifice\nCallu Baptismal Church\nChurch of Pilgrimage\nDemi-Human Forest Ruins\nEarthbore Cave\nForest Lookout Tower\nFourth Church of Marika\nImpaler's Catacombs\nMinor Erdtree(Weeping Peninsula)\nMorne Tunnel\nOridys's Rise\nTombsward Catacombs\nTombsward Cave\nTombsward Ruins\nTower of Return\nWeeping Evergaol\nWitchbane Ruins"
                          .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(weepingPenninsulaLocation);
                    difficultyOfLocationDict.Add(weepingPenninsulaLocation, 1.3f);

                    string[] luriniaOfTheLakesLocations =
                    "(Liur\nSouthwest Liurnia\nWest Liurnia\nEast Liurnia\nAcademy Crystal Cave\nAcademy Gate Town\nAinsel River Well\nArtist's Shack (Liurnia)\nBlack Knife Catacombs\nBoil Prawn Shack\nCarian Study Hall\nChurch of Irith\nChurch of Vows\nCliffbottom Catacombs\nConverted Fringe Tower\nConverted Tower\nCuckoo's Evergaol\nDeep Ainsel Well\nDivine Tower of Liurnia\nJarburg\nKingsrealm Ruins\nLakeside Crystal Cave\nLaskyar Ruins\nLunar Estate Ruins\nMalefactor's Evergaol\nMinor Erdtree(Liurnia Northeast)\nMinor Erdtree(Liurnia Southwest)\nPurified Ruins\nAcademy of\nLucaria Crystal Tunnel\nRevenger's Shack\nRoad's End Catacombs\nRose Church\nSlumbering Wolf's Shack\nStillwater Cave\nTemple Quarter\nTestu's Rise\nThe Four Belfries\nVillage of the Albinaurics\nStormveil Castle Exit\nWest Albinauric Village"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(luriniaOfTheLakesLocations);
                    difficultyOfLocationDict.Add(luriniaOfTheLakesLocations, 3);

                    string[] cariaManorLocations =
                        "Caria Manor\nBehind Caria Manor\nRanni's Rise\nRenna's Risen"
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(cariaManorLocations);
                    difficultyOfLocationDict.Add(cariaManorLocations, 4);

                    string[] extralevel4Locations =
                    "Liurnia to Altus Plateau\nFrenzied Flame Village\nFrenzy-Flaming Tower\nSouthwest of Grand Lift of Dectus\nBellum Church\nBellum Highway\nRuin-Strewn Precipice\nChurch of Inhibition"
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(extralevel4Locations);
                    difficultyOfLocationDict.Add(extralevel4Locations, 4);

                    string[] moonlightAltarLocations =
                    "Moonlight Altar\nWest of Deep Ainsel Well\nMoonlight Altar - Ringleader's Evergaol\nLunar Estate Ruins"
                    .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(moonlightAltarLocations);
                    difficultyOfLocationDict.Add(moonlightAltarLocations, 7.2f);


                    /*string[] caelidLocations =
                    "Caelid\nCaelem Ruins\nCaelid Catacombs\nCaelid Colosseum\nCaelid Waypoint Ruins\nCathedral of Dragon Communion\nChurch of the Plague\nDivine Tower of Caelid\nForsaken Ruins\nFort Gael\nGael Tunnel\nGaol Cave\nGowry's Shack\nMinor Erdtree (Caelid)\nMinor Erdtree Catacombs\nSellia Crystal Tunnel\nSellia Gateway\nSellia, Town of Sorcery\nShack of the Rotting\nSmoldering Church\nStreet of Sages Ruins\nSwamp Lookout Tower\nSwamp of Aeonia\nWailing Dunes\nWar-Dead Catacombs"

                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(caelidLocations);
                    difficultyOfLocationDict.Add(caelidLocations, 4);*/

                    string[] caelidLvl5Locations =
                    "Redmane Castle"
                     .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(caelidLvl5Locations);
                    difficultyOfLocationDict.Add(caelidLvl5Locations, 5);

                    string[] dragonBarrowLocations =
                        //"Greyoll's Dragonbarrow\nAbandoned Cave\nBestial Sanctum\nDragonbarrow Cave\nDivine Tower of Caelid\nDeep Siofra Well\nFort Faroth\nLenne's Rise\nMinor Erdtree (Dragonbarrow)\nSellia Evergaol\nSellia Hideaway"
                        "Fort Faroth"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(dragonBarrowLocations);
                    difficultyOfLocationDict.Add(dragonBarrowLocations, 7f);



                    string[] altusLocations =
                        "Altus Plateau\nAltus Tunnel\nDominula, Windmill Village\nEast Windmill Pasture\nAltus Plateau - East Windmill Village\nGolden Lineage Evergaol\nGrand Lift of Dectus\nLux Ruins\nMinor Erdtree (Altus Plateau)\nMirage Rise\nOld Altus Tunnel\nPerfumer's Grotto\nPerfumer's Ruins\nSage's Cave\nSainted Hero's Grave\nSecond Church of Marika\nStormcaller Church\nThe Shaded Castle\nUnsightly Catacombs\nVillage Windmill Pasture\nWest Windmill Pasture\nWoodfolk Ruins\nWritheblood Ruins\nWyndham Catacombs\nWyndham Ruins\nRoad of Iniquity Side Path\nAltus Plateau - Altus Plateau"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(altusLocations);
                    difficultyOfLocationDict.Add(altusLocations, 4.7f);

                    string[] mountGelmirLocations =
                        "Gelmir\nCorpse - Stench Shack\nSeethewater Cave\nCraftsman's Shack\nFort Laiedd\nGelmir Hero's Grave\nHermit Village\nHermit's Shack\nMinor Erdtree (Mt. Gelmir)\nRoad of Iniquity\nVolcano Manor Entrance\nVolcano Cave Entrance\nAltus Plateau - West of Shaded Castle\nMt. Gelmir - Primeval Sorcerer Azur\nAbductor Virgins Exit"
                          .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(mountGelmirLocations);
                    difficultyOfLocationDict.Add(mountGelmirLocations, 4); //changed from 6

                    string[] mountGelmirLvl5 =
                        "Volcano Manor\nVolcano Cave"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(mountGelmirLvl5);
                    difficultyOfLocationDict.Add(mountGelmirLvl5, 6);

                    string[] CapitalOutskirtsLocations =    //5
                        "Capital Outskirts\nAuriza Hero's Grave\nAuriza Side Tomb\nDivine Tower of East Altus\nDivine Tower of West Altus\nHermit Merchant's Shack\nMinor Erdtree (Capital Outskirts)\nMinor Erdtree Church\nSealed Tunnel"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(CapitalOutskirtsLocations);
                    difficultyOfLocationDict.Add(CapitalOutskirtsLocations, 5);

                    string[] UpperLleyndelLocations =
                        "Royal Colosseum\nLeyndell, Royal Capital\nIsolated Divine Tower"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(UpperLleyndelLocations);
                    difficultyOfLocationDict.Add(UpperLleyndelLocations, 6);

                    string[] LowerLleyndelLocations =
                        "Subterranean Shunning-Grounds\nLeyndell Catacombs"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(LowerLleyndelLocations);
                    difficultyOfLocationDict.Add(LowerLleyndelLocations, 7);

                    string[] deeprootDepthsLocations = {
                    "Deeproot Depths"
                    };
                    allLocationGroups.Add(deeprootDepthsLocations);
                    difficultyOfLocationDict.Add(deeprootDepthsLocations, 6f);

                    /*string[] siofraRiverLocations = {
                    "Siofra River",
                    "Nokron",
                    };
                    allLocationGroups.Add(siofraRiverLocations);
                    difficultyOfLocationDict.Add(siofraRiverLocations, 3);

                    string[] AinselRiverLocations = {
                    "Ainsel River"
                    };
                    allLocationGroups.Add(AinselRiverLocations);
                    difficultyOfLocationDict.Add(AinselRiverLocations, 3);*/


                    string[] MoghwynPalaceLocations = {
                    "Mohgwyn Palace"
                    };
                    allLocationGroups.Add(MoghwynPalaceLocations);
                    difficultyOfLocationDict.Add(MoghwynPalaceLocations, 8);

                    string[] mountainTopsLocations =    //7
                        "Mountaintops\nCastle Sol\nChurch of Repose\nFirst Church of Marika\nFlame Peak\nForbidden Lands\nForge of the Giants\nGrand Lift of Rold\nGiant-Conquering Hero's Grave\nGiants' Mountaintop Catacombs\nGuardians' Garrison\nHeretical Rise\nLord Contender's Evergaol\nMinor Erdtree (Mountaintops East)\nShack of the Lofty\nSpiritcaller Cave\nStargazers' Ruins\nZamor Ruins"
                          .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(mountainTopsLocations);
                    difficultyOfLocationDict.Add(mountainTopsLocations, 7);

                    string[] farumAzulaLocations = //8
                        "Crumbling Farum Azula"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(farumAzulaLocations);
                    difficultyOfLocationDict.Add(farumAzulaLocations, 8);

                    string[] pathToHaligTreeLocations =
                        "Hidden Path to the Haligtree"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(pathToHaligTreeLocations);
                    difficultyOfLocationDict.Add(pathToHaligTreeLocations, 7);

                    string[] concentratedSnowfieldsLocations =
                        "West of Castle Sol\nOrdina\nConsecrated Snowfield\nAlbinauric Rise\nApostate Derelict\nCave of the Forlorn\nConsecrated Snowfield Catacombs\nMinor Erdtree (Consecrated Snowfield)\nOrdina, Liturgical Town\nYelough Anix Ruins\nYelough Anix Tunnel"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(concentratedSnowfieldsLocations);
                    difficultyOfLocationDict.Add(concentratedSnowfieldsLocations, 7.3f);

                    string[] haligtreeLocations =
                        "Miquella's Haligtree"
                         .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(haligtreeLocations);
                    difficultyOfLocationDict.Add(haligtreeLocations, 7.85f);


                    string[] AshenCapitalLocations = {
                    "Ashen Capital"
                    };
                    allLocationGroups.Add(AshenCapitalLocations);
                    difficultyOfLocationDict.Add(AshenCapitalLocations, 8);
                }   //all locations setting

                while ((line = sr.ReadLine()) != null)
                {
                    string pattern = @"\((.*?)\)"; // Regular expression pattern to match text between parentheses
                    Match match = Regex.Match(line, pattern);
                    if (!match.Success)
                    {
                        //Util.println("line:\n" + line + "\nfailed to find location name.");
                        continue;
                    }
                    var locationName = match.Groups[1].Value;

                    match = Regex.Match(line.Remove(0,line.IndexOf(")")+1),pattern);    //gets second parrentheses that contains name.
                    if (!match.Success)
                    {
                        //Util.println("line:\n" + line + "\nfailed to find npc name.");
                        continue;
                    }
                    var npcname = match.Groups[1].Value;
                    //rename to Leyndell Margit to Morgott

                    pattern = @"npc (\d+)";
                    match = System.Text.RegularExpressions.Regex.Match(line, pattern);
                    if (!match.Success)
                    {
                        //Util.println("line:\n" + line + "\nfailed to find npc id.");
                        continue;
                    }

                    var npcID = int.Parse(match.Groups[1].Value);

                    float difficulty = -1;
                    string selectedString = "";
                    int selectedLength = 0;
                    //allLocationGroups.Reverse(); //used to debug difficulty conflicts/changes/priority.
                    foreach (string[] group in allLocationGroups)
                    {
                        foreach (string s in group)
                        {

                            if (s.Length > selectedLength && locationName.Contains(s))
                            {
                                float oldDif = difficulty;
                                string oldSelectedString = selectedString;
                                difficulty = difficultyOfLocationDict[group];
                                //break;
                                selectedString = s;
                                selectedLength = s.Length;
                                //if (oldDif != -1 && oldDif != difficulty) //debug difficulty conflicts / changes / priority.
                                //    Util.println( Util.IndentedText(NpcParam.GetLineWithId(npcID)._idName, 50) + oldDif +" -> "+difficulty+ "   "+ oldSelectedString+" -> "+ selectedString);
                            }

                        }
                    }


                    //if (difficulty == -1)
                    //continue;
                    //Util.println("no difficulty group found for: "+ locationName + "  enemy name: "+ NpcParam.GetFieldWithLineID(1,npcID.ToString()));



                    if (!npcsDocDifficultyDict.ContainsKey(npcID))
                    {
                        if (difficulty != -1)
                            npcsDocDifficultyDict.Add(npcID, difficulty);

                        //MAIN set bosses itemlotmap and data and stuff
                        if (   BossOrMiniBossIds.Contains(npcID) && !BossOrMiniBossToDataDict.ContainsKey(npcID))
                        {

                            string data = "(" + locationName + ")" + " " + "(" + npcname + ")";
                            BossOrMiniBossToDataDict.Add(npcID, data);

                            if (BossOrMiniBossToItemLotMapDict.ContainsKey(npcID))
                            {
                                if (!ItemLotMapToAssignedDataDict.ContainsKey(BossOrMiniBossToItemLotMapDict[npcID]))
                                ItemLotMapToAssignedDataDict.Add(BossOrMiniBossToItemLotMapDict[npcID], "(" + locationName + ")" + " " + "(" + npcname + ")");
                                continue;
                            }

                            var importantWords = npcname.Split(LineFunctions.wordSplitters2, StringSplitOptions.RemoveEmptyEntries);
                            string npcLineName = NpcParam.GetLineWithId(npcID).name;
                            int npcLineNameEndIndex = npcLineName.IndexOf("(");
                            string npcLineNameForImportantWords = npcLineName;
                            string npcLineNameExtraWords = "";
                            if (npcLineNameEndIndex > -1) {
                                npcLineNameForImportantWords = npcLineName.Remove(npcLineNameEndIndex);
                                npcLineNameExtraWords = npcLineName.Remove(0, npcLineNameEndIndex);
                            }

                            importantWords = importantWords.Concat(npcLineNameForImportantWords.Split(LineFunctions.wordSplitters2, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToArray();
                            string[] targetWords = (locationName +" "+ npcLineNameExtraWords).Split(LineFunctions.wordSplitters2, StringSplitOptions.RemoveEmptyEntries);
                            //targetWords = targetWords.Concat(importantWords).ToArray();
                            //if (npcID == 49100038)
                            //    Util.p();
                            string[] exclusions = new string[] { " of ", " the " };
                            var orderedLines = LineFunctions.GetOrderedWordMatchedLinesDict(targetWords, BossItemLotMapLines, out int maxMatchCount, out int curScore, 0, importantWords, false, exclusions, "[", "]"," - ");

                            Util.p(LineFunctions.Debug_LastBestlWordsTogether);
                            Util.p(LineFunctions.Debug_LastBesttargetWordsTogether);
                            Util.p(LineFunctions.Debug_LastBestintersectTogether);
                            Util.p(LineFunctions.Debug_LastBestlWordsTogether2);
                            Util.p(LineFunctions.Debug_LastBesttargetWordsTogether2);
                            Util.p(LineFunctions.Debug_LastBestintersectTogether2);

                            ALLordered.Add(npcID, orderedLines);
                            int curIndex = 0;
                            var favoriteLine = orderedLines[curScore][curIndex];
                            //if (favoriteLine == null)
                            //    Util.p();

                            string favName = favoriteLine.name;
                            int favoriteId = favoriteLine.id_int;



                            while (true)
                            {
                                //if (npcID == 49100038)
                                //   Util.p();
                                if (ItemLotMapToHighestMatchScoreDict.ContainsKey(favoriteId))
                                {
                                    int oldCurIndex = ItemLotMapToCurIndexDict[favoriteId];
                                    int oldScore = ItemLotMapToHighestMatchScoreDict[favoriteId];
                                    int oldNpcId = ItemLotMapToBossOrMiniBoss[favoriteId];
                                    string oldData = "";
                                    if(ItemLotMapToAssignedDataDict.ContainsKey(favoriteId))
                                        oldData = ItemLotMapToAssignedDataDict[favoriteId];

                                    if (curScore > ItemLotMapToHighestMatchScoreDict[favoriteId])
                                    {
                                        int prevCurScore = curScore;
                                        int prevOldScore = oldScore;
                                        int prevFavoriteId = favoriteId;
                                        int prevNPCId = npcID;

                                        ItemLotMapToCurIndexDict[favoriteId] = curIndex;
                                        ItemLotMapToBossOrMiniBoss[favoriteId] = npcID;
                                        ItemLotMapToHighestMatchScoreDict[favoriteId] = curScore;
                                        ItemLotMapToAssignedDataDict[favoriteId] =  data;

                                        curIndex = oldCurIndex;
                                        curScore = oldScore;
                                        npcID = oldNpcId;
                                        data = oldData;

                                        orderedLines = ALLordered[oldNpcId];
                                        favoriteLine = orderedLines[oldScore][oldCurIndex];
                                        favName = favoriteLine.name;
                                        bool n = favoriteLine == null;
                                        favoriteId = favoriteLine.id_int;
                                        if (favoriteId == 30620)
                                            Util.p();
                                        break;
                                        //Util.println(npcname + "\n   " + locationName + "\n   " + favoriteLine._idName + "\n   " +
                                        //    "BossOrMiniBossToItemLotMapDict.Add(" + npcID + ", " + favoriteLine.id + ");   //" + npcname + "\n");

                                        //BossOrMiniBossToItemLotMapDict.Add(npcID, favoriteLine);
                                        //if (AssignedBossItemLotMapLines.Contains(favoriteLine))
                                        //    Util.p();
                                        //else
                                    }
                                    else
                                    {
                                        //if (npcID == 49100038 && oldScore == curScore)
                                        //    Util.p();
                                        curIndex++;
                                        if (curIndex >= orderedLines[curScore].Count)
                                        {
                                            curIndex = 0;
                                            curScore--;
                                            while (curScore > -1 && (!orderedLines.ContainsKey(curScore) || orderedLines[curScore].Count == 0))
                                            {
                                                curScore--;
                                            }
                                        }
                                        if (curScore >= 0)
                                        {
                                            favoriteLine = orderedLines[curScore][curIndex];
                                            favoriteId = favoriteLine.id_int;
                                        }
                                        else
                                            break;
                                        
                                    }
                                }
                                else
                                {

                                    ItemLotMapToBossOrMiniBoss.Add(favoriteId, npcID);
                                    ItemLotMapToHighestMatchScoreDict.Add(favoriteId, curScore);
                                    ItemLotMapToCurIndexDict.Add(favoriteId, curIndex);
                                    ItemLotMapToAssignedDataDict.Add(favoriteId,data);

                                    //if (npcID == 49100038)
                                    //    Util.p();
                                    break;
                                }
                            }
                        }
                       
                    }
                    else if (difficulty != -1 && npcsDocDifficultyDict[npcID]!= difficulty)
                    {
                        float oldDifficulty = npcsDocDifficultyDict[npcID];
                        //Util.println(NpcParam.GetFieldWithLineID(1, npcID.ToString()) + " difficulty conflict: " + oldDifficulty + " and " + difficulty);
                        
                        //prioritises the lower difficulty to avoid  enemies in the same region dropping the same shard.
                        npcsDocDifficultyDict[npcID] = Math.Min(difficulty, npcsDocDifficultyDict[npcID]);
                    }

                }

                /*foreach(int npcID in npcsDocDifficultyDict.Keys)
                {
                    Line npcLine = NpcParam.GetLineWithId(npcID);
                    float difficulty = npcsDocDifficultyDict[npcID];
                    string keywordCode = "SS 60 ##4.5";
                    string searchName = "Leyndell Knight";
                    string overrideExtension = "(Altus Override)";
                    if (difficulty == 4.7f && npcLine.name.Contains(searchName) && !npcLine.name.Contains("Horse"))
                        Util.println("keywordOverrideIDsDict.Add("+ npcID + ", \""+keywordCode+" "+searchName+" "+overrideExtension+"\");");
                }*/
                const bool debugAssignMapLots = false;
                //added new assigned itemlotmaps to main dict
                foreach (var lineKey in ItemLotMapToBossOrMiniBoss.Keys)   
                {
                    int npcId = ItemLotMapToBossOrMiniBoss[lineKey];
                    if (BossOrMiniBossToItemLotMapDict.ContainsKey(npcId))
                        continue;
                    BossOrMiniBossToItemLotMapDict.Add(npcId, lineKey);
                }

                //debug: rate bosses itemlotmaps in main dict
                if (debugAssignMapLots)
                {
                    foreach (var key in BossOrMiniBossToItemLotMapDict.Keys)
                    {
                        int lotId = BossOrMiniBossToItemLotMapDict[key];
                        string data = "";
                        if (ItemLotMapToAssignedDataDict.ContainsKey(lotId))
                            data = ItemLotMapToAssignedDataDict[lotId];
                        int score = ItemLotMapToHighestMatchScoreDict[lotId];
                        string scoreRating = "HIGH";
                        if (score < 30)
                            scoreRating = "MID";
                        if (score < 20)
                            scoreRating = "LOW";
                        if (score < 11)
                            scoreRating = "VERY LOW";
                        var npcLine = NpcParam.GetLineWithId(key);
                        var lotLine = ItemLotParam_map.GetLineWithId(lotId);
                        string npcName = npcLine._idName;
                        string lotName = lotLine._idName;
                        string addLine = "BossOrMiniBossToItemLotMapDict.Add(" + npcLine.id + ", " + lotLine.id + ");    //" + npcLine.name;
                        Util.println(npcLine.id + " - " + lotLine.id + "  score: " + score + "  " + scoreRating + "\n" + addLine + "\n" + npcLine.name + "\n   " + data + "\n   " + lotLine.name + "\n");
                    }
                }

                
                //add remaining unassigned bosses (duo bosses)to maindict
                foreach(var id in BossOrMiniBossIds)
                {
                    if (!BossOrMiniBossToItemLotMapDict.Keys.Contains(id))
                    {
                        var npcLine = NpcParam.GetLineWithId(id);
                        var idName = npcLine._idName;
                        var data = BossOrMiniBossToDataDict[id];
                        var location = BossOrMiniBossToDataDict[id];
                        location = location.Remove(location.IndexOf(")"));
                        if (debugAssignMapLots)
                        {
                            Util.println("DID NOT ASSIGN:   " + idName);
                            Util.println("    My Data:          " + data);
                        }

                        bool foundOther = false;
                        string foundOtherData = "";
                        int foundItemLotId = -1;
                        int curScore = 0;

                        foreach (var key in ItemLotMapToAssignedDataDict.Keys)
                        {
                            var otherData = ItemLotMapToAssignedDataDict[key];
                            var otherLocation = ItemLotMapToAssignedDataDict[key];
                            otherLocation = otherLocation.Remove(otherLocation.IndexOf(")"));

                            var otherId = ItemLotMapToBossOrMiniBoss[key];
                            if (otherId == id)
                                continue;
                            //if (otherId == 47210070)
                            //    Util.p();
                            if (location == otherLocation)
                            {
                                //if (id == 47200070)
                                //   Util.p();

                                int prevScore = curScore;
                                if (curScore < 10)
                                {
                                    curScore = 10;
                                }
                                {
                                    string first3Dig = id.ToString().Remove(3);
                                    string other_first3Dig = otherId.ToString().Remove(3);
                                    if (curScore < 20 && first3Dig == other_first3Dig)
                                    {
                                        curScore = 20;
                                    }

                                }

                                if (prevScore < curScore)
                                {
                                    foundOther = true;
                                    foundItemLotId = otherId;
                                    foundItemLotId = key;
                                    foundOtherData = otherData;
                                    if (prevScore == 0)
                                        BossOrMiniBossToItemLotMapDict.Add(id, key);
                                    else
                                        BossOrMiniBossToItemLotMapDict[id] = key;
                                }
                            }
                        }
                        if (foundOther && debugAssignMapLots)
                        {
                            Util.println("    Other Data:       " + foundOtherData);
                            Util.println("  ALLY FOUND BossOrMiniBossToItemLotMapDict.Add(" + id + ", " + foundItemLotId + ");    //" + npcLine.name);
                        }
                        if(debugAssignMapLots)
                            Util.println();
                    }
                }

                //debug: unassigned itemlotmap lines
                if (debugAssignMapLots)
                {
                    foreach (var itemlotline in BossItemLotMapLines)
                    {
                        if (!ItemLotMapToBossOrMiniBoss.Keys.Contains(itemlotline.id_int))
                        {
                            Util.println("DID NOT FIND:   " + itemlotline._idName);
                        }
                    }
                }

                //return;
            }


            var spLevelToDifficultyDict = new Dictionary<int, float>();
            {
                {
                    spLevelToDifficultyDict.Add(1, 1);
                    spLevelToDifficultyDict.Add(2, 1);
                    spLevelToDifficultyDict.Add(3, 1.3f);
                    spLevelToDifficultyDict.Add(4, 1.15f);
                    spLevelToDifficultyDict.Add(5, 2f);
                    spLevelToDifficultyDict.Add(6, 3.85f);
                    spLevelToDifficultyDict.Add(7, 2.85f);
                    spLevelToDifficultyDict.Add(8, 3f);
                    spLevelToDifficultyDict.Add(9, 3.15f);
                    spLevelToDifficultyDict.Add(10, 4f);
                    spLevelToDifficultyDict.Add(11, 5);
                    spLevelToDifficultyDict.Add(12, 6);
                    spLevelToDifficultyDict.Add(13, 6f);
                    {
                        spLevelToDifficultyDict.Add(14, 3);
                        spLevelToDifficultyDict.Add(15, 3);
                        spLevelToDifficultyDict.Add(16, 3);
                        spLevelToDifficultyDict.Add(17, 3);
                        spLevelToDifficultyDict.Add(18, 3);
                        spLevelToDifficultyDict.Add(19, 3);
                        spLevelToDifficultyDict.Add(20, 3);
                        spLevelToDifficultyDict.Add(21, 3);
                    }//unused
                    spLevelToDifficultyDict.Add(22, 6.85f);
                    spLevelToDifficultyDict.Add(23, 7.2f);
                    spLevelToDifficultyDict.Add(24, 8.15f);
                    spLevelToDifficultyDict.Add(25, 8.25f);
                    spLevelToDifficultyDict.Add(26, 8);
                    spLevelToDifficultyDict.Add(27, 7.85f);
                    spLevelToDifficultyDict.Add(28, 7.85f);
                    spLevelToDifficultyDict.Add(29, 7.85f);
                }
                int agreers = 0;
                int disagreers = 0;
                foreach (Line npcLine in NpcParam.lines)
                {
                    //if (npcsDifficultyDict2.ContainsKey(npcLine.id_int))
                    //    continue;
                    string locSpEffectID = npcLine.GetField("spEffectID3");
                    if (locSpEffectID == "0" || locSpEffectID == "-1")
                        continue;               //this seems to be npcs. we already excpetions.
                    var spLine = SpEffectParam.GetLineWithId(locSpEffectID);
                    if (!spLine.name.Contains("Area Scaling"))
                        continue;
                    bool isBoss = BossOrMiniBossIds.Contains( npcLine.id_int );
                    if (!isBoss && npcLine.GetField("getSoul") == "0")
                        continue;
                    int mySpTier = Util.GetFirstIntInString(spLine.name);
                    npcsIdToSpLevelsDict.Add(npcLine.id_int,mySpTier);
                    if (npcsDocDifficultyDict.ContainsKey(npcLine.id_int)) {
                        if(npcsDocDifficultyDict[npcLine.id_int] != spLevelToDifficultyDict[mySpTier])
                        {
                            if(Math.Abs(npcsDocDifficultyDict[npcLine.id_int] - spLevelToDifficultyDict[mySpTier]) > 1.5 && Math.Abs(npcsDocDifficultyDict[npcLine.id_int] - spLevelToDifficultyDict[mySpTier]) < 2)
                            //Util.println("LEVELS DISAGREE " + npcLine._idName + "  spLevel:" + spLevelToDifficultyDict[mySpTier] + "  docLevel:" + npcsDocDifficultyDict[npcLine.id_int]);
                            disagreers++;
                        }
                        else
                            agreers++;
                    }
                }
                //Util.println("agreers" + agreers + "   disagreeers:" + disagreers);
            }

            string[] smithingKeywords;
            string[] somberKeywords;
            string[] bothKeywords;
            string[] runeKeywords;
            string[] exceptions;
            string[] allKeywords;
            Dictionary<int, string> keywordOverrideIDsDict;

            //MAIN assign all keyword arrays
            {

                //first number is chance to drop.
                // >number is how many levels cascade, or the amount of smithing ston levels below the level that can also be dropped. 
                //>2 or >> both mean 2 level cascades. if the level was 3 it would drop 3, 2 and 1.
                //if level is one, this multiples the chance (currently the best way i think)

                // %number is level mult.           -prioritized first
                // - or +number is level modifier.  -prioritized second.
                // xnumber is how many drop.
                // xxxnumber is casscade for multiple. x3 is 3; xx3 is 3 or 2; xxx3 is 3, 2, or 1.
                // xv multiple chance mult (split). keeps overall chance of a drop the same. if "x3xxx xv0.5" then it has half the chance to drop a multiple.
                // ##number is the set level.
                // #number is the max level clamp.
                // |number is level cascade pooling mult. At 0 level 1 chance doesnt stack.
                // }number is chance mult per Level
                // {number is chance mult per LevelCasscade
                // /number is the percent split for somberstones if drops both.
                // spnumber is the decimal from 0 - 1 that represents the percent of spLevel that goes into the level (if applicable)
                // & is single line. one item type per drop.
                // $$ is Guarrentee first Drop & avoiddropingx1 (includde only x2 and above *if available)
                // $$$ is FTD drops only highest xAmount .
                // $$$$ is FTD drops only highest xAmount & highestLevel.
                // $$$$$ is FTD drops only firstType(normal -> somber) highest xAmount & notSingleLine
                // $$$$$$ is FTD drops only highest xAmount & highestLevel & notSingleLine
                // $$$$$$$ is FTD drops only forcedSomber & highestLevel
                // $$$$$$$$ is FTD drops only firstType(normal -> somber) highest xAmount & highestLevel
                // $number is to set the level adj the Guarrentee first Drop.
                // ! is Force Boss Display. the item displays on screen, rather than on corpse.
                // sss+number or sss-number adds and adjust to Somber Smithing Stones
                //Only use if all instances are non respawning. 
                //Only use is if 100% drop rate.
                //MAYBE i should also make this so first item is 100% chance.

                string x2Chance = " x2xx xvv0.3 ";
                string x3Chance = " x3xxx xv0.65 xvv0.65 ";
                string footSoldier = " -0.9 sp0.1 #4 }1.028 ";          //}0.965    //we changed them to drop more with higher levels.
                string soldier = " %0.875 sp0.12 >2 #8 }1.035 {0.65 ";        //}0.985
                string knight = " +0.15 sp0.1 #8 }1.05 & $$ ";            //}0.985 
                string banishedK = " +0.2 sp0.15 " + knight;
                string imp = " -1 #8 }1.035 %0.8 ";                   //}0.965

                string GiantCrab = " %0.5 +1.5 #13 }0.85 sp1 $0.5 ";

                smithingKeywords = new string[]{


                    "23 > " + knight + x2Chance + "Leyndell Knight (Leyndell- Royal Capital)",
                    "23 > " + knight + x2Chance + "Leyndell Knight - Archer (Leyndell- Royal Capital)",

                    "60 " + banishedK + x3Chance + " Banished Knight",
                    "60 " + banishedK + x3Chance + " Bloodhound Knight",


                    "14 > {0.72 %0.9" + x2Chance + "Large Exile Soldier",
                    "9 > {0.72 %0.9" + x2Chance + "Exile Soldier",

                    "4" + footSoldier + "sp0.5 " + x2Chance + "Radahn Foot Soldier",
                    "5.5 #4" + soldier + "sp0.5 " + x2Chance + "Radahn Soldier",
                    "45 #4" + knight + x2Chance + "Redmane Knight",

                    "45 " + knight + x2Chance + "Cuckoo Knight",

                    "4" + footSoldier + x2Chance + "Foot Soldier",
                    "5.5 " + soldier + x2Chance + "Soldier",
                    "45" + knight + x2Chance + "Knight",

                    "7" + imp + x2Chance + "Imp",
                    "1.75 >>> x1x2x3x4xxx %0.5 Vulgar Militia",

                    "45 +1 x2xx $$ ! Duelist",

                    "30 -0.33 $$ Scaly Misbegotten",
                    "2.5 > Misbegotten",

                    "10 > Page",
                    "6.5 -0.25 %0.75 Marionette",
                    "6.5 -0.25 %0.75 Avionette",
                    "10 Man-Serpent",
                    "100 $1 ! #8 Alabaster Lord",
                    "100 -1.3f x5 ! Onyx Lord",
                    "10 >>" +x2Chance+ "Azula Beastman",
                    "10 >>" +x2Chance+ " Beastman of Farum Azula (Limgrave Cave Boss)",
                    "15 >> $"+x3Chance+ "+Armored Beastman of Farum Azula",
                    "10 >> x6 ##4 Armored Beastman of Farum Azula (Boss)", //super high level for some reason
                    "10 >> x4 ##4 Azula Beastman (Boss)",                   //super high levvel for some reason

                    "25 #8 " + x2Chance + "Kaiden Sellsword",
                    "12 >> -1  x2x3x3xx $$ Demi-Human Chief",
                    "4.2 >> -1 %0.75 x1x2x3 Large Demi-Human",
                    "2.5 >> -1 %0.6 x1x1x3 Demi-Human",
                    //"100 Erdtree Burial Watchdog (Limgrave Catacombs)",
                    //"100 Erdtree Burial Watchdog (Limgrave Catacombs)",
                    //"100 +0.5 $$$$$ x1x2xx xv0.55 xvv0.55 #8 ! Elder Lion", //worse version of crucible knight drop
                    "4.5 {0.85 >>> x1x1x2x3xx Stonedigger",
                    "100 & >> x3x4x5xxx $$$$$ Stonedigger Troll",

                    "100 x5 Magma Wyrm",
                    "100 x5 Dragonkin Soldier",

                    "100 ##500 x5 Placidusax", //level 500 allows for x5 ancient drops. 
                    //"100 & +22 xxx5 $$$$ Ancient Dragon",    //the ones that need to drop already drop ancient dragon smithing stone.
                    
                

                    "0 Dragonfly",
                    "50 -0.5 > & x4xxx %0.7 sp0.5 Flying Dragon (Small)",
                    "100 & +1 x4xx $$$$ #8 sp0.15 Dragon",
                    "100 & x4xx $$$$ ##3 Glintstone Dragon Smarag",
                    "100 & x4xx $$$$ ##8 sss-0.8 Glintstone Dragon Adula",
                    "100 & x4xx $$$$ ##7 sss-0.7 Glintstone Dragon (Moonlight Plateau)",


                    "10 #4 Ancestral Follower",

                    "25 " + x3Chance + " Flame Chariot",
                    "100 > & $$$ x3x4xxx xvv0.85 Fire Prelate",

                    "50 $$$$  x3xxx Leonine Misbegotten",    
                    "50 $$$$$$$ +0.5 x3xxx Misbegotten Warrior", //forced somber - wont drop SS
                    
                    "65 > -0.8 x5x7xxx sp0.5 $$$$ Valiant Gargoyle",
                    "65 > -0.8 x5x7xxx sp0.5 $$$$ Black Blade Kindred",

                };

                somberKeywords = new string[]{

                    //"100 Godskin Noble",
                    //"100 x3 #9 -2 Godskin Apostle",

                    "35 +0.5 #8 ! Elder Lion", //somber drop just like existing ones.
                    "100 & +22 xxx5 $$$$ Lichdragon Fortissax",
                    //"1 Lightning Ball",
                    "100 x2 %0.9 #9 Night's Cavalry",

                    "7.5 -0.5f >>> Albinauric Archer",
                    "7.5 -0.5f >>> Elder Albinauric Sorcerer",
                    "6 > x1x2 -1 %0.9 #9 Giant Albinauric Crab",
                    "1.5 ##1 Albinauric Crab",
                    "3.5 Large Albinauric",
                    "2 Albinauric",

                    "100 +0.5 ! Red Wolf of the Champion",
                    "100 +0.5 ! Red Wolf of Radagon Sword",
                    "100 -0.3 ! Red Wolf of Radagon",        //override for bosses to avoid from catching "Wolf" in runeKeywords.
                    "0 $$$$ ##10 /100 Draconic Tree Sentinel",
                    "45 > +1 $$$$$$ {2 Tree Sentinel",
                    "4 Nox",

                    "13 #4 Ancestral Follower Shaman",  //ghost ones.

                    "10 Sanguine Noble",
                    "3 #1 Commoner",
                    "3 %0.5 -1 Putrid Corpse",
                    "5 %0.75 Revenant Follower",

                    "3 %0.75 sp1 Clayman",
                    "8 -1 Kindred of Rot",
                    "100 & ! Demi-Human Queen",
                    "5 -0.5 %0.7 Basilisk",
                    "2 -1 %0.5 Small Fingercreeper",
                    "100 >> x5xxx +1 & $$$5 Giant Fingercreeper",
                    "5 > %0.85 Fingercreeper",
                    "0.22 >5 #9 {1.2 }0.85 Wandering Noble",


                    "15 >> x1x2x3xxx $$$$$$ -0.75 %0.9 xv0.5 sp1 Grafted Scion",
                    "35 x2 $$ -1 &1 Death Bird",
                    "100 > x2xx & -1 $$$0.5 ! Death Rite Bird",               //set up just in case there is a respawner
                    "63 >" + x3Chance + "-2 $$$0.5 & ! Black Knife Assassin",   //set up just in case there is a respawner
                    "100 #10 $$$$ ! Black Knife Ringleader",    //Alecto
                    "30 >> & $$0.5 #9 x1x1x2xx Zamor",
                    
                    

                    "50 $$$$ +0.5 x4xxxx Misbegotten Crusader",
                    "2 -0.5 }0.9 >>>> x2x3xxx Perfumer",

                    "100 & ##3 Carian Knight Bols",
                    "15 %0.85 -1.35 > sp1 Snowfield Troll",
                    "15 %0.85 x3xxx -1.35 > $$$$ sp1 Frenzied Troll",
                    
                    "5 -2.2 sp1 Skeletal Grave Warden",

                };
                bothKeywords = new string[]{

                    "15 -1 >>> x1x2x3xx sss-1 $$$$$$$1 /50 Crystalian", //force somber ftd

                    "15" +x2Chance+" sss-1.8 -0.1 /25 Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.5 -0.1 $ /25 Karolos Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.5 -0.1 $ /25 Lazuli Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.5 -0.1 $ /25 Twinsage Glintstone Sorcerer",
     
                    //"0 x4 $$$$$$ #8 & Draconic Tree Sentinel",

                    "70 $$$$$$$ & x3xxx xvv0.5  /25 Troll Knight", // these are the raya lucaria ones.
                    "30 $ +0.75 {1.25 > x1x3xxx xvv0.65 /18 sss-2.2 sp1 Troll",

                    "10 #4 /25 Ancestral Follower (Siofra River)",   //ghost ones.

                    "14 > {0.72 /20 %0.9" + x2Chance + "Mausoleum Large Exile Soldier",
                    "9 > {0.72 /20 %0.9" + x2Chance + "Mausoleum Exile Soldier",

                    "45" + knight + x2Chance + "/20 Mausoleum Knight",
                    "4" + footSoldier + x2Chance + "/20 Mausoleum Foot Soldier",
                    "5.5" + soldier + x2Chance + "/20 Mausoleum Soldier",
                    "45" + banishedK + x3Chance + "/20 Mausoleum Banished Knight",

                    "65 /12 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen",
                    "65 /12 x4xx xv0.65 xvv0.5 Fell Twin",
                    "55 > /15 +0.5 & #8 $$$$$ ! " + "x2x3xx xv0.55 xvv0.55" + " Crucible Knight", //the percent chance increase has to acount for casscade.
                    "55 > /15 +0.5 & #8 $$$$$ ! " + "x2x3xx xv0.55 xvv0.55" + " Tanith's Knight", //the percent chance increase has to acount for casscade.


                    "45 -1> /20 $$ sss-1 -0.8 #8 Giant Beast Skeleton",
                    "45 -1 /20 $$ sss-0.8 #8 Giant Skeleton",
                    "4.5 -1 /20 sss-1.2 #8 Skeleton",
                    "4.6 -1> /20 sss-1.2 #8 Beast Skeletal",
                    "6 -1> /20 sss-1.2 #8 Beast Skeletal Knight",
                    "9 -1 /20 sss-1.2 #8 Skeletal Soldier",
                    "4.5 -1 /20 sss-1.2 #8 Skeletal",

                    "50 /30 x3 $$$$$$$ #8 Battlemage", //forced somber

                    "8 Guardian",
                    "15 High Page",
                    "65 /10" + knight + x3Chance + "Cleanrot Knight",
                    "10 >> sss-1.5 Mad Pumpkin Head",
                    "3 Highwayman",
                    "4.5 {0.85 >>> x1x1x2x3xx Glintstone Digger",
                    "25 /25 x4xxx %0.85 $$ xv0.6 xvv0.75 Guardian Golem",    //tweek to make archers kill.
                    "10 >2 x2 /8 Omenkiller",

                    "10 -1> x2x3xx /20 $$ Depraved Perfumer",
                    
                    "100 +1 sss-1 /20 $$$ Erdtree Burial Watchdog",


                
                };

                runeKeywords = new string[]{
                    "100 ##13 Elite Runebear",
                    "100 %0.5 +3 #13 {1.15 }0.85 sp1 & $2 Runebear",
                    "60" + GiantCrab + "Giant Death Crab",
                    "10 %0.5 #1 }0.85 sp1 Death Crab",
                    "60" + GiantCrab + "Giant Black Crab",
                    "10 %0.5 #1 }0.85 sp1 Black Crab",
                    "60" + GiantCrab + "Giant Crab",
                    "10 %0.5 #1 }0.85 sp1 Crab",

                    "20 %0.5 #5 }0.85 +1.5 sp1 Warhawk",

                    "15 > #6 }0.45 +0.6 & sp1 & $0.6 White Wolf",
                    "5 > %0.65 > #4 }0.85 -1 & sp1 Wolf",
                     "10 %0.5 >> }0.85 #13 +1 & sp1 Large Azula Stray",
                    "10 %0.5 >> }0.85 #13 +1 & sp1 Large Stray",
                    "7.5 %0.5 >> }0.85 #5 & sp1 Stray",

                    "12 >> $1 %0.35 +2.5 }0.85 {1.2 #8 sp1 & $$ Giant Rat",
                    "5 >> %0.35 }0.85 #3 sp1 & Rat",


                    "15 $4 ##5 Operatic Bat",   //uses flags that we want to keep.
                    "12 >>> %0.85 +0.7 |0 }0.875 {1.125 #5 sp1 Man-Bat",
                    "9 %0.5 > #6 }0.9 sp0.5 Dominula Celebrant", //already drops runes
                    "30  $$ %0.5 > #13 +4 }0.85 sp1 & Giant Land Squirt",
                    "30  $$ %0.5 > #13 +4 }0.85 sp1 & Giant Rotten Land Squirt",
                    "10 %0.5 > #5 +3 }0.85 sp1 & Land Squirt",


                    "45 %0.5 >> }0.85 +1 $1.5 sp1 & Giant Land Octopus",
                    "15 #1 }0.85 sp1 & Small Land Octopus",

                    "10 %0.5 #5 }0.85 sp1 & Guilty",
                    "8 %0.75 $3 sp1 #10 Fire Monk", //lands between runes.
                    "12 %0.75 $3 sp1 #10 Blackflame Monk",

                    "30  %0.5 +1 }0.85 #13 sp1 & Miranda Blossom",
                    "10  %0.5 }0.85 #3 & Miranda Sprout",

                    "10 >>  %0.5 +2 }0.85 #13 sp1 & Giant Dog",
                    "10 >>  %0.5 +2 }0.85 #13 sp1 & Giant Crow",

                    "30  %0.65 +2 }0.85 #13 sp1 & Wormface",
                    "30  %0.5 +2 }0.85 sp1 & Giant Ant",
                    "30  %0.5 +2 }0.85 sp1 & Starcaller",
                    "30  %0.65 +2 }0.85 sp1 & Giant Crayfish",
                    //"30  %0.5 +2 }0.85 sp1 Watcher Stones",
                    "30  %0.65 +2 }0.85 sp1 & Abductor Virgin",
                    //"30  %0.65 +2 }0.85 sp1 & Snowfield Troll",
                };

                string[] uncertain = {};    //unused

                exceptions = new string[]{

                    "Dragonfly",
                    "Dragonkin Soldier (Ice Lightning)",

                    "Ancient Dragon", //the ones that need to drop already drop ancient dragon smithing stone.
                    "Flying Dragon Greyll (Caelid)",    //she already OP.

                    "Arghanthy",
                    "Flame Guardian",

                    "(Mirage)",
                    "(Puppet)",
                    //"Tricia",
                     "(Spirit)",
                    "[Spirit Summon]",
                

                    "Horse",
                    "Donkey",

                    "Albinauric Archer's Wolf",
                    //"Tree Sentinel (Altus Plateua)",
                    "Soldier of Godrick",

                    "(Rennala)", //rennala summons

                    "Blaidd the Half-Wolf",
                    "Loretta",
                    "Bernahl",
                    "Istvan",
                    "Diallos",
                    "Margit",
                    "Morgott",
                    "Mohg",
                    "Juno",
                    //"Bols",   //this is the troll
                    "Tanith's Knight",
                    "Vyke",
                    "Boc",
                    "Knight of the Great",
                    "Latenna",
                    "Rogier",
                    "Thops",
                    "Moongrum",
                    "Imprisoned",
                    "Merchant",
                    "Black Dumpling Elder Albinauric",
                    "Imprisoned Elder Albinauric",

                    "Battlemage Hugues (Boss)",
                    "Cleanrot Knight (Boss)",

                    "Exile Soldier (Boss)",     //they respawn in O'Neal fight..
                    "Banished Knight (Niall Boss)", //just for consitancy


                    //"Bloodhound Knight Darriwil",
                    //"Bloodhound Knight (Boss)",

            };


                allKeywords = bothKeywords.Concat(somberKeywords).Concat(smithingKeywords).Concat(runeKeywords).ToArray();

                keywordOverrideIDsDict = new Dictionary<int, string>();
                {
                    keywordOverrideIDsDict.Add(43530020, "SS 60 ##3 " + knight + " Leyndell Knight (Lurinia Override)");
                    string altusLleyndelKnight = "SS 60 ##4.5 " + knight + " Leyndell Knight (Altus Override)";
                    string altusLleyndelSoldier = "SS 4.5 ##4.5 " + soldier + " Leyndell Soldier (Altus Override)";
                    keywordOverrideIDsDict.Add(43530032, altusLleyndelKnight);
                    keywordOverrideIDsDict.Add(43530030, altusLleyndelKnight);
                    keywordOverrideIDsDict.Add(43531130, altusLleyndelKnight);
                    keywordOverrideIDsDict.Add(43532030, altusLleyndelKnight);
                    keywordOverrideIDsDict.Add(43130030, altusLleyndelKnight);
                    keywordOverrideIDsDict.Add(43130130, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43131032, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43130032, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43132032, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43132030, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43130332, altusLleyndelSoldier);
                    keywordOverrideIDsDict.Add(43133030, altusLleyndelSoldier);

                    keywordOverrideIDsDict.Add(21400930, "SSSSS 65 ##4.5 /12 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen (Undocumented Omen Override)");  //undocumented omen in altus - lleyndel outskirts
                    keywordOverrideIDsDict.Add(21401930, "SSSSS 65 ##4.5 /12 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen (Undocumented Override)");  //undocumented omen in altus - lleyndel outskirts

                    keywordOverrideIDsDict.Add(46003140, "SSS 8 %0.85 -1.35 > sp1 $$$$1 Troll (Pot Thrower Override)");//Pot Thrower? Troll override
                    keywordOverrideIDsDict.Add(46000040, "SSS 8 %0.85 -1.35 > sp1 $$$$1 Troll (Pot Thrower Override)");//Pot Thrower? Troll override
                    keywordOverrideIDsDict.Add(46001020, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46001030, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46001010, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46000065, "");//Mimic Troll override

                    keywordOverrideIDsDict.Add(43520020, "SS 45 ##3.5 " + knight + " Cuckoo Knight (Four Belfries and Bellum Override)");
                    keywordOverrideIDsDict.Add(43550020, "SSSSS 25 /20 ##3 " + knight + " Mausoleum Knight (BK Catacombs Override)"); //too farmabale
                    keywordOverrideIDsDict.Add(45100572, "SSSSS 100 > +10 xxx5 & $$$$ Ancient Dragon (Droppers Override)");
                    keywordOverrideIDsDict.Add(46500265, "SS 100 x5 ##3 Dragonkin Soldier (Nokrom Override)");
                    keywordOverrideIDsDict.Add(45102030, "SS 100 & +22 xxx5 $$$$$$$$ Ancient Dragon Lansseax (Ancient Dragon Exception");

                    keywordOverrideIDsDict.Add(30100172, "SSSSS 45" + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    keywordOverrideIDsDict.Add(30101172, "SSSSS 45" + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    keywordOverrideIDsDict.Add(30102172, "SSSSS 45" + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    
                    keywordOverrideIDsDict.Add(34510912, "SS 30 -0.33 x2 $$ Scaly Misbegotten (Morne Tunnel Boss Override)");

                    keywordOverrideIDsDict.Add(31810022, "SSS 100 ##6.9 ! Red Wolf of Radagon (Moonlight Altar Override)");
                    keywordOverrideIDsDict.Add(45021922, "SS 100 & x4xx $$$$ ##8 sss-0.8 Glintstone Dragon Adula (Moonlight Altar Override)");
                    keywordOverrideIDsDict.Add(45020022, "SS 100 & x4xx $$$$ ##7 sss-0.7 Glintstone Dragon (Moonlight Plateau Override)");
                    
                    //keywordOverrideIDsDict.Add(35700028, "SSS 100 ##8 Godskin Noble (Liurnia Divine Tower Override)");

                    //keywordOverrideIDsDict.Add(32511030, "SSS 45 > +1 $$$$$$ Tree Sentinel (Lleyndel Outskirts Duo Override)");
                    //keywordOverrideIDsDict.Add(32510030, "SSS 45 > +1 $$$$$$ Tree Sentinel (Lleyndel Outskirts Duo Override)");
                }
            }
            //




            //PRORITY GOES -BOTH KWs -> SOMBER KEYWORDS -> SMITHING KWs. from most specific to ease generic.

            Dictionary<string, bool> isSmithingDict = new Dictionary<string, bool>();
            Dictionary<string, bool> isSomberDict = new Dictionary<string, bool>();
            Dictionary<string, bool> isForceBossDisplayDict = new Dictionary<string, bool>();
            Dictionary<string, bool> isRuneDict = new Dictionary<string, bool>();
            Dictionary<string, int> firstTimeDropDict = new Dictionary<string, int>();
            Dictionary<string, float> firstTimeDropAdjDict = new Dictionary<string, float>();
            Dictionary<string, int[]> amountMultsDict = new Dictionary<string, int[]>();
            Dictionary<string, int> XasscadeDict = new Dictionary<string, int>();
            Dictionary<string, float> XasscadeMultToXDict = new Dictionary<string, float>();
            Dictionary<string, float> XasscadePowToXDict = new Dictionary<string, float>();

            Dictionary<string, float> spLevelSplitDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMultDict = new Dictionary<string, float>();
            Dictionary<string, float> levelAdjDict = new Dictionary<string, float>();
            Dictionary<string, float> levelClampDict = new Dictionary<string, float>();
            Dictionary<string, float> setLevelDict = new Dictionary<string, float>();
            Dictionary<string, float> levelToChanceMultDict = new Dictionary<string, float>();
            Dictionary<string, float> somberLevelAdjDict = new Dictionary<string, float>();


            Dictionary<string, float> casscadeLevelToChanceMultDict = new Dictionary<string, float>();
            Dictionary<string, int> levelCasscadeDict = new Dictionary<string, int>();
            Dictionary<string, float> casscadePoolingMultDict = new Dictionary<string, float>();
            Dictionary<string, int> bothPercentSplitForSomberDict = new Dictionary<string, int>();
            Dictionary<string, string> simplifiedKeywordNameDict = new Dictionary<string, string>();
            Dictionary<string, float> percentNumDict = new Dictionary<string, float>();
            Dictionary<string, bool> oneLineDict = new Dictionary<string, bool>();

            /*var cumulateNumFlagId = ItemLotParam_enemy.GetFieldIndex("cumulateNumFlagId");
            var cumulateNumMax = ItemLotParam_enemy.GetFieldIndex("cumulateNumMax");
            var cumulateLotPoint01 = ItemLotParam_enemy.GetFieldIndex("cumulateLotPoint01");
            var cumulateReset01 = ItemLotParam_enemy.GetFieldIndex("cumulateReset01");*/
            

            for (int k = 0; k < 2; k++)
            {
                string[] keywords = null;
                bool isOverride = (k == 1);
                if (k == 0)
                    keywords = allKeywords;
                else if (k == 1)    //1 == isOverride
                {
                    var keywordList = new List<string>();
                    foreach (string keyword in keywordOverrideIDsDict.Values)
                        keywordList.Add(keyword);
                    keywords = keywordList.ToArray();
                }
                foreach (string keyword in keywords)
                {
                    if (keyword == "")
                        continue;
                    if (isOverride && isSmithingDict.ContainsKey(keyword))
                        continue;

                    bool isSmithing;
                    bool isSomber;
                    bool isRune;




                    float percentNum = 0;
                    //num at the beggining. 
                        //"30 knight" = 30.

                    List<int> amountMults = new List<int>();
                    int xasscade = 0;
                    float xasscadeMultToX = 1;
                    float xasscadePowToX = 1;
                    //num after 'x'. 
                    //"30 x2 knight" = 3 

                    float casscadePoolingMult = 1;

                    bool isForceBossDisplay = false;
                    int firstTimeDropSeverity = -1;
                    float firstTimeDropAdj = 0;

                    float somberLevelAdj = 0;

                    float levelMult = 1;
                    float levelAdj = 0;
                    //num after + or - based on the adj.
                    //"30 +1 knight" = +1 
                    //"30 -1 knight" = -1 
                    float spLevelSplit = 0;
                    float setLevel = -1;    //##num
                    float levelClamp = -1;    //#num
                    float levelToChanceMult = 1; //}num
                    float casscadeLevelToChanceMult = 1; //{num

                    int LevelCasscade = 0;  //-1 means all the way to lowed level. >1 specific cascade amount.
                                            //based on number of '>'.
                                            //if one is found. then -1. full cascade.
                                            //if num of ">" is greater than one is found then its that number minus one.
                                            //if a number is found directly after a ">" that will be the casscade level. 
                                            //"30> knight"  = -1
                                            //"30>> knight" = 1
                                            //"30>>>> knight" = 3
                                            //"30>3 knight" = 3

                    bool oneLine = false;   //&

                    


                    int bothPercentSplitForSomber = 25; //if is both, this represents the chance for it to be a somber drop vs smithing drop.
                                                        //num after '/'. 
                                                        //"30 /50 knight" = 50 

                    string simplifiedKeywordName = "";

                    string normalizedKeyword = keyword; //used for getting a name. removes "S"s used in overrides. 

                    
                    //assign keyword values;
                    {
                        bool isBoth = false;
                        if (k == 1)
                        {
                            isBoth = keyword.IndexOf("SSSSS") == 0;
                            isSomber = isBoth || keyword.IndexOf("SSS") == 0;
                            isSmithing = isBoth || (!isSomber && keyword.IndexOf("SS") == 0);
                            isRune = keyword.IndexOf("GRUNE") == 0;
                            int newKeywordStartingIndex = 1;
                            if (isSmithing)
                                newKeywordStartingIndex += 2;
                            if (isSomber)
                                newKeywordStartingIndex += 3;
                            normalizedKeyword = keyword.Remove(0, newKeywordStartingIndex);
                        }
                        else
                        {
                            isBoth = bothKeywords.Contains(keyword);
                            isSmithing = isBoth || smithingKeywords.Contains(keyword);
                            isSomber = isBoth || somberKeywords.Contains(keyword);
                            isRune = runeKeywords.Contains(keyword);
                        }

                        if (isBoth)
                        {
                            //get both percent split
                            string pattern = @"/(\d+)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                bothPercentSplitForSomber = int.Parse(match.Groups[1].Value);
                        }

                        {
                            //get level adjust
                            string pattern = @" \+(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                levelAdj = float.Parse(match.Groups[1].Value);
                            else
                            {
                                pattern = @" -(\d+(\.\d+)?)";
                                match = Regex.Match(keyword, pattern);
                                if (match.Success)
                                    levelAdj = -float.Parse(match.Groups[1].Value);
                            }

                        }
                        {

                            //get level adjust
                            string pattern = @"sss\+(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                            {
                                somberLevelAdj = float.Parse(match.Groups[1].Value);
                                normalizedKeyword = normalizedKeyword.Replace("sss+","");
                            }
                            else
                            {
                                pattern = @"sss-(\d+(\.\d+)?)";
                                match = Regex.Match(keyword, pattern);
                                if (match.Success)
                                    somberLevelAdj = -float.Parse(match.Groups[1].Value);
                                normalizedKeyword = normalizedKeyword.Replace("sss-", "");
                            }

                        }
                        {
                            //get casscade pooling mult
                            string pattern = @"\|(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                casscadePoolingMult = float.Parse(match.Groups[1].Value);

                        }

                        {
                            //get level mult
                            string pattern = @"\%(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                levelMult = float.Parse(match.Groups[1].Value);
                        }

                        {
                            //get spLevelSplit
                            string pattern = @"sp(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                spLevelSplit = float.Parse(match.Groups[1].Value);
                        }

                        {
                            int xFound = 0;
                            string xx = "#";
                            while (keyword.IndexOf(xx) != -1)
                            {
                                xFound++;
                                xx += "#";
                            }
                            if (xFound > 0)
                            {
                                //get setLevel
                                string pattern = @"#(\d+(\.\d+)?)";
                                Match match = Regex.Match(keyword, pattern);
                                if (match.Success)
                                {
                                    if(xFound == 2)
                                        setLevel = float.Parse(match.Groups[1].Value);
                                    else if(xFound == 1)
                                        levelClamp = float.Parse(match.Groups[1].Value);
                                }
                                //if (isOverride && keywordOverrideIDsDict[43530020] == keyword)
                                //    Util.p();
                            }
                        }                        
                        {
                            //get level to chance mult
                            string pattern = @"}(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                            {
                                    levelToChanceMult = float.Parse(match.Groups[1].Value);
                            }
                        }
                        {
                            //get casscade level to chance mult
                            string pattern = @"{(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                            {
                                casscadeLevelToChanceMult = float.Parse(match.Groups[1].Value);
                            }
                        }
                        {

                            //get amount mult
                            string curKeyword = keyword;
                            
                            string pattern = @"x(\d+)";
                            Match match = Regex.Match(curKeyword, pattern);
                            amountMults = new List<int>();
                            while (match.Success)
                            {
                                for (int i = 1; i < match.Groups.Count; i+=2)
                                {

                                    string val = match.Groups[i].Value;
                                    int matchNum = int.Parse(val);
                                    amountMults.Add(matchNum);
                                    curKeyword = curKeyword.Remove(0, curKeyword.IndexOf("x"+val) + ("x"+val).Length);
                                }
                                match = Regex.Match(curKeyword, pattern);
                            }
                            /*if(amountMults.Count > 0)
                            {
                                var s = ":";
                                for (int i = 0; i < amountMults.Count; i++)
                                {
                                    s+=" "+amountMults[i] + ",";
                                }
                                Util.println(keyword + "        " + s);
                            }*/
                            int xFound = 0;
                            string xx = "x";
                            while (keyword.IndexOf(xx) != -1)
                            {
                                xFound++;
                                xx += "x";
                            }
                            xasscade = Math.Max(xFound - 1,0);

                            /*int vFound = 0;
                            string vv = "v";
                            while (keyword.IndexOf(vv) != -1)
                            {
                                vFound++;
                                vv += "v";
                            }
                            xasscadeMultToX = Math.Max(vFound,1);*/

                            pattern = @"xv(\d+(\.\d+)?)";
                            match = Regex.Match(keyword, pattern);
                            if (match.Success)
                            {
                                xasscadeMultToX = float.Parse(match.Groups[1].Value);
                            }

                            pattern = @"xvv(\d+(\.\d+)?)";
                            match = Regex.Match(keyword, pattern);
                            if (match.Success)
                            {
                                xasscadePowToX = float.Parse(match.Groups[1].Value);
                            }

                        }

                        {
                            //get amount mult
                            int num = keyword.Count(c => c == '>');
                            if (num == 1)
                            {
                                //LevelCasscade = -1;
                                LevelCasscade = 1;

                                //checks if number after '>' to override cascade level.
                                string pattern = @">(\d+)";
                                Match match = Regex.Match(keyword, pattern);
                                if (match.Success)
                                    LevelCasscade = int.Parse(match.Groups[1].Value);

                            }
                            else if (num > 1)
                            {
                                LevelCasscade = num;
                            }

                        }
                        {
                            //get isForceBoss
                            if (keyword.Contains("!"))
                            {
                                isForceBossDisplay = true;
                            }
                        }
                        {
                            //get firstTimeDrop
                            int xFound = 0;
                            string xx = "$";
                            while (keyword.IndexOf(xx) != -1)
                            {
                                xFound++;
                                xx += "$";
                            }
                            if (xFound!= 0)
                            {
                                firstTimeDropSeverity = xFound;
                                //fistTimeDropAdj
                                string pattern = @"\$(\d+(\.\d+)?)";
                                Match match = Regex.Match(keyword, pattern);
                                if (match.Success)
                                {
                                    firstTimeDropAdj = float.Parse(match.Groups[1].Value);
                                }
                                else
                                {
                                    pattern = @"\$\+(\d+(\.\d+)?)";
                                    match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        firstTimeDropAdj = float.Parse(match.Groups[1].Value);
                                    }
                                    else
                                    {
                                        pattern = @"\$-(\d+(\.\d+)?)";
                                        match = Regex.Match(keyword, pattern);
                                        if (match.Success)
                                        {
                                            firstTimeDropAdj = -float.Parse(match.Groups[1].Value);
                                        }
                                    }
                                }
                            }
                        }

                        {
                            //get simplified name

                            // Define the regular expression pattern to split the inputString into words
                            string pattern = @"\b\w+\b";

                            // Use Regex.Matches to find all occurrences of the pattern in the inputString
                            MatchCollection matches = Regex.Matches(normalizedKeyword, pattern);

                            // If there are no matches, return an empty string
                            if (matches.Count == 0)
                            {
                                simplifiedKeywordName = "INVALID KEYWORD";
                            }
                            else
                            {
                                // Check each word to find the first word that meets the criteria
                                for (int i = matches.Count - 1; i >= 0; i--)
                                {
                                    var match = matches[i];
                                    string word = match.Value;
                                    // Define a regular expression pattern to check for symbols or numbers in the word
                                    pattern = @"[\p{P}\d]";

                                    // Use Regex.IsMatch to check if the word contains any symbols or numbers
                                    if (!Regex.IsMatch(word, pattern))
                                    {
                                        // Get the position of the last character of the word
                                        int wordStartPosition = match.Index;

                                        // Extract the remainder of the string after the word
                                        string remainder = normalizedKeyword.Substring(wordStartPosition).TrimStart();
                                        simplifiedKeywordName = remainder;
                                    }
                                }
                            }
                        }
                        {
                            //get isOneLine
                            oneLine = normalizedKeyword.Contains("&");
                        }
                        {
                            //get percent num
                            //Util.println(keyword.Split(' ')[0]);
                            percentNum = float.Parse(normalizedKeyword.Split(' ')[0]);
                        }
                    }

                    //Debug.Assert(!simplifiedKeywordNameDict.ContainsKey(simplifiedKeywordName), "cant share same keyword phrase");
                    //Debug.Assert(!isSmithingDict.ContainsKey(keyword), "cant share same keyword");


                    isSmithingDict.Add(keyword, isSmithing);
                    isSomberDict.Add(keyword, isSomber);
                    isRuneDict.Add(keyword, isRune);
                    isForceBossDisplayDict.Add(keyword, isForceBossDisplay);
                    firstTimeDropDict.Add(keyword, firstTimeDropSeverity);
                    firstTimeDropAdjDict.Add(keyword, firstTimeDropAdj);

                    amountMultsDict.Add(keyword, amountMults.ToArray());
                    XasscadeDict.Add(keyword, xasscade);
                    XasscadeMultToXDict.Add(keyword, xasscadeMultToX);
                    XasscadePowToXDict.Add(keyword, xasscadePowToX);
                    bothPercentSplitForSomberDict.Add(keyword, bothPercentSplitForSomber);

                    spLevelSplitDict.Add(keyword, spLevelSplit);
                    levelMultDict.Add(keyword, levelMult);
                    levelAdjDict.Add(keyword, levelAdj);
                    setLevelDict.Add(keyword, setLevel);
                    levelClampDict.Add(keyword, levelClamp);
                    levelToChanceMultDict.Add(keyword, levelToChanceMult);
                    casscadeLevelToChanceMultDict.Add(keyword, casscadeLevelToChanceMult);
                    somberLevelAdjDict.Add(keyword, somberLevelAdj);

                    levelCasscadeDict.Add(keyword, LevelCasscade);
                    casscadePoolingMultDict.Add(keyword, casscadePoolingMult);
                    simplifiedKeywordNameDict.Add(simplifiedKeywordName,keyword);
                    percentNumDict.Add(keyword, percentNum);
                    oneLineDict.Add(keyword, oneLine);
                }
            }

            //check names in dictionary.
            //check name kewords to see if its smithing, somber, or both.
            //parse percent/itemlot data from keyword.

            Line baseEmptyItemLotParamLine = ItemLotParam_enemy.vanillaParamFile.GetLineWithId(460000500).Copy(ItemLotParam_enemy)
                .SetField(1,"");

            Lines itemLotMainLines = new Lines(ItemLotParam_enemy);

            //string[] typeStrings = { "Smithing Stone", "Somber Smithing Stone", "Golden Rune" };
            var smithingStoneIDsDict = new Dictionary<int, int>();
            var somberSmithingStoneIDsDict = new Dictionary<int, int>();
            var runesIDsDict = new Dictionary<int, int>();
            {
                smithingStoneIDsDict.Add(1, 10100);
                smithingStoneIDsDict.Add(2, 10101);
                smithingStoneIDsDict.Add(3, 10102);
                smithingStoneIDsDict.Add(4, 10103);
                smithingStoneIDsDict.Add(5, 10104);
                smithingStoneIDsDict.Add(6, 10105);
                smithingStoneIDsDict.Add(7, 10106);
                smithingStoneIDsDict.Add(8, 10107);
                smithingStoneIDsDict.Add(9, 10140);
                somberSmithingStoneIDsDict.Add(1, 10160);
                somberSmithingStoneIDsDict.Add(2, 10161);
                somberSmithingStoneIDsDict.Add(3, 10162);
                somberSmithingStoneIDsDict.Add(4, 10163);
                somberSmithingStoneIDsDict.Add(5, 10164);
                somberSmithingStoneIDsDict.Add(6, 10165);
                somberSmithingStoneIDsDict.Add(7, 10166);
                somberSmithingStoneIDsDict.Add(8, 10167);
                somberSmithingStoneIDsDict.Add(9, 10200);
                somberSmithingStoneIDsDict.Add(10, 10168);
                //golden rune ids are 2899 + golden rune level. for golen rune ID
                int i = 1;
                for (; i <= 20; i++)// first 13 are golden runes, 1 numen rune, 5 hero runes, 1 lord rune.
                {
                    runesIDsDict.Add(i, 2899 + i);
                }
                runesIDsDict.Add(i, 2990);  //lands berween runes.

            }//create ditionaries

            Dictionary<int, int>[] dropTypesIDsDictionaries = new Dictionary<int, int>[3];
            dropTypesIDsDictionaries[0] = smithingStoneIDsDict;
            dropTypesIDsDictionaries[1] = somberSmithingStoneIDsDict;
            dropTypesIDsDictionaries[2] = runesIDsDict;



            //loop through npc ids.
            List<Line> documentedLines = new List<Line>();
            List<int> npcIDs = new List<int>();
            {
                //string documentedMark = " !";
                foreach (int npcID in npcsDocDifficultyDict.Keys)
                {
                    Line npcLine = NpcParam.GetLineWithId(npcID);
                    if (npcLine == null)
                    {
                        Util.println("ID not found: " + npcID + " ... excluding");
                        continue;
                    }
                    bool isBoss = BossOrMiniBossIds.Contains(npcLine.id_int);
                    if (!isBoss && npcLine.GetField("getSoul") == "0")
                        continue;
                    documentedLines.Add(npcLine);
                    //npcLine.SetField(1, npcLine.name + documentedMark);
                    npcIDs.Add(npcID);
                }
                foreach (Line npcLine in NpcParam.lines)
                {
                    if (!npcIDs.Contains(npcLine.id_int))
                    {
                        bool isBoss = BossOrMiniBossIds.Contains(npcLine.id_int);
                        if (!isBoss && npcLine.GetField("getSoul") == "0")
                            continue;
                        npcIDs.Add(npcLine.id_int);
                    }
                }
            }   //sets npcIDs, putting the documented ones first.

            int test = -1;

            //List<int> freedFlagIds = new List<int>();
            int specialDropUniqueIndex = 0;
            /*
            IntFilter[] typeGetItemFlagIdFilters = new IntFilter[3];
            var commongetItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), -1, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            typeGetItemFlagIdFilters[0] = commongetItemFlagIDFilter;  //Smithing Stone getItemFlagID 
            typeGetItemFlagIdFilters[1] = commongetItemFlagIDFilter;  //Somber Smithing Stone getItemFlagID Filters
            typeGetItemFlagIdFilters[2] = commongetItemFlagIDFilter;  //Golden Rune getItemFlagID Filters
            */
            //var commongetItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), -1, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0); //all available ids.

                                                                                                 //Specific Filter Digit
            var StoneDrop_getItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5),     1,      IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            var RuneDrop_getItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5),      2,      IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            var OneTimeDrop_getItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5),   9,      IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);

            List<int> usedGetItemFlagId;
            {
                int _getItemFlagId = ItemLotParam_map.GetFieldIndex("getItemFlagId");
                var inIdRange = new Condition.FloatFieldBetween(_getItemFlagId, 1000000000, 2000000000);
                usedGetItemFlagId = Util.ToInts(((Lines)NpcParam.GetLinesOnCondition(inIdRange)).GetFields(_getItemFlagId)).ToList();
                usedGetItemFlagId = usedGetItemFlagId.Concat(Util.ToInts(((Lines)NpcParam.GetLinesOnCondition(inIdRange)).GetFields(_getItemFlagId))).ToList();
            }

            //Util.println("" + 7.5f);
            Dictionary<int, int> AsssignedWeaponsFlagIdDict;
            Dictionary<int, int> AsssignedArmorsFlagIdDict;
            AsssignedWeaponsFlagIdDict = new Dictionary<int, int>();
            AsssignedArmorsFlagIdDict = new Dictionary<int, int>();

            Dictionary<Line, string> idToVariantsDict = new Dictionary<Line, string>();
            Dictionary<Line, float> idToLevelDict = new Dictionary<Line, float>();

            foreach (int npcID in npcIDs)
            {
                test = -1;
                //bool isDocumented = l.name.Contains(documentedMark);

                //  Get the npcparam line with id. 
                Line npcLine = NpcParam.GetLineWithId(npcID);

                bool documented = npcsDocDifficultyDict.ContainsKey(npcLine.id_int);
                //if (!npcLine.name.Contains("Omen"))
                //    continue;
                bool isOverrideKeywordID = false;
                string simplifiedKeyword = "";
                string keyword = "";
                int keywordIndex = -1;


                //keyword check
                if (keywordOverrideIDsDict.ContainsKey(npcID))
                {
                    keyword = keywordOverrideIDsDict[npcID];
                    if (keyword == "")
                        continue;
                    isOverrideKeywordID = true;
                    foreach (string _simplifiedKeyword in simplifiedKeywordNameDict.Keys)
                    {
                        keywordIndex++;
                        if (simplifiedKeywordNameDict[_simplifiedKeyword] == keyword)
                        {
                            simplifiedKeyword = _simplifiedKeyword;
                            break;
                        }
                    }
                }
                else
                {
                    bool isException = false;
                    foreach (string exception in exceptions)
                    {
                        if (npcLine.name.Contains(exception))
                        {
                            //Util.println("exception: " + '"' + exception + '"' + " found in" + npcLine.name);
                            isException = true;
                            break;
                        }
                    }
                    if (isException)
                        continue;


                    string target = npcLine.name;
                    var targetWords = target.Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                    var importantWords = targetWords;
                    if (target.Contains("("))
                    {
                        importantWords = target.Remove(target.IndexOf("(")).Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                    }

                    string BestSimpKeyword = "";
                    int BestSimpKeywordScore = -1;
                    int BestSimpKeywordIndex = -1;
                    foreach (string _simplifiedKeyword in simplifiedKeywordNameDict.Keys)
                    {
                        keywordIndex++;
                        //Util.println(simplifiedKeyword + "  " + simplifiedKeywordNameDict[simplifiedKeyword]);

                        if (npcLine.name.Contains(_simplifiedKeyword)) // must contain.
                        {
                            bool isSame = _simplifiedKeyword == target;
                            int score = 0;
                            if (!isSame)
                            {
                                score = Util.WordMatchScore(_simplifiedKeyword, targetWords, out int matchCount, 0, importantWords, true);  //choose best that contains
                            }
                            if (isSame || score > BestSimpKeywordScore)
                            {
                                BestSimpKeywordScore = score;
                                BestSimpKeywordIndex = keywordIndex;
                                BestSimpKeyword = _simplifiedKeyword;
                                if (isSame)
                                    break;
                            }
                        }
                    }

                    if (BestSimpKeywordScore == -1)
                        continue;
                    keywordIndex = BestSimpKeywordIndex;
                    simplifiedKeyword = BestSimpKeyword;
                    keyword = simplifiedKeywordNameDict[simplifiedKeyword];

                }

                if (keyword == "")
                {
                    continue;
                }

                //if(keywordOverrideIDsDict.ContainsKey(npcID))
               // if(somberLevelAdjDict[keyword] != 0)\
               //if(keyword == "15 x2xx xvv0.3  sss-1.6 -0.1 /25 Glintstone Sorcerer")
                //SET TEST
                //if (npcID == 46010920)
                //if( 
                //    npcLine.name.Contains("Draconic Tree"))
                //&& (keywordOverrideIDsDict.ContainsKey(npcID))
                //  npcLine.name.Contains("Godrick Soldier") )
                //if     (npcLine.name.Contains("igger"))

                //test = npcID;


                bool isBoss = BossOrMiniBossIds.Contains(npcLine.id_int);




                if (test == npcID)
                    Util.println(npcLine._idName + " documented " + documented + "  isBoss:" + isBoss);


                float level = -1;

                Line mostMatchedLine = null;

                const bool debugAssignedLevels = false;

                int mySpLevelNum = -1;
                float spLevel = -1;
                {

                    if (npcsIdToSpLevelsDict.ContainsKey(npcID))
                    {
                        mySpLevelNum = npcsIdToSpLevelsDict[npcID];
                        if (spLevelToDifficultyDict.ContainsKey(mySpLevelNum))
                            spLevel = spLevelToDifficultyDict[mySpLevelNum];
                    }
                }


                //aqusitionIDs limitations. GOTO
                //


                level = setLevelDict[keyword];

                if (level == -1)
                {
                    if (documented)
                    {

                        level = npcsDocDifficultyDict[npcLine.id_int];
                        if (test == npcID || debugAssignedLevels)
                            Util.println(npcLine._idName + " GOT DOCUMENTED LEVEL: " + level);
                    }
                    else
                    {
                        if (spLevel == -1)
                            continue;

                        level = spLevel;


                        if (level == -1)
                        {
                            string target = npcLine.name;
                            var targetWords = target.Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                            var importantWords = targetWords;
                            if (target.Contains("("))
                            {
                                importantWords = target.Remove(target.IndexOf("(")).Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                            }

                            int maxMatchCount = 0;
                            var orderedNestedLines = LineFunctions.GetOrderedWordMatchedNestedLines(targetWords, documentedLines, out maxMatchCount, out int maxScore, importantWords.Length, importantWords, true);



                            if (npcID == test)
                                Util.println("Searched for \"" + target + "\"" + " maxMatchCount:" + maxMatchCount);
                            if (!npcsIdToSpLevelsDict.ContainsKey(npcID))
                                continue;

                            float minDocumentedLevel = int.MaxValue;
                            float maxDocumentedLevel = -1;

                            Line maxLine = null;
                            Line minLine = null;

                            Line perfectLine = null;

                            bool minLocked = false;
                            bool maxLocked = false;

                            if (orderedNestedLines != null && orderedNestedLines.Count() > 0 && orderedNestedLines[0][0] != null && maxMatchCount == importantWords.Length)
                                mostMatchedLine = orderedNestedLines[0][0];

                            for (int i = 0; i < 2; i++) //0 = min (SS) 1 = max (SSS)
                            {
                                if (level != -1)
                                    break;
                                foreach (List<Line> lines in orderedNestedLines)
                                {
                                    //if (npcID == test)
                                    //    Util.println("ll " + level);
                                    foreach (Line l in lines)
                                    {
                                        //bool ldocumented = l.name.Contains(documentedMark);
                                        if (l.id_int == npcID)
                                            continue;
                                        if (!npcsIdToSpLevelsDict.ContainsKey(l.id_int))
                                            continue;
                                        int lSpLevelNum = npcsIdToSpLevelsDict[l.id_int];
                                        if (isBoss != BossOrMiniBossIds.Contains(l.id_int))
                                            //bosses only compare themselves to other bosses. and none bosses only compare to non bosses
                                            continue;
                                        //if (!npcsDifficultyDict.ContainsKey(l.id_int)) //is not documented
                                        //    continue;                                  //NOT NEEDED NOW uses docmuntedLines anyways.
                                        if (keywordOverrideIDsDict.ContainsKey(l.id_int))//dont count weird exceptions.
                                            continue;


                                        if (lSpLevelNum == mySpLevelNum)
                                        {
                                            perfectLine = l;
                                            level = npcsDocDifficultyDict[l.id_int];
                                            break;
                                        }
                                        else
                                        {
                                            if (i == 0 && !minLocked && lSpLevelNum < minDocumentedLevel && Math.Abs(lSpLevelNum - mySpLevelNum) < Math.Abs(mySpLevelNum - minDocumentedLevel) && lSpLevelNum != maxDocumentedLevel)
                                            {
                                                minDocumentedLevel = spLevelToDifficultyDict[lSpLevelNum];
                                                minLine = l;
                                            }
                                            if (i == 1 && !maxLocked && lSpLevelNum > maxDocumentedLevel && Math.Abs(lSpLevelNum - mySpLevelNum) < Math.Abs(mySpLevelNum - maxDocumentedLevel) && lSpLevelNum != minDocumentedLevel)
                                            {
                                                maxDocumentedLevel = spLevelToDifficultyDict[lSpLevelNum];
                                                maxLine = l;
                                            }
                                        }
                                    }
                                    if (level != -1)
                                        break;
                                    if (i == 0)
                                    {
                                        if (minLine != null)
                                            minLocked = true;
                                        if (minLocked)
                                            break;
                                    }
                                    else
                                    {
                                        if (maxLine != null)
                                            maxLocked = true;
                                        if (maxLocked)
                                            break;
                                    }
                                }
                            }





                            if (level == -1)
                            {
                                level = spLevelToDifficultyDict[mySpLevelNum];
                                if (level == -1)
                                {
                                    const bool testingEstimate = true;
                                    if (maxLocked && minLocked)
                                    {
                                        float minLvl = npcsDocDifficultyDict[minLine.id_int];
                                        float maxLvl = npcsDocDifficultyDict[maxLine.id_int];

                                        float runesPerLevel = (maxDocumentedLevel - minDocumentedLevel) / (maxLvl - minLvl);
                                        level = mySpLevelNum / runesPerLevel;    //exact value.
                                        level = (float)Math.Round(level / levelEstimateIntreval) * levelEstimateIntreval;

                                        //Debug.Assert(level != 0, ""+npcID);
                                        if (level == 0)
                                        {
                                            if (testingEstimate || test == npcID)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (level < 0)
                                        {
                                            if (testingEstimate || test == npcID)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (level > 10)
                                        {
                                            if (testingEstimate || test == npcID)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (testingEstimate || test == npcID)
                                            Util.println(npcLine._idName + " ESTIMATED!!! |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                    }
                                    else
                                    {


                                        if (minLocked)
                                        {
                                            //level = npcsDifficultyDict[minLine.id_int];
                                            float minLvl = npcsDocDifficultyDict[minLine.id_int];
                                            float maxLvl = 1;
                                            maxDocumentedLevel = 200;
                                            float runesPerLevel = (maxDocumentedLevel - minDocumentedLevel) / (maxLvl - minLvl);
                                            level = mySpLevelNum / runesPerLevel;    //exact value.
                                            level = (float)Math.Round(level / levelEstimateIntreval) * levelEstimateIntreval;
                                        }
                                        //else if (maxLocked) //this probably doesnt happen minLvl is always proritized
                                        //    level = npcsDifficultyDict[maxLine.id_int];
                                        else
                                        {
                                            //Debug.Fail("" + npcID);
                                            if (testingEstimate || test == npcID)
                                                Util.println(npcLine._idName + " FAILED to estimate");
                                            continue;
                                        }
                                        if (testingEstimate || test == npcID)
                                            Util.println(npcLine._idName + " CHOSE IMPERFECTLY myRunes:" + mySpLevelNum + "    minRune:" + minDocumentedLevel + "   minLvl: " + npcsDocDifficultyDict[minLine.id_int] + "   level:" + level);

                                    }
                                }
                                else if (test == npcID || debugAssignedLevels)
                                    Util.println(npcLine._idName + " SP ASSIGNED LEVEL: " + level + "  mySpLevel:" + mySpLevelNum);
                            }
                            else if (test == npcID || debugAssignedLevels)
                                Util.println(npcLine._idName + " FOUND MATCHED LEVEL: " + level + "   " + perfectLine._idName + " shares mySpLevel:" + mySpLevelNum);
                        }
                        else if (test == npcID || debugAssignedLevels)
                            Util.println(npcLine._idName + " FOUND SP LEVEL: " + level);
                        //check it has !

                        //check hp to be equal.
                        //  if take item lot param and continue.
                        //if we reach the end of best lines. AND we have 2 or more lines best lines with diffrent health. We approximate our level.

                        //hpPerLvl = ((maxHP - minHP) / (maxLvl - minLvl))
                        //myHP / hpPerLvl

                        //if (npcID == test)
                        //   Util.println("Estimated level " + level);

                    }

                    float spLevelSplit = spLevelSplitDict[keyword];
                    float preSplitLevel = level;
                    if (spLevelSplit != 0 && spLevel != -1 && level != spLevel)
                        level = (spLevelSplit * spLevel) + (level * (1 - spLevelSplit));
                    if (test == npcID)//|| (spLevelSplit != 0 && preSplitLevel != level))
                        Util.println(npcLine._idName + " documented " + documented + " preSplitlevel " + preSplitLevel + "  spLevelSplit " + spLevelSplit + "  level " + level);
                }
                else if (test == npcID || debugAssignedLevels)
                    Util.println(npcLine._idName + " KEY WORD ASSIGNED LEVEL: " + level);


                //  look at its itemLotParam_enemy. 

                bool IsItemLotMapDrop = false;
                List<int> ItemLotMapIds = new List<int>();


                int itemLotID = -1;

                if (BossOrMiniBossToItemLotMapDict.ContainsKey(npcID))
                    ItemLotMapIds.Add(BossOrMiniBossToItemLotMapDict[npcID]);
                if (BossOrMiniBossToItemLotMapDict2.ContainsKey(npcID))     //for exception where one npc id has 2 itemlotmaps
                    ItemLotMapIds.Add(BossOrMiniBossToItemLotMapDict2[npcID]);
                if (ItemLotMapIds.Count == 0)
                {
                    itemLotID = int.Parse(npcLine.GetField("itemLotId_enemy"));
                }
                else
                    IsItemLotMapDrop = true;


                //var levelString = "";
                //if (!IsItemLotMapDrop)
                //    levelString = " (Level: " + level + ")";

                for (int itemLotMapIndex = 0; itemLotMapIndex < ItemLotMapIds.Count || itemLotMapIndex == 0; itemLotMapIndex++) //cycles through itemLotMapids, if nessesary.
                {
                    if (IsItemLotMapDrop)
                        itemLotID = ItemLotMapIds[itemLotMapIndex];



                    Debug.Assert(keywordIndex != -1);

                    string variantID = "";
                    //string variantString = "";
                    string simplifiedNpcName = "";
                    string itemLotEnemyName = "";

                    Line itemLotLine = null;
                    ParamFile ItemLotParam = ItemLotParam_enemy;
                    if (IsItemLotMapDrop)
                        ItemLotParam = ItemLotParam_map;

                    if (itemLotID != -1)
                    {
                        itemLotLine = ItemLotParam.GetLineWithId(itemLotID);
                    }

                    if (IsItemLotMapDrop)
                    {
                        if (itemLotLine.name != "")
                        {
                            if (itemLotLine.name.StartsWith("["))
                            {
                                var endOfTag = itemLotLine.name.IndexOf("]");
                                if (endOfTag != 0)
                                    endOfTag++;
                                itemLotEnemyName = itemLotLine.name.Remove(endOfTag);
                                if (endOfTag != 0)
                                {
                                    var start = itemLotLine.name.IndexOf("-") + 2;
                                    simplifiedNpcName = itemLotLine.name.Substring(start, itemLotLine.name.IndexOf("]") - start);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (npcLine.name.Contains("("))
                        {
                            int endIndex = npcLine.name.IndexOf("(");
                            simplifiedNpcName = npcLine.name.Remove(endIndex - 1);
                        }
                        else
                        {
                            //if (npcLine.name.Contains(documentedMark)) {
                            //    var ogName = npcLine.name.Remove(npcLine.name.IndexOf(documentedMark), documentedMark.Length);
                            //    simplified =  ogName ;
                            //}
                            //else
                            {
                                simplifiedNpcName = npcLine.name;
                            }
                        }

                        {
                            if (itemLotLine != null && itemLotLine.name.Contains("]"))
                            {
                                int endIndex = itemLotLine.name.IndexOf("]") + 1;
                                itemLotEnemyName = itemLotLine.name.Remove(endIndex);
                            }
                            else
                            {
                                itemLotEnemyName = "[" + simplifiedNpcName + "]";
                            }
                        }
                    }

                    bool UniqueVariant = false; // so it doesnt look for variants.


                    string backUpVariantId = "";


                    bool isForceBossDisplay = isForceBossDisplayDict[keyword];
                    int firstTimeDropStyle = firstTimeDropDict[keyword];
                    bool isFirstTimeDrop = firstTimeDropStyle != -1;

                    if (!IsItemLotMapDrop)
                    {
                        if (itemLotID == -1)
                        {
                            variantID = "k" + keywordIndex.ToString()
                            //    +"npcID"+NpcParam.GetLineOnCondition
                            //    (new Condition.NameStartsWith(simplifiedNpcName+" (").OR(new Condition.NameIs(simplifiedNpcName)) )
                            //    .id
                            ;
                        }
                        else
                        {
                            if (!isOverrideKeywordID)
                                variantID = ("k" + keywordIndex.ToString() + "lotID" + itemLotID.ToString());
                            else
                                variantID = ("ok" + keywordIndex.ToString() + "lotID" + itemLotID.ToString());
                        }
                        if (isBoss)
                            variantID += "boss";
                        else if (isFirstTimeDrop)
                        {
                            backUpVariantId = variantID;
                            variantID += "ftd" + specialDropUniqueIndex;
                            specialDropUniqueIndex++;
                            UniqueVariant = true;
                        }
                        if (isBoss || isForceBossDisplay)
                            npcLine.SetField("dropType", 1);

                        //variantString = "(VariantID: " + variantID + ")";
                    }
                    

                    bool FTD_OnlyHighestX = firstTimeDropStyle == 3 || firstTimeDropStyle == 4 || firstTimeDropStyle == 5 || firstTimeDropStyle == 6 || firstTimeDropStyle == 8;
                    bool FTD_OnlyFirstType = firstTimeDropStyle == 5 || firstTimeDropStyle == 8;
                    bool FTD_OnlySomberType = firstTimeDropStyle == 7;
                    bool FTD_DropMultiple = firstTimeDropStyle == 5 || firstTimeDropStyle == 6; //$$$$
                    bool FTD_OnlyHighestLevel = firstTimeDropStyle == 4 || firstTimeDropStyle == 7 || firstTimeDropStyle == 8;
                    bool FTD_AvoidDropx1 = firstTimeDropStyle > 1;   //because the other system didnt work so fuck it.
                    bool FTD_SeperateLineForEachType = false;


                    //if (npcID == test)
                    //    Util.println(npcLine.id + ":" + npcLine.name + "'s itemLotID set to " + itemLotID + " valid itemLotLine:" + (itemLotLine != null).ToString());

                    //if(itemLotLine == null)
                    //{
                    //    Util.println(npcLine.id + ":" + npcLine.name + "'s itemLotID: "+ itemLotID + " couldnt be found.");
                    //    continue;
                    //}


                    int targetSmithingStoneLineID = -1;

                    const bool CanHijack = false;   //has issues right now due to.
                                                    //there exists the pssibility that a none keyworded NPC is refrencing the same item lot, so we shouldnt change it.
                                                    //We could make it work by checking if any NPCParam shares the item lot, 
                                                    //but that becoems msssy and prioritises the last one, which at that point may alreay have a proper line for it.
                                                    //we can make it work by checking all if any of the lines share share keyword, but that sounds messy.

                    //      if itemLotParam not assined a Level, then we hijack it with the level we want and moify it to our liking.
                    bool dropSmithing = isSmithingDict[keyword];
                    bool dropSomber = isSomberDict[keyword];
                    bool dropRune = isRuneDict[keyword];


                    if (isBoss && dropSmithing)                             //if we want somber to be more tied to boss fights
                    {
                        dropSomber = true;
                        FTD_SeperateLineForEachType = true;
                    }

                    if (!OneTimeWeaponAndArmorDrops)
                    {
                        if (!RUNES && !STONES)
                            continue;
                        bool dropStones = (dropSmithing || dropSomber);
                        if (!STONES && dropStones && !dropRune)
                            continue;
                        if (!RUNES && dropRune && !dropStones)
                            continue;
                    }



                    string[] smithingStoneItemsToRemove = new string[]
    {
                        "Smithing Stone [",
                        "Ancient Dragon Smithing Stone",

    };
                    string[] runeItemsToRemove = new string[]
                    {
                        "Golden Rune [",
                        "Numen Rune",
                        "Hero Rune [",
                        "Lord Rune"
                    };

                    var isArmorAdjustExceptionCond = new Condition.HasInName("Banished Knight");
                    var isNotArrowCond = new Condition.FloatFieldCompare(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), Condition.LESS_THAN, 50000000);
                    var isWeaponCond = new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemCategory02"), "2");
                    var isArmorCond = new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemCategory02"), "3");

                    var isSmithingLineCondition = new Condition.HasInName(smithingStoneItemsToRemove);
                    var isRuneLineCondition = new Condition.HasInName(runeItemsToRemove);
                    var curLineEmptyAndNoDropCondition =
                        new Condition.Either(
                            new Condition.HasInName("None"),
                            new Condition.AllOf(
                                new Condition.NameIs(""),
                                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId01"), "0"),
                                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), "0"))
                        );
                    var curLineHasOneItemCond = //naturally excludes any guarenteed drops (1000 chance_ that are narmally at lot 01.
                            new Condition.AllOf(
                            new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId01"), "0"),
                            //new Condition.FloatFieldCompare(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), Condition.GREATER_THAN, 0),
                            new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId03"), "0"));


                    int HighestIdPossible = int.MaxValue;
                    int itemLotLineMapDrop_getItemFlagId = -1;

                    List<Line> linesToReplaceOrRemove = new List<Line>();

                    List<Line> newLines = new List<Line>();
                    bool foundOneTimeDrop = false;
                    bool foundRuneDrop = false;
                    bool foundSmithingDrop = false;

                    if (IsItemLotMapDrop)
                    {
                        //hijack
                        Line curLine = itemLotLine;
                        int idAdjust = 0;

                        itemLotLine = ItemLotParam.GetLineWithId(itemLotID);
                        itemLotLineMapDrop_getItemFlagId = int.Parse(itemLotLine.GetField("getItemFlagId"));


                        bool deleteOthers = !itemLotMainLines.Contains(itemLotLine);

                        //check each itemlot in group to find smithing stone drops. 
                        //we delete those lines.
                        int curId = curLine.id_int;
                        var curLineIndex = -1;


                        bool exit = false;
                        while (true)
                        {
                            if (curLine == null)
                            {
                                targetSmithingStoneLineID = curId + idAdjust;
                                break;
                            }
                            if (deleteOthers &&
                                (curLineEmptyAndNoDropCondition.Pass(curLine) ||
                                ((dropSmithing || dropSomber) && isSmithingLineCondition.Pass(curLine)) ||
                                (dropRune && isRuneLineCondition.Pass(curLine)))
                               )
                            {
                                exit = true; // fuck it dont add if it already has one.
                                break;
                                /*
                                //Util.println("deleted " + nextID + ";"+ nextLine.name);
                                if (itemLotLine == curLine)
                                    itemLotLine = null;
                                //itemLotParam.RemoveLine(curId);
                                linesToReplaceOrRemove.Add(curLine);
                                idAdjust -= 1;
                                */
                            }
                            curId++;
                            curLine = ItemLotParam.GetLineWithId(curId, out curLineIndex, curLineIndex);
                        }
                        if (exit)
                            continue;

                        HighestIdPossible = ItemLotParam.GetNextLine(targetSmithingStoneLineID, false, curLineIndex).id_int - 2;


                        var n = ItemLotParam.paramName;
                        var nn = ItemLotParam.lines.Count();


                        if (itemLotLine != null)
                        {
                            if (!idToVariantsDict.ContainsKey(itemLotLine))
                            {
                                idToVariantsDict.Add(itemLotLine, variantID);
                                idToLevelDict.Add(itemLotLine, level);
                            }
                            //itemLotLine.SetField(1, itemLotLine.name + levelString + variantString);
                            itemLotMainLines.lines.Add(itemLotLine);
                        }
                    }
                    else if (CanHijack && itemLotLine != null && !itemLotLine.name.Contains("(Level: ") //unused
                        && NpcParam.GetLineOnCondition(
                                new Condition.FieldEqualTo(NpcParam.GetFieldIndex("itemLotId_enemy"), itemLotLine.ToString()).
                                AND(new Condition.IDCheck(npcID).IsFalse)
                            ) == null
                        )
                    {

                        //if (itemLotLine.GetField("getItemFlagId") != "0")
                        //    Util.println(itemLotLine._idName + "  flagId:" + itemLotLine.GetField("getItemFlagId"));
                        //hijack
                        Line curLine = itemLotLine;
                        int idAdjust = 0;

                        //check each itemlot in group to find smithing stone drops. 
                        //we delete those lines.
                        int nextID = curLine.id_int;
                        var curLineIndex = -1;
                        while (true)
                        {
                            if (curLine == null)
                            {
                                targetSmithingStoneLineID = nextID + idAdjust;
                                break;
                            }
                            if (
                                curLineEmptyAndNoDropCondition.Pass(curLine) ||
                                ((dropSmithing || dropSomber) && isSmithingLineCondition.Pass(curLine)) ||
                                (dropRune && isRuneLineCondition.Pass(curLine))
                               )
                            {
                                int flagID = int.Parse(curLine.GetField("getItemFlagId"));
                                //if (flagID != -1)
                                //    freedFlagIds.Add(flagID);
                                //Util.println("deleted " + nextID + ";"+ nextLine.name);
                                ItemLotParam_enemy.RemoveLine(nextID);
                                idAdjust -= 1;
                            }
                            else
                            {

                                curLine.SetField(0, curLine.id_int + idAdjust);
                                if (OneTimeWeaponAndArmorDrops && curLineHasOneItemCond.Pass(curLine))
                                {
                                    //
                                    int FlagId = -1;
                                    int OtherFlagId = -1;
                                    //OR
                                    bool createUniqueFlagId = false;
                                    bool createUniqueFlagIdForOther = false;
                                    //

                                    string addToName = "";
                                    float PercentMult = 1;

                                    int OtherToAddId = -1;
                                    float OtherToAddPercentMult = 1;
                                    int OtherCategory = 0;

                                    int _curLot2Id = int.Parse(curLine.GetField("lotItemId02"));

                                    bool isWeapon = isWeaponCond.Pass(curLine);
                                    bool isArmor = isArmorCond.Pass(curLine);
                                    bool isNotArrow = isNotArrowCond.Pass(curLine);

                                    if (isArmor)
                                    {
                                        isArmor = true;
                                        const bool ALLOW_REVERSE_ALTERED_DROP = false;
                                        OtherToAddId = _curLot2Id + 1000;
                                        var OtherToAddEquipLine = EquipParamProtector.GetLineWithId(OtherToAddId);
                                        if (OtherToAddEquipLine != null)
                                        {
                                            if (isArmorAdjustExceptionCond.Pass(OtherToAddEquipLine))
                                            {
                                                OtherToAddId = -1;
                                            }
                                        }
                                        else if (ALLOW_REVERSE_ALTERED_DROP)         // if we want to allow revers altering (this tends to always be problematic
                                        {
                                            OtherToAddId = _curLot2Id - 1000;
                                            OtherToAddEquipLine = EquipParamWeapon.GetLineWithId(OtherToAddId);
                                            if (OtherToAddEquipLine == null || isArmorAdjustExceptionCond.Pass(OtherToAddEquipLine))
                                                OtherToAddId = -1;
                                        }
                                        else
                                            OtherToAddId = -1;

                                        if (AsssignedArmorsFlagIdDict.ContainsKey(_curLot2Id))
                                            FlagId = AsssignedArmorsFlagIdDict[_curLot2Id];
                                        else
                                            createUniqueFlagId = true;

                                        if (OtherToAddId != -1)
                                        {
                                            OtherCategory = 3;
                                            if (AsssignedArmorsFlagIdDict.ContainsKey(OtherToAddId))   //fair to assume it exists
                                                OtherFlagId = AsssignedArmorsFlagIdDict[OtherToAddId];
                                            else
                                                createUniqueFlagIdForOther = true;
                                            PercentMult = OneTimeWeaponAndArmorDrops_ArmorDropChanceMult;
                                            OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult;
                                            addToName = " & " + OtherToAddEquipLine.name;
                                        }
                                    }
                                    else if (isWeapon && isNotArrow)
                                    {
                                        isWeapon = true;
                                        PercentMult = OneTimeWeaponAndArmorDrops_WeaponDropChanceMult;
                                        OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult;
                                        if (AsssignedWeaponsFlagIdDict.ContainsKey(_curLot2Id))
                                            FlagId = AsssignedWeaponsFlagIdDict[_curLot2Id];
                                        else
                                            createUniqueFlagId = true;
                                        //createUniqueFlagIdForOther = true; //if we dont want to drop infinitly
                                        OtherCategory = 2;
                                        OtherToAddId = _curLot2Id;
                                    }
                                    int _curLot2Percent = int.Parse(curLine.GetField("lotItemBasePoint02"));
                                    if (OtherToAddId != -1)
                                    {
                                        curLine.SetField("lotItemId03", OtherToAddId);
                                        curLine.SetField("lotItemNum03", 1);
                                        curLine.SetField("enableLuck03", 1);
                                        curLine.SetField("lotItemCategory03", OtherCategory);
                                        int OtherPercent = Math.Max(1, (int)Math.Round(_curLot2Percent * OtherToAddPercentMult));
                                        curLine.SetField("lotItemBasePoint03", Math.Min(34463,OtherPercent));
                                        int _curLot1Percent = int.Parse(curLine.GetField("lotItemBasePoint01"));
                                        curLine.SetField("lotItemBasePoint01", Math.Max(1, Math.Min(34463, _curLot1Percent - OtherPercent)));
                                    }
                                    curLine.SetField(1, curLine.name + addToName);
                                    if (PercentMult != -1)
                                    {
                                        int NewPercent = Math.Max(1, (int)Math.Round(_curLot2Percent * PercentMult));
                                        int addedAmount = NewPercent - _curLot2Percent;
                                        curLine.SetField("lotItemBasePoint02", Math.Min(34463, NewPercent));
                                        int _curLot1Percent = int.Parse(curLine.GetField("lotItemBasePoint01"));
                                        curLine.SetField("lotItemBasePoint01", Math.Min(34463, Math.Max(1, _curLot1Percent - addedAmount)));
                                    }

                                    
                                    if (FlagId != -1)
                                    {
                                        if (OtherToAddId == -1)
                                            curLine.SetField("getItemFlagId", FlagId);
                                        else
                                            curLine.SetField("getItemFlagId02", FlagId);
                                    }
                                    else if (createUniqueFlagId)
                                    {
                                        int currentGetItemFlagId = IntFilter.GetRandomInt(npcID, OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                                        usedGetItemFlagId.Add(currentGetItemFlagId);

                                        if (isArmor)
                                            AsssignedArmorsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);
                                        else if (isWeapon)
                                            AsssignedWeaponsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);

                                        if (OtherToAddId == -1)
                                        {
                                            curLine.SetField("getItemFlagId", currentGetItemFlagId);
                                            //line.SetField("canExecByFriendlyGhost", "0");
                                        }
                                        else
                                            curLine.SetField("getItemFlagId02", currentGetItemFlagId);
                                    }
                                    if (OtherFlagId != -1)
                                        curLine.SetField("getItemFlagId02", OtherFlagId);
                                    else if (createUniqueFlagIdForOther)
                                    {
                                        int currentGetItemFlagId = IntFilter.GetRandomInt(npcID, OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                                        usedGetItemFlagId.Add(currentGetItemFlagId);
                                        if (isArmor)
                                            AsssignedArmorsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                                        else if (isWeapon)
                                            AsssignedWeaponsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                                        curLine.SetField("getItemFlagId03", currentGetItemFlagId);
                                    }
                                }
                            }
                            nextID++;
                            curLine = ItemLotParam_enemy.GetLineWithId(nextID, out curLineIndex, curLineIndex);
                        }

                        HighestIdPossible = ItemLotParam.GetNextLine(targetSmithingStoneLineID, false, curLineIndex).id_int - 2;
                        itemLotLine = ItemLotParam_enemy.GetLineWithId(itemLotID);


                        if (itemLotLine != null)
                        {
                            if (!idToVariantsDict.ContainsKey(itemLotLine))
                            {
                                idToVariantsDict.Add(itemLotLine, variantID);
                                idToLevelDict.Add(itemLotLine, level);
                            }
                            //itemLotLine.SetField(1, itemLotLine.name + levelString + variantString);
                            itemLotMainLines.lines.Add(itemLotLine);
                        }
                        //npcLine.MarkModified(false);    //just for now.

                        //if we find them change the item lot to the specifc smithing stones we want. chanece and all

                        //if not then add a new line.
                    }
                    else
                    { //if it assigned a level

                        //look for "proper" item lotParam that has same variantID and same difficulty.
                        if (!UniqueVariant)
                        {
                            Line properItemLotLineFound = null;
                            //Condition isProperItemLotCondition =
                            //    new Condition.HasInName(levelString).AND(
                            //    new Condition.HasInName(variantString));

                            //if (itemLotLine != null && isProperItemLotCondition.Pass(itemLotLine))
                            if (itemLotLine != null && idToLevelDict.ContainsKey(itemLotLine) && idToVariantsDict.ContainsKey(itemLotLine) && idToLevelDict[itemLotLine] == level && idToVariantsDict[itemLotLine] == variantID)
                            {
                                //npcLine.MarkModified(false); //temp just so its considered modified
                                if (npcID == test)
                                    Util.println(npcLine._idName + "   SUCCESS" + "    already had proper" + itemLotLine.id);
                                continue;
                            }
                            else
                            {
                                //properItemLotLineFound = itemLotMainLines.GetLineOnCondition(isProperItemLotCondition);
                                foreach(var ilml in itemLotMainLines.lines)
                                {
                                    if(idToLevelDict.ContainsKey(ilml) && idToVariantsDict.ContainsKey(ilml) && idToLevelDict[ilml] == level && idToVariantsDict[ilml] == variantID)
                                    {
                                        properItemLotLineFound = ilml;
                                        break;
                                    }
                                }
                            }

                            //if we find one assign it.

                            //if (npcID == test)
                            //    Util.println("properItemLotLine Found: " + (properItemLotLineFound != null).ToString() + "   size = "+ itemLotMainLines.Length);;

                            if (properItemLotLineFound != null)
                            {
                                npcLine.SetField("itemLotId_enemy", properItemLotLineFound.id);
                                if (npcID == test)
                                    Util.println(npcLine._idName + "   SUCCESS" + "    found proper" + properItemLotLineFound.id);
                                continue;
                            }
                        }

                        // proper item lot not found.  so instead we will create one.


                        //find an ID to use as the start to search for a location to create it.
                        int startingSearchForLocationItemLotID = -1;
                        if (itemLotLine == null)
                        {
                            if (mostMatchedLine != null)
                            {
                                startingSearchForLocationItemLotID = mostMatchedLine.id_int;
                            }
                            else
                            {

                                //Line l = ItemLotParam_enemy.GetLineOnCondition(new Condition.HasInName(simplifiedKeyword));

                                string target = npcLine.name;

                                //string target = simplifiedKeyword;
                                string[] targetWords = target.Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                                var importantWords = targetWords;
                                if (target.Contains("("))
                                {
                                    importantWords = target.Remove(target.IndexOf("(")).Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                                }

                                int maxMatchCount = 0;
                                var mostMatchedLines = LineFunctions.GetOrderedWordMatchedNestedLines(targetWords, ItemLotParam_enemy.lines, out maxMatchCount, out int maxScore, 1, importantWords, false, null, "[", "]");

                                if (mostMatchedLines.Count > 0 && mostMatchedLines[0].Count > 0)
                                    mostMatchedLine = mostMatchedLines[0][0];


                                if (mostMatchedLine != null)
                                {
                                    startingSearchForLocationItemLotID = mostMatchedLine.id_int;
                                }
                                else
                                {
                                    //target = simplifiedKeyword;
                                    //targetWords = target.Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);

                                    maxMatchCount = 0;
                                    List<Line> orderedLines = LineFunctions.GetOrderedWordMatchedLines(targetWords, NpcParam.lines, out maxMatchCount, out maxScore, 1, importantWords, true);
                                    foreach (Line l in orderedLines)
                                    {
                                        int foundID = int.Parse(l.GetField("itemLotId_enemy"));
                                        if (foundID != -1)
                                        {
                                            startingSearchForLocationItemLotID = foundID;
                                            break;
                                        }
                                    }
                                    if (startingSearchForLocationItemLotID == -1)
                                    {
                                        Debug.WriteLine("still didnt find starting ID. TARGET: " + target + "  mmc" + maxMatchCount);
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            startingSearchForLocationItemLotID = itemLotLine.id_int;
                        }
                        //if (npcID == test)
                        //    Util.println("startingSearchFOrLocationItemLotID: " + startingSearchForLocationItemLotID + "   " + npcLine.id + ":" + npcLine.name );

                        //now we search down for a free ID spot to create it.
                        //looks for free 100s place
                        int newItemLotID = startingSearchForLocationItemLotID;
                        {
                            int newNextLineID = -1;
                            //if(newItemLotID == -1)
                            //Util.println("newItemLotID: " + newItemLotID + npcLine.id + ":" + npcLine.name + "  " + newItemLotID);
                            int nextLineIndex = 0;
                            int newLineIndex = 0;
                            //find new line ID
                            while (ItemLotParam_enemy.GetLineWithId(newItemLotID, out newLineIndex, newLineIndex) != null || newNextLineID < newItemLotID + 100)
                            {
                                newItemLotID = ItemLotParam_enemy.GetNextFreeId(newItemLotID, out newLineIndex);
                                newItemLotID += 100 - (newItemLotID % 100);
                                newNextLineID = ItemLotParam_enemy.GetNextLine(newItemLotID, out nextLineIndex, nextLineIndex).id_int;
                                /*Console.WriteLine(npcLine._idName +
                                    Util.IndentedText("start:" + startingSearchForLocationItemLotID, 25) +
                                    Util.IndentedText("newItemLotID:" + newItemLotID, 25) +
                                    Util.IndentedText("   newNextLineID:" + newNextLineID, 25));
                                    */
                            }
                        }

                        //now we loop and copy to create our line.
                        int currentLineToCopyID = itemLotID;
                        int currentNewLineCopyID = newItemLotID;
                        //copy lines from itemLotLine to the new itemLotID location

                        var curLineIndex = -1;
                        while (true)
                        {

                            Line curLine = ItemLotParam_enemy.GetLineWithId(currentLineToCopyID, out curLineIndex, curLineIndex);

                            if (curLine == null)
                            {
                                targetSmithingStoneLineID = currentNewLineCopyID;
                                break;
                            }
                            if (
                                curLineEmptyAndNoDropCondition.Pass(curLine) ||
                                ((dropSmithing || dropSomber) && isSmithingLineCondition.Pass(curLine)) ||
                                (dropRune && isRuneLineCondition.Pass(curLine))
                               )
                            {

                                /*int flagID = int.Parse(curLine.GetField("getItemFlagId"));
                                if (flagID != 0 && flagID != -1)
                                {
                                    //Util.println(flagID + "  " + curLine._idName);
                                    freedFlagIds.Add(flagID);
                                }*/
                                currentNewLineCopyID--;
                            }
                            else
                            {
                                Line copy = ItemLotParam_enemy.GetLineWithId(currentLineToCopyID).Copy().SetField(0, currentNewLineCopyID);
                                newLines.Add(copy);
                                //ItemLotParam_enemy.OverrideOrAddLine(copy);
                                if (OneTimeWeaponAndArmorDrops && curLineHasOneItemCond.Pass(copy))
                                {
                                    var xxxx = copy.GetField("lotItemCategory02");
                                    //
                                    int FlagId = -1;
                                    int OtherFlagId = -1;
                                    //OR
                                    bool createUniqueFlagId = false;
                                    bool createUniqueFlagIdForOther = false;
                                    //

                                    string addToName = "";
                                    float PercentMult = 1;

                                    int OtherToAddId = -1;
                                    float OtherToAddPercentMult = 1;
                                    int OtherCategory = 0;

                                    int _curLot2Id = int.Parse(copy.GetField("lotItemId02"));

                                    bool isWeapon = isWeaponCond.Pass(copy);
                                    bool isArmor = isArmorCond.Pass(copy);
                                    RunSettings.Testing_FunctionDebug = true;
                                    bool isNotArrow = isNotArrowCond.Pass(curLine);
                                    RunSettings.Testing_FunctionDebug = false;

                                    if (isArmor)
                                    {
                                        foundOneTimeDrop = true;
                                        isArmor = true;
                                        const bool ALLOW_REVERSE_ALTERED_DROP = false;
                                        OtherToAddId = _curLot2Id + 1000;
                                        var OtherToAddEquipLine = EquipParamProtector.GetLineWithId(OtherToAddId);
                                        if (OtherToAddEquipLine != null)
                                        {
                                            if (isArmorAdjustExceptionCond.Pass(OtherToAddEquipLine))
                                            {
                                                OtherToAddId = -1;
                                            }
                                        }
                                        else if (ALLOW_REVERSE_ALTERED_DROP)         // if we want to allow revers altering (this tends to always be problematic
                                        {
                                            OtherToAddId = _curLot2Id - 1000;
                                            OtherToAddEquipLine = EquipParamWeapon.GetLineWithId(OtherToAddId);
                                            if (OtherToAddEquipLine == null || isArmorAdjustExceptionCond.Pass(OtherToAddEquipLine))
                                                OtherToAddId = -1;
                                        }
                                        else
                                            OtherToAddId = -1;

                                        if (AsssignedArmorsFlagIdDict.ContainsKey(_curLot2Id))
                                            FlagId = AsssignedArmorsFlagIdDict[_curLot2Id];
                                        else
                                            createUniqueFlagId = true;

                                        if (OtherToAddId != -1)
                                        {
                                            OtherCategory = 3;
                                            if (AsssignedArmorsFlagIdDict.ContainsKey(OtherToAddId))   //fair to assume it exists
                                                OtherFlagId = AsssignedArmorsFlagIdDict[OtherToAddId];
                                            else
                                                createUniqueFlagIdForOther = true;
                                            PercentMult = OneTimeWeaponAndArmorDrops_ArmorDropChanceMult;
                                            OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult;
                                            addToName = " & " + OtherToAddEquipLine.name;
                                        }
                                    }
                                    else if (isWeapon && isNotArrow)
                                    {
                                        foundOneTimeDrop = true;
                                        isWeapon = true;
                                        PercentMult = OneTimeWeaponAndArmorDrops_WeaponDropChanceMult;
                                        OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult;
                                        if (AsssignedWeaponsFlagIdDict.ContainsKey(_curLot2Id))
                                            FlagId = AsssignedWeaponsFlagIdDict[_curLot2Id];
                                        else
                                            createUniqueFlagId = true;
                                        //createUniqueFlagIdForOther = true; //if we dont want to drop infinitly
                                        OtherCategory = 2;
                                        OtherToAddId = _curLot2Id;
                                    }
                                    int _curLot2Percent = int.Parse(copy.GetField("lotItemBasePoint02"));
                                    if (OtherToAddId != -1)
                                    {
                                        copy.SetField("lotItemId03", OtherToAddId);
                                        copy.SetField("lotItemNum03", 1);
                                        copy.SetField("enableLuck03", 1);
                                        copy.SetField("lotItemCategory03", OtherCategory);
                                        int OtherPercent = Math.Max(1, (int)Math.Round(_curLot2Percent * OtherToAddPercentMult));
                                        copy.SetField("lotItemBasePoint03", Math.Min(34463, OtherPercent));
                                        int _curLot1Percent = int.Parse(copy.GetField("lotItemBasePoint01"));
                                        copy.SetField("lotItemBasePoint01", Math.Min(34463, Math.Max(1, _curLot1Percent - OtherPercent)));
                                    }
                                    copy.SetField(1, copy.name + addToName);
                                    if (PercentMult != -1)
                                    {
                                        int NewPercent = Math.Max(1, (int)Math.Round(_curLot2Percent * PercentMult));
                                        int addedAmount = NewPercent - _curLot2Percent;
                                        copy.SetField("lotItemBasePoint02", Math.Min(34463, NewPercent));
                                        int _curLot1Percent = int.Parse(copy.GetField("lotItemBasePoint01"));
                                        copy.SetField("lotItemBasePoint01", Math.Min(34463, Math.Max(1, _curLot1Percent - addedAmount)));
                                    }
                                    if (FlagId != -1)
                                    {
                                        if (OtherToAddId == -1)
                                            copy.SetField("getItemFlagId", FlagId);
                                        else
                                            copy.SetField("getItemFlagId02", FlagId);
                                    }
                                    else if (createUniqueFlagId)
                                    {
                                        int currentGetItemFlagId = IntFilter.GetRandomInt(npcID, OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                                        usedGetItemFlagId.Add(currentGetItemFlagId);

                                        if (isArmor)
                                            AsssignedArmorsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);
                                        else if (isWeapon)
                                            AsssignedWeaponsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);

                                        if (OtherToAddId == -1)
                                        {
                                            copy.SetField("getItemFlagId", currentGetItemFlagId);
                                            //line.SetField("canExecByFriendlyGhost", "0");
                                        }
                                        else
                                            copy.SetField("getItemFlagId02", currentGetItemFlagId);
                                    }
                                    if (OtherFlagId != -1)
                                        copy.SetField("getItemFlagId02", OtherFlagId);
                                    else if (createUniqueFlagIdForOther)
                                    {
                                        int currentGetItemFlagId = IntFilter.GetRandomInt(npcID, OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                                        usedGetItemFlagId.Add(currentGetItemFlagId);
                                        if (isArmor)
                                            AsssignedArmorsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                                        else if (isWeapon)
                                            AsssignedWeaponsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                                        copy.SetField("getItemFlagId03", currentGetItemFlagId);
                                    }
                                }
                            }
                            currentLineToCopyID++;
                            currentNewLineCopyID++;
                        }

                            

                        Debug.Assert(newItemLotID != -1, npcLine._idName);

                        //if (npcID == test)
                        //    Util.println("newItemLotID: "+newItemLotID+"   "+npcLine.id+":"+npcLine.name+"   " +newItemLotID);

                        itemLotID = newItemLotID;

                        npcLine.SetField("itemLotId_enemy", itemLotID);
                        HighestIdPossible = ItemLotParam_enemy.GetNextLine(targetSmithingStoneLineID, false, curLineIndex).id_int - 2;
                        itemLotLine = ItemLotParam_enemy.GetLineWithId(itemLotID);

                        if (itemLotLine != null)
                        {
                            //if (!idToVariantsDict.ContainsKey[itemLotLine]) //should always be true
                            {
                                idToVariantsDict.Add(itemLotLine, variantID);
                                idToLevelDict.Add(itemLotLine, level);
                            }
                            //itemLotLine.SetField(1, itemLotLine.name + levelString + variantString);
                            itemLotMainLines.lines.Add(itemLotLine);
                        }

                    }

                    //create smithing stone line.

                    if (npcID == test)
                        Util.println(npcLine._idName + "   SUCCESS" + "    making proper" + itemLotID);

                    float percentChance = percentNumDict[keyword] * 10;   //100 chance to 1000 item lot chance

                    if (npcID == test)
                        Util.p();

                    int[] xAmounts = amountMultsDict[keyword];
                    
                    float XasscadeMultToX = XasscadeMultToXDict[keyword];
                    float XasscadePowToX = XasscadePowToXDict[keyword];

                    const bool ableToAvoidUnessasryFTD = false;


                    bool isBoth = false;
                    if (dropSomber && dropSmithing)
                        isBoth = true;

                    float adjustedLevel = level;
                    float mult = levelMultDict[keyword];
                    adjustedLevel *= mult;
                    float adj = levelAdjDict[keyword];
                    adjustedLevel += adj;

                    {   //level max clamp
                        float levelClamp = levelClampDict[keyword];
                        if (levelClamp != -1 && adjustedLevel > levelClamp)
                            adjustedLevel = levelClamp;
                        if (adjustedLevel < 1)
                            adjustedLevel = 1; //makes foot soldiers and imps still useful in the beginning.

                        //if (isBoss) //bosses cant be decimal levels.
                        //    adjustedLevel = (int)(adjustedLevel + 0.7f);
                    }



                    float levelToChanceMult = levelToChanceMultDict[keyword];
                    float casscadeLevelToChanceMult = casscadeLevelToChanceMultDict[keyword];

                    int levelCasscade = levelCasscadeDict[keyword];


                    float DROPMULTamountMult = DROPMULT;
                    float DROPMULTpercentMult = DROPMULT;

                    {
                        float effectivePercent = percentChance * (levelCasscade + 1);
                        DROPMULTpercentMult = Math.Min(1000 / effectivePercent, DROPMULTpercentMult);
                        float MaxPercent = (effectivePercent * 1.5f) + (effectivePercent * 0.25f * DROPMULT);
                        float MaxPercentMult = MaxPercent / effectivePercent;
                        DROPMULTpercentMult = Math.Min(MaxPercentMult, DROPMULTpercentMult);
                        DROPMULTamountMult = 1 + ((DROPMULT - 1) - (DROPMULTpercentMult - 1));
                        if (test == npcID)
                            Util.p();
                    }
                    //percentChance *= DROPMULTpercentMult; done way later.

                    int lotIndex = 2;
                    Line stoneLine = null;
                    int lotPercentTotal = 0;
                    int curTotalPercent = 0;
                    //int lotPercentTotalExcess = 0; //unused
                    Dictionary<int, int> lotToChanceDict = new Dictionary<int, int>();

                    int backupItem_PercentChanceInt = -1;
                    int backupItem_itemID = -1;
                    int backupItem_Amount = -1;
                    bool backupItem_IsLuck = false;

                    int backupItem2_PercentChanceInt = -1;
                    int backupItem2_itemID = -1;
                    int backupItem2_Amount = -1;
                    bool backupItem2_IsLuck = false;

                    bool donePostHawkFix = false;

                    //bool createNewLineForSpecialDrop = false;

                    bool emptyLotReplaced = false;

                    int d = 0;
                    int dmax = 0;
                    if (isFirstTimeDrop)
                        dmax = 1;


                    int curMaxLotIndex = maxLotIndex;
                    if (d == 0 && dmax == 1)
                        curMaxLotIndex--;//we need to save one slot for our first time empty.

                    List<Line> curStoneLinesToAddGetItemFlagId = new List<Line>();
                    List<int> curStoneLinesAddItemFlagIdLots = new List<int>();

                    bool treatAsBoss = isBoss;

                    if (IsItemLotMapDrop &&
                        (
                        isFirstTimeDrop
                        //|| isForceBossDisplay
                        )
                        )
                    {
                        treatAsBoss = true;
                        //isBoss = true; //treat as a boss. THis avoids poinless FTD entries, and allows for 1 time drops.
                    }

                    for (; d <= dmax; d++)
                    {

                        //if(createNewLineForSpecialDrop)
                        //{

                        //posthok fix
                        if (stoneLine != null && !donePostHawkFix && curTotalPercent > 1000)
                        {
                            float postHok_amountMult = (curTotalPercent / 1000f);
                            var keys = lotToChanceDict.Keys.ToArray();
                            for (int i = 0; i < keys.Length; i++)
                            {
                                stoneLine.OperateField("lotItemNum0" + keys[i], new OperateIntField(Operation.MULTIPLY, postHok_amountMult));
                                if (test == npcID)
                                    Util.p();
                            }
                            donePostHawkFix = true;
                        }

                        donePostHawkFix = false;
                        emptyLotReplaced = false;

                        lotIndex = 2;
                        stoneLine = null;
                        lotToChanceDict.Clear();
                        lotPercentTotal = 0;
                        //lotPercentTotalExcess = 0;

                        backupItem_PercentChanceInt = -1;
                        backupItem_itemID = -1;
                        backupItem_Amount = -1;
                        backupItem_IsLuck = false;

                        backupItem2_PercentChanceInt = -1;
                        backupItem2_itemID = -1;
                        backupItem2_Amount = -1;
                        backupItem2_IsLuck = false;

                        //}

                        float currentFirstTimeLevelAdj = firstTimeDropAdjDict[keyword];

                        bool isDecimalLevel = false;
                        bool useSingleLine = false;



                        float curAdjustedLevel = adjustedLevel;
                        float curPercentChance = percentChance;


                        bool curGiveUniqueItemFlagID = false;
                        bool currentlyFirstDropGuarentee = d == 1;





                        if (currentlyFirstDropGuarentee || treatAsBoss)       // we made the firstTImeAdj also aply to bosses. Cuz why not. ALso helps us with making zamor better.
                            curAdjustedLevel = adjustedLevel + currentFirstTimeLevelAdj;

                        if (treatAsBoss)
                        {
                            currentlyFirstDropGuarentee = false;
                            dmax = 0;
                        }
                        else if (currentlyFirstDropGuarentee)
                            curGiveUniqueItemFlagID = true;



                        if (treatAsBoss || currentlyFirstDropGuarentee)
                        {
                            float bossPercentDiv = 1;
                            if (FTD_DropMultiple) //this style intnetionally drops multiple.
                                useSingleLine = false;
                            else
                            {
                                useSingleLine = true;
                                bossPercentDiv = (levelCasscade + 1);
                            }

                            if (test == npcID)
                                Util.p();

                            //if (createNewLineForSpecialDrop)
                            //{
                            lotIndex = 1;
                            emptyLotReplaced = true;

                            if (curPercentChance != 0 || isFirstTimeDrop)
                                curPercentChance = 1000/bossPercentDiv;    //doesnt really cause it to work how we want but whaterver ts fine.
                                                            //}
                                                            //else
                                                            //{
                                                            //    curPercentChance *= 100;
                                                            //}
                        }
                        else
                        {
                            useSingleLine = oneLineDict[keyword];
                            lotIndex = 2;
                        }



                        if (npcID == test)
                            Util.println("useSingleLine:" + useSingleLine + "   currentlyFirstDropGuarentee:" + currentlyFirstDropGuarentee + "   d:" + d + "   dmax:" + dmax);






                        //int debugNoFinalPercent = 0;
                        int typeIndex = 0;
                        if ((treatAsBoss || currentlyFirstDropGuarentee) && FTD_OnlySomberType)
                            typeIndex = 1;


                        for (; typeIndex <= 2; typeIndex++)
                        {
                            bool createNewLineforThisType = (treatAsBoss || currentlyFirstDropGuarentee) && FTD_SeperateLineForEachType;

                            if (typeIndex == 0 && (!STONES || !dropSmithing))
                                continue;
                            if (typeIndex == 1 && (!STONES || !dropSomber))
                                continue;
                            if (typeIndex == 2 && (!RUNES  || !dropRune ))
                                continue;
                            float curTypeAdjustedLevel = curAdjustedLevel;
                            if (typeIndex == 1)
                            {
                                float somberMult = 10f / 9f;
                                curTypeAdjustedLevel *= somberMult;

                                curTypeAdjustedLevel += somberLevelAdjDict[keyword];

                                if (curTypeAdjustedLevel < 1)
                                    curTypeAdjustedLevel = 1; //makes trolls still drop the chance.

                                //if (30101172 == npcID)
                                Util.p();
                                //if (30101451 == npcID)
                                    Util.p();
                            }

                            if (curTypeAdjustedLevel % 1 != 0)
                                isDecimalLevel = true;
                            
                            levelCasscade = levelCasscadeDict[keyword];
                            int Xasscade = XasscadeDict[keyword];

                            if (isDecimalLevel) {
                                if (isForceBossDisplay || treatAsBoss || currentlyFirstDropGuarentee) //rounds to start level
                                {
                                    curTypeAdjustedLevel = (int)(curTypeAdjustedLevel + 0.65f);
                                    isDecimalLevel = false;
                                }
                                else if (typeIndex == 1) {
                                    if(curTypeAdjustedLevel % 1 < 0.225f || curTypeAdjustedLevel % 1 > 0.775f)
                                    {
                                        curTypeAdjustedLevel = (int)(curTypeAdjustedLevel + 0.5);
                                        isDecimalLevel = false;
                                    }
                                }
                                else if (curTypeAdjustedLevel % 1 < 0.15f || curTypeAdjustedLevel % 1 > 0.85f)
                                {
                                    curTypeAdjustedLevel = (int)(curTypeAdjustedLevel + 0.5);
                                    isDecimalLevel = false;
                                }
                            }

                            //for consistant boss drops
                            if (treatAsBoss)
                                Xasscade = 0; //we want boss drops to be consistent

                            else if (currentlyFirstDropGuarentee && FTD_OnlyHighestX)   //for FTD
                                Xasscade = 0;
                            if (treatAsBoss && useSingleLine) //we want boss drops to be consistent
                                levelCasscade = 0;
                           
                            int startLevel;
                            if (!isDecimalLevel)
                                startLevel = (int)(curTypeAdjustedLevel);
                            else
                                startLevel = (int)(curTypeAdjustedLevel + 1);

                            //reset but used before
                            bool useSingleLineOnce = false;

                            float startLevelPercentMult = 1;
                            float lastLevelPercentMult = 1;
                            if (isDecimalLevel)
                            {
                                startLevelPercentMult = Math.Abs(curTypeAdjustedLevel) % 1;    // level 1.96 -> 0.96
                                if (levelCasscade == 0)
                                {
                                    //useSingleLine = true;
                                    createNewLineforThisType = true; // has to create a singleLine Tis type.
                                    useSingleLineOnce = true;   //so you still can only get one level of drop per kill.
                                }
                                if (levelCasscade != -1)
                                    levelCasscade++;
                                lastLevelPercentMult *= 1 - startLevelPercentMult;    //level 0.04
                            }

                            if (npcID == test)
                                Util.println("level:" + level + "   curTypeAdjustedLevel:" + curTypeAdjustedLevel + "   startLevelPercentMult:" + startLevelPercentMult + "   secondLevelPercentMult:" + lastLevelPercentMult + " levelCascade" + levelCasscade + "    " + keyword);

                            int minLevel = startLevel;
                            if (levelCasscade == -1)
                            {
                                minLevel = 1;
                            }
                            else
                            {
                                minLevel = startLevel - levelCasscade;
                            }
                            float curTypePercentChance = curPercentChance;

                            if (test == npcID)
                                Util.p();

                            if (startLevel <= 0)    // pushes up levels but reduces chance. 
                                                    //this shouldnt happen to regular smithing stones due to them being clamped to 1. 
                                                    //Maybe i sohould do the same clamping after somber adjust. Ok i did.
                            {
                                //string pre = "sl:" + startLevel + "  curPercentChance:" + curPercentChance;
                                curTypePercentChance = curTypePercentChance / ((Math.Abs(startLevel) * 4f) + 6f);
                                minLevel += 1 - startLevel;   //pushes up minLevel cascades stays the same.
                                startLevel = 1;
                                //Util.println(Util.IndentedText( npcLine._idName+" |  ", 50)+pre+ " -> " + curPercentChance);
                            }


                            

                            if (test == npcID)
                                Util.p();

                            if (isBoth)   //if is both reduce chance for both
                            {
                                if (typeIndex == 0)
                                    curTypePercentChance = curPercentChance * ((100f - bothPercentSplitForSomberDict[keyword]) / 100f);
                                else if (typeIndex == 1)
                                    curTypePercentChance = curPercentChance * (bothPercentSplitForSomberDict[keyword] / 100f);
                            }

                            if (test == npcID)
                                Util.p();

                            float startLevelPercent = curTypePercentChance;
                            int typeLevelCount = dropTypesIDsDictionaries[typeIndex].Count;

                            bool canDropAncient = false;

                            int ancientDropXAmount = Math.Max(1, startLevel / 100);

                            if (typeIndex < 2 && startLevel >= typeLevelCount)   //ancient somber modifier, only  sombers and smithing, not runes.
                            {
                                if (startLevel >= typeLevelCount + 1 && isDecimalLevel)    //if poking past ancient level, we dont care about decimal passes anymore.
                                {
                                    startLevelPercentMult = 1;
                                    lastLevelPercentMult = 1;
                                    isDecimalLevel = false;
                                    if (levelCasscade != -1)
                                        minLevel++;
                                }

                                float excessLevelMult = 1 + (1f * (startLevel - typeLevelCount));
                                startLevelPercent = (float)Math.Max(Math.Round(0.025 * startLevelPercent * excessLevelMult), 1);

                                canDropAncient = true;

                                if (startLevelPercent <= 1000)
                                    minLevel = typeLevelCount - (startLevel - minLevel + 1); //allows the dropping of one level lower. increases cascade by 1.
                                else
                                    minLevel = typeLevelCount - (startLevel - minLevel); //realigns the min level. keeping it the same, in essence.

                                startLevel = typeLevelCount;

                                //chance to get final level (if available).


                                //if (targetSmithingStoneLineID == 451072101)
                                //    Util.println();

                            }
                            if (test == npcID)
                                Util.p();
                            float curTypelvl1Mult = 1;
                            if (minLevel <= 0)
                            {
                                float casscadePoolingMult = casscadePoolingMultDict[keyword];
                                for (int negativeLevel = 0; negativeLevel >= minLevel; negativeLevel--)
                                {

                                    int stepsBelow = 1 - negativeLevel;
                                    float percentMult = 1;
                                    float casscadeLevelMult = (float)Math.Pow(casscadeLevelToChanceMult, stepsBelow);
                                    //float lvlBelow1Mult = (1 / ((Math.Abs(lvlBelow1) * 4f) + 6f));
                                    //float lvlBelow1Mult = (1 / ((Math.Abs(lvlBelow1) * 1f) + 2f));
                                    float lvlBelow1Mult = 1;
                                    //float lvlBelow1Mult = 0.75;
                                    float curCasscadePoolingMult = casscadePoolingMult;

                                    if (negativeLevel == minLevel)
                                    {
                                        percentMult = lastLevelPercentMult;
                                        if (isDecimalLevel)
                                        {
                                            curCasscadePoolingMult = 1;
                                            casscadeLevelMult = 1;
                                            lvlBelow1Mult = 1;  //we ignore any non important mult.
                                        }
                                    }

                                    curTypelvl1Mult += lvlBelow1Mult * percentMult * casscadeLevelMult * curCasscadePoolingMult;
                                    if (test == npcID)
                                        Util.p();
                                }
                            }

                            startLevelPercent = startLevelPercent * startLevelPercentMult;
                            float lastLevelPercent = curTypePercentChance * lastLevelPercentMult;


                            int casscadeIndex = -1;


                            int lastCasscadeXAmount = -1;

                            for (int curLevel = startLevel; curLevel >= minLevel && curLevel > 0; curLevel--)
                            {
                                casscadeIndex++;
                                bool outOfLots = false;
                                if  ((useSingleLine || useSingleLineOnce)&& lotIndex > maxLotIndex)
                                {
                                    outOfLots = true;
                                    Util.println(//stoneLine._idName + 
                                        " out of itemlot slots!!!! " + keyword + "       npcID:" + npcID);
                                    //break;
                                }

                                float curLevelPercentChance = curTypePercentChance;
                                bool canUseLuck = !treatAsBoss && !currentlyFirstDropGuarentee;
                                bool isAncient = false;
                                //int curAmountMult = amountMult;
                                bool canDropXAmount = true;
                                if (curLevel == startLevel)
                                {
                                    curLevelPercentChance = startLevelPercent;
                                    if (canDropAncient)
                                    {
                                        canUseLuck = false;
                                        canDropXAmount = false;
                                        isAncient = true;
                                    }
                                    //curAmountMult = 1;
                                }
                                else if (curLevel == minLevel)   //this only occurs if minlevel is accually reachable. if its bellow 1 the other system (above kicks in)
                                {
                                    curLevelPercentChance = lastLevelPercent;
                                }


                                if (curLevel == 1)
                                {
                                    //string pre = "lastLevelMult:"+lastLevelMult+" x curLevelPercentChance:"+curLevelPercentChance;
                                    curLevelPercentChance *= curTypelvl1Mult;
                                    //if(lastLevelMult != 1) Util.println(Util.IndentedText(npcLine._idName +" lvl:" + level+" cas:"+levelCasscade+ " |  ", 50) + pre + " -> " + curLevelPercentChance);
                                }
                                if (!(curLevel == minLevel && isDecimalLevel)) //last dicimal level is excepmt from further manipulation.
                                {
                                    //curLevelPercentChance *= (float)Math.Pow(levelToChanceMult, curLevel - 1);    //-1 so it doesnt effect level 1
                                    if (levelToChanceMult != 1)
                                        Util.p();

                                    curLevelPercentChance += curLevelPercentChance * (levelToChanceMult - 1) * (curLevel - 1);    //-1 so it doesnt effect level 1 //multiplicative.
                                    if (canDropAncient)
                                        curLevelPercentChance *= (float)Math.Pow(casscadeLevelToChanceMult, Math.Max(casscadeIndex - 1, 0));   //if it can drop an ancient it wont effect other drop rates.
                                    else
                                        curLevelPercentChance *= (float)Math.Pow(casscadeLevelToChanceMult, casscadeIndex);
                                }

                                const bool SomberNoXAmount = true;

                                if (SomberNoXAmount && isBoth && typeIndex == 1)
                                    canDropXAmount = false;

                                int curCasscadeXAmount = 1;
                                {
                                    int xAmountCasscadeIndex = casscadeIndex;
                                    if (canDropAncient)
                                        xAmountCasscadeIndex--;
                                    if (isDecimalLevel)
                                        xAmountCasscadeIndex = Math.Max(0, xAmountCasscadeIndex - 1);
                                    // if is decimal, the second casscade and use the first xAmount. (which is added for decimal system.)
                                    if (canDropXAmount && xAmounts.Length > xAmountCasscadeIndex)
                                        curCasscadeXAmount = xAmounts[xAmountCasscadeIndex];
                                    else if (lastCasscadeXAmount != -1)   //if not assigned it steals previous casscadeAmountMult
                                        curCasscadeXAmount = lastCasscadeXAmount;
                                    lastCasscadeXAmount = curCasscadeXAmount;
                                    if (isAncient)
                                        curCasscadeXAmount = ancientDropXAmount;
                                }

                                int finalPercentInt;
                                {
                                    float perXasscadePercentChance = curLevelPercentChance / Math.Min(curCasscadeXAmount, Math.Max(1, Xasscade + 1));   //divide by amount of lots that hold the same item.

                                    if (perXasscadePercentChance < 0.5)
                                    {
                                        //Util.println(npcLine._idName + "  StoneLine:" + itemLotEnemyName + " " + typeStrings[typeIndex] + " [" + curLevel + "]   SKIPPPED finalPercent <= 0  :" + curLevelPercentChance);
                                        //debugNoFinalPercent++;
                                        
                                        if (test == npcID)
                                            Util.println("curLevel:" + curLevel + " too small chance. skipped");
                                        continue;
                                    }
                                    perXasscadePercentChance *= DROPMULTpercentMult;
                                    finalPercentInt = (int)(perXasscadePercentChance + 0.5);
                                }



                                //int tempLotIndex = -1;
                                if (createNewLineforThisType || (!useSingleLine && !useSingleLineOnce) || stoneLine == null)
                                {
                                    //if (createNewLineNextTime)
                                    //    useSingleLineOnce = false; //createNewLineNextTime overrides and resets useSingleLineOnce
                                    createNewLineforThisType = false;
                                    string newItemName = EquipParamGoods.GetFieldWithLineID(1, dropTypesIDsDictionaries[typeIndex][curLevel].ToString());

                                    //posthok fix
                                    if (stoneLine != null && !donePostHawkFix && curTotalPercent > 1000)
                                    {
                                        float postHok_amountMult = (curTotalPercent / 1000f);
                                        var keys = lotToChanceDict.Keys.ToArray();
                                        for (int i = 0; i < keys.Length; i++)
                                        {
                                            stoneLine.OperateField("lotItemNum0" + keys[i], new OperateIntField(Operation.MULTIPLY, postHok_amountMult));
                                            if (test == npcID)
                                                Util.p();
                                        }
                                        donePostHawkFix = true;
                                    }

                                    if ((treatAsBoss || currentlyFirstDropGuarentee))// special excpetion where we dont  
                                    {
                                        lotIndex = 1;
                                        emptyLotReplaced = true;
                                    }
                                    else
                                    {
                                        emptyLotReplaced = false;
                                        lotIndex = 2;
                                    }
                                    string spaceString = "";
                                    if (itemLotEnemyName != "")
                                        spaceString = " ";

                                    if (test == npcID)
                                        Util.p();

                                    if (linesToReplaceOrRemove.Count == 0)
                                    {

                                        stoneLine = baseEmptyItemLotParamLine.Copy(ItemLotParam)
                                            .SetField(0, targetSmithingStoneLineID)
                                            .SetField(1, itemLotEnemyName + spaceString + newItemName);

                                        targetSmithingStoneLineID = stoneLine.GetNextFreeId();
                                        if (targetSmithingStoneLineID > HighestIdPossible)
                                        {
                                            Util.p(itemLotLine._idName + keyword + npcLine._idName);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        int replacedId = linesToReplaceOrRemove[linesToReplaceOrRemove.Count - 1].id_int;
                                        stoneLine = baseEmptyItemLotParamLine.Copy(ItemLotParam)
                                            .SetField(0, replacedId)
                                            .SetField(1, itemLotEnemyName + spaceString + newItemName);
                                        linesToReplaceOrRemove.Remove(stoneLine);
                                    }
                                    if (IsItemLotMapDrop)
                                        stoneLine.SetField("getItemFlagId", itemLotLineMapDrop_getItemFlagId);

                                    donePostHawkFix = false;

                                    lotToChanceDict.Clear();
                                    lotPercentTotal = 0;
                                    //lotPercentTotalExcess = 0;
                                    curTotalPercent = 0;

                                    backupItem_PercentChanceInt = -1;
                                    backupItem_itemID = -1;
                                    backupItem_Amount = -1;
                                    backupItem_IsLuck = false;

                                    backupItem2_PercentChanceInt = -1;
                                    backupItem2_itemID = -1;
                                    backupItem2_Amount = -1;
                                    backupItem2_IsLuck = false;



                                    if (curGiveUniqueItemFlagID)//&& createNewLineForSpecialDrop)
                                    {
                                        curStoneLinesToAddGetItemFlagId.Add(stoneLine);
                                        curStoneLinesAddItemFlagIdLots.Add(-1);
                                    }

                                    if (!useSingleLine && !useSingleLineOnce) //for singleLines, name gets added at the end.
                                    {
                                        if (itemLotLine == null)
                                        {
                                            idToVariantsDict.Add(stoneLine, variantID);
                                            idToLevelDict.Add(stoneLine, level);
                                            //stoneLine.SetField(1, stoneLine.name + levelString + variantString);
                                            itemLotLine = stoneLine;
                                            itemLotMainLines.lines.Add(itemLotLine);
                                        }
                                    }

                                    if (treatAsBoss)
                                        stoneLine.SetField("canExecByFriendlyGhost", "0");

                                    newLines.Add(stoneLine);
                                    if (typeIndex == 2)
                                        foundRuneDrop = true;
                                    else if (typeIndex == 1 || typeIndex == 0)
                                        foundSmithingDrop = true;
                                    //ItemLotParam.OverrideOrAddLine(stoneLine);
                                }
                                else
                                {
                                    useSingleLineOnce = false;
                                    if (!outOfLots)
                                    {
                                        string newItemName = EquipParamGoods.GetFieldWithLineID(1, dropTypesIDsDictionaries[typeIndex][curLevel].ToString());
                                        stoneLine.SetField(1, stoneLine.name + " & " + newItemName);
                                    }
                                }


                                int HighestAmount = 0;
                                int lowestAmount = Math.Max(1, curCasscadeXAmount - Xasscade);
                                Dictionary<int, int> curLineAmountsToChanceDict = new Dictionary<int, int>();
                                {
                                    int percentToAddToXasscadeNum1 = 0;
                                    int curXasscadeNumToDrop = curCasscadeXAmount;
                                    for (int curXasscade = 0; curXasscade <= Xasscade && curXasscadeNumToDrop > 0; curXasscade++)
                                    {

                                        int curXasscadeFinalPercentInt = finalPercentInt;
                                        float curXasscadeFinalPercent = curXasscadeFinalPercentInt;
                                        if (curXasscadeNumToDrop == lowestAmount)
                                        {
                                            curXasscadeFinalPercent += percentToAddToXasscadeNum1;
                                            curXasscadeFinalPercentInt = (int)(curXasscadeFinalPercent + 0.5);
                                            if (test == npcID)
                                                Util.p();
                                            percentToAddToXasscadeNum1 = 0;

                                        }
                                        else if (XasscadeMultToX != 1 || XasscadePowToX != 1)
                                        {
                                            curXasscadeFinalPercent = finalPercentInt * (XasscadeMultToX * (1 + (1 - XasscadeMultToX)));
                                            float powMult = (float)Math.Pow(XasscadePowToX, curXasscadeNumToDrop - lowestAmount);
                                            curXasscadeFinalPercent *= powMult; //* (1 + (1 - powMult));

                                            curXasscadeFinalPercentInt = (int)(curXasscadeFinalPercent + 0.5);
                                            percentToAddToXasscadeNum1 += finalPercentInt - curXasscadeFinalPercentInt;

                                            if (test == npcID)
                                                Util.p();

                                        }
                                        //curXasscadeFinalPercentInt = Math.Min((int)(curLevelPercentChance + 0.5), curXasscadeFinalPercentInt);

                                        if (curXasscadeFinalPercentInt <= 0)
                                        {
                                            curXasscadeNumToDrop--;
                                            continue;
                                        }
                                        if (curXasscadeNumToDrop > HighestAmount)
                                            HighestAmount = curXasscadeNumToDrop;
                                        curLineAmountsToChanceDict.Add(curXasscadeNumToDrop, curXasscadeFinalPercentInt);
                                        if (test == npcID)
                                            Util.p();
                                        curXasscadeNumToDrop--;

                                        if (curXasscadeNumToDrop == 1 && (treatAsBoss || currentlyFirstDropGuarentee) && FTD_AvoidDropx1)
                                            break;
                                    }
                                    curXasscadeNumToDrop++;
                                    //adds addtiona percent to last if it wasnt added to 1
                                    /*if (percentToAddToXasscadeNum1 != 0)
                                    {
                                        curLineAmountsToChanceDict[curXasscadeNumToDrop] += percentToAddToXasscadeNum1;
                                    }*/

                                    float curAmountMult = DROPMULTamountMult;
                                    if (test == npcID)
                                        Util.p();
                                    if (currentlyFirstDropGuarentee || treatAsBoss)
                                    {
                                        curAmountMult = DROPMULT;
                                        donePostHawkFix = true; //replaces posthawk fix
                                    }

                                    if (curAmountMult != 1)
                                    {
                                        var newCurLineAmountsToChanceDict = new Dictionary<int, int>();

                                        const bool distributeToNext = false;
                                        var newCurLineAmountsToDistibutedOddsDict = new Dictionary<int, int>();
                                        var XasscadeKeys = curLineAmountsToChanceDict.Keys.ToArray();

                                        for (int i = XasscadeKeys.Length - 1; i >= 0; i--)
                                        {
                                            int oddsToDistributeToNext = 0;
                                            int curNumToDrop = XasscadeKeys[i];
                                            int curPercentInt = curLineAmountsToChanceDict[curNumToDrop];

                                            float Raw = curNumToDrop * curAmountMult;

                                            int newNumToDrop = (int)Raw;
                                            float DROPMULTamountMultremainder = Raw - newNumToDrop;

                                            oddsToDistributeToNext = (int)(DROPMULTamountMultremainder * curPercentInt);

                                            if (distributeToNext)
                                            {
                                                if (oddsToDistributeToNext <= 0)
                                                {
                                                    newCurLineAmountsToChanceDict.Add(newNumToDrop, curPercentInt);
                                                    if (newNumToDrop > HighestAmount)
                                                        HighestAmount = newNumToDrop;
                                                    continue;
                                                }
                                                curPercentInt -= oddsToDistributeToNext;
                                            }
                                            else
                                                curPercentInt += oddsToDistributeToNext;


                                            //curLineAmountsToChanceDict[newNumToDrop] = curPercentInt;
                                            newCurLineAmountsToChanceDict.Add(newNumToDrop, curPercentInt);
                                            if (newNumToDrop > HighestAmount)
                                                HighestAmount = newNumToDrop;

                                            if (test == npcID)
                                                Util.p();

                                            if (distributeToNext)
                                            {

                                                if (newCurLineAmountsToDistibutedOddsDict.ContainsKey(newNumToDrop + 1))
                                                {
                                                    newCurLineAmountsToDistibutedOddsDict[newNumToDrop + 1] += oddsToDistributeToNext;
                                                }
                                                else
                                                {
                                                    newCurLineAmountsToDistibutedOddsDict.Add(newNumToDrop + 1, oddsToDistributeToNext);
                                                    if (newNumToDrop + 1 > HighestAmount)
                                                        HighestAmount = newNumToDrop + 1;
                                                }
                                            }
                                        }
                                        if (distributeToNext)
                                        {
                                            foreach (int key in newCurLineAmountsToDistibutedOddsDict.Keys)
                                            {
                                                if (newCurLineAmountsToChanceDict.ContainsKey(key))
                                                    newCurLineAmountsToChanceDict[key] += newCurLineAmountsToDistibutedOddsDict[key];
                                                else
                                                    newCurLineAmountsToChanceDict.Add(key, newCurLineAmountsToDistibutedOddsDict[key]);
                                                if (test == npcID)
                                                    Util.println();
                                            }
                                        }
                                        curLineAmountsToChanceDict = newCurLineAmountsToChanceDict;
                                    }

                                    foreach (int key in curLineAmountsToChanceDict.Keys)
                                    {
                                        int _curKeyAmount = curLineAmountsToChanceDict[key]; ;
                                        curTotalPercent += _curKeyAmount;
                                        if (test == npcID)
                                            Util.p();
                                    }
                                }


                                for (int curNumToDrop = HighestAmount; curNumToDrop >= 1; curNumToDrop--)
                                {
                                    if (!curLineAmountsToChanceDict.ContainsKey(curNumToDrop))
                                    {
                                        continue;
                                    }
                                    int curFinalPercentInt = curLineAmountsToChanceDict[curNumToDrop];
                                    int curLotIndex = lotIndex;

                                    if (!emptyLotReplaced && curTotalPercent >= 1000)
                                    {
                                        curLotIndex = 1;
                                        emptyLotReplaced = true;
                                        if (ableToAvoidUnessasryFTD && d == 0 && dmax == 1 && currentFirstTimeLevelAdj == 0
                                        && !(FTD_OnlyHighestLevel && levelCasscade > 0)
                                        && !(FTD_OnlyHighestX && Xasscade > 0)
                                        && !(FTD_OnlyFirstType && isBoth)
                                        && !(FTD_OnlySomberType && (dropSmithing || !dropSomber))
                                        && !(FTD_AvoidDropx1 && Xasscade > 0)
                                        && !(FTD_DropMultiple == oneLineDict[keyword])  //checks if single line is mismatched.
                                        ) // if there is a level adjust on the ftd then we dont remove the ftd.
                                        {
                                            curStoneLinesToAddGetItemFlagId.Clear();
                                            curStoneLinesAddItemFlagIdLots.Clear();
                                            curGiveUniqueItemFlagID = false;
                                            currentlyFirstDropGuarentee = false;
                                            dmax = 0;
                                            variantID = backUpVariantId;
                                            curMaxLotIndex++;
                                        }
                                    }
                                    else if (lotIndex > curMaxLotIndex)
                                    {
                                        outOfLots = true;
                                        int lowestLot = -1;
                                        int lowestLotPercent = curFinalPercentInt;
                                        foreach (int lot in lotToChanceDict.Keys)
                                        {
                                            if (lotToChanceDict[lot] < lowestLotPercent)
                                            {
                                                lowestLotPercent = lotToChanceDict[lot];
                                                lowestLot = lot;
                                            }
                                        }
                                        if (lowestLotPercent < curFinalPercentInt)
                                            curLotIndex = lowestLot;
                                        else
                                        {
                                            //lotPercentTotalExcess += curFinalPercentInt;
                                            if (curFinalPercentInt > backupItem_PercentChanceInt)
                                            {
                                                backupItem2_PercentChanceInt = backupItem_PercentChanceInt;
                                                backupItem2_itemID = backupItem_itemID;
                                                backupItem2_IsLuck = backupItem_IsLuck;
                                                backupItem2_Amount = backupItem_Amount;

                                                backupItem_PercentChanceInt = curFinalPercentInt;
                                                backupItem_itemID = dropTypesIDsDictionaries[typeIndex][curLevel];
                                                backupItem_IsLuck = canUseLuck;
                                                backupItem_Amount = curNumToDrop;

                                            }
                                            else if (curFinalPercentInt > backupItem2_PercentChanceInt)
                                            {
                                                backupItem2_PercentChanceInt = curFinalPercentInt;
                                                backupItem2_itemID = dropTypesIDsDictionaries[typeIndex][curLevel];
                                                backupItem2_IsLuck = canUseLuck;
                                                backupItem2_Amount = curNumToDrop;
                                            }
                                            continue;
                                        }
                                    }



                                    if (lotToChanceDict.ContainsKey(curLotIndex))
                                    {
                                        lotPercentTotal -= lotToChanceDict[curLotIndex];
                                        //lotPercentTotalExcess += lotToChanceDict[curLotIndex];
                                        lotToChanceDict.Remove(curLotIndex);
                                    }
                                    lotToChanceDict.Add(curLotIndex, curFinalPercentInt);
                                    lotPercentTotal += curFinalPercentInt;

                                    if (curLotIndex == lotIndex)
                                        lotIndex++;


                                    stoneLine
                                        .SetField("lotItemId0" + curLotIndex.ToString(), dropTypesIDsDictionaries[typeIndex][curLevel])
                                        .SetField("lotItemCategory0" + curLotIndex.ToString(), 1)
                                        .SetField("lotItemNum0" + curLotIndex.ToString(), curNumToDrop)
                                        .SetField("lotItemBasePoint0" + curLotIndex.ToString(), Math.Min(34463, curFinalPercentInt))
                                        .SetField("enableLuck0" + curLotIndex.ToString(), canUseLuck);



                                    /*if (curGiveUniqueItemFlagID )//&& !createNewLineForSpecialDrop)
                                    {
                                        stoneLinesToAddGetItemFlagId.Add(stoneLine);
                                        stoneLinAddItemFlagIdLots.Add(curLotIndex);
                                    }*/


                                    if (test == npcID)
                                        Util.println("      lot:" + lotIndex + " isSomber:" + (typeIndex == 1).ToString() + "  [" + curLevel + "]   x" + curNumToDrop);
                                }

                                if (test == npcID)
                                    Util.println(npcLine.name + " " + typeIndex + " " + casscadeIndex + " " + curTotalPercent + "  " + lotPercentTotal);

                                if (!emptyLotReplaced)
                                {
                                    if (curTotalPercent >= 1000)
                                    {
                                        emptyLotReplaced = true;
                                        if (ableToAvoidUnessasryFTD && d == 0 && dmax == 1 && currentFirstTimeLevelAdj == 0
                                        && !(FTD_OnlyHighestLevel && levelCasscade > 0)
                                        && !(FTD_OnlyHighestX && Xasscade > 0)
                                        && !(FTD_OnlyFirstType && isBoth)
                                        && !(FTD_OnlySomberType && (dropSmithing || !dropSomber))
                                        && !(FTD_AvoidDropx1 && Xasscade > 0)
                                        && !(FTD_DropMultiple == oneLineDict[keyword])  //checks if single line is mismatched.
                                        ) // if there is a level adjust on the ftd then we dont remove the ftd.
                                        {
                                            curStoneLinesToAddGetItemFlagId.Clear();
                                            curStoneLinesAddItemFlagIdLots.Clear();
                                            curGiveUniqueItemFlagID = false;
                                            currentlyFirstDropGuarentee = false;
                                            dmax = 0;
                                            variantID = backUpVariantId;
                                            curMaxLotIndex++;
                                        }
                                    }
                                    else
                                        stoneLine.SetField("lotItemBasePoint01", Math.Max(0, 1000 - curTotalPercent));
                                }
                                if (emptyLotReplaced && backupItem_PercentChanceInt != -1 && !lotToChanceDict.ContainsKey(1))
                                {
                                    lotToChanceDict.Add(1, backupItem_PercentChanceInt);
                                    lotPercentTotal += backupItem_PercentChanceInt;
                                    //lotPercentTotalExcess -= backupItem_PercentChanceInt;
                                    stoneLine
                                        .SetField("lotItemId01", backupItem_itemID)
                                        .SetField("lotItemCategory01", 1)
                                        .SetField("lotItemNum01", backupItem_Amount)
                                        .SetField("lotItemBasePoint01", Math.Min(34463, backupItem_PercentChanceInt))
                                        .SetField("enableLuck01", backupItem_IsLuck);
                                    backupItem_itemID = backupItem2_itemID;
                                    backupItem_Amount = backupItem2_Amount;
                                    backupItem_IsLuck = backupItem2_IsLuck;
                                    backupItem_PercentChanceInt = backupItem2_PercentChanceInt;
                                    backupItem2_PercentChanceInt = -1;
                                    backupItem2_itemID = -1;
                                    backupItem2_Amount = -1;
                                    backupItem2_IsLuck = false;
                                }
                                if (backupItem_PercentChanceInt != -1 && !lotToChanceDict.ContainsKey(curMaxLotIndex))//curMaxLotIndex increased already if nessery
                                {
                                    lotToChanceDict.Add(maxLotIndex, backupItem_PercentChanceInt);
                                    lotPercentTotal += backupItem_PercentChanceInt;
                                    //lotPercentTotalExcess -= backupItem_PercentChanceInt;
                                    stoneLine
                                        .SetField("lotItemId0" + maxLotIndex, backupItem_itemID)
                                        .SetField("lotItemCategory0" + maxLotIndex, 1)
                                        .SetField("lotItemNum0" + maxLotIndex, backupItem_Amount)
                                        .SetField("lotItemBasePoint0" + maxLotIndex, Math.Min(34463, backupItem_PercentChanceInt))
                                        .SetField("enableLuck0" + maxLotIndex, backupItem_IsLuck);
                                    backupItem_itemID = backupItem2_itemID;
                                    backupItem_Amount = backupItem2_Amount;
                                    backupItem_IsLuck = backupItem2_IsLuck;
                                    backupItem_PercentChanceInt = backupItem2_PercentChanceInt;
                                    backupItem2_PercentChanceInt = -1;
                                    backupItem2_itemID = -1;
                                    backupItem2_Amount = -1;
                                    backupItem2_IsLuck = false;
                                }

                                //Util.println(itemLotEnemyName + " " + typeStrings[typeIndex] + " [" + curLevel + "]" + "   " + curPercentChance);
                                if (!useSingleLine && !useSingleLineOnce) //reset for next line
                                {
                                    //if(giveUniqueItemFlagID && createNewLineForSpecialDrop)
                                    if (d == 0 && dmax == 1) //if there will be a FTD will add a FIRST TIME NO DROP (near guarentee) for none FTD 
                                    {
                                        if (lotIndex > maxLotIndex) //this shouldnt happen because of our curMaxLorIndex.
                                        {
                                            //this shu=ouldnt happen at all.
                                            Util.println(//stoneLine._idName + 
                                                " out of itemlot slots!!!! " + keyword + "       npcID:" + npcID);
                                            break;
                                        }
                                        stoneLine.SetField("lotItemBasePoint0" + lotIndex.ToString(), 34463);//
                                        if (npcID == 21400114)
                                            Util.p();
                                        curStoneLinesToAddGetItemFlagId.Add(stoneLine);
                                        curStoneLinesAddItemFlagIdLots.Add(lotIndex);
                                        //lotIndex++;
                                    }

                                }

                                if ((treatAsBoss || currentlyFirstDropGuarentee) && FTD_OnlyHighestLevel)
                                {
                                    break;
                                }
                            }
                            //DropType_CanOnlyDropOneType
                            if ((treatAsBoss || currentlyFirstDropGuarentee) && FTD_OnlyFirstType)    // only one type can drop with 4
                                break;


                        }

                        if (stoneLine == null)
                        {
                            if (test == npcID)
                                Util.println(npcLine._idName + "  stoneLine is null.");
                            if (d == 0 && dmax == 1)
                            {
                                treatAsBoss = true; //i gues this is to skip to rune.
                                d = -1;
                                dmax = 0;


                                curPercentChance = 0;
                                percentChance = 0;
                            }
                            else
                                npcLine.RevertFieldToVanilla("itemLotId_enemy");
                        }
                        else
                        {
                            //if (stoneLine.id_int == 301001006)
                            //    Util.p();
                            if (useSingleLine)   //fr non-singleLines, name get added during loop.
                            {

                                if (d == 0 && dmax == 1) //if there will be a FTD will add a FIRST TIME NO DROP (near guarentee) for none FTD 
                                {
                                    if (lotIndex > maxLotIndex) //this shouldnt happen because of our curMaxLorIndex.
                                    {
                                        //this shu=ouldnt happen at all.
                                        Util.println(//stoneLine._idName + 
                                            " out of itemlot slots!!!! " + keyword + "       npcID:" + npcID);
                                        break;
                                    }
                                    stoneLine.SetField("lotItemBasePoint0" + lotIndex.ToString(), 34463);//
                                    curStoneLinesToAddGetItemFlagId.Add(stoneLine);
                                    curStoneLinesAddItemFlagIdLots.Add(lotIndex);
                                    //lotIndex++;
                                }
                                if (itemLotLine == null)
                                {
                                    idToVariantsDict.Add(stoneLine, variantID);
                                    idToLevelDict.Add(stoneLine, level);
                                    //stoneLine.SetField(1, stoneLine.name + levelString + variantString);
                                    itemLotLine = stoneLine;
                                    itemLotMainLines.lines.Add(itemLotLine);
                                }
                            }
                            if (curStoneLinesAddItemFlagIdLots.Count > 0 && currentlyFirstDropGuarentee)
                            {

                                IntFilter.Single filter = StoneDrop_getItemFlagIDFilter;
                                if (typeIndex == 2)
                                    filter = RuneDrop_getItemFlagIDFilter;

                                int currentGetItemFlagId = IntFilter.GetRandomInt(npcID, filter, usedGetItemFlagId);
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                                for (int i = 0; i < curStoneLinesToAddGetItemFlagId.Count; i++)
                                {
                                    var line = curStoneLinesToAddGetItemFlagId[i];
                                    var lot = curStoneLinesAddItemFlagIdLots[i];
                                    if (lot == -1)
                                    {
                                        line.SetField("getItemFlagId", currentGetItemFlagId);
                                        line.SetField("canExecByFriendlyGhost", "0");
                                    }
                                    else
                                        line.SetField("getItemFlagId0" + lot, currentGetItemFlagId);
                                }
                            }
                        }
                    }

                    if (foundOneTimeDrop || foundRuneDrop || foundSmithingDrop)
                    {
                        var debugId = -1;
                        var debugNewLine = "";
                        if (newLines.Count > 0)
                        {
                            debugId = newLines[0].id_int;
                            debugNewLine = newLines[0].ToWrite();
                        }

                        foreach (Line line in newLines) {
                            ItemLotParam.OverrideOrAddLine(line);
                        }
                        //ItemLotParam.OverrideOrAddLines(newLines, true);
                        if (debugId != -1)
                        {
                            if (ItemLotParam.GetLineWithId(debugId).ToWrite() == debugNewLine)//new lines getting deleted
                            {
                                Util.p();
                                Util.p();
                            }
                        }
                    }

                    foreach (Line l in linesToReplaceOrRemove)//removes the lines we wanted to delete in the beggining only if it wont screw the itemLot group
                    {
                       if( targetSmithingStoneLineID >= l.id_int)
                       {
                            ItemLotParam.RemoveLine(l.id_int);
                       }
                    }
                }

                
            }
            
            //Util.PrintStrings(Util.ToStrings(freedFlagIds.ToArray()));
        }
        static void WeaponDefaultSkillChanges()
        {
            //Erdsteel Dagger - Spinning Slash.
            //Bloodstained Dagger - Repeating thrust
            //Misericorde - Impaling Thrust
            //Great Knife - War Cry
            //Crystal Knife - Endure
            //Wakizashi - Double Slash

            //Short Sword - Spinning Slash
            //Broad Sword - Stamp Sweep
            //Weathered Straight Sword - Endure
            //Noble's Slender Sword - Repeating Thrust
            //Cane Sword - Determination

            //Bastard Sword - Stamp(Sweep)
            //Iron Greatsword - Endure
            //Forked Greatsword - Warcry
            //Flamberge - Sword Dance

            //Greatsword - Stamp(Sweep)
            //Watchdog's Greatsword - Ground Slam

            //Noble's Estoc - Repeating Thrusts

            //Great Epee - Peircing Fang
            //Godskin Stitcher -  Repeating Thrusts

            //Falchion - Warcry
            //Shamshir - Sword Dance
            //Bandit's Curved Sword - Double Slash
            //Scavenger's Curved Sword - Quickstep

            //Omen Cleaver - Stomp(Upward Cut)
            //Beastman's Cleaver - Stomp(Sweep)

            //Jawbone Axe - Stomp(Upward Cut)

            //Stone Club - Endure
            //Mace - Endure
            //Morning Star - Endure
            //Warpic - Endure
            //Hammer - Determination
            //Monk's Flamemace - Stamp(Sweep)

            //Spiked Spear - Spinning Slash
            //Cross-Nagakiba - Spinning Slash

            //Lucerne - Determinantion
        }
        static void MaloModShopChanges()
        {
            if (!IsRunningParamFile(new ParamFile[] { ShopLineupParam }))
                return;
            int sellQuantity = ShopLineupParam.GetFieldIndex("sellQuantity");
            int value = ShopLineupParam.GetFieldIndex("value");
            int equipId = ShopLineupParam.GetFieldIndex("equipId");
            Line toReplaceWithDaggerLine = ShopLineupParam.GetLineOnCondition(new Condition.NameIs("[Merchant - North Limgrave] Pickled Turtle Neck"))
                .SetField(equipId, 1090000)  //Great Knife
                .SetField("equipType", 0)   //weapon
                .SetField(1, "[Merchant - North Limgrave] Great Knife")
                .SetField(sellQuantity, 1)
                .SetField(value, 600);

            foreach (Line l in ShopLineupParam.GetLinesOnCondition(new Condition.HasInName("Festering Bloody Finger")))
            {
                l.Operate(new OperateIntField(sellQuantity, Operation.PLUS, 4));
                l.Operate(new OperateIntField(sellQuantity, Operation.MULTIPLY, 1.3f));
                l.Operate(new OperateIntField(value, Operation.DIVIDED, 10));
            }
        }
        static void SmithingStoneShopLineupChanges()
        {
            if (!IsRunningParamFile(new ParamFile[] { ShopLineupParam }))
                return;
            int sellQuantity = ShopLineupParam.GetFieldIndex("sellQuantity");
            int value = ShopLineupParam.GetFieldIndex("value");
            int equipId = ShopLineupParam.GetFieldIndex("equipId");
            foreach (Line l in ShopLineupParam.GetLinesOnCondition(new Condition.HasInName("Smithing Stone [").AND(new Condition.HasInName("Merchant")))){
                l.SetField(sellQuantity, 1);
                l.Operate(new OperateIntField(value, Operation.MULTIPLY, 2f));
            }

            int i = 0;
            foreach (Line l in ShopLineupParam.GetLinesOnCondition(new Condition.HasInName("Somber Smithing Stone [").AND(new Condition.HasInName("Iji"))))
            {
                if (i == 3)
                {
                    l.SetField(1, "[Iji] Lost Ashes of War");
                    l.SetField(sellQuantity, 3);
                    l.SetField(value, 2000);
                    l.SetField(equipId, 10070); //lost ash of war
                }
                else
                {
                    //string oldNum = l.GetField(sellQuantity);
                    int num = Math.Max(1, (int)(2.5 - (i * 0.5)));
                    //Util.println(l._idName + "  " + oldNum + " --> " + num);
                    l.SetField(sellQuantity, num);

                    //string oldprice = l.GetField(value);
                    int price = (int)(((float.Parse(l.GetField(value)) / 1000) * (1 + (i * 0.2))) + 0.5) * 1000;
                    //Util.println(l._idName + "  " + oldprice + " --> " + price);
                    l.SetField(value, price);
                }

                i++;
            }

        }
        static void replaceOpenWorldSmithingStones()
        {
            if (!IsRunningParamFile(new ParamFile[] { ItemLotParam_map}))
                return;


            //Raptor's Talons - Raptor's of the Mist


            //golden rune ids are 2899 + golden rune level. for golen rune ID


            var smithingStoneItemIDsDict = new Dictionary<int, int>();
            var somberSmithingStoneItemIDsDict = new Dictionary<int, int>();
            {
                smithingStoneItemIDsDict.Add(10100, 1);
                smithingStoneItemIDsDict.Add(10101, 2);
                smithingStoneItemIDsDict.Add(10102, 3);
                smithingStoneItemIDsDict.Add(10103, 4);
                smithingStoneItemIDsDict.Add(10104, 5);
                smithingStoneItemIDsDict.Add(10105, 6);
                smithingStoneItemIDsDict.Add(10106, 7);
                smithingStoneItemIDsDict.Add(10107, 8);
                smithingStoneItemIDsDict.Add(10140, 9);
                somberSmithingStoneItemIDsDict.Add(10160, 1);
                somberSmithingStoneItemIDsDict.Add(10161, 2);
                somberSmithingStoneItemIDsDict.Add(10162, 3);
                somberSmithingStoneItemIDsDict.Add(10163, 4);
                somberSmithingStoneItemIDsDict.Add(10164, 5);
                somberSmithingStoneItemIDsDict.Add(10165, 6);
                somberSmithingStoneItemIDsDict.Add(10166, 7);
                somberSmithingStoneItemIDsDict.Add(10167, 8);
                somberSmithingStoneItemIDsDict.Add(10200, 9);
                somberSmithingStoneItemIDsDict.Add(10168, 10);
            }//create ditionaries

            var smithingStoneIDsDict = new Dictionary<int, int>();
            var somberSmithingStoneIDsDict = new Dictionary<int, int>();
            {
                smithingStoneIDsDict.Add(1, 10100);
                smithingStoneIDsDict.Add(2, 10101);
                smithingStoneIDsDict.Add(3, 10102);
                smithingStoneIDsDict.Add(4, 10103);
                smithingStoneIDsDict.Add(5, 10104);
                smithingStoneIDsDict.Add(6, 10105);
                smithingStoneIDsDict.Add(7, 10106);
                smithingStoneIDsDict.Add(8, 10107);
                smithingStoneIDsDict.Add(9, 10140);
                somberSmithingStoneIDsDict.Add(1, 10160);
                somberSmithingStoneIDsDict.Add(2, 10161);
                somberSmithingStoneIDsDict.Add(3, 10162);
                somberSmithingStoneIDsDict.Add(4, 10163);
                somberSmithingStoneIDsDict.Add(5, 10164);
                somberSmithingStoneIDsDict.Add(6, 10165);
                somberSmithingStoneIDsDict.Add(7, 10166);
                somberSmithingStoneIDsDict.Add(8, 10167);
                somberSmithingStoneIDsDict.Add(9, 10200);
                somberSmithingStoneIDsDict.Add(10, 10168);
            }//create ditionaries


            var isBossToIgnore = new Condition.FieldCondition.FloatFieldCompare(0, Condition.LESS_THAN, 40000);
            var isTearDropScarab = new Condition.HasInName("[Teardrop Scarab -");
            var isLegacyDungeon = new Condition.HasInName("[LD -");
            var isFieldBoss = new Condition.HasInName("- Field ");
            var isMaterialNode = new Condition.HasInName("[Material Node]");
            var isTunnel = new Condition.HasInName(new string[] { "Tunnel"});

            //var isInDungeon = new Condition.NameStartsWith("["); //is in dungen or material node or dropped by bosses.

            

            int lotItemId01 = ItemLotParam_map.GetFieldIndex("lotItemId01");
            var isSmithingStone = new Condition.FieldEqualTo(lotItemId01,Util.ToStrings(smithingStoneItemIDsDict.Keys.ToArray()));
            var isSomberSmithingStone = new Condition.FieldEqualTo(lotItemId01,Util.ToStrings(somberSmithingStoneItemIDsDict.Keys.ToArray()));

            var isStone = new Condition.Either(isSmithingStone, isSomberSmithingStone);
            //var isSmithingStone = isSomberSmithingStone.IsFalse;
            //var isAncientSS = new Condition.HasInName("Ancient Dragon Smithing Stone");
            //var isAncientSomberSS = new Condition.HasInName("Somber Ancient Dragon Smithing Stone");

            //turn tear scarabs of sombers into lost ashes of wars.
            //reduce level of sombers that are not LD by 2.
            var stoneLines = ItemLotParam_map.GetLinesOnCondition(isStone);

            Random rand = new Random(0);    //seed is 0 for consistency.

            foreach (var stoneLine in stoneLines)
            {
                var debugLine = stoneLine.line;

                int id = stoneLine.id_int;


                if (isBossToIgnore.Pass(stoneLine) || isFieldBoss.Pass(stoneLine)) // removes most bosses.
                    continue;

                bool canTransformItem = !isTunnel.Pass(stoneLine) && !isMaterialNode.Pass(stoneLine);

                bool legacyDungeon = isLegacyDungeon.Pass(stoneLine);
                bool teardrop = isTearDropScarab.Pass(stoneLine);
                //bool isDungeon = isInDungeon.Pass(stoneLine);
                int oldLineItemId = stoneLine.GetFieldAsInt(lotItemId01);
                string oldItemName = EquipParamGoods.GetLineWithId(oldLineItemId).name;
                int newItemId = -1;
                if (teardrop)
                {
                    newItemId = 10070;
                     //itemID - lost ashes of war
                    stoneLine.SetField("lotItemCategory01", 1);   //category - good
                }
                else
                {
                    if (ItemLotParam_map.GetLineWithId(id + 1) != null || ItemLotParam_map.GetLineWithId(id - 1) != null)   //is not a solo pick up so ignore. this avoids most enemy related drops.
                        continue;

                    bool isSomber = isSomberSmithingStone.Pass(stoneLine);

                    int itemID = int.Parse(stoneLine.GetField(lotItemId01));
                    int curLevel = 0;

                    bool isAncient = false;

                    if (smithingStoneItemIDsDict.ContainsKey(itemID))
                    {
                        curLevel = smithingStoneItemIDsDict[itemID];
                        if (curLevel == 9)
                            isAncient = true;
                    }
                    else if (somberSmithingStoneItemIDsDict.ContainsKey(itemID))
                    {
                        curLevel = somberSmithingStoneItemIDsDict[itemID];
                        if (curLevel == 10)
                            isAncient = true;
                    }
                    else
                        continue;

                    if (isAncient)
                        continue;

                    //Open World Changes
                    const float SSchanceToReduceLevel = 0f;
                    const float SSLevelMult = 1f;
                    const float SomberSSchanceToReduceLevel = 1f;
                    const float SomberSSLevelMult = 0.7f;                    

                    const float SSchanceToChangeIntoRune = 0.3f;
                    const int SSRuneLevelAdj = 1;
                    const float SomberSSchanceToChangeIntoRune = 0.25f;
                    const int SomberSSRuneLevelAdj = 3;

                    const float SomberSSchanceToChangeIntoSmithing = 0.25f;
                    const float SomberSSChangeIntoSmithing_LevelMult = 0.93f; //0.9 would reduce it to be level relative.




                    //Legacy Dungeon Changes
                    const float LD_SSchanceToReduceLevel = 0f;
                    const float LD_SSLevelMult = 1f;
                    const float LD_SomberSSchanceToReduceLevel = 1f;
                    const float LD_SomberSSLevelMult = 0.875f;

                    const float LD_SSchanceToChangeIntoRune = 0;
                    const int LD_SSRuneLevelAdj = 1;
                    const float LD_SomberSSchanceToChangeIntoRune = 0.25f;
                    const int LD_SomberSSRuneLevelAdj = 3;

                    const float LD_SomberSSchanceToChangeIntoSmithing = 0.35f;
                    const float LD_SomberSSChangeIntoSmithing_LevelMult = 0.93f; //reduces it to be relative to other.





                    int runeLevel = -1;
                    bool turnToSmithing = false;
                    if (canTransformItem)
                    {
                        if (!isSomber)
                        {
                            if (legacyDungeon)
                            {
                                if (rand.NextDouble() < LD_SSchanceToChangeIntoRune)
                                    runeLevel = curLevel + LD_SSRuneLevelAdj;
                            }
                            else
                            if (rand.NextDouble() < SSchanceToChangeIntoRune)
                                runeLevel = curLevel + SSRuneLevelAdj;
                        }
                        else
                        {
                            if (legacyDungeon)
                            {
                                if (rand.NextDouble() < LD_SomberSSchanceToChangeIntoSmithing)
                                    turnToSmithing = true;
                            }
                            else
                            if (rand.NextDouble() < SomberSSchanceToChangeIntoSmithing)
                                turnToSmithing = true;

                            if (!turnToSmithing)
                            {
                                if (legacyDungeon)
                                {
                                    if (rand.NextDouble() < LD_SomberSSchanceToChangeIntoRune)
                                        runeLevel = curLevel + LD_SomberSSRuneLevelAdj;
                                }
                                else
                                if (rand.NextDouble() < SomberSSchanceToChangeIntoRune)
                                    runeLevel = curLevel + SomberSSRuneLevelAdj;
                            }
                        }
                    }

                    if (runeLevel == -1)
                    {
                        int newLevel = curLevel;
                        {
                            float nf = newLevel;
                            if (!isSomber || turnToSmithing)
                            {
                                if (legacyDungeon)
                                {
                                    if (turnToSmithing)
                                        nf *= LD_SomberSSChangeIntoSmithing_LevelMult;
                                    if (rand.NextDouble() < LD_SSchanceToReduceLevel)
                                        nf *= LD_SSLevelMult;
                                }
                                else
                                {
                                    if (turnToSmithing)
                                        nf *= SomberSSChangeIntoSmithing_LevelMult;
                                    if (rand.NextDouble() < SSchanceToReduceLevel)
                                        nf *= SSLevelMult;
                                }
                            }
                            else
                            {
                                if (legacyDungeon)
                                {
                                    if (rand.NextDouble() < LD_SomberSSchanceToReduceLevel)
                                        nf *= LD_SomberSSLevelMult;
                                }
                                else
                                if (rand.NextDouble() < SomberSSchanceToReduceLevel)
                                    nf *= SomberSSLevelMult;
                                
                            }
                            const float randomnessAmount = 0.16f;
                            nf += -randomnessAmount + (randomnessAmount * (float)rand.NextDouble()); // this adds some additional randomness +- (0.16) to avoid potential rounding jank.
                            newLevel = (int)(nf + 0.5f);
                        }
                        newLevel = Math.Max(1, newLevel);

                        if (!isSomber || turnToSmithing)
                        {
                            int highestlevel = smithingStoneIDsDict.Count;
                            newLevel = Math.Min(highestlevel, newLevel);
                            if (highestlevel == newLevel)
                                isAncient = true;
                            newItemId = smithingStoneIDsDict[newLevel];
                        }
                        else
                        {
                            int highestlevel = somberSmithingStoneIDsDict.Count;
                            newLevel = Math.Min(highestlevel, newLevel);
                            if (highestlevel == newLevel)
                                isAncient = true;
                            newItemId = somberSmithingStoneIDsDict[newLevel];
                        }
                        stoneLine.SetField("lotItemCategory01", 1);   //category - good
                    }
                    else
                    {
                        newItemId = 2899 + runeLevel;
                        stoneLine.SetField("lotItemCategory01", 1);   //category - good
                    }

                }
                if (newItemId != -1) {
                    stoneLine.SetField(lotItemId01, newItemId);
                    string newItemName = EquipParamGoods.GetLineWithId(newItemId).name;
                    if (stoneLine.name.Contains(oldItemName))
                        stoneLine.SetField(1, stoneLine.name.Replace(oldItemName, newItemName));
                }
                /*
                {
                    int startIndex = stoneLine.name.IndexOf("Somber Ancient Dragon Smithing Stone");
                    if (startIndex == -1)
                        startIndex = stoneLine.name.IndexOf("Ancient Dragon Smithing Stone");
                    if (startIndex == -1)
                        startIndex = stoneLine.name.IndexOf("Somber Smithing Stone");
                    if (startIndex == -1)
                        startIndex = stoneLine.name.IndexOf("Smithing Stone");

                    string newName = stoneLine.name.Remove(startIndex) + newItemName;
                    stoneLine.SetField(1, newName);
                }*/
            }


        }

        static void naturalFpRegen()
        {
            if (!IsRunningParamFile(SpEffectParam))
                return;
            Lines handLines = SpEffectParam.GetLinesWithId(new string[] { "100620", "100621" });
            handLines.Operate(new SetFieldTo(handLines.GetFieldIndex("motionInterval"), "0.750"));
            handLines.Operate(new SetFieldTo(handLines.GetFieldIndex("changeMpRate"), "-1.250"));
            handLines.Operate(new SetFieldTo(handLines.GetFieldIndex("changeMpPoint"), "-1"));
        }
        static void weaponArtsDamageModify()
        {
            if (!IsRunningParamFile(AtkParam_Pc) )
                return;

            //No FP row names and FP row names arent in these new vanilla files. i can potentially get the lines from my current file.

            var dmgCorrectionFieldindexes = AtkParam_Pc.GetFieldIndexes(new string[] { "atkPhysCorrection","atkMagCorrection", "atkFireCorrection", "atkThunCorrection" });

            Lines noFPLines = BehaviorParam_PC.GetLinesOnCondition(new Condition.HasInName("No FP"));
            string[] noFpAttackParamIds = noFPLines.GetFields(BehaviorParam_PC.GetFieldIndex("refId"));

            //multiply the damage of noFpLines.
            Lines noFpAttackParamLines = AtkParam_Pc.GetLinesWithId(noFpAttackParamIds);
            noFpAttackParamLines.Operate( new OperateIntField(dmgCorrectionFieldindexes , Operation.MULTIPLY, 0.75f));

            //change the names of the Attack Params to be a bit more discriptive
            for (int i = 0; i < noFpAttackParamLines.lines.Count; i++)
            {
                var line = noFpAttackParamLines.lines[i];

                string aowName = "[AOW] " + noFPLines.lines[i].name;
                aowName = aowName.Replace("[Ash of War]", "");
                line.Operate(new SetFieldTo(1, aowName), new Condition.NameIs(""));

                string aowName2 = line.name + " - No FP";
                line.Operate(new SetFieldTo(1, aowName2), new Condition.NameIs(aowName).IsFalse);
            }

            Lines fPLines = BehaviorParam_PC.GetLinesOnCondition(new Condition.HasInName(" - FP"));
            string[] fpAttackParamIds = fPLines.GetFields(BehaviorParam_PC.GetFieldIndex("refId"));

            //multiply the damage of fpLines.
            Lines fpAttackParamLines = AtkParam_Pc.GetLinesWithId(fpAttackParamIds);
            fpAttackParamLines.Operate(new OperateIntField(dmgCorrectionFieldindexes, Operation.MULTIPLY, 1.5f));
            
            for (int i = 0; i < fpAttackParamLines.lines.Count; i++)
            {
                var line = fpAttackParamLines.lines[i];

                string aowName = "[AOW] " + noFPLines.lines[i].name;
                aowName = aowName.Replace(" [Ash of War]", "");
                line.Operate(new SetFieldTo(1, aowName), new Condition.NameIs(""));

                string aowName2 = line.name + " - FP";
                line.Operate(new SetFieldTo(1, aowName2), new Condition.NameIs(aowName).IsFalse);
            }
        }
        static void weaponArtsCostMoreMana()
        {
            if (!IsRunningParamFile(SwordArtsParam))
                return;
            string[] fpCostsFieldNames = new string[] { "useMagicPoint_L1", "useMagicPoint_L2", "useMagicPoint_R1", "useMagicPoint_R2" };
            int[] fpCostFieldIndexes = SwordArtsParam.GetFieldIndexes(fpCostsFieldNames);
            {
                var indexRef = new IntRef(-1);
                var nonZero = new Condition.FloatFieldCompare(-1,Condition.GREATER_THAN, 0f);
                var noDamageTypeConditon = new Condition.FloatFieldBetween(0, 600, 850, true);
                var l2IsGreatest = new Condition.FloatFieldCompare(fpCostFieldIndexes[1], Condition.GREATER_THAN, new FloatFieldRef(fpCostFieldIndexes[2]))
                    .AND(new Condition.FloatFieldCompare(fpCostFieldIndexes[1], Condition.GREATER_THAN, new FloatFieldRef(fpCostFieldIndexes[3])));
                var l2IGAndIsL2 = l2IsGreatest.AND(new Condition.FloatCompare(indexRef, Condition.EQUAL_TO, fpCostFieldIndexes[1]));
                var notL2IGAndIsR1OrR2 = l2IsGreatest.IsFalse.AND(new Condition.FloatCompare(indexRef, Condition.EQUAL_TO, fpCostFieldIndexes[2]).OR(fpCostFieldIndexes[3]));
                var addPlus = l2IGAndIsL2.OR(notL2IGAndIsR1OrR2);

                //var bools = new string[fpCostFieldIndexes.Length][];

                for (int i = 0; i < fpCostFieldIndexes.Length; i++)
                {
    
                    int index = fpCostFieldIndexes[i];
                    indexRef.SetTo(index);
                    nonZero.field = index;

                    var toAdd1 = new FloatFieldRef(index).MIN(10).MAX(2).MULTIPLY(0.5f).PLUS(new FloatFieldRef(index).MIN(20).MULTIPLY(0.25f));

                    //bools[i] = Util.GetBoolsConditionPass(SwordArtsParam.lines, nonZero.AND(notL2IGAndIsR1OrR2).AND(noDamageTypeConditon.IsFalse) );
                    
                    SwordArtsParam.Operate(new OperateIntField(index).PLUS(toAdd1), nonZero.AND(addPlus).AND(noDamageTypeConditon.IsFalse), true);
                    SwordArtsParam.Operate(new OperateIntField(index).MULTIPLY(2f), nonZero.AND(noDamageTypeConditon.IsFalse));

                    var toAdd2 = new FloatFieldRef(index).MIN(5).MAX(2).MULTIPLY(1f).PLUS(new FloatFieldRef(index).MIN(20).DIVIDED(4f));
                    SwordArtsParam.Operate(new OperateIntField(index).PLUS(toAdd2), nonZero.AND(noDamageTypeConditon), true);
                    SwordArtsParam.Operate(new OperateIntField(index).MULTIPLY(1.5f), nonZero.AND(noDamageTypeConditon));
                }

                var stompIdsCondition = new Condition.NameIs(new string[] { "Storm Stomp", "Hoarfrost Stomp" });
                SwordArtsParam.Operate(new OperateIntField(fpCostFieldIndexes[1]).MULTIPLY(0.5f), stompIdsCondition);

                //
                /*var before = SwordArtsParam.vanillaLines.GetFields(fpCostFieldIndexes);
                var after = SwordArtsParam.GetFields(fpCostFieldIndexes);

                var fieldnames = new string[][] { SwordArtsParam.GetFields(0), SwordArtsParam.GetFields(1),fpCostsFieldNames };
                Util.PrintFieldsCompare(fieldnames," ", before, " to ", Util.Append(after,"   ",Util.GridifyStrings(bools)),"", "-1 0");*/

            }
        }
        static void spellsCostMoreMana()
        {
            if (!IsRunningParamFile(Magic))
                return;
            var mpCostFieldIndexes = Magic.GetFieldIndexes(new string[] { "mp", "mp_charge" });
            Magic.Operate(new OperateIntField(mpCostFieldIndexes).MULTIPLY(2.5f));
        }
        static void changeMindToBeLinear()
        {
            if (!IsRunningParamFile(CalcCorrectGraph))
                return;
            const float baseAmount = 40;
            const float amountPerLevel = 3.5f;
            //40 + 3.5 + (3.5 * (99 - 1))adjPt_maxGrowVal0
            var stageLevelValues = CalcCorrectGraph.GetFieldIndexes(new string[]{ "stageMaxVal0", "stageMaxVal1", "stageMaxVal2", "stageMaxVal3", "stageMaxVal4" });
            var stageGrowthValues = CalcCorrectGraph.GetFieldIndexes(new string[] { "stageMaxGrowVal0", "stageMaxGrowVal1", "stageMaxGrowVal2", "stageMaxGrowVal3", "stageMaxGrowVal4"});
            var adjValues = CalcCorrectGraph.GetFieldIndexes(new string[] { "adjPt_maxGrowVal0", "adjPt_maxGrowVal1", "adjPt_maxGrowVal2", "adjPt_maxGrowVal3", "adjPt_maxGrowVal4" });

            Line FPScalingLine = CalcCorrectGraph.GetLineWithName("FP Scaling - Mind");

            FPScalingLine.Operate(new SetFieldTo(stageLevelValues[2], "30"));

            for (int i = 0; i < stageGrowthValues.Length; i++)
            {
                float val = float.Parse(FPScalingLine.GetField(stageLevelValues[i]));
                val = baseAmount + amountPerLevel + (amountPerLevel * (val - 1));
                FPScalingLine.Operate(new SetFloatFieldTo(stageGrowthValues[i], val ));
            }
            for (int i = 0; i < adjValues.Length; i++)
            {
                FPScalingLine.Operate(new SetFloatFieldTo(adjValues[i], 1f));
            }



        }

        static void manaGradualRegenOnCast()
        {
            if (!IsRunningParamFile(new ParamFile[] { Magic, SpEffectParam }))
                return;

            Line FPRegenBaseLine = SpEffectParam.GetLineWithName("[Sorcery] Scholar's Armament").Copy()
                    .SetField("cycleOccurrenceSpEffectId", -1)
                    .SetField("effectTargetOpposeTarget", true)
                    .SetField("effectTargetFriendlyTarget", true)
                    .SetField("vfxId", -1)
                    .SetField("effectEndurance", 0.0f)
                    .SetField("motionInterval", 0.0f)
                    ;

            int[] magicRefIdFIs = Magic.GetFieldIndexesContains("refId");
            int[] magicRefCategoryFIs = Magic.GetFieldIndexesContains("refCategory");
            int[] consumetTypeFIs = Magic.GetFieldIndexesContains("consumeType");
            int mp = Magic.GetFieldIndex("mp");

            int effectEndurance = SpEffectParam.GetFieldIndex("effectEndurance");
            int motionInterval = SpEffectParam.GetFieldIndex("motionInterval");
            int changeMpRate = SpEffectParam.GetFieldIndex("changeMpRate");
            int changeMpPoint = SpEffectParam.GetFieldIndex("changeMpPoint");
            int spCategory = SpEffectParam.GetFieldIndex("spCategory");//10 stacks//20 reset time.

            const int maxFP = 386;
            const float regenRatePercent = 0.5f;
            const float ableToRegenPercent = 1.0f;
            const float secondsPerTic = 1;
            const float regenPerSec = 3;


            var NpcLineCondition = new Condition.NameStartsWith("[Npc]");
            var isMagicCondition = new Condition.HasInName("[Sor]").OR(new Condition.HasInName("[Inc]"));
            foreach (Line magicLine in Magic.lines)
            {
                if (NpcLineCondition.Pass(magicLine))
                    continue;
                if (!isMagicCondition.Pass(magicLine))
                    continue;
                var lineNameID = magicLine._idName;
                var spellName = magicLine.name;
                spellName = spellName.Substring(spellName.IndexOf("]") + 1);
                magicLine.GetFirstFieldIndexOnCondition(magicRefIdFIs, new Condition.FieldEqualTo("-1"), out int magicArrayIndex);
                if(magicArrayIndex == -1)
                {
                    Util.println(magicLine._idName + " no free slots found.. skipping");
                    continue;
                }
                int magicRefIdFI = magicRefIdFIs[magicArrayIndex];
                int magicRefCategoryFI = magicRefCategoryFIs[magicArrayIndex];
                int consumeTypeFI = consumetTypeFIs[magicArrayIndex];
                int spEffectID = -1;
                {
                    //spEffectID = FPRegenBaseLine.GetNextFreeId();
                    {
                        Line l = SpEffectParam.GetLineOnCondition(new Condition.HasInName(spellName));
                        if (l == null)
                            spEffectID = SpEffectParam.GetNextFreeId(int.Parse(magicLine.id+"000"), true);
                        else
                            spEffectID = l.GetNextFreeId();

                    }
                    int fpCost = int.Parse(magicLine.GetField(mp));

                    float regenTime = fpCost / regenPerSec;
                    int totalTics = 1 + (int)(regenTime / (1 / secondsPerTic));

                    float amountToPointRegen = ableToRegenPercent * fpCost * (1 - regenRatePercent);
                    int finalRegenPoint = (int)(amountToPointRegen / totalTics);

                   

                    float amountRemainder = ((fpCost * ableToRegenPercent) - (finalRegenPoint *  totalTics ));
                    float maxAmountToRateRegen = amountRemainder * (100f / maxFP); 
                    //float maxAmountToRateRegen = fpCost * regenRatePercent * ableToRegenPercent * (1 / maxFP);
                    float finalRegenRate = maxAmountToRateRegen * secondsPerTic / regenTime;

                    Line newFPLine = FPRegenBaseLine.Copy()
                        .SetField(0, spEffectID)
                        .SetField(1, magicLine.name + " - gradual FP regen")
                        .SetField(changeMpPoint, -finalRegenPoint)
                        .SetField(changeMpRate, -finalRegenRate)
                        .SetField(spCategory, 10) //stacks.
                        .SetField(effectEndurance, regenTime)
                        .SetField(motionInterval, secondsPerTic)
                        ;
                    SpEffectParam.OverrideOrAddLine(newFPLine);

                }//create SP Effect Line.

                magicLine.SetField(magicRefIdFI, spEffectID);
                magicLine.SetField(magicRefCategoryFI, 2);
                magicLine.SetField(consumeTypeFI, 2); //None


            }

        }
        static void manaRegenOnHit()
        {
            if (!IsRunningParamFile(new ParamFile[] { AtkParam_Pc, SpEffectParam, Magic } ))
                return;
            //every wwapon has a mana on hit.

            //Option 1
            //Add mana regen effect to all weapons.
            //get the effect found in the bucher cleaver and change it from HP to FP.
            //Make an effect for all weapon stypes and match to the weapn type accordingly

            //Flaws: Attacks all recover same amount of FP. Unbalnaced weapon arts.

            //Option 2
            //Add mana regen effect to all necessery attacks.
            //copy blood tax FP effect and add ir to the attack




            // Add weapon information to the dictionary
            
            var weaponTypeInfo = new Dictionary<string, float>();
            {
                weaponTypeInfo.Add("Dagger", 1.0f);
                weaponTypeInfo.Add("Straight Sword", 1.2f);
                weaponTypeInfo.Add("Greatsword", 2.5f);
                weaponTypeInfo.Add("Colossal Sword", 4.5f);
                weaponTypeInfo.Add("Thrusting Sword", 1.1f);
                weaponTypeInfo.Add("Heavy Thrusting Sword", 1.5f);
                weaponTypeInfo.Add("Curved Sword", 1.3f);
                weaponTypeInfo.Add("Curved Greatsword", 2.0f);
                weaponTypeInfo.Add("Katana", 1.8f);
                weaponTypeInfo.Add("Twinblade", 2.2f);
                weaponTypeInfo.Add("Axe", 1.7f);
                weaponTypeInfo.Add("Greataxe", 3.0f);
                weaponTypeInfo.Add("Hammer", 1.6f);
                weaponTypeInfo.Add("Flail", 2.0f);
                weaponTypeInfo.Add("Warhammer", 2.5f);
                weaponTypeInfo.Add("Colossal Weapon", 4.5f);
                weaponTypeInfo.Add("Spear", 1.4f);
                weaponTypeInfo.Add("Lance", 1.9f);
                weaponTypeInfo.Add("Halberd", 2.3f);
                weaponTypeInfo.Add("Scythe", 1.8f);
                weaponTypeInfo.Add("Whip", 1.2f);
                weaponTypeInfo.Add("Fist", 1.0f);
                weaponTypeInfo.Add("Claw", 1.1f);
                weaponTypeInfo.Add("Torch", 1.0f);
                weaponTypeInfo.Add("Small Shield", 0.8f);
                weaponTypeInfo.Add("Medium Shield", 1.1f);
                weaponTypeInfo.Add("Greatshield", 1.5f);
                weaponTypeInfo.Add("Sorcery Catalyst", 0.5f);
                weaponTypeInfo.Add("Incantation Catalyst", 0.7f);
                weaponTypeInfo.Add("Bow", 0.9f);
                weaponTypeInfo.Add("Crossbow", 1.0f);
                weaponTypeInfo.Add("Greatbow", 1.8f);
                weaponTypeInfo.Add("Ballista", 3.5f);
            }

            const float baseFPRegenPoint = 8;
            const float baseFPRegenRate = 1.3f;

            Line FPRegenBaseLine = SpEffectParam.vanillaParamFile.GetLineWithId(1831).Copy(SpEffectParam);
            FPRegenBaseLine.SetField("changeHpRate", 1.0f).SetField("changeHpPoint", 0).SetField("vfxId", -1);

            var spEffectFieldIndexes = AtkParam_Pc.GetFieldIndexesContains("spEffectId");

            Dictionary<int, int> fpRegenSpEffectDictionary = new Dictionary<int, int>();




            foreach (var de in weaponTypeInfo)
            {
                string name = de.Key;
                //Lines WeaponTypeLines = AtkParam_Pc.
                //    GetLinesOnCondition(
                //    new Condition.HasInName("Default - "+name)
                //    //.AND(new Condition.HasInName(name))
                //    );

                Lines WeaponTypeLines = null;
                {
                    Line WeaponTypeLine = AtkParam_Pc.
                        GetLineOnCondition(
                        new Condition.HasInName("Default - " + name)
                        //.AND(new Condition.HasInName(name))
                        );

                    if (WeaponTypeLine == null)
                    {
                        if (RunSettings.CanDebugInsideFunction)
                        {
                            Util.println("Weapon Type " + name + " not found");
                        }
                        continue;
                    }

                    string requiredIDStart = WeaponTypeLine.id.Substring(0, WeaponTypeLine.id.Length - 5);

                    List<Line> selectedLines = new List<Line>();
                    bool isFound = false;
                    foreach(Line l in AtkParam_Pc.lines)
                    {

                        if (l.id.Length > 5 && l.id.Substring(0, l.id.Length - 5) == requiredIDStart)
                        {
                            isFound = true;
                            selectedLines.Add(l);
                        }
                        else if (isFound)
                        {
                            break;
                        }
                    }
                    WeaponTypeLines = new Lines(selectedLines);
                }



                foreach (Line line in WeaponTypeLines.lines)
                {
                    int fieldIndex = line.GetFirstFieldIndexOnCondition(spEffectFieldIndexes, new Condition.FieldCondition.FieldIs(-1, "-1"));

                    if(fieldIndex == -1)
                    {
                        if(RunSettings.CanDebugInsideFunction)
                            Util.println("AtkParam_Pc;" + line.id + "; " + line.name + "  does not have a free spEffect Slot.");
                        continue;
                    }

                    //get ot create fpEffect
                    float attackCorrection = float.Parse(line.GetField("atkPhysCorrection"))/100;
                    int FPRegenPoint = (int)(de.Value * baseFPRegenPoint * attackCorrection);
                    float FPRegenRate = de.Value * baseFPRegenRate * attackCorrection;

                    if (FPRegenPoint == 0 && FPRegenRate == 0)
                        continue;

                    int fpRegenSpEffectID;


                    if (fpRegenSpEffectDictionary.ContainsKey(FPRegenPoint))
                    {
                        fpRegenSpEffectID = fpRegenSpEffectDictionary[FPRegenPoint];
                    }
                    else {
                        fpRegenSpEffectID = FPRegenBaseLine.GetNextFreeId();

                        Line FPRegenLine = FPRegenBaseLine.Copy()
                            .SetField(0, fpRegenSpEffectID)
                            .SetField(1,"gain " +FPRegenPoint +  " FP AND " +FPRegenRate + " percent FP on Hit" )
                            .SetField("changeMpRate", -FPRegenRate)
                            .SetField("changeMpPoint", -FPRegenPoint)
                        ;
                        SpEffectParam.OverrideOrAddLine(FPRegenLine);
                        fpRegenSpEffectDictionary.Add(FPRegenPoint, fpRegenSpEffectID);
                    }
                    line.SetField(fieldIndex, fpRegenSpEffectID);
                }
            }

        }

        static void reduceMaxManaSystem()
        {
            if (!IsRunningParamFile(new ParamFile[] { Magic, Bullet, SpEffectParam }))
                return;
            var healingSpellIds = new int[] {
            6420, 6421, 6422, 6423, 6424
            };
            var bodyBuffSpellIds = new int[] {
            6050,
            6060,
            6270,
            6330,
            6340,
            6430,
            6431,
            6450,
            6460,
            6470,
            6480,
            6490,
            6510, //Assasins Aproach
            6600,
            6850, //Bestial Vitality
            6860,
            6970,
            6971,
            7903,
            };
            var weaponBuffSpellIds = new int[] {
            4460,
            4490,
            4660,   //UnseenBlade
            6260,
            6320,
            6770,
            6960
            };
            var shieldBuffSpellIds = new int[] {
            4470,
            6740,
            };
            var aidSpellIds = new int[] {
            4480,
            6040,
            6440,
            6441,
            6730,
            6780,
            };

            Lines bodyBuffs = Magic.GetLinesOnCondition(new Condition.IDCheck(bodyBuffSpellIds));
            Lines weaponBuffs = Magic.GetLinesOnCondition(new Condition.IDCheck(weaponBuffSpellIds));
            Lines shieldBuffs = Magic.GetLinesOnCondition(new Condition.IDCheck(shieldBuffSpellIds));
            Lines healingSpells = Magic.GetLinesOnCondition(new Condition.IDCheck(healingSpellIds));
            Lines aidSpells = Magic.GetLinesOnCondition(new Condition.IDCheck(aidSpellIds));
            Lines spells;
            {
                var spellLines = new List<Line>();
                spellLines.AddRange(bodyBuffs.lines);
                spellLines.AddRange(weaponBuffs.lines);
                spellLines.AddRange(shieldBuffs.lines);
                spellLines.AddRange(healingSpells.lines);
                spellLines.AddRange(aidSpells.lines);
                spells = new Lines(spellLines);
            }

            //List<Line> bulletLines = new List<Line>();
            //List<Line> spEffectLines = new List<Line>();

            var refCategoryIndexes = Magic.GetFieldIndexesContains("refCategory");
            var refIdIndexes = Magic.GetFieldIndexesContains("refId");

            var spEffectIdIndexes = Bullet.GetFieldIndexesContains("spEffectId");
            int sfxId_BulletIndex = Bullet.GetFieldIndex("sfxId_Bullet");

            Line bulletLineBase;
            {
                bulletLineBase = Bullet.GetLineWithId("10642000").Copy();   //urgentheal bullet used as base
                bulletLineBase.SetField(spEffectIdIndexes[0], 0);
                bulletLineBase.SetField(spEffectIdIndexes[1], 0);
                bulletLineBase.SetField(sfxId_BulletIndex, -1);
            }

            var effectEnduranceFI = SpEffectParam.GetFieldIndex("effectEndurance");
            var motionIntervalFI = SpEffectParam.GetFieldIndex("motionInterval");
            var eraseOnBonfireRecoverFI = SpEffectParam.GetFieldIndex("eraseOnBonfireRecover");
            var wepParamChangeFI = SpEffectParam.GetFieldIndex("wepParamChange");
            var addWillpowerStatusFI = SpEffectParam.GetFieldIndex("addWillpowerStatus");
            var cycleOccurrenceSpEffectIdFI = SpEffectParam.GetFieldIndex("cycleOccurrenceSpEffectId");
            var spCategoryFI = SpEffectParam.GetFieldIndex("spCategory");
                //10 stacks
                //20 reset time.


            Line fpReductionLineBase;
            {
                //Scholar's Armament
                fpReductionLineBase = SpEffectParam.GetLineWithName("[Sorcery] Scholar's Armament").Copy()
                    .SetField(cycleOccurrenceSpEffectIdFI, -1)
                    .SetField("effectTargetOpposeTarget", true)
                    .SetField("effectTargetFriendlyTarget", true)
                    .SetField("vfxId", -1)
                    .SetField("effectEndurance", 0.0f)
                    .SetField("motionInterval", 0.0f)
                    ;
                    
            }
            bool forceCreateBullet = false;
            for (int i = 0; i < spells.Length; i++)
            {
                //Console.WriteLine("3.1" + spells.lines[i]._idName);
                Line l = spells.lines[i];
                    int refCategoryFieldIndex = -1;
                    int refIdFieldIndex;
                Line spEffectLine;
                    int fpReductionSpEffectID;

                bool isBuff = !(healingSpells.lines.Contains(l) || aidSpells.lines.Contains(l));
                bool isCycle = isBuff;
                {
                    
                    string spellName = l.name;
                    //    spellName = spellName.Remove(0, l.name.IndexOf(']') + 2);   // deletes "[Incantation] "
                    float fpReductionMult = 1;
                    if (healingSpells.Contains(l))
                        fpReductionMult = 0.5f;
                    if (aidSpells.Contains(l))
                        fpReductionMult = 0.3f;

                    //looks for spEffect refrence inside the Magic Param;
                    int refIndex = -1;
                    if (!forceCreateBullet)
                        refCategoryFieldIndex = l.GetFirstFieldIndexOnCondition(refCategoryIndexes, new Condition.FloatFieldCompare(Condition.EQUAL_TO, 2), out refIndex);
                    
                    if (forceCreateBullet || refCategoryFieldIndex == -1)   //no speffect found. 
                    {
                        Line bulletLine;
                        int spEffectFieldIndex;
                        bool bulletCreated = false;

                        //looks for a bullet refrence inside the Magic Param
                        if (!forceCreateBullet)
                            refCategoryFieldIndex = l.GetFirstFieldIndexOnCondition(refCategoryIndexes, new Condition.FloatFieldCompare(Condition.EQUAL_TO, 1), out refIndex);//bullet
                        if (forceCreateBullet || refCategoryFieldIndex == -1)//||isOffensiveBulletSpell      //no bullet found.
                        {
                            refIdFieldIndex = l.GetFirstFieldIndexOnCondition(refIdIndexes, new Condition.FloatFieldCompare(Condition.EQUAL_TO, -1), out refIndex);//empty;
                            refCategoryFieldIndex = refCategoryIndexes[refIndex];

                            //create bullet
                            bulletLine = bulletLineBase.Copy(); //copying urgent heal bullet line as the base.
                            bulletLine.SetField(0, Bullet.GetNextFreeId(int.Parse("10" + l.id + "00"), true)); //creates and sets an id using the convention.
                            Bullet.OverrideOrAddLine(bulletLine);   //adds the line to Bullet ParamFile.
                            bulletLine.SetField(1, spellName + " Bullet [MAX_FP_REDU. X"+fpReductionMult+" ]");

                            //assign it the MagicParam
                            l.SetField(refCategoryFieldIndex, 1);//set it the category to bullet.
                            l.SetField(refIdFieldIndex, bulletLine.id);//set the refID to the bullet we just created.

                            bulletCreated = true;
                            forceCreateBullet = false;

                        }
                        else     //bullet found
                        {
                            refIdFieldIndex = refIdIndexes[refIndex];
                            bulletLine = Bullet.GetLineWithId(l.GetField(refIdFieldIndex)); //gets the bullet line.
                        }
                        //bulletLines.Add(bulletLine);

                                                
                        //get Full SpEffect Slot
                        spEffectFieldIndex = bulletLine.GetFirstFieldIndexOnCondition(spEffectIdIndexes, new Condition.FloatFieldCompare(Condition.NOT_EQUAL_TO, 0));//full
                        if (spEffectFieldIndex == -1)
                        //bullet is out of empty spEffect slots. use case: Law of Regression. 
                        //repeat this loop but its forced to create a new bullet.
                        {
                            if (RunSettings.CanDebugInsideFunction)
                                Util.println("NO SPEFFECT FOUND IN \n   Bullet." + bulletLine.id + ";" + bulletLine.name + "\n   Magic." + l.id + ";" + l.name + "    Skipping...");
                            continue;
                        }
                        //sets the spEffect to be the slot we found.
                        spEffectLine = SpEffectParam.GetLineWithId(bulletLine.GetField(spEffectFieldIndex));
                        fpReductionSpEffectID = spEffectLine.GetNextFreeId();
                        if (RunSettings.CanDebugInsideFunction)
                            Util.println(l.name + " FOUND" +"\n   spEffectID: " + spEffectLine.id +"\n   Bullet."+ bulletLine.id + ";" + bulletLine.name + "\n   Magic." + l.id + ";" + l.name );


                        //get empty SpEffect Slot.
                        spEffectFieldIndex = bulletLine.GetFirstFieldIndexOnCondition(spEffectIdIndexes, new Condition.FloatFieldCompare(Condition.EQUAL_TO, 0));//empty
                        if (spEffectFieldIndex == -1)
                        //bullet is out of empty spEffect slots. use case: Law of Regression. 
                        //repeat this loop but its forced to create a new bullet.
                        {
                            if (RunSettings.CanDebugInsideFunction)
                                Util.println("NO EMPTY SPEFFECT SLOT IN \n   Bullet." + bulletLine.id + ";" + bulletLine.name + "\n   Magic." + l.id + ";" + l.name + "    Retrying...");
                            forceCreateBullet = true;
                            i--;
                            continue;
                        }
                        if(!bulletCreated)
                            bulletLine.SetField(1, spellName + " Bullet [BASE_EFFECT + MAX_FP_REDU. X" + fpReductionMult + " ]");
                        bulletLine.SetField(spEffectFieldIndex, fpReductionSpEffectID);
                    }
                    else 
                    {
                        refIdFieldIndex = refIdIndexes[refIndex];
                        spEffectLine = SpEffectParam.GetLineWithId(l.GetField(refIdFieldIndex));
                        //Util.println("refindex " + refIndex + "   " + Magic.header[refIdFieldIndex] +"   "+ l.GetField(refIdFieldIndex) + "   isnull: "+  (spEffectLine == null));
                        fpReductionSpEffectID = spEffectLine.GetNextFreeId();
                       // Util.println("fpReductionSpEffectID " + fpReductionSpEffectID);

                    }
                    Line fpReduction = fpReductionLineBase.Copy();
                    fpReduction.SetField(0, fpReductionSpEffectID);
                    if (isCycle) {

                        fpReduction.SetField(1, spellName + " [CYCLE][MAX_FP_REDU. X" + fpReductionMult + " ]");
                        Line spCycle = spEffectLine;

                        while (spCycle.GetField(cycleOccurrenceSpEffectIdFI) != "-1")
                        {
                            spCycle = SpEffectParam.GetLineWithId(spCycle.GetField(cycleOccurrenceSpEffectIdFI));
                            //Console.WriteLine(spCycle._idName + " cycleOccoranceIdFI: "+ spCycle.GetField(cycleOccurrenceSpEffectIdFI));
                        }
                        spCycle.SetField(motionIntervalFI, 0.06f);
                        spCycle.SetField(cycleOccurrenceSpEffectIdFI, fpReduction.id);
                        fpReduction.SetField(spCategoryFI, 20); //reset time
                        fpReduction.SetField(effectEnduranceFI, 0.1f);

                        //spCycle.PrintFields(0, 1, motionIntervalFI, cycleOccurrenceSpEffectIdFI);
                        //Util.println();
                        //fpReduction.PrintFields(0, 1, spCategoryFI, effectEnduranceFI);
                        //Util.println();
                    }
                    else
                    {
                        fpReduction.SetField(1, spellName + " [MAX_FP_REDU.x" + fpReductionMult + "]");
                        fpReduction.SetField(spCategoryFI, 10); //stacking effect
                        fpReduction.SetField(effectEnduranceFI, "-1.000");
                    }
                    {
                        //wepParamChange
                        int wepParamInt = 3;    //body
                        if (weaponBuffs.Contains(l))
                            wepParamInt = 1;    //right hand
                        else if (shieldBuffs.Contains(l))
                            wepParamInt = 2;    //left hand
                        fpReduction.SetField(wepParamChangeFI, wepParamInt);
                    }
                    {
                        //eraseOnBonfireRecover
                        spEffectLine.SetField(eraseOnBonfireRecoverFI, 1);
                        fpReduction.SetField(eraseOnBonfireRecoverFI, 1);
                    }
                    {
                        //addWillpowerStatus
                        const float amountPerLevel = 3.5f;
                        int mindToReduce = (int)((int.Parse(l.GetField(Magic.GetFieldIndex("mp"))) / amountPerLevel) * fpReductionMult);
                        fpReduction.SetField(addWillpowerStatusFI, -mindToReduce);
                    }
                    //Util.println("adding line " + fpReduction.id + " : " + fpReduction.name );
                    
                    SpEffectParam.OverrideOrAddLine(fpReduction);
                    

                }

            }



        }

        static bool IsRunningParamFile(ParamFile file)
        {
            if (RunSettings.RunVanilla)
                return false;
            if (RunSettings.ToRun == null)
                return RunSettings.RunIfNull;
            if (RunSettings.ToRun == file)
                return true;
            return false;
        }
        static bool IsRunningParamFile(ParamFile[] file)
        {
            if (RunSettings.RunVanilla)
                return false;
            if (RunSettings.ToRun == null)
                return RunSettings.RunIfNull;
            if (file.Contains(RunSettings.ToRun))
                return true;
            return false;
        }












    }
}





