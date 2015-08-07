using System.IO;
using System.Runtime.CompilerServices;
using Patchwork.Attributes;

namespace IEMod.Helpers
{

	/// <summary>
	/// Indicates that the code should be unreachable.
	/// </summary>
	[NewType]
	public class DeadEndException : IEModException {
		public DeadEndException(string location)
		: base(string.Format("Code should be unreachable. Location: {0}", location)){
			
		}
	}

}
