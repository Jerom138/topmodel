using Microsoft.Extensions.Logging;
using TopModel.Core;
using TopModel.Core.FileModel;
using TopModel.Utils;

namespace TopModel.Generator.TopModel;

/// <summary>
/// Générateur des objets de traduction javascripts.
/// </summary>
public class TopModelGenerator : GeneratorBase
{
    private readonly TopModelConfig _config;
    private readonly ILogger<TopModelGenerator> _logger;

    private readonly IDictionary<string, ModelFile> _files = new Dictionary<string, ModelFile>();
    public override string Name => "TopModelGenerator";

    public TopModelGenerator(ILogger<TopModelGenerator> logger, TopModelConfig config)
        : base(logger, config)
    {
        _config = config;
        _logger = logger;
    }
    protected override void HandleFiles(IEnumerable<ModelFile> files)
    {
        foreach (var file in files.Where(f => f.Classes.Any() || f.Endpoints.Any()))
        {
            _files[file.Name] = file;
            GenerateFile(file);
        }
    }

    public override IEnumerable<string> GeneratedFiles => _files.Where(f => f.Value.Classes.Any() || f.Value.Endpoints.Any()).Select(f => GetFilePath(f.Value));

    private string GetFilePath(ModelFile file)
    {
        return $"{GetDestinationFolder(file)}\\{file.Name.Split("/").Last()}.tmd";
    }

    private string GetDestinationFolder(ModelFile file)
    {
        return Path.Combine(_config.ModelOutputDirectory!, string.Join("/", file.Name.Split("/").SkipLast(1)));
    }

    private void GenerateFile(ModelFile file)
    {
        var destFolder = GetDestinationFolder(file);
        Directory.CreateDirectory(destFolder);

        using var fw = new FileWriter($"{GetFilePath(file)}", _logger)
        {
            StartCommentToken = "#"
        };
        fw.WriteLine("---");
        fw.WriteLine($"module: {file.Module}");
        fw.WriteLine($"tags:");
        file.Tags.ForEach(t =>
        {
            fw.WriteLine($"  - {t}");
        });

        if (file.Uses.Any(u => this._files.Any(f => f.Key == u.ReferenceName)))
        {
            fw.WriteLine($"uses:");
            file.Uses.Where(u => this._files.Any(f => f.Key == u.ReferenceName)).ToList().ForEach(u =>
            {
                fw.WriteLine($"  - {u.ReferenceName}");
            });
        }

        foreach (Class classe in file.Classes)
        {
            WriteClasse(classe, fw);
        }

        foreach (Endpoint endpoint in file.Endpoints)
        {
            WriteEndpoint(endpoint, fw);
        }
    }

    private void WriteClasse(Class classe, FileWriter fw)
    {
        fw.WriteLine();
        fw.WriteLine("---");
        fw.WriteLine($"class:");
        fw.WriteLine($"  name: {classe.Name}");
        fw.WriteLine($"  comment: {classe.Comment}");

        if (classe.Reference)
        {
            fw.WriteLine($"  reference: true");
        }

        if (classe.Trigram != null)
        {
            fw.WriteLine($"  trigram: {classe.Trigram}");
        }

        if (classe.Extends != null)
        {
            fw.WriteLine($"  extends: {classe.Extends}");
        }

        if (classe.Label != classe.Name)
        {
            fw.WriteLine($"  label: {classe.Label}");
        }

        if (classe.PluralName != (classe.Name.EndsWith("s") ? classe.Name : $"{classe.Name}s"))
        {
            fw.WriteLine($"  pluralName: {classe.PluralName}");
        }

        if (classe.SqlName != null)
        {
            fw.WriteLine($"  sqlName: {classe.SqlName}");
        }

        if (classe.Decorators.Any())
        {
            fw.WriteLine($"  decorators:");
            classe.Decorators.ForEach(d =>
            {
                fw.WriteLine($"    - {d.Name}");
            });
        }



        fw.WriteLine($"  properties:");
        foreach (var p in classe.Properties)
        {
            WriteProperty(p, fw);
        }
        if (classe.ReferenceValues.Any())
        {
            fw.WriteLine();
            fw.WriteLine($"  values:");
            foreach (var v in classe.ReferenceValues)
            {
                fw.WriteLine(@$"    {v.Name}: {{ {string.Join(", ", v.Value.Select(val => $"{val.Key.Name}: {val.Value}"))} }}");

            }
        }
        WriteMappers(classe, fw);
    }



    private void WriteProperty(IProperty property, FileWriter fw, Boolean isList = true, Boolean skipPrimaryKey = false)
    {
        if (property is RegularProperty rp)
        {
            fw.WriteLine();
            fw.WriteLine($"    - name: {rp.Name}");
            fw.WriteLine($"      comment: {rp.Comment}");
            fw.WriteLine($"      domain: {rp.Domain.Name}");
            if (rp.Label != null)
            {
                fw.WriteLine($"      label: {rp.Label}");
            }

            if (rp.DefaultValue != null)
            {
                fw.WriteLine($"      label: {rp.DefaultValue}");
            }

            if (rp.PrimaryKey && !skipPrimaryKey)
            {
                fw.WriteLine($"      primaryKey: true");
            }

            if (rp.Required)
            {
                fw.WriteLine($"      required: true");
            }
        }
        else if (property is CompositionProperty cp)
        {
            if (!IsAvailable(cp.Composition))
            {
                throw new ModelException(cp, "La composition n'est pas accessible");
            }
            fw.WriteLine();
            fw.WriteLine($"    {(isList ? "- " : string.Empty)}composition: {cp.Composition.Name}");
            fw.WriteLine($"    {(isList ? "  " : string.Empty)}name: {cp.Name}");
            fw.WriteLine($"    {(isList ? "  " : string.Empty)}comment: {cp.Comment}");
            fw.WriteLine($"    {(isList ? "  " : string.Empty)}kind: {cp.Kind}");
        }
        else if (property is AliasProperty ap && this._files.Any(f => ap.OriginalProperty?.Class != null && f.Key == ap.OriginalProperty!.Class.ModelFile.Name))
        {
            fw.WriteLine();
            fw.WriteLine($"    - alias:");
            fw.WriteLine($"        class: {ap.OriginalProperty!.Class.Name}");
            fw.WriteLine($"        property: {ap.OriginalProperty!.Name}");
            if (ap.Prefix != null)
            {
                fw.WriteLine($"      prefix: {ap.Prefix}");
            }

            if (ap.Suffix != null)
            {
                fw.WriteLine($"      suffix: {ap.Suffix}");
            }
            if (ap.Required != ap.Property.Required)
            {
                fw.WriteLine($"      required: {(ap.Required ? "true" : "false")}");
            }
        }
        else if (property is AliasProperty ap2)
        {
            if (ap2.Property is AssociationProperty asp)
            {
                WriteProperty(asp.Association.PrimaryKey!, fw, isList, true);
            }
            else
            {
                WriteProperty(ap2.Property, fw, isList, ap2.Prefix == null && ap2.Suffix == null);
            }
        }
        else if (property is AssociationProperty asp && this._files.Any(f => asp.Association != null && f.Key == asp.Association.ModelFile.Name))
        {
            fw.WriteLine();
            fw.WriteLine($"    - association: {asp.Association.Name}");
            fw.WriteLine($"      comment: {asp.Comment}");
            if (asp.Type != AssociationType.ManyToOne)
            {
                fw.WriteLine($"      type: {asp.Type}");
            }
        }
        else if (property is AssociationProperty asp2)
        {
            WriteProperty(asp2.Association.PrimaryKey!, fw, true);
        }
    }

    private void WriteMappers(Class classe, FileWriter fw)
    {
        var availableToMappers = classe.ToMappers.Where(tm => IsAvailable(tm.Class)).ToList();
        var availableFromMappers = classe.FromMappers.Where(fm => !fm.Params.Any(p => !IsAvailable(p.Class))).ToList();
        if (availableToMappers.Any() || availableFromMappers.Any())
        {
            fw.WriteLine();
            fw.WriteLine($"  mappers:");
            availableToMappers.ForEach(tm =>
            {
                fw.WriteLine($"    to:");
                fw.WriteLine($"      - class: {tm.Class.Name}");
                if (tm.Name != $"To{tm.Class.Name}")
                {
                    fw.WriteLine($"        name: {tm.Name}");
                }

                if (tm.Comment != null)
                {
                    fw.WriteLine($"        comment: {tm.Comment}");
                }
            });
            availableFromMappers.ForEach(fm =>
            {
                fw.WriteLine($"    from:");
                fw.WriteLine($"      - params:");
                fm.Params.ForEach(fmp =>
                {
                    fw.WriteLine($"        - class: {fmp.Class.Name}");
                });
                if (fm.Comment != null)
                {
                    fw.WriteLine($"        comment: {fm.Comment}");
                }
            });
        }
    }

    private void WriteEndpoint(Endpoint endpoint, FileWriter fw)
    {
        fw.WriteLine();
        fw.WriteLine("---");
        fw.WriteLine("endpoint:");
        fw.WriteLine($"  name: {endpoint.Name}");
        fw.WriteLine($"  method: {endpoint.Method}");
        fw.WriteLine($"  route: {endpoint.Route}");
        fw.WriteLine($"  description: {endpoint.Description}");
        if (endpoint.Params.Any())
        {
            fw.WriteLine($"  params:");
            foreach (var param in endpoint.Params)
            {
                WriteProperty(param, fw);
            }
        }
        if (endpoint.Returns != null)
        {
            fw.WriteLine();
            fw.WriteLine($"  returns:");
            WriteProperty(endpoint.Returns, fw, false);
        }
    }

    private bool IsAvailable(Class classe)
    {
        return _files.Values.SelectMany(c => c.Classes).Contains(classe);
    }
}