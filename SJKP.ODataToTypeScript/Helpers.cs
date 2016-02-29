﻿using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Values;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SJKP.ODataToTypeScript
{

    public enum LanguageOption
    {
        /// <summary>Generate code for C# language.</summary>
        CSharp = 0,

        /// <summary>Generate code for Visual Basic language.</summary>
        VB = 1,
    }
    /// <summary>
    /// Context object to provide the model and configuration info to the code generator.
    /// </summary>
    public class CodeGenerationContext
    {
        /// <summary>
        /// The namespace of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        private const string ConventionTermNamespace = "Com.Microsoft.OData.Service.Conventions.V1";

        /// <summary>
        /// The name of the term to use when building value annotations for indicating the conventions used.
        /// </summary>
        private const string ConventionTermName = "UrlConventions";

        /// <summary>
        /// The string value for indicating that the key-as-segment convention is being used in annotations and headers.
        /// </summary>
        private const string KeyAsSegmentConventionName = "KeyAsSegment";

        /// <summary>
        /// The XElement for the edmx
        /// </summary>
        private readonly XElement edmx;

        /// <summary>
        /// The namespacePrefix is used as the only namespace in generated code when there's only one schema in edm model,
        /// and as a prefix for the namespace from the model with multiple schemas. If this argument is null, the
        /// namespaces from the model are used for all types.
        /// </summary>
        private readonly string namespacePrefix;

        /// <summary>
        /// The EdmModel to generate code for.
        /// </summary>
        private IEdmModel edmModel;

        /// <summary>
        /// The array of namespaces in the current edm model.
        /// </summary>
        private string[] namespacesInModel;

        /// <summary>
        /// The array of warnings occured when parsing edm model.
        /// </summary>
        private string[] warnings;

        /// <summary>
        /// true if the model contains any structural type with inheritance, false otherwise.
        /// </summary>
        private bool? modelHasInheritance;

        /// <summary>
        /// If the namespacePrefix is not null, this contains the mapping of namespaces in the model to the corresponding prefixed namespaces.
        /// Otherwise this is an empty dictionary.
        /// </summary>
        private Dictionary<string, string> namespaceMap;

        /// <summary>
        /// Maps the element type of a navigation source to the navigation source.
        /// </summary>
        private Dictionary<IEdmEntityType, List<IEdmNavigationSource>> elementTypeToNavigationSourceMap;

        /// <summary>
        /// HashSet contains the pair of Names and Namespaces of EntityContainers using KeyAsSegment url convention
        /// </summary>
        private HashSet<string> keyAsSegmentContainers;

        /// <summary>
        /// Constructs an instance of <see cref="CodeGenerationContext"/>.
        /// </summary>
        /// <param name="metadataUri">The Uri to the metadata document. The supported scheme are File, http and https.</param>
        public CodeGenerationContext(Uri metadataUri, string namespacePrefix)
            : this(GetEdmxStringFromMetadataPath(metadataUri), namespacePrefix)
        {
        }

        /// <summary>
        /// Constructs an instance of <see cref="CodeGenerationContext"/>.
        /// </summary>
        /// <param name="edmx">The string for the edmx.</param>
        /// <param name="namespacePrefix">The namespacePrefix is used as the only namespace in generated code
        /// when there's only one schema in edm model, and as a prefix for the namespace from the model with multiple
        /// schemas. If this argument is null, the namespaces from the model are used for all types.</param>
        public CodeGenerationContext(string edmx, string namespacePrefix)
        {
            this.edmx = XElement.Parse(edmx);
            this.namespacePrefix = namespacePrefix;
        }

        /// <summary>
        /// The EdmModel to generate code for.
        /// </summary>
        public XElement Edmx
        {
            get { return this.edmx; }
        }

        /// <summary>
        /// The EdmModel to generate code for.
        /// </summary>
        public IEdmModel EdmModel
        {
            get
            {
                if (this.edmModel == null)
                {
                    Debug.Assert(this.edmx != null, "this.edmx != null");

                    IEnumerable<Microsoft.OData.Edm.Validation.EdmError> errors;
                    EdmxReaderSettings edmxReaderSettings = new EdmxReaderSettings()
                    {
                        GetReferencedModelReaderFunc = this.GetReferencedModelReaderFuncWrapper,
                        IgnoreUnexpectedAttributesAndElements = this.IgnoreUnexpectedElementsAndAttributes
                    };
                    if (!EdmxReader.TryParse(this.edmx.CreateReader(ReaderOptions.None), Enumerable.Empty<IEdmModel>(), edmxReaderSettings, out this.edmModel, out errors))
                    {
                        Debug.Assert(errors != null, "errors != null");
                        throw new InvalidOperationException(errors.FirstOrDefault().ErrorMessage);
                    }
                    else if (this.IgnoreUnexpectedElementsAndAttributes)
                    {
                        if (errors != null && errors.Any())
                        {
                            this.warnings = errors.Select(e => e.ErrorMessage).ToArray();
                        }
                    }
                }

                return this.edmModel;
            }
        }

        /// <summary>
        /// The func for user code to overwrite and provide referenced model's XmlReader.
        /// </summary>
        public Func<Uri, XmlReader> GetReferencedModelReaderFunc
        {
            get { return getReferencedModelReaderFunc; }
            set { this.getReferencedModelReaderFunc = value; }
        }

        /// <summary>
        /// Basic setting for XmlReader.
        /// </summary>
        private static readonly XmlReaderSettings settings = new XmlReaderSettings() { IgnoreWhitespace = true };

        /// <summary>
        /// The func for user code to overwrite and provide referenced model's XmlReader.
        /// </summary>
        private Func<Uri, XmlReader> getReferencedModelReaderFunc = uri => XmlReader.Create(GetEdmxStreamFromUri(uri), settings);

        /// <summary>
        /// The Wrapper func for user code to overwrite and provide referenced model's stream.
        /// </summary>
        public Func<Uri, XmlReader> GetReferencedModelReaderFuncWrapper
        {
            get
            {
                return (uri) =>
                {
                    using (XmlReader reader = GetReferencedModelReaderFunc(uri))
                    {
                        if (reader == null)
                        {
                            return null;
                        }

                        XElement element = XElement.Load(reader);
                        if (this.ReferencesMap == null)
                        {
                            this.ReferencesMap = new Dictionary<Uri, XElement>();
                        }

                        this.ReferencesMap.Add(uri, element);
                        return element.CreateReader(ReaderOptions.None);
                    }
                };
            }
        }

        /// <summary>
        /// Dictionary that stores uri and referenced xml mapping.
        /// </summary>
        public Dictionary<Uri, XElement> ReferencesMap
        {
            get;
            set;
        }

        /// <summary>
        /// The array of namespaces in the current edm model.
        /// </summary>
        public string[] NamespacesInModel
        {
            get
            {
                if (this.namespacesInModel == null)
                {
                    Debug.Assert(this.EdmModel != null, "this.EdmModel != null");
                    this.namespacesInModel = GetElementsFromModelTree(this.EdmModel, (m) => m.SchemaElements.Select(e => e.Namespace)).Distinct().ToArray();
                }

                return this.namespacesInModel;
            }
        }

        /// <summary>
        /// The array of warnings occured when parsing edm model.
        /// </summary>
        public string[] Warnings
        {
            get { return this.warnings ?? (this.warnings = new string[] { }); }
        }

        /// <summary>
        /// true if the model contains any structural type with inheritance, false otherwise.
        /// </summary>
        public bool ModelHasInheritance
        {
            get
            {
                if (!this.modelHasInheritance.HasValue)
                {
                    Debug.Assert(this.EdmModel != null, "this.EdmModel != null");
                    this.modelHasInheritance = this.EdmModel.SchemaElementsAcrossModels().OfType<IEdmStructuredType>().Any(t => t.BaseType != null);
                }

                return this.modelHasInheritance.Value;
            }
        }

        /// <summary>
        /// true if we need to generate the ResolveNameFromType method, false otherwise.
        /// </summary>
        public bool NeedResolveNameFromType
        {
            get { return this.ModelHasInheritance || this.NamespaceMap.Count > 0 || this.EnableNamingAlias; }
        }

        /// <summary>
        /// true if we need to generate the ResolveTypeFromName method, false otherwise.
        /// </summary>
        public bool NeedResolveTypeFromName
        {
            get { return this.NamespaceMap.Count > 0 || this.EnableNamingAlias; }
        }

        /// <summary>
        /// If the namespacePrefix is not null, this contains the mapping of namespaces in the model to the corresponding prefixed namespaces.
        /// Otherwise this is an empty dictionary.
        /// </summary>
        public Dictionary<string, string> NamespaceMap
        {
            get
            {
                if (this.namespaceMap == null)
                {
                    if (!string.IsNullOrEmpty(this.namespacePrefix))
                    {
                        if (this.NamespacesInModel.Count() == 1)
                        {
                            IEdmEntityContainer container = this.EdmModel.EntityContainer;
                            string containerNamespace = container == null ? null : container.Namespace;
                            this.namespaceMap = this.NamespacesInModel
                                .Distinct()
                                .ToDictionary(
                                    ns => ns,
                                    ns => ns == containerNamespace ?
                                        this.namespacePrefix :
                                        this.namespacePrefix + "." + (this.EnableNamingAlias ? Customization.CustomizeNamespace(ns) : ns));
                        }
                        else
                        {
                            this.namespaceMap = this.NamespacesInModel
                                .Distinct()
                                .ToDictionary(
                                    ns => ns,
                                    ns => this.namespacePrefix + "." + (this.EnableNamingAlias ? Customization.CustomizeNamespace(ns) : ns));
                        }
                    }
                    else if (this.EnableNamingAlias)
                    {
                        this.namespaceMap = this.NamespacesInModel
                                .Distinct()
                                .ToDictionary(
                                    ns => ns,
                                    ns => Customization.CustomizeNamespace(ns));
                    }
                    else
                    {
                        this.namespaceMap = new Dictionary<string, string>();
                    }
                }

                return this.namespaceMap;
            }
        }

        /// <summary>
        /// true to use DataServiceCollection in the generated code, false otherwise.
        /// </summary>
        public bool UseDataServiceCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies which specific .Net Framework language the generated code will target.
        /// </summary>
        public LanguageOption TargetLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// true to use Upper camel case for all class and property names, false otherwise.
        /// </summary>
        public bool EnableNamingAlias
        {
            get;
            set;
        }

        /// <summary>
        /// true to ignore unknown elements or attributes in metadata, false otherwise.
        /// </summary>
        public bool IgnoreUnexpectedElementsAndAttributes
        {
            get;
            set;
        }

        /// <summary>
        /// Maps the element type of an entity set to the entity set.
        /// </summary>
        public Dictionary<IEdmEntityType, List<IEdmNavigationSource>> ElementTypeToNavigationSourceMap
        {
            get
            {
                return this.elementTypeToNavigationSourceMap ?? (this.elementTypeToNavigationSourceMap = new Dictionary<IEdmEntityType, List<IEdmNavigationSource>>(EqualityComparer<IEdmEntityType>.Default));
            }
        }

        /// <summary>
        /// true if this EntityContainer need to set the UrlConvention to KeyAsSegment, false otherwise.
        /// </summary>
        public bool UseKeyAsSegmentUrlConvention(IEdmEntityContainer currentContainer)
        {
            if (this.keyAsSegmentContainers == null)
            {
                this.keyAsSegmentContainers = new HashSet<string>();
                Debug.Assert(this.EdmModel != null, "this.EdmModel != null");
                IEnumerable<IEdmVocabularyAnnotation> annotations = this.EdmModel.VocabularyAnnotations;
                foreach (IEdmValueAnnotation valueAnnotation in annotations.OfType<IEdmValueAnnotation>())
                {
                    IEdmEntityContainer container = valueAnnotation.Target as IEdmEntityContainer;
                    IEdmValueTerm valueTerm = valueAnnotation.Term as IEdmValueTerm;
                    IEdmStringConstantExpression expression = valueAnnotation.Value as IEdmStringConstantExpression;
                    if (container != null && valueTerm != null && expression != null)
                    {
                        if (valueTerm.Namespace == ConventionTermNamespace &&
                            valueTerm.Name == ConventionTermName &&
                            expression.Value == KeyAsSegmentConventionName)
                        {
                            this.keyAsSegmentContainers.Add(container.FullName());
                        }
                    }
                }
            }

            return this.keyAsSegmentContainers.Contains(currentContainer.FullName());
        }

        /// <summary>
        /// Gets the enumeration of schema elements with the given namespace.
        /// </summary>
        /// <param name="ns">The namespace of the schema elements to get.</param>
        /// <returns>The enumeration of schema elements with the given namespace.</returns>
        public IEnumerable<IEdmSchemaElement> GetSchemaElements(string ns)
        {
            Debug.Assert(ns != null, "ns != null");
            Debug.Assert(this.EdmModel != null, "this.EdmModel != null");
            return GetElementsFromModelTree(this.EdmModel, m => m.SchemaElements.Where(e => e.Namespace == ns));
        }

        /// <summary>
        /// Gets the namespace qualified name for the given <paramref name="schemaElement"/> with the namespace prefix applied if this.NamespacePrefix is specified.
        /// </summary>
        /// <param name="schemaElement">The schema element to get the full name for.</param>
        /// <param name="schemaElementFixedName">The fixed name of this schemaElement.</param>
        /// <param name="template">The current code generate template.</param>
        /// <returns>The namespace qualified name for the given <paramref name="schemaElement"/> with the namespace prefix applied if this.NamespacePrefix is specified.</returns>
        public string GetPrefixedFullName(IEdmSchemaElement schemaElement, string schemaElementFixedName, ODataClientTemplate template, bool needGlobalPrefix = true)
        {
            if (schemaElement == null)
            {
                return null;
            }

            return this.GetPrefixedNamespace(schemaElement.Namespace, template, true, needGlobalPrefix) + "." + schemaElementFixedName;
        }

        /// <summary>
        /// Gets the prefixed namespace for the given <paramref name="ns"/>.
        /// </summary>
        /// <param name="ns">The namespace without the prefix.</param>
        /// <param name="template">The current code generate template.</param>
        /// <param name="needFix">The flag indicates whether the namespace need to be fixed now.</param>
        /// <param name="needGlobalPrefix">The flag indicates whether the namespace need to be added by gloabal prefix.</param>
        /// <returns>The prefixed namespace for the given <paramref name="ns"/>.</returns>
        public string GetPrefixedNamespace(string ns, ODataClientTemplate template, bool needFix, bool needGlobalPrefix)
        {
            if (ns == null)
            {
                return null;
            }

            string prefixedNamespace;
            if (!this.NamespaceMap.TryGetValue(ns, out prefixedNamespace))
            {
                prefixedNamespace = ns;
            }

            if (needFix)
            {
                string[] segments = prefixedNamespace.Split('.');
                prefixedNamespace = string.Empty;
                int n = segments.Length;
                for (int i = 0; i < n; ++i)
                {
                    if (template.LanguageKeywords.Contains(segments[i]))
                    {
                        prefixedNamespace += string.Format(template.FixPattern, segments[i]);
                    }
                    else
                    {
                        prefixedNamespace += segments[i];
                    }

                    prefixedNamespace += (i == n - 1 ? string.Empty : ".");
                }
            }

            if (needGlobalPrefix)
            {
                prefixedNamespace = template.GlobalPrefix + prefixedNamespace;
            }

            return prefixedNamespace;
        }

        /// <summary>
        /// Reads the edmx string from a file path or a http/https path.
        /// </summary>
        /// <param name="metadataUri">The Uri to the metadata document. The supported scheme are File, http and https.</param>
        private static string GetEdmxStringFromMetadataPath(Uri metadataUri)
        {
            string content = null;
            using (StreamReader streamReader = new StreamReader(GetEdmxStreamFromUri(metadataUri)))
            {
                content = streamReader.ReadToEnd();
            }

            return content;
        }

        /// <summary>
        /// Get the metadata stream from a file path or a http/https path.
        /// </summary>
        /// <param name="metadataUri">The Uri to the stream. The supported scheme are File, http and https.</param>
        private static Stream GetEdmxStreamFromUri(Uri metadataUri)
        {
            Debug.Assert(metadataUri != null, "metadataUri != null");
            Stream metadataStream = null;
            if (metadataUri.Scheme == "file")
            {
                metadataStream = new FileStream(Uri.UnescapeDataString(metadataUri.AbsolutePath), FileMode.Open, FileAccess.Read);
            }
            else if (metadataUri.Scheme == "http" || metadataUri.Scheme == "https")
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(metadataUri);
                    WebResponse webResponse = webRequest.GetResponse();
                    metadataStream = webResponse.GetResponseStream();
                }
                catch (WebException e)
                {
                    HttpWebResponse webResponse = e.Response as HttpWebResponse;
                    if (webResponse != null && webResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new WebException("Failed to access the metadata document. The OData service requires authentication for accessing it. Please download the metadata, store it into a local file, and set the value of “MetadataDocumentUri” in the .odata.config file to the file path. After that, run custom tool again to generate the OData Client code.");
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Only file, http, https schemes are supported for paths to metadata source locations.");
            }

            return metadataStream;
        }

        private static IEnumerable<T> GetElementsFromModelTree<T>(IEdmModel mainModel, Func<IEdmModel, IEnumerable<T>> getElementFromOneModelFunc)
        {
            List<T> ret = new List<T>();
            if (mainModel is EdmCoreModel || mainModel.FindDeclaredValueTerm(CoreVocabularyConstants.OptimisticConcurrencyControl) != null)
            {
                return ret;
            }

            ret.AddRange(getElementFromOneModelFunc(mainModel));
            foreach (var tmp in mainModel.ReferencedModels)
            {
                if (tmp is EdmCoreModel ||
                    tmp.FindDeclaredValueTerm(CoreVocabularyConstants.OptimisticConcurrencyControl) != null ||
                    tmp.FindDeclaredValueTerm(CapabilitiesVocabularyConstants.ChangeTracking) != null ||
                    tmp.FindDeclaredValueTerm(AlternateKeysVocabularyConstants.AlternateKeys) != null)
                {
                    continue;
                }

                ret.AddRange(getElementFromOneModelFunc(tmp));
            }

            return ret;
        }
    }

    /// <summary>
    /// The template class to generate the OData client code.
    /// </summary>
    public abstract class ODataClientTemplate : TemplateBase
    {
        protected readonly string singleSuffix = "Single";
        protected const string T4Version = "2.4.0";

        /// <summary>
        /// The code generation context.
        /// </summary>
        protected readonly CodeGenerationContext context;

        /// <summary>
        /// The Dictionary to store identifier mappings when there are duplicate names between properties and Entity/Complex types
        /// </summary>
        protected Dictionary<string, string> IdentifierMappings = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Creates an instance of the ODataClientTemplate.
        /// </summary>
        /// <param name="context">The code generation context.</param>
        public ODataClientTemplate(CodeGenerationContext context)
        {
            this.context = context;
        }

        #region Get Language specific keyword names.
        internal abstract string GlobalPrefix { get; }
        internal abstract string SystemTypeTypeName { get; }
        internal abstract string AbstractModifier { get; }
        internal abstract string DataServiceActionQueryTypeName { get; }
        internal abstract string DataServiceActionQuerySingleOfTStructureTemplate { get; }
        internal abstract string DataServiceActionQueryOfTStructureTemplate { get; }
        internal abstract string NotifyPropertyChangedModifier { get; }
        internal abstract string ClassInheritMarker { get; }
        internal abstract string ParameterSeparator { get; }
        internal abstract string KeyParameterSeparator { get; }
        internal abstract string KeyDictionaryItemSeparator { get; }
        internal abstract string SystemNullableStructureTemplate { get; }
        internal abstract string ICollectionOfTStructureTemplate { get; }
        internal abstract string DataServiceCollectionStructureTemplate { get; }
        internal abstract string DataServiceQueryStructureTemplate { get; }
        internal abstract string DataServiceQuerySingleStructureTemplate { get; }
        internal abstract string ObservableCollectionStructureTemplate { get; }
        internal abstract string ObjectModelCollectionStructureTemplate { get; }
        internal abstract string DataServiceCollectionConstructorParameters { get; }
        internal abstract string NewModifier { get; }
        internal abstract string GeoTypeInitializePattern { get; }
        internal abstract string Int32TypeName { get; }
        internal abstract string StringTypeName { get; }
        internal abstract string BinaryTypeName { get; }
        internal abstract string DecimalTypeName { get; }
        internal abstract string Int16TypeName { get; }
        internal abstract string SingleTypeName { get; }
        internal abstract string BooleanTypeName { get; }
        internal abstract string DoubleTypeName { get; }
        internal abstract string GuidTypeName { get; }
        internal abstract string ByteTypeName { get; }
        internal abstract string Int64TypeName { get; }
        internal abstract string SByteTypeName { get; }
        internal abstract string DataServiceStreamLinkTypeName { get; }
        internal abstract string GeographyTypeName { get; }
        internal abstract string GeographyPointTypeName { get; }
        internal abstract string GeographyLineStringTypeName { get; }
        internal abstract string GeographyPolygonTypeName { get; }
        internal abstract string GeographyCollectionTypeName { get; }
        internal abstract string GeographyMultiPolygonTypeName { get; }
        internal abstract string GeographyMultiLineStringTypeName { get; }
        internal abstract string GeographyMultiPointTypeName { get; }
        internal abstract string GeometryTypeName { get; }
        internal abstract string GeometryPointTypeName { get; }
        internal abstract string GeometryLineStringTypeName { get; }
        internal abstract string GeometryPolygonTypeName { get; }
        internal abstract string GeometryCollectionTypeName { get; }
        internal abstract string GeometryMultiPolygonTypeName { get; }
        internal abstract string GeometryMultiLineStringTypeName { get; }
        internal abstract string GeometryMultiPointTypeName { get; }
        internal abstract string DateTypeName { get; }
        internal abstract string DateTimeOffsetTypeName { get; }
        internal abstract string DurationTypeName { get; }
        internal abstract string TimeOfDayTypeName { get; }
        internal abstract string XmlConvertClassName { get; }
        internal abstract string EnumTypeName { get; }
        internal abstract HashSet<string> LanguageKeywords { get; }
        internal abstract string FixPattern { get; }
        internal abstract string EnumUnderlyingTypeMarker { get; }
        internal abstract string ConstantExpressionConstructorWithType { get; }
        internal abstract string TypeofFormatter { get; }
        internal abstract string UriOperationParameterConstructor { get; }
        internal abstract string UriEntityOperationParameterConstructor { get; }
        internal abstract string BodyOperationParameterConstructor { get; }
        internal abstract string BaseEntityType { get; }
        internal abstract string OverloadsModifier { get; }
        internal abstract string ODataVersion { get; }
        internal abstract string ParameterDeclarationTemplate { get; }
        internal abstract string DictionaryItemConstructor { get; }
        #endregion Get Language specific keyword names.

        #region Language specific write methods.
        internal abstract void WriteFileHeader();
        internal abstract void WriteNamespaceStart(string fullNamespace);
        internal abstract void WriteClassStartForEntityContainer(string originalContainerName, string containerName, string fixedContainerName);
        internal abstract void WriteMethodStartForEntityContainerConstructor(string containerName, string fixedContainerName);
        internal abstract void WriteKeyAsSegmentUrlConvention();
        internal abstract void WriteInitializeResolveName();
        internal abstract void WriteInitializeResolveType();
        internal abstract void WriteClassEndForEntityContainerConstructor();
        internal abstract void WriteMethodStartForResolveTypeFromName();
        internal abstract void WriteResolveNamespace(string typeName, string fullNamespace, string languageDependentNamespace);
        internal abstract void WriteMethodEndForResolveTypeFromName();
        internal abstract void WriteMethodStartForResolveNameFromType(string containerName, string fullNamespace);
        internal abstract void WriteResolveType(string fullNamespace, string languageDependentNamespace);
        internal abstract void WriteMethodEndForResolveNameFromType(bool modelHasInheritance);
        internal abstract void WriteContextEntitySetProperty(string entitySetName, string entitySetFixedName, string originalEntitySetName, string entitySetElementTypeName, bool inContext = true);
        internal abstract void WriteContextSingletonProperty(string singletonName, string singletonFixedName, string originalSingletonName, string singletonElementTypeName, bool inContext = true);
        internal abstract void WriteContextAddToEntitySetMethod(string entitySetName, string originalEntitySetName, string typeName, string parameterName);
        internal abstract void WriteGeneratedEdmModel(string escapedEdmxString);
        internal abstract void WriteClassEndForEntityContainer();
        internal abstract void WriteSummaryCommentForStructuredType(string typeName);
        internal abstract void WriteKeyPropertiesCommentAndAttribute(IEnumerable<string> keyProperties, string keyString);
        internal abstract void WriteEntityTypeAttribute();
        internal abstract void WriteEntitySetAttribute(string entitySetName);
        internal abstract void WriteEntityHasStreamAttribute();
        internal abstract void WriteClassStartForStructuredType(string abstractModifier, string typeName, string originalTypeName, string baseTypeName);
        internal abstract void WriteSummaryCommentForStaticCreateMethod(string typeName);
        internal abstract void WriteParameterCommentForStaticCreateMethod(string parameterName, string propertyName);
        internal abstract void WriteDeclarationStartForStaticCreateMethod(string typeName, string fixedTypeName);
        internal abstract void WriteParameterForStaticCreateMethod(string parameterTypeName, string parameterName, string parameterSeparater);
        internal abstract void WriteDeclarationEndForStaticCreateMethod(string typeName, string instanceName);
        internal abstract void WriteParameterNullCheckForStaticCreateMethod(string parameterName);
        internal abstract void WritePropertyValueAssignmentForStaticCreateMethod(string instanceName, string propertyName, string parameterName);
        internal abstract void WriteMethodEndForStaticCreateMethod(string instanceName);
        internal abstract void WritePropertyForStructuredType(string propertyType, string originalPropertyName, string propertyName, string fixedPropertyName, string privatePropertyName, string propertyInitializationValue, bool writeOnPropertyChanged);
        internal abstract void WriteINotifyPropertyChangedImplementation();
        internal abstract void WriteClassEndForStructuredType();
        internal abstract void WriteNamespaceEnd();
        internal abstract void WriteEnumFlags();
        internal abstract void WriteSummaryCommentForEnumType(string enumName);
        internal abstract void WriteEnumDeclaration(string enumName, string originalEnumName, string underlyingType);
        internal abstract void WriteMemberForEnumType(string member, string originalMemberName, bool last);
        internal abstract void WriteEnumEnd();
        internal abstract void WritePropertyRootNamespace(string containerName, string fullNamespace);
        internal abstract void WriteFunctionImportReturnCollectionResult(string functionName, string originalFunctionName, string returnTypeName, string parameters, string parameterValues, bool isComposable, bool useEntityReference);
        internal abstract void WriteFunctionImportReturnSingleResult(string functionName, string originalFunctionName, string returnTypeName, string parameters, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference);
        internal abstract void WriteBoundFunctionInEntityTypeReturnCollectionResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool useEntityReference);
        internal abstract void WriteBoundFunctionInEntityTypeReturnSingleResult(bool hideBaseMethod, string functionName, string originalFunctionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference);
        internal abstract void WriteActionImport(string actionName, string originalActionName, string returnTypeName, string parameters, string parameterValues);
        internal abstract void WriteBoundActionInEntityType(bool hideBaseMethod, string actionName, string originalActionName, string returnTypeName, string parameters, string fullNamespace, string parameterValues);
        internal abstract void WriteConstructorForSingleType(string singleTypeName, string baseTypeName);
        internal abstract void WriteExtensionMethodsStart();
        internal abstract void WriteExtensionMethodsEnd();
        internal abstract void WriteByKeyMethods(string entityTypeName, string returnTypeName, IEnumerable<string> keys, string keyParameters, string keyDictionaryItems);
        internal abstract void WriteCastToMethods(string baseTypeName, string derivedTypeName, string derivedTypeFullName, string returnTypeName);
        internal abstract void WriteBoundFunctionReturnSingleResultAsExtension(string functionName, string originalFunctionName, string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool isReturnEntity, bool useEntityReference);
        internal abstract void WriteBoundFunctionReturnCollectionResultAsExtension(string functionName, string originalFunctionName, string boundTypeName, string returnTypeName, string parameters, string fullNamespace, string parameterValues, bool isComposable, bool useEntityReference);
        internal abstract void WriteBoundActionAsExtension(string actionName, string originalActionName, string boundSourceType, string returnTypeName, string parameters, string fullNamespace, string parameterValues);
        #endregion Language specific write methods.

        internal HashSet<EdmPrimitiveTypeKind> ClrReferenceTypes
        {
            get
            {
                if (clrReferenceTypes == null)
                {
                    clrReferenceTypes = new HashSet<EdmPrimitiveTypeKind>()
            {
                EdmPrimitiveTypeKind.String, EdmPrimitiveTypeKind.Binary, EdmPrimitiveTypeKind.Geography, EdmPrimitiveTypeKind.Stream,
                EdmPrimitiveTypeKind.GeographyPoint, EdmPrimitiveTypeKind.GeographyLineString, EdmPrimitiveTypeKind.GeographyPolygon,
                EdmPrimitiveTypeKind.GeographyCollection, EdmPrimitiveTypeKind.GeographyMultiPolygon, EdmPrimitiveTypeKind.GeographyMultiLineString,
                EdmPrimitiveTypeKind.GeographyMultiPoint, EdmPrimitiveTypeKind.Geometry, EdmPrimitiveTypeKind.GeometryPoint,
                EdmPrimitiveTypeKind.GeometryLineString, EdmPrimitiveTypeKind.GeometryPolygon, EdmPrimitiveTypeKind.GeometryCollection,
                EdmPrimitiveTypeKind.GeometryMultiPolygon, EdmPrimitiveTypeKind.GeometryMultiLineString, EdmPrimitiveTypeKind.GeometryMultiPoint
            };
                }
                return clrReferenceTypes;
            }
        }
        private HashSet<EdmPrimitiveTypeKind> clrReferenceTypes;

        /// <summary>
        /// Generates code for the OData client.
        /// </summary>
        /// <returns>The generated code for the OData client.</returns>
        public override string TransformText()
        {
            this.WriteFileHeader();
            this.WriteNamespaces();
            return this.GenerationEnvironment.ToString();
        }

        internal void WriteNamespaces()
        {
            foreach (string fullNamespace in context.NamespacesInModel)
            {
                this.WriteNamespace(fullNamespace);
            }
        }

        internal void WriteNamespace(string fullNamespace)
        {
            this.WriteNamespaceStart(this.context.GetPrefixedNamespace(fullNamespace, this, true, false));

            IEdmSchemaElement[] schemaElements = this.context.GetSchemaElements(fullNamespace).ToArray();
            if (schemaElements.OfType<IEdmEntityContainer>().Any())
            {
                IEdmEntityContainer container = schemaElements.OfType<IEdmEntityContainer>().Single();
                this.WriteEntityContainer(container, fullNamespace);
            }

            Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationsMap = new Dictionary<IEdmStructuredType, List<IEdmOperation>>();
            foreach (IEdmOperation operation in schemaElements.OfType<IEdmOperation>())
            {
                if (operation.IsBound)
                {
                    IEdmType edmType = operation.Parameters.First().Type.Definition;
                    IEdmStructuredType edmStructuredType = edmType as IEdmStructuredType;
                    if (edmStructuredType != null)
                    {
                        List<IEdmOperation> operationList;
                        if (!boundOperationsMap.TryGetValue(edmStructuredType, out operationList))
                        {
                            operationList = new List<IEdmOperation>();
                        }

                        operationList.Add(operation);
                        boundOperationsMap[edmStructuredType] = operationList;
                    }
                }
            }

            Dictionary<IEdmStructuredType, List<IEdmStructuredType>> structuredBaseTypeMap = new Dictionary<IEdmStructuredType, List<IEdmStructuredType>>();
            foreach (IEdmSchemaType type in schemaElements.OfType<IEdmSchemaType>())
            {
                IEdmEnumType enumType = type as IEdmEnumType;
                if (enumType != null)
                {
                    this.WriteEnumType(enumType);
                }
                else
                {
                    IEdmComplexType complexType = type as IEdmComplexType;
                    if (complexType != null)
                    {
                        this.WriteComplexType(complexType, boundOperationsMap);
                    }
                    else
                    {
                        IEdmEntityType entityType = type as IEdmEntityType;
                        this.WriteEntityType(entityType, boundOperationsMap);
                    }

                    IEdmStructuredType structuredType = type as IEdmStructuredType;
                    if (structuredType.BaseType != null)
                    {
                        List<IEdmStructuredType> derivedTypes;
                        if (!structuredBaseTypeMap.TryGetValue(structuredType.BaseType, out derivedTypes))
                        {
                            structuredBaseTypeMap[structuredType.BaseType] = new List<IEdmStructuredType>();
                        }

                        structuredBaseTypeMap[structuredType.BaseType].Add(structuredType);
                    }
                }
            }

            if (schemaElements.OfType<IEdmEntityType>().Any() ||
                schemaElements.OfType<IEdmOperation>().Any(o => o.IsBound))
            {
                this.WriteExtensionMethodsStart();
                foreach (IEdmEntityType type in schemaElements.OfType<IEdmEntityType>())
                {
                    string entityTypeName = type.Name;
                    entityTypeName = context.EnableNamingAlias ? Customization.CustomizeNaming(entityTypeName) : entityTypeName;
                    string entityTypeFullName = context.GetPrefixedFullName(type, GetFixedName(entityTypeName), this);
                    string returnTypeName = context.GetPrefixedFullName(type, GetFixedName(entityTypeName + this.singleSuffix), this);

                    var keyProperties = type.Key();
                    if (keyProperties != null && keyProperties.Any())
                    {
                        List<string> keyParameters = new List<string>();
                        List<string> keyDictionaryItems = new List<string>();
                        List<string> keyNames = new List<string>();
                        foreach (IEdmProperty key in keyProperties)
                        {
                            string typeName = Utils.GetClrTypeName(key.Type, this.context.UseDataServiceCollection, this, this.context);
                            string keyName = Utils.CamelCase(key.Name);
                            keyNames.Add(keyName);
                            keyParameters.Add(string.Format(this.ParameterDeclarationTemplate, typeName, this.GetFixedName(keyName)));
                            keyDictionaryItems.Add(string.Format(this.DictionaryItemConstructor, "\"" + key.Name + "\"", this.GetFixedName(keyName)));
                        }

                        string keyParametersString = string.Join(this.KeyParameterSeparator, keyParameters);
                        string keyDictionaryItemsString = string.Join(this.KeyDictionaryItemSeparator, keyDictionaryItems);
                        this.WriteByKeyMethods(entityTypeFullName, returnTypeName, keyNames, keyParametersString, keyDictionaryItemsString);
                    }

                    IEdmEntityType current = (IEdmEntityType)type.BaseType;
                    while (current != null)
                    {
                        string baseTypeName = current.Name;
                        baseTypeName = context.EnableNamingAlias ? Customization.CustomizeNaming(baseTypeName) : baseTypeName;
                        baseTypeName = context.GetPrefixedFullName(current, GetFixedName(baseTypeName), this);
                        this.WriteCastToMethods(baseTypeName, entityTypeName, entityTypeFullName, returnTypeName);
                        current = (IEdmEntityType)current.BaseType;
                    }
                }

                HashSet<string> boundOperations = new HashSet<string>(StringComparer.Ordinal);
                foreach (IEdmFunction function in schemaElements.OfType<IEdmFunction>())
                {
                    if (function.IsBound)
                    {
                        IEdmTypeReference edmTypeReference = function.Parameters.First().Type;
                        string functionName = this.context.EnableNamingAlias ? Customization.CustomizeNaming(function.Name) : function.Name;
                        string parameterString, parameterExpressionString, parameterTypes, parameterValues;
                        bool useEntityReference;
                        this.GetParameterStrings(function.IsBound, false, function.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                        string sourceTypeName = GetSourceOrReturnTypeName(edmTypeReference);
                        sourceTypeName = string.Format(edmTypeReference.IsCollection() ? this.DataServiceQueryStructureTemplate : this.DataServiceQuerySingleStructureTemplate, sourceTypeName);
                        string returnTypeName = GetSourceOrReturnTypeName(function.ReturnType);
                        string fixedFunctionName = GetFixedName(functionName);
                        string func = string.Format("{0}({1},{2})", fixedFunctionName, sourceTypeName, parameterTypes);

                        if (!boundOperations.Contains(func))
                        {
                            boundOperations.Add(func);

                            if (function.ReturnType.IsCollection())
                            {
                                this.WriteBoundFunctionReturnCollectionResultAsExtension(fixedFunctionName, function.Name, sourceTypeName, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, useEntityReference);
                            }
                            else
                            {
                                this.WriteBoundFunctionReturnSingleResultAsExtension(fixedFunctionName, function.Name, sourceTypeName, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, function.ReturnType.IsEntity(), useEntityReference);
                            }
                        }

                        IEdmStructuredType structuredType;
                        if (edmTypeReference.IsCollection())
                        {
                            IEdmCollectionType collectionType = edmTypeReference.Definition as IEdmCollectionType;
                            structuredType = (IEdmStructuredType)collectionType.ElementType.Definition;
                        }
                        else
                        {
                            structuredType = (IEdmStructuredType)edmTypeReference.Definition;
                        }

                        List<IEdmStructuredType> derivedTypes;
                        if (structuredBaseTypeMap.TryGetValue(structuredType, out derivedTypes))
                        {
                            foreach (IEdmStructuredType type in derivedTypes)
                            {
                                IEdmTypeReference derivedTypeReference = new EdmEntityTypeReference((IEdmEntityType)type, true);
                                List<IEdmTypeReference> currentParameters = function.Parameters.Select(p => p.Type).ToList();
                                currentParameters[0] = derivedTypeReference;

                                sourceTypeName = string.Format(edmTypeReference.IsCollection() ? this.DataServiceQueryStructureTemplate : this.DataServiceQuerySingleStructureTemplate, GetSourceOrReturnTypeName(derivedTypeReference));
                                string currentFunc = string.Format("{0}({1},{2})", fixedFunctionName, sourceTypeName, parameterTypes);
                                if (!boundOperations.Contains(currentFunc))
                                {
                                    boundOperations.Add(currentFunc);

                                    if (function.ReturnType.IsCollection())
                                    {
                                        this.WriteBoundFunctionReturnCollectionResultAsExtension(fixedFunctionName, function.Name, sourceTypeName, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, useEntityReference);
                                    }
                                    else
                                    {
                                        this.WriteBoundFunctionReturnSingleResultAsExtension(fixedFunctionName, function.Name, sourceTypeName, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, function.ReturnType.IsEntity(), useEntityReference);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (IEdmAction action in schemaElements.OfType<IEdmAction>())
                {
                    if (action.IsBound)
                    {
                        IEdmTypeReference edmTypeReference = action.Parameters.First().Type;
                        string actionName = this.context.EnableNamingAlias ? Customization.CustomizeNaming(action.Name) : action.Name;
                        string parameterString, parameterExpressionString, parameterTypes, parameterValues;
                        bool useEntityReference;
                        this.GetParameterStrings(action.IsBound, true, action.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                        string sourceTypeName = GetSourceOrReturnTypeName(edmTypeReference);
                        sourceTypeName = string.Format(edmTypeReference.IsCollection() ? this.DataServiceQueryStructureTemplate : this.DataServiceQuerySingleStructureTemplate, sourceTypeName);
                        string returnTypeName;
                        if (action.ReturnType != null)
                        {
                            returnTypeName = GetSourceOrReturnTypeName(action.ReturnType);
                            if (action.ReturnType.IsCollection())
                            {
                                returnTypeName = string.Format(this.DataServiceActionQueryOfTStructureTemplate, returnTypeName);
                            }
                            else
                            {
                                returnTypeName = string.Format(this.DataServiceActionQuerySingleOfTStructureTemplate, returnTypeName);
                            }
                        }
                        else
                        {
                            returnTypeName = this.DataServiceActionQueryTypeName;
                        }

                        string fixedActionName = GetFixedName(actionName);
                        string ac = string.Format("{0}({1},{2})", fixedActionName, sourceTypeName, parameterTypes);
                        if (!boundOperations.Contains(ac))
                        {
                            boundOperations.Add(ac);
                            this.WriteBoundActionAsExtension(fixedActionName, action.Name, sourceTypeName, returnTypeName, parameterString, action.Namespace, parameterValues);
                        }

                        IEdmStructuredType structuredType;
                        if (edmTypeReference.IsCollection())
                        {
                            IEdmCollectionType collectionType = edmTypeReference.Definition as IEdmCollectionType;
                            structuredType = (IEdmStructuredType)collectionType.ElementType.Definition;
                        }
                        else
                        {
                            structuredType = (IEdmStructuredType)edmTypeReference.Definition;
                        }

                        List<IEdmStructuredType> derivedTypes;
                        if (structuredBaseTypeMap.TryGetValue(structuredType, out derivedTypes))
                        {
                            foreach (IEdmStructuredType type in derivedTypes)
                            {
                                IEdmTypeReference derivedTypeReference = new EdmEntityTypeReference((IEdmEntityType)type, true);
                                List<IEdmTypeReference> currentParameters = action.Parameters.Select(p => p.Type).ToList();
                                currentParameters[0] = derivedTypeReference;

                                sourceTypeName = string.Format(edmTypeReference.IsCollection() ? this.DataServiceQueryStructureTemplate : this.DataServiceQuerySingleStructureTemplate, GetSourceOrReturnTypeName(derivedTypeReference));
                                string currentAc = string.Format("{0}({1},{2})", fixedActionName, sourceTypeName, parameterTypes);
                                if (!boundOperations.Contains(currentAc))
                                {
                                    boundOperations.Add(currentAc);
                                    this.WriteBoundActionAsExtension(fixedActionName, action.Name, sourceTypeName, returnTypeName, parameterString, action.Namespace, parameterValues);
                                }
                            }
                        }
                    }
                }

                this.WriteExtensionMethodsEnd();
            }

            this.WriteNamespaceEnd();
        }

        internal bool HasBoundOperations(IEnumerable<IEdmOperation> operations)
        {
            foreach (IEdmOperation opeartion in operations)
            {
                if (opeartion.IsBound)
                {
                    return true;
                }
            }

            return false;
        }

        internal void WriteEntityContainer(IEdmEntityContainer container, string fullNamespace)
        {
            string camelCaseContainerName = container.Name;
            if (this.context.EnableNamingAlias)
            {
                camelCaseContainerName = Customization.CustomizeNaming(camelCaseContainerName);
            }

            this.WriteClassStartForEntityContainer(container.Name, camelCaseContainerName, GetFixedName(camelCaseContainerName));
            this.WriteEntityContainerConstructor(container);

            if (this.context.NeedResolveNameFromType)
            {
                this.WritePropertyRootNamespace(GetFixedName(camelCaseContainerName), this.context.GetPrefixedNamespace(fullNamespace, this, false, false));
            }

            this.WriteResolveTypeFromName();
            this.WriteResolveNameFromType(camelCaseContainerName, fullNamespace);

            foreach (IEdmEntitySet entitySet in container.EntitySets())
            {
                IEdmEntityType entitySetElementType = entitySet.EntityType();
                string entitySetElementTypeName = GetElementTypeName(entitySetElementType, container);

                string camelCaseEntitySetName = entitySet.Name;
                if (this.context.EnableNamingAlias)
                {
                    camelCaseEntitySetName = Customization.CustomizeNaming(camelCaseEntitySetName);
                }

                this.WriteContextEntitySetProperty(camelCaseEntitySetName, GetFixedName(camelCaseEntitySetName), entitySet.Name, GetFixedName(entitySetElementTypeName));
                List<IEdmNavigationSource> edmNavigationSourceList = null;
                if (!this.context.ElementTypeToNavigationSourceMap.TryGetValue(entitySet.EntityType(), out edmNavigationSourceList))
                {
                    edmNavigationSourceList = new List<IEdmNavigationSource>();
                    this.context.ElementTypeToNavigationSourceMap.Add(entitySet.EntityType(), edmNavigationSourceList);
                }

                edmNavigationSourceList.Add(entitySet);
            }

            foreach (IEdmEntitySet entitySet in container.EntitySets())
            {
                IEdmEntityType entitySetElementType = entitySet.EntityType();

                string entitySetElementTypeName = GetElementTypeName(entitySetElementType, container);

                UniqueIdentifierService uniqueIdentifierService = new UniqueIdentifierService(/*IsLanguageCaseSensitive*/true);
                string parameterName = GetFixedName(uniqueIdentifierService.GetUniqueParameterName(entitySetElementType.Name));

                string camelCaseEntitySetName = entitySet.Name;
                if (this.context.EnableNamingAlias)
                {
                    camelCaseEntitySetName = Customization.CustomizeNaming(camelCaseEntitySetName);
                }

                this.WriteContextAddToEntitySetMethod(camelCaseEntitySetName, entitySet.Name, GetFixedName(entitySetElementTypeName), parameterName);
            }

            foreach (IEdmSingleton singleton in container.Singletons())
            {
                IEdmEntityType singletonElementType = singleton.EntityType();
                string singletonElementTypeName = GetElementTypeName(singletonElementType, container);
                string camelCaseSingletonName = singleton.Name;
                if (this.context.EnableNamingAlias)
                {
                    camelCaseSingletonName = Customization.CustomizeNaming(camelCaseSingletonName);
                }

                this.WriteContextSingletonProperty(camelCaseSingletonName, GetFixedName(camelCaseSingletonName), singleton.Name, singletonElementTypeName + "Single");

                List<IEdmNavigationSource> edmNavigationSourceList = null;
                if (this.context.ElementTypeToNavigationSourceMap.TryGetValue(singleton.EntityType(), out edmNavigationSourceList))
                {
                    edmNavigationSourceList.Add(singleton);
                }
            }

            this.WriteGeneratedEdmModel(Utils.SerializeToString(this.context.Edmx).Replace("\"", "\"\""));

            bool hasOperationImport = container.OperationImports().OfType<IEdmOperationImport>().Any();
            foreach (IEdmFunctionImport functionImport in container.OperationImports().OfType<IEdmFunctionImport>())
            {
                string parameterString, parameterTypes, parameterExpressionString, parameterValues;
                bool useEntityReference;
                this.GetParameterStrings(false, false, functionImport.Function.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                string returnTypeName = GetSourceOrReturnTypeName(functionImport.Function.ReturnType);
                string fixedContainerName = this.GetFixedName(functionImport.Container.Name);
                bool isCollectionResult = functionImport.Function.ReturnType.IsCollection();
                string functionImportName = functionImport.Name;
                if (this.context.EnableNamingAlias)
                {
                    functionImportName = Customization.CustomizeNaming(functionImportName);
                    fixedContainerName = Customization.CustomizeNaming(fixedContainerName);
                }

                if (functionImport.Function.ReturnType.IsCollection())
                {
                    this.WriteFunctionImportReturnCollectionResult(this.GetFixedName(functionImportName), functionImport.Name, returnTypeName, parameterString, parameterValues, functionImport.Function.IsComposable, useEntityReference);
                }
                else
                {
                    this.WriteFunctionImportReturnSingleResult(this.GetFixedName(functionImportName), functionImport.Name, returnTypeName, parameterString, parameterValues, functionImport.Function.IsComposable, functionImport.Function.ReturnType.IsEntity(), useEntityReference);
                }
            }

            foreach (IEdmActionImport actionImport in container.OperationImports().OfType<IEdmActionImport>())
            {
                string parameterString, parameterTypes, parameterExpressionString, parameterValues;
                bool useEntityReference;
                this.GetParameterStrings(false, true, actionImport.Action.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                string returnTypeName = null;
                string fixedContainerName = this.GetFixedName(actionImport.Container.Name);

                if (actionImport.Action.ReturnType != null)
                {
                    returnTypeName = GetSourceOrReturnTypeName(actionImport.Action.ReturnType);
                    if (actionImport.Action.ReturnType.IsCollection())
                    {
                        returnTypeName = string.Format(this.DataServiceActionQueryOfTStructureTemplate, returnTypeName);
                    }
                    else
                    {
                        returnTypeName = string.Format(this.DataServiceActionQuerySingleOfTStructureTemplate, returnTypeName);
                    }
                }
                else
                {
                    returnTypeName = this.DataServiceActionQueryTypeName;
                }

                string actionImportName = actionImport.Name;
                if (this.context.EnableNamingAlias)
                {
                    actionImportName = Customization.CustomizeNaming(actionImportName);
                    fixedContainerName = Customization.CustomizeNaming(fixedContainerName);
                }

                this.WriteActionImport(this.GetFixedName(actionImportName), actionImport.Name, returnTypeName, parameterString, parameterValues);
            }

            this.WriteClassEndForEntityContainer();
        }

        internal void WriteEntityContainerConstructor(IEdmEntityContainer container)
        {
            string camelCaseContainerName = container.Name;
            if (this.context.EnableNamingAlias)
            {
                camelCaseContainerName = Customization.CustomizeNaming(camelCaseContainerName);
            }

            this.WriteMethodStartForEntityContainerConstructor(camelCaseContainerName, GetFixedName(camelCaseContainerName));

            if (this.context.UseKeyAsSegmentUrlConvention(container))
            {
                this.WriteKeyAsSegmentUrlConvention();
            }

            if (this.context.NeedResolveNameFromType)
            {
                this.WriteInitializeResolveName();
            }

            if (this.context.NeedResolveTypeFromName)
            {
                this.WriteInitializeResolveType();
            }

            this.WriteClassEndForEntityContainerConstructor();
        }

        internal void WriteResolveTypeFromName()
        {
            if (!this.context.NeedResolveTypeFromName)
            {
                return;
            }

            this.WriteMethodStartForResolveTypeFromName();

            // NOTE: since multiple namespaces can have the same prefix and match the namespace
            // prefix condition, it's important that the prefix check is done is prefix-length
            // order, starting with the longest prefix.
            IEnumerable<KeyValuePair<string, string>> namespaceToPrefixedNamespacePairs = this.context.NamespaceMap.OrderByDescending(p => p.Key.Length).ThenBy(p => p.Key);

            string typeName = this.SystemTypeTypeName + " ";
            foreach (KeyValuePair<string, string> namespaceToPrefixedNamespacePair in namespaceToPrefixedNamespacePairs)
            {
                this.WriteResolveNamespace(typeName, namespaceToPrefixedNamespacePair.Key, namespaceToPrefixedNamespacePair.Value);
                typeName = string.Empty;
            }

            this.WriteMethodEndForResolveTypeFromName();
        }

        internal void WriteResolveNameFromType(string containerName, string fullNamespace)
        {
            if (!this.context.NeedResolveNameFromType)
            {
                return;
            }

            this.WriteMethodStartForResolveNameFromType(GetFixedName(containerName), fullNamespace);

            // NOTE: in this case order also matters, but the length of the CLR
            // namespace is what needs to be considered.
            IEnumerable<KeyValuePair<string, string>> namespaceToPrefixedNamespacePairs = this.context.NamespaceMap.OrderByDescending(p => p.Value.Length).ThenBy(p => p.Key);

            foreach (KeyValuePair<string, string> namespaceToPrefixedNamespacePair in namespaceToPrefixedNamespacePairs)
            {
                this.WriteResolveType(namespaceToPrefixedNamespacePair.Key, namespaceToPrefixedNamespacePair.Value);
            }

            this.WriteMethodEndForResolveNameFromType(this.context.ModelHasInheritance);
        }

        internal void WritePropertiesForSingleType(IEnumerable<IEdmProperty> properties)
        {
            foreach (IEdmProperty property in properties.Where(i => i.PropertyKind == EdmPropertyKind.Navigation))
            {
                string propertyType;
                string propertyName = this.context.EnableNamingAlias ? Customization.CustomizeNaming(property.Name) : property.Name;
                if (property.Type is Microsoft.OData.Edm.Library.EdmCollectionTypeReference)
                {
                    propertyType = GetSourceOrReturnTypeName(property.Type);
                    WriteContextEntitySetProperty(propertyName, GetFixedName(propertyName), property.Name, propertyType, false);
                }
                else
                {
                    propertyType = Utils.GetClrTypeName(property.Type, true, this, this.context, true);
                    WriteContextSingletonProperty(propertyName, GetFixedName(propertyName), property.Name, propertyType + "Single", false);
                }
            }
        }

        internal void WriteEntityType(IEdmEntityType entityType, Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationsMap)
        {
            string entityTypeName = ((IEdmSchemaElement)entityType).Name;
            entityTypeName = this.context.EnableNamingAlias ? Customization.CustomizeNaming(entityTypeName) : entityTypeName;
            this.WriteSummaryCommentForStructuredType(entityTypeName + this.singleSuffix);
            this.WriteStructurdTypeDeclaration(entityType,
                this.ClassInheritMarker + string.Format(this.DataServiceQuerySingleStructureTemplate, GetFixedName(entityTypeName)),
                this.singleSuffix);
            string singleTypeName = (this.context.EnableNamingAlias ?
                Customization.CustomizeNaming(((IEdmSchemaElement)entityType).Name) : ((IEdmSchemaElement)entityType).Name) + this.singleSuffix;
            this.WriteConstructorForSingleType(GetFixedName(singleTypeName), string.Format(this.DataServiceQuerySingleStructureTemplate, GetFixedName(entityTypeName)));
            IEdmEntityType current = entityType;
            while (current != null)
            {
                this.WritePropertiesForSingleType(current.DeclaredProperties);
                current = (IEdmEntityType)current.BaseType;
            }

            this.WriteClassEndForStructuredType();

            this.WriteSummaryCommentForStructuredType(this.context.EnableNamingAlias ? Customization.CustomizeNaming(entityType.Name) : entityType.Name);

            if (entityType.Key().Any())
            {
                IEnumerable<string> keyProperties = entityType.Key().Select(k => k.Name);
                this.WriteKeyPropertiesCommentAndAttribute(
                    this.context.EnableNamingAlias ? keyProperties.Select(k => Customization.CustomizeNaming(k)) : keyProperties,
                    string.Join("\", \"", keyProperties));
            }
            else
            {
                this.WriteEntityTypeAttribute();
            }

            if (this.context.UseDataServiceCollection)
            {
                List<IEdmNavigationSource> navigationSourceList;
                if (this.context.ElementTypeToNavigationSourceMap.TryGetValue(entityType, out navigationSourceList))
                {
                    if (navigationSourceList.Count == 1)
                    {
                        this.WriteEntitySetAttribute(navigationSourceList[0].Name);
                    }
                }
            }

            if (entityType.HasStream)
            {
                this.WriteEntityHasStreamAttribute();
            }

            this.WriteStructurdTypeDeclaration(entityType, this.BaseEntityType);
            this.SetPropertyIdentifierMappingsIfNameConflicts(entityType.Name, entityType);
            this.WriteTypeStaticCreateMethod(entityType.Name, entityType);
            this.WritePropertiesForStructuredType(entityType.DeclaredProperties);

            if (entityType.BaseType == null && this.context.UseDataServiceCollection)
            {
                this.WriteINotifyPropertyChangedImplementation();
            }

            this.WriteBoundOperations(entityType, boundOperationsMap);

            this.WriteClassEndForStructuredType();
        }

        internal void WriteComplexType(IEdmComplexType complexType, Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationsMap)
        {
            this.WriteSummaryCommentForStructuredType(this.context.EnableNamingAlias ? Customization.CustomizeNaming(complexType.Name) : complexType.Name);
            this.WriteStructurdTypeDeclaration(complexType, string.Empty);
            this.SetPropertyIdentifierMappingsIfNameConflicts(complexType.Name, complexType);
            this.WriteTypeStaticCreateMethod(complexType.Name, complexType);
            this.WritePropertiesForStructuredType(complexType.DeclaredProperties);

            if (complexType.BaseType == null && this.context.UseDataServiceCollection)
            {
                this.WriteINotifyPropertyChangedImplementation();
            }

            this.WriteClassEndForStructuredType();
        }

        internal void WriteBoundOperations(IEdmStructuredType structuredType, Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationsMap)
        {
            List<IEdmOperation> operations;
            if (boundOperationsMap.TryGetValue(structuredType, out operations))
            {
                foreach (IEdmFunction function in operations.OfType<IEdmFunction>())
                {
                    string parameterString, parameterExpressionString, parameterTypes, parameterValues;
                    bool useEntityReference;
                    bool hideBaseMethod = this.CheckMethodsInBaseClass(structuredType.BaseType, function, boundOperationsMap);
                    this.GetParameterStrings(function.IsBound, false, function.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                    string returnTypeName = GetSourceOrReturnTypeName(function.ReturnType);
                    string functionName = function.Name;
                    if (this.context.EnableNamingAlias)
                    {
                        functionName = Customization.CustomizeNaming(functionName);
                    }

                    if (function.ReturnType.IsCollection())
                    {
                        this.WriteBoundFunctionInEntityTypeReturnCollectionResult(hideBaseMethod, GetFixedName(functionName), function.Name, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, useEntityReference);
                    }
                    else
                    {
                        this.WriteBoundFunctionInEntityTypeReturnSingleResult(hideBaseMethod, GetFixedName(functionName), function.Name, returnTypeName, parameterString, function.Namespace, parameterValues, function.IsComposable, function.ReturnType.IsEntity(), useEntityReference);
                    }
                }

                foreach (IEdmAction action in operations.OfType<IEdmAction>())
                {
                    string parameterString, parameterExpressionString, parameterTypes, parameterValues;
                    bool useEntityReference;
                    bool hideBaseMethod = this.CheckMethodsInBaseClass(structuredType.BaseType, action, boundOperationsMap);
                    this.GetParameterStrings(action.IsBound, true, action.Parameters.ToArray(), out parameterString, out parameterTypes, out parameterExpressionString, out parameterValues, out useEntityReference);
                    string returnTypeName;
                    if (action.ReturnType != null)
                    {
                        returnTypeName = GetSourceOrReturnTypeName(action.ReturnType);
                        if (action.ReturnType.IsCollection())
                        {
                            returnTypeName = string.Format(this.DataServiceActionQueryOfTStructureTemplate, returnTypeName);
                        }
                        else
                        {
                            returnTypeName = string.Format(this.DataServiceActionQuerySingleOfTStructureTemplate, returnTypeName);
                        }
                    }
                    else
                    {
                        returnTypeName = this.DataServiceActionQueryTypeName;
                    }

                    string actionName = action.Name;
                    if (this.context.EnableNamingAlias)
                    {
                        actionName = Customization.CustomizeNaming(actionName);
                    }

                    this.WriteBoundActionInEntityType(hideBaseMethod, GetFixedName(actionName), action.Name, returnTypeName, parameterString, action.Namespace, parameterValues);
                }
            }
        }

        internal bool CheckMethodsInBaseClass(IEdmStructuredType structuredType, IEdmOperation operation, Dictionary<IEdmStructuredType, List<IEdmOperation>> boundOperationsMap)
        {
            if (structuredType != null)
            {
                List<IEdmOperation> operations;
                if (boundOperationsMap.TryGetValue(structuredType, out operations))
                {
                    foreach (IEdmOperation op in operations)
                    {
                        if (this.context.TargetLanguage == LanguageOption.VB)
                        {
                            if (operation.Name == op.Name)
                            {
                                return true;
                            }
                        }

                        List<IEdmOperationParameter> targetParameter = operation.Parameters.ToList();
                        List<IEdmOperationParameter> checkParameter = op.Parameters.ToList();
                        if (operation.Name == op.Name && targetParameter.Count == checkParameter.Count)
                        {
                            bool areSame = true;
                            for (int i = 1; i < targetParameter.Count; ++i)
                            {
                                var targetParameterType = targetParameter[i].Type;
                                var checkParameterType = checkParameter[i].Type;
                                if (!targetParameterType.Definition.Equals(checkParameterType.Definition)
                                    || targetParameterType.IsNullable != checkParameterType.IsNullable)
                                {
                                    areSame = false;
                                    break;
                                }
                            }

                            if (areSame)
                            {
                                return true;
                            }
                        }
                    }
                }

                return CheckMethodsInBaseClass(structuredType.BaseType, operation, boundOperationsMap);
            }

            return false;
        }

        internal void WriteEnumType(IEdmEnumType enumType)
        {
            this.WriteSummaryCommentForEnumType(this.context.EnableNamingAlias ? Customization.CustomizeNaming(enumType.Name) : enumType.Name);
            if (enumType.IsFlags)
            {
                this.WriteEnumFlags();
            }

            string underlyingType = string.Empty;
            if (enumType.UnderlyingType != null && enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.Int32)
            {
                underlyingType = Utils.GetClrTypeName(enumType.UnderlyingType, this);
                underlyingType = this.EnumUnderlyingTypeMarker + underlyingType;
            }

            this.WriteEnumDeclaration(this.context.EnableNamingAlias ? GetFixedName(Customization.CustomizeNaming(enumType.Name)) : GetFixedName(enumType.Name), enumType.Name, underlyingType);
            this.WriteMembersForEnumType(enumType.Members);
            this.WriteEnumEnd();
        }

        internal void WriteStructurdTypeDeclaration(IEdmStructuredType structuredType, string baseEntityType, string typeNameSuffix = null)
        {
            string abstractModifier = structuredType.IsAbstract && typeNameSuffix == null ? this.AbstractModifier : string.Empty;
            string baseTypeName = baseEntityType;

            if (typeNameSuffix == null)
            {
                if (structuredType.BaseType == null)
                {
                    if (this.context.UseDataServiceCollection)
                    {
                        if (this.context.TargetLanguage == LanguageOption.CSharp)
                        {
                            baseTypeName += string.IsNullOrEmpty(baseTypeName) ? this.ClassInheritMarker : ", ";
                        }

                        baseTypeName += this.NotifyPropertyChangedModifier;
                    }
                }
                else
                {
                    IEdmSchemaElement baseType = (IEdmSchemaElement)structuredType.BaseType;
                    string baseTypeFixedName = this.context.EnableNamingAlias ? GetFixedName(Customization.CustomizeNaming(baseType.Name)) : GetFixedName(baseType.Name);
                    baseTypeName = ((IEdmSchemaElement)structuredType).Namespace == baseType.Namespace ? baseTypeFixedName : this.context.GetPrefixedFullName(baseType, baseTypeFixedName, this);
                    baseTypeName = this.ClassInheritMarker + baseTypeName;
                }
            }

            string structuredTypeName = this.context.EnableNamingAlias ?
                Customization.CustomizeNaming(((IEdmSchemaElement)structuredType).Name) : ((IEdmSchemaElement)structuredType).Name;
            this.WriteClassStartForStructuredType(abstractModifier, GetFixedName(structuredTypeName + typeNameSuffix), ((IEdmSchemaElement)structuredType).Name + typeNameSuffix, baseTypeName);
        }

        internal string GetSourceOrReturnTypeName(IEdmTypeReference typeReference)
        {
            IEdmCollectionType edmCollectionType = typeReference.Definition as IEdmCollectionType;
            bool addNullableTemplate = true;
            if (edmCollectionType != null)
            {
                typeReference = edmCollectionType.ElementType;
                addNullableTemplate = false;
            }

            return Utils.GetClrTypeName(typeReference, this.context.UseDataServiceCollection, this, this.context, addNullableTemplate);
        }

        internal void GetParameterStrings(bool isBound, bool isAction, IEdmOperationParameter[] parameters, out string parameterString, out string parameterTypes, out string parameterExpressionString, out string parameterValues, out bool useEntityReference)
        {
            parameterString = string.Empty;
            parameterExpressionString = string.Empty;
            parameterTypes = string.Empty;
            parameterValues = string.Empty;
            useEntityReference = false;

            int n = parameters.Count();
            for (int i = isBound ? 1 : 0; i < n; ++i)
            {
                IEdmOperationParameter param = parameters[i];
                if (i == (isBound ? 1 : 0))
                {
                    if (this.context.TargetLanguage == LanguageOption.CSharp)
                    {
                        parameterExpressionString += "\r\n                        ";
                    }
                    else
                    {
                        parameterExpressionString += "\r\n                            ";
                    }
                }

                string typeName = Utils.GetClrTypeName(param.Type, this.context.UseDataServiceCollection, this, this.context, true, true, true);
                if (this.context.TargetLanguage == LanguageOption.CSharp)
                {
                    parameterString += typeName;
                    parameterString += (" " + GetFixedName(param.Name));
                }
                else if (this.context.TargetLanguage == LanguageOption.VB)
                {
                    parameterString += GetFixedName(param.Name);
                    parameterString += (this.EnumUnderlyingTypeMarker + typeName);
                }

                parameterString += i == n - 1 ? string.Empty : ", ";
                parameterTypes += string.Format(this.TypeofFormatter, typeName) + ", ";
                parameterExpressionString += this.GetParameterExpressionString(param, typeName) + ", ";

                if (i != (isBound ? 1 : 0))
                {
                    parameterValues += ",\r\n                    ";
                }

                if (isAction)
                {
                    parameterValues += string.Format(this.BodyOperationParameterConstructor, param.Name, GetFixedName(param.Name));
                }
                else if (param.Type.IsEntity() || (param.Type.IsCollection() && param.Type.AsCollection().ElementType().IsEntity()))
                {
                    useEntityReference = true;
                    parameterValues += string.Format(this.UriEntityOperationParameterConstructor, param.Name, GetFixedName(param.Name), "useEntityReference");
                }
                else
                {
                    parameterValues += string.Format(this.UriOperationParameterConstructor, param.Name, GetFixedName(param.Name));
                }
            }
        }

        internal string GetParameterExpressionString(IEdmOperationParameter param, string typeName)
        {
            string clrTypeName;
            IEdmType edmType = param.Type.Definition;
            IEdmPrimitiveType edmPrimitiveType = edmType as IEdmPrimitiveType;
            if (edmPrimitiveType != null)
            {
                clrTypeName = Utils.GetClrTypeName(edmPrimitiveType, this);
                if (param.Type.IsNullable && !this.ClrReferenceTypes.Contains(edmPrimitiveType.PrimitiveKind))
                {
                    clrTypeName += "?";
                }

                return string.Format(this.ConstantExpressionConstructorWithType, GetFixedName(param.Name), clrTypeName);
            }

            return string.Format(this.ConstantExpressionConstructorWithType, GetFixedName(param.Name), typeName);
        }

        // This is to solve duplicate names between property and type
        internal void SetPropertyIdentifierMappingsIfNameConflicts(string typeName, IEdmStructuredType structuredType)
        {
            if (this.context.EnableNamingAlias)
            {
                typeName = Customization.CustomizeNaming(typeName);
            }

            // PropertyName in VB is case-insensitive.
            bool isLanguageCaseSensitive = this.context.TargetLanguage == LanguageOption.CSharp;

            // In VB, it is allowed that a type has a property whose name is same with the type's name
            bool allowPropertyNameSameWithTypeName = this.context.TargetLanguage == LanguageOption.VB;

            Func<string, string> customizePropertyName = (name) => { return this.context.EnableNamingAlias ? Customization.CustomizeNaming(name) : name; };

            var propertyGroups = structuredType.Properties()
                .GroupBy(p => isLanguageCaseSensitive ? customizePropertyName(p.Name) : customizePropertyName(p.Name).ToUpperInvariant());

            // If the group contains more than one property, or the property in the group has the same name with the type (only for C#), we need to rename the property
            var propertyToBeRenamedGroups = propertyGroups.Where(g => g.Count() > 1 || !allowPropertyNameSameWithTypeName && g.Key == typeName);

            var knownIdentifiers = propertyGroups.Select(g => customizePropertyName(g.First().Name)).ToList();
            if (!allowPropertyNameSameWithTypeName && !knownIdentifiers.Contains(typeName))
            {
                knownIdentifiers.Add(typeName);
            }
            UniqueIdentifierService uniqueIdentifierService =
                new UniqueIdentifierService(knownIdentifiers, isLanguageCaseSensitive);

            IdentifierMappings.Clear();
            foreach (IGrouping<string, IEdmProperty> g in propertyToBeRenamedGroups)
            {
                bool hasPropertyNameSameWithCustomizedPropertyName = false;
                int itemCount = g.Count();
                for (int i = 0; i < itemCount; i++)
                {
                    var property = g.ElementAt(i);
                    var customizedPropertyName = customizePropertyName(property.Name);

                    if (this.context.EnableNamingAlias && customizedPropertyName == property.Name)
                    {
                        hasPropertyNameSameWithCustomizedPropertyName = true;
                    }

                    if (isLanguageCaseSensitive)
                    {
                        // If a property name is same as its customized property name, then we don't rename it.
                        // Or we don't rename the last property in the group
                        if (customizedPropertyName != typeName
                            && (customizedPropertyName == property.Name
                                || (!hasPropertyNameSameWithCustomizedPropertyName && i == itemCount - 1)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // When EnableNamingAlias = true, If a property name is same as its customized property name, then we don't rename it.
                        // Or we don't rename the last property in the group.
                        if ((this.context.EnableNamingAlias && customizedPropertyName == property.Name)
                            || (!hasPropertyNameSameWithCustomizedPropertyName && i == itemCount - 1))
                        {
                            continue;
                        }
                    }
                    var renamedPropertyName = uniqueIdentifierService.GetUniqueIdentifier(customizedPropertyName);
                    IdentifierMappings.Add(property.Name, renamedPropertyName);
                }
            }
        }

        internal void WriteTypeStaticCreateMethod(string typeName, IEdmStructuredType structuredType)
        {
            Debug.Assert(structuredType != null, "structuredType != null");
            if (structuredType.IsAbstract)
            {
                return;
            }

            Func<IEdmProperty, bool> hasDefault = p => p.PropertyKind == EdmPropertyKind.Structural && ((IEdmStructuralProperty)p).DefaultValueString != null;

            if (this.context.EnableNamingAlias)
            {
                typeName = Customization.CustomizeNaming(typeName);
            }

            IEnumerable<IEdmProperty> parameters = structuredType.Properties()
                .Where(p => !p.Type.IsNullable && !p.Type.IsCollection() && !hasDefault(p));
            if (!parameters.Any())
            {
                return;
            }

            this.WriteSummaryCommentForStaticCreateMethod(typeName);

            UniqueIdentifierService uniqueIdentifierService = new UniqueIdentifierService( /*IsLanguageCaseSensitive*/true);
            string instanceName = GetFixedName(uniqueIdentifierService.GetUniqueParameterName(typeName));
            KeyValuePair<IEdmProperty, string>[] propertyToParameterNamePairs = parameters
                .Select(p =>
                    new KeyValuePair<IEdmProperty, string>(p,
                        uniqueIdentifierService.GetUniqueParameterName(
                            IdentifierMappings.ContainsKey(p.Name) ? IdentifierMappings[p.Name] : p.Name)))
                .ToArray();

            foreach (var propertyToParameterNamePair in propertyToParameterNamePairs)
            {
                string propertyName = propertyToParameterNamePair.Key.Name;
                propertyName = IdentifierMappings.ContainsKey(propertyName) ?
                    IdentifierMappings[propertyName] : (this.context.EnableNamingAlias ? Customization.CustomizeNaming(propertyName) : propertyName);
                this.WriteParameterCommentForStaticCreateMethod(propertyToParameterNamePair.Value, propertyName);
            }

            propertyToParameterNamePairs = propertyToParameterNamePairs
                .Select(p => p = new KeyValuePair<IEdmProperty, string>(p.Key, GetFixedName(p.Value)))
                .ToArray();

            this.WriteDeclarationStartForStaticCreateMethod(typeName, GetFixedName(typeName));
            this.WriteStaticCreateMethodParameters(propertyToParameterNamePairs);
            this.WriteDeclarationEndForStaticCreateMethod(GetFixedName(typeName), instanceName);

            foreach (var propertyToParameterNamePair in propertyToParameterNamePairs)
            {
                IEdmProperty property = propertyToParameterNamePair.Key;
                string parameterName = propertyToParameterNamePair.Value;

                Debug.Assert(!property.Type.IsCollection(), "!property.Type.IsCollection()");
                Debug.Assert(!property.Type.IsNullable, "!property.Type.IsNullable");

                // The static create method only sets non-nullable properties. We should add the null check if the type of the property is not a clr ValueType.
                // For now we add the null check if the property type is non-primitive. We should add the null check for non-ValueType primitives in the future.
                if (!property.Type.IsPrimitive() && !property.Type.IsEnum())
                {
                    this.WriteParameterNullCheckForStaticCreateMethod(parameterName);
                }

                var uniqIdentifier = IdentifierMappings.ContainsKey(property.Name) ?
                    IdentifierMappings[property.Name] : (this.context.EnableNamingAlias ? Customization.CustomizeNaming(property.Name) : property.Name);
                this.WritePropertyValueAssignmentForStaticCreateMethod(instanceName,
                    GetFixedName(uniqIdentifier),
                    parameterName);
            }

            this.WriteMethodEndForStaticCreateMethod(instanceName);
        }

        internal void WriteStaticCreateMethodParameters(KeyValuePair<IEdmProperty, string>[] propertyToParameterPairs)
        {
            if (propertyToParameterPairs.Length == 0)
            {
                return;
            }

            // If the number of parameters are greater than 5, we put them in separate lines.
            string parameterSeparator = propertyToParameterPairs.Length > 5 ? this.ParameterSeparator : ", ";
            for (int idx = 0; idx < propertyToParameterPairs.Length; idx++)
            {
                KeyValuePair<IEdmProperty, string> propertyToParameterPair = propertyToParameterPairs[idx];

                string parameterType = Utils.GetClrTypeName(propertyToParameterPair.Key.Type, this.context.UseDataServiceCollection, this, this.context);
                string parameterName = propertyToParameterPair.Value;
                if (idx == propertyToParameterPairs.Length - 1)
                {
                    // No separator after the last parameter.
                    parameterSeparator = string.Empty;
                }

                this.WriteParameterForStaticCreateMethod(parameterType, GetFixedName(parameterName), parameterSeparator);
            }
        }

        internal void WritePropertiesForStructuredType(IEnumerable<IEdmProperty> properties)
        {
            bool useDataServiceCollection = this.context.UseDataServiceCollection;

            var propertyInfos = properties.Select(property =>
            {
                string propertyName = IdentifierMappings.ContainsKey(property.Name) ?
                    IdentifierMappings[property.Name] : (this.context.EnableNamingAlias ? Customization.CustomizeNaming(property.Name) : property.Name);

                return new
                {
                    PropertyType = Utils.GetClrTypeName(property.Type, useDataServiceCollection, this, this.context),
                    PropertyVanillaName = property.Name,
                    PropertyName = propertyName,
                    FixedPropertyName = GetFixedName(propertyName),
                    PrivatePropertyName = "_" + propertyName,
                    PropertyInitializationValue = Utils.GetPropertyInitializationValue(property, useDataServiceCollection, this, this.context)
                };
            }).ToList();

            // Private name should not confict with field name
            UniqueIdentifierService uniqueIdentifierService = new UniqueIdentifierService(propertyInfos.Select(_ => _.FixedPropertyName),
                this.context.TargetLanguage == LanguageOption.CSharp);

            foreach (var propertyInfo in propertyInfos)
            {
                string privatePropertyName = uniqueIdentifierService.GetUniqueIdentifier("_" + propertyInfo.PropertyName);

                this.WritePropertyForStructuredType(
                    propertyInfo.PropertyType,
                    propertyInfo.PropertyVanillaName,
                    propertyInfo.PropertyName,
                    propertyInfo.FixedPropertyName,
                    privatePropertyName,
                    propertyInfo.PropertyInitializationValue,
                    useDataServiceCollection);
            }
        }

        internal void WriteMembersForEnumType(IEnumerable<IEdmEnumMember> members)
        {
            int n = members.Count();
            for (int idx = 0; idx < n; ++idx)
            {
                IEdmEnumMember member = members.ElementAt(idx);
                string value = string.Empty;
                if (member.Value != null)
                {
                    IEdmIntegerValue integerValue = member.Value as IEdmIntegerValue;
                    if (integerValue != null)
                    {
                        value = " = " + integerValue.Value.ToString(CultureInfo.InvariantCulture);
                    }
                }

                string memberName = this.context.EnableNamingAlias ? Customization.CustomizeNaming(member.Name) : member.Name;
                this.WriteMemberForEnumType(GetFixedName(memberName) + value, member.Name, idx == n - 1);
            }
        }

        internal string GetFixedName(string originalName)
        {
            string fixedName = originalName;

            if (this.LanguageKeywords.Contains(fixedName))
            {
                fixedName = string.Format(this.FixPattern, fixedName);
            }

            return fixedName;
        }

        internal string GetElementTypeName(IEdmEntityType elementType, IEdmEntityContainer container)
        {
            string elementTypeName = elementType.Name;

            if (this.context.EnableNamingAlias)
            {
                elementTypeName = Customization.CustomizeNaming(elementTypeName);
            }

            if (elementType.Namespace != container.Namespace)
            {
                elementTypeName = this.context.GetPrefixedFullName(elementType, GetFixedName(elementTypeName), this);
            }

            return elementTypeName;
        }
    }

    /// <summary>
    /// Base class for text transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "11.0.0.0")]
    public abstract class TemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion

        /// <summary>
        /// Create the template output
        /// </summary>
        public abstract string TransformText();

        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0)
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                        typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                            this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }

    /// <summary>
    /// Service making names within a scope unique. Initialize a new instance for every scope.
    /// </summary>
    internal sealed class UniqueIdentifierService
    {
        // This is the list of keywords we check against when creating parameter names from propert. 
        // If a name matches this keyword we prefix it.
        private static readonly string[] Keywords = new string[] { "class", "event" };

        /// <summary>
        /// Hash set to detect identifier collision.
        /// </summary>
        private readonly HashSet<string> knownIdentifiers;

        /// <summary>
        /// Constructs a <see cref="UniqueIdentifierService"/>.
        /// </summary>
        /// <param name="caseSensitive">true if the language we are generating the code for is case sensitive, false otherwise.</param>
        internal UniqueIdentifierService(bool caseSensitive)
        {
            this.knownIdentifiers = new HashSet<string>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Constructs a <see cref="UniqueIdentifierService"/>.
        /// </summary>
        /// <param name="identifiers">identifiers used to detect collision.</param>
        /// <param name="caseSensitive">true if the language we are generating the code for is case sensitive, false otherwise.</param>
        internal UniqueIdentifierService(IEnumerable<string> identifiers, bool caseSensitive)
        {
            this.knownIdentifiers = new HashSet<string>(identifiers ?? Enumerable.Empty<string>(), caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Given an identifier, makes it unique within the scope by adding
        /// a suffix (1, 2, 3, ...), and returns the adjusted identifier.
        /// </summary>
        /// <param name="identifier">Identifier. Must not be null or empty.</param>
        /// <returns>Identifier adjusted to be unique within the scope.</returns>
        internal string GetUniqueIdentifier(string identifier)
        {
            Debug.Assert(!string.IsNullOrEmpty(identifier), "identifier is null or empty");

            // find a unique name by adding suffix as necessary
            int numberOfConflicts = 0;
            string uniqueIdentifier = identifier;
            while (this.knownIdentifiers.Contains(uniqueIdentifier))
            {
                ++numberOfConflicts;
                uniqueIdentifier = identifier + numberOfConflicts.ToString(CultureInfo.InvariantCulture);
            }

            // remember the identifier in this scope
            Debug.Assert(!this.knownIdentifiers.Contains(uniqueIdentifier), "we just made it unique");
            this.knownIdentifiers.Add(uniqueIdentifier);

            return uniqueIdentifier;
        }

        /// <summary>
        /// Fix up the given parameter name and make it unique.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>Fixed parameter name.</returns>
        internal string GetUniqueParameterName(string name)
        {
            name = Utils.CamelCase(name);

            // FxCop consider 'iD' as violation, we will change any property that is 'id'(case insensitive) to 'ID'
            if (StringComparer.OrdinalIgnoreCase.Equals(name, "id"))
            {
                name = "ID";
            }

            return this.GetUniqueIdentifier(name);
        }
    }

    /// <summary>
    /// Utility class.
    /// </summary>    
    internal static class Utils
    {
        /// <summary>
        /// Serializes the xml element to a string.
        /// </summary>
        /// <param name="xml">The xml element to serialize.</param>
        /// <returns>The string representation of the xml.</returns>
        internal static string SerializeToString(XElement xml)
        {
            // because comment nodes can contain special characters that are hard to embed in VisualBasic, remove them here
            xml.DescendantNodes().OfType<XComment>().Remove();

            var stringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(
                stringBuilder,
                new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    NewLineHandling = NewLineHandling.Replace,
                    Indent = true,
                }))
            {
                xml.WriteTo(writer);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Changes the text to use camel case, which lower case for the first character.
        /// </summary>
        /// <param name="text">Text to convert.</param>
        /// <returns>The converted text in camel case</returns>
        internal static string CamelCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            }

            return text[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + text.Substring(1);
        }

        /// <summary>
        /// Changes the text to use pascal case, which upper case for the first character.
        /// </summary>
        /// <param name="text">Text to convert.</param>
        /// <returns>The converted text in pascal case</returns>
        internal static string PascalCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
            }

            return text[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant() + text.Substring(1);
        }

        /// <summary>
        /// Gets the clr type name from the give type reference.
        /// </summary>
        /// <param name="edmTypeReference">The type reference in question.</param>
        /// <param name="useDataServiceCollection">true to use the DataServicCollection type for entity collections and the ObservableCollection type for non-entity collections,
        /// false to use Collection for collections.</param>
        /// <param name="clientTemplate">ODataClientTemplate instance that call this method.</param>
        /// <param name="context">CodeGenerationContext instance in the clientTemplate.</param>
        /// <param name="addNullableTemplate">This flag indicates whether to return the type name in nullable format</param>
        /// <param name="needGlobalPrefix">The flag indicates whether the namespace need to be added by global prefix</param>
        /// <param name="isOperationParameter">This flag indicates whether the edmTypeReference is for an operation parameter</param>
        /// <returns>The clr type name of the type reference.</returns>
        internal static string GetClrTypeName(IEdmTypeReference edmTypeReference, bool useDataServiceCollection, ODataClientTemplate clientTemplate, CodeGenerationContext context, bool addNullableTemplate = true, bool needGlobalPrefix = true, bool isOperationParameter = false)
        {
            string clrTypeName;
            IEdmType edmType = edmTypeReference.Definition;
            IEdmPrimitiveType edmPrimitiveType = edmType as IEdmPrimitiveType;
            if (edmPrimitiveType != null)
            {
                clrTypeName = Utils.GetClrTypeName(edmPrimitiveType, clientTemplate);
                if (edmTypeReference.IsNullable && !clientTemplate.ClrReferenceTypes.Contains(edmPrimitiveType.PrimitiveKind) && addNullableTemplate)
                {
                    clrTypeName = string.Format(clientTemplate.SystemNullableStructureTemplate, clrTypeName);
                }
            }
            else
            {
                IEdmComplexType edmComplexType = edmType as IEdmComplexType;
                if (edmComplexType != null)
                {
                    clrTypeName = context.GetPrefixedFullName(edmComplexType,
                        context.EnableNamingAlias ? clientTemplate.GetFixedName(Customization.CustomizeNaming(edmComplexType.Name)) : clientTemplate.GetFixedName(edmComplexType.Name), clientTemplate);
                }
                else
                {
                    IEdmEnumType edmEnumType = edmType as IEdmEnumType;
                    if (edmEnumType != null)
                    {
                        clrTypeName = context.GetPrefixedFullName(edmEnumType,
                            context.EnableNamingAlias ? clientTemplate.GetFixedName(Customization.CustomizeNaming(edmEnumType.Name)) : clientTemplate.GetFixedName(edmEnumType.Name), clientTemplate, needGlobalPrefix);
                        if (edmTypeReference.IsNullable && addNullableTemplate)
                        {
                            clrTypeName = string.Format(clientTemplate.SystemNullableStructureTemplate, clrTypeName);
                        }
                    }
                    else
                    {
                        IEdmEntityType edmEntityType = edmType as IEdmEntityType;
                        if (edmEntityType != null)
                        {
                            clrTypeName = context.GetPrefixedFullName(edmEntityType,
                                context.EnableNamingAlias ? clientTemplate.GetFixedName(Customization.CustomizeNaming(edmEntityType.Name)) : clientTemplate.GetFixedName(edmEntityType.Name), clientTemplate);
                        }
                        else
                        {
                            IEdmCollectionType edmCollectionType = (IEdmCollectionType)edmType;
                            IEdmTypeReference elementTypeReference = edmCollectionType.ElementType;
                            IEdmPrimitiveType primitiveElementType = elementTypeReference.Definition as IEdmPrimitiveType;
                            if (primitiveElementType != null)
                            {
                                clrTypeName = Utils.GetClrTypeName(primitiveElementType, clientTemplate);
                            }
                            else
                            {
                                IEdmSchemaElement schemaElement = (IEdmSchemaElement)elementTypeReference.Definition;
                                clrTypeName = context.GetPrefixedFullName(schemaElement,
                                    context.EnableNamingAlias ? clientTemplate.GetFixedName(Customization.CustomizeNaming(schemaElement.Name)) : clientTemplate.GetFixedName(schemaElement.Name), clientTemplate);
                            }

                            string collectionTypeName = isOperationParameter
                                                            ? clientTemplate.ICollectionOfTStructureTemplate
                                                            : (useDataServiceCollection
                                                                ? (elementTypeReference.TypeKind() == EdmTypeKind.Entity
                                                                    ? clientTemplate.DataServiceCollectionStructureTemplate
                                                                    : clientTemplate.ObservableCollectionStructureTemplate)
                                                                : clientTemplate.ObjectModelCollectionStructureTemplate);

                            clrTypeName = string.Format(collectionTypeName, clrTypeName);
                        }
                    }
                }
            }

            return clrTypeName;
        }

        /// <summary>
        /// Gets the value expression to initualize the property with.
        /// </summary>
        /// <param name="property">The property in question.</param>
        /// <param name="useDataServiceCollection">true to use the DataServicCollection type for entity collections and the ObservableCollection type for non-entity collections,
        /// false to use Collection for collections.</param>
        /// <param name="clientTemplate">ODataClientTemplate instance that call this method.</param>
        /// <param name="context">CodeGenerationContext instance in the clientTemplate.</param>
        /// <returns>The value expression to initualize the property with.</returns>
        internal static string GetPropertyInitializationValue(IEdmProperty property, bool useDataServiceCollection, ODataClientTemplate clientTemplate, CodeGenerationContext context)
        {
            IEdmTypeReference edmTypeReference = property.Type;
            IEdmCollectionTypeReference edmCollectionTypeReference = edmTypeReference as IEdmCollectionTypeReference;
            if (edmCollectionTypeReference == null)
            {
                IEdmStructuralProperty structuredProperty = property as IEdmStructuralProperty;
                if (structuredProperty != null)
                {
                    if (!string.IsNullOrEmpty(structuredProperty.DefaultValueString))
                    {
                        string valueClrType = GetClrTypeName(edmTypeReference, useDataServiceCollection, clientTemplate, context);
                        string defaultValue = structuredProperty.DefaultValueString;
                        bool isCSharpTemplate = clientTemplate is ODataClientCSharpTemplate;
                        if (edmTypeReference.Definition.TypeKind == EdmTypeKind.Enum)
                        {
                            var enumValues = defaultValue.Split(',');
                            string fullenumTypeName = GetClrTypeName(edmTypeReference, useDataServiceCollection, clientTemplate, context);
                            string enumTypeName = GetClrTypeName(edmTypeReference, useDataServiceCollection, clientTemplate, context, false, false);
                            List<string> customizedEnumValues = new List<string>();
                            foreach (var enumValue in enumValues)
                            {
                                string currentEnumValue = enumValue.Trim();
                                int indexFirst = currentEnumValue.IndexOf('\'') + 1;
                                int indexLast = currentEnumValue.LastIndexOf('\'');
                                if (indexFirst > 0 && indexLast > indexFirst)
                                {
                                    currentEnumValue = currentEnumValue.Substring(indexFirst, indexLast - indexFirst);
                                }

                                var customizedEnumValue = context.EnableNamingAlias ? Customization.CustomizeNaming(currentEnumValue) : currentEnumValue;
                                if (isCSharpTemplate)
                                {
                                    currentEnumValue = "(" + fullenumTypeName + ")" + clientTemplate.EnumTypeName + ".Parse(" + clientTemplate.SystemTypeTypeName + ".GetType(\"" + enumTypeName + "\"), \"" + customizedEnumValue + "\")";
                                }
                                else
                                {
                                    currentEnumValue = clientTemplate.EnumTypeName + ".Parse(" + clientTemplate.SystemTypeTypeName + ".GetType(\"" + enumTypeName + "\"), \"" + currentEnumValue + "\")";
                                }
                                customizedEnumValues.Add(currentEnumValue);
                            }
                            if (isCSharpTemplate)
                            {
                                return string.Join(" | ", customizedEnumValues);
                            }
                            else
                            {
                                return string.Join(" Or ", customizedEnumValues);
                            }
                        }

                        if (valueClrType.Equals(clientTemplate.StringTypeName))
                        {
                            defaultValue = "\"" + defaultValue + "\"";
                        }
                        else if (valueClrType.Equals(clientTemplate.BinaryTypeName))
                        {
                            defaultValue = "System.Text.Encoding.UTF8.GetBytes(\"" + defaultValue + "\")";
                        }
                        else if (valueClrType.Equals(clientTemplate.SingleTypeName))
                        {
                            if (isCSharpTemplate)
                            {
                                defaultValue = defaultValue.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? defaultValue : defaultValue + "f";
                            }
                            else
                            {
                                defaultValue = defaultValue.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? defaultValue : defaultValue + "F";
                            }
                        }
                        else if (valueClrType.Equals(clientTemplate.DecimalTypeName))
                        {
                            if (isCSharpTemplate)
                            {
                                // decimal in C# must be initialized with 'm' at the end, like Decimal dec = 3.00m
                                defaultValue = defaultValue.EndsWith("m", StringComparison.OrdinalIgnoreCase) ? defaultValue : defaultValue + "m";
                            }
                            else
                            {
                                // decimal in VB must be initialized with 'D' at the end, like Decimal dec = 3.00D
                                defaultValue = defaultValue.ToLower().Replace("m", "D");
                                defaultValue = defaultValue.EndsWith("D", StringComparison.OrdinalIgnoreCase) ? defaultValue : defaultValue + "D";
                            }
                        }
                        else if (valueClrType.Equals(clientTemplate.GuidTypeName)
                            | valueClrType.Equals(clientTemplate.DateTimeOffsetTypeName)
                            | valueClrType.Equals(clientTemplate.DateTypeName)
                            | valueClrType.Equals(clientTemplate.TimeOfDayTypeName))
                        {
                            defaultValue = valueClrType + ".Parse(\"" + defaultValue + "\")";
                        }
                        else if (valueClrType.Equals(clientTemplate.DurationTypeName))
                        {
                            defaultValue = clientTemplate.XmlConvertClassName + ".ToTimeSpan(\"" + defaultValue + "\")";
                        }
                        else if (valueClrType.Contains("Microsoft.Spatial"))
                        {
                            defaultValue = string.Format(clientTemplate.GeoTypeInitializePattern, valueClrType, defaultValue);
                        }

                        return defaultValue;
                    }
                    else
                    {
                        // doesn't have a default value 
                        return null;
                    }
                }
                else
                {
                    // only structured property has default value
                    return null;
                }
            }
            else
            {
                string constructorParameters;
                if (edmCollectionTypeReference.ElementType().IsEntity() && useDataServiceCollection)
                {
                    constructorParameters = clientTemplate.DataServiceCollectionConstructorParameters;
                }
                else
                {
                    constructorParameters = "()";
                }

                string clrTypeName = GetClrTypeName(edmTypeReference, useDataServiceCollection, clientTemplate, context);
                return clientTemplate.NewModifier + clrTypeName + constructorParameters;
            }
        }

        /// <summary>
        /// Gets the clr type name from the give Edm primitive type.
        /// </summary>
        /// <param name="edmPrimitiveType">The Edm primitive type in question.</param>
        /// <param name="clientTemplate">ODataClientTemplate instance that call this method.</param>
        /// <returns>The clr type name of the Edm primitive type.</returns>
        internal static string GetClrTypeName(IEdmPrimitiveType edmPrimitiveType, ODataClientTemplate clientTemplate)
        {
            EdmPrimitiveTypeKind kind = edmPrimitiveType.PrimitiveKind;

            string type = "UNKNOWN";
            if (kind == EdmPrimitiveTypeKind.Int32)
            {
                type = clientTemplate.Int32TypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.String)
            {
                type = clientTemplate.StringTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Binary)
            {
                type = clientTemplate.BinaryTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Decimal)
            {
                type = clientTemplate.DecimalTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Int16)
            {
                type = clientTemplate.Int16TypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Single)
            {
                type = clientTemplate.SingleTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Boolean)
            {
                type = clientTemplate.BooleanTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Double)
            {
                type = clientTemplate.DoubleTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Guid)
            {
                type = clientTemplate.GuidTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Byte)
            {
                type = clientTemplate.ByteTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Int64)
            {
                type = clientTemplate.Int64TypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.SByte)
            {
                type = clientTemplate.SByteTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Stream)
            {
                type = clientTemplate.DataServiceStreamLinkTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Geography)
            {
                type = clientTemplate.GeographyTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyPoint)
            {
                type = clientTemplate.GeographyPointTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyLineString)
            {
                type = clientTemplate.GeographyLineStringTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyPolygon)
            {
                type = clientTemplate.GeographyPolygonTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyCollection)
            {
                type = clientTemplate.GeographyCollectionTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyMultiPolygon)
            {
                type = clientTemplate.GeographyMultiPolygonTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyMultiLineString)
            {
                type = clientTemplate.GeographyMultiLineStringTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeographyMultiPoint)
            {
                type = clientTemplate.GeographyMultiPointTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Geometry)
            {
                type = clientTemplate.GeometryTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryPoint)
            {
                type = clientTemplate.GeometryPointTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryLineString)
            {
                type = clientTemplate.GeometryLineStringTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryPolygon)
            {
                type = clientTemplate.GeometryPolygonTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryCollection)
            {
                type = clientTemplate.GeometryCollectionTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryMultiPolygon)
            {
                type = clientTemplate.GeometryMultiPolygonTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryMultiLineString)
            {
                type = clientTemplate.GeometryMultiLineStringTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.GeometryMultiPoint)
            {
                type = clientTemplate.GeometryMultiPointTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.DateTimeOffset)
            {
                type = clientTemplate.DateTimeOffsetTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Duration)
            {
                type = clientTemplate.DurationTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.Date)
            {
                type = clientTemplate.DateTypeName;
            }
            else if (kind == EdmPrimitiveTypeKind.TimeOfDay)
            {
                type = clientTemplate.TimeOfDayTypeName;
            }
            else
            {
                throw new Exception("Type " + kind.ToString() + " is unrecognized");
            }

            return type;
        }
    }
}
