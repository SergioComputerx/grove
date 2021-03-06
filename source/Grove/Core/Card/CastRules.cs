﻿namespace Grove
{
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;

  public class CastRules : GameObject, ICopyContributor
  {
    private readonly CardBase _cardBase;
    private readonly Characteristic<List<CastRule>> _castRules;

    private CastRules() {}

    public CastRules(CardBase cardBase)
    {
      _cardBase = cardBase;

      _castRules = new Characteristic<List<CastRule>>(cardBase.Value.CastInstructions);
    }

    public bool HasXInCost { get { return _castRules.Value.Any(x => x.HasXInCost); } }
    public int Count { get { return _castRules.Value.Count; } }

    public void AfterMemberCopy(object original)
    {
      _cardBase.Changed += OnCardBaseChanged;
    }

    public List<ActivationPrerequisites> GetPrerequisites(
      bool payManaCost,
      bool shouldFullyEvaluateEvenIfCannotBePlayed)
    {
      var result = new List<ActivationPrerequisites>();

      for (var index = 0; index < _castRules.Value.Count; index++)
      {
        var castRule = _castRules.Value[index];
        var prerequisites = castRule.GetPrerequisites(payManaCost, 
          shouldFullyEvaluateEvenIfCannotBePlayed);        
        
        prerequisites.Index = index;
        result.Add(prerequisites);        
      }

      return result;
    }

    public void Initialize(Card card, Game game)
    {
      Game = game;

      _castRules.Initialize(game, card);
      _cardBase.Changed += OnCardBaseChanged;
    }

    public void Cast(int index, ActivationParameters activationParameters)
    {
      _castRules.Value[index].Cast(activationParameters);
    }

    public bool CanTarget(ITarget target)
    {
      return _castRules.Value.Any(x => x.CanTarget(target));
    }

    public bool IsGoodTarget(ITarget target, Player controller)
    {
      return _castRules.Value.Any(x => x.IsGoodTarget(target, controller));
    }

    public ManaAmount GetManaCost(int index)
    {
      return _castRules.Value[index].GetManaCost();
    }

    private void OnCardBaseChanged()
    {
      _castRules.ChangeBaseValue(_cardBase.Value.CastInstructions);
    }
  }
}