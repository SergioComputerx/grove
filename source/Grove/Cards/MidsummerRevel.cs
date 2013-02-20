﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TimingRules;
  using Core.Costs;
  using Core.Counters;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;
  using Core.Modifiers;
  using Core.Triggers;

  public class MidsummerRevel : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Midsummer Revel")
        .ManaCost("{3}{G}{G}")
        .Type("Enchantment")
        .Text(
          "At the beginning of your upkeep, you may put a verse counter on Midsummer Revel.{EOL}{G},Sacrifice Midsummer Revel: Put X 3/3 green Beast creature tokens onto the battlefield, where X is the number of verse counters on Midsummer Revel.")
        .Cast(p => p.TimingRule(new SecondMain()))
        .TriggeredAbility(p =>
          {
            p.Text = "At the beginning of your upkeep, you may put a verse counter on Midsummer Revel.";
            p.Trigger(new OnStepStart(Step.Upkeep));
            p.Effect = () => new ApplyModifiersToSelf(() => new AddCounters(() => new ChargeCounter(), 1));
            p.TriggerOnlyIfOwningCardIsInPlay = true;
          })
        .ActivatedAbility(p =>
          {
            p.Text =
              "{G},Sacrifice Midsummer Revel: Put X 3/3 green Beast creature tokens onto the battlefield, where X is the number of verse counters on Midsummer Revel.";
            p.Cost = new AggregateCost(
              new PayMana(ManaAmount.Green, ManaUsage.Abilities),
              new Sacrifice());
            p.Effect = () => new CreateTokens(
              count: e => e.Source.OwningCard.Counters.GetValueOrDefault(),
              token: Card
                .Named("Beast Token")
                .FlavorText(
                  "All we know about the Krosan Forest we have learned from those few who made it out alive.{EOL}—Elvish refugee")
                .Power(3)
                .Toughness(3)
                .Type("Creature Token Beast")
                .Colors(ManaColors.Green));

            p.TimingRule(new ChargeCounters(3));
          });
    }
  }
}