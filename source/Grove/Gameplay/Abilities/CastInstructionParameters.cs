﻿namespace Grove.Gameplay.Abilities
{
  using CastingRules;
  using Costs;

  public class CastInstructionParameters : AbilityOrCastParameters
  {
    public Cost Cost;
    public string KickerDescription = "Cast {0} with kicker.";
    public CastingRule Rule;
  }
}