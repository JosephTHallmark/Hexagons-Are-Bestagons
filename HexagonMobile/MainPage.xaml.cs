using HexagonBrains;

namespace HexagonMobile
{
	public partial class MainPage : ContentPage
	{
		private HexagonSolver Solver { get; set; }

		public MainPage()
		{
			InitializeComponent();
			var view = this;

			int mapsX = 1;
			int mapsY = 1;

			Solver = HexagonSolver.SolverFromBattletechMaps(mapsX, mapsY, 15, 17);

			//foreach (var item in Solver.ShortStringToTII)
			//{
			//	VSL2.Add(new Label()
			//	{
			//		Text = item.Key.ToString(),
			//	});
			//}
		}

		private void Submit(object sender, EventArgs e)
		{
			// Try to find hexes via text 
			try
			{
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
				var hexesPrime = Solver.HexesCrossedPrime(hex1.Hexagon, hex2.Hexagon, 10);

				var diff = hexesPrime.Where(x => !hexes.Contains(x)).ToList();
				var dictHexs = Solver.TTIByHexes.Where(x => hexes.Contains(x.Key)).ToList();
				var dictDiffHexs = Solver.TTIByHexes.Where(x => diff.Contains(x.Key)).ToList();

				foreach (var item in dictHexs)
				{
					VSL2.Add(new Label()
					{
						Text = $"{Solver.BTHexesByTII[item.Value].ToShortString()}",
					});
				}

			}
			catch (Exception)
			{
			}
		}
	}
}