﻿namespace Grove.UserInterface.TurnNumber
{
  using Events;
  using Infrastructure;

  public class ViewModel : ViewModelBase, IReceive<TurnStarted>
  {
    public virtual int Number { get; protected set; }

    public void Receive(TurnStarted message)
    {
      Number = message.TurnCount;
    }

    public override void Initialize()
    {
      Number = Game.Turn.TurnCount;
    }
  }
}