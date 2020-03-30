﻿namespace Grove.UserInterface
{
  using System;
  using System.Linq;
  using System.Threading;

  public class CardViewModel : ViewModelBase, IDisposable
  {
    private readonly Timer _timer;

    public CardViewModel(Card card)
    {
      Card = card;
      Colors = new CardColor[] {};
      
      Update();

      _timer = new Timer(delegate { Update(); }, null,
        TimeSpan.FromMilliseconds(20),
        TimeSpan.FromMilliseconds(20));
    }

    public Card Card { get; private set; }

    public string Name { get { return Card.Name; } }
    public bool HasXInCost { get { return Card.HasXInCost; } }
    public ManaAmount ManaCost { get { return Card.ManaCost; } }
    public string Illustration { get { return Card.Illustration; } }
    public CardText Text { get { return Card.Text; } }
    public CardText FlavorText { get { return Card.FlavorText; } }
    public int CharacterCount { get { return Card.CharacterCount; } }
    public virtual int? Power { get; protected set; }
    public virtual int? Toughness { get; protected set; }
    public virtual int? BasePower { get; protected set; }
    public virtual int? BaseToughness { get; protected set; }
    public virtual bool IsVisibleInUi { get; protected set; }
    public virtual CardColor[] Colors { get; protected set; }
    public virtual int Counters { get; protected set; }
    public virtual int? Level { get; protected set; }
    public virtual CardType Type { get; protected set; }
    public virtual int Damage { get; protected set; }
    public virtual bool IsTapped { get; protected set; }
    public virtual bool HasSummoningSickness { get; protected set; }
    public string Set { get { return Card.Set; } }
    public Rarity? Rarity { get { return Card.Rarity; } }
    public virtual int? Loyality { get; protected set; }

    public virtual void Dispose()
    {
      _timer.Dispose();
    }

    private void Update()
    {
      Update(() => Power != Card.Power, () =>
        {
          Power = Card.Power;
          BasePower = Card.BasePower;
        });
      
      Update(() => Toughness != Card.Toughness, () =>
        {
          Toughness = Card.Toughness;
          BaseToughness = Card.BaseToughness;
        });

      Update(() => IsVisibleInUi != Card.IsVisibleInUi, () => IsVisibleInUi = Card.IsVisibleInUi);
      Update(() => !Colors.SequenceEqual(Card.Colors), () => Colors = Card.Colors);
      Update(() => Counters != Card.Counters, () => Counters = Card.Counters);
      Update(() => Level != Card.Level, () => Level = Card.Level);
      Update(() => Type != Card.Type, () => Type = Card.Type);
      Update(() => Damage != Card.Damage, () => Damage = Card.Damage);
      Update(() => IsTapped != Card.IsTapped, () => IsTapped = Card.IsTapped);
      Update(() => Loyality != Card.Loyality, () => Loyality = Card.Loyality);

      Update(() => HasSummoningSickness != (Card.HasSummoningSickness && Card.Is().Creature && !Card.Has().Haste),
        () => HasSummoningSickness = Card.HasSummoningSickness && Card.Is().Creature && !Card.Has().Haste);
    }

    private static void Update(Func<bool> condition, Action update)
    {
      if (condition()) update();
    }
  }
}