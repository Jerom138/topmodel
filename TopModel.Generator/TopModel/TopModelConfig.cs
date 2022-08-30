namespace TopModel.Generator.TopModel;

public class TopModelConfig : GeneratorConfigBase
{
#nullable disable
    /// <summary>
    /// Dossier de sortie pour le modèle.
    /// </summary>
    public string ModelOutputDirectory { get; set; }

    /// <summary>
    /// Tag à placer sur les fichiers générés.
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// Liste des équivalences des décorateurs.
    /// </summary>
    public List<DecoratorConfig> Decorators { get; set; } = new List<DecoratorConfig>();

    /// <summary>
    /// Liste des équivalences des domaines.
    /// </summary>
    public List<DomainConfig> Domains { get; set; } = new List<DomainConfig>();
}