using Dalamud.Game.ClientState.JobGauge.Types;
using RotationSolver.Actions;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Attributes;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Updaters;
using System.Linq;

namespace RotationSolver.Rotations.Basic;

internal abstract class PLD_Base : CustomRotation.CustomRotation
{
    private static PLDGauge JobGauge => Service.JobGauges.Get<PLDGauge>();

    /// <summary>
    /// �����
    /// </summary>
    protected static byte OathGauge => JobGauge.OathGauge;

    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Paladin, ClassJobID.Gladiator };

    private sealed protected override IBaseAction Shield => IronWill;

    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Count() == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction IronWill { get; } = new BaseAction(ActionID.IronWill, shouldEndSpecial: true);

    /// <summary>
    /// �ȷ潣
    /// </summary>
    public static IBaseAction FastBlade { get; } = new BaseAction(ActionID.FastBlade);

    /// <summary>
    /// ���ҽ�
    /// </summary>
    public static IBaseAction RiotBlade { get; } = new BaseAction(ActionID.RiotBlade);

    /// <summary>
    /// ��Ѫ��
    /// </summary>
    public static IBaseAction GoringBlade { get; } = new BaseAction(ActionID.GoringBlade, isEot: true)
    {
        TargetStatus = new[]
        {
            StatusID.GoringBlade,
            StatusID.BladeofValor,
        }
    };

    /// <summary>
    /// սŮ��֮ŭ(��Ȩ��)
    /// </summary>
    public static IBaseAction RageofHalone { get; } = new BaseAction(ActionID.RageofHalone);

    /// <summary>
    /// Ͷ��
    /// </summary>
    public static IBaseAction ShieldLob { get; } = new BaseAction(ActionID.ShieldLob)
    {
        FilterForTarget = b => TargetFilter.ProvokeTarget(b),
    };

    /// <summary>
    /// ս�ӷ�Ӧ
    /// </summary>
    public static IBaseAction FightorFlight { get; } = new BaseAction(ActionID.FightorFlight, true);

    /// <summary>
    /// ȫʴն
    /// </summary>
    public static IBaseAction TotalEclipse { get; } = new BaseAction(ActionID.TotalEclipse);

    /// <summary>
    /// ����ն
    /// </summary>
    public static IBaseAction Prominence { get; } = new BaseAction(ActionID.Prominence);

    /// <summary>
    /// Ԥ��
    /// </summary>
    public static IBaseAction Sentinel { get; } = new BaseAction(ActionID.Sentinel, isTimeline: true, isFriendly: true)
    {
        StatusProvide = Rampart.StatusProvide,
        ActionCheck = BaseAction.TankDefenseSelf,
    };

    /// <summary>
    /// ������ת
    /// </summary>
    public static IBaseAction CircleofScorn { get; } = new BaseAction(ActionID.CircleofScorn);

    /// <summary>
    /// ���֮��
    /// </summary>
    public static IBaseAction SpiritsWithin { get; } = new BaseAction(ActionID.SpiritsWithin);

    /// <summary>
    /// ��ʥ����
    /// </summary>
    public static IBaseAction HallowedGround { get; } = new BaseAction(ActionID.HallowedGround, isTimeline: true);

    /// <summary>
    /// ʥ��Ļ��
    /// </summary>
    public static IBaseAction DivineVeil { get; } = new BaseAction(ActionID.DivineVeil, true, isTimeline: true);

    /// <summary>
    /// ���ʺ���
    /// </summary>
    public static IBaseAction Clemency { get; } = new BaseAction(ActionID.Clemency, true, true, isTimeline: true);

    /// <summary>
    /// ��Ԥ
    /// </summary>
    public static IBaseAction Intervention { get; } = new BaseAction(ActionID.Intervention, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
    };

    /// <summary>
    /// ��ͣ
    /// </summary>
    public static IBaseAction Intervene { get; } = new BaseAction(ActionID.Intervene, shouldEndSpecial: true)
    {
        ChoiceTarget = TargetFilter.FindTargetForMoving,
    };

    /// <summary>
    /// ���｣
    /// </summary>
    public static IBaseAction Atonement { get; } = new BaseAction(ActionID.Atonement)
    {
        StatusNeed = new[] { StatusID.SwordOath },
    };

    /// <summary>
    /// ���꽣
    /// </summary>
    public static IBaseAction Expiacion { get; } = new BaseAction(ActionID.Expiacion);

    /// <summary>
    /// Ӣ��֮��
    /// </summary>
    public static IBaseAction BladeofValor { get; } = new BaseAction(ActionID.BladeofValor);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static IBaseAction BladeofTruth { get; } = new BaseAction(ActionID.BladeofTruth);

    /// <summary>
    /// ����֮��
    /// </summary>
    public static IBaseAction BladeofFaith { get; } = new BaseAction(ActionID.BladeofFaith)
    {
        StatusNeed = new[] { StatusID.ReadyForBladeofFaith },
    };

    /// <summary>
    /// ��������
    /// </summary>
    public static IBaseAction Requiescat { get; } = new BaseAction(ActionID.Requiescat, true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Confiteor { get; } = new BaseAction(ActionID.Confiteor)
    {
        StatusNeed = new StatusID[] {StatusID.ConfiteorReady},
    };

    /// <summary>
    /// ʥ��
    /// </summary>
    public static IBaseAction HolyCircle { get; } = new BaseAction(ActionID.HolyCircle);

    /// <summary>
    /// ʥ��
    /// </summary>
    public static IBaseAction HolySpirit { get; } = new BaseAction(ActionID.HolySpirit);

    /// <summary>
    /// ��װ����
    /// </summary>
    public static IBaseAction PassageofArms { get; } = new BaseAction(ActionID.PassageofArms, true, isTimeline: true);

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Cover { get; } = new BaseAction(ActionID.Cover, true, isTimeline: true)
    {
        ChoiceTarget = TargetFilter.FindAttackedTarget,
        ActionCheck = b => OathGauge >= 50,
    };

    /// <summary>
    /// ����
    /// </summary>
    public static IBaseAction Sheltron { get; } = new BaseAction(ActionID.Sheltron, isTimeline: true)
    {
        ActionCheck = Cover.ActionCheck,
    };

    public static IBaseAction Bulwark { get; } = new BaseAction(ActionID.Bulwark, isTimeline: true);


    private protected override bool EmergencyAbility(byte abilitiesRemaining, IAction nextGCD, out IAction act)
    {
        if (HallowedGround.CanUse(out act) && BaseAction.TankBreakOtherCheck(JobIDs[0], HallowedGround.Target)) return true;
        return base.EmergencyAbility(abilitiesRemaining, nextGCD, out act);
    }

    [RotationDesc(ActionID.Intervene)]
    private protected sealed override bool MoveForwardAbility(byte abilitiesRemaining, out IAction act)
    {
        if (Intervene.CanUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    [RotationDesc(ActionID.Clemency)]
    private protected sealed override bool HealSingleGCD(out IAction act)
    {
        if (Clemency.CanUse(out act)) return true;
        return false;
    }
}
