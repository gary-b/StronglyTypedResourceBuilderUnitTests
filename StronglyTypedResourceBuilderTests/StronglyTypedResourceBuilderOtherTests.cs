using NUnit.Framework;
using System;
using System.Resources.Tools;
using System.CodeDom;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace StronglyTypedResourceBuilderTests {
	[TestFixture]
	public class StronglyTypedResourceBuilderOtherTests	{
		static string [] keywords = {"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", 
									"checked", "class", "const", "continue", "decimal", "default", "delegate", 
									"do", "double", "else", "enum", "event", "explicit", "extern", "FALSE", 
									"false", "finally", "fixed", "float", "for", "foreach", "goto", "if", 
									"implicit", "in", "int", "interface", "internal", "is", "lock", "long", 
									"namespace", "new", "null", "object", "operator", "out", "override", "params", 
									"private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", 
									"short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", 
									"throw", "TRUE", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", 
									"ushort", "using", "virtual", "volatile", "void", "while" };
		static char [] specialChars = { ' ', '\u00A0', '.', ',', ';', '|', '~', '@', '#', '%', '^', '&', 
										'*', '+', '-', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', 
										'}', '\"', '\'', ':', '!'};
		CSharpCodeProvider provider = new CSharpCodeProvider ();
		
		Dictionary<string,object> GetAllResourceTypes ()
		{
			Dictionary<string, object> tempDict = new Dictionary<string, object> ();
			Bitmap bmp = new Bitmap (100,100);
			MemoryStream wav = new MemoryStream (1000);
			
			tempDict.Add ("astring", "myvalue");
			tempDict.Add ("bmp", bmp);
			tempDict.Add ("wav", wav);  
			
			return tempDict;
		}
		
		Dictionary<string,object> GetTestResources ()
		{
			Dictionary<string, object> tempDict = new Dictionary<string, object> ();
			tempDict.Add ("akey", string.Empty);
			
			return tempDict;
		}
		
		[Test]
		public void InternalClassFalse ()
		{
			// check access modifiers for class, Culture, ResourceManager, string, stream and standard resource properties
			Dictionary<string, object> testResources = GetAllResourceTypes ();
			string [] unmatchables;
			CodeCompileUnit ccu;
			CodeMemberProperty cmp;
			
			bool isInternal = false;
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            isInternal,
			                                            out unmatchables);
			
			CodeTypeDeclaration resType = ccu.Namespaces [0].Types [0];
			Assert.IsTrue (resType.TypeAttributes ==  TypeAttributes.Public);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("Culture", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("astring", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("bmp", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("wav", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.FamilyAndAssembly
			                                  | MemberAttributes.FamilyOrAssembly));
		}
		
		[Test]
		public void InternalClassTrue ()
		{
			// check access modifiers for class, Culture, ResourceManager, string, stream and standard resource properties
			Dictionary<string, object> testResources = GetAllResourceTypes ();
			string [] unmatchables;
			CodeCompileUnit ccu;
			CodeMemberProperty cmp;
			
			bool isInternal = true;
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            isInternal,
			                                            out unmatchables);
			
			
			CodeTypeDeclaration resType = ccu.Namespaces [0].Types [0];
			Assert.IsTrue (resType.TypeAttributes ==  TypeAttributes.NotPublic);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
               								  | MemberAttributes.Final
                                              | MemberAttributes.Assembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("Culture", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                  	| MemberAttributes.Assembly));
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("astring", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						 | MemberAttributes.Final
			                                 	| MemberAttributes.Assembly));
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("bmp", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                    | MemberAttributes.Assembly));
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("wav", ccu);
			Assert.IsTrue (cmp.Attributes == (MemberAttributes.Abstract
			               						| MemberAttributes.Final
			                                  	| MemberAttributes.Assembly));
		}
		
		[Test, ExpectedException (typeof (ArgumentNullException))]
		public void ProviderNull ()
		{
			// should throw exception
			Dictionary<string, object> testResources = GetTestResources ();
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									null, //setting provider to null
			                                            true,
			                                            out unmatchables);
		}
		
		[Test]
		public void ResourceListEmpty ()
		{
			//should still create class with default members
			Dictionary<string, object> testResources;
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			testResources = new Dictionary<string, object> ();
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			Assert.AreEqual(5,ccu.Namespaces [0].Types [0].Members.Count);
		}
		
		[Test, ExpectedException (typeof (ArgumentNullException))]
		public void ResourceListNull ()
		{
			// should through exception
			Dictionary<string, object> testResources;
			string [] unmatchables; 
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			testResources = null;
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
	}
}





























