using HexagonBrains.RedblobHexs;

namespace HexagonBrains
{
	public class HexagonSolver
	{
		public int Size { get; }
		public int Width { get; }
		public int Height { get; }
		public Dictionary<Tuple<int, int>, OffsetCoord> HexagonsAsOffset { get; }
		public Dictionary<Tuple<int, int>, Hex> HexagonsAsHexs { get; }
		public Dictionary<Hex, Tuple<int, int>> HexagonsAsHexsPrime { get; }
		public Layout ourLayout { get; private set; }

		// Battletech uses an odd-q 
		public static HexagonSolver SolverFromBattletechMaps(int x, int y)
		{
			int width = (x > 1) ? 16 * x : 15;
			int height = (y > 1) ? 18 * y : 17;
			return new HexagonSolver(width, height);
		}
		public HexagonSolver(int width = 15, int height = 17, int size = 25)
		{
			Width = width;
			Height = height;
			Size = size;
			HexagonsAsOffset = new Dictionary<Tuple<int, int>, OffsetCoord>();
			HexagonsAsHexs = new Dictionary<Tuple<int, int>, Hex>();
			HexagonsAsHexsPrime = new Dictionary<Hex, Tuple<int, int>>();

			for (var x = 0; x < width; x++)
				for (var y = 0; y < height; y++)
					HexagonsAsOffset.TryAdd(new Tuple<int, int>(x, y), new OffsetCoord(x, y));

			foreach (var key in HexagonsAsOffset.Keys)
			{
				HexagonsAsHexs.Add(key, OffsetCoord.QoffsetToCube(-1, HexagonsAsOffset[key]));
				HexagonsAsHexsPrime.Add(OffsetCoord.QoffsetToCube(-1, HexagonsAsOffset[key]), key);
			}
			ourLayout = new Layout(orientation: Layout.flat, size: new Point(size, size), origin: new Point(size, size * (Math.Sqrt(3) / 2)));

			// Uncomment if unsure if changes have compromised integrity 
			//hexTest();
		}
		public String BTHex(int x, int y)
		{
			string firstCoords = $"{x:D2}{y:D2}";
			

			string output = firstCoords;
			return output;
		}


		public int HexDistance(Hex h1, Hex h2)
		{
			return h1.Distance(h2);
		}

		public List<Hex> HexesCrossed(Hex h1, Hex h2)
		{
			return FractionalHex.HexLinedraw(h1, h2);
		}

		public void Resize(int size)
		{
			ourLayout = new Layout(orientation: Layout.flat, size: new Point(size, size), origin: new Point(size, size * (Math.Sqrt(3) / 2)));
		}

		private void hexTest()
		{
			foreach (var offsetkvp in HexagonsAsOffset)
			{
				if (HexagonsAsHexs[offsetkvp.Key] != OffsetCoord.QoffsetToCube(1, offsetkvp.Value))
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}