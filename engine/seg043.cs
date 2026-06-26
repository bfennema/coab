using Classes;
using Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace engine
{
    public class seg043
    {
        static bool in_print_and_exit = false;
        public static void print_and_exit()
        {
            if (in_print_and_exit == false)
            {
                in_print_and_exit = true;

                seg044.PlaySound(Sound.sound_FF);

                Logger.Close();

                ItemLibrary.Write();

                seg001.EngineStop();
            }
        }

        public static void DumpPlayerAffects()
        {
            foreach (Player player in gbl.TeamList)
            {
                foreach (Affect affect in player.affects)
                {
                    Logger.Debug("who: {0}  sp#: {1} - {2}", player.name, (int)affect.type, affect.type);
                }
            }
        }

        public static void ToggleCommandDebugging()
        {
            gbl.printCommands = !gbl.printCommands;

            if (gbl.printCommands == true)
            {
                Logger.Debug(System.DateTime.Now.ToString());
            }
        }

        internal static void clear_one_keypress()
        {
            if (Input.KEYPRESSED() == true)
            {
                Input.GetInputKey();
            }
        }


        public static void DumpTreasureItems()
        {
            //for (int i = 0; i < 0x81; i++)
            //{
            //    //var older = new System.Collections.Generic.List<string>();

            //    //for (int j = 0; j < 1000; j++)
            //    //{
            //    Item it = ovr022.create_item(i);
            //    it.name = it.GenerateName(0);
            //    Player pl = new Player();
            //    pl.field_151 = it;
            //    bool ranged = ovr025.is_weapon_ranged(pl);
            //    bool rangedMelee = ovr025.is_weapon_ranged_melee(pl);

            //    //if (older.Contains(name) == false)
            //    //{
            //    Logging.Logger.Debug("Id: {0} {1} Ranged: {2} Ranged-Melee: {3}", i, it.name, ranged, rangedMelee);
            //    //older.Add(name);
            //    //}
            //    //}
            //}
        }

        static void TxtDumpPlayer(Player p, int area, int id)
        {
            string str100 = p.stats.Str.Current == 18 ? string.Format("({0})", p.stats.Str00.Current) : "";
            Logger.Debug("Area {0} Id {1} {2} exp: {3} hp: {4} ac: {5} thac0: {6}", area, id, p.name, p.exp, p.hit_point_max, p.DisplayAc, 0x3c - p.hitBonus);
            Logger.Debug("   S: {0}{1} D: {2} C: {3} I: {4} W: {5} Ch: {6}", p.stats.Str.Current, str100, p.stats.Dex.Current, p.stats.Con.Current, p.stats.Int.Current, p.stats.Wis.Current, p.stats.Cha.Current);
            Logger.Debug("   Lvls: {0} {1} {2} {3} {4} {5} {6} {7}", p.ClassLevel[0], p.ClassLevel[1], p.ClassLevel[2], p.ClassLevel[3], p.ClassLevel[4], p.ClassLevel[5], p.ClassLevel[6], p.ClassLevel[7]);
            if (p.activeItems.primaryWeapon != null)
                Logger.Debug("   Weapon: {0}", p.activeItems.primaryWeapon.GenerateName(0));
            if (p.activeItems.armor != null)
                Logger.Debug("   Armor: {0}", p.activeItems.armor.GenerateName(0));

            Logger.Debug("   Damage: {0}d{1}{2}{3}", p.attack1_DiceCount, p.attack1_DiceSize,
                p.attack1_DamageBonus > 0 ? "+" : "", p.attack1_DamageBonus != 0 ? p.attack1_DamageBonus.ToString() : "");

            foreach (int sp in p.spellList.IdList())
            {
                Logger.Debug("   Spell: {0}", ovr023.SpellNames[sp]);
            }

            foreach (var af in p.affects)
            {
                Logger.Debug("   Affect: {0}", af.type);
            }
        }


        private static void HtmlTableDumpPlayer(DebugWriter dw, Player p, byte area, int id)
        {
            dw.Write("<tr>");
            dw.Write("<td>{0}</td>", area);
            dw.Write("<td>{0}</td>", id);
            dw.Write("<td nowrap=\"nowrap\">{0}</td>", p.name);
            dw.Write("<td>{0}</td>", p.exp);
            dw.Write("<td>{0}</td>", p.hit_point_max);
            dw.Write("<td>{0}</td>", 0x3c - p.ac);
            dw.Write("<td>{0}</td>", 0x3c - p.hitBonus);
            string str100 = p.stats.Str.Current == 18 ? string.Format("({0})", p.stats.Str00.Current) : "";
            dw.Write("<td>{0}{1}</td>", p.stats.Str.Current, str100);
            dw.Write("<td>{0}</td>", p.stats.Dex.Current);
            dw.Write("<td>{0}</td>", p.stats.Con.Current);
            dw.Write("<td>{0}</td>", p.stats.Int.Current);
            dw.Write("<td>{0}</td>", p.stats.Wis.Current);
            dw.Write("<td>{0}</td>", p.stats.Cha.Current);
            dw.Write("<td>{0}</td>", p.ClassLevel[0]);
            dw.Write("<td>{0}</td>", p.ClassLevel[1]);
            dw.Write("<td>{0}</td>", p.ClassLevel[2]);
            dw.Write("<td>{0}</td>", p.ClassLevel[3]);
            dw.Write("<td>{0}</td>", p.ClassLevel[4]);
            dw.Write("<td>{0}</td>", p.ClassLevel[5]);
            dw.Write("<td>{0}</td>", p.ClassLevel[6]);
            dw.Write("<td>{0}</td>", p.ClassLevel[7]);

            dw.Write("<td nowrap=\"nowrap\">{0}</td>", p.activeItems.primaryWeapon != null ? p.activeItems.primaryWeapon.GenerateName(0) : "");
            dw.Write("<td nowrap=\"nowrap\">{0}</td>", p.activeItems.armor != null ? p.activeItems.armor.GenerateName(0) : "");
            dw.Write("<td nowrap=\"nowrap\">{0}d{1}{2}{3}</td>", p.attack1_DiceCount, p.attack1_DiceSize,
                p.attack1_DamageBonus > 0 ? "+" : "", p.attack1_DamageBonus != 0 ? p.attack1_DamageBonus.ToString() : "");

            int last = 0;
            int count = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (int sp in p.spellList.IdList())
            {
                if (sp != last)
                {
                    if (last != 0)
                    {
                        sb.Append(ovr023.SpellNames[last]);
                        if (count > 1)
                        {
                            sb.Append(string.Format(" ({0})", count));
                        }
                        sb.Append(", ");
                    }
                    last = sp;
                    count = 1;
                }
                else
                {
                    count += 1;
                }
            }

            if (last != 0)
            {
                sb.Append(ovr023.SpellNames[last]);
                if (count > 1)
                {
                    sb.Append(string.Format(" ({0})", count));
                }
            }

            dw.Write("<td nowrap=\"nowrap\">{0}</td>", sb.ToString());


            sb = new System.Text.StringBuilder();
            foreach (var af in p.affects)
            {
                sb.Append(af.type.ToString());
                sb.Append(", ");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            dw.Write("<td nowrap=\"nowrap\">{0}</td>", sb.ToString());
            dw.WriteLine("</tr>");
        }

        public static async Task<bool> DumpMonstersFiltered()
        {
            var bkupArea = gbl.game_area;

            string filename = System.IO.Path.Combine(Logger.GetPath(), "MonsterFiltered.txt");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            DebugWriter dw = new DebugWriter(filename);

            dw.WriteLine("GnomeVsManSizedGiant");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.GnomeBonus) != 0);
            dw.WriteLine("");

            dw.WriteLine("RangerBonus");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.RangerBonus) != 0);
            dw.WriteLine("");

            dw.WriteLine("Giant");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Giant) != 0);
            dw.WriteLine("");

            dw.WriteLine("Dragon");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Dragon) != 0);
            dw.WriteLine("");

            dw.WriteLine("Undead");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Undead) != 0);
            dw.WriteLine("");

            dw.WriteLine("Cold");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Cold) != 0);
            dw.WriteLine("");

            dw.WriteLine("Fire");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Fire) != 0);
            dw.WriteLine("");

            dw.WriteLine("Regenrate");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Regenerate) != 0);
            dw.WriteLine("");

            dw.WriteLine("Aviant");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Avian) != 0);
            dw.WriteLine("");

            dw.WriteLine("Snake");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Snake) != 0);
            dw.WriteLine("");

            dw.WriteLine("Plant");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Plant) != 0);
            dw.WriteLine("");

            dw.WriteLine("Animal");
            await DumpMonstersFilteredSub(dw, p => (p.flags & Flags.Animal) != 0);
            dw.WriteLine("");


            dw.Close();

            gbl.game_area = bkupArea;

            return true;
        }

        static async Task<bool> DumpMonstersFilteredSub(DebugWriter dw, System.Predicate<Player> filter)
        {
            for (byte area = 1; area <= 6; area++)
            {
                gbl.game_area = area;
                for (int id = 0; id < 256; id++)
                {
                    Player p = await ovr017.load_mob(id, false);
                    if (p != null)
                    {
                        ovr025.reclac_player_values(p);

                        if (filter(p))
                        {
                            dw.WriteLine(p.ToString());
                        }
                    }
                }
            }
            return true;
        }

        public static async Task<bool> DumpMonsters()
        {
            await DumpMonstersFiltered();

            var bkupArea = gbl.game_area;

            string filename = System.IO.Path.Combine(Logger.GetPath(), "Monster.html");
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
            DebugWriter dw = new DebugWriter(filename);

            dw.WriteLine("<html><body><table><tbody>");

            for (byte area = 1; area <= 6; area++)
            {
                gbl.game_area = area;
                for (int id = 0; id < 256; id++)
                {
                    Player p = await ovr017.load_mob(id, false);
                    if (p != null)
                    {
                        ovr025.reclac_player_values(p);

                        //TxtDumpPlayer(p, area, id);
                        HtmlTableDumpPlayer(dw, p, area, id);
                    }
                }
            }

            dw.WriteLine("</tbody></table></body></html>");
            dw.Close();

            gbl.game_area = bkupArea;

            return true;
        }
        public static async void CompareSave(string file_name)
        {
            System.IO.Stream file = await seg042.find_and_open_file(true, gbl.SavePath, file_name);

            var saveData = new SaveData(file);

            if (gbl.saveData != saveData)
            {
                string filename = System.IO.Path.Combine(Logger.GetPath(), "CompareSave.txt");
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
                DebugWriter dw = new DebugWriter(filename);

                dw.WriteLine("                      {0,-15} Loaded", file_name);

                if (saveData.game_area != gbl.saveData.game_area)
                {
                    dw.WriteLine("game_area             {0,-15} {1}", saveData.game_area, gbl.saveData.game_area);
                }
                if (saveData.mapPosX != gbl.saveData.mapPosX)
                {
                    dw.WriteLine("mapPosX               {0,-15} {1}", saveData.mapPosX, gbl.saveData.mapPosX);
                }
                if (saveData.mapPosY != gbl.saveData.mapPosY)
                {
                    dw.WriteLine("mapPosY               {0,-15} {1}", saveData.mapPosY, gbl.saveData.mapPosY);
                }
                if (saveData.mapDirection != gbl.saveData.mapDirection)
                {
                    dw.WriteLine("mapDirection          {0,-15} {1}", saveData.mapDirection, gbl.saveData.mapDirection);
                }
                if (saveData.mapWallType != gbl.saveData.mapWallType)
                {
                    dw.WriteLine("mapWallType           {0,-15} {1}", saveData.mapWallType, gbl.saveData.mapWallType);
                }
                if (saveData.mapWallRoof != gbl.saveData.mapWallRoof)
                {
                    dw.WriteLine("mapWallRoof           {0,-15} {1}", saveData.mapWallRoof, gbl.saveData.mapWallRoof);
                }
                if (saveData.game_state != gbl.saveData.game_state)
                {
                    dw.WriteLine("game_state            {0,-15} {1}", saveData.game_state, gbl.saveData.game_state);
                }
                if (saveData.last_game_state != gbl.saveData.last_game_state)
                {
                    dw.WriteLine("last_game_state       {0,-15} {1}", saveData.last_game_state, gbl.saveData.last_game_state);
                }
                if (saveData.area_ptr != gbl.saveData.area_ptr)
                {
                    var fileByteArray = saveData.area_ptr.ToByteArray();
                    var byteArray = gbl.saveData.area_ptr.ToByteArray();
                    for (int i = 0; i < fileByteArray.Length; i+=2)
                    {
                        if (fileByteArray[i] != byteArray[i] || fileByteArray[i+1] != byteArray[i+1])
                        {
                            dw.WriteLine("  area_ptr.field_{0:X4} 0x{1:X4} ({1})      0x{2:X4} ({2})", gbl.vm_mem0_offset + (i/2), fileByteArray[i+1] << 8 | fileByteArray[i], byteArray[i+1] << 8 | byteArray[i]);
                        }
                    }
                }
                if (saveData.stru_1B2CA != gbl.saveData.stru_1B2CA)
                {
                    var fileByteArray = saveData.stru_1B2CA.ToByteArray();
                    var byteArray = gbl.saveData.stru_1B2CA.ToByteArray();
                    for (int i = 0; i < fileByteArray.Length; i += 2)
                    {
                        if (fileByteArray[i] != byteArray[i] || fileByteArray[i + 1] != byteArray[i + 1])
                        {
                            dw.WriteLine("stru_1B2CA.field_{0:X4} 0x{1:X4} ({1})      0x{2:X4} ({2})", gbl.vm_mem2_offset + (i / 2), fileByteArray[i + 1] << 8 | fileByteArray[i], byteArray[i + 1] << 8 | byteArray[i]);
                        }
                    }
                }
                if (saveData.area2_ptr != gbl.saveData.area2_ptr)
                {
                    var fileByteArray = saveData.area2_ptr.ToByteArray();
                    var byteArray = gbl.saveData.area2_ptr.ToByteArray();
                    for (int i = 0; i < fileByteArray.Length; i += 2)
                    {
                        if (fileByteArray[i] != byteArray[i] || fileByteArray[i + 1] != byteArray[i + 1])
                        {
                            dw.WriteLine(" area2_ptr.field_{0:X4} 0x{1:X4} ({1})      0x{2:X4} ({2})", gbl.vm_mem1_offset + (i / 2), fileByteArray[i + 1] << 8 | fileByteArray[i], byteArray[i + 1] << 8 | byteArray[i]);
                        }
                    }
                }
                //if (saveData.ecl_ptr != gbl.saveData.ecl_ptr)
                //{
                //    var fileByteArray = saveData.ecl_ptr.ToByteArray();
                //    var byteArray = gbl.saveData.ecl_ptr.ToByteArray();
                //    for (int i = 0; i < fileByteArray.Length; i ++)
                //    {
                //        if (fileByteArray[i] != byteArray[i])
                //        {
                //            dw.WriteLine("       ecl_ptr 0x{0:X4} 0x{1:X4} ({1})      0x{2:X4} ({2})", gbl.initial_ecl_offset + i, fileByteArray[i], byteArray[i]);
                //        }
                //    }
                //}

                dw.Close();
            }
        }
    }
}
