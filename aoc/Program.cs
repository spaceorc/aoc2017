using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc
{
	class Program
	{
		static void Main()
		{
			Main15_2();
		}

		static void Main15_2()
		{
			var a = 703L;
			var b = 516L;
			var result = 0;
			for (int i = 0; i < 5_000_000; i++)
			{
				do
				{
					a = a * 16807L % 2147483647L;
				} while ((a & 0b11) != 0);
				do
				{
					b = b * 48271L % 2147483647L;
				} while ((b & 0b111) != 0);
				if ((a & 0b1111111111111111) == (b & 0b1111111111111111))
					result++;
			}
			Console.Out.WriteLine(result);
		}

		static void Main15()
		{
			var a = 703L;
			var b = 516L;
			var result = 0;
			for (int i = 0; i < 40_000_000; i++)
			{
				a = a * 16807L % 2147483647L;
				b = b * 48271L % 2147483647L;
				if ((a & 0b1111111111111111) == (b & 0b1111111111111111))
					result++;
			}
			Console.Out.WriteLine(result);
		}

		static void Main14_2()
		{
			var input = "hxtvlmkl";
			var grid = new int[128, 128];
			for (int y = 0; y < 128; y++)
			{
				var hash = KnotHash(input + "-" + y);
				for (var hi = 0; hi < hash.Length; hi++)
				{
					var c = hash[hi];
					var x = hi * 8 + 7;
					while (c > 0)
					{
						if (c % 2 == 1)
							grid[x, y] = 1;
						c >>= 1;
						x--;
					}
				}
			}

			var result = 0;
			for (int x = 0; x < 128; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					if (grid[x, y] == 1)
					{
						result++;
						var queue = new Queue<(int x, int y)>();
						queue.Enqueue((x, y));
						grid[x, y] = 0;
						while (queue.Count > 0)
						{
							var cur = queue.Dequeue();
							var d = new (int dx, int dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
							foreach (var (dx, dy) in d)
							{
								if (cur.x + dx >= 0 && cur.x + dx < 128 
									&& cur.y + dy >= 0 && cur.y + dy < 128)
								{
									if (grid[cur.x + dx, cur.y + dy] == 1)
									{
										grid[cur.x + dx, cur.y + dy] = 0;
										queue.Enqueue((cur.x + dx, cur.y + dy));
									}
								}
							}
						}
					}
				}
			}
			Console.Out.WriteLine(result);
		}

		static void Main14()
		{
			var input = "hxtvlmkl";
			var result = 0;
			for (int i = 0; i < 128; i++)
			{
				var hash = KnotHash(input + "-" + i);
				foreach (var h in hash)
				{
					var c = h;
					while (c > 0)
					{
						if (c % 2 == 1)
							result++;
						c >>= 1;
					}
				}
			}
			Console.Out.WriteLine(result);
		}

		static void Main13_2()
		{
			var lines = File.ReadAllLines(@"..\..\input13.txt");
			var scanners = new(int level, int depth)[lines.Length];
			var forbidTimes = new bool[10000000];
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var split = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
				var level = int.Parse(split[0]);
				var depth = int.Parse(split[1]);
				scanners[i] = (level, depth);
				for (int t = 0; t < forbidTimes.Length + level; t += depth * 2 - 2)
				{
					if (t - level >= 0)
						forbidTimes[t - level] = true;
				}
			}
			for (int i = 0; i < forbidTimes.Length; i++)
			{
				if (!forbidTimes[i])
				{
					Console.Out.WriteLine(i);
					return;
				}
			}
		}

		static void Main13()
		{
			var lines = File.ReadAllLines(@"..\..\input13.txt");
			var sev = 0;
			foreach (var line in lines)
			{
				var split = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
				var level = int.Parse(split[0]);
				var depth = int.Parse(split[1]);
				var pos = level % (depth * 2 - 2);
				if (pos == 0)
					sev += level * depth;
			}
			Console.Out.WriteLine(sev);
		}

		static void Main12_2()
		{
			var lines = File.ReadAllLines(@"..\..\input12.txt");
			var re = new Regex(@"^(?<id>\d+) <-> (?<link>\d+)(?:, (?<link>\d+))*$", RegexOptions.Compiled);
			var gr = new Dictionary<int, int[]>();
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var id = int.Parse(match.Groups["id"].Value);
				var links = match.Groups["link"].Captures.Cast<Capture>().Select(x => int.Parse(x.Value)).ToArray();
				gr[id] = links;
			}
			var totalUsed = new HashSet<int>();
			var groupCount = 0;
			foreach (var start in gr.Keys)
			{
				if (!totalUsed.Add(start))
					continue;
				groupCount++;
				var used = new HashSet<int>();
				var queue = new Queue<int>();
				queue.Enqueue(start);
				used.Add(start);
				while (queue.Count > 0)
				{
					var cur = queue.Dequeue();
					foreach (var link in gr[cur])
					{
						if (used.Add(link))
							queue.Enqueue(link);
					}
				}
				totalUsed.UnionWith(used);
			}
			Console.Out.WriteLine(groupCount);
		}

		static void Main12()
		{
			var lines = File.ReadAllLines(@"..\..\input12.txt");
			var re = new Regex(@"^(?<id>\d+) <-> (?<link>\d+)(?:, (?<link>\d+))*$", RegexOptions.Compiled);
			var gr = new Dictionary<int, int[]>();
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var id = int.Parse(match.Groups["id"].Value);
				var links = match.Groups["link"].Captures.Cast<Capture>().Select(x => int.Parse(x.Value)).ToArray();
				gr[id] = links;
			}
			var used = new HashSet<int>();
			var queue = new Queue<int>();
			queue.Enqueue(0);
			used.Add(0);
			while (queue.Count > 0)
			{
				var cur = queue.Dequeue();
				foreach (var link in gr[cur])
				{
					if (used.Add(link))
						queue.Enqueue(link);
				}
			}
			Console.Out.WriteLine(used.Count);
		}

		static void Main11_2()
		{
			var input = File.ReadAllText(@"..\..\input11.txt").Trim();
			var steps = input.Split(',');
			var dirs = new[] { "n", "nw", "sw", "s", "se", "ne" };
			var pos = new Coord(0, 0);
			var max = 0;
			foreach (var step in steps)
			{
				var dir = Array.IndexOf(dirs, step);
				pos = pos.Neighbor(dir);
				var dist = pos.DistanceTo(new Coord(0, 0));
				if (dist > max)
					max = dist;
			}
			Console.Out.WriteLine(max);
		}

		static void Main11()
		{
			var input = File.ReadAllText(@"..\..\input11.txt").Trim();
			var steps = input.Split(',');
			var dirs = new[] { "n", "nw", "sw", "s", "se", "ne" };
			var pos = new Coord(0, 0);
			foreach (var step in steps)
			{
				var dir = Array.IndexOf(dirs, step);
				pos = pos.Neighbor(dir);
			}
			Console.Out.WriteLine(pos.DistanceTo(new Coord(0, 0)));
		}

		static void Main10_2()
		{
			var input = "187,254,0,81,169,219,1,190,19,102,255,56,46,32,2,216";
			var knotHash = KnotHash(input);
			var res = KnotHashToString(knotHash);
			Console.Out.WriteLine(res);
		}

		private static string KnotHashToString(byte[] dense)
		{
			var res = "";
			foreach (var d in dense)
				res += d.ToString("x2");
			return res;
		}

		private static byte[] KnotHash(string input)
		{
			var lens = input.Select(c => (int)c).Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();
			var skip = 0;
			var cur = 0;
			var list = Enumerable.Range(0, 256).ToArray();
			for (int k = 0; k < 64; k++)
			{
				foreach (var len in lens)
				{
					for (int i = 0; i < len / 2; i++)
					{
						var tmp = list[(cur + i) % list.Length];
						list[(cur + i) % list.Length] = list[(cur + len - 1 - i) % list.Length];
						list[(cur + len - 1 - i) % list.Length] = tmp;
					}
					cur = (cur + len + skip) % list.Length;
					skip++;
				}
			}
			var dense = new byte[16];
			for (int i = 0; i < 16; i++)
			{
				for (int k = 0; k < 16; k++)
					dense[i] ^= (byte)list[i * 16 + k];
			}
			return dense;
		}

		static void Main10()
		{
			var input = "187,254,0,81,169,219,1,190,19,102,255,56,46,32,2,216";
			var lens = input.Split(',').Select(int.Parse).ToArray();
			var skip = 0;
			var cur = 0;
			var list = Enumerable.Range(0, 256).ToArray();
			foreach (var len in lens)
			{
				for (int i = 0; i < len / 2; i++)
				{
					var tmp = list[(cur + i) % list.Length];
					list[(cur + i) % list.Length] = list[(cur + len - 1 - i) % list.Length];
					list[(cur + len - 1 - i) % list.Length] = tmp;
				}
				cur = (cur + len + skip) % list.Length;
				skip++;
			}
			Console.Out.WriteLine(list[0] * list[1]);
		}

		static void Main9_2()
		{
			var input = File.ReadAllText(@"..\..\input9.txt").Trim();
			var level = 0;
			var state = 0;
			var score = 0;
			for (var i = 0; i < input.Length; i++)
			{
				var c = input[i];
				switch (state)
				{
					case 0:
						switch (c)
						{
							case '{':
								level++;
								break;
							case '}':
								level--;
								if (level < 0)
									throw new InvalidOperationException($"char: {c}; pos: {i}");
								break;
							case '<':
								state = 1;
								break;
							case ',':
								break;
							default:
								throw new InvalidOperationException($"char: {c}; pos: {i}");
						}
						break;
					case 1:
						switch (c)
						{
							case '>':
								state = 0;
								break;
							case '!':
								state = 2;
								break;
							default:
								score++;
								break;
						}
						break;
					case 2:
						state = 1;
						break;
				}
			}
			Console.Out.WriteLine(score);
		}

		static void Main9()
		{
			var input = File.ReadAllText(@"..\..\input9.txt").Trim();
			var level = 0;
			var state = 0;
			var score = 0;
			for (var i = 0; i < input.Length; i++)
			{
				var c = input[i];
				switch (state)
				{
					case 0:
						switch (c)
						{
							case '{':
								level++;
								score += level;
								break;
							case '}':
								level--;
								if (level < 0)
									throw new InvalidOperationException($"char: {c}; pos: {i}");
								break;
							case '<':
								state = 1;
								break;
							case ',':
								break;
							default:
								throw new InvalidOperationException($"char: {c}; pos: {i}");
						}
						break;
					case 1:
						switch (c)
						{
							case '>':
								state = 0;
								break;
							case '!':
								state = 2;
								break;
						}
						break;
					case 2:
						state = 1;
						break;
				}
			}
			Console.Out.WriteLine(score);
		}

		static void Main8_2()
		{
			var lines = File.ReadAllLines(@"..\..\input8.txt");
			var re = new Regex(@"^(?<r>\w+) (?<op>inc|dec) (?<v>-?\d+) if (?<cr>\w+) (?<cop><|>|<=|>=|==|!=) (?<cv>-?\d+)$", RegexOptions.Compiled);
			var registers = new ConcurrentDictionary<string, int>();
			var max = 0;
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var r = match.Groups["r"].Value;
				var op = match.Groups["op"].Value;
				var v = int.Parse(match.Groups["v"].Value);
				var cr = match.Groups["cr"].Value;
				var cop = match.Groups["cop"].Value;
				var cv = int.Parse(match.Groups["cv"].Value);
				if (Check(cr, cop, cv))
				{
					var vv = Apply(r, op, v);
					if (vv > max)
						max = vv;
				}
			}
			Console.Out.WriteLine(max);

			int Apply(string r, string op, int v)
			{
				var vv = registers.GetOrAdd(r, 0);
				switch (op)
				{
					case "inc":
						registers[r] = vv + v;
						return vv + v;
					case "dec":
						registers[r] = vv - v;
						return vv - v;
					default:
						throw new InvalidOperationException(op);
				}
			}

			bool Check(string r, string op, int v)
			{
				var vv = registers.GetOrAdd(r, 0);
				switch (op)
				{
					case "==":
						return vv == v;
					case "!=":
						return vv != v;
					case ">":
						return vv > v;
					case "<":
						return vv < v;
					case ">=":
						return vv >= v;
					case "<=":
						return vv <= v;
					default:
						throw new InvalidOperationException(op);
				}
			}
		}

		static void Main8()
		{
			var lines = File.ReadAllLines(@"..\..\input8.txt");
			var re = new Regex(@"^(?<r>\w+) (?<op>inc|dec) (?<v>-?\d+) if (?<cr>\w+) (?<cop><|>|<=|>=|==|!=) (?<cv>-?\d+)$", RegexOptions.Compiled);
			var registers = new ConcurrentDictionary<string, int>();
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var r = match.Groups["r"].Value;
				var op = match.Groups["op"].Value;
				var v = int.Parse(match.Groups["v"].Value);
				var cr = match.Groups["cr"].Value;
				var cop = match.Groups["cop"].Value;
				var cv = int.Parse(match.Groups["cv"].Value);
				if (Check(cr, cop, cv))
					Apply(r, op, v);
			}
			Console.Out.WriteLine(registers.Values.Max());

			void Apply(string r, string op, int v)
			{
				var vv = registers.GetOrAdd(r, 0);
				switch (op)
				{
					case "inc":
						registers[r] = vv + v;
						break;
					case "dec":
						registers[r] = vv - v;
						break;
					default:
						throw new InvalidOperationException(op);
				}
			}

			bool Check(string r, string op, int v)
			{
				var vv = registers.GetOrAdd(r, 0);
				switch (op)
				{
					case "==":
						return vv == v;
					case "!=":
						return vv != v;
					case ">":
						return vv > v;
					case "<":
						return vv < v;
					case ">=":
						return vv >= v;
					case "<=":
						return vv <= v;
					default:
						throw new InvalidOperationException(op);
				}
			}
		}

		static void Main7_2()
		{
			var lines = File.ReadAllLines(@"..\..\input7.txt");
			var re = new Regex(@"^(?<parent>\w+) \((?<weight>\d+)\)(?: -> ((?<child>\w+), )*(?<child>\w+))?$", RegexOptions.Compiled);
			var all = new HashSet<string>();
			var nodes = new Dictionary<string, (int weight, string[] children)>();
			var allChildren = new HashSet<string>();
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var parent = match.Groups["parent"].Value;
				var weight = int.Parse(match.Groups["weight"].Value);
				var children = match.Groups["child"].Captures.Cast<Capture>().Select(x => x.Value).ToArray();
				nodes.Add(parent, (weight, children));
				all.Add(parent);
				allChildren.UnionWith(children);
			}
			all.ExceptWith(allChildren);
			var root = all.Single();

			var totalWeights = new Dictionary<string, long>();
			foreach (var n in nodes.Keys)
				GetTotalWeight(n);

			(string node, long validWeight) inv = (root, 0);
			while (true)
			{
				var next = FindInvalid(inv.node);
				if (next.node == null)
				{
					var validTotalWeight = inv.validWeight;
					var totalWeight = totalWeights[inv.node];
					Console.Out.WriteLine(nodes[inv.node].weight - totalWeight + validTotalWeight);
					return;
				}
				inv = next;
			}

			(string node, long validWeight) FindInvalid(string n)
			{
				var weights = nodes[n].children
					.GroupBy(c => totalWeights[c])
					.Select(x => new { w = x.Key, cnt = x.Count() })
					.ToList();
				if (weights.Count <= 1)
					return (null, 0);
				var wrongWeight = weights
					.OrderBy(x => x.cnt)
					.Select(x => x.w)
					.First();
				var validWeight = weights
					.OrderBy(x => x.cnt)
					.Select(x => x.w)
					.Last();
				foreach (var c in nodes[n].children)
				{
					if (totalWeights[c] == wrongWeight)
						return (c, validWeight);
				}
				return (null, 0);
			}

			long GetTotalWeight(string n)
			{
				if (totalWeights.TryGetValue(n, out var value))
					return value;
				var totalWeight = nodes[n].weight + nodes[n].children.Select(GetTotalWeight).Sum();
				totalWeights[n] = totalWeight;
				return totalWeight;
			}
		}

		static void Main7()
		{
			var lines = File.ReadAllLines(@"..\..\input7.txt");
			var re = new Regex(@"^(?<parent>\w+) \((?<weight>\d+)\)(?: -> ((?<child>\w+), )*(?<child>\w+))?$", RegexOptions.Compiled);
			var allChildren = new HashSet<string>();
			var all = new HashSet<string>();
			foreach (var line in lines)
			{
				var match = re.Match(line);
				if (!match.Success)
					throw new InvalidOperationException(line);
				var parent = match.Groups["parent"].Value;
				var weight = int.Parse(match.Groups["weight"].Value);
				var children = match.Groups["child"].Captures.Cast<Capture>().Select(x => x.Value).ToArray();
				all.Add(parent);
				allChildren.UnionWith(children);
			}
			all.ExceptWith(allChildren);
			Console.Out.WriteLine(all.Single());
		}

		static void Main6_2()
		{
			var input = "4	1	15	12	0	9	9	5	5	8	7	3	14	5	12	3";
			var banks = input.Split('\t').Select(uint.Parse).ToArray();
			var memory = GetMemoryStatus(banks);
			var used = new Dictionary<ulong, int>();
			var result = 0;
			int found;
			while (!used.TryGetValue(memory.Item1, out found))
			{
				used.Add(memory.Item1, result);
				var start = memory.Item2;
				var val = banks[start];
				banks[start] = 0;
				for (int i = 1; i <= val; i++)
				{
					banks[(start + i) % banks.Length]++;
				}

				result++;
				memory = GetMemoryStatus(banks);
			}
			Console.Out.WriteLine(result - found);

			(ulong, int) GetMemoryStatus(uint[] bs)
			{
				var res = 0UL;
				var max = 0UL;
				var maxi = -1;
				for (int i = 0; i < bs.Length; i++)
				{
					if (bs[i] > max)
					{
						maxi = i;
						max = bs[i];
					}
					if (bs[i] >= 16)
						throw new InvalidOperationException();
					res <<= 4;
					res |= bs[i];
				}
				return (res, maxi);
			}
		}

		static void Main6()
		{
			var input = "4	1	15	12	0	9	9	5	5	8	7	3	14	5	12	3";
			var banks = input.Split('\t').Select(uint.Parse).ToArray();
			var memory = GetMemoryStatus(banks);
			var used = new HashSet<ulong>();
			var result = 0;
			while (used.Add(memory.Item1))
			{
				var start = memory.Item2;
				var val = banks[start];
				banks[start] = 0;
				for (int i = 1; i <= val; i++)
				{
					banks[(start + i) % banks.Length]++;
				}

				result++;
				memory = GetMemoryStatus(banks);
			}
			Console.Out.WriteLine(result);

			(ulong, int) GetMemoryStatus(uint[] bs)
			{
				var res = 0UL;
				var max = 0UL;
				var maxi = -1;
				for (int i = 0; i < bs.Length; i++)
				{
					if (bs[i] > max)
					{
						maxi = i;
						max = bs[i];
					}
					if (bs[i] >= 16)
						throw new InvalidOperationException();
					res <<= 4;
					res |= bs[i];
				}
				return (res, maxi);
			}
		}

		static void Main5_2()
		{
			var input = File.ReadAllLines(@"..\..\input5.txt").Select(int.Parse).ToArray();
			var p = 0;
			var c = 0;
			while (p >= 0 && p < input.Length)
			{
				var offset = input[p];
				c++;
				if (offset >= 3)
					input[p]--;
				else
					input[p]++;
				p += offset;
			}
			Console.Out.WriteLine(c);
		}

		static void Main5()
		{
			var input = File.ReadAllLines(@"..\..\input5.txt").Select(int.Parse).ToArray();
			var p = 0;
			var c = 0;
			while (p >= 0 && p < input.Length)
			{
				c++;
				p += input[p]++;
			}
			Console.Out.WriteLine(c);
		}

		static void Main4_2()
		{
			bool IsValid(string l)
			{
				var used = new HashSet<string>();
				var words = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return words.All(w => used.Add(new string(w.OrderBy(x => x).ToArray())));
			}

			var lines = File.ReadAllLines(@"..\..\input4.txt");
			var validCount = 0;
			foreach (var line in lines)
			{
				if (IsValid(line))
					validCount++;
			}
			Console.Out.WriteLine(validCount);
		}

		static void Main4()
		{
			bool IsValid(string l)
			{
				var used = new HashSet<string>();
				var words = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				return words.All(w => used.Add(w));
			}

			var lines = File.ReadAllLines(@"..\..\input4.txt");
			var validCount = 0;
			foreach (var line in lines)
			{
				if (IsValid(line))
					validCount++;
			}
			Console.Out.WriteLine(validCount);
		}

		static void Main3_2()
		{
			var input = 361527;
			var table = new long[1000, 1000];

			long Get(int gx, int gy) => table[gx + 500, gy + 500];
			void Set(int gx, int gy, long gv) => table[gx + 500, gy + 500] = gv;

			Set(0, 0, 1);
			var i = 0;
			var x = 0;
			var y = 0;
			var n = 0;
			var side = 0;
			var si = 0;
			var sii = 0;
			var total = 1;
			var dx = new[] { 0, -1, 0, 1 };
			var dy = new[] { -1, 0, 1, 0 };
			long res = 0;
			while (res <= input)
			{
				++i;
				if (i == total)
				{
					n++;
					side = 2 * n;
					total += side * 4;
					si = 0;
					sii = 0;
					x++;
				}
				else
				{
					sii++;
					if (sii == side)
					{
						sii = 0;
						si++;
					}
					x += dx[si];
					y += dy[si];
				}
				res = 0;
				for (int dxx = -1; dxx <= 1; dxx++)
				{
					for (int dyy = -1; dyy <= 1; dyy++)
						res += Get(x + dxx, y + dyy);
				}
				Set(x, y, res);
			}

			Console.Out.WriteLine(res);
		}

		static void Main3()
		{
			var input = 361527;
			var n = (int)Math.Floor((Math.Sqrt(input - 1) - 1) / 2.0);
			var side = 2 * (n + 1);
			var pos = (input - (4 * n * n + 4 * n + 1) - 1) % side;
			var y = n + 1;
			var x = pos - n;
			Console.Out.WriteLine(Math.Abs(x) + Math.Abs(y));
		}

		static void Main2_2()
		{
			var input = @"493	458	321	120	49	432	433	92	54	452	41	461	388	409	263	58
961	98	518	188	958	114	1044	881	948	590	972	398	115	116	451	492
76	783	709	489	617	72	824	452	748	737	691	90	94	77	84	756
204	217	90	335	220	127	302	205	242	202	259	110	118	111	200	112
249	679	4015	106	3358	1642	228	4559	307	193	4407	3984	3546	2635	3858	924
1151	1060	2002	168	3635	3515	3158	141	4009	3725	996	142	3672	153	134	1438
95	600	1171	1896	174	1852	1616	928	79	1308	2016	88	80	1559	1183	107
187	567	432	553	69	38	131	166	93	132	498	153	441	451	172	575
216	599	480	208	224	240	349	593	516	450	385	188	482	461	635	220
788	1263	1119	1391	1464	179	1200	621	1304	55	700	1275	226	57	43	51
1571	58	1331	1253	60	1496	1261	1298	1500	1303	201	73	1023	582	69	339
80	438	467	512	381	74	259	73	88	448	386	509	346	61	447	435
215	679	117	645	137	426	195	619	268	223	792	200	720	260	303	603
631	481	185	135	665	641	492	408	164	132	478	188	444	378	633	516
1165	1119	194	280	223	1181	267	898	1108	124	618	1135	817	997	129	227
404	1757	358	2293	2626	87	613	95	1658	147	75	930	2394	2349	86	385";
			var rows = input.Split('\n');
			var res = 0;
			foreach (var row in rows)
			{
				var items = row.Split('\t').Select(int.Parse).ToList();
				foreach (var item in items)
				{
					var r = items.SingleOrDefault(it => it < item && item % it == 0);
					if (r != 0)
					{
						res += item / r;
						break;
					}
				}
			}
			Console.Out.WriteLine(res);
		}

		static void Main2()
		{
			var input = @"493	458	321	120	49	432	433	92	54	452	41	461	388	409	263	58
961	98	518	188	958	114	1044	881	948	590	972	398	115	116	451	492
76	783	709	489	617	72	824	452	748	737	691	90	94	77	84	756
204	217	90	335	220	127	302	205	242	202	259	110	118	111	200	112
249	679	4015	106	3358	1642	228	4559	307	193	4407	3984	3546	2635	3858	924
1151	1060	2002	168	3635	3515	3158	141	4009	3725	996	142	3672	153	134	1438
95	600	1171	1896	174	1852	1616	928	79	1308	2016	88	80	1559	1183	107
187	567	432	553	69	38	131	166	93	132	498	153	441	451	172	575
216	599	480	208	224	240	349	593	516	450	385	188	482	461	635	220
788	1263	1119	1391	1464	179	1200	621	1304	55	700	1275	226	57	43	51
1571	58	1331	1253	60	1496	1261	1298	1500	1303	201	73	1023	582	69	339
80	438	467	512	381	74	259	73	88	448	386	509	346	61	447	435
215	679	117	645	137	426	195	619	268	223	792	200	720	260	303	603
631	481	185	135	665	641	492	408	164	132	478	188	444	378	633	516
1165	1119	194	280	223	1181	267	898	1108	124	618	1135	817	997	129	227
404	1757	358	2293	2626	87	613	95	1658	147	75	930	2394	2349	86	385";
			var rows = input.Split('\n');
			var res = 0;
			foreach (var row in rows)
			{
				var items = row.Split('\t').Select(int.Parse).ToList();
				res += items.Max() - items.Min();
			}
			Console.Out.WriteLine(res);
		}

		static void Main1_2()
		{
			var input = @"9513446799636685297929646689682997114316733445451534532351778534251427172168183621874641711534917291674333857423799375512628489423332297538215855176592633692631974822259161766238385922277893623911332569448978771948316155868781496698895492971356383996932885518732997624253678694279666572149831616312497994856288871586777793459926952491318336997159553714584541897294117487641872629796825583725975692264125865827534677223541484795877371955124463989228886498682421539667224963783616245646832154384756663251487668681425754536722827563651327524674183443696227523828832466473538347472991998913211857749878157579176457395375632995576569388455888156465451723693767887681392547189273391948632726499868313747261828186732986628365773728583387184112323696592536446536231376615949825166773536471531487969852535699774113163667286537193767515119362865141925612849443983484245268194842563154567638354645735331855896155142741664246715666899824364722914296492444672653852387389477634257768229772399416521198625393426443499223611843766134883441223328256883497423324753229392393974622181429913535973327323952241674979677481518733692544535323219895684629719868384266425386835539719237716339198485163916562434854579365958111931354576991558771236977242668756782139961638347251644828724786827751748399123668854393894787851872256667336215726674348886747128237416273154988619267824361227888751562445622387695218161341884756795223464751862965655559143779425283154533252573949165492138175581615176611845489857169132936848668646319955661492488428427435269169173654812114842568381636982389224236455633316898178163297452453296667661849622174541778669494388167451186352488555379581934999276412919598411422973399319799937518713422398874326665375216437246445791623283898584648278989674418242112957668397484671119761553847275799873495363759266296477844157237423239163559391553961176475377151369399646747881452252547741718734949967752564774161341784833521492494243662658471121369649641815562327698395293573991648351369767162642763475561544795982183714447737149239846151871434656618825566387329765118727515699213962477996399781652131918996434125559698427945714572488376342126989157872118279163127742349";
			var len = input.Length;
			input += input;
			var res = 0;
			for (int i = 0; i < len; i++)
			{
				if (input[i] == input[i + len / 2])
					res += input[i] - '0';
			}
			Console.Out.WriteLine(res);
		}

		static void Main1()
		{
			var input = @"9513446799636685297929646689682997114316733445451534532351778534251427172168183621874641711534917291674333857423799375512628489423332297538215855176592633692631974822259161766238385922277893623911332569448978771948316155868781496698895492971356383996932885518732997624253678694279666572149831616312497994856288871586777793459926952491318336997159553714584541897294117487641872629796825583725975692264125865827534677223541484795877371955124463989228886498682421539667224963783616245646832154384756663251487668681425754536722827563651327524674183443696227523828832466473538347472991998913211857749878157579176457395375632995576569388455888156465451723693767887681392547189273391948632726499868313747261828186732986628365773728583387184112323696592536446536231376615949825166773536471531487969852535699774113163667286537193767515119362865141925612849443983484245268194842563154567638354645735331855896155142741664246715666899824364722914296492444672653852387389477634257768229772399416521198625393426443499223611843766134883441223328256883497423324753229392393974622181429913535973327323952241674979677481518733692544535323219895684629719868384266425386835539719237716339198485163916562434854579365958111931354576991558771236977242668756782139961638347251644828724786827751748399123668854393894787851872256667336215726674348886747128237416273154988619267824361227888751562445622387695218161341884756795223464751862965655559143779425283154533252573949165492138175581615176611845489857169132936848668646319955661492488428427435269169173654812114842568381636982389224236455633316898178163297452453296667661849622174541778669494388167451186352488555379581934999276412919598411422973399319799937518713422398874326665375216437246445791623283898584648278989674418242112957668397484671119761553847275799873495363759266296477844157237423239163559391553961176475377151369399646747881452252547741718734949967752564774161341784833521492494243662658471121369649641815562327698395293573991648351369767162642763475561544795982183714447737149239846151871434656618825566387329765118727515699213962477996399781652131918996434125559698427945714572488376342126989157872118279163127742349";
			input += input[0];
			var res = 0;
			for (int i = 0; i < input.Length - 1; i++)
			{
				if (input[i] == input[i + 1])
					res += input[i] - '0';
			}
			Console.Out.WriteLine(res);
		}
	}
}
