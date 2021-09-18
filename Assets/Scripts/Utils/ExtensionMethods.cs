using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public static class ExtensionMethods
{
	public async static Task<AsyncOperation> GetAwaiter(this AsyncOperation op)
	{
		var task = new TaskCompletionSource<bool>();
		op.completed += (a) => task.TrySetResult(true);
		await task.Task;
		return op;
	}
	public async static Task<T> GetAwaiter<T>(this AsyncOperation op, CancellationToken? cancelToken = null) where T : AsyncOperation
	{
		var task = new TaskCompletionSource<bool>();
		op.completed += (a) => task.TrySetResult(true);
		if (cancelToken != null)
			cancelToken.Value.Register(() => task.TrySetCanceled());
		await task.Task;
		return (T)op;
	}
	public static void Toggle(this GameObject go, bool active)
	{
		if (go.activeSelf != active)
			go.SetActive(active);
	}
	public static int[] ToIntArray(this long number)
	{
		var arr = new int[8];
		for (int i = 0; i < 8; i++)
		{
			arr[i] = (int)(number >> (i * 8) & 0xFF);
		}
		return arr;
	}
	public static long ToLong(this int[] values)
	{
		long number = 0;
		for (int i = 0; i < values.Length; i++)
		{
			number |= ((long)values[i] << 8 * i);
		}
		return number;
	}
	public static string ToHex(this Color color)
	{
		var col = (Color32)color;
		return "#" + col.r.ToString("x2") + col.g.ToString("x2") + col.b.ToString("x2") + col.a.ToString("x2");
	}

	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
		T[] result = new T[length];
		Array.Copy(data, index, result, 0, length);
		return result;
	}
}