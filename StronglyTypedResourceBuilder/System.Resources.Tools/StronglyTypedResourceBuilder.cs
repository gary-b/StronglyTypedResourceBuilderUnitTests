//
// StronglyTypedResourceBuilder.cs
//
// Author:
//	Atsushi Enomoto (atsushi@ximian.com)
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
using System.Reflection;

#if NET_2_0

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel.Design;

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
				generatedCodeNamespaceToUse = codeProvider.CreateValidIdentifier (generatedCodeNamespaceToUse);
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
    		CodeConstructor ctor = new CodeConstructor();
    		ctor.Attributes = MemberAttributes.FamilyAndAssembly; // always internal
    		resType.Members.Add(ctor);
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
			CodeMemberProperty resourceManagerProp = new CodeMemberProperty();
			
    		resourceManagerProp.Name = "ResourceManager";
    		resourceManagerProp.HasGet = true;
    		resourceManagerProp.Type = new CodeTypeReference (typeof (System.Resources.ResourceManager));
    		
		    // accessor
			if (internalClass)
				resourceManagerProp.Attributes = (MemberAttributes.Abstract
												| MemberAttributes.Final
												| MemberAttributes.Assembly);
			else
				resourceManagerProp.Attributes = (MemberAttributes.Abstract
												| MemberAttributes.Final
												| MemberAttributes.FamilyAndAssembly
												| MemberAttributes.FamilyOrAssembly);

			// attributes
			// "System.ComponentModel.EditorBrowsableAttribute", "Advanced","System.ComponentModel.EditorBrowsableState"
			CodeAttributeDeclaration editBrowseAt = new CodeAttributeDeclaration ("System.ComponentModel.EditorBrowsableAttribute",
			                                                                    new CodeAttributeArgument (
																				new CodeFieldReferenceExpression (
																				new CodeTypeReferenceExpression ("System.ComponentModel.EditorBrowsableState"),
																				"Advanced")));

			resourceManagerProp.CustomAttributes.Add (editBrowseAt);
			// getter

			// true statments for check if resourceMan null to go inside getter
			CodeStatement [] trueStatements = new CodeStatement [2];

			trueStatements [0] = new CodeVariableDeclarationStatement (new CodeTypeReference ("System.Resources.ResourceManager"),
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

			unmatchable = null;
			return ccu;
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
	}
}

#endif
