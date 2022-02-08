using System.Text;

namespace InParallel
{
  public class BidirectionalDijkstra
  {
	public static void Run()
	{
	  int R = 56, C = 236;
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

	  new BidirectionalDijkstra(grid, (R / 2 - 1, 0), (R / 2 - 1, C - 1));
	  //new BidirectionalDijkstra(grid, (R / 2 - 1, C / 2 - 2), (R / 2 - 1, C / 2 + 2));
	  //new BidirectionalDijkstra(grid, (0, C / 2 - 1), (R - 1, C / 2 - 1));
	  //new BidirectionalDijkstra(grid, (0, 0), (R / 2 - 1, C - 1));
	  //new BidirectionalDijkstra(grid, (0, 0), (R - 1, C - 1));
	}

	private readonly Node[,] _grid;
	private readonly int _rows, _cols;
	private readonly (int Row, int Col)[] _directions;
	private int _meetingRow, _meetingCol;

	public BidirectionalDijkstra(Node[,] grid, (int Row, int Col) start, (int Row, int Col) finish)
	{
	  _grid = grid;
	  _rows = grid.GetLength(0);
	  _cols = grid.GetLength(1);
	  _directions = new (int Row, int Col)[]
	  {
		(-1,0),
		(0,1),
		(1,0),
		(0,-1)
	  };
	  _meetingRow = -1;
	  _meetingCol = -1;

	  bool[,] s_marked = new bool[_rows, _cols],
			  f_marked = new bool[_rows, _cols];
	  int[,] s_destTo = new int[_rows, _cols],
			 f_destTo = new int[_rows, _cols];
	  (int Row, int Col)[,] s_edgeTo = new (int Row, int Col)[_rows, _cols],
							f_edgeTo = new (int Row, int Col)[_rows, _cols];

	  Parallel.Invoke(() => Run(s_edgeTo, s_destTo, s_marked, f_marked, start),
				() => Run(f_edgeTo, f_destTo, f_marked, s_marked, finish));

	  Stack<(int Row, int Col)> s_path = new(), f_path = new();

	  Parallel.Invoke(() => PathTo(s_path, (_meetingRow, _meetingCol), s_destTo, s_edgeTo),
					  () => PathTo(f_path, (_meetingRow, _meetingCol), f_destTo, f_edgeTo));

	  var path = s_path.Concat(f_path.Reverse()).ToHashSet();

	  StringBuilder s = new(_cols);
	  for (int r = 0; r < _rows; r++)
	  {
		for (int c = 0; c < _cols; c++)
		{
		  s.Append((r, c) == start ? 'S' :
				   (r, c) == finish ? 'F' :
				   (r, c) == (_meetingRow, _meetingCol) ? '@' :
				   path.Contains((r, c)) ? ' ' :
				   _grid[r, c].IsWall ? '|' :
				   s_marked[r, c] || f_marked[r, c] ? '+' : '-');
		}
		Console.WriteLine(s);
		s.Clear();
	  }

	  // Console.WriteLine(s.Append('#', _cols));
	  // s.Clear();

	  // for (int r = 0; r < _rows; r++)
	  // {
	  //for (int c = 0; c < _cols; c++)
	  //{
	  //  s.Append(s_marked[r, c] ? '+' : '-');
	  //}
	  //Console.WriteLine(s);
	  //s.Clear();
	  // }

	  // Console.WriteLine(s.Append('#', _cols));
	  // s.Clear();

	  // for (int r = 0; r < _rows; r++)
	  // {
	  //for (int c = 0; c < _cols; c++)
	  //{
	  //  s.Append(f_marked[r, c] ? '+' : '-');
	  //}
	  //Console.WriteLine(s);
	  //s.Clear();
	  // }
	}

	private void Run((int Row, int Col)[,] edgeTo, int[,] destTo, bool[,] marked, bool[,] other_marked, (int Row, int Col) start)
	{
	  PriorityQueue<Node, int> pq = new();
	  pq.Enqueue(_grid[start.Row, start.Col], 0);
	  marked[start.Row, start.Col] = true;
	  destTo[start.Row, start.Col] = 0;
	  while (pq.Count > 0)
	  {
		Node n = pq.Dequeue();
		if (marked[n.Row, n.Col] && other_marked[n.Row, n.Col])
		{
		  if (Interlocked.CompareExchange(ref _meetingRow, n.Row, -1) == -1)
		  {
			Interlocked.CompareExchange(ref _meetingCol, n.Col, -1);
		  }
		  return;
		}
		foreach (var dir in _directions)
		{
		  var (r, c) = (n.Row + dir.Row, n.Col + dir.Col);
		  if (!IsNodeValid(marked, (r, c))) continue;
		  marked[r, c] = true;
		  destTo[r, c] = destTo[n.Row, n.Col] + _grid[r, c].Weight;
		  edgeTo[r, c] = (n.Row, n.Col);
		  pq.Enqueue(_grid[r, c], destTo[r, c]);
		}
	  }
	}

	private void PathTo(Stack<(int Row, int Col)> path, (int Row, int Col) to, int[,] dest, (int Row, int Col)[,] edgeTo)
	{
	  var (r, c) = to;
	  while (dest[r, c] != 0)
	  {
		path.Push((r, c));
		(r, c) = edgeTo[r, c];
	  }
	  path.Push((r, c));
	}

	private bool IsNodeValid(bool[,] marked, (int Row, int Col) pos)
	{
	  return pos.Row >= 0 && pos.Row < _rows &&
			 pos.Col >= 0 && pos.Col < _cols &&
			 !_grid[pos.Row, pos.Col].IsWall &&
			 !marked[pos.Row, pos.Col];
	}
  }
}
