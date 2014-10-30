﻿namespace Grove.CardsLibrary
{
  using System.Collections.Generic;
  using Modifiers;

  public class TuskguardCaptain : CardTemplateSource
  {
    public override IEnumerable<CardTemplate> GetCards()
    {
      yield return Card
        .Named("Tuskguard Captain")
        .ManaCost("{2}{G}")
        .Type("Creature - Human Warrior")
        .Text("Outlast {G}{I}({G}, {T}: Put a +1/+1 counter on this creature. Outlast only as a sorcery.){/I}{EOL}Each creature you control with a +1/+1 counter on it has trample.")
        .FlavorText("One quiet word sets off the stampede.")
        .Power(2)
        .Toughness(3)
        .Outlast("{G}")
        .ContinuousEffect(p =>
        {
          p.CardFilter = (card, effect) =>
            card.Is().Creature &&
            card.CountersCount(CounterType.PowerToughness) > 0 &&
            card.Controller == effect.Source.Controller;
          p.Modifier = () => new AddStaticAbility(Static.Trample);
        });
    }
  }
}
