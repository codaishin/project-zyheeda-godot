#if TOOLS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

public class Printer : ITestListener {
	private int indent = 0;

	private void Print(string value) {
		var prefix = new string(' ', this.indent);
		value = string.Join("\n", value.Split("\n").Select(v => prefix + v));
		GD.Print(value);
	}

	private void Print(ITest test) {
		if (!test.HasChildren) {
			return;
		}
		this.Print("\n" + test.FullName);
	}

	private void Print(ITestResult result) {
		if (result.Test.HasChildren) {
			return;
		}
		this.Print($"[{result.ResultState}] {result.Test.FullName}");
		if (result.Message != null) {
			this.Print(result.Message);
		}
		if (result.StackTrace != null) {
			this.Print(result.StackTrace);
		}
	}

	public void SendMessage(TestMessage message) { }
	public void TestFinished(ITestResult result) {
		this.Print(result);
		this.indent -= 2;
	}
	public void TestOutput(TestOutput output) { }
	public void TestStarted(ITest test) {
		if (test.Parent == null) {
			return;
		}
		this.indent += 2;
		this.Print(test);
	}
}

public class Tests : Node {

	private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
	private static readonly Dictionary<string, object> settings = new();
	private static readonly string idPrefix = "__test__";

	private static readonly ITestListener listener = new Printer();
	private static readonly ITestFilter filter = new MatchEverything();


	public override void _Ready() {
		var controller = new FrameworkController(
			Tests.assembly,
			Tests.idPrefix,
			Tests.settings
		);
		_ = controller.LoadTests();

		GD.Print("\nRunning tests");

		ITestResult result = controller.Runner.Run(Tests.listener, Tests.filter);
		var failCount = result.TotalCount - result.PassCount;

		GD.Print(failCount == 0 ? "\nSuccess" : "\nSome tests had Errors");
		GD.Print(result.PassCount, " of ", result.TotalCount, " tests passed\n");
		this.GetTree().Quit(failCount);
	}
}
#endif
