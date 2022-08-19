using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public class Coroutines : Node {
	private int counter = 0;
	private (Task, CancellationTokenSource)? taskAndToken;

	public override void _Ready() {
		var ctSrc = new CancellationTokenSource();
		var task = Task.Factory.StartNew(this.RunCount(ctSrc.Token), ctSrc.Token);
		this.taskAndToken = (task, ctSrc);
	}

	public override void _Process(float delta) {
		if (this.counter < 100) {
			return;
		}
		if (this.taskAndToken is null) {
			GD.Print("no task or token");
			return;
		}
		var (task, token) = this.taskAndToken.Value;
		token.Cancel();
		task.Dispose();
		this.taskAndToken = null;
	}

	private Func<Task> RunCount(CancellationToken ct) {
		return async () => {
			using var signalAwaiters = this.Count();
			while (!ct.IsCancellationRequested && signalAwaiters.MoveNext()) {
				_ = await signalAwaiters.Current;
			}
		};
	}

	private IEnumerator<SignalAwaiter> Count() {
		while (true) {
			yield return this.ToSignal(this.GetTree(), "idle_frame");
			GD.Print($"Counter: {++this.counter}");
		}
	}
}
