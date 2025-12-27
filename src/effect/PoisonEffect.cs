using Godot;

namespace FirstGodotGame;

public class PoisonEffect : Effect
{
    public override string Name => "Poison";
    public override string Description => $"Take {Intensity} Poison Damage for {Duration} Rounds";
    public override string IconPath => "Poison_Icon.png";

    public override Effect Copy()
    {
        return new PoisonEffect() //susceptible to copy mistakes...
        {
            Intensity = Intensity,
            Duration = Duration,
        };
    }

    public override void TurnStartTrigger(EntityStats entityStats)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Shot);
        entityStats.TakePierceDamage(Intensity);
        Intensity--;
        Duration--;
    }

    public override void TurnEndTrigger(EntityStats entityStats)
    {
        return;
    }

    public override void StackEffect(Effect effect)
    {
        Intensity += effect.Intensity;
        Duration += effect.Duration;
    }
}