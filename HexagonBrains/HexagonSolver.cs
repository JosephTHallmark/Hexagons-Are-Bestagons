using HexagonBrains.RedblobHexs;

namespace HexagonBrains
{
	public class HexagonSolver
	{
		/// <summary>
		/// Pixel size of each hex (raidus) 
		/// </summary>
		public int Size { get; }
		/// <summary>
		/// Width in hexes
		/// </summary>
		public int Width { get; }
		/// <summary>
		/// Height in hexes
		/// </summary>
		public int Height { get; }
		private readonly Point mapSize;
		private readonly Point mapComposition;
		public Dictionary<Tuple<int, int>, OffsetCoord> OffsetsByTII { get; }
		public Dictionary<Tuple<int, int>, Hex> HexesByTII { get; }
		public Dictionary<Tuple<int, int>, BTHex> BTHexesByTII { get; }
		public Dictionary<string, Tuple<int, int>> ShortStringToTII { get; }
		public Dictionary<Hex, Tuple<int, int>> TTIByHexes { get; }
		public Layout ourLayout { get; private set; }

		// Battletech uses an odd-q 
		public static HexagonSolver SolverFromBattleTechMaps(int x, int y, int mapSizeX = 16, int mapSizeY = 17)
		{
			int width = x * mapSizeX;
			int height = y * mapSizeY;
			return new HexagonSolver(new Point(mapSizeX, mapSizeY), new Point(x, y), width, height);
		}
		public HexagonSolver(Point MapSize, Point mapComp, int width = 15, int height = 17, int size = 25)
		{


			mapSize = MapSize;
			mapComposition = mapComp;
			Width = width;
			Height = height;
			Size = size;
			OffsetsByTII = new();
			HexesByTII = new();
			BTHexesByTII = new();
			TTIByHexes = new();
			ShortStringToTII = new();

			for (var x = 0; x < width; x++)
				for (var y = 0; y < height; y++)
				{
					OffsetsByTII.TryAdd(new Tuple<int, int>(x, y), new OffsetCoord(x, y));
				}
			try
			{

				foreach (var key in OffsetsByTII.Keys)
				{
					Hex hex = OffsetCoord.QoffsetToCube(-1, OffsetsByTII[key]);
					HexesByTII.Add(key, hex);
					BTHex bt = new BTHex(key, mapSize, hex);
					BTHexesByTII.Add(key, bt);
					ShortStringToTII.Add(bt.ToShortString(), key);
					TTIByHexes.Add(hex, key);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine(exp.Message);
				throw;
			}
			ourLayout = new Layout(orientation: Layout.flat, size: new Point(size, size), origin: new Point(size, size * (Math.Sqrt(3) / 2)));
		}

		public int HexDistance(Hex h1, Hex h2)
		{
			return h1.Distance(h2);
		}

		public List<Hex> HexesCrossed(Hex h1, Hex h2)
		{
			return FractionalHex.HexLinedraw(h1, h2);
		}
		public List<Hex> HexesCrossedPrime(Hex h1, Hex h2, int sensitivity = 25)
		{
			return FractionalHex.HexLinedrawPrime(h1, h2, sensitivity);
		}

		public void Resize(int size)
		{
			ourLayout = new Layout(orientation: Layout.flat, size: new Point(size, size), origin: new Point(size, size * (Math.Sqrt(3) / 2)));
		}

	}
	public class BTHex
	{
		public Point Coordinates { get; }
		public Point Map { get; }
		private readonly Point MapSize;
		public Hex Hexagon { get; set; }

		public BTHex(Tuple<int, int> offsetKey, Point mapSize, Hex hex)
		{
			MapSize = mapSize;
			double x = (offsetKey.Item1 + 1) % mapSize.x;
			double y = (offsetKey.Item2 + 1) % mapSize.y;
			if (x == 0) x = MapSize.x;
			if (y == 0) y = MapSize.y;
			Coordinates = new Point(x, y);
			int x1 = (int)(offsetKey.Item1 / MapSize.x);
			int y1 = (int)(offsetKey.Item2 / MapSize.y);
			Map = new Point(x1 + 1, y1 + 1);
			Hexagon = hex;
			var ss = ToShortString();
		}
		public override string ToString()
		{
			return $"Map:({Map.x},{Map.y}) BTHex:{(int)Coordinates.x:D2}{(int)Coordinates.y:D2} Hex {Hexagon}";
		}
		public string ToShortString()
		{
			return $"({Map.x},{Map.y}){(int)Coordinates.x:D2}{(int)Coordinates.y:D2}";
		}
	}
}