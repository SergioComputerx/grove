﻿namespace Grove.Core.Decisions.Human
{
  using System.Linq;
  using Targeting;
  using Ui;
  using Ui.SelectTarget;
  using Ui.Shell;

  public class DeclareAttackers : Decisions.DeclareAttackers
  {
    public ViewModel.IFactory DialogFactory { get; set; }
    public IShell Shell { get; set; }

    protected override void ExecuteQuery()
    {
      var tp = new TargetValidatorParameters {MinCount = 0, MaxCount = null, Message = "Select attackers."}
        .Is.Card(c => c.CanAttackThisTurn && c.Controller == Controller)
        .On.Battlefield();


      tp.MustBeTargetable = false;

      var validator = new TargetValidator(tp);
      validator.Initialize(Game, Controller);

      var selectParameters = new SelectTargetParameters
        {
          Validator = validator,          
          CanCancel = false,
          Instructions = "(Press Spacebar when done.)",          
          TargetSelected = target => Publish(
            new AttackerSelected
              {
                Attacker = target.Card()
              }),
          TargetUnselected = target => Publish(
            new AttackerUnselected
              {
                Attacker = target.Card()
              })
        };

      var dialog = DialogFactory.Create(selectParameters);
      Shell.ShowModalDialog(dialog, DialogType.Small, InteractionState.SelectTarget);
      Result = dialog.Selection.ToList();
    }
  }
}