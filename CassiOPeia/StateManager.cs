using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace CassiOPeia
{
    class StateManager
    {
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        private static long LastQCast = 0;
        private static long LastECast = 0;

        public static float GetDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float ap = ObjectManager.Player.FlatMagicDamageMod + ObjectManager.Player.BaseAbilityDamage;
            if (spell == SpellSlot.E)
            {
                if (!Program.E.IsReady())
                    return 0;
                return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical, 55f + 25f * (Program.E.Level - 1) + 55 / 100 * ap);
            }
            return 0;
        }

        public static void Combo()
        {
            var target = TargetSelector2.GetTarget(850, DamageType.Magical);
            {
                if (Program.E.IsReady() && Program.ComboMenu["eCombo"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
                {
                    if ((target.HasBuffOfType(BuffType.Poison)))
                    {
                        if (target.IsValidTarget(Program.E.Range))
                        {
                            Program.E.Cast(target);
                        }
                    }
                }
                if (Program.Q.IsReady() && Program.ComboMenu["qCombo"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }

                if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && Program.ComboMenu["wCombo"].Cast<CheckBox>().CurrentValue && Environment.TickCount > LastQCast + Program.Q.CastDelay * 1000)
                {
                    Program.W.Cast(target);
                }
            }
        }
        public static void Harass()
        {
          var target = TargetSelector2.GetTarget(850, DamageType.Magical);
          {
            if (Program.E.IsReady() && Program.HarassMenu["eHarass"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.E.Range) && target != null && target.IsVisible && !target.IsDead)
            {
                if ((target.HasBuffOfType(BuffType.Poison)))
                {
                    if (target.IsValidTarget(Program.E.Range))
                    {
                        Program.E.Cast(target);
                    }
                }
            }
            if (Program.Q.IsReady() && Program.HarassMenu["qHarass"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Program.Q.Range))
            {
                Program.Q.Cast(target);
            }
          }
        }

        public static void LastHit()
        {
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => Program.E.IsInRange(x)
                && !x.IsDead
                && x.IsEnemy
                && x.HasBuffOfType(BuffType.Poison)
                && x.Health + 5 < GetDamage(SpellSlot.E, x)))
            {
                Program.E.Cast(minion);
            }
        }

        public static void ToggleHarass()
        {
            var target = TargetSelector2.GetTarget(850, DamageType.Magical);
            {
                if (Program.Q.IsReady() && Program.HarassMenu["qHarassToggle"].Cast<KeyBind>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }
            }
        }

        public static void Ultimate()
        {
            var target = TargetSelector2.GetTarget(500, DamageType.Magical);
            var castPred = Program.R.GetPrediction(target); 
            {
                {
                    foreach (
                        var enemy in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(enemy => enemy.Distance(_Player) <= Program.R.Range))
                    {
                        if (enemy.CountEnemiesInRange(500) >= Program.UltimateMenu["minR"].Cast<Slider>().CurrentValue && Program.UltimateMenu["useautoultimate"].Cast<CheckBox>().CurrentValue && enemy.IsFacing(ObjectManager.Player))
                        {
                            Program.R.Cast(target.Position);
                        }
                    }
                }
            }
        }
    }
}