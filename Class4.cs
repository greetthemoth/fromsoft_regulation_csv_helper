using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EldenRingCSVHelper
{
    public static class SmithingStones
    {
         static bool init = false;
        public const int maxLevel = 9;
        static Dictionary<int, int> _somberSmithingStoneItemIDsDict = new Dictionary<int, int>();
        static Dictionary<int, int> _somberSmithingStoneIDsDict = new Dictionary<int, int>();
        static Dictionary<int, int> _smithingStoneItemIDsDict = new Dictionary<int, int>();
         static Dictionary<int, int> _smithingStoneIDsDict = new Dictionary<int, int>();

        static Dictionary<int, string> _smithingStonelevelToNameDict = new Dictionary<int, string>();
        static Dictionary<int, string> _somberSmithingStonelevelToNameDict = new Dictionary<int, string>();
        public static Dictionary<int, int> ItemIDsDict { get { Secure(); return _smithingStoneIDsDict; } }
        public static Dictionary<int, int> IDsDict { get { Secure(); return _smithingStoneIDsDict; } }
        public static Dictionary<int, string> LevelToNameDict { get { Secure(); return _smithingStonelevelToNameDict; } }
        public static class Somber
        {
            public const int maxLevel = 10;
            public static Dictionary<int, int> ItemIDsDict { get { Secure(); return _somberSmithingStoneItemIDsDict; } }
            public static Dictionary<int, int> IDsDict { get { Secure(); return _somberSmithingStoneIDsDict; } }
            public static Dictionary<int, string> LevelToNameDict { get { Secure(); return _somberSmithingStonelevelToNameDict; } }
        }

        

        public static void Secure()
        {
            if (init)
                return;
            init = true;
            CreateDicts();
        }

        static void CreateDicts() {
            {
                _smithingStoneItemIDsDict.Add(10100, 1);
                _smithingStoneItemIDsDict.Add(10101, 2);
                _smithingStoneItemIDsDict.Add(10102, 3);
                _smithingStoneItemIDsDict.Add(10103, 4);
                _smithingStoneItemIDsDict.Add(10104, 5);
                _smithingStoneItemIDsDict.Add(10105, 6);
                _smithingStoneItemIDsDict.Add(10106, 7);
                _smithingStoneItemIDsDict.Add(10107, 8);
                _smithingStoneItemIDsDict.Add(10140, 9);
                _somberSmithingStoneItemIDsDict.Add(10160, 1);
                _somberSmithingStoneItemIDsDict.Add(10161, 2);
                _somberSmithingStoneItemIDsDict.Add(10162, 3);
                _somberSmithingStoneItemIDsDict.Add(10163, 4);
                _somberSmithingStoneItemIDsDict.Add(10164, 5);
                _somberSmithingStoneItemIDsDict.Add(10165, 6);
                _somberSmithingStoneItemIDsDict.Add(10166, 7);
                _somberSmithingStoneItemIDsDict.Add(10167, 8);
                _somberSmithingStoneItemIDsDict.Add(10200, 9);
                _somberSmithingStoneItemIDsDict.Add(10168, 10);

                
            }
            {
                _smithingStoneIDsDict.Add(1, 10100);
                _smithingStoneIDsDict.Add(2, 10101);
                _smithingStoneIDsDict.Add(3, 10102);
                _smithingStoneIDsDict.Add(4, 10103);
                _smithingStoneIDsDict.Add(5, 10104);
                _smithingStoneIDsDict.Add(6, 10105);
                _smithingStoneIDsDict.Add(7, 10106);
                _smithingStoneIDsDict.Add(8, 10107);
                _smithingStoneIDsDict.Add(9, 10140);
                _somberSmithingStoneIDsDict.Add(1, 10160);
                _somberSmithingStoneIDsDict.Add(2, 10161);
                _somberSmithingStoneIDsDict.Add(3, 10162);
                _somberSmithingStoneIDsDict.Add(4, 10163);
                _somberSmithingStoneIDsDict.Add(5, 10164);
                _somberSmithingStoneIDsDict.Add(6, 10165);
                _somberSmithingStoneIDsDict.Add(7, 10166);
                _somberSmithingStoneIDsDict.Add(8, 10167);
                _somberSmithingStoneIDsDict.Add(9, 10200);
                _somberSmithingStoneIDsDict.Add(10, 10168);

                foreach (int level in _smithingStoneIDsDict.Keys)
                {
                    _smithingStonelevelToNameDict.Add(level, Program.EquipParamGoods.GetLineWithId(_smithingStoneIDsDict[level]).name);
                }
                foreach (int level in _somberSmithingStoneIDsDict.Keys)
                {
                    _somberSmithingStonelevelToNameDict.Add(level, Program.EquipParamGoods.GetLineWithId(_somberSmithingStoneIDsDict[level]).name);
                }
            }
        }

            
    }
    public static class makeInfo
    {
        public static void Names()
        {
            foreach (Line line in Program.ItemLotParam_map.lines)
            {
                var n = LotItem.Get(line, 1).Name;
                if (n == "invalid" || n == "")
                    continue;
                Util.println(line.id + ";" + n);
            }
            return;
        }
    }
    public struct Keyword
    {
        public string keyword;
        public float value;
        public bool copyable;
        public bool permanent;

        public Keyword(string keyword, float scale = 100, bool copyKWforCopys = false, bool dontClear = false)
        {
            this.keyword = keyword;
            this.value = scale;
            this.copyable = copyKWforCopys;
            this.permanent = dontClear;
        }
        public static bool IfModifiedSet_ON = false;
        public static Keyword IfModifiedSet;
    }

    public interface IThemescaped
    {
        Themescaped addKeyword(string keyword, float scale = 100);
        Themescaped addKeyword(Keyword keyword);
        bool hasKeyword(Keyword keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue);
        bool hasKeyword(string keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue);

    }

    public class Themescaped : IThemescaped
    {
        public List<Keyword> keywords = new List<Keyword>();
        public Keyword GetKeyword(int index)
        {
            return keywords[index];
        }
        public Keyword GetKeywordContaining(string keywordContains)
        {
            return keywords[getIndexKeywordContaining(keywords, keywordContains)];
        }
        public void SetKeywords(List<Keyword> newKeywords)
        {
            keywords = newKeywords;
        }
        public void ClearKeywords()
        {
            for (int i = 0; i < keywords.Count; i++)
            {
                if (!keywords[i].permanent)
                {
                    keywords.RemoveAt(i);
                    i--;
                }
                else
                {
                    Util.p(keywords[i].keyword);
                }
            }
        }
        public Themescaped addKeyword(string keyword, float scale = 100)
        {
            return addKeyword(new Keyword(keyword, scale));
        }
        public Themescaped addKeyword(Keyword keyword)
        {
            keywords.Add(keyword);
            return this;
        }
        public bool hasKeyword(Keyword keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            return hasKeyword(keywords, keyword, minScale, maxScale);
        }
        public bool hasKeyword(string keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            return hasKeyword(keywords, keyword, minScale, maxScale);
        }
        public bool hasKeywordContaining(string keywordContains, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            return getIndexKeywordContaining(keywords, keywordContains, minScale, maxScale)!=-1;
        }
        public static bool hasKeyword(List<Keyword> keywords, Keyword keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            foreach (Keyword kw in keywords)
            {
                if (kw.keyword == keyword.keyword)
                {
                    if (kw.value >= minScale && kw.value <= maxScale)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public static bool hasKeyword(List<Keyword> keywords, string keyword, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            foreach (Keyword kw in keywords)
            {
                if (kw.keyword == keyword)
                {
                    if (kw.value >= minScale && kw.value <= maxScale)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }
        public static int getIndexKeywordContaining(List<Keyword> keywords, string keywordContains, int minScale = -int.MaxValue, int maxScale = int.MaxValue)
        {
            int i = 0;
            foreach (Keyword kw in keywords)
            {
                if (kw.keyword.Contains(keywordContains))
                {
                    if (kw.value >= minScale && kw.value <= maxScale)
                        return i;
                    else
                        return -1;
                }
                i++;
            }
            return -1;
        }
        public static IThemescaped[] getHasKeyword(Themescaped[] objs, Keyword keyword)
        {
            List<IThemescaped> ret = new List<IThemescaped>();
            foreach (IThemescaped obj in objs)
            {
                if (obj.hasKeyword(keyword))
                    ret.Add(obj);
            }
            return ret.ToArray();
        }
    }

    public class LotItem : Themescaped
    {
        public const int MAX_LOT_INDEX = 8;
        public const int MAX_CHANCE = 34463;
        public static class Category
        {
            public static int None = 0;
            public static int Good = 1;
            public static int Weapon = 2;
            public static int Armor = 3;
            public static int Accessory = 4;
            public static int AshOfWar = 5;
            public static int CustomWeapon = 6;
        }


        public int id;
        public int category;
        public string Name
        {
            get
            {
                if (category == 0)
                    return "";
                var l = CategoryParamFile.GetLineWithId(id);
                if (l == null)
                    return "invalid";
                return l.name;
            }
        }
        public LotItem Copy()
        {
            var ret = new LotItem(category, id, amount, chance, affectByLuck);
            ret.hasLotItem_getItemFlagId = hasLotItem_getItemFlagId;
            ret.lotItem_getItemFlagId = lotItem_getItemFlagId;
            ret.SetKeywords(new List<Keyword>().Concat(keywords).ToList());
            return ret;
        }
        public static Line newBaseItemLotLine(ParamFile param, int id = 0, string name = "base item lot line")
        {
            return Program.ItemLotParam_enemy.vanillaParamFile.GetLineWithId(215000000).Copy(param).SetField(0, id).Operate(new SetLotItem(newEmpty(), new int[] { 2, 3, 4, 5, 6, 7, 8 })).SetField(1, name).Operate(new SetLotItem(newEmpty(1000),1)).SetField(1, name);
        }
        ParamFile CategoryParamFile
        {
            get
            {
                if (category == -1 || category == Category.None)
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
                else if (category == Category.AshOfWar)
                    return Program.EquipParamGem;
                else return null;

            }
        }
        public int amount = 1;
        public int chance;
        public bool affectByLuck = false;

        public bool hasLotItem_getItemFlagId;
        public int lotItem_getItemFlagId;

        public bool cumulateSet = false;
        public int cumulateNumFlagId = 0;
        public int cumulateNumMax = 0;
        public int cumulateLotPoint = 0;
        public bool cumulateResetReset = false;

        public static int getItemFlagIdFI = Program.ItemLotParam_enemy.GetFieldIndex("getItemFlagId");
        public static int[] idFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemId");
        public static int[] categoryFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemCategory");
        public static int[] amountFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemNum");
        public static int[] chanceFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemBasePoint");
        public static int[] affectByLuckFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("enableLuck");
        public static int[] lotItem_getItemFlagIdFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("getItemFlagId0");
        public static int cumulateNumFlagIdFI = Program.ItemLotParam_enemy.GetFieldIndex("cumulateNumFlagId");
        public static int cumulateNumMaxFI = Program.ItemLotParam_enemy.GetFieldIndex("cumulateNumMax");
        public static int[] cumulateLotPointFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("cumulateLotPoint0");
        public static int[] cumulateResetFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("cumulateReset0");

        public LotItem addKW(string keyword, int scale = 100)
        {
            return addKW(new Keyword(keyword, scale));
        }
        public LotItem addKW(Keyword keyword)
        {
            keywords.Add(keyword);
            return this;
        }

        public static LotItem Get(Line line, int lotIndex)
        {
            return new LotItem(line, lotIndex, true);
        }
        public static int GetChanceTotal(Line line, bool excludeMaxChance = false)
        {
            int total = 0;
            for (int i = 0; i < chanceFIs.Length; i++)
            {
                var li = Get(line, i + 1);
                if (excludeMaxChance && li.chance == MAX_CHANCE)
                    continue;
                total += li.chance;
            }
            return total;
        }
        public static int GetItemChanceTotal(Line line)
        {
            int total = 0;
            for (int i = 0; i < chanceFIs.Length; i++)
            {
                var li = Get(line, i + 1);
                if (li.IsEmpty())
                    continue;
                total += li.chance;
            }
            return total;
        }
        public static void RegulateLine(Line line, bool excludeMaxChance = false)
        {
            if (line.id_int == 308000155)
                Util.p();
            int total = GetChanceTotal(line, excludeMaxChance);
            if (total == 0)
                return;
            float mult = 1000f / total;
            for (int lotIndex = 1; lotIndex <= MAX_LOT_INDEX; lotIndex++)
            {
                int fi = chanceFIs[lotIndex - 1];
                int chance = line.GetFieldAsInt(fi);
                if (excludeMaxChance && chance == MAX_CHANCE)
                    continue;
                int newChance = (int)((chance * mult) + 0.5);
                if (chance == 0)
                    continue;
                line.SetField(fi, newChance);
            }
        }
        public static int GetFirstNonEmptyItemLotIndex(Line line)
        {
            for (int i = 0; i < MAX_LOT_INDEX; i++)
            {
                var li = Get(line, i + 1);
                bool isEmpty = li.IsEmpty();
                if (isEmpty)
                    continue;
                return i + 1;
            }
            return -1;
        }
        public static int GetEmptyChanceTotal(Line line)
        {
            int total = 0;
            for (int i = 0; i < chanceFIs.Length; i++)
            {
                var li = Get(line, i + 1);
                if (!li.IsEmpty())
                    continue;
                total += li.chance;
            }
            return total;
        }

        public void SetLotItemToLine(Line line, int lotIndex = 1)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            if (chance < 0)
                Util.p();
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            if (hasLotItem_getItemFlagId)
                line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);

            if (cumulateSet)
            {
                line.SetField(cumulateNumFlagIdFI, cumulateNumFlagId);
                line.SetField(cumulateNumMaxFI, cumulateNumMaxFI);
                line.SetField(cumulateLotPointFIs[lotIndex - 1], cumulateLotPoint);
                line.SetField(cumulateResetFIs[lotIndex - 1], cumulateResetReset);
            }
        }
        public void SetLotItemToLine(Line line, int lotIndex, int chance, int amount = 1, bool affectByLuck = true)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            if (chance < 0)
                Util.p();
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            if (lotItem_getItemFlagId != -1)
                line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);

            if (cumulateSet)
            {
                line.SetField(cumulateNumFlagIdFI, cumulateNumFlagId);
                line.SetField(cumulateNumMaxFI, cumulateNumMaxFI);
                line.SetField(cumulateLotPointFIs[lotIndex - 1], cumulateLotPoint);
                line.SetField(cumulateResetFIs[lotIndex - 1], cumulateResetReset);
            }
        }
        public void SetLotItemToLine(Line line, int lotIndex, int chance, int amount, bool affectByLuck, int lotItem_getItemFlagId)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            if (chance < 0)
                Util.p();
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
            line.SetField(lotItem_getItemFlagIdFIs[lotIndex - 1], lotItem_getItemFlagId);

            if (cumulateSet)
            {
                line.SetField(cumulateNumFlagIdFI, cumulateNumFlagId);
                line.SetField(cumulateNumMaxFI, cumulateNumMax);
                line.SetField(cumulateLotPointFIs[lotIndex - 1], cumulateLotPoint);
                line.SetField(cumulateResetFIs[lotIndex - 1], cumulateResetReset);
            }
        }

        public void setCumulateInfo(int cumulateNumFlagId, int cumulateNumMax, int cumulateLotPoint, bool cumulateLotReset)
        {
            cumulateSet = true;
            this.cumulateNumFlagId = cumulateNumFlagId;
            this.cumulateNumMax = cumulateNumMax;
            this.cumulateLotPoint = cumulateLotPoint;
            this.cumulateResetReset = cumulateLotReset;
        }

        public bool LineHasLotItem(Line line, bool sharesLotInfo)
        {
            for (int lotIndex = 1; lotIndex <= idFIs.Length; lotIndex++)
            {
                if (LotHasLotItem(line, lotIndex, sharesLotInfo))
                    return true;
            }
            return false;
        }
        public int GetLotItemIndex(Line line, bool sharesLotInfo)
        {
            for (int lotIndex = 1; lotIndex <= idFIs.Length; lotIndex++)
            {
                if (LotHasLotItem(line, lotIndex, sharesLotInfo))
                    return lotIndex;
            }
            return -1;
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
        public static LotItem newEmpty(int chance, bool affectByLuck, int itemLot_getItemFlagId, int cumulateNumFlagId, int cumulateNumMax, int cumulateLotPoint = MAX_CHANCE,bool cumulateReset = false)
        {
            return new LotItem(0,0,0,chance, affectByLuck, itemLot_getItemFlagId, cumulateNumFlagId, cumulateNumMax, cumulateLotPoint, cumulateReset);
        }
        public bool IsEmpty()
        {
            return (category == 0 && id == 0);
        }

        public void SetItemLot_getItemFlagId(int lotItem_getItemFlagId)
        {
            hasLotItem_getItemFlagId = true;
            this.lotItem_getItemFlagId = lotItem_getItemFlagId;
        }
        public bool LotHasLotItem(Line line, int lotIndex, bool sharesLotInfo)
        {
            if (id == line.GetFieldAsInt(idFIs[lotIndex - 1]) && category == line.GetFieldAsInt(categoryFIs[lotIndex - 1]))
            {
                if (sharesLotInfo)
                    return
                    chance == line.GetFieldAsInt(chanceFIs[lotIndex - 1]) &&
                    amount == line.GetFieldAsInt(amountFIs[lotIndex - 1]) &&
                    affectByLuck == (line.GetFieldAsInt(affectByLuckFIs[lotIndex - 1]) == 1);
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
        public LotItem(int category, string itemName, int amount = 1, int chance = 1000, bool affectByLuck = false)
        {
            this.category = category;
            this.id = CategoryParamFile.GetLineWithName(itemName).id_int;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
        }
        public LotItem(int category, string itemName, int amount, int chance, bool affectByLuck, int lotItem_getItemFlagId, int cumulateNumFlagId = 0, int cumulateNumMax = 0, int cumulateLotPoint = 0, bool cumulateReset = false)
        {
            this.category = category;
            this.id = CategoryParamFile.GetLineWithName(itemName).id_int;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
            this.hasLotItem_getItemFlagId = true;
            this.lotItem_getItemFlagId = lotItem_getItemFlagId;

            if (cumulateNumFlagId != 0) {
                this.cumulateSet = true;
                this.cumulateNumFlagId = cumulateNumFlagId;
                this.cumulateNumMax = cumulateNumMax;
                this.cumulateLotPoint = cumulateLotPoint;
                this.cumulateResetReset = cumulateReset;
            }
        }
        public LotItem(int category, int id, int amount, int chance, bool affectByLuck, int lotItem_getItemFlagId, int cumulateNumFlagId = 0, int cumulateNumMax = 0, int cumulateLotPoint = 0, bool cumulateReset = false)
        {
            this.category = category;
            this.id = id;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
            this.hasLotItem_getItemFlagId = true;
            this.lotItem_getItemFlagId = lotItem_getItemFlagId;

            if (cumulateNumFlagId != 0)
            {
                this.cumulateSet = true;
                this.cumulateNumFlagId = cumulateNumFlagId;
                this.cumulateNumMax = cumulateNumMax;
                this.cumulateLotPoint = cumulateLotPoint;
                this.cumulateResetReset = cumulateReset;
            }
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
            this.hasLotItem_getItemFlagId = true;
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

        public class HasLotItem : Condition
        {
            LotItem[] lotItems;
            int lotIndex = -1;
            bool sharesLineInfo = false;
            int amountToHave = 1;
            public HasLotItem(LotItem lotItem, int lotIndex = -1, bool sharesLineInfo = false)
            {
                lotItems = new LotItem[] { lotItem };
                this.lotIndex = lotIndex;
                this.sharesLineInfo = sharesLineInfo;
            }
            public HasLotItem(LotItem[] lotItems, int amountToHave = 1, int lotIndex = -1, bool sharesLineInfo = false)
            {
                this.lotItems = lotItems;
                this.lotIndex = lotIndex;
                this.sharesLineInfo = sharesLineInfo;
                this.amountToHave = amountToHave;
            }
            public override bool Pass(Line line)
            {
                int countFound = 0;
                foreach (LotItem lotItem in lotItems)
                {
                    if (lotIndex == -1)
                    {
                        if (lotItem.LineHasLotItem(line, sharesLineInfo))
                        {
                            countFound++;
                            if (countFound >= amountToHave)
                                return true;
                        }
                    }
                    else
                    {
                        if (lotItem.LotHasLotItem(line, lotIndex, sharesLineInfo))
                        {
                            countFound++;
                            if (countFound >= amountToHave)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public class SetLotItem : LineModifier
        {
            LotItem lotItem;
            int[] lotIndexes;
            bool useInfo = true;
            int chance;
            int amount;
            bool affectByLuck;
            int lotItem_getItemFlagId;
            public SetLotItem(LotItem lotItem, int lotIndex)
            {
                this.lotItem = lotItem;
                lotIndexes = new int[] { lotIndex };
            }
            public SetLotItem(LotItem lotItem, int[] lotIndexes)
            {
                this.lotItem = lotItem;
                this.lotIndexes = lotIndexes;
            }
            public SetLotItem(LotItem lotItem, int lotIndex = 1, int chance = 1000, int amount = 1, bool affectByLuck = true, int lotItem_getItemFlagId = -1)
            {
                this.lotItem = lotItem;
                lotIndexes = new int[] { lotIndex };
                useInfo = false;
                this.chance = chance;
                this.amount = amount;
                this.affectByLuck = affectByLuck;
                this.lotItem_getItemFlagId = lotItem_getItemFlagId;
            }
            public SetLotItem(LotItem lotItem, int[] lotIndexes, int chance = 1000, int amount = 1, bool affectByLuck = true, int lotItem_getItemFlagId = -1)
            {
                this.lotItem = lotItem;
                this.lotIndexes = lotIndexes;
                useInfo = false;
                this.chance = chance;
                this.amount = amount;
                this.affectByLuck = affectByLuck;
                this.lotItem_getItemFlagId = lotItem_getItemFlagId;
            }
            public override void Operate(Line line)
            {
                foreach (int lotIndex in lotIndexes)
                {
                    if (useInfo)
                        lotItem.SetLotItemToLine(line, lotIndex);
                    else
                        lotItem.SetLotItemToLine(line, lotIndex, chance, amount, affectByLuck, lotItem_getItemFlagId);
                }
            }
        }
    }

    public static class AddedLineManager
    {
        static Dictionary< ParamFile, List<AddedLineInfo >> Dict = new Dictionary<ParamFile, List<AddedLineInfo>>();
        static Dictionary<string, int> ChangeNameToOrderDict = new Dictionary<string, int>();

        public class AddedLineInfo
        {
            public static int nextOrder = 0;
            public ParamFile file;
            public string changeName;
            public int lineId;
            public int order;

            public AddedLineInfo(ParamFile file, string changeName, int lineId)
            {
                order = nextOrder;
                this.file = file;
                this.changeName = changeName;
                this.lineId = lineId;
            }
        }

        public static void Catalog (string changeName)
        {
            if (ChangeNameToOrderDict.ContainsKey(changeName))
                return;

            bool foundAnAddedLine = false;

            foreach(ParamFile p in ParamFile.paramFiles)
            {
                if (p.numberOfModifiedOrAddedLines == 0)
                    continue;
                foreach(Line l in p.lines)
                {
                    if (l.added && !l.hasKeyword("!Buffer!"))
                    {
                        foundAnAddedLine = true;
                        if (!Dict.ContainsKey(p))
                        {
                            Dict.Add(p, new List<AddedLineInfo>());
                        }
                        Dict[p].Add(new AddedLineInfo(p, changeName, l.id_int));
                    }
                }
            }
            
            if (foundAnAddedLine) {
                ChangeNameToOrderDict.Add(changeName, AddedLineInfo.nextOrder);
                AddedLineInfo.nextOrder++;
            }
        }
        public static void CreateEmptyLines(Line baseLineToCopy, string curFunctionOrderName = "", int dontCreateOrder = -1)
        {
            foreach(ParamFile p in ParamFile.paramFiles)
            {
                if (p.numberOfModifiedOrAddedLines != 0 && Dict.ContainsKey(p))
                    CreateEmptyLines(p,baseLineToCopy, curFunctionOrderName, dontCreateOrder);
            }
        }
        public static void CreateEmptyLines(ParamFile p, Line baseLineToCopy, string curFunctionOrderName = "", int dontCreateOrder = -1)
        {
            int curOrder;
            if (curFunctionOrderName != "" && ChangeNameToOrderDict.ContainsKey(curFunctionOrderName))
                curOrder = ChangeNameToOrderDict[curFunctionOrderName];
            else
                curOrder = AddedLineInfo.nextOrder;
            //List<AddedLineInfo> addedLineInfos = Dict[p];
            List<AddedLineInfo> addedLineInfos = Dict[p].Concat(new List<AddedLineInfo>()).ToList();
            addedLineInfos.Reverse();
            int startIndex = 0;
            foreach (AddedLineInfo ali in addedLineInfos)
            {
                if (curOrder <= ali.order || dontCreateOrder == ali.order)
                    continue;
                //if (curFunctionOrderName == "StoneDrops")
                //    Util.p();
                Line l = baseLineToCopy.Copy().SetField(0, ali.lineId).SetField(1, "_Compatability Buffer for " + ali.changeName +"_Order: " + ali.order).addKW("!Buffer!", ali.order);
                p.InsertLine(l, out startIndex, startIndex);
            }
        }
        public static bool AttemptToCreateEmptyLine( int id , ParamFile p, Line baseLineToCopy, string curFunctionOrderName = "")
        {
            int curOrder;
            if (curFunctionOrderName != "" && ChangeNameToOrderDict.ContainsKey(curFunctionOrderName))
                curOrder = ChangeNameToOrderDict[curFunctionOrderName];
            else
                curOrder = AddedLineInfo.nextOrder;
            //List < AddedLineInfo > addedLineInfos = Dict[p];
            List<AddedLineInfo> addedLineInfos = Dict[p].Concat(new List<AddedLineInfo>()).ToList();
            addedLineInfos.Reverse();
            foreach (AddedLineInfo ali in addedLineInfos)
            {   
                if (curOrder <= ali.order || id != ali.lineId)
                    continue;
                Line l = baseLineToCopy.Copy().SetField(0, ali.lineId).SetField(1, "_Compatability Buffer for " + ali.changeName + "_").addKW("!Buffer!", curOrder);
                p.InsertLine(l);
                return true;
            }
            return false;
        }
    }
    public static class NpcData
    {
        //Bosses vvvvvvvvvvvvvvvvvvvvvv
        static List<int> _bossOrMiniBossIds = new List<int>();
        static List<int> _multiBoss = new List<int>();
        static Dictionary<int,int> _bossOrMiniBossToItemLotMapDict = new Dictionary<int, int>();
        static Dictionary<int,int> _bossOrMiniBossToItemLotMapDict2 = new Dictionary<int, int>();
        public static List<int> BossOrMiniBossIds { get { if (_bossOrMiniBossIds == null && !setInfo) SetInfo(); return _bossOrMiniBossIds; }}
        public static List<int> MultiBoss { get { if (_multiBoss == null && !setInfo) SetInfo(); return _multiBoss; }}
        public static Dictionary<int, int> BossOrMiniBossToItemLotMapDict { get { if (_bossOrMiniBossToItemLotMapDict == null && !setInfo) SetInfo(); return _bossOrMiniBossToItemLotMapDict; } }
        public static Dictionary<int, int> BossOrMiniBossToItemLotMapDict2 { get { if (_bossOrMiniBossToItemLotMapDict2 == null && !setInfo) SetInfo(); return _bossOrMiniBossToItemLotMapDict2; } }
        public static void CheckDataSet() { if (!setInfo) SetInfo(); }
        static void SetBossInfo()
        {
            _bossOrMiniBossIds = new List<int>();
            _bossOrMiniBossIds = new int[]
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

            _multiBoss = new List<int>();
            _multiBoss = new int[] //unfinished list only used for one putpose
            {
43400940,// Mad Pumpkin Head (Flail)
43401940,// Mad Pumpkin Head (Hammer)
            }.ToList();
            if (_bossOrMiniBossIds.Count == 0)
            {   //only runs if list is empty, its not but if we want to upfdate the list, we leave it empty.

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
                    isBossExceptionCond = new Condition.FieldIs(0, Util.ToStrings(BossExceptionIDs));
                }
                //get group after npc id. create an exception that allows boses to sill be allowed if ni group/
                var isBossCondition = new Condition.TRUE().IsFalse;
                {
                    var BossSoulDropCond = new Condition.FieldIs(Program.NpcParam.GetFieldIndex("isSoulGetByBoss"), "1");
                    var BossSpEffectCond = new Condition.FieldIs(Program.NpcParam.GetFieldIndex("spEffectID31"), "4301");
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
                Line testline = Program.NpcParam.GetLineWithId(21000830);
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
                        var match = Regex.Match(line, npcRegexPattern);
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
                        match = Regex.Match(line, groupRegexPattern);
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
                    isDocumentedIDCond = new Condition.FieldIs(0, Util.ToStrings(isDocumentedIDs.ToArray()));
                    if (EXCLUDE_IN_GROUP)
                    {
                        List<int> SharesIDsWithBosses = new List<int>();
                        List<int> ShareAGroupIDs = new List<int>();
                        //Util.println(2 + " " + isBossCondition.Pass(testline).ToString());
                        foreach (int npcId in idToGroups.Keys)
                        {
                            //if (ShareAGroupIDs.Contains(npcId))
                            //    continue;
                            Line npcLine = Program.NpcParam.GetLineWithId(npcId);
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
                                    bool _isBoss = isBossCondition.Pass(Program.NpcParam.GetLineWithId(_npcId));
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
                        foreach(int id in SharesIDsWithBosses)
                        {
                            if (ignoreIsInGroupIDs.Contains(id))
                                continue;
                            _multiBoss.Add(id);
                        }
                        //Util.println(3 + " " + isBossCondition.Pass(testline).ToString());
                        //Util.println(2);
                        isGroupAppropriateCond = new Condition.FieldIs(0, Util.ToStrings(ignoreIsInGroupIDs.ToArray())).OR(
                            new Condition.FieldIs(0, Util.ToStrings(SharesIDsWithBosses.ToArray())).IsFalse
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

                var lines = (Lines)Program.NpcParam.GetLinesOnCondition(isGroupAppropriateBossCondition);
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



                _bossOrMiniBossIds = lines.GetIntFields(0).ToList();

                //-------if you want to print a bunch of lineIDs to out them into an Array..
                LineFunctions.PrintLineIDsAsToPutInArray(lines);
                Util.println("FOR EFFICIENCY copy and paste the following ids into the array which assigns to BossOrMiniBossIds ." +
                    " Then rerun the program. You can also comment out the return after this debug if you dont care about ineffeicency.");
                return;

            }

            _bossOrMiniBossToItemLotMapDict = new Dictionary<int, int>();
            _bossOrMiniBossToItemLotMapDict2 = new Dictionary<int, int>();

            //Preassign bosses to itemlotmap
            {
                //DLC
                //_bossOrMiniBossToItemLotMapDict.Add(21300534, 30820);    //Blackgoal Knight
                //_bossOrMiniBossToItemLotMapDict.Add(21300534, 20800);    //Chied Bloodfiend
                //_bossOrMiniBossToItemLotMapDict.Add(21300534, 20750);    //Demi-Human Swordmaster Onze
                //


                _bossOrMiniBossToItemLotMapDict.Add(21300534, 10040);    //Morgott
                _bossOrMiniBossToItemLotMapDict.Add(47210070, 10070);    //Hoarah Loux
                _bossOrMiniBossToItemLotMapDict.Add(21403050, 10740);    //Fell Twins
                _bossOrMiniBossToItemLotMapDict.Add(21403150, 10740);    //Fell Twins
                _bossOrMiniBossToItemLotMapDict.Add(71000030, 20080);    //Ancient Hero of Zamor (Sainted Hero's Grave)

                _bossOrMiniBossToItemLotMapDict.Add(31500010, 1043370400);    //Night's Cavalry (Repeating Thrust)
                _bossOrMiniBossToItemLotMapDict.Add(31500042, 1052410100);    //Night's Cavalry (BHS Drop)
                _bossOrMiniBossToItemLotMapDict.Add(31501040, 1049370100);    //Night's Cavalry (Poison Moth Flight Drop)
                _bossOrMiniBossToItemLotMapDict.Add(31500052, 1048550710);    //Night's Cavalry (Night's Cavalry Set + Ancient  Drop) Duo
                _bossOrMiniBossToItemLotMapDict.Add(31501052, 1048550710);    //Night's Cavalry (Night's Cavalry Set + Ancient  Drop) Duo
                _bossOrMiniBossToItemLotMapDict.Add(31500050, 1048510700);    //Night's Cavalry (Phantom Slash Drop)
                _bossOrMiniBossToItemLotMapDict.Add(31500020, 1036480400);    //Night's Cavalry (Giant's Hunt + Glaive Drop) //shares id
                _bossOrMiniBossToItemLotMapDict2.Add(31500020, 1039430400);    //Night's Cavalry (Ice Spear Drop)    //shares id.
                _bossOrMiniBossToItemLotMapDict.Add(31501030, 1039510200);    //Night's Cavalry (Shared Order Drop)
                _bossOrMiniBossToItemLotMapDict.Add(31501012, 1044320410);    //Night's Cavalry (Barricade Shield + Flail Drop)


                _bossOrMiniBossToItemLotMapDict.Add(25000010, 30120);    //crucible Knight (Stormhill) (Tail spell Drop)
                _bossOrMiniBossToItemLotMapDict.Add(25000014, 10001295);    //crucible Knight (Stormveil Castle) (Horn spell Drop)
                _bossOrMiniBossToItemLotMapDict.Add(25000165, 12020435);    //crucible Knight (Siofra Aquaduct) (Horn Shield Drop)
                _bossOrMiniBossToItemLotMapDict.Add(25009138, 16000950);    //tanith's crucible Knight (Volcano Manor) (Horn Shield Drop)

                _bossOrMiniBossToItemLotMapDict.Add(41300012, 1043340400);    //Demi-human Queen (Weeping peninsula) (Demi-human queen staff + Crystal Burst drop)
                _bossOrMiniBossToItemLotMapDict.Add(41301030, 1038510050);    //Demi-human Queen Gilika (Lux Ruins) (Ritual Sword Talisman staff drop)
                _bossOrMiniBossToItemLotMapDict.Add(41301932, 20400);    //Demi-human Queen Margot (Volcano Cave) (Jar Canon staff drop) // could be done auto
                _bossOrMiniBossToItemLotMapDict.Add(41300032, 30395);    //Demi-human Queen Maggie (Sorcerer Azur) (Memory Stone drop) // could be done auto
                //btw Gilika drops nothing if your wondering.


                _bossOrMiniBossToItemLotMapDict.Add(42700014, 10001085);    //Elder Lion (Stormveil Castle) (SSS+Beastblood+OldFang drop)
                _bossOrMiniBossToItemLotMapDict.Add(42700040, 1047380700);    //Elder Lion (Fort Gael) (Lion's Claw AOW drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict2.Add(42700040, 1049370700);    //Elder Lion (Caelid) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict.Add(42700041, 1051360700);    //Elder Lion (Redmane Castle) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict2.Add(42700041, 1051360800);    //Elder Lion (Redmane Castle) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict.Add(42700034, 11000195);    //Elder Lion (Lleyndel) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict.Add(42700030, 11000185);    //Elder Lion (Lleyndel Outskirts) (SSS+Beastblood+OldFang drop)    //shares id and dropId
                _bossOrMiniBossToItemLotMapDict.Add(42701051, 1051570800);    //Elder Lion (Castle Sol) (SSS+Beastblood+OldFang drop)
                _bossOrMiniBossToItemLotMapDict2.Add(42701051, 1051570810);    //Elder Lion (Castle Sol) (SSS+Beastblood+OldFang drop)
                //wiki has no information on other lion drops, assuming no drops for them.

                _bossOrMiniBossToItemLotMapDict.Add(32500072, 13002095);    //Draconic Tree Sentinel (Farum Azula) (Malformed Dragon Set Drop)
                _bossOrMiniBossToItemLotMapDict.Add(32500033, 30315);    //Draconic Tree Sentinel (Lleyndel Entrence) (Dragonclaw Shield + Dragon Greatclaw Drop)


                _bossOrMiniBossToItemLotMapDict.Add(49100032, 16002000);    //Magma Wyrm (Volcano Manor) (Dragon Heart)
                _bossOrMiniBossToItemLotMapDict.Add(49100038, 30390);    //Magma Wyrm (Lava Lake Boss) (Dragon Heart) //no sure which is the "real" ItemLotMap lot so ill set it to both.
                _bossOrMiniBossToItemLotMapDict2.Add(49100038, 30400);    //Magma Wyrm (Lava Lake Boss) (Dragon Heart) //no sure which is the "real" ItemLotMap lot so ill set it to both.


                _bossOrMiniBossToItemLotMapDict.Add(32510030, 30335);    // Tree Sentinel (Altus Plateua) (Erdtree Shield Drop)
                _bossOrMiniBossToItemLotMapDict.Add(32511030, 30335);    // Tree Sentinel - Torch (Altus Plateua) (Erdtree Shield Srop)

                _bossOrMiniBossToItemLotMapDict.Add(33001940, 1049390800);   //Nox Monk (Boss) (Nox Flowing Sword Drop)
                _bossOrMiniBossToItemLotMapDict.Add(33000940, 1049390800);   //Nox Swordstress (Boss) (Nox Flowing Sword Drop)

                _bossOrMiniBossToItemLotMapDict.Add(31000010, 1042380410);    //Bell Bearing Hunter (Limgrave) Bone Peddelers Bell Bearing
                _bossOrMiniBossToItemLotMapDict.Add(31000020, 1037460400);    //Bell Bearing Hunter (Church of Vows) Meat Peddlers Bell Bearing
                _bossOrMiniBossToItemLotMapDict.Add(31000033, 1043530400);    //Bell Bearing Hunter (Hermit's Shack) Medicine Peddlers Bell Bearing
                _bossOrMiniBossToItemLotMapDict.Add(31000042, 1048410800);    //Bell Bearing Hunter (Hermit's Shack) Medicine Peddlers Bell Bearing


                _bossOrMiniBossToItemLotMapDict.Add(71000012, 1042330100);    //Ancient Hero of Zamor (Weeping Peninsula Boss) Radagon's Scarseal

                _bossOrMiniBossToItemLotMapDict.Add(49801052, 1048570700);    //Death Rite Bird (Ordina, Liturgical Town) Explosive Ghostflame
                _bossOrMiniBossToItemLotMapDict.Add(49801040, 1049370110);    //Death Rite Bird (Southern Aeonia Swamp Bank) Death's Poker
                _bossOrMiniBossToItemLotMapDict.Add(49801020, 1036450400);    //Death Rite Bird (Liurnia of the Lakes - Gate Town Northwest) Ancient Death Rancor
                _bossOrMiniBossToItemLotMapDict.Add(49800020, 1037420400);    //Death Bird (Liurnia of the Lakes - Scenic Isle, Laskyar Ruins) Red-Feathered Branchsword
                _bossOrMiniBossToItemLotMapDict.Add(49800033, 1044530300);    //Death Bird (Capital Outskirts - Minor Erdtree) Twinbird Kiteshield
                _bossOrMiniBossToItemLotMapDict.Add(49800012, 1044320400);    //Death Bird (Weeping Peninsula - Castle Morne Approach North) Sacrificial Axe
                _bossOrMiniBossToItemLotMapDict.Add(49800010, 1042380400);    //Death Bird (Stormhill - Warmaster's Shack, Stormgate) Blue-Feathered Branchsword

                _bossOrMiniBossToItemLotMapDict.Add(48100150, 30525);    //Erdtree Avatar (Cerulean Crystal Tear Drop)

                _bossOrMiniBossToItemLotMapDict.Add(47700033, 1042510900);    //Gargoyle (Lleyndel Outskirts) (Gargoyle's Great Axe Drop)
                _bossOrMiniBossToItemLotMapDict.Add(47700250, 30505);    //Gargoyle (Rold Lift) (G's Black Blades + G's Black Axe Drop)
                _bossOrMiniBossToItemLotMapDict.Add(47701242, 30425);    //Gargoyle (Bestial Sanctum) (G's Blackblade + G's Black Halberd Drop)
                _bossOrMiniBossToItemLotMapDict.Add(47702034, 11001187);    //Gargoyle (Lleyndel) (Gargoyle's Halberd Drop)
                //BossOrMiniBossToItemLotMapDict.Add(47700070, );    //Gargoyle (Lleyndel Ashen Capital) // we accually want this one to drop since it respawns
                _bossOrMiniBossToItemLotMapDict.Add(47701165, 10100);    //Valiant Gargoyle Duo (Siofra River) (Gargoyle's Greatsword Drop)
                _bossOrMiniBossToItemLotMapDict.Add(47700165, 10100);    //Valiant Gargoyle Duo (Siofra River) (Gargoyle's Twinblade drop Drop)



                //47701034 documented in lleyndel, but only ones compared to 47701034 which is documented 3 times in lleyndel. im assuming is unused due to it having a weirld assigned itemLot_enemy unlike 47701034
                //47700034 is tagged fore lleyndel, undocumented
                //47702066 has item lot, undocumented


                _bossOrMiniBossToItemLotMapDict.Add(31811032, 20090);    //Red Wolf of the Champion (Floh Drop)
                _bossOrMiniBossToItemLotMapDict.Add(25001066, 12030950);    //Crucible Knight Siluria (Siluria Tree Drop)

                _bossOrMiniBossToItemLotMapDict.Add(44800912, 20300);    //Miranda the Blighted Bloom
                _bossOrMiniBossToItemLotMapDict.Add(48200930, 20410);    //Omenkiller (Boss Village) - duo fight
                _bossOrMiniBossToItemLotMapDict.Add(44800930, 20410);    //Miranda the Blighted Bloom - duo fight

                _bossOrMiniBossToItemLotMapDict.Add(523290066, 10350);    //Lionel the Lionhearted - Fia's Champions (Fia's Mist)

                _bossOrMiniBossToItemLotMapDict.Add(47500030, 1039500100);    //Godefroy the Grafted (Godfrey Icon Drop)
                _bossOrMiniBossToItemLotMapDict.Add(47200134, 101100);    //Godfrey, First Elden Lord (Phantom) (Talisman Pouch Drop)

                _bossOrMiniBossToItemLotMapDict.Add(35600042, 34110400);    //Godskin Apostle (Divine Tower of Caelid) (Godskin Apostle Set Drop)
                _bossOrMiniBossToItemLotMapDict.Add(37040940, 1049390850);  //Battlemage Hughes (Battlemage Hughes Ashes Drop)(Evergoal) (weirdly, the name and doc location points to diffrent things, just trust)
                _bossOrMiniBossToItemLotMapDict.Add(40200920, 1034480100);  //Royal Revenant (Frozen Needle Drop)

                _bossOrMiniBossToItemLotMapDict.Add(35500930, 1040530010);  //Sanguine Noble (Bloody Helice Drop)
                
                _bossOrMiniBossToItemLotMapDict.Add(46300912, 20310);  //Runebear (spelldrake talisman drop)
                _bossOrMiniBossToItemLotMapDict.Add(34510912, 20600);  //misbegotten (rusted anchor drop)
                _bossOrMiniBossToItemLotMapDict.Add(34600913, 10800);  //lionine misbegotten (grafted blade greatsword)


            }
        }
        //Bosses^^^^^^^^^^^^^^^
        //Enemies vvvvvvvvvvvv
        static List<int> _documentedNpcIDsList;
        static Dictionary<int, float> _npcsDocDifficultyDict;
        static Dictionary<int, List<Keyword>> _npcDocToLocationsDict;
        static Dictionary<int, int> _npcsIdToSpLevelsDict;
        static Dictionary<int, bool> _npcsIdToIsHumanspDict;
        static Dictionary<int, bool> _npcsIdToIsDLCspDict;
        static Dictionary<int, float> _spLevelToDifficultyDict;
        static Dictionary<int, float> _spLevelToStoneDifficultyDict;
        static bool setInfo = false;
        
        public static Dictionary<int, float> NpcIdsToDocDifficultyDict { get { if (_npcsDocDifficultyDict == null && !setInfo) SetInfo(); return _npcsDocDifficultyDict; } }
        public static Dictionary<int, int> NpcIdsToSpLevelsDict { get { if (_npcsIdToSpLevelsDict == null && !setInfo) SetInfo(); return _npcsIdToSpLevelsDict; } }
        public static Dictionary<int, bool> NpcIdsToIsHumanspDict { get { if (_npcsIdToIsHumanspDict == null && !setInfo) SetInfo(); return _npcsIdToIsHumanspDict; } } 
        public static Dictionary<int, bool> NpcIdsToIsDLCspDict { get { if (_npcsIdToIsDLCspDict == null && !setInfo) SetInfo(); return _npcsIdToIsDLCspDict; } }
        public static Dictionary<int, float> SpLevelToDifficultyDict { get { if (_spLevelToDifficultyDict == null && !setInfo) SetInfo(); return _spLevelToDifficultyDict; } }
        public static Dictionary<int, float> SpLevelToStoneDifficultyDict { get { if (_spLevelToStoneDifficultyDict == null && !setInfo) SetInfo(); return _spLevelToStoneDifficultyDict; } }
        public static List<int> DocumentedNpcIDsList { get { if (_documentedNpcIDsList == null && !setInfo) SetInfo(); return _documentedNpcIDsList; } }
        static void SetNpcDifficulty()
        {
            using (var sr = new StreamReader(@"C:\CODING\Souls Modding\ModdingTools\Docs\NPCLocations.txt"))
            {
                _documentedNpcIDsList = new List<int>();
                _npcsDocDifficultyDict = new Dictionary<int, float>();
                _npcDocToLocationsDict = new Dictionary<int, List<Keyword>>();

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
                var BossItemLotMapLines = Program.ItemLotParam_map.GetLinesOnCondition(
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


                    string[] caelidLocations =
                    "Caelid\nCaelem Ruins\nCaelid Catacombs\nCaelid Colosseum\nCaelid Waypoint Ruins\nCathedral of Dragon Communion\nChurch of the Plague\nDivine Tower of Caelid\nForsaken Ruins\nFort Gael\nGael Tunnel\nGaol Cave\nGowry's Shack\nMinor Erdtree (Caelid)\nMinor Erdtree Catacombs\nSellia Crystal Tunnel\nSellia Gateway\nSellia, Town of Sorcery\nShack of the Rotting\nSmoldering Church\nStreet of Sages Ruins\nSwamp Lookout Tower\nSwamp of Aeonia\nWailing Dunes\nWar-Dead Catacombs"
                        .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    allLocationGroups.Add(caelidLocations);
                    difficultyOfLocationDict.Add(caelidLocations, 4);

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
                    System.Text.RegularExpressions.Match match = Regex.Match(line, pattern);
                    if (!match.Success)
                    {
                        //Util.println("line:\n" + line + "\nfailed to find location name.");
                        continue;
                    }
                    var locationName = match.Groups[1].Value;

                    match = Regex.Match(line.Remove(0, line.IndexOf(")") + 1), pattern);    //gets second parrentheses that contains name.
                    if (!match.Success)
                    {
                        //Util.println("line:\n" + line + "\nfailed to find npc name.");
                        continue;
                    }
                    var npcname = match.Groups[1].Value;
                    //rename to Leyndell Margit to Morgott

                    pattern = @"npc (\d+)";
                    match = Regex.Match(line, pattern);
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
                    if (!_documentedNpcIDsList.Contains(npcID))
                        _documentedNpcIDsList.Add(npcID);

                    if (!_npcDocToLocationsDict.ContainsKey(npcID))
                        _npcDocToLocationsDict.Add(npcID, new List<Keyword>());
                    _npcDocToLocationsDict[npcID].Add(new Keyword("location: " + locationName, difficulty,false, true));


                    if (!_npcsDocDifficultyDict.ContainsKey(npcID))
                    {
                        if (difficulty != -1)
                            _npcsDocDifficultyDict.Add(npcID, difficulty);

                        //MAIN set bosses itemlotmap and data and stuff
                        if (BossOrMiniBossIds.Contains(npcID) && !BossOrMiniBossToDataDict.ContainsKey(npcID))
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
                            string npcLineName = Program.NpcParam.GetLineWithId(npcID).name;
                            int npcLineNameEndIndex = npcLineName.IndexOf("(");
                            string npcLineNameForImportantWords = npcLineName;
                            string npcLineNameExtraWords = "";
                            if (npcLineNameEndIndex > -1)
                            {
                                npcLineNameForImportantWords = npcLineName.Remove(npcLineNameEndIndex);
                                npcLineNameExtraWords = npcLineName.Remove(0, npcLineNameEndIndex);
                            }

                            importantWords = importantWords.Concat(npcLineNameForImportantWords.Split(LineFunctions.wordSplitters2, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToArray();
                            string[] targetWords = (locationName + " " + npcLineNameExtraWords).Split(LineFunctions.wordSplitters2, StringSplitOptions.RemoveEmptyEntries);
                            //targetWords = targetWords.Concat(importantWords).ToArray();
                            //if (npcID == 49100038)
                            //    Util.p();
                            string[] exclusions = new string[] { " of ", " the " };
                            var orderedLines = LineFunctions.GetOrderedWordMatchedLinesDict(targetWords, BossItemLotMapLines, out int maxMatchCount, out int curScore, 0, importantWords, false, false, exclusions,  "[", "]", " - ");

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
                                    if (ItemLotMapToAssignedDataDict.ContainsKey(favoriteId))
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
                                        ItemLotMapToAssignedDataDict[favoriteId] = data;

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
                                    ItemLotMapToAssignedDataDict.Add(favoriteId, data);

                                    //if (npcID == 49100038)
                                    //    Util.p();
                                    break;
                                }
                            }
                        }

                    }
                    else if (difficulty != -1 && _npcsDocDifficultyDict[npcID] != difficulty)
                    {
                        float oldDifficulty = _npcsDocDifficultyDict[npcID];
                        //Util.println(NpcParam.GetFieldWithLineID(1, npcID.ToString()) + " difficulty conflict: " + oldDifficulty + " and " + difficulty);

                        //prioritises the lower difficulty to avoid  enemies in the same region dropping the same shard.
                        _npcsDocDifficultyDict[npcID] = Math.Min(difficulty, oldDifficulty);
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
                foreach(int npcId in _npcDocToLocationsDict.Keys)
                {
                    var npcLine = Program.NpcParam.GetLineWithId(npcId);
                    foreach (Keyword k in _npcDocToLocationsDict[npcId])
                    {
                        npcLine.vanillaLine.addKeyword(k);
                    }
                }
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
                        var npcLine = Program.NpcParam.GetLineWithId(key);
                        var lotLine = Program.ItemLotParam_map.GetLineWithId(lotId);
                        string npcName = npcLine._idName;
                        string lotName = lotLine._idName;
                        string addLine = "BossOrMiniBossToItemLotMapDict.Add(" + npcLine.id + ", " + lotLine.id + ");    //" + npcLine.name;
                        Util.println(npcLine.id + " - " + lotLine.id + "  score: " + score + "  " + scoreRating + "\n" + addLine + "\n" + npcLine.name + "\n   " + data + "\n   " + lotLine.name + "\n");
                    }
                }


                //add remaining unassigned bosses (duo bosses)to maindict
                foreach (var id in BossOrMiniBossIds)
                {
                    if (!BossOrMiniBossToItemLotMapDict.Keys.Contains(id))
                    {
                        var npcLine = Program.NpcParam.GetLineWithId(id);
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
                        if (debugAssignMapLots)
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


            {
                _npcsIdToSpLevelsDict = new Dictionary<int, int>();
                _spLevelToDifficultyDict = new Dictionary<int, float>();
                _npcsIdToIsHumanspDict = new Dictionary<int, bool>();
                _npcsIdToIsDLCspDict = new Dictionary<int, bool>();
                //set spLevelToDifficyltyDict
                {
                    _spLevelToDifficultyDict.Add(1, 1); //stranded graveyard
                    _spLevelToDifficultyDict.Add(2, 1); //limgrave
                    _spLevelToDifficultyDict.Add(3, 1.3f); //weeping pennisula
                    _spLevelToDifficultyDict.Add(4, 2.15f); //stormveil castle
                    _spLevelToDifficultyDict.Add(5, 2.3f); //misc. limgrave
                    _spLevelToDifficultyDict.Add(6, 2.5f); //ainsel river
                    _spLevelToDifficultyDict.Add(7, 2.85f); //liurnia / caria manor / acadamy
                    _spLevelToDifficultyDict.Add(8, 3.2f); //caelid
                    _spLevelToDifficultyDict.Add(9, 3.5f); //altus
                    _spLevelToDifficultyDict.Add(10, 4.7f); //mt.gelmir / capital coutskirts
                    _spLevelToDifficultyDict.Add(11, 5.7f); //lleyndel / se caelid
                    _spLevelToDifficultyDict.Add(12, 6.5f); // volcano manor / deeproot depths / nokstella
                    _spLevelToDifficultyDict.Add(13, 7f); //mountaintops
                    _spLevelToDifficultyDict.Add(14, 7.5f); //moonlight altar / lleyndel sewer
                    _spLevelToDifficultyDict.Add(15, 8.6f); //farum azula
                    _spLevelToDifficultyDict.Add(16, 8.7f); //dragonbarrow /ashen capital
                    _spLevelToDifficultyDict.Add(17, 8.75f); //Concentrated Snowfields
                    _spLevelToDifficultyDict.Add(18, 8.8f); //Mohgwyn Palavce
                    _spLevelToDifficultyDict.Add(19, 8.85f); //Haligtree Roots
                    _spLevelToDifficultyDict.Add(20, 8.9f); //Elphael
                    _spLevelToDifficultyDict.Add(21, 9); //Malenia
                }
                _spLevelToStoneDifficultyDict = new Dictionary<int, float>();
                //set spLevelToDifficyltyDict
                {
                    _spLevelToStoneDifficultyDict.Add(1, 1); //stranded graveyard
                    _spLevelToStoneDifficultyDict.Add(2, 1); //limgrave
                    _spLevelToStoneDifficultyDict.Add(3, 1.3f); //weeping pennisula
                    _spLevelToStoneDifficultyDict.Add(4, 1.9f); //stormveil castle
                    _spLevelToStoneDifficultyDict.Add(5, 2f); //misc. limgrave
                    _spLevelToStoneDifficultyDict.Add(6, 3f); //ainsel river
                    _spLevelToStoneDifficultyDict.Add(7, 3f); //liurnia / caria manor / acadamy
                    _spLevelToStoneDifficultyDict.Add(8, 4f); //caelid
                    _spLevelToStoneDifficultyDict.Add(9, 4.75f); //altus
                    _spLevelToStoneDifficultyDict.Add(10, 4.3f); //mt.gelmir / capital coutskirts
                    _spLevelToStoneDifficultyDict.Add(11, 5f); //lleyndel / se caelid
                    _spLevelToStoneDifficultyDict.Add(12, 6f); // volcano manor / deeproot depths / nokstella
                    _spLevelToStoneDifficultyDict.Add(13, 7f); //mountaintops
                    _spLevelToStoneDifficultyDict.Add(14, 6f); //moonlight altar / lleyndel sewer
                    _spLevelToStoneDifficultyDict.Add(15, 11f); //farum azula
                    _spLevelToStoneDifficultyDict.Add(16, 6f); //dragonbarrow / ashen capital
                    _spLevelToStoneDifficultyDict.Add(17, 7.25f); //Concentrated Snowfields
                    _spLevelToStoneDifficultyDict.Add(18, 8f); //Mohgwyn Palavce
                    _spLevelToStoneDifficultyDict.Add(19, 9f); //Haligtree Roots
                    _spLevelToStoneDifficultyDict.Add(20, 9.2f); //Elphael
                    _spLevelToStoneDifficultyDict.Add(21, 9.2f); //Malenia
                }
                int agreers = 0;
                int disagreers = 0;
                foreach (Line npcLine in Program.NpcParam.lines)
                {
                    //if (npcsDifficultyDict2.ContainsKey(npcLine.id_int))
                    //    continue;
                    string locSpEffectID = npcLine.GetField("spEffectID3");
                    if (locSpEffectID == "0" || locSpEffectID == "-1")
                        continue;               //this seems to be npcs. we already excpetions.
                    bool isBoss = BossOrMiniBossIds.Contains(npcLine.id_int);
                    if (!isBoss && npcLine.GetField("getSoul") == "0")
                        continue;
                    var spLine = Program.SpEffectParam.GetLineWithId(locSpEffectID);
                    if (!spLine.name.Contains("Area Scaling"))
                        continue;
                    

                    bool isDLCSp = spLine.name.Contains("SOTE") || (spLine.name.Contains("Malenia") && !npcLine.name.Contains("Malenia"));
                    bool isHumanSp = false;
                    int mySpTier = -1;
                    isHumanSp = spLine.name.Contains("Human-NPC");
                    if (isDLCSp)
                        mySpTier = 21;
                    else
                    {
                        
                        if (!isHumanSp)
                            mySpTier = Util.GetFirstIntInString(spLine.name);
                    }
                    _npcsIdToIsHumanspDict.Add(npcLine.id_int, isHumanSp);
                    if(!isHumanSp)
                        _npcsIdToIsDLCspDict.Add(npcLine.id_int, isDLCSp);
                    if (mySpTier != -1)
                        _npcsIdToSpLevelsDict.Add(npcLine.id_int, mySpTier);
                    if (_npcsDocDifficultyDict.ContainsKey(npcLine.id_int) && mySpTier != -1)
                    {
                        if (_npcsDocDifficultyDict[npcLine.id_int] != _spLevelToDifficultyDict[mySpTier])
                        {
                            if (Math.Abs(_npcsDocDifficultyDict[npcLine.id_int] - _spLevelToDifficultyDict[mySpTier]) > 1.5 && Math.Abs(_npcsDocDifficultyDict[npcLine.id_int] - _spLevelToDifficultyDict[mySpTier]) < 2)
                                //Util.println("LEVELS DISAGREE " + npcLine._idName + "  spLevel:" + spLevelToDifficultyDict[mySpTier] + "  docLevel:" + npcsDocDifficultyDict[npcLine.id_int]);
                                disagreers++;
                        }
                        else
                            agreers++;
                    }
                }
            }

            //Util.println("agreers" + agreers + "   disagreeers:" + disagreers);
        }
        //Enemies ^^^^^^^^^^^^^

        static void SetInfo()
        {
            setInfo = true;
            SetBossInfo();
            SetNpcDifficulty();
        }
    }

    public static class FlagIds
    {
        //public static IntFilter.Single cumulateIds = new IntFilter.Multiple(IntFilter.Single(-1, -1), new IntFilter.Single(1,new IntFilter.DigitRange(0,1),-1), new IntFilter.Single(1, 2, IntFilter.DigitRange(0,8)));
        //public static List<int> usedCumulateNumFlagIds = SetUsedCumulateNumFlagIds();
        /*public static int lastCumulateNumFlagId = 0;
        public static int GetNextCulmulateNumFlagId()
        {
            lastCumulateNumFlagId++;
            while (usedCumulateNumFlagIds.Contains(lastCumulateNumFlagId))
            {
                lastCumulateNumFlagId++; 
            }
            return lastCumulateNumFlagId;
        }*/
        static int _getItemFlagId = Program.ItemLotParam_map.GetFieldIndex("getItemFlagId");
        public static Condition is10digitFlag = new Condition.FloatFieldBetween(_getItemFlagId, 1000000000, 2000000000);
        public static List<int> usedGetItemFlagId = GetUsedGetItemFlagIds(is10digitFlag, new ParamFile[] { Program.ItemLotParam_enemy, Program.ItemLotParam_map });
        public static List<int> GetUsedGetItemFlagIds(Condition condition, ParamFile[] ps)
        {
            
            List<int> _usedGetItemFlagId = new List<int>();
            foreach (ParamFile p in ps)
            {
                _usedGetItemFlagId = Util.ToInts(((Lines)p.GetLinesOnCondition(condition)).GetFields(_getItemFlagId)).Concat(_usedGetItemFlagId).ToList();
            }
            return _usedGetItemFlagId;
        }
        public static void ClearUsedFlagIds()
        {
            usedGetItemFlagId.Clear();
        }

        static List<int> SetUsedCumulateNumFlagIds()
        {
            int cumulateNumFlagIdFI = Program.ItemLotParam_map.GetFieldIndex("cumulateNumFlagId");
            var inIdRange = new Condition.FloatFieldBetween(cumulateNumFlagIdFI, 1000000000, 2000000000);
            List<int> _usedGetItemFlagId =
                Util.ToInts(((Lines)Program.ItemLotParam_enemy.GetLinesOnCondition(inIdRange)).GetFields(cumulateNumFlagIdFI))
                .Concat(
                    Util.ToInts(((Lines)Program.ItemLotParam_map.GetLinesOnCondition(inIdRange)).GetFields(cumulateNumFlagIdFI))).ToList();
            return _usedGetItemFlagId;
        }


    }

    public static class ItemLot
    {
        public static Lines getItemLotLines(ParamFile file, int id, int startIndex = 0)
        {
            return getItemLotLines(file, id, out int nextLineIndex, startIndex);
        }
        public static Lines getItemLotLines(ParamFile file, int id, out int nextLineIndex, int startIndex = 0)
        {
            List<Line> ret = new List<Line>();
            bool inclusive = true;
            if (inclusive)
                id--;

            var lines = file.lines;

            int last_iid = 0;

            nextLineIndex = -1;

            for (int i = startIndex; true; i++)
            {
                int iid = lines[i].id_int;
                if (last_iid < iid - 1)
                {
                    if (iid >= id)
                    {
                        nextLineIndex = i;
                        return ret;
                    }
                    else
                        ret.Clear();
                }
                ret.Add(lines[i]);
                last_iid = iid;
            }
            return (Lines)ret;
        }
        public static Lines getItemLotLines(Line line, int startIndex)
        {
            return getItemLotLines(line.file, line.id_int, startIndex);
        }
        public static Lines getItemLotLines(Line line, out int nextLineIndex, int startIndex)
        {
            return getItemLotLines(line.file, line.id_int, out nextLineIndex, startIndex);
        }

        
        public static void CopyItemLotAt(Line itemLotLine, int copyAt = -1)
        {
            Lines itemLotLines = getItemLotLines(itemLotLine.file, itemLotLine.id_int);
            CopyItemLotAt(itemLotLines, copyAt);
        }
        public static void CopyItemLotAt(ParamFile file, int itemLotId, int copyAt = -1)
        {
            Lines itemLotLines = getItemLotLines(file, itemLotId);
            CopyItemLotAt(itemLotLines, copyAt);
        }
        public static void CopyItemLotAt(List<Line> itemLotLines, int copyAt = -1)
        {
            CopyItemLotAt((Lines)itemLotLines, copyAt);
        }
        public static void CopyItemLotAt(Lines itemLotLines, int copyAt = -1)
        {
            
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
                Lines lines = Program.ItemLotParam_map.GetLinesOnCondition(new LotItem.HasLotItem(lotItemGroup, 1, 1, true));
                for (int i = 0; i < lotItemGroup.Length; i++)
                {
                    int lotIndex = i + 1;
                    var setItemLotOperation = new LotItem.SetLotItem(lotItemGroup[i], lotIndex);
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
