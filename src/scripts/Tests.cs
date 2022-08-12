#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

public class MatchEverything : ITestFilter {
	public TNode AddToXml(TNode parentNode, bool recursive) {
		throw new NotImplementedException();
	}
	public TNode ToXml(bool recursive) {
		throw new NotImplementedException();
	}
	public bool IsExplicitMatch(ITest test) {
		return true;
	}
	public bool Pass(ITest test) {
		return true;
	}
}

public static class WindowsHelper {
	// https://docs.microsoft.com/en-us/windows/console/getstdhandle
	private const int STD_OUTPUT = -11;

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetStdHandle(int nStdHandle);


	// https://docs.microsoft.com/en-us/windows/console/getconsolemode
	// https://docs.microsoft.com/en-us/windows/console/setconsolemode
	private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

	[DllImport("kernel32.dll")]
	private static extern bool GetConsoleMode(
		IntPtr hConsoleHandle,
		out uint lpMode
	);

	[DllImport("kernel32.dll")]
	private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	public static bool TryEnableTerminalColors() {
		var stdOut = WindowsHelper.GetStdHandle(WindowsHelper.STD_OUTPUT);
		return
			WindowsHelper.GetConsoleMode(stdOut, out var mode) &&
			WindowsHelper.SetConsoleMode(
				stdOut,
				mode | WindowsHelper.ENABLE_VIRTUAL_TERMINAL_PROCESSING
			);
	}

	public static bool IsWindows => OS.GetName() == "Windows";
}

public class Printer : ITestListener {
	private int indent = 0;
	private readonly string red = "";
	private readonly string green = "";
	private readonly string reset = "";

	private static bool CanUseColors() {
		return !WindowsHelper.IsWindows || WindowsHelper.TryEnableTerminalColors();
	}

	public Printer(bool useColors) {
		if (!useColors || !Printer.CanUseColors()) {
			return;
		}

		this.red = "\u001b[31m";
		this.green = "\u001b[32m";
		this.reset = "\u001b[0m";
	}

	private void PrintIndented(string value) {
		var spaceIndent = new string(' ', this.indent);
		var prefixIndent = (string line) => spaceIndent + line;
		value = value
			.Split("\n")
			.Select(prefixIndent)
			.Join("\n");

		GD.Print(value);
	}

	private void PrintIndented(ITest test) {
		if (test.Parent != null) {
			return;
		}
		var title = new[] {
			"#################",
			"# Running tests #",
			"#################"
		}.Join("\n");
		this.PrintIndented($"\n{title}\n");
	}

	private void PrintFinalResult(ITestResult result) {
		var message = result.TotalCount == 0
			? $"\n[{this.red}Failed(Empty){this.reset}] No tests found!\n"
			: $"\n{result.PassCount} of {result.TotalCount} tests passed\n";
		this.PrintIndented(message);
	}

	private void PrintTestResult(ITestResult result) {
		this.indent += Printer.IndentationFor(result.Test);

		var clr = result.ResultState == ResultState.Success
			? this.green
			: this.red;
		var summary = $"[{clr}{result.ResultState}{this.reset}] {result.Test.Name}";

		this.PrintIndented(summary);
		if (result.Message != null) {
			this.PrintIndented(result.Message);
		}
		if (result.StackTrace != null) {
			this.PrintIndented(result.StackTrace);
		}

		this.indent -= Printer.IndentationFor(result.Test);
	}

	private void PrintFixtureResult(ITestResult result) {
		var clr = result.ResultState == ResultState.Success
			? this.green
			: this.red;
		var summary = $"[{clr}{result.ResultState}{this.reset}] {result.Test.Name}";

		this.PrintIndented(summary);
		if (result.ResultState == ResultState.Success) {
			return;
		}
		foreach (var child in result.Children) {
			this.PrintTestResult(child);
		}
	}

	private void PrintIndented(ITestResult result) {
		if (result.Test.Parent == null) {
			this.PrintFinalResult(result);
			return;
		}
		if (!result.Test.HasChildren) {
			return;
		}
		this.PrintFixtureResult(result);
	}

	private static int IndentationFor(ITest test) {
		return test.Parent == null ? 0 : 2;
	}

	public void TestFinished(ITestResult result) {
		this.PrintIndented(result);
		this.indent -= Printer.IndentationFor(result.Test);
	}

	public void TestStarted(ITest test) {
		this.PrintIndented(test);
		this.indent += Printer.IndentationFor(test);
	}

	public void SendMessage(TestMessage message) { }

	public void TestOutput(TestOutput output) { }
}

public class Tests : Node {
	private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
	private static readonly string idPrefix = "__test__";
	private static readonly Dictionary<string, object> settings = new();

	private static Node? baseNode = null;
	public static Node BaseNode =>
		Tests.baseNode ?? throw new NullReferenceException("scene tree not set");

	private readonly ITestListener listener = new Printer(
		useColors: OS.GetCmdlineArgs().Contains("--terminal-colored")
	);
	private readonly ITestFilter filter = new MatchEverything();
	private readonly FrameworkController controller = new(
		Tests.assembly,
		Tests.idPrefix,
		Tests.settings
	);

	private bool isRunning = false;

	public override void _Process(float delta) {
		if (this.controller.Runner.WaitForCompletion(0)) {
			var result = this.controller.Runner.Result;
			var returnCode = result.TotalCount == 0
				? 1
				: result.TotalCount - result.PassCount;
			this.GetTree().Quit(returnCode);
		}
		if (this.isRunning) {
			return;
		}
		Tests.baseNode = this;
		_ = this.controller.LoadTests();
		this.controller.Runner.RunAsync(this.listener, this.filter);
		this.isRunning = true;
	}
}
#endif
