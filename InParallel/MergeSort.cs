namespace InParallel
{
  public static class MergeSort
  {
	public static void Run()
	{
	  Random rand = new((int)DateTime.Now.Ticks);
	  int[] arr = Enumerable.Range(0, 1000000000).AsParallel().Select(i => rand.Next()).ToArray();
	  int[] sortedArr = Sort(arr);
	  Console.WriteLine(IsSorted(sortedArr));
	}

	public static bool IsSorted(int[] arr)
	{
	  for (var i = 1; i < arr.Length; i++)
	  {
		if (arr[i] < arr[i - 1]) return false;
	  }
	  return true;
	}

	public static int[] Sort(int[] items)
	{
	  int[] aux = new int[items.Length];
	  int[] arr = (int[])items.Clone();
	  int maxDepth = (int)Math.Ceiling(Math.Log(Environment.ProcessorCount, 2.0));
	  Sort(arr, aux, 0, items.Length - 1, maxDepth);
	  return arr;
	}

	private static void Sort(int[] arr, int[] aux, int lo, int hi, int depth)
	{
	  if (hi <= lo) return;
	  int mid = (hi + lo) / 2;
	  if (depth >= 0)
	  {
		Parallel.Invoke(() => Sort(arr, aux, lo, mid, depth - 1),
						() => Sort(arr, aux, mid + 1, hi, depth - 1));
	  }
	  else
	  {
		Sort(arr, aux, lo, mid, depth);
		Sort(arr, aux, mid + 1, hi, depth);
	  }
	  Merge(arr, aux, lo, mid, hi);
	}

	private static void Merge(int[] arr, int[] aux, int lo, int mid, int hi)
	{
	  for (int k = lo; k <= hi; k++) aux[k] = arr[k];

	  int i = lo, j = mid + 1;

	  for (int k = lo; k <= hi; k++)
	  {
		if (i > mid) arr[k] = aux[j++];
		else if (j > hi) arr[k] = aux[i++];
		else if (aux[j] < aux[i]) arr[k] = aux[j++];
		else arr[k] = aux[i++];
	  }
	}
  }
}
