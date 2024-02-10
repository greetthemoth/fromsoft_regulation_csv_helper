namespace EldenRingCSVHelper
{
    public static class RunSettings
    {
        public static bool Write = true;
        public static bool Write_OnlyModifiedLines= true;
        public static bool Write_CanBeSingleField = true;
        public static bool Write_AllModifiedFiles = false;
        public static char Write_Delimiter = ';';
        public static string Write_directory = @"C:\CODING OUTPUT\CSV";


        public static ParamFile ToRun = null;//which script to print out
        public static bool RunVanilla = false;//if we just want to print out vanilla values.
        public static bool RunIfNull = false; //"ToRun" is null it will run ALL funcs instead of none.
        //PrintFile
        public static bool PrintFile = true;    //do we even want to print or just run. Testing must be OFF   
        public static bool PrintFile_OnlyModifiedLines = true;
        public static bool PrintFile_ModificationSummary = true;
        public static bool PrintFile_VerifyFieldCounts = false;
        public static bool PrintFile_VerifyFieldCounts_OnlyModifiedLines = false;

        //Run Overrides
        public static bool CreateSmithingStoneMods = false; 


        public static bool PrintOnConsole = false; //print on console or on debug tab

        public static void SetToRun()
        {
            //ToRun = Program.AtkParam_Pc;
            //ToRun = Program.BehaviorParam_PC;//currently no change - except the names.
            //ToRun = Program.Bullet;
            //ToRun = Program.CalcCorrectGraph;
            //ToRun = Program.ItemLotParam_enemy;
            //ToRun = Program.ItemLotParam_map;
            //ToRun = Program.NpcParam;
            //ToRun = Program.Magic;
            //ToRun = Program.SpEffectParam;
            //ToRun = Program.SwordArtsParam;

            //ToRun = Program.ShopLineupParam;
            ToRun = Program.BonfireWarpParam;
            
            //ToRun = null;
        }



        //TESTING  
        public  static bool Testing = false; //if testing you wont print file.
        public static bool Testing_FunctionDebug = false; //if testing you wont print file.
        public static void Tests()
        {
            TestingBanner();
            int getItemFlagId = Program.ItemLotParam_map.GetFieldIndex("getItemFlagId");
            Lines sss = Program.ItemLotParam_map.GetLinesOnCondition(
                new Condition.Either(
                    new Condition.NameStartsWith("Smithing Stone ["),
                    new Condition.NameStartsWith("Somber Smithing Stone [")
                ).AND(new Condition.FloatFieldBetween(
                    getItemFlagId
                    , 1000000000, 2000000000))
                );

            var f = IntFilter.CreateFromAcceptableInts(Util.ToInts(sss.GetFields(getItemFlagId)));
            f.Print();

            int num = 0;
            while(num != -1)
            {
                num = f.GetNext();
                Util.println(num);
            }
            var iis = new int[]
            {

1030427890,
1030427900,
1030427910,
1030427920,
1030427930,
1030427940,
1030427950,
1030427960,
1030427970,
1030427980,

            };

            foreach(int i in iis)
            {
                Util.println(f.Pass(i).ToString()+" i");
            }
            
            //Program.ItemLotParam_enemy.ModifiedLines.PrintFieldIndexes(new int[] { 0, 1 });  //print names and 
            //Program.NpcParam.ModifiedLines.PrintFieldIndexes(new int[] { 0, 1 , Program.NpcParam.GetFieldIndex("itemLotId_enemy")});  //print names and 
            
            
            //ToRun.PrintFile();
            //Program.Magic.PrintFieldIndexes(new int[] { 0, 1 }.Concat(Program.Magic.GetFieldIndexesContains("refId")).ToArray());
            ///////////////////////////////////////////
            ///
                        //ToRun.PrintCompareLineChanges();          //compare changes.
            //ToRun.PrintFieldIndexes(new int[] { 0, 1 });  //print names and 
            //ToRun.PrintFile();
            //
            ////////////////////////////////////////////

            TestingBanner();
        }
        public static void TestingBanner()
        {
            Util.println("~~~~~~~~~~~~CURRENTLY TESTING~~~~~~~~~~~~");
            Util.println("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        public static bool CanDebugInsideFunction {
            get { return Testing && Testing_FunctionDebug; }
        }
    }
}
