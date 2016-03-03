using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.ODataToTypeScript
{
    public sealed class ODataClientCSharpTemplate : ODataClientTemplate
    {
        /// <summary>
        /// Creates an instance of the ODataClientTemplate.
        /// </summary>
        /// <param name="context">The code generation context.</param>
        public ODataClientCSharpTemplate(CodeGenerationContext context)
            : base(context)
        {

        }

        internal override string GlobalPrefix { get { return "global::"; } }
        internal override string SystemTypeTypeName { get { return "global::System.Type"; } }
        internal override string AbstractModifier { get { return " abstract"; } }
        internal override string DataServiceActionQueryTypeName { get { return "global::Microsoft.OData.Client.DataServiceActionQuery"; } }
        internal override string DataServiceActionQuerySingleOfTStructureTemplate { get { return "global::Microsoft.OData.Client.DataServiceActionQuerySingle<{0}>"; } }
        internal override string DataServiceActionQueryOfTStructureTemplate { get { return "global::Microsoft.OData.Client.DataServiceActionQuery<{0}>"; } }
        internal override string NotifyPropertyChangedModifier { get { return "global::System.ComponentModel.INotifyPropertyChanged"; } }
        internal override string ClassInheritMarker { get { return " : "; } }
        internal override string ParameterSeparator { get { return ", \r\n                    "; } }
        internal override string KeyParameterSeparator { get { return ", \r\n            "; } }
        internal override string KeyDictionaryItemSeparator { get { return ", \r\n                "; } }
        //internal override string SystemNullableStructureTemplate { get { return "global::System.Nullable<{0}>"; } }
        internal override string SystemNullableStructureTemplate { get { return "{0}"; } }
        internal override string ICollectionOfTStructureTemplate { get { return "global::System.Collections.Generic.ICollection<{0}>"; } }
        internal override string DataServiceCollectionStructureTemplate { get { return "global::Microsoft.OData.Client.DataServiceCollection<{0}>"; } }
        internal override string DataServiceQueryStructureTemplate { get { return "global::Microsoft.OData.Client.DataServiceQuery<{0}>"; } }
        internal override string DataServiceQuerySingleStructureTemplate { get { return "global::Microsoft.OData.Client.DataServiceQuerySingle<{0}>"; } }
        internal override string ObservableCollectionStructureTemplate { get { return "global::System.Collections.ObjectModel.ObservableCollection<{0}>"; } }
        internal override string ObjectModelCollectionStructureTemplate { get { return "global::System.Collections.ObjectModel.Collection<{0}>"; } }
        internal override string DataServiceCollectionConstructorParameters { get { return "(null, global::Microsoft.OData.Client.TrackingMode.None)"; } }
        internal override string NewModifier { get { return "new "; } }
        internal override string GeoTypeInitializePattern { get { return "global::Microsoft.Spatial.SpatialImplementation.CurrentImplementation.CreateWellKnownTextSqlFormatter(false).Read<{0}>(new global::System.IO.StringReader(\"{1}\"))"; } }
        internal override string Int32TypeName { get { return "int"; } }
        internal override string StringTypeName { get { return "string"; } }
        internal override string BinaryTypeName { get { return "byte[]"; } }
        internal override string DecimalTypeName { get { return "decimal"; } }
        internal override string Int16TypeName { get { return "short"; } }
        internal override string SingleTypeName { get { return "float"; } }
        internal override string BooleanTypeName { get { return "bool"; } }
        internal override string DoubleTypeName { get { return "double"; } }
        internal override string GuidTypeName { get { return "global::System.Guid"; } }
        internal override string ByteTypeName { get { return "byte"; } }
        internal override string Int64TypeName { get { return "long"; } }
        internal override string SByteTypeName { get { return "sbyte"; } }
        internal override string DataServiceStreamLinkTypeName { get { return "global::Microsoft.OData.Client.DataServiceStreamLink"; } }
        internal override string GeographyTypeName { get { return "global::Microsoft.Spatial.Geography"; } }
        internal override string GeographyPointTypeName { get { return "global::Microsoft.Spatial.GeographyPoint"; } }
        internal override string GeographyLineStringTypeName { get { return "global::Microsoft.Spatial.GeographyLineString"; } }
        internal override string GeographyPolygonTypeName { get { return "global::Microsoft.Spatial.GeographyPolygon"; } }
        internal override string GeographyCollectionTypeName { get { return "global::Microsoft.Spatial.GeographyCollection"; } }
        internal override string GeographyMultiPolygonTypeName { get { return "global::Microsoft.Spatial.GeographyMultiPolygon"; } }
        internal override string GeographyMultiLineStringTypeName { get { return "global::Microsoft.Spatial.GeographyMultiLineString"; } }
        internal override string GeographyMultiPointTypeName { get { return "global::Microsoft.Spatial.GeographyMultiPoint"; } }
        internal override string GeometryTypeName { get { return "global::Microsoft.Spatial.Geometry"; } }
        internal override string GeometryPointTypeName { get { return "global::Microsoft.Spatial.GeometryPoint"; } }
        internal override string GeometryLineStringTypeName { get { return "global::Microsoft.Spatial.GeometryLineString"; } }
        internal override string GeometryPolygonTypeName { get { return "global::Microsoft.Spatial.GeometryPolygon"; } }
        internal override string GeometryCollectionTypeName { get { return "global::Microsoft.Spatial.GeometryCollection"; } }
        internal override string GeometryMultiPolygonTypeName { get { return "global::Microsoft.Spatial.GeometryMultiPolygon"; } }
        internal override string GeometryMultiLineStringTypeName { get { return "global::Microsoft.Spatial.GeometryMultiLineString"; } }
        internal override string GeometryMultiPointTypeName { get { return "global::Microsoft.Spatial.GeometryMultiPoint"; } }
        internal override string DateTypeName { get { return "global::Microsoft.OData.Edm.Library.Date"; } }
        internal override string DateTimeOffsetTypeName { get { return "global::System.DateTimeOffset"; } }
        internal override string DurationTypeName { get { return "global::System.TimeSpan"; } }
        internal override string TimeOfDayTypeName { get { return "global::Microsoft.OData.Edm.Library.TimeOfDay"; } }
        internal override string XmlConvertClassName { get { return "global::System.Xml.XmlConvert"; } }
        internal override string EnumTypeName { get { return "global::System.Enum"; } }
        internal override string FixPattern { get { return "@{0}"; } }
        internal override string EnumUnderlyingTypeMarker { get { return " : "; } }
        internal override string ConstantExpressionConstructorWithType { get { return "global::System.Linq.Expressions.Expression.Constant({0}, typeof({1}))"; } }
        internal override string TypeofFormatter { get { return "typeof({0})"; } }
        internal override string UriOperationParameterConstructor { get { return "new global::Microsoft.OData.Client.UriOperationParameter(\"{0}\", {1})"; } }
        internal override string UriEntityOperationParameterConstructor { get { return "new global::Microsoft.OData.Client.UriEntityOperationParameter(\"{0}\", {1}, {2})"; } }
        internal override string BodyOperationParameterConstructor { get { return "new global::Microsoft.OData.Client.BodyOperationParameter(\"{0}\", {1})"; } }
        internal override string BaseEntityType { get { return " : global::Microsoft.OData.Client.BaseEntityType"; } }
        internal override string OverloadsModifier { get { return "new "; } }
        internal override string ODataVersion { get { return "global::Microsoft.OData.Core.ODataVersion.V4"; } }
        internal override string ParameterDeclarationTemplate { get { return "{0} {1}"; } }
        internal override string DictionaryItemConstructor { get { return "{{ {0}, {1} }}"; } }
        internal override HashSet<string> LanguageKeywords
        {
            get
            {
                if (CSharpKeywords == null)
                {
                    CSharpKeywords = new HashSet<string>(StringComparer.Ordinal)
            {
                "abstract", "as", "base", "byte", "bool", "break", "case", "catch", "char", "checked", "class", "const", "continue",
                "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "for",
                "foreach", "finally", "fixed", "float", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
                "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
                "readonly", "ref", "return", "sbyte", "sealed", "string", "short", "sizeof", "stackalloc", "static", "struct", "switch",
                "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "volatile",
                "void", "while"
            };
                }
                return CSharpKeywords;
            }
        }
        private HashSet<string> CSharpKeywords;

        internal override void WriteFileHeader()
        {

            var s = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: " + Environment.Version + @"
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: " + DateTime.Now.ToString(global::System.Globalization.CultureInfo.CurrentCulture);
            GenerationEnvironment.Append(s);            
        }

        internal override void WriteNamespaceStart(string fullNamespace)
        {
            GenerationEnvironment.Append(@" 
            namespace " + fullNamespace + @"
            {
            ");
        }

        internal override void WriteClassStartForEntityContainer(string originalContainerName, string containerName, string fixedContainerName)
        {
            GenerationEnvironment.Append(@"
                /// <summary>
                /// There are no comments for "+ containerName +@" in the schema.
                /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                    [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalContainerName + @""")]
                ");
            }

            GenerationEnvironment.Append(@"
            public partial class " + fixedContainerName + @" : global::Microsoft.OData.Client.DataServiceContext
                {
            ");
        }

        internal override void WriteMethodStartForEntityContainerConstructor(string containerName, string fixedContainerName)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Initialize a new "+ containerName +@" object.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ + T4Version +@""")]
            public "+ fixedContainerName +@"(global::System.Uri serviceRoot) : 
                            base(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
                    {
            ");
        }

        internal override void WriteKeyAsSegmentUrlConvention()
        {
            GenerationEnvironment.Append(@"
                this.UrlConventions = global::Microsoft.OData.Client.DataServiceUrlConventions.KeyAsSegment;
            ");
        }

        internal override void WriteInitializeResolveName()
        {
            GenerationEnvironment.Append(@"
                this.ResolveName = new global::System.Func<global::System.Type, string>(this.ResolveNameFromType);
            ");
        }

        internal override void WriteInitializeResolveType()
        {
            GenerationEnvironment.Append(@"
                this.ResolveType = new global::System.Func<string, global::System.Type>(this.ResolveTypeFromName);
            ");
        }

        internal override void WriteClassEndForEntityContainerConstructor()
        {
            GenerationEnvironment.Append(@"
                this.OnContextCreated();
                this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
                this.Format.UseJson();
            }
            partial void OnContextCreated();
            ");
        }

        internal override void WriteMethodStartForResolveTypeFromName()
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Since the namespace configured for this service reference
                    /// in Visual Studio is different from the one indicated in the
                    /// server schema, use type-mappers to map between the two.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ +T4Version + @""")]
            protected global::System.Type ResolveTypeFromName(string typeName)
            {
            ");
        }

        internal override void WriteResolveNamespace(string typeName, string fullNamespace, string languageDependentNamespace)
        {
            GenerationEnvironment.Append(@"
                        " + typeName +@"resolvedType = this.DefaultResolveType(typeName, """ + fullNamespace + @""", """+ languageDependentNamespace +@""");
                        if ((resolvedType != null))
            {
                return resolvedType;
            }
            ");
        }

        internal override void WriteMethodEndForResolveTypeFromName()
        {
            GenerationEnvironment.Append(@"
                return null;
            }
            ");
        }

        internal override void WritePropertyRootNamespace(string containerName, string fullNamespace)
        {

        }

        internal override void WriteMethodStartForResolveNameFromType(string containerName, string fullNamespace)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Since the namespace configured for this service reference
                    /// in Visual Studio is different from the one indicated in the
                    /// server schema, use type-mappers to map between the two.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+ T4Version+@""")]
        protected string ResolveNameFromType(global::System.Type clientType)
        {
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        global::Microsoft.OData.Client.OriginalNameAttribute originalNameAttribute = (global::Microsoft.OData.Client.OriginalNameAttribute)global::System.Linq.Enumerable.SingleOrDefault(global::Microsoft.OData.Client.Utility.GetCustomAttributes(clientType, typeof(global::Microsoft.OData.Client.OriginalNameAttribute), true));
                ");
            }
        }

        internal override void WriteResolveType(string fullNamespace, string languageDependentNamespace)
        {
            GenerationEnvironment.Append(@"
                if (clientType.Namespace.Equals(""" + languageDependentNamespace+ @""", global::System.StringComparison.Ordinal))
                {
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                            if (originalNameAttribute != null)
                            {
                                return string.Concat(""" + fullNamespace + @"."", originalNameAttribute.OriginalName);
                            }
                ");
            }
            GenerationEnvironment.Append(@"
                    return string.Concat(""" + fullNamespace + @"."", clientType.Name);
                }
            ");
        }

        internal override void WriteMethodEndForResolveNameFromType(bool modelHasInheritance)
        {
            if (this.context.EnableNamingAlias && modelHasInheritance)
            {
                GenerationEnvironment.Append(@"
                        if (originalNameAttribute != null)
                        {
                            return clientType.Namespace + ""."" + originalNameAttribute.OriginalName;
                        }
            ");
            }
            GenerationEnvironment.Append(@"
                return "+ modelHasInheritance.ToString().ToLower() +@" ? ""clientType.FullName"" : ""null"";
                    }
            ");
        }

        internal override void WriteConstructorForSingleType(string singleTypeName, string baseTypeName)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Initialize a new " + singleTypeName + @" object.
                    /// </summary>
                    public " + singleTypeName + @"(global::Microsoft.OData.Client.DataServiceContext context, string path)
                        : base(context, path) {}

                    /// <summary>
                    /// Initialize a new " + singleTypeName + @" object.
                    /// </summary>
                    public " + singleTypeName + @"(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
                        : base(context, path, isComposable) {}

                    /// <summary>
                    /// Initialize a new " + singleTypeName + @" object.
                    /// </summary>
                    public " + singleTypeName + @"(" + baseTypeName + @" query)
                        : base(query) {}

            ");
        }

        internal override void WriteContextEntitySetProperty(string entitySetName, string entitySetFixedName, string originalEntitySetName, string entitySetElementTypeName, bool inContext)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + entitySetName +@" in the schema.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ + T4Version + @""")]
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalEntitySetName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
            public global::Microsoft.OData.Client.DataServiceQuery<"+entitySetElementTypeName+@"> " + entitySetFixedName +@"
                    {
                        get
                        {
            ");
            if (!inContext)
            {
                GenerationEnvironment.Append(@"
                                if (!this.IsComposable)
                                {
                    throw new global::System.NotSupportedException(""The previous function is not composable."");
                }
                ");
            }
            GenerationEnvironment.Append(@"
                            if ((this._" + entitySetName + @" == null))
                            {
                                this._" + entitySetName +" = " + (inContext ? "base" : "Context") +".CreateQuery<" + entitySetElementTypeName +">(" + (inContext ? "\"" + originalEntitySetName + "\"" : "GetPath(\"" + originalEntitySetName + "\")") + @");
                            }
                            return this._"+ entitySetName +@";
                        }
}
[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ + T4Version +@""")]
private global::Microsoft.OData.Client.DataServiceQuery<"+ entitySetElementTypeName +@"> _"+ entitySetName +@";
            ");
        }

        internal override void WriteContextSingletonProperty(string singletonName, string singletonFixedName, string originalSingletonName, string singletonElementTypeName, bool inContext)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for "+ singletonName +@" in the schema.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalSingletonName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
            public " + singletonElementTypeName + @" " + singletonFixedName + @"
                    {
                        get
                        {
            ");
            if (!inContext)
            {
                GenerationEnvironment.Append(@"
                                if (!this.IsComposable)
                                {
                    throw new global::System.NotSupportedException(""The previous function is not composable."");
                }
                ");
            }
            GenerationEnvironment.Append(@"
                            if ((this._" + singletonName + @" == null))
                            {
                                this._"+ singletonName +" = new "+ singletonElementTypeName +"(" +(inContext ? "this" : "this.Context") +", "+ (inContext ? "\"" + originalSingletonName + "\"" : "GetPath(\"" + originalSingletonName + "\")") +@");
                            }
                            return this._" + singletonName + @";
                        }
                    }
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private " + singletonElementTypeName + @" _" + singletonName + @";
            ");
        }

        internal override void WriteContextAddToEntitySetMethod(string entitySetName, string originalEntitySetName, string typeName, string parameterName)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + entitySetName + @" in the schema.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+ @""")]
            public void AddTo" + entitySetName + @"(" + typeName + @" " + parameterName + @")
                    {
                        base.AddObject(""+ originalEntitySetName +@"", " + parameterName + @");
                    }
            ");
        }

        internal override void WriteGeneratedEdmModel(string escapedEdmxString)
        {
            GenerationEnvironment.Append(@"
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private abstract class GeneratedEdmModel
            {
            ");
            if (this.context.ReferencesMap != null)
            {
                GenerationEnvironment.Append(@"
                            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
                    private static global::System.Collections.Generic.Dictionary<string, string> ReferencesMap = new global::System.Collections.Generic.Dictionary<string, string>()
                                {
                ");
                foreach (var reference in this.context.ReferencesMap)
                {
                    GenerationEnvironment.Append(@"
                                        {@""+ reference.Key.OriginalString.Replace("""", """""") +@"", @""+ Utils.SerializeToString(reference.Value).Replace("""", """""") +@""},
                    ");
                }
                GenerationEnvironment.Append(@"
                //                };
                ");
            }
            GenerationEnvironment.Append(@"
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private const string Edmx = @"""+ escapedEdmxString +@""";

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            ");
            if (this.context.ReferencesMap != null)
            {
                GenerationEnvironment.Append(@"
                            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ + T4Version + @""")]
                private static global::System.Xml.XmlReader getReferencedModelFromMap(global::System.Uri uri)
                            {
                    string referencedEdmx;
                    if (ReferencesMap.TryGetValue(uri.OriginalString, out referencedEdmx))
                    {
                        return CreateXmlReader(referencedEdmx);
                    }

                    return null;
                }
                [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
                private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
                            {
                    global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                    try
                    {
                        return global::Microsoft.OData.Edm.Csdl.EdmxReader.Parse(reader, getReferencedModelFromMap);
                    }
                    finally
                    {
                        ((global::System.IDisposable)(reader)).Dispose();
                    }
                }
                ");
            }
            else
            {
                GenerationEnvironment.Append(@"
                            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
                private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
                {
                    global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                    try
                    {
                        return global::Microsoft.OData.Edm.Csdl.EdmxReader.Parse(reader);
                    }
                    finally
                    {
                        ((global::System.IDisposable)(reader)).Dispose();
                    }
                }
                ");
            }
            GenerationEnvironment.Append(@"
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }
                    }
            ");
        }

        internal override void WriteClassEndForEntityContainer()
        {
            GenerationEnvironment.Append(@"
            }
            ");
        }

        internal override void WriteSummaryCommentForStructuredType(string typeName)
        {
            GenerationEnvironment.Append(@"
                /// <summary>
                /// There are no comments for " + typeName + @" in the schema.
                /// </summary>
            ");
        }

        internal override void WriteKeyPropertiesCommentAndAttribute(IEnumerable<string> keyProperties, string keyString)
        {
            GenerationEnvironment.Append(@"
                /// <KeyProperties>
            ");
            foreach (string key in keyProperties)
            {
                GenerationEnvironment.Append(@"
                    /// " + key + @"
                ");
            }
            GenerationEnvironment.Append(@"
                /// </KeyProperties>
                [global::Microsoft.OData.Client.Key("""+ keyString +@""")]
            ");
        }

        internal override void WriteEntityTypeAttribute()
        {
            GenerationEnvironment.Append(@"
                [global::Microsoft.OData.Client.EntityType()]
            ");
        }

        internal override void WriteEntitySetAttribute(string entitySetName)
        {
            GenerationEnvironment.Append(@"
                [global::Microsoft.OData.Client.EntitySet("""+ entitySetName +@""")]
            ");
        }

        internal override void WriteEntityHasStreamAttribute()
        {
            GenerationEnvironment.Append(@"
                [global::Microsoft.OData.Client.HasStream()]
            ");
        }

        internal override void WriteClassStartForStructuredType(string abstractModifier, string typeName, string originalTypeName, string baseTypeName)
        {
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                    [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalTypeName +@""")]
                ");
            }
            if (baseTypeName == BaseEntityType +", "+ this.NotifyPropertyChangedModifier)
            {
                GenerationEnvironment.Append(@"[TypeLite.TsClass]");
            }
            GenerationEnvironment.Append(@"
                public" + abstractModifier + @" partial class " + typeName + @"" + baseTypeName + @"
                {
            ");
        }

        internal override void WriteSummaryCommentForStaticCreateMethod(string typeName)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Create a new " + typeName + @" object.
                    /// </summary>
            ");
        }

        internal override void WriteParameterCommentForStaticCreateMethod(string parameterName, string propertyName)
        {
            GenerationEnvironment.Append(@"
                    /// <param name="""+ parameterName +@""">Initial value of " + propertyName + @".</param>
            ");
        }

        internal override void WriteDeclarationStartForStaticCreateMethod(string typeName, string fixedTypeName)
        {
            GenerationEnvironment.Append(@"
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            public static " + fixedTypeName + @" Create" + typeName + @"(
            ");
        }

        internal override void WriteParameterForStaticCreateMethod(string parameterTypeName, string parameterName, string parameterSeparater)
        {
            GenerationEnvironment.Append(@"
                        " + parameterTypeName + @" " + parameterName + @"" + parameterSeparater + @"
            ");
        }

        internal override void WriteDeclarationEndForStaticCreateMethod(string typeName, string instanceName)
        {
            GenerationEnvironment.Append(@")
                {
                        " + typeName + @" " + instanceName + @" = new " + typeName + @"();
            ");
        }

        internal override void WriteParameterNullCheckForStaticCreateMethod(string parameterName)
        {
            GenerationEnvironment.Append(@"
                if ((" + parameterName + @" == null))
                        {
                    throw new global::System.ArgumentNullException("""+ parameterName +@""");
                }
            ");
        }

        internal override void WritePropertyValueAssignmentForStaticCreateMethod(string instanceName, string propertyName, string parameterName)
        {
            GenerationEnvironment.Append(@"
                        "+ instanceName +@"."+ propertyName +@" = "+ parameterName +@";
            ");
        }

        internal override void WriteMethodEndForStaticCreateMethod(string instanceName)
        {
            GenerationEnvironment.Append(@"
                return " + instanceName + @";
                    }
            ");
        }

        internal override void WritePropertyForStructuredType(string propertyType, string originalPropertyName, string propertyName, string fixedPropertyName, string privatePropertyName, string propertyInitializationValue, bool writeOnPropertyChanged)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for Property " + propertyName + @" in the schema.
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            ");
            if (this.context.EnableNamingAlias || IdentifierMappings.ContainsKey(originalPropertyName))
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalPropertyName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
            public " + propertyType + @" " + fixedPropertyName + @"
                    {
                        get
                        {
                            return this." + privatePropertyName + @";
                        }
            set
                        {
                            this.On" + propertyName + @"Changing(value);
                            this." + privatePropertyName + @" = value;
                            this.On" + propertyName + @"Changed();                        
            ");
            if (writeOnPropertyChanged)
            {
                GenerationEnvironment.Append(@"
                    this.OnPropertyChanged("""+ originalPropertyName +@""");
                ");
            }
            GenerationEnvironment.Append(@"
            }
                    }

                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            private " + propertyType + @" " + privatePropertyName + @"" + (propertyInitializationValue != null ? " = " + propertyInitializationValue : string.Empty) + @";
                    partial void On" + propertyName + @"Changing(" + propertyType + @" value);
                    partial void On" + propertyName + @"Changed();
            ");
        }

        internal override void WriteINotifyPropertyChangedImplementation()
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// This event is raised when the value of the property is changed
                    /// </summary>
                    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """+T4Version+@""")]
            public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            /// <summary>
            /// The value of the property is changed
            /// </summary>
            /// <param name=""property"">property name</param>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""Microsoft.OData.Client.Design.T4"", """ + T4Version + @""")]
            protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
            ");
        }

        internal override void WriteClassEndForStructuredType()
        {
            GenerationEnvironment.Append(@"
            }
            ");
        }

        internal override void WriteEnumFlags()
        {
            GenerationEnvironment.Append(@"
                [global::System.Flags]
            ");
        }

        internal override void WriteSummaryCommentForEnumType(string enumName)
        {
            GenerationEnvironment.Append(@"
                /// <summary>
                /// There are no comments for " + enumName + @" in the schema.
                /// </summary>
            ");
        }

        internal override void WriteEnumDeclaration(string enumName, string originalEnumName, string underlyingType)
        {
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                    [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalEnumName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                public enum " + enumName + @"" + underlyingType + @"
                {
            ");
        }

        internal override void WriteMemberForEnumType(string member, string originalMemberName, bool last)
        {
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalMemberName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                    " + member + (last ? string.Empty : ",") + @"
            ");
        }

        internal override void WriteEnumEnd()
        {
            GenerationEnvironment.Append(@"
            }
            ");
        }

        internal override void WriteFunctionImportReturnCollectionResult(string functionName, string originalFunctionName, string returnTypeName, string parameters, string parameterValues, bool isComposable, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }
            var s = (useEntityReference ? ", bool useEntityReference = false" : string.Empty);
            var s2 = string.IsNullOrEmpty(parameterValues) ? string.Empty : ", ";
            GenerationEnvironment.Append(@"
                    public global::Microsoft.OData.Client.DataServiceQuery<" + returnTypeName + @"> " + functionName + @"(" + parameters + @"" + s + @")
                    {
                        return this.CreateFunctionQuery<" + returnTypeName + @">("", """+ originalFunctionName +@""", " + isComposable.ToString().ToLower() + @"" + s2 + parameterValues + @");
                    }
            ");
        }

        internal override void WriteFunctionImportReturnSingleResult(string functionName, string originalFunctionName, string returnTypeName, string parameters, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }
            var s1 = isReturnEntity ? returnTypeName + this.singleSuffix : string.Format(this.DataServiceQuerySingleStructureTemplate, returnTypeName);
            var s2 = useEntityReference ? ", bool useEntityReference = false" : string.Empty;
            var s3 = "return " + (isReturnEntity ? "new " + returnTypeName + this.singleSuffix + "(" : string.Empty) + "this.CreateFunctionQuerySingle<" + returnTypeName + @">("", " + originalFunctionName + ", " + isComposable.ToString().ToLower() + "" + (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) + ")" + (isReturnEntity ? ")" : string.Empty);
            GenerationEnvironment.Append(@"
                    public " + s1 + @" " + functionName + @"(" + parameters + @"" + s2 + @")
                    {
                        return " + s3 +@";
        }
            ");


        }

        internal override void WriteBoundFunctionInEntityTypeReturnCollectionResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }
            var s1 = hideBaseMethod ? this.OverloadsModifier : string.Empty + @"global::Microsoft.OData.Client.DataServiceQuery<" + returnTypeName + @"> " + functionName + @"(" + parameters + "" + (useEntityReference ? ", bool useEntityReference = false" : string.Empty) + ")";
            var s2 = ">(string.Join(\"/\", global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Skip(requestUri.Segments, this.Context.BaseUri.Segments.Length), s => s.Trim('/'))), \"" + fullNamespace + "."+ originalFunctionName + "\", " + isComposable.ToString().ToLower() + "" + (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues + @"" + (useEntityReference ? ", bool useEntityReference = false" : string.Empty + ");"));
            GenerationEnvironment.Append(@"
                    public " + s1 + @"
                    {
                        global::System.Uri requestUri;
            Context.TryGetUri(this, out requestUri);
                        return this.Context.CreateFunctionQuery<" + returnTypeName + s2 + @"
                    }
            ");
        }

        internal override void WriteBoundFunctionInEntityTypeReturnSingleResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }

            var s1 = "public " +(hideBaseMethod ? this.OverloadsModifier : string.Empty) + " " + (isReturnEntity ? returnTypeName + this.singleSuffix : string.Format(this.DataServiceQuerySingleStructureTemplate, returnTypeName)) + " " + functionName + "("+ parameters +"" + (useEntityReference ? ", bool useEntityReference = false" : string.Empty) + ")";
            var s2 = "return " + (isReturnEntity ? "new " + returnTypeName + this.singleSuffix + "(" : string.Empty) + "this.Context.CreateFunctionQuerySingle<" + returnTypeName + ">(string.Join(\"/\", global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Skip(requestUri.Segments, this.Context.BaseUri.Segments.Length), s => s.Trim('/'))), " + fullNamespace + "." + originalFunctionName + ", " + isComposable.ToString().ToLower() + "" + (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) + ")" + (isReturnEntity ? ")" : string.Empty) + ";";
            GenerationEnvironment.Append(@"
                    " + s1 + @"
                    {
                        global::System.Uri requestUri;
            Context.TryGetUri(this, out requestUri);

                        " + s2 + @"
                    }
            ");
        }

        internal override void WriteActionImport(string actionName, string originalActionName, string returnTypeName, string parameters, string parameterValues)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + actionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalActionName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                    public " + returnTypeName + @" " + actionName + @"(" + parameters + @")
                    {
                        return new " + returnTypeName + @"(this, this.BaseUri.OriginalString.Trim('/') + " +"/"+ @""+ originalActionName +@"" + (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) + @");
                    }
            ");
        }

        internal override void WriteBoundActionInEntityType(bool hideBaseMethod, string actionName, string originalActionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + actionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalActionName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                    public " + (hideBaseMethod ? this.OverloadsModifier : string.Empty) + "" + returnTypeName + " " + actionName + "(" + parameters + @")
                    {
                        global::Microsoft.OData.Client.EntityDescriptor resource = Context.EntityTracker.TryGetEntityDescriptor(this);
                        if (resource == null)
                        {
                            throw new global::System.Exception(""cannot find entity"");
                        }

                        return new "+ returnTypeName +"(this.Context, resource.EditLink.OriginalString.Trim('/') + \"/" + fullNamespace + "."+ originalActionName +"\""+ (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) +@");
                    }
            ");
        }

        internal override void WriteExtensionMethodsStart()
        {
            GenerationEnvironment.Append(@"
                /// <summary>
                /// Class containing all extension methods
                /// </summary>
                public static class ExtensionMethods
            {
            "); 
        }

        internal override void WriteExtensionMethodsEnd()
        {
            GenerationEnvironment.Append(@"
            }
            ");
        }

        internal override void WriteByKeyMethods(string entityTypeName, string returnTypeName, IEnumerable<string> keys, string keyParameters, string keyDictionaryItems)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Get an entity of type " + entityTypeName + @" as " + entityTypeName + this.singleSuffix + @" specified by key from an entity set
                    /// </summary>
                    /// <param name=""source"">source entity set</param>
                    /// <param name=""keys"">dictionary with the names and values of keys</param>
                    public static "+ returnTypeName +@" ByKey(this global::Microsoft.OData.Client.DataServiceQuery<"+ entityTypeName +@"> source, global::System.Collections.Generic.Dictionary<string, object> keys)
        {
            return new "+ returnTypeName +@"(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
        }
                    /// <summary>
                    /// Get an entity of type "+ entityTypeName +@" as "+ entityTypeName + this.singleSuffix +@" specified by key from an entity set
                    /// </summary>
                    /// <param name=""source"">source entity set</param>
            ");
            foreach (var key in keys)
            {
                GenerationEnvironment.Append(@"
                        /// <param name=""+ key +@"">The value of " + key + @"</param>
                ");
            }
            GenerationEnvironment.Append(@"
                    public static " + returnTypeName + @" ByKey(this global::Microsoft.OData.Client.DataServiceQuery<" + entityTypeName + @"> source,
                        " + keyParameters + @")
                    {
                        global::System.Collections.Generic.Dictionary<string, object> keys = new global::System.Collections.Generic.Dictionary<string, object>
                        {
                            " + keyDictionaryItems + @"
                        };
                        return new " + returnTypeName + @"(source.Context, source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)));
                    }
            ");
        }

        internal override void WriteCastToMethods(string baseTypeName, string derivedTypeName, string derivedTypeFullName, string returnTypeName)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// Cast an entity of type " + baseTypeName + @" to its derived type " + derivedTypeFullName + @"
                    /// </summary>
                    /// <param name=""source"">source entity</param>
                    public static "+ returnTypeName +@" CastTo"+ derivedTypeName +@"(this global::Microsoft.OData.Client.DataServiceQuerySingle<"+ baseTypeName +@"> source)
                    {
                        global::Microsoft.OData.Client.DataServiceQuerySingle<"+ derivedTypeFullName +@"> query = source.CastTo < "+ derivedTypeFullName +@" > ();
                        return new "+ returnTypeName +@"(source.Context, query.GetPath(null));
                    }
            ");
        }

        internal override void WriteBoundFunctionReturnSingleResultAsExtension(string functionName, string originalFunctionName, string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                    public static " + (isReturnEntity ? returnTypeName + this.singleSuffix : string.Format(this.DataServiceQuerySingleStructureTemplate, returnTypeName)) + " " + functionName + "(this " + boundTypeName + " source" + (string.IsNullOrEmpty(parameters) ? string.Empty : ", " + parameters) + "" + (useEntityReference ? ", bool useEntityReference = false" : string.Empty) + @")
                    {
                        if (!source.IsComposable)
                        {
                            throw new global::System.NotSupportedException(""The previous function is not composable."");
                        }

                        return "+ (isReturnEntity ? "new " + returnTypeName + this.singleSuffix + "(" : string.Empty +"source.CreateFunctionQuerySingle<"+ returnTypeName +">(\""+ fullNamespace +"."+ originalFunctionName +"\","+ isComposable.ToString().ToLower()) +""+ (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) +")"+ (isReturnEntity ? ")" : string.Empty)+@";
                    }
            ");
        }

        internal override void WriteBoundFunctionReturnCollectionResultAsExtension(string functionName, string originalFunctionName, string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool useEntityReference)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + functionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalFunctionName +@""")]
                ");
            }
            var s1 = string.IsNullOrEmpty(parameters) ? string.Empty : ", " + parameters + "" + (useEntityReference ? ", bool useEntityReference = true" : string.Empty);
            GenerationEnvironment.Append(@"
                    public static global::Microsoft.OData.Client.DataServiceQuery<" + returnTypeName + @"> " + functionName + @"(this " + boundTypeName + @" source" + s1 + @")
                    {
                        if (!source.IsComposable)
                        {
                            throw new global::System.NotSupportedException(""The previous function is not composable."");
                        }

                        return source.CreateFunctionQuery<"+ returnTypeName +@">("""+ fullNamespace +"."+ originalFunctionName +@""", "+ isComposable.ToString().ToLower() +@""+ (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) +@");
                    }
            ");
        }

        internal override void WriteBoundActionAsExtension(string actionName, string originalActionName, string boundSourceType, string returnTypeName, string parameters, string fullNamespace, string parameterValues)
        {
            GenerationEnvironment.Append(@"
                    /// <summary>
                    /// There are no comments for " + actionName + @" in the schema.
                    /// </summary>
            ");
            if (this.context.EnableNamingAlias)
            {
                GenerationEnvironment.Append(@"
                        [global::Microsoft.OData.Client.OriginalNameAttribute("""+ originalActionName +@""")]
                ");
            }
            GenerationEnvironment.Append(@"
                    public static " + returnTypeName + @" " + actionName + @"(this " + boundSourceType + @" source" + (string.IsNullOrEmpty(parameters) ? string.Empty : ", " + parameters) + @")
                    {
                        if (!source.IsComposable)
                        {
                            throw new global::System.NotSupportedException(""The previous function is not composable."");
                        }

                        return new "+ returnTypeName +@"(source.Context, source.AppendRequestUri("""+ fullNamespace +@"."+ originalActionName +@""")"+ (string.IsNullOrEmpty(parameterValues) ? string.Empty : ", " + parameterValues) +@");
                    }
            "); 
        }

        internal override void WriteNamespaceEnd()
        {
            GenerationEnvironment.Append(@"
            }
            ");
        }
    }
}
