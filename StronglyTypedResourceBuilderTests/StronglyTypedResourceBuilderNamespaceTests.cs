using NUnit.Framework;
using System;
using System.Resources.Tools;
using System.CodeDom;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace StronglyTypedResourceBuilderTests
{
	[TestFixture()]
	public class StronglyTypedResourceBuilderNamespaceTests
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
		
		
		[Test ()]
		public void GeneratedCodeNamespaceEmpty ()
		{
			
			// empty namespace allowed
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			
			input = "";
			
			expected = "";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            input,
			                                            "TestResourceNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			Assert.AreEqual (expected,ccu.Namespaces[0].Name);
		}
		
		[Test ()]
		public void GeneratedCodeNamespaceNull ()
		{
			
			// null should be replaced with ""
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			
			input = null;
			
			expected = "";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            input,
			                                            "TestResourceNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			Assert.AreEqual (expected,ccu.Namespaces[0].Name);
		}
		
		[Test ()]
		public void GeneratedCodeNamespaceProviderInvalidIdentifiersOK ()
		{
			// identifiers which are still invalid after CreateValidIdentifier called allowed through in .NET framework
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, output, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			
			input = "te$st";
			expected = "te$st";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            input,
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			output = ccu.Namespaces[0].Name;
			
			Assert.AreEqual (expected,output);
		}
		
		[Test()]
		public void GeneratedCodeNamespaceProviderKeywords ()
		{
			string expected;
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			foreach (string input in keywords) {
			
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            input,
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = provider.CreateValidIdentifier(input);
				
				Assert.AreEqual (expected,ccu.Namespaces[0].Name);
			}
		}
		
		[Test()]
		public void GeneratedCodeNamespaceProviderKeywordsMultipart ()
		{
			// .NET framework does not check individiual elements of multipart namespace
			
			string expected, input;
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			foreach (string word in keywords) {
				
				input = "Primary." + word;
				
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            input,
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = provider.CreateValidIdentifier(input);
				
				Assert.AreEqual (expected,ccu.Namespaces[0].Name);
			}
		}
		
		[Test ()]
		public void GeneratedCodeNamespaceSpecialChars ()
		{
			// invalid chars replaced with _ noting (. and :) are allowed by .NET framework
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, output, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			
			foreach (char c in specialChars) {
				
				input = "test" + c.ToString ();
				
				if (c == '.' || c == ':')
					expected = input;
				else
					expected = StronglyTypedResourceBuilder.VerifyResourceName(input,provider);
			
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            input,
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				output = ccu.Namespaces[0].Name;
				
				Assert.AreEqual (expected,output);
			}
		}
		
		[Test ()]
		public void ResourcesNamespaceEmpty ()
		{
			
			// when ResourcesNamespace is String.Empty no namespace is used
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, output, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			CodeMemberProperty resourceManager;
			CodeVariableDeclarationStatement cvds;
			
			input = "";
			
			expected = "TestClass";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNameSpace",
			                                            input,
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			resourceManager = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
			cvds = ((CodeVariableDeclarationStatement)((CodeConditionStatement)resourceManager.GetStatements[0]).TrueStatements[0]);
			output  = ((CodePrimitiveExpression)((CodeObjectCreateExpression)cvds.InitExpression).Parameters[0]).Value.ToString ();
			
			Assert.AreEqual (expected,output);
		}
		
		[Test ()]
		public void ResourcesNamespaceNull ()
		{
			
			// when ResourcesNamespace is null generatedCodeNamespace is used in its place
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, output, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			CodeMemberProperty resourceManager;
			CodeVariableDeclarationStatement cvds;
			
			input = null;
			
			expected = "TestNameSpace.TestClass";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNameSpace",
			                                            input,
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			resourceManager = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
			cvds = ((CodeVariableDeclarationStatement)((CodeConditionStatement)resourceManager.GetStatements[0]).TrueStatements[0]);
			output  = ((CodePrimitiveExpression)((CodeObjectCreateExpression)cvds.InitExpression).Parameters[0]).Value.ToString ();
			
			Assert.AreEqual (expected,output);
		}
				
		[Test()]
		public void ResourcesNamespaceProviderKeywords ()
		{
			// not validated against provider keywords in .net framework
			
			string output,expected;
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeMemberProperty resourceManager;
			CodeVariableDeclarationStatement cvds;
			
			foreach (string input in keywords) {
			
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            "TestNamespace",
				                                            input,
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				
				expected = input + ".TestClass";
				resourceManager = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
				cvds = ((CodeVariableDeclarationStatement)((CodeConditionStatement)resourceManager.GetStatements[0]).TrueStatements[0]);
				output  = ((CodePrimitiveExpression)((CodeObjectCreateExpression)cvds.InitExpression).Parameters[0]).Value.ToString ();
				
				Assert.AreEqual (expected,output);
			}
		}
		
		[Test ()]
		public void ResourcesNamespaceSpecialChars ()
		{
			
			// ResourcesNamespace doesnt seem to be validated at all in .NET framework
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			string input, output, expected;
			
			testResources.Add ("akey", "");    		 
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			CodeCompileUnit ccu;
			CodeMemberProperty resourceManager;
			CodeVariableDeclarationStatement cvds;
			
			foreach (char c in specialChars) {
				
				input = "test" + c.ToString ();
				
				expected = input + ".TestClass";
				
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            "TestClass",
				                                            "TestNameSpace",
				                                            input,
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				resourceManager = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("ResourceManager", ccu);
				cvds = ((CodeVariableDeclarationStatement)((CodeConditionStatement)resourceManager.GetStatements[0]).TrueStatements[0]);
				output  = ((CodePrimitiveExpression)((CodeObjectCreateExpression)cvds.InitExpression).Parameters[0]).Value.ToString ();
				
				Assert.AreEqual (expected,output);
			}
		}
		
		
		
	}
}

