﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc
{
	internal class Program
	{
		private static void Main()
		{
			Main25();
		}

		private static void Main25()
		{
			var diag = 12134527;
			var states = new[]
			{
				// A
				new []
				{
					new {write = 1, move = 1, state = 'B'}, // if 0
					new {write = 0, move = -1, state = 'C'}  // if 1
				},
				// B
				new []
				{
					new {write = 1, move = -1, state = 'A'}, // if 0
					new {write = 1, move = 1, state = 'C'}  // if 1
				},
				// C
				new []
				{
					new {write = 1, move = 1, state = 'A'}, // if 0
					new {write = 0, move = -1, state = 'D'}  // if 1
				},
				// D
				new []
				{
					new {write = 1, move = -1, state = 'E'}, // if 0
					new {write = 1, move = -1, state = 'C'}  // if 1
				},
				// E
				new []
				{
					new {write = 1, move = 1, state = 'F'}, // if 0
					new {write = 1, move = 1, state = 'A'}  // if 1
				},
				// F
				new []
				{
					new {write = 1, move = 1, state = 'A'}, // if 0
					new {write = 1, move = 1, state = 'E'}  // if 1
				},
			};
			var s = 'A';
			var c = 0;
			var values = new HashSet<int>();
			for (int i = 0; i < diag; i++)
			{
				var v = values.Contains(c) ? 1 : 0;
				var state = states[s - 'A'][v];
				if (state.write == 0)
					values.Remove(c);
				else
					values.Add(c);
				c += state.move;
				s = state.state;
			}

			Console.Out.WriteLine(values.Count);
		}

		private static void Main24_2()
		{
			var input = File.ReadLines(@"..\..\input24.txt");
			var ports = input
				.Select((line, i) => new
				{
					n = i,
					ends = line
						.Split('/')
						.Select(int.Parse)
						.OrderBy(x => x)
						.ToArray()
				})
				.ToList();
			var portsDct = ports
				.SelectMany(p => p.ends.Distinct().Select(e => new { port = p, end = e }))
				.GroupBy(x => x.end)
				.ToDictionary(x => x.Key, x => x.Select(t => t.port).ToArray());

			HashSet<int> used = new HashSet<int>();
			var max = 0;
			var maxL = 0;
			Iterate(0, 0);
			Console.Out.WriteLine(max);

			void Iterate(int end, int strength)
			{
				if (used.Count > maxL || used.Count == maxL && strength > max)
				{
					max = strength;
					maxL = used.Count;
				}
				if (portsDct.TryGetValue(end, out var candidates))
				{
					foreach (var next in candidates.Where(p => !used.Contains(p.n)))
					{
						used.Add(next.n);
						Iterate(
							next.ends[0] == end ? next.ends[1] : next.ends[0],
							strength + next.ends[0] + next.ends[1]);
						used.Remove(next.n);
					}
				}
			}
		}

		private static void Main24()
		{
			var input = File.ReadLines(@"..\..\input24.txt");
			var ports = input
				.Select((line, i) => new
				{
					n = i,
					ends = line
						.Split('/')
						.Select(int.Parse)
						.OrderBy(x => x)
						.ToArray()
				})
				.ToList();
			var portsDct = ports
				.SelectMany(p => p.ends.Distinct().Select(e => new {port = p, end = e}))
				.GroupBy(x => x.end)
				.ToDictionary(x => x.Key, x => x.Select(t => t.port).ToArray());

			HashSet<int> used = new HashSet<int>();
			var max = 0;
			Iterate(0, 0);
			Console.Out.WriteLine(max);
			
			void Iterate(int end, int strength)
			{
				if (strength > max)
					max = strength;
				if (portsDct.TryGetValue(end, out var candidates))
				{
					foreach (var next in candidates.Where(p => !used.Contains(p.n)))
					{
						used.Add(next.n);
						Iterate(
							next.ends[0] == end ? next.ends[1] : next.ends[0],
							strength + next.ends[0] + next.ends[1]);
						used.Remove(next.n);
					}
				}
			}
		}

		private static void Main23_2()
		{
			long Run()
			{
				long b = 93 * 100 + 100_000;
				long result = 0;
				for (int i = 0; i <= 1000; i++, b += 17)
				{
					bool f = false;
					for (int d = 2; d < b / 2; d++)
					{
						if (b % d == 0)
						{
							f = true;
							break;
						}
					}
					if (f)
						result++;
				}
				return result;
			}

			Console.Out.WriteLine(Run());
		}

		private static void Main23()
		{
			var input = File.ReadLines(@"..\..\input23.txt");
			var commands = input
				.Select(s => s.Split(' '))
				.Select<string[], (string cmd, char r, long v, char argr, long argv)>(
					x => (x[0],
						!int.TryParse(x[1], out _) ? x[1][0] : '\0',
						int.TryParse(x[1], out var v) ? v : 0,
						x.Length > 2 && !int.TryParse(x[2], out _) ? x[2][0] : '\0',
						x.Length > 2 && int.TryParse(x[2], out var num) ? num : 0)).ToArray();
			var regs = new ConcurrentDictionary<char, long>();
			long c = 0;
			long res = 0;
			while (c >= 0 && c < commands.Length)
			{
				var cmd = commands[c];
				var v = cmd.r != '\0' ? regs.GetOrAdd(cmd.r, 0) : cmd.v;
				var argv = cmd.argr != '\0' ? regs.GetOrAdd(cmd.argr, 0) : cmd.argv;
				switch (cmd.cmd)
				{
					case "set":
						regs[cmd.r] = argv;
						c++;
						break;
					case "sub":
						regs[cmd.r] = v - argv;
						c++;
						break;
					case "mul":
						regs[cmd.r] = v * argv;
						res++;
						c++;
						break;
					case "jnz":
						if (v != 0)
							c += argv;
						else
							c++;
						break;
					default:
						throw new InvalidOperationException(cmd.cmd);
				}
			}
			Console.Out.WriteLine(res);
		}

		private static void Main22_2()
		{
			var lines = File.ReadAllLines(@"..\..\input22.txt");
			var map = new Dictionary<(long x, long y), int>();
			for (var yy = 0; yy < lines.Length; yy++)
				for (var xx = 0; xx < lines[0].Length; xx++)
					if (lines[yy][xx] == '#')
						map.Add((xx, yy), 2);
			long x = lines[0].Length / 2;
			long y = lines.Length / 2;
			var dir = 0;
			var dirs = new(long dx, long dy)[]
			{
				(0, -1),
				(1, 0),
				(0, 1),
				(-1, 0)
			};
			var result = 0;
			for (var i = 0; i < 10_000_000; i++)
			{
				int value;
				map.TryGetValue((x, y), out value);
				switch (value)
				{
					case 0:
						dir = (dir + 3) % dirs.Length;
						break;
					case 1:
						break;
					case 2:
						dir = (dir + 1) % dirs.Length;
						break;
					case 3:
						dir = (dir + 2) % dirs.Length;
						break;
				}
				value = (value + 1) % 4;
				if (value == 0)
					map.Remove((x, y));
				else
					map[(x, y)] = value;
				if (value == 2)
					result++;
				x += dirs[dir].dx;
				y += dirs[dir].dy;
			}
			Console.Out.WriteLine(result);
		}

		private static void Main22()
		{
			var lines = File.ReadAllLines(@"..\..\input22.txt");
			var map = new HashSet<(long x, long y)>();
			for (var yy = 0; yy < lines.Length; yy++)
				for (var xx = 0; xx < lines[0].Length; xx++)
					if (lines[yy][xx] == '#')
						map.Add((xx, yy));
			long x = lines[0].Length / 2;
			long y = lines.Length / 2;
			var dir = 0;
			var dirs = new(long dx, long dy)[]
			{
				(0, -1),
				(1, 0),
				(0, 1),
				(-1, 0)
			};
			var result = 0;
			for (var i = 0; i < 10000; i++)
			{
				if (map.Contains((x, y)))
				{
					dir = (dir + 1) % dirs.Length;
					map.Remove((x, y));
				}
				else
				{
					dir = (dir + 3) % dirs.Length;
					map.Add((x, y));
					result++;
				}
				x += dirs[dir].dx;
				y += dirs[dir].dy;
			}
			Console.Out.WriteLine(result);
		}

		private static void Main21_2()
		{
			var lines = File.ReadAllLines(@"..\..\input21.txt");
			var patterns = new SortedDictionary<bool[,], bool[,]>(Comparer<bool[,]>.Create(Compare));
			foreach (var line in lines)
			{
				var split = line.Split(new[] { " => " }, StringSplitOptions.None);
				var source = Normalize(Parse(split[0]));
				var target = Parse(split[1]);
				patterns.Add(source, target);
			}
			var current = Parse(".#./..#/###");
			for (var i = 0; i < 18; i++)
				current = Join(Split(current)
					.Select(s => patterns[s]).ToList());
			var r = 0;
			for (var i = 0; i < current.GetLength(0); i++)
				for (var j = 0; j < current.GetLength(1); j++)
					if (current[i, j])
						r++;
			Console.Out.WriteLine(r);

			void Print(bool[,] rows)
			{
				for (var i = 0; i < rows.GetLength(0); i++)
				{
					for (var j = 0; j < rows.GetLength(0); j++)
						Console.Out.Write(rows[i, j] ? '#' : '.');
					Console.Out.WriteLine();
				}
			}

			bool[,] Join(List<bool[,]> rowses)
			{
				var size = (int)Math.Round(Math.Sqrt(rowses.Count));
				var result = new bool[size * rowses[0].GetLength(0), size * rowses[0].GetLength(0)];
				for (var i = 0; i < size; i++)
					for (var j = 0; j < size; j++)
						for (var ii = 0; ii < rowses[0].GetLength(0); ii++)
							for (var jj = 0; jj < rowses[0].GetLength(0); jj++)
								result[i * rowses[0].GetLength(0) + ii,
									j * rowses[0].GetLength(0) + jj] = rowses[i * size + j][ii, jj];
				return result;
			}

			List<bool[,]> Split(bool[,] rows)
			{
				var result = new List<bool[,]>();
				if (rows.GetLength(0) % 2 == 0)
					for (var i = 0; i < rows.GetLength(0); i += 2)
						for (var j = 0; j < rows.GetLength(0); j += 2)
							result.Add(Normalize(new[,]
							{
							{rows[i, j], rows[i, j + 1]},
							{rows[i + 1, j], rows[i + 1, j + 1]}
						}));
				else
					for (var i = 0; i < rows.GetLength(0); i += 3)
						for (var j = 0; j < rows.GetLength(0); j += 3)
							result.Add(Normalize(new[,]
							{
							{rows[i, j], rows[i, j + 1], rows[i, j + 2]},
							{rows[i + 1, j], rows[i + 1, j + 1], rows[i + 1, j + 2]},
							{rows[i + 2, j], rows[i + 2, j + 1], rows[i + 2, j + 2]}
						}));
				return result;
			}

			bool[,] Normalize(bool[,] rows)
			{
				var res = rows;
				bool[,] min = null;
				for (var i = 0; i < 2; i++)
				{
					res = Flip(res);
					for (var j = 0; j < 4; j++)
					{
						res = Rotate(res);
						if (min == null || Compare(res, min) < 0)
							min = res;
					}
				}
				return min;
			}

			int Compare(bool[,] a, bool[,] b)
			{
				if (a.Length < b.Length)
					return -1;
				if (a.Length > b.Length)
					return 1;
				var comparer = Comparer<bool>.Default;
				for (var i = 0; i < a.GetLength(0); i++)
					for (var j = 0; j < a.GetLength(0); j++)
					{
						var compare = comparer.Compare(a[i, j], b[i, j]);
						if (compare != 0)
							return compare;
					}
				return 0;
			}

			bool[,] Rotate(bool[,] rows)
			{
				var res = new bool[rows.GetLength(0), rows.GetLength(0)];
				for (var i = 0; i < rows.GetLength(0); i++)
					for (var j = 0; j < rows.GetLength(0); j++)
						res[i, j] = rows[j, rows.GetLength(0) - i - 1];
				return res;
			}

			bool[,] Flip(bool[,] rows)
			{
				var res = new bool[rows.GetLength(0), rows.GetLength(0)];
				for (var i = 0; i < rows.GetLength(0); i++)
					for (var j = 0; j < rows.GetLength(0); j++)
						res[i, j] = rows[i, rows.GetLength(0) - j - 1];
				return res;
			}

			bool[,] Parse(string source)
			{
				var rows = source.Split('/');
				var res = new bool[rows.Length, rows.Length];
				for (var i = 0; i < rows.Length; i++)
					for (var j = 0; j < rows.Length; j++)
						res[i, j] = rows[i][j] == '#';
				return res;
			}
		}

		private static void Main21()
		{
			var lines = File.ReadAllLines(@"..\..\input21.txt");
			var patterns = new SortedDictionary<bool[,], bool[,]>(Comparer<bool[,]>.Create(Compare));
			foreach (var line in lines)
			{
				var split = line.Split(new[] { " => " }, StringSplitOptions.None);
				var source = Normalize(Parse(split[0]));
				var target = Parse(split[1]);
				patterns.Add(source, target);
			}
			var current = Parse(".#./..#/###");
			Print(current);
			for (var i = 0; i < 5; i++)
			{
				current = Join(Split(current)
					.Select(s => patterns[s]).ToList());
				Console.WriteLine();
				Print(current);
			}
			var r = 0;
			for (var i = 0; i < current.GetLength(0); i++)
				for (var j = 0; j < current.GetLength(1); j++)
					if (current[i, j])
						r++;
			Console.Out.WriteLine(r);

			void Print(bool[,] rows)
			{
				for (var i = 0; i < rows.GetLength(0); i++)
				{
					for (var j = 0; j < rows.GetLength(0); j++)
						Console.Out.Write(rows[i, j] ? '#' : '.');
					Console.Out.WriteLine();
				}
			}

			bool[,] Join(List<bool[,]> rowses)
			{
				var size = (int)Math.Round(Math.Sqrt(rowses.Count));
				var result = new bool[size * rowses[0].GetLength(0), size * rowses[0].GetLength(0)];
				for (var i = 0; i < size; i++)
					for (var j = 0; j < size; j++)
						for (var ii = 0; ii < rowses[0].GetLength(0); ii++)
							for (var jj = 0; jj < rowses[0].GetLength(0); jj++)
								result[i * rowses[0].GetLength(0) + ii,
									j * rowses[0].GetLength(0) + jj] = rowses[i * size + j][ii, jj];
				return result;
			}

			List<bool[,]> Split(bool[,] rows)
			{
				var result = new List<bool[,]>();
				if (rows.GetLength(0) % 2 == 0)
					for (var i = 0; i < rows.GetLength(0); i += 2)
						for (var j = 0; j < rows.GetLength(0); j += 2)
							result.Add(Normalize(new[,]
							{
							{rows[i, j], rows[i, j + 1]},
							{rows[i + 1, j], rows[i + 1, j + 1]}
						}));
				else
					for (var i = 0; i < rows.GetLength(0); i += 3)
						for (var j = 0; j < rows.GetLength(0); j += 3)
							result.Add(Normalize(new[,]
							{
							{rows[i, j], rows[i, j + 1], rows[i, j + 2]},
							{rows[i + 1, j], rows[i + 1, j + 1], rows[i + 1, j + 2]},
							{rows[i + 2, j], rows[i + 2, j + 1], rows[i + 2, j + 2]}
						}));
				return result;
			}

			bool[,] Normalize(bool[,] rows)
			{
				var res = rows;
				bool[,] min = null;
				for (var i = 0; i < 2; i++)
				{
					res = Flip(res);
					for (var j = 0; j < 4; j++)
					{
						res = Rotate(res);
						if (min == null || Compare(res, min) < 0)
							min = res;
					}
				}
				return min;
			}

			int Compare(bool[,] a, bool[,] b)
			{
				if (a.Length < b.Length)
					return -1;
				if (a.Length > b.Length)
					return 1;
				var comparer = Comparer<bool>.Default;
				for (var i = 0; i < a.GetLength(0); i++)
					for (var j = 0; j < a.GetLength(0); j++)
					{
						var compare = comparer.Compare(a[i, j], b[i, j]);
						if (compare != 0)
							return compare;
					}
				return 0;
			}

			bool[,] Rotate(bool[,] rows)
			{
				var res = new bool[rows.GetLength(0), rows.GetLength(0)];
				for (var i = 0; i < rows.GetLength(0); i++)
					for (var j = 0; j < rows.GetLength(0); j++)
						res[i, j] = rows[j, rows.GetLength(0) - i - 1];
				return res;
			}

			bool[,] Flip(bool[,] rows)
			{
				var res = new bool[rows.GetLength(0), rows.GetLength(0)];
				for (var i = 0; i < rows.GetLength(0); i++)
					for (var j = 0; j < rows.GetLength(0); j++)
						res[i, j] = rows[i, rows.GetLength(0) - j - 1];
				return res;
			}

			bool[,] Parse(string source)
			{
				var rows = source.Split('/');
				var res = new bool[rows.Length, rows.Length];
				for (var i = 0; i < rows.Length; i++)
					for (var j = 0; j < rows.Length; j++)
						res[i, j] = rows[i][j] == '#';
				return res;
			}
		}

		private static void Main20_2()
		{
			var lines = File.ReadAllLines(@"..\..\input20.txt");
			var particles = new List<(int n, int x, int y, int z, int vx, int vy, int vz, int ax, int ay, int az)>();
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var split = line.Split(new[] { "p=<", "v=<", "a=<", ">", ",", " " }, StringSplitOptions.RemoveEmptyEntries)
					.Select(int.Parse).ToArray();
				particles.Add((i, split[0], split[1], split[2], split[3], split[4], split[5], split[6], split[7], split[8]));
			}
			var intersections = new Dictionary<(int n1, int n2), long[]>();
			for (var i = 0; i < particles.Count - 1; i++)
			{
				var p0 = particles[i];
				for (var j = i + 1; j < particles.Count; j++)
				{
					var p1 = particles[j];
					var xtimes = GetIntersectionTimes(p0.x, p1.x, p0.vx, p1.vx, p0.ax, p1.ax);
					var ytimes = GetIntersectionTimes(p0.y, p1.y, p0.vy, p1.vy, p0.ay, p1.ay);
					var ztimes = GetIntersectionTimes(p0.z, p1.z, p0.vz, p1.vz, p0.az, p1.az);
					xtimes = xtimes ?? ytimes ?? ztimes;
					ytimes = ytimes ?? xtimes ?? ztimes;
					ztimes = ztimes ?? xtimes ?? ytimes;
					if (xtimes == null)
					{
						intersections.Add((i, j), new[] { 0L });
						intersections.Add((j, i), new[] { 0L });
					}
					else
					{
						var times = xtimes.Intersect(ytimes).Intersect(ztimes).ToArray();
						if (times.Any())
						{
							intersections.Add((i, j), times);
							intersections.Add((j, i), times);
						}
					}
				}
			}

			var removed = new Dictionary<int, long>();
			foreach (var g in intersections.SelectMany(x => x.Value.Select(v => new { pair = x.Key, time = v }))
				.GroupBy(x => x.time).OrderBy(g => g.Key))
			{
				var time = g.Key;
				foreach (var kvp in g)
				{
					var (n1, n2) = kvp.pair;
					if ((!removed.TryGetValue(n1, out var time1) || time1 == time)
						&& (!removed.TryGetValue(n2, out var time2) || time2 == time))
					{
						removed[n1] = time;
						removed[n2] = time;
					}
				}
			}
			Console.Out.WriteLine(particles.Count - removed.Count);

			List<long> GetIntersectionTimes(int x0, int x1, int v0, int v1, int a0, int a1)
			{
				var a = a0 - a1;
				var b = 2 * (v0 - v1) + a;
				var c = 2 * (x0 - x1);
				if (a == 0)
				{
					if (c == 0)
						return null;
					if (b == 0)
						return new List<long>();
					if (c % b == 0 && -c / b >= 0)
						return new List<long> { -c / b };
					return new List<long>();
				}

				var d = (long)b * b - 4L * a * c;
				if (d < 0)
					return new List<long>();
				if (d == 0)
				{
					if (-b % (2 * a) == 0 && -b / (2 * a) >= 0)
						return new List<long> { -b / (2 * a) };
					return new List<long>();
				}
				var sqt = (long)Math.Round(Math.Sqrt(d));
				if (sqt * sqt == d)
				{
					var result = new List<long>();
					if ((-b + sqt) % (2 * a) == 0 && (-b + sqt) / (2 * a) >= 0)
						result.Add((-b + sqt) / (2 * a));
					if ((-b - sqt) % (2 * a) == 0 && (-b - sqt) / (2 * a) >= 0)
						result.Add((-b - sqt) / (2 * a));
					return result;
				}
				return new List<long>();
			}
		}

		private static void Main20()
		{
			var lines = File.ReadAllLines(@"..\..\input20.txt");
			var particles = new List<(int n, int x, int y, int z, int vx, int vy, int vz, int ax, int ay, int az)>();
			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var split = line.Split(new[] { "p=<", "v=<", "a=<", ">", ",", " " }, StringSplitOptions.RemoveEmptyEntries)
					.Select(int.Parse).ToArray();
				particles.Add((i, split[0], split[1], split[2], split[3], split[4], split[5], split[6], split[7], split[8]));
			}
			var result = particles
				.OrderBy(p => Math.Abs(p.ax) + Math.Abs(p.ay) + Math.Abs(p.az))
				.ThenBy(p => Math.Abs(p.vx) + Math.Abs(p.vy) + Math.Abs(p.vz))
				.ThenBy(p => Math.Abs(p.x) + Math.Abs(p.y) + Math.Abs(p.z))
				.First();
			Console.Out.WriteLine(result.n);
		}

		private static void Main19_2()
		{
			var input = File.ReadAllLines(@"..\..\input19.txt");
			var x = input[0].IndexOf('|');
			var y = 0;
			var dir = 0;
			var d = new(int dx, int dy)[]
			{
				(0, 1),
				(-1, 0),
				(0, -1),
				(1, 0)
			};
			var result = 0;
			var done = false;
			while (!done)
			{
				result++;
				switch (input[y][x])
				{
					case ' ':
						done = true;
						break;
					case '|':
					case '-':
						x = x + d[dir].dx;
						y = y + d[dir].dy;
						break;
					case '+':
						var nd = (dir + 1) % d.Length;
						for (var i = 0; i < 2; i++)
						{
							var nx = x + d[nd].dx;
							var ny = y + d[nd].dy;
							if (input[ny][nx] != ' ')
							{
								x = nx;
								y = ny;
								dir = nd;
								break;
							}
							nd = (nd + 2) % d.Length;
						}
						break;
					default:
						if (input[y][x] >= 'A' && input[y][x] <= 'Z')
						{
							x = x + d[dir].dx;
							y = y + d[dir].dy;
							break;
						}
						else
						{
							throw new InvalidOperationException();
						}
				}
			}
			Console.Out.WriteLine(result - 1);
		}

		private static void Main19()
		{
			var input = File.ReadAllLines(@"..\..\input19.txt");
			var x = input[0].IndexOf('|');
			var y = 0;
			var dir = 0;
			var d = new(int dx, int dy)[]
			{
				(0, 1),
				(-1, 0),
				(0, -1),
				(1, 0)
			};
			var result = "";
			var done = false;
			while (!done)
				switch (input[y][x])
				{
					case ' ':
						done = true;
						break;
					case '|':
					case '-':
						x = x + d[dir].dx;
						y = y + d[dir].dy;
						break;
					case '+':
						var nd = (dir + 1) % d.Length;
						for (var i = 0; i < 2; i++)
						{
							var nx = x + d[nd].dx;
							var ny = y + d[nd].dy;
							if (input[ny][nx] != ' ')
							{
								x = nx;
								y = ny;
								dir = nd;
								break;
							}
							nd = (nd + 2) % d.Length;
						}
						break;
					default:
						if (input[y][x] >= 'A' && input[y][x] <= 'Z')
						{
							result += input[y][x];
							x = x + d[dir].dx;
							y = y + d[dir].dy;
							break;
						}
						else
						{
							throw new InvalidOperationException();
						}
				}
			Console.Out.WriteLine(result);
		}

		private static void Main18_2()
		{
			var input = File.ReadLines(@"..\..\input18.txt");
			var commands = input
				.Select(s => s.Split(' '))
				.Select<string[], (string cmd, char r, long v, char argr, long argv)>(
					x => (x[0],
						!int.TryParse(x[1], out _) ? x[1][0] : '\0',
						int.TryParse(x[1], out var v) ? v : 0,
						x.Length > 2 && !int.TryParse(x[2], out _) ? x[2][0] : '\0',
						x.Length > 2 && int.TryParse(x[2], out var num) ? num : 0)).ToArray();
			var regs = new[] { new ConcurrentDictionary<char, long>(), new ConcurrentDictionary<char, long>() };
			regs[0]['p'] = 0;
			regs[1]['p'] = 1;
			var queues = new[] { new Queue<long>(), new Queue<long>() };
			var c = new long[] { 0, 0 };
			var locked = new[] { false, false };
			var terminated = new[] { false, false };
			var deadlocked = false;
			var p = 0;
			var result = 0;
			while (!deadlocked && (!terminated[0] || !terminated[1]))
			{
				while (c[p] >= 0 && c[p] < commands.Length && !deadlocked && !terminated[p])
				{
					var cmd = commands[c[p]];
					var v = cmd.r != '\0' ? regs[p].GetOrAdd(cmd.r, 0) : cmd.v;
					var argv = cmd.argr != '\0' ? regs[p].GetOrAdd(cmd.argr, 0) : cmd.argv;
					switch (cmd.cmd)
					{
						case "snd":
							queues[1 - p].Enqueue(v);
							locked[1 - p] = false;
							c[p]++;
							if (p == 1)
								result++;
							break;
						case "set":
							regs[p][cmd.r] = argv;
							c[p]++;
							break;
						case "add":
							regs[p][cmd.r] = v + argv;
							c[p]++;
							break;
						case "mul":
							regs[p][cmd.r] = v * argv;
							c[p]++;
							break;
						case "mod":
							regs[p][cmd.r] = v % argv;
							c[p]++;
							break;
						case "rcv":
							if (locked[p])
							{
								deadlocked = true;
							}
							else if (queues[p].Count > 0)
							{
								regs[p][cmd.r] = queues[p].Dequeue();
								c[p]++;
							}
							else
							{
								locked[p] = true;
								p = 1 - p;
							}
							break;
						case "jgz":
							if (v > 0)
								c[p] += argv;
							else
								c[p]++;
							break;
						default:
							throw new InvalidOperationException(cmd.cmd);
					}
				}
				terminated[p] = true;
				p = 1 - p;
			}
			Console.Out.WriteLine(result);
		}

		private static void Main18()
		{
			var input = File.ReadLines(@"..\..\input18.txt");
			var commands = input
				.Select(s => s.Split(' '))
				.Select<string[], (string cmd, char r, long v, char argr, long argv)>(
					x => (x[0],
						!int.TryParse(x[1], out _) ? x[1][0] : '\0',
						int.TryParse(x[1], out var v) ? v : 0,
						x.Length > 2 && !int.TryParse(x[2], out _) ? x[2][0] : '\0',
						x.Length > 2 && int.TryParse(x[2], out var num) ? num : 0)).ToArray();
			var regs = new ConcurrentDictionary<char, long>();
			long snd = 0;
			long c = 0;
			while (c >= 0 && c < commands.Length)
			{
				var cmd = commands[c];
				Console.Out.WriteLine($"{c}: {cmd.cmd}");
				var v = cmd.r != '\0' ? regs.GetOrAdd(cmd.r, 0) : cmd.v;
				var argv = cmd.argr != '\0' ? regs.GetOrAdd(cmd.argr, 0) : cmd.argv;
				switch (cmd.cmd)
				{
					case "snd":
						snd = v;
						c++;
						break;
					case "set":
						regs[cmd.r] = argv;
						c++;
						break;
					case "add":
						regs[cmd.r] = v + argv;
						c++;
						break;
					case "mul":
						regs[cmd.r] = v * argv;
						c++;
						break;
					case "mod":
						regs[cmd.r] = v % argv;
						c++;
						break;
					case "rcv":
						if (v != 0)
						{
							Console.Out.WriteLine(snd);
							return;
						}
						c++;
						break;
					case "jgz":
						if (v > 0)
							c += argv;
						else
							c++;
						break;
					default:
						throw new InvalidOperationException(cmd.cmd);
				}
			}
		}

		private static void Main17_2()
		{
			var input = 345;
			var cur = 0;
			var result = 0;
			for (var count = 1; count <= 50_000_000; count++)
			{
				cur = (cur + input + 1) % count;
				if (cur == 0)
					cur = count;
				if (cur == 1)
					result = count;
			}
			Console.Out.WriteLine(result);
		}

		private static void Main17()
		{
			var input = 345;
			var buffer = new List<int> { 0 };
			var cur = 0;
			for (var i = 1; i <= 2017; i++)
			{
				cur = (cur + input + 1) % buffer.Count;
				buffer.Insert(cur, i);
			}
			cur = (cur + 1) % buffer.Count;
			Console.Out.WriteLine(buffer[cur]);
		}

		private static void Main16_2()
		{
			var input = File.ReadAllText(@"..\..\input16.txt").Trim();
			var commandS = input.Split(',');
			var commands = new(char cmd, int arg0, int arg1, char carg0, char carg1)[commandS.Length];
			for (var ci = 0; ci < commandS.Length; ci++)
			{
				var c = commandS[ci];
				switch (c[0])
				{
					case 's':
						var i = int.Parse(c.Substring(1));
						commands[ci] = (c[0], i, 0, '\0', '\0');
						break;
					case 'x':
						var s = c.Substring(1).Split('/');
						var i1 = int.Parse(s[0]);
						var i2 = int.Parse(s[1]);
						commands[ci] = (c[0], i1, i2, '\0', '\0');
						break;
					case 'p':
						s = c.Substring(1).Split('/');
						commands[ci] = (c[0], 0, 0, s[0][0], s[1][0]);
						break;
					default:
						throw new InvalidOperationException(c);
				}
			}
			var programs = "abcdefghijklmnop".ToCharArray();
			var indexes = Enumerable.Range(0, 16).ToArray();
			var start = 0;
			for (var cc = 0; cc < 1_000_000_000; cc++)
			{
				for (var ci = 0; ci < commands.Length; ci++)
				{
					var c = commands[ci];
					switch (c.cmd)
					{
						case 's':
							start = (start + 16 - c.arg0) % 16;
							break;
						case 'x':
							var i1 = (c.arg0 + start) % 16;
							var i2 = (c.arg1 + start) % 16;
							var ti = indexes[programs[i1] - 'a'];
							indexes[programs[i1] - 'a'] = indexes[programs[i2] - 'a'];
							indexes[programs[i2] - 'a'] = ti;
							var t = programs[i1];
							programs[i1] = programs[i2];
							programs[i2] = t;
							break;
						case 'p':
							i1 = c.carg0 - 'a';
							i2 = c.carg1 - 'a';
							t = programs[indexes[i1]];
							programs[indexes[i1]] = programs[indexes[i2]];
							programs[indexes[i2]] = t;
							ti = indexes[i1];
							indexes[i1] = indexes[i2];
							indexes[i2] = ti;
							break;
						default:
							throw new InvalidOperationException(c.ToString());
					}
				}
				if (new string(programs.Skip(start).Concat(programs.Take(start)).ToArray()) == "abcdefghijklmnop")
				{
					var cycle = cc + 1;
					Console.Out.WriteLine("cycle = " + cycle);
					cc += (1_000_000_000 / cycle - 1) * cycle;
				}
			}
			Console.Out.WriteLine(new string(programs.Skip(start).Concat(programs.Take(start)).ToArray()));
		}

		private static void Main16()
		{
			var input = File.ReadAllText(@"..\..\input16.txt").Trim();
			var commands = input.Split(',');
			var programs = "abcdefghijklmnop".ToCharArray();
			var start = 0;
			foreach (var c in commands)
				switch (c[0])
				{
					case 's':
						var i = int.Parse(c.Substring(1));
						start = (start + 16 - i) % 16;
						break;
					case 'x':
						var s = c.Substring(1).Split('/');
						var i1 = (int.Parse(s[0]) + start) % 16;
						var i2 = (int.Parse(s[1]) + start) % 16;
						var t = programs[i1];
						programs[i1] = programs[i2];
						programs[i2] = t;
						break;
					case 'p':
						s = c.Substring(1).Split('/');
						i1 = Array.IndexOf(programs, s[0][0]);
						i2 = Array.IndexOf(programs, s[1][0]);
						t = programs[i1];
						programs[i1] = programs[i2];
						programs[i2] = t;
						break;
					default:
						throw new InvalidOperationException(c);
				}
			Console.Out.WriteLine(new string(programs.Skip(start).Concat(programs.Take(start)).ToArray()));
		}

		private static void Main15_2()
		{
			var a = 703L;
			var b = 516L;
			var result = 0;
			for (var i = 0; i < 5_000_000; i++)
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

		private static void Main15()
		{
			var a = 703L;
			var b = 516L;
			var result = 0;
			for (var i = 0; i < 40_000_000; i++)
			{
				a = a * 16807L % 2147483647L;
				b = b * 48271L % 2147483647L;
				if ((a & 0b1111111111111111) == (b & 0b1111111111111111))
					result++;
			}
			Console.Out.WriteLine(result);
		}

		private static void Main14_2()
		{
			var input = "hxtvlmkl";
			var grid = new int[128, 128];
			for (var y = 0; y < 128; y++)
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
			for (var x = 0; x < 128; x++)
				for (var y = 0; y < 128; y++)
					if (grid[x, y] == 1)
					{
						result++;
						var queue = new Queue<(int x, int y)>();
						queue.Enqueue((x, y));
						grid[x, y] = 0;
						while (queue.Count > 0)
						{
							var cur = queue.Dequeue();
							var d = new(int dx, int dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
							foreach (var (dx, dy) in d)
								if (cur.x + dx >= 0 && cur.x + dx < 128
									&& cur.y + dy >= 0 && cur.y + dy < 128)
									if (grid[cur.x + dx, cur.y + dy] == 1)
									{
										grid[cur.x + dx, cur.y + dy] = 0;
										queue.Enqueue((cur.x + dx, cur.y + dy));
									}
						}
					}
			Console.Out.WriteLine(result);
		}

		private static void Main14()
		{
			var input = "hxtvlmkl";
			var result = 0;
			for (var i = 0; i < 128; i++)
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

		private static void Main13_2()
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
				for (var t = 0; t < forbidTimes.Length + level; t += depth * 2 - 2)
					if (t - level >= 0)
						forbidTimes[t - level] = true;
			}
			for (var i = 0; i < forbidTimes.Length; i++)
				if (!forbidTimes[i])
				{
					Console.Out.WriteLine(i);
					return;
				}
		}

		private static void Main13()
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

		private static void Main12_2()
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
						if (used.Add(link))
							queue.Enqueue(link);
				}
				totalUsed.UnionWith(used);
			}
			Console.Out.WriteLine(groupCount);
		}

		private static void Main12()
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
					if (used.Add(link))
						queue.Enqueue(link);
			}
			Console.Out.WriteLine(used.Count);
		}

		private static void Main11_2()
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

		private static void Main11()
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

		private static void Main10_2()
		{
			var input = "187,254,0,81,169,219,1,190,19,102,255,56,46,32,2,216";
			var knotHash = KnotHash(input);
			var res = "";
			foreach (var d in knotHash)
				res += d.ToString("x2");
			Console.Out.WriteLine(res);
		}

		private static byte[] KnotHash(string input)
		{
			var lens = input.Select(c => (int)c).Concat(new[] { 17, 31, 73, 47, 23 }).ToArray();
			var skip = 0;
			var cur = 0;
			var list = Enumerable.Range(0, 256).ToArray();
			for (var k = 0; k < 64; k++)
				foreach (var len in lens)
				{
					for (var i = 0; i < len / 2; i++)
					{
						var tmp = list[(cur + i) % list.Length];
						list[(cur + i) % list.Length] = list[(cur + len - 1 - i) % list.Length];
						list[(cur + len - 1 - i) % list.Length] = tmp;
					}
					cur = (cur + len + skip) % list.Length;
					skip++;
				}
			var dense = new byte[16];
			for (var i = 0; i < 16; i++)
				for (var k = 0; k < 16; k++)
					dense[i] ^= (byte)list[i * 16 + k];
			return dense;
		}

		private static void Main10()
		{
			var input = "187,254,0,81,169,219,1,190,19,102,255,56,46,32,2,216";
			var lens = input.Split(',').Select(int.Parse).ToArray();
			var skip = 0;
			var cur = 0;
			var list = Enumerable.Range(0, 256).ToArray();
			foreach (var len in lens)
			{
				for (var i = 0; i < len / 2; i++)
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

		private static void Main9_2()
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

		private static void Main9()
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

		private static void Main8_2()
		{
			var lines = File.ReadAllLines(@"..\..\input8.txt");
			var re = new Regex(@"^(?<r>\w+) (?<op>inc|dec) (?<v>-?\d+) if (?<cr>\w+) (?<cop><|>|<=|>=|==|!=) (?<cv>-?\d+)$",
				RegexOptions.Compiled);
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

		private static void Main8()
		{
			var lines = File.ReadAllLines(@"..\..\input8.txt");
			var re = new Regex(@"^(?<r>\w+) (?<op>inc|dec) (?<v>-?\d+) if (?<cr>\w+) (?<cop><|>|<=|>=|==|!=) (?<cv>-?\d+)$",
				RegexOptions.Compiled);
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

		private static void Main7_2()
		{
			var lines = File.ReadAllLines(@"..\..\input7.txt");
			var re = new Regex(@"^(?<parent>\w+) \((?<weight>\d+)\)(?: -> ((?<child>\w+), )*(?<child>\w+))?$",
				RegexOptions.Compiled);
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
					if (totalWeights[c] == wrongWeight)
						return (c, validWeight);
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

		private static void Main7()
		{
			var lines = File.ReadAllLines(@"..\..\input7.txt");
			var re = new Regex(@"^(?<parent>\w+) \((?<weight>\d+)\)(?: -> ((?<child>\w+), )*(?<child>\w+))?$",
				RegexOptions.Compiled);
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

		private static void Main6_2()
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
				for (var i = 1; i <= val; i++)
					banks[(start + i) % banks.Length]++;

				result++;
				memory = GetMemoryStatus(banks);
			}
			Console.Out.WriteLine(result - found);

			(ulong, int) GetMemoryStatus(uint[] bs)
			{
				var res = 0UL;
				var max = 0UL;
				var maxi = -1;
				for (var i = 0; i < bs.Length; i++)
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

		private static void Main6()
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
				for (var i = 1; i <= val; i++)
					banks[(start + i) % banks.Length]++;

				result++;
				memory = GetMemoryStatus(banks);
			}
			Console.Out.WriteLine(result);

			(ulong, int) GetMemoryStatus(uint[] bs)
			{
				var res = 0UL;
				var max = 0UL;
				var maxi = -1;
				for (var i = 0; i < bs.Length; i++)
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

		private static void Main5_2()
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

		private static void Main5()
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

		private static void Main4_2()
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
				if (IsValid(line))
					validCount++;
			Console.Out.WriteLine(validCount);
		}

		private static void Main4()
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
				if (IsValid(line))
					validCount++;
			Console.Out.WriteLine(validCount);
		}

		private static void Main3_2()
		{
			var input = 361527;
			var table = new long[1000, 1000];

			long Get(int gx, int gy)
			{
				return table[gx + 500, gy + 500];
			}

			void Set(int gx, int gy, long gv)
			{
				table[gx + 500, gy + 500] = gv;
			}

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
				for (var dxx = -1; dxx <= 1; dxx++)
					for (var dyy = -1; dyy <= 1; dyy++)
						res += Get(x + dxx, y + dyy);
				Set(x, y, res);
			}

			Console.Out.WriteLine(res);
		}

		private static void Main3()
		{
			var input = 361527;
			var n = (int)Math.Floor((Math.Sqrt(input - 1) - 1) / 2.0);
			var side = 2 * (n + 1);
			var pos = (input - (4 * n * n + 4 * n + 1) - 1) % side;
			var y = n + 1;
			var x = pos - n;
			Console.Out.WriteLine(Math.Abs(x) + Math.Abs(y));
		}

		private static void Main2_2()
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

		private static void Main2()
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

		private static void Main1_2()
		{
			var input =
				@"9513446799636685297929646689682997114316733445451534532351778534251427172168183621874641711534917291674333857423799375512628489423332297538215855176592633692631974822259161766238385922277893623911332569448978771948316155868781496698895492971356383996932885518732997624253678694279666572149831616312497994856288871586777793459926952491318336997159553714584541897294117487641872629796825583725975692264125865827534677223541484795877371955124463989228886498682421539667224963783616245646832154384756663251487668681425754536722827563651327524674183443696227523828832466473538347472991998913211857749878157579176457395375632995576569388455888156465451723693767887681392547189273391948632726499868313747261828186732986628365773728583387184112323696592536446536231376615949825166773536471531487969852535699774113163667286537193767515119362865141925612849443983484245268194842563154567638354645735331855896155142741664246715666899824364722914296492444672653852387389477634257768229772399416521198625393426443499223611843766134883441223328256883497423324753229392393974622181429913535973327323952241674979677481518733692544535323219895684629719868384266425386835539719237716339198485163916562434854579365958111931354576991558771236977242668756782139961638347251644828724786827751748399123668854393894787851872256667336215726674348886747128237416273154988619267824361227888751562445622387695218161341884756795223464751862965655559143779425283154533252573949165492138175581615176611845489857169132936848668646319955661492488428427435269169173654812114842568381636982389224236455633316898178163297452453296667661849622174541778669494388167451186352488555379581934999276412919598411422973399319799937518713422398874326665375216437246445791623283898584648278989674418242112957668397484671119761553847275799873495363759266296477844157237423239163559391553961176475377151369399646747881452252547741718734949967752564774161341784833521492494243662658471121369649641815562327698395293573991648351369767162642763475561544795982183714447737149239846151871434656618825566387329765118727515699213962477996399781652131918996434125559698427945714572488376342126989157872118279163127742349";
			var len = input.Length;
			input += input;
			var res = 0;
			for (var i = 0; i < len; i++)
				if (input[i] == input[i + len / 2])
					res += input[i] - '0';
			Console.Out.WriteLine(res);
		}

		private static void Main1()
		{
			var input =
				@"9513446799636685297929646689682997114316733445451534532351778534251427172168183621874641711534917291674333857423799375512628489423332297538215855176592633692631974822259161766238385922277893623911332569448978771948316155868781496698895492971356383996932885518732997624253678694279666572149831616312497994856288871586777793459926952491318336997159553714584541897294117487641872629796825583725975692264125865827534677223541484795877371955124463989228886498682421539667224963783616245646832154384756663251487668681425754536722827563651327524674183443696227523828832466473538347472991998913211857749878157579176457395375632995576569388455888156465451723693767887681392547189273391948632726499868313747261828186732986628365773728583387184112323696592536446536231376615949825166773536471531487969852535699774113163667286537193767515119362865141925612849443983484245268194842563154567638354645735331855896155142741664246715666899824364722914296492444672653852387389477634257768229772399416521198625393426443499223611843766134883441223328256883497423324753229392393974622181429913535973327323952241674979677481518733692544535323219895684629719868384266425386835539719237716339198485163916562434854579365958111931354576991558771236977242668756782139961638347251644828724786827751748399123668854393894787851872256667336215726674348886747128237416273154988619267824361227888751562445622387695218161341884756795223464751862965655559143779425283154533252573949165492138175581615176611845489857169132936848668646319955661492488428427435269169173654812114842568381636982389224236455633316898178163297452453296667661849622174541778669494388167451186352488555379581934999276412919598411422973399319799937518713422398874326665375216437246445791623283898584648278989674418242112957668397484671119761553847275799873495363759266296477844157237423239163559391553961176475377151369399646747881452252547741718734949967752564774161341784833521492494243662658471121369649641815562327698395293573991648351369767162642763475561544795982183714447737149239846151871434656618825566387329765118727515699213962477996399781652131918996434125559698427945714572488376342126989157872118279163127742349";
			input += input[0];
			var res = 0;
			for (var i = 0; i < input.Length - 1; i++)
				if (input[i] == input[i + 1])
					res += input[i] - '0';
			Console.Out.WriteLine(res);
		}
	}
}