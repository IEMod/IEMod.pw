using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using Mono.Cecil;
using Patchwork;
using Patchwork.Attributes;
using Patchwork.Utility;

namespace Start {
	internal static class ModifiedDecompiler {
		private static string Decompile(string name, MethodDefinition mtd) {
			
			var decompilerSettings = new ICSharpCode.Decompiler.DecompilerSettings {
				ShowXmlDocumentation = false,
				UsingDeclarations = false,
			};
			var output = new ICSharpCode.Decompiler.PlainTextOutput();
			var method = mtd;
			var astBuilder = new AstBuilder(new DecompilerContext(method.DeclaringType.Module) {
				CancellationToken = new CancellationToken(),
				CurrentType = method.DeclaringType,
				Settings = decompilerSettings,
			});

			astBuilder.AddMethod(method);
			astBuilder.GenerateCode(output);
			var methodCode = output.ToString();

			// remove top comment line
			//if (methodCode.StartsWith("//")) {
			//	methodCode = methodCode.Substring(methodCode.IndexOf('\n') + 1);
			//}

			// Remove any attributes
			//var attrRE = new Regex(@"^(?:\[[^]]+]\s*){1,}");
			//methodCode = attrRE.Replace(methodCode, "", 1);

			// change the method name to the mod's name for the method, and replace parameter names with game names
			var methodName = mtd.Name;
			var nameLocation = methodCode.IndexOf(" " + methodName) + 1;
			var nameEnd = nameLocation + methodName.Length;

			// Prepend "void " if this was a constructor (since methodCode won't have a return type)
			var correctName = mtd.IsConstructor ? ("void " + name) : name;
			methodCode = methodCode.Substring(0, nameLocation) + correctName + methodCode.Substring(nameEnd);
			return methodCode;
		}

		/// <summary>
		/// This method replaces the bodies of methods with ModifiesMember() with the original bodies, from the modified members. This lets you merge any changes in the game code with your mod code.
		/// </summary>
		/// <param name="changes"></param>
		/// <param name="overwriteExistingSource">If true, the existing files will be modified, instead of the source.</param>
		public static void ReplaceCodeWithOriginal(PatchingManifest manifest, bool overwriteExistingSource = false) {
			var methodActions = manifest.MethodActions[typeof (ModifiesMemberAttribute)];
			var changeEntries =
				from methodAction in methodActions
				let yourMember = methodAction.YourMember
				let change = yourMember.Body
				select new {
					Start = change.FirstSequencePoint(),
					End = change.LastSequencePoint(),
					Name = yourMember.Name,
					ModifiedMember = methodAction.TargetMember
				};
			foreach (var byFile in changeEntries.GroupBy(x => x.Start.Document.Url)) {
				var newName = overwriteExistingSource ? byFile.Key : Path.ChangeExtension(byFile.Key, ".new.cs");

				var contents = File.ReadAllLines(byFile.Key);
				var linkedList = new LinkedList<char>();
				var lineNodes = new List<LinkedListNode<char>>();
				foreach (var line in contents) {
					var curLine = linkedList.AddLastRange(line + "\r\n");
					lineNodes.Add(curLine);
				}

				var fileChanges = byFile.OrderBy(x => x.Start.StartLine).ToList();

				for (int index = 0; index < fileChanges.Count; index++) {
					//basically, we're selecting all of the text from the first method instruction to the last
					//and replacing it with the decompiled text of the original method. We exactly preserves the structure
					//(even the brace style and indentation), so the diff tools will know exactly what to compare with what.
					var change = fileChanges[index];
					var startNode = lineNodes[change.Start.StartLine - 1].Skip(change.Start.StartColumn - 1);
					var preStart = startNode.Previous;
					var endNode = lineNodes[change.End.EndLine - 1].Skip(change.End.EndColumn - 1);
					
					var decompiledSource = Decompile(change.Name, change.ModifiedMember);
					var lines = decompiledSource.Split(new[] {
						'\r',
						'\n'
					}, StringSplitOptions.RemoveEmptyEntries).ToList();
					lines.RemoveAt(0); //remove the method signature
					//find the indent of the current level:
					var indent =
						lineNodes[change.Start.StartLine - 1].NextNodes().TakeWhile(node => node.Value == '\t' || node.Value == ' ')
							.Aggregate("",
								(acc, cur) => acc + cur.Value);
					//add the indent to every level except for the first:
					decompiledSource = lines.Select((line, i) => i == 0 ? line : indent + line).Join("\r\n");
					//replace the decompiled body with the existing body:
					linkedList.RemoveRange(startNode, endNode);
					linkedList.AddAfterRange(preStart, decompiledSource);
				}
				File.WriteAllText(newName, new string(linkedList.ToArray()));
			}
		}
	}
}