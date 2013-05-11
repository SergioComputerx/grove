﻿namespace Grove.Gameplay.CastingRules
{
  public class Aura : Sorcery
  {
    public override void AfterResolve()
    {
      var attachedCardController = Card.AttachedTo.Controller;
      attachedCardController.PutCardToBattlefield(Card);
    }
  }
}