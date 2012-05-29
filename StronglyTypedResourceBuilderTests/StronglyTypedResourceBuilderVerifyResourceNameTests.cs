using NUnit.Framework;
using System;
using System.Resources.Tools;
using Microsoft.CSharp;

namespace StronglyTypedResourceBuilderTests
{
	[TestFixture()]
	public class StronglyTypedResourceBuilderVerifyResourceNameTests
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
		public void VerifyResourceNameEmpty ()
		{
			// should return _
			
			string output = StronglyTypedResourceBuilder.VerifyResourceName ("", new CSharpCodeProvider ());
			
			Assert.AreEqual ("_",output);
		}
		
		[Test ()]
		public void VerifyResourceNameSpecialChars ()
		{
			// should replace with _
			
			string input, expected, output;
			
			foreach (char c in specialChars)
			{
				input 	 = string.Format ("{0}a{0}b{0}", c); 
				expected = string.Format ("{0}a{0}b{0}", '_');
				
				output = StronglyTypedResourceBuilder.VerifyResourceName (input,new CSharpCodeProvider());
				
				Assert.AreEqual (expected,output);
			}
		}
		
		[Test ()]
		public void VerifyResourceNameProviderKeywords ()
		{
			// not complete list, doesnt really need to be
						
			string expected, output;
			
			foreach (string input in keywords)
			{
				CSharpCodeProvider provider = new CSharpCodeProvider();
				
				output = StronglyTypedResourceBuilder.VerifyResourceName (input,provider);
				
				expected = provider.CreateValidIdentifier(input);
				
				Assert.AreEqual (expected,output);
			}
		}
		
		[Test ()]
		public void VerifyResourceNameProviderInvalidIdentifiers ()
		{
			// function tests by means of provider.IsValidIdentifier after replaces special chars presumably and returns null
			
			string output;
			
			CSharpCodeProvider provider = new CSharpCodeProvider();
			
			output = StronglyTypedResourceBuilder.VerifyResourceName ("tes$t",provider);
			
			Assert.AreEqual (null,output);
		}
	}
}

