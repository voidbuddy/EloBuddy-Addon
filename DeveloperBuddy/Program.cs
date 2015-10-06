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

namespace DeveloperBuddy
{
    public static class Program
    {
        private static List<GameObject> _lstGameObjects = new List<GameObject>();
        private static List<GameObject> _lstObjCloseToMouse = new List<GameObject>();
        private static Menu Config, menu;
        private static int _lastUpdateTick = 0;
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {

            Hacks.AntiAFK = true;
            Bootstrap.Init(null);

            menu = MainMenu.AddMenu("devbuddy", "DeveloperBuddy");
            menu.AddGroupLabel("ヽ༼ຈل͜ຈ༽ﾉ RAISE YOUR DONGERS ヽ༼ຈل͜ຈ༽ﾉ ");
            menu.AddLabel("PORTED FROM YOU KNOW WHERE");
            menu.AddLabel("Ported by vOID github.com/voidbuddy");
            menu.AddLabel("www.elobuddy.net/user/188-void/");



            Config = menu.AddSubMenu("Config", "config");
            Config.AddGroupLabel("Config");
            Config.Add("range", new Slider("Range from mouse to obj", 400, 100, 1000));



            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;

        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Environment.TickCount - _lastUpdateTick > 150)
            {
                _lstGameObjects = ObjectManager.Get<GameObject>().ToList();
                _lstObjCloseToMouse =
                    _lstGameObjects.Where(o => o.Position.Distance(Game.CursorPos) < Config["range"].Cast<Slider>().CurrentValue && !(o is Obj_Turret) && o.Name != "missile" && !(o is Obj_LampBulb) && !(o is Obj_SpellMissile) && !(o is GrassObject) && !(o is DrawFX) && !(o is LevelPropSpawnerPoint) && !(o is Obj_GeneralParticleEmitter) && !o.Name.Contains("MoveTo")).ToList();
                _lastUpdateTick = Environment.TickCount;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            foreach (var obj in _lstObjCloseToMouse)
            {
                if (!obj.IsValid) return;
                var X = Drawing.WorldToScreen(obj.Position).X;
                var Y = Drawing.WorldToScreen(obj.Position).Y;
                Drawing.DrawText(X, Y + 10, System.Drawing.Color.Red, obj.Type.ToString());
                Drawing.DrawText(X, Y + 20, System.Drawing.Color.Red, "NetworkID: " + obj.NetworkId);
                Drawing.DrawText(X, Y + 30, System.Drawing.Color.Red, obj.Position.ToString());
                if (obj is Obj_AI_Base)
                {
                    var aiobj = obj as Obj_AI_Base;
                    Drawing.DrawText(X, Y + 40, System.Drawing.Color.Red, "Health: " + aiobj.Health + "/" + aiobj.MaxHealth + "(" + aiobj.HealthPercent + "%)");
                }
                if (obj is AIHeroClient)
                {
                    var hero = obj as AIHeroClient;
                    Drawing.DrawText(X, Y + 50, System.Drawing.Color.Red, "Spells:");
                    Drawing.DrawText(X, Y + 60, System.Drawing.Color.Red, "(Q): " + hero.Spellbook.Spells[0].Name);
                    Drawing.DrawText(X, Y + 70, System.Drawing.Color.Red, "(W): " + hero.Spellbook.Spells[1].Name);
                    Drawing.DrawText(X, Y + 80, System.Drawing.Color.Red, "(E): " + hero.Spellbook.Spells[2].Name);
                    Drawing.DrawText(X, Y + 90, System.Drawing.Color.Red, "(R): " + hero.Spellbook.Spells[3].Name);
                    Drawing.DrawText(X, Y + 100, System.Drawing.Color.Red, "(D): " + hero.Spellbook.Spells[4].Name);
                    Drawing.DrawText(X, Y + 110, System.Drawing.Color.Red, "(F): " + hero.Spellbook.Spells[5].Name);
                    var buffs = hero.Buffs;
                    if (buffs.Any())
                    {
                        Drawing.DrawText(X, Y + 120, System.Drawing.Color.Red, "Buffs:");
                    }
                    for (var i = 0; i < buffs.Count() * 10; i += 10)
                    {
                        Drawing.DrawText(X, (Y + 130 + i), System.Drawing.Color.Red, buffs[i / 10].Count + "x " + buffs[i / 10].Name);
                    }

                }
                if (obj is Obj_SpellMissile)
                {
                    var missile = obj as Obj_SpellMissile;
                    Drawing.DrawText(X, Y + 40, System.Drawing.Color.Red, "Missile Speed: " + missile.SData.MissileSpeed);
                    Drawing.DrawText(X, Y + 50, System.Drawing.Color.Red, "Cast Range: " + missile.SData.CastRange);
                }

                if (obj is MissileClient && obj.Name != "missile")
                {
                    var missile = obj as MissileClient;
                    Drawing.DrawText(X, Y + 40, System.Drawing.Color.Red, "Missile Speed: " + missile.SData.MissileSpeed);
                    Drawing.DrawText(X, Y + 50, System.Drawing.Color.Red, "Cast Range: " + missile.SData.CastRange);
                }
            }
        }
    }
}

