using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using MiniJSON;

namespace advent_of_code
{
    class Program
    {
        static void Main(string[] args)
        {
            Day18();
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();  
        }

        static void Day18()
        {
            var grid =
                (from line in File.ReadLines("input18.txt")
                select line.ToCharArray()).ToArray();
            var grid1 = grid;
            var grid2 = grid;
            grid2[0][0] = '#';
            grid2[0][99] = '#';
            grid2[99][0] = '#';
            grid2[99][99] = '#';
            for (int i = 0; i < 100; ++i)
            {
                grid1 = GameOfLife(grid1);
                grid2 = GameOfLife(grid2);
                grid2[0][0] = '#';
                grid2[0][99] = '#';
                grid2[99][0] = '#';
                grid2[99][99] = '#';
            }
            var count1 = 0;
            var count2 = 0;
            foreach (var line in grid1){
                foreach (var c in line)
                {
                    if (c == '#')
                        count1++;
                }
            }
            foreach (var line in grid2)
            {
                foreach (var c in line)
                {
                    if (c == '#')
                        count2++;
                }
            }
            Console.WriteLine(count1);
            Console.WriteLine(count2);
            
        }

        static char[][] GameOfLife(char[][] board)
        {
            var on = '#';
            var off = '.';

            var output = new List<string>();

            for (int i = 0; i < board.Length; ++i)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < board[i].Length; ++j)
                {
                    var neighbors = 0;
                    if (i != 0 && j != 0 && board[i - 1][j - 1] == on)
                        ++neighbors;
                    if (i != 0 && board[i - 1][j] == on)
                        ++neighbors;
                    if (i != 0 && j != board[i].Length - 1 && board[i - 1][j + 1] == on)
                        ++neighbors;
                    if (j != board[i].Length - 1 && board[i][j + 1] == on)
                        ++neighbors;
                    if (i != board.Length - 1 && j != board[i].Length - 1 && board[i + 1][j + 1] == on)
                        ++neighbors;
                    if (i != board.Length - 1 && board[i + 1][j] == on)
                        ++neighbors;
                    if (i != board.Length - 1 && j != 0 && board[i + 1][j - 1] == on)
                        ++neighbors;
                    if (j != 0 && board[i][j - 1] == on)
                        ++neighbors;
                    if (board[i][j] == on)
                    {
                        if (neighbors == 2 || neighbors == 3)
                            sb.Append(on);
                        else
                            sb.Append(off);
                    }
                    else
                    {
                        if (neighbors == 3)
                            sb.Append(on);
                        else
                            sb.Append(off);
                    }
                }
                output.Add(sb.ToString());
            }
            return (from line in output select line.ToCharArray()).ToArray();
        }

        // Day 17 was done in python!

        static void Day16()
        {
            Console.WriteLine("Day 16:");
            var sues =
                from line in File.ReadLines("input16.txt")
                let words = line.Split(new[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries)
                select new Func<Sue>(() =>
                {
                    var sue = new Sue { number = int.Parse(words[1]) };
                    sue.info[words[2]] = int.Parse(words[3]);
                    sue.info[words[4]] = int.Parse(words[5]);
                    sue.info[words[6]] = int.Parse(words[7]);
                    return sue;
                }
            )();
            var testsue = new Sue
            {
                number = 0,
                info = new Dictionary<string, int>
                {
                    {"children", 3},
                    {"cats", 7},
                    {"samoyeds", 2},
                    {"pomeranians", 3},
                    {"akitas", 0},
                    {"vizslas", 0},
                    {"goldfish", 5},
                    {"trees", 3},
                    {"cars", 2},
                    {"perfumes", 1}
                }
            };
            var ineligible1 = new List<int>();
            var ineligible2 = new List<int>();
            foreach (var sue in sues)
            {
                foreach (var key in testsue.info.Keys)
                {
                    if (sue.info.ContainsKey(key))
                    {
                        if (sue.info[key] != testsue.info[key])
                        {
                            ineligible1.Add(sue.number);
                            break;
                        }
                    }
                }
                foreach (var key in testsue.info.Keys)
                {
                    if (sue.info.ContainsKey(key))
                    {
                        if (key == "cats" || key == "trees")
                        {
                            if (sue.info[key] <= testsue.info[key])
                            {
                                ineligible2.Add(sue.number);
                                break;
                            }
                        }
                        else if (key == "pomeranians" || key == "goldfish")
                        {
                            if (sue.info[key] >= testsue.info[key])
                            {
                                ineligible2.Add(sue.number);
                                break;
                            } 
                        }
                        else
                        {
                            if (sue.info[key] != testsue.info[key])
                            {
                                ineligible2.Add(sue.number);
                                break;
                            }
                        }
                    }
                }
            }
            foreach (var sue in sues.Where(a => !ineligible1.Contains(a.number)))
            {
                Console.WriteLine("Part 1: {0}", sue.number);
            }
            foreach (var sue in sues.Where(a => !ineligible2.Contains(a.number)))
            {
                Console.WriteLine("Part 2: {0}", sue.number);
            }
        }

        class Sue
        {
            public int number;
            public Dictionary<string, int> info = new Dictionary<string,int>();
        }

        static void Day15()
        {
            Console.WriteLine("Day 15:");
            var ingredients =
                (from line in File.ReadLines("input15.txt")
                let words = line.Split(new[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries)
                select new Ingredient() { 
                    name = words[0],
                    capacity = int.Parse(words[2]), 
                    durability = int.Parse(words[4]),
                    flavor = int.Parse(words[6]),
                    texture = int.Parse(words[8]),
                    calories = int.Parse(words[10])
                }).ToList();
            var topscore = 0;
            var setcal = 0;
            var cookie = new Cookie();

            for (int a = 0; a < 101; ++a)
            {
                for (int b = 0; b < 101 - a; ++b)
                {
                    for (int c = 0; c < 101 - a - b; ++c)
                    {
                        for (int d = 0; d < 101 - a - b - c; ++d)
                        {
                            cookie.ingredients[ingredients[0]] = a;
                            cookie.ingredients[ingredients[1]] = b;
                            cookie.ingredients[ingredients[2]] = c;
                            cookie.ingredients[ingredients[3]] = d;
                            if (cookie.Score > topscore) topscore = cookie.Score;
                            if (cookie.Calories == 500 && cookie.Score > setcal) setcal = cookie.Score;
                        }
                    }
                }
            }

            Console.WriteLine("Part 1: {0}", topscore);
            Console.WriteLine("Part 2: {0}", setcal);
        }

        class Ingredient
        {
            public string name;
            public int capacity, durability, flavor, texture, calories;

            public override bool Equals(object other)
            {
                if (other is Ingredient)
                    return name == (other as Ingredient).name;
                else return false;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }
        }

        class Cookie
        {
            public Dictionary<Ingredient, int> ingredients = new Dictionary<Ingredient, int>();

            public int Capacity { get { return Math.Max(0, ingredients.Sum(a => a.Key.capacity * a.Value)); } }
            public int Durability { get { return Math.Max(0, ingredients.Sum(a => a.Key.durability * a.Value)); } }
            public int Flavor { get { return Math.Max(0, ingredients.Sum(a => a.Key.flavor * a.Value)); } }
            public int Texture { get { return Math.Max(0, ingredients.Sum(a => a.Key.texture * a.Value)); } }
            public int Calories { get { return ingredients.Sum(a => a.Key.calories * a.Value); } }
            public int Score { get { return Capacity * Durability * Flavor * Texture; } }
        }

        static void Day14()
        {
            Console.WriteLine("Day 14:");
            var reindeer =
                from line in File.ReadLines("input14.txt")
                let words = line.Split(' ')
                select new
                {
                    name = words[0],
                    speed = int.Parse(words[3]),
                    flighttime = int.Parse(words[6]),
                    resttime = int.Parse(words[13])
                };
            var distances =
                from deer in reindeer
                let totaltime = deer.flighttime + deer.resttime
                select ((2503 / totaltime) * deer.flighttime + (2503 % totaltime > deer.flighttime ? deer.flighttime : 2503 % totaltime)) * deer.speed;
            Console.WriteLine("Part 1: {0}", distances.Max());
            var points = new Dictionary<string, int>();
            foreach (var deer in reindeer) { points[deer.name] = 0; }
            for (int i = 1; i < 2503; ++i)
            {
                var currdist =
                    from deer in reindeer
                    let totaltime = deer.flighttime + deer.resttime
                    select new { deer, distance = ((i / totaltime) * deer.flighttime + (i % totaltime > deer.flighttime ? deer.flighttime : i % totaltime)) * deer.speed };
                var front = currdist.Max(a => a.distance);
                foreach (var deer in currdist) 
                {
                    if (deer.distance == front)
                    {
                        ++points[deer.deer.name];
                    }
                }
            }
            Console.WriteLine("Part 2: {0}", points.Max(a => a.Value));
        }

        static void Day13()
        {
            Console.WriteLine("Day 13:");
            var happy = new Dictionary<string, Dictionary<string, int>>();
            var allHappiness = new List<int>();
            foreach (var line in File.ReadLines("input13.txt"))
            {
                var words = line.Split(' ', '.');
                if (!happy.ContainsKey(words[0])){
                    happy[words[0]] = new Dictionary<string,int>();
                }
                happy[words[0]][words[10]] = int.Parse(words[3]) * (words[2] == "gain" ? 1 : -1);
            }
            foreach (var perm in Permutations(happy.Keys))
            {
                var hsum = 0;
                for (int i = 0; i < happy.Count(); ++i)
                {
                    var lneighbor = i == 0 ? happy.Count() - 1 : i - 1;
                    var rneighbor = i == happy.Count() - 1 ? 0 : i + 1;
                    hsum += happy[perm.ElementAt(i)][perm.ElementAt(lneighbor)];
                    hsum += happy[perm.ElementAt(i)][perm.ElementAt(rneighbor)];
                }
                allHappiness.Add(hsum);
            }
            Console.WriteLine("Part 1: {0}", allHappiness.Max());
            allHappiness.Clear();
            happy["self"] = new Dictionary<string, int>();
            foreach (var p in happy.Keys.Except(new[] { "self" }))
            {
                happy[p]["self"] = 0;
                happy["self"][p] = 0;
            }
            foreach (var perm in Permutations(happy.Keys))
            {
                var hsum = 0;
                for (int i = 0; i < happy.Count(); ++i)
                {
                    var lneighbor = i == 0 ? happy.Count() - 1 : i - 1;
                    var rneighbor = i == happy.Count() - 1 ? 0 : i + 1;
                    hsum += happy[perm.ElementAt(i)][perm.ElementAt(lneighbor)];
                    hsum += happy[perm.ElementAt(i)][perm.ElementAt(rneighbor)];
                }
                allHappiness.Add(hsum);
            }
            Console.WriteLine("Part 2: {0}", allHappiness.Max());
        }

        static IEnumerable<IEnumerable<T>> Permutations<T>(IEnumerable<T> list)
        {
            if (list.Count() == 1) yield return list;
            else
            {
                foreach (var elem in list)
                {
                    var others = Permutations(list.Except(new[] { elem }));
                    foreach (var other in others)
                    {
                        yield return other.Concat(new[] { elem });
                    }
                }
            }
        }

        static void Day12()
        {
            Console.WriteLine("Day 12:");

            var jobj = Json.Deserialize(File.ReadAllText("input12.json"));
            
            Console.WriteLine("Part 1: {0}", Sum(jobj));
            Console.WriteLine("Part 2: {0}", SumWithoutRed(jobj));
        }

        static double SumWithoutRed(object jo)
        {
            if (jo is Dictionary<string, object>)
            {
                var s = 0.0;
                if ((jo as Dictionary<string, object>).Values.Contains("red")) return 0;
                    foreach (var val in (jo as Dictionary<string, object>))
                    {
                        s += SumWithoutRed(val.Value);
                    }
                return s;
            }
            else if (jo is List<object>)
            {
                var s = 0.0;
                foreach (var val in (jo as List<object>))
                {
                    s += SumWithoutRed(val);
                }
                return s;
            }
            else if (jo is string)
            {
                return 0;
            }
            else
            {
                return double.Parse(jo.ToString());
            }
        }

        static double Sum(object jo)
        {
            if (jo is Dictionary<string, object>)
            {
                var s = 0.0;
                foreach (var val in (jo as Dictionary<string, object>).Values)
                {
                    s += Sum(val);
                }
                return s;
            }
            else if (jo is List<object>)
            {
                var s = 0.0;
                foreach (var val in (jo as List<object>))
                {
                    s += Sum(val);
                }
                return s;
            }
            else if (jo is string)
            {
                return 0;
            }
            else 
            {
                return double.Parse(jo.ToString());
            }
        }


        static void Day11()
        {
            Console.WriteLine("Day 11:");
            var input = "cqjxjnds".ToList();
            int part = 1;
            while (true)
            {
                if (HasTwoDoublets(input) && HasStraight(input) && !input.Any(a => a == 'i' || a == 'o' || a == 'l'))
                {
                    Console.WriteLine("Part {0}: {1}", part, new string(input.ToArray()));
                    ++part;
                    if (part > 2) break;
                }
                var incpos = 7;
                while (true)
                {
                    if (input[incpos] == 'z')
                    {
                        input[incpos] = 'a';
                        --incpos;
                    }
                    else
                    {
                        ++input[incpos];
                        break;
                    }
                }
            }
        }

        static bool HasTwoDoublets(List<char> s)
        {
            var pairs = new List<string>();
            for (int i = 0; i < s.Count - 1; ++i)
            {
                pairs.Add(new string(new[] { s[i], s[i + 1] }));
            }
            for (int i = 0; i < pairs.Count; ++i)
            {
                if (pairs[i][0] == pairs[i][1] && pairs.Skip(i + 2).Any(a => a[0] == a[1]))
                {
                    return true;
                }
            }
            return false;
        }

        static bool HasStraight(List<char> s)
        {
            var trips = new List<string>();
            for (int i = 0; i < s.Count - 2; ++i){
                trips.Add(new string(new[] { s[i], s[i + 1], s[i + 2] }));
            }
            return trips.Any(a => a[0] + 1 == a[1] && a[1] + 1 == a[2]);
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
            var sb = new StringBuilder();
            var curr = s[0];
            var count = 1;
            for (int i = 1; i < s.Length; ++i)
            {
                if (s[i] == curr) ++count;
                else
                {
                    sb.Append(count.ToString());
                    sb.Append(curr);
                    curr = s[i];
                    count = 1;
                }
            }
            sb.Append(count.ToString());
            sb.Append(curr);
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
