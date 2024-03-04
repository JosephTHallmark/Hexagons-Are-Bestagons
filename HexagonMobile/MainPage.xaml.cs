using HexagonBrains;
using HexagonBrains.RedblobHexs;

namespace HexagonMobile
{
	public partial class MainPage : ContentPage
	{
		private HexagonSolver Solver { get; set; }

		public MainPage()
		{
			InitializeComponent();

			//Solver = HexagonSolver.SolverFromBattletechMaps(Convert.ToInt32(mapSizeX.Text), Convert.ToInt32(mapSizeY.Text), 15, 17);
		}

		private void Submit(object sender, EventArgs e)
		{
			try
			{
				//Solver = HexagonSolver.SolverFromBattletechMaps(Convert.ToInt32(mapSizeX.Text), Convert.ToInt32(mapSizeY.Text), 15, 17);
				// Parse x and y from both inputs 
				var firsthex = FirstHexInput.Text.Substring(1, FirstHexInput.Text.IndexOf(')') - 1).Split(',');
				var secondhex = SecondHexInput.Text.Substring(1, SecondHexInput.Text.IndexOf(')') - 1).Split(',');
				var xm = Math.Max(Convert.ToInt32(firsthex[0]), Convert.ToInt32(secondhex[0]));
				var ym = Math.Max(Convert.ToInt32(firsthex[1]), Convert.ToInt32(secondhex[1]));
				Solver = HexagonSolver.SolverFromBattleTechMaps(xm, ym);

				// Try to find hexes via text 
				VSL2.Clear();

				BTHex hex1 = null;
				BTHex hex2 = null;

				try
				{
					hex1 = Solver.BTHexesByTII[Solver.ShortStringToTII[FirstHexInput.Text]];
				}
				catch
				{
					FirstHexInput.Text = string.Empty;
					FirstHexInput.Placeholder = "Not a hex (1,1)0101";
				}
				try
				{
					hex2 = Solver.BTHexesByTII[Solver.ShortStringToTII[SecondHexInput.Text]];
				}
				catch
				{
					SecondHexInput.Text = string.Empty;
					SecondHexInput.Placeholder = "Not a hex (1,1)0101";
				}

				if (hex1 == null || hex2 == null)
					throw new Exception();
				var dist = Solver.HexDistance(hex1.Hexagon, hex2.Hexagon);
				VSL2.Add(new Label() { Text = $"Distance: {dist}" });

				var hexes = Solver.HexesCrossed(hex1.Hexagon, hex2.Hexagon);
				var hexesPrime = Solver.HexesCrossedPrime(hex1.Hexagon, hex2.Hexagon, 1);

				var dictHexs = Solver.TTIByHexes.Where(x => hexes.Contains(x.Key)).ToList();
				var dictHexesPrime = Solver.TTIByHexes.Where(x => hexesPrime.Contains(x.Key)).ToList();

				List<BTHex> bTHexes = new List<BTHex>();
				List<Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>> distanceTo = new List<Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>>();
				foreach (var item in dictHexs)
				{
					distanceTo.Add(new Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>(item, Solver.HexDistance(hex1.Hexagon, item.Key)));
				}
				distanceTo = distanceTo.OrderBy(x => x.Item2).ToList();
				foreach (var item in distanceTo)
				{
					bTHexes.Add(Solver.BTHexesByTII[item.Item1.Value]);
				}

				List<BTHex> bTHexesPrime = new List<BTHex>();
				List<Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>> distanceToPrime = new List<Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>>();
				foreach (var item in dictHexesPrime)
				{
					distanceToPrime.Add(new Tuple<KeyValuePair<Hex, Tuple<int, int>>, int>(item, Solver.HexDistance(hex1.Hexagon, item.Key)));
				}
				distanceToPrime = distanceToPrime.OrderBy(x => x.Item2).ToList();
				foreach (var item in distanceToPrime)
				{
					bTHexesPrime.Add(Solver.BTHexesByTII[item.Item1.Value]);
				}

				for (int i = 0; i < dictHexs.Count; i++)
				{
					VSL2.Add(new Label()
					{
						Text = $"{i + 1:d2} {bTHexes[i].ToShortString()} {(bTHexes.Contains(bTHexesPrime[i]) ? "" : "-" + bTHexesPrime[i].ToShortString())}",
					});
				}

			}
			catch (Exception)
			{
			}
		}

		private void mapSizeChange(object sender, TextChangedEventArgs e)
		{

		}
	}
}