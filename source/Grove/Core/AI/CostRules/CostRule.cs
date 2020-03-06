﻿namespace Grove.AI.CostRules
{
  using System.Linq;

  public abstract class CostRule : MachinePlayRule
  {    
    private void Process(ActivationContext c)
    {
      if (c.HasTargets == false)
      {
        var p = new CostRuleParameters(c.Card, c.MaxX.GetValueOrDefault());
        c.X = CalculateX(p);

        if (c.X > c.MaxX)
        {
          c.CancelActivation = true;
        }
        return;
      }

      var targetsCombinations = c.TargetsCombinations().ToList();

      foreach (var targetsCombination in targetsCombinations)
      {
        var p = new CostRuleParameters(c.Card, c.MaxX.GetValueOrDefault(), targetsCombination.Targets);
        targetsCombination.X = CalculateX(p);

        if (targetsCombination.X > c.MaxX)
        {
          c.RemoveTargetCombination(targetsCombination);
        }
      }

      if (c.HasTargets == false)
        c.CancelActivation = true;
    }
    
    public override bool Process(int pass, ActivationContext c)
    {                  
      if (pass == 2)
      {
        Process(c);
        return true;
      }

      return false;
    }

    public abstract int CalculateX(CostRuleParameters p);
  }
}