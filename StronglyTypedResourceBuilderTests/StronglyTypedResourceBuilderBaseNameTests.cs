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
	public class StronglyTypedResourceBuilderBaseNameTests
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
		public void BaseNameEmpty ()
		{
			// empty class name should change to _
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			string input, expected;
			
			testResources.Add ("akey", "");    		 
			
			input = "";
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            input,
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			expected = "_";
			
			Assert.AreEqual (expected,ccu.Namespaces[0].Types[0].Name);
		}
		
		[Test ()]
		public void BaseNameInvalidIdentifier ()
		{
			// identifier invalid after Going through provider.CreateValidIdentifier throw exception in .NET framework
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			string input;
			
			testResources.Add ("akey", "");    		 
			
			input = "cla$ss";
			
			try {
			
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
			} catch (Exception ex) {
				exceptionRaised = true;
				Assert.IsInstanceOf<ArgumentException> (ex);
			}
			finally {
				Assert.IsTrue (exceptionRaised,"An exception is expected here");
			}
		}
		
		[Test()]
		public void BaseNameKeywords ()
		{
			// provider.CreateValidIdentifier used to return valid identifier
			
			string expected;
			
			Dictionary<string, object> testResources = new Dictionary<string, object> ();
			string[] unmatchables;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			testResources.Add ("akey", "");    		 
			
			foreach (string input in keywords) {
				
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = provider.CreateValidIdentifier(input);
				
				Assert.AreEqual (expected,ccu.Namespaces[0].Types[0].Name);
			}
		}
		
		[Test ()]
		public void BaseNameNull ()
		{
			// should throw exception
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			string input;
			
			testResources.Add ("akey", "");    		 
			
			input = null;
			
			try {
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
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
		public void BaseNameSpecialChars ()
		{
			// provider.CreateValidIdentifier used
			
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string[] unmatchables;
			CodeCompileUnit ccu;
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			string input, expected;
			
			testResources.Add ("akey", "");    		 
			
			foreach (char c in specialChars) {
				
				input = c.ToString ();
				
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = StronglyTypedResourceBuilder.VerifyResourceName(input, provider);
				
				Assert.AreEqual (expected,ccu.Namespaces[0].Types[0].Name);
			}
		}
	}
}

