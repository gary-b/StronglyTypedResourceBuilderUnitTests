using NUnit.Framework;
using System;
using System.Resources.Tools;
using Microsoft.CSharp;
using System.IO;
using System.CodeDom;
using System.Resources;
using System.Drawing;

namespace StronglyTypedResourceBuilderTests {
	[TestFixture]
	public class StronglyTypedResourceBuilderResxFileTests 	{	
		CSharpCodeProvider provider = new CSharpCodeProvider ();
		
		[Test, ExpectedException (typeof (ArgumentException))]
		public void ResXFilenameEmpty ()
		{
			// in .NET framework throws exception
			string [] unmatchables;

			string resx = String.Empty;
			
			StronglyTypedResourceBuilder.Create (resx,
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

			string resx = @"C::::\\\\Hello/World";
			
			StronglyTypedResourceBuilder.Create (resx,
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

			string resx = null;
			
			StronglyTypedResourceBuilder.Create (resx,
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

			//get a valid new filename and then make it not exist
			string resx = Path.GetTempFileName ();
			File.Delete (resx);
			
			StronglyTypedResourceBuilder.Create (resx,
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

			string resx = Path.GetTempFileName();
			
			try {
				StronglyTypedResourceBuilder.Create (resx,
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
		
		[Test]
		public void ResxFileProcessed ()
		{
			// resources in resx should be present in codecompileunit with correct property type
			string [] unmatchables;
			CodeCompileUnit ccu;
			
			Bitmap bmp = new Bitmap (100, 100);
			MemoryStream wav = new MemoryStream (1000);
			
			string resxFileName = Path.GetTempFileName();
			
			using (ResXResourceWriter writer = new ResXResourceWriter(resxFileName)) {
				writer.AddResource ("astring", "myvalue"); // dont use key of "string" as its a keyword
				writer.AddResource ("bmp", bmp);
				writer.AddResource ("wav", wav);
				writer.Generate ();
			}
			
			ccu = StronglyTypedResourceBuilder.Create (resxFileName,
			                                            "TestRes",
			                                            "TestNamespace",
			                                            "TestResourcesNameSpace",
			         									provider,
			                                            true,
			                                            out unmatchables);
			
			CodeMemberProperty cmp;
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("astring", ccu);
			Assert.IsNotNull (cmp);
			Assert.AreEqual ("System.String", cmp.Type.BaseType);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("wav", ccu);
			Assert.IsNotNull (cmp);
			Assert.AreEqual ("System.IO.UnmanagedMemoryStream", cmp.Type.BaseType);
			
			cmp = StronglyTypedResourceBuilderCodeDomTest.Get<CodeMemberProperty> ("bmp", ccu);
			Assert.IsNotNull (cmp);
			Assert.AreEqual ("System.Drawing.Bitmap", cmp.Type.BaseType);
			
			wav.Close ();
			File.Delete (resxFileName);
		}
	}
}

