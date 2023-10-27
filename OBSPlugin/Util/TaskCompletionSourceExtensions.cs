using System.Threading;

namespace OBSPlugin.Util;

public static class TaskCompletionSourceExtensions
{
  public static async Task WaitAsync<T>(this TaskCompletionSource<T> tcs, CancellationToken cToken = default)
  {
    CancellationTokenSource? cts = null;
    CancellationTokenSource? linkedCts = null;

    try
    {
      cts = new CancellationTokenSource();
      linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cToken);

      var exitToken = linkedCts.Token;
      Func<Task> listenForCancelTaskFactory = async () => {
        await Task.Delay(-1, exitToken).ConfigureAwait(false);
      };

      var cancelTask = listenForCancelTaskFactory();

      await Task.WhenAny(new[] { tcs.Task, cancelTask }).ConfigureAwait(false);

      cts.Cancel();
    }
    finally
    {
      if (linkedCts != null)
        linkedCts.Dispose();
    }
  }
}
