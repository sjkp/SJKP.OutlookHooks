﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.OData.Client;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TypeLite;

namespace SJKP.ODataToTypeScript
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Commands();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine(options.GetUsage());
                return;
            }
            CodeGenerationContext context;
            

                context = new CodeGenerationContext(new Uri(options.MetadataDocumentUri, UriKind.Absolute), options.NamespacePrefix)
                {
                    UseDataServiceCollection = options.UseDataServiceCollection,
                    TargetLanguage = LanguageOption.CSharp,
                    EnableNamingAlias = options.EnableNamingAlias,
                    IgnoreUnexpectedElementsAndAttributes = options.IgnoreUnexpectedElementsAndAttributes
                };
                      

            ODataClientTemplate template = new ODataClientCSharpTemplate(context);           
            
            var s = template.TransformText();
            File.WriteAllText(options.CsharpOutput, s);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(s);
            string assemblyName = Path.GetRandomFileName();

            List<MetadataReference> references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(DataServiceActionQuery).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ODataAction).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(GeneratedCodeAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEdmModel).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TimeOfDay).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),

                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(XmlDocument).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TsClassAttribute).Assembly.Location)

            };

            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            op = op.WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);            
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: op);
            Assembly assembly = null;
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    assembly = Assembly.Load(ms.ToArray());
                }
            }



            

            var ts = TypeScript.Definitions().For(assembly);

            var props = ts.Generate(TsGeneratorOutput.Properties);
            var enums = ts.Generate(TsGeneratorOutput.Enums);

            File.WriteAllText(options.Output, props + enums);
        }
    }
}
