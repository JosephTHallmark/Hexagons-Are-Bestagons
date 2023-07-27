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
		public Dictionary<Tuple<int, int>, OffsetCoord> HexagonsAsOffset { get; }
		public Dictionary<Tuple<int, int>, Hex> HexagonsAsHexs { get; }
		public Dictionary<Tuple<int, int>, BTHex> HexagonsAsBTHexs { get; }
		public Dictionary<Hex, Tuple<int, int>> HexagonsAsHexsPrime { get; }
		public Layout ourLayout { get; private set; }

		// Battletech uses an odd-q 
		public static HexagonSolver SolverFromBattletechMaps(int x, int y, int mapSizeX = 15, int mapSizeY = 17)
		{
			int width = (x > 1) ? (mapSizeX + 1) * x : mapSizeX;
			int height = y * mapSizeY;
			return new HexagonSolver(new Point((x > 1) ? mapSizeX + 1 : mapSizeX, mapSizeY), new Point(x, y), width, height);
		}
		public HexagonSolver(Point MapSize, Point mapComp, int width = 15, int height = 17, int size = 25)
		{
			mapSize = MapSize;
			mapComposition = mapComp;
			Width = width;
			Height = height;
			Size = size;
			HexagonsAsOffset = new Dictionary<Tuple<int, int>, OffsetCoord>();
			HexagonsAsHexs = new Dictionary<Tuple<int, int>, Hex>();
			HexagonsAsBTHexs = new Dictionary<Tuple<int, int>, BTHex>();
			HexagonsAsHexsPrime = new Dictionary<Hex, Tuple<int, int>>();

			for (var x = 0; x < width; x++)
				for (var y = 0; y < height; y++)
					HexagonsAsOffset.TryAdd(new Tuple<int, int>(x, y), new OffsetCoord(x, y));

			foreach (var key in HexagonsAsOffset.Keys)
			{
				Hex hex = OffsetCoord.QoffsetToCube(-1, HexagonsAsOffset[key]);
				HexagonsAsHexs.Add(key, hex);
				HexagonsAsBTHexs.Add(key, new BTHex(key, mapSize, hex));
				HexagonsAsHexsPrime.Add(hex, key);
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
			// Shift because physical maps start 0101 
			int x = offsetKey.Item1;
			int y = offsetKey.Item2;
			x++; 
			y++;
			// Find the maps 
			Map = new Point((int)(x / mapSize.x) + 1, (int)(y / mapSize.y) + 1);
			// Find the map coordinate 
			// If X is on the mapsize border its on a blank hex which we will say belongs to the map on which that would col 18
			Coordinates = new Point(x % (mapSize.x + 1) , y % (mapSize.y + 1));
			Hexagon = hex;
		}
		public override string ToString()
		{
			return $"Map:({Map.x},{Map.y}) Hex:{{{((Coordinates.x == MapSize.x)? "Blank " + Coordinates.x.ToString() : Coordinates.x)},{Coordinates.y}}}";
		}
	}

}