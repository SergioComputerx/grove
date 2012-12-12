﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Cards.Costs;
  using Core.Cards.Effects;
  using Core.Dsl;
  using Core.Mana;
  using Core.Targeting;

  public class CarrionBeetles : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Carrion Beetles")
        .ManaCost("{B}")
        .Type("Creature Insect")
        .Text("{2}{B},{T}: Exile up to three target cards from a single graveyard.")
        .FlavorText("It's all fun and games until someone loses an eye.")
        .Power(1)
        .Toughness(1)
        .Timing(Timings.Creatures())
        .Abilities(
          ActivatedAbility(
            "{2}{B},{T}: Exile up to three target cards from a single graveyard.",
            Cost<TapOwnerPayMana>(cost =>
              {
                cost.TapOwner = true;
                cost.Amount = "{2}{B}".ParseManaAmount();
              }),
            Effect<ExileTargets>(),
            effectValidator: TargetValidator(
              TargetIs.Card(),
              ZoneIs.Graveyard(),
              minCount: 0,
              maxCount: 3),
            targetSelectorAi: TargetSelectorAi.RemoveCardsFromOpponentsGraveyard(),
            timing: Timings.EndOfTurn())
        );
    }
  }
}