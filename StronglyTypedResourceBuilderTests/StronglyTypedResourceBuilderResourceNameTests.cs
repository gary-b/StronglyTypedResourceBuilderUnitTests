using NUnit.Framework;
using System;
using System.Resources.Tools;
using System.CodeDom;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace StronglyTypedResourceBuilderTests {
	[TestFixture]
	public class StronglyTypedResourceBuilderResourceNameTests	{
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
		
		[Test, ExpectedException (typeof (ArgumentException))]
		public void ResourceNamesCaseSensitiveDupes ()
		{
			// 2 resources with same names in different cases throws exception in .NET framework
			Dictionary<string, object> testResources = new Dictionary<string, object> ();
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			testResources.Add ("FortyTwo", String.Empty);
			testResources.Add ("fortytwo", String.Empty);      		 
			
			ccu = StronglyTypedResourceBuilder.Create (testResources,
			                                            "TestClass",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
		[Test]
		public void ResourceNamesDuplicate_NETBUG ()
		{
			/* 	
			 * DUPES CHECK HAPPENS AFTER VerifyResourceName called which changed eg.
			 *   language keywords have _ appended to start
			 *   string.emtpy converted to _
			 *   various chars replaced
			 * .net inconsistency:
			 * if keys contain multiple single char names made up of invalid chars
			 * which are converted to _, sometimes none are used and all are returned 
			 * in unmatchables, while other times one is used, and other times 
			 * none are used but one is still missing from unmatchables like in this case
			*/
			Dictionary<string, object> testResources = new Dictionary<string, object> ();
			string [] unmatchables;
			
			testResources.Add ("for", "1");
			testResources.Add ("_for", "2"); 
			testResources.Add (String.Empty, String.Empty);
			testResources.Add ("*", String.Empty);
			testResources.Add ("_", String.Empty);
			testResources.Add (".", String.Empty);	
			testResources.Add ("/", String.Empty);	
			testResources.Add ("\\", String.Empty);	
			testResources.Add ("imok", "2");
			
			CodeCompileUnit ccu = StronglyTypedResourceBuilder.Create (testResources,
							                                            "TestRes",
							                                            "TestNamespace",
							                                            "TestResourcesNameSpace",
							         									provider,
							                                            true,
							                                            out unmatchables);
							
			int matchedResources = testResources.Count - unmatchables.Length;
			int membersExpected = matchedResources + 5; // 5 standard members
			
			Assert.AreEqual (membersExpected,ccu.Namespaces [0].Types [0].Members.Count);	
		}
		
		[Test]
		public void ResourceNamesDuplicate ()
		{
			/* 	
			 * DUPES CHECK HAPPENS AFTER VerifyResourceName called which changed eg.
			 *   language keywords have _ appended to start
			 *   string.emtpy converted to _
			 *   various chars replaced
			*/
			Dictionary<string, object> testResources = new Dictionary<string, object> ();
			string [] unmatchables;
			
			testResources.Add ("for", "1");
			testResources.Add ("_for", "2"); 
			testResources.Add ("&", String.Empty);     		
			testResources.Add ("_", String.Empty);
			testResources.Add ("imok", "2");
			
			CodeCompileUnit ccu = StronglyTypedResourceBuilder.Create (testResources,
							                                            "TestRes",
							                                            "TestNamespace",
							                                            "TestResourcesNameSpace",
							         									provider,
							                                            true,
							                                            out unmatchables);
			
			int matchedResources = testResources.Count - unmatchables.Length;
			int membersExpected = matchedResources + 5; // 5 standard members
			Assert.AreEqual (membersExpected,ccu.Namespaces [0].Types [0].Members.Count);	
		}
		
		[Test]
		public void ResourceNamesIgnored ()
		{
			// names beginning with the chars "$" and ">>" ignored and not put in unmatchables
			Dictionary<string, object> testResources = new Dictionary<string, object>();
			string [] unmatchables;
			
			testResources.Add ("$test1", String.Empty);
			testResources.Add ("$test2", String.Empty); 
			testResources.Add (">>test1", String.Empty);     		
			testResources.Add (">>test2", String.Empty);     		 
			testResources.Add ("$", String.Empty);
			testResources.Add (">>", String.Empty);	
			testResources.Add (">", String.Empty);	
			testResources.Add (">test1", String.Empty);
			testResources.Add ("test>>", String.Empty);
			// resource name with $ somwhere else goes into unmatchables as invalid with csharpprovider
			
			CodeCompileUnit ccu = StronglyTypedResourceBuilder.Create (testResources,
							                                            "TestRes",
							                                            "TestNamespace",
							                                            "TestResourcesNameSpace",
							         									provider,
							                                            true,
							                                            out unmatchables);
			
			Assert.AreEqual(0,unmatchables.Length);
			
			Assert.AreEqual (8,ccu.Namespaces [0].Types [0].Members.Count);	// 3 valid + 5 standard
			Assert.IsNotNull (StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("_", ccu));
			Assert.IsNotNull (StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("_test1", ccu));
			Assert.IsNotNull (StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("test__", ccu));
		}
		
		[Test]
		public void ResourceNamesInvalid ()
		{
			// resources named Culture and ResourceManager go into unmatchables (case sensitive)
			// if there is a $ in resource name after char 0 it goes into unmatchables with csharpprovider
			Dictionary<string, object> testResources = new Dictionary<string, object> ();
			string [] unmatchables;
			
			testResources.Add ("ResourceManager", String.Empty);
			testResources.Add ("Culture", String.Empty);
			testResources.Add ("t$est", String.Empty);
			
			CodeCompileUnit ccu = StronglyTypedResourceBuilder.Create (testResources,
							                                            "TestRes",
							                                            "TestNamespace",
							                                            "TestResourcesNameSpace",
							         									provider,
							                                            true,
							                                            out unmatchables);
			
			Assert.AreEqual (3,unmatchables.Length);
			Assert.AreEqual (5,ccu.Namespaces [0].Types [0].Members.Count);	// 5 standard
		}
		
	}
}

