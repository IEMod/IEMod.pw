namespace PrepareEnvironment {
	public class Paths {
		/// <summary>
		///     Path to your Pillars of Eternity game folder.
		/// </summary>
		public const string YourGameFolderPath = @"C:\Games\Pillars of Eternity";

		/// <summary>
		///     Path to your backup Managed folder, which should contain the original versions of all the DLL files.
		/// </summary>
		public const string YourOriginalManagedFolder = YourGameFolderPath + @"\PillarsOfEternity_Data\Managed - Copy";

		/// <summary>
		///     Path to your normal Managed folder, which is assumed to be modified, and to which patched assemblies will
		///     automatically be copied.
		/// </summary>
		public const string YourNormalManagedFolder = YourGameFolderPath + @"\PillarsOfEternity_Data\Managed";

		/// <summary>
		///     Path to the DLLs used as sources for modification.
		/// </summary>
		public const string YourDllSourcesPath = @"..\..\..\PoE Dll Source\";

		/// <summary>
		///     Path to the DLLs used as refernces in projects.
		/// </summary>
		public const string YourDllReferencesPath = @"..\..\..\PoE References";
	}
}