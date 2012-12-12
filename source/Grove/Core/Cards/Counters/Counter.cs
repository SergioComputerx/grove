﻿namespace Grove.Core.Cards.Counters
{
  using Grove.Core.Dsl;
  using Grove.Infrastructure;

  [Copyable]
  public abstract class Counter
  {
    protected ChangeTracker ChangeTracker { get { return Game.ChangeTracker; } }
    protected Game Game { get; set; }
    public virtual void ModifyPower(Power power) {}
    public virtual void ModifyToughness(Toughness toughness) {}

    public abstract void Remove();

    [Copyable]
    public class Factory<T> : ICounterFactory where T : Counter, new()
    {      
      public Initializer<T> Init { get; set; }

      public Counter Create(Game game)
      {
        var counter = new T();
        counter.Game = game;
        Init(counter);

        return counter;
      }
    }
  }
}