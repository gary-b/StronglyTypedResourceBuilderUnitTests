//
// StronglyTypedResourceBuilder.cs
//
// Author:
//	Atsushi Enomoto (atsushi@ximian.com)
//  	Gary B (2012)
// 
// Copyright (C) 2007 Novell, Inc.
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System.IO;


#if NET_2_0

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;
using System.Collections.Generic;

namespace System.Resources.Tools
{
	public static class StronglyTypedResourceBuilder
	{

		static char [] specialChars = { ' ', '\u00A0', '.', ',', ';', '|', '~', '@', '#', '%', '^', '&', 
						'*', '+', '-', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', 
						'}', '\"', '\'', ':', '!'};

		static char [] specialCharsNameSpace = { ' ', '\u00A0', ',', ';', '|', '~', '@', '#', '%', '^', '&', 
						'*', '+', '-', '/', '\\', '<', '>', '?', '[', ']', '(', ')', '{', 
						'}', '\"', '\'', '!'};

		//[MonoTODO]
		public static CodeCompileUnit Create (string resxFile,
						      string baseName,
						      string generatedCodeNamespace,
						      CodeDomProvider codeProvider, bool internalClass,
						      out string [] unmatchable)
		{
			throw new NotImplementedException ();
		}

		//[MonoTODO]
		public static CodeCompileUnit Create (string resxFile,
						      string baseName,
						      string generatedCodeNamespace,
						      string resourcesNamespace,
						      CodeDomProvider codeProvider, bool internalClass,
						      out string [] unmatchable)
		{
			throw new NotImplementedException ();
		}

		//[MonoTODO]
		public static CodeCompileUnit Create (IDictionary resourceList,
						      string baseName,
						      string generatedCodeNamespace,
						      CodeDomProvider codeProvider, bool internalClass,
						      out string [] unmatchable)
		{
			throw new NotImplementedException ();
		}

		//[MonoTODO]
		public static CodeCompileUnit Create (IDictionary resourceList, 
						      string baseName,
						      string generatedCodeNamespace,
						      string resourcesNamespace,
						      CodeDomProvider codeProvider, bool internalClass,
						      out string [] unmatchable)
		{
			string baseNameToUse, generatedCodeNamespaceToUse;
			string resourcesToUse;
			
			// validate ResourcesList
			if (resourceList == null)
				throw new ArgumentNullException ("Parameter resourceList must not be null");
			
			// validate provider
			if (codeProvider == null)
				throw new ArgumentNullException ("Parameter: codeProvider must not be null");
			
			// validate baseName, get baseNameToUse
			if (baseName == null)
				throw new ArgumentNullException ("Parameter: baseName must not be null");

			baseNameToUse = VerifyResourceName (baseName, codeProvider);
			
			if (baseNameToUse == null)
				throw new ArgumentException ("Parameter: baseName is invalid");
			
			// validate generatedCodeNamespace
			if (generatedCodeNamespace == null) {
				generatedCodeNamespaceToUse = "";
			} else {
				generatedCodeNamespaceToUse = CleanNamespaceChars (generatedCodeNamespace);
				generatedCodeNamespaceToUse = codeProvider.CreateValidIdentifier (
											generatedCodeNamespaceToUse);
			}
			
			// validate resourcesNamespace
			if (resourcesNamespace == null)
				resourcesToUse = generatedCodeNamespaceToUse + "." + baseNameToUse;
			else if (resourcesNamespace == String.Empty)
				resourcesToUse = baseNameToUse;
			else
				resourcesToUse = resourcesNamespace + "." + baseNameToUse;
			
			
			// CodeDOM
			
			CodeCompileUnit ccu = new CodeCompileUnit ();
			ccu.ReferencedAssemblies.Add (@"System.dll");

			//namespace
			CodeNamespace nsMain = new CodeNamespace (generatedCodeNamespaceToUse);
			ccu.Namespaces.Add (nsMain);
			nsMain.Imports.Add (new CodeNamespaceImport ("System"));
			
			//class
			CodeTypeDeclaration resType = new CodeTypeDeclaration (baseNameToUse);
			nsMain.Types.Add (resType);
			resType.IsClass = true;
			// set access modifier for class
			if (internalClass)
				resType.TypeAttributes =  TypeAttributes.NotPublic;
			else
				resType.TypeAttributes =  TypeAttributes.Public;
			
			//class CustomAttributes
			
			CodeAttributeDeclaration genCodeAt = new CodeAttributeDeclaration (
											"System.CodeDom.Compiler.GeneratedCodeAttribute");
			CodeAttributeArgument genCodeAtTool = new CodeAttributeArgument (
											new CodePrimitiveExpression (
											"System.Resources.Tools.StronglyTypedResourceBuilder"));
			genCodeAt.Arguments.Add (genCodeAtTool);
			//FIXME: .net always returns 4.0.0.0?
			CodeAttributeArgument genCodeAtVer = new CodeAttributeArgument (
											new CodePrimitiveExpression ("4.0.0.0")); 
			genCodeAt.Arguments.Add (genCodeAtVer);
			resType.CustomAttributes.Add (genCodeAt);
			
			// System.Diagnostics.DebuggerNonUserCodeAttribute
			CodeAttributeDeclaration dbNonUserAt = new CodeAttributeDeclaration (
											"System.Diagnostics.DebuggerNonUserCodeAttribute");
			resType.CustomAttributes.Add (dbNonUserAt);
			// System.Runtime.CompilerServices.CompilerGeneratedAttribute
			CodeAttributeDeclaration compGenAt = new CodeAttributeDeclaration (
											"System.Runtime.CompilerServices.CompilerGeneratedAttribute");
			resType.CustomAttributes.Add (compGenAt);
			
			//resourceMan field
			CodeMemberField resourceManField = new CodeMemberField();
			resourceManField.Attributes = (MemberAttributes.Abstract
							| MemberAttributes.Final
							| MemberAttributes.Assembly
							| MemberAttributes.FamilyOrAssembly);
			resourceManField.Name = "resourceMan";
			resourceManField.Type = new CodeTypeReference (typeof (System.Resources.ResourceManager));
			resType.Members.Add (resourceManField);
			
			//resourceCulture field
			CodeMemberField resourceCultureField = new CodeMemberField();
			resourceCultureField.Attributes = (MemberAttributes.Abstract
							| MemberAttributes.Final
							| MemberAttributes.Assembly
							| MemberAttributes.FamilyOrAssembly);
			resourceCultureField.Name = "resourceCulture";
			resourceCultureField.Type = new CodeTypeReference (typeof (System.Globalization.CultureInfo));
			resType.Members.Add (resourceCultureField);
			
			//constructor
			CodeConstructor ctor = new CodeConstructor ();
			ctor.Attributes = MemberAttributes.FamilyAndAssembly; // always internal
			resType.Members.Add (ctor);
			//constructor CustomAttributes
			CodeAttributeDeclaration supMsgAt = new CodeAttributeDeclaration (
										"System.Diagnostics.CodeAnalysis.SuppressMessageAttribute");
			CodeAttributeArgument supMsgAtCat = new CodeAttributeArgument (
										new CodePrimitiveExpression ("Microsoft.Performance"));
			supMsgAt.Arguments.Add (supMsgAtCat);
			CodeAttributeArgument supMsgAtRul = new CodeAttributeArgument (
										new CodePrimitiveExpression ("CA1811:AvoidUncalledPrivateCode")); 
			supMsgAt.Arguments.Add (supMsgAtRul);
			ctor.CustomAttributes.Add (supMsgAt);
			
			// ResourceManager property
			CodeMemberProperty resourceManagerProp = PropertyBase ("ResourceManager",
			                                                       typeof (System.Resources.ResourceManager),
			                                                       internalClass,
			                                                       true,
			                                                       false);

			// attributes
			// "System.ComponentModel.EditorBrowsableAttribute", "Advanced","System.ComponentModel.EditorBrowsableState"
			CodeAttributeDeclaration editBrowseAt = new CodeAttributeDeclaration ("System.ComponentModel.EditorBrowsableAttribute",
			                                                                new CodeAttributeArgument (
											new CodeFieldReferenceExpression (
											new CodeTypeReferenceExpression (
											"System.ComponentModel.EditorBrowsableState"),
											"Advanced")));
			
			resourceManagerProp.CustomAttributes.Add (editBrowseAt);
			// getter
			
			// true statments for check if resourceMan null to go inside getter
			CodeStatement [] trueStatements = new CodeStatement [2];
			
			trueStatements [0] = new CodeVariableDeclarationStatement (
										new CodeTypeReference (
										"System.Resources.ResourceManager"),
				                                                "temp", new CodeObjectCreateExpression (
										new CodeTypeReference ("System.Resources.ResourceManager"),
										new CodePrimitiveExpression (resourcesToUse),
										new CodePropertyReferenceExpression (
										new CodeTypeOfExpression (baseNameToUse),"Assembly")));

			trueStatements [1] = new CodeAssignStatement ( new CodeFieldReferenceExpression (null, "resourceMan"),
			                                              new CodeVariableReferenceExpression ("temp"));
			
			resourceManagerProp.GetStatements.Add (new CodeConditionStatement (
								new CodeMethodInvokeExpression (
								new CodeMethodReferenceExpression (
								new CodeTypeReferenceExpression ("System.Object"), "Equals"),
								new CodePrimitiveExpression(null),
								new CodeFieldReferenceExpression (
								null,"resourceMan")),trueStatements));
			
			resourceManagerProp.GetStatements.Add (new CodeMethodReturnStatement ( 
			                                       new CodeFieldReferenceExpression ( null,"resourceMan")));
			
			resType.Members.Add(resourceManagerProp);

			// Culture property
			CodeMemberProperty cultureProp = PropertyBase ("Culture",
			                                               typeof (System.Globalization.CultureInfo),
			                                               internalClass,
			                                               true,
			                                               true);
			
			// attributes - same as ResourceManager
			cultureProp.CustomAttributes.Add (editBrowseAt);

			// getter
			cultureProp.GetStatements.Add (new CodeMethodReturnStatement (
							new CodeFieldReferenceExpression (
							null,"resourceCulture")));

			// setter
			cultureProp.SetStatements.Add (new CodeAssignStatement (
				new CodeFieldReferenceExpression (
				null,"resourceCulture"),
				new CodePropertySetValueReferenceExpression ()));

			resType.Members.Add (cultureProp);

			// properties for resources
			//FIXME: is it correct to use OrdinalIgnoreCase here?

			Dictionary<string,ResourceItem> resourceItemDict;
			resourceItemDict = new Dictionary<string,ResourceItem> (StringComparer.OrdinalIgnoreCase);

			//allow ArgumentException to be raised if case insensitive dupes present
			foreach (DictionaryEntry de in resourceList)
				resourceItemDict.Add (de.Key.ToString (), new ResourceItem (de.Value));

			foreach (KeyValuePair<string, ResourceItem> kvp in resourceItemDict) {
				//ignore
				if (kvp.Key.StartsWith(">>") || kvp.Key.StartsWith ("$")) {
					kvp.Value.toIgnore = true;
					continue;
				}
				//invalid names
				if (kvp.Key == "ResourceManager" || kvp.Key == "Culture") {
					kvp.Value.isUnmatchable = true;
					continue;
				}
				// verify name
				kvp.Value.VerifiedKey = VerifyResourceName (kvp.Key, codeProvider);
				// null check
				if (kvp.Value.VerifiedKey == null) {
					kvp.Value.isUnmatchable = true;
					continue;
				}
				//dupe check
				foreach (KeyValuePair<string, ResourceItem> item in resourceItemDict) {
					// skip on encountering kvp or if VerifiedKey on object null (ie hasnt been processed yet)
					if (Object.ReferenceEquals (item.Value, kvp.Value)
					    || item.Value.VerifiedKey == null)
					    continue;
					// if case insensitive dupe found mark both
					if (item.Value.VerifiedKey.ToLower () == kvp.Value.VerifiedKey.ToLower ()) {
						item.Value.isUnmatchable = true;
						kvp.Value.isUnmatchable = true;
					}
				}
			}

			// either create properties for resources, ignore or add to unmatchableList
			List<string> unmatchableList = new List<string> ();

			foreach (KeyValuePair<string, ResourceItem> kvp in resourceItemDict) {
				if (kvp.Value.isUnmatchable)
					unmatchableList.Add (kvp.Key); // orig key
				else if (!kvp.Value.toIgnore) {
						if (kvp.Value.Resource is Stream)
							resType.Members.Add (StreamProperty (kvp.Value.VerifiedKey,
												kvp.Key,
												internalClass));
						else if (kvp.Value.Resource is String)
							resType.Members.Add (StringProperty (kvp.Value.VerifiedKey,
												kvp.Key,
						                                     		internalClass));
						else
							resType.Members.Add (StandardProperty (kvp.Value.VerifiedKey,
												kvp.Key,
												kvp.Value.Resource.GetType (),
												internalClass));
				}
			}


			unmatchable = unmatchableList.ToArray ();

			return ccu;

		}

		static CodeMemberProperty StandardProperty (string propName, string resName, Type propertyType, bool isInternal)
		{

			CodeMemberProperty prop = PropertyBase (propName, propertyType, isInternal, true, false);

			prop.GetStatements.Add (new CodeVariableDeclarationStatement (
						new CodeTypeReference ("System.Object"),
						"obj",
						new CodeMethodInvokeExpression (
						new CodePropertyReferenceExpression (null,"ResourceManager"),
						"GetObject",
						new CodePrimitiveExpression (resName),
						new CodeFieldReferenceExpression (null,"resourceCulture"))));

			prop.GetStatements.Add (new CodeMethodReturnStatement (
						new CodeCastExpression (
						new CodeTypeReference (propertyType),
						new CodeVariableReferenceExpression ("obj"))));

			return prop;
		}

		static CodeMemberProperty StringProperty (string propName, string resName, bool isInternal)
		{
			CodeMemberProperty prop = PropertyBase (propName, typeof (String), isInternal, true, false);

			prop.GetStatements.Add (new CodeMethodReturnStatement (
						new CodeMethodInvokeExpression (
						new CodeMethodReferenceExpression (
						new CodePropertyReferenceExpression (null,"ResourceManager"),
						"GetString"),
						new CodePrimitiveExpression (resName),
						new CodeFieldReferenceExpression (null,"resourceCulture"))));
						
			return prop;
		}

		static CodeMemberProperty StreamProperty (string propName, string resName, bool isInternal)
		{
			CodeMemberProperty prop = PropertyBase (propName, typeof (UnmanagedMemoryStream), isInternal, true, false);

			prop.GetStatements.Add (new CodeMethodReturnStatement (
						new CodeMethodInvokeExpression (
						new CodeMethodReferenceExpression (
						new CodePropertyReferenceExpression (null,"ResourceManager"),
						"GetStream"),
						new CodePrimitiveExpression (resName),
						new CodeFieldReferenceExpression (null,"resourceCulture"))));

			return prop;
		}

		static CodeMemberProperty PropertyBase (string name, Type propertyType, bool isInternal, bool hasGet, bool hasSet)
		{
			CodeMemberProperty prop = new CodeMemberProperty ();

			prop.Name = name;
			prop.Type = new CodeTypeReference (propertyType);
			
			// accessor
			if (isInternal)
				prop.Attributes = (MemberAttributes.Abstract
							| MemberAttributes.Final
							| MemberAttributes.Assembly);
			else
				prop.Attributes = (MemberAttributes.Abstract
							| MemberAttributes.Final
							| MemberAttributes.FamilyAndAssembly
							| MemberAttributes.FamilyOrAssembly);
			
			prop.HasGet = hasGet;
			prop.HasSet = hasSet;
			return prop;
		}

		//[MonoTODO]
		public static string VerifyResourceName (string key, CodeDomProvider provider)
		{
			string keyToUse;
			char [] charKey;

			// check params
			if (key == null)
				throw new ArgumentNullException ("Parameter: key must not be null");
			if (provider == null)
				throw new ArgumentNullException ("Parameter: provider must not be null");

			if (key == String.Empty) {
				// replace "" key with "_", 
				keyToUse = "_";
			} else {
				// replaces special chars
				charKey = key.ToCharArray ();
				for (int i = 0; i < charKey.Length; i++)
					charKey [i] = VerifySpecialChar (charKey [i]);
				keyToUse = new string(charKey);
			}
			// resolve if keyword
			keyToUse = provider.CreateValidIdentifier (keyToUse);
			// check if still not valid for provider
			if (provider.IsValidIdentifier (keyToUse))
				return keyToUse;
			else
				return null;
		}

		static char VerifySpecialChar (char ch)
		{
			for (int i = 0; i < specialChars.Length; i++) {
				if ( specialChars [i] == ch)
					return '_';
			}
			return ch;
		}

		static string CleanNamespaceChars (string name)
		{
			char[] nameChars = name.ToCharArray ();
			for (int i = 0; i < nameChars.Length ;i++) {
				foreach (char c in specialCharsNameSpace) {
					if (nameChars [i] == c)
						nameChars [i] = '_';
				}
			}
			return new string (nameChars);
		}

		class ResourceItem {
			public string VerifiedKey { get;set; }
			public object Resource { get;set; }
			public bool isUnmatchable { get;set; }
			public bool toIgnore { get;set; }

			public ResourceItem (object value)
			{
				Resource = value;
			}
		}

	}
}

#endif
