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

	private void PrintIndented(string value) {
		var spaceIndent = new string(' ', this.indent);
		value = value
			.Split("\n")
			.Select(line => spaceIndent + line)
			.Join("\n");
		GD.Print(value);
	}

	private void PrintIndented(ITest test) {
		if (test.Parent == null) {
			GD.Print("\nRunning tests");
			return;
		}
		if (!test.HasChildren) {
			return;
		}
		this.PrintIndented("\n" + test.FullName);
	}

	private void PrintIndented(ITestResult result) {
		if (result.Test.Parent == null) {
			var status =
				result.TotalCount - result.PassCount == 0
					? "\nSuccess"
					: "\nSome tests failed";
			var counts = $"{result.PassCount} of {result.TotalCount}, tests passed\n";
			this.PrintIndented(status);
			this.PrintIndented(counts);
			return;
		}
		if (result.Test.HasChildren) {
			return;
		}
		this.PrintIndented($"[{result.ResultState}] {result.Test.Name}");
		if (result.Message != null) {
			this.PrintIndented(result.Message);
		}
		if (result.StackTrace != null) {
			this.PrintIndented(result.StackTrace);
		}
	}

	private static int IndentationFor(ITest test) {
		return test.Parent == null ? 0 : 2;
	}

	public void SendMessage(TestMessage message) { }

	public void TestFinished(ITestResult result) {
		this.PrintIndented(result);
		this.indent -= Printer.IndentationFor(result.Test);
	}

	public void TestOutput(TestOutput output) { }

	public void TestStarted(ITest test) {
		this.indent += Printer.IndentationFor(test);
		this.PrintIndented(test);
	}
}

public class Tests : Node {
	private readonly Assembly assembly = Assembly.GetExecutingAssembly();
	private readonly string idPrefix = "__test__";
	private readonly Dictionary<string, object> settings = new();

	private readonly ITestListener listener = new Printer();
	private readonly ITestFilter filter = new MatchEverything();

	private static SceneTree? tree;

	public static SceneTree Tree =>
		Tests.tree ?? throw new NullReferenceException("scene tree not set");

	public override void _Ready() {
		Tests.tree = this.GetTree();

		var controller = new FrameworkController(
			this.assembly,
			this.idPrefix,
			this.settings
		);
		_ = controller.LoadTests();
		var result = controller.Runner.Run(this.listener, this.filter);

		Tests.tree.Quit(result.TotalCount - result.PassCount);
	}
}
#endif
