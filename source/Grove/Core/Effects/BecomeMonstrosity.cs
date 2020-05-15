﻿namespace Grove.Effects
{
  using AI;
  using Modifiers;

  public class BecomeMonstrous : Effect
  {
    private readonly int _counterCount;

    private BecomeMonstrous() {}

    public BecomeMonstrous(int counterCount)
    {
      _counterCount = counterCount;

      SetTags(EffectTag.IncreasePower, EffectTag.IncreaseToughness);
    }

    protected override void ResolveEffect()
    {
      if(Source.OwningCard.Has().Monstrosity)
        return;

      var p = new ModifierParameters
      {
        SourceEffect = this,
        SourceCard = Source.OwningCard,
        X = X
      };

      var modifier1 = new AddCounters(() => new PowerToughness(1, 1), count: _counterCount);
      var modifier2 = new AddSimpleAbility(Static.Monstrosity);

      Source.OwningCard.AddModifier(modifier1, p);
      Source.OwningCard.AddModifier(modifier2, p);
    }
  }
}
