using InParallel;

//Func<int[], bool> IsSorted = (arr) =>
// {
//   for (var i = 1; i < arr.Length; i++)
//   {
//	 if (arr[i] < arr[i - 1]) return false;
//   }
//   return true;
// };

//Random rand = new((int)DateTime.Now.Ticks);
//int[] arr = Enumerable.Range(0, 1000000000).AsParallel().Select(i => rand.Next()).ToArray();
//int[] sortedArr = MergeSort.Sort(arr);
//Console.WriteLine(IsSorted(arr));
//Console.WriteLine(IsSorted(sortedArr));

int R = 56, C = 236;
//R = 10000; C = 10000;
Node[,] grid = new Node[R, C];
//Random rand = new((int)DateTime.Now.Ticks);
for (int r = 0; r < R; r++)
{
  for (int c = 0; c < C; c++)
  {
	//grid[r, c] = new(r, c, 1, rand.Next(0, 6) == 0);
	grid[r, c] = new(r, c, 1, false);
  }
}

var dijktra = new BidirectionalDijkstra(grid, (R / 2 - 1, 0), (R / 2 - 1, C - 1));
//var dijktra = new BidirectionalDijkstra(grid, (R / 2 - 1, C/2-2), (R / 2 - 1, C / 2 +2));
//var dijktra = new BidirectionalDijkstra(grid, (0, C / 2 - 1), (R - 1, C / 2 - 1));
//var dijktra = new BidirectionalDijkstra(grid, (0, 0), (R / 2 - 1, C - 1));
//var dijktra = new BidirectionalDijkstra(grid, (0, 0), (R-1, C - 1));
