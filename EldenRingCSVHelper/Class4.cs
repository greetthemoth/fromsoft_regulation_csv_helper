using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenRingCSVHelper
{

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
