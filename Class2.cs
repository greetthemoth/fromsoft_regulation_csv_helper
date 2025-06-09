using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EldenRingCSVHelper
{
    public class ParamFile : LineContainer
    {
        public readonly char delimiter;
        public readonly string filename;
        public readonly string paramName;
        public readonly string fileExtension;
        public readonly string parentFile;
        public readonly string[] header;

        public string HeaderLine
        {
            get
            {
                return Line.DataToLine(header, delimiter);
            }
        }


        /// <summary> 
        /// Returns the header as a string with the proper Write Delimiter. Used when writing a csv. The header is the first line of a csv that denotes the names of each field.
        /// </summary>
        public string HeaderToWrite()
        {
            return Line.DataToLine(header, RunSettings.Write_Delimiter);
        }


        public ParamFile vanillaParamFile;

        public static List<ParamFile> paramFiles = new List<ParamFile>();
        public static List<ParamFile> vanillaParamFiles = new List<ParamFile>();

        public int numberOfModifiedOrAddedLines = 0;
        public int numberOfModifiedFields = 0;
        /// <summary> 
        /// Resets this param to its vanilla state.
        /// </summary>
        public void Revert()
        {
            numberOfModifiedFields = 0;
            numberOfModifiedOrAddedLines = 0;
            lines.Clear();
            foreach(Line vLine in vanillaParamFile.lines)
            {
                lines.Add(vLine.Copy(this,true));
            }
        }

        public void SetToFakeVanilla()
        {
            numberOfModifiedFields = 0;
            numberOfModifiedOrAddedLines = 0;
            this.vanillaParamFile.lines.Clear();
            foreach (Line line in lines)
            {
                line.TurnFakeVanilla();
                vanillaParamFile.lines.Add(line.Copy(vanillaParamFile, true));
            }
        }
        /// <summary> 
        /// Resets ALL params to their vanilla state.
        /// </summary>
        public static void RevertAll(bool checkIsModified = false)
        {
            FlagIds.ClearUsedFlagIds();
            foreach(ParamFile file in paramFiles)
            {
                if(!checkIsModified || file.numberOfModifiedOrAddedLines > 0)
                    file.Revert();
            }
        }
        public static ParamFile GetFile(string filename)
        {
            for (int i = 0; i < paramFiles.Count; i++)
            {
                if (paramFiles[i].parentFile == filename)
                    return paramFiles[i];
            }
            return null;
        }

        public ParamFile(string fileLocation, string filename, char delimiter = ';', bool addToParamFilesList = true)
        {
            this.delimiter = delimiter;
            this.filename = filename;
            int periodIndex = filename.IndexOf(".");
            this.fileExtension = filename.Substring(periodIndex);
            this.paramName = filename.Remove(periodIndex);
            this.parentFile = fileLocation + filename;
            using (var sr = new StreamReader(this.parentFile))
            {
                string line;
                header = Line.CreateData(sr.ReadLine(), delimiter);
                var vanillaLinesList = new List<Line>();

                vanillaParamFile = new ParamFile(this.delimiter, this.filename, this.paramName, this.fileExtension, this.parentFile, this.header);

                while ((line = sr.ReadLine()) != null)
                {
                    var vanillaLine = new Line(line, vanillaParamFile);
                    vanillaLine.isVanillaLine = true;
                    vanillaLinesList.Add (vanillaLine);
                    Line myLine = vanillaLine.Copy(this, true);
                    lines.Add(myLine);
                }

                vanillaParamFile.lines = vanillaLinesList;
            }
            lines = GetOrderedLines();
            vanillaParamFile.lines = vanillaParamFile.GetOrderedLines();
            vanillaParamFiles.Add(vanillaParamFile);
            if(addToParamFilesList)
                paramFiles.Add(this);
        }

        ParamFile(char delimiter,string filename,string paramName,string fileExtension,string parentFile, string[] header) 
        {
            this.delimiter = delimiter;
            this.filename = filename;
            this.paramName = paramName;
            this.fileExtension = fileExtension;
            this.parentFile = parentFile;
            this.header = header;
        }
        /// <summary> 
        /// Returns the field indexes of all given fields. Ordered Fields: more efficient but only works if fields are ordered approprietly. 
        /// </summary>
        public int[] GetFieldIndexes(string[] fieldNames, bool orderedFields = true)
        {
            int[] indexes = new int[fieldNames.Length];
            int lookingForIndex = 0;
            Debug.Assert(header.Length != 0);
            if (orderedFields)
            {
                for (int i = 0; (i < lines.Count && lookingForIndex < indexes.Length); i++)
                {
                    Debug.Assert(header.Length > i);
                    if (header[i] == fieldNames[lookingForIndex])
                    {
                        indexes[lookingForIndex] = i;
                        lookingForIndex++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < header.Length; i++)
                {
                    for (int j = 0; j < fieldNames.Length; j++)
                    {
                        if (fieldNames[j] == header[i])
                        {
                            lookingForIndex++;
                            indexes[j] = i;
                        }
                    }
                }
            }
            if (lookingForIndex < indexes.Length)
            {
                Util.println("did not find all indexes. found " + lookingForIndex +" of "+ indexes.Length);
            }
            return indexes;
        }
        /// <summary> 
        /// Returns the field index of the given field.
        /// </summary>
        public int GetFieldIndex(string fieldName)
        {

            for (int i = 0; i < header.Length; i++)
            {
                if (header[i] == fieldName)
                    return i;
            }
            Util.println("did not find field: \"" + fieldName +"\"");
            return -1;
        }
        /// <summary> 
        /// Get the field indexes of all fields which name contains strings. Useful for getting multiple similarly named fieldIndexes easily. 
        /// </summary>
        public int[] GetFieldIndexesContains(string[] fieldNameContains)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < header.Length; i++)
            {
                for (int j = 0; j < fieldNameContains.Length; j++)
                {
                    if (header[i].Contains(fieldNameContains[j]))
                    {
                        indexes.Add(i);
                    }
                }
            }
            return indexes.ToArray();
        }
        /// <summary> 
        /// Get the field indexes of all fields which name contains string. Useful for getting multiple similarly named fieldIndexes easily. 
        /// </summary>
        public int[] GetFieldIndexesContains(string fieldNameContains)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < header.Length; i++)
            {
                if (header[i].Contains(fieldNameContains))
                {
                    indexes.Add(i);
                }
            }
            return indexes.ToArray();
        }
        /// <summary> 
        /// Get the name of fields with field indexes.
        /// </summary>
        public string[] GetFieldNames(int[] fieldIndexes)
        {
            string[] ret = new string[fieldIndexes.Length];
            for (int i = 0; i < fieldIndexes.Length; i++)
            {
                ret[i] = GetFieldName(fieldIndexes[i]);
            }
            return ret;
        }
        /// <summary> 
        /// Get the name of a field with the field index.
        /// </summary>
        public string GetFieldName(int index)
        {
            return header[index];
        }
        /// <summary> 
        /// Run a line modifier into all lines in this Param. Includes an optional Line Condition.   ConditionOnVanillaLines: the line is only modified if the condition passes on the vanilla version of the line.
        /// </summary>
        public void Operate(LineModifier operation, Condition condition = null, bool conditionOnVanillaLines = false)
        {
            if (conditionOnVanillaLines)
                Line.Operate(lines, operation, condition, conditionOnVanillaLines);
            else
                Line.Operate(lines, operation, condition, lines);
        }
        /// <summary> 
        /// Run line modifiers into all lines in this Param. Includes an optional Line Condition.  ConditionOnVanillaLines:the line is only modified if the condition passes on the vanilla version of the line.
        /// </summary>
        public void Operate(LineModifier[] operations, Condition condition = null, bool conditionOnVanillaLines = false)
        {
            if (conditionOnVanillaLines)
                Line.Operate(lines, operations, condition, conditionOnVanillaLines);
            else
                Line.Operate(lines, operations, condition, lines);
        }


        /// <summary> 
        /// Prints this param file.
        /// </summary>
        public void PrintFile()
        {
            Util.println("________________________________________________");
            Util.println("Starting print of " + this.filename + "...");
            Util.println("________________________________________________");
            Util.println(HeaderLine);
            PrintLines();
            Util.println("________________________________________________");
            Util.println("Finished print of " + this.filename + "!" + ModificationSummary() + VerifyFieldCounts());
            Util.println("________________________________________________");
        }
        /// <summary> 
        /// Returns all modified field indexes found in this Param.
        /// </summary>
        public List<int> GetModifiedFieldIndexes(Condition condition = null)
        {
            List<int> fieldIndexes = new List<int>();
            foreach(Line line in lines)
            {
                if (condition != null && !condition.Pass(line))
                    continue;
                if (line.IsExtraLine)   //does not include modified indexes of added lines.
                    continue;
                foreach (int fieldIndex in line.modifiedFieldIndexes) {
                    if (fieldIndexes.Contains(fieldIndex))
                        continue;
                    //Util.println(header[fieldIndex]);
                    fieldIndexes.Add(fieldIndex);
                }
            }
            return fieldIndexes;
        }
        /// <summary> 
        /// returns first modified field indexes found in this param.
        /// </summary>
        public int GetModifiedFieldIndex(Condition condition = null)
        {
            foreach (Line line in lines)
            {
                if (condition != null && !condition.Pass(line))
                    continue;
                foreach (int fieldIndex in line.modifiedFieldIndexes)
                {
                    return fieldIndex;
                }
            }return -1;
        }
        /// <summary> 
        /// Writes a csv with all the modified vanilla lines. Includes a folder for field specific imports.
        /// </summary>
        public void WriteModifiedFile(string fileNamePre = "", string fileNamePost ="", Condition condition = null, Condition conditionToTurnToRelaceWithBaseLine = null, Line baseLine = null)
        {
            bool WriteSingleFields = false;

            string ToUseFileName = paramName;
            ToUseFileName = fileNamePre + ToUseFileName;
            //Util.println("----" + filename);
            var indexes = GetModifiedFieldIndexes(condition);
            if (RunSettings.Write_CanBeSingleField && indexes.Count <= 5)
            {
                WriteSingleFields = true;
            }

            bool hasExtraLines = lines.Count != vanillaParamFile.lines.Count;
            string curSpecialDirectory = RunSettings.Write_directory;

            bool onlyHasExtraLinesNoChanges = hasExtraLines && indexes.Count == 0;

            curSpecialDirectory += @"\" + paramName;
            if (WriteSingleFields)
                curSpecialDirectory += " Field Imports";
            if (WriteSingleFields && hasExtraLines)
                curSpecialDirectory += " and";
            if (hasExtraLines)
                curSpecialDirectory += " Extra Lines";

            for (int i = -2; i < indexes.Count; i++)
            {
                List<Line> curLines = lines;
                if (condition != null)
                    curLines = GetLinesOnCondition(condition);
                if (curLines.Count == 0)
                    continue;

                int curIndex = -1;
                string curDirectory = RunSettings.Write_directory;
                string curToUseFileName = ToUseFileName;
                if(i != -2)
                {
                    curDirectory = curSpecialDirectory;
                    if (i == -1)
                    {
                        if (!hasExtraLines)
                            continue;
                        if (onlyHasExtraLinesNoChanges)
                            curDirectory = RunSettings.Write_directory;
                        curToUseFileName += " ALL(ExtraLines)";
                    }
                    else if (WriteSingleFields)
                    {
                        curIndex = indexes[i];
                        curToUseFileName += " FIELD " + header[curIndex];
                    }
                }
                else
                {
                    if (onlyHasExtraLinesNoChanges) //skip ALL for "Extra Lines"
                        continue;
                    curToUseFileName += " ALL";
                }
                
                curToUseFileName += fileNamePost;
                // Combine the directory path and file name to create the full file path
                curToUseFileName += fileExtension;
                
                string filePath = Path.Combine(curDirectory, curToUseFileName);


                // Check if the directory exists; if not, create it
                if (!Directory.Exists(curDirectory))
                {
                    //Debug.Fail("ToWriteCSV_directory not found. ");
                    Directory.CreateDirectory(curDirectory);
                }

                // Create a new text file and write some content

                
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    if (i < 0)
                        writer.WriteLine(HeaderToWrite());
                    foreach (var line in curLines)
                    {
                        
                        if (condition != null && !condition.Pass(line))
                            continue;
                        Line curLine = line;
                        if (conditionToTurnToRelaceWithBaseLine != null && conditionToTurnToRelaceWithBaseLine.Pass(line))
                        {
                            curLine = baseLine.Copy().SetField(0, curLine.id);
                        }
                        if (!RunSettings.Write_OnlyModifiedLines || curLine.modified)
                        {
                            if(i < 0)
                            {
                                if(i == -1 && (line.added)) //use line not cur line becuase base ;ine is not considered added
                                    writer.WriteLine(curLine.ToWrite());

                                if (i == -2)
                                    writer.WriteLine(curLine.ToWrite());
                            }
                            else
                            {
                                //Util.PrintStrings(Util.Append(Util.ToStrings(line.modifiedFieldIndexes.ToArray())," - ", line.file.GetFieldNames(line.modifiedFieldIndexes.ToArray())));
                                //Util.println("has " + curIndex  + " ?" +"  "+ line.modifiedFieldIndexes.Contains(curIndex).ToString());
                                if(curLine.modifiedFieldIndexes.Contains(curIndex))
                                    writer.WriteLine(curLine.id + RunSettings.Write_Delimiter + curLine.GetField(curIndex));
                            }
                        }
                    }
                }
                string msg = Util.IndentedText(curToUseFileName + " created successfully.     ",95) + DateTime.Now.ToString("h:mm:ss tt");
                Util.println(msg);
                if (!RunSettings.PrintOnConsole)
                    Console.WriteLine(msg);

                if (WriteSingleFields)
                    continue;
                break;
            }
        }
        public static void OrderAllModifiedParams()
        {
            foreach(ParamFile p in paramFiles)
            {
                if(p.numberOfModifiedOrAddedLines > 0)
                {
                    p.lines = p.GetOrderedLines();
                }
            }
        }
        public static void ImportCSVs(string directory)
        {
            ImportCSVs(directory, RunSettings.Write_Delimiter);
        }
        public static void ImportCSVs(string directory, char delimiter )
        {
            
            List<string> csvDirectories = new List<string>();
            if (directory.EndsWith(".csv"))
            {
                csvDirectories.Add(directory);
            }
            else
            {
                csvDirectories = Directory.GetFiles(directory).ToList() ;
            }

            foreach(string csvDir in csvDirectories)
            {
                string filename = Path.GetFileName(csvDir);
                int periodIndex = filename.IndexOf(".");
                string paramName = filename.Remove(filename.IndexOf(" "));

                ParamFile paramFile = null;
                foreach(ParamFile p in paramFiles)
                {
                    if (p.paramName == paramName)
                    {
                        paramFile = p;
                        break;
                    }
                }
                if(paramFile == null)
                {
                    Util.p();
                    return;
                }

                using (var sr = new StreamReader(csvDir))
                {
                    string line = sr.ReadLine();
                    int startIndex = 0;
                    int lastId = -1;
                    if (int.TryParse(Line.CreateData(line, delimiter)[0], out int i))
                    {
                        var newLine = new Line(line, paramFile);
                        //newLines.Add(newLine);

                        if (lastId > newLine.id_int)
                            Util.p();

                        paramFile.OverrideOrAddLine(newLine,out startIndex,startIndex);


                        if (startIndex == paramFile.lines.Count-1)
                            Util.p();

                        lastId = newLine.id_int;
                        //startIndex--;
                    }
                    //var newLines = new List<Line>();

                    while ((line = sr.ReadLine()) != null)
                    {
                        var newLine = new Line(line, paramFile);
                        newLine.UpdateModificationStatus();
                        //newLine.MarkModified(false);
                        //newLines.Add(newLine);

                        //if (newLine.id_int == 463000405 )
                        //    Util.println(newLine._idName);
                        if (lastId > newLine.id_int)
                            Util.p();

                        paramFile.OverrideOrAddLine(newLine, out startIndex, startIndex);
                        //startIndex--;

                        if (startIndex == paramFile.lines.Count-1)
                            Util.p();
                        lastId = newLine.id_int;
                    }
                }
            }
        }

        /// <summary> 
        /// Writes csvs for all Params with modified vanilla lines. Includes a folder for field specific imports.
        /// </summary>
        public static int WriteModifiedFiles(string fileNamePre = "", string fileNamePost = "", Condition condition = null, Condition conditionToTurnToRelaceWithBaseLine = null, Line baseLine = null)
        {
            int filesPrinted = 0;
            foreach (ParamFile paramFile in ParamFile.paramFiles)
            {
                string debugName = paramFile.paramName;
                if (paramFile.numberOfModifiedOrAddedLines > 0)
                {
                    paramFile.WriteModifiedFile(fileNamePre,fileNamePost,condition, conditionToTurnToRelaceWithBaseLine, baseLine);
                    filesPrinted++;
                }
            }
            if (filesPrinted > 0)
                Console.WriteLine();
            return filesPrinted;
        }

        public static int WriteModifiedField(int fieldIndex, string fileNamePre = "", string fileNamePost = "", Condition condition = null, Condition conditionToTurnToRelaceWithBaseLine = null, Line baseLine = null)
        {
            int filesPrinted = 0;
            foreach (ParamFile paramFile in ParamFile.paramFiles)
            {
                string debugName = paramFile.paramName;
                if (paramFile.numberOfModifiedOrAddedLines > 0)
                {
                    //paramFile.WriteModifiedField(fieldIndex, fileNamePre, fileNamePost, condition, conditionToTurnToRelaceWithBaseLine, baseLine);
                    filesPrinted++;
                }
            }
            if (filesPrinted > 0)
                Console.WriteLine();
            return filesPrinted;
        }
        /// <summary> 
        /// Prints a csv with only the modified lines.
        /// </summary>
        public void PrintModifiedFile()
        {
            Util.println("________________________________________________");
            Util.println("Starting print of " + RunSettings.ToRun.filename + "...");
            Util.println("________________________________________________");
            Util.println(HeaderLine);
            PrintModifiedLines();
            Util.println("________________________________________________");
            Util.println("Finished print of " + RunSettings.ToRun.filename + "!" + ModificationSummary() + VerifyFieldCounts());
            Util.println("________________________________________________");
        }
        /// <summary> 
        /// Prints a summary of all lines modifications incluing the number of modified lines, number of modified fields, and the number of lines added or removed.
        /// </summary>
        string ModificationSummary()
        {
            if (!RunSettings.PrintFile_ModificationSummary)
                return "";
            int lineAMountDiffrence = lines.Count - vanillaParamFile.lines.Count;
            string lineAmountModificationString = "";
            if(lineAMountDiffrence > 0)
            {
                lineAmountModificationString += "\nNumber of lines added: " + lineAMountDiffrence;
            }else if (lineAMountDiffrence < 0)
            {
                lineAmountModificationString += "\nNumber of lines removed: " + -lineAMountDiffrence;
            }
            return "\nNumber of modified lines: " + numberOfModifiedOrAddedLines + "\nNumber of modified fields: " + numberOfModifiedFields + lineAmountModificationString; 
        }
        /// <summary> 
        /// Returns a string that Varifies if the number of fields in all lines in this param are consitant with the vanilla. Useful if you are getting the "wrong number of fields" error when attempting to import.
        /// If you continue to get the error, despite passing the Verfication, try updating your vanilla csvs.
        /// </summary>
        public string VerifyFieldCounts()
        {
            if (!RunSettings.PrintFile_VerifyFieldCounts)
                return "";
            int targetAmount = Line.GetFieldCount(HeaderLine, delimiter);
            bool failed = false;

            string ret = "";

            int totalFailedLines = 0;

            List<int> failedFieldAmounts;
            List<int> failedLineAmounts;
            List<string> failedLineIDsString;

            const bool collapse = true;
            const int IDLimit = 4;

            if (collapse)
            {
                 failedFieldAmounts = new List<int>();
                 failedLineAmounts = new List<int>();
                 failedLineIDsString = new List<string>();
            }

            foreach (var line in lines)
            {
                if (!line.modified && RunSettings.PrintFile_VerifyFieldCounts_OnlyModifiedLines)
                    continue;
                int fieldsFound = Line.GetFieldCount(line.line, delimiter);
                if (fieldsFound != targetAmount)
                {
                    failed = true;
                    totalFailedLines++;
                    if (!collapse)
#pragma warning disable CS0162 // Unreachable code detected
                        ret+=("\n   " + fieldsFound + " fields found instead of " + targetAmount + " on line " + line.id);
#pragma warning restore CS0162 // Unreachable code detected
                    else
                    {
                        int fi = failedFieldAmounts.IndexOf(fieldsFound);
                        if (fi == -1)
                        {
                            failedFieldAmounts.Add(fieldsFound);
                            failedLineAmounts.Add(1);
                            failedLineIDsString.Add(line.id);
                        }
                        else
                        {
                            failedLineAmounts[fi]++;

                            if(IDLimit == -1 || failedLineAmounts[fi] <= IDLimit)
                                failedLineIDsString[fi] += ", " + line.id;
                            else if(failedLineAmounts[fi] == IDLimit + 1)
                            {
                                failedLineIDsString[fi] += "...";
                            }
                        }

                    }
                    //line.Print();
                }
                //Debug.Assert(Line.GetFieldCount(line.line) != targetAmount);
            }
            if (failed)
            {
                ret += "\nWrong field counts in " + totalFailedLines + " / " + lines.Count + " lines";
                if (collapse) 
                {
                    for(int i = 0; i < failedFieldAmounts.Count; i++)
                    {
                        ret+=("\n    " + failedFieldAmounts[i] + " fields found instead of " + targetAmount + " on " + failedLineAmounts[i] + " lines. " + failedLineIDsString[i]);
                    }
                }
                return ret;
            }
            return "\nField Count Verification Passed!";

        }

        public override void OverrideOrAddLine(Line overrideLine)
        {
            
            lines = Line.OverrideOrAddLine(lines, overrideLine, this);
        }
        public override void OverrideOrAddLine(Line overrideLine, out int lineIndex, int startIndex)
        {

            lines = Line.OverrideOrAddLine(lines, overrideLine, this, out lineIndex, startIndex);
        }
        public override void InsertLine(Line overrideLine, int startIndex = 0)
        {
            //lines = 
            Line.InsertLine(lines, overrideLine, out int lineIndex, this, startIndex);
        }
        public override void InsertLine(Line overrideLine, out int lineIndex, int startIndex = 0)
        {
            //lines = 
            Line.InsertLine(lines, overrideLine, out lineIndex, this,startIndex );
        }
    }
    public class Lines : LineContainer
    {
        /// <summary> 
        /// You can use a List of Lines as a Lines.
        /// </summary>
        public static implicit operator Lines(List<Line> list)
        {
            return new Lines(list);
        }
        public ParamFile file
        {
            get;
        }
        /// <summary> 
        /// Get the field index of the given field.
        /// </summary>
        public int GetFieldIndex(string fieldName)
        {
            return file.GetFieldIndex(fieldName);
        }
        /// <summary> 
        /// Get the field indexes of the given fields.
        /// </summary>
        public int[] GetFieldIndexes(string[] fieldNames)
        {
            return file.GetFieldIndexes(fieldNames);
        }
        /// <summary> 
        /// Create a Lines form an array of lines. Parentfile is the Paramfile these lines are associated with.
        /// </summary>
        public Lines(string[] lines, string parentFile)
        {

            ParamFile p = ParamFile.GetFile(parentFile);
            this.file = p;
            foreach (var line in lines)
            {
                this.lines.Add(new Line(line, p));
            }
        }
        /// <summary> 
        /// Create a Lines form an array of lines.
        /// </summary>
        public Lines(List<Line> lines)
        {

            foreach (var line in lines)
            {
                this.lines.Add(line);
            }
            if (lines.Count != 0)
                file = lines[0].file;
        }

        /// <summary> 
        ///  Parentfile is the Paramfile these lines are associated with the lines in this Lines.
        /// </summary>
        public Lines(ParamFile parentFile)
        {
            file = parentFile;
        }

    }
    public abstract class LineContainer
    {
        List<Line> _lines = new List<Line>();
        public List<Line> lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                //_orderedLinesQued = true;
            }
        }
        /// <summary> 
        /// Number of lines in this Container.
        /// </summary>
        public int Length
        {
            get
            {
                return lines.Count;
            }
        }
        /// <summary> 
        /// Checks if this container contains a certain line.
        /// </summary>
        public bool Contains(Line line)
        {
            return lines.Contains(line);
        }
        /// <summary> 
        /// Removes the line with a given id from this container.
        /// </summary>
        public void RemoveLine(int id)
        {
            lines.Remove(GetLineWithId(id));
        }
        /// <summary> 
        /// currently doesnt work use OVerrideOrAddLine
        /// Overrides the lines in this container with the given lines. Replaces lines with shared ids.
        /// </summary>
        /*public void OverrideLines(List<Line> overrideLines, bool preOrderedLines = true)
        {
            lines = Line.OverrideLines(lines, overrideLines, preOrderedLines);
        }*/
        /// <summary> 
        /// currently doenst work use OVerrideOrAddLine
        /// Overrides the lines in this container with the given lines. Replaces lines with shared ids. Also adds lines that dont share an id. Useful for adding or replacing lines in a Param. 
        /// </summary>
        /*public void OverrideOrAddLines(List<Line> overrideLines, bool preOrderedLines = true)
        {
            lines = Line.OverrideOrAddLines(lines, overrideLines, preOrderedLines);
        }*/
        /// <summary> 
        /// Overrides the line in this container with the given line. Replaces the existing line if it shares an id. Also adds the line if it doesnt share an id.  Useful for adding or replacing lines in a Param.
        /// </summary>
        public virtual void OverrideOrAddLine(Line overrideLine, out int lineIndex, int startIndex)
        {

            lines = Line.OverrideOrAddLine(lines, overrideLine, null, out lineIndex, startIndex);
        }
        /// <summary> 
        /// Overrides the line in this container with the given line. Replaces the existing line if it shares an id. Also adds the line if it doesnt share an id.  Useful for adding or replacing lines in a Param.
        /// </summary>
        public virtual void OverrideOrAddLine(Line overrideLine)
        {
            lines = Line.OverrideOrAddLine(lines, overrideLine);
        }
        public virtual void InsertLine(Line overrideLine, int startIndex = 0)
        {
            //lines = 
            Line.InsertLine(lines, overrideLine, out int lineIndex, null, startIndex);
        }
        public virtual void InsertLine(Line overrideLine, out int lineIndex, int startIndex = 0)
        {
            //lines = 
            Line.InsertLine(lines, overrideLine, out lineIndex, null, startIndex);
        }
        /// <summary> 
        /// Run a line modifier into all lines in this Container. Includes an optional Condition.
        /// </summary>
        public void Operate(LineModifier operation, Condition condition = null, List<Line> conditionLines = null)
        {
            Line.Operate(lines, operation, condition, conditionLines);
        }
        public void RevertFieldsToVanilla()
        {
            foreach(Line line in lines)
            {
                line.RevertFieldsToVanilla();
            }
        }
        /// <summary> 
        /// Run line modifiers into all lines in this Container. Includes an optional Condition.
        /// </summary>
        public void Operate(LineModifier[] operations, Condition condition = null, List<Line> conditionLines = null)
        {
            Line.Operate(lines, operations, condition, conditionLines);
        }
        /// <summary> 
        /// Get line with the given Id.
        /// </summary>
        public Line GetLineWithId(string id)
        {
            return GetLineOnCondition(new Condition.IDCheck(id));
        }
        /// <summary> 
        /// Get line with the given Id.
        /// </summary>
        public Line GetLineWithId(int id)
        {
            return GetLineOnCondition(new Condition.IDCheck(id), out int i);
        }
        /// <summary> 
        /// Get line with the given Id. out lineIndex: the index of the retured line. StartIndex: line index to start looking.
        /// </summary>
        public Line GetLineWithId(int id, out int lineIndex, int startIndex = 0)
        {
            return GetLineOnCondition(new Condition.IDCheck(id),out lineIndex, startIndex);
        }
        /// <summary> 
        /// Returns lines with the given Ids.
        /// </summary>
        public List<Line> GetLinesWithId(string[] ids)
        {
            List<Line> list = GetLinesOnCondition(new Condition.IDCheck(ids));
            return list;
        }
        /// <summary> 
        /// Returns lines with the given Ids.
        /// </summary>
        public List<Line> GetLinesWithId(int[] ids)
        {
            List<Line> list = GetLinesOnCondition(new Condition.IDCheck(ids));
            return list;
        }
        /// <summary> 
        /// Returns first line that passes the condition.  StartIndex: line index to start looking.
        /// </summary>
        public Line GetLineOnCondition(Condition condition, int startIndex = 0)
        {
            return Line.GetLineOnCondition(lines, condition, out int lineIndex, startIndex);
        }
        /// <summary> 
        /// Returns first line that passes the condition. out lineIndex: the index of the retured line. StartIndex: line index to start looking.
        /// </summary>
        public Line GetLineOnCondition(Condition condition, out int lineIndex, int startIndex = 0)
        {
            return Line.GetLineOnCondition(lines, condition, out lineIndex, startIndex);
        }
        /// <summary> 
        /// Returns first line with the given name.
        /// </summary>
        public Line GetLineWithName(string name)
        {
            return GetLineOnCondition(new Condition.NameIs(name));
        }
        /// <summary> 
        /// Returns lines that pass the condition.
        /// </summary>
        public List<Line> GetLinesOnCondition(Condition condition)
        {
            List<Line> list = Line.GetLinesOnConditon(lines, condition);
            return list;
        }
        /// <summary> 
        /// Returns the field of the first line that passes the given condition.
        /// </summary>
        public string GetFieldOnCondition(int fieldIndex, Condition condition)
        {
            return Line.GetFieldOnCondition(fieldIndex, lines, condition);
        }
        /// <summary> 
        /// Returns the fields of all lines that passes the given condition.
        /// </summary>
        public string[] GetFieldsOnCondition(int fieldIndex, Condition condition)
        {
            return Line.GetFieldsOnCondition(fieldIndex, lines, condition);
        }
        /// <summary> 
        /// Gets the the given field of all lines, parsed into ints.
        /// </summary>
        public int[] GetIntFields(int fieldIndex)
        {
            return Line.GetIntFields(fieldIndex, lines);
        }
        /// <summary> 
        /// Gets the the given field of all lines.
        /// </summary>
        public string[] GetFields(int fieldIndex)
        {
            return Line.GetFields(fieldIndex, lines);
        }
        /// <summary> 
        /// Gets the the given fields of all lines.
        /// </summary>
        public string[,] GetFields(int[] fieldIndexes)
        {
            var ret = new string[fieldIndexes.Length, lines.Count];
            for (int i = 0; i < fieldIndexes.Length; i++)
            {
                var fs = Line.GetFields(fieldIndexes[i], lines);
                for (int x = 0; x < fs.Length; x++)
                {
                    ret[i, x] = fs[x];
                }
            }
            return ret;
        }
        /// <summary> 
        /// Gets the given field of the first line with the given line name.
        /// </summary>
        public string GetFieldWithLineName(int fieldIndex, string linename)
        {
            Condition condition = new Condition.FieldIs(1, linename);
            return Line.GetFieldOnCondition(fieldIndex, lines, condition);
        }
        /// <summary> 
        /// Gets the given fields of the lines with the given line names.
        /// </summary>
        public string[] GetFieldsWithLineNames(int fieldIndex, string[] linenames)
        {
            Condition condition = new Condition.FieldIs(1, linenames);
            return Line.GetFieldsOnCondition(fieldIndex, lines, condition);
        }
        /// <summary> 
        /// Gets the given field of the line with the given id.
        /// </summary>
        public string GetFieldWithLineID(int fieldIndex, string id)
        {
            Condition condition = new Condition.FieldIs(0, id);
            return Line.GetFieldOnCondition(fieldIndex, lines, condition);
        }
        public int[] GetIDs(int adj = 0, int mult = 1)
        {
            var ret = Util.ToInts(Line.GetFields(0, lines));
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] += adj;
                ret[i] *= mult;
            }
            return ret;

        }
        /// <summary> 
        /// Gets the ID of the lines that pass the given condifiton.
        /// </summary>
        public int[] GetIDsWithCondition(Condition condition)
        {
            return Util.ToInts( Line.GetFieldsOnCondition(0, lines, condition));
        }
        /// <summary> 
        /// Gets the ID of the lines with the given line names.
        /// </summary>
        public int[] GetIDsWithLineName(string[] names)
        {
            Condition condition = new Condition.FieldIs(1, names);
            return Util.ToInts(Line.GetFieldsOnCondition(0, lines, condition));
        }
        /// <summary> 
        /// Gets the ID of the line with the given line names.
        /// </summary>
        public int[] GetIDsWithLineName(string name)
        {
            Condition condition = new Condition.FieldIs(1, name);
            return Util.ToInts(Line.GetFieldsOnCondition(0, lines, condition));
        }

        /*List<Line> _orderedLines;
        bool _orderedLinesQued = true;
        List<Line> OrderedLines
        {
            get
            {
                if (_orderedLinesQued)
                    _orderedLines = GetOrderedLines();
                return _orderedLines;
            }
        }*/

        public List<Line> GetOrderedLines()
        {
            int largestId = -1;
            List<Line> ret = new List<Line>();

            for (int curIndex = 0; curIndex < lines.Count; curIndex++)
            {
                int id = lines[curIndex].id_int;

                if (largestId < id)
                {
                    largestId = id;
                    ret.Add(lines[curIndex]);
                }else if(largestId == id)
                {
                    Util.println("duplicated id in " + lines[curIndex].file.paramName + ":" +lines[curIndex].id) ;
                }
                else
                {
                    int foundId = Line.GetNextFreeId(ret, id ,out int out_index, 0, true);
                    if (out_index == curIndex)
                        Util.p();//bruh wtf?
                    else
                    {
                        ret.Insert(out_index, lines[curIndex]);
                    }
                }
            }
            return ret;
        }

        /// <summary> 
        /// Gets the next id available for a new line found after the given id. Useful for adding new lines to an itemLot, or adding a new line under anouther line.
        /// Inclusive: can return the line of the given id. StartIndex: the line index to start looking at.
        /// </summary>
        public int GetNextFreeId(int id, bool inclusive = false, int startIndex = 0)
        {
            return GetNextFreeId(id, out int index, startIndex, inclusive);
        }
        public int GetNextFreeId(int id, bool inclusive, out int index, int startIndex = 0)
        {
            return GetNextFreeId(id, out index, startIndex, inclusive);
        }

        public int GetNextFreeIdIntreval(int startingId, bool inclusive, int searchInterval, int startIndex, out int lastLineIndex, List<int> IdsToExclude)
        {
            if (inclusive)
                startingId -= searchInterval;
            int newItemLotID = startingId;
                
            int newNextLineID = -1;
            //if(newItemLotID == -1)
            //Util.println("newItemLotID: " + newItemLotID + npcLine.id + ":" + npcLine.name + "  " + newItemLotID);
            int nextLineIndex = 0;
            int newLineIndex = Math.Max(0,startIndex);
            //find new line ID

            while ((IdsToExclude != null && IdsToExclude.Contains(newItemLotID)) || this.GetLineWithId(newItemLotID, out newLineIndex, newLineIndex) != null || newNextLineID < newItemLotID + searchInterval)
            {
                newItemLotID = this.GetNextFreeId(newItemLotID, out newLineIndex);
                newItemLotID += searchInterval - (newItemLotID % searchInterval);
                newNextLineID = this.GetNextLine(newItemLotID, out nextLineIndex, nextLineIndex).id_int;
                /*Console.WriteLine(npcLine._idName +
                    Util.IndentedText("start:" + startingSearchForLocationItemLotID, 25) +
                    Util.IndentedText("newItemLotID:" + newItemLotID, 25) +
                    Util.IndentedText("   newNextLineID:" + newNextLineID, 25));
                    */
            }
            lastLineIndex = newLineIndex;
            return newItemLotID;
        }
        /// <summary> 
        /// Gets the next id available for a new line found after the given id. Useful for adding new lines to an itemLot, or adding a new line under anouther line.
        /// out nextLineIndex: the line index of the returned line. Inclusive: can return the line of the given id. StartIndex: the line index to start looking at.
        /// </summary>
        public int GetNextFreeId(int id, out int nextLineIndex, int startIndex = 0, bool inclusive = false)
        {
            if (inclusive)
                id--;
            bool foundId = false;
            int count = lines.Count;

            id = Math.Max(id, lines[startIndex].id_int);
            int nextValid = id + 1;
            int i = startIndex;
            int iid;
            for (;i < count; i++)
            {  
                iid = lines[i].id_int;
                //if (!foundId && (iid == id || iid == nextValid))
                //    foundId = true;

                //if (foundId)
                {
                    if (iid > id)
                    {
                        if (iid == nextValid)
                        {
                            nextValid++;
                        }
                        else if (iid > nextValid)
                        {
                            break;
                        }
                    }
                }
            }
            nextLineIndex = i;
            return nextValid;
        }
        
        /// <summary> 
        /// Returns the set of ids available for a new line for each of the given lines. Useful for adding new lines to an itemLots in bulk, or adding a new line under anouther line.
        /// Inclusive: can return the line of the given id.
        /// </summary>
        public int[] GetNextFreeIds(bool inclusive = false)
        {
            int[] ret = new int[lines.Count];
            for (int i = 0; i < lines.Count; i++)
            {
                ret[i] = GetNextFreeId(lines[i].id_int, inclusive, 0);
            }
            return ret;
        }
        /// <summary> 
        /// Gets the next line found after the given id. Inclusive: can return the line of the given id. StartIndex: the line index to start looking at.
        /// </summary>
        public Line GetNextLine(int id, bool inclusive = false, int startIndex = 0)
        {
            return GetNextLine(id, out int index, startIndex, inclusive);
        }
        /// <summary> 
        /// Gets the next line found after the given id. out Index: the line index of the returned line. Inclusive: can return the line of the given id. StartIndex: the line index to start looking at.
        /// </summary>
        public Line GetNextLine(int id, out int index, int startIndex = 0, bool inclusive = false)
        {
            if (inclusive)
                id--;
            index = -1;
            startIndex = Math.Max(0, startIndex);

            for (int i = startIndex; true; i++)
            {
                int iid = lines[i].id_int;
                if (iid > id)
                {
                    index = i;
                    return lines[i];
                }
            }
        }




        /// <summary> 
        /// Prints all lines in this container.
        /// </summary>
        public void PrintLines()
        {
            foreach (var line in lines)
            {
                line.Print();
            }
        }
        /// <summary> 
        /// Prints all modified lines in this container.
        /// </summary>
        public void PrintModifiedLines()
        {
            foreach (var line in lines)
            {
                if (line.modified)
                    line.Print();
            }
        }
        public Lines ModifiedLines
        {
            get
            {
                return GetModifiedLines();
            }
        }
        Lines GetModifiedLines()
        {
            List<Line> lines = new List<Line>();
            foreach (var line in this.lines)
            {
                if (line.modified)
                    lines.Add(line);
            }
            return new Lines(lines);
        }
        /// <summary> 
        /// Prints a comparison of all line changes. Useful for debugging all changes in a csv.
        /// </summary>
        public void PrintCompareLineChanges(Line lineToCompare, List<int> fieldIndexesToExclude = null)
        {
            foreach (var line in lines)
            {
                if (line.modified)
                {
                    string s = line.id +"; "+ Util.IndentedText(line.name, 50);
                    Util.println(s);
                    if (lineToCompare == null)
                    {
                        foreach (int i in line.modifiedFieldIndexes)
                        {
                            if (fieldIndexesToExclude != null && fieldIndexesToExclude.Contains(i))
                                continue;
                            s = Util.IndentedText("");
                            s += Util.IndentedText(line.file.header[i] + ":", 30);
                            if (!line.added)
                                s += Util.IndentedText(Util.IndentedText(line.vanillaLine.GetField(i), 3) + "  -->  " + line.GetField(i), 20);
                            Util.println(s);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < line.file.header.Length-1; i++)
                        {
                            if (fieldIndexesToExclude != null && fieldIndexesToExclude.Contains(i))
                                continue;
                            if (lineToCompare.GetField(i) == line.GetField(i))
                                continue;
                            s = Util.IndentedText("");
                            s += Util.IndentedText(line.file.header[i] + ":", 30);
                            s += Util.IndentedText(Util.IndentedText(lineToCompare.GetField(i), 3) + "  -->  " + line.GetField(i), 20);
                            Util.println(s);
                        }
                    }
                    Util.println();
                }
            }
        }
        /// <summary> 
        /// Prints all IDs and NAMEs of lines in this container. "[ID][DELIMITER][NAME]"
        /// </summary>
        public void PrintIDAndNames(string delimiter = ";")
        {
            Util.PrintStrings(Util.Append(GetFields(0), delimiter, GetFields(1)));
        }
        public void PrintIDAndField(int fieldIndex, string delimiter = ";")
        {
            Util.PrintStrings(Util.Append(GetFields(0), delimiter, GetFields(fieldIndex)));
        }
        public void PrintModifiedIDAndField(int fieldIndex, string delimiter = ";")
        {
            var lines =(Lines)GetLinesOnCondition(new Condition.IsFieldModified(fieldIndex));
            Util.PrintStrings(Util.Append(lines.GetFields(0), delimiter, lines.GetFields(1)));
        }
        /// <summary> 
        /// Prints field for of all lines in this container.
        /// </summary>
        public void PrintFieldIndex(int fieldindex)
        {
            Util.PrintStrings(GetFields(fieldindex));
        }
        /// <summary> 
        /// Prints fields for of all lines in this container.
        /// </summary>
        public void PrintFieldIndexes(int fieldIndex0,int fieldIndex1, string delimiter = ";")
        {
            Util.PrintStrings(Util.Append(GetFields(fieldIndex0), delimiter, GetFields(fieldIndex1)));
        }
        /// <summary> 
        /// Prints fields for of all lines in this container.
        /// </summary>
        public void PrintFieldIndexes(int[] fieldiIndexes, string delimiter = ";")
        {
            if (fieldiIndexes.Length == 0)
                return;
            string[] output = GetFields(fieldiIndexes[0]);
            for(int i = 1; i < fieldiIndexes.Length; i++)
            {
                output = Util.Append(output, delimiter, GetFields(fieldiIndexes[i]));
            }
            //Util.Append(output, ";");
            Util.PrintStrings(output); ; ;
        }



    }
    public class Line : Themescaped
    {

        public Line addKW(string keyword, int scale = 100)
        {
            return addKW(new Keyword(keyword, scale));
        }
        public Line addKW(Keyword keyword)
        {
            keywords.Add(keyword);
            return this;
        }
        public ParamFile file
        {
            get;
        }

        string _line;
        public string line
        {
            get
            {
                return GetLine();
            }
            /*set
            {
                SetLine(value);
            }*/
        }

        bool _extraLineCheckQued = false;    //we assume everything starts as a copy of a vanilla line. so the check is false.
        bool _wasExtraLineLastCheck = false;
        /// <summary> 
        /// If line was added rather than modified. Compares vanillla param.
        /// </summary>
        public bool IsExtraLine
        {
            get
            {
                if (_extraLineCheckQued)
                {
                    //finds first id in vannilla file lines that is greater or equal to id, if not equal we know its an extra line.
                    Line vanillaIdLine = file.vanillaParamFile.GetLineOnCondition(new Condition.FloatCompare(new FloatFieldRef(0), Condition.GREATER_THAN_OR_EQUAL_TO, id_int));
                    if (vanillaIdLine == null)
                        _wasExtraLineLastCheck = false;
                    else
                        _wasExtraLineLastCheck = vanillaIdLine.id != id;
                }
                return _wasExtraLineLastCheck;
            }
        }
        public static bool IsAdded(Line line)
        {
            return line.added;
        }
        public static bool IsModified(Line line)
        {
            return line.modified;
        }
        public static bool IsFieldModified(int fieldindex, Line line)
        {
            return line.modifiedFieldIndexes.Contains(fieldindex);
        }
        public static bool IsExtraLineStatic(Line line)
        {
            return line.IsExtraLine;
        }
        public static bool IsInFile(Line line)
        {
            return line.inFile;
        }
        public Line vanillaLine { get; private set; }
        public bool inFile {
            get;
            private set;
        } = false;
        public bool added
        {
            get;
            private set;
        } = false;
        public bool modified { get; private set; } = false;

        public bool isVanillaLine = false;

        public void TurnFakeVanilla()
        {
            this.added = false;
            this.inFile = true;
            this.vanillaLine = null;
            this.modifiedFieldIndexes.Clear();
            this.modified = false;
            this.ClearKeywords();
        }


        public void UpdateModificationStatus()
        {
            vanillaLine = file.vanillaParamFile.GetLineWithId(id_int);
            if (vanillaLine == null) {
                added = true;
                modified = true;
                return;
            }
            for(int FI = 0; FI < file.header.Length-1; FI++)
            {
                if(GetField(FI) != vanillaLine.GetField(FI))
                {
                    modified = true;
                    modifiedFieldIndexes.Add(FI);
                }
            }
        }
        /// <summary> 
        /// marks this line as modified, even if its not. Ueful for printing it alongside the modified lines.
        /// </summary>
        public void MarkModified(bool addToFileModifiedAmount)
        {
            modified = true;
            if (addToFileModifiedAmount && inFile)
                file.numberOfModifiedOrAddedLines++;
        }
        /// <summary> 
        /// all fieldIndex of fields that have been modified in this line.
        /// </summary>
        public List<int> modifiedFieldIndexes { get; private set; } = new List<int>();

        /// <summary> 
        /// all field substrings of this line.
        /// </summary>
        string[] _data;
        /*
        public string[] data
        {
            get
            {
                return GetData();
            }
            set
            {
                SetLine(value);
            }
        }*/

        /// <summary> 
        /// returns field 0 which always denotes the ID of a line.
        /// </summary>
        public string id
        {
            get
            {
                return GetData()[0];
            }
        }
        /// <summary> 
        /// returns field 0 which always denotes the ID of a line, but parsed as an int.
        /// </summary>
        public int id_int
        {
            get
            {
                return int.Parse(GetData()[0]);
            }
        }
        /// <summary> 
        /// returns field 1 which always denotes the Name of a line, 
        /// </summary>
        public string name
        {
            get
            {
                return GetData()[1];
            }
        }
        /// <summary> 
        /// returns the ID and the Name of the line, useful for debuging a line.
        /// </summary>
        public string _idName
        {
            get
            {
                return GetData()[0] + ":" + GetData()[1];
            }
        }

        bool queUpdateData;
        bool queUpdateLine;
        


        /// <summary> 
        /// create a copy of this line.
        /// </summary>
        public Line Copy()
        {
            var copy = new Line(this.line, file);
            if (vanillaLine == null)
                copy.vanillaLine = this;
            else
                copy.vanillaLine = vanillaLine;
            copy.modified = modified;
            copy.modifiedFieldIndexes = new List<int>(modifiedFieldIndexes);
            copy.inFile = false;
            copy.added = false;
            foreach(Keyword k in keywords)
            {
                if(k.copyable)
                    copy.keywords.Add(k);
            }
            return copy;
        }
        /// <summary> 
        /// create a copy of this line but with an assiged parent file.
        /// </summary>
        public Line Copy(ParamFile parentFile, bool inFile = false)
        {
            var copy = new Line(this.line, parentFile);
            if (vanillaLine == null)
                copy.vanillaLine = this;
            else
                copy.vanillaLine = vanillaLine;
            copy.modified = modified;
            copy.modifiedFieldIndexes = new List<int>(modifiedFieldIndexes);
            copy.inFile = inFile;
            copy.added = added;
            foreach (Keyword k in keywords)
            {
                if (k.copyable)
                    copy.keywords.Add(k);
            }
            return copy;
        }

        public Line(string line, ParamFile paramFile)
        {
            file = paramFile;
            this._line = line;
            this._data = CreateData(line, file.delimiter);
            queUpdateData = false;
            queUpdateLine = false;
        }

        /// <summary> 
        /// Set a field of this line with a given field name. 
        /// </summary>
        public Line SetField(string fieldName, float setTo)
        {
            return SetField(file.GetFieldIndex(fieldName), Util.FloatTo3DecimalPlaces(setTo).ToString());
        }
        /// <summary> 
        /// Set a field of this line with a given field name. 
        /// </summary>
        public Line SetField(string fieldName, int setTo)
        {
            return SetField(file.GetFieldIndex(fieldName), (setTo).ToString());
        }
        /// <summary> 
        /// Set a field of this line with a given field name. 
        /// </summary>
        public Line SetField(string fieldName, string setTo)
        {
            return SetField(file.GetFieldIndex(fieldName), setTo);
        }
        /// <summary> 
        /// Set a field of this line with a given field name. 
        /// </summary>
        public Line SetField(string fieldName, bool setTo)
        {
            string s = "0";
            if (setTo)
                s = "1";
            return SetField(file.GetFieldIndex(fieldName), s);
        }

        /// <summary> 
        /// Set a field of this line with a given field index. 
        /// </summary>
        /// 
        public Line SetField(int fieldIndex, string setTo )
        {
            if (isVanillaLine)
            {
                Util.p(); //this should never happen
                Debug.Fail("attempted to modify a vanilla line: "     + id + ":" + name);
            }
            if (setTo == _data[fieldIndex])
                return this;
            //if (file.filename == "ItemLotParam_enemy.csv" && fieldIndex == Program.ItemLotParam_enemy.GetFieldIndex("lotItemBasePoint01") && id_int == 407100103)
            //    Util.p();
            //if (file.filename == "NpcParam.csv" && fieldIndex == Program.NpcParam.GetFieldIndex("itemLotId_enemy") && id_int == 38500020)
            //    Util.p();
            if (fieldIndex == 0)
            {
                _extraLineCheckQued = true;
                vanillaLine = null;
            }
            _data[fieldIndex] = setTo;
            queUpdateLine = true;
            queUpdateData = false;
            if (!modified)
            {
                modified = true;
                if(inFile)
                 file.numberOfModifiedOrAddedLines++;
            }
            if (!modifiedFieldIndexes.Contains(fieldIndex))
            {
                modifiedFieldIndexes.Add(fieldIndex);
                file.numberOfModifiedFields++;
            }
            if (modified && Keyword.IfModifiedSet_ON && !hasKeyword(Keyword.IfModifiedSet))
            {
                addKeyword(Keyword.IfModifiedSet);
            }
            return this;
        }
        /// <summary> 
        /// Set a field of this line with a given field index. 
        /// </summary>
        public Line SetField(int fieldIndex, float setTo)
        {
            return SetField(fieldIndex, Util.FloatTo3DecimalPlaces(setTo).ToString());
        }
        /// <summary> 
        /// Set a field of this line with a given field index. 
        /// </summary>
        public Line SetField(int fieldIndex, int setTo)
        {
            return SetField(fieldIndex, (setTo).ToString());
        }
        /// <summary> 
        /// Set a field of this line with a given field index. 
        /// </summary>
        public Line SetField(int fieldIndex, bool setTo)
        {
            string s = "0";
            if (setTo)
                s = "1";
            return SetField(fieldIndex, s);
        }
        /// <summary> 
        /// Get a field of this line with a given field index. 
        /// </summary>
        public string GetField(int fieldIndex)
        {
            return GetData()[fieldIndex];
        }
        /// <summary> 
        /// Get a field of this line with a given field index parsed as an int.
        /// </summary>
        public int GetFieldAsInt(int fieldIndex)
        {
            return int.Parse(GetData()[fieldIndex]);
        }
        /// <summary> 
        /// Get a field of this line with a given field index parsed as a float.
        /// </summary>
        public float GetFieldAsFloat(int fieldIndex)
        {
            return float.Parse(GetData()[fieldIndex]);
        }
        /// <summary> 
        /// Get a field of this line with a given field index parsed as an int.
        /// </summary>
        public int GetFieldAsInt(string fieldName)
        {
            return int.Parse(GetData()[file.GetFieldIndex(fieldName)]);
        }
        /// <summary> 
        /// Get a field of this line with a given field index parsed as a float.
        /// </summary>
        public float GetFieldAsFloat(string fieldName)
        {
            return float.Parse(GetData()[file.GetFieldIndex(fieldName)]);
        }
        /// <summary> 
        /// Get a field of this line with a given field name. 
        /// </summary>
        public string GetField(string fieldName)
        {
            return GetData()[file.GetFieldIndex(fieldName)];
        }
        /*
        /// <summary> 
        /// Override this line with the given string.
        /// </summary>
        void SetLine(string line)
        {
            queUpdateLine = false;
            queUpdateData = true;
            this._line = line;
        }
        /// <summary> 
        /// Override this line with the given data.
        /// </summary>
        public void SetLine(string[] data)
        {
            queUpdateLine = true;
            queUpdateData = false;
            this._data = data;
            //this.line = DataToLine(data);
        }
        */
        /// <summary> 
        /// Revert the given field to vanilla
        /// </summary>
        public void RevertFieldToVanilla(int fieldIndex)
        {
            SetField(fieldIndex, vanillaLine.GetField(fieldIndex));
            modifiedFieldIndexes.Remove(fieldIndex);
        }
        public void RevertFieldsToVanilla(bool clearKeywords = false)
        {
            if (clearKeywords)
                ClearKeywords();
            for (int i = 0; i < _data.Length; i++)
            {
                SetField(i, vanillaLine.GetField(i));
                modifiedFieldIndexes.Remove(i);
                
            }
        }
        /// <summary> 
        /// Revert the given field to vanilla
        /// </summary>
        public void RevertFieldToVanilla(string fieldName)
        {
            int fieldIndex = file.GetFieldIndex(fieldName);
            SetField(fieldIndex, vanillaLine.GetField(fieldIndex));
            modifiedFieldIndexes.Remove(fieldIndex);
        }
        /// <summary> 
        /// Gets the line as a string
        /// </summary>
        string GetLine()
        {
            if (queUpdateLine)
            {
                queUpdateLine = false;
                _line = DataToLine(_data, file.delimiter);
            }
            return _line;
        }
        /// <summary> 
        /// Gets the line as a string using the Write_Delimiter
        /// </summary>
        public string ToWrite()
        {
            return DataToLine(GetData(),RunSettings.Write_Delimiter);
        }
        /// <summary> 
        /// Gets the fieldIndexes of this line under a certain field condition
        /// </summary>
        public int[] GetFieldIndexesOnCondition(int[] fieldIndexes, Condition.FieldCondition fCondition)
        {
            return GetFieldIndexesOnCondition(fieldIndexes, fCondition, out int rand);
        }
        /// <summary> 
        /// Gets the fieldIndexes of this line under a certain field condition. Out arrayIndex: returns the field's index within the array
        /// </summary>
        public int[] GetFieldIndexesOnCondition(int[] fieldIndexes, Condition.FieldCondition fCondition, out int arrayIndex)
        {
            List<int> indexes = new List<int>();
            arrayIndex = -1;
            for (int i = 0; i < fieldIndexes.Length; i++)
            {
                fCondition.field = fieldIndexes[i];
                if (fCondition.Pass(this))
                {
                    arrayIndex = i;
                    indexes.Add(fieldIndexes[i]);
                }
            }
            
            return indexes.ToArray();
        }
        /// <summary> 
        /// Gets the first fieldIndex of this line under a certain field condition.
        /// </summary>
        public int GetFirstFieldIndexOnCondition(int[] fieldIndexes, Condition.FieldCondition fCondition)
        {
            return GetFirstFieldIndexOnCondition(fieldIndexes, fCondition, out int rand);
        }
        /// <summary> 
        /// Gets the first fieldIndex of this line under a certain field condition. Out arrayIndex: returns the field's index within the array
        /// </summary>
        public int GetFirstFieldIndexOnCondition(int[] fieldIndexes, Condition.FieldCondition fCondition, out int arrayIndex)
        {
            for (int i = 0; i < fieldIndexes.Length; i++)
            {
                fCondition.field = fieldIndexes[i];
                if (fCondition.Pass(this))
                {
                    arrayIndex = i;
                    return fieldIndexes[i];
                }
            }
            arrayIndex = -1;
            return -1;
        }

        /// <summary> 
        /// Returns the field data of this line.
        /// </summary>
        public string[] GetData()
        {
            if (queUpdateData)
            {
                queUpdateData = false;
                _data = CreateData(_line, file.delimiter);
            }
            return _data;
        }
        /// <summary> 
        /// Gets the next id available for a new line found after this line's id. Useful for adding new lines after this line.
        /// Inclusive: can return the line of the given id. StartIndex: the line index to start looking at.
        /// </summary>
        public int GetNextFreeId(bool inclusive = false)
        {
            ParamFile p = this.file;
            return p.GetNextFreeId(this.id_int, inclusive);
        }

       

        /// <summary> 
        /// Splits the string into data using the given line string and delimiter.
        /// </summary>
        public static string[] CreateData(string line, char delimiter)
        {
            return line.Split(delimiter);
        }
        /// <summary> 
        /// Joins data into a string line string using the given delimiter.
        /// </summary>
        public static string DataToLine(string[] data, char delimiter)
        {
            return string.Join(delimiter.ToString(), data);
        }
        /// <summary> 
        /// Joins data into a string line string using the given delimiter.
        /// </summary>
        public Line Operate(LineModifier operation)
        {
            operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies on this line using a line modifier only if it passes the condition
        /// </summary>
        public Line Operate(LineModifier operation, Condition condition)
        {
            if(condition.Pass(this))
                operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies on a specific field using a field modifier
        /// </summary>
        public Line OperateField(int fieldIndex, FieldModifier operation)
        {
            operation.SetFieldIndexTo(fieldIndex);
            operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies on a specific field using a field modifier only if the line passes the condition
        /// </summary>
        public Line OperateField(int fieldIndex, FieldModifier operation, Condition condition)
        {
            operation.SetFieldIndexTo(fieldIndex);
            if (condition.Pass(this))
                operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies specific fields using a field modifier
        /// </summary>
        public Line OperateFields(int[] fieldIndexes, FieldModifier operation)
        {
            operation.SetFieldIndexesTo(fieldIndexes);
            operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies specific fields using a field modifier only if the line passes the condition
        /// </summary>
        public Line OperateFields(int[] fieldIndexes, FieldModifier operation, Condition condition)
        {
            operation.SetFieldIndexesTo(fieldIndexes);
            if (condition.Pass(this))
                operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies on a specific field using a field modifier
        /// </summary>
        public Line OperateField(string fieldName, FieldModifier operation)
        {
            operation.SetFieldIndexTo(file.GetFieldIndex(fieldName));
            operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies on a specific field using a field modifier only if the line passes the condition
        /// </summary>
        public Line OperateField(string fieldName, FieldModifier operation, Condition condition)
        {
            operation.SetFieldIndexTo(file.GetFieldIndex(fieldName));
            if (condition.Pass(this))
                operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies specific fields using a field modifier
        /// </summary>
        public Line OperateFields(string[] fieldNames, FieldModifier operation)
        {
            operation.SetFieldIndexesTo(file.GetFieldIndexes(fieldNames));
            operation.Operate(this);
            return this;
        }
        /// <summary> 
        /// Modifies specific fields using a field modifier only if the line passes the condition
        /// </summary>
        public Line OperateFields(string[] fieldNames, FieldModifier operation, Condition condition)
        {
            operation.SetFieldIndexesTo(file.GetFieldIndexes(fieldNames));
            if (condition.Pass(this))
                operation.Operate(this);
            return this;
        }


        /// <summary> 
        /// returns the number of fields in this
        /// </summary>
        public static  int GetFieldCount(string line, char delimiter)
        {
            int n = line.Split(delimiter).Length;
            if (line[line.Length - 1] == delimiter)
                n--;
            return n;
        }
        /// <summary> 
        /// print this line
        /// </summary>
        public void Print()
        {
            Util.println(line);
        }
        /// <summary> 
        /// print every field of this line next to the field name.
        /// </summary>
        public void PrintFieldsWithFieldName()
        {
            for (int i = 0; i < file.header.Length-1; i++)
            {
                Util.println(Util.IndentedText(file.header[i], 12, '_') + GetField(i));
            }
        }
        /// <summary> 
        /// print specified fields in this line next to the field name.
        /// </summary>
        public void PrintFieldsWithFieldName(int[] fieldIndexes)
        {
            for (int i = 0; i < fieldIndexes.Length; i++)
            {
                string _head = file.header[fieldIndexes[i]];
                Util.println(Util.IndentedText(_head, 12, '_')+ GetField(fieldIndexes[i]));
            }
        }
        /// <summary> 
        /// print specified fields in this line next to the field name.
        /// </summary>
        public void PrintFieldsWithFieldName(int i0, int i1 = -1, int i2 = -1, int i3 = -1, int i4 = -1, int i5 = -1, int i6 = -1, int i7 = -1, int i8 = -1, int i9 = -1, int i10 = -1)
        {
            List<int> iss = new List<int>();
            iss.Add(i0);
            if (i1 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i1);
            if (i2 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i2);
            if (i3 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i3);
            if (i4 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i4);
            if (i5 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i5);
            if (i6 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i6);
            if (i7 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i7);
            if (i8 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i8);
            if (i9 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i9);
            if (i10 == -1) { PrintFieldsWithFieldName(iss.ToArray()); return; }
            iss.Add(i10);
        }
        /// <summary> 
        /// Returns given lines that pass the condition.
        /// </summary>
        public static List<Line> GetLinesOnConditon(List<Line> lines, Condition condition)
        {
            List<Line> newLines = new List<Line>();
            foreach (Line line in lines)
            {
                if (!condition.Pass(line))
                    continue;
                //Utility.println("new lines "+newLines.Count);
                newLines.Add(line);
            }
            //Utility.println(newLines.Count + " " + lines.Count);
            return newLines;
        }
        /// <summary> 
        /// Returns first line that passes the condition. StartIndex: line index to start looking.
        /// </summary>
        public static Line GetLineOnCondition(List<Line> lines, Condition condition, int startIndex = 0)
        {
            return GetLineOnCondition(lines, condition, out int lineIndex, startIndex);
        }
        /// <summary> 
        /// Returns first line that passes the condition. out lineIndex: the index of the retured line. StartIndex: line index to start looking.
        /// </summary>
        public static Line GetLineOnCondition(List<Line> lines, Condition condition, out int lineIndex, int startIndex = 0)
        {
            lineIndex = -1;
            startIndex = Math.Max(0, startIndex);
            for (int i = startIndex; i < lines.Count; i++)
            {
                Line line = lines[i];
                if (!condition.Pass(line))
                {
                    continue;
                }
                lineIndex = i;
                return (line);
            }
            return null;
        }
        /// <summary> 
        /// Returns the fields of the given lines that passes the given condition.
        /// </summary>
        public static string[] GetFieldsOnCondition(int fieldIndex, List<Line> lines, Condition condition)
        {
            var selected = GetLinesOnConditon(lines, condition);
            var fields = new string[selected.Count];
            int i = 0;
            foreach (var line in selected)
            {
                fields[i] = line.GetField(fieldIndex);
                i++;
            }
            return fields;
        }
        /// <summary> 
        /// Returns the field of first line that passes the given condition.
        /// </summary>
        public static string GetFieldOnCondition(int fieldIndex, List<Line> lines, Condition condition)
        {
            return GetLineOnCondition(lines, condition).GetField(fieldIndex);
        }
        /// <summary> 
        /// Returns the given field of the given lines as a string[].
        /// </summary>
        public static string[] GetFields(int fieldIndex, List<Line> lines)
        {
            var fields = new string[lines.Count];
            int i = 0;
            foreach (var line in lines)
            {
                fields[i] = line.GetField(fieldIndex);
                i++;
            }
            return fields;
        }
        /// <summary> 
        /// Returns the given field of the given lines but parsed into ints.
        /// </summary>
        public static int[] GetIntFields(int fieldIndex, List<Line> lines)
        {
            var fields = new int[lines.Count];
            int i = 0;
            foreach (var line in lines)
            {
                fields[i] = int.Parse(line.GetField(fieldIndex));
                i++;
            }
            return fields;
        }
        /// <summary> 
        /// Returns the bool of if the given condition passes for each given line as a bool[].
        /// </summary>
        public static bool[] GetBoolsConditionPass(List<Line> lines, Condition condition)
        {
            var bools = new bool[lines.Count];
            for (int i = 0; i < bools.Length; i++)
            {
                if (condition.Pass(lines[i]))
                    bools[i] = true;
                else
                    bools[i] = false;
            }
            return bools;
        }
        /// <summary> 
        /// Currently doenst work use OVerrideOrAddLine
        /// </summary>
        public static List<Line> OverrideLines(List<Line> lines, List<Line> overrideLines, bool preOrderedLines = true)
        {
            List<Line> newLines = new List<Line>();

            string[] modifiedLinesIDs = new string[overrideLines.Count];

            for (int i = 0; i < overrideLines.Count; i++)
            {
                modifiedLinesIDs[i] = lines[i].GetData()[0];
            }

            int startingIndex = 0;
            int lookingFor = 0;
            for (int l = startingIndex; l < lines.Count; l++)
            {
                string[] data = lines[l].GetData();

                if (preOrderedLines)
                {
                    if (overrideLines.Count < lookingFor && data[0] == modifiedLinesIDs[lookingFor])
                    {
                        newLines.Add(overrideLines[lookingFor]);
                        lookingFor++;
                        continue;
                    }
                    else
                    {
                        newLines.Add(lines[l]);
                    }
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < modifiedLinesIDs.Length; i++)
                    {
                        if (data[0] == modifiedLinesIDs[i])
                        {
                            newLines.Add(overrideLines[i]);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        newLines.Add(lines[l]);
                    }
                }
            }
            return newLines;
        }
        /// <summary> 
        /// Currently doenst work use OVerrideOrAddLine
        /// </summary>
        public static List<Line> OverrideOrAddLines(List<Line> lines, List<Line> overrideLines, bool preOrderedLines = true, ParamFile fileInto = null)
        {
            bool changingFile = fileInto != null;
            List<Line> newLines = new List<Line>();
            
            string[] modifiedLinesIDs = new string[overrideLines.Count];

            for (int i = 0; i < overrideLines.Count; i++)
            {
                modifiedLinesIDs[i] = overrideLines[i].GetData()[0];
            }

            //if(lines[0] != null && lines[0].file!= null)
            //    lines[0].file.numberOfModifiedOrAddedLines += overrideLines.Count;

            int startingIndex = 0;
            int lookingFor = 0;

            for (int l = startingIndex; l < lines.Count; l++)
            {
                string[] data = lines[l].GetData();

                if (preOrderedLines)
                {
                    bool stillfinding = overrideLines.Count < lookingFor;
                    if (stillfinding && data[0] == modifiedLinesIDs[lookingFor])    //override
                    {
                        newLines.Add(overrideLines[lookingFor]);
                        if (changingFile && lines[l].inFile)
                        {
                            overrideLines[lookingFor].inFile = true;
                            overrideLines[lookingFor].vanillaLine = lines[l].vanillaLine;
                            lines[l].inFile = false;
                            lines[l].added = false;
                            if (lines[l].modified)
                                fileInto.numberOfModifiedFields -= lines[l].modifiedFieldIndexes.Count;
                            if (overrideLines[lookingFor].modified)
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += overrideLines[lookingFor].modifiedFieldIndexes.Count;
                            }
                        }
                        lookingFor++;
                    }
                    else if (stillfinding && int.Parse(data[0]) > int.Parse(modifiedLinesIDs[lookingFor]))  //add
                    {
                        newLines.Add(overrideLines[lookingFor]);
                        if (changingFile)
                        {
                            overrideLines[lookingFor].inFile = true;
                            overrideLines[lookingFor].added = true;
                            overrideLines[lookingFor].vanillaLine = null;
                            if (overrideLines[lookingFor].modified)
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += overrideLines[lookingFor].modifiedFieldIndexes.Count;
                            }
                        }
                        lookingFor++;
                        l--;
                    }
                    else
                    {
                        newLines.Add(lines[l]);
                    }
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < modifiedLinesIDs.Length; i++)
                    {
                        if (data[0] == modifiedLinesIDs[i]) //override
                        {
                            newLines.Add(overrideLines[i]);
                            if (changingFile && lines[l].inFile)
                            {
                                overrideLines[i].inFile = true;
                                overrideLines[i].added = lines[l].added;
                                overrideLines[i].vanillaLine = lines[l].vanillaLine;
                                lines[l].inFile = false;
                                lines[l].added = false;
                                if (lines[l].modified)
                                    fileInto.numberOfModifiedFields -= lines[l].modifiedFieldIndexes.Count;
                                if (overrideLines[i].modified)
                                {
                                    fileInto.numberOfModifiedOrAddedLines++;
                                    fileInto.numberOfModifiedFields += overrideLines[i].modifiedFieldIndexes.Count;
                                }
                            }
                            found = true;
                            break;
                        }
                        else if (int.Parse(data[0]) > int.Parse(modifiedLinesIDs[i]))
                        {
                            newLines.Add(overrideLines[i]); //add
                            if (changingFile)
                            {
                                overrideLines[i].inFile = true;
                                overrideLines[i].added = true;
                                overrideLines[i].vanillaLine = null;
                                if (overrideLines[i].modified)
                                {
                                    fileInto.numberOfModifiedOrAddedLines++;
                                    fileInto.numberOfModifiedFields += overrideLines[i].modifiedFieldIndexes.Count;
                                }
                            }
                            found = true;
                            l--;
                            break;
                        }
                    }
                    if (!found)
                    {
                        newLines.Add(lines[l]);
                    }
                }
            }
            return newLines;
        }

        /// <summary> 
        /// Overrides the line the given lines with the given Overrideline. Replaces the first lines if it shares an id. Also adds the line if it doesnt share an id.
        /// </summary>
        public static List<Line> OverrideOrAddLine(List<Line> lines, Line overrideLine, ParamFile fileInto = null)
        {
            return OverrideOrAddLine(lines, overrideLine, fileInto, out int lineIndex, 0);
        }
            /// <summary> 
            /// Overrides the line the given lines with the given Overrideline. Replaces the first lines if it shares an id. Also adds the line if it doesnt share an id.
            /// </summary>
        public static List<Line> OverrideOrAddLine(List<Line> lines, Line overrideLine, ParamFile fileInto, out int lineIndex, int startIndex = 0)
        {
            bool changingFile = fileInto != null;
            List<Line> newLines = new List<Line>();
            bool found = false;
            lineIndex = -1;
            for (int l = 0; l < lines.Count; l++)
            {
                
                if (!found)
                {
                    if(l < startIndex)
                        newLines.Add(lines[l]);
                    else if (lines[l].id == overrideLine.id) //override line
                    {
                        //lines[l] = overrideLine;
                        newLines.Add(overrideLine);
                        if (changingFile && lines[l].inFile)
                        {
                            overrideLine.inFile = true;
                            overrideLine.added = lines[l].added;
                            overrideLine.vanillaLine = lines[l].vanillaLine;
                            lines[l].inFile = false;
                            lines[l].added = false;
                            if (lines[l].modified)
                                fileInto.numberOfModifiedFields -= lines[l].modifiedFieldIndexes.Count;
                            if (overrideLine.modified)
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += overrideLine.modifiedFieldIndexes.Count;
                            }
                        }
                        lineIndex = l;
                        found = true;
                        //return;
                    }
                    else if (lines[l].id_int > overrideLine.id_int) //add line
                    {
                        //lines.Insert(l, overrideLine);
                        newLines.Add(overrideLine);
                        if (changingFile)
                        {
                            overrideLine.inFile = true;
                            overrideLine.added = true;
                            overrideLine.vanillaLine = null;
                            if (true || overrideLine.modified) //always true
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += overrideLine.modifiedFieldIndexes.Count;
                            }
                        }
                        lineIndex = l;
                        l--;
                        found = true;
                        //return;
                    }
                    else
                    {
                        newLines.Add(lines[l]);
                    }
                }
                else
                {
                    newLines.Add(lines[l]);
                }
            }
            //if (!found)
            //    Util.println("failed to add line " + overrideLine.id + " : " + overrideLine.name);
            if (!found)
            {
                newLines.Add(overrideLine);
                lineIndex = lines.Count;
                if (changingFile)
                {
                    overrideLine.inFile = true;
                    overrideLine.added = true;
                    overrideLine.vanillaLine = null;
                    if (true || overrideLine.modified) //always true
                    {
                        fileInto.numberOfModifiedOrAddedLines++;
                        fileInto.numberOfModifiedFields += overrideLine.modifiedFieldIndexes.Count;
                    }
                }
                found = true;
            }
            return newLines;
        }

        public static void InsertLine(List<Line> lines, Line lineToInsert, out int lineIndex, ParamFile fileInto = null, int startIndex = 0)
        {
            bool changingFile = fileInto != null;
            //List<Line> newLines = new List<Line>();
            bool found = false;
            lineIndex = lines.Count;
            int lastLineId = -1;
            for (int l = startIndex; l < lines.Count; l++)
            {
                if (!found)
                {
                    if (lines[l].id == lineToInsert.id) //override line
                    {
                        //lines[l] = overrideLine;
                        //newLines.Add(lineToInsert);
                        if (changingFile && lines[l].inFile)
                        {
                            lineToInsert.inFile = true;
                            lineToInsert.added = lines[l].added;
                            lineToInsert.vanillaLine = lines[l].vanillaLine;
                            if (lineToInsert.modified)
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += lineToInsert.modifiedFieldIndexes.Count;
                            }
                        }
                        lines.Insert(l, lineToInsert);
                        lineToInsert.SetField(0, lines[l].id);
                        lines[l].SetField(0, lineToInsert.id_int + 1);
                        lineIndex = l;
                        //foreach(Line line in 
                        InsertLine(lines, lines[l], out int newInt, fileInto, l + 1);
                        /*    )
                        {
                            newLines.Add(line);
                        }*/
                        found = true;
                        //return;
                    }
                    else if (lines[l].id_int > lineToInsert.id_int) //add line
                    {
                        lines.Insert(l, lineToInsert);
                        lineToInsert.SetField(0, lastLineId+1);
                        //newLines.Add(lineToInsert);
                        if (changingFile)
                        {
                            lineToInsert.inFile = true;
                            lineToInsert.added = true;
                            lineToInsert.vanillaLine = null;
                            if (lineToInsert.modified)
                            {
                                fileInto.numberOfModifiedOrAddedLines++;
                                fileInto.numberOfModifiedFields += lineToInsert.modifiedFieldIndexes.Count;
                            }
                        }
                        l--;
                        found = true;
                        lineIndex = l;
                        //return;
                    }
                    /*else
                    {
                        newLines.Add(lines[l]);
                    }*/
                }
                lastLineId = lines[l].id_int;
                /*else
                {
                    newLines.Add(lines[l]);
                }*/
            }
            //if (!found)
            //    Util.println("failed to add line " + lineToInsert.id + " : " + lineToInsert.name);
            /*if (!found)
            {
                newLines.Add(lineToInsert);
            }

            Util.println(newLines.Count);*/
            
            //return newLines;
        }

        public static int GetNextFreeId(List<Line> lines, int id, out int nextLineIndex, int startIndex = 0, bool inclusive = false)
        {
            if (inclusive)
                id--;
            //bool foundId = false;
            int nextValid = id + 1;
            int iid = -1;
            for (int i = startIndex; i < lines.Count; i++)
            {

                iid = lines[i].id_int;
                //if (!foundId && (iid == id || iid == nextValid))
               //     foundId = true;

               // if (foundId)
                {
                    if (iid > id)
                    {
                        if (iid == nextValid)
                        {
                            nextValid++;
                        }
                        else if (iid > nextValid)
                        {
                            nextLineIndex = i;
                            return nextValid;
                        }
                    }
                }
            }
            nextLineIndex = lines.Count;
            return iid+1;
        }


    /// <summary> 
    /// Modifies the given lines with the given line modifier. Includes an optional Line Condition. Condition Lines are the lines you wish to run the conditions on in place of the accual lines (Useful in niche senarios)
    /// </summary>
    public static List<Line> Operate(List<Line> lines, LineModifier operation, Condition condition = null, List<Line> conditionLines = null)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (condition == null || (conditionLines != null && condition.Pass(conditionLines[i])) || (conditionLines == null && condition.Pass(lines[i])))
                    lines[i].Operate(operation);
            }
            return lines;
        }
        public static List<Line> Operate(List<Line> lines, LineModifier operation, Condition condition, bool useVanillaLinesForCondition)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var conditionLine = lines[i];
                if (useVanillaLinesForCondition && lines[i].vanillaLine != null)
                    conditionLine =lines[i].vanillaLine;
                if (condition == null || condition.Pass(conditionLine))
                    lines[i].Operate(operation);
            }
            return lines;
        }
        /// <summary> 
        /// Modifies the given lines with the given line modifiers. Includes an optional Line Condition. Condition Lines are the lines you wish to run the conditions on in place of the accual lines (Useful in niche senarios)
        /// </summary>
        public static List<Line> Operate(List<Line> lines, LineModifier[] operations, Condition condition = null, List<Line> conditionLines = null)
        {
            foreach (var op in operations)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (condition == null || (conditionLines != null && condition.Pass(conditionLines[i])) || (conditionLines == null && condition.Pass(lines[i])))
                        op.Operate(lines[i]);
                }
            }
            return lines;
        }        
        public static List<Line> Operate(List<Line> lines, LineModifier[] operations, Condition condition, bool useVanillaLinesForCondition)
        {
            foreach (var op in operations)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    var conditionLine = lines[i];
                    if (useVanillaLinesForCondition && lines[i].vanillaLine != null)
                        conditionLine = lines[i].vanillaLine;
                    if (condition == null || condition.Pass(conditionLine))
                        lines[i].Operate(op);
                }
            }
            return lines;
        }
    }
}
