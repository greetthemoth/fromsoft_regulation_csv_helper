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
        static void RunOverride_CreateSmithingStoneMod()
        {
            RunSettings.Testing = false;
            RunSettings.Write_OnlyModifiedLines = true;
            RunSettings.RunIfNull = true;

            //RunSettings.RunIfNull = false;  //dont re create files



            //worldChangesPlus(false,true); //testing


            const bool CREATE_SOFT_RANDOMIZER = false;

            string exportDirectory;

            Line emptyItemLot = ItemLotParam_enemy.vanillaParamFile.GetLineWithId(460000500).Copy(ItemLotParam_enemy).SetField(1, "");

            var DropMultsToWrite = new float[]
            {
                1,
                //1.5f,2f,3f,5f
            };

            /*exportDirectory = @"C:\CODING OUTPUT\CSV\Individual Options (slower)\RuneDrops (import first)";
            ParamFile.ImportCSVs(exportDirectory);
            ItemLotParam_enemy.PrintCompareLineChanges(emptyItemLot);
            Util.println(ItemLotParam_enemy.VerifyFieldCounts());*/
            {
                exportDirectory = @"C:\CODING OUTPUT\CSV\Individual Options (slower)";
                Keyword.IfModifiedSet_ON = true;
                string ChangeKeyword;
                //Condition WriteCond;


                string individual_RuneDropsDirectory = "";

                const bool MATERIAL_DROP_MULT_OPTIONS = true;
                var individual_MaterialDropsDirectory = "";
                for (int i = 0; i < DropMultsToWrite.Length && (MATERIAL_DROP_MULT_OPTIONS || i == 0); i++)
                {
                    RunSettings.Write_directory = exportDirectory + @"\MaterialDrops";
                    string multString = "";
                    if (MATERIAL_DROP_MULT_OPTIONS)
                    {
                        multString = "x" + DropMultsToWrite[i];
                        string multDirString = @"\MaterialDrops\" + multString + @" Mats";
                        if (DropMultsToWrite[i] == 1)
                        {
                            //multDirString += "(recommended)";
                            //multDirString += "";//"(recommended)";
                            multDirString = @"\MaterialDrops\Default";
                            multString = "";
                            individual_MaterialDropsDirectory = exportDirectory + multDirString;
                        }
                        else if (DropMultsToWrite[i] == 1.5f)
                            multDirString += "";
                        RunSettings.Write_directory = exportDirectory + multDirString;
                    }
                    ChangeKeyword = "!Material Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondMatDrops = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                    enemyDrops_IncreasedMaterialDrops(DropMultsToWrite[i]);
                    Keyword.IfModifiedSet_ON = false;
                    enemyDrops_MoreSmithingStoneDrops(false, false, 1);
                    Keyword.IfModifiedSet_ON = true;
                    ParamFile.WriteModifiedFiles("", "__" + multString + "MatDrops", WriteCondMatDrops);
                    ParamFile.RevertAll(true);
                }

                //FirstTImeEquipDrop
                var individual_FTEDDirectory = "";
                {
                    ChangeKeyword = "!FirstTime Equipment Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondFTED = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                    enemyDrops_OneTimeEquipmentDrops(true);
                    Keyword.IfModifiedSet_ON = false;
                    enemyDrops_MoreSmithingStoneDrops(false, false, 1);
                    Keyword.IfModifiedSet_ON = true;
                    RunSettings.Write_directory = exportDirectory + @"\Equipment Drops\First Time Drops";
                    individual_FTEDDirectory = RunSettings.Write_directory;
                    ParamFile.WriteModifiedFiles("", "__" + "FTED", WriteCondFTED);
                    ParamFile.RevertAll(true);
                }
                ChangeKeyword = "!Material Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); //var WriteCondMatDrops = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                enemyDrops_IncreasedMaterialDrops();
                ChangeKeyword = "!Soft Item Randomizer!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondSIR = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                if (CREATE_SOFT_RANDOMIZER) worldChangesPlus(true, false);
                ChangeKeyword = "!Add Roundtable Items!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondARI = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                if (CREATE_SOFT_RANDOMIZER) worldChangesPlus(false, true);
                ChangeKeyword = "!Unupgrade NPC Weap!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondUNW = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                noupgradedweaponsFromNpcs();
                ChangeKeyword = "!OneTime Equipment Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondOTED = new Condition.OneKeywordPassesCondition(new KeywordCondition.Is(ChangeKeyword));
                enemyDrops_OneTimeEquipmentDrops();

                Keyword.IfModifiedSet_ON = false;
                enemyDrops_MoreSmithingStoneDrops(true, true, 1);
                Keyword.IfModifiedSet_ON = true;

                //RunSettings.Write_directory = exportDirectory + @"\MaterialDrops";
                //ParamFile.WriteModifiedFiles("", "__" + "MatDrops", WriteCondMatDrops);
                RunSettings.Write_directory = exportDirectory + @"\Other Changes\World Changes\Soft Item Randomizer";
                var individual_WCP_SoftItemRandomizerDirectory = RunSettings.Write_directory;
                if (CREATE_SOFT_RANDOMIZER) ParamFile.WriteModifiedFiles("", "__" + "WCP_SIR", WriteCondSIR);
                RunSettings.Write_directory = exportDirectory + @"\Other Changes\World Changes\Add Roundtable Items";
                var individual_WCP_AddRoundtableItemsDirectory = RunSettings.Write_directory;
                if (CREATE_SOFT_RANDOMIZER) ParamFile.WriteModifiedFiles("", "__" + "WCP_ARI", WriteCondARI);

                RunSettings.Write_directory = exportDirectory + @"\Other Changes\World Changes\Unupgrade NPC Weap";
                var individual_WCP_UnupgradeNPCWeapDirectory = RunSettings.Write_directory;
                ParamFile.WriteModifiedFiles("", "__" + "WCP_UNW", WriteCondUNW);

                RunSettings.Write_directory = exportDirectory + @"\Equipment Drops\One Time Drops";
                var individual_OTEDDirectory = RunSettings.Write_directory;
                ParamFile.WriteModifiedFiles("", "__" + "OTED", WriteCondOTED);

                ParamFile.RevertAll(true);

                const bool STONE_DROP_MULT_OPTIONS = true;
                //var individual_StoneDropsDirectory = "";
                var individual_StoneDropMultsDirectory = new string[DropMultsToWrite.Length];
                for (int i = 0; i < DropMultsToWrite.Length && (STONE_DROP_MULT_OPTIONS || i == 0); i++)
                {
                    RunSettings.Write_directory = exportDirectory + @"\StoneDrops (import first)";
                    string multString = "";
                    if (STONE_DROP_MULT_OPTIONS)
                    {
                        multString = "x" + DropMultsToWrite[i];
                        string multDirString = @"\StoneDrops\" + multString + @" Stones";
                        if (DropMultsToWrite[i] == 1)
                        {
                            //multDirString += "(recommended)";
                            //multDirString += "";//"(recommended)";
                            multDirString = @"\StoneDrops\Default";
                            multString = "";
                            //individual_StoneDropsDirectory = exportDirectory + multDirString;
                        }
                        else if (DropMultsToWrite[i] == 1.5f)
                            multDirString += "";
                        RunSettings.Write_directory = exportDirectory + multDirString;
                    }
                    individual_StoneDropMultsDirectory[i] = RunSettings.Write_directory;

                    Keyword.IfModifiedSet = new Keyword("!!!", 0, true);
                    enemyDrops_IncreasedMaterialDrops();
                    if (CREATE_SOFT_RANDOMIZER)
                    {
                        worldChangesPlus(true, false);
                        worldChangesPlus(false, true);
                    }
                    noupgradedweaponsFromNpcs();
                    enemyDrops_OneTimeEquipmentDrops();

                    foreach (ParamFile p in ParamFile.paramFiles)
                    {
                        foreach (Line line in p.lines)
                        {
                            if (!line.added && line.modified)
                            {
                                line.RevertFieldsToVanilla(true);
                            }
                            /*else if (line.added)//testo
                            {
                                Util.println(line._idName);
                            }*/
                        }
                    }

                    ChangeKeyword = "!Stone Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondStoneDrops = new KeywordCondition.Is(ChangeKeyword);
                    enemyDrops_MoreSmithingStoneDrops(false, true, DropMultsToWrite[i]);
                    //var individual_StoneDropsDirectory = RunSettings.Write_directory;
                    var emptyLineCond = new Condition.Func(Line.IsAdded).AND(new KeywordCondition.Is("!!!").AND(new KeywordCondition.Is("!VanillaItemLotCopy!").IsFalse));

                    //var testLine = ItemLotParam_enemy.GetLineWithId(410000206);
                    //var testPass = emptyLineCond.Pass(testLine);
                    //var debuggggy = testLine._idName;

                    ParamFile.WriteModifiedFiles("", "__" + multString + "StoneDrops", WriteCondStoneDrops, emptyLineCond, LotItem.newBaseItemLotLine(ItemLotParam_enemy));
                    ParamFile.RevertAll(true);
                }

                {
                    Keyword.IfModifiedSet = new Keyword("!!!", 0, true);
                    enemyDrops_IncreasedMaterialDrops();
                    if (CREATE_SOFT_RANDOMIZER)
                    {
                        worldChangesPlus(true, false);
                        worldChangesPlus(false, true);
                    }
                    noupgradedweaponsFromNpcs();
                    enemyDrops_OneTimeEquipmentDrops();

                    foreach (ParamFile p in ParamFile.paramFiles)
                    {
                        foreach (Line line in p.lines)
                        {
                            if (!line.added && line.modified)
                            {
                                line.RevertFieldsToVanilla();
                            }
                            /*else if (line.added)//testo
                            {
                                Util.println(line._idName);
                            }*/
                        }
                    }

                    ChangeKeyword = "!Rune Drops!"; Keyword.IfModifiedSet = new Keyword(ChangeKeyword, 0, true); var WriteCondRuneDrops = new KeywordCondition.Is(ChangeKeyword);
                    enemyDrops_MoreSmithingStoneDrops(true, false, 1);
                    RunSettings.Write_directory = exportDirectory + @"\RuneDrops (import first)";
                    individual_RuneDropsDirectory = RunSettings.Write_directory;
                    var emptyLineCond = new Condition.Func(Line.IsAdded).AND(new KeywordCondition.Is("!!!").AND(new KeywordCondition.Is("!VanillaItemLotCopy!").IsFalse));


                    //var testLine = ItemLotParam_enemy.GetLineWithId(463000204);
                    //var testPass = emptyLineCond.Pass(testLine);
                    //var debuggggy = testLine._idName;

                    ParamFile.WriteModifiedFiles("", "__" + "RuneDrops", WriteCondRuneDrops, emptyLineCond, LotItem.newBaseItemLotLine(ItemLotParam_enemy));
                    ParamFile.RevertAll(true);
                }
                string lessSmithingStonePickups_DirString = @"\Less Stone Pickups";
                RunSettings.Write_directory = exportDirectory + lessSmithingStonePickups_DirString;
                var individual_WorldChangesDirectory = RunSettings.Write_directory;
                //ShopLineupChanges(true, false, false, false);
                //var testLine = ItemLotParam_enemy.GetLineWithId(463000204);
                //var debuggggy = testLine._idName;
                reduceSmithingStonePickups();
                //AddedLineManager.CreateEmptyLines(emptyItemLot, "WorldChanges");
                ParamFile.WriteModifiedFiles("", "__LessStonePickups");
                //AddedLineManager.Catalog("WorldChanges");
                ParamFile.RevertAll(true);

                string ShopChanges_DirString = @"\Other Changes\Shop Changes\Vendor Stone Nerf";
                RunSettings.Write_directory = exportDirectory + ShopChanges_DirString;
                var individual_ShopChanges_StoneNerfDirectory = RunSettings.Write_directory;
                ShopLineupChanges(true, true, false, false);
                //AddedLineManager.CreateEmptyLines(emptyItemLot, "SC_VSN");
                ParamFile.WriteModifiedFiles("", "__" + "SC_VSN");
                //AddedLineManager.Catalog("SC_VSN");
                ParamFile.RevertAll(true);

                /*ShopChanges_DirString = @"\Other Changes\Shop Changes\TwinMaiden Stone Nerf";
                RunSettings.Write_directory = exportDirectory + ShopChanges_DirString;
                ShopLineupChanges(false, true, false, false);
                //AddedLineManager.CreateEmptyLines(emptyItemLot ,"SC_TMSN");
                ParamFile.WriteModifiedFiles("", "__" + "SC_TMSN");
                //AddedLineManager.Catalog("SC_TMSN");
                ParamFile.ResetAll();*/

                ShopChanges_DirString = @"\Other Changes\Shop Changes\Vendor Material Buff";
                RunSettings.Write_directory = exportDirectory + ShopChanges_DirString;
                var individual_ShopChanges_MaterialBuffDirectory = RunSettings.Write_directory;
                ShopLineupChanges(false, false, true, false);
                //AddedLineManager.CreateEmptyLines(emptyItemLot, "SC_VMB");
                ParamFile.WriteModifiedFiles("", "__" + "SC_VMB");
               /// AddedLineManager.Catalog("SC_VMB");
                ParamFile.RevertAll(true);

                ShopChanges_DirString = @"\Other Changes\Shop Changes\Misc Balancing";
                RunSettings.Write_directory = exportDirectory + ShopChanges_DirString;
                var individual_ShopChanges_MiscBalancingDirectory = RunSettings.Write_directory;
                ShopLineupChanges(false, false, false, true);
                //AddedLineManager.CreateEmptyLines(emptyItemLot, "SC_MB");
                ParamFile.WriteModifiedFiles("", "__" + "SC_MB");
                //AddedLineManager.Catalog("SC_MB");
                ParamFile.RevertAll(true);

                /*
                for (int i = 0; i < DropMultsToWrite.Length; i++)
                {
                    string multString = "x" + DropMultsToWrite[i];
                    string multDirString = @"\MaterialDrops\" + multString + @" Mats";
                    if (DropMultsToWrite[i] == 1)
                    {
                        multDirString += "(recommended)";
                        multString = "";
                    }
                    else if (DropMultsToWrite[i] == 1.5f)
                        multDirString += "";
                    RunSettings.Write_directory = exportDirectory + multDirString;
                    //ShopLineupChanges(false,false,true, false);
                    enemyDrops_IncreasedMaterialDrops(DropMultsToWrite[i]);
                    AddedLineManager.CreateEmptyLines(emptyItemLot, "MatDrops");
                    ParamFile.WriteModifiedFiles("", "__" + multString + "MatDrops");
                    AddedLineManager.Catalog("MatDrops");
                    ParamFile.RevertAll();
                }

                for (int i = 0; i < DropMultsToWrite.Length; i++)
                {
                    string multString = "x" + DropMultsToWrite[i];
                    string multDirString = @"\StoneDrops\" + multString + @" Stones";
                    if (DropMultsToWrite[i] == 1)
                    {
                        multDirString += "(recommended)";
                        multString = "";
                    }
                    else if (DropMultsToWrite[i] == 1.5f)
                        multDirString += "";
                    RunSettings.Write_directory = exportDirectory + multDirString;
                    enemyDrops_MoreSmithingStoneDrops(false, true, DropMultsToWrite[i]);
                    AddedLineManager.CreateEmptyLines(emptyItemLot, "StoneDrops");
                    ParamFile.WriteModifiedFiles("", "__" + multString + "StoneDrops");
                    AddedLineManager.Catalog("StoneDrops");
                    ParamFile.RevertAll();
                }
                return;
                */


                exportDirectory = @"C:\CODING OUTPUT\CSV\All In One";
                RunSettings.Write_directory = exportDirectory;
                ParamFile.ImportCSVs(individual_StoneDropMultsDirectory[0]);
                ParamFile.ImportCSVs(individual_RuneDropsDirectory);
                ParamFile.ImportCSVs(individual_OTEDDirectory);
                ParamFile.ImportCSVs(individual_WorldChangesDirectory);
                ParamFile.ImportCSVs(individual_MaterialDropsDirectory);
                ParamFile.ImportCSVs(individual_ShopChanges_MaterialBuffDirectory);
                ParamFile.ImportCSVs(individual_ShopChanges_MiscBalancingDirectory);
                ParamFile.ImportCSVs(individual_ShopChanges_StoneNerfDirectory);
                if (CREATE_SOFT_RANDOMIZER)
                {
                    ParamFile.ImportCSVs(individual_WCP_AddRoundtableItemsDirectory);
                    ParamFile.ImportCSVs(individual_WCP_SoftItemRandomizerDirectory);
                }
                ParamFile.ImportCSVs(individual_WCP_UnupgradeNPCWeapDirectory);
                ParamFile.WriteModifiedFiles("", "__" + "AllInOne");
                ParamFile.RevertAll(true);

                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\CODING\Noftifications\chord.wav");
                player.Play();

                exportDirectory = @"C:\CODING OUTPUT\CSV\Choose Options";
                {

                    for (int i = 0; i < DropMultsToWrite.Length && (STONE_DROP_MULT_OPTIONS || i == 0); i++)
                    {
                        string multString = "";
                        string multDirString = "";
                        if (STONE_DROP_MULT_OPTIONS)
                        {
                            multString = "x" + DropMultsToWrite[i];
                            multDirString = @"\" + multString + @" Stones";
                            if (DropMultsToWrite[i] == 1)
                            {
                                //multDirString += "(recommended)";
                                //multDirString += "";//"(recommended)";
                                multDirString = @"\Default";
                                multString = "";
                            }
                            else if (DropMultsToWrite[i] == 1.5f)
                                multDirString += "";
                        }
                        for (bool MatDrops = false; ; MatDrops = true)
                        {
                            string matDrops_string = "";
                            if (MatDrops)
                                matDrops_string = "+MD";//" and OneTimeEquipmentDrops";

                            string  matDrops_DirString = "";
                            /*string matDrops_DirString = @"\";
                            if (MatDrops)
                                matDrops_DirString += @"MatDrops";
                            else
                                matDrops_DirString += @"No MatDrops";*/

                            

                            for (bool WorldChanges = false; ; WorldChanges = true)
                            {
                                string worldChanges_string = "";
                                if (WorldChanges)
                                    worldChanges_string = "+LSPU";//" and OneTimeEquipmentDrops";

                                lessSmithingStonePickups_DirString = @"\";
                                if (WorldChanges)
                                    lessSmithingStonePickups_DirString += @"Less Stone Pickups";
                                else
                                    lessSmithingStonePickups_DirString += @"Regular Stone Pickups";

                                for (int equipType = 0; equipType < 3; equipType++)
                                {
                                    bool OneTimeEquipmentDrop = false;
                                    bool FirstTimeEquipmentDrop = false;

                                    if (equipType == 1)
                                        OneTimeEquipmentDrop = true;
                                    else if (equipType == 2)
                                        FirstTimeEquipmentDrop = true;

                                    string ED_string = "";
                                    if (OneTimeEquipmentDrop)
                                        ED_string = "+OTED";//" and OneTimeEquipmentDrops";
                                    if (FirstTimeEquipmentDrop)
                                        ED_string = "+FTED";

                                    var ED_DirString = @"\";
                                    if (OneTimeEquipmentDrop)
                                        ED_DirString += @"OneTime EquipDrops";
                                    else if (FirstTimeEquipmentDrop)
                                        ED_DirString += @"FirstTime EquipDrops";
                                    else
                                        ED_DirString += @"Regular EquipDrops";

                                    for (bool DropRunes = false; ; DropRunes = true)
                                    {
                                        string dropTypeString = "StDrops";//" StoneDrops";
                                        if (DropRunes)
                                            dropTypeString += "+RuDrops";//" and RuneDrops";
                                                                         //else
                                                                         //dropTypeString += "Only";

                                        var dropTypeDirString = @"\";
                                        if (MatDrops)
                                        {
                                            if (DropRunes)
                                                dropTypeDirString += @"StoneDrops RuneDrops and MatDrops";
                                            else
                                                dropTypeDirString += @"StoneDrops and MatDrops";
                                        }
                                        else
                                        {
                                            if (DropRunes)
                                                dropTypeDirString += @"StoneDrops and RuneDrops";
                                            else
                                                dropTypeDirString += @"StoneDrops ONLY";
                                        }

                                        RunSettings.Write_directory = exportDirectory + dropTypeDirString + multDirString + ED_DirString + matDrops_DirString + lessSmithingStonePickups_DirString;

                                        ParamFile.ImportCSVs(individual_StoneDropMultsDirectory[i]);//add the multiplier
                                        if (DropRunes)
                                            ParamFile.ImportCSVs(individual_RuneDropsDirectory);

                                        if (MatDrops)
                                            ParamFile.ImportCSVs(individual_MaterialDropsDirectory);

                                        if (OneTimeEquipmentDrop)
                                        {
                                            ParamFile.ImportCSVs(individual_OTEDDirectory);
                                            //enemyDrops_OneTimeEquipmentDrops();
                                        }
                                        else if (FirstTimeEquipmentDrop)
                                        {
                                            ParamFile.ImportCSVs(individual_FTEDDirectory);
                                            //enemyDrops_OneTimeEquipmentDrops(true);
                                        }

                                        ////enemyDrops_IncreasedMaterialDrops();
                                        //enemyDrops_MoreSmithingStoneDrops(DropRunes, true, DropMultsToWrite[i]);

                                        if (WorldChanges)
                                        {
                                            ////ShopLineupChanges(true, false,false,false);
                                            //reduceSmithingStonePickups();
                                            ParamFile.ImportCSVs(individual_WorldChangesDirectory);
                                        }
                                        //AddedLineManager.CreateEmptyLines(emptyItemLot, "");
                                        ParamFile.WriteModifiedFiles("", "__" + multString + dropTypeString + matDrops_string + ED_string + worldChanges_string);
                                        ParamFile.RevertAll(true);

                                        if (DropRunes == true)
                                            break;
                                    }
                                    //if (OneTimeEquipmentDrop == true)
                                    if (FirstTimeEquipmentDrop == true)
                                        break;
                                }
                                if (WorldChanges == true)
                                    break;
                            }
                            if (MatDrops == true)
                                break;
                        }
                    }

                }//old version

                player.Play();
            }

        }
        public static class IdFilters
        {

            ////base id for my mods: 	1029570000
            ////10(3, 5) - (3, 5) - (0, 7)--0                                                                                     //0+4+(7-9) saved flags
            ////specific number                       //specific range 2                                                                 
            //public static IntFilter.Single RoundtableItem_getItemFlagIDFilter =         IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(0, 0), -1, -1, -1);
            //public static IntFilter.Single RandomizedItem_getItemFlagIDFilter =         IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(4, 4), -1, -1, -1);
            public static IntFilter.Single RoundtableItem_getItemFlagIDFilter =         IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 8, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            public static IntFilter.Single RandomizedItem_getItemFlagIDFilter =         IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 7, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            ////specific number                       //specific range 2
            ////public static IntFilter.Single RoundtableItem_emptyLotCumulateFlagIDFilter =         IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5),       3      , IntFilter.Digit(3, 5),         7       , IntFilter.Digit(7, 9), -1, -1, -1);
            ////public static IntFilter.Single RandomizedItem_emptyLotCumulateFlagIDFilter =         IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5),       3      , IntFilter.Digit(3, 5),         6       , IntFilter.Digit(7, 9), -1, -1, -1);

            ////public static IntFilter.Single MaterialOneTimeDrop_getItemFlagIDFilter =    IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(7, 7), IntFilter.Digit(0,4), -1, -1);
            public static IntFilter.Single MaterialOneTimeDrop_getItemFlagIDFilter =    IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 6, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);

            //public static IntFilter.Single OneTimeDrop_getItemFlagIDFilter =            IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(7, 7), -1, -1, -1);
            public static IntFilter.Single OneTimeDrop_getItemFlagIDFilter =            IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 9, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            ////Specific Filter Digit
            //public static IntFilter.Single StoneDrop_getItemFlagIDFilter =              IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(8, 8), -1, -1, -1);
            //public static IntFilter.Single RuneDrop_getItemFlagIDFilter =               IntFilter.Create(true, 1, 0, 2, 9, 5, 7, IntFilter.Digit(9, 9), -1, -1, -1);
                                                                                                        //1041347160 was invalid
            public static IntFilter.Single StoneDrop_getItemFlagIDFilter =              IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 1, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
            public static IntFilter.Single RuneDrop_getItemFlagIDFilter =               IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 2, IntFilter.Digit(3, 5), -1, 7, -1, -1, 0);
        }

        const string VanillaFilesPath = @"C:\CODING\Souls Modding\Elden Ring Modding\ModEngine-2.0.0\mod\Vanilla 1.12\CSV\";

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
        public static ParamFile EquipParamAccessory = new ParamFile(VanillaFilesPath, "EquipParamAccessory.csv");
        public static ParamFile EquipParamGem = new ParamFile(VanillaFilesPath, "EquipParamGem.csv");
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

        public static ParamFile EquipParamCustomWeapon = new ParamFile(VanillaFilesPath, "EquipParamCustomWeapon.csv");


        static void Main()
        {
            if (false)
            {
                RunSettings.RunIfNull = true;
                //noupgradedweaponsFromNpcs();
                //enemyDrops_IncreasedMaterialDrops();
                //enemyDrops_OneTimeEquipmentDrops();
                //worldChangesPlus();
                //reduceSmithingStonePickups();
                /*string exportDirectory = @"C:\CODING OUTPUT\CSV\Individual Options (slower)";
                string dropTypeDirString = @"\WorldChangesPlus";
                RunSettings.Write_directory = exportDirectory + dropTypeDirString;
                ParamFile.WriteModifiedFiles("", "__" + "WCP");
                ParamFile.ResetAll();*/
                int[] resists = null;
                int resist_sleep =  NpcParam.GetFieldIndex("resist_sleep");
                {
                    resists = NpcParam.GetFieldIndexesContains("resist_");
                    var r2 = resists.ToList();
                    r2.Remove(resist_sleep);
                    resists = r2.ToArray();
                }
                var c = new Condition.MultiFieldCondition(new Condition.FloatFieldCompare(Condition.LESS_THAN_OR_EQUAL_TO, new FloatFieldRef(resist_sleep)), resists, true);
                var ls = ((Lines)NpcParam.GetLinesOnCondition(c));
                    ls.PrintIDAndNames();
                Util.println("found "+ ls.Length);
            }
            else
            if (RunSettings.CreateSmithingStoneMods)
            {
                /*const string VanillaFilesPathOld = @"C:\CODING\Souls Modding\Elden Ring Modding\ModEngine-2.0.0\mod\Vanilla 1.10\CSV\";
                foreach (ParamFile param in ParamFile.paramFiles)
                {
                    ParamFile pold = new ParamFile(VanillaFilesPathOld, param.filename,';',false);
                    foreach (Line line in param.lines)
                    {
                        if (line.name != "")
                            continue;
                        var lineold = pold.GetLineWithId(line.id);
                        if (lineold == null)
                            continue;
                        line.SetField(1, lineold.name);
                    }
                    param.PrintModifiedIDAndField(1);
                    Util.println("_______________________________________________________");
                }*/

                //var oldPath = @"C:\CODING OUTPUT\";
                //ParamFile ItemLotParam_enemy_old = new ParamFile(oldPath, "ItemLotParam_enemy ALL.csv");


                //var v1 = FlagIds.GetUsedGetItemFlagIds(FlagIds.is10digitFlag, new ParamFile[] { ItemLotParam_enemy_old });
                //var v2 = FlagIds.GetUsedGetItemFlagIds(FlagIds.is10digitFlag, new ParamFile[] { ItemLotParam_enemy, ItemLotParam_map });
                //foreach(Line l in ItemLotParam_enemy.lines.Concat(ItemLotParam_map.lines))
                /*{
                    var f = l.GetFieldAsInt(LotItem.getItemFlagIdFI);
                    if (v1.Contains(f))
                    {
                        
                        var ol = ItemLotParam_enemy_old.GetLinesOnCondition(new Condition.FieldIs(LotItem.getItemFlagIdFI, f));
                        //if (ol.GetField(LotItem.idFIs[1]) == l.GetField(LotItem.idFIs[1]) && ol.GetField(LotItem.idFIs[0]) == l.GetField(LotItem.idFIs[0]))
                        //    continue;
                        Util.println("vanilla occurence:  " + l._idName + "      flag:" + f);
                        foreach (Line o in ol)
                        {
                            Util.println("mod occurence:      " + o._idName + "      flag:" + o.GetField(LotItem.getItemFlagIdFI));
                        }
                        Util.println();
                    }
                }*/
                

                /*var flagIds = ItemLotParam_enemy.GetIntFields(LotItem.getItemFlagIdFI)
                    .Concat(ItemLotParam_map.GetIntFields(LotItem.getItemFlagIdFI))
                    .ToArray();
                IntFilter.CreateFromAcceptableInts(flagIds, 10).Print();*/

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
                ShopLineupChanges(true, false, false, false);
                Console.WriteLine("7");

                enemyDrops_MoreSmithingStoneDrops(true);
                Console.WriteLine("8");

                reduceSmithingStonePickups();
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

        

       static void noupgradedweaponsFromNpcs()
       {
           if (!IsRunningParamFile(new ParamFile[] { ItemLotParam_map, EquipParamCustomWeapon }))
               return;

           int lotItemCategory01 = ItemLotParam_map.GetFieldIndex("lotItemCategory01");
           int lotItemId01 = ItemLotParam_enemy.GetFieldIndex("lotItemId01");

           var isWeaponCond = new Condition.FieldIs(lotItemCategory01, "2");
           var isUpgradedCond = new Condition.FieldEndsWith(lotItemId01, "0").IsFalse;
           var upgradedWeaponLines = ItemLotParam_map.GetLinesOnCondition(isWeaponCond.AND(isUpgradedCond));

           foreach (Line line in upgradedWeaponLines)
           {
               var debugline = line._idName;
               string name = line.name;
               string new_name = "";
               if(name.Contains("+")){
                   int toplus = name.IndexOf("+");
                   int tospace = name.Substring(toplus).IndexOf(" ");
                   if (tospace != -1)
                       new_name = name.Remove(toplus, tospace + 1);
                   else
                       new_name = name.Remove(toplus);
               }
               var upgradedWeapId = line.GetField(lotItemId01);
               var newId = upgradedWeapId.Remove(upgradedWeapId.Length - 1);//removes last digit
               newId += "0"; //adds a 0;
               line.SetField("lotItemId01", newId).SetField(1, new_name);
           }

           int[] customWeaponIdsToNotDrop = new int[]{
               5000,
               5010
           };

           var customWeaponsNotToDrop = EquipParamCustomWeapon.GetLinesWithId(customWeaponIdsToNotDrop);

           foreach (Line line in customWeaponsNotToDrop)
           {
               string name = line.name;
               string new_name;
               {

                   int toplus = name.IndexOf("+");
                   int tospace = name.Substring(toplus).IndexOf(" ");
                   if (tospace != -1)
                       new_name = name.Remove(toplus, tospace+1);
                   else
                       new_name = name.Remove(toplus);
               }
               Line newLine = line.Copy().SetField(0, line.GetNextFreeId()).SetField(1, new_name).SetField("reinforceLv", 0);


               var m1 = newLine.modified;
               var f1 = newLine.inFile;

               EquipParamCustomWeapon.OverrideOrAddLine(newLine);

               var m = newLine.modified;
               var f = newLine.inFile;

               var isCustomWeaponCond = new Condition.FieldIs(lotItemCategory01, "6");
               var dropsThisWeaponIdCond = new Condition.FieldIs(lotItemId01, line.id);
               ItemLotParam_map.Operate(new SetFieldTo(lotItemId01, newLine.id), isCustomWeaponCond.AND(dropsThisWeaponIdCond));
           }


       }

       static void poisonSpellsUseIntelligence()
       {
           if (!IsRunningParamFile(new ParamFile[] { Magic }))
               return;

           int[] poisonSpells = new int[]
           {
               6440, //[Inc] Cure Poison
               7220, //[Inc] Poison Mist
               7230, //[Inc] Poison Armament
           };

           var poisonLines = Magic.GetLinesWithId(poisonSpells);
           foreach(Line line in poisonLines)
           {
               int req = line.GetFieldAsInt("requirementFaith");
               line.SetField("requirementIntellect", req);
               line.SetField("requirementFaith", 0);
           }

       }

       static void worldChangesPlus(bool RANDOMIZE_CERTAIN_ITEMS, bool ADD_ROUNDTABLE_ITEMS)
       {
           if (!IsRunningParamFile(new ParamFile[] { ItemLotParam_map , ItemLotParam_enemy}))
               return;
           
            List<int> usedGetItemFlagId = FlagIds.usedGetItemFlagId;
           int getItemFlagIdFI = ItemLotParam_map.GetFieldIndex("getItemFlagId");

           Dictionary<LotItem, int> lotItemToFlagIdDict = new Dictionary<LotItem, int>();
           Lines EnemyDropLinesWorthCheckingForFlagIds = ItemLotParam_enemy.vanillaParamFile.GetLinesOnCondition(
                    new Condition.FieldIs(LotItem.getItemFlagIdFI, 0).IsFalse
               .AND(new Condition.FloatFieldBetween(LotItem.categoryFIs[0], 2, 3, true))
               //.AND(new Condition.FieldIs(LotItem.chanceFIs[0], 1000))
               );
           EnemyDropLinesWorthCheckingForFlagIds = ItemLotParam_enemy.GetLinesWithId(EnemyDropLinesWorthCheckingForFlagIds.GetIDs());//gets the none vanilla versions of the lines.


           const bool createNewRoundtableItemLots = false;
           const bool stealItemLotsForRoundtable = true;
           if (createNewRoundtableItemLots)
           {

               LotItem[] roundtableItems = new LotItem[]
              {

               //twin fingermaiden
               new LotItem(LotItem.Category.Weapon, 1000000), //Dagger
               new LotItem(LotItem.Category.Weapon, 14000000), //Battle Axe
               new LotItem(LotItem.Category.Weapon, 11000000), //Mace
               new LotItem(LotItem.Category.Weapon, 5020000), //Rapier
               new LotItem(LotItem.Category.Weapon, 16000000), //Short Spear
               new LotItem(LotItem.Category.Weapon, 7140000), //Scimitar


               new LotItem(LotItem.Category.Weapon, 41000000), //Longbow
               new LotItem(LotItem.Category.Weapon, 34000000), //Finger Seal

               //brother corhyn
               new LotItem(LotItem.Category.Good, 6420), //Urgent Heal
               new LotItem(LotItem.Category.Good, 6421), //Heal
               new LotItem(LotItem.Category.Good, 6440), //Cure Poison
               new LotItem(LotItem.Category.Good, 6460), //Magic Fortification
               new LotItem(LotItem.Category.Good, 6450), //Flame Fortification
               new LotItem(LotItem.Category.Good, 6400), //Rejection
               new LotItem(LotItem.Category.Good, 6000), //Catch Flame
               new LotItem(LotItem.Category.Good, 6010), //Flame Sling
               //altus
               new LotItem(LotItem.Category.Good, 6422), //Great Heal
               new LotItem(LotItem.Category.Good, 6470), //Lightning Fortification
               //goldmask
               new LotItem(LotItem.Category.Good, 6700), //Discus of Light
               new LotItem(LotItem.Category.Good, 6010), //Immutable Shield

               //D Hunter of the Dead
               new LotItem(LotItem.Category.Good, 6750), //Litany of Proper Death
               new LotItem(LotItem.Category.Good, 6770), //Order's Blade

               //Gideon Ofnir
               new LotItem(LotItem.Category.Good, 6260), //Black Flame's Protection
               new LotItem(LotItem.Category.Good, 6760), //Law of Causality
               new LotItem(LotItem.Category.Good, 6490), //Lord's Divine Fortification

               new LotItem(LotItem.Category.Good, 8859), //Assassin's Prayerbook
              };
               int roundtableItemsStartId = 0;
               Line baseline = ItemLotParam_map.GetLineWithId(10000);
               int lastId = roundtableItemsStartId - 5;

               foreach (LotItem lotItem in roundtableItems)
               {
                   Line line = baseline.Copy();
                   lastId = ItemLotParam_map.GetNextFreeId(lastId + 5, true);

                   line.SetField(0, lastId);
                   if (lotItem.category == 1)
                       line.SetField(1, "Roundtable Item - " + EquipParamGoods.GetLineWithId(lotItem.id).name);
                   else if (lotItem.category == 2)
                       line.SetField(1, "Roundtable Item - " + EquipParamWeapon.GetLineWithId(lotItem.id).name);

                   lotItem.SetLotItemToLine(line, 1);
                   int currentGetItemFlagId = IntFilter.GetRandomInt(line.id_int, IdFilters.RoundtableItem_getItemFlagIDFilter, usedGetItemFlagId);
                   usedGetItemFlagId.Add(currentGetItemFlagId);
                   line.SetField(getItemFlagIdFI, currentGetItemFlagId);
                   ItemLotParam_map.OverrideOrAddLine(line);
               }


           }
           else if (stealItemLotsForRoundtable)
           {
               /*int[] itemlotidsTobeReplaced = new int[]
               {
                   30027000, //[Limgrave - Stormfoot Catacombs] Root Resin
                   1041380030, // Limgrave Smithing Stone x3
               };*/

                var isOneSmithingStoneCond =
                        new Condition.FieldIs(LotItem.categoryFIs[0], "1")
                        .AND(new Condition.FieldIs(LotItem.amountFIs[0], "1"))
                        .AND(new Condition.FloatFieldBetween(LotItem.idFIs[0], 10100, 10101, true));//smithing stone [1] & [2]

                var isBossToIgnore = new Condition.FloatFieldCompare(0, Condition.LESS_THAN, 40000);
                var isTearDropScarab = new Condition.HasInName("[Teardrop Scarab -");
                var isLegacyDungeon = new Condition.HasInName("[LD -");
                var isFieldBoss = new Condition.HasInName("- Field ");
                var isMaterialNode = new Condition.HasInName("[Material Node]");
                var isTunnel = new Condition.HasInName(new string[] { "Tunnel" , "[Ruin - Strewn Precipice]"});

                var isValidSmithingStoneForWeapon =
                    new Condition.AllOf(
                        isBossToIgnore.IsFalse,
                        isTearDropScarab.IsFalse,
                        //isLegacyDungeon.IsFalse,
                        isFieldBoss.IsFalse,
                        isMaterialNode.IsFalse,
                        isTunnel.IsFalse);

                var baseLine = ItemLotParam_map.GetLineWithId(10000);

                //OLD
                {
                    //Basic Weapons
                    {
                        /*
                        LotItem[] roundtableBasicWeapons = new LotItem[]
                        {
                        //twin fingermaiden
                        new LotItem(LotItem.Category.Weapon, 1000000), //Dagger
                        new LotItem(LotItem.Category.Weapon, 14000000), //Battle Axe
                        new LotItem(LotItem.Category.Weapon, 11000000), //Mace
                        new LotItem(LotItem.Category.Weapon, 5020000), //Rapier
                        new LotItem(LotItem.Category.Weapon, 16000000), //Short Spear
                        new LotItem(LotItem.Category.Weapon, 7140000), //Scimitar
                        };



                        Lines linesToReplaceWithWeaponPickups = ItemLotParam_map.GetLinesOnCondition(
                            isOneSmithingStoneCond.AND(isValidSmithingStoneForWeapon)
                            );



                        for (int i = 0; i < roundtableBasicWeapons.Length; i++)
                        {
                            //if(i == roundtableBasicWeapons.Length)
                            //empty space
                            LotItem lotItem = roundtableBasicWeapons[i];
                            int currentGetItemFlagId;
                            if (lotItemToFlagIdDict.ContainsKey(lotItem))
                            {
                                currentGetItemFlagId = lotItemToFlagIdDict[lotItem];
                            }
                            else
                            {
                                currentGetItemFlagId = IntFilter.GetRandomInt(lotItem.id, RoundtableItem_getItemFlagIDFilter, usedGetItemFlagId);
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                                lotItemToFlagIdDict.Add(lotItem, currentGetItemFlagId);
                            }

                            foreach (Line line in linesToReplaceWithWeaponPickups.lines)
                            {
                                line.SetField(LotItem.chanceFIs[0], 1);
                                if (lotItem.category == 1)
                                    line.SetField(1, "Roundtable Item - " + EquipParamGoods.GetLineWithId(lotItem.id).name);
                                else if (lotItem.category == 2)
                                    line.SetField(1, "Roundtable Item - " + EquipParamWeapon.GetLineWithId(lotItem.id).name);
                                lotItem.SetLotItemToLine(line, i + 2, LotItem.maxChance, 1, false, currentGetItemFlagId);
                            }
                        }
                        */
                    }

                    //FINGER SEAL
                    {
                        /*
                        LotItem[] roundtableTalisman = new LotItem[]
                        {
                        //twin fingermaiden
                        new LotItem(LotItem.Category.Weapon, 34000000), //Finger Seal
                        };
                        isOneSmithingStoneCond =
                            new Condition.FieldIs(ItemLotParam_map.GetFieldIndex("lotItemCategory01"), "1")
                            .AND(new Condition.FieldIs(ItemLotParam_map.GetFieldIndex("lotItemNum01"), "3"))
                            .AND(new Condition.FloatFieldBetween(10100, 10101, true));

                        Lines linesToReplaceWithWeaponPickups = ItemLotParam_map.GetLinesOnCondition(
                            isOneSmithingStoneCond.AND(isValidSmithingStone)
                            );

                        for (int i = 0; i <= roundtableTalisman.Length; i++)
                        {
                            //if(i == roundtableTalisman.Length)
                            //empty space
                            LotItem lotItem = roundtableTalisman[i];
                            int currentGetItemFlagId;
                            if (lotItemToFlagIdDict.ContainsKey(lotItem))
                            {
                                currentGetItemFlagId = lotItemToFlagIdDict[lotItem];
                            }
                            else
                            {
                                currentGetItemFlagId = IntFilter.GetRandomInt(lotItem.id, RoundtableItem_getItemFlagIDFilter, usedGetItemFlagId);
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                                lotItemToFlagIdDict.Add(lotItem, currentGetItemFlagId);
                            }

                            foreach (Line line in linesToReplaceWithWeaponPickups.lines)
                            {
                                if (lotItem.category == 1)
                                    line.SetField(1, "Roundtable Item - " + EquipParamGoods.GetLineWithId(lotItem.id).name);
                                else if (lotItem.category == 2)
                                    line.SetField(1, "Roundtable Item - " + EquipParamWeapon.GetLineWithId(lotItem.id).name);
                                lotItem.SetLotItemToLine(line, i + 2, LotItem.maxChance, 1, false, currentGetItemFlagId);
                            }
                        }
                        */
                    }
                    //CREPUS'S BLACK-KEY CROSSBOW
                    {
                        //adds the crepus bow along side the black key arrows found in fortified manor
                        /*var lotItem = new LotItem(LotItem.Category.Weapon, 43110000); //Crepus's black-key crossbow

                        List<Line> linesToAddTo = ItemLotParam_map.GetLinesOnCondition(
                            new Condition.IDCheck(11000125) //fortified  anor black key arrow
                            );

                        int currentGetItemFlagId = int.Parse(ItemLotParam_map.GetFieldOnCondition(LotItem.getItemFlagIdFI, new Condition.IDCheck(11100710)));

                        foreach (Line lineToAddTo in linesToAddTo)
                        {   //Certain item pickups will include a longbow drop. Only appears once.

                            Line line = lineToAddTo.Copy().SetField(0, lineToAddTo.id_int + 1);

                            line.SetField(1, "Roundtable Item - " + EquipParamWeapon.GetLineWithId(lotItem.id).name);

                            lotItem.SetLotItemToLine(line, 1);


                            line.SetField(getItemFlagIdFI, currentGetItemFlagId);
                            ItemLotParam_map.OverrideOrAddLine(line);
                        }*/
                    }
                    //LONGBOW
                    {
                        /*


                        Line baseLine = ItemLotParam_map.GetLineWithId(10000);
                        LotItem lotItem = new LotItem(LotItem.Category.Weapon, 41000000); //Longbow
                        LotItem arrowLotItem = new LotItem(LotItem.Category.Weapon, 50000000, 10); //Arrow
                        LotItem fireArrowLotItem = new LotItem(LotItem.Category.Weapon, 50010000); //Fire Arrow
                        Condition isAnArrowCond = new Condition.Either(new Condition.HasLotItem(arrowLotItem, 1), new Condition.HasLotItem(fireArrowLotItem, 1));
                        Condition isABowTalismanCond =
                            new Condition.FieldEqualTo(LotItem.categoryFIs[0], LotItem.Category.Accessory.ToString()).AND(
                            new Condition.HasInName("Arrow's Reach Talisman")
                            .OR(new Condition.HasInName("Arrow's Sting Talisman"))
                            );
                        List<Line> linesToAddTo = ItemLotParam_map.GetLinesOnCondition(
                                isABowTalismanCond
                                .OR(isAnArrowCond));

                        foreach (Line lineToAddTo in linesToAddTo)
                        {   //Certain item pickups will include a longbow drop. Only appears once.

                            Line line = lineToAddTo.Copy().SetField(0, lineToAddTo.id_int + 1);

                            line.SetField(1, "Roundtable Item - " + EquipParamWeapon.GetLineWithId(lotItem.id).name);

                            lotItem.SetLotItemToLine(line, 1);

                            int currentGetItemFlagId;
                            if (lotItemToFlagIdDict.ContainsKey(lotItem))
                            {
                                currentGetItemFlagId = lotItemToFlagIdDict[lotItem];
                            }
                            else
                            {
                                currentGetItemFlagId = IntFilter.GetRandomInt(line.id_int, RoundtableItem_getItemFlagIDFilter, usedGetItemFlagId);
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                                lotItemToFlagIdDict.Add(lotItem, currentGetItemFlagId);
                            }
                            line.SetField(getItemFlagIdFI, currentGetItemFlagId);

                            ItemLotParam_map.OverrideOrAddLine(line);

                            if (isABowTalismanCond.Pass(lineToAddTo))//IF IS A BOW TALISMAN
                            {
                                //ALSO ADD ARROWS
                                Line arrowLine = baseLine.Copy().SetField(0, line.id_int + 1);
                                arrowLine.Operate(new SetLotItem(arrowLotItem, 1));
                                arrowLine.SetField(1, "For Bow - Arrow");
                                arrowLine.SetField(getItemFlagIdFI, lineToAddTo.GetField(getItemFlagIdFI)); //USES THE SAME PICKUP FLAG AS ITEM
                                arrowLine.SetField(LotItem.lotItem_getItemFlagIdFIs[0], currentGetItemFlagId);//USES THE SAME LOT SPECIFIC FLAG AS BOW
                            }

                        }
                        */
                    }
                }
                



                //SPELLS
                {
                    Dictionary<LotItem, int[]> ItemsToReplacementItemLotId = new Dictionary<LotItem, int[]>();
                    Dictionary<LotItem, LotItem> BackupExistingItemLot = new Dictionary<LotItem, LotItem>();
                    Dictionary<LotItem, LotItem[]> AdditionalItemLots = new Dictionary<LotItem, LotItem[]>();
                    Dictionary<LotItem, LotItem[]> AdditionalItemLotsOneLine = new Dictionary<LotItem, LotItem[]>();
                    Dictionary<LotItem, Condition> ConditionForAdditionalItemLots = new Dictionary<LotItem, Condition>();
                    Dictionary<LotItem, LotItem> ReplaceFirstLotItem = new Dictionary<LotItem, LotItem>();
                    Dictionary<LotItem, Condition> ConditionForReplaceFirstLotItem = new Dictionary<LotItem, Condition>();

                    var ss3Ids = ItemLotParam_map.GetIDsWithLineName("Smithing Stone [3]");

                    var sss6ValidIds = ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("Somber Smithing Stone[6]").AND(isValidSmithingStoneForWeapon));
                    var sss7ValidIds = ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("Somber Smithing Stone [7]").AND(isValidSmithingStoneForWeapon));

                    var sss6Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [6]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [6]")))).ToArray();
                    var sss5Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [5]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [5]")))).ToArray();
                    var sss4Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [4]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [4]")))).ToArray();
                    var sss3Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [3]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [3]")))).ToArray();
                    var sss2Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [2]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [2]")))).ToArray();
                    var sss1Ids = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [1]").Concat(ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [1]")))).ToArray();
                    var neutralizingBolusesIds = ItemLotParam_map.GetIDsWithCondition(new LotItem.HasLotItem(new LotItem(1, 900, 2), 1, true)); //2x Neutralizing Boluses
                    var x8smolderingButterflyIds = ItemLotParam_map.GetIDsWithCondition(new LotItem.HasLotItem(new LotItem(1, 20802, 8), 1, true)); //8x Smoldering Butterfly

                    var sss8Ids2 = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [8]");
                    var sss3Ids2 = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [3]");
                    var sss2Ids2 = ItemLotParam_map.GetIDsWithLineName("Somber Smithing Stone [2]");

                    var sss3Ids3 = ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [3]")));
                    var sss2Ids3 = ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("[Teardrop Scarab - ").AND(new Condition.NameEndsWith("Somber Smithing Stone [2]")));

                    var gr13_ow = ItemLotParam_map.GetIDsWithLineName("Golden Rune [13]");
                    var gr7 = ItemLotParam_map.GetIDsWithCondition(new Condition.HasInName("Golden Rune [7]"));
                    var gr5_ow = ItemLotParam_map.GetIDsWithLineName("Golden Rune [5]");




                    int RCI_last_index = -1;

                    const bool V6 = false;
                    //RANDOMIZE Items
                    LotItem.Default_Chance = 1;
                    if (RANDOMIZE_CERTAIN_ITEMS)
                    {
                        LotItem[] talimans1 = new LotItem[]
                        {
                            //new LotItem(LotItem.Category.Accessory, "Crimson Amber Medallion").addKW("erdtree",25), //sold by mercahnt
                            new LotItem(LotItem.Category.Accessory, "Cerulean Amber Medallion").addKW("magic").addKW("erdtree",25), //crystal cave, bloodhound knight boss
                            new LotItem(LotItem.Category.Accessory, "Viridian Amber Medallion").addKW("erdtree",25), //Miranda the Blighted Bloom
                            new LotItem(LotItem.Category.Accessory, "Green Turtle Talisman").addKW("erdtree",25), //turtle room imp sea
                            new LotItem(LotItem.Category.Accessory, "Arsenal Charm"), //given by nepheli

                            //new LotItem(LotItem.Category.Accessory, "Starscourge Heirloom"), //Fort Gael +strength
                            //new LotItem(LotItem.Category.Accessory, "Prosthesis-Wearer Heirloom"), //Millicent +dexterity
                            //new LotItem(LotItem.Category.Accessory, "Stargazer Heirloom"), //Liurnia Divine tower +intelligence
                            new LotItem(LotItem.Category.Accessory, "Two Fingers Heirloom"), //Purified Ruins +faith

                            new LotItem(LotItem.Category.Accessory, "Dragoncrest Shield Talisman").addKW("dragon"), //Edges of Beastial Sactum
                            new LotItem(LotItem.Category.Accessory, "Spelldrake Talisman").addKW("dragon"), //Earthbore Cave Runebear boss 
                            new LotItem(LotItem.Category.Accessory, "Flamedrake Talisman").addKW("dragon"), //Groveside Cave Beastman boss
                            new LotItem(LotItem.Category.Accessory, "Boltdrake Talisman").addKW("dragon"), //Stomveil, divine tower path 
                            new LotItem(LotItem.Category.Accessory, "Haligdrake Talisman").addKW("dragon"), //Stranded Graveyard, loop back
                            new LotItem(LotItem.Category.Accessory, "Pearldrake Talisman").addKW("dragon"), //Four belfries gateway, Farum azula

                            new LotItem(LotItem.Category.Accessory, "Immunizing Horn Charm").addKW("ancestral follower"), //Ainsel River Ant Nest
                            new LotItem(LotItem.Category.Accessory, "Stalwart Horn Charm").addKW("ancestral follower"), //Liurnia Mausoleum Compound
                            new LotItem(LotItem.Category.Accessory, "Clarifying Horn Charm").addKW("ancestral follower"), //Deep Siofra Well
                            new LotItem(LotItem.Category.Accessory, "Mottled Necklace").addKW("ancestral follower"), //Four Belfries gateway, Nokron

                            new LotItem(LotItem.Category.Accessory, "Prince of Death's Pustule").addKW("deathblight"), //Stormveil, godwyn corpse

                            new LotItem(LotItem.Category.Accessory, "Curved Sword Talisman"), //Stromviel, dark room chest
                            new LotItem(LotItem.Category.Accessory, "Twinblade Talisman"), //Castle Morne, tower chest
                            new LotItem(LotItem.Category.Accessory, "Axe Talisman"), //Mistwood Ruins, chest
                            new LotItem(LotItem.Category.Accessory, "Hammer Talisman"), //Recusant Henricus drop
                            new LotItem(LotItem.Category.Accessory, "Spear Talisman"), //Lakeside Crystal cave
                            new LotItem(LotItem.Category.Accessory, "Lance Talisman"), //Stormhill, by bats
                            new LotItem(LotItem.Category.Accessory, "Arrow's Reach Talisman").addKW("highground"), //above stormgate chest

                            new LotItem(LotItem.Category.Accessory, "Primal Glintstone Blade").addKW("magic"), //MtGiants, Stargazer ruins, 
                            new LotItem(LotItem.Category.Accessory, "Moon of Nokstella").addKW("magic").addKW("nokstella"), //Nokstella giant throne chest
                            new LotItem(LotItem.Category.Accessory, "Old Lord's Talisman").addKW("dragon").addKW("farum azula"), //Farum Azula
                            new LotItem(LotItem.Category.Accessory, "Radagon Icon").addKW("radagon"), //Raya Lucaria
                            new LotItem(LotItem.Category.Accessory, "Roar Medallion").addKW("troll"), //Limgrave Tunnels Troll boss
                            //new LotItem(LotItem.Category.Accessory, "Companion Jar"), //Jar Bairn questline
                            //new LotItem(LotItem.Category.Accessory, "Perfumer's Talisman").addKW("perfumer"), //Perfumer ruins
                            //new LotItem(LotItem.Category.Accessory, "Carian Filigreed Crest").addKW("magic").addKW("carian"), //Bought from iji
                            //new LotItem(LotItem.Category.Accessory, "Warrior Jar Shard").addKW("great jar", 150), //Iron fist alexander
                            //new LotItem(LotItem.Category.Accessory, "Shard of Alexander"), //Shard of Alexander
                            new LotItem(LotItem.Category.Accessory, "Godfrey Icon").addKW("godfrey"), //Gidefroy drop
                            new LotItem(LotItem.Category.Accessory, "Bull-Goat's Talisman"), //Dragonbarrow cave
                            new LotItem(LotItem.Category.Accessory, "Blue Dancer Charm").addKW("magic").addKW("uhl"), //Highroad Cave golem boss

                            new LotItem(LotItem.Category.Accessory, "Crucible Scale Talisman").addKW("crucible"), //Lleyndel Catacombs
                            new LotItem(LotItem.Category.Accessory, "Crucible Feather Talisman").addKW("crucible"), //Auriza Hero's Grave
                            new LotItem(LotItem.Category.Accessory, "Crucible Knot Talisman").addKW("crucible"), //Village of Albinaurics omen killer drop

                            //new LotItem(LotItem.Category.Accessory, "Red-Feather Branchsword").addKW("death bird"), //Death bird
                            //new LotItem(LotItem.Category.Accessory, "Blue-Feather Branchsword").addKW("death bird"), //Death bird

                            //new LotItem(LotItem.Category.Accessory, "Assassin's Crimson Dagger").addKW("black knives"), //Deathtouched Catacombs, limgrave, BK drop
                            //new LotItem(LotItem.Category.Accessory, "Assassin's Cerulean Dagger").addKW("magic").addKW("black knives"), //Black Knife Catacombs, Liurnia, BK drop

                            //new LotItem(LotItem.Category.Accessory, "Winged Sword Insignia").addKW("malenia"), //Still water cave Clean Rot Knight drop
                            //new LotItem(LotItem.Category.Accessory, "Rotten Winged Insignia").addKW("malenia").addKW("rot"), //Millicent questline option
                            //new LotItem(LotItem.Category.Accessory, "Millicent's Prosthesis").addKW("millicent").addKW("malenia",50), //Millicent questline iption

                            //new LotItem(LotItem.Category.Accessory, "Godskin Swaddling Cloth").addKW("godskin"), //Spirit caller cave godskin drop
                            //new LotItem(LotItem.Category.Accessory, "Kindred of Rot's Exultation").addKW("rot"), //Kindred rot boss drop
                            //new LotItem(LotItem.Category.Accessory, "Taker's Cameo").addKW("rykard"), //Tanith reward item
                            //new LotItem(LotItem.Category.Accessory, "Ancestral Spirit's Horn").addKW("ancestral follower"), //Remmerance of regal ancestar


                            //new LotItem(LotItem.Category.Accessory, "Crepus's Vial").addKW("confessor"), //Relight the Idle drop
                            //new LotItem(LotItem.Category.Accessory, "Concealing Veil").addKW("black knife"), //Sage Cave BK drop
                            //magic new LotItem(LotItem.Category.Accessory, "Longtail Cat Talisman").addKW("magic"), //Raya lucaria
                            //new LotItem(LotItem.Category.Accessory, "Furled Finger's Trick Mirror").addKW("two finger"), //finger maiden husks
                            //new LotItem(LotItem.Category.Accessory, "Host's Trick-Mirror").addKW("two finger"), //finger maiden husks
                            //new LotItem(LotItem.Category.Accessory, "Shabriri's Woe").addKW("frenzy").addKW("shabriri"), //Frenzied Flame Village
                            //new LotItem(LotItem.Category.Accessory, "Daedicar's Woe").addKW("rykard").addKW("snake").addKW("daedicar"), //Rya quest
                            //new LotItem(LotItem.Category.Accessory, "Sacrificial Twig"), //many places
                            //new LotItem(LotItem.Category.Accessory, "Entwining Umbilical Cord"), //cut
                        };
                        LotItem[] talimans2 = new LotItem[]
                        {
                            new LotItem(LotItem.Category.Accessory, "Radagon's Scarseal").addKW("radagon"),//Weeping Evergael
                            new LotItem(LotItem.Category.Accessory, "Marika's Scarseal").addKW("marika"),//Nokron
                            new LotItem(LotItem.Category.Accessory, "Crimson Amber Medallion +1").addKW("erdtree", 25), //Volcano Manor imp seal
                            new LotItem(LotItem.Category.Accessory, "Cerulean Amber Medallion +1").addKW("magic").addKW("erdtree", 25), //Castle Sol
                            new LotItem(LotItem.Category.Accessory, "Viridian Amber Medallion +1").addKW("erdtree", 25), //Margit secound encounter
                            new LotItem(LotItem.Category.Accessory, "Crimson Seed Talisman").addKW("erdtree", 25), //Sainted Heros imp seal
                            //magic new LotItem(LotItem.Category.Accessory, "Cerulean Seed Talisman").addKW("magic").addKW("erdtree"), //Carian Study hall
                            new LotItem(LotItem.Category.Accessory, "Arsenal Charm +1"), //cave by falling star beast
                            new LotItem(LotItem.Category.Accessory, "Blessed Dew Talisman").addKW("erdtree"),    //tower of return teleport
                            new LotItem(LotItem.Category.Accessory, "Erdtree's Favor"),//fringe folk ruins

                            new LotItem(LotItem.Category.Accessory, "Dragoncrest Shield Talisman +1").addKW("dragon"), //Sainted Hero's grave imp seal
                            //magic new LotItem(LotItem.Category.Accessory, "Spelldrake Talisman +1").addKW("dragon"), //Sellia secret chest
                            new LotItem(LotItem.Category.Accessory, "Flamedrake Talisman +1").addKW("dragon"), //leyndell, random column
                            new LotItem(LotItem.Category.Accessory, "Boltdrake Talisman +1").addKW("dragon"), //Old Altus Tunnel shed
                            new LotItem(LotItem.Category.Accessory, "Haligdrake Talisman +1").addKW("dragon"), //leyndell
                            new LotItem(LotItem.Category.Accessory, "Pearldrake Talisman +1").addKW("dragon"), //Mt.Gelmir ruins imp seal

                            //ancestral followers new LotItem(LotItem.Category.Accessory, "Immunizing Horn Charm +1").addKW("ancestral follower"), //Lake of Rot shaman drop
                            //ancestral followers new LotItem(LotItem.Category.Accessory, "Stalwart Horn Charm +1").addKW("ancestral follower"), //concentrated snow field
                            //ancestral followers new LotItem(LotItem.Category.Accessory, "Clarifying Horn Charm +1").addKW("ancestral follower"), //Nokron upper
                            //ancestral followers new LotItem(LotItem.Category.Accessory, "Mottled Necklace +1").addKW("ancestral follower"), //Nokron upper

                            //new LotItem(LotItem.Category.Accessory, "Prince of Death's Cyst").addKW("deathblight"), //Deeproot Depths, bear drop

                            new LotItem(LotItem.Category.Accessory, "Claw Talisman").addKW("occult",50).addKW("death bird", 70).addKW("highground", 50), //Stormveil ladder tower
                            new LotItem(LotItem.Category.Accessory, "Greatshield Talisman"), //Ancient dragon Lanseax area
                            new LotItem(LotItem.Category.Accessory, "Arrow's Sting Talisman").addKW("highground"), //redmane castle tower chest
                            
                            //magic secret new LotItem(LotItem.Category.Accessory, "Graven-School Talisman").addKW("Magic").addKW("Gravenmass").addKW("Occult"), //raya lucaria secret room
                            new LotItem(LotItem.Category.Accessory, "Faithful's Canvas Talisman").addKW("rot").addKW("occult").addKW("sacred",30), //Sellia crystal tunnel (has rot followers)

                            //magic secret new LotItem(LotItem.Category.Accessory, "Magic Scorpion Charm").addKW("Magic").addKW("Occult"), //Preceptor Selivus
                            new LotItem(LotItem.Category.Accessory, "Fire Scorpion Charm").addKW("fire").addKW("occult"), //Fort Laiedd mt.gelmir
                            new LotItem(LotItem.Category.Accessory, "Lightning Scorpion Charm").addKW("lightning").addKW("occult"), //Wyndhelm catacombs imp seal
                            new LotItem(LotItem.Category.Accessory, "Sacred Scorpion Charm").addKW("sacred").addKW("occult"), //Anastacia drop

                            new LotItem(LotItem.Category.Accessory, "Ritual Sword Talisman"), //Lux Ruins Altus
                            new LotItem(LotItem.Category.Accessory, "Ritual Shield Talisman"), //Leyndell

                            new LotItem(LotItem.Category.Accessory, "Gold Scarab"), //Cleanrot knight duo boss
                            new LotItem(LotItem.Category.Accessory, "Silver Scarab"), //Hidden Path to Haligtree, chest

                        };
                        LotItem[] talimans3 = new LotItem[]
                        {
                            new LotItem(LotItem.Category.Accessory, "Radagon's Soreseal").addKW("radagon"),//Fort Faroth
                            new LotItem(LotItem.Category.Accessory, "Marika's Soreseal").addKW("marika"),//Haligtree

                            new LotItem(LotItem.Category.Accessory, "Crimson Amber Medallion +2").addKW("erdtree", 25), //Capital of Ash
                            //new LotItem(LotItem.Category.Accessory, "Cerulean Amber Medallion +2").addKW("erdtree", 25), //Moonlight Alter Ruins
                            new LotItem(LotItem.Category.Accessory, "Viridian Amber Medallion +2").addKW("erdtree", 25), //Haligtree, room with mushroom people 
                            //new LotItem(LotItem.Category.Accessory, "Great-Jar's Arsenal").addKW("great jar",200),//Cealid Arena reward
                            new LotItem(LotItem.Category.Accessory, "Erdtree's Favor +1"),//

                            new LotItem(LotItem.Category.Accessory, "Dragoncrest Shield Talisman +2").addKW("dragon"), //Farum Azula
                            new LotItem(LotItem.Category.Accessory, "Spelldrake Talisman +2").addKW("dragon"), //Hidden Path to haligtree
                            new LotItem(LotItem.Category.Accessory, "Flamedrake Talisman +2").addKW("dragon"), //Dragon barrow cave beastmen duo boss
                            new LotItem(LotItem.Category.Accessory, "Boltdrake Talisman +2").addKW("dragon"), //Farum Azula
                            new LotItem(LotItem.Category.Accessory, "Haligdrake Talisman +2").addKW("dragon"), //Moghwyn Palace
                            new LotItem(LotItem.Category.Accessory, "Pearldrake Talisman +2").addKW("dragon"), //Haligtree

                            new LotItem(LotItem.Category.Accessory, "Dagger Talisman").addKW("Occult"), //Temple of Eiglay, imp seal


                            
                            //magic secret new LotItem(LotItem.Category.Accessory, "Graven-Mass Talisman").addKW("magic").addKW("gravenmass").addKW("occult"), //Concentrated Snowfield, Albinauric Rise
                            new LotItem(LotItem.Category.Accessory, "Flock's Canvas Talisman").addKW("rot").addKW("occult").addKW("sacred",30), //Dropped by Gowry

                            
                            //blood new LotItem(LotItem.Category.Accessory, "Lord of Blood's Exultation").addKW("mogh"), //Esgar priest of blood droop
                        };
                        LotItem[] talisman4 = new LotItem[]
                        {
                            //new LotItem(LotItem.Category.Accessory, "Ertdtree's Favor +2").addKW("erdtree"), //Ashen Capital
                            new LotItem(LotItem.Category.Accessory, "Dragoncrest Greatshield Talisman").addKW("dragon"), //Haligtree drainage channel
                        };

                        LotItem[] deathRiteBird = new LotItem[]
                        {
                            new LotItem(LotItem.Category.Weapon, 3200000), //Death's Poker
                            new LotItem(LotItem.Category.Weapon, 16120000), //Death Ritual Spear
                            new LotItem(LotItem.Category.Good, 5001), //Ancient Death Rancor
                            new LotItem(LotItem.Category.Good, 5010), //Explosive Ghostflame
                        };
                        LotItem[] deathbirdItems = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 14110000), //Sacrificial Axe
                            new LotItem(LotItem.Category.Weapon, 31090000), //Twinbird Kite Shield
                            new LotItem(LotItem.Category.Accessory, 4080), //Blue-Feathered Branchsword
                            new LotItem(LotItem.Category.Accessory, 2040), //Red-Feathered Branchsword
                        };
                        LotItem[] bloodhoundItems = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 8030000), //Bloodhound's Fang
                            new LotItem(LotItem.Category.Weapon, 22020000), //Bloodhound Claws
                        };
                        //bloodhound step to some bloodhounds
                        {
                            var bloodhoundStepIds = new int[]
                            {
                              20371,    //crystal cave boss
                              //30130,    //evergeal boss, fang drop
                              -429000001,    //None drop
                              //-429000804,   //Gelmir hero cave, Armor Set Drop
                              //=429000701, //Mt.Gelmir, Claw drop
                            };
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.AshOfWar, 80100, 1, 1000, false, -1), bloodhoundStepIds); //Bloodhound Step
                            BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(LotItem.Category.Good, 10168, 1)); //1 somber ancient dragon smithing stone
                        }

                        //Warrior Jar Shard rare drops from jars
                        {
                            var warrorJarItemLotIds = Util.makeNegative(((Lines)ItemLotParam_enemy.GetLinesOnCondition(new Condition.NameIs("[Living Pot - Large] Living Jar Shard").AND(new Condition.FieldIs(LotItem.getItemFlagIdFI, 0).IsFalse))).GetIDs());
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Accessory, 1230, 1, 150, true, 400175), warrorJarItemLotIds); //Warrior Jar Shard
                            ItemsToReplacementItemLotId.Add(LotItem.newEmpty(LotItem.MAX_CHANCE,false,400175), warrorJarItemLotIds); //Warrior Jar Shard
                            warrorJarItemLotIds = Util.makeNegative(((Lines)ItemLotParam_enemy.GetLinesOnCondition(new Condition.NameIs("[Living Pot - Large] Living Jar Shard").AND(new Condition.FieldIs(LotItem.getItemFlagIdFI, 0)))).GetIDs());
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Accessory, 1230, 1, 20, true, 400175), warrorJarItemLotIds); //Warrior Jar Shard
                            BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(LotItem.Category.Good, 2916, 1)); //1 Hero Rune [3]
                        }

                        LotItem[] earlyBasicWeapons = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 13010000), //Flail //carriage
                            //new LotItem(LotItem.Category.Weapon, 3030000), //Lordsworn's Greatsword //carriage                  moved
                            new LotItem(LotItem.Category.Weapon, 11050000), //Morning Star //carriage
                            new LotItem(LotItem.Category.Weapon, 6020000), //Great epee  //chest
                            new LotItem(LotItem.Category.Weapon, 10000000), //Twinblade  //chest
                            new LotItem(LotItem.Category.Weapon, 15000000), //Greataxe //carriage
                            //new LotItem(LotItem.Category.Weapon, 4000000), //Greatsword //carriage                  moved
                           // new LotItem(LotItem.Category.Weapon, 3180000), //Claymore //chest                  moved
                            //new LotItem(LotItem.Category.Weapon, 3030000), //Lordsworn's Greatsword //carriage                  moved
                        };

                        LotItem[] earlyBasicWeaponsHeavy = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 4000000), //Greatsword //carriage
                            new LotItem(LotItem.Category.Weapon, 3180000), //Claymore //chest
                            new LotItem(LotItem.Category.Weapon, 3030000), //Lordsworn's Greatsword //carriage
                            new LotItem(LotItem.Category.Weapon, 18020000), //Lucerne //body
                        };

                        LotItem[] midgameWeapons = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 21100000), //Katar  //chest
                            //new LotItem(LotItem.Category.Weapon, 18020000), //Lucerne //body                  moved
                            new LotItem(LotItem.Category.Weapon, 24020000), //Steel-Wire Torch  //body
                            new LotItem(LotItem.Category.Weapon, 10030000), //Twinned Knight Swords  //body
                            new LotItem(LotItem.Category.Weapon, 2210000), //Cane Sword  //body
                            new LotItem(LotItem.Category.Weapon, 17060000), //Lance  //body
                            //new LotItem(LotItem.Category.Weapon, 43080000), //Arbalest //body
                            new LotItem(LotItem.Category.Weapon, 16070000), //Pike //body
                        };

                        LotItem[] lateGameWeapons = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 17070000), //Treespear
                            new LotItem(LotItem.Category.Weapon, 4030000), //Troll's Golden Sword
                            new LotItem(LotItem.Category.Weapon, 23110000), //Giant-Crusher
                            //new LotItem(LotItem.Category.Weapon, 24070000), //Sentry's Torch //sold by merchant
                        };

                        LotItem[] edgyWeapons = new LotItem[]{
                            new LotItem(LotItem.Category.Weapon, 16110000), //Cross-Naginata //body cealid
                            new LotItem(LotItem.Category.Weapon, 1100000), //Wakizashi //body cealid

                            new LotItem(LotItem.Category.Weapon, 7030000), //Shamshir //body 
                            new LotItem(LotItem.Category.Weapon, 9000000), //Uchigatana //body
                            new LotItem(LotItem.Category.Weapon, 7080000), //Scavenger's Curved Sword //body near grafted scion /
                            //new LotItem(LotItem.Category.Weapon, 41070000), //Black Bow // body in a rooftop, slight thematic value
              
                            new LotItem(LotItem.Category.Weapon, 22000000), //Hookclaws //body
                            new LotItem(LotItem.Category.Weapon, 1030000), //Miséricorde //body
                        };

                        //for weapons randomization v1,
                        //Get all lines with weapons to randomize.
                        //add all weapons to the line. give each lot item a uniqe flog.
                        //This failed to work as reseting the game made a new weapon respawn. 

                        //for weapon v2.
                        //Shuffle existing item lots with anouther weapon.
                        //No ingame randomization (preset)

                        //for weapons randomization v3.
                        //give each lot item a unique flag and a chance of 1.
                        //Give a unique culilateflagId to lot 0, which moves it from 0 to maxValue.
                        //this ingame allows randomization


                        //for rountable weapons randomization v1,
                        //Get all lines you want to become replacable with item.
                        //add all lotitems to that lo give each lot item a uniqe flag. except for original drop.
                        //This failed to work as reseting the game made a new weapon respawn. 

                        //for weapon v2.
                        //Get all lines you want to become replacable with item.
                        //lines will be divided by number of lines present.
                        //One lot item given to each group of lines.
                        //No accual randomization

                        //for weapons randomization v3.
                        //Get all lines you want to become replacable with item.
                        //give each lot item a unique flag and a chance of 1.
                        //Give a unique culilateflagId to lot[0],
                        //  chance which moves it from 0 to maxValue.
                        //this ingame allows randomization.
                        //however this will expire the culminateFlag 8 bit limit.

                        //for weapons randomization v3.
                        //Get all lines you want to become replacable with item.
                        //give each lot item a unique flag and a chance of 1.
                        //Create (number or weapons to radomize) unique culilateflagId for lot[0] and distribute them rouhgly equally to all lines.
                        //  chance culminates from 0 to max Value.
                        //this ingame allows randomization withiught expireing the 8 bit limit

                        //TODO: test what happens when all lotItems have flag Ids and all flagids are expired.

                        LotItem[][] lotItemGroupsToRandomize = new LotItem[][]
                        {
                deathRiteBird,
                deathbirdItems,
                //bloodhoundItems,

                earlyBasicWeaponsHeavy,

                earlyBasicWeapons,            //edit: fixing. -- doesnt work the way i thought it would. reseting game respawns weapons. 
                midgameWeapons,
                edgyWeapons,
                lateGameWeapons,


                            //new LotItem(LotItem.Category.Weapon, 2180000), //Carian Knight's Sword, found in luirnia thematic value




                            //new LotItem(LotItem.Category.Weapon, 12180000), //Great Stars, thematic value, found in blood related area             
                            //new LotItem(LotItem.Category.Weapon, 24040000), //St. Trina's Torch,  concentrated snow field
                            //new LotItem(LotItem.Category.Weapon, 7060000), //Flowing Curved Sword, concentrated snow field slight thematic value

                            //shop weapons 
                            /*
                                 new LotItem(LotItem.Category.Weapon, 4040000), //Zweihander

                                 new LotItem(LotItem.Category.Weapon, 3000000), //Bastard Sword
                                 new LotItem(LotItem.Category.Weapon, 43020000), //Light Crossbow

                                 new LotItem(LotItem.Category.Weapon, 2020000), //Broadsword
                                 new LotItem(LotItem.Category.Weapon, 11010000), //Club
                                 new LotItem(LotItem.Category.Weapon, 40000000), //Shortbow

                                 new LotItem(LotItem.Category.Weapon, 14020000), //Hand Axe

                                 new LotItem(LotItem.Category.Weapon, 18000000), //Halberd

                                 new LotItem(LotItem.Category.Weapon, 21010000), //Spiked Caestus
                                 new LotItem(LotItem.Category.Weapon, 24060000), //Beast-Repellent Torch, thematically accurate

                                 new LotItem(LotItem.Category.Weapon, 5000000), //Estoc
                                 new LotItem(LotItem.Category.Weapon, 33130000), //Astrologer's Staff

                                 new LotItem(LotItem.Category.Weapon, 40050000), //Composite Bow



                                 new LotItem(LotItem.Category.Weapon, 7020000), //Shotel

                                 //patches
                                 new LotItem(LotItem.Category.Weapon, 5000000), //Estoc, also sold by a nomadic merchant
                                 new LotItem(LotItem.Category.Weapon, 1020000), //Parrying Dagger

                                 //gostoc
                                 new LotItem(LotItem.Category.Weapon, 21000000), //Caestus
                            */
                            //^shop weapons









                            /* 
                            new LotItem(LotItem.Category.Weapon, 12190000), //Brick Hammer , stormveil
                            new LotItem(LotItem.Category.Weapon, 14100000), //Highland Axe, thematicly zccurate, in front of godfrey painting stormveil

                             //castle morce cell area, thematically accurate
                             new LotItem(LotItem.Category.Weapon, 20000000), //Whip

                             //demihuman body, limgrave
                             new LotItem(LotItem.Category.Weapon, 12000000), //Large Club

                             //found in proximitty to jerren, slight thematic value
                             new LotItem(LotItem.Category.Weapon, 3050000), //Flamberge

                             //catacombs
                             new LotItem(LotItem.Category.Weapon, 19000000), //Scythe
                             */
                        };

                        foreach (LotItem[] lotItemGroup in lotItemGroupsToRandomize)
                        {
                            Lines lines_enemy = EnemyDropLinesWorthCheckingForFlagIds.GetLinesOnCondition(new LotItem.HasLotItem(lotItemGroup, 1, 1, false));
                            Lines lines_map = ItemLotParam_map.GetLinesOnCondition(new LotItem.HasLotItem(lotItemGroup, 1, 1, false));

                            if (!V6)
                            {
                                lines_enemy.Operate(new LotItem.SetLotItem(LotItem.newEmpty(), 1));    //removes the orignal lot
                                lines_map.Operate(new LotItem.SetLotItem(LotItem.newEmpty(), 1));    //removes the orignal lot
                            }

                            int[] lineIds_map = lines_map.GetIDs();
                            int[] lineIds_enemy = lines_enemy.GetIDs();
                            for (int i = 0; i < lineIds_enemy.Length; i++)
                            {
                                lineIds_enemy[i] *= -1; //enemy lines are marked negative.
                            }
                            int[] lineIds = lineIds_map.Concat(lineIds_enemy).ToArray();

                            {//new culminateFIx. Adds a empty lot that becomes guarenteed after the first drop. One unique id per unique item, equally distributed throughout lines.
                            
                                /*
                                List<int>[] lineIdsDistibution = new List<int>[lotItemGroup.length];
                                for (int i = 0; i < lotItemGroup.Length(); i++)
                                {
                                    int indexToAdd = i;
                                    while (i < lineIds.Length)
                                    {
                                        lineIdsDistibution[i].Add(lineIds[indexToAdd]);
                                        indexToAdd += lotItemGroup.Length();
                                    }
                                }
                                for (int i = 0; i < lotItemGroup.length; i++)
                                {
                                    int cumu = IntFilter.GetRandomInt(lotItemGroup[i].id, RandomizedItem_emptyLotCumulateFlagIDFilter, FlagIds.usedCumulateNumFlagIds);
                                    FlagIds.usedCumulateNumFlagIds.Add(cumu);
                                    ItemsToReplacementItemLotId.Add(LotItem.newEmpty(0, false, 0, cumu, 1, LotItem.MAX_CHANCE, false), lineIdsDistibution[i].ToArray());
                                }
                                */
                            }
                            if (V6)
                            {
                                var emptyLot = LotItem.newEmpty();
                                ItemsToReplacementItemLotId.Add(emptyLot, lineIds);
                                AdditionalItemLotsOneLine.Add(emptyLot, lotItemGroup);
                                if(lotItemGroup == earlyBasicWeapons)
                                    ConditionForAdditionalItemLots.Add(emptyLot, new Condition.FieldIs(LotItem.getItemFlagIdFI, 400370));
                            }
                            else
                            {
                                foreach (LotItem lotItem in lotItemGroup)
                                {
                                    lotItem.SetItemLot_getItemFlagId(-1); //this will mark it as forced to create a new id, not search for one. 
                                    ItemsToReplacementItemLotId.Add(lotItem, lineIds);
                                }
                            }
                        }
                        RCI_last_index = ItemsToReplacementItemLotId.Count-1;
                    }

                    LotItem.Default_Chance = LotItem.MAX_CHANCE;

                    if (ADD_ROUNDTABLE_ITEMS)
                    {
                        //Talisman Pouch
                        {
                            ItemsToReplacementItemLotId.Add(new LotItem(1, 10040, 1, LotItem.MAX_CHANCE, false, 60520), new int[] { 10301, 1043520500, 1039500101 }); //Enia's 2 Greatrune Talisman Pouch, radahn, margit second encounter, godefroy
                            BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 2990, 1)); //1 lands between runes.
                        }

                        int[] linesToReplaceWithBasicWeaponPickups;
                        {
                            linesToReplaceWithBasicWeaponPickups = ItemLotParam_map.GetIDsWithCondition(isOneSmithingStoneCond.AND(isValidSmithingStoneForWeapon));
                        }

                        //KNIGHT ARMOR
                        {
                            //rando fix - removes id:1045370010 from viable weapon lines.
                            var linesToReplaceWithBasicWeaponPickupsL = linesToReplaceWithBasicWeaponPickups.ToList();
                            linesToReplaceWithBasicWeaponPickupsL.Remove(1045370010);
                            linesToReplaceWithBasicWeaponPickups = linesToReplaceWithBasicWeaponPickupsL.ToArray();
                        }
                        ItemsToReplacementItemLotId.Add(LotItem.newEmpty(LotItem.MAX_CHANCE, false), new int[] { 1045370010, 31010100 }); //Empty,  mistwood ruins chest, earthbore cave chest,
                        AdditionalItemLots.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem[] {
                        new LotItem(LotItem.Category.Armor, 1500000), //Knight Helm
                        new LotItem(LotItem.Category.Armor, 1500100), //Knight Armor
                        new LotItem(LotItem.Category.Armor, 1500200), //Knight Gauntlets
                        new LotItem(LotItem.Category.Armor, 1500300), //Knight Greaves

                    });
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 31330000), new int[] { 1045350000, 1042340020, 1046400040 }); //Heater Shield



                        //ROUNDTABLE BASIC WEAPONS

                        {
                            LotItem[] roundtableBasicWeapons = new LotItem[]
                            {
                            //twin fingermaiden
                            new LotItem(LotItem.Category.Weapon, 1000000), //Dagger
                            new LotItem(LotItem.Category.Weapon, 14000000), //Battle Axe
                            new LotItem(LotItem.Category.Weapon, 11000000), //Mace
                            new LotItem(LotItem.Category.Weapon, 5020000), //Rapier
                            new LotItem(LotItem.Category.Weapon, 16000000), //Short Spear
                            new LotItem(LotItem.Category.Weapon, 7140000), //Scimitar
                            };

                            if (V6)
                            {
                                var emptyLot = LotItem.newEmpty();
                                ItemsToReplacementItemLotId.Add(emptyLot, linesToReplaceWithBasicWeaponPickups);
                                AdditionalItemLotsOneLine.Add(emptyLot, roundtableBasicWeapons);
                            }
                            else
                            {
                                foreach (LotItem lotItem in roundtableBasicWeapons)
                                {
                                    ItemsToReplacementItemLotId.Add(lotItem, linesToReplaceWithBasicWeaponPickups);
                                }
                            }
                        }


                        //brother corhyn spells
                        {
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6420), sss1Ids); //Urgent Heal
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6421), sss1Ids); //Heal
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6440), neutralizingBolusesIds); //Cure Poison , Fort Faroth Neutralizing Boluses 1051390050, 
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6460), sss2Ids.Concat(sss3Ids).ToArray()); //Magic Fortification
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6450), sss2Ids); //Flame Fortification
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6400), sss4Ids); //Rejection
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6000), sss3Ids2.Concat(sss2Ids2).Concat(new int[] { 1037540050 }).ToArray()); //Catch Flame, Missionary's Cookbook [3] 1046400030, Mt.Gelmir burned minor erdtree x5 Golden Arrow 
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6010), sss3Ids3.Concat(sss2Ids3).Concat(x8smolderingButterflyIds).ToArray()); //Flame Sling, Nomadic Warrior's Cookbook [14] 1046400050, cealid entrance overlook shed Golden Rune [3] 1046400020
                        }

                        //Finger Seal & Glintstone Staff
                        {
                            //x3 Smithing Stone [1] lines
                            int[] is3xSS1LineIds =
                                ItemLotParam_map.GetIDsWithCondition(new Condition.FieldIs(LotItem.categoryFIs[0], "1")
                                .AND(new Condition.FieldIs(LotItem.amountFIs[0], "3"))
                                .AND(new Condition.FloatFieldBetween(LotItem.idFIs[0], 10100, 10101, true)));
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 34000000), is3xSS1LineIds); //Finger Seal

                            Lines glintStoneStaffDropLines = ItemLotParam_enemy.GetLinesOnCondition(new LotItem.HasLotItem(new LotItem(LotItem.Category.Weapon, 33000000), 2));
                            glintStoneStaffDropLines.Operate(new LotItem.SetLotItem(LotItem.newEmpty(), 2));//remove second lot
                                                                                                            //var v1 = glintStoneStaffDropLines.lines.Count;
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 33000000, 1, 1000), is3xSS1LineIds.Concat(Util.makeNegative(glintStoneStaffDropLines.GetIDs())).ToArray()); //Glintstone Staff. lower chance than finger seal

                        }

                        //Other roundtable spells
                        {
                            //altus
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6422), sss5Ids.Concat(sss6Ids).ToArray()); //Great Heal
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6470), sss5Ids.Concat(sss6Ids).ToArray()); //Lightning Fortification
                                                                                                                                          //goldmask
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6700), new int[] { 15000270 }); //Discus of Light, haligtree in front of miquella statue and misbegotten statue Ancient Dragon Smithing Stone
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6740), new int[] { 1040510400 }); //Immutable Shield, stormcaller church sacred tear


                            //D Hunter of the Dead
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6750), new int[] { 1043350100 }); //Litany of Proper Death, church of pilgramige sacred tear (cant get id)
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6770), new int[] { 1036490000 }); //Order's Blade,  bellum church sacred tear

                            //Gideon Ofnir
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6260), new int[] { 11050010 }); //Black Flame's Protection
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6760), new int[] { 11050030 }); //Law of Causality
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 6490), new int[] { 11050050 }); //Lord's Divine Fortification

                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 8859), new int[] { 10000960 }); //Assassin's Prayerbook,  stormveil picked turtle neck
                            BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 190, 3));
                            ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 43110000), new int[] { 11000126 }); //Crepus's black-key crossbow,  added to black key bolt in fortified manor
                            BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(ItemLotParam_map.GetLineWithId(11000125), 1, true)); //Black-Key Arrows
                        }

                        //LONGBOW
                        {
                            LotItem lotItem = new LotItem(LotItem.Category.Weapon, 41000000); //Longbow
                            LotItem arrowLotItem = new LotItem(LotItem.Category.Weapon, 50000000, 10); //Arrow
                            LotItem fireArrowLotItem = new LotItem(LotItem.Category.Weapon, 50010000); //Fire Arrow
                            Condition isAnArrowCond = new Condition.Either(new LotItem.HasLotItem(arrowLotItem, 1), new LotItem.HasLotItem(fireArrowLotItem, 1));
                            Condition isABowTalismanCond =
                                new Condition.FieldIs(LotItem.categoryFIs[0], LotItem.Category.Accessory.ToString()).AND(
                                new Condition.HasInName("Arrow's Reach Talisman")
                                .OR(new Condition.HasInName("Arrow's Sting Talisman"))
                                );
                            int[] linesToAddToIds = ItemLotParam_map.GetIDsWithCondition(
                                isABowTalismanCond
                                .OR(isAnArrowCond));
                            for (int i = 0; i < linesToAddToIds.Length; i++)
                            {
                                linesToAddToIds[i]++; //doesnt replace original drop, bow is added to arrow drops
                            }
                            ItemsToReplacementItemLotId.Add(lotItem, linesToAddToIds); //Assassin's Prayerbook,  stormveil picked turtle neck
                            AdditionalItemLots.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem[] { arrowLotItem });
                            ConditionForAdditionalItemLots.Add(ItemsToReplacementItemLotId.Keys.Last(), isABowTalismanCond);
                        }


                        
                        //ROYAL REMAINS SET
                        {
                            ItemsToReplacementItemLotId.Add(LotItem.newEmpty(LotItem.MAX_CHANCE, false, 400490), sss8Ids2); //Empty
                            AdditionalItemLots.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem[] {
                        new LotItem(LotItem.Category.Armor, 690000),
                        new LotItem(LotItem.Category.Armor, 690100), //Royal Remains Set
                        new LotItem(LotItem.Category.Armor, 690200), //
                        new LotItem(LotItem.Category.Armor, 690300), //
                        });
                        }
                        //CLINGING BONE 
                        //ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 21110000, 1, 200), sss7ValidIds); //Clinging Bone
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 21110000), sss8Ids2.Concat(sss7ValidIds).ToArray()); //Clinging Bone

                        //ARSENAL CHARM
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Accessory, 1030), new int[] { 100612, 110601, 110611, 110622 }); //Arsenal Charm, Banished Knight's Halberd +8 - Spinning Strikes. 
                        BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 10163));// Somber Smithing Stone [4]

                        //TWINNED SET
                        ItemsToReplacementItemLotId.Add(LotItem.newEmpty(LotItem.MAX_CHANCE, false, 400349), new int[] { 13000200 }); //Empty , rejuvinating boluses in farum azula surounded by worm faces
                        AdditionalItemLots.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem[] {
                        new LotItem(LotItem.Category.Armor, 600000), //
                        new LotItem(LotItem.Category.Armor, 600100), //Twinned Set
                        new LotItem(LotItem.Category.Armor, 600200), //
                        new LotItem(LotItem.Category.Armor, 600300), //
                    });
                        BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 100250, 10));

                        //Fevor's Cookbook [3]
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 9421), gr13_ow); //Fevors Cookbook [3],  found in open world golden rune [13]s
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Good, 9421, 1, 150, true), gr5_ow); //Fevors Cookbook [3],  found in open world golden rune [5]s
                        BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 1290, 3)); //x3 Starlight Shards

                        //Cipher Pata
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 21130000), sss6ValidIds); //Cipher Pata,  found along side two fingers heirloom, found in exchange for any golden rune [7]
                        ItemsToReplacementItemLotId.Add(new LotItem(LotItem.Category.Weapon, 21130000, 1, 300, true), gr7); //Cipher Pata,  found along side two fingers heirloom, found in exchange for any golden rune [7]
                        BackupExistingItemLot.Add(ItemsToReplacementItemLotId.Keys.Last(), new LotItem(1, 190, 2)); //x2 Rune Arcs

                        //1046370000, 1035450030, 1038430000, 1049560320 strips of white flesh x2 - x3
                        //church of pilgramige blood grease 1042340000
                        //Missionary's Cookbook [3] 1046400050, Nomadic Warrior's Cookbook [14] 1046400030

                        //used for heater shield
                        //rot view balcony perserving bouluses 1046400040
                        //weeping pennisula Golden Rune [2] 1042340020
                        //mistwood shore Golden Rune [1] 1045350000

                    }

                    LotItem.Default_Chance = 1000;

                    Dictionary<int, List<LotItem>> lineIdToLotItemsDict = new Dictionary<int, List<LotItem>>();
                    int index = 0;


                    foreach (LotItem lotItem in ItemsToReplacementItemLotId.Keys)
                    {
                        index++;
                        {
                            int currentGetItemFlagId = -1;
                            if (lotItem.hasLotItem_getItemFlagId && lotItem.lotItem_getItemFlagId != -1)
                                currentGetItemFlagId = lotItem.lotItem_getItemFlagId;
                            Line l = null;
                            var hasLotCond = new LotItem.HasLotItem(lotItem, 1);

                            IntFilter.Single filter = IdFilters.RandomizedItem_getItemFlagIDFilter;
                            if (index > RCI_last_index)
                                filter = IdFilters.RoundtableItem_getItemFlagIDFilter;

                            if (!lotItem.IsEmpty())
                            {
                                l = ((Lines)ItemLotParam_map.GetLinesWithId(((Lines)ItemLotParam_map.vanillaParamFile.GetLinesOnCondition(hasLotCond)).GetIDs())).GetLineOnCondition(hasLotCond); //both vanilla and current lines have the lot
                                if (l == null)
                                    l = EnemyDropLinesWorthCheckingForFlagIds.GetLineOnCondition(new LotItem.HasLotItem(lotItem, 1));
                            }
                            else if (!AdditionalItemLots.ContainsKey(lotItem) && !AdditionalItemLotsOneLine.ContainsKey(lotItem))
                            {
                                continue; //no point in continueing
                            }
                            if (AdditionalItemLotsOneLine.ContainsKey(lotItem)) 
                            {
                                foreach (LotItem add_lotItem in AdditionalItemLotsOneLine[lotItem])
                                {
                                    {
                                        int add_currentGetItemFlagId = -1;
                                        if (add_lotItem.hasLotItem_getItemFlagId && add_lotItem.lotItem_getItemFlagId != -1)
                                            add_currentGetItemFlagId = add_lotItem.lotItem_getItemFlagId;
                                        Line add_l = null;
                                       if (!add_lotItem.IsEmpty())
                                        {
                                            var add_hasLotCond = new LotItem.HasLotItem(add_lotItem, 1);
                                            add_l = ((Lines)ItemLotParam_map.GetLinesWithId(((Lines)ItemLotParam_map.vanillaParamFile.GetLinesOnCondition(add_hasLotCond)).GetIDs())).GetLineOnCondition(add_hasLotCond); //both vanilla and current lines have the lot
                                            if (add_l == null)
                                                add_l = EnemyDropLinesWorthCheckingForFlagIds.GetLineOnCondition(new LotItem.HasLotItem(add_lotItem, 1));
                                        }else
                                            continue;

                                        if (add_l != null)
                                        {
                                            if (add_currentGetItemFlagId == -1)
                                                add_currentGetItemFlagId = add_l.GetFieldAsInt(LotItem.getItemFlagIdFI);
                                            /*if (BackupExistingItemLot.ContainsKey(add_lotItem) && add_l.file == ItemLotParam_map)
                                            {
                                                var backUpLI = BackupExistingItemLot[add_lotItem];
                                                backUpLI.SetLotItemToLine(add_l, 2, 1, backUpLI.amount, false);
                                                l.SetField(LotItem.lotItem_getItemFlagIdFIs[add_lotItem.GetLotItemIndex(l, false) - 1], add_currentGetItemFlagId);
                                                int newGetItemFlagIdForOldLine = IntFilter.GetRandomInt(add_lotItem.id, filter, usedGetItemFlagId);
                                                usedGetItemFlagId.Add(newGetItemFlagIdForOldLine);

                                                //replaces all line with currentGetItemFlagId to the newGetItemFlagId
                                                ItemLotParam_map.Operate(new SetFieldTo(LotItem.getItemFlagIdFI, newGetItemFlagIdForOldLine), new Condition.FieldIs(LotItem.getItemFlagIdFI, currentGetItemFlagId), true);
                                                //l.SetField(LotItem.getItemFlagIdFI, newGetItemFlagIdForOldLine);
                                            }*/
                                        }
                                        else if (add_currentGetItemFlagId == -1)
                                            add_currentGetItemFlagId = IntFilter.GetRandomInt(add_lotItem.id, filter, usedGetItemFlagId);
                                        if (add_currentGetItemFlagId > 0)
                                            usedGetItemFlagId.Add(add_currentGetItemFlagId);
                                        lotItemToFlagIdDict.Add(add_lotItem, add_currentGetItemFlagId);
                                    }
                                }
                            }

                            

                            if (l != null)
                            {
                                if(currentGetItemFlagId == -1)
                                    currentGetItemFlagId = l.GetFieldAsInt(LotItem.getItemFlagIdFI);
                                if (BackupExistingItemLot.ContainsKey(lotItem) && l.file == ItemLotParam_map)
                                {
                                    var backUpLI = BackupExistingItemLot[lotItem];
                                    backUpLI.SetLotItemToLine(l, 2, 1, backUpLI.amount, false);
                                    l.SetField(LotItem.lotItem_getItemFlagIdFIs[lotItem.GetLotItemIndex(l,false)-1], currentGetItemFlagId);
                                    int newGetItemFlagIdForOldLine = IntFilter.GetRandomInt(lotItem.id, filter, usedGetItemFlagId);
                                    usedGetItemFlagId.Add(newGetItemFlagIdForOldLine);

                                    //replaces all line with currentGetItemFlagId to the newGetItemFlagId
                                    ItemLotParam_map.Operate(new SetFieldTo(LotItem.getItemFlagIdFI, newGetItemFlagIdForOldLine), new Condition.FieldIs(LotItem.getItemFlagIdFI, currentGetItemFlagId), true);
                                    //l.SetField(LotItem.getItemFlagIdFI, newGetItemFlagIdForOldLine);
                                }
                            }
                            else if(currentGetItemFlagId == -1)
                                currentGetItemFlagId = IntFilter.GetRandomInt(lotItem.id, filter, usedGetItemFlagId);
                            if(currentGetItemFlagId > 0)
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                            lotItemToFlagIdDict.Add(lotItem, currentGetItemFlagId);
                        }

                        foreach (int id in ItemsToReplacementItemLotId[lotItem])
                        {
                            if (!lineIdToLotItemsDict.ContainsKey(id))
                            {
                                lineIdToLotItemsDict.Add(id, new List<LotItem>());
                            }
                            lineIdToLotItemsDict[id].Add(lotItem);
                        }
                    }


                    foreach (int dict_id in lineIdToLotItemsDict.Keys)
                    {
                        Line line;
                        int id = dict_id;
                        ParamFile ItemLotFile = ItemLotParam_map;
                        if (dict_id < 0)
                        {
                            id = -dict_id;
                            ItemLotFile = ItemLotParam_enemy;
                        }
                        line = ItemLotFile.GetLineWithId(id);

                        if (lineIdToLotItemsDict[dict_id].Count > 6)
                        {
                            Util.p();
                        }

                        if (lineIdToLotItemsDict[dict_id].Count > 1)//cumu fix
                        {
                            var emptyLot = LotItem.newEmpty(0, false, 0, line.GetFieldAsInt(LotItem.getItemFlagIdFI), 1, 1, false);
                            lineIdToLotItemsDict[dict_id].Add(emptyLot); //uses getitemflagid as the cumuflagid, works for forcing drop to be 1time drop, even when using multiple lotitem-specific getItemLotIds
                            //lotItemToFlagIdDict.Add(emptyLot, 0);
                        }

                        bool isNewLine = false;
                        int lotIndex = 2;
                        if (line == null)   //lot item should not be empty or null if this is the case
                        {
                            isNewLine = true;
                            lotIndex = 1;
                            Line prevLine = ItemLotFile.GetLineWithId(id - 1);
                            if (prevLine == null)
                            {
                                Util.println("id:" + id + " does not exist in ItemLotParam_map");
                                continue;
                                int currentGetItemFlagId = IntFilter.GetRandomInt(id, IdFilters.RoundtableItem_getItemFlagIDFilter, usedGetItemFlagId);
                                usedGetItemFlagId.Add(currentGetItemFlagId);
                                line = ItemLotFile.GetLineWithId(10000).Copy().SetField(LotItem.getItemFlagIdFI, currentGetItemFlagId);
                            }
                            else
                            {
                                line = prevLine.Copy();
                            }

                             
                            line.SetField(0, id);
                            string name;
                            if (lineIdToLotItemsDict[dict_id].Count == 1)
                            {
                                LotItem lotItem = lineIdToLotItemsDict[dict_id][0];
                                name =// " - " + 
                                    lotItem.Name;
                            }
                            else
                                name = "";//"s"; // "items" plural
                            line.SetField(1, //"Roundtable Item" + 
                                name);
                            ItemLotFile.OverrideOrAddLine(line);
                        }
                        else 
                        {
                            lotIndex = new LotItem(0, 0, 0, 0, false).GetLotItemIndex(line,true);
                        }


                        foreach (LotItem lotItem in lineIdToLotItemsDict[dict_id])
                        {
                            if (ReplaceFirstLotItem.ContainsKey(lotItem) && (!ConditionForReplaceFirstLotItem.ContainsKey(lotItem) || ConditionForReplaceFirstLotItem[lotItem].Pass(line)))
                            {
                                ReplaceFirstLotItem[lotItem].SetLotItemToLine(line, 1);
                            }
                            if ((!ConditionForAdditionalItemLots.ContainsKey(lotItem) || ConditionForAdditionalItemLots[lotItem].Pass(line))) {

                                if (AdditionalItemLots.ContainsKey(lotItem))    //should only happen if there is one lineIdToLot
                                {
                                    foreach (LotItem AdditionalItemLot in AdditionalItemLots[lotItem])
                                    {

                                        Line newLine = baseLine.Copy().SetField(0, line.GetNextFreeId());
                                        newLine.Operate(new LotItem.SetLotItem(AdditionalItemLot, 1));
                                        //Util.println(AdditionalItemLot.id);
                                        newLine.SetField(1, AdditionalItemLot.Name);
                                        newLine.SetField(getItemFlagIdFI, line.GetField(getItemFlagIdFI)); //USES THE SAME PICKUP FLAG AS ITEM
                                        newLine.SetField(LotItem.lotItem_getItemFlagIdFIs[0], lotItemToFlagIdDict[lotItem]);//USES THE SAME LOT SPECIFIC FLAG AS BOW
                                        ItemLotFile.OverrideOrAddLine(newLine);
                                    }

                                }
                                if (AdditionalItemLotsOneLine.ContainsKey(lotItem))
                                {
                                    Line newLine = baseLine.Copy().SetField(0, line.GetNextFreeId());
                                    newLine.SetField(1, "");
                                    newLine.SetField(getItemFlagIdFI, line.GetField(getItemFlagIdFI)); //USES THE SAME PICKUP FLAG AS ITEM
                                    int add_lotIndex = 1;
                                    foreach (LotItem AdditionalItemLotOneLine in AdditionalItemLotsOneLine[lotItem])
                                    {
                                        
                                        if (!AdditionalItemLotOneLine.IsEmpty())
                                        {
                                            var add_namestr = newLine.name;
                                            if (add_namestr != "")
                                                add_namestr += " & ";
                                            add_namestr += AdditionalItemLotOneLine.Name;
                                            newLine.SetField(1, add_namestr);
                                        }
                                        newLine.Operate(new LotItem.SetLotItem(AdditionalItemLotOneLine, add_lotIndex));
                                        newLine.SetField(LotItem.lotItem_getItemFlagIdFIs[add_lotIndex-1], lotItemToFlagIdDict[AdditionalItemLotOneLine]);//USES SPECIFIC FLAG TO Item
                                        //Util.println(AdditionalItemLotOneLine.id);
                                        add_lotIndex++;
                                    }
                                    ItemLotFile.OverrideOrAddLine(newLine);
                                }
                            }
                            if (false)//testing
                            {
                                var t = ItemLotFile.paramName;
                                var y = line.file.paramName;
                                var m = line.modified;
                                var v = line.IsVannilaLine;
                                var vv = line.vanillaLine.IsVannilaLine;
                                var i = line.inFile;
                                var test = 429000700;
                                if (line.id_int == test)
                                    Util.p();

                            }

                            if (!isNewLine && !lotItem.IsEmpty())
                                line.SetField(1, line.name + " & " + lotItem.Name);


                            
                            if (lotItem.chance == LotItem.MAX_CHANCE)
                                line.SetField(LotItem.chanceFIs[0], 1); //set regular drop to 1

                            int getItemFlagId = 0;
                            if (lotItemToFlagIdDict.ContainsKey(lotItem))
                                getItemFlagId = lotItemToFlagIdDict[lotItem];

                            if (lotIndex == 9)
                                Util.p();

                            lotItem.SetLotItemToLine(line, lotIndex, lotItem.chance, lotItem.amount, false, getItemFlagId);
                            lotIndex++;
                        }
                    }

                    //((Lines)ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.getItemFlagIdFI, 400370))).RevertFieldsToVanilla();  //Revert Corhyns flail drop to vanilla

                }
            }

        }
        static void enemyDrops_IncreasedMaterialDrops(float DROPMULT = 1)
        {
            if (!IsRunningParamFile(new ParamFile[] { ItemLotParam_enemy }))
                return;
            int good = LotItem.Category.Good;
            int itemLotId_enemy = NpcParam.GetFieldIndex("itemLotId_enemy");

                                                                                                           //specific num 1                    //specific num 2
            
            var NonMaterialLinesToInclude = new List<Line>();
            Condition isMaterial =
                    new Condition.NameStartsWith("[").AND(
                    new Condition.FieldIs(LotItem.categoryFIs[1], LotItem.Category.Good).AND(new Condition.FloatFieldBetween(LotItem.idFIs[1], 15000, 25000))
                    .OR(new Condition.FieldIs(LotItem.categoryFIs[0], LotItem.Category.Good).AND(new Condition.FloatFieldBetween(LotItem.idFIs[0], 15000, 25000))));


            List<LotItem> materials_to_exclude = new List<LotItem>();
            List<LotItem> materials_chance_percentMultipler = new List<LotItem>();
            List<LotItem> materials_to_set_max = new List<LotItem>();
            List<LotItem> setLots = new List<LotItem>();

            //More Drops
            {
                NpcData.CheckDataSet();
                Dictionary<LotItem, int[]> lotItemToLineIDsDict = new Dictionary<LotItem, int[]>();

                //starlight shard, nox, glintsone sorcerer
                {
                    string[] names = new string[] { "Nox Swordstress", "Nox Monk", "Nox Nightmaiden", "Glintstone Sorcerer" };
                    for (int i = 0; i < names.Length; i++)
                    {
                        var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                            new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse.AND(
                            new Condition.HasInName(names[i])))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds()
                            .GetIDs();
                            ;
                        if (i == 0)
                            lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 115,true), ids);
                        if (i == 1)
                            lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 115,true), ids);
                        if (i == 2)
                        {
                            lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 115, true), ids);
                            /*int currentGetItemFlagId = IntFilter.GetRandomInt(ids[0], MaterialOneTimeDrop_getItemFlagIDFilter, FlagIds.usedGetItemFlagId);
                            FlagIds.usedGetItemFlagId.Add(currentGetItemFlagId);
                            lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 3, 1000, false, currentGetItemFlagId), ids);*/
                        }
                        if(i == 3)
                            lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 70, true), ids);
                        
                    }
                    materials_to_set_max.Add(new LotItem(good, "Starlight Shards", 4).addKW("Nox"));
                    materials_to_set_max.Add(new LotItem(good, "Starlight Shards", 2).addKW(" Glintstone Sorcerer"));//non generic glintstone sorcerer
                    materials_to_set_max.Add(new LotItem(good, "Starlight Shards", 1).addKW("Glintstone Sorcerer"));

                    materials_chance_percentMultipler.Add(new LotItem(good, "Starlight Shards", 0, 130).addKW(" Glintstone Sorcerer"));//non generic glintstone sorcerer
                    materials_chance_percentMultipler.Add(new LotItem(good, "Starlight Shards", 0, 100));
                }
                //starlight shard, Graven School
                {
                    var GravenSchoolLines = (Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(new Condition.NameIs(new string[] { "Graven School" }));
                    List<Line> itemLotLinesAdded = new List<Line>(); 
                    List<int> itemLotLinesAddedIds = new List<int>();
                    int lastlineIndex = -1;
                    foreach (Line npcLine in GravenSchoolLines.lines)
                    {
                        
                        //create an id.
                        const int startingId = 330010000;
                        int newItemLotID = ItemLotParam_enemy.GetNextFreeIdIntreval(startingId, true, 4, lastlineIndex, out lastlineIndex, itemLotLinesAddedIds);
                        itemLotLinesAddedIds.Add(newItemLotID);
                        //create line.
                        Line newLine = LotItem.newBaseItemLotLine(ItemLotParam_enemy, newItemLotID,"["+npcLine.name+"] None");
                        ItemLotParam_enemy.OverrideOrAddLine(newLine);
                        npcLine.SetField("itemLotId_enemy", newItemLotID);
                        int flagId = IntFilter.GetRandomInt(newItemLotID, IdFilters.MaterialOneTimeDrop_getItemFlagIDFilter, FlagIds.usedGetItemFlagId);
                        FlagIds.usedGetItemFlagId.Add(flagId);
                        lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 450, true, -flagId), new int[] { newItemLotID });
                        lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 3, 1000, false,flagId), new int[] { newItemLotID });
                    }
                    //lotItemToLineIDsDict.Add(new LotItem(good, "Starlight Shards", 1, 25, true), itemLotLinesAddedIds.ToArray());
                    
                }
                //raw meat dumpling - misbegotten
                {
                    var lines = ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.NameIs(new string[] { "Misbegotten", "Winged Misbegotten", "Scaly Misbegotten", "Leonine Misbegoten" })
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Castle Morne", "Village Windmill" }), true)
                        ))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray());
                    var ids = ((Lines)lines).GetIDs();

                    lotItemToLineIDsDict.Add(new LotItem(good, "Raw Meat Dumpling", 1, 40,true), ids);
                    materials_chance_percentMultipler.Add(new LotItem(good, "Raw Meat Dumpling", 0, 160).addKW("Scaly Misbegotten"));
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //raw meat dumpling - Living Jar
                {
                    string[] names = new string[] { "Living Jar", "Great Jar" };
                    for (int i = 0; i < names.Length; i++)
                    {
                        var lines = ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                                new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse.AND(
                                new Condition.HasInName(names[i])))).GetIntFields(itemLotId_enemy).Distinct().ToArray());
                        var ids = ((Lines)lines).GetIDs();
                        if (names[i] == "Living Jar")
                            lotItemToLineIDsDict.Add(new LotItem(good, "Raw Meat Dumpling", 1, 25,true), ids);
                        if (names[i] == "Great Jar")
                            lotItemToLineIDsDict.Add(new LotItem(good, "Raw Meat Dumpling", 1, 100,true), ids);

                        //var debugCount = lines.Count();
                        NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                    }
                }
                //Large Glintstone Scrap - increased chance from sorcerer miner, greater amount dropped
                {
                    /*    //check the lines 
                        var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.GetLinesOnCondition(
                                new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse.AND(
                                new Condition.HasInName(new string[] { "Glintstone Stonedigger", "Cuckoo Knight" })))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                                //.GetNextFreeIds();
                                .GetIDs()
                                ;
                        lotItemToLineIDsDict.Add(new LotItem(good, "Large Glintstone Scrap"), ids);
                        */
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(ItemLotParam_enemy.GetLinesOnCondition(new Condition.HasInName("] Large Glintstone Scrap"))).ToList();
                    materials_chance_percentMultipler.Add(new LotItem(good, "Large Glintstone Scrap", 0, 220));
                    materials_to_set_max.Add(new LotItem(good, "Large Glintstone Scrap", 4));
                }
                //Cuckoo Glintstone
                {
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(ItemLotParam_enemy.GetLinesOnCondition(new Condition.HasInName("] Large Glintstone Scrap"))).ToList();
                    materials_chance_percentMultipler.Add(new LotItem(good, "Cuckoo Glintstone", 0, 150));
                    materials_to_set_max.Add(new LotItem(good, "Cuckoo Glintstone", 3));
                }

                //Buff all arrow and bolt drops
                {
                    var Cond = new Condition.NameEndsWith("Arrow").OR(new Condition.NameEndsWith("Bolt"));//new Condition.FloatFieldBetween(1, 50000000, 60000000);
                    var Ids = ((Lines)EquipParamWeapon.GetLinesOnCondition(Cond)).GetIDs();
                    foreach (int id in Ids)
                    {
                        materials_chance_percentMultipler.Add(new LotItem(LotItem.Category.Weapon, id, 0, 100));
                        materials_to_set_max.Add(new LotItem(LotItem.Category.Weapon, id, 200).addKW("%").addKW("!Param:ItemLotParam_map!").addKW(""));
                        materials_to_set_max.Add(new LotItem(LotItem.Category.Weapon, id, 200).addKW("%").addKW("!Param:ItemLotParam_enemy!").addKW(""));
                        setLots.Add(new LotItem(LotItem.Category.Weapon, id, 0).addKW("!Unspecified Amount!").addKW("!Param:ItemLotParam_map!").addKW(""));
                    }
                    var lines = ItemLotParam_enemy.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[1], Ids)).
                        Concat(ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[0], Ids)));
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //Buff all throwing daggers type consumables drops
                {
                    var Cond = new Condition.NameIs("Throwing Dagger").OR(new Condition.NameIs("Fan Daggers")).OR(new Condition.NameEndsWith("Dart"));
                    var Ids = ((Lines)EquipParamGoods.GetLinesOnCondition(Cond)).GetIDs();
                    foreach (int id in Ids)
                    {
                        materials_chance_percentMultipler.Add(new LotItem(good, id, 0, 100));
                        materials_to_set_max.Add(new LotItem(good, id, 300).addKW("%").addKW("!Param:ItemLotParam_map!").addKW(""));
                        materials_to_set_max.Add(new LotItem(good, id, 300).addKW("%").addKW("!Param:ItemLotParam_enemy!").addKW(""));
                        setLots.Add(new LotItem(good, id, 5).addKW("!Unspecified Amount!").addKW("!Param:ItemLotParam_map!").addKW(""));
                    }
                    var lines = ItemLotParam_enemy.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[1], Ids)).
                        Concat(ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[0], Ids)));
                    //var debugCount = lines.Count();
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //Buff all kukris
                {
                    var Cond = new Condition.NameIs("Kukri");
                    var Ids = ((Lines)EquipParamGoods.GetLinesOnCondition(Cond)).GetIDs();
                    foreach (int id in Ids)
                    {
                        materials_chance_percentMultipler.Add(new LotItem(good, id, 0, 100));
                        materials_to_set_max.Add(new LotItem(good, id, 250).addKW("%").addKW("!Param:ItemLotParam_map!").addKW(""));
                        materials_to_set_max.Add(new LotItem(good, id, 250).addKW("%").addKW("!Param:ItemLotParam_enemy!").addKW(""));
                        setLots.Add(new LotItem(good, id, 0).addKW("!Unspecified Amount!").addKW("!Param:ItemLotParam_map!").addKW(""));
                    }
                    var lines = ItemLotParam_enemy.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[1], Ids)).
                        Concat(ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[0], Ids)));
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //Buff all gravity stone type items
                {
                    var Cond = new Condition.NameStartsWith("Gravity Stone");
                    var Ids = ((Lines)EquipParamGoods.GetLinesOnCondition(Cond)).GetIDs();
                    foreach (int id in Ids)
                    {
                        materials_chance_percentMultipler.Add(new LotItem(good, id, 0, 250));
                        materials_to_set_max.Add(new LotItem(good, id, 250).addKW("%").addKW("!Param:ItemLotParam_map!").addKW(""));
                        materials_to_set_max.Add(new LotItem(good, id, 250).addKW("%").addKW("!Param:ItemLotParam_enemy!").addKW(""));
                        setLots.Add(new LotItem(good, id, 0).addKW("!Unspecified Amount!").addKW("!Param:ItemLotParam_map!").addKW(""));
                    }
                    var lines = ItemLotParam_enemy.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[1], Ids)).
                        Concat(ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[0], Ids)));
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //Buff drawstring grease
                {
                    var Cond = new Condition.NameStartsWith("Drawstring").AND(new Condition.NameEndsWith("Grease"));
                    //var Cond = new Condition.NameStartsWith("Drawstring").IsFalse.AND(Condition.FieldEndsWith("Grease")); normal grease
                    var Ids = ((Lines)EquipParamGoods.GetLinesOnCondition(Cond)).GetIDs();
                    foreach (int id in Ids)
                    {
                        materials_chance_percentMultipler.Add(new LotItem(good, id, 0, 100));
                        materials_to_set_max.Add(new LotItem(good, id, 250).addKW("%").addKW("!Param:ItemLotParam_map!").addKW(""));
                        setLots.Add(new LotItem(good, id, 2).addKW("!Unspecified Amount!").addKW("!Param:ItemLotParam_map!").addKW(""));
                    }
                    var lines = ItemLotParam_enemy.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[1], Ids)).
                        Concat(ItemLotParam_map.GetLinesOnCondition(new Condition.FieldIs(LotItem.idFIs[0], Ids)));
                    NonMaterialLinesToInclude = NonMaterialLinesToInclude.Concat(lines).ToList();
                }
                //root resin - skeletons - erdtree guardan
                {
                    var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] { "Skeleton", "Guardian" })
                        .AND(new Condition.HasInName(new string[] { "Giant Skeleton Torso", "Guardian Golem", "Flame Guardian", "Chief Guardian Arghanthy" }).IsFalse)
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Catacombs", "Hero's Grave", "Cave" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Root Resin", 1, 70,true), ids);
                }
                //Golden rowa - lleyndel foot soldier, celebrant
                {
                    var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] { "Leyndell Footsoldier", "Wandering Noble" })
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Altus" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Golden Rowa", 1, 50,true), ids);
                }
                //Rimed rowa - snowfield troll, wandring noble in snowfield or mountain tops
                {
                    var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] { "Wandering Noble", "Troll" })
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Snowfield","Mountaintop" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Rimed Rowa", 1, 50, true), ids);
                }
                //Crystal Bud - cuckoo foot soldiers?, albinauric crabs?, crayfish (in luirnuia), miranda (magic varient)?, 
                {
                    var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] {"Crayfish"})
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Liurnia" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Crystal Bud",12,200, true), ids);
                }
                //gold firefly - bear, rune bear
                {
                    var names = new string[] { "Runebear", "Bear" };
                    for (int i = 0; i < names.Length; i++)
                    {
                        var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(names[i])
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Mistwood" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                        if (names[i] == "Runebear")
                            lotItemToLineIDsDict.Add(new LotItem(good, "Gold Firefly", 7, 150), ids);
                        else if(names[i] == "Bear")
                            lotItemToLineIDsDict.Add(new LotItem(good, "Gold Firefly", 2, 150), ids);
                    }
                }
                //cave moss - demihumans (inside caves), highwayman?
                {
                    var ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] { "Demi-Human" })
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Cave" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Cave Moss", 1, 80, true), ids);
                    //------
                    ids = ((Lines)ItemLotParam_enemy.GetLinesWithId(((Lines)NpcParam.vanillaParamFile.GetLinesOnCondition(
                        new Condition.FloatFieldBetween(itemLotId_enemy, -1, 0, true).IsFalse
                        .AND(new Condition.HasInName(new string[] { "Highwayman" })
                        .AND(new Condition.OneKeywordPassesCondition(new KeywordCondition.StartsWith("location:")
                            .AND(new KeywordCondition.Contains(new string[] { "Cave" }),true)))
                        ))).GetIntFields(itemLotId_enemy).Distinct().ToArray()))
                            //.GetNextFreeIds();
                            .GetIDs()
                            ;
                    lotItemToLineIDsDict.Add(new LotItem(good, "Cave Moss", 1, 80, true), ids);
                    materials_to_set_max.Add(new LotItem(good, "Cave Moss", 4).addKW("Highwayman"));
                }

                //throwing dart - imps that throw darts?
                //cuckoo glintstone - cuckoo soldiers?
                //glintstone scrap - miners?

                //we will simply create a line with lot of the requested item, Increased Material Drops will st the lots to out prefrence.

                //Util.PrintStrings(Util.ToStrings(ItemLotParam_enemy.GetIDs()));

                int c = ItemLotParam_enemy.lines.Count();
                //ItemLotParam_enemy.lines = ItemLotParam_enemy.GetOrderedLines();

                foreach(LotItem lotItem in lotItemToLineIDsDict.Keys)
                {
                    int[] ids = lotItemToLineIDsDict[lotItem];
                    foreach (int id in ids)
                    {
                        Line ogLine = ItemLotParam_enemy.GetLineWithId(id);
                        int newLineId = ItemLotParam_enemy.GetNextFreeId(id);
                        string lotItemName = lotItem.Name;
                        
                        //if (ogLine.name.Contains("[Demi-"))
                        //    Util.p();
                        int firstEmptyLotIndex = LotItem.GetFirstNonEmptyItemLotIndex(ogLine);
                        string oldName;
                        if (firstEmptyLotIndex == -1)
                        {
                            oldName = "None";
                            firstEmptyLotIndex = 1;
                            Util.p();
                        }
                        else
                        {
                            oldName = new LotItem(ogLine, firstEmptyLotIndex).Name;
                        }
                        string lotName = ogLine.name.Replace(oldName, "") +lotItemName;
                        Line line = LotItem.newBaseItemLotLine(ItemLotParam_enemy)
                            .SetField(0, newLineId)
                            .SetField(1, lotName);
                        int flagId = lotItem.lotItem_getItemFlagId;

                        LotItem lastLotItem = LotItem.newEmpty();
                        if (lotItem.hasLotItem_getItemFlagId && flagId < -1)
                        {
                            lotItem.lotItem_getItemFlagId = -flagId;
                            lastLotItem = LotItem.newEmpty(LotItem.MAX_CHANCE, false, -flagId);
                        }
                        line.Operate(new SetLotItems(new LotItem[] { LotItem.newEmpty(Math.Max(0,1000 - lotItem.chance)), lotItem, lastLotItem }, 1));
                        //line.PrintFieldsWithFieldName();
                        //if(!isMaterial.Pass(line))
                        //    NonMaterialLinesToInclude.Add(line);
                        //Util.println(id + " --> " + newLineId);
                        ItemLotParam_enemy.OverrideOrAddLine(line);
                    }
                }
                
            }
           /* materials_to_exclude.Clear();
            material_chancePercent_multipler.Clear();
            materials_to_set_max.Clear();
            setLots.Clear();
            NonMaterialLinesToAdd.Clear();*/

            //Increased Materal Drops
            {
                
                var materialLines = ItemLotParam_enemy.GetLinesOnCondition(isMaterial);

                materialLines = materialLines.Concat(NonMaterialLinesToInclude).ToList();

                materials_to_exclude = materials_to_exclude.Concat(new LotItem[] {

                }).ToList();

                const int defaultMax = 3;
                const int chanceAdjPercent = 125;
                //const int chanceAdjPercent_PerXamount = 50;
                const int minChance = 0;

                LotItem.Default_Chance = -1;

                materials_chance_percentMultipler = materials_chance_percentMultipler.Concat(new LotItem[]
                {

                    new LotItem(good,"Four-Toed Fowl Foot",0, 125),
                    new LotItem(good,"Trina's Lily",0,300),
                    new LotItem(good,"Miquella's Lily",0,300),
                    new LotItem(good,"Beast Liver",0,200),
                    new LotItem(good,"Turtle Neck Meat",0,200),

                    new LotItem(good,"Stormhawk Feather",0,200).addKW("[Warhawk]"),
                    new LotItem(good,"Stormhawk Feather",0,250).addKW(""),

                    new LotItem(good,"Flight Pinion",0,250).addKW("[Warhawk]"),

                    new LotItem(good,"Land Octopus Ovary",0, 200),


                    new LotItem(good,"Thin Beast Bones",0,100).addKW("[Goat]").addKW("[Rabbitgaroo]").addKW("Wolf]"),
                    new LotItem(good,"Thin Beast Bones",0,150),

                    new LotItem(good,"Hefty Beast Bone",0,100).addKW("[Goat]").addKW("[Rabbitgaroo]"),
                    new LotItem(good,"Hefty Beast Bone",0,125).addKW("[Runebear]"),
                    new LotItem(good,"Hefty Beast Bone",0,150).addKW("Putrid Flesh]"),
                    new LotItem(good,"Hefty Beast Bone",0,200),

                    new LotItem(good,"Budding Horn",0,250),

                    new LotItem(good,"Great Dragonfly Head",0,150),

                    new LotItem(good,"Bloodrose",0,250),
                    new LotItem(good,"Aeonian Butterfly",0,250).addKW("of Rot]"),
                    new LotItem(good,"Smoldering Butterfly",0,200).addKW("Fire Monk").addKW("[Blackflame Monk]"),

                    new LotItem(good,"Old Fang",0,450).addKW("[Winged Misbegotten]"), //Leonine Misbegotten for some reason misnamed itemlot 
                    new LotItem(good,"Old Fang",0,200),

                    new LotItem(good,"String",0,160).addKW("[Large Demi-Human]"),
                }).ToList();

                materials_to_set_max = materials_to_set_max.Concat(new LotItem[] {
                    new LotItem(good,"Four-Toed Fowl Foot",2), //
                    new LotItem(good,"Trina's Lily",1), //maintain rarity
                    new LotItem(good,"Beast Liver",1),
                    new LotItem(good,"Turtle Neck Meat",1),
                    new LotItem(good,"Great Dragonfly Head",1),
                    new LotItem(good,"Yellow Ember",2),

                    //new LotItem(good,"Bloodrose",4).addKW("[Sanguine Noble]"),

                    new LotItem(good,"Stormhawk Feather",5).addKW("[Warhawk]"),
                    new LotItem(good,"Flight Pinion",5).addKW("[Warhawk]"),

                    new LotItem(good,"Land Octopus Ovary",2),  //only 2 overies in body
                    new LotItem(good,"Budding Horn",1).addKW("Perfumer"),
                    new LotItem(good,"Budding Horn",4).addKW("Ancestral Follower"),
                    new LotItem(good,"Budding Horn",2),


                    new LotItem(good,"Old Fang",4).addKW("[Winged Misbegotten]"), //Leonine Misbegotten for some reason misnamed itemlot 
                    new LotItem(good,"Old Fang",3).addKW("Misbegotten]"),
                    new LotItem(good,"Old Fang",2),

                    new LotItem(good,"Hefty Beast Bone",3).addKW("[Giant Putrid Flesh]"),
                    new LotItem(good,"Hefty Beast Bone",1).addKW("[Small Putrid Flesh]"),
                }).ToList();

                setLots = setLots.Concat(new LotItem[]
                {
                    new LotItem(good,"String",1,15).addKW("[Demi-Human]").addKW("[Large Demi-Human]").addKW("[Demi-Human Shaman]"),
                    new LotItem(good,"String",2,40).addKW("[Demi-Human]").addKW("[Large Demi-Human]").addKW("[Demi-Human Shaman]"),
                    new LotItem(good,"String",3,70).addKW("[Demi-Human]").addKW("[Large Demi-Human]").addKW("[Demi-Human Shaman]"),
                    new LotItem(good,"String",4,100).addKW("[Demi-Human]").addKW("[Large Demi-Human]").addKW("[Demi-Human Shaman]"),
                    new LotItem(good,"String",5,25).addKW("[Demi-Human]").addKW("[Large Demi-Human]").addKW("[Demi-Human Shaman]"),

                    new LotItem(good,"String",4,100).addKW("[Demi-Human Beastman]"),
                    new LotItem(good,"String",5,150).addKW("[Demi-Human Beastman]"),
                    new LotItem(good,"String",6,200).addKW("[Demi-Human Beastman]"),
                    new LotItem(good,"String",7,50).addKW("[Demi-Human Beastman]"),


                    new LotItem(good,"Thin Beast Bones",1,350).addKW("[Goat]").addKW("[Rabbitgaroo]").addKW("Deer"),
                    new LotItem(good,"Thin Beast Bones",2,300).addKW("[Goat]").addKW("[Rabbitgaroo]").addKW("Deer"),
                    new LotItem(good,"Thin Beast Bones",3,100).addKW("[Goat]").addKW("[Rabbitgaroo]").addKW("Deer"),

                    new LotItem(good,"Thin Beast Bones",2,10).addKW("%").addKW("[White Wolf]"),
                    new LotItem(good,"Thin Beast Bones",3,30).addKW("%").addKW("[White Wolf]"),
                    new LotItem(good,"Thin Beast Bones",4,35).addKW("%").addKW("[White Wolf]"),
                    new LotItem(good,"Thin Beast Bones",5,25).addKW("%").addKW("[White Wolf]"),

                    new LotItem(good,"Thin Beast Bones",1,20).addKW("").addKW("%"),
                    new LotItem(good,"Thin Beast Bones",2,30).addKW("").addKW("%"),
                    new LotItem(good,"Thin Beast Bones",3,40).addKW("").addKW("%"),
                    new LotItem(good,"Thin Beast Bones",4,10).addKW("").addKW("%"),

                    


                    new LotItem(good,"Stormhawk Feather",1,10).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Stormhawk Feather",2,30).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Stormhawk Feather",3,50).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Stormhawk Feather",4,20).addKW("[Warhawk]").addKW("%"),

                    new LotItem(good,"Flight Pinion",1,20).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Flight Pinion",2,30).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Flight Pinion",3,40).addKW("[Warhawk]").addKW("%"),
                    new LotItem(good,"Flight Pinion",4,10).addKW("[Warhawk]").addKW("%"),

                    new LotItem(good,"Flight Pinion",1,36).addKW("").addKW("%"),
                    new LotItem(good,"Flight Pinion",2,25).addKW("").addKW("%"),
                    new LotItem(good,"Flight Pinion",3,10).addKW("").addKW("%"),
                    new LotItem(good,"Flight Pinion",4,4).addKW("").addKW("%"),

                    new LotItem(good,"Hefty Beast Bone",1,200).addKW("[Bear]"),
                    new LotItem(good,"Hefty Beast Bone",2,350).addKW("[Bear]"),
                    new LotItem(good,"Hefty Beast Bone",3,50).addKW("[Bear]"),

                    new LotItem(good,"Hefty Beast Bone",1,200).addKW("[Boar]"),
                    new LotItem(good,"Hefty Beast Bone",2,125).addKW("[Boar]"),
                    new LotItem(good,"Hefty Beast Bone",3,25).addKW("[Boar]"),

                    new LotItem(good,"Hefty Beast Bone",2,10).addKW("%").addKW("[Runebear]"),
                    new LotItem(good,"Hefty Beast Bone",3,30).addKW("%").addKW("[Runebear]"),
                    new LotItem(good,"Hefty Beast Bone",4,40).addKW("%").addKW("[Runebear]"),
                    new LotItem(good,"Hefty Beast Bone",5,20).addKW("%").addKW("[Runebear]"),


                    new LotItem(good,"Hefty Beast Bone",1,65).addKW("[Small Putrid Flesh]").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",2,35).addKW("[Small Putrid Flesh]").addKW("%"),


                    new LotItem(good,"Hefty Beast Bone",1,20).addKW("[Giant Putrid Flesh]").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",2,40).addKW("[Giant Putrid Flesh]").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",3,30).addKW("[Giant Putrid Flesh]").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",4,10).addKW("[Giant Putrid Flesh]").addKW("%"),


                    new LotItem(good,"Hefty Beast Bone",1,20).addKW("").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",2,30).addKW("").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",3,40).addKW("").addKW("%"),
                    new LotItem(good,"Hefty Beast Bone",4,10).addKW("").addKW("%"),

                    new LotItem(good,"Smoldering Butterfly",1,200).addKW("[Guilty]"),
                    new LotItem(good,"Smoldering Butterfly",2,50).addKW("[Guilty]"),

                    //new LotItem(good,"Smoldering Butterfly",3,45).addKW("[Fire Monk]").addKW("%"),
                    //new LotItem(good,"Smoldering Butterfly",4,60).addKW("[Fire Monk]").addKW("%"),
                    new LotItem(good,"Smoldering Butterfly",5,100).addKW("[Fire Monk]").addKW("%"),

                    //new LotItem(good,"Smoldering Butterfly",4,45).addKW("[Blackflame Monk]").addKW("%"),
                    //new LotItem(good,"Smoldering Butterfly",5,60).addKW("[Blackflame Monk]").addKW("%"),
                    new LotItem(good,"Smoldering Butterfly",6,100).addKW("[Blackflame Monk]").addKW("%"),

                }).ToList();

                foreach (Line curLine in materialLines)
                {
                    bool test = false;//curLine.id == "355000000";
                    string debugName = curLine._idName;
                    
                    int lotIndex = 1;
                    if (test)
                        Util.p();
                    {
                        bool doSkip = false;
                        foreach (LotItem li in materials_to_exclude)
                        {
                            if (li.LineHasLotItem(curLine, false))
                            {
                                doSkip = true;
                                break;
                            }
                        }
                        if (doSkip)
                            continue;
                    }
                    float chanceMult = (float)chanceAdjPercent / 100;
                    foreach (LotItem li in materials_chance_percentMultipler)
                    {
                        if (li.hasKeywordContaining("!Param:") && !li.hasKeyword("!Param:" + curLine.file.paramName.Replace(".csv","") + "!"))
                            continue;
                        
                        if (!li.LineHasLotItem(curLine, false))
                            continue;
                        if (test)
                            Util.p();
                        if (li.keywords.Count == 0)
                        {
                            if (test)
                                Util.p();
                            chanceMult = (float)li.chance / 100;
                            break;
                        }
                        else
                        {
                            bool doBreak = false;
                            foreach (Keyword k in li.keywords)
                            {
                                if (curLine.name.Contains(k.keyword))
                                {
                                    chanceMult = (float)li.chance / 100;
                                    doBreak = true;
                                    break;
                                }
                            }
                            if (doBreak)
                                break;
                        }
                    }
                    int maxAmount = defaultMax;
                    //int maxAmountPercentMultiplier = -1;
                    bool maxAmountPercentMultiplier = false;
                    foreach (LotItem li in materials_to_set_max)
                    {
                        if (li.hasKeywordContaining("!Param:") && !li.hasKeyword("!Param:" + curLine.file.paramName.Replace(".csv","") + "!"))
                            continue;
                        if (!li.LineHasLotItem(curLine, false))
                            continue;
                        if (li.keywords.Count == 0)
                        {
                            maxAmount = li.amount;
                            break;
                        }
                        else
                        {
                            bool doBreak = false;
                            foreach (Keyword k in li.keywords)
                            {
                                if (k.keyword == "%")
                                    continue;
                                if (curLine.name.Contains(k.keyword))
                                {
                                    if (li.hasKeyword("%"))
                                        //maxAmountPercentMultiplier = li.keywords[li.keywords.GetIndexOf("%")].value;
                                        maxAmountPercentMultiplier = true;
                                    maxAmount = li.amount;
                                    doBreak = true;
                                    break;
                                }
                            }
                            if (doBreak)
                                break;
                        }
                    }

                    bool defaultMaxSpread = true;
                    int maxSpread = 2;
                    int minSpread = -2;

                    List<LotItem> lotItemsSelected = new List<LotItem>();
                    {
                        int state = 0;
                        const int lookingForLotsState = 0;
                        const int foundLotsGeneralState = 1;
                        const int foundLotsSpecificState = 2;
                        for (int i = 0; i < setLots.Count; i++)
                        {
                            LotItem li = setLots[i];
                           

                            if (li.hasKeywordContaining("!Param:") && !li.hasKeyword("!Param:" + curLine.file.paramName.Replace(".csv", "") + "!"))
                                continue;

                            if (test)
                                Util.p();
                            if (!li.LineHasLotItem(curLine, false))
                            {
                                if (state != lookingForLotsState)
                                    break;
                                continue;
                            }
                            if (state != foundLotsSpecificState && li.hasKeyword(""))
                            {
                                //if (!maxAmountPercentMultiplier && maxAmount < li.amount)
                                //    maxAmount = li.amount;
                                state = foundLotsGeneralState;
                                if (maxAmount < li.amount)
                                    maxAmount = li.amount;
                                var liCopy = li.Copy();
                                if (li.hasKeyword("!Unspecified Amount!"))
                                {
                                    if (defaultMaxSpread)
                                    {
                                        maxSpread = 0;
                                        minSpread = 0;
                                    }
                                    defaultMaxSpread = false;
                                    if (maxSpread < li.amount)
                                        maxSpread = li.amount;
                                    if (minSpread > li.amount)
                                        minSpread = li.amount;
                                }
                                else
                                {
                                    liCopy.addKW("!Specified Amount!");
                                }
                                lotItemsSelected.Add(liCopy);
                                continue;
                            }
                            else if (state == foundLotsGeneralState)
                            {
                                break;
                            }
                            else
                            {
                                bool found = false;
                                foreach (Keyword k in li.keywords)
                                {
                                    if (k.keyword == "%")
                                        continue;
                                    if (curLine.name.Contains(k.keyword))
                                    {
                                        
                                        state = foundLotsSpecificState;
                                        found = true;
                                        if (maxAmount < li.amount)
                                            maxAmount = li.amount;
                                        var liCopy = li.Copy();
                                        if (li.hasKeyword("!Unspecified Amount!"))
                                        {
                                            if (defaultMaxSpread)
                                            {
                                                maxSpread = 0;
                                                minSpread = 0;
                                            }
                                            defaultMaxSpread = false;
                                            if (maxSpread < li.amount)
                                                maxSpread = li.amount;
                                            if (minSpread > li.amount)
                                                minSpread = li.amount;
                                        }
                                        else
                                            liCopy.addKW("!Specified Amount!");
                                        lotItemsSelected.Add(liCopy);
                                        break;
                                    }
                                }
                                if (!found && state == foundLotsSpecificState)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    float totalItemPercent = LotItem.GetItemChanceTotal(curLine);
                    float totalEmptyPercent = LotItem.GetEmptyChanceTotal(curLine);
                    bool firstLotEmpty = totalEmptyPercent > 0;


                    float newTotalItemPercent = 0;
                    float newTotalEmptyPercent = 1000;

                    float currentChance = totalItemPercent / (totalEmptyPercent + totalItemPercent);
                    float newChance = currentChance * chanceMult * DROPMULT;
                    float newItemTotalTarget = newChance * 1000;

                    if (test)
                        Util.p();
                    int totalSpread = maxSpread - minSpread;
                    LotItem baseLotItem = new LotItem(curLine, LotItem.GetFirstNonEmptyItemLotIndex(curLine), true);
                    var baseLineAmount = baseLotItem.amount;
                    if (maxAmountPercentMultiplier)
                         maxAmount = (int)(((float)maxAmount / 100 * baseLineAmount)+0.5);
                    int startingAmount = Math.Max(baseLineAmount, maxAmount - maxSpread);
                    if(!maxAmountPercentMultiplier)
                        maxAmount += Math.Max(0, baseLineAmount - 1);
                    if (test)
                        Util.p();

                    if (lotItemsSelected.Count == 0)
                    {
                        lotItemsSelected = new LotItem[] {
                            new LotItem(good, baseLotItem.id, -2,7).addKW("%"),//remove startingAmount? test
                            new LotItem(good, baseLotItem.id, -1,18).addKW("%"),
                            new LotItem(good, baseLotItem.id, 0,41).addKW("%"),
                            new LotItem(good, baseLotItem.id, 1,26).addKW("%"),
                            new LotItem(good, baseLotItem.id, 2,12).addKW("%"),
                        }.ToList();
                        //newItemTotalTarget = totalItemPercent * DROPMULT;
                        //newTotalEmptyPercent = 1000 - totalItemPercent;
                    }
                    var floatChances = new List<float>();
                    {
                        
                        for (int i = 0; i < lotItemsSelected.Count; i++)
                        {
                            if (lotItemsSelected[i].hasKeyword("!Specified Amount!"))
                                lotItemsSelected[i].amount = lotItemsSelected[i].amount;
                            else
                                lotItemsSelected[i].amount = startingAmount + (lotItemsSelected[i].amount); 
                            float chance = lotItemsSelected[i].chance;
                            if (lotItemsSelected[i].hasKeyword("%"))
                            {
                                chance = (chance / 100f) * newItemTotalTarget;
                            }
                            else
                            {
                                chance *= DROPMULT;
                            }
                            if (test)
                                Util.p();
                            //lotItemsSelected[i].chance = (int)Math.Round(chance);
                            floatChances.Add(chance);
                            newTotalItemPercent += chance;
                            newTotalEmptyPercent -= chance;
                        }
                        if (newTotalEmptyPercent < 0.5f)
                            firstLotEmpty = false;
                        if (test)
                            Util.p();
                    }

                    
                    {
                        float lostPercent = 0;
                        //float chanceMultAddedChance = 0;
                        for (int i = 0; i < lotItemsSelected.Count; i++)
                        {
                            var li = lotItemsSelected[i];

                            float chance = floatChances[i];
                            //chanceMultAddedChance += (chance * DROPMULT) - chance;
                            if (test)
                                Util.p();
                            if (li.amount < 1 || li.amount > maxAmount || chance < minChance)
                            {
                                lostPercent += chance; //* Math.Max(1,li.amount);
                                lotItemsSelected.RemoveAt(i);
                                floatChances.RemoveAt(i);
                                if (test)
                                    Util.p();
                                i--;
                                continue;
                            }
                        }
                        if (lotItemsSelected.Count == 0)
                            continue;
                        float lostPercentMult = 1+(lostPercent/newTotalItemPercent);
                        if (test)
                            Util.p();
                        for (int i = 0; i < floatChances.Count; i++)
                        {
                            floatChances[i] *= lostPercentMult;
                        }
                    }

                    //float newChanceMult = (totalItemPercent / ((float)totalItemPercent - lostPercent));
                    if (test)
                       Util.p();

                    if (firstLotEmpty)
                    {
                        LotItem.newEmpty((int)Math.Round(newTotalEmptyPercent)).SetLotItemToLine(curLine, lotIndex);
                        lotIndex++;
                    }

                    for (int i = 0; i < lotItemsSelected.Count; i++)
                    {
                        var li = lotItemsSelected[i];
                        li.chance = Math.Min(LotItem.MAX_CHANCE,Math.Max(0,(int)Math.Round(floatChances[i])));
                        curLine.Operate(new LotItem.SetLotItem(li, lotIndex));
                        if (firstLotEmpty)
                            curLine.SetField(LotItem.affectByLuckFIs[lotIndex - 1], true);
                        if (test)
                            Util.p();
                        lotIndex++;
                        if (lotIndex > LotItem.MAX_LOT_INDEX)
                            break;
                    }
                    for (; lotIndex <= LotItem.MAX_LOT_INDEX; lotIndex++)
                    {
                        LotItem.newEmpty().SetLotItemToLine(curLine, lotIndex);
                    }
                    LotItem.RegulateLine(curLine);
                    //check set slots
                    //check if lotitem applies to line  if not next.
                    //manual check all kws to see if it contains the name
                    //use kw "" as a sign to use percentages not numbers.
                }
                LotItem.Default_Chance = 1000;
            }


            /*ItemLotParam_enemy.PrintCompareLineChanges();
            var ml = ItemLotParam_enemy.numberOfModifiedOrAddedLines;
            RunSettings.PrintFile_VerifyFieldCounts = true;
            Util.println(ItemLotParam_enemy.VerifyFieldCounts());*/

        }
        static void enemyDrops_OneTimeEquipmentDrops(bool guarenteedFirstDrop = false)
        {

            if (!IsRunningParamFile(new ParamFile[] { ItemLotParam_enemy }))
                return;

            var usedGetItemFlagId = FlagIds.usedGetItemFlagId;

            float OneTimeWeaponAndArmorDrops_WeaponDropChanceMult = 2f;
            float OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult = 0.4f;
            if (guarenteedFirstDrop)
            {
                OneTimeWeaponAndArmorDrops_WeaponDropChanceMult = 10000;
                OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult = 0.4f;
            }

            float OneTimeWeaponAndArmorDrops_ArmorDropChanceMult = 2.5f;
            float OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult = 0.25f;
            if (guarenteedFirstDrop)
            {
                OneTimeWeaponAndArmorDrops_ArmorDropChanceMult = 10000;
                OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult = 1;
            }



            

            Dictionary<int, int> AssignedWeaponsFlagIdDict;
            Dictionary<int, int> AssignedArmorsFlagIdDict;
            AssignedWeaponsFlagIdDict = new Dictionary<int, int>();
            AssignedArmorsFlagIdDict = new Dictionary<int, int>();

            {
                Lines oneTimeWeaponDropLines = ItemLotParam_enemy.GetLinesOnCondition(
                    new Condition.FieldIs(LotItem.idFIs[0], LotItem.Category.Weapon.ToString())
                    .AND(new Condition.FieldIs(LotItem.getItemFlagIdFI, "0"))
                    );
                oneTimeWeaponDropLines.PrintIDAndNames(" - pre assigned - ");
                foreach (Line line in oneTimeWeaponDropLines.lines)
                {
                    AssignedWeaponsFlagIdDict.Add(line.GetFieldAsInt(LotItem.idFIs[0]), line.GetFieldAsInt(LotItem.getItemFlagIdFI));
                }

                Lines oneTimeArmorDropLines = ItemLotParam_enemy.GetLinesOnCondition(
                    new Condition.FieldIs(LotItem.idFIs[0], LotItem.Category.Armor.ToString())
                    .AND(new Condition.FieldIs(LotItem.getItemFlagIdFI, "0"))
                    );
                oneTimeArmorDropLines.PrintIDAndNames(" - pre assigned - ");
                foreach (Line line in oneTimeArmorDropLines.lines)
                {
                    AssignedArmorsFlagIdDict.Add(line.GetFieldAsInt(LotItem.idFIs[0]), line.GetFieldAsInt(LotItem.getItemFlagIdFI));
                }
            }

            var isArmorAdjustExceptionCond = new Condition.HasInName("Banished Knight");
            var isNotArrowCond = new Condition.FloatFieldCompare(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), Condition.LESS_THAN, 50000000);
            var isWeaponCond = new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemCategory02"), "2");
            var isArmorCond = new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemCategory02"), "3");

            var curLineHasOneItemCond = //naturally excludes any guarenteed drops (1000 chance_ that are narmally at lot 01.
                                        
                //new Condition.MultiFieldCondition(new Condition.FloatFieldCompare(Condition.GREATER_THAN, 0), LotItem.lotItem_getItemFlagIdFIs, false);
                new Condition.AllOf(
                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId01"), "0"),
                new Condition.FloatFieldCompare(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), Condition.GREATER_THAN, 0),
                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId03"), "0")) ;


            foreach (Line curLine in ItemLotParam_enemy.lines)
            {

                if (curLineHasOneItemCond.Pass(curLine))
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
                        //OtherToAddId = _curLot2Id + 1000;
                        //var OtherToAddEquipLine = EquipParamProtector.GetLineWithId(OtherToAddId);
                        var ItemName = EquipParamProtector.GetLineWithId(_curLot2Id).name;
                        var OtherToAddEquipLine = EquipParamProtector.GetLineWithName(ItemName + " (Altered)");
                        if (OtherToAddEquipLine != null)
                        {
                            OtherToAddId = OtherToAddEquipLine.id_int;
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

                        if (AssignedArmorsFlagIdDict.ContainsKey(_curLot2Id))
                            FlagId = AssignedArmorsFlagIdDict[_curLot2Id];
                        else
                            createUniqueFlagId = true;
                        PercentMult = OneTimeWeaponAndArmorDrops_ArmorDropChanceMult;
                        if (OtherToAddId != -1)
                        {
                            OtherCategory = 3;
                            if (AssignedArmorsFlagIdDict.ContainsKey(OtherToAddId))   //fair to assume it exists
                                OtherFlagId = AssignedArmorsFlagIdDict[OtherToAddId];
                            else
                                createUniqueFlagIdForOther = true;
                            OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_AlteredArmorDropChanceMult;
                            addToName = ItemName + " & " + OtherToAddEquipLine.name;
                        }
                    }
                    else if (isWeapon && isNotArrow)
                    {
                        isWeapon = true;
                        PercentMult = OneTimeWeaponAndArmorDrops_WeaponDropChanceMult;
                        OtherToAddPercentMult = OneTimeWeaponAndArmorDrops_SecondWeaponDropChanceMult;
                        if (AssignedWeaponsFlagIdDict.ContainsKey(_curLot2Id))
                            FlagId = AssignedWeaponsFlagIdDict[_curLot2Id];
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
                        curLine.SetField("lotItemBasePoint03", Math.Min(34463, OtherPercent));
                        int _curLot1Percent = int.Parse(curLine.GetField("lotItemBasePoint01"));
                        curLine.SetField("lotItemBasePoint01", Math.Max(0, Math.Min(34463, _curLot1Percent - OtherPercent)));
                    }
                    curLine.SetField(1, curLine.name + addToName);
                    if (PercentMult != -1)
                    {
                        int NewPercent = Math.Max(1, (int)Math.Round(_curLot2Percent * PercentMult));
                        int addedAmount = NewPercent - _curLot2Percent;
                        curLine.SetField("lotItemBasePoint02", Math.Min(34463, NewPercent));
                        if (!guarenteedFirstDrop)
                        {
                            int _curLot1Percent = int.Parse(curLine.GetField("lotItemBasePoint01"));
                            curLine.SetField("lotItemBasePoint01", Math.Min(34463, Math.Max(0, _curLot1Percent - addedAmount)));
                        }
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
                        int currentGetItemFlagId = IntFilter.GetRandomInt(curLine.id_int, IdFilters.OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                        usedGetItemFlagId.Add(currentGetItemFlagId);

                        if (isArmor)
                            AssignedArmorsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);
                        else if (isWeapon)
                            AssignedWeaponsFlagIdDict.Add(_curLot2Id, currentGetItemFlagId);

                        if (OtherToAddId == -1)
                        {
                            curLine.SetField("getItemFlagId", currentGetItemFlagId);
                            //line.SetField("canExecByFriendlyGhost", "0");
                        }
                        else
                            curLine.SetField("getItemFlagId02", currentGetItemFlagId);
                    }
                    if (OtherFlagId != -1)
                        curLine.SetField("getItemFlagId03", OtherFlagId);
                    else if (createUniqueFlagIdForOther)
                    {
                        int currentGetItemFlagId = IntFilter.GetRandomInt(curLine.id_int, IdFilters.OneTimeDrop_getItemFlagIDFilter, usedGetItemFlagId);
                        usedGetItemFlagId.Add(currentGetItemFlagId);
                        if (isArmor)
                            AssignedArmorsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                        else if (isWeapon)
                            AssignedWeaponsFlagIdDict.Add(OtherToAddId, currentGetItemFlagId);
                        curLine.SetField("getItemFlagId03", currentGetItemFlagId);
                    }
                }
            }

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

        static void enemyDrops_MoreSmithingStoneDrops(bool RUNES = false, bool STONES = true, float DROPMULT = 1)
        {

            if (!IsRunningParamFile(new ParamFile[]{ ItemLotParam_enemy, NpcParam, ItemLotParam_map}))
                return;

            Lines StoneLines = new Lines(ItemLotParam_enemy);

                //old buffer system
            //ItemLotParam_enemy.GetLineWithId(429000000).addKW("!ADD BUFFER LINE!1"); //compatability plead
            //ItemLotParam_map.GetLineWithId(20370).addKW("!ADD BUFFER LINE!"); //compatability plead

            const int maxLotIndex = 8;


            //set npc Difficulty Dict using NPC Locations
            const float levelEstimateIntreval = 0.5f;

            var npcsDocDifficultyDict = NpcData.NpcIdsToDocDifficultyDict;
            var npcsIdToSpLevelsDict = NpcData.NpcIdsToSpLevelsDict;
            var spLevelToDifficultyDict = NpcData.SpLevelToDifficultyDict;
            var spLevelToStoneDifficultyDict = NpcData.SpLevelToStoneDifficultyDict;

            var BossOrMiniBossIds = NpcData.BossOrMiniBossIds;
            var BossOrMiniBossToItemLotMapDict = NpcData.BossOrMiniBossToItemLotMapDict;
            var BossOrMiniBossToItemLotMapDict2 = NpcData.BossOrMiniBossToItemLotMapDict2;



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
                // numberxxx is casscade for multiple. x3 is 3; 3xx is 3 or 2; 3xxx is 3, 2, or 1.
                // xv multiple chance mult (split). keeps overall chance of a drop the same. if "x3xxx xv0.5" then it has half the chance to drop a multiple.
                // xvv multiple chance mult (split). keeps overall chance of a drop the same. if "x3xxx xvv0.5" then it has half the chance to drop a x2 and quarter chance to drop x1.
                // #number is the set level.
                // ##number is the min level clamp.
                // ###number is the max level clamp.
                // ####number is the min level clamp for ftd and treasure drops.
                // #####number is the max level clamp for ftd and treasure drops.
                // |number is level cascade pooling mult. At 0 level 1 chance doesnt stack.
                // }number is chance mult per Level
                // {number is chance mult per LevelCasscade
                // /number is the percent split for somberstones if drops both.
                // spnumber is the decimal from 0 - 1 that represents the percent of spLevel that goes into the level (if applicable)
                // sspnumber is the decimal from 0 - 1 that represents the percent of stoneSpLevel that goes into the level (if applicable)
                // & is single line. one item type per drop.

                // $ is first time guarendteed drop
                // $$ is Guarrentee first Drop & avoiddropingx1 (includde only x2 and above *if available)
                // $$$ is FTD drops only highest xAmount .
                // $$$$ is FTD drops only highest xAmount & highestLevel.
                // $$$$$ is FTD drops only firstType(normal -> somber) highest xAmount & notSingleLine
                // $$$$$$ is FTD drops only highest xAmount & highestLevel & notSingleLine
                // $$$$$$$ is FTD drops only forcedSomber & highestLevel
                // $$$$$$$$ is FTD drops only firstType(normal -> somber) highest xAmount & highestLevel
                // $$$$$$$$$ is FTD drops x1, only highest level
                // $number is to set the level adj the Guarrentee first Drop.

                //@number           is first drop x1 chance mult. 
                //@@number          is first drop x1 chance mult, excess chance is given to an empty lot
                //@@@number         is first drop x1 chance mult, excess chance is given to an empty lot, flagId shared with Level(level is round to 1)
                //@@@@number        is first drop x1 chance mult, excess chance is given to an empty lot, flagId is shared by all with keyword
                //@@@@@number       is first drop x1 chance mult, excess chance is given to an empty lot, flagId shared with Level(level is round to 1),    dont drop regular drops until treasure is found
                //@@@@@@number      is first drop x1 chance mult, excess chance is given to an empty lot, flagId is shared by all with keyword,             dont drop regular drops until treasure is found
                //@@@@@@@number     is first drop x1 chance mult, excess chance is given to an empty lot, flagId shared with Level(level is round to 1),    dont drop regular drops of type until treasure is found 
                //@@@@@@@@number    is first drop x1 chance mult, excess chance is given to an empty lot, flagId is shared by all with keyword,             dont drop regular drops of type until treasure is found

                // ! is Force Boss Display. the item displays on screen, rather than on corpse.
                // sss+number or sss-number adds and adjust to Somber Smithing Stones
                //Only use if all instances are non respawning. 
                //Only use is if 100% drop rate.
                //MAYBE i should also make this so first item is 100% chance.

                string x3ChanceLevelSpread = " x3x4x5xx xv0.65 xvv0.65 ";
                string x2ChanceLevelSpread = " x2x3x4xx xv0.3 ";
                string x1ChanceLevelSpread = " x1x2x2x3x xvv0.5 ";
                string x2Chance = " x2xx xvv0.3 ";
                string x3Chance = " x3xxx xv0.65 xvv0.65 ";
                string footSoldier = " > -0.9 sp0.1 ###4 }1.028 {0.65";          //}0.965    //we changed them to drop more with higher levels.
                string soldier = " %0.875 sp0.12 >2 ###8 }1.035 {0.65 ";        //}0.985
                string knight = " +0.08 sp0.1 ###8 }1.05 $ @0.175 ";            //}0.985 
                string banishedK = " ###11 +0.11 sp0.15 }1.05 $ @0.175 ";
                string imp = " -1 ###8 }1.035 %0.8 ";                   //}0.965

                string GiantCrab = " %0.5 +1.5 ###13 }0.85 sp1 $0.5 ";

                smithingKeywords = new string[]{


                    "23 > " + knight + x2Chance + "Leyndell Knight (Leyndell- Royal Capital)",
                    "23 > " + knight + x2Chance + "Leyndell Knight - Archer (Leyndell- Royal Capital)",

                    "60 " + banishedK + x3Chance + " Banished Knight",
                    "60 " + banishedK + x3Chance + " Bloodhound Knight",


                    "14 > {0.72 %0.9" + x2ChanceLevelSpread + "Large Exile Soldier",
                    "9 > {0.72 %0.9" + x2ChanceLevelSpread + "Exile Soldier",

                    "2.75" + footSoldier + x2Chance + "Radahn Foot Soldier",
                    "5.5 ##3.5 ###4" + soldier + x2ChanceLevelSpread + "Radahn Soldier",
                    "45 ##4 ###5 sp0.5" + knight + x2Chance + "Redmane Knight",

                    "45 " + knight + x2Chance + "Cuckoo Knight",

                    "2.75" + footSoldier + x2Chance + "Foot Soldier",
                    "5.5 " + soldier + x2ChanceLevelSpread + "Soldier",
                    "45" + knight + x2Chance + "Knight",

                    "7" + imp + x2Chance + "Imp",
                    "1.75 >>> x1x2x3x4xxx %0.5 Vulgar Militia",

                    "45 +1 x2xx $$ ! Duelist",

                    "30 -0.33 $$ Scaly Misbegotten",
                    "2.5 > Misbegotten",

                    "10 > "+x2ChanceLevelSpread+" Page",
                    "2.5 >>"+x1ChanceLevelSpread+"-0.25 %0.75 {0.65 Marionette",
                    "2.5 >>"+x1ChanceLevelSpread+"-0.25 %0.75 {0.65 Avionette",
                    "10 Man-Serpent",
                    "100 $1 ! ###8 Alabaster Lord",
                    "100 -1.3f x5 ! Onyx Lord",
                    "10 >>" +x2ChanceLevelSpread+ "Azula Beastman",
                    "10 >>" +x2Chance+ " Beastman of Farum Azula (Limgrave Cave Boss)",
                    "15 >> $"+x3ChanceLevelSpread+ "Armored Beastman of Farum Azula",
                    "10 >> x6 #4 Armored Beastman of Farum Azula (Boss)", //super high level for some reason
                    "10 >> x4 #4 Azula Beastman (Boss)",                   //super high levvel for some reason

                    "27.5 #8 " + x2Chance + "Kaiden Sellsword",
                    "12 >> -1  x2x3x3xx $$ Demi-Human Chief",
                    "4.2 >> -1 %0.75 x1x2x3 Large Demi-Human",
                    "2.5 >> -1 %0.6 x1x1x3 Demi-Human",
                    //"100 Erdtree Burial Watchdog (Limgrave Catacombs)",
                    //"100 Erdtree Burial Watchdog (Limgrave Catacombs)",
                    //"100 +0.5 $$$$$ x1x2xx xv0.55 xvv0.55 ###8 ! Elder Lion", //worse version of crucible knight drop
                    "4.5 {0.85 >>> x1x1x2x3xx Stonedigger",
                    "100 & >> x3x4x5xxx $$$$$ Stonedigger Troll",

                    "100 x5 Magma Wyrm",
                    "100 x5 Dragonkin Soldier",

                    "100 #500 x5 Placidusax", //level 500 allows for x5 ancient drops. 
                    //"100 & +22 xxx5 $$$$ Ancient Dragon",    //the ones that need to drop already drop ancient dragon smithing stone.
                    
                    "0 Dragonfly",
                    "50 -0.5 > & x4xxx %0.7 sp0.5 Flying Dragon (Small)",
                    "100 & +1 x4xx $$$$ ###8 sp0.15 Dragon",
                    "100 & x4xx $$$$ #3 Glintstone Dragon Smarag",
                    "100 & x4xx $$$$ #8 sss-0.8 Glintstone Dragon Adula",
                    "100 & x4xx $$$$ #7 sss-0.7 Glintstone Dragon (Moonlight Plateau)",

                    "15 %0.70 +1 ssp1 "+x2Chance+" Ancestral Follower",

                    "25 " + x3Chance + " Flame Chariot",
                    "100 > & $$$ x3x4xxx xvv0.85 Fire Prelate",

                    "50 $$$$  x3xxx Leonine Misbegotten",    
                    "50 $$$$$$$ @@@@ +0.5 x3xxx Misbegotten Warrior", //forced somber - wont drop SS
                    
                    "65 > -0.8 x5x7xxx sp0.5 $$$$ Valiant Gargoyle",
                    "65 > -0.8 x5x7xxx sp0.5 $$$$ Black Blade Kindred",

                    "1.75 >> %0.5 }0.85 sp1 & Starcaller",

                    "100 x3 Commander Niall",
                    "100 x3 Commander O'Neil"
                };

                somberKeywords = new string[]{//302b

                    //"100 Godskin Noble",
                    //"100 x3 #9 -2 Godskin Apostle",

                    "35 +0.5 ###9 ! Elder Lion", //somber drop just like existing ones.
                    "100 & +22 xxx5 $$$$ Lichdragon Fortissax",
                    //"1 Lightning Ball",
                    "100 x2 %0.9 ###9 Night's Cavalry",

                    "7.5 -0.5f >>> $$$$ Albinauric Archer",
                    "2.5 %0.6 -0.35 Elder Albinauric Sorcerer",
                    "3 > -1 %0.9 $$$$-1 @@@@@@0.15 ###9 Giant Albinauric Crab",
                    "1.5 #1 Albinauric Crab",
                    "3.5 %0.6 $ @@@0.25 Large Albinauric",
                    "2.5 %0.6 $0.35 @@@0.12 -0.35 Albinauric",

                    "100 +0.5 ! Red Wolf of the Champion",
                    "100 +0.5 ! Red Wolf of Radagon Sword",
                    "100 -0.3 ! Red Wolf of Radagon",        //override for bosses to avoid from catching "Wolf" in runeKeywords.
                    "0 $$$$ #10 /100 Draconic Tree Sentinel",
                    "45 > +1 $$$$$$ {2 Tree Sentinel",

                    "4 @@@@@0.2 Nox",
                    "4 @@@@@0.3 Nightmaiden",

                    "5 %0.70 +1 $ ssp1 Ancestral Follower Shaman",

                    "10 $ @@@@@@ Sanguine Noble",

                    "0.22 >5 ###9 {1.2 }0.85 Wandering Noble",
                    "2.5 #1 Commoner",

                    "3 %0.5 -1 Putrid Corpse",
                    "4 %0.75 -0.35 sp1 Revenant Follower", //slightly better drops than albinaurics because they dont drop much thats useful.

                    "3 %0.75 sp1 sp1 Clayman",
                    "6 -1 ###9 Kindred of Rot",
                    "100 & ! -0.5 Demi-Human Queen",
                    "5 -0.5 %0.7 sp1 Basilisk",
                    "2 -1.5 %0.4 sp1 Small Fingercreeper",
                    "100 >> x5 +1 & ###10 $$$$5 Giant Fingercreeper",
                    "5 > %0.85 Fingercreeper",

                    "15 >> $$$$ %0.9 xv0.5 sp1 Grafted Scion",
                    "35 x2 $$ &1 Death Bird",
                    "100 > x2xx & -1 $$$0.5 ! Death Rite Bird",               //set up just in case there is a respawner
                    "63 >" + x3Chance + "-2 $$$0.5 & ! Black Knife Assassin",   //set up just in case there is a respawner
                    "100 #10 $$$$ ! Black Knife Ringleader",    //Alecto
                    "30 >> & $$$$ ###9 @@@@ x1x1x2xx Zamor",
                    
                    

                    "50 $$$$ +0.5 x4xxxx Misbegotten Crusader",
                    "2 -0.5 }0.9 >>>> x2x3xxx Perfumer",

                    "100 & #3 Carian Knight Bols",
                    "15 %0.85 -1.35 > sp1 Snowfield Troll",
                    "12 %0.85"+x3Chance+"-1.35 > $$$$ sp1 @@@@@0.33 Frenzied Troll",
                    
                    "5 -2.2 sp1 Skeletal Grave Warden",

                    "15  %0.85 }0.85 ###13 & Giant Wormface",
                    "7  %0.85 }0.85 ###13 & Wormface",

                };
                bothKeywords = new string[]{

                    "15 -1 >>> x2x3x4xx sss-2.5 @@@ $$$$$$$2.5 /25 Crystalian", //force somber ftd

                    "8 >>" +x2ChanceLevelSpread+" sss-1.8 -0.1 /25 Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.35 -0.1 $ /25 Karolos Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.35 -0.1 $ /25 Lazuli Glintstone Sorcerer",
                    "22" +x3Chance+" sss-1.35 -0.1 $ /25 Twinsage Glintstone Sorcerer",
     
                    //"0 x4 $$$$$$ #8 & Draconic Tree Sentinel",

                    "70 $$$$$$$ @@@ & x3xxx xvv0.5  /15 Troll Knight", // these are the raya lucaria ones.
                    "30 $ +0.75 {1.25 > x1x3xxx xvv0.65 /18 sss-2.2 sp1 Troll",

                    "15 %0.70 +1 $$$$$$$-0.5 sss-0.5 @@@0.25 /15 ssp1"+x2Chance+" Ancestral Follower (Siofra River)",   //ghost ones.

                    "14 > sss-0.22 {0.72 /15 %0.9" + x2ChanceLevelSpread + "Mausoleum Large Exile Soldier",
                    "9 > sss-0.22 {0.72 /15 %0.9" + x2ChanceLevelSpread + "Mausoleum Exile Soldier",

                    "45 sss-0.5" + knight + x2Chance + "/15 Mausoleum Knight",
                    "2.75 sss-0.5" + footSoldier + x2Chance + "/15 Mausoleum Foot Soldier",
                    "5.5 sss-0.5" + soldier + x2ChanceLevelSpread + "/15 Mausoleum Soldier",
                    "60 sss-0.5" + banishedK + x3Chance + "/15 Mausoleum Banished Knight",

                    "65 /8 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen",
                    "65 /8 x4xx xv0.65 xvv0.5 Fell Twin",
                    "55 > /15 +0.5 & ###8 $$$$$ ! " + "x2x3xx xv0.55 xvv0.55" + " Crucible Knight", //the percent chance increase has to acount for casscade.
                    "55 > /15 +0.5 & ###8 $$$$$ ! " + "x2x3xx xv0.55 xvv0.55" + " Tanith's Knight", //the percent chance increase has to acount for casscade.


                    "30 -1 > /4 $$$$$$$2 @@@@@@@0.30 sss-2.8 ###8 Giant Beast Skeleton",
                    "45 -1 /4 $$$$$$$2 @@@@@@@0.45 sss-2.8 ###8 Giant Skeleton",
                    "4.5 -1 /8 $$$$$$$0.6 @@@@@@@@0.045 sss-1.2 ###8 Skeleton",
                    "4.6 -1 > /6 $$$$$$$0.6 @@@@@@@@0.046 sss-1.2 ###8 Beast Skeletal",
                    "6 -1 > /6 $$$$$$$0.6 @@@@@@@@0.06 sss-1.2 ###8 Beast Skeletal Knight",
                    "9 -1 /6 $$$$$$$0.6 @@@@@@@@0.09 sss-1.2 ###8 Skeletal Soldier",
                    "4.5 -1 /8 $$$$$$$0.6 @@@@@@@@0.045 sss-1.2 ###8 Skeletal",

                    "40 sss-0.7 ssp0.5 /8 x3xx $$$$$$$ @@@ ###8 Battlemage", //forced somber

                    "25 sss-1.2 /15  x4xxx %0.85 $$ @@@ xv0.6 xvv0.75 Guardian Golem",    //none archer

                    "10 $$$$$$$$ @@@@@@0.3"+x2ChanceLevelSpread+"sss-0.5 /10 Guardian",

                    "65 sss-0.3 /8" + knight + x3Chance + "Cleanrot Knight",
                    "10 >> sss-1.5 /5 Mad Pumpkin Head",
                    "3 %0.5 sss-1.3 /20 Highwayman",
                    "4.5 {0.85 >>> x1x1x2x3xx /8 Glintstone Digger",
                    
                    "10 >2 x2 /8 Omenkiller",

                    "15 -1> x3x4xxx /8 $$ Depraved Perfumer",
                    
                    "100 +1 sss-1 /20 $$$ Erdtree Burial Watchdog",

                    "20 > $$$$$$$$-0.7 /20"+x3ChanceLevelSpread+" High Page",

                };

                runeKeywords = new string[]{
                    "100 ###13 Elite Runebear",
                    "100 %0.5 +3 ###13 {1.15  sp1 & $2 Runebear",//}0.85
                    "60" + GiantCrab + "Giant Death Crab",
                    "10 %0.5 #1 }0.85 sp1 Death Crab",
                    "60" + GiantCrab + "Giant Black Crab",
                    "10 %0.5 #1 }0.85 sp1 Black Crab",
                    "60" + GiantCrab + "Giant Crab",
                    "10 %0.5 #1 }0.85 sp1 Crab",

                    //"20 %0.5 #5 }0.85 +1.5 sp1 Warhawk",

                    "15 > %0.65 ###6 #####13 }0.85 +0.6 & sp1 & $$$$5 @@@0.2 $0.6 White Wolf",
                    "5 > %0.65 ###4 }0.85 -1 & sp1 Wolf",
                    "8.5 %0.5 >> }0.825 ###13 +1.85 & sp1 Large Azula Stray",
                    "8.5 %0.5 >> }0.825 ###13 +1.85 & sp1 Large Stray",
                    "3.5 %0.5 >> }0.825 ###5 -0.65 & sp1 Stray",

                    "12 >> $1 %0.35 +2.5 }0.85 {1.2 ###8 sp1 & $$ Giant Rat",
                    "5 >> %0.35 }0.85 ###3 sp1 & Rat",


                    "45 +0.7 %0.85 ####9 Operatic Bat",   //uses flags that we want to keep.
                    "12 >>> %0.85 +0.7 |0 }0.875 {1.125 ###5 sp1 Man-Bat",
                    "9 %0.5 >> ###6 }0.9 sp0.5 Dominula Celebrant", //already drops runes
                    "10 $$$$6.5 @@@@0.08 %0.5 > ###1 #####13 +0.5 }0.85 sp1 & Land Squirt",
                    "65 x4xx $$$$$$$$$8 @@@@@0.25  %0.5 > ###1 #####13 +2 }0.85 sp1 & Giant Land Squirt",
                    "65 x6xxx $$$$$$$$$10 @@@@@0.25  %0.5 > ###1 #####13 +2 }0.85 sp1 & Giant Rotten Land Squirt",
                    


                    "30 %0.5 > }0.85 +0.75 $$$$4.2 @@@@@0.30 sp1 & Giant Land Octopus",
                    "8 #1 }0.85 sp1 & Land Octopus",

                    //"10 %0.5 ###5 }0.85 sp1 & Guilty",
                    //"8 %0.75 $3 sp1 ###10 Fire Monk", //lands between runes.
                    //"12 %0.75 $3 sp1 ###10 Blackflame Monk",

                    "23 > $$$$4.3 @@@0.2 %0.5 +1 }0.87 #13 sp1 & Miranda Blossom",
                    "8 #2 %0.25 }0.55 sp1 &  Miranda Sprout",

                    "15 >> $$$$6 @@@0.08 %0.5 +1.65 }0.92 ###13 sp1 & Giant Dog",
                    "20 > $$$$6 @@@0.15 %0.5 +1.65 }0.92 ###13 sp1 & Giant Crow",

                    "12 > $$$$ ####14 #####14 @@@0.03 }0.9 sp1 %0.65 +1 & Giant Ant", //drops numen runes

                    "5 $$$$12 ####14 #####18 @@@0.2 %0.65 +1.2 }1.1 sp1 & Giant Crayfish",
                    //"30  %0.5 +2 }0.85 sp1 Watcher Stones",
                    "100 ###1 ####14 #####18 x6xxxxx $$$$$$$$$14 @@@@@0.1 %0.85 sp1 Abductor Virgin",
                    //"30  %0.65 +2 }0.85 sp1 & Snowfield Troll",
                };

                string[] uncertain = {};    //unused

                exceptions = new string[]{
                    "Elder Dragon Greyoll",
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
                    foreach (int npcId in ((Lines)NpcParam.GetLinesOnCondition(new Condition.HasInName("Guardian Golem").AND(new Condition.FloatFieldCompare(NpcParam.GetFieldIndex("enableSoundObjDist"), Condition.EQUAL_TO, 500)))).GetIDs()) {
                        keywordOverrideIDsDict.Add(npcId, "SSSSS 18 /15 sss-1.2 x2xx %0.85 $ xv0.6 xvv0.75 Guardian Golem (Archer Override)");    //tweek to make archers kill.
                    }
                    
                    keywordOverrideIDsDict.Add(43530020, "SS 60 #3 " + knight + " Leyndell Knight (Lurinia Override)");
                    string altusLleyndelKnight = "SS 60 #4.5 " + knight + " Leyndell Knight (Altus Override)";
                    string altusLleyndelSoldier = "SS 4.5 #4.5 " + soldier + " Leyndell Soldier (Altus Override)";
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

                    //keywordOverrideIDsDict.Add(33600020, "SS 15 #3 Ancestral Follower");
                    //.Add(33600520, "SS 15 #3 Ancestral Follower");

                    keywordOverrideIDsDict.Add(43400940, "SS 10 $ >> #3.3 sss-1.5 Mad Pumpkin Head (Flail duo boss)"); //special case where its both a duo boss and drops nothing. if we want to auto mate this we will need to add an "is duo" variable. Thats not tied to IsItemLotMap List. For now we will simply create overrides that add one time drop.
                    keywordOverrideIDsDict.Add(43401940, "SS 10 $ >> #3.3 sss-1.5 Mad Pumpkin Head (Hammer duo boss)");

                    keywordOverrideIDsDict.Add(21400930, "SSSSS 65 #4.5 /12 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen (Undocumented Omen Override)");  //undocumented omen in altus - lleyndel outskirts
                    keywordOverrideIDsDict.Add(21401930, "SSSSS 65 #4.5 /12 x4xxxx xv0.65 xvv0.5 sss-0.5 $$ Omen (Undocumented Override)");  //undocumented omen in altus - lleyndel outskirts

                    keywordOverrideIDsDict.Add(46003140, "SSS 8 %0.85 -1.35 > sp1 $$$$1 Troll (Pot Thrower Override)");//Pot Thrower? Troll override
                    keywordOverrideIDsDict.Add(46000040, "SSS 8 %0.85 -1.35 > sp1 $$$$1 Troll (Pot Thrower Override)");//Pot Thrower? Troll override
                    keywordOverrideIDsDict.Add(46001020, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46001030, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46001010, "SSS 6 #1 x3xx Troll (Carriage Override)");//Carriage Troll override
                    keywordOverrideIDsDict.Add(46000065, "");//Mimic Troll override

                    keywordOverrideIDsDict.Add(43520020, "SS 45 #3.5 " + knight + " Cuckoo Knight (Four Belfries and Bellum Override)");
                    keywordOverrideIDsDict.Add(43550020, "SSSSS 25 /20 #3 " + knight + " Mausoleum Knight (BK Catacombs Override)"); //too farmabale
                    keywordOverrideIDsDict.Add(45100572, "SSSSS 100 > +10 xxx5 & $$$$ Ancient Dragon (Droppers Override)");
                    keywordOverrideIDsDict.Add(46500265, "SS 100 x5 #3 Dragonkin Soldier (Nokrom Override)");
                    keywordOverrideIDsDict.Add(45102030, "SS 100 & +22 xxx5x $$$$$$$$ Ancient Dragon Lansseax (Ancient Dragon Exception");

                    keywordOverrideIDsDict.Add(30100172, "SSSSS 45 " + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    keywordOverrideIDsDict.Add(30101172, "SSSSS 45 " + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    keywordOverrideIDsDict.Add(30102172, "SSSSS 45 " + banishedK + x3Chance + " /12  Banished Knight (Farum Azula Dragon Communion Override)");
                    
                    keywordOverrideIDsDict.Add(34510912, "SS 30 -0.33 x2 $$ Scaly Misbegotten (Morne Tunnel Boss Override)");

                    keywordOverrideIDsDict.Add(31810022, "SSS 100 #6.9 ! Red Wolf of Radagon (Moonlight Altar Override)");
                    keywordOverrideIDsDict.Add(45021922, "SS 100 & x4xx $$$$ #8 sss-0.8 Glintstone Dragon Adula (Moonlight Altar Override)");
                    keywordOverrideIDsDict.Add(45020022, "SS 100 & x4xx $$$$ #7 sss-0.7 Glintstone Dragon (Moonlight Plateau Override)");
                    
                    //keywordOverrideIDsDict.Add(35700028, "SSS 100 #8 Godskin Noble (Liurnia Divine Tower Override)");

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
            Dictionary<string, float> ftdx1ChanceMultDict = new Dictionary<string, float>();
            Dictionary<string, bool> ftdx1ExcessToEmptyDict = new Dictionary<string, bool>();
            Dictionary<string, int> ftdx1TypeDict = new Dictionary<string, int>();

            Dictionary<string, int[]> amountMultsDict = new Dictionary<string, int[]>();
            Dictionary<string, int> XasscadeDict = new Dictionary<string, int>();
            Dictionary<string, float> XasscadeMultToXDict = new Dictionary<string, float>();
            Dictionary<string, float> XasscadePowToXDict = new Dictionary<string, float>();

            Dictionary<string, float> spLevelSplitDict = new Dictionary<string, float>();
            Dictionary<string, float> stoneSpLevelSplitDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMultDict = new Dictionary<string, float>();
            Dictionary<string, float> levelAdjDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMaxUniqueDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMinUniqueDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMaxDict = new Dictionary<string, float>();
            Dictionary<string, float> levelMinDict = new Dictionary<string, float>();
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

            Dictionary<string, string> descriptionDict = new Dictionary<string, string>();
            const bool PrintOutDescriptions = false;
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
                    float ftdx1ChanceMult = 1;
                    bool ftdx1ExcessToEmpty = false;
                    int ftdx1Type = 0;

                    float somberLevelAdj = 0;

                    float levelMult = 1;
                    float levelAdj = 0;
                    //num after + or - based on the adj.
                    //"30 +1 knight" = +1 
                    //"30 -1 knight" = -1 
                    float spLevelSplit = 0;
                    float stoneSpLevelSplit = 0;
                    float setLevel = -1;    //#num
                    float levelMaxUnique = -1;    //#####num
                    float levelMinUnique = -1;    //####num
                    float levelMax = -1;    //###num
                    float levelMin = -1;    //##num
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

                    string description = "";

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
                            string pattern = @" sp(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                spLevelSplit = float.Parse(match.Groups[1].Value);
                        }
                        {
                            //get stoneSpLevelSplit
                            string pattern = @" ssp(\d+(\.\d+)?)";
                            Match match = Regex.Match(keyword, pattern);
                            if (match.Success)
                                stoneSpLevelSplit = float.Parse(match.Groups[1].Value);
                        }

                        {
                            
                            {
                                {
                                    string pattern = @" #(\d+(\.\d+)?)";
                                    Match match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        setLevel = float.Parse(match.Groups[1].Value);
                                    }
                                }
                                {
                                    string pattern = @" ###(\d+(\.\d+)?)";
                                    Match match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        levelMax = float.Parse(match.Groups[1].Value);
                                    }
                                }
                                {
                                    string pattern = @" ##(\d+(\.\d+)?)";
                                    Match match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        levelMin = float.Parse(match.Groups[1].Value);
                                    }
                                }
                                {
                                    string pattern = @" #####(\d+(\.\d+)?)";
                                    Match match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        levelMaxUnique = float.Parse(match.Groups[1].Value);
                                    }
                                }
                                {
                                    string pattern = @" ####(\d+(\.\d+)?)";
                                    Match match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        levelMinUnique = float.Parse(match.Groups[1].Value);
                                    }
                                }
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

                                xFound = 0;
                                xx = "@";
                                while (keyword.IndexOf(xx) != -1)
                                {
                                    xFound++;
                                    xx += "@";
                                }
                                if (xFound != 0)
                                {
                                    ftdx1ExcessToEmpty = xFound >= 2;
                                    ftdx1ChanceMult = 1;
                                    ftdx1Type = xFound;
                                    //fistTimeDropAdj
                                    pattern = @"\@(\d+(\.\d+)?)";
                                    match = Regex.Match(keyword, pattern);
                                    if (match.Success)
                                    {
                                        ftdx1ChanceMult = float.Parse(match.Groups[1].Value);
                                    }
                                    else
                                    {
                                        pattern = @"\@\+(\d+(\.\d+)?)";
                                        match = Regex.Match(keyword, pattern);
                                        if (match.Success)
                                        {
                                            ftdx1ChanceMult = float.Parse(match.Groups[1].Value);
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
                    if (PrintOutDescriptions)
                    {
                        description += percentNum * (LevelCasscade + 1) + "% chance to drop ";
                        string noun = "Stone";

                        if (isSmithing)
                            description += "Smithing Stones ";
                        if (isSmithing && isSomber)
                            description += "& ";
                        if (isSomber)
                            description += "Somber Smithing Stones ";
                        if ((isSmithing || isSomber) && isRune)
                            description += "& ";
                        if (isRune)
                        {
                            description += "Runes ";
                            noun = "Rune";
                        }


                        float hL = levelAdj;
                        float lL = levelAdj - LevelCasscade;

                        {
                            string qualifier = "";
                            if (Math.Abs(somberLevelAdj) < 1)
                                qualifier = "can ";
                            if (levelAdj > 0)
                            {
                                description += "\n   level adjustment compared to area level:" + levelAdj + " (" + qualifier + "can drop higher level " + noun + "s)";
                            }
                            else if (levelAdj < 0)
                            {
                                description += "\n   level adjustment compared to area level:" + levelAdj + " (" + qualifier + "drops lower level " + noun + "s)";
                            }
                        }
                        if (isSomber)
                        {
                            string qualifier = "";
                            if (Math.Abs(somberLevelAdj) < 1)
                                qualifier = "can ";
                            if (somberLevelAdj + levelAdj > 0)
                            {
                                description += "\n   level adjustment compared to area level specifically for Somber Stones:" + (somberLevelAdj + levelAdj) + " (" + qualifier + "drop higher level " + noun + "s)";
                            }
                            else if (somberLevelAdj + levelAdj < 0)
                            {
                                description += "\n   level adjustment compared to area level specifically for Somber Stones:" + (somberLevelAdj + levelAdj) + " (" + qualifier + "drops lower level " + noun + "s)";
                            }
                        }

                        if (LevelCasscade == 1)
                            description += "\n   inludes " + noun + "s " + 1 + " level lower than adjusted level";
                        else if (LevelCasscade > 1)
                            description += "\n   inludes " + noun + "s up to " + LevelCasscade + " levels lower than adjusted level";

                        if (firstTimeDropSeverity != -1)
                        {
                            description += "\n   has a guarenteed drop on the first kill (varient based)";
                        }

                        descriptionDict.Add(keyword, description);
                    }

                    isSmithingDict.Add(keyword, isSmithing);
                    isSomberDict.Add(keyword, isSomber);
                    isRuneDict.Add(keyword, isRune);
                    isForceBossDisplayDict.Add(keyword, isForceBossDisplay);
                    firstTimeDropDict.Add(keyword, firstTimeDropSeverity);
                    firstTimeDropAdjDict.Add(keyword, firstTimeDropAdj);
                    ftdx1ChanceMultDict.Add(keyword, ftdx1ChanceMult);
                    ftdx1ExcessToEmptyDict.Add(keyword, ftdx1ExcessToEmpty);
                    ftdx1TypeDict.Add(keyword, ftdx1Type);

                    amountMultsDict.Add(keyword, amountMults.ToArray());
                    XasscadeDict.Add(keyword, xasscade);
                    XasscadeMultToXDict.Add(keyword, xasscadeMultToX);
                    XasscadePowToXDict.Add(keyword, xasscadePowToX);
                    bothPercentSplitForSomberDict.Add(keyword, bothPercentSplitForSomber);

                    spLevelSplitDict.Add(keyword, spLevelSplit);
                    stoneSpLevelSplitDict.Add(keyword, stoneSpLevelSplit);
                    levelMultDict.Add(keyword, levelMult);
                    levelAdjDict.Add(keyword, levelAdj);
                    setLevelDict.Add(keyword, setLevel);
                    levelMaxUniqueDict.Add(keyword, levelMaxUnique);
                    levelMinUniqueDict.Add(keyword, levelMinUnique);
                    levelMaxDict.Add(keyword, levelMax);
                    levelMinDict.Add(keyword, levelMin);
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
            
            if (PrintOutDescriptions) {
                foreach(string key in descriptionDict.Keys)
                {
                    Util.println("keyword: " + simplifiedKeywordNameDict[key]);
                    Util.println(descriptionDict[key]);
                    Util.println();
                }
                Util.p();
            }

            //check names in dictionary.
            //check name kewords to see if its smithing, somber, or both.
            //parse percent/itemlot data from keyword.

            Line baseEmptyItemLotParamLine = ItemLotParam_enemy.vanillaParamFile.GetLineWithId(460000500).Copy(ItemLotParam_enemy)
                .SetField(1,"");

            Lines itemLotMainLines = new Lines(ItemLotParam_enemy);
            List<int> itemLotMainLinesIDs = new List<int>();

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

            int testId = -1;

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

            Dictionary<string, int> TreasureDrodIdToFlagId = new Dictionary<string, int>();
            
            List<int> usedGetItemFlagId = FlagIds.usedGetItemFlagId;

            //Util.println("" + 7.5f);


            Dictionary<Line, string> idToVariantsDict = new Dictionary<Line, string>();
            Dictionary<Line, float> idToLevelDict = new Dictionary<Line, float>();

            foreach (int npcID in npcIDs)
            {
                testId = -1;//43401940;
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
                //if(keyword.Contains("Giant Dog"))
                //SET TEST
                //if (npcID == 21000052)
                //if( 
                //    npcLine.name.Contains("Draconic Tree"))
                //&& (keywordOverrideIDsDict.ContainsKey(npcID))
                //  npcLine.name.Contains("Godrick Soldier") )
                //if     (npcLine.name.Contains("igger"))

                //testId = npcID;

                bool test = testId == npcID;

                bool isBoss = BossOrMiniBossIds.Contains(npcID);
                bool isMultiBoss = NpcData.MultiBoss.Contains(npcID);



                if (test)
                    Util.println(npcLine._idName + " documented " + documented + "  isBoss:" + isBoss);


                float level = -1;

                Line mostMatchedLine = null;

                const bool debugAssignedLevels = false;

                int mySpLevelNum = -1;
                float spLevel = -1;
                float stoneSpLevel = -1;
                {

                    if (npcsIdToSpLevelsDict.ContainsKey(npcID))
                    {
                        mySpLevelNum = npcsIdToSpLevelsDict[npcID];
                        if (spLevelToDifficultyDict.ContainsKey(mySpLevelNum))
                            spLevel = spLevelToDifficultyDict[mySpLevelNum];
                        if (spLevelToStoneDifficultyDict.ContainsKey(mySpLevelNum))
                            stoneSpLevel = spLevelToDifficultyDict[mySpLevelNum];
                    }
                }


                //aqusitionIDs limitations. GOTO
                //
                //-------------------
                //      FIND LEVEL
                //--------------------
                level = setLevelDict[keyword];
                if (level == -1)
                {
                    if (documented)
                    {

                        level = npcsDocDifficultyDict[npcLine.id_int];
                        if (test || debugAssignedLevels)
                            Util.println(npcLine._idName + " GOT DOCUMENTED LEVEL: " + level);
                    }
                    else
                    {
                        if (stoneSpLevel == -1)
                            continue;

                        //var idName = npcLine._idName;
                        //Console.WriteLine(idName);
                        level = stoneSpLevel;


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
                            var orderedNestedLines = LineFunctions.GetOrderedWordMatchedNestedLines(targetWords, documentedLines, out maxMatchCount, out int maxScore, importantWords.Length, importantWords, true, true);



                            if (test)
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
                                    //if (test)
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
                                            if (testingEstimate || test)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (level < 0)
                                        {
                                            if (testingEstimate || test)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (level > 10)
                                        {
                                            if (testingEstimate || test)
                                                Util.println(npcLine._idName + " FAILED |  myRunes:" + mySpLevelNum + "    minRune: " + minDocumentedLevel + "   maxRune: " + maxDocumentedLevel + "  minLvl: " + minLvl + "   maxLvl: " + maxLvl + "   level: " + level + "            minLine:" + minLine._idName + "     maxLine:" + maxLine._idName);
                                            continue;
                                        }
                                        if (testingEstimate || test)
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
                                            if (testingEstimate || test)
                                                Util.println(npcLine._idName + " FAILED to estimate");
                                            continue;
                                        }
                                        if (testingEstimate || test)
                                            Util.println(npcLine._idName + " CHOSE IMPERFECTLY myRunes:" + mySpLevelNum + "    minRune:" + minDocumentedLevel + "   minLvl: " + npcsDocDifficultyDict[minLine.id_int] + "   level:" + level);

                                    }
                                }
                                else if (test || debugAssignedLevels)
                                    Util.println(npcLine._idName + " SP ASSIGNED LEVEL: " + level + "  mySpLevel:" + mySpLevelNum);
                            }
                            else if (test || debugAssignedLevels)
                                Util.println(npcLine._idName + " FOUND MATCHED LEVEL: " + level + "   " + perfectLine._idName + " shares mySpLevel:" + mySpLevelNum);
                        }
                        else if (test || debugAssignedLevels)
                            Util.println(npcLine._idName + " FOUND SP LEVEL: " + level);
                        //check it has !

                        //check hp to be equal.
                        //  if take item lot param and continue.
                        //if we reach the end of best lines. AND we have 2 or more lines best lines with diffrent health. We approximate our level.

                        //hpPerLvl = ((maxHP - minHP) / (maxLvl - minLvl))
                        //myHP / hpPerLvl

                        //if (test)
                        //   Util.println("Estimated level " + level);

                    }

                    float spLevelSplit = spLevelSplitDict[keyword];
                    float preSplitLevel = level;
                    if (spLevelSplit != 0 && spLevel != -1 && level != spLevel)
                        level = (spLevelSplit * spLevel) + (level * (1 - spLevelSplit));
                    if (test)//|| (spLevelSplit != 0 && preSplitLevel != level))
                        Util.println(npcLine._idName + " documented " + documented + " preSplitlevel " + preSplitLevel + "  spLevelSplit " + spLevelSplit + "  level " + level);

                    spLevelSplit = stoneSpLevelSplitDict[keyword];
                    preSplitLevel = level;
                    if (spLevelSplit != 0 && stoneSpLevel != -1 && level != spLevel)
                        level = (spLevelSplit * stoneSpLevel) + (level * (1 - spLevelSplit));
                    if (test)//|| (spLevelSplit != 0 && preSplitLevel != level))
                        Util.println(npcLine._idName + " documented " + documented + " preStoneSplitlevel " + preSplitLevel + "  spStoneLevelSplit " + spLevelSplit + "  level " + level);
                    //if (npcID == 30100172)
                    //    Util.p();

                }
                else if (test || debugAssignedLevels)
                    Util.println(npcLine._idName + " KEY WORD ASSIGNED LEVEL: " + level);
                //----------------------------------

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

                    /*if (itemLotLine != null && (STONES || RUNES))
                    {
                        var kwIndex = itemLotLine.hasKeywordContaining("!ADD BUFFER LINE!");
                        if (kwIndex != -1)
                        {
                            bool wasParsed = int.TryParse(itemLotLine.keywords[kwIndex].keyword.Replace("!ADD BUFFER LINE!", ""), out int bufferAmount);
                            if (!wasParsed)
                                bufferAmount = 1;
                            for (int i = 0; i < bufferAmount; i++)
                            {
                                int newLineId = itemLotLine.GetNextFreeId();
                                Line bufferLine = baseEmptyItemLotParamLine.Copy().SetField(0, newLineId ).SetField(1, "COMPATIBILITY BUFFER");
                                ItemLotParam.OverrideOrAddLine(bufferLine);
                            }
                            itemLotLine.keywords.RemoveAt(kwIndex);
                        }
                    }*/


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
                            if(isBoss) 
                                itemLotEnemyName += "[Boss]";
                        }
                    }

                    bool UniqueVariant = false; // so it doesnt look for variants.


                    string backUpVariantId = "";


                    bool isForceBossDisplay = isForceBossDisplayDict[keyword];
                    int firstTimeDropStyle = firstTimeDropDict[keyword];
                    bool isFirstTimeDrop = firstTimeDropStyle != -1;
                    float ftdx1ChanceMult = ftdx1ChanceMultDict[keyword];
                    bool ftdx1ExcessToEmpty = ftdx1ExcessToEmptyDict[keyword];
                    bool isTreasure = (ftdx1ChanceMult < 1 && ftdx1ExcessToEmpty);
                    int ftdx1Type = ftdx1TypeDict[keyword];
                    bool FTD_LevelsShareId = ftdx1Type == 3 || ftdx1Type == 5;
                    bool FTD_KeywordShardsId = ftdx1Type == 4 || ftdx1Type == 6;
                    bool Treasure_BlockNormalDropsUntilFound = ftdx1Type == 5 || ftdx1Type == 6;
                    bool Treasure_Treasure_BlockNormalDropsUntilFound_TypeSpecific = ftdx1Type == 7 || ftdx1Type == 8;


                    if (test)
                        Util.p();


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
                            if (isTreasure)
                            {
                                variantID += "tre";
                                
                            }
                            else
                            {
                                variantID += "ftd";
                            }

                            if (FTD_LevelsShareId)
                            {
                                variantID += "_flagLvl" + level;
                            }
                            else if (FTD_KeywordShardsId)
                            {
                                variantID += "_flagKw" + keywordIndex;
                            }
                            else //doesnt share id
                            {
                                variantID += specialDropUniqueIndex;
                                specialDropUniqueIndex++;
                                UniqueVariant = true;
                            }
                        }
                        

                        //variantString = "(VariantID: " + variantID + ")";
                    }
                    

                    bool FTD_OnlyHighestX = firstTimeDropStyle == 3 || firstTimeDropStyle == 4 || firstTimeDropStyle == 5 || firstTimeDropStyle == 6 || firstTimeDropStyle == 8;
                    bool FTD_OnlyFirstType = firstTimeDropStyle == 5 || firstTimeDropStyle == 8;
                    bool FTD_OnlySomberType = firstTimeDropStyle == 7;
                    bool FTD_DropMultiple = firstTimeDropStyle == 5 || firstTimeDropStyle == 6; //$$$$
                    bool FTD_OnlyHighestLevel = firstTimeDropStyle == 4 || firstTimeDropStyle == 7 || firstTimeDropStyle == 8 || firstTimeDropStyle == 9;
                    bool FTD_Dropx1 = firstTimeDropStyle == 9;
                    bool FTD_AvoidDropx1 = firstTimeDropStyle > 1;   //because the other system didnt work so fuck it.
                    bool FTD_SeperateLineForEachType = false;


                    //if (test)
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

                    //if (!OneTimeWeaponAndArmorDrops)
                    /*{
                        if (!RUNES && !STONES)
                            continue;
                        bool dropStones = (dropSmithing || dropSomber);
                        if (!STONES && dropStones && !dropRune)
                            continue;
                        if (!RUNES && dropRune && !dropStones)
                            continue;
                    }*/



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

                   

                    var isSmithingLineCondition = new Condition.HasInName(smithingStoneItemsToRemove);
                    var isRuneLineCondition = new Condition.HasInName(runeItemsToRemove);
                    /*var curLineEmptyAndNoDropCondition =
                        new Condition.Either(
                            new Condition.HasInName("None"),
                            new Condition.AllOf(
                                new Condition.NameIs(""),
                                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId01"), "0"),
                                new Condition.FieldIs(ItemLotParam_enemy.GetFieldIndex("lotItemId02"), "0"))
                        );*/
                    

                    int HighestIdPossible = int.MaxValue;
                    int found_getItemFlagId = -1;
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
                                //(curLineEmptyAndNoDropCondition.Pass(curLine) ||
                                ((dropSmithing || dropSomber) && isSmithingLineCondition.Pass(curLine)) ||
                                (dropRune && isRuneLineCondition.Pass(curLine))//)
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
                        itemLotMainLinesIDs.Add(itemLotID);
                    }
                    else if (CanHijack && itemLotLine != null && !itemLotLine.name.Contains("(Level: ") //unused
                        && NpcParam.GetLineOnCondition(
                                new Condition.FieldIs(NpcParam.GetFieldIndex("itemLotId_enemy"), itemLotLine.ToString()).
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
                                //curLineEmptyAndNoDropCondition.Pass(curLine) ||
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
                        itemLotMainLinesIDs.Add(itemLotID);
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
                                if (test)
                                    Util.println(npcLine._idName + "   SUCCESS" + "    already had proper" + itemLotLine.id);
                                continue;
                            }
                            else
                            {
                                //properItemLotLineFound = itemLotMainLines.GetLineOnCondition(isProperItemLotCondition);
                                foreach (var ilml in itemLotMainLines.lines)
                                {
                                    if (idToLevelDict.ContainsKey(ilml) && idToVariantsDict.ContainsKey(ilml) && idToLevelDict[ilml] == level && idToVariantsDict[ilml] == variantID)
                                    {
                                        properItemLotLineFound = ilml;
                                        break;
                                    }
                                }
                            }

                            //if we find one assign it.

                            //if (test)
                            //    Util.println("properItemLotLine Found: " + (properItemLotLineFound != null).ToString() + "   size = "+ itemLotMainLines.Length);;

                            if (properItemLotLineFound != null)
                            {
                                npcLine.SetField("itemLotId_enemy", properItemLotLineFound.id);
                                if (test)
                                    Util.println(npcLine._idName + "   SUCCESS" + "    found proper" + properItemLotLineFound.id);
                                continue;
                            }
                        }

                        // proper item lot not found.  so instead we will create one.


                        string matchDebug = "";
                        const bool DEBUG_MATCH_NAME = false;
                        //find an ID to use as the start to search for a location to create it.
                        int searchInterval = 100;
                        int startingSearchForLocationItemLotID = -1;
                        if (itemLotLine == null)
                        {
                            searchInterval = 10000;
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
                                var mostMatchedLines = LineFunctions.GetOrderedWordMatchedNestedLines(targetWords, ItemLotParam_enemy.lines, out maxMatchCount, out int maxScore, 1, importantWords, false,true, null, "[", "]");

                                if (DEBUG_MATCH_NAME)
                                {
                                    matchDebug = target + "     ";
                                    foreach (string tw in importantWords)
                                    {
                                        matchDebug += "tw:" + tw + " ";
                                    }
                                    foreach (string iw in importantWords)
                                    {
                                        matchDebug += "iw:" + iw + " ";
                                    }
                                    matchDebug += "     ";
                                }
                                foreach (List<Line> lines in mostMatchedLines)
                                {
                                    List<string> cont = new List<string>();
                                    foreach(Line l in lines)
                                    {
                                        var sub = l.name.Substring(l.name.IndexOf("[")+1, l.name.IndexOf("]"));
                                        if (!cont.Contains(sub))
                                        {
                                            cont.Add(sub);
                                            matchDebug += sub + ", ";
                                        }
                                    }
                                }
                                

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
                                    List<Line> orderedLines = LineFunctions.GetOrderedWordMatchedLines(targetWords, NpcParam.lines, out maxMatchCount, out maxScore, 1, importantWords, true,true);

                                    if (DEBUG_MATCH_NAME)
                                    {
                                        matchDebug = target + "     ";
                                        foreach (string tw in importantWords)
                                        {
                                            matchDebug += "tw:" + tw + " ";
                                        }
                                        foreach (string iw in importantWords)
                                        {
                                            matchDebug += "iw:" + iw + " ";
                                        }
                                        matchDebug += "     ";
                                    }
                                    foreach (List<Line> lines in mostMatchedLines)
                                    {
                                        List<string> cont = new List<string>();
                                        foreach (Line l in lines)
                                        {
                                            var sub = l.name.Substring(l.name.IndexOf("[") + 1, l.name.IndexOf("]"));
                                            if (!cont.Contains(sub))
                                            {
                                                cont.Add(sub);
                                                matchDebug += sub + ", ";
                                            }
                                        }
                                    }

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
                        //if (test)
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
                            while (itemLotMainLinesIDs.Contains(newItemLotID) || ItemLotParam_enemy.GetLineWithId(newItemLotID, out newLineIndex, newLineIndex) != null || newNextLineID < newItemLotID + searchInterval)
                            {
                                newItemLotID = ItemLotParam_enemy.GetNextFreeId(newItemLotID, out newLineIndex);
                                newItemLotID += searchInterval - (newItemLotID % searchInterval);
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
                            //if (currentLineToCopyID == 463000004)
                            //    Util.p();

                            Line curLine = ItemLotParam_enemy.GetLineWithId(currentLineToCopyID, out curLineIndex, curLineIndex);
                            var debugLineName = "";
                            if (curLine == null)
                            {
                                targetSmithingStoneLineID = currentNewLineCopyID;
                                break;
                            }
                            else
                                debugLineName = curLine._idName;

                            int curGetItemFlagId = curLine.GetFieldAsInt(LotItem.getItemFlagIdFI);
                            if (curGetItemFlagId > 0 && LotItem.GetEmptyChanceTotal(curLine) == 0)
                                found_getItemFlagId = curGetItemFlagId;

                            if (
                                //curLineEmptyAndNoDropCondition.Pass(curLine) ||
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
                                var lineToCopy = ItemLotParam_enemy.GetLineWithId(currentLineToCopyID);
                                Line copy = lineToCopy.Copy().SetField(0, currentNewLineCopyID);
                                newLines.Add(copy);

                                //if (currentNewLineCopyID == 375001301)
                                //    Util.p();

                                if (!lineToCopy.added)
                                {
                                    copy.addKW("!VanillaItemLotCopy!", 0);
                                }
                                else
                                {
                                    Util.p();
                                    //Util.println(lineToCopy._idName);
                                }
                                //ItemLotParam_enemy.OverrideOrAddLine(copy);
                            }
                            currentLineToCopyID++;
                            currentNewLineCopyID++;
                        }



                        Debug.Assert(newItemLotID != -1, npcLine._idName);

                        //if (test)
                        //    Util.println("newItemLotID: "+newItemLotID+"   "+npcLine.id+":"+npcLine.name+"   " +newItemLotID);

                        itemLotID = newItemLotID;
                        var debugfjddkd = npcID;
                        HighestIdPossible = ItemLotParam_enemy.GetNextLine(targetSmithingStoneLineID, false, curLineIndex).id_int - 2;
                        //if (npcID == 38500024)
                        //    Util.p();

                        itemLotLine = null;
                        if(newLines.Count > 0)
                            itemLotLine = newLines[0]; //ItemLotParam_enemy.GetLineWithId(itemLotID);

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
                        itemLotMainLinesIDs.Add(itemLotID);
                    }
                    
                   
                    {
                        if (!RUNES && !STONES)
                        {
                            foreach (Line line in newLines)
                            {
                                ItemLotParam.OverrideOrAddLine(line);
                            }
                            continue;
                        }
                        bool dropStones = (dropSmithing || dropSomber);
                        if (!STONES && dropStones && !dropRune)
                            continue;
                        if (!RUNES && dropRune && !dropStones)
                            continue;
                    }
                    if (!IsItemLotMapDrop && (isBoss || isForceBossDisplay))
                        npcLine.SetField("dropType", 1);
                    if(!IsItemLotMapDrop)
                        npcLine.SetField("itemLotId_enemy", itemLotID);
                    //create smithing stone line.

                    if (test)
                        Util.println(npcLine._idName + "   SUCCESS" + "    making proper" + itemLotID);

                    float percentChance = percentNumDict[keyword] * 10;   //100 chance to 1000 item lot chance

                    if (test)
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

                    



                    float levelToChanceMult = levelToChanceMultDict[keyword];
                    float casscadeLevelToChanceMult = casscadeLevelToChanceMultDict[keyword];

                    int levelCasscade = levelCasscadeDict[keyword];


                    float DROPMULTamountMult = DROPMULT; 
                    float DROPMULTpercentMult = DROPMULT;

                    {
                        float effectivePercent = percentChance * (levelCasscade + 1);
                        DROPMULTpercentMult = Math.Min(Math.Max(1f,1000 / effectivePercent), DROPMULTpercentMult);
                        float MaxPercent = (effectivePercent * 1.5f) + (effectivePercent * 0.25f * DROPMULT);
                        float MaxPercentMult = MaxPercent / effectivePercent;
                        DROPMULTpercentMult = Math.Min(MaxPercentMult, DROPMULTpercentMult);
                        DROPMULTamountMult = 1 + ((DROPMULT - 1) - (DROPMULTpercentMult - 1));
                        if (test)
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
                                if (test)
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


                        int currentRarity = -1;

                        if (currentlyFirstDropGuarentee || treatAsBoss)       // we made the firstTImeAdj also aply to bosses. Cuz why not. ALso helps us with making zamor better.
                        {
                            curAdjustedLevel = adjustedLevel + currentFirstTimeLevelAdj;

                            if (FTD_LevelsShareId)
                            {
                                const int TreasureLevelInterval = 1;
                                curAdjustedLevel = Util.RoundToNearestInterval(curAdjustedLevel, TreasureLevelInterval);
                            }

                            if (currentlyFirstDropGuarentee &&(((FTD_OnlyHighestX || FTD_DropMultiple) && xAmounts.Count() > 0 && xAmounts[xAmounts.Count()-1] > 1 ) || currentFirstTimeLevelAdj > 1))
                            {
                                currentRarity = 4;
                            }
                        }
                        if (treatAsBoss)
                        {
                            currentlyFirstDropGuarentee = false;
                            dmax = 0;
                            if (!IsItemLotMapDrop && isMultiBoss)   // to prevent farming ne of the bosses and reseting boss fight
                                curGiveUniqueItemFlagID = true;
                        }
                        else if (currentlyFirstDropGuarentee)
                            curGiveUniqueItemFlagID = true;

                        
                        //level max clamp
                        float levelMax = levelMaxDict[keyword];
                        float levelMin = levelMinDict[keyword];

                        if(treatAsBoss || currentlyFirstDropGuarentee)
                        {
                            float levelMaxU = levelMaxUniqueDict[keyword];
                            if (levelMaxU != -1)
                                levelMax = levelMaxU;
                            float levelMinU = levelMinUniqueDict[keyword];
                            if (levelMinU != -1)
                                levelMin = levelMinU;
                        }


                        if (levelMax != -1 && curAdjustedLevel > levelMax)
                            curAdjustedLevel = levelMax;
                        if (levelMin != -1 && curAdjustedLevel < levelMin)
                            curAdjustedLevel = levelMin;
                        if (curAdjustedLevel < 1)
                            curAdjustedLevel = 1; //makes foot soldiers and imps still useful in the beginning.
                        //if (npcID == 30100172)
                        //    Util.p();
                        //if (isBoss) //bosses cant be decimal levels.
                        //    curAdjustedLevel = (int)(curAdjustedLevel + 0.7f);
                        

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

                            if (test)
                                Util.p();

                            //if (createNewLineForSpecialDrop)
                            //{
                            if (treatAsBoss || !ftdx1ExcessToEmpty)
                            {
                                lotIndex = 1;
                                emptyLotReplaced = true;
                            }
                            if (test)
                                Util.p();

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



                        if (test)
                            Util.println("useSingleLine:" + useSingleLine + "   currentlyFirstDropGuarentee:" + currentlyFirstDropGuarentee + "   d:" + d + "   dmax:" + dmax);






                        //int debugNoFinalPercent = 0;
                        int typeIndex = 0;
                        if ((treatAsBoss || currentlyFirstDropGuarentee) && FTD_OnlySomberType)
                            typeIndex = 1;

                        int firstTypeIndex = -1;
                        if (dropSmithing)
                            firstTypeIndex = 0;
                        if (dropSomber)
                            firstTypeIndex = 1;
                        if (dropRune)
                            firstTypeIndex = 2;


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

                            float decimalMiss = 0;
                            if (isDecimalLevel)
                            {
                                if((int)curTypeAdjustedLevel+1 - curTypeAdjustedLevel > curTypeAdjustedLevel - (int)curTypeAdjustedLevel)
                                {
                                    decimalMiss = curTypeAdjustedLevel - (int)curTypeAdjustedLevel;
                                }
                                else
                                {
                                    decimalMiss = (int)curTypeAdjustedLevel + 1 - curTypeAdjustedLevel;
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

                            int typeLevelCount = dropTypesIDsDictionaries[typeIndex].Count;

                            if (isDecimalLevel)
                            {
                                startLevelPercentMult = Math.Abs(curTypeAdjustedLevel) % 1;    // level 1.96 -> 0.96
                                if (levelCasscade == 0)
                                {
                                    //useSingleLine = true;
                                    createNewLineforThisType = true; // has to create a singleLine Tis type.
                                    useSingleLineOnce = true;   //so you still can only get one level of drop per kill.
                                }
                                if (levelCasscade != -1 && !(typeIndex < 2 && startLevel >= typeLevelCount))
                                    levelCasscade++;
                                lastLevelPercentMult *= 1 - startLevelPercentMult;    //level 0.04
                            }

                            if (test)
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

                            if (test)
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


                            

                            if (test)
                                Util.p();

                            if (isBoth)   //if is both reduce chance for both
                            {
                                if (typeIndex == 0)
                                    curTypePercentChance = curPercentChance * ((100f - bothPercentSplitForSomberDict[keyword]) / 100f);
                                else if (typeIndex == 1)
                                    curTypePercentChance = curPercentChance * (bothPercentSplitForSomberDict[keyword] / 100f);
                            }

                            if (test)
                                Util.p();

                            float startLevelPercent = curTypePercentChance;
                            

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
                            if (test)
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
                                    if (test)
                                        Util.p();
                                }
                            }

                            startLevelPercent = startLevelPercent * startLevelPercentMult;
                            float lastLevelPercent = curTypePercentChance * lastLevelPercentMult;


                            int casscadeIndex = -1;


                            int lastCasscadeXAmount = -1;

                            if(test)
                                Util.p();

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


                                bool canUseLuck = !treatAsBoss && !(currentlyFirstDropGuarentee && !ftdx1ExcessToEmpty);
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
                                if (!(curLevel == minLevel && isDecimalLevel) && !((treatAsBoss || currentlyFirstDropGuarentee) && curLevel == minLevel)) //last dicimal level is excepmt from further manipulation.
                                {
                                    //curLevelPercentChance *= (float)Math.Pow(levelToChanceMult, curLevel - 1);    //-1 so it doesnt effect level 1
                                    if (test && levelToChanceMult != 1)
                                        Util.p();

                                    //curLevelPercentChance += curLevelPercentChance * (levelToChanceMult - 1) * (curLevel - 1);    //-1 so it doesnt effect level 1 //multiplicative.
                                    curLevelPercentChance = curLevelPercentChance * (float)Math.Pow(levelToChanceMult, (curLevel - 1));
                                    if (canDropAncient)
                                        curLevelPercentChance *= (float)Math.Pow(casscadeLevelToChanceMult, Math.Max(casscadeIndex - 1, 0));   //if it can drop an ancient it wont effect other drop rates.
                                    else
                                        curLevelPercentChance *= (float)Math.Pow(casscadeLevelToChanceMult, casscadeIndex);
                                }

                                const bool SomberNoXAmount = true;

                                if (SomberNoXAmount && isBoth && typeIndex == 1)
                                    canDropXAmount = false;

                                int curCasscadeXAmount = 1;
                                if (!FTD_Dropx1 || !(treatAsBoss || currentlyFirstDropGuarentee)) {
                                    //if (targetSmithingStoneLineID == 431200307)
                                    //    Util.p();
                                    int xAmountCasscadeIndex = casscadeIndex;
                                    if (canDropAncient)
                                        xAmountCasscadeIndex--;
                                    if (isDecimalLevel && decimalMiss > 0.25)
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
                                        
                                        if (test)
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
                                    if(stoneLine != null && createNewLineforThisType && useSingleLineOnce && useSingleLine)// for the niche senario where createNewLineforThisType leads to avoiding the ftd buffer from being added to stone line
                                    {
                                        if (d == 0 && dmax == 1 && (!isTreasure || Treasure_BlockNormalDropsUntilFound || (Treasure_Treasure_BlockNormalDropsUntilFound_TypeSpecific && ((FTD_OnlySomberType && typeIndex == 1) || (FTD_OnlyFirstType && typeIndex == firstTypeIndex)) ) )) //if there will be a FTD will add a FIRST TIME NO DROP (near guarentee) for none FTD 
                                        {
                                            //Dont allow regular drops on first fill
                                            if (lotIndex > maxLotIndex) //this shouldnt happen because of our curMaxLorIndex.
                                            {
                                                //this shu=ouldnt happen at all.
                                                Util.println(//stoneLine._idName + 
                                                    " out of itemlot slots!!!! " + keyword + "       npcID:" + npcID);
                                                break;
                                            }
                                            stoneLine.SetField("lotItemBasePoint0" + lotIndex.ToString(), 34463);//
                                                                                                                 //if (npcID == 21400114)
                                                                                                                 //    Util.p();
                                            curStoneLinesToAddGetItemFlagId.Add(stoneLine);
                                            curStoneLinesAddItemFlagIdLots.Add(lotIndex);
                                            //lotIndex++;
                                        }
                                    }
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
                                            if (test)
                                                Util.p();
                                        }
                                        donePostHawkFix = true;
                                    }
                                    if (treatAsBoss || (currentlyFirstDropGuarentee && !ftdx1ExcessToEmpty ))// special excpetion where we dont  
                                    {
                                        lotIndex = 1;
                                        emptyLotReplaced = true;
                                    }
                                    else
                                    {
                                        emptyLotReplaced = false;
                                        lotIndex = 2;
                                    }
                                    if (test)
                                        Util.p();
                                    string spaceString = "";
                                    if (itemLotEnemyName != "")
                                        spaceString = " ";

                                    if (test)
                                        Util.p();

                                    if (linesToReplaceOrRemove.Count == 0)
                                    {

                                        stoneLine = baseEmptyItemLotParamLine.Copy(ItemLotParam)
                                            .SetField(0, targetSmithingStoneLineID)
                                            .SetField(1, itemLotEnemyName + spaceString + newItemName)
                                            .SetField("lotItem_Rarity", currentRarity)
                                            ;

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
                                //int chanceToGiveToEmpty = 0;
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
                                            if(ftdx1ChanceMult != 1 && curXasscadeNumToDrop ==1 && currentlyFirstDropGuarentee)
                                            {
                                                float prevAmount = curXasscadeFinalPercent;
                                                curXasscadeFinalPercent *= ftdx1ChanceMult;
                                                //if (keyword == "15 > %0.65 #6 }0.85 +0.6 & sp1 & $$$$5 @@0.3  $0.6 White Wolf")
                                                //    Util.p();
                                                //if (stoneLine.id_int == 407100103)
                                                //    Util.p();
                                                if (ftdx1ExcessToEmpty && !emptyLotReplaced)
                                                {
                                                    var stoneId = stoneLine.id_int;
                                                    int chanceToGiveToEmpty = Math.Max(0,(int)((prevAmount - curXasscadeFinalPercent) + 0.5));
                                                    stoneLine.SetField("lotItemBasePoint01", chanceToGiveToEmpty).addKW("lotItemBasePoint01_Set");
                                                }
                                            }
                                            curXasscadeFinalPercentInt = (int)(curXasscadeFinalPercent + 0.5);
                                            if (test)
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

                                            if (test)
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
                                        if (test)
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
                                    if (test)
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

                                            if (test)
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
                                                if (test)
                                                    Util.println();
                                            }
                                        }
                                        curLineAmountsToChanceDict = newCurLineAmountsToChanceDict;
                                    }

                                    foreach (int key in curLineAmountsToChanceDict.Keys)
                                    {
                                        int _curKeyAmount = curLineAmountsToChanceDict[key]; ;
                                        curTotalPercent += _curKeyAmount;
                                        if (test)
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


                                    if (test)
                                        Util.println("      lot:" + lotIndex + " isSomber:" + (typeIndex == 1).ToString() + "  [" + curLevel + "]   x" + curNumToDrop);
                                }

                                if (test)
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
                                    else if(!stoneLine.hasKeyword("lotItemBasePoint01_Set"))
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
                                    if (d == 0 && dmax == 1 && (!isTreasure || Treasure_BlockNormalDropsUntilFound)) //if there will be a FTD will add a FIRST TIME NO DROP (near guarentee) for none FTD 
                                    {
                                        //Dont allow regular drops on first fill
                                        if (lotIndex > maxLotIndex) //this shouldnt happen because of our curMaxLorIndex.
                                        {
                                            //this shu=ouldnt happen at all.
                                            Util.println(//stoneLine._idName + 
                                                " out of itemlot slots!!!! " + keyword + "       npcID:" + npcID);
                                            break;
                                        }
                                        stoneLine.SetField("lotItemBasePoint0" + lotIndex.ToString(), 34463);//
                                        //if (npcID == 21400114)
                                        //    Util.p();
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
                            if (test)
                                Util.println(npcLine._idName + "  stoneLine is null.");
                            if (d == 0 && dmax == 1)
                            {
                                if (!ftdx1ExcessToEmpty)
                                {
                                    treatAsBoss = true; //i gues this is to skip to rune.
                                    d = -1;
                                    dmax = 0;

                                    curPercentChance = 0;
                                    percentChance = 0;
                                }
                                else
                                {
                                    curPercentChance = 1000;
                                    percentChance = 1000;
                                }
                            }
                            else
                                npcLine.RevertFieldToVanilla("itemLotId_enemy");
                        }
                        else
                        {
                            

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
                            if (curStoneLinesAddItemFlagIdLots.Count > 0 && curGiveUniqueItemFlagID)
                            {
                                IntFilter.Single filter = IdFilters.StoneDrop_getItemFlagIDFilter;
                                if (typeIndex == 2)
                                    filter = IdFilters.RuneDrop_getItemFlagIDFilter;
                                int currentGetItemFlagId;
                                if (FTD_KeywordShardsId || FTD_LevelsShareId)
                                {
                                    string treasureDropId = keyword;
                                    if (FTD_LevelsShareId)
                                        treasureDropId += " " + curAdjustedLevel;
                                    if (TreasureDrodIdToFlagId.ContainsKey(treasureDropId))
                                        currentGetItemFlagId = TreasureDrodIdToFlagId[treasureDropId];
                                    else
                                    {
                                        currentGetItemFlagId = IntFilter.GetRandomInt(npcID, filter, usedGetItemFlagId);
                                        TreasureDrodIdToFlagId.Add(treasureDropId, currentGetItemFlagId);
                                    }
                                }
                                else if (found_getItemFlagId != -1)
                                    currentGetItemFlagId = found_getItemFlagId;
                                else
                                    currentGetItemFlagId = IntFilter.GetRandomInt(npcID, filter, usedGetItemFlagId);
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
        static void ShopLineupChanges(bool VendorsSmithingStoneNerf, bool TwinMaidenSmithingStoneNerf, bool VendorsMaterialBuff, bool MiscNerfs)
        {
            if (!IsRunningParamFile(new ParamFile[] { ShopLineupParam }))
                return;

            int sellQuantity = ShopLineupParam.GetFieldIndex("sellQuantity");
            int value = ShopLineupParam.GetFieldIndex("value");
            int equipId = ShopLineupParam.GetFieldIndex("equipId");
            int equipType = ShopLineupParam.GetFieldIndex("equipType");
            int goodsCategory = 3;
            int weaponCategory = 0;

            if (VendorsSmithingStoneNerf)
            {
                foreach (Line l in ShopLineupParam.GetLinesOnCondition(new Condition.HasInName("Smithing Stone [").AND(new Condition.HasInName("Merchant"))))
                {
                    l.SetField(sellQuantity, 1);
                    //l.Operate(new OperateIntField(value, Operation.MULTIPLY, 1.5f));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 400), new Condition.HasInName("Smithing Stone [1]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 600), new Condition.HasInName("Smithing Stone [2]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 900), new Condition.HasInName("Smithing Stone [3]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 1200), new Condition.HasInName("Smithing Stone [4]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 1600), new Condition.HasInName("Smithing Stone [5]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 2400), new Condition.HasInName("Smithing Stone [6]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 3200), new Condition.HasInName("Smithing Stone [7]"));
                    l.Operate(new OperateIntField(value, Operation.EQUALS, 4500), new Condition.HasInName("Smithing Stone [8]"));
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

            if (TwinMaidenSmithingStoneNerf)
            {
                Condition isSomberCond = new Condition.HasInName("Somber Smithing Stone [");
                foreach (Line l in ShopLineupParam.GetLinesOnCondition(new Condition.HasInName("Smithing Stone [").AND(new Condition.HasInName("[Twin Maiden Husks]"))))
                {
                    l.Operate(new OperateIntField(value, Operation.MULTIPLY, 1.5f));

                    if (isSomberCond.Pass(l))
                    {
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 2000), new Condition.HasInName("Somber Smithing Stone [1]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 4000), new Condition.HasInName("Somber Smithing Stone [2]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 6000), new Condition.HasInName("Somber Smithing Stone [3]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 8000), new Condition.HasInName("Somber Smithing Stone [4]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 10000), new Condition.HasInName("Somber Smithing Stone [5]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 12000), new Condition.HasInName("Somber Smithing Stone [6]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 16000), new Condition.HasInName("Somber Smithing Stone [7]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 20000), new Condition.HasInName("Somber Smithing Stone [8]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 25000), new Condition.HasInName("Somber Smithing Stone [9]"));
                    }
                    else
                    {
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 400), new Condition.HasInName("Smithing Stone [1]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 600), new Condition.HasInName("Smithing Stone [2]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 900), new Condition.HasInName("Smithing Stone [3]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 1200), new Condition.HasInName("Smithing Stone [4]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 1600), new Condition.HasInName("Smithing Stone [5]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 2400), new Condition.HasInName("Smithing Stone [6]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 3200), new Condition.HasInName("Smithing Stone [7]"));
                        l.Operate(new OperateIntField(value, Operation.EQUALS, 4500), new Condition.HasInName("Smithing Stone [8]"));
                    }
                }
            }

            if (VendorsMaterialBuff)
            {
                var isMaterialCondition = new Condition.FieldIs(equipType, goodsCategory).AND(new Condition.FloatFieldBetween(equipId, 15000, 25000));
                var quantityLimitedCond = new Condition.FieldIs(sellQuantity, -1).IsFalse;

                foreach (Line l in ShopLineupParam.GetLinesOnCondition(isMaterialCondition.AND(quantityLimitedCond)))
                {
                    l.Operate(new OperateIntField(sellQuantity, Operation.MULTIPLY, 2f));
                    l.Operate(new OperateIntField(value, Operation.DIVIDED, 2f));
                }
            }

            if (MiscNerfs)
            {
                //arros cost 40 from 20
                var isShittyArrow = new Condition.FieldIs(equipType, weaponCategory).AND(new Condition.FieldIs(equipId, 50000000));
                foreach (Line l in ShopLineupParam.GetLinesOnCondition(isShittyArrow))
                {
                    l.Operate(new OperateIntField(sellQuantity, Operation.EQUALS, 40));
                }
            }
        }
        static void reduceSmithingStonePickups()
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
            var isTunnel = new Condition.HasInName(new string[] { "Tunnel", "[Ruin-Strewn Precipice]" });

            //var isInDungeon = new Condition.NameStartsWith("["); //is in dungen or material node or dropped by bosses.

            

            int lotItemId01 = ItemLotParam_map.GetFieldIndex("lotItemId01");
            var isSmithingStone = new Condition.FieldIs(lotItemId01,Util.ToStrings(smithingStoneItemIDsDict.Keys.ToArray()));
            var isSomberSmithingStone = new Condition.FieldIs(lotItemId01,Util.ToStrings(somberSmithingStoneItemIDsDict.Keys.ToArray()));

            var isStone = new Condition.Either(isSmithingStone, isSomberSmithingStone);
            //var isSmithingStone = isSomberSmithingStone.IsFalse;
            //var isAncientSS = new Condition.HasInName("Ancient Dragon Smithing Stone");
            //var isAncientSomberSS = new Condition.HasInName("Somber Ancient Dragon Smithing Stone");

            //turn tear scarabs of sombers into lost ashes of wars.
            //reduce level of sombers that are not LD by 2.
            var stoneLines = ItemLotParam_map.GetLinesOnCondition(isStone);

            //Random rand = new Random(0);    //seed is 0 for consistency.

            //somber lines to not transform?
            //liurnia gazebo smithing stones
            int[] luirniaGazeboIDs = new int[] {
                1035440110,
                1035470010,
                1037430010,
                1037450100,
                1038420000,
                1036430000,
            };

            var isLuirniaGazeboCond = new Condition.IDCheck(luirniaGazeboIDs);
            

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

                    bool forceTransformToSmithingStone = false;
                    if (isLuirniaGazeboCond.Pass(stoneLine))
                    {
                        forceTransformToSmithingStone = true;
                    }

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

                    Random rand = new Random(stoneLine.id_int);    //seed is line id for consistency.

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
                                if (forceTransformToSmithingStone || rand.NextDouble() < LD_SomberSSchanceToChangeIntoSmithing)
                                    turnToSmithing = true;
                            }
                            else
                            if (forceTransformToSmithingStone || rand.NextDouble() < SomberSSchanceToChangeIntoSmithing)
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

            //ItemLotParam_map.PrintCompareLineChanges();
            //var i = ItemLotParam_map.numberOfModifiedOrAddedLines;
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
                magicLine.GetFirstFieldIndexOnCondition(magicRefIdFIs, new Condition.FieldIs("-1"), out int magicArrayIndex);
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





