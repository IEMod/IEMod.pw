using System;

namespace ModdingEnvironment {

	public static class RelativePaths {
		/// <summary>
		///     Path to the DLLs used as sources for modification.
		/// </summary>
		public const string DllSourcesPath = @"..\..\..\PoE Dll Source\";

		/// <summary>
		///     Path to the DLLs used as refernces in projects.
		/// </summary>
		public const string ReferencesPath = @"..\..\..\PoE References";

		/// <summary>
		/// Folder that will contain the latest "production" build. Used for manufacturing releases.
		/// </summary>
		public const string LatestBuildFolder = @"..\..\..\PoE Latest Builds";

		public const string ILSpyPath = @"..\..\..\Start\Decompiler\ILSpy.exe";

		/// <summary>
		/// This is probably your path to PEVerify as well. PEVerify is a program that checks IL for issues.
		/// </summary>
		public const string PeVerifyPath = @"..\..\..\Binaries\PEVerify\PEVerify.exe";
	}
}
