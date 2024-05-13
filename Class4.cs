using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingCSVHelper
{

    public struct Keyword
    {
        public string keyword;
        public int scale;

        public Keyword(string keyword, int scale = 100)
        {
            this.keyword = keyword;
            this.scale = scale;
        }
    }

    public interface IThemescaped
    {
        Themescaped addKeyword(string keyword, int scale = 100);
        Themescaped addKeyword(Keyword keyword);
        bool hasKeyword(Keyword keyword);
    }

    public class Themescaped
    {
        public List<Keyword> keywords = new List<Keyword>();
        public Themescaped addKeyword(string keyword, int scale = 100)
        {
            return addKeyword(new Keyword(keyword, scale));
        }
        public Themescaped addKeyword(Keyword keyword)
        {
            keywords.Add(keyword);
            return this;
        }
        public bool hasKeyword(Keyword keyword)
        {
            return keywords.Contains(keyword);
        }
        public static Themescaped[] getHasKeyword(Themescaped[] objs, Keyword keyword)
        {
            List<Themescaped> ret = new List<Themescaped>();
            foreach(Themescaped obj in objs)
            {
                if (obj.hasKeyword(keyword))
                    ret.Add(obj);
            }
            return ret.ToArray();
        }
    }


    public class LotItem:Themescaped
    {
        
        public const int MAX_CHANCE = 32000;
        public static class Category
        {
            public static int None = 0;
            public static int Good = 1;
            public static int Weapon = 2;
            public static int Armor = 3;
            public static int Accessory = 4;
            public static int AshOfWar = 5;
            public static int CustomWeapon = 3;
        }


        public int id;
        public int category;
        public string Name
        {
            get
            {
                return CategoryParamFile.GetLineWithId(id).name;
            }
        }

        ParamFile CategoryParamFile
        {
            get
            {
                if (category == null || category == Category.None)
                    return null;
                else if (category == Category.Good)
                    return Program.EquipParamGoods;
                else if (category == Category.Weapon)
                    return Program.EquipParamWeapon;
                else if (category == Category.Armor)
                    return Program.EquipParamProtector;
                else if (category == Category.Accessory)
                    return Program.EquipParamAccessory;
                else if (category == Category.CustomWeapon)
                    return Program.EquipParamCustomWeapon;
                return null;
            }
        }
        public int amount = 1;
        public int chance;
        public bool affectByLuck = false;

        public bool hasLotItem_getItemFlagIdFIs;
        public int lotItem_getItemFlagId;

        public static int getItemFlagIdFI = Program.ItemLotParam_enemy.GetFieldIndex("getItemFlagId");
        public static int[] idFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemId");
        public static int[] categoryFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemCategory");
        public static int[] amountFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemNum");
        public static int[] chanceFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemBasePoint");
        public static int[] affectByLuckFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("enableLuck");
        public static int[] lotItem_getItemFlagIdFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("getItemFlagId0");

        public LotItem addKW(string keyword, int scale = 100)
        {
            return addKW(new Keyword(keyword, scale));
        }
        public LotItem addKW(Keyword keyword)
        {
            keywords.Add(keyword);
            return this;
        }

        public void SetLotItemToLine(Line line, int lotIndex = 1)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            if (hasLotItem_getItemFlagIdFIs)
                line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);

        }
        public void SetLotItemToLine(Line line, int lotIndex, int chance, int amount = 1, bool affectByLuck = true)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            if (lotItem_getItemFlagId != -1)
                line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);
        }
        public void SetLotItemToLine(Line line, int lotIndex, int chance, int amount, bool affectByLuck, int lotItem_getItemFlagId)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);
        }

        public bool LineHasLotItem(Line line, bool sharesLotInfo)
        {
            for(int lotIndex = 1; lotIndex <= idFIs.Length; lotIndex++)
            {
                if (LotHasLotItem(line, lotIndex, sharesLotInfo))
                    return true;
            }
            return false;
        }
        public static LotItem newEmpty()
        {
            return new LotItem(0, 0, 0, 0, false);
        }
        public static LotItem newEmpty(int chance, bool affectByLuck = false)
        {
            return new LotItem(0, 0, 0, chance, affectByLuck);
        }
        public static LotItem newEmpty(int chance, bool affectByLuck, int itemLot_getItemFlagId)
        {
            return new LotItem(0, 0, 0, chance, affectByLuck, itemLot_getItemFlagId);
        }
        public bool IsEmpty()
        {
            return (category == 0 && id == 0);
        }

        public void SetItemLot_getItemFlagId(int lotItem_getItemFlagId)
        {
            hasLotItem_getItemFlagIdFIs = true;
            this.lotItem_getItemFlagId = lotItem_getItemFlagId;
        }
        public bool LotHasLotItem(Line line, int lotIndex, bool sharesLotInfo)
        {
            if(id == line.GetFieldAsInt(idFIs[lotIndex - 1]) && category == line.GetFieldAsInt(categoryFIs[lotIndex - 1]))
            {
                if (sharesLotInfo)
                    return
                    chance      == line.GetFieldAsInt(chanceFIs[lotIndex - 1]) &&
                    amount      == line.GetFieldAsInt(amountFIs[lotIndex - 1]) &&
                    affectByLuck== (line.GetFieldAsInt(affectByLuckFIs[lotIndex - 1]) == 1);
                else
                    return true;
            }
            return false;
        }
        public static int Default_Chance = 1000;
        static int Default_Amount = 1;
        static bool Default_AffectByLuck = false;

        public LotItem(int category, string itemName)
        {
            this.category = category;
            this.id = CategoryParamFile.GetLineWithName(itemName).id_int;
            this.amount = Default_Amount;
            this.chance = Default_Chance;
            this.affectByLuck = Default_AffectByLuck;
        }
        public LotItem(int category, int id)
        {
            this.category = category;
            this.id = id;
            this.amount = Default_Amount;
            this.chance = Default_Chance;
            this.affectByLuck = Default_AffectByLuck;
        }
        public LotItem(int category, int id, int amount = 1, int chance = 1000, bool affectByLuck = false)
        {
            this.category = category;
            this.id = id;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
        }
        public LotItem(int category, int id, int amount, int chance, bool affectByLuck, int lotItem_getItemFlagId)
        {
            this.category = category;
            this.id = id;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
            this.hasLotItem_getItemFlagIdFIs = true;
            this.lotItem_getItemFlagId = lotItem_getItemFlagId;
        }
        public LotItem(Line line, int lotIndex = 2, bool copyLotInfo = false)
        {
            id = line.GetFieldAsInt(idFIs[lotIndex - 1]);
            category = line.GetFieldAsInt(categoryFIs[lotIndex - 1]);
            if (copyLotInfo)
            {
                chance = line.GetFieldAsInt(chanceFIs[lotIndex - 1]);
                amount = line.GetFieldAsInt(amountFIs[lotIndex - 1]);
                affectByLuck = line.GetFieldAsInt(affectByLuckFIs[lotIndex - 1]) == 1;
            }
        }
    }
    static class oldTestsAndStuff
    {
        static ParamFile ToRun = null; //(dummy to avoid errors)
        static void FixNames()
        {
            for (int i = 0; i < ToRun.lines.Count; i++)
            {
                var line = ToRun.lines[i];
                int c = line.name.IndexOf("-" + line.GetField(2));
                if (c != -1)
                {
                    string name = line.name.Remove(c, line.name.Length - c);
                    line.Operate(new SetFieldTo(1, name));
                }
            }
        }
        public static void PrintOriginalNames(ParamFile Param)
        {
            PrintOriginalFields(Param, 1);
        }
        public static void PrintOriginalFields(ParamFile Param, string fieldName)
        {
            PrintOriginalFields(Param, Param.GetFieldIndex(fieldName));
        }
        public static void PrintOriginalFields(ParamFile Param, int fieldIndex = 1)
        {
            List<string> ogFields = new List<string>();
            foreach(Line l in Param.lines)
            {
                if (!ogFields.Contains(l.GetField(fieldIndex)))
                {
                    ogFields.Add(l.name);
                }
            }
            foreach(string s in ogFields)
            {
                Util.println('"'+ s +'"'+ ",");
            }
        }
        static void randomizedWeapons()
        {
            //if (!Program.IsRunningParamFile(new ParamFile[] { Program.ItemLotParam_map }))
            //   return;

                                                                                             //specific number                       //specific range 2
            var RandomizedItem_getItemFlagIDFilter = IntFilter.Create(true, 1, 0, IntFilter.Digit(3, 5), 3, IntFilter.Digit(3, 5), IntFilter.Digit(0, 8), 7, -1, -1, 0);



            List<int> usedGetItemFlagId;
            {
                int _getItemFlagId = Program.ItemLotParam_map.GetFieldIndex("getItemFlagId");
                var inIdRange = new Condition.FloatFieldBetween(_getItemFlagId, 1000000000, 2000000000);
                usedGetItemFlagId = Util.ToInts(((Lines)Program.ItemLotParam_enemy.GetLinesOnCondition(inIdRange)).GetFields(_getItemFlagId)).ToList();
                usedGetItemFlagId = usedGetItemFlagId.Concat(Util.ToInts(((Lines)Program.ItemLotParam_map.GetLinesOnCondition(inIdRange)).GetFields(_getItemFlagId))).ToList();
            }
            int getItemFlagIdFI = Program.ItemLotParam_map.GetFieldIndex("getItemFlagId");
            Dictionary<LotItem, int> lotItemToFlagIdDict = new Dictionary<LotItem, int>();


            LotItem[] exceptions = new LotItem[]
            {
                new LotItem(LotItem.Category.Weapon, 33060000), //Demi-Human Queen's Staff
                new LotItem(LotItem.Category.Weapon, 4100000), //Grafted Blade Greatsword
                new LotItem(LotItem.Category.Weapon, 42030000), //Erdtree Greatbow
                new LotItem(LotItem.Category.Weapon, 18080000), //Golden Halberd

                new LotItem(LotItem.Category.Weapon, 1040000), //Reduvia
                new LotItem(LotItem.Category.Weapon, 6000000), //Bloody Helice





                new LotItem(LotItem.Category.Weapon, 4070000), //Godslayer's Greatsword
                new LotItem(LotItem.Category.Weapon, 18160000), // Gargoyle's Black Halberd
                new LotItem(LotItem.Category.Weapon, 3210000), //Gargoyle's Blackblade
                new LotItem(LotItem.Category.Weapon, 15130000), //Gargoyle's Great Axe
                new LotItem(LotItem.Category.Weapon, 18150000), //Gargoyle's Halberd
                new LotItem(LotItem.Category.Weapon, 10090000), //Gargoyle's Black Blades
                new LotItem(LotItem.Category.Weapon, 3190000), //Gargoyle's Greatsword
                new LotItem(LotItem.Category.Weapon, 10080000), //Gargoyle's Twinblade

                new LotItem(LotItem.Category.Weapon, 18040000), //Commander's Standard

                new LotItem(LotItem.Category.Weapon, 18050000), //Nightrider Glaive
                new LotItem(LotItem.Category.Weapon, 13000000), //Nightrider Flail

                new LotItem(LotItem.Category.Weapon, 1110000), //Cinquedea
                new LotItem(LotItem.Category.Weapon, 12150000), //Beastclaw Greathammer
                new LotItem(LotItem.Category.Weapon, 34040000), //Clawmark Seal

                //kenneth haight, also in lleyndel
                new LotItem(LotItem.Category.Weapon, 1150000), //Erdsteel Dagger 

                new LotItem(LotItem.Category.Weapon, 2080000), //Nox Flowing Sword
                new LotItem(LotItem.Category.Weapon, 33240000), //Lusat's Glintstone Staff
                new LotItem(LotItem.Category.Weapon, 33280000), //Staff of Loss

                new LotItem(LotItem.Category.Weapon, 2060000), //Ornamental Straight Sword

                new LotItem(LotItem.Category.Weapon, 3090000), //Dark Moon Greatsword
                new LotItem(LotItem.Category.Weapon, 4060000), //Royal Greatsword

                new LotItem(LotItem.Category.Weapon, 17050000), //Vyke's War Spear
                new LotItem(LotItem.Category.Weapon, 13020000), //Family Heads

                //omenkiller
                new LotItem(LotItem.Category.Weapon, 15020000), //Great Omenkiller Cleaver, volcano mannor and miranda boss

                //given by patches
                new LotItem(LotItem.Category.Weapon, 20030000), //Magma Whip Candlestick

                //given by tanith after quest, serpent stuff
                new LotItem(LotItem.Category.Weapon, 9080000), //Serpentbone Blade

                new LotItem(LotItem.Category.Weapon, 17030000), //Serpent-Hunter

                new LotItem(LotItem.Category.Weapon, 16130000), //Inquisitor's Girandole
                new LotItem(LotItem.Category.Weapon, 6010000), //Godskin Stitcher
                new LotItem(LotItem.Category.Weapon, 10010000), //Godskin Peeler
                new LotItem(LotItem.Category.Weapon, 23080000), //Fallingstar Beast Jaw

                new LotItem(LotItem.Category.Weapon, 15120700), //Sacred Butchering Knife

                new LotItem(LotItem.Category.Weapon, 5040000), //Antspur Rapier
                new LotItem(LotItem.Category.Weapon, 3150000), //Marais Executioner's Sword

                new LotItem(LotItem.Category.Weapon, 9010000), //Nagakiba, given from yura
                new LotItem(LotItem.Category.Weapon, 10050000), //Eleonora's Poleblade

                new LotItem(LotItem.Category.Weapon, 40030000), //Harp Bow

                new LotItem(LotItem.Category.Weapon, 34060000), //Golden Order Seal

                new LotItem(LotItem.Category.Weapon, 8010000), //Onyx Lord's Greatsword

                new LotItem(LotItem.Category.Weapon, 21070000), //Iron Ball
                new LotItem(LotItem.Category.Weapon, 3160000), //Sword of Milos

                new LotItem(LotItem.Category.Weapon, 23060000), //Dragon Greatclaw

                new LotItem(LotItem.Category.Weapon, 3060000), //Ordovis's Greatsword

                new LotItem(LotItem.Category.Weapon, 16090000), //Bolt Of Gransax
                new LotItem(LotItem.Category.Weapon, 34030000), //Gravel Stone Seal

                new LotItem(LotItem.Category.Weapon, 11110000), //Scepter of the All-Knowing

                new LotItem(LotItem.Category.Weapon, 2110000), //Coded Sword

                new LotItem(LotItem.Category.Weapon, 23120000), //Golem's Halberd
                new LotItem(LotItem.Category.Weapon, 1160000), //Blade of Calling

                new LotItem(LotItem.Category.Weapon, 9040000), //Rivers of Blood
                new LotItem(LotItem.Category.Weapon, 20050000), //Hoslow's Petal Whip

                //deathy
                new LotItem(LotItem.Category.Weapon, 3130000), //Helphen's Steeple
                new LotItem(LotItem.Category.Weapon, 24050000), //Ghostflame Torch

                new LotItem(LotItem.Category.Weapon, 7100000), //Eclipse Shotel

                new LotItem(LotItem.Category.Weapon, 21120000), //Veteran's Prosthesis

                new LotItem(LotItem.Category.Weapon, 3170000), //Golden Order Greatsword

                new LotItem(LotItem.Category.Weapon, 12210000), //Rotten Battle Hammer
                new LotItem(LotItem.Category.Weapon, 23150000), //Rotten Greataxe

                new LotItem(LotItem.Category.Weapon, 18100000), //Loretta's War Sickle

                new LotItem(LotItem.Category.Weapon, 2260000), //Rotten Crystal Sword

                new LotItem(LotItem.Category.Weapon, 12200000), //Devourer's Scepter

                new LotItem(LotItem.Category.Weapon, 17020000), //Siluria's Tree

                new LotItem(LotItem.Category.Weapon, 2090000), //Inseparable Sword

                new LotItem(LotItem.Category.Weapon, 11060000), //Varré's Bouquet

                new LotItem(LotItem.Category.Weapon, 11080000), //Hammer, //fortified manor
            };
            LotItem[] chestItems = new LotItem[]
            {
                 //carriage chest
                 new LotItem(LotItem.Category.Weapon, 13010000), //Flail
                 new LotItem(LotItem.Category.Weapon, 3030000), //Lordsworn's Greatsword
                 new LotItem(LotItem.Category.Weapon, 11050000), //Morning Star
                 new LotItem(LotItem.Category.Weapon, 15000000), //Greataxe
                 new LotItem(LotItem.Category.Weapon, 4000000), //Greatsword
                 //--------
                 //good carriage weapons
                 new LotItem(LotItem.Category.Weapon, 2180000), //Carian Knight's Sword, found in luirnia thematic value
                 new LotItem(LotItem.Category.Weapon, 17070000), //Treespear
                 new LotItem(LotItem.Category.Weapon, 4030000), //Troll's Golden Sword
                 new LotItem(LotItem.Category.Weapon, 23110000), //Giant-Crusher
                 new LotItem(LotItem.Category.Weapon, 12180000), //Great Stars, thematic value, found in blood related area             
                 new LotItem(LotItem.Category.Weapon, 24040000), //St. Trina's Torch,  concentrated snow field
                 new LotItem(LotItem.Category.Weapon, 7060000), //Flowing Curved Sword, concentrated snow field slight thematic value
                 //-------

                 //world chest
                 new LotItem(LotItem.Category.Weapon, 3180000), //Claymore
                 new LotItem(LotItem.Category.Weapon, 6020000), //Great epee
                 new LotItem(LotItem.Category.Weapon, 10000000), //Twinblade
                 new LotItem(LotItem.Category.Weapon, 21100000), //Katar
                 //------
                 new LotItem(LotItem.Category.Weapon, 5050000), //Frozen Needle //carian stuff
                 new LotItem(LotItem.Category.Weapon, 2140000), //Sword of Night and Flame // found in carian manor, carian stuff
                 new LotItem(LotItem.Category.Weapon, 12060000), //Great Mace
                 new LotItem(LotItem.Category.Weapon, 42040000), //Greatbow, // in a highway look out, fitting, in altus
                 new LotItem(LotItem.Category.Weapon, 41030000), //Erdtree Bow
                 

                 //thematically vauge chest locations
                 new LotItem(LotItem.Category.Weapon, 19060000), //Winged Scythe
                 new LotItem(LotItem.Category.Weapon, 2190000), //Sword of St. Trina
                 new LotItem(LotItem.Category.Weapon, 14080000), //Icerind Hatchet
                
                 //thematically accurate chest location
                 new LotItem(LotItem.Category.Weapon, 44000000), //Hand Ballista, high area by balista
                 new LotItem(LotItem.Category.Weapon, 16020000), //Crystal Spear, crystal tunnel
                 new LotItem(LotItem.Category.Weapon, 34010000), //Godslayer's Seal, stormviel castle sealed chest
                 //crystal stuff.
                 new LotItem(LotItem.Category.Weapon, 1050000), //Crystal Knife, raya lucaria crystal tunnel, 
                 new LotItem(LotItem.Category.Weapon, 22030000), //Raptor Talons
                 new LotItem(LotItem.Category.Weapon, 1080000), //Scorpion's Stinger
                 new LotItem(LotItem.Category.Weapon, 7070000), //Wing of Astel


                 //unsure
                 new LotItem(LotItem.Category.Weapon, 2220000), //Regalia of Eochaid
                 new LotItem(LotItem.Category.Weapon, 23100000), //Ghiza's Wheel

                 
            };
            LotItem[] merchantitems = new LotItem[]
            {
                
                //patches
                new LotItem(LotItem.Category.Weapon, 5000000), //Estoc, also sold by a nomadic merchant
                new LotItem(LotItem.Category.Weapon, 1020000), //Parrying Dagger
                //------

                //gostoc
                new LotItem(LotItem.Category.Weapon, 21000000), //Caestus
                //-------

                //pidia
                new LotItem(LotItem.Category.Weapon, 14050000), //Ripple Blade

                //-----------
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

                     new LotItem(LotItem.Category.Weapon, 24070000), //Sentry's Torch

                     new LotItem(LotItem.Category.Weapon, 7020000), //Shotel
                //-----------
            };
            LotItem[] corpseOrBossItems = new LotItem[]
            {
                new LotItem(LotItem.Category.Weapon, 24020000), //Steel-Wire Torch
                new LotItem(LotItem.Category.Weapon, 7030000), //Shamshir
                new LotItem(LotItem.Category.Weapon, 9000000), //Uchigatana
                new LotItem(LotItem.Category.Weapon, 17060000), //Lance
                new LotItem(LotItem.Category.Weapon, 18020000), //Lucerne
                new LotItem(LotItem.Category.Weapon, 7080000), //Scavenger's Curved Sword
                new LotItem(LotItem.Category.Weapon, 10030000), //Twinned Knight Swords
                new LotItem(LotItem.Category.Weapon, 41070000), //Black Bow, in a rooftop, slight thematic value
                new LotItem(LotItem.Category.Weapon, 2210000), //Cane Sword
                //-------

                new LotItem(LotItem.Category.Weapon, 21080000), //Star Fist. found around arena. thematic value

                new LotItem(LotItem.Category.Weapon, 14140000), //Stormhawk Axe, fortified manor, and castle soll

                new LotItem(LotItem.Category.Weapon, 23130000), //Troll's Hammer, found neear boss
                new LotItem(LotItem.Category.Weapon, 23020000), //Great Club ,dropeed by stone digger troll

                new LotItem(LotItem.Category.Weapon, 43050000), //Pulley Crossbow
                new LotItem(LotItem.Category.Weapon, 41060000), //Pulley Bow

                //dropped by demi human queen margot
                new LotItem(LotItem.Category.Weapon, 44010000), //Jar Cannon


                new LotItem(LotItem.Category.Weapon, 34070000), //Erdtree Seal
                new LotItem(LotItem.Category.Weapon, 2070000), //Golden Epitaph

                new LotItem(LotItem.Category.Weapon, 1010000), //Black Knife


               //stromveil
               new LotItem(LotItem.Category.Weapon, 12190000), //Brick Hammer
               new LotItem(LotItem.Category.Weapon, 22000000), //Hookclaws
               new LotItem(LotItem.Category.Weapon, 1030000), //Miséricorde
               new LotItem(LotItem.Category.Weapon, 14100000), //Highland Axe, thematicly zccurate, in front of godfrey painting
               new LotItem(LotItem.Category.Weapon, 43080000), //Arbalest
               new LotItem(LotItem.Category.Weapon, 16070000), //Pike
               //-----------

                //caelid
                new LotItem(LotItem.Category.Weapon, 16110000), //Cross-Naginata
                new LotItem(LotItem.Category.Weapon, 1100000), //Wakizashi
                //----------
                new LotItem(LotItem.Category.Weapon, 33250000), //Meteorite Staff
                new LotItem(LotItem.Category.Weapon, 9030000), //Meteoric Ore Blade

                //catacombs
                new LotItem(LotItem.Category.Weapon, 19000000), //Scythe
                //---
                new LotItem(LotItem.Category.Weapon, 23010000), //Watchdog's Staff
                new LotItem(LotItem.Category.Weapon, 14120000), //Rosus' Axe

                //carian
                new LotItem(LotItem.Category.Weapon, 33170000), //Carian Glintblade Staff
                new LotItem(LotItem.Category.Weapon, 33210000), //Carian Glintstone Staff

                //crystal stuff
                new LotItem(LotItem.Category.Weapon, 2150000), //Crystal Sword


                new LotItem(LotItem.Category.Weapon, 2200000), //Miquellan Knight's Sword

                new LotItem(LotItem.Category.Weapon, 1130000), //Ivory Sickle
                new LotItem(LotItem.Category.Weapon, 33190000), //Albinauric Staff
                new LotItem(LotItem.Category.Weapon, 20070000), //Urumi

                new LotItem(LotItem.Category.Weapon, 41020000), //Horn Bow. found near ancester spirits in nokrom. fitting enough

                new LotItem(LotItem.Category.Weapon, 11120000), //Nox Flowing Hammer //found in nokrom, fitting enough



                //serpent related
                new LotItem(LotItem.Category.Weapon, 41040000), //Serpent Bow
                new LotItem(LotItem.Category.Weapon, 22010000), //Venomous Fang
                new LotItem(LotItem.Category.Weapon, 7110000), //Serpent-God's Curved Sword

                //frenzied related
                new LotItem(LotItem.Category.Weapon, 34090000), //Frenzied Flame Seal, given by hyetta

                new LotItem(LotItem.Category.Weapon, 12130000), //Celebrant's Skull. found in dominula, fitting.

                //blashphemous
                new LotItem(LotItem.Category.Weapon, 11130000), //Ringed Finger

                //cementry shade
                new LotItem(LotItem.Category.Weapon, 7120000), //Mantis Blade

                //found in proximitty to jerren, slight thematic value
                new LotItem(LotItem.Category.Weapon, 3050000), //Flamberge
                //-----

                //castle morce cell area, thematically accurate
                new LotItem(LotItem.Category.Weapon, 20000000), //Whip
                //-----

                //demihuman body, limgrave
                new LotItem(LotItem.Category.Weapon, 12000000), //Large Club
                //------

                //misbegotten cave boss, weeping pennisula, thematically accurate location
                new LotItem(LotItem.Category.Weapon, 15060100), //Heavy Rusted Anchor

                //magama wyrm geal cave, weird drop, thematically vauge
                new LotItem(LotItem.Category.Weapon, 9060000), //Moonveil

                //magma wyrm in ruin strwn precupice
                new LotItem(LotItem.Category.Weapon, 8040000), //Magma Wyrm's Scalesword

                //prelate
                new LotItem(LotItem.Category.Weapon, 23000000), //Prelate's Inferno Crozier
                new LotItem(LotItem.Category.Weapon, 34020000), //Giant's Seal
                new LotItem(LotItem.Category.Weapon, 12170000), //Cranial Vessel Candlestand
                new LotItem(LotItem.Category.Weapon, 20020000), //Thorned Whip

                new LotItem(LotItem.Category.Weapon, 40020000), //Red Branch Shortbow, dropped by first page in lleyndell
               
                new LotItem(LotItem.Category.Weapon, 8050000), //Zamor Curved Sword. //ancient hero of zamor boss

                new LotItem(LotItem.Category.Weapon, 23140000), //Rotten Staff // dropped by rotten tree avatar in haligtree
                new LotItem(LotItem.Category.Weapon, 23070000), //Staff of the Avatar


                new LotItem(LotItem.Category.Weapon, 3070000), //Alabaster Lord's Sword

                new LotItem(LotItem.Category.Weapon, 9070000), //Dragonscale Blade
                new LotItem(LotItem.Category.Weapon, 18140000), //Dragon Halberd



            };

            LotItem[] deathbirdItems = new LotItem[]
{
                new LotItem(LotItem.Category.Weapon, 14110000), //Sacrificial Axe
                new LotItem(LotItem.Category.Weapon, 3200000), //Death's Poker
                new LotItem(LotItem.Category.Weapon, 16120000), //Death Ritual Spear

};
            LotItem[] bloodhoundItems = new LotItem[]
            {
                new LotItem(LotItem.Category.Weapon, 8030000), //Bloodhound's Fang
                new LotItem(LotItem.Category.Weapon, 22020000), //Bloodhound Claws
            };
            LotItem[] earlyBasicWeapons = new LotItem[]{
                new LotItem(LotItem.Category.Weapon, 13010000), //Flail //carriage
                new LotItem(LotItem.Category.Weapon, 3030000), //Lordsworn's Greatsword //carriage
                new LotItem(LotItem.Category.Weapon, 11050000), //Morning Star //carriage
                new LotItem(LotItem.Category.Weapon, 15000000), //Greataxe //carriage
                new LotItem(LotItem.Category.Weapon, 4000000), //Greatsword //carriage
                new LotItem(LotItem.Category.Weapon, 3180000), //Claymore //chest
                new LotItem(LotItem.Category.Weapon, 6020000), //Great epee  //chest
                new LotItem(LotItem.Category.Weapon, 10000000), //Twinblade  //chest
            };

            LotItem[] midgameWeapons = new LotItem[]{
                new LotItem(LotItem.Category.Weapon, 21100000), //Katar  //chest
                new LotItem(LotItem.Category.Weapon, 18020000), //Lucerne //body
                new LotItem(LotItem.Category.Weapon, 24020000), //Steel-Wire Torch  //body
                new LotItem(LotItem.Category.Weapon, 10030000), //Twinned Knight Swords  //body
                new LotItem(LotItem.Category.Weapon, 2210000), //Cane Sword  //body
                new LotItem(LotItem.Category.Weapon, 17060000), //Lance  //body
                new LotItem(LotItem.Category.Weapon, 43080000), //Arbalest //body
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
                new LotItem(LotItem.Category.Weapon, 41070000), //Black Bow // body in a rooftop, slight thematic value
              
                new LotItem(LotItem.Category.Weapon, 22000000), //Hookclaws //body
                new LotItem(LotItem.Category.Weapon, 1030000), //Miséricorde //body
            };

            LotItem[][] lotItemGroupsToRandomize = new LotItem[][]
            {
                deathbirdItems,
                bloodhoundItems,

                earlyBasicWeapons,
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
                Lines lines = Program.ItemLotParam_map.GetLinesOnCondition(new Condition.HasLotItem(lotItemGroup, 1, 1, true));
                for (int i = 0; i < lotItemGroup.Length; i++)
                {
                    int lotIndex = i + 1;
                    var setItemLotOperation = new SetLotItem(lotItemGroup[i], lotIndex);
                    int getItemFlagFI = Program.ItemLotParam_map.GetFieldIndex("getItemFlagId0" + lotIndex);

                    int currentGetItemFlagId;
                    if (lotItemToFlagIdDict.ContainsKey(lotItemGroup[i]))
                    {
                        currentGetItemFlagId = lotItemToFlagIdDict[lotItemGroup[i]];
                    }
                    else
                    {
                        currentGetItemFlagId = IntFilter.GetRandomInt(lotItemGroup[i].id, RandomizedItem_getItemFlagIDFilter, usedGetItemFlagId);
                        usedGetItemFlagId.Add(currentGetItemFlagId);
                        lotItemToFlagIdDict.Add(lotItemGroup[i], currentGetItemFlagId);
                    }

                    foreach (Line line in lines.lines)
                    {
                        line.Operate(setItemLotOperation);
                        line.SetField(getItemFlagFI, currentGetItemFlagId);
                    }
                }
            }

            Program.ItemLotParam_map.GetLineWithId(103701).RevertFieldsToVanilla();  //Coryns flail i dont want to change

        }
    }
}
