namespace DefaultRotations.Healer;

[Rotation("Default", CombatType.PvE, GameVersion = "7.15")]
[SourceCode(Path = "main/BasicRotations/Healer/SGE_Default.cs")]
[Api(4)]
public sealed class SGE_Default : SageRotation
{
    #region Config Options
    [RotationConfig(CombatType.PvE, Name = "Use spells with cast times to heal. (Ignored if you are the only healer in party)")]
    public bool GCDHeal { get; set; } = false;

    [RotationConfig(CombatType.PvE, Name = "Enable Swiftcast Restriction Logic to attempt to prevent actions other than Raise when you have swiftcast")]
    public bool SwiftLogic { get; set; } = true;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold party member needs to be to use Taurochole")]
    public float TaurocholeHeal { get; set; } = 0.8f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold party member needs to be to use Soteria")]
    public float SoteriaHeal { get; set; } = 0.85f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Average health threshold party members need to be to use Holos")]
    public float HolosHeal { get; set; } = 0.5f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold tank party member needs to use Zoe")]
    public float ZoeHeal { get; set; } = 0.6f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold party member needs to be to use an OGCD Heal while not holding addersgal stacks")]
    public float OGCDHeal { get; set; } = 0.20f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold tank party member needs to use an OGCD Heal on Tanks while not holding addersgal stacks")]
    public float OGCDTankHeal { get; set; } = 0.65f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold party member needs to be to use Krasis")]
    public float KrasisHeal { get; set; } = 0.3f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold tank party member needs to use Krasis")]
    public float KrasisTankHeal { get; set; } = 0.7f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold party member needs to be to use Pneuma as a ST heal")]
    public float PneumaSTPartyHeal { get; set; } = 0.2f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold tank party member needs to use Pneuma as a ST heal")]
    public float PneumaSTTankHeal { get; set; } = 0.6f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Average health threshold party members need to be to use Pneuma as an AOE heal")]
    public float PneumaAOEPartyHeal { get; set; } = 0.65f;

    [Range(0, 1, ConfigUnitType.Percent)]
    [RotationConfig(CombatType.PvE, Name = "Health threshold tank party member needs to use Pneuma as an AOE heal")]
    public float PneumaAOETankHeal { get; set; } = 0.6f;

    #endregion

    #region Countdown Logic
    protected override IAction? CountDownAction(float remainTime)
    {
        if (remainTime < DosisPvE.Info.CastTime + CountDownAhead
            && DosisPvE.CanUse(out var act)) return act;
        if (remainTime <= 3 && UseBurstMedicine(out act)) return act;
        return base.CountDownAction(remainTime);
    }
    #endregion

    #region oGCD Logic
    [RotationDesc(ActionID.PsychePvE)]
    protected override bool AttackAbility(IAction nextGCD, out IAction? act)
    {
        if (PsychePvE.CanUse(out act)) return true;

        return base.AttackAbility(nextGCD, out act);
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction? act)
    {
        if (base.EmergencyAbility(nextGCD, out act)) return true;

        if (nextGCD.IsTheSameTo(false, PneumaPvE, EukrasianDiagnosisPvE,
            EukrasianPrognosisPvE, EukrasianPrognosisIiPvE, DiagnosisPvE, PrognosisPvE))
        {
            if (ZoePvE.CanUse(out act)) return true;
        }

        if (nextGCD.IsTheSameTo(false, PneumaPvE, EukrasianDiagnosisPvE,
             EukrasianPrognosisPvE, EukrasianPrognosisIiPvE, DiagnosisPvE, PrognosisPvE))
        {
            if (KrasisPvE.CanUse(out act)) return true;
        }

        if (nextGCD.IsTheSameTo(false, PneumaPvE, EukrasianDiagnosisPvE,
             EukrasianPrognosisPvE, EukrasianPrognosisIiPvE, DiagnosisPvE, PrognosisPvE))
        {
            if (PhilosophiaPvE.CanUse(out act)) return true;
        }

        return base.EmergencyAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.PanhaimaPvE, ActionID.KeracholePvE, ActionID.HolosPvE)]
    protected override bool DefenseAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (Addersgall <= 1)
        {
            if (PanhaimaPvE.CanUse(out act)) return true;
        }

        if (KeracholePvE.CanUse(out act)) return true;

        if (HolosPvE.CanUse(out act)) return true;

        return base.DefenseAreaAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.HaimaPvE, ActionID.TaurocholePvE, ActionID.PanhaimaPvE, ActionID.KeracholePvE, ActionID.HolosPvE)]
    protected override bool DefenseSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (Addersgall <= 1)
        {
            if (HaimaPvE.CanUse(out act)) return true;
        }

        if (TaurocholePvE.CanUse(out act) && TaurocholePvE.Target.Target?.GetHealthRatio() < TaurocholeHeal) return true;

        if (Addersgall <= 1)
        {
            if ((!HaimaPvE.EnoughLevel || HaimaPvE.Cooldown.ElapsedAfter(20)) && PanhaimaPvE.CanUse(out act)) return true;
        }

        if ((!TaurocholePvE.EnoughLevel || TaurocholePvE.Cooldown.ElapsedAfter(20)) && KeracholePvE.CanUse(out act)) return true;

        if (HolosPvE.CanUse(out act)) return true;

        return base.DefenseSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.KeracholePvE, ActionID.PhysisPvE, ActionID.HolosPvE, ActionID.IxocholePvE)]
    protected override bool HealAreaAbility(IAction nextGCD, out IAction? act)
    {
        if (PhysisIiPvE.CanUse(out act)) return true;
        if (!PhysisIiPvE.EnoughLevel && PhysisPvE.CanUse(out act)) return true;

        if (KeracholePvE.CanUse(out act) && EnhancedKeracholeTrait.EnoughLevel) return true;

        if (HolosPvE.CanUse(out act) && PartyMembersAverHP < HolosHeal) return true;

        if (IxocholePvE.CanUse(out act)) return true;

        if (KeracholePvE.CanUse(out act)) return true;

        return base.HealAreaAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.TaurocholePvE, ActionID.KeracholePvE, ActionID.DruocholePvE, ActionID.HolosPvE, ActionID.PhysisPvE, ActionID.PanhaimaPvE)]
    protected override bool HealSingleAbility(IAction nextGCD, out IAction? act)
    {
        if (TaurocholePvE.CanUse(out act)) return true;

        if (KeracholePvE.CanUse(out act) && EnhancedKeracholeTrait.EnoughLevel) return true;

        if ((!TaurocholePvE.EnoughLevel || TaurocholePvE.Cooldown.IsCoolingDown) && DruocholePvE.CanUse(out act)) return true;

        if (SoteriaPvE.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < SoteriaHeal)) return true;

        var tank = PartyMembers.GetJobCategory(JobRole.Tank);
        if (Addersgall < 1 && (tank.Any(t => t.GetHealthRatio() < OGCDTankHeal) || PartyMembers.Any(b => b.GetHealthRatio() < OGCDHeal)))
        {
            if (HaimaPvE.CanUse(out act)) return true;

            if (PhysisIiPvE.CanUse(out act)) return true;
            if (!PhysisIiPvE.EnoughLevel && PhysisPvE.CanUse(out act)) return true;

            if (HolosPvE.CanUse(out act)) return true;

            if ((!HaimaPvE.EnoughLevel || HaimaPvE.Cooldown.ElapsedAfter(20)) && PanhaimaPvE.CanUse(out act)) return true;
        }

        if (tank.Any(t => t.GetHealthRatio() < ZoeHeal))
        {
            if (ZoePvE.CanUse(out act)) return true;
        }

        if (tank.Any(t => t.GetHealthRatio() < KrasisTankHeal) || PartyMembers.Any(b => b.GetHealthRatio() < KrasisHeal))
        {
            if (KrasisPvE.CanUse(out act)) return true;
        }

        if (KeracholePvE.CanUse(out act)) return true;

        return base.HealSingleAbility(nextGCD, out act);
    }

    [RotationDesc(ActionID.EukrasianPrognosisPvE, ActionID.EukrasianPrognosisIiPvE)]
    protected override bool GeneralAbility(IAction nextGCD, out IAction? act)
    {
        // If not in combat and lacking the Kardia status, attempt to use KardiaPvE
        if (!InCombat && !Player.HasStatus(true, StatusID.Kardia) && KardiaPvE.CanUse(out act)) return true;

        if (KardiaPvE.CanUse(out act)) return true;

        if (Addersgall <= 1 && RhizomataPvE.CanUse(out act)) return true;

        if (SoteriaPvE.CanUse(out act) && PartyMembers.Any(b => b.HasStatus(true, StatusID.Kardion) && b.GetHealthRatio() < HealthSingleAbility)) return true;

        if (PepsisPvE.CanUse(out act)) return true;

        return base.GeneralAbility(nextGCD, out act);
    }
    #endregion

    #region Eukrasia Logic
    private IBaseAction? _EukrasiaActionAim = null;

    // Sets the target Eukrasia action to be performed next.
    // If the action is null, it exits early.
    // If the current action aim is not null and the last action matches certain conditions, it exits early.
    // Finally, updates the current Eukrasia action aim if it's different from the incoming action.
    private void SetEukrasia(IBaseAction act)
    {
        if (act == null) return;

        if (_EukrasiaActionAim != null && IsLastGCD(false, _EukrasiaActionAim)) return;

        if (_EukrasiaActionAim != act)
        {
            _EukrasiaActionAim = act;
        }
    }

    // Clears the Eukrasia action aim, effectively resetting any planned Eukrasia action.
    private void ClearEukrasia()
    {
        if (_EukrasiaActionAim != null)
        {
            _EukrasiaActionAim = null;
        }
    }

    private bool ChoiceEukrasia(out IAction? act)
    {
        act = null;

        if (!EukrasiaPvE.CanUse(out _)) return false;
        // Checks for Eukrasia status.
        // Attempts to set correct Eurkrasia action based on availablity and MergedStatus.
        if (EukrasianPrognosisIiPvE.CanUse(out _) && EukrasianPrognosisIiPvE.EnoughLevel && MergedStatus.HasFlag(AutoStatus.DefenseArea))
        {
            SetEukrasia(EukrasianPrognosisIiPvE);
            return false;
        }

        if (EukrasianPrognosisPvE.CanUse(out _) && EukrasianPrognosisPvE.EnoughLevel && MergedStatus.HasFlag(AutoStatus.DefenseArea))
        {
            SetEukrasia(EukrasianPrognosisPvE);
            return false;
        }

        if (EukrasianDiagnosisPvE.CanUse(out _) && EukrasianDiagnosisPvE.EnoughLevel && MergedStatus.HasFlag(AutoStatus.DefenseSingle))
        {
            SetEukrasia(EukrasianDiagnosisPvE);
            return false;
        }

        if (EukrasianDyskrasiaPvE.CanUse(out _) && EukrasianDyskrasiaPvE.EnoughLevel && (!MergedStatus.HasFlag(AutoStatus.DefenseSingle) || !MergedStatus.HasFlag(AutoStatus.DefenseSingle)))
        {
            SetEukrasia(EukrasianDyskrasiaPvE);
            return false;
        }

        if (EukrasianDosisIiiPvE.CanUse(out _) && EukrasianDosisIiiPvE.EnoughLevel && (!MergedStatus.HasFlag(AutoStatus.DefenseSingle) || !MergedStatus.HasFlag(AutoStatus.DefenseSingle)))
        {
            SetEukrasia(EukrasianDosisIiiPvE);
            return false;
        }

        if (EukrasianDosisIiPvE.CanUse(out _) && EukrasianDosisIiPvE.EnoughLevel && (!MergedStatus.HasFlag(AutoStatus.DefenseSingle) || !MergedStatus.HasFlag(AutoStatus.DefenseSingle)))
        {
            SetEukrasia(EukrasianDosisIiPvE);
            return false;
        }

        if (EukrasianDosisPvE.CanUse(out _) && EukrasianDosisPvE.EnoughLevel && (!MergedStatus.HasFlag(AutoStatus.DefenseSingle) || !MergedStatus.HasFlag(AutoStatus.DefenseSingle)))
        {
            SetEukrasia(EukrasianDosisPvE);
            return false;
        }

        // If the last action performed matches any of a list of specific actions, it clears the Eukrasia aim.
        // This serves as a reset/cleanup mechanism to ensure the decision logic starts fresh for the next cycle.
        if (IsLastGCD(false, EukrasianPrognosisIiPvE, EukrasianPrognosisPvE,
            EukrasianDiagnosisPvE, EukrasianDyskrasiaPvE, EukrasianDosisIiiPvE, EukrasianDosisIiPvE,
            EukrasianDosisPvE) || !InCombat)
        {
            ClearEukrasia();
        }
        return false; // Indicates that no specific Eukrasia action was chosen in this cycle.
    }
    #endregion

    #region Eukrasia Execution
    // Attempts to perform a Eukrasia action, based on the current game state and conditions.
    private bool DoEukrasia(out IAction? act)
    {
        act = null;

        if (_EukrasiaActionAim != null && _EukrasiaActionAim.CanUse(out act))
        {
            if (EukrasiaPvE.CanUse(out act)) return true;

            act = _EukrasiaActionAim;
            return true;
        }
        return false;
    }
    #endregion

    #region GCD Logic 
    [RotationDesc(ActionID.PneumaPvE, ActionID.PrognosisPvE, ActionID.EukrasianPrognosisPvE, ActionID.EukrasianPrognosisIiPvE)]
    protected override bool HealAreaGCD(out IAction? act)
    {
        act = null;

        if (HasSwift && SwiftLogic && EgeiroPvE.CanUse(out _)) return false;

        if (PartyMembersAverHP < PneumaAOEPartyHeal || DyskrasiaPvE.CanUse(out _) && PartyMembers.GetJobCategory(JobRole.Tank).Any(t => t.GetHealthRatio() < PneumaAOETankHeal))
        {
            if (PneumaPvE.CanUse(out act)) return true;
        }

        if (Player.HasStatus(false, StatusID.EukrasianDiagnosis, StatusID.EukrasianPrognosis, StatusID.Galvanize))
        {
            if (PrognosisPvE.CanUse(out act)) return true;
        }

        if (EukrasianPrognosisIiPvE.CanUse(out _))
        {
            if (EukrasiaPvE.CanUse(out act)) return true;
            act = EukrasianPrognosisIiPvE;
            return true;
        }

        if (!EukrasianPrognosisIiPvE.EnoughLevel && EukrasianPrognosisPvE.CanUse(out _))
        {
            if (EukrasiaPvE.CanUse(out act)) return true;
            act = EukrasianPrognosisPvE;
            return true;
        }

        return base.HealAreaGCD(out act);
    }

    [RotationDesc(ActionID.DiagnosisPvE)]
    protected override bool HealSingleGCD(out IAction? act)
    {
        act = null;

        if (HasSwift && SwiftLogic && EgeiroPvE.CanUse(out _)) return false;

        if (DiagnosisPvE.CanUse(out _) && !EukrasianDiagnosisPvE.CanUse(out _, skipCastingCheck: true) && InCombat)
        {
            if (DiagnosisPvE.CanUse(out act))
            {
                return true;
            }
        }
        return base.HealSingleGCD(out act);
    }

    protected override bool GeneralGCD(out IAction? act)
    {
        act = null;

        if (HasSwift && SwiftLogic && EgeiroPvE.CanUse(out _)) return false;

        if (!InCombat && !Player.HasStatus(true, StatusID.Eukrasia) && EukrasiaPvE.CanUse(out act)) return true;

        if (PhlegmaIiiPvE.CanUse(out act, usedUp: IsMoving)) return true;
        if (PhlegmaIiPvE.CanUse(out act, usedUp: IsMoving)) return true;
        if (PhlegmaPvE.CanUse(out act, usedUp: IsMoving)) return true;

        if (PartyMembers.Any(b => b.GetHealthRatio() < PneumaSTPartyHeal && !b.IsDead) || PartyMembers.GetJobCategory(JobRole.Tank).Any(t => t.GetHealthRatio() < PneumaSTTankHeal && !t.IsDead))
        {
            if (PneumaPvE.CanUse(out act)) return true;
        }

        if (IsMoving && ToxikonPvE.CanUse(out act)) return true;

        if (ChoiceEukrasia(out act)) return true;
        if (DoEukrasia(out act)) return true;

        if (DyskrasiaPvE.CanUse(out act)) return true;

        if (DosisPvE.CanUse(out act)) return true;

        return base.GeneralGCD(out act);
    }
    #endregion

    #region Extra Methods
    public override bool CanHealSingleSpell => base.CanHealSingleSpell && (GCDHeal || PartyMembers.GetJobCategory(JobRole.Healer).Count() < 2);
    public override bool CanHealAreaSpell => base.CanHealAreaSpell && (GCDHeal || PartyMembers.GetJobCategory(JobRole.Healer).Count() < 2);

    public override void DisplayStatus()
    {
        ImGui.Text($"Eukrasian Action: {_EukrasiaActionAim}");
        ImGui.Text("HasEukrasia: " + HasEukrasia.ToString());
        ImGui.Text("Addersgall: " + Addersgall.ToString());
        ImGui.Text("Addersting: " + Addersting.ToString());
        ImGui.Text("AddersgallTime: " + AddersgallTime.ToString());
    }
    #endregion
}
