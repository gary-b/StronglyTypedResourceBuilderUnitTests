using NUnit.Framework;
using System;
using System.Resources.Tools;
using Microsoft.CSharp;
using System.IO;
using System.CodeDom;

namespace StronglyTypedResourceBuilderTests {
	[TestFixture]
	public class StronglyTypedResourceBuilderResxFileTests 	{	
		CSharpCodeProvider provider = new CSharpCodeProvider ();
		
		[Test, ExpectedException (typeof (ArgumentException))]
		public void ResXFilenameEmpty ()
		{
			// in .NET framework throws exception
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			string resx = String.Empty;
			
			ccu = StronglyTypedResourceBuilder.Create (resx,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
		[Test, ExpectedException (typeof (ArgumentException))]
		public void ResXFilenameInvalid ()
		{
			// in .NET framework throws exception
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			string resx = @"C::::\\\\Hello/World";
			
			ccu = StronglyTypedResourceBuilder.Create (resx,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
		[Test, ExpectedException (typeof (ArgumentNullException))]
		public void ResXFilenameNull ()
		{
			//should throw exception
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			string resx = null;
			
			ccu = StronglyTypedResourceBuilder.Create (resx,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
		[Test, ExpectedException (typeof (FileNotFoundException))]
		public void ResXFileNotFound ()
		{
			// not documented on msdn but throws FileNotFoundException
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			//get a valid new filename and then make it not exist
			string resx = Path.GetTempFileName ();
			File.Delete (resx);
			
			ccu = StronglyTypedResourceBuilder.Create (resx,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
		}
		
		[Test]
		public void ResXFileNotResx ()
		{
			//***should throw exception but Not using ExpectedException as i want to delete temp file***
			string [] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			string resx = Path.GetTempFileName();
			
			try {
				ccu = StronglyTypedResourceBuilder.Create (resx,
				                                            "TestRes",
				                                            "TestNamespace",
				                                            "TestResourcesNameSpace",
				         									provider,
				                                            true,
				                                            out unmatchables);
			} catch (Exception ex) {
				exceptionRaised = true;
				Assert.IsInstanceOf<ArgumentException> (ex);
			} finally {
				Assert.IsTrue (exceptionRaised);
				File.Delete (resx);
			}
		}
		
	}
}

