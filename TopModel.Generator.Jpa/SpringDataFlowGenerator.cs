﻿using System.Data;
using Microsoft.Extensions.Logging;
using TopModel.Core;
using TopModel.Core.FileModel;
using TopModel.Generator.Core;
using TopModel.Utils;

namespace TopModel.Generator.Jpa;

public class SpringDataFlowGenerator : GeneratorBase<JpaConfig>
{
    private readonly ILogger<SpringDataFlowGenerator> _logger;

    public SpringDataFlowGenerator(ILogger<SpringDataFlowGenerator> logger)
        : base(logger)
    {
        _logger = logger;
    }

    public override IEnumerable<string> GeneratedFiles => Files.Values.SelectMany(f => f.DataFlows)
        .SelectMany(df => Config.Tags.Intersect(df.ModelFile.Tags)
            .SelectMany(tag => new[] { Config.GetDataFlowFilePath(df, tag) }))
        .Distinct()
        .Concat(Files.Values.Where(f => f.DataFlows.Any()).Select(f => Config.GetDataFlowConfigFilePath(f.Namespace.Module)))
        ;

    public override string Name => "SpringDataFlowGen";

    protected override void HandleFiles(IEnumerable<ModelFile> files)
    {
        foreach (var file in files)
        {
            foreach (var dataFlow in file.DataFlows)
            {
                foreach (var (tag, fileName) in Config.Tags.Intersect(file.Tags)
                    .Select(tag => (tag, fileName: Config.GetDataFlowFilePath(dataFlow, tag)))
                    .DistinctBy(t => t.fileName))
                {
                    HandleDataFlow(fileName, dataFlow, tag);
                }
            }
        }

        foreach (var module in files.Where(f => f.DataFlows.Any()).Select(f => f.Namespace.Module))
        {
            WriteModuleConfig(module, files.Where(f => f.Namespace.Module == module).SelectMany(f => f.DataFlows));
        }
    }

    private static void WriteBeanFlow(JavaWriter fw, DataFlow dataFlow)
    {
        fw.AddImport("org.springframework.context.annotation.Bean");
        fw.AddImport("org.springframework.batch.core.job.flow.Flow");
        fw.AddImport("org.springframework.beans.factory.annotation.Qualifier");
        fw.AddImport("org.springframework.batch.core.Step");
        fw.AddImport("org.springframework.batch.core.job.builder.FlowBuilder");

        fw.WriteLine();
        fw.WriteLine(1, @$"@Bean(""{dataFlow.Name.ToPascalCase()}Flow"")");
        fw.WriteLine(1, @$"public static Flow {dataFlow.Name.ToCamelCase()}Flow(");
        if (dataFlow.Type == DataFlowType.Replace)
        {
            fw.WriteLine(1, @$"			@Qualifier(""{dataFlow.Name.ToPascalCase()}TruncateStep"") Step {dataFlow.Name.ToCamelCase()}TruncateStep,");
        }

        if (dataFlow.Hooks.Contains(FlowHook.BeforeFlow))
        {
            fw.WriteLine(1, @$"			@Qualifier(""{dataFlow.Name.ToPascalCase()}BeforeFLowStep"") Step {dataFlow.Name.ToCamelCase()}BeforeFLowStep,");
        }

        if (dataFlow.Hooks.Contains(FlowHook.AfterFlow))
        {
            fw.WriteLine(1, @$"			@Qualifier(""{dataFlow.Name.ToPascalCase()}AfterFLowStep"") Step {dataFlow.Name.ToCamelCase()}AfterFLowStep,");
        }

        fw.WriteLine(1, @$"			@Qualifier(""{dataFlow.Name.ToPascalCase()}Step"") Step {dataFlow.Name.ToCamelCase()}Step) {{");
        fw.WriteLine(2, @$"return new FlowBuilder<Flow>(""{dataFlow.Name.ToPascalCase()}Flow"") //");
        var isFirst = true;
        if (dataFlow.Hooks.Contains(FlowHook.BeforeFlow))
        {
            fw.WriteLine(3, @$".start({dataFlow.Name.ToCamelCase()}BeforeFLowStep) //");
            isFirst = false;
        }

        if (dataFlow.Type == DataFlowType.Replace)
        {
            fw.WriteLine(3, @$".{(isFirst ? "start" : "next")}({dataFlow.Name.ToCamelCase()}TruncateStep) //");
            fw.WriteLine(3, @$".next({dataFlow.Name.ToCamelCase()}Step) //");
        }
        else
        {
            fw.WriteLine(3, @$".{(isFirst ? "start" : "next")}({dataFlow.Name.ToCamelCase()}Step) //");
        }

        if (dataFlow.Hooks.Contains(FlowHook.AfterFlow))
        {
            fw.WriteLine(3, @$".next({dataFlow.Name.ToCamelCase()}AfterFLowStep) //");
        }

        fw.WriteLine(3, ".build();");
        fw.WriteLine(1, "}");
    }

    private static void WriteBeanTruncateStep(JavaWriter fw, DataFlow dataFlow)
    {
        fw.WriteLine();
        fw.AddImport("io.github.kleecontrib.spring.batch.tasklet.QueryTasklet");
        fw.WriteLine(1, @$"@Bean(""{dataFlow.Name.ToPascalCase()}TruncateStep"")");
        fw.WriteLine(1, @$"public static Step {dataFlow.Name.ToCamelCase()}TruncateStep( //");
        fw.WriteLine(1, @$"		JobRepository jobRepository, //");
        fw.WriteLine(1, @$"		PlatformTransactionManager transactionManager, //");
        fw.WriteLine(1, @$"		@Qualifier(""{dataFlow.Target}"") DataSource dataSource) {{");
        fw.WriteLine(1, @$"return new StepBuilder(""{dataFlow.Name.ToPascalCase()}TruncateStep"", jobRepository) //");
        fw.WriteLine(2, @$"		.tasklet(new QueryTasklet(dataSource, ""truncate table {dataFlow.Class.SqlName}""), transactionManager) //");

        fw.WriteLine(3, ".build();");
        fw.WriteLine(1, "}");
    }

    private void HandleDataFlow(string fileName, DataFlow dataFlow, string tag)
    {
        var packageName = Config.ResolveVariables(
            Config.DataFlowsPath!,
            tag,
            module: dataFlow.ModelFile.Namespace.Module).ToPackageName();

        using var fw = new JavaWriter(fileName, _logger, packageName);
        WriteClassFlow(fw, dataFlow, tag);
    }

    private void WriteBeanReader(JavaWriter fw, DataFlow dataFlow, string tag)
    {
        fw.WriteLine();
        var tagToUse = tag;
        if (!dataFlow.Class.ModelFile.Tags.Contains(tag))
        {
            tagToUse = Config.Tags.Intersect(dataFlow.Sources.First().Class.ModelFile.Tags).First();
        }

        var properties = dataFlow.Sources[0].Class.Properties.OfType<IFieldProperty>();
        var query = $"select {string.Join(", ", properties.Select(p => $"{p.SqlName} as {p.NameCamel}"))} from {(Config.ResolveVariables(Config.DbSchema!, tag: tagToUse) == null ? string.Empty : $"{Config.ResolveVariables(Config.DbSchema!, tag: tagToUse)}.")}{dataFlow.Sources.First().Class.SqlName}";
        if (dataFlow.Sources.First().Mode == DataFlowSourceMode.Partial)
        {
            query = string.Empty;
        }

        fw.AddImport("org.springframework.batch.item.database.builder.JdbcCursorItemReaderBuilder");
        fw.WriteLine(1, @$"@Bean(""{dataFlow.Name.ToPascalCase()}Reader"")");
        fw.WriteLine(1, @$"public static ItemReader<{dataFlow.Sources.First().Class.NamePascal}> {dataFlow.Name.ToCamelCase()}Reader( //");
        fw.WriteLine(1, @$"		@Qualifier(""{dataFlow.Sources.First().Source}"") DataSource datasource) {{");
        fw.WriteLine(2, $"return new JdbcCursorItemReaderBuilder<{dataFlow.Sources.First().Class.NamePascal}>() //");
        fw.WriteLine(2, @$"		.name(""{dataFlow.Name.ToPascalCase()}Reader"") //");
        fw.WriteLine(2, @$"		.beanRowMapper({dataFlow.Sources.First().Class.NamePascal}.class) //");
        fw.WriteLine(2, @$"		.sql(""{query}"") //");
        fw.WriteLine(2, @$"		.fetchSize({Config.DataFlowsBulkSize}) //");
        fw.WriteLine(2, @$"		.dataSource(datasource) //");
        fw.WriteLine(2, @$"		.build();");
        fw.WriteLine(1, "}");
    }

    private void WriteBeanStep(JavaWriter fw, DataFlow dataFlow, string tag)
    {
        fw.AddImport("org.springframework.batch.core.step.builder.StepBuilder");
        fw.AddImport("org.springframework.batch.core.repository.JobRepository");
        fw.AddImport("org.springframework.transaction.PlatformTransactionManager");
        fw.AddImport("org.springframework.batch.item.ItemReader");
        fw.AddImport("org.springframework.batch.item.ItemWriter");
        fw.AddImport(dataFlow.Sources.First().Class.GetImport(Config, tag));
        fw.AddImport(dataFlow.Class.GetImport(Config, tag));

        var hookStep = dataFlow.Hooks.Where(h => h != FlowHook.BeforeFlow && h != FlowHook.AfterFlow);

        fw.WriteLine();
        fw.WriteLine(1, @$"@Bean(""{dataFlow.Name.ToPascalCase()}Step"")");
        fw.WriteLine(1, @$"public static Step {dataFlow.Name.ToCamelCase()}Step(");
        fw.WriteLine(1, @$"		JobRepository jobRepository, //");
        fw.WriteLine(1, @$"		PlatformTransactionManager transactionManager, //");
        foreach (var listener in Config.DataFlowsListeners)
        {
            fw.AddImport("org.springframework.batch.core.StepListener");
            fw.WriteLine(1, @$"		@Qualifier(""{listener}"") StepListener {listener.ToCamelCase()}, //");
        }

        fw.WriteLine(1, @$"		@Qualifier(""{dataFlow.Name.ToPascalCase()}Reader"") ItemReader<{dataFlow.Sources.First().Class.NamePascal}> reader, //");
        fw.WriteLine(1, @$"		@Qualifier(""{dataFlow.Name.ToPascalCase()}Writer"") ItemWriter<{dataFlow.Class.NamePascal}> writer{(hookStep.Any() ? ',' : string.Empty)} //");
        var index = 0;
        foreach (var hook in hookStep)
        {
            var isLast = index == hookStep.Count() - 1;
            var processorName = GetProcessorName(dataFlow, hook, index).ToCamelCase();
            var processorSourceClass = GetProcessorSourceClass(hook, dataFlow);
            var processorTargetClass = GetProcessorTargetClass(hook, dataFlow);
            fw.AddImport("org.springframework.batch.item.ItemProcessor");
            fw.WriteLine(1, @$"		@Qualifier(""{GetProcessorName(dataFlow, hook, index)}"") ItemProcessor<{processorSourceClass}, {processorTargetClass}> {processorName}{(isLast ? string.Empty : ",")} //");
            index++;
        }

        var processors = new List<string>();
        fw.WriteLine(1, ") {");
        fw.WriteLine(2, @$"return new StepBuilder(""{dataFlow.Name.ToPascalCase()}Step"", jobRepository) //");
        fw.WriteLine(3, @$".<{dataFlow.Sources.First().Class.NamePascal}, {dataFlow.Class.NamePascal}>chunk({Config.DataFlowsBulkSize}, transactionManager) //");
        fw.WriteLine(3, ".reader(reader) //");
        var i = 0;
        foreach (var t in dataFlow.Hooks.Where(h => h == FlowHook.AfterSource))
        {
            processors.Add(GetProcessorName(dataFlow, FlowHook.AfterSource, i++).ToCamelCase());
        }

        if (dataFlow.Hooks.Contains(FlowHook.Map))
        {
            foreach (var t in dataFlow.Hooks.Where(h => h == FlowHook.Map))
            {
                processors.Add(GetProcessorName(dataFlow, FlowHook.Map, 0).ToCamelCase());
            }
        }
        else if (dataFlow.Sources.First().Class != dataFlow.Class)
        {
            processors.Add($"({dataFlow.Sources[0].Class.NamePascal} item) -> new {dataFlow.Class.NamePascal}(item)");
        }

        i = 0;
        foreach (var t in dataFlow.Hooks.Where(h => h == FlowHook.BeforeTarget))
        {
            processors.Add(GetProcessorName(dataFlow, FlowHook.BeforeTarget, i++).ToCamelCase());
        }

        if (processors.Count() == 1)
        {
            fw.WriteLine(3, $".processor({processors[0]}) //");
        }
        else if (processors.Count() > 1)
        {
            fw.AddImport("org.springframework.batch.item.support.CompositeItemProcessor");
            fw.WriteLine(3, $".processor(new CompositeItemProcessor<>({string.Join(", ", processors)})) //");
        }

        foreach (var listener in Config.DataFlowsListeners)
        {
            fw.WriteLine(3, $".listener({listener.ToCamelCase()}) //");
        }

        fw.WriteLine(3, ".writer(writer) //");
        fw.WriteLine(3, ".build();");
        fw.WriteLine(1, "}");
    }

    private string GetProcessorSourceClass(FlowHook flowHook, DataFlow flow)
    {
        return flowHook switch
        {
            FlowHook.AfterSource or FlowHook.Map => flow.Sources[0].Class.NamePascal,
            FlowHook.BeforeTarget => flow.Class.NamePascal,
            _ => string.Empty,
        };
    }

    private string GetProcessorTargetClass(FlowHook flowHook, DataFlow flow)
    {
        return flowHook switch
        {
            FlowHook.AfterSource => flow.Sources[0].Class.NamePascal,
            FlowHook.BeforeTarget or FlowHook.Map => flow.Class.NamePascal,
            _ => string.Empty,
        };
    }

    private string GetProcessorName(DataFlow dataFlow, FlowHook flowHook, int index)
    {
        var suffix = string.Empty;
        if (dataFlow.Hooks.Where(h => h == flowHook).Count() > 1)
        {
            suffix = "_" + dataFlow.Hooks.Take(index + 1).Where(h => h == flowHook).Count();
        }

        return $@"{dataFlow.Name.ToPascalCase()}{flowHook}{suffix}";
    }

    private void WriteBeanWriter(JavaWriter fw, DataFlow dataFlow, string tag)
    {
        fw.AddImport("io.github.kleecontrib.spring.batch.bulk.upsert.BulkItemWriter");
        fw.AddImport("javax.sql.DataSource");

        fw.WriteLine();
        fw.WriteLine(1, @$"@Bean(""{dataFlow.Name.ToPascalCase()}Writer"")");
        fw.WriteLine(1, @$"public static ItemWriter<{dataFlow.Class.NamePascal}> {dataFlow.Name.ToCamelCase()}Writer(@Qualifier(""{dataFlow.Target}"") DataSource targetDataSource) {{");
        fw.WriteLine(2, @$"return new BulkItemWriter<>(targetDataSource, new {dataFlow.Class.NamePascal}Mapping());");
        fw.WriteLine(1, "}");

        WriteBulkMappingWriter(fw, dataFlow, tag);
    }

    private void WriteBulkMappingWriter(JavaWriter fw, DataFlow dataFlow, string tag)
    {
        fw.WriteLine();
        if (dataFlow.Type != DataFlowType.Merge)
        {
            fw.AddImport("de.bytefish.pgbulkinsert.mapping.AbstractMapping");
            fw.WriteLine(1, @$"private static class {dataFlow.Class.NamePascal}Mapping extends AbstractMapping<{dataFlow.Class.NamePascal}> {{");
        }
        else
        {
            fw.AddImport("io.github.kleecontrib.spring.batch.bulk.mapping.AbstractUpsertMapping");
            fw.WriteLine(1, @$"private static class {dataFlow.Class.NamePascal}Mapping extends AbstractUpsertMapping<{dataFlow.Class.NamePascal}> {{");
        }

        fw.WriteLine(2, @$"public {dataFlow.Class.NamePascal}Mapping() {{");

        var tagToUse = tag;

        if (!dataFlow.Class.ModelFile.Tags.Contains(tag))
        {
            tagToUse = Config.Tags.Intersect(dataFlow.Class.ModelFile.Tags).First();
        }

        if (dataFlow.Type != DataFlowType.Merge)
        {
            fw.WriteLine(3, @$"super(""{Config.ResolveVariables(Config.DbSchema!, tag: tagToUse)}"", ""{dataFlow.Class.SqlName}"");");
        }
        else
        {
            fw.WriteLine(3, @$"super(""{Config.ResolveVariables(Config.DbSchema!, tag: tagToUse)}"", ""{dataFlow.Class.SqlName}"", ""{string.Join(',', dataFlow.Class.PrimaryKey.Select(pk => pk.SqlName))}"");");
        }

        fw.AddImport("de.bytefish.pgbulkinsert.pgsql.constants.DataType");
        var mapper = dataFlow.Class.FromMappers.Where(m =>
            {
                var result = true;
                for (int i = 0; i < m.Params.Count; i++)
                {
                    result = result && m.Params[i].Class == dataFlow.Sources[i].Class;
                }

                return result;
            }).FirstOrDefault();
        if (dataFlow.Hooks.Contains(FlowHook.Map))
        {
            mapper = null;
        }

        foreach (var property in dataFlow.Class.Properties.OfType<IFieldProperty>()
            .Where(p => mapper == null || mapper.Params.SelectMany(pa => pa.Mappings).Select(mapping => mapping.Key).Contains(p)))
        {
            var sqlType = property.Domain.Implementations["sql"].Type ?? string.Empty;
            string dataType = sqlType.ToUpper() switch
            {
                "VARCHAR" => "VarChar",
                _ => sqlType.ToPascalCase()
            };
            fw.WriteLine(3, $@"map(""{property.SqlName}"", DataType.{dataType}, {dataFlow.Class.NamePascal}::{(Config.GetType(property) == "boolean" ? "is" : "get")}{property.NamePascal.ToFirstUpper()});");
        }

        fw.WriteLine(2, "}");
        fw.WriteLine(1, "}");
    }

    private void WriteClassFlow(JavaWriter fw, DataFlow dataFlow, string tag)
    {
        fw.AddImport("org.springframework.context.annotation.Configuration");
        fw.WriteLine();
        fw.WriteLine("@Configuration");
        var javaOrJakarta = Config.PersistenceMode.ToString().ToLower();
        fw.AddImport($"{javaOrJakarta}.annotation.Generated");
        fw.WriteLine("@Generated(\"TopModel : https://github.com/klee-contrib/topmodel\")");
        fw.WriteClassDeclaration($"{dataFlow.Name}Flow", null);
        WriteBeanFlow(fw, dataFlow);
        WriteBeanStep(fw, dataFlow, tag);

        if (dataFlow.Type == DataFlowType.Replace)
        {
            WriteBeanTruncateStep(fw, dataFlow);
        }

        if (dataFlow.Sources[0].Mode == DataFlowSourceMode.QueryAll)
        {
            WriteBeanReader(fw, dataFlow, tag);
        }

        WriteBeanWriter(fw, dataFlow, tag);
        fw.WriteLine("}");
    }

    private void WriteModuleConfig(string module, IEnumerable<DataFlow> flows)
    {
        var configFilePath = Config.GetDataFlowConfigFilePath(module);
        var packageName = Config.ResolveVariables(
            Config.DataFlowsPath!,
            module: module).ToPackageName();
        using var fw = new JavaWriter(configFilePath, _logger, packageName);
        fw.AddImport("org.springframework.context.annotation.Configuration");
        fw.AddImport("org.springframework.context.annotation.Bean");
        fw.AddImport("org.springframework.batch.core.Job");
        fw.AddImport("org.springframework.batch.core.repository.JobRepository");
        fw.AddImport("org.springframework.beans.factory.annotation.Qualifier");
        fw.AddImport("org.springframework.batch.core.job.flow.Flow");
        fw.AddImport("org.springframework.batch.core.job.builder.JobBuilder");
        fw.AddImport("org.springframework.batch.core.launch.support.RunIdIncrementer");
        fw.AddImport("org.springframework.batch.core.job.builder.FlowBuilder");
        fw.AddImport("org.springframework.core.task.TaskExecutor");
        fw.AddImport("org.springframework.context.annotation.Import");
        fw.WriteLine();
        fw.WriteLine("@Configuration");
        var javaOrJakarta = Config.PersistenceMode.ToString().ToLower();
        fw.AddImport($"{javaOrJakarta}.annotation.Generated");
        fw.WriteLine("@Generated(\"TopModel : https://github.com/klee-contrib/topmodel\")");
        fw.WriteLine(@$"@Import({{{string.Join(", ", flows.Select(f => $@"{f.Name.ToPascalCase()}Flow.class"))}}})");

        var className = configFilePath.Split("\\").Last().Split('.').First();
        fw.WriteClassDeclaration($"{className}", null);
        fw.WriteLine(1, @$"@Bean(""{module.ToPascalCase()}Job"")");
        fw.WriteLine(1, @$"public Job {module.ToCamelCase()}Job( //");
        fw.WriteLine(1, @$"			JobRepository jobRepository, //");
        fw.WriteLine(1, @$"			TaskExecutor taskExecutor, //");
        fw.WriteLine("			" + string.Join(", //\n			", flows.Select(f => $@"@Qualifier(""{f.Name}Flow"") Flow {f.Name.ToCamelCase()}Flow")));
        fw.WriteLine(1, ") {");
        fw.WriteLine(2, $@"return new JobBuilder(""{module.ToPascalCase()}Job"", jobRepository) //");
        fw.WriteLine(2, $@"		.incrementer(new RunIdIncrementer()) //");

        var flowTree = new FlowTree(flows.ToList());
        fw.WriteLine(2, $"		.start({flowTree.ToFlow(0)})");
        fw.WriteLine(2, "		.end()");
        fw.WriteLine(2, "		.build();");
        fw.WriteLine(1, "}");
        fw.WriteLine("}");
    }
}
