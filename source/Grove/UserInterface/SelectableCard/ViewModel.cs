﻿namespace Grove.UserInterface.SelectableCard
{
  using System;
  using Infrastructure;
  using Messages;

  public class ViewModel : CardViewModel, IReceive<TargetSelected>, IReceive<TargetUnselected>,
    IReceive<UiInteractionChanged>
  {
    private Action _select = delegate { };

    public ViewModel(Card card) : base(card) {}

    public virtual bool IsSelected { get; protected set; }

    public void Receive(TargetSelected message)
    {
      if (message.Target == Card)
      {
        IsSelected = true;
      }
    }

    public void Receive(TargetUnselected message)
    {
      if (message.Target == Card)
      {
        IsSelected = false;
      }
    }

    public void Receive(UiInteractionChanged message)
    {
      switch (message.State)
      {
        case (InteractionState.SelectTarget):
          {
            _select = ChangeSelection;
            break;
          }
        default:
          _select = delegate { };
          break;
      }

      IsSelected = false;
    }

    private void ChangeSelection()
    {
      Ui.Publisher.Publish(
        new SelectionChanged {Selection = Card});
    }

    public void Select()
    {
      _select();
    }

    public void ChangePlayersInterest()
    {
      ChangePlayersInterest(this);
    }

    public interface IFactory
    {
      ViewModel Create(Card card);
      void Destroy(ViewModel viewModel);
    }
  }
}