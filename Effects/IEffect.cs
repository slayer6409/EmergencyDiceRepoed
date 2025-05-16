namespace RepoDice.Effects;

public enum EffectType
{
    Awful,
    Bad,
    Mixed,
    Good,
    Great,
    Horrendous,
    Abhorrent,
    Rainbow
}

public interface IEffect
{
    EffectType Outcome { get; }
    bool ShowDefaultTooltip { get; }
    string Name { get; }
    string Tooltip { get; }
    void Use(PlayerAvatar? roller);
}