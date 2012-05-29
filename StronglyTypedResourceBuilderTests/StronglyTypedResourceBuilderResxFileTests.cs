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
	public class StronglyTypedResourceBuilderResxFileTests
	{
				
		[Test()]
		public void ResXFileNotFound ()
		{
			// not documented on msdn but throws FileNotFoundException
			
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			//get a valid new filename and then make it not exist
			string resx = Path.GetTempFileName();
			File.Delete (resx);
			
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
				Assert.IsInstanceOf<FileNotFoundException> (ex);
			} finally {
				Assert.IsTrue (exceptionRaised);
			}
		}
		
		[Test()]
		public void ResXFileNotResx ()
		{
			//should throw exception
			
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
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
		
		[Test()]
		public void ResXFilenameEmpty ()
		{
			// in .NET framework throws exception
			
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			string resx = "";
			
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
			}
		}
		
		[Test()]
		public void ResXFilenameInvalid ()
		{
			// in .NET framework throws exception
			
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			string resx = @"C::::\\\\Hello/World";
			
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
			}
		}
		
		[Test()]
		public void ResXFilenameNull ()
		{
			//should throw exception
			
			string[] unmatchables;
			bool exceptionRaised = false;
			CodeCompileUnit ccu;
			
			CSharpCodeProvider provider = new CSharpCodeProvider ();
			
			string resx = null;
			
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
				Assert.IsInstanceOf<ArgumentNullException> (ex);
			} finally {
				Assert.IsTrue (exceptionRaised);
			}
		}
	}
}

