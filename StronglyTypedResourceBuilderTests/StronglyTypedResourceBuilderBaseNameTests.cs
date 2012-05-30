using NUnit.Framework;
using System;
using System.Resources.Tools;
using System.CodeDom;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace StronglyTypedResourceBuilderTests {
	[TestFixture]
	public class StronglyTypedResourceBuilderBaseNameTests	{
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
		Dictionary<string, object> testResources;
		
		[SetUp]
		public void Setup ()
		{
			testResources = new Dictionary<string, object> ();
			testResources.Add ("akey", String.Empty);
		}
		
		[Test]
		public void BaseNameEmpty ()
		{
			// empty class name should change to _
			string [] unmatchables;
			CodeCompileUnit ccu;
			string input, expected;
			
			input = String.Empty;
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            input,
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			expected = "_";
			
			Assert.AreEqual (expected,ccu.Namespaces [0].Types [0].Name);
		}
		
		[Test, ExpectedException (typeof (ArgumentException))]
		public void BaseNameInvalidIdentifier ()
		{
			// identifier invalid after Going through provider.CreateValidIdentifier throw exception in .NET framework
			string [] unmatchables;
			string input;
			
			input = "cla$ss";
			
			StronglyTypedResourceBuilder.Create (testResources,
	                                            input,
	                                            "TestNamespace",
	                                            "TestResourcesNameSpace",
	         									provider,
	                                            true,
	                                            out unmatchables);
		}
		
		[Test]
		public void BaseNameKeywords ()
		{
			// provider.CreateValidIdentifier used to return valid identifier
			string expected;
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			foreach (string input in keywords) {
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = provider.CreateValidIdentifier (input);
				
				Assert.AreEqual (expected,ccu.Namespaces [0].Types [0].Name);
			}
		}
		
		[Test, ExpectedException (typeof (ArgumentNullException))]
		public void BaseNameNull ()
		{
			// should throw exception
			string [] unmatchables;
			string input;
			
			input = null;
			
			StronglyTypedResourceBuilder.Create (testResources,
	                                            input,
	                                            "TestNamespace",
	                                            "TestResourcesNameSpace",
	         									provider,
	                                            true,
	                                            out unmatchables);
		}
		
		[Test]
		public void BaseNameSpecialChars ()
		{
			// StronglyTypedResourceBuilder.VerifyResourceName seems to be used
			string [] unmatchables;
			CodeCompileUnit ccu;
			string input, expected;

			foreach (char c in specialChars) {
				input = c.ToString ();
				
				ccu = StronglyTypedResourceBuilder.Create (testResources,
				                                            input,
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
				
				expected = StronglyTypedResourceBuilder.VerifyResourceName (input, provider);
				
				Assert.AreEqual (expected,ccu.Namespaces [0].Types [0].Name); 
			}
		}
	}
}

