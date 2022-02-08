namespace InParallel
{
  public static class AsyncLINQ
  {
	public static async Task<R> Bind<T, R>(this Task<T> task, Func<T, Task<R>> cont) => await cont(await task.ConfigureAwait(false)).ConfigureAwait(false);
	public static async Task<R> SelectMany<T, R>(this Task<T> task, Func<T, Task<R>> then) => await Bind(task, then);
	public static async Task<T> Tap<T>(this Task<T> task, Func<T, Task> action)
	{
	  var value = await task;
	  await action(value);
	  return value;
	}
  }
}
