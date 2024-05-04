using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingCSVHelper
{
    public class LotItem
    {

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
        public int amount = 1;
        public int chance = 1000;
        public bool affectByLuck = false;

        static int[] idFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemId");
        static int[] categoryFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemCategory");
        static int[] amountFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemNum");
        static int[] chanceFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("lotItemBasePoint");
        static int[] affectByLuckFIs = Program.ItemLotParam_enemy.GetFieldIndexesContains("enableLuck");

        public void SetLotItemToLine(Line line, int lotIndex = 1)
        {
            line.SetField(idFIs[lotIndex - 1], id);
            line.SetField(categoryFIs[lotIndex - 1], category);
            line.SetField(amountFIs[lotIndex - 1], amount);
            line.SetField(chanceFIs[lotIndex - 1], chance);
            line.SetField(affectByLuckFIs[lotIndex - 1], affectByLuck);
        }

        public bool LineHasLotItem(Line line, bool sharesLotInfo)
        {
            for(int lotIndex = 1; lotIndex <= idFIs.Length; lotIndex++)
            {
                LotHasLotItem(line, lotIndex, sharesLotInfo);
            }
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

        public LotItem(int category, int id, int amount = 1, int chance = 1000, bool affectByLuck = false)
        {
            this.category = category;
            this.id = id;
            this.amount = amount;
            this.chance = chance;
            this.affectByLuck = affectByLuck;
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
    }
}
