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
				var FirstHexString = $"({FHxm.Text},{FHym.Text}){FHcord.Text}";
				var SecondHexString = $"({SHxm.Text},{SHym.Text}){SHcord.Text}";

				var xm = Math.Max(Convert.ToInt32(FHxm.Text), Convert.ToInt32(SHxm.Text));
				var ym = Math.Max(Convert.ToInt32(FHym.Text), Convert.ToInt32(SHym.Text));
				Solver = HexagonSolver.SolverFromBattleTechMaps(xm, ym, Convert.ToInt32(mapSizeX.Text), Convert.ToInt32(mapSizeY.Text));

				// Try to find hexes via text 
				VSL2.Clear();

				BTHex hex1 = null;
				BTHex hex2 = null;

				try
				{
					hex1 = Solver.BTHexesByTII[Solver.ShortStringToTII[FirstHexString]];
				}
				catch
				{
					FirstHexString = string.Empty;
					FHxm.Placeholder = "1st Hex Map X";
					FHym.Placeholder = "1st Hex Map Y";
					FHcord.Placeholder = "1st Hex Map Cord";
				}
				try
				{
					hex2 = Solver.BTHexesByTII[Solver.ShortStringToTII[SecondHexString]];					
				}
				catch
				{
					SecondHexString = string.Empty; 
					SHxm.Placeholder = "2nd Hex Map X";
					SHym.Placeholder = "2nd Hex Map Y";
					SHcord.Placeholder = "2nd Hex Map Cord";
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
						Text = $"{i:d2} {bTHexes[i].ToShortString()} {(bTHexes.Contains(bTHexesPrime[i]) ? "" : "-" + bTHexesPrime[i].ToShortString())}",
					});
				}

			}
			catch (Exception exp)
			{
			}
		}

		private void mapSizeChange(object sender, TextChangedEventArgs e)
		{

		}

		private void FHxm_Completed(object sender, EventArgs e)
		{
			FHym.Focus();
		}
		private void FHym_Completed(object sender, EventArgs e)
		{
			FHcord.Focus();
		}
		private void FHcord_Completed(object sender, EventArgs e)
		{
			SHxm.Focus();
		}

		private void SHxm_Completed(object sender, EventArgs e)
		{
			SHym.Focus();
		}
		private void SHym_Completed(object sender, EventArgs e)
		{
			SHcord.Focus();
		}
		private void SHcord_Completed(object sender, EventArgs e)
		{
			//Submit(sender, e);
		}

		private void ClearBtn_Clicked(object sender, EventArgs e)
		{
			FHxm.Text = string.Empty;
			FHym.Text = string.Empty; 
			FHcord.Text = string.Empty;
			SHxm.Text = string.Empty;
			SHym.Text = string.Empty;
			SHcord.Text = string.Empty;			
		}

		private void ClearBtn2_Clicked(object sender, EventArgs e)
		{
			VSL2.Clear();
		}
	}
}