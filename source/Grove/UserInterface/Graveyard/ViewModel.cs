﻿namespace Grove.UserInterface.Graveyard
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Caliburn.Micro;
  using Infrastructure;

  public class ViewModel : ViewModelBase, IDisposable
  {
    private readonly BindableCollection<Spell.ViewModel> _cards =
      new BindableCollection<Spell.ViewModel>();

    private readonly Player _owner;

    public ViewModel(Player owner)
    {
      _owner = owner;
    }

    public IEnumerable<Spell.ViewModel> Cards { get { return _cards; } }

    public override void Initialize()
    {
      foreach (var card in _owner.Graveyard)
      {
        AddCard(card);
      }

      _owner.Graveyard.CardAdded += OnCardAdded;
      _owner.Graveyard.CardRemoved += OnCardRemoved;
    }

    private void OnCardRemoved(object sender, ZoneChangedEventArgs e)
    {
      var viewModel = _cards.Single(x => x.Card == e.Card);

      _cards.Remove(viewModel);
      viewModel.Close();
      ViewModels.Spell.Destroy(viewModel);
    }

    private void OnCardAdded(object sender, ZoneChangedEventArgs e)
    {
      AddCard(e.Card);
    }

    private void AddCard(Card card)
    {
      _cards.Add(ViewModels.Spell.Create(card));
    }

    public interface IFactory
    {
      ViewModel Create(Player owner);
    }

    public void Dispose()
    {
      foreach (var viewModel in _cards)
      {
        viewModel.Dispose();
      }
    }
  }
}