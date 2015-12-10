using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace advent_of_code
{
    class Program
    {
        static void Main(string[] args)
        {
            Day10();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        static void Day10()
        {
            Console.WriteLine("Day 10:");
            var las = "1113222113";
            for (int i = 0; i < 40; ++i)
            {
                las = LookAndSay(las);
            }
            Console.WriteLine("Part 1: {0}", las.Length);
            for (int i = 0; i < 10; ++i)
            {
                las = LookAndSay(las);
            }
            Console.WriteLine("Part 2: {0}", las.Length);
        }

        static string LookAndSay(string s)
        {
            var rest = s;
            var sb = new StringBuilder();
            while (rest.Length > 0)
            {
                var run = rest.TakeWhile(a => a == rest.First());
                sb.Append(run.Count().ToString());
                sb.Append(run.First().ToString());
                rest = rest.Substring(run.Count());
            }
            return sb.ToString();
        }

        static void Day9()
        {
            Console.WriteLine("Day 9:");
            var distances = new Dictionary<string, Dictionary<string, int>>();
            var alldists = new List<int>();
            var paths =
                from line in File.ReadLines("input9.txt")
                let words = line.Split(' ')
                select new { places = new[] { words[0], words[2] }, dist = int.Parse(words[4]) };
            foreach (var path in paths)
            {
                if (!distances.ContainsKey(path.places[0])) distances.Add(path.places[0], new Dictionary<string, int>());
                if (!distances.ContainsKey(path.places[1])) distances.Add(path.places[1], new Dictionary<string, int>());
                distances[path.places[0]][path.places[1]] = path.dist;
                distances[path.places[1]][path.places[0]] = path.dist;
            }
            foreach (var p1 in distances.Keys)
            {
                foreach (var p2 in distances.Keys.Except(new[]{p1}))
                {
                    foreach (var p3 in distances.Keys.Except(new[] { p1, p2 }))
                    {
                        foreach (var p4 in distances.Keys.Except(new[] { p1, p2, p3 }))
                        {
                            foreach (var p5 in distances.Keys.Except(new[] { p1, p2, p3, p4 }))
                            {
                                foreach (var p6 in distances.Keys.Except(new[] { p1, p2, p3, p4, p5 }))
                                {
                                    foreach (var p7 in distances.Keys.Except(new[] { p1, p2, p3, p4, p5, p6 }))
                                    {
                                        foreach (var p8 in distances.Keys.Except(new[] { p1, p2, p3, p4, p5, p6, p7 }))
                                        {
                                            alldists.Add(distances[p1][p2] + distances[p2][p3] + distances[p3][p4]
                                                + distances[p4][p5] + distances[p5][p6] + distances[p6][p7] + distances[p7][p8]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Part 1: {0}", alldists.Min());
            Console.WriteLine("Part 2: {0}", alldists.Max());
        }

        static void Day8()
        {
            Console.WriteLine("Day 8:");
            int full = 0, shortened = 0, lengthened = 0;
            foreach (var line in File.ReadLines("input8.txt"))
            {
                full += line.Length;
                for (int i = 1; i < line.Length - 1; ++i)
                {
                    if (line[i] == '\\')
                    {
                        if (line[i + 1] == 'x')
                            i += 3;
                        else
                            i += 1;
                    }
                    ++shortened;
                }
                lengthened += line.Length + 2 + line.Count(a => a == '\\' || a == '"');
            }
            Console.WriteLine("Part 1: {0}", full - shortened);
            Console.WriteLine("Part 2: {0}", lengthened - full);
        }

        static void Day7()
        {
            Console.WriteLine("Day 7:");
            foreach (var line in File.ReadLines("input7.txt"))
            {
                var sep = new string[] { " -> " };
                var parts = line.Split(sep, StringSplitOptions.None);
                Wire.wires[parts[1]] = new Wire(parts[0]);
            }
            var aval = Wire.wires["a"].Value;
            foreach (var wire in Wire.wires.Values)
            {
                wire.Reset();
            }
            Wire.wires["b"].Value = aval;
            Console.WriteLine("Part 1: {0}", aval);
            Console.WriteLine("Part 2: {0}", Wire.wires["a"].Value);
        }

        class Wire
        {
            private string input;
            private ushort? val = null;
            static public Dictionary<string, Wire> wires = new Dictionary<string, Wire>();

            public Wire(string i) { input = i; }

            public ushort Value
            {
                get
                {
                    if (val == null)
                    {
                        var args = input.Split(' ');
                        if (args.Length == 1)
                        {
                            val = Parse(args[0]);
                        }
                        else if (args.Length == 2)
                        {
                            val = (ushort)~Parse(args[1]);
                        }
                        else
                        {
                            val = Op(args[1])(Parse(args[0]), Parse(args[2]));
                        }
                    }
                    return val ?? 0;
                }

                set { val = value; }
            }

            public void Reset() { val = null; }

            static ushort Parse(string s)
            {
                return IsNumeric(s) ? ushort.Parse(s) : wires[s].Value;
            }

            static Func<ushort, ushort, ushort> Op(string s)
            {
                if (s == "AND")
                {
                    return (a, b) => (ushort)(a & b);
                }
                else if (s == "OR")
                {
                    return (a, b) => (ushort)(a | b);
                }
                else if (s == "RSHIFT")
                {
                    return (a, b) => (ushort)(a >> b);
                }
                else // s == "LSHIFT"
                {
                    return (a, b) => (ushort)(a << b);
                }
            }
        }

        static bool IsNumeric(string s)
        {
            return s.All(Char.IsDigit);
        }

        static void Day6()
        {
            Console.WriteLine("Day 6:");
            int count1 = 0, count2 = 0, x1, x2, y1, y2;
            var lights1 = new bool[1000, 1000];
            var lights2 = new int[1000, 1000];            
            string[] coords1, coords2;
            foreach (var line in File.ReadLines("input6.txt"))
            {
                var words = line.Split(' ');
                if (words[0] == "toggle")
                {
                    coords1 = words[1].Split(',');
                    coords2 = words[3].Split(',');
                }
                else
                {
                    coords1 = words[2].Split(',');
                    coords2 = words[4].Split(',');
                }
                x1 = int.Parse(coords1[0]);
                y1 = int.Parse(coords1[1]);
                x2 = int.Parse(coords2[0]);
                y2 = int.Parse(coords2[1]);
                for (int i = x1; i <= x2; ++i)
                {
                    for (int j = y1; j <= y2; ++j)
                    {
                        if (words[0] == "toggle")
                        {
                            lights1[i, j] = !lights1[i, j];
                            lights2[i, j] += 2;
                        }
                        else if (words[1] == "on")
                        {
                            lights1[i, j] = true;
                            lights2[i, j] += 1;
                        }
                        else
                        {
                            lights1[i, j] = false;
                            if (lights2[i, j] > 0)
                                lights2[i, j] -= 1;
                        }
                    }
                }
            }
            foreach (var light in lights1)
            {
                if (light) ++count1;
            }
            foreach (var light in lights2)
            {
                count2 += light;
            }
            Console.WriteLine("Part 1: {0}", count1);
            Console.WriteLine("Part 2: {0}", count2);
        }

        static void Day5()
        {
            Console.WriteLine("Day 5:");
            int count1 = 0, count2 = 0;
            foreach (var line in File.ReadLines("input5.txt"))
            {
                if (line.Select(a => IsVowel(a)).Sum(a => a ? 1 : 0) >= 3 &&
                    HasDuplicate(line) && !HasNaughty(line))
                    ++count1;
                if (HasTwoPairs(line) && HasTriplet(line))
                    ++count2;
            }
            Console.WriteLine("Part 1: {0}", count1);
            Console.WriteLine("Part 2: {0}", count2);
        }

        static bool IsVowel(char c)
        {
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
        }

        static bool HasDuplicate(string s)
        {
            var alphabet = Enumerable.Range('a', 26).Select(x => (char)x).ToArray();
            foreach (var letter in alphabet)
            {
                if (s.Contains(new string(new[] { letter, letter }))) return true;
            }
            return false;
        }

        static bool HasNaughty(string s)
        {
            var naughty = new[] { "ab", "cd", "pq", "xy" };
            foreach (var str in naughty)
            {
                if (s.Contains(str)) return true;
            }
            return false;
        }

        static bool HasTwoPairs(string s)
        {
            var pairs = new List<string>();
            for (int i = 0; i < s.Length - 1; ++i)
            {
                pairs.Add(new string(new[] { s[i], s[i + 1] }));
            }
            for (int i = 0; i < pairs.Count; ++i)
            {
                if (pairs.Skip(i + 2).Contains(pairs[i])) return true;
            }
            return false;
        }

        static bool HasTriplet(string s)
        {
            for (int i = 0; i < s.Length - 2; ++i)
            {
                if (s[i] == s[i + 2]) return true;
            }
            return false;
        }

        static void Day4()
        {
            Console.WriteLine("Day 4:");
            var m = MD5.Create();
            var found1 = false;
            for (int i = 0; ; ++i)
            {
                var str = Hash(m, "iwrupvqb" + i.ToString());
                if (str.Take(5).All(a => a == '0') && !found1) 
                { 
                    Console.WriteLine("Part 1: {0}", i);
                    found1 = true;
                }
                if (str.Take(6).All(a => a == '0'))
                {
                    Console.WriteLine("Part 2: {0}", i);
                    return;
                }
            }
        }

        static string Hash(MD5 m, string str)
        {
            var data = m.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (var b in data)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        static void Day3()
        {
            Console.WriteLine("Day 3:");
            var visited1 = new HashSet<Tuple<int, int>>();
            var visited2 = new HashSet<Tuple<int, int>>();
            var robo = false;
            var santa1 = new[] { 0, 0 };
            var santa2 = new[] { 0, 0 };
            var robosanta = new[] { 0, 0 };
            foreach (var dir in File.ReadAllText("input3.txt"))
            {
                if (dir == '^')
                {
                    ++santa1[1];
                    if (robo) ++robosanta[1];
                    else ++santa2[1];
                }
                else if (dir == 'v')
                {
                    --santa1[1];
                    if (robo) --robosanta[1];
                    else --santa2[1];
                }
                else if (dir == '>')
                {
                    ++santa1[0];
                    if (robo) ++robosanta[0];
                    else ++santa2[0];
                }
                else if (dir == '<')
                {
                    --santa1[0];
                    if (robo) --robosanta[0];
                    else --santa2[0];
                }
                visited1.Add(Tuple.Create(santa1[0], santa1[1]));
                visited2.Add(Tuple.Create(santa2[0], santa2[1]));
                visited2.Add(Tuple.Create(robosanta[0], robosanta[1]));
                robo = !robo;
            }
            Console.WriteLine("Part 1: {0}", visited1.Count);
            Console.WriteLine("Part 2: {0}", visited2.Count);
        }

        static void Day2()
        {
            Console.WriteLine("Day 2:");
            int paper = 0, ribbon = 0;
            foreach (var line in File.ReadLines("input2.txt"))
            {
                var sides = line.Split('x').Select(int.Parse).OrderBy(a => a).ToArray();
                paper += 3 * sides[0] * sides[1] + 2 * (sides[1] * sides[2] + sides[0] * sides[2]);
                ribbon += 2 * (sides[0] + sides[1]) + sides[0] * sides[1] * sides[2];
            }
            Console.WriteLine("Part 1: {0}", paper);
            Console.WriteLine("Part 2: {0}", ribbon);
        }

        static void Day1()
        {
            Console.WriteLine("Day 1:");
            int floor = 0, pos = 0, firstbasement = 0;
            var found = false;
            foreach (var c in File.ReadAllText("input1.txt"))
            {
                ++pos;
                if (c == '(') ++floor;
                else --floor;
                if (floor == -1 && !found)
                {
                    firstbasement = pos;
                    found = true;
                }
            }
            Console.WriteLine("Part 1: {0}", floor);
            Console.WriteLine("Part 2: {0}", firstbasement);
        }
    }
}
