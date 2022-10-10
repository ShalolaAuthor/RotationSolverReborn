﻿using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos
{
    internal class BLMCombo : JobGaugeCombo<BLMGauge>
    {
        internal class BLMAction : BaseAction
        {
            private static BLMGauge JobGauge => BLMCombo.JobGauge;

            private readonly bool _isFire;

            internal override uint MPNeed
            {
                get
                {
                    double multiply = 1;
                    //冰状态
                    if (JobGauge.InUmbralIce)
                    {
                        //火魔法无魔力
                        if (_isFire)
                        {
                            multiply = 0;
                        }
                        //冰魔法三层半魔力，前两层不变。
                        else
                        {
                            switch (JobGauge.UmbralIceStacks)
                            {
                                case 1:
                                    multiply = 0.75;
                                    break;
                                case 2:
                                    multiply = 0.5;
                                    break;
                                case 3:
                                    multiply = 0;
                                    break;
                            }
                        }
                    }
                    //火状态
                    else if (JobGauge.InAstralFire)
                    {
                        //火魔法双倍魔力
                        if (_isFire)
                        {
                            multiply = 2;
                        }
                        //冰魔法无魔力
                        else
                        {
                            multiply = 0;
                        }
                    }
                    return (uint)(base.MPNeed * multiply);
                }
            }

            internal BLMAction(uint actionID, bool isFire)
                : base(actionID)
            {
                this._isFire = isFire;
            }
        }

        internal override uint JobID => 25;

        /// <summary>
        /// 判断通晓是否满了。
        /// </summary>
        protected static bool IsPolyglotStacksMaxed
        {
            get
            {
                if (Service.ClientState.LocalPlayer.Level < 80)
                {
                    return JobGauge.PolyglotStacks == 1;
                }
                else
                {
                    return JobGauge.PolyglotStacks == 2;
                }
            }
        }
        private bool HasFire => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Firestarter);
        private bool HasThunder => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Thundercloud);
        internal static bool InTranspose = false;
        private bool HasSwift => StatusHelper.FindStatusFromSelf(Target, ObjectStatus.Thunder,
                        ObjectStatus.Thunder2,
                        ObjectStatus.Thunder3,
                        ObjectStatus.Thunder4).Length > 0;
        internal static bool UseThunderIn { get; set; } = false;
        protected override bool CanHealSingleAbility => false;
        private bool CanGoFire
        {
            get
            {
                if (!(JobGauge.InUmbralIce && LocalPlayer.CurrentMp > 9000 && (JobGauge.UmbralHearts == 3 || Service.ClientState.LocalPlayer.Level < 58))) return false;

                if (UseThunderIn) return true;
                if (Service.TargetManager.Target is BattleChara b)
                {
                    if (StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.Thunder, ObjectStatus.Thunder3) > 20) return true;
                    if (StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.Thunder2, ObjectStatus.Thunder4) > 10) return true;
                }
                return false;
            }
        } 

        internal struct Actions
        {

            public static readonly BaseAction
                //雷1
                Thunder = new(144u)
                {
                    //    TargetStatus = new ushort[]
                    //{
                    //    ObjectStatus.Thunder,
                    //    ObjectStatus.Thunder2,
                    //    ObjectStatus.Thunder3,
                    //    ObjectStatus.Thunder4,
                    //},
                    OtherIDsNot = new uint[] { 153u, 144u, 7420u, 7447u }, //雷1,3 ID
                    AfterUse = () => UseThunderIn = true,
                },

                //雷2
                Thunder2 = new(7447u)
                {
                    TargetStatus = Thunder.TargetStatus,
                    OtherIDsNot = new[] { 153u, 144u, 7420u, 7447u }, //雷2,4 ID
                    AfterUse = () => UseThunderIn = true,
                },

                //星灵移位
                Transpose = new (149u) { OtherCheck = b => JobGauge.InUmbralIce || JobGauge.InAstralFire },

                //灵极魂
                UmbralSoul = new (16506u) { OtherCheck = b => JobGauge.InUmbralIce },

                //魔罩
                Manaward = new (157u),

                //魔泉
                Manafont = new (158u),

                //激情咏唱
                Sharpcast = new(3574u)
                {
                    BuffsProvide = new[] { ObjectStatus.Sharpcast }
                },

                //三连咏唱
                Triplecast = new (7421u)
                {
                    BuffsProvide = GeneralActions.Swiftcast.BuffsProvide,
                    //OtherCheck = () => JobGauge.InAstralFire && JobGauge.UmbralHearts < 2 && JobGauge.ElementTimeRemaining > 10000,
                },

                //黑魔纹
                Leylines = new (3573u, shouldEndSpecial: true)
                {
                    BuffsProvide = new [] { ObjectStatus.LeyLines, },
                },

                //魔纹步
                BetweenTheLines = new (7419u, shouldEndSpecial: true)
                {
                    BuffsNeed = new [] { ObjectStatus.LeyLines },
                },

                //以太步
                AetherialManipulation = new (155)
                {
                    ChoiceFriend = TargetFilter.FindMoveTarget,
                },

                //详述
                Amplifier = new (25796u) { OtherCheck = b => !IsPolyglotStacksMaxed && JobGauge.EnochianTimer > 10000 },

                //核爆
                Flare = new (162u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 4000 },

                //绝望
                Despair = new (16505u) { OtherCheck = b => JobGauge.AstralFireStacks == 3 && JobGauge.ElementTimeRemaining > 3000 },

                //秽浊
                Foul = new (7422u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 },

                //异言
                Xenoglossy = new (16507u) { OtherCheck = b => JobGauge.PolyglotStacks != 0 };



            public static readonly BLMAction

                //火1
                Fire = new (141u, true),

                //火2
                Fire2 = new (147u, true)
                {
                    AfterUse = () => InTranspose = false,
                },

                //火3
                Fire3 = new (152u, true)
                {
                    AfterUse = () => InTranspose = false,
                },

                //火4
                Fire4 = new (3577u, true) { OtherCheck = b => JobGauge.InAstralFire && JobGauge.ElementTimeRemaining > 5000 },


                //冰1
                Blizzard = new (142u, false)
                {
                    AfterUse = () => UseThunderIn = false,
                },

                //冰2
                Blizzard2 = new (25793u, false)
                {
                    AfterUse = () => UseThunderIn = false,
                },

                //冰3
                Blizzard3 = new (154u, false)
                {
                    AfterUse = () => UseThunderIn = false,
                },

                //冰4
                Blizzard4 = new (3576u, false) { OtherCheck = b => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2500 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) },

                //冻结
                Freeze = new (159u, false) { OtherCheck = b => JobGauge.InUmbralIce && JobGauge.ElementTimeRemaining > 2800 * (JobGauge.UmbralIceStacks == 3 ? 0.5 : 1) };
        }

        internal override SortedList<DescType, string> Description => new ()
        {
            { DescType.单体治疗, $"{Actions.BetweenTheLines.Action.Name}, {Actions.Leylines.Action.Name}, 这个很特殊！" },
            { DescType.单体防御, $"{Actions.Manaward.Action.Name}" },
            { DescType.移动, $"{Actions.AetherialManipulation.Action.Name}，目标为面向夹角小于30°内最远目标。" },
        };

        private protected override ActionConfiguration CreateConfiguration()
        {
            return base.CreateConfiguration().SetBool("AutoLeylines", true, "自动上黑魔纹").SetBool("StartFire", false, "火起手")
                .SetFloat("TimeToAdd", 5.4f, 3, 8, "火阶段还剩几秒时补时间", 0.01f);
        }

        private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
        {
            if (Actions.BetweenTheLines.ShouldUseAction(out act)) return true;
            if (Actions.Leylines.ShouldUseAction(out act, mustUse:true)) return true;

            return base.HealSingleAbility(abilityRemain, out act);
        }

        private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
        {
            //加个魔罩
            if (Actions.Manaward.ShouldUseAction(out act)) return true;

            return base.DefenceSingleAbility(abilityRemain, out act);
        }

        private protected override bool MoveAbility(byte abilityRemain, out IAction act)
        {
            if (Actions.AetherialManipulation.ShouldUseAction(out act, mustUse: true)) return true;

            return base.MoveAbility(abilityRemain, out act);
        }
        private bool Maintence( out IAction act)
        {
            if (Actions.UmbralSoul.ShouldUseAction(out act)) return true;
            if (Actions.Transpose.ShouldUseAction(out act)) return true;

            //if (Service.ClientState.LocalPlayer.Level >= Actions.UmbralSoul.Level)
            //{
            //    if (JobGauge.InAstralFire)
            //    {
            //        if (Actions.Transpose.ShouldUseAction(out act)) return true;
            //    }
            //}
            //else
            //{
            //    if (Actions.Transpose.ShouldUseAction(out act)) return true;
            //}

            return false;
        }
        private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
        {
            act = null;
            //刚刚魔泉，别给我转冰了。
            if (LastAbility == Actions.Manafont.ID) return false;

            //星灵转冰
            if (Service.ClientState.LocalPlayer.Level >= 90 && JobGauge.InAstralFire && Service.ClientState.LocalPlayer.CurrentMp == 0
                && (JobGauge.PolyglotStacks > 0 || JobGauge.EnochianTimer < 3000)
                && (HasFire || !GeneralActions.Swiftcast.IsCoolDown || GeneralActions.Swiftcast.RecastTimeRemain < 5 
                || !Actions.Triplecast.IsCoolDown || Actions.Triplecast.RecastTimeRemain < 15
                ||　(Service.TargetManager.Target is BattleChara b  &&
                StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.Thunder, ObjectStatus.Thunder3) > 15)))
            {
                Actions.Transpose.AfterUse = () =>
                {
                    InTranspose = true;
                    UseThunderIn = false;
                };
                if (Actions.Transpose.ShouldUseAction(out act)) return true;
            }
            else
            {
                Actions.Transpose.AfterUse = () => InTranspose = false;
            }
            //星灵转火
            if (JobGauge.InUmbralIce && InTranspose && (HasFire || HaveSwift) && 
                (nextGCD.ID == Actions.Fire3.ID || nextGCD.ID == Actions.Fire2.ID || Service.ClientState.LocalPlayer.CurrentMp >= 8000
                || (HasFire && Service.ClientState.LocalPlayer.CurrentMp >= 5600)) )
            {
                if (Actions.Transpose.ShouldUseAction(out act)) return true;
            }
            //有火苗转火
            if(nextGCD.ID == Actions.Fire3.ID && HasFire && JobGauge.InUmbralIce)
            {
                if (Actions.Transpose.ShouldUseAction(out act)) return true;
            }

            act = null;
            return false;
        }

        private protected override bool GeneralAbility(byte abilityRemain, out IAction act)
        {
            if (!HaveTargetAngle && Maintence(out act)) return true;


            return base.GeneralAbility(abilityRemain, out act);
        }

        private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            //刚刚魔泉，别给我转冰了。
            if (LastAbility == Actions.Manafont.ID) return false;

            if (IsMoving)
            {
                if (JobGauge.InAstralFire && (LocalPlayer.CurrentMp < 5000 || JobGauge.ElementTimeRemaining < 5000))
                {
                    if (Actions.Transpose.ShouldUseAction(out act)) return true;
                }
                if (!InTranspose && Actions.Triplecast.ShouldUseAction(out act, mustUse: true)) return true;
                //if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
            }

            if ((JobGauge.InUmbralIce && !HasSwift) || (JobGauge.InAstralFire && Service.ClientState.LocalPlayer.CurrentMp <= 4400)
                || GeneralActions.Swiftcast.RecastTimeRemain > 20)
            {
                //加个激情
                if (Actions.Sharpcast.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            }

            if (JobGauge.InUmbralIce)
            {
                if (InTranspose)
                {
                    //加个醒梦
                    if (GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;

                    //加个即刻
                    if (!HasFire && (UseThunderIn || HasThunder) && GeneralActions.Swiftcast.ShouldUseAction(out act)) return true;
                    if (!HasFire && GeneralActions.Swiftcast.RecastTimeRemain >= 5 && Actions.Triplecast.RecastTimeRemain < 15 && 
                        Actions.Triplecast.ShouldUseAction(out act, emptyOrSkipCombo:true)) return true;
                }
            }

            if (JobGauge.InAstralFire)
            {
                //三连
                if (Actions.Triplecast.ShouldUseAction(out act)) return true;

                //爆发药！
                if (UseBreakItem(out act)) return true;

                //自动黑魔纹
                if(Config.GetBoolByName("AutoLeylines") && Actions.Leylines.ShouldUseAction(out act, mustUse: true)) return true;
            }


            //else if (nextGCD.ID == Actions.Fire.ID || nextGCD.ID == Actions.Fire3.ID)
            //{
            //    //加个激情
            //    if (Actions.Sharpcast.ShouldUseAction(out act)) return true;
            //}

            //加个通晓
            if (Actions.Amplifier.ShouldUseAction(out act)) return true;

            return false;
        }

        private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
        {

            //冰状态
            if (JobGauge.InUmbralIce)
            {
                //双星灵
                if (InTranspose)
                {
                    if (!Actions.Fire2.ShouldUseAction(out _) && JobGauge.IsParadoxActive && Actions.Fire.ShouldUseAction(out act)) return true;

                    //如果可以不硬读条转火
                    if(HasFire || HaveSwift || !GeneralActions.Swiftcast.IsCoolDown || GeneralActions.Swiftcast.RecastTimeRemain < 1.5)
                    {
                        //补雷
                        if (!UseThunderIn && HasThunder && AddThunder(lastComboActionID, out act)) return true;

                        //硬读条补雷
                        if (!UseThunderIn && HasSwift && AddThunder(lastComboActionID, out act)) return true;

                        if (AddPolyglotAttach(out act)) return true;
                    }

                    if (Actions.Fire2.ShouldUseAction(out act)) return true;
                    if (Actions.Fire3.ShouldUseAction(out act)) return true;
                }
                else
                {
                    //常规转火状态
                    if (CanGoFire)
                    {
                        //进入火状态
                        //试试看火2,3
                        if (Actions.Fire2.ShouldUseAction(out act)) return true;

                        //把冰悖论放掉
                        if (JobGauge.IsParadoxActive && Actions.Fire.ShouldUseAction(out act)) return true;

                        if (Actions.Fire3.ShouldUseAction(out act)) return true;
                    }

                    //如果通晓满了，就放掉。
                    if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer < 7000)
                    {
                        if (AddPolyglotAttach(out act)) return true;
                    }

                    //上雷
                    if (!UseThunderIn && AddThunder(lastComboActionID, out act))
                    {
                        //加个激情
                        var relayAct = act;
                        if (Actions.Sharpcast.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

                        act = relayAct;
                        return true;
                    }

                    //加冰心
                    if (AddUmbralHearts(out act)) return true;

                    //把冰悖论放掉
                    if (!Actions.Fire2.ShouldUseAction(out _) && JobGauge.IsParadoxActive && Actions.Fire.ShouldUseAction(out act)) return true;


                    //试试看冰2,3
                    if (Actions.Blizzard2.ShouldUseAction(out act)) return true;
                    if (Actions.Blizzard4.ShouldUseAction(out act)) return true;
                    if (Actions.Blizzard3.ShouldUseAction(out act)) return true;
                    if (Actions.Blizzard.ShouldUseAction(out act)) return true;
                }
            }
            //火状态
            else if (JobGauge.InAstralFire)
            {
                //如果需要续时间,提高档数
                if (JobGauge.ElementTimeRemaining < Config.GetFloatByName("TimeToAdd") * 1000)
                {
                    if(Service.ClientState.LocalPlayer.CurrentMp >= 4000 || JobGauge.AstralFireStacks == 2)
                    {
                        if (Actions.Fire.ShouldUseAction(out act)) return true;
                    }
                    else
                    {
                        if (Actions.Flare.ShouldUseAction(out act)) return true;
                        if (Actions.Despair.ShouldUseAction(out act)) return true;
                    }
                }
                if (JobGauge.AstralFireStacks == 1)
                {
                    if (Actions.Fire2.ShouldUseAction( out act)) return true;
                    if (Actions.Fire3.ShouldUseAction( out act)) return true;
                    if (Actions.Fire.ShouldUseAction(out act)) return true;
                }

                //火起手上雷
                if(Config.GetBoolByName("StartFire") && !InTranspose)
                {
                    //上雷
                    if (Service.ClientState.LocalPlayer.CurrentMp == 8000 && AddThunder(lastComboActionID, out act)) return true;
                    //强插三连
                    if (Service.ClientState.LocalPlayer.CurrentMp == 6000 && Actions.Triplecast.ShouldUseAction(out act)) return true;
                }

                //如果通晓满了，就放掉。
                if (IsPolyglotStacksMaxed && JobGauge.EnochianTimer <7000)
                {
                    if (AddPolyglotAttach(out act)) return true;
                }

                //三连
                if (Service.ClientState.LocalPlayer.CurrentMp >= 4000 && Actions.Triplecast.ShouldUseAction(out act)) return true;

                //冰针不够，马上核爆
                if (JobGauge.UmbralHearts == 1 || Service.ClientState.LocalPlayer.CurrentMp < 3800)
                {
                    if (Actions.Flare.ShouldUseAction(out act)) return true;
                }
                //蓝不够，马上绝望
                if (Service.ClientState.LocalPlayer.CurrentMp < Actions.Fire4.MPNeed + Actions.Despair.MPNeed)
                {
                    if (Actions.Despair.ShouldUseAction(out act)) return true;
                }

                //试试看火2
                if (Actions.Fire2.ShouldUseAction(out act)) return true;

                //如果MP够打一发伤害。
                if (Service.ClientState.LocalPlayer.CurrentMp >= AttackAstralFire(out act))
                {
                    //火状态，攻击，强插三连。
                    //if (Actions.Triplecast.ShouldUseAction(out IAction action)) act = action;
                    return true;
                }

                //加个魔泉
                if (Actions.Manafont.ShouldUseAction(out act)) return true;

                //刚刚魔泉，别给我转冰了。
                if (LastAbility == Actions.Manafont.ID) return false;

                //否则，转入冰状态。
                if (JobGauge.PolyglotStacks == 2 || (JobGauge.PolyglotStacks == 1 && JobGauge.EnochianTimer < 3000))
                {
                    if((HasFire || !GeneralActions.Swiftcast.IsCoolDown || GeneralActions.Swiftcast.RecastTimeRemain < 5
                        || (Service.TargetManager.Target is BattleChara b &&
                        StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.Thunder, ObjectStatus.Thunder3) > 10))
                       && Service.ClientState.LocalPlayer.Level >= 90 && AddPolyglotAttach(out act)) return true;
                }
            }

            //赶在前面弄个激情
            if (!TargetHelper.InBattle && Actions.Sharpcast.ShouldUseAction(out act)) return true;

            if (Config.GetBoolByName("StartFire") && !JobGauge.InAstralFire && !JobGauge.InUmbralIce)
            {
                if (Actions.Fire2.ShouldUseAction(out act)) return true;
                if (Actions.Fire3.ShouldUseAction(out act)) return true;
            }

            //进入冰状态
            //试试看冰2,3,1给个冰状态
            if (Actions.Blizzard2.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.Blizzard3.ShouldUseAction(out act, lastComboActionID)) return true;

            if (Service.ClientState.LocalPlayer.Level < Actions.Blizzard3.Level && Actions.Transpose.ShouldUseAction(out act)) return true;

            //移动
            if (IsMoving && HaveTargetAngle)
            {
                if (AddPolyglotAttach(out act)) return true;
                if (Actions.Triplecast.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
                //if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;
            }

            if (!HaveTargetAngle && Maintence(out act)) return true;

            return false;
        }

        /// <summary>
        /// In AstralFire, maintain the time.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private uint AttackAstralFire( out IAction act)
        {
            uint addition = Service.ClientState.LocalPlayer.Level < Actions.Despair.Level ? 0u : 800u;

            if (Actions.Fire4.ShouldUseAction( out act)) return Actions.Fire4.MPNeed + addition;
            //if (Actions.Paradox.ShouldUseAction(out act)) return Actions.Paradox.MPNeed + addition;

            //如果有火苗了，那就来火3
            if (HasFire)
            {
                act = Actions.Fire3;
                return addition;
            }
            if (Actions.Fire.ShouldUseAction(out act)) return Actions.Fire.MPNeed + addition;

            return uint.MaxValue;
        }

        private bool AddThunder(uint lastAct, out IAction act)
        {
            //试试看雷2
            if (Actions.Thunder2.ShouldUseAction(out act, lastAct)) return true;

            //试试看雷1
            if (!InTranspose && Actions.Thunder.ShouldUseAction(out act, lastAct)) return true;

            if (Service.TargetManager.Target is BattleChara b &&
                            StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.Thunder, ObjectStatus.Thunder3) < 9
                            && Actions.Thunder.ShouldUseAction(out act, lastAct)) return true;

            return false;
        }

        private bool AddUmbralHearts(out IAction act)
        {
            //如果满了，或者等级太低，没有冰心，就别加了。
            act = null;
            if (JobGauge.UmbralHearts == 3 || Service.ClientState.LocalPlayer.Level < Actions.Blizzard4.Level) return false;

            //冻结
            if (Actions.Freeze.ShouldUseAction(out act)) return true;

            //冰4
            if (Actions.Blizzard4.ShouldUseAction(out act)) return true;

            return false;
        }

        private bool AddPolyglotAttach(out IAction act)
        {
            if (JobGauge.PolyglotStacks > 0)
            {
                if (Actions.Foul.ShouldUseAction(out act)) return true;
                if (Actions.Xenoglossy.ShouldUseAction(out act)) return true;
                if (Actions.Foul.ShouldUseAction(out act, mustUse: true)) return true;
            }
            act = null;
            return false;
        }

        private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
        {
            //混乱
            if (GeneralActions.Addle.ShouldUseAction(out act)) return true;
            return false;
        }
    }
}
