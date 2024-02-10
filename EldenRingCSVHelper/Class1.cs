using System;
using System.Collections.Generic;
using System.Linq;

namespace EldenRingCSVHelper
{
    //conditons
    public abstract class Condition
    {
        public static bool GREATER_THAN(float i, float x) { return i > x; }
        public static bool LESS_THAN(float i, float x) { return i < x; }
        public static bool GREATER_THAN_OR_EQUAL_TO(float i, float x) { return i >= x; }
        public static bool LESS_THAN_OR_EQUAL_TO(float i, float x) { return i <= x; }
        public static bool EQUAL_TO(float i, float x) { return i == x; }
        public static bool NOT_EQUAL_TO(float i, float x) { return i != x; }
        public abstract bool Pass(Line line);
        public static bool CheckConditions(Line line, Condition[] conditons, bool andTrue_OrFalse = true)
        {
            foreach (Condition condition in conditons)
            {
                if (andTrue_OrFalse && !condition.Pass(line))
                    return false;
                else if(!andTrue_OrFalse && condition.Pass(line))
                    return true;
            }
            if (andTrue_OrFalse)
                return true;
            else
                return false;
        }
        //for some reason cond0.(cond1.False) makes cond0 false;
        public Condition IsFalse
        {
            get
            {
                return GetFalse();
            }
        }
        protected virtual Condition GetFalse()
        {
            return new ConditionIsFalse(this);
        }
        public virtual Condition AND(Condition condition)
        {
            return new AllOf(this, condition);
        }
        public virtual Condition OR(Condition condition)
        {
            return new Either(this, condition);
        }

        

        public class FloatBetween : Condition
        {
            FloatReturn num;
            FloatReturn min;
            FloatReturn max;
            bool include;
            public FloatBetween(FloatReturn num, FloatReturn min, FloatReturn max, bool include = false)
            {
                this.num = num;
                this.min = min;
                this.max = max;
                this.include = include;
            }
            public override bool Pass(Line line)
            {
                var conditions = new Condition[2];
                Func<float, float, bool>[] fs = new Func<float, float, bool>[2];
                if (include)
                {
                    fs[0] = GREATER_THAN_OR_EQUAL_TO;
                    fs[1] = LESS_THAN_OR_EQUAL_TO;
                }
                else
                {
                    fs[0] = GREATER_THAN;
                    fs[1] = LESS_THAN;
                }
                conditions[0] = new FloatCompare(num, fs[0], min);
                conditions[1] = new FloatCompare(num, fs[1], max);
                return CheckConditions(line, conditions);
            }
        }
        public class TRUE : Condition
        {
            public TRUE() { }
            public override bool Pass(Line line)
            {
                return true;
            }
        }
        public class FloatCompare : Condition
        {
            FloatReturn num;
            FloatReturn compareTo;
            Func<float, float, bool> comparerFunc;
            public FloatCompare(FloatReturn num, Func<float, float, bool> comparer, FloatReturn toCompareNum) { this.num = num; comparerFunc = comparer; compareTo = toCompareNum; }
            public override bool Pass(Line line)
            {
                return comparerFunc(num.Get(line), compareTo.Get(line));
            }
            public Condition OR(FloatReturn toCompareNum)
            {
                return this.OR(new FloatCompare(num, comparerFunc, toCompareNum));
            }
        }


        public class FloatFieldBetween : FieldCondition
        {
            FloatReturn min;
            FloatReturn max;
            bool include;
            public FloatFieldBetween(int fieldIndex, FloatReturn min, FloatReturn max, bool include = false)
            {
                field = fieldIndex;
                this.min = min;
                this.max = max;
                this.include = include;
            }
            public FloatFieldBetween(FloatReturn min, FloatReturn max, bool include = false)
            {
                this.min = min;
                this.max = max;
                this.include = include;
            }
            public override bool Pass(Line line)
            {
                var conditions = new Condition[2];
                Func<float, float, bool>[] fs = new Func<float, float, bool>[2];
                if (include)
                {
                    fs[0] = GREATER_THAN_OR_EQUAL_TO;
                    fs[1] = LESS_THAN_OR_EQUAL_TO;
                }
                else
                {
                    fs[0] = GREATER_THAN;
                    fs[1] = LESS_THAN;
                }
                conditions[0] = new FloatFieldCompare(field, fs[0], min);
                conditions[1] = new FloatFieldCompare(field, fs[1], max);
                return CheckConditions(line, conditions);
            }
        }
        public class FloatFieldCompare : FieldCondition
        {
            FloatReturn compareTo;
            Func<float, float, bool> comparerFunc;
            public FloatFieldCompare(IntReturn fieldIndex, Func<float, float, bool> comparer, FloatReturn toCompareNum) { field = fieldIndex; comparerFunc = comparer; compareTo = toCompareNum; }
            public FloatFieldCompare(Func<float, float, bool> comparer, FloatReturn toCompareNum) { comparerFunc = comparer; compareTo = toCompareNum; }
            public override bool Pass(Line line)
            {
                return comparerFunc(int.Parse(line.GetData()[field.GetInt(line)]), compareTo.Get(line));
            }
            public Condition OR(FloatReturn toCompareNum)
            {
                return this.OR(new FloatFieldCompare(field, comparerFunc, toCompareNum));
            }
        }
        public class FieldEqualTo : FieldCondition
        {
            string[] equalTo;
            public FieldEqualTo(int fieldIndex, string mustBeEqualTo) { field = fieldIndex; equalTo = new string[] { mustBeEqualTo }; }
            public FieldEqualTo(string mustBeEqualTo) { equalTo = new string[] { mustBeEqualTo }; }
            public FieldEqualTo(int fieldIndex, string[] mustBeEqualTo) { field = fieldIndex; equalTo =  mustBeEqualTo ; }
            public FieldEqualTo(string[] mustBeEqualTo) { equalTo = mustBeEqualTo; }

            public override bool Pass(Line line)
            {
                foreach (string s in equalTo) {
                    if (line.GetData()[field.GetInt(line)] == s)
                        return true;
                }
                return false;
            }
        }
        public class FieldIs : FieldCondition
        {
            string[] validValues;

            public FieldIs(int _field, string[] _validValues) { field = _field; validValues = _validValues; }
            public FieldIs(int _field, string _validValue) { field = _field; validValues = new string[] { _validValue }; }
            public FieldIs(string[] _validValues) { validValues = _validValues; }
            public FieldIs(string _validValue) { validValues = new string[] { _validValue }; }

            public override bool Pass(Line line)
            {
                return (validValues.Contains(line.GetData()[field.GetInt(line)]));
            }
        }
        public class FieldStartsWith : FieldCondition
        {
            public string startsWith;

            public FieldStartsWith(string _startsWith) { startsWith = _startsWith; }
            public FieldStartsWith(IntReturn fieldIndex, string _startsWith) { field = fieldIndex; startsWith = _startsWith; }
            public override bool Pass(Line line)
            {
                return (line.GetData()[field.GetInt(line)].IndexOf(startsWith) == 0);
            }
        }

        public class FieldEndsWith : FieldCondition
        {
            public string endsWith;

            public FieldEndsWith(string _endsWith) { endsWith = _endsWith; }
            public FieldEndsWith(IntReturn fieldIndex, string _endsWith) { field = fieldIndex; endsWith = _endsWith; }
            public override bool Pass(Line line)
            {
                string s = line.GetData()[field.GetInt(line)];
                string sEnding = s.Remove(0, s.Length - endsWith.Length);
                return (sEnding == endsWith);
            }
        }

        public class NameIs : Condition
        {
            string[] validFields;
            public NameIs(string[] _validFields) { validFields = _validFields; }
            public NameIs(string _validField) { validFields = new string[] { _validField }; }

            public override bool Pass(Line line)
            {
                return (validFields.Contains(line.GetData()[1]));
            }
        }
        public class IDCheck : Condition
        {
            string[] validIDs;

            public IDCheck(int _validID) { validIDs = Util.ToStrings(new int[] { _validID }); }
            public IDCheck(int[] _validIDs) { validIDs = Util.ToStrings(_validIDs); }
            public IDCheck(string[] _validIDs) { validIDs = _validIDs; }
            public IDCheck(string _validID) { validIDs = new string[] { _validID }; }
            public override bool Pass(Line line)
            {
                //if(validIDs.Contains(line.GetData()[0]))
                //    Util.println( validIDs[0] + " == " + (validIDs[0] == line.GetData()[0]).ToString() + " " + line.GetData()[0]);
                //    Utility.println(line.GetData()[0] + "  " + validIDs.Contains(line.GetData()[0]));
                return (validIDs.Contains(line.GetData()[0]));
            }
        }
        public class HasInName : Condition
        {
            public string[] hasInName;

            public HasInName(string _hasInName) { hasInName = new string[] { _hasInName }; }
            public HasInName(string[] _hasInName) { hasInName = _hasInName; }
            public override bool Pass(Line line)
            {
                foreach (string name in hasInName)
                {
                    if (line.GetData()[1].Contains(name))
                        return true;
                }
                return false;
            }
        }

        public class NameStartsWith : Condition
        {
            public string startWith;

            public NameStartsWith(string _startsWith) { startWith = _startsWith; }
            public override bool Pass(Line line)
            {
                return (line.GetData()[1].IndexOf(startWith) == 0);
            }
        }


        class ConditionIsFalse : Condition
        {
            Condition baseCondition;
            public ConditionIsFalse(Condition condition)
            {
                baseCondition = condition;
            }
            public override bool Pass(Line line)
            {
                return !baseCondition.Pass(line);
            }
        }
        public abstract class Conditions : Condition
        {
            protected Condition[] conditions;
        }
        public class AllOf : Conditions
        {
            public AllOf(Condition[] conditions) { this.conditions = conditions; }
            public AllOf(Condition c0, Condition c1 = null, Condition c2 = null, Condition c3 = null, Condition c4 = null, Condition c5 = null, Condition c6 = null, Condition c7 = null, Condition c8 = null, Condition c9 = null)
            {
                List<Condition> cs = new List<Condition>();
                cs.Add(c0);
                if (c1 != null)
                    cs.Add(c1);
                if (c2 != null)
                    cs.Add(c2);
                if (c3 != null)
                    cs.Add(c3);
                if (c4 != null)
                    cs.Add(c4);
                if (c5 != null)
                    cs.Add(c5);
                if (c6 != null)
                    cs.Add(c6);
                if (c7 != null)
                    cs.Add(c7);
                if (c8 != null)
                    cs.Add(c8);
                if (c9 != null)
                    cs.Add(c9);
                this.conditions = cs.ToArray();
            }
            public override bool Pass(Line line)
            {
                return CheckConditions(line, conditions);
            }
            public static bool CheckConditions(Line line, Condition[] conditions)
            {
                return Condition.CheckConditions(line, conditions, true);
            }
            public override Condition AND(Condition condition)
            {
                conditions = conditions.Append(condition).ToArray();
                return this;
            }
        }
        public class Either : Conditions
        {
            public Either(Condition[] conditions) { this.conditions = conditions; }
            public Either(Condition c0, Condition c1 = null, Condition c2 = null, Condition c3 = null, Condition c4 = null, Condition c5 = null, Condition c6 = null, Condition c7 = null, Condition c8 = null, Condition c9 = null)
            {
                List<Condition> cs = new List<Condition>();
                cs.Add(c0);
                if (c1 != null)
                    cs.Add(c1);
                if (c2 != null)
                    cs.Add(c2);
                if (c3 != null)
                    cs.Add(c3);
                if (c4 != null)
                    cs.Add(c4);
                if (c5 != null)
                    cs.Add(c5);
                if (c6 != null)
                    cs.Add(c6);
                if (c7 != null)
                    cs.Add(c7);
                if (c8 != null)
                    cs.Add(c8);
                if (c9 != null)
                    cs.Add(c9);
                this.conditions = cs.ToArray();
            }
            public override bool Pass(Line line)
            {
                return CheckConditions(line, conditions);
            }
            public static bool CheckConditions(Line line, Condition[] conditions)
            {
                return Condition.CheckConditions(line, conditions, false);
            }
            public override Condition OR(Condition condition)
            {
                conditions = conditions.Append(condition).ToArray();
                return this;
            }
        }

        public abstract class FieldCondition : Condition
        {
            public IntReturn field;
            public Condition AND(FieldCondition condition)
            {
                ShareField(condition);
                return new AllOf(this, condition);
            }
            public Condition OR(FieldCondition condition)
            {
                ShareField(condition);
                return new Either(this, condition);
            }
            public void ShareField(FieldCondition condition)
            {
                if (field != null && condition.field == null)
                    condition.field = field;
            }
            public MultiFieldCondition SetMultiField(int[] fields, bool allFieldsMustBeTrue)
            {
                return new MultiFieldCondition(this, fields, allFieldsMustBeTrue);
            }
            public MultiFieldCondition SetMultiField(int[] fields, int numOfFieldsToBeTrueToPass)
            {
                return new MultiFieldCondition(this, fields, numOfFieldsToBeTrueToPass);
            }
            public void SetField(int field)
            {
                this.field = field;
            }

        }
        public class MultiFieldCondition : Condition
        {
            int numMustBeTrue = 1;
            FieldCondition fieldCondition;
            int[] fields;

            public MultiFieldCondition(FieldCondition fieldCondition, int[] fields, bool allFieldsMustBeTrue) { this.fieldCondition = fieldCondition; this.fields = fields; if (allFieldsMustBeTrue) numMustBeTrue = fields.Length; }
            public MultiFieldCondition(FieldCondition fieldCondition, int[] fields, int numOfFieldsToBeTrueToPass) { this.fieldCondition = fieldCondition; this.fields = fields; numMustBeTrue = numOfFieldsToBeTrueToPass; }
            public MultiFieldCondition(FieldCondition fieldCondition, int[] fields, int numOfFieldsToBeTrueToPass, bool allFieldsMustBeTrue) { this.fieldCondition = fieldCondition; this.fields = fields; numMustBeTrue = numOfFieldsToBeTrueToPass; }

            public override bool Pass(Line line)
            {
                int passes = 0;
                foreach (var index in fields)
                {
                    fieldCondition.field = index;
                    if (fieldCondition.Pass(line))
                        passes++;
                    if (passes >= numMustBeTrue)
                        return true;
                }
                return false;
            }
            public Condition AND(FieldCondition condition)
            {
                return new AllOf(this, ShareField(condition));
            }
            public Condition OR(FieldCondition condition)
            {
                return new Either(this, ShareField(condition));
            }
            public Condition ShareField(FieldCondition condition)
            {
                if (condition.field == null)
                {
                    return condition.SetMultiField(fields, numMustBeTrue);
                }
                return condition;
            }
        }

    }



    //operations
    public abstract class LineModifier
    {
        public abstract void Operate(Line line);
    }
    public abstract class FieldModifier : LineModifier
    {
        protected int[] fieldIndexes;
        public void SetFieldIndexesTo(int[] _fieldIndexes)
        {
            fieldIndexes = _fieldIndexes;
        }
        public void SetFieldIndexTo(int _fieldIndex)
        {
            fieldIndexes = new int[] { _fieldIndex };
        }
    }
    public class SetFieldTo : FieldModifier
    {
        string setTo;
        public SetFieldTo(int fieldIndex, string setTo)
        {
            this.fieldIndexes = new int[] { fieldIndex };
            this.setTo = setTo;
        }
        public SetFieldTo(int[] fieldIndexes, string setTo)
        {
            this.fieldIndexes = fieldIndexes;
            this.setTo = setTo;
        }
        public override void Operate(Line line)
        {
            foreach (int fieldIndex in fieldIndexes)
            {
                line.SetField(fieldIndex, setTo);
            }
        }
    }

    public class SetFieldToValues : FieldModifier    //similar t  SetFieldToiterates through a array of values. Useful if devived the list from a group.
    {
        string[] newValues;
        int i = 0;
        public SetFieldToValues(int fieldIndex, string[] newValues)
        {
            this.fieldIndexes = new int[] { fieldIndex };
            this.newValues = newValues;
        }
        public SetFieldToValues(int[] fieldIndexes, string[] newValues)
        {
            this.fieldIndexes = fieldIndexes;
            this.newValues = newValues;
        }
        public override void Operate(Line line)
        {
            foreach (int fieldIndex in fieldIndexes)
            {
                line.SetField(fieldIndex, newValues[i]);
                i++;
            }
        }
    }
    public class SetFloatFieldTo : FieldModifier
    {
        FloatReturn setTo;
        public SetFloatFieldTo(int fieldIndex, FloatReturn setTo)
        {
            this.fieldIndexes = new int[] { fieldIndex };
            this.setTo = setTo;
        }
        public SetFloatFieldTo(int[] fieldIndexes, FloatReturn setTo)
        {
            this.fieldIndexes = fieldIndexes;
            this.setTo = setTo;
        }
        public override void Operate(Line line)
        {
            foreach (int fieldIndex in fieldIndexes)
            {
                line.SetField(fieldIndex, setTo.Get(line));
            }
        }
    }
public abstract class Operation : FieldModifier
    {

        protected Func<float, float, float> operation;
        protected FloatReturn by;
        public static float MULTIPLY(float i, float x) { return i * x; }
        public static float DIVIDED(float i, float x) { return i / x; }
        public static float DIVIDED_REV(float i, float x) { return x / i; }
        public static float PLUS(float i, float x) { return i + x; }
        public static float MINUS(float i, float x) { return i - x; }
        public static float MINUS_REV(float i, float x) { return x - i; }
        public static float EQUALS(float i, float x) { return x; }
        public static float MAX (float i, float x) { return Math.Max(x, i); }
        public static float MIN(float i, float x) { return Math.Min(x, i); }


        public Operation MULTIPLY( FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, MULTIPLY, _by);
            return this;
        }
        public Operation DIVIDED(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, DIVIDED, _by);
            return this;
        }
        public Operation DIVIDED_REV(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, DIVIDED_REV, _by);
            return this;
        }
        public Operation PLUS(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, PLUS, _by);
            return this;
        }
        public Operation MINUS(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, MINUS, _by);
            return this;
        }
        public Operation MINUS_REV(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, MINUS_REV, _by);
            return this;
        }
        public Operation MAX(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, MAX, _by);
            return this;
        }
        public Operation MIN(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, MIN, _by);
            return this;
        }
        /*public Operation EQUALS(FloatReturn _by)
        {
            this.by = new FloatComplex(this.by, EQUALS, _by);
            return this;
        }*/

    }
    public class OperateIntField : Operation
    {
        FloatFieldRef alternateField;
        //bool fieldCycle = false;
        public OperateIntField(Func<float, float, float> _operation, FloatReturn _by) { operation = _operation; by = _by; }
        public OperateIntField(int _intFieldIndex) { fieldIndexes = new int[] { _intFieldIndex }; operation = Operation.EQUALS; alternateField = new FloatFieldRef(-1); by = alternateField;}
        public OperateIntField(int[] _intFieldIndexes) { fieldIndexes = _intFieldIndexes; operation = Operation.EQUALS; alternateField = new FloatFieldRef(-1); by = alternateField; }
        public OperateIntField(int _intFieldIndex, Func<float, float, float> _operation, FloatReturn _by) { fieldIndexes = new int[] { _intFieldIndex }; operation = _operation; by = _by; }
        public OperateIntField(int[] _intFieldIndexes, Func<float, float, float> _operation, FloatReturn _by) { fieldIndexes = _intFieldIndexes; operation = _operation; by = _by; }
        
        public override void Operate(Line line)
        {
            foreach (int fieldIndex in fieldIndexes)
            {
                if(alternateField != null)
                    alternateField.fieldIndex = fieldIndex;
                var newValue = ((int)operation(int.Parse(line.GetField(fieldIndex)), by.Get(line))).ToString();
                line.SetField(fieldIndex, newValue);
            }
        }
    }
    public class OperateFloatField : Operation
    {
        FloatFieldRef alternateField;
        public OperateFloatField(Func<float, float, float> _operation, FloatReturn _by) { operation = _operation; by = _by; }
        public OperateFloatField(int _intFieldIndex) { fieldIndexes = new int[] { _intFieldIndex }; operation = Operation.EQUALS; alternateField = new FloatFieldRef(-1); by = alternateField; }
        public OperateFloatField(int[] _intFieldIndexes) { fieldIndexes = _intFieldIndexes; operation = Operation.EQUALS; alternateField = new FloatFieldRef(-1); by = alternateField; }
        public OperateFloatField(int _intFieldIndex, Func<float, float, float> _operation, FloatReturn _by) { fieldIndexes = new int[] { _intFieldIndex }; operation = _operation; by = _by; }
        public OperateFloatField(int[] _intFieldIndexes, Func<float, float, float> _operation, FloatReturn _by) { fieldIndexes = _intFieldIndexes; operation = _operation; by = _by; }
       
        public override void Operate(Line line)
        {
            foreach (int fieldIndex in fieldIndexes)
            {
                if (alternateField != null)
                    alternateField.fieldIndex = fieldIndex;
                line.SetField(fieldIndex, operation(float.Parse(line.GetField(fieldIndex)), by.Get(line)).ToString());
            }
        }
    }

    //float return
    public abstract class FloatReturn
    {
        public static implicit operator FloatReturn(float f)
        {
            return new FloatRef(f);
        }
        public abstract float Get(Line line);

        public FloatReturn MULTIPLY(FloatReturn _by)
        {
           return new FloatComplex(this, Operation.MULTIPLY, _by);
        }
        public FloatReturn DIVIDED(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.DIVIDED, _by);
        }
        public FloatReturn DIVIDE_REV(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.DIVIDED_REV, _by);
        }
        public FloatReturn PLUS(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.PLUS, _by);
        }
        public FloatReturn MINUS(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.MINUS, _by);
        }
        public FloatReturn MINUS_REV(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.MINUS_REV, _by);
        }
        public FloatReturn MAX(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.MAX, _by);
        }
        public FloatReturn MIN(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.MIN, _by);
        }
        /*public FloatReturn EQUALS(FloatReturn _by)
        {
            return new FloatComplex(this, Operation.EQUALS, _by);
        }*/
    }
    public abstract class IntReturn: FloatReturn
    {
        public static implicit operator IntReturn(int i)
        {
            return new IntRef(i);
        }

        public abstract int GetInt(Line line);
    }
    public class IntRef : IntReturn
    {
        public static implicit operator IntRef(int f)
        {
            return new IntRef(f);
        }
        public int ret;
        public IntRef(int f) { ret = f; }
        public void SetTo(int f)
        {
            ret = f;
        }
        public override int GetInt(Line line)
        {
            return ret;
        }
        public override float Get(Line line)
        {
            return ret;
        }
    }
    public class FloatRef : FloatReturn
    {
        public static implicit operator FloatRef(float f)
        {
            return new FloatRef(f);
        }
        float ret;
        public FloatRef(float f) { ret = f; }
        public void SetTo(float f)
        {
            ret = f;
        }
        public override float Get(Line line)
        {
            return ret;
        }
    }
    public class FloatFieldRef : FloatReturn
    {
        public int fieldIndex;
        public FloatFieldRef(int fieldIndex) { this.fieldIndex = fieldIndex; }
        public override float Get(Line line)
        {
            return float.Parse(line.GetData()[fieldIndex]);
        }
    }
    public class OperatedFloat : FloatReturn
    {
        int returnFieldIndex;
        LineModifier operation;
        public OperatedFloat(int _returnFieldIndex, LineModifier _operation) { returnFieldIndex = _returnFieldIndex; operation = _operation; }

        public override float Get(Line line)
        {
            line.Operate(operation);
            return float.Parse(line.GetData()[returnFieldIndex]);
        }
    }
    public class FloatComplex : FloatReturn
    {
        FloatReturn a;
        FloatReturn b;
        Func<float, float, float> operation;
        public FloatComplex(FloatReturn i, Func<float, float, float> operation, FloatReturn x) { a = i; b = x; this.operation = operation; }
        public override float Get(Line line)
        {
            return operation(a.Get(line), b.Get(line));
        }
    }
}
