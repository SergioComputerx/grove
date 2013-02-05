﻿namespace Grove.Core.Effects
{
  using System;
  using Grove.Core.Decisions;
  using Grove.Core.Decisions.Results;
  using Grove.Infrastructure;
  using Grove.Core.Zones;

  public class SacPermanentOrSacrificeOwner : Effect, IProcessDecisionResults<ChosenCards>
  {
    public Func<Player, Card, bool> ShouldPayAi = delegate { return true; };
    public string Text = "Select permanent to sacrifice";
    public Func<Card, bool> Validator = delegate { return true; };

    public void ProcessResults(ChosenCards results)
    {
      if (results.None())
      {
        Source.OwningCard.Sacrifice();
      }
    }

    protected override void ResolveEffect()
    {
      Game.Enqueue<SelectCardsToSacrificeAsCost>(Controller, p =>
        {
          p.Ai = ShouldPayAi;
          p.QuestionText = FormatText("Pay upkeep cost?");
          p.Validator = Validator;
          p.Zone = Zone.Battlefield;
          p.MinCount = 1;
          p.MaxCount = 1;
          p.CardToPayUpkeepFor = Source.OwningCard;
          p.Text = Text;
          p.ProcessDecisionResults = this;
        });
    }
  }
}