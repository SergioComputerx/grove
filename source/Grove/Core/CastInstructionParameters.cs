﻿namespace Grove.Core
{
  using System;
  using Casting;
  using Costs;

  public class CastInstructionParameters : AbilityOrCastParameters
  {
    public string KickerDescription = "Cast {0} with kicker.";
    public Cost Cost;    
    public CastingRule Rule;    
  }
}