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
using CassiOPeia.Activator;
using Item = CassiOPeia.Activator.Item;

namespace CassiOPeia
{
    internal class Program
    {
        public static Dictionary<AIHeroClient, Slider> _SkinVals = new Dictionary<AIHeroClient, Slider>();
        public static Menu ComboMenu, HarassMenu, UltimateMenu, DrawingsMenu, menu;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        public static int Mana { get { return (int) _Player.Mana; } }
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            Hacks.AntiAFK = true;
            Bootstrap.Init(null);
            ItemManager.Init();
            TargetSelector2.init();
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 750, int.MaxValue, 150);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 250, 2500, 250);
            E = new Spell.Targeted(SpellSlot.E, 700);
            R = new Spell.Skillshot(SpellSlot.R, 825, SkillShotType.Cone, (int)0.6f, int.MaxValue, (int)(80 * Math.PI / 180));

            menu = MainMenu.AddMenu("CassiOPeia", "CassiOPeia");
            menu.AddGroupLabel("CassiOPeia");
            menu.AddLabel("Made by vOID");
            menu.AddSeparator();
            menu.AddLabel("Changelog:");
            menu.AddLabel("v0.01 - Initial release");
            menu.AddLabel("v0.02 - Auto-Ultimate, Harass");
            menu.AddLabel("v0.03 - Skin Hack");
            menu.AddLabel("v0.04 - Deleted Skin Hack (Was causing bugsplats), Added Interrupter, Toggle Q Harass, Farm with E");

            ComboMenu = menu.AddSubMenu("Combo Menu", "comboMenu");
            
            ComboMenu.AddGroupLabel("Combo Menu");
            ComboMenu.Add("qCombo", new CheckBox("Use Q"));
            ComboMenu.Add("wCombo", new CheckBox("Use W"));
            ComboMenu.Add("eCombo", new CheckBox("Use E"));

            HarassMenu = menu.AddSubMenu("Harass Menu", "harassMenu");
            HarassMenu.Add("qHarass", new CheckBox("Use Q"));
            HarassMenu.Add("eHarass", new CheckBox("Use E"));
            HarassMenu.Add("qHarassToggle", new KeyBind("Q Toggle Harass", false, KeyBind.BindTypes.PressToggle, 'T'));

            UltimateMenu = menu.AddSubMenu("Ultimate", "ultimateMenu");
            UltimateMenu.Add("useautoultimate", new CheckBox("Use Auto-Ultimate"));
            UltimateMenu.Add("ultimateinterrupt", new CheckBox("Use Ultimate to Interrupt Spells"));
            UltimateMenu.AddSeparator();
            UltimateMenu.Add("minR", new Slider("Minimum Enemies to Cast Ultimate", 2, 1, 5));

            DrawingsMenu = menu.AddSubMenu("Drawings", "drawingsMenu");
            DrawingsMenu.Add("drawq", new CheckBox("Draw Q Range"));

            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (UltimateMenu["ultimateinterruptt"].Cast<CheckBox>().CurrentValue)
            if (sender.IsValidTarget(Program.R.Range))
                if (Program.R.IsReady())
                    Program.R.Cast(sender);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingsMenu["drawq"].Cast<CheckBox>().CurrentValue)
                new Circle { Color = E.IsReady() ? Color.Green : Color.Red, Radius = Program.Q.Range }.Draw(ObjectManager.Player.Position);
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                StateManager.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                StateManager.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                StateManager.LastHit();
            }
            {
                StateManager.Ultimate();
            }
            {
                StateManager.ToggleHarass();
            }
        }
    }


}
