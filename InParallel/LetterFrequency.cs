namespace InParallel
{
  public static class LetterFrequency
  {
	public static void Run()
	{
	  var lines = Directory.GetFiles(Path.GetFullPath(@"..\..\..\..\Data"), "*.txt").SelectMany(path => File.ReadLines(path));
	  foreach (var lf in Frequency(lines)) Console.WriteLine(lf);
	}
	public static IReadOnlyDictionary<char, int> Frequency(IEnumerable<string> lines)
	{
	  return lines.AsParallel().SelectMany(line => line.ToCharArray().Where(c => char.IsLetter(c)).Select(c => char.ToUpper(c)))
	  .GroupBy(c => c)
	  .OrderByDescending(c => c.Count())
	  .ToDictionary(k => k.Key, v => v.Count());
	}
  }
}
