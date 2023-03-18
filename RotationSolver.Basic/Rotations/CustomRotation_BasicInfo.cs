﻿using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Basic.Actions;
using RotationSolver.Basic.Attributes;
using RotationSolver.Basic.Configuration.RotationConfig;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Rotations;
namespace RotationSolver.Rotations.CustomRotation;

[RotationDesc(DescType.BurstActions)]
public abstract partial class CustomRotation : ICustomRotation
{
    public abstract ClassJobID[] JobIDs { get; }

    public abstract string GameVersion { get; }

    public ClassJob Job => Service.GetSheet<ClassJob>().GetRow((uint)JobIDs[0]);

    public string Name => Job.Abbreviation + " - " + Job.Name;

    public abstract string RotationName { get; }

    public bool IsEnabled
    {
        get => !Service.Config.DisabledCombos.Contains(Name);
        set
        {
            if (value)
            {
                Service.Config.DisabledCombos.Remove(Name);
            }
            else
            {
                Service.Config.DisabledCombos.Add(Name);
            }
        }
    }

    public uint IconID { get; }

    public IRotationConfigSet Configs { get; }

    public BattleChara MoveTarget { get; private set; }

    public virtual string Description { get; } = string.Empty;

    public IAction ActionHealAreaGCD { get; private set; }

    public IAction ActionHealAreaAbility { get; private set; }

    public IAction ActionHealSingleGCD { get; private set; }

    public IAction ActionHealSingleAbility { get; private set; }

    public IAction ActionDefenseAreaGCD { get; private set; }

    public IAction ActionDefenseAreaAbility { get; private set; }

    public IAction ActionDefenseSingleGCD { get; private set; }

    public IAction ActionDefenseSingleAbility { get; private set; }

    public IAction ActionMoveForwardGCD { get; private set; }

    public IAction ActionMoveForwardAbility { get; private set; }

    public IAction ActionMoveBackAbility { get; private set; }

    public IAction EsunaStanceNorthGCD { get; private set; }

    public IAction EsunaStanceNorthAbility { get; private set; }

    public IAction RaiseShirkGCD { get; private set; }

    public IAction RaiseShirkAbility { get; private set; }

    public IAction AntiKnockbackAbility { get; private set; }

    /// <summary>
    /// Description about the actions.
    /// </summary>
    //public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();
    private protected CustomRotation()
    {
        IconID = IconSet.GetJobIcon(this);
        Configs = CreateConfiguration();
    }

    protected virtual IRotationConfigSet CreateConfiguration()
    {
        return new RotationConfigSet(JobIDs[0], RotationName);
    }


    /// <summary>
    /// Update your customized field.
    /// </summary>
    protected virtual void UpdateInfo() { }

    public override string ToString() => RotationName;
}