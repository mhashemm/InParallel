using System.Threading.Tasks.Dataflow;

namespace InParallel
{
  public static class Download
  {
	public static async Task Run()
	{
	  await FileAsync("https://picsum.photos/200", Path.GetFullPath(@$"..\..\..\..\Data\image.jpg"));

	  var downloadAndSave = DownloadAndSaveTDF();
	  foreach (var i in Enumerable.Range(0, 10))
	  {
		downloadAndSave.Post(("https://picsum.photos/200", Path.GetFullPath(@$"..\..\..\..\Data\{i}.jpg")));
	  }
	}

	public static async Task FileAsync(string url, string path)
	{
	  using var http = new HttpClient();
	  using var fs = File.Create(path);
	  await http.GetAsync(url).SelectMany(async (req) => await req.Content.ReadAsStreamAsync()).Tap((s) => s.CopyToAsync(fs));
	  await fs.FlushAsync();
	}

	public static ITargetBlock<(string Url, string Path)> DownloadAndSaveTDF()
	{
	  var download = new TransformBlock<(string Url, string Path), (string, byte[])>(async (t) =>
	  {
		using var http = new HttpClient();
		var image = await (await http.GetAsync(t.Url)).Content.ReadAsByteArrayAsync();
		return (t.Path, image);
	  });

	  var save = new ActionBlock<(string, byte[])>(async (s) =>
	  {
		await File.WriteAllBytesAsync(s.Item1, s.Item2);
		Console.WriteLine(s.Item1);
	  });

	  download.LinkTo(save);
	  return download;
	}
  }
}
