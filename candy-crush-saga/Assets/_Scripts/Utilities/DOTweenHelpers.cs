using Cysharp.Threading.Tasks;
using DG.Tweening;

public static class DOTweenHelpers
{
    public static UniTask WaitForSequenceCompletion(Sequence sequence)
    {
        var taskCompletionSource = new UniTaskCompletionSource();
        sequence.OnComplete(() => taskCompletionSource.TrySetResult());
        return taskCompletionSource.Task;
    }
}
