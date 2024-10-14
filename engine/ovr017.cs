using Classes;
using System.Collections.Generic;
using Logging;
using System;

namespace engine
{
    class ovr017
    {
        static async IAsyncEnumerable<(string,string,string)> BuildLoadablePlayersLists(Logging.Game game, short playerFileSize, int npcOffset, int nameOffset, string fileFilter) // sub_4708B
        {
            byte[] data = new byte[16];

            string path = game == Logging.Game.None ? gbl.SavePath : Config.SavePathByGame[(int)game];

            await foreach ((var file, var stream) in gbl.file.OpenAll(path, fileFilter))
            {
                if (stream.Length == playerFileSize)
                {
                    stream.Seek(nameOffset, System.IO.SeekOrigin.Begin);
                    stream.Read(data, 0, 16);

                    string playerName = Sys.ArrayToString(data, 0, 15).Trim();

                    byte var_164;

                    if (gbl.import_from == ImportSource.Hillsfar)
                    {
                        var_164 = 0;
                    }
                    else
                    {
                        stream.Seek(npcOffset, System.IO.SeekOrigin.Begin);
                        stream.Read(data, 0, 1);
                        var_164 = data[0];
                    }

                    string fullNameText =
                        string.Compare(System.IO.Path.GetExtension(file), ".SAV", true) == 0 ?
                        string.Format("{0,-15} from save game {1}", playerName, file[6]) : playerName;

                    bool found = gbl.TeamList.Find(player => playerName == player.name.Trim()) != null;

                    if (found == false && var_164 <= 0x7F)
                    {
                        yield return (path, file, fullNameText);
                    }
                }

                stream.Close();
            }
        }

        static int[] PlayerNameOffset = { 0, 0, 4 };
        static int[] NpcFileOffset = { 0xf7, 0x84, 0x13 };

        internal static async IAsyncEnumerable<(string, string, string)> BuildLoadablePlayersLists() // sub_47465
        {
            if (gbl.import_from == ImportSource.Curse)
            {
                await foreach ((var a, var b, var c) in BuildLoadablePlayersLists(Logging.Game.CurseOfTheAzureBonds, Classes.Curse.Player.StructSize, NpcFileOffset[0], PlayerNameOffset[0], "*.GUY"))
                {
                    yield return (a, b, c);
                }
                await foreach ((var a, var b, var c) in BuildLoadablePlayersLists(Logging.Game.CurseOfTheAzureBonds, Classes.Curse.Player.StructSize, NpcFileOffset[0], PlayerNameOffset[0], "*.SAV"))
                {
                    yield return (a, b, c);
                }
            }
            else if (gbl.import_from == ImportSource.Pool)
            {
                await foreach ((var a, var b, var c) in BuildLoadablePlayersLists(Logging.Game.PoolOfRadiance, Classes.PoolRad.Player.StructSize, NpcFileOffset[1], PlayerNameOffset[1], "*.CHA"))
                {
                    yield return (a, b, c);
                }
                await foreach ((var a, var b, var c) in BuildLoadablePlayersLists(Logging.Game.PoolOfRadiance, Classes.PoolRad.Player.StructSize, NpcFileOffset[1], PlayerNameOffset[1], "*.SAV"))
                {
                    yield return (a, b, c);
                }
            }
            else if (gbl.import_from == ImportSource.Hillsfar)
            {
                await foreach ((var a, var b, var c) in BuildLoadablePlayersLists(Logging.Game.None, HillsFarPlayer.StructSize, NpcFileOffset[2], PlayerNameOffset[2], "*.HIL"))
                {
                    yield return (a, b, c);
                }
            }
        }

        static Set unk_47635 = new Set(0, 5);


        internal static void LoadPlayerCombatIcon(bool recolour) /* sub_47A90 */
        {
            seg042.set_game_area(1);

            Player player = gbl.SelectedPlayer;

            char[] sizeToken = new char[] { '\0', 'S', 'T' };

            ovr034.chead_cbody_comspr_icon(11, player.head_icon, "CHEAD" + sizeToken[player.icon_size].ToString());
            ovr034.chead_cbody_comspr_icon(player.icon_id, player.weapon_icon, "CBODY" + sizeToken[player.icon_size].ToString());

            gbl.combat_icons[player.icon_id].MergeIcon(gbl.combat_icons[11]);

            if (recolour)
            {
                byte[] newColors = new byte[16];
                byte[] oldColors = new byte[16];

                for (byte i = 0; i <= 15; i++)
                {
                    oldColors[i] = i;
                    newColors[i] = i;
                }

                for (int i = 0; i < 6; i++)
                {
                    newColors[gbl.default_icon_colours[i]] = (byte)(player.icon_colours[i] & 0x0F);
                    newColors[gbl.default_icon_colours[i] + 8] = (byte)((player.icon_colours[i] & 0xF0) >> 4);
                }

                gbl.combat_icons[player.icon_id].Recolor(false, newColors, oldColors);
            }

            ovr034.ReleaseCombatIcon(11);
            seg042.restore_game_area();
            seg043.clear_keyboard();
        }


        internal static async void remove_player_file(Player player)
        {
            var filename = File.CleanFilename(player.name);

            gbl.file.Delete(gbl.SavePath, string.Format("{0}.{1}", filename, gbl.game.SavePlayerExt));
            gbl.file.Delete(gbl.DataPath, string.Format("{0}.{1}", filename, gbl.game.SaveItemExt));
            gbl.file.Delete(gbl.SavePath, string.Format("{0}.{1}", filename, gbl.game.SaveAffectExt));
        }

        internal static async void SavePlayer(string arg_0, Player player) // sub_47DFC
        {
            char input_key;

            gbl.import_from = ImportSource.Curse;

            string ext_text;
            string file_text;

            if (arg_0 == "")
            {
                ext_text = gbl.game.SavePlayerExt;
                file_text = File.CleanFilename(player.name);
            }
            else
            {
                ext_text = "SAV";
                file_text = arg_0;
            }

            input_key = 'N';

            while (input_key == 'N' &&
                arg_0.Length == 0 &&
                await gbl.file.Find(gbl.SavePath, string.Format("{0}.{1}", file_text, ext_text)) == true)
            {
                input_key = ovr027.yes_no(gbl.alertMenuColors, "Overwrite " + file_text + "? ");

                if (input_key == 'N')
                {
                    file_text = string.Empty;

                    while (file_text == string.Empty)
                    {
                        file_text = seg041.getUserInputString(8, 0, 10, "New file name: ");
                    }
                }
            }
            System.IO.Stream player_stream;
            System.IO.Stream? item_stream = null, affect_stream = null;

            player_stream = await gbl.file.Create(gbl.SavePath, string.Format("{0}.{1}", file_text, ext_text));

            if (player.items.Count > 0)
            {
                item_stream = await gbl.file.Create(gbl.SavePath, string.Format("{0}.{1}", file_text, gbl.game.SaveItemExt));
            }
            else
            {
                gbl.file.Delete(gbl.SavePath, string.Format("{0}.{1}", file_text, gbl.game.SaveItemExt));
            }

            if (player.affects.Count > 0)
            {
                affect_stream = await gbl.file.Create(gbl.SavePath, string.Format("{0}.{1}", file_text, gbl.game.SaveAffectExt));
            }
            else
            {
                gbl.file.Delete(gbl.SavePath, string.Format("{0}.{1}", file_text, gbl.game.SaveAffectExt));
            }

            gbl.game.SavePlayer(player, player_stream, item_stream, affect_stream);

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(player.GetType());
            var stream = await gbl.file.Create(gbl.SavePath, string.Format("{0}.XML", file_text));
            stream.SetLength(0);
            x.Serialize(stream, player);
            stream.Close();
        }

        internal static async System.Threading.Tasks.Task<bool> PlayerFileExists(string fileExt, string player_name) // sub_483AE
        {
            byte[] data = new byte[0x10];

            await foreach ((var filename, var stream) in gbl.file.OpenAll(gbl.SavePath, string.Format("*{1}", fileExt)))
            {
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                stream.Read(data, 0, 16);
                stream.Close();

                string in_file_name = Sys.ArrayToString(data, 0, 15).Trim();
                if (in_file_name == player_name)
                {
                    return true;
                }
            }
            return false;
        }


        internal static void TransferHillsFarCharacter(HillsFarPlayer hf_player, Player player, Player previousSelectPlayer) // sub_48F35
        {
            if (player.stats.Str.cur < hf_player.stat_str)
            {
                player.stats.Str.Load(hf_player.stat_str);
            }

            if (player.stats.Str00.cur < hf_player.stat_str00)
            {
                player.stats.Str00.Load(hf_player.stat_str00);
            }

            if (player.stats.Int.cur < hf_player.stat_int)
            {
                player.stats.Int.Load(hf_player.stat_int);
            }

            if (player.stats.Wis.cur < hf_player.stat_wis)
            {
                player.stats.Wis.Load(hf_player.stat_wis);
            }

            if (player.stats.Dex.cur < hf_player.stat_dex)
            {
                player.stats.Dex.Load(hf_player.stat_dex);
            }

            if (player.stats.Con.cur < hf_player.stat_con)
            {
                player.stats.Con.Load(hf_player.stat_con);
            }

            if (player.stats.Cha.cur < hf_player.stat_cha)
            {
                player.stats.Cha.Load(hf_player.stat_cha);
            }

            if (player.exp < hf_player.field_2E)
            {
                player.exp = hf_player.field_2E;
            }

            // If imported player has more than 500 platinum import that amount.
            if (player.Money.GetGoldWorth() < hf_player.field_28)
            {
                for (int slot = 0; slot < 5; slot++)
                {
                    ovr022.DropCoins(slot, player.Money.GetCoins(slot), player);
                }
                ovr022.addPlayerGold((short)(hf_player.field_28 / 5));
            }

            if (player.age < hf_player.age)
            {
                player.age = hf_player.age;
            }

            player.cleric_lvl = (hf_player.field_B7 > 0) ? (byte)1 : (byte)0;
            player.magic_user_lvl = (hf_player.field_B8 > 0) ? (byte)1 : (byte)0;
            player.fighter_lvl = (hf_player.field_B9 > 0) ? (byte)1 : (byte)0;
            player.thief_lvl = (hf_player.field_BA > 0) ? (byte)1 : (byte)0;

            player.HitDice = 1;

            if (hf_player.field_26 != 0)
            {
                player.field_192 = 1;
            }

            SilentTrainPlayer();
            gbl.SelectedPlayer = previousSelectPlayer;

            player.hit_point_max = hf_player.field_21;
            player.hit_point_rolled = (byte)(player.hit_point_max - ovr018.get_con_hp_adj(player));
            player.hit_point_current = hf_player.field_20;
        }

        internal static void SilentTrainPlayer()
        {
            gbl.area2_ptr.training_class_mask = 0xff;
            gbl.can_train_no_more = false;
            gbl.silent_training = true;

            do
            {
                ovr018.train_player();
            } while (gbl.can_train_no_more == false);

            gbl.silent_training = false;
        }


        static ClassId[] HillsFarClassMap = {
    ClassId.unknown,    ClassId.thief,      ClassId.fighter,    ClassId.mc_f_t, ClassId.magic_user,
    ClassId.mc_mu_t,    ClassId.mc_f_mu,    ClassId.mc_f_mu_t,  ClassId.cleric, ClassId.mc_c_t,
    ClassId.mc_c_f,     ClassId.unknown,    ClassId.mc_c_mu,    ClassId.unknown, ClassId.mc_c_f_m, 
    ClassId.unknown};


        internal static async System.Threading.Tasks.Task<Player> import_char01(string path, string filename)
        {
            Player player = null;
            System.IO.Stream player_stream;
            System.IO.Stream? item_stream = null, affect_stream = null;

            player_stream = await seg042.find_and_open_file(false, path, filename);

            seg041.displayString("Loading...Please Wait", 0, 10, 0x18, 0);

            if (gbl.import_from == ImportSource.Hillsfar)
            {
                byte[] data = new byte[HillsFarPlayer.StructSize];
                gbl.file.BlockRead(HillsFarPlayer.StructSize, data, player_stream);
                gbl.file.Close(player_stream);

                HillsFarPlayer var_1C4 = new HillsFarPlayer(data);

                player = await ConvertHillsFarPlayer(var_1C4, filename);
            }
            else if (gbl.import_from == gbl.game.ImportFrom)
            {
                filename = string.Format("{0}.{1}", System.IO.Path.GetFileNameWithoutExtension(filename), gbl.game.SaveItemExt);

                if (await gbl.file.Find(path, filename) == true)
                {
                    item_stream = await seg042.find_and_open_file(false, path, filename);
                }

                filename = string.Format("{0}.{1}", System.IO.Path.GetFileNameWithoutExtension(filename), gbl.game.SaveAffectExt);

                if (await gbl.file.Find(path, filename) == true)
                {
                    affect_stream = await seg042.find_and_open_file(false, path, filename);
                }

                player = gbl.game.LoadPlayer(player_stream, item_stream, affect_stream);
            }
            else
            {
                player = await gbl.import_func[(int)gbl.import_from](player_stream, path, System.IO.Path.GetFileNameWithoutExtension(filename));
            }

            seg043.clear_keyboard();
            ovr025.reclac_player_values(player);
            ovr026.ReclacClassBonuses(player);

            return player;
        }


        private static async System.Threading.Tasks.Task<Player> ConvertHillsFarPlayer(HillsFarPlayer hf_player, string arg_8)
        {
            Player player = new Player();
            System.IO.Stream file;

            player.items = new List<Item>();
            player.affects = new List<Affect>();
            player.actions = null;

            string fileExt = ".GUY";

            if (await PlayerFileExists(fileExt, hf_player.name) == true)
            {
                string savename = System.IO.Path.ChangeExtension(arg_8, fileExt);

                file = await seg042.find_and_open_file(false, gbl.SavePath, savename);

                byte[] data = new byte[Player.StructSize];

                gbl.file.BlockRead(Player.StructSize, data, file);
                gbl.file.Close(file);

                player = new Classes.Curse.Player(data, 0).Load();

                Player PreviousSelectedPlayer = gbl.SelectedPlayer;
                gbl.SelectedPlayer = player;

                TransferHillsFarCharacter(hf_player, player, PreviousSelectedPlayer);

                if (hf_player.field_1D > 0)
                {
                    Item newItem = new Item(Classes.Affects.none, Classes.Affects.helpless, (Classes.Affects)hf_player.field_1D,
                        (short)(hf_player.field_1D * 200), 0, 0,
                        false, 0, false, 0, 0, Item.Names.Chime, Item.Names.of, Item.Names.Vulnerability, Item.Type.GemsJewelry, true);

                    player.items.Add(newItem);
                }

                if (hf_player.field_23 > 0)
                {
                    Item newItem = new Item(Classes.Affects.none, Classes.Affects.poison_plus_4, (Classes.Affects)hf_player.field_23,
                        (short)(hf_player.field_23 * 0x15E), 0, 1,
                        false, 0, false, 0, 1, Item.Names.Wand, Item.Names.of, Item.Names.Magic_Missiles, Item.Type.WandB, true);

                    player.items.Add(newItem);
                }

                if (hf_player.field_86 > 0)
                {
                    Item newItem = new Item(Classes.Affects.none, Classes.Affects.helpless, (Classes.Affects)hf_player.field_86,
                        (short)(hf_player.field_86 * 0xc8), 0, 0,
                        false, 0, false, 0, 0, Item.Names.Ring, Item.Names.of, Item.Names.Vulnerability, Item.Type.Ring, true);

                    player.items.Add(newItem);
                }

                if (hf_player.field_87 > 0)
                {
                    Item newItem = new Item(Classes.Affects.none, Classes.Affects.highConRegen, (Classes.Affects)hf_player.field_87,
                        (short)(hf_player.field_87 * 0x190), 0, (short)(hf_player.field_87 * 10),
                        false, 0, false, 0, 0, Item.Names.Potion, Item.Names.of, Item.Names.Healing, Item.Type.GemsJewelry, true);

                    player.items.Add(newItem);
                }
            }
            else
            {
                fileExt = ".CHA";

                if (await PlayerFileExists(fileExt, hf_player.name) == true)
                {
                    byte[] data = new byte[Classes.PoolRad.Player.StructSize];

                    string savename = System.IO.Path.ChangeExtension(arg_8, fileExt);

                    file = await seg042.find_and_open_file(false, gbl.SavePath, savename);

                    gbl.file.BlockRead(Classes.PoolRad.Player.StructSize, data, file);
                    gbl.file.Close(file);

                    player = new Classes.PoolRad.Player(data).Load();

                    Player PreviousSelectedPlayer = gbl.SelectedPlayer;
                    gbl.SelectedPlayer = player;

                    TransferHillsFarCharacter(hf_player, player, PreviousSelectedPlayer);
                }
                else
                {
                    Player PreviousSelectedPlayer = gbl.SelectedPlayer;
                    gbl.SelectedPlayer = player;

                    for (int i = 0; i < 6; i++)
                    {
                        player.icon_colours[i] = (byte)(((gbl.default_icon_colours[i] + 8) << 4) + gbl.default_icon_colours[i]);
                    }

                    player.base_ac = 50;
                    player.thac0 = 40;
                    player.health_status = Status.okey;
                    player.in_combat = true;
                    player.head_portrait = 1;
                    player.body_portrait = 1;
                    player.icon_dimensions = 1;

                    player.mod_id = seg051.Random((byte)0xff);
                    player.icon_id = 0x0A;

                    player.attacksCount = 2;
                    player.attack1_DiceCountBase = 1;
                    player.attack1_DiceSizeBase = 2;
                    player.useStrBonus = 1;
                    player.base_movement = 12;

                    player.name = hf_player.name;
                    player.stats.Str.Load(hf_player.stat_str);
                    player.stats.Str00.Load(hf_player.stat_str00);
                    player.stats.Int.Load(hf_player.stat_int);
                    player.stats.Wis.Load(hf_player.stat_wis);
                    player.stats.Dex.Load(hf_player.stat_dex);
                    player.stats.Con.Load(hf_player.stat_con);
                    player.stats.Cha.Load(hf_player.stat_cha);

                    player.race = (Race)(hf_player.field_2D + 1);

                    if (player.race == Race.half_orc)
                    {
                        player.race = Race.human;
                    }

                    switch (player.race)
                    {
                        case Race.halfling:
                            player.icon_size = 1;
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.con_saving_bonus, player);
                            break;

                        case Race.dwarf:
                            player.icon_size = 1;
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.con_saving_bonus, player);
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.dwarf_vs_orc_goblin, player);
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.giant_vs_dwarf_gnome, player);
                            break;

                        case Race.gnome:
                            player.icon_size = 1;
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.con_saving_bonus, player);
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.gnome_vs_goblin_kobold, player);
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.giant_vs_dwarf_gnome, player);
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.gnoll_bugbear_vs_gnome, player);
                            break;

                        case Race.elf:
                            player.icon_size = 2;
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.elf_resist_sleep, player);
                            break;

                        case Race.half_elf:
                            player.icon_size = 2;
                            ovr024.add_affect(false, 0xff, 0, Classes.Affects.halfelf_resistance, player);
                            break;

                        default:
                            player.icon_size = 2;
                            break;
                    }

                    player._class = HillsFarClassMap[hf_player.field_35 & 0x0F];
                    player.age = hf_player.age;

                    player.cleric_lvl = (hf_player.field_B7 > 0) ? (byte)1 : (byte)0;
                    player.magic_user_lvl = (hf_player.field_B8 > 0) ? (byte)1 : (byte)0;
                    player.fighter_lvl = (hf_player.field_B9 > 0) ? (byte)1 : (byte)0;
                    player.thief_lvl = (hf_player.field_BA > 0) ? (byte)1 : (byte)0;
                    player.HitDice = 1;
                    player.sex = hf_player.field_2C;
                    player.alignment = hf_player.alignment;
                    player.exp = hf_player.field_2E;

                    if (player.magic_user_lvl > 0)
                    {
                        player.spellBook.LearnSpell(Spells.detect_magic_MU);
                        player.spellBook.LearnSpell(Spells.read_magic);
                        player.spellBook.LearnSpell(Spells.shield);
                        player.spellBook.LearnSpell(Spells.sleep);
                    }

                    SilentTrainPlayer();

                    ovr022.addPlayerGold(300);
                    gbl.SelectedPlayer = PreviousSelectedPlayer;
                    player.hit_point_max = hf_player.field_21;
                    player.hit_point_rolled = (byte)(player.hit_point_max - ovr018.get_con_hp_adj(player));
                    player.hit_point_current = hf_player.field_20;
                }
            }

            return player;
        }


        internal static Player load_mob(int monster_id)
        {
            return load_mob(monster_id, true);
        }

        internal static Player load_mob(int monster_id, bool exit)
        {
            byte[] player_data, item_data, affect_data;
            ushort player_len, item_len, affect_len;
            seg042.load_decode_dax(out player_data, out player_len, monster_id, string.Format("MON{0}CHA", gbl.game_area));

            if (player_len == 0)
            {
                if (exit)
                {
                    seg041.DisplayAndPause("Unable to load monster", 15);
                    seg043.print_and_exit();
                }
                else
                {
                    return null;
                }
            }

            seg042.load_decode_dax(out item_data, out item_len, monster_id, string.Format("MON{0}ITM", gbl.game_area));

            seg042.load_decode_dax(out affect_data, out affect_len, monster_id, string.Format("MON{0}SPC", gbl.game_area));

            var player = gbl.game.LoadPlayer(player_data, item_data, item_len, affect_data, affect_len);

            seg043.clear_keyboard();

            return player;
        }


        internal static void load_npc(int monster_id, byte morale) // sub_4A57D
        {
            if (gbl.area2_ptr.party_size <= 7)
            {
                Player player = load_mob(monster_id);

                player.mod_id = (byte)monster_id;

                player.control_morale = (byte)((morale >> 1) + Control.NPC_Base);

                AssignPlayerIconId(player);

                if (gbl.game.Name == Logging.Game.PoolOfRadiance)
                {

                    if (player.icon_size == 0)
                    {
                        player.icon_size = 2;

                        for (int i = 0; i < 6; i++)
                        {
                            byte colour = i == 3 ? gbl.default_icon_colours[i] : ovr024.roll_dice(7, 1);

                            player.icon_colours[i] = (byte)(((colour + 8) << 4) + colour);
                        }
                    }

                    player.combat_team = CombatTeam.Ours;
                }
                else // if (gbl.game == Game.CurseOfTheAzureBonds)
                {
                    ovr034.chead_cbody_comspr_icon(player.icon_id, monster_id, "CPIC");
                }
            }
        }

        internal static void AssignPlayerIconId(Player player) // sub_4A60A
        {
            player.icon_id = 0xff;

            gbl.TeamList.Add(player);
            gbl.SelectedPlayer = player;

            bool[] icon_slot = new bool[8];

            foreach (Player tmpPlayer in gbl.TeamList)
            {
                if (tmpPlayer.icon_id >= 0 && tmpPlayer.icon_id < 8)
                {
                    icon_slot[tmpPlayer.icon_id] = true;
                }
            }

            // Now find the lowest free icon slot.
            player.icon_id = 0;

            while (player.icon_id < 8 &&
                icon_slot[player.icon_id] == true)
            {
                player.icon_id += 1;
            }

            gbl.area2_ptr.party_size++;

            if (player.control_morale >= Control.NPC_Base)
            {
                ovr026.ReclacClassBonuses(player);
            }
        }

        static Set save_game_keys = new Set('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'); // asc_4A761


        internal static async void loadGameMenu() // loadGame
        {
            gbl.import_from = gbl.game.ImportFrom;

            string games_list = string.Empty;

            for (char save_letter = 'A'; save_letter <= 'J'; save_letter++)
            {
                if (await gbl.file.Find(gbl.SavePath, string.Format("SAVGAM{0}.DAT", save_letter.ToString())) == true)
                {
                    games_list += save_letter.ToString() + " ";
                }
            }

            if (games_list.Length != 0)
            {
                games_list = games_list.TrimEnd();

                bool stop_loop = false;
                char save_letter = '\0';
                do
                {
                    bool speical_key;
                    char input_key = ovr027.displayInput(out speical_key, false, 0, gbl.defaultMenuColors, games_list, "Load Which Game: ");

                    stop_loop = input_key == 0x00; // Escape
                    save_letter = '\0';

                    if (save_game_keys.MemberOf(input_key) == true)
                    {
                        save_letter = input_key;
                        string file_name = string.Format("SAVGAM{0}.DAT", save_letter.ToString());
                        stop_loop = await gbl.file.Find(gbl.SavePath, file_name);
                    }
                } while (stop_loop == false);

                if (save_letter != '\0')
                {
                    string file_name = string.Format("SAVGAM{0}.DAT", save_letter.ToString());

                    loadSaveGame(string.Format("SAVGAM{0}.DAT", save_letter.ToString()));
                }
            }
        }

        internal static async void loadSaveGame(string file_name)
        {
            System.IO.Stream file = await seg042.find_and_open_file(true, gbl.SavePath, file_name);

            ovr027.ClearPromptArea();
            seg041.displayString("Loading...Please Wait", 0, 10, 0x18, 0);
            gbl.reload_ecl_and_pictures = true;

            byte[] data = new byte[0x2000];

            gbl.file.BlockRead(1, data, file);
            gbl.game_area = data[0];

            gbl.file.BlockRead(0x800, data, file);
            gbl.area_ptr = new Area1(data, 0);

            gbl.file.BlockRead(0x800, data, file);
            gbl.area2_ptr = new Area2(data, 0);

            gbl.file.BlockRead(0x400, data, file);
            gbl.stru_1B2CA = new Struct_1B2CA(data, 0);

            if (gbl.game.StoreEclBlock)
            {
            gbl.file.BlockRead(0x1E00, data, file);
            gbl.ecl_ptr = new EclBlock(data, 0);
            }

            gbl.file.BlockRead(5, data, file);
            gbl.mapPosX = (sbyte)data[0];
            gbl.mapPosY = (sbyte)data[1];
            gbl.mapDirection = data[2];
            gbl.mapWallType = data[3];
            gbl.mapWallRoof = data[4];

            gbl.file.BlockRead(1, data, file);
            gbl.last_game_state = gbl.game.GameState(data[0]);

            gbl.file.BlockRead(1, data, file);
            gbl.game_state = gbl.game.GameState(data[0]);

            if (gbl.game.SetBlocksInArea1)
            {
                gbl.setBlocks[0].blockId = gbl.area_ptr.field_3F4;
                gbl.setBlocks[0].setId = gbl.area_ptr.field_3FA;
                gbl.setBlocks[1].blockId = gbl.area_ptr.field_3F6;
                gbl.setBlocks[1].setId = gbl.area_ptr.field_3FC;
                gbl.setBlocks[2].blockId = gbl.area_ptr.field_3F8;
                gbl.setBlocks[2].setId = gbl.area_ptr.field_3FE;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    gbl.file.BlockRead(2, data, file);
                    gbl.setBlocks[i].blockId = Sys.ArrayToShort(data, 0);

                    gbl.file.BlockRead(2, data, file);
                    gbl.setBlocks[i].setId = Sys.ArrayToShort(data, 0);
                }
            }

            gbl.file.BlockRead(1, data, file);
            int number_of_players = data[0];

            gbl.file.BlockRead(0x148, data, file);
            string[] var_148 = Sys.ArrayToStrings(data, 0, System.Math.Min(0x148, 0x29 * number_of_players), 0x29);

            gbl.file.Close(file);

            //gbl.PicsOn = ((gbl.area_ptr.pics_on >> 1) != 0);
            //gbl.AnimationsOn = ((gbl.area_ptr.pics_on & 1) != 0);
            gbl.game_speed_var = gbl.area_ptr.game_speed;
            gbl.area2_ptr.party_size = 0;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Player));

            for (int index = 0; index < number_of_players; index++)
            {
                string var_1F6 = Classes.File.CleanFilename(var_148[index]);

                if (await gbl.file.Find(gbl.SavePath, string.Format("{0}.XML", var_1F6)) == true)
                {
                    var stream = await gbl.file.Open(gbl.SavePath, string.Format("{0}.XML", var_1F6));
                    Player player = (Player)x.Deserialize(stream);
                    stream.Close();
                    player.stats.ReInit();
                    AssignPlayerIconId(player);
                }
                else if (await gbl.file.Find(gbl.SavePath, string.Format("{0}.SAV", var_1F6)) == true)
                {
                    Player player = await import_char01(gbl.SavePath, string.Format("{0}.SAV", var_1F6));
                    AssignPlayerIconId(player);
                }
            }

            foreach (Player tmp_player in gbl.TeamList)
            {
                remove_player_file(tmp_player);
            }

            foreach (Player tmp_player in gbl.TeamList)
            {
                gbl.SelectedPlayer = tmp_player;
                if (tmp_player.head_portrait == 0xFF && tmp_player.body_portrait == 0x00)
                {
                    ovr034.chead_cbody_comspr_icon(tmp_player.icon_id, tmp_player.mod_id, "CPIC");
                }
                else
                {
                    LoadPlayerCombatIcon(true);
                }
            }


            gbl.SelectedPlayer = gbl.TeamList[0];

            gbl.game_area = gbl.area2_ptr.game_area;

            if (gbl.area_ptr.inDungeon != 0)
            {
                if (gbl.game_state != GameState.StartGameMenu)
                {
                    if (gbl.setBlocks[0].blockId > 0)
                    {
                        gbl.byte_1AB0B = true;
                        ovr031.Load3DMap(gbl.area_ptr.current_3DMap_block_id);
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        if (gbl.setBlocks[i].blockId > 0)
                        {
                            ovr031.LoadWalldef(gbl.setBlocks[i].setId, gbl.setBlocks[i].blockId);
                        }
                    }
                }
            }
            else if (gbl.game.WildernessImage != 0xFF)
            {
                ovr030.load_bigpic(gbl.game.WildernessImage);
            }

            seg043.clear_keyboard();
            ovr027.ClearPromptArea();

            var game_state = gbl.game_state;
            gbl.game_state = gbl.game.LoadGameState;

            if (gbl.game_state != game_state)
            {
                gbl.last_game_state = game_state;
            }
        }

        static Set save_slots = new Set(0, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'); // unk_4AEA0
        static Set unk_4AEEF = new Set(0, 2, 18); 


        internal static async void SaveGame()
        {
            char inputKey;
            string[] var_171 = new string[9];
            System.IO.Stream save_file;

            do
            {
                inputKey = ovr027.displayInput((gbl.game_state == GameState.Camping), 0, gbl.defaultMenuColors, "A B C D E F G H I J", "Save Which Game: ");

            } while (save_slots.MemberOf(inputKey) == false);

            if (inputKey != '\0')
            {
                gbl.import_from = ImportSource.Curse;

                do
                {
                    save_file = await gbl.file.Create(gbl.SavePath, string.Format("SAVGAM{0}.DAT", inputKey));
                    gbl.file.Rewrite(save_file);

                    if (unk_4AEEF.MemberOf(gbl.FIND_result) == false)
                    {
                        seg041.DisplayAndPause("Unexpected error during save: " + gbl.FIND_result.ToString(), 14);
                        gbl.file.Close(save_file);
                        return;
                    }
                } while (unk_4AEEF.MemberOf(gbl.FIND_result) == false);

                ovr027.ClearPromptArea();
                seg041.displayString("Saving...Please Wait", 0, 10, 0x18, 0);

                gbl.area_ptr.game_speed = (byte)gbl.game_speed_var;
                gbl.area_ptr.pics_on = (byte)(((gbl.PicsOn) ? 0x02 : 0) | ((gbl.AnimationsOn) ? 0x01 : 0));
                gbl.area2_ptr.game_area = gbl.game_area;

                byte[] data = new byte[0x1E00];

                data[0] = gbl.game_area;
                gbl.file.BlockWrite(1, data, save_file);

                gbl.file.BlockWrite(0x800, gbl.area_ptr.ToByteArray(), save_file);
                gbl.file.BlockWrite(0x800, gbl.area2_ptr.ToByteArray(), save_file);
                gbl.file.BlockWrite(0x400, gbl.stru_1B2CA.ToByteArray(), save_file);
                if (gbl.game.StoreEclBlock)
                {
                gbl.file.BlockWrite(0x1E00, gbl.ecl_ptr.ToByteArray(), save_file);
                }

                data[0] = (byte)gbl.mapPosX;
                data[1] = (byte)gbl.mapPosY;
                data[2] = gbl.mapDirection;
                data[3] = gbl.mapWallType;
                data[4] = gbl.mapWallRoof;
                gbl.file.BlockWrite(5, data, save_file);

                data[0] = gbl.game.GameState(gbl.last_game_state);
                gbl.file.BlockWrite(1, data, save_file);
                data[0] = gbl.game.GameState(gbl.game_state);
                gbl.file.BlockWrite(1, data, save_file);

                if (!gbl.game.SetBlocksInArea1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Sys.ShortToArray((short)gbl.setBlocks[i].blockId, data, (i * 4) + 0);
                        Sys.ShortToArray((short)gbl.setBlocks[i].setId, data, (i * 4) + 2);
                    }
                    gbl.file.BlockWrite(12, data, save_file);
                }

                int party_count = 0;
                foreach (Player tmp_player in gbl.TeamList)
                {
                    party_count++;
                    var_171[party_count - 1] = string.Format("CHRDAT{0}{1}", Char.ToUpper(inputKey), party_count.ToString());
                }

                data[0] = (byte)party_count;
                gbl.file.BlockWrite(1, data, save_file);

                for (int i = 0; i < party_count; i++)
                {
                    Sys.StringToArray(data, 0x29 * i, var_171[i].Length, var_171[i]);
                }
                gbl.file.BlockWrite(0x148, data, save_file);
                gbl.file.Close(save_file);

                party_count = 0;
                foreach (Player tmp_player in gbl.TeamList)
                {
                    party_count++;
                    SavePlayer(string.Format("CHRDAT{0}{1}", Char.ToUpper(inputKey), party_count.ToString()), tmp_player);
                    remove_player_file(tmp_player);
                }

                gbl.gameSaved = true;
                ovr027.ClearPromptArea();
            }
        }
    }
}
