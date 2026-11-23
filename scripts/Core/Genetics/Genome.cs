namespace LifeSimulator.Core.Genetics;

/// <summary>
/// Herança genética (GDD §6.3): características físicas herdadas dos
/// pais, variações genéticas e predisposições. Cada gene vai de 0 a
/// 100; a herança mistura os genes dos pais com uma mutação leve,
/// influenciando sem determinar o destino do personagem.
/// </summary>
public sealed class Genome
{
    public const int MutationRange = 12;

    // Aparência
    public int SkinTone { get; set; }
    public int HairColor { get; set; }
    public int HairTexture { get; set; }
    public int EyeColor { get; set; }
    public int Height { get; set; }
    public int Build { get; set; }

    // Predisposições
    public int Longevity { get; set; }
    public int Metabolism { get; set; }
    public int ImmuneSystem { get; set; }

    public static Genome Random(Random rng) => new()
    {
        SkinTone = rng.Next(101),
        HairColor = rng.Next(101),
        HairTexture = rng.Next(101),
        EyeColor = rng.Next(101),
        Height = rng.Next(101),
        Build = rng.Next(101),
        Longevity = rng.Next(101),
        Metabolism = rng.Next(101),
        ImmuneSystem = rng.Next(101),
    };

    public static Genome Inherit(Genome mother, Genome father, Random rng) => new()
    {
        SkinTone = Blend(mother.SkinTone, father.SkinTone, rng),
        HairColor = Blend(mother.HairColor, father.HairColor, rng),
        HairTexture = Blend(mother.HairTexture, father.HairTexture, rng),
        EyeColor = Blend(mother.EyeColor, father.EyeColor, rng),
        Height = Blend(mother.Height, father.Height, rng),
        Build = Blend(mother.Build, father.Build, rng),
        Longevity = Blend(mother.Longevity, father.Longevity, rng),
        Metabolism = Blend(mother.Metabolism, father.Metabolism, rng),
        ImmuneSystem = Blend(mother.ImmuneSystem, father.ImmuneSystem, rng),
    };

    private static int Blend(int a, int b, Random rng) =>
        Math.Clamp((a + b) / 2 + rng.Next(-MutationRange, MutationRange + 1), 0, 100);
}
