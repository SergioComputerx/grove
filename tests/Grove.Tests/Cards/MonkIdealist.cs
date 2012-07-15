﻿namespace Grove.Tests.Cards
{
  using Core.Zones;
  using Infrastructure;
  using Xunit;

  public class MonkIdealist
  {
    public class Ai : AiScenario
    {
      [Fact]
      public void ReturnEnchantmentFromGraveyard()
      {
        var pariah = C("Pariah");
        var idealist = C("Monk Idealist");

        Hand(P1, idealist);
        Graveyard(P1, pariah);
        Battlefield(P1, "Plains", "Plains", "Plains");

        RunGame(1);

        Equal(Zone.Hand, C(pariah).Zone);
      }
    }
  }
}