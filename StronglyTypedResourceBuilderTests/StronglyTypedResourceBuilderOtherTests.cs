using NUnit.Framework;
using System;
using System.Resources.Tools;
using System.CodeDom;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace StronglyTypedResourceBuilderTests
{
	[TestFixture()]
	public class StronglyTypedResourceBuilderOtherTests
	{
		
		static string[] keywords = {"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", 
									"checked", "class", "const", "continue", "decimal", "default", "delegate", 
									"do", "double", "else", "enum", "event", "explicit", "extern", "FALSE", 
									"false", "finally", "fixed", "float", "for", "foreach", "goto", "if", 
									"implicit", "in", "int", "interface", "internal", "is", "lock", "long", 
									"namespace", "new", "null", "object", "operator", "out", "override", "params", 
									"private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", 
									"short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", 
									"throw", "TRUE", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", 
									"ushort", "using", "virtual", "volatile", "void", "while" };
		
		static char[] specialChars = { ' ', '\u00A0', '.', ',', ';', '|', '~', '@', '#', '%', '^', '&', 
									'*', '+', '-', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', 
									'}', '\"', '\'', ':', '!'};
		
		
		[Test()]
		public void InternalClassFalse ()
		{
			
			// check access modifiers for class, Culture, ResourceManager, string, stream and standard resource properties
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			CodeMemberProperty cmp;
			
			Bitmap bmp = new Bitmap (100,100); //size 100x100
			MemoryStream wav = new MemoryStream (1000); //size in bytes
			
			testResources.Add ("astring", "myvalue"); // dont use key of "string" as its a keyword
			testResources.Add ("bmp", bmp);
			testResources.Add ("wav", wav);  
			
			bool isInternal = false;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            isInternal,
			                                            out unmatchables);
			
			CodeTypeDeclaration resType = ccu.Namespaces [0].Types [0];
			Assert.IsTrue (resType.TypeAttributes ==  TypeAttributes.Public);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("Culture",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("astring",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("bmp",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("wav",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
		}
		
		[Test()]
		public void InternalClassTrue ()
		{
			// check access modifiers for class, Culture, ResourceManager, string, stream and standard resource properties
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			CodeMemberProperty cmp;
			
			Bitmap bmp = new Bitmap (100,100); //size 100x100
			MemoryStream wav = new MemoryStream (1000); //size in bytes
			
			testResources.Add ("astring", "myvalue"); // dont use key of "string" as its a keyword
			testResources.Add ("bmp", bmp);
			testResources.Add ("wav", wav);  
			
			bool isInternal = true;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            isInternal,
			                                            out unmatchables);
			
			
			CodeTypeDeclaration resType = ccu.Namespaces [0].Types [0];
			Assert.IsTrue (resType.TypeAttributes ==  TypeAttributes.NotPublic);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.Assembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("Culture",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                  	| MemberAttributes.Assembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("astring",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						 | MemberAttributes.Final
			                                 	| MemberAttributes.Assembly));
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("bmp",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                    | MemberAttributes.Assembly));
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("wav",ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                  	| MemberAttributes.Assembly));
		}
		
		[Test ()]
		public void ProviderNull ()
		{
			// should throw exception
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			testResources.Add ("akey", "");
			
			CSharpCodeProvider provider = null;
			
			try {
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
			} catch (Exception ex) {
				exceptionRaised = true;
				Assert.IsInstanceOf<ArgumentNullException> (ex);
			}
			finally {
				Assert.IsTrue (exceptionRaised,"An exception is expected here");
			}
		}
		
		[Test ()]
		public void ResourceListEmpty ()
		{
			
			//should still create class with default members
			
			Dictionary<string, object> testResources;
			string[] unmatchables;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			testResources = new Dictionary<string, object>();
			
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			Assert.AreEqual(5,ccu.Namespaces[0].Types[0].Members.Count);
		}
		
		[Test ()]
		public void ResourceListNull ()
		{
			// should through exception
			
			Dictionary<string, object> testResources;
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			testResources = null;
			
			try {
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestRes",
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
			} catch (Exception ex) {
				exceptionRaised = true;
				Assert.IsInstanceOf<ArgumentNullException> (ex);
			} finally {
				Assert.IsTrue (exceptionRaised);
			}
		}
		
		
		
		
	}
}





























