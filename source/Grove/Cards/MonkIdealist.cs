﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai.TargetingRules;
  using Core.Ai.TimingRules;
  using Core.Dsl;
  using Core.Effects;
  using Core.Triggers;
  using Core.Zones;

  public class MonkIdealist : CardsSource
  {
    public override IEnumerable<CardFactory> GetCards()
    {
      yield return Card
        .Named("Monk Idealist")
        .ManaCost("{2}{W}")
        .Type("Creature - Human Monk Cleric")
        .Text(
          "When Monk Idealist enters the battlefield, return target enchantment card from your graveyard to your hand.")
        .FlavorText("Belief is the strongest mortar.")
        .Power(2)
        .Toughness(2)
        .Cast(p =>
          {
            p.TimingRule(new FirstMain());
            p.TimingRule(new ControllerGravayardCountIs(minCount: 1, selector: c => c.Is().Enchantment));
          })
        .TriggeredAbility(p =>
          {
            p.Text =
              "When Monk Idealist enters the battlefield, return target enchantment card from your graveyard to your hand.";
            p.Trigger(new OnZoneChanged(to: Zone.Battlefield));
            p.Effect = () => new ReturnToHand();
            p.TargetSelector.AddEffect(trg =>
              {
                trg.Is.Enchantment().In.YourGraveyard();
                trg.Message = "Select an enchantment in your graveyard.";
              });

            p.TargetingRule(new OrderByRank(c => -c.Score));
          });
    }
  }
}