﻿using Microsoft.Extensions.Logging;
using TopModel.Core;
using TopModel.Core.FileModel;
using TopModel.Utils;

namespace TopModel.Generator.Javascript;

/// <summary>
/// Générateur des objets de traduction javascripts.
/// </summary>
public class AngularApiClientGenerator : GeneratorBase
{
    private readonly JavascriptConfig _config;
    private readonly ILogger<AngularApiClientGenerator> _logger;

    public AngularApiClientGenerator(ILogger<AngularApiClientGenerator> logger, JavascriptConfig config)
        : base(logger, config)
    {
        _config = config;
        _logger = logger;
    }

    public override string Name => "JSNGApiClientGen";

    public override List<string> GeneratedFiles => Files.Values.Where(f => f.Endpoints.Any())
        .SelectMany(file => _config.Tags.Intersect(file.Tags).Select(tag => _config.GetEndpointsFileName(file, tag)))
        .Distinct()
        .ToList();

    protected override void HandleFiles(IEnumerable<ModelFile> files)
    {
        foreach (var file in files)
        {
            GenerateClientFile(file);
        }
    }

    private void GenerateClientFile(ModelFile file)
    {
        if (!file.Endpoints.Any())
        {
            return;
        }

        foreach (var (tag, fileName) in _config.Tags.Intersect(file.Tags)
            .Select(tag => (tag, fileName: _config.GetEndpointsFileName(file, tag)))
            .DistinctBy(t => t.fileName))
        {
            using var fw = new FileWriter(fileName, _logger, false);
            var imports = _config.GetEndpointImports(file, tag, Classes);

            imports.AddRange(new List<(string Import, string Path)>()
            {
                (Import: "Injectable", Path: "@angular/core"),
                (Import: "HttpClient", Path: "@angular/common/http"),
                (Import: "Observable", Path: "rxjs"),
            });

            if (file.Endpoints.Any(e => e.GetQueryParams().Any()))
            {
                imports.Add((Import: "HttpParams", Path: "@angular/common/http"));
            }

            imports = imports.GroupAndSort();

            if (imports.Any())
            {
                fw.WriteLine();

                foreach (var (import, path) in imports)
                {
                    fw.WriteLine($@"import {{{import}}} from ""{path}"";");
                }
            }

            fw.WriteLine("@Injectable({");
            fw.WriteLine(1, "providedIn: 'root'");
            fw.WriteLine("})");

            fw.WriteLine(@$"export class {GetClassName(file)} {{");
            fw.WriteLine();
            fw.WriteLine(1, "constructor(private http: HttpClient) {}");
            foreach (var endpoint in file.Endpoints)
            {
                WriteEndpoint(endpoint, fw);
            }

            fw.WriteLine("}");
        }
    }

    private void WriteEndpoint(Endpoint endpoint, FileWriter fw)
    {
        fw.WriteLine();
        fw.WriteLine(1, "/**");
        fw.WriteLine(1, $" * {endpoint.Description}");

        foreach (var param in endpoint.Params)
        {
            fw.WriteLine(1, $" * @param {param.GetParamName()} {param.Comment}");
        }

        fw.WriteLine(1, " * @param options Options pour 'fetch'.");

        if (endpoint.Returns != null)
        {
            fw.WriteLine(1, $" * @returns {endpoint.Returns.Comment}");
        }

        fw.WriteLine(1, " */");
        fw.Write(1, $"{endpoint.Name.ToFirstLower()}(");

        var hasForm = endpoint.Params.Any(p => p is IFieldProperty fp && fp.Domain.TS!.Type.Contains("File"));
        var hasProperty = false;
        foreach (var param in endpoint.Params)
        {
            hasProperty = true;
            fw.Write($"{param.GetParamName()}{(param.IsQueryParam() && !hasForm ? "?" : string.Empty)}: {param.GetPropertyTypeName(Classes)} ");
        }

        string returnType;
        if (endpoint.GetQueryParams().Any())
        {
            if (hasProperty)
            {
                fw.Write(", ");
            }

            fw.Write("queryParams: any = {}");
        }

        fw.Write("): Observable<");

        if (endpoint.Returns == null)
        {
            returnType = "void";
        }
        else
        {
            returnType = endpoint.Returns.GetPropertyTypeName(Classes);
        }

        fw.Write(returnType);
        fw.WriteLine("> {");

        if (endpoint.GetQueryParams().Any())
        {
            fw.WriteLine(2, "const httpParams = new HttpParams({fromObject : queryParams});");
            fw.WriteLine(2, "const httpOptions = { params: httpParams }");
            foreach (var qParam in endpoint.GetQueryParams())
            {
                fw.WriteLine(2, @$"if({qParam.GetParamName()} !== null) {{");
                fw.WriteLine(3, $"httpOptions.params.set('{qParam.GetParamName()}', {qParam.GetParamName()})");
                fw.WriteLine(2, @$"}}");
            }

            fw.WriteLine();
        }

        fw.Write(2, $@"return this.http.{endpoint.Method.ToLower()}<{returnType}>(`/{endpoint.FullRoute.Replace("{", "${")}`");

        if (endpoint.GetBodyParam() != null)
        {
            fw.Write($", {endpoint.GetBodyParam()!.GetParamName()}");
        }

        if (endpoint.GetQueryParams().Any())
        {
            fw.Write(", httpOptions");
        }

        fw.WriteLine(");");
        fw.WriteLine(1, "}");
    }

    private string GetClassName(ModelFile file)
    {
        if (file.Options?.Endpoints?.FileName != null)
        {
            return $"{file.Options.Endpoints.FileName.ToFirstUpper()}Service";
        }

        var filePath = file.Name.Split("/").Last();
        return $"{string.Join('_', filePath.Split("_").Skip(filePath.Contains('_') ? 1 : 0)).ToFirstUpper()}Service";
    }
}