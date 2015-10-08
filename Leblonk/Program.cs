using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Leblonk.Activator;
using Item = Leblonk.Activator.Item;
namespace Leblonk
{
    public static class Program
    {
        public const string ChampName = "Leblanc";
        public static Spell.Targeted Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static Menu ComboMenu, KillstealMenu, HarassMenu, FleeMenu, DrawMenu, menu;
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }
        public static void Main(string[] args)

        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
            {
                return;
            }

            Hacks.AntiAFK = true;
            Bootstrap.Init(null);
            ItemManager.Init();
            TargetSelector2.init();

            Q = new Spell.Targeted(SpellSlot.Q, 720);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, int.MaxValue, 1450, 220);
            E = new Spell.Skillshot(SpellSlot.E, 950, SkillShotType.Linear, 250, 1600, 70);
            R = new Spell.Targeted(SpellSlot.R, 720);

            menu = MainMenu.AddMenu("LeBlanc", "LeBlanc");
            menu.AddGroupLabel("Once you go leblonk you never go back");
            menu.AddGroupLabel("ヽ༼ຈل͜ຈ༽ﾉ RAISE YOUR DONGERS ヽ༼ຈل͜ຈ༽ﾉ ");
            menu.AddLabel("Made by vOID github.com/voidbuddy");
            menu.AddLabel("www.elobuddy.net/user/188-void/");

            ComboMenu = menu.AddSubMenu("Combo Menu", "comboMenu");

            ComboMenu.AddGroupLabel("Combo Menu");
            ComboMenu.Add("qCombo", new CheckBox("Use Q"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("wCombo", new CheckBox("Use W"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("eCombo", new CheckBox("Use E"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("chainz", new KeyBind("2Chainz", false, KeyBind.BindTypes.HoldActive, 'A' ));

            HarassMenu = menu.AddSubMenu("Harass Menu", "HarassMenu");
            HarassMenu.AddGroupLabel("Harass Menu");
            HarassMenu.Add("useQ", new CheckBox("Use Q"));
            HarassMenu.AddSeparator();
            HarassMenu.Add("useW", new CheckBox("Use W"));
            HarassMenu.AddSeparator();
            HarassMenu.Add("useE", new CheckBox("Use E"));
            HarassMenu.Add("qHarassToggle", new KeyBind("Q Toggle Harass", false, KeyBind.BindTypes.PressToggle, 'T'));

            KillstealMenu = menu.AddSubMenu("Killsteal Menu", "killstealMenu");
            KillstealMenu.AddGroupLabel("Killsteal Menu");
            KillstealMenu.Add("ksQ", new CheckBox("Killsteal with Q"));
            KillstealMenu.Add("ksQR", new CheckBox("Killsteal with QR"));

            FleeMenu = menu.AddSubMenu("Flee Menu", "fleeMenu");
            FleeMenu.AddGroupLabel("Flee Menu");
            FleeMenu.Add("useW", new CheckBox("Flee with W"));
            FleeMenu.Add("useWR", new CheckBox("Flee with RW"));

            DrawMenu = menu.AddSubMenu("Draw Menu", "drawMenu");
            DrawMenu.AddGroupLabel("Draw Menu");
            DrawMenu.Add("drawQ", new CheckBox("Draw Q"));
            DrawMenu.Add("drawE", new CheckBox("Draw E"));
            DrawMenu.Add("drawWQ", new CheckBox("Draw W+Q"));
            DrawMenu.Add("killable", new CheckBox("Draw Killable"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }

        public static Vector3 mousePos
        {
            get { return Game.CursorPos; }
        }
        
        private static void Game_OnTick(EventArgs args)
        {         
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                SpellManager.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                SpellManager.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                SpellManager.Flee();
            }
            if (ComboMenu["chainz"].Cast<KeyBind>().CurrentValue)
            {
                SpellManager.Chainz();
                Orbwalker.OrbwalkTo(mousePos);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                SpellManager.Killsteal();
            }
            {
                SpellManager.ToggleHarass();
            }

        }

        public static Dictionary<string, string> Spells = new Dictionary<string, string>
        {
            {"ER", "LeblancSoulShackleM"},
            {"E", "LeblancSoulShackle"},
            {"W", "LeblancSlide"},
        };

        public static Dictionary<string, long> LastCast = new Dictionary<string, long>
        {
            {"LeblancSoulShackleM", 0},
            {"LeblancSoulShackle", 0},
            {"LeblancSlide", 0},
        };

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            LastCast[Program.Spells["ER"]] = Environment.TickCount;
            LastCast[Program.Spells["E"]] = Environment.TickCount;
            LastCast[Program.Spells["W"]] = Environment.TickCount;
        }

        private static void OnDraw(EventArgs args)
        {
            var target = TargetSelector2.GetTarget(1320, DamageType.Magical);

            Boolean drawQ = DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue;
            Boolean drawE = DrawMenu["drawE"].Cast<CheckBox>().CurrentValue;
            Boolean drawWQ = DrawMenu["drawWQ"].Cast<CheckBox>().CurrentValue;
            Boolean drawKillable = DrawMenu["killable"].Cast<CheckBox>().CurrentValue;


            if (Q.IsReady() && drawQ)
                Circle.Draw(SharpDX.Color.Aqua, Q.Range, Player.Instance.Position);
            if (E.IsReady() && drawE)
                Circle.Draw(SharpDX.Color.AliceBlue, E.Range, Player.Instance.Position);
            if (drawWQ)
                Circle.Draw(SharpDX.Color.BlanchedAlmond, Q.Range + W.Range, Player.Instance.Position);

            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if (drawKillable && enemy.Health < SpellManager.QDamage(enemy)
                    && Q.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q = DEAD", 200);

                    if (drawKillable && enemy.Health < SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.WDamage(enemy)
                    && enemy.Health > SpellManager.QDamage(enemy)
                    && Q.IsReady() && W.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q + W = DEAD", 500);

                    if (drawKillable && enemy.Health < SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy)
                    && enemy.Health > SpellManager.QDamage(enemy) + SpellManager.WDamage(target)
                    && Q.IsReady() && R.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q + R = DEAD", 500);

                if (drawKillable && enemy.Health < SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy) + SpellManager.RQ2Damage(enemy) + SpellManager.WDamage(enemy)
                    && enemy.Health > SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy)
                    && Q.IsReady() && R.IsReady() && W.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q + R + W = DEAD", 500);

                if (drawKillable && enemy.Health < SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy) + SpellManager.RQ2Damage(enemy) + SpellManager.WDamage(enemy) + SpellManager.EDamage(enemy)
                    && enemy.Health > SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy) + SpellManager.RQ2Damage(enemy) + SpellManager.WDamage(enemy)
                    && Q.IsReady() && R.IsReady() && W.IsReady() && E.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q + R + W + E = DEAD", 500);

                if (drawKillable && enemy.Health < SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy) + SpellManager.RQ2Damage(enemy) + SpellManager.WDamage(enemy) + SpellManager.EDamage(enemy) + SpellManager.E2Damage(enemy)
                    && enemy.Health > SpellManager.QDamage(enemy) + SpellManager.Q2Damage(enemy) + SpellManager.RQDamage(enemy) + SpellManager.RQ2Damage(enemy) + SpellManager.WDamage(enemy) + SpellManager.EDamage(enemy)
                    && Q.IsReady() && R.IsReady() && W.IsReady() && E.IsReady())
                    Drawing.DrawText(Drawing.WorldToScreen(enemy.Position), System.Drawing.Color.White, "Q + R + W + E + E2 = DEAD", 500);
            }
        }
    }
}
