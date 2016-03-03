#Commandline tool for generating typedefinition from OData v4 metadata

This is a small command line util, that can generate typedefinition file from the odata metadata endpoint. 

Currently it only supports odata v4. And I have only tested it with https://graph.microsoft.com/beta/$metadata the generated output is located in https://github.com/sjkp/SJKP.OutlookHooks/blob/master/SJKP.OutlookHooksWeb/typings/Microsoft.Graph/microsoft.graph.d.ts

Run it like
```
SJKP.ODataToTypeScript.exe -u https://graph.microsoft.com/beta/$metadata -n Microsoft.Graph
```

```
 -u, --Uri                                      Required.

 -n, --Namespace                                Required.

 -d, --UseDataServiceCollection                 (Default: True)

 -a, --EnableNamingAlias                        (Default: True)

 -i, --IgnoreUnexpectedElementsAndAttributes    (Default: True)

 -o, --Output                                   (Default: output.d.ts)

 -c, --CSharp Output                            (Default: class.cs)

 --help                                         Display this help screen.
```

##Todo
The typedefinition could be nicer formatted, there are some C# reminisce in it. 
