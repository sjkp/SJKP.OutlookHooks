using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJKP.ODataToTypeScript
{
    class Commands
    {
        [Option('u', "Uri", Required = true)]
        public string MetadataDocumentUri { get; set; }

        [Option('n', "Namespace", Required =true)]
        public string NamespacePrefix { get; set; }

        [Option('d', "UseDataServiceCollection", DefaultValue = true)]
        public bool UseDataServiceCollection { get; set; }

        [Option('a', "EnableNamingAlias", DefaultValue = true)]
        public bool EnableNamingAlias { get; set; }

        [Option('i', "IgnoreUnexpectedElementsAndAttributes", DefaultValue =true)]
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
