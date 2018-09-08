using ApprovalTests.Reporters;
using NUnit.Framework;

[assembly: UseReporter(typeof(DiffReporter))]
[assembly: NonParallelizable]