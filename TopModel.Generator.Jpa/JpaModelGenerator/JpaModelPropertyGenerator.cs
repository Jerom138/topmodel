﻿using TopModel.Core;
using TopModel.Core.Model.Implementation;
using TopModel.Generator.Core;
using TopModel.Utils;

namespace TopModel.Generator.Jpa;

/// <summary>
/// Générateur de fichiers de modèles JPA.
/// </summary>
public class JpaModelPropertyGenerator
{
    private readonly JpaConfig _config;

    public JpaModelPropertyGenerator(JpaConfig config)
    {
        _config = config;
    }

    public void WriteProperty(JavaWriter fw, Class classe, IProperty property, string tag)
    {
        fw.WriteLine();
        fw.WriteDocStart(1, property.Comment);
        switch (property)
        {
            case CompositionProperty cp:
                WriteProperty(fw, cp);
                break;
            case AssociationProperty { Association.IsPersistent: true } ap:
                WriteProperty(fw, classe, ap);
                break;
            case IFieldProperty fp:
                WriteProperty(fw, classe, fp, tag);
                break;
        }
    }

    public void WriteProperty(JavaWriter fw, CompositionProperty property)
    {
        fw.WriteDocEnd(1);
        fw.WriteLine(1, $"private {_config.GetJavaType(property)} {property.NameCamel};");
    }

    public void WriteProperties(JavaWriter fw, Class classe, List<Class> availableClasses, string tag)
    {
        foreach (var property in classe.GetProperties(availableClasses, tag))
        {
            WriteProperty(fw, classe, property, tag);
        }
    }

    private void WriteProperty(JavaWriter fw, Class classe, AssociationProperty property)
    {
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        fw.WriteDocEnd(1);
        fw.AddImport($"{javaOrJakarta}.persistence.FetchType");
        fw.AddImport($"{javaOrJakarta}.persistence.{property.Type}");
        switch (property.Type)
        {
            case AssociationType.ManyToOne:
                WriteManyToOne(fw, classe, property);
                break;
            case AssociationType.OneToMany:
                WriteOneToMany(fw, classe, property);
                break;
            case AssociationType.ManyToMany:
                WriteManyToMany(fw, classe, property);
                break;
            case AssociationType.OneToOne:
                WriteOneToOne(fw, classe, property);
                break;
        }

        if (property.Type == AssociationType.ManyToMany || property.Type == AssociationType.OneToMany)
        {
            if (property.Association.OrderProperty != null)
            {
                fw.WriteLine(1, @$"@OrderBy(""{property.Association.OrderProperty.GetJavaName()} ASC"")");
                fw.AddImport($"{javaOrJakarta}.persistence.OrderBy");
            }
        }

        var defaultValue = string.Empty;
        if (!(property.DefaultValue == null || property.DefaultValue == "null" || property.DefaultValue == "undefined"))
        {
            defaultValue += " = ";
            var quote = string.Empty;
            if (property.GetJavaType() == "String")
            {
                quote = @"""";
            }

            if (property is AssociationProperty ap && ap.IsEnum() && ap.Association.Values.Any(r => r.Value.ContainsKey(ap.Association.PrimaryKey.Single()) && r.Value[ap.Association.PrimaryKey.Single()] == property.DefaultValue))
            {
                defaultValue += ap.Association.NamePascal + ".Values." + property.DefaultValue + ".getEntity()";
            }
            else
            {
                defaultValue += quote + property.DefaultValue + quote;
            }
        }

        fw.WriteLine(1, $"private {property.GetJavaType()} {property.GetAssociationName()}{defaultValue};");
    }

    private void WriteManyToOne(JavaWriter fw, Class classe, AssociationProperty property)
    {
        var fk = ((IFieldProperty)property).SqlName;
        var apk = property.Association.PrimaryKey.Single().SqlName;
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        fw.WriteLine(1, @$"@{property.Type}(fetch = FetchType.LAZY, optional = {(property.Required ? "false" : "true")}, targetEntity = {property.Association.NamePascal}.class)");
        fw.WriteLine(1, @$"@JoinColumn(name = ""{fk}"", referencedColumnName = ""{apk}"")");
        fw.AddImport($"{javaOrJakarta}.persistence.JoinColumn");
    }

    private void WriteOneToOne(JavaWriter fw, Class classe, AssociationProperty property)
    {
        var fk = ((IFieldProperty)property).SqlName;
        var apk = property.Association.PrimaryKey.Single().SqlName;
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        fw.AddImport($"{javaOrJakarta}.persistence.CascadeType");
        fw.WriteLine(1, @$"@{property.Type}(fetch = FetchType.LAZY, cascade = CascadeType.ALL, optional = {(!property.Required).ToString().ToLower()})");
        fw.WriteLine(1, @$"@JoinColumn(name = ""{fk}"", referencedColumnName = ""{apk}"", unique = true)");
        fw.AddImport($"{javaOrJakarta}.persistence.JoinColumn");
    }

    private void WriteManyToMany(JavaWriter fw, Class classe, AssociationProperty property)
    {
        var role = property.Role is not null ? "_" + property.Role.ToConstantCase() : string.Empty;
        var fk = ((IFieldProperty)property).SqlName;
        var pk = classe.PrimaryKey.Single().SqlName + role;
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        if (!property.Association.IsStatic())
        {
            fw.AddImport($"{javaOrJakarta}.persistence.CascadeType");
        }

        var cascade = property.Association.IsStatic() ? string.Empty : $", cascade = {{ CascadeType.PERSIST, CascadeType.MERGE }}";
        if (property is ReverseAssociationProperty rap)
        {
            fw.WriteLine(1, @$"@{property.Type}(fetch = FetchType.LAZY, mappedBy = ""{rap.ReverseProperty.GetJavaName()}""{cascade})");
        }
        else
        {
            fw.AddImport($"{javaOrJakarta}.persistence.JoinTable");
            fw.WriteLine(1, @$"@{property.Type}(fetch = FetchType.LAZY{cascade})");
            fw.WriteLine(1, @$"@JoinTable(name = ""{property.Class.SqlName}_{property.Association.SqlName}{(property.Role != null ? "_" + property.Role.ToConstantCase() : string.Empty)}"", joinColumns = @JoinColumn(name = ""{pk}""), inverseJoinColumns = @JoinColumn(name = ""{fk}""))");
            fw.AddImport($"{javaOrJakarta}.persistence.JoinColumn");
        }
    }

    private void WriteOneToMany(JavaWriter fw, Class classe, AssociationProperty property)
    {
        var pk = classe.PrimaryKey.Single().SqlName;
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        fw.AddImport($"{javaOrJakarta}.persistence.CascadeType");
        if (property is ReverseAssociationProperty rap)
        {
            fw.WriteLine(1, @$"@{property.Type}(cascade = {{CascadeType.PERSIST, CascadeType.MERGE}}, fetch = FetchType.LAZY, mappedBy = ""{rap.ReverseProperty.GetJavaName()}"")");
        }
        else
        {
            var hasReverse = property.Class.Namespace.RootModule == property.Association.Namespace.RootModule;
            fw.WriteLine(1, @$"@{property.Type}(cascade = CascadeType.ALL, fetch = FetchType.LAZY{(hasReverse ? @$", mappedBy = ""{property.Class.NameCamel}{property.Role ?? string.Empty}""" : string.Empty)})");
            if (!hasReverse)
            {
                fw.WriteLine(1, @$"@JoinColumn(name = ""{pk}"", referencedColumnName = ""{pk}"")");
                fw.AddImport($"{javaOrJakarta}.persistence.JoinColumn");
            }
        }
    }

    private void WriteProperty(JavaWriter fw, Class classe, IFieldProperty property, string tag)
    {
        var javaOrJakarta = _config.PersistenceMode.ToString().ToLower();
        if (property is AliasProperty alp)
        {
            fw.WriteLine(1, $" * Alias of {{@link {alp.Property.Class.GetImport(_config, tag)}#get{alp.Property.NameCamel.ToFirstUpper()}() {alp.Property.Class.NamePascal}#get{alp.Property.NameCamel.ToFirstUpper()}()}} ");
        }

        fw.WriteDocEnd(1);
        if (property.PrimaryKey && classe.IsPersistent)
        {
            fw.WriteLine(1, "@Id");
            fw.AddImport($"{javaOrJakarta}.persistence.Id");
            var javaDomain = _config.GetImplementation(property.Domain)!;
            if (property.Domain.AutoGeneratedValue)
            {
                fw.AddImports(new List<string>
                {
                    $"{javaOrJakarta}.persistence.GeneratedValue",
                    $"{javaOrJakarta}.persistence.GenerationType"
                });

                if (_config.Identity.Mode == IdentityMode.IDENTITY)
                {
                    fw.WriteLine(1, @$"@GeneratedValue(strategy = GenerationType.IDENTITY)");
                }
                else if (_config.Identity.Mode == IdentityMode.SEQUENCE)
                {
                    fw.AddImport($"{javaOrJakarta}.persistence.SequenceGenerator");
                    var seqName = $"SEQ_{classe.SqlName}";
                    var initialValue = _config.Identity.Start != null ? $", initialValue = {_config.Identity.Start}" : string.Empty;
                    var increment = _config.Identity.Increment != null ? $", allocationSize = {_config.Identity.Increment}" : string.Empty;
                    fw.WriteLine(1, @$"@SequenceGenerator(name = ""{seqName}"", sequenceName = ""{seqName}""{initialValue}{increment})");
                    fw.WriteLine(1, @$"@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = ""{seqName}"")");
                }
            }
        }

        if (classe.IsPersistent && !_config.GetImplementation(property.Domain)!.Annotations
        .Where(i =>
                classe.IsPersistent && (Target.Persisted & i.Target) > 0
            || !classe.IsPersistent && (Target.Dto & i.Target) > 0)
            .Any(a => a.Text.Replace("@", string.Empty).StartsWith("Column")))
        {
            var column = @$"@Column(name = ""{property.SqlName}"", nullable = {(!property.Required).ToString().ToFirstLower()}";
            if (property.Domain.Length != null)
            {
                if (_config.GetImplementation(property.Domain)!.Type.ToUpper() == "STRING")
                {
                    column += $", length = {property.Domain.Length}";
                }
                else
                {
                    column += $", precision = {property.Domain.Length}";
                }
            }

            if (property.Domain.Scale != null)
            {
                column += $", scale = {property.Domain.Scale}";
            }

            column += ")";
            fw.AddImport($"{javaOrJakarta}.persistence.Column");
            fw.WriteLine(1, column);
        }
        else if (property.Required && !property.PrimaryKey && !classe.IsPersistent)
        {
            fw.WriteLine(1, @$"@NotNull");
            fw.AddImport($"{javaOrJakarta}.validation.constraints.NotNull");
        }

        if (property.PrimaryKey && classe.Reference && classe.Values.Any())
        {
            fw.AddImports(new List<string>
            {
                $"{javaOrJakarta}.persistence.Enumerated",
                $"{javaOrJakarta}.persistence.EnumType",
            });
            fw.WriteLine(1, "@Enumerated(EnumType.STRING)");
        }

        var javaType = _config.GetImplementation(property.Domain);
        if (javaType is not null && javaType.Annotations is not null)
        {
            foreach (var annotation in javaType.Annotations.Where(a =>
                classe.IsPersistent && (Target.Persisted & a.Target) > 0
            || !classe.IsPersistent && (Target.Dto & a.Target) > 0))
            {
                fw.AddImports(annotation.Imports.Select(i => i.ParseTemplate(property)));
                fw.WriteLine(1, $"{(annotation.Text.StartsWith("@") ? string.Empty : '@')}{annotation.Text.ParseTemplate(property)}");
            }
        }

        var defaultValue = string.Empty;
        if (!(property.DefaultValue == null || property.DefaultValue == "null" || property.DefaultValue == "undefined"))
        {
            defaultValue += " = ";
            if (property.IsEnum() && classe.IsStatic())
            {
                defaultValue += "Values." + property.DefaultValue;
            }
            else if (property is AliasProperty aslp && (aslp.Property.IsEnum() || aslp.IsAssociatedEnum()))
            {
                if (aslp.Property.IsEnum())
                {
                    defaultValue += aslp.Property.Class.NamePascal + ".Values." + property.DefaultValue;
                }
                else if (aslp.Property is AssociationProperty aslpAss)
                {
                    defaultValue += aslpAss.Association.NamePascal + ".Values." + property.DefaultValue;
                }
            }
            else
            {
                var quote = string.Empty;
                if (_config.GetJavaType(property) == "String")
                {
                    quote = @"""";
                }

                defaultValue += quote + property.DefaultValue + quote;
            }
        }

        fw.WriteLine(1, $"private {_config.GetJavaType(property)} {property.GetJavaName()}{defaultValue};");
    }
}