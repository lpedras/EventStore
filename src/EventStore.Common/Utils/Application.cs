using System;
using System.Collections.Generic;
using System.Threading;
using ILogger = Serilog.ILogger;

namespace EventStore.Common.Utils {
	public enum ExitCode {
		Success = 0,
		Error = 1
	}

	public class Application {
		public const string AdditionalCommitChecks = "ADDITIONAL_COMMIT_CHECKS";
		public const string InfiniteMetastreams = "INFINITE_METASTREAMS";
		public const string DumpStatistics = "DUMP_STATISTICS";
		public const string DoNotTimeoutRequests = "DO_NOT_TIMEOUT_REQUESTS";
		public const string AlwaysKeepScavenged = "ALWAYS_KEEP_SCAVENGED";
		public const string DisableMergeChunks = "DISABLE_MERGE_CHUNKS";

		private static readonly ILogger Log = Serilog.Log.ForContext<Application>();

		private static Action<int> _exit = delegate {
		};

		private static int _exited;

		private static readonly HashSet<string> _defines = new HashSet<string>();

		public static void RegisterExitAction(Action<int> exitAction) {
			Ensure.NotNull(exitAction, "exitAction");

			_exit = exitAction;
		}

		public static void ExitSilent(int exitCode, string reason) => Exit(exitCode, reason, true);
		public static void Exit(ExitCode exitCode, string reason) => Exit((int)exitCode, reason);
		public static void Exit(int exitCode, string reason) => Exit(exitCode, reason, false);

		private static void Exit(int exitCode, string reason, bool silent) {
			if (Interlocked.CompareExchange(ref _exited, 1, 0) != 0)
				return;

			Ensure.NotNullOrEmpty(reason, "reason");

			if (!silent) {
				if (exitCode != 0)
					Log.Error("Exiting with exit code: {exitCode}.\nExit reason: {e}", exitCode, reason);
				else
					Log.Information("Exiting with exit code: {exitCode}.\nExit reason: {e}", exitCode, reason);
			}

			_exit?.Invoke(exitCode);
		}

		public static void AddDefines(IEnumerable<string> defines) {
			foreach (var define in defines.Safe()) {
				_defines.Add(define.ToUpper());
			}
		}

		public static bool IsDefined(string define) {
			Ensure.NotNull(define, "define");
			return _defines.Contains(define.ToUpper());
		}
	}
}
