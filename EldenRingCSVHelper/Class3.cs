using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace EldenRingCSVHelper
{
    public static class Util
    {
        public static void PrintStrings(string[] outputLines)
        {
            foreach (string outputLine in outputLines)
            {
                println(outputLine);
            }
        }
        public static void PrintStrings(string[][] outputLines)
        {
            foreach (string[] outputLiness in outputLines)
            {
                foreach (string outputLine in outputLiness)
                {
                    println(outputLine);
                }
            }
        }
        public static void PrintStrings(string[,] outputLines)
        {
            foreach (string outputLine in outputLines)
            {
                println(outputLine);
            }
        }
        public static void println(string outputLine)
        {
            if (!RunSettings.PrintOnConsole)
                Debug.WriteLine(outputLine);
            else
                Console.WriteLine(outputLine);
        }
        public static void println()
        {
            println("");
        }

        public static string IndentedText(string text, int spaces = 12, char OptionalSpecialCharacter = ' ', char space_char = ' ' )
        {
            string indent = "";
            for (int i = 0; i < spaces; i++)
            {
                indent += space_char;
            }
            string specialCharacterString = "";
            if(OptionalSpecialCharacter != ' ')
                specialCharacterString += OptionalSpecialCharacter;
            return text + specialCharacterString + indent.Remove(0, Math.Min(indent.Length, text.Length));
        }

        public static void println(int outputLine)
        {
            println(outputLine.ToString());
        }
        public static void p(string s = "")
        {

        }
        public static int[] ToInts(string[] intStrings)
        {
            int[] ints = new int[intStrings.Length];
            for (int i = 0; i < intStrings.Length; i++)
            {
                ints[i] = int.Parse(intStrings[i]);
            }
            return ints;
        }
        public static string[] ToStrings(int[] ints)
        {
            string[] strings = new string[ints.Length];
            for (int i = 0; i < ints.Length; i++)
            {
                strings[i] = ints[i].ToString();
            }
            return strings;
        }
        public static string[] GetBoolsConditionPass(List<Line> lines, Condition condition)
        {
            var bools = new string[lines.Count];
            for (int i = 0; i < bools.Length; i++)
            {
                if (condition.Pass(lines[i]))
                    bools[i] = true.ToString();
                else
                    bools[i] = false.ToString();
            }
            return bools;
        }

        //APPEND string to string
        public static string Append(string a,string middle, string b)
        {
            return a + middle + b;
        }
        public static string Append(string a, string b)
        {
            return a + b;
        }

        public static string AppendToString(string[] a, string b = "")
        {
            string s = "";
            for (int i = 0; i < a.Length; i++)
            {
                s += a[i] + b;
            }
            return s;
        }
        //APPEND [] to string
        static string[] Append(string[] a, string b, bool reverseOrder)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (reverseOrder)
                    a[i] = b + a[i];
                else
                    a[i] += b;
            }
            return a;
        }
        public static string[] Append(string[] a, string b)
        {
            return Append(a, b, false);
        }
        public static string[] Append(string a, string[] b)
        {
            return Append(b, a, true);
        }

        //APPEND [,] to string
        static string[,] Append(string[,] a, string b, bool reverseOrder)
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int x = 0; x < a.GetLength(1); x++)
                {
                    if (reverseOrder)
                        a[i, x] = b + a[i, x];
                    else
                        a[i, x] += b;
                }
            }
            return a;
        }
        public static string[,] Append(string[,] a, string b)
        {
            return Append(a, b, false);
        }
        public static string[,] Append(string a, string[,] b)
        {
            return Append(b, a, true);
        }

        //APPEND [,] to [,]
        public static string[,] Append(string[,] a, string middle, string[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
            {
                println("COULD NOT APPEND STRINGS [" + a.GetLength(0) + "," + a.GetLength(1) + "] to [" + b.GetLength(0) + "," + b.GetLength(1) + "]");
                return null;
            }
            var newStrings = new string[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int x = 0; x < a.GetLength(1); x++)
                {
                    newStrings[i,x] = a[i,x] + middle + b[i,x];
                }
            }
            return newStrings;
        }
        public static string[,] Append(string[,] a, string[,] b)
        {
            return Append(a, "", b);
        }


        //APPEND [,] to [][]
        static string[,] Append(string[,] a, string middle, string[][] b, bool reverseOrder)
        {
            if (b != null)
            {
                for (int i = 0; i < b.Length; i++)
                {
                    if (reverseOrder)
                        a = Append(b[i], middle, a);
                    else
                        a = Append(a, middle, b[i]);
                    if (a == null)
                    {
                        if (reverseOrder)
                            println("COULD NOT APPEND STRINGS [" + b[i].Length + "] to [" + a.GetLength(0) + "," + a.GetLength(1) + "]");
                        else
                            println("COULD NOT APPEND STRINGS [" + a.GetLength(0) + "," + a.GetLength(1) + "] to [" + b[i].Length + "]");
                        return null;
                    }
                }
            }
            return a;
        }
        public static string[,] Append(string[,] a, string middle, string[][] b)
        {
            return Append(a, middle, b, false);
        }
        public static string[,] Append(string[][] a, string middle, string[,] b)
        {
            return Append(b, middle, a, true);
        }
        public static string[,] Append(string[,] a, string[][] b)
        {
            return Append(a,"", b, false);
        }
        public static string[,] Append(string[][] a, string[,] b)
        {
            return Append(b,"", a, true);
        }

        //APPEND [,] to []
        static string[,] Append(string[,] a, string middle, string[] b, bool reverseOrder)
        {
            bool FieldnamesIsField;
            if (a.GetLength(0) == b.Length)
            {
                FieldnamesIsField = true;
            }
            else if (a.GetLength(1) == b.Length)
            {
                FieldnamesIsField = false;
            }
            else
            {
                if(reverseOrder)
                    println("COULD NOT APPEND STRINGS ["+b.Length+ "] to [" + a.GetLength(0) + "," + a.GetLength(1) + "]");
                else
                    println("COULD NOT APPEND STRINGS [" + a.GetLength(0) + "," + a.GetLength(1) + "] to [" + b.Length + "]");
                return null;
            }
            var newStrings = new string[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int x = 0; x < a.GetLength(1); x++)
                {
                    if (reverseOrder)
                    {
                        if (FieldnamesIsField)
                            newStrings[i, x] = b[i] + middle + a[i, x];
                        else
                            newStrings[i, x] = b[x] + middle + a[i, x];
                    }
                    else
                    {
                        if (FieldnamesIsField)
                            newStrings[i, x] = a[i, x] + middle + b[i];
                        else
                            newStrings[i, x] = a[i, x] + middle + b[x];
                    }

                }
            }
            return newStrings;
        }
        public static string[,] Append(string[,] a, string middle, string[] b)
        {
            return Append(a, middle, b, false);
        }
        public static string[,] Append(string[] a, string middle, string[,] b)
        {
            return Append(b, middle, a, true);
        }
        public static string[,] Append(string[,] a, string[] b)
        {
            return Append(a, "", b, false);
        }
        public static string[,] Append(string[] a, string[,] b)
        {
            return Append(b, "", a, true);
        }
        
        //APPEND [] to []
        public static string[] Append(string[] a, string middle, string[] b)
        {
            if (a.Length != b.Length)
            {

                println("COULD NOT APPEND STRINGS [" + a.Length+"] to [" + b.Length + "]");
                return null;
            }
            var newStrings = new string[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                newStrings[i] = a[i] + middle + b[i];
            }
            return newStrings;
        }
        public static string[] Append(string[] a, string[] b)
        {
            return Append(a, "", b);
        }


        //To String Array
        public static string[] ToStringArray(string[] strings, string excludeWithTerm = null)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < strings.Length; i++)
            {
                if (excludeWithTerm == null || !ArrayContainsStrings(strings[i].Split(' '), excludeWithTerm))
                    ret.Add(strings[i]);
            }
            return ret.ToArray();

        }
        public static string[] ToStringArray(string[] strings, int[] indexes, string excludeWithTerm = null)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < strings.Length; i++)
            {
                for (int ii = 0; ii < indexes.Length; ii++)
                {

                    if (indexes[ii] == i && (excludeWithTerm == null || !ArrayContainsStrings(strings[i].Split(' '), excludeWithTerm)) )
                        ret.Add(strings[i]);
                }
            }
            return ret.ToArray();

        }
        public static string[] ToStringArray(string[,] strings, bool gridded = true, string excludeWithTerm = null)
        {
            List<string> ll = new List<string>();
            int size0, size1;
            if (gridded)
            {
                size1 = strings.GetLength(0);
                size0 = strings.GetLength(1);
            }
            else
            {
                size0 = strings.GetLength(0);
                size1 = strings.GetLength(1);
            }

            for (int i = 0; i < size0; i++)
            {
                for (int x = 0; x < size1; x++)
                {
                    int i0, i1;
                    if (gridded)
                    {
                        i1 = i;
                        i0 = x;
                    }
                    else
                    {
                        i0 = i;
                        i1 = x;
                    }

                    if (excludeWithTerm == null || !ArrayContainsStrings( strings[i0, i1].Split(' '), excludeWithTerm))
                        ll.Add(strings[i0, i1]);
                }
            }
            return ll.ToArray();
        }
        public static string[] ToStringArray(string[] strings, string[] excludeWithTerm = null)
        {
            List<string> ret = new List<string>();
            for (int i = 0; i < strings.Length; i++)
            {
                if (excludeWithTerm == null || !ArrayContainsStrings(strings[i].Split(' '), excludeWithTerm))
                    ret.Add(strings[i]);
            }
            return ret.ToArray();

        }
        public static string[] ToStringArray(string[,] strings, bool gridded = true, string[] excludeWithTerm = null)
        {
            List<string> ll = new List<string>();
            int size0, size1;
            if (gridded)
            {
                size1 = strings.GetLength(0);
                size0 = strings.GetLength(1);
            }
            else
            {
                size0 = strings.GetLength(0);
                size1 = strings.GetLength(1);
            }

            for (int i = 0; i < size0; i++)
            {
                for (int x = 0; x < size1; x++)
                {
                    int i0, i1;
                    if (gridded)
                    {
                        i1 = i;
                        i0 = x;
                    }
                    else
                    {
                        i0 = i;
                        i1 = x;
                    }

                    if (excludeWithTerm == null || !ArrayContainsStrings(strings[i0, i1].Split(' '), excludeWithTerm))
                        ll.Add(strings[i0, i1]);
                }
            }
            return ll.ToArray();
        }

        public static bool ArrayContainsStrings(string[] strings, string has)
        {
            foreach (string s in strings)
            {
                if (s == has)
                    return true;
            }
            return false;
        }
        public static bool ArrayContainsStrings(string[] strings, string[] has)
        {
            foreach (string s in strings)
            {
                foreach (string h in has)
                {
                    if (s == h)
                        return true;
                }
            }
            return false;
        }

        public static string[,] GridifyStrings(string[][] strings)
        {
            var grid = new string[strings.Length,strings[1].Length];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                if (strings[i].Length != grid.GetLength(1))
                {
                    println("CANT GRIDIFY STRINGS +["+strings.Length+"]+[" + strings[i].Length + "] to [" + grid.GetLength(0) + "," + grid.GetLength(1) + "]");
                    return null;
                }
                for (int x = 0; x < grid.GetLength(1); x++)
                {
                    grid[i, x] = strings[i][x];
                }
            }
            return grid;
        }

        //Append(Fieldnames, before, Append(Append(first, middle, last), end))
        public static void PrintFieldsCompare(string[][] Fieldnames, string before, string[,] first, string middle, string[,] last, string end = "", string remove = null)
        {
            var main = Append(first, middle, last);
            main = Append(main, end);
            main = Append(Fieldnames, before, main);
            PrintStrings(ToStringArray(main,true,remove.Split(' ')));
        }
        public static void PrintFieldsCompare(string[] Fieldnames, string before, string[,] first, string middle, string[,] last, string end = "", string remove = null)
        {
            PrintFieldsCompare(new string[1][] { Fieldnames }, before, first, middle, last, end, remove);
        }
        public static void PrintFieldsCompare(string[] Fieldnames, string[,] first, string[,] last, string end = "", string remove = null)
        {
            PrintFieldsCompare(Fieldnames, " ", first, " ", last, end,remove);
        }
        public static void PrintFieldsCompare(string[,] first, string[,] last, string end = "", string remove = null)
        {
            PrintFieldsCompare((string[][])null, "", first, " ", last, end,remove);
        }
        public static void PrintFieldsCompare(string[,] first, string middle, string[,] last, string end = "", string remove = null)
        {
            PrintFieldsCompare((string[][])null, "", first, middle, last, end, remove);
        }
        public static void PrintFieldsCompare(string before, string[,] first, string middle, string[,] last, string end = "", string remove = null)
        {
            PrintFieldsCompare((string[][])null, before, first, middle, last, end, remove);
        }
        public static void PrintStrings(string[] Fieldnames, string before, string[] first, string middle, string[] last, string end = "", string remove = null)
        {
            for(int i = 0; i < first.Length; i++)
            {
                if (remove != null && (remove == first[i] || remove == last[i]))
                    continue;
                var name = "";
                if (Fieldnames == null)
                    name = Fieldnames[i];
                println(name + before + first[i] + middle + last[i] + end);
            }
        }
        public static void PrintStrings(string[] Fieldnames, string[] first, string middle, string[] last, string end = "", string remove = null)
        {
            PrintStrings(Fieldnames, "", first, middle, last, end);
        }
        public static void PrintStrings(string[] Fieldnames, string[] first, string[] last, string end = "", string remove = null)
        {
            PrintStrings(Fieldnames, "", first, " ", last, end);
        }
        public static void PrintStrings(string[] first, string middle, string[] last, string end = "", string remove = null)
        {
            PrintStrings(null,"", first, middle, last, end);
        }
        public static void PrintStrings(string[] first, string[] last, string end = "", string remove = null)
        {
            PrintStrings(null,"", first, " ", last, end);
        }

        public static float FloatTo3DecimalPlaces(float f, bool round = true)
        {
            f = (f * 1000);
            if (round)
                f += 0.5f;
            return (((int)f) / 1000f);
        }
        public static int GetFirstIntInString(string s)
        {
            string pattern = @"\d+";
            Match match = Regex.Match(s, pattern);
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            return -1;
        }

        public static string Debug_LastlWordsTogether;
        public static string Debug_LasttargetWordsTogether;
        public static string Debug_LastintersectTogether;


        public static int WordMatchScore(string s, string[] targetWords, out int matchCount, int minMatchCount = 0, string[] importantWords = null, bool deprioritizeExtraWords = false)
        {
            string[] lWords = s.Split(LineFunctions.wordSplitters, StringSplitOptions.RemoveEmptyEntries);

            
            

            int score = 0;
            var intersect = lWords.Intersect(targetWords, StringComparer.OrdinalIgnoreCase);
            matchCount = intersect.Count();

            //debug
             Debug_LastlWordsTogether = String.Join(" ", lWords);
             Debug_LasttargetWordsTogether = String.Join(" ", targetWords);
             Debug_LastintersectTogether = String.Join(" ", intersect);
            //

            int importantMatchCount = matchCount;
            if (importantWords != null)
                importantMatchCount = lWords.Intersect(importantWords, StringComparer.OrdinalIgnoreCase).Count();


            int nonImportantMatchCount = matchCount - importantMatchCount;

            score = (importantMatchCount * 10) + nonImportantMatchCount;

            if (deprioritizeExtraWords && lWords.Length > targetWords.Length)
            {
                int extraWords = lWords.Length - targetWords.Length;
                score -= extraWords;
            }
            if (matchCount < minMatchCount)
                score = -1;

            return score;
        }

    }
    public class LineFunctions
    {

        public static void CompareLines(Lines a, Lines b)
        {
            string s = "\n";
            List<string> ss = new List<string>();
            int i = 0;
            foreach (Line l in a.lines)
            {
                if (!b.Contains(l))
                {
                    i++;
                    //lines removed with new exception.
                    ss.Add("A lines NOT in B:" + l._idName + "\n");
                }
            }
            if (i == a.Length) 
            {
                s = "\nAll " + i + " A lines NOT in B.\n";
            Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            else
            {
                s += i + " A lines NOT in B.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            ss.Clear();
            s = "\n";
            i = 0;
            foreach (Line l in a.lines)
            {
                if (b.Contains(l))
                {
                    i++;
                    ss.Add("A line in B:    " + l._idName);
                }
            }
            if (i == a.Length)
            {
                s = "\nAll " + i + " A lines in B.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            else
            {
                s += i + " A lines in B.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            ss.Clear();
            s = "\n";
            i = 0;
            foreach (Line l in b.lines)
            {
                if (!a.Contains(l))
                {
                    i++;
                    ss.Add("B line NOT in A:" + l._idName);
                }
            }
            if (i == a.Length)
            {
                s = "\nAll " + i + " B lines NOT in A.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            else
            {
                s += i + " B lines NOT in A.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            ss.Clear();
            s = "\n";
            i = 0;
            foreach (Line l in b.lines)
            {
                if (a.Contains(l))
                {
                    i++;
                    ss.Add("B line in A:    " + l._idName);
                }
            }
            if (i == a.Length)
            {
                s = "\nAll " + i + " B lines in A.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
            else
            {
                s += i + " B lines in A.\n";
                Util.println(s);
                Util.PrintStrings(ss.ToArray());
            }
        }
        public static char[] wordSplitters = new[] { ' ', ']', '[', '(', ')', '!', '-' };
        public static char[] wordSplitters2 = new[] { ' ', ']', '[', '(', ')', '!', '-', ',' };
        public static List<Line> GetBestWordMatchedLines(string[] targetWords, List<Line> lines, out int maxMatchCount, int minMatchCount = 0)
        {
            List<Line> bestLines = new List<Line>();
            maxMatchCount = minMatchCount;
            //Util.PrintStrings(Util.Append("targetWord: ",targetWords));
            foreach (Line l in lines)
            {
                string[] lWords = l.name.Split(wordSplitters, StringSplitOptions.RemoveEmptyEntries);
               // Util.PrintStrings(Util.Append("lWords: ", lWords));
                int matchCount = lWords.Intersect(targetWords, StringComparer.OrdinalIgnoreCase).Count();
               // Util.println(Util.AppendToString(lWords,"-") +"   "+matchCount);
                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = matchCount;
                    bestLines.Clear();
                    bestLines.Add(l);
                }
                else if (matchCount == maxMatchCount)
                {
                    bestLines.Add(l);
                }
            }
            return bestLines;
        }

        public static List<Line> GetBestWordMatchedLinesCapped(string[] targetWords, List<Line> lines, out int maxMatchCount, int maxCap, int minMatchCount = 0)
        {
            List<Line> bestLines = new List<Line>();
            maxMatchCount = minMatchCount;
            foreach (Line l in lines)
            {
                string[] lWords = l.name.Split(wordSplitters, StringSplitOptions.RemoveEmptyEntries);
                int matchCount = lWords.Intersect(targetWords, StringComparer.OrdinalIgnoreCase).Count();
                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = Math.Min(matchCount, maxCap);
                    bestLines.Clear();
                    bestLines.Add(l);
                }
                else if (matchCount == maxMatchCount)
                {
                    bestLines.Add(l);
                }
            }
            return bestLines;
        }


        public static string Debug_LastBestlWordsTogether;
        public static string Debug_LastBesttargetWordsTogether;
        public static string Debug_LastBestintersectTogether;
        public static string Debug_LastBestlWordsTogether2;
        public static string Debug_LastBesttargetWordsTogether2;
        public static string Debug_LastBestintersectTogether2;
        public static Dictionary<int, List<Line>> GetOrderedWordMatchedLinesDict(string[] targetWords, List<Line> lines, out int maxMatchCount, out int maxScore, int minMatchCount = 0, string[] importantWords = null, bool deprioritizeExtraWords = false, string[] exclusionTerms = null, string lineNameStart = "", string lineNameCutOff = "", string lineNameImportantPartStart = "", string lineNameImportantPartEnd = "")
        {
            string Debug_LastBestlWordsTogether2Temp = "";
            string Debug_LastBesttargetWordsTogether2Temp = "";
            string Debug_LastBestintersectTogether2Temp = "";

            Dictionary<int, List<Line>> orderedLinesDict = new Dictionary<int, List<Line>>();
            maxMatchCount = 0;
            maxScore = 0;

            foreach (Line l in lines)
            {
                string target = l.name;
                if (lineNameStart != "")
                {
                    int targetStart = target.IndexOf(lineNameStart);
                    if (targetStart == -1)
                        continue;
                    target = target.Remove(0,targetStart);
                }
                if (lineNameCutOff != "")
                {
                    int targetEnd = target.IndexOf(lineNameCutOff);
                    if(targetEnd != -1)
                        target = target.Remove(targetEnd);
                }
                if (exclusionTerms != null)
                {
                    foreach (string exclusion in exclusionTerms)
                    {
                        target = target.Replace(exclusion, " ");
                    }
                }
                int matchCount;
                int score;
                if (lineNameImportantPartStart != "")
                {
                    string importantPart = target;
                    if (lineNameImportantPartStart != "")
                    {
                        int targetStart = target.IndexOf(lineNameImportantPartStart);
                        if (targetStart == -1)
                            targetStart = 0;
                        importantPart = importantPart.Remove(0, targetStart);
                    }
                    if (lineNameImportantPartEnd != "")
                    {
                        int targetEnd = importantPart.IndexOf(lineNameImportantPartEnd);
                        if (targetEnd != -1)
                            importantPart = importantPart.Remove(targetEnd);
                    }
                    score = Util.WordMatchScore(target.Replace(importantPart, " "), targetWords, out matchCount, minMatchCount, null, deprioritizeExtraWords);
                    Debug_LastBestintersectTogether2Temp = Util.Debug_LastintersectTogether;
                    Debug_LastBestlWordsTogether2Temp = Util.Debug_LastlWordsTogether;
                    Debug_LastBesttargetWordsTogether2Temp = Util.Debug_LasttargetWordsTogether;
                    score += Util.WordMatchScore(importantPart, importantWords, out int matchCount2, minMatchCount, importantWords, deprioritizeExtraWords);
                    matchCount += matchCount2;
                }
                else
                {
                    score = Util.WordMatchScore(target, targetWords, out matchCount, minMatchCount, importantWords, deprioritizeExtraWords);
                }

                /*if (target.Contains("Night"))
                {
                    var targetWordsAll = String.Join(" ", targetWords);
                    Util.p();
                }*/
                if (score > maxScore)
                {
                    maxScore = score;
                    Debug_LastBestintersectTogether = Util.Debug_LastintersectTogether;
                    Debug_LastBestlWordsTogether = Util.Debug_LastlWordsTogether;
                    Debug_LastBesttargetWordsTogether = Util.Debug_LasttargetWordsTogether;
                    Debug_LastBestintersectTogether2 = Debug_LastBestintersectTogether2Temp;
                    Debug_LastBestlWordsTogether2 = Debug_LastBestlWordsTogether2Temp;
                    Debug_LastBesttargetWordsTogether2 = Debug_LastBesttargetWordsTogether2Temp;
                }

                if (matchCount < minMatchCount)
                    continue;
                
                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = matchCount;
                }

                if (!orderedLinesDict.ContainsKey(score))
                {
                    orderedLinesDict.Add(score, new List<Line>());
                }
                orderedLinesDict[score].Add(l);
            }
            
            return orderedLinesDict;
        }
        public static List<Line> GetOrderedWordMatchedLines(string[] targetWords, List<Line> lines, out int maxMatchCount, out int maxScore, int minMatchCount = 0, string[]importantWords = null, bool deprioritizeExtraWords = false, string[] exclusionTerms = null, string lineNameStart = "", string lineNameCutOff = "")
        {
            var orderedLinesDict = GetOrderedWordMatchedLinesDict(targetWords, lines, out maxMatchCount, out maxScore, minMatchCount, importantWords, deprioritizeExtraWords, exclusionTerms, lineNameStart, lineNameCutOff);

            List<Line> orderedLineList = new List<Line>();

            for (int i = maxScore; i >= 0 ; i--)
            {
                if (orderedLinesDict.ContainsKey(i))
                {
                    var ls = orderedLinesDict[i];
                    foreach (Line l in ls)
                    {
                        orderedLineList.Add(l);
                    }
                }
            }
            return orderedLineList;
        }

        public static List<List<Line>> GetOrderedWordMatchedNestedLines(string[] targetWords, List<Line> lines, out int maxMatchCount, out int maxScore, int minMatchCount = 0, string[] importantWords = null, bool deprioritizeExtraWords = false, string[] exclusionTerms = null, string lineNameStart = "", string lineNameCutOff = "")
        {
            var orderedLinesDict = GetOrderedWordMatchedLinesDict(targetWords, lines, out maxMatchCount, out maxScore, minMatchCount, importantWords, deprioritizeExtraWords,exclusionTerms, lineNameStart, lineNameCutOff);

            List<List<Line>> orderedLineNestedList = new List<List<Line>>();


            for (int i = maxScore; i >= 0; i--)
            {

                if (orderedLinesDict.ContainsKey(i))
                {
                    orderedLineNestedList.Add(orderedLinesDict[i]);
                }
            }
            return orderedLineNestedList;
        }

        public static void PrintLineIDsAsToPutInArray(Lines lines)
        {
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(lines.GetFields(0), "")), ",// ", lines.GetFields(1)));
        }
        public static void PrintLineIDsAsToPutInArray(List<Line> lines)
        {
            Lines _lines = lines;
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(_lines.GetFields(0), "")), ",// ", _lines.GetFields(1)));
        }
        public static void PrintLineFieldssAsToPutInArray(Lines lines, int Field)
        {
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(lines.GetFields(Field), "")), ",// ", lines.GetFields(1)));
        }
        public static void PrintLineFieldsAsToPutInArray(List<Line> lines, int Field)
        {
            Lines _lines = lines;
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(_lines.GetFields(Field), "")), ",// ", _lines.GetFields(1)));
        }
        public static void PrintLineFieldssAsToPutInArray(Lines lines, string Field_name)
        {
            int int_Field = lines.lines[0].file.GetFieldIndex(Field_name);
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(lines.GetFields(int_Field), "")), ",// ", lines.GetFields(1)));
        }
        public static void PrintLineFieldsAsToPutInArray(List<Line> lines, string Field_name)
        {
            int int_Field = lines[0].file.GetFieldIndex(Field_name);
            Lines _lines = lines;
            Util.PrintStrings(Util.Append(Util.Append("", Util.Append(_lines.GetFields(int_Field), "")), ",// ", _lines.GetFields(1)));
        }
    }

    public abstract class IntFilter
    {
        public static DigitRange Digit(int min, int max)
        {
            return new DigitRange(min, max);
        }
        public class DigitRange: IntFilter
        {
            public static implicit operator DigitRange(int num)
            {
                if (num <= -1)
                    return new DigitRange(0, 9);
                return new DigitRange(num, num);
            }
            public int minNum = 0;
            public int maxNum = 9;
            public DigitRange(int minNum, int maxNum)
            {
                this.maxNum = maxNum;
                this.minNum = minNum;
            }
            public override bool Pass(int num)
            {
                if (num < minNum)
                    return false;
                if (num > maxNum)
                    return false;
                return true;
            }
        }
        public static Single CreateFromAcceptableInts(int[] ints, int setDigitAmount = -1)
        {
            List<DigitRange> filters = new List<DigitRange>();
            foreach (var num in ints)
            {
                if (num < 0)    //will skip -1 or any negative number.
                    continue;

                List<int> digitList = num.ToString().Select(digit => int.Parse(digit.ToString())).ToList();

                if (setDigitAmount != -1 && digitList.Count != setDigitAmount)
                    continue;


                int digitOffset = 0;
                digitOffset = filters.Count - digitList.Count;
                int i = 0;
                for (; i < digitOffset; i++)
                {
                    digitList.Insert(0, 0);
                }
                for (; i < digitList.Count; i++)
                {
                    if (filters.Count == i)
                        filters.Add(digitList[i]);
                    if (filters[i].minNum > digitList[i])
                        filters[i].minNum = digitList[i];
                    if (filters[i].maxNum < digitList[i])
                        filters[i].maxNum = digitList[i];
                }
            }
            if (setDigitAmount != -1 && filters.Count != setDigitAmount)
            {
                Util.println("No filters found for the digits");
                return null;
            }
            return Create(setDigitAmount != -1, filters.ToArray());
        }
        public static Single Create(DigitRange nr0, 
            DigitRange nr1 = null
            , DigitRange nr2 = null
            , DigitRange nr3 = null
            , DigitRange nr4 = null
            , DigitRange nr5 = null
            , DigitRange nr6 = null
            , DigitRange nr7 = null
            , DigitRange nr8 = null
            , DigitRange nr9 = null
            , DigitRange nr10 = null
            , DigitRange nr11 = null
            , DigitRange nr12 = null
            , DigitRange nr13 = null
            , DigitRange nr14 = null
            , DigitRange nr15 = null
            )
        {
            return new Single(nr0,
                                        nr1,
                                        nr2,
                                        nr3,
                                        nr4,
                                        nr5,
                                        nr6,
                                        nr7,
                                        nr8,
                                        nr9,
                                        nr10,
                                        nr11,
                                        nr12,
                                        nr13,
                                        nr14,
                                        nr15
                                        );
        }
        public static Single Create(bool samePlaceAmount
            , DigitRange nr0
            , DigitRange nr1 = null
            , DigitRange nr2 = null
            , DigitRange nr3 = null
            , DigitRange nr4 = null
            , DigitRange nr5 = null
            , DigitRange nr6 = null
            , DigitRange nr7 = null
            , DigitRange nr8 = null
            , DigitRange nr9 = null
            , DigitRange nr10 = null
            , DigitRange nr11 = null
            , DigitRange nr12 = null
            , DigitRange nr13 = null
            , DigitRange nr14 = null
            , DigitRange nr15 = null
            )
        {
            return new Single(samePlaceAmount,
                                        nr0,
                                        nr1,
                                        nr2,
                                        nr3,
                                        nr4,
                                        nr5,
                                        nr6,
                                        nr7,
                                        nr8,
                                        nr9,
                                        nr10,
                                        nr11,
                                        nr12,
                                        nr13,
                                        nr14,
                                        nr15
                                        );
        }
        public static Single Create(DigitRange[] numRanges)
        {
            return new Single(numRanges);
        }
        public static Single Create(bool mustShareDigitAmount, DigitRange[] numRanges)
        {
            return new Single(mustShareDigitAmount, numRanges);
        }
        public class Single : IntFilter
        {
            public int lastNumberGiven = -1;
            public bool mustShareDigitAmount = false;
            DigitRange[] digitRanges;
            public Single(
                  DigitRange nr0
                , DigitRange nr1 = null
                , DigitRange nr2 = null
                , DigitRange nr3 = null
                , DigitRange nr4 = null
                , DigitRange nr5 = null
                , DigitRange nr6 = null
                , DigitRange nr7 = null
                , DigitRange nr8 = null
                , DigitRange nr9 = null
                , DigitRange nr10 = null
                , DigitRange nr11 = null
                , DigitRange nr12 = null
                , DigitRange nr13 = null
                , DigitRange nr14 = null
                , DigitRange nr15 = null
                )
            {
                List<DigitRange> numRanges = new List<DigitRange>();
                numRanges.Add(nr0);
                numRanges.Add(nr1);
                numRanges.Add(nr2);
                numRanges.Add(nr3);
                numRanges.Add(nr4);
                numRanges.Add(nr5);
                numRanges.Add(nr6);
                numRanges.Add(nr7);
                numRanges.Add(nr8);
                numRanges.Add(nr9);
                numRanges.Add(nr10);
                numRanges.Add(nr11);
                numRanges.Add(nr12);
                numRanges.Add(nr13);
                numRanges.Add(nr14);
                numRanges.Add(nr15);
                this.digitRanges = numRanges.ToArray();
            }
            public Single(bool samePlaceAmount
                , DigitRange nr0
                , DigitRange nr1 = null
                , DigitRange nr2 = null
                , DigitRange nr3 = null
                , DigitRange nr4 = null
                , DigitRange nr5 = null
                , DigitRange nr6 = null
                , DigitRange nr7 = null
                , DigitRange nr8 = null
                , DigitRange nr9 = null
                , DigitRange nr10 = null
                , DigitRange nr11 = null
                , DigitRange nr12 = null
                , DigitRange nr13 = null
                , DigitRange nr14 = null
                , DigitRange nr15 = null
                )
            {
                this.mustShareDigitAmount = samePlaceAmount;
                List<DigitRange> numRanges = new List<DigitRange>();
                if (nr0 != null)
                    numRanges.Add(nr0);
                if (nr1 != null)
                    numRanges.Add(nr1);
                if (nr2 != null)
                    numRanges.Add(nr2);
                if (nr3 != null)
                    numRanges.Add(nr3);
                if (nr4 != null)
                    numRanges.Add(nr4);
                if (nr5 != null)
                    numRanges.Add(nr5);
                if (nr6 != null)
                    numRanges.Add(nr6);
                if (nr7 != null)
                    numRanges.Add(nr7);
                if (nr8 != null)
                    numRanges.Add(nr8);
                if (nr9 != null)
                    numRanges.Add(nr9);
                if (nr10 != null)
                    numRanges.Add(nr10);
                if (nr11 != null)
                    numRanges.Add(nr11);
                if (nr12 != null)
                    numRanges.Add(nr12);
                if (nr13 != null)
                    numRanges.Add(nr13);
                if (nr14 != null)
                    numRanges.Add(nr14);
                if (nr15 != null)
                    numRanges.Add(nr15);
                this.digitRanges = numRanges.ToArray();
            }
            public Single(DigitRange[] numRanges)
            {
                this.digitRanges = numRanges;
            }
            public Single(bool samePlaceAmount, DigitRange[] numRanges)
            {
                this.mustShareDigitAmount = samePlaceAmount;
                this.digitRanges = numRanges;
            }
            public override bool Pass(int num)
            {
                return Pass(num, out int i);
            }
            public bool Pass(int num, out int index)
            {
                index = -1;
                if (num < 0)
                    return false;

                List<int> digitList = num.ToString().Select(digit => int.Parse(digit.ToString())).ToList();
                return Pass(digitList, out index);
            }
            public bool Pass(List<int> digitList, out int index)
            {
                index = -1;
                if ((mustShareDigitAmount && digitList.Count != digitRanges.Length) || digitList.Count > digitRanges.Length)
                    return false;
                int digitOffset = digitRanges.Length - digitList.Count;
                int i = 0;
                for (; i < digitOffset; i++)
                {
                    if (digitRanges[i].Pass(0))
                    {
                        index = i;
                        return false;
                    }
                }
                for (; i < digitList.Count && i < digitRanges.Length; i++)
                {
                    if (!digitRanges[i].Pass(digitList[i]))
                    {
                        index = i;
                        return false;
                    }
                }

                return true;
            }

            public int GetNext()
            {
                List<int> digits = new List<int>();
                if (lastNumberGiven != -1)
                    digits = lastNumberGiven.ToString().Select(digit => int.Parse(digit.ToString())).ToList();
                else
                {
                    for (int i = 0; i < digitRanges.Length; i++)
                    {
                        digits.Add(digitRanges[i].minNum);
                    }
                    //Util.println("firstdigit "+int.Parse(string.Join("", digits)));
                }
                int toMake =  digitRanges.Count() - digits.Count;
                for (int i = 0; i < toMake; i++)
                {
                    digits.Insert(0, 0);
                }

                for (int i = digitRanges.Length-1; i >= 0; i--)
                {
                    int min = digitRanges[i].minNum;
                    int max = digitRanges[i].maxNum;
                    int dig = digits[i];
                    if (digitRanges[i].maxNum > dig)
                    {
                        digits[i]++;
                        lastNumberGiven = int.Parse(string.Join("", digits));
                        return lastNumberGiven;
                    }
                    else
                    {
                        digits[i] = digitRanges[i].minNum;
                    }
                }
                return -1;
            }

            public int GetRandom(int seed, int steps)
            {
                Random rand = new Random(seed);

                List<int> digits = new List<int>();

                for (int i = 0; i < digitRanges.Length; i++)
                {
                    int min = digitRanges[i].minNum;
                    int max = digitRanges[i].maxNum;
                    int dig = rand.Next(min, max);
                    digits.Add(dig);
                }
                int d = int.Parse(string.Join("", digits));
                if (steps == 0)
                    return int.Parse(string.Join("", digits));

                int stepsLeft = Math.Abs(steps);
                if (steps > 0)
                {
                    for (int i = digitRanges.Length - 1; i >= 0; i--)
                    {
                        int min = digitRanges[i].minNum;
                        int max = digitRanges[i].maxNum;
                        int dig = digits[i];
                        if (max > dig)
                        {
                            stepsLeft--;
                            digits[i]++;
                            if (stepsLeft <= 0)
                                return int.Parse(string.Join("", digits));
                            i = digitRanges.Length - 1;
                        }
                        else
                        {
                            digits[i] = min;
                        }
                    }
                }
                else
                {
                    for (int i = digitRanges.Length - 1; i >= 0; i--)
                    {
                        int min = digitRanges[i].minNum;
                        int max = digitRanges[i].maxNum;
                        int dig = digits[i];
                        if (min < dig)
                        {
                            stepsLeft--;
                            digits[i]--;
                            if (stepsLeft <= 0)
                                return int.Parse(string.Join("", digits));
                            i = digitRanges.Length - 1;
                        }
                        else
                        {
                            digits[i] = max;
                        }
                    }
                }
                return -1;
            }

            public void Print()
            {
                string s = "";
                for (int i = 0; i < digitRanges.Length; i++)
                {
                    var range = digitRanges[i];
                    if (range.minNum == range.maxNum)
                        s += range.minNum;
                    else if (range.minNum == 0 && range.maxNum == 9)
                        s += "-";
                    else
                        s += "(" + range.minNum + "," + range.maxNum + ")";
                }
                Util.println(s);
            }
        }

        public class Multiple: IntFilter
        {
            IntFilter[] filters;

            public Multiple(IntFilter f0, IntFilter f1 = null, IntFilter f2 =null, IntFilter f3 = null, IntFilter f4 = null, IntFilter f5 = null, IntFilter f6 = null, IntFilter f7 = null, IntFilter f8 = null, IntFilter f9 = null, IntFilter f10 = null, IntFilter f11 = null, IntFilter f12 = null, IntFilter f13 = null, IntFilter f14 = null, IntFilter f15 = null)
            {
                List<IntFilter> filters = new List<IntFilter>();

                filters.Add(f0);
                if (f1 != null)
                    filters.Add(f1);
                if (f2 != null)
                    filters.Add(f2);
                if (f3 != null)
                    filters.Add(f3);
                if (f4 != null)
                    filters.Add(f4);
                if (f5 != null)
                    filters.Add(f5);
                if (f6 != null)
                    filters.Add(f6);
                if (f7 != null)
                    filters.Add(f7);
                if (f8 != null)
                    filters.Add(f8);
                if (f9 != null)
                    filters.Add(f9);
                if (f10 != null)
                    filters.Add(f10);
                if (f11 != null)
                    filters.Add(f11);
                if (f12 != null)
                    filters.Add(f12);
                if (f13 != null)
                    filters.Add(f13);
                if (f14 != null)
                    filters.Add(f14);
                if (f15 != null)
                    filters.Add(f15);

                foreach (IntFilter intFilter in filters)
                {
                    if (intFilter == this)
                    {
                        Util.println("INT FILTER INCLUDES ITSELF");
                        return;
                    }
                }

                this.filters = filters.ToArray();


            }
            public Multiple(IntFilter[] filters)
            {
                this.filters = filters;
                foreach (IntFilter intFilter in filters)
                {
                    if (intFilter == this)
                    {
                        Util.println("INT FILTER INCLUDES ITSELF");
                        return;
                    }
                }
            }
            public override bool Pass(int num)
            {
                foreach (var filter in filters)
                {
                    if (!filter.Pass(num))
                        return false;
                }
                return true;
            }
        }

        public class Exceptions : IntFilter
        {
            public static implicit operator Exceptions (int exception)
            {
                return new Exceptions(new int[] { exception }.ToList());
            }
            public static implicit operator Exceptions (List<int> exception)
            {
                return new Exceptions(exception);
            }
            List<int> exceptions = new List<int>();
            public Exceptions( List<int> exceptions)
            {
                this.exceptions = exceptions;
            }
            public override bool Pass(int num)
            {
                return !exceptions.Contains(num);
            }
        }

        public static int GetNextInt(Single filter, Exceptions exceptions)
        {
            int num = filter.GetNext();
            while (!exceptions.Pass(num))
            {
                num = filter.GetNext();
            }
            return num;
        }
        public static int GetRandomInt(int seed, Single filter, Exceptions exceptions)
        {
            int curStepsPos = 0;
            int curStepsNeg = 0;
            bool negative = false;
            bool lockNegative = false;
            int num = filter.GetRandom(seed, 0);
            while (!exceptions.Pass(num))
            {
                int curSteps = curStepsPos;
                if (negative)
                {
                    curSteps = curStepsNeg;
                    curStepsNeg--;
                }
                else
                    curStepsPos++;

                if (!lockNegative)
                    negative = !negative;

                num = filter.GetRandom(seed, curSteps);
                if (num == -1)
                {
                    if (lockNegative)
                        Debug.Fail("out of Ids");
                    else
                        lockNegative = true;
                }
            }
            return num;
        }

        public abstract bool Pass(int num);
        
    }
    class UnUsed
    {
        static void CreateMassEditText(string id)
        {
            string param = "AtkParam_Pc";

            bool idCheck = true;

            bool propCheck = false;
            string[][] prop = new string[2][];                          //same list checks both (&&), seprate list checks seperate(||)
            prop[1] = new string[] { "subcategory1" };
            prop[1] = new string[] { "subcategory2" };

            string[][] propCompare = new string[2][];
            propCompare[0] = new string[] { "112" };
            propCompare[1] = new string[] { "112" };


            string[] modifyField = new string[] { "atkMagCorrection", "atkFireCorrection", "atkThunCorrection", "atkPhysCorrection" };
            string modifyOperation = "* 2";


            List<string> outputLines = new List<string>();

            for (int l = 0; l < Math.Max(prop.Length, 1); l++)
            {
                string row = "param " + param + ": ";

                string idCheckString = "";
                if (idCheck)
                {
                    idCheckString = "id " + id + "";
                }

                string propCheckString = "";
                if (propCheck)
                {
                    if (idCheck)
                        propCheckString += " ";
                    for (int i = 0; i < prop[l].Length; i++)
                    {
                        propCheckString += "prop " + prop[l][i] + " " + propCompare[l][i];
                    }
                }
                row += idCheckString + propCheckString + ": ";
                for (int m = 0; m < modifyField.Length; m++)
                {
                    string line = row + modifyField[m] + ":" + modifyOperation + ";";
                    outputLines.Add(line);
                }
            }

            Util.PrintStrings(outputLines.ToArray());
        }
    }


    
}
