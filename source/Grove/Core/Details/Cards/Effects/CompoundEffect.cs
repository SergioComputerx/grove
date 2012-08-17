﻿namespace Grove.Core.Details.Cards.Effects
{
  using System.Collections.Generic;
  using System.Linq;

  public class CompoundEffect : Effect
  {
    private readonly List<Effect> _childEffects = new List<Effect>();
    private readonly List<IEffectFactory> _effectFactories = new List<IEffectFactory>();

    public void ChildEffects(params IEffectFactory[] effectFactories)
    {
      _effectFactories.AddRange(effectFactories);
    }

    public override int CalculateCreatureDamage(Card creature)
    {
      return _childEffects.Sum(x => x.CalculateCreatureDamage(creature));            
    }

    public override int CalculatePlayerDamage(Player player)
    {
      return _childEffects.Sum(x => x.CalculatePlayerDamage(player));
    }

    public override bool NeedsTargets
    {
      get { return _childEffects.Any(x => x.NeedsTargets); }
    }

    protected override void Init()
    {
      foreach (var effectFactory in _effectFactories)
      {
        _childEffects.Add(effectFactory.CreateEffect(
          new EffectParameters(
            source: Source,
            targets: GetAllTargets())));
      }
    }

    protected override void ResolveEffect()
    {
      foreach (var effect in _childEffects)
      {
        effect.Resolve();
      }
    }
  }
}