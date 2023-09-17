using HexagonBrains;

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



				if (hex1 == null || hex2 == null) throw new Exception();
				var dist = Solver.HexDistance(hex1.Hexagon, hex2.Hexagon);
				VSL2.Add(new Label() { Text = $"Distance: {dist}" });

				var hexes = Solver.HexesCrossed(hex1.Hexagon, hex2.Hexagon);
				var hexesPrime = Solver.HexesCrossedPrime(hex1.Hexagon, hex2.Hexagon, 1);

				var diff = hexesPrime.Where(x => !hexes.Contains(x)).ToList();
				var dictHexs = Solver.TTIByHexes.Where(x => hexes.Contains(x.Key)).ToList();
				var dictHexesPrime = Solver.TTIByHexes.Where(x => hexesPrime.Contains(x.Key)).ToList();
				var dictDiffHexs = Solver.TTIByHexes.Where(x => diff.Contains(x.Key)).ToList();

				VSL2.Add(new Label()
				{
					Text = $"{Solver.BTHexesByTII[dictHexs.First().Value].ToShortString()} your hex\r\n---------------------------------",
				});
				dictHexs.RemoveAt(0);
				dictHexesPrime.RemoveAt(0);

				for (int i = 0; i < dictHexs.Count; i++)
				{
					VSL2.Add(new Label()
					{
						Text = $"{i + 1:d2} {Solver.BTHexesByTII[dictHexs[i].Value].ToShortString()} - {(dictHexs.Contains(dictHexesPrime[i]) ? "" : Solver.BTHexesByTII[dictHexesPrime[i].Value].ToShortString())}",
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