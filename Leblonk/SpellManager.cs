using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Leblonk

{
    class SpellManager
    {
        public static float QDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 55, 80, 105, 130, 155 }[Program.Q.Level - 1] +
                0.4f * Program.myHero.FlatMagicDamageMod);
        }

        public static float Q2Damage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 55, 80, 105, 130, 155 }[Program.Q.Level - 1] +
                0.4f * Program.myHero.FlatMagicDamageMod);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 85, 125, 165, 205, 245 }[Program.W.Level - 1] +
                0.6f * Program.myHero.FlatMagicDamageMod);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 40, 65, 90, 115, 140 }[Program.W.Level - 1] +
                0.5f * Program.myHero.FlatMagicDamageMod);
        }

        public static float E2Damage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 40, 65, 90, 115, 140 }[Program.W.Level - 1] +
                0.5f * Program.myHero.FlatMagicDamageMod);
        }

        public static float RQDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 100, 200, 300 }[Program.R.Level - 1] +
                0.65f * Program.myHero.FlatMagicDamageMod);
        }

        public static float RQ2Damage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 100, 200, 300 }[Program.R.Level - 1] +
                0.65f * Program.myHero.FlatMagicDamageMod);
        }

        public static void Killsteal()
        {
            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget(Program.Q.Range))
                {
                    if (QDamage(enemy) >= enemy.Health && Program.KillstealMenu["ksQ"].Cast<CheckBox>().CurrentValue) { Program.Q.Cast(enemy); }
                    if (RQDamage(enemy) >= enemy.Health && !Program.Q.IsReady()
                        && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM"
                        && Program.KillstealMenu["ksQR"].Cast<CheckBox>().CurrentValue)
                    { Program.R.Cast(enemy); }
                }
            }
        }

        public static void Combo()
        {
            var target = TargetSelector2.GetTarget(1320, DamageType.Magical);
            var jumpPoint = Program.myHero.Position.Extend(target, 600).To3D();
            {
                if (Program.W.IsReady() && !target.IsValidTarget(Program.W.Range)
                && target.IsValidTarget(Program.W.Range + Program.Q.Range)
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn"
                && Program.Q.IsReady() && Program.R.IsReady()
                && target.Health < QDamage(target) + Q2Damage(target) + RQDamage(target))
                {
                    Program.W.Cast(jumpPoint);
                }
                if (Program.W.IsReady() && !target.IsValidTarget(Program.W.Range)
                && target.IsValidTarget(Program.W.Range + Program.Q.Range)
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn"
                && Program.Q.IsReady()
                && target.Health < QDamage(target))
                {
                    Program.W.Cast(jumpPoint);
                }


                if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }


                if (Program.R.IsReady() && target.IsValidTarget(Program.Q.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM")
                {
                    Program.R.Cast(target);
                }


                if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn"
                && !Program.R.IsReady())

                    Program.W.Cast(target);

                else if (Program.R.IsReady() && Program.LastCast[Program.Spells["W"]] + 150 < Environment.TickCount && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn")

                    Program.W.Cast(target);


                if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range) && Program.LastCast[Program.Spells["E"]] + 150 < Environment.TickCount)
                {
                    var e = Program.E.GetPrediction(target);
                    if (e.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                    {
                        var predE = Prediction.Position.PredictLinearMissile(target, Program.E.Range, 55, 250, 1600, 0);
                        Program.E.Cast(predE.CastPosition);
                    }
                }
            }
        }

        public static void Harass()
        {
            var target = TargetSelector2.GetTarget(720, DamageType.Magical);
            {
                if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range)
                    && Program.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue)
                {
                    Program.Q.Cast(target);

                }

                if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn"
                    && Program.HarassMenu["useW"].Cast<CheckBox>().CurrentValue)
                {
                    {
                        var w = Program.W.GetPrediction(target);
                        if (w.HitChance >= EloBuddy.SDK.Enumerations.HitChance.Low)
                        {
                            var predW = Prediction.Position.PredictCircularMissile(target, Program.E.Range, 500, 250, 1450);
                            Program.W.Cast(target);
                        }
                    }
                }

                if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range) && !Program.R.IsReady()
                     && Program.HarassMenu["useE"].Cast<CheckBox>().CurrentValue)
                {
                    var e = Program.E.GetPrediction(target);
                    if (e.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                    {
                        var predE = Prediction.Position.PredictLinearMissile(target, Program.E.Range, 55, 250, 1600, 0);
                        Program.E.Cast(predE.CastPosition);
                    }
                }
            }
        }


        public static void Flee()
        {
            var jumpPoint = Program.myHero.Position.Extend(Program.mousePos, 600).To3D();
            if (Program.W.IsReady()
                && Program.FleeMenu["useW"].Cast<CheckBox>().CurrentValue
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name != "leblancslidereturn")
                {
                            Program.W.Cast(jumpPoint);
                }

            if (Program.R.IsReady()
                && Program.FleeMenu["useWR"].Cast<CheckBox>().CurrentValue 
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM"
                && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name != "leblancslidereturnm")
            {
                Program.R.Cast(jumpPoint);
            }

        }

        public static void Chainz()
        {       
            var target = TargetSelector2.GetTarget(950, DamageType.Magical);
            if (Program.E.IsReady() && target.IsValidTarget(Program.E.Range))
                {
                    var e = Program.E.GetPrediction(target);
                    if (e.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High)
                    {
                        var predE = Prediction.Position.PredictLinearMissile(target, Program.E.Range, 55, 250, 1600, 0);
                        Program.E.Cast(predE.CastPosition);
                    }
                }
            if (Program.R.IsReady() && target.IsValidTarget(Program.E.Range) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM")
            {
                var e = Program.E.GetPrediction(target);
                if (e.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && Program.LastCast[Program.Spells["ER"]] + 1000 < Environment.TickCount)
                {
                    var predR = Prediction.Position.PredictLinearMissile(target, Program.E.Range, 55, 250, 1600, 0);                  
                    Program.R.Cast(predR.CastPosition);
                    
                }
            }
        }

        public static void ToggleHarass()
        {
            var target = TargetSelector2.GetTarget(720, DamageType.Magical);
            {
                if (Program.Q.IsReady() && Program.HarassMenu["qHarassToggle"].Cast<KeyBind>().CurrentValue && target.IsValidTarget(Program.Q.Range))
                {
                    Program.Q.Cast(target);
                }
            }
        }
    }
}