using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace CDCGameKit
{
    public static class Extensions
    {
        #region Floats
        public static float Remap(this float value, float oldMin, float oldMax, float newMin, float newMax, bool clamped = true)
        {
            float fraction = (value - oldMin) / (oldMax - oldMin);
            if (clamped)
                fraction = Mathf.Clamp(value, oldMin, oldMax);

            return Mathf.Lerp(newMin, newMax, fraction);
        }
        public static float Fraction(this float value, float lower, float uppper, bool clamped = true)
        {
            var toReturn = (value - lower) / (uppper - lower);

            if (clamped) toReturn = Mathf.Clamp01(toReturn);

            return toReturn;
        }
        /// <summary>
        /// Returns inverted value. Original is unchanged.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Invert (this float value)
        {
            if (value > 1 || value < 0)
            {
                Debug.LogWarning("Cannot invert value because outside of 0 to 1. Returning original value of " + value);
                return value;
            }
            else 
            {
                value = 1 - value;
                return value; 
            }
        }
        #endregion

        #region Vectors
        public static Vector3 Project(this Vector3 v, bool normalize = false)
        {
            v.y = 0;

            return normalize ? v.normalized : v;
        }

        public static Vector3 Clamp(this Vector3 vector, float magnitude)
        {
            if (vector.magnitude < magnitude)
                return vector;
            else return vector.normalized * magnitude;
        }

        public static Vector3 ProjectPositionToHeight(Vector3 position, float projectedheight = 0)
        {
            position.y = projectedheight;
            return position;
        }
        #endregion

        #region Transforms
        public static Transform FindRecursive(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }
        public static List<Transform> FindAllRecursive(this Transform aParent, string name, bool partialMatchOK = true)
        {
            List<Transform> toReturn = new List<Transform>();

            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                if (partialMatchOK)
                {
                    if (c.name.Contains(name)) toReturn.Add(c);
                }
                else
                {
                    if (c.name == name) toReturn.Add(c);
                }

                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return toReturn;
        }
        public static List<Transform> GetChildren(this Transform aParent)
        {
            var toReturn = new List<Transform>();
            foreach (Transform child in aParent) toReturn.Add(child);
            return toReturn;
        }
        public static List<Transform> GetChildrenDeep(this Transform aParent)
        {
            var toReturn = new List<Transform>();
            var queue = new Queue<Transform>();
            queue.Enqueue(aParent);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();

                var thisGen = GetChildren(c);
                AppendList(toReturn, thisGen);

                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            return toReturn;
        }
        public static void Reset(this Transform aTransform)
        {
            aTransform.position = Vector3.zero;
            aTransform.rotation = Quaternion.identity;
            aTransform.localScale = Vector3.one;
        }
        public static void LocalReset(this Transform aTransform)
        {
            aTransform.localPosition = Vector3.zero;
            aTransform.localRotation = Quaternion.identity;
            aTransform.localScale = Vector3.one;
        }
        #endregion

        #region Colors
        public static Color MoveTowards(this Color c, Color destination, float maxDistance)
        {
            Vector4 start = (Vector4)c;
            Vector4 end = (Vector4)destination;
            var toReturn = Vector4.MoveTowards(start, end, maxDistance);

            return (Color)toReturn;
        }

        public static Color AlphaZero(this Color c)
        {
            c.a = 0;
            return c;
        }
        #endregion

        #region Lists
        public static List<T> AppendList<T>(this List<T> beingAppendedTo, List<T> toAppend, bool addIfRedundant = true)
        {
            if (addIfRedundant)
            {
                foreach (var t in toAppend) beingAppendedTo.Add(t);
            }
            else
            {
                foreach (var t in toAppend) if (!beingAppendedTo.Contains(t)) beingAppendedTo.Add(t);
            }
            return beingAppendedTo;
        }

        public static List<T> AppendList<T>(this List<T> beingAppendedTo, T[] toAppend, bool addIfRedundant = true)
        {
            if (addIfRedundant)
            {
                foreach (var t in toAppend) beingAppendedTo.Add(t);
            }
            else
            {
                foreach (var t in toAppend) if (!beingAppendedTo.Contains(t)) beingAppendedTo.Add(t);
            }
            return beingAppendedTo;
        }

        public static List<T> Shuffle<T>(this List<T> source)
        {
            int count = source.Count;
            int shuffleCount = count * 2;

            for (int i = 0; i < shuffleCount; i++)
            {
                var shuffle = source[Random.Range(0, count)];
                source.Remove(shuffle);
                source.Insert(Random.Range(0, count - 1), shuffle);
            }
            return source;
        }

        public static T RandomOne<T>(this List<T> source)
        {
            if (source == null) Debug.LogError("ERROR: List<" + typeof(T) + "> is null");
            if (source.Count == 0) Debug.LogError("ERROR: List<" + typeof(T) + "> is empty");

            return source[Random.Range(0, source.Count)];
        }

        public static List<T> Copy<T> (this List<T> source)
        {
            List<T> toReturn = new List<T>();
            foreach (var entry in source)
                toReturn.Add(entry);
            return toReturn;
        }
        #endregion
    }

    public static class Tools
    {
        public static class Colors
        {
            public static Color Random(float alpha = 1, bool randomAlpha = false)
            {
                if (randomAlpha) alpha = UnityEngine.Random.value;
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, alpha);
            }
        }
        public static class Math
        {
            #region Floats
            public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax, bool clamped = true)
            {
                if (clamped)
                    value = Mathf.Clamp(value, oldMin, oldMax);

                float fraction = (value - oldMin) / (oldMax - oldMin);

                return Mathf.Lerp(newMin, newMax, fraction);
            }
            public static float Fraction(float value, float lower, float uppper, bool clamped = true)
            {
                var toReturn = (value - lower) / (uppper - lower);

                if (clamped) toReturn = Mathf.Clamp01(toReturn);

                return toReturn;
            }
            #endregion

            #region Vector3
            public static float ProjectedDistance(Vector3 a, Vector3 b)
            {
                a.y = b.y = 0;
                return Vector3.Distance(a, b);
            }
            public static Vector3 ProjectedDirection(Vector3 origin, Vector3 destination)
            {
                origin.y = 0;
                destination.y = 0;
                return (destination - origin).normalized;
            }
            public static Vector3 ClampVector(Vector3 vector, float magnitude = 1)
            {
                if (vector.magnitude < magnitude)
                    return vector;
                else return vector.normalized * magnitude;
            }
            public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
            {
                Vector3 ab = Vector3.Lerp(a, b, t);
                Vector3 bc = Vector3.Lerp(b, c, t);
                Vector3 cd = Vector3.Lerp(c, d, t);

                Vector3 abc = Vector3.Lerp(ab, bc, t);
                Vector3 bcd = Vector3.Lerp(bc, cd, t);

                return Vector3.Lerp(abc, bcd, t);
            }
            #endregion

            #region Translations
            public static int DirToBrg(Vector3 direction)
            {
                var dot = Vector3.Dot(Vector3.forward, direction);

                if (direction.x == 0 && direction.z == 0) return 0;
                else if (direction.x >= 0) return Mathf.FloorToInt(90 - dot * 90);
                else return Mathf.FloorToInt(270 + dot * 90);
            }

            public static int DirToBrg(Vector3 origin, Vector3 destination)
            {
                var dir = ProjectedDirection(origin, destination);
                return (DirToBrg(dir));
            }

            public static Vector3 BrgToDir(int bearing)
            {
                return Quaternion.Euler(0, bearing, 0) * Vector3.forward;
            }
            #endregion

            public static class ProcRandom
            {
                public static int Range(int min, int max, int seed)
                {
                    Debug.Assert(seed >= 0, "Seed must be 0 or greater.");
                    Debug.Assert(max - min <= 255, "Procedural random range excessive.");

                    for (int i = 0; i < (seed / 256) + 1; i++)
                    {
                        seed = perm[seed % 256];
                    }

                    return (seed % (max - min)) + min;
                }

                static int[] perm = {
                151,160,137,91,90,15,
                131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
                151
            };
            }
        }

        public class Line
        {
            private bool changed;

            public Vector3? end
            {
                get { return e; }
                set
                {
                    if (value != e)
                    {
                        e = value;
                        changed = true;
                    }
                }
            }
            private Vector3? e;
            public Vector3? origin
            {
                get { return o; }
                set
                {
                    if (value != o)
                    {
                        o = value;
                        changed = true;
                    }
                }
            }
            private Vector3? o;

            public float? magnitude
            {
                get
                {
                    if (e == null || o == null) return null;
                    else
                    {
                        if (changed) Calculate();
                        return m;
                    }
                }
            }
            private float m;
            public float? projectedMagnitude
            {
                get
                {
                    if (e == null || o == null) return null;
                    else
                    {
                        if (changed) Calculate();
                        return pM;
                    }
                }
            }
            private float pM;
            public Vector3? direction
            {
                get
                {
                    if (e == null || o == null) return null;
                    else
                    {
                        if (changed) Calculate();
                        return d;
                    }
                }
            }
            private Vector3 d;
            public Vector3? projectedDirection
            {
                get
                {
                    if (e == null || o == null) return null;
                    else
                    {
                        if (changed) Calculate();
                        return pD;
                    }
                }
            }
            private Vector3 pD;
            public Vector3? projectedLine
            {
                get
                {
                    if (e == null || o == null) return null;
                    else
                    {
                        if (changed) Calculate();
                        return pL;
                    }
                }
            }
            private Vector3 pL;

            public Line() { }
            public Line(Vector3 end, Vector3 origin)
            {
                this.end = end;
                this.origin = origin;
            }

            private void Calculate()
            {
                changed = false;
                if (e == null || o == null) return;
                else
                {
                    Vector3 E = (Vector3)e;
                    Vector3 O = (Vector3)o;

                    d = E - O;
                    m = d.magnitude;

                    pL = new Vector3(d.x, 0, d.z);
                    pM = pL.magnitude;

                    d = d.normalized;

                    pD = pL.normalized;
                }
            }
            public Vector3? GetPoint(float magnitude)
            {
                if (e == null || o == null) return null;
                else return o + d * magnitude;
            }
            public Vector3? GetProjectedPoint(float magnitude)
            {
                if (e == null || o == null) return null;
                else
                {
                    Vector3 O = (Vector3)o;
                    O.y = 0;
                    return O + pD * magnitude;
                }
            }
            public void Log()
            {
                if (e == null && o == null) Debug.Log("Cannot log Line: end and origin are null.");
                else if (e == null) Debug.Log("Cannot log Line: end is null.");
                else if (o == null) Debug.Log("Cannot log Line: origin is null.");
                else
                {
                    Vector3 E = (Vector3)e;
                    Vector3 O = (Vector3)o;

                    string toReturn = "Origin: (";
                    toReturn += O.x.ToString() + ", ";
                    toReturn += O.y.ToString() + ", ";
                    toReturn += O.y.ToString() + "), End: (";
                    toReturn += E.x.ToString() + ", ";
                    toReturn += E.y.ToString() + ", ";
                    toReturn += E.z.ToString() + ")";
                    Debug.Log(toReturn);
                }
            }
        }

        public static class Formatting
        {
            public static List<string> ProcessCSV(TextAsset file)
            {
                // Populate from prototype
                return null;
            }
            public static string[] CSVSplit(string toSplit)
            {
                var toReturn = new List<string>();
                var value = "";
                for (var i = 0; i < toSplit.Length; i++)
                {
                    if (toSplit[i] == '"')
                    {
                        while (toSplit[++i] != '"')
                            value += toSplit[i];
                    }

                    if (toSplit[i] == ',')
                    {
                        toReturn.Add(value);
                        value = "";
                    }
                    else if (toSplit[i] != '"')
                    {
                        value += toSplit[i];
                    }
                }

                if (value != "")
                    toReturn.Add(value);

                return toReturn.ToArray();
            }
            public static List<int> BreakToIntList(string raw)
            {
                List<int> toReturn = new List<int>();

                if (raw.Equals("NONE", System.StringComparison.OrdinalIgnoreCase) || raw.Length == 0)
                    return toReturn;
                else
                {
                    var stringList = new List<string>(raw.TrimEnd('|').Split('|'));
                    foreach (var item in stringList)
                    {
                        int test = 9999;
                        int.TryParse(item, out test);
                        if (test != 9999)
                            toReturn.Add(test);
                    }
                    return toReturn;
                }
            }

            public static List<string> BreakToStringList(string raw)
            {
                //Forced to lower
                List<string> toReturn = new List<string>();

                if (raw.Equals("NONE", System.StringComparison.OrdinalIgnoreCase) || raw.Length == 0)
                    return toReturn;
                else
                {
                    var notYetToLower = new List<string>(raw.TrimEnd('|').Split('|'));
                    foreach (var item in notYetToLower)
                    {
                        toReturn.Add(item.ToLower());
                    }
                    return toReturn;
                }
            }

            public static List<float> BreakToFloatList(string raw)
            {
                List<float> toReturn = new List<float>();

                if (raw.Equals("NONE", System.StringComparison.OrdinalIgnoreCase) || raw.Length == 0)
                    return toReturn;
                else
                {
                    var stringList = new List<string>(raw.TrimEnd('|').Split('|'));
                    foreach (var item in stringList)
                    {
                        float test = 9999;
                        float.TryParse(item, out test);
                        if (test != 9999)
                            toReturn.Add(test);
                    }
                    return toReturn;
                }
            }
        }

        public static class Date
        {
            public static int GetCurrentWeek()
            {
                // Note: Week 1 is the first full week of the year.
                System.DateTime thisNewYear = new System.DateTime();
                thisNewYear = thisNewYear.AddYears(System.DateTime.Now.Year - 1);
                int dateOfFirstFullWeek = 8 - (int)thisNewYear.DayOfWeek;
                System.DateTime startOfFirstFullWeek = new System.DateTime(thisNewYear.Year, 1, dateOfFirstFullWeek);

                int weeks = 0;
                for (int i = 0; i < 53; i++)
                {
                    System.DateTime evaluatedWeek = startOfFirstFullWeek.AddDays(7 * i);
                    if (evaluatedWeek >= System.DateTime.Now.ToUniversalTime())
                    {
                        weeks = i;
                        Debug.Log("This week is " + (i) + " calendar weeks into the year.");
                        break;
                    }
                }

                return weeks;
            }
        }

        public static class NamingTools
        {
            static string[] lowers = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            static string[] uppers = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            public static string GetLower(int index, bool rollover = false)
            {
                if (rollover) index = index % 25;
                else index = Mathf.Clamp(index, 0, 25);

                return lowers[index];
            }

            public static string GetUpper(int index, bool rollover = false)
            {
                if (rollover) index = index % 25;
                else index = Mathf.Clamp(index, 0, 25);

                return lowers[index].ToUpper();
            }

            public static string OnlyNumbers(string input, bool keepDecimals = false, bool keepSigns = false)
            {
                // Removes spaces
                input = Regex.Replace(input, "[ ]", "");

                if (keepDecimals && !keepSigns) return Regex.Replace(input, "[^0-9].", "");
                else if (keepDecimals && keepSigns) return Regex.Replace(input, "[^0-9].+-", "");
                else if (!keepDecimals && keepSigns) return Regex.Replace(input, "[^0-9]-", "");
                else return Regex.Replace(input, "[^0-9]", "");
            }

            public static bool IsUpper(string letter)
            {
                if (letter.Length > 1) Debug.LogWarning("NamingTools.IsUpper() can only assess strings of length 1.");
                foreach (var u in uppers) if (letter == u) return true;
                return false;
            }

            public static bool IsLower(string letter)
            {
                if (letter.Length > 1) Debug.LogWarning("NamingTools.IsLower() can only assess strings of length 1.");
                foreach (var l in lowers) if (letter == l) return true;
                return false;
            }

            public static string NumberWithCommas(int value)
            {
                string toReturn = value.ToString();

                int length = toReturn.Length;
                if (length <= 3) return toReturn;
                else
                {
                    for (int i = length - 1 - 3; i >= 0; i -= 3)
                    {
                        toReturn = toReturn.Insert(i + 1, ",");
                    }
                    return toReturn;
                }
            }

            public static string RemoveSpaces(string value)
            {
                int length = value.Length;
                for (int i = length - 1; i >= 0; i--)
                {
                    Regex rgx = new Regex("[^a-zA-Z]");
                    value = rgx.Replace(value, "");
                }
                return value;
            }

            public static string AddSpaces(string value, bool ignoreConsecutiveCaps = false)
            {
                string toReturn = "";
                for (int i = 0; i < value.Length; i++)
                {
                    if (ignoreConsecutiveCaps)
                    {
                        int prevIdx = i - 1;
                        if (prevIdx >= 0)
                        {
                            if (IsUpper(value.Substring(i, 1)))
                            {
                                if (IsUpper(value.Substring(prevIdx, 1))) toReturn += value.Substring(i, 1);
                                else toReturn += " " + value.Substring(i, 1);
                            }
                            else toReturn += " " + value.Substring(i, 1);
                        }
                        else toReturn += " " + value.Substring(i, 1);
                    }
                    else
                    {
                        if (IsUpper(value.Substring(i, 1))) toReturn += " " + value.Substring(i, 1);
                        else toReturn += value.Substring(i, 1);
                    }
                }
                return toReturn;
            }

            public static string SecondsToTime(float seconds)
            {
                int remainderSeconds = (int)(seconds % 60);
                int minutesTotal = (int)(seconds / 60);
                int remainderMinutes = (int)(minutesTotal / 60);
                int hoursTotal = (int)(minutesTotal / 60);

                string toReturn = "";
                if (hoursTotal > 0) toReturn += hoursTotal.ToString() + ":";
                if (remainderMinutes <= 9) toReturn += "0";
                toReturn += remainderMinutes + ":";
                if (remainderSeconds <= 9) toReturn += "0";
                toReturn += remainderSeconds;

                return toReturn;
            }
        }
    }

    static public class Easings
    {
        /// <summary>
        /// Constant Pi.
        /// </summary>
        private const float PI = (float)Mathf.PI;

        /// <summary>
        /// Constant Pi / 2.
        /// </summary>
        private const float HALFPI = (float)Mathf.PI / 2.0f;

        /// <summary>
        /// Easing Functions enumeration
        /// </summary>
        public enum Functions
        {
            Linear,
            QuadraticEaseIn,
            QuadraticEaseOut,
            QuadraticEaseInOut,
            CubicEaseIn,
            CubicEaseOut,
            CubicEaseInOut,
            QuarticEaseIn,
            QuarticEaseOut,
            QuarticEaseInOut,
            QuinticEaseIn,
            QuinticEaseOut,
            QuinticEaseInOut,
            SineEaseIn,
            SineEaseOut,
            SineEaseInOut,
            CircularEaseIn,
            CircularEaseOut,
            CircularEaseInOut,
            ExponentialEaseIn,
            ExponentialEaseOut,
            ExponentialEaseInOut,
            ElasticEaseIn,
            ElasticEaseOut,
            ElasticEaseInOut,
            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,
            BounceEaseIn,
            BounceEaseOut,
            BounceEaseInOut
        }

        /// <summary>
        /// Interpolate using the specified function.
        /// </summary>
        static public float Interpolate(float p, Functions function)
        {
            switch (function)
            {
                default:
                case Functions.Linear: return Linear(p);
                case Functions.QuadraticEaseOut: return QuadraticEaseOut(p);
                case Functions.QuadraticEaseIn: return QuadraticEaseIn(p);
                case Functions.QuadraticEaseInOut: return QuadraticEaseInOut(p);
                case Functions.CubicEaseIn: return CubicEaseIn(p);
                case Functions.CubicEaseOut: return CubicEaseOut(p);
                case Functions.CubicEaseInOut: return CubicEaseInOut(p);
                case Functions.QuarticEaseIn: return QuarticEaseIn(p);
                case Functions.QuarticEaseOut: return QuarticEaseOut(p);
                case Functions.QuarticEaseInOut: return QuarticEaseInOut(p);
                case Functions.QuinticEaseIn: return QuinticEaseIn(p);
                case Functions.QuinticEaseOut: return QuinticEaseOut(p);
                case Functions.QuinticEaseInOut: return QuinticEaseInOut(p);
                case Functions.SineEaseIn: return SineEaseIn(p);
                case Functions.SineEaseOut: return SineEaseOut(p);
                case Functions.SineEaseInOut: return SineEaseInOut(p);
                case Functions.CircularEaseIn: return CircularEaseIn(p);
                case Functions.CircularEaseOut: return CircularEaseOut(p);
                case Functions.CircularEaseInOut: return CircularEaseInOut(p);
                case Functions.ExponentialEaseIn: return ExponentialEaseIn(p);
                case Functions.ExponentialEaseOut: return ExponentialEaseOut(p);
                case Functions.ExponentialEaseInOut: return ExponentialEaseInOut(p);
                case Functions.ElasticEaseIn: return ElasticEaseIn(p);
                case Functions.ElasticEaseOut: return ElasticEaseOut(p);
                case Functions.ElasticEaseInOut: return ElasticEaseInOut(p);
                case Functions.BackEaseIn: return BackEaseIn(p);
                case Functions.BackEaseOut: return BackEaseOut(p);
                case Functions.BackEaseInOut: return BackEaseInOut(p);
                case Functions.BounceEaseIn: return BounceEaseIn(p);
                case Functions.BounceEaseOut: return BounceEaseOut(p);
                case Functions.BounceEaseInOut: return BounceEaseInOut(p);
            }
        }

        static public float Lerp(float a, float b, float t, Functions function = Functions.Linear)
        {
            return (b - a) * Interpolate(t, function) + a;
        }

        static public Vector3 Lerp(Vector3 a, Vector3 b, float t, Functions function = Functions.Linear)
        {
            return new Vector3(Lerp(a.x, b.x, t, function), Lerp(a.y, b.y, t, function), Lerp(a.z, b.z, t, function));
        }

        static public Quaternion Slerp(Quaternion a, Quaternion b, float t, Functions function = Functions.Linear)
        {
            return new Quaternion(Lerp(a.x, b.x, t, function), Lerp(a.y, b.y, t, function), Lerp(a.z, b.z, t, function), Lerp(a.w, b.w, t, function));
        }

        /// <summary>
        /// Modeled after the line y = x
        /// </summary>
        static public float Linear(float p)
        {
            return p;
        }

        /// <summary>
        /// Modeled after the parabola y = x^2
        /// </summary>
        static public float QuadraticEaseIn(float p)
        {
            return p * p;
        }

        /// <summary>
        /// Modeled after the parabola y = -x^2 + 2x
        /// </summary>
        static public float QuadraticEaseOut(float p)
        {
            return -(p * (p - 2));
        }

        /// <summary>
        /// Modeled after the piecewise quadratic
        /// y = (1/2)((2x)^2)             ; [0, 0.5)
        /// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        /// </summary>
        static public float QuadraticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 2 * p * p;
            }
            else
            {
                return (-2 * p * p) + (4 * p) - 1;
            }
        }

        /// <summary>
        /// Modeled after the cubic y = x^3
        /// </summary>
        static public float CubicEaseIn(float p)
        {
            return p * p * p;
        }

        /// <summary>
        /// Modeled after the cubic y = (x - 1)^3 + 1
        /// </summary>
        static public float CubicEaseOut(float p)
        {
            float f = (p - 1);
            return f * f * f + 1;
        }

        /// <summary>	
        /// Modeled after the piecewise cubic
        /// y = (1/2)((2x)^3)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        /// </summary>
        static public float CubicEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 4 * p * p * p;
            }
            else
            {
                float f = ((2 * p) - 2);
                return 0.5f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quartic x^4
        /// </summary>
        static public float QuarticEaseIn(float p)
        {
            return p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quartic y = 1 - (x - 1)^4
        /// </summary>
        static public float QuarticEaseOut(float p)
        {
            float f = (p - 1);
            return f * f * f * (1 - p) + 1;
        }

        /// <summary>
        // Modeled after the piecewise quartic
        // y = (1/2)((2x)^4)        ; [0, 0.5)
        // y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
        /// </summary>
        static public float QuarticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 8 * p * p * p * p;
            }
            else
            {
                float f = (p - 1);
                return -8 * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quintic y = x^5
        /// </summary>
        static public float QuinticEaseIn(float p)
        {
            return p * p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quintic y = (x - 1)^5 + 1
        /// </summary>
        static public float QuinticEaseOut(float p)
        {
            float f = (p - 1);
            return f * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quintic
        /// y = (1/2)((2x)^5)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        /// </summary>
        static public float QuinticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 16 * p * p * p * p * p;
            }
            else
            {
                float f = ((2 * p) - 2);
                return 0.5f * f * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave
        /// </summary>
        static public float SineEaseIn(float p)
        {
            return (float)Mathf.Sin((p - 1) * HALFPI) + 1;
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave (different phase)
        /// </summary>
        static public float SineEaseOut(float p)
        {
            return (float)Mathf.Sin(p * HALFPI);
        }

        /// <summary>
        /// Modeled after half sine wave
        /// </summary>
        static public float SineEaseInOut(float p)
        {
            return (float)(0.5f * (1 - Mathf.Cos(p * PI)));
        }

        /// <summary>
        /// Modeled after shifted quadrant IV of unit circle
        /// </summary>
        static public float CircularEaseIn(float p)
        {
            return (float)(1 - Mathf.Sqrt(1 - (p * p)));
        }

        /// <summary>
        /// Modeled after shifted quadrant II of unit circle
        /// </summary>
        static public float CircularEaseOut(float p)
        {
            return (float)(Mathf.Sqrt((2 - p) * p));
        }

        /// <summary>	
        /// Modeled after the piecewise circular function
        /// y = (1/2)(1 - Math.Sqrt(1 - 4x^2))           ; [0, 0.5)
        /// y = (1/2)(Math.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        /// </summary>
        static public float CircularEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return (float)(0.5f * (1 - Mathf.Sqrt(1 - 4 * (p * p))));
            }
            else
            {
                return (float)(0.5f * (Mathf.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1));
            }
        }

        /// <summary>
        /// Modeled after the exponential function y = 2^(10(x - 1))
        /// </summary>
        static public float ExponentialEaseIn(float p)
        {
            return (float)((p == 0.0f) ? p : Mathf.Pow(2, 10 * (p - 1)));
        }

        /// <summary>
        /// Modeled after the exponential function y = -2^(-10x) + 1
        /// </summary>
        static public float ExponentialEaseOut(float p)
        {
            return (float)((p == 1.0f) ? p : 1 - Mathf.Pow(2, -10 * p));
        }

        /// <summary>
        /// Modeled after the piecewise exponential
        /// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        /// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        /// </summary>
        static public float ExponentialEaseInOut(float p)
        {
            if (p == 0.0 || p == 1.0) return p;

            if (p < 0.5f)
            {
                return (float)(0.5f * Mathf.Pow(2, (20 * p) - 10));
            }
            else
            {
                return (float)(-0.5f * Mathf.Pow(2, (-20 * p) + 10) + 1);
            }
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(13pi/2*x)*Math.Pow(2, 10 * (x - 1))
        /// </summary>
        static public float ElasticEaseIn(float p)
        {
            return (float)(Mathf.Sin(13 * HALFPI * p) * Mathf.Pow(2, 10 * (p - 1)));
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*Math.Pow(2, -10x) + 1
        /// </summary>
        static public float ElasticEaseOut(float p)
        {
            return (float)(Mathf.Sin(-13 * HALFPI * (p + 1)) * Mathf.Pow(2, -10 * p) + 1);
        }

        /// <summary>
        /// Modeled after the piecewise exponentially-damped sine wave:
        /// y = (1/2)*sin(13pi/2*(2*x))*Math.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        /// y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        /// </summary>
        static public float ElasticEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return (float)(0.5f * Mathf.Sin(13 * HALFPI * (2 * p)) * Mathf.Pow(2, 10 * ((2 * p) - 1)));
            }
            else
            {
                return (float)(0.5f * (Mathf.Sin(-13 * HALFPI * ((2 * p - 1) + 1)) * Mathf.Pow(2, -10 * (2 * p - 1)) + 2));
            }
        }

        /// <summary>
        /// Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        /// </summary>
        static public float BackEaseIn(float p)
        {
            return (float)(p * p * p - p * Mathf.Sin(p * PI));
        }

        /// <summary>
        /// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        /// </summary>	
        static public float BackEaseOut(float p)
        {
            float f = (1 - p);
            return (float)(1 - (f * f * f - f * Mathf.Sin(f * PI)));
        }

        /// <summary>
        /// Modeled after the piecewise overshooting cubic function:
        /// y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        /// y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        /// </summary>
        static public float BackEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                float f = 2 * p;
                return (float)(0.5f * (f * f * f - f * Mathf.Sin(f * PI)));
            }
            else
            {
                float f = (1 - (2 * p - 1));
                return (float)(0.5f * (1 - (f * f * f - f * Mathf.Sin(f * PI))) + 0.5f);
            }
        }

        /// <summary>
        /// </summary>
        static public float BounceEaseIn(float p)
        {
            return 1 - BounceEaseOut(1 - p);
        }

        /// <summary>
        /// </summary>
        static public float BounceEaseOut(float p)
        {
            if (p < 4 / 11.0f)
            {
                return (121 * p * p) / 16.0f;
            }
            else if (p < 8 / 11.0f)
            {
                return (363 / 40.0f * p * p) - (99 / 10.0f * p) + 17 / 5.0f;
            }
            else if (p < 9 / 10.0f)
            {
                return (4356 / 361.0f * p * p) - (35442 / 1805.0f * p) + 16061 / 1805.0f;
            }
            else
            {
                return (54 / 5.0f * p * p) - (513 / 25.0f * p) + 268 / 25.0f;
            }
        }

        /// <summary>
        /// </summary>
        static public float BounceEaseInOut(float p)
        {
            if (p < 0.5f)
            {
                return 0.5f * BounceEaseIn(p * 2);
            }
            else
            {
                return 0.5f * BounceEaseOut(p * 2 - 1) + 0.5f;
            }
        }
    }

    public static class Perlin
    {
        // --------------------------------------------------
        // Perlin noise generator for Unity
        // Keijiro Takahashi, 2013, 2015
        // https://github.com/keijiro/PerlinNoise
        //
        // Based on the original implementation by Ken Perlin
        // http://mrl.nyu.edu/~perlin/noise/
        // --------------------------------------------------

        #region Noise functions
        public static float Noise(float x)
        {
            // X: whole number portion of x that rolls over every 255 to 0 as INT
            var X = Mathf.FloorToInt(x) & 0xff;
            // x: decimal portion of x as FLOAT
            x -= Mathf.Floor(x);
            // u: 6x^5 - 15x^4 - 10x^3
            // Fade is a pentic smoothing function for creating an "s-curve" relationship between 0 and 1 for feeding into Lerp
            var u = Fade(x);
            // returned value: is between "+/-x" or "+/-(x-1)" times 2
            // returned value is "u / 1" between "+/-x" or "+/-(x-1)", then multiplied by 2
            // "+/-" is determined based on bitwise operation of the hash lookup
            // in this case, based on the specific overload method, if the returned hash of index X is odd, "+/-" is "+", else "-"
            return Lerp(u, Grad(perm[X], x), Grad(perm[X + 1], x - 1)) * 2;
        }

        public static float Noise(float x, float y)
        {
            var X = Mathf.FloorToInt(x) & 0xff;
            var Y = Mathf.FloorToInt(y) & 0xff;
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            var u = Fade(x);
            var v = Fade(y);
            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;
            return Lerp(v, Lerp(u, Grad(perm[A], x, y), Grad(perm[B], x - 1, y)),
                           Lerp(u, Grad(perm[A + 1], x, y - 1), Grad(perm[B + 1], x - 1, y - 1)));
        }

        public static float Noise(Vector2 coord)
        {
            return Noise(coord.x, coord.y);
        }

        public static float Noise(float x, float y, float z)
        {
            var X = Mathf.FloorToInt(x) & 0xff;
            var Y = Mathf.FloorToInt(y) & 0xff;
            var Z = Mathf.FloorToInt(z) & 0xff;
            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);
            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;
            var AA = (perm[A] + Z) & 0xff;
            var BA = (perm[B] + Z) & 0xff;
            var AB = (perm[A + 1] + Z) & 0xff;
            var BB = (perm[B + 1] + Z) & 0xff;
            return Lerp(w, Lerp(v, Lerp(u, Grad(perm[AA], x, y, z), Grad(perm[BA], x - 1, y, z)),
                                   Lerp(u, Grad(perm[AB], x, y - 1, z), Grad(perm[BB], x - 1, y - 1, z))),
                           Lerp(v, Lerp(u, Grad(perm[AA + 1], x, y, z - 1), Grad(perm[BA + 1], x - 1, y, z - 1)),
                                   Lerp(u, Grad(perm[AB + 1], x, y - 1, z - 1), Grad(perm[BB + 1], x - 1, y - 1, z - 1))));
        }

        public static float Noise(Vector3 coord)
        {
            return Noise(coord.x, coord.y, coord.z);
        }

        #endregion

        #region Private functions
        //Fade takes a float "t" and returns 6t^5 - 15t^4 - 10t^3
        static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        //Lerp returns the unclamped value that is "t / 1" fraction between "a" and "b"
        static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        //Grad takes an integer "hash" and will return either "x" or "-x"
        //If the bitwise operation of "hash & 1" is equal to zero, it will return "x", otherwise "-x"
        //Functionally if hash is an odd number, then "x" is returned; if hash is even, then "-x" is returned
        static float Grad(int hash, float x)
        {
            return (hash & 1) == 0 ? x : -x;
        }

        // This Grad overflow method returns the sum of combination of "+/-x" and "+/-y"
        // Bitwise operation of x is same as above
        // Bitwise operation of y is conditional based on "0x10", as opposed to "0x01"
        static float Grad(int hash, float x, float y)
        {
            return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
        }

        static float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15;
            var u = h < 8 ? x : y;
            var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        static int[] perm = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151
    };

        #endregion
    }
}
