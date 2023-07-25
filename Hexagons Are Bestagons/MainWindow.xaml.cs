using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using HexagonBrains;
using System.Collections.Generic;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using HexagonBrains.RedblobHexs;
using System.Linq;
using System.Windows.Ink;

namespace Hexagons_Are_Bestagons
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HexagonSolver Solver { get; set; }
		public Dictionary<Tuple<int, int>, Polygon> polys;
		private double ratio;
		private KeyValuePair<Tuple<int, int>, Hex>? FirstHex;
		private Line? LOS;
		private Line? LOS2;
		public MainWindow()
		{

			InitializeComponent();
			DataContext = this;
			// Prompt for size and then init an array of hexagons 
			int mapsX = 2;
			int mapsY = 1;

			Solver = HexagonSolver.SolverFromBattletechMaps(mapsX, mapsY);
			//Solver = new HexagonSolver(cols, rows, size);
			polys = new Dictionary<Tuple<int, int>, Polygon>();

			double hexWidth = 2 * Solver.Size;
			double hexHeight = Math.Sqrt(3) * Solver.Size;

			// The + 100 is to add in exta text
			Width = hexWidth + (0.75 * hexWidth * Solver.Width) + 100;
			Height = hexHeight * (Solver.Height + 1.5);
			ratio = Width / Height;

			

			Random randy = new Random();
			foreach (var hexagon in Solver.HexagonsAsHexs)
			{
				var poly = new Polygon()
				{
					Points = Helpers.HexPointsToPointsColl(Solver.ourLayout.PolygonCorners(hexagon.Value)),
					Fill = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 0,
							G = 0,
							B = 0
						}
					},
					Stroke = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 255,
							G = 255,
							B = 255
						}
					}
				};
				poly.Tag = hexagon;
				poly.MouseDown += Poly_MouseDown;
				polys.Add(hexagon.Key, poly);
				MainGrid.Children.Add(poly);
			}

			MyWindow.Content = MainGrid;
		}

		private void Poly_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var me = (Polygon)sender;			

			if (FirstHex == null)
			{
				FirstHex = (KeyValuePair<Tuple<int,int>,Hex>)me.Tag;
				
				var coord = Solver.BTHex(FirstHex.Value.Key.Item1, FirstHex.Value.Key.Item2);
				// remove los line 
				MainGrid.Children.Remove(LOS);
				MainGrid.Children.Remove(LOS2);
				// set all polys back to black 
				foreach (var item in polys)
				{
					item.Value.Fill = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 0,
							G = 0,
							B = 0
						}
					};
					item.Value.Stroke = new SolidColorBrush()
					{
						Color = new Color()
						{							
							A = 255,
							R = 255,
							G = 255,
							B = 255,
						}
					};
				}
				Random randy = new Random();
				me.Fill = new SolidColorBrush()
				{
					Color = new Color()
					{
						A = 255,
						R = Convert.ToByte(randy.Next(0, 255)),
						G = Convert.ToByte(randy.Next(0, 255)),
						B = Convert.ToByte(randy.Next(0, 255)),
					}
				};
			}
			else 
			{
				// interp to new selected hex
				KeyValuePair<Tuple<int, int>, Hex> destination = (KeyValuePair<Tuple<int, int>, Hex>)me.Tag;

				// return distance and all hexes underneath 
				var dist = Solver.HexDistance(FirstHex.Value.Value, destination.Value);
				var hexes = Solver.HexesCrossed(FirstHex.Value.Value, destination.Value);
				var hexeses = Solver.HexagonsAsHexsPrime.Where(x => hexes.Contains(x.Key)).ToList();
				foreach (var item in hexeses)
				{
					polys[item.Value].Fill = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 255,
							G = 255,
							B = 255,
						}
					};
					polys[item.Value].Stroke = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 0,
							G = 0,
							B = 0
						}
					};
				}

				// Add a line 
				Random randy = new Random();
				var p1 = Solver.ourLayout.HexToPixel(FirstHex.Value.Value);
				var p2 = Solver.ourLayout.HexToPixel(destination.Value);
				LOS = new Line()
				{
					Stroke = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 255,
							G = 255,
							B = 255,
						}
					},					
					StrokeThickness = 2,
					
					X1 = p1.x,
					Y1 = p1.y,
					X2 = p2.x,
					Y2 = p2.y,
				};
				LOS2 = new Line()
				{
					Stroke = new SolidColorBrush()
					{
						Color = new Color()
						{
							A = 255,
							R = 0,
							G = 0,
							B = 0,
						}
					},
					StrokeThickness = 1,

					X1 = p1.x,
					Y1 = p1.y,
					X2 = p2.x,
					Y2 = p2.y,
				};
				MainGrid.Children.Add(LOS);
				MainGrid.Children.Add(LOS2);
				// Set first hex back to null
				FirstHex = null;
			}

			MyWindow.InvalidateVisual();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			var percentWidthChange = Math.Abs(sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width) / sizeInfo.PreviousSize.Width;
			var percentHeightChange = Math.Abs(sizeInfo.NewSize.Height - sizeInfo.PreviousSize.Height) / sizeInfo.PreviousSize.Height;

			if (percentWidthChange > percentHeightChange)
				this.Height = sizeInfo.NewSize.Width / ratio;
			else
				this.Width = sizeInfo.NewSize.Height * ratio;
			double fakeWidth = Width - 100;
			Solver.Resize((int)(2 * fakeWidth / (3 * Solver.Width + 3)));
			ResizePolys();

			base.OnRenderSizeChanged(sizeInfo);
		}

		private void ResizePolys()
		{
			foreach (var hexagon in Solver.HexagonsAsHexs)
			{
				polys[hexagon.Key].Points = Helpers.HexPointsToPointsColl(Solver.ourLayout.PolygonCorners(hexagon.Value));
			}
		}
	}
	public static class Helpers
	{
		public static PointCollection HexPointsToPointsColl(IList<HexagonBrains.RedblobHexs.Point> input)
		{
			PointCollection output = new PointCollection();
			foreach (var point in input)
			{
				output.Add(new System.Windows.Point(point.x, point.y));
			}

			return output;
		}
	}
}
