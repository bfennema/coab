using Classes;
using Classes.Combat;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace engine
{
    public class seg001
    {
        internal static System.Threading.Thread EngineThread;

        public delegate void VoidDelegate();
        static VoidDelegate EngineStoppedCallback;

        internal static void EngineStop()
        {
            EngineStoppedCallback();

            throw new OperationCanceledException(gbl.Token);
            //gbl.Exit = true;
            //Input.AddKey('Q');

            //EngineThread.Abort();
        }

        public static void __SystemInit(VoidDelegate stoppedCallback, string resourceName)
        {
            EngineThread = System.Threading.Thread.CurrentThread;
            EngineStoppedCallback = stoppedCallback;

            ConfigGame(resourceName);
        }

        internal static void ConfigGame(string resourceName)
        {
            gbl.exe_path = System.IO.Directory.GetCurrentDirectory();

            seg044.SoundInit(resourceName);
        }

        public static async Task<bool> PROGRAM(CancellationToken token)
        {
            gbl.Token = token;
            /* Memory Init - Start */
            gbl.CombatMap = new CombatantMap[gbl.MaxCombatantCount + 1]; /* God damm 1-n arrays */
            for (int i = 0; i <= gbl.MaxCombatantCount; i++)
            {
                gbl.CombatMap[i] = new CombatantMap();
            }
            /* Memory Init - End */

            ovr003.SetupCommandTable();

            while (Logging.Config.DataPath.Length == 0)
            {
                gbl.Token.ThrowIfCancellationRequested();
                seg041.GameDelay();
            }
            gbl.DataPath = Logging.Config.DataPath;
            gbl.SavePath = Logging.Config.SavePath;

            gbl.game = gbl.games[(int)Logging.Config.Game];

            await InitFirst();

            ItemLibrary.Read();

            seg044.PlaySound(Sound.sound_0);

            //Logging.Logger.Debug("Field_6 & 0x0F == 0");
            //foreach (var s in gbl.spellCastingTable )
            //{
            //    if (s != null && (s.field_6 & 0x0f) == 0)
            //    {
            //        Logging.Logger.Debug("{0} {1}", s.spellIdx, (Spells)s.spellIdx);
            //    }
            //}
            //Logging.Logger.Debug("");
            //Logging.Logger.Debug("Field_6 & 0x0F == 5");
            //foreach (var s in gbl.spellCastingTable)
            //{
            //    if (s != null && (s.field_6 & 0x0f) == 5)
            //    {
            //        Logging.Logger.Debug("{0} {1}", s.spellIdx, (Spells)s.spellIdx);
            //    }
            //}
            //Logging.Logger.Debug("");
            //Logging.Logger.Debug("Field_6 & 0x0F == 15");
            //foreach (var s in gbl.spellCastingTable)
            //{
            //    if (s != null && (s.field_6 & 0x0f) == 15)
            //    {
            //        Logging.Logger.Debug("{0} {1}", s.spellIdx, (Spells)s.spellIdx);
            //    }
            //}
            //Logging.Logger.Debug("");
            //Logging.Logger.Debug("Field_6 & 0x0F >= 8 <= 14");
            //foreach (var s in gbl.spellCastingTable)
            //{
            //    if (s != null)
            //    {
            //        int v = s.field_6 & 0x0f;
            //        if (v >= 8 && v <= 14)
            //        {
            //            Logging.Logger.Debug("{0} {1}", s.spellIdx, (Spells)s.spellIdx);

            //        }
            //    }
            //}
            //Logging.Logger.Debug("");
            //Logging.Logger.Debug("Field_6 & 0x0F otherwise");
            //foreach (var s in gbl.spellCastingTable)
            //{
            //    if (s != null)
            //    {
            //        int v = s.field_6 & 0x0f;
            //        if (v >= 1 && v <= 7 && v != 5)
            //        {
            //            Logging.Logger.Debug("{0} {1} {2}", s.spellIdx, (Spells)s.spellIdx, (v & 3)+1);
            //        }
            //    }
            //}
            //Logging.Logger.Debug("");

            //Classes.Debug.AddBreakpoint(0x9A18);
            //Classes.Debug.AddBreakpoint(0x9A36);
            //Classes.Debug.AddBreakpoint(0x9964);

            await gbl.game.Load();

            if (Cheats.skip_title_screen == false)
            {
                await ovr002.title_screen();
            }

            string demoString = gbl.game.DemoString;

            if (demoString != null)
            {
                char inputKey = ovr027.displayInput(false, 0, gbl.defaultMenuColors, "Play Demo", demoString);

                gbl.displayInputSecondsToWait = 0;
                gbl.displayInputTimeoutValue = '\0';

                if (inputKey == 'D')
                {
                    gbl.inDemo = true;

                }
            }

            if (Cheats.skip_copy_protection == false &&
                gbl.inDemo == false)
            {
                ovr004.copy_protection();
            }

            while (gbl.Token.IsCancellationRequested == false)
            {
                if (gbl.inDemo == true)
                {
                    gbl.game_area = gbl.game.DemoGameArea;
                    gbl.game_speed_var = gbl.game.DemoGameSpeed;
                }
                else
                {
                    gbl.game_area = gbl.game.InitialGameArea;
                    gbl.vm_mem0_offset = gbl.game.InitialVmMem0Offset;
                    gbl.vm_mem0_size = gbl.game.InitialVmMem0Size;
                    gbl.vm_mem1_offset = gbl.game.InitialVmMem1Offset;
                    gbl.vm_mem1_size = gbl.game.InitialVmMem1Size;
                    gbl.vm_mem2_offset = gbl.game.InitialVmMem2Offset;
                    gbl.vm_mem2_size = gbl.game.InitialVmMem2Size;
                    gbl.initial_ecl_offset = gbl.game.InitialEclOffset;
                }

                if (gbl.inDemo == false)
                {
                    await ovr018.startGameMenu();
                    gbl.Token.ThrowIfCancellationRequested();
                }

                await ovr003.sub_29758();

                InitAgain();

                if (gbl.inDemo == true)
                {
                    await ovr002.title_screen();
                    Input.ClearKeyboard();

                    demoString = gbl.game.DemoString;

                    if (demoString != null)
                    {
                        char inputKey = ovr027.displayInput(false, 0, gbl.defaultMenuColors, "Play Demo", demoString);

                        gbl.displayInputSecondsToWait = 0;
                        gbl.displayInputTimeoutValue = '\0';

                        if (inputKey == 'D')
                        {
                            gbl.inDemo = true;

                        }
                    }
                    else
                    {
                        gbl.inDemo = false;
                    }

                    if (Cheats.skip_copy_protection == false &&
                        gbl.inDemo == false)
                    {
                        ovr004.copy_protection();
                    }

                    seg044.PlaySound(Sound.sound_0);
                }
            }

            throw new OperationCanceledException(token);
        }

        static async Task<bool> InitFirst() /* sub_39054 POR: sub_26B5D */
        {
            seg051.Randomize();

            gbl._displayString = seg041.displayString;
            gbl._put8x8Symbol = ovr038.Put8x8Symbol;
            gbl._draw8x8_clear_area = seg037.draw8x8_clear_area;
            gbl._drawIsoTile = ovr034.DrawIsoTile;
            gbl._load24x24Set = ovr034.Load24x24Set;
            gbl._calcStatBonuses = ovr024.CalcStatBonuses;

            gbl.saveData = new SaveData();
            //gbl.saveData.area_ptr = new Area1();
            //gbl.saveData.area2_ptr = new Area2();
            //gbl.saveData.stru_1B2CA = new Struct_1B2CA();
            //gbl.saveData.ecl_ptr = new EclBlock();
            gbl.dax_8x8d1_201 = new byte[177, 8];
            gbl.geo_ptr.LoadData(new byte[0x402]);

            ovr016.BuildEffectNameMap();

            gbl.cmd_ops.Init(Vm.GetMemoryValue);

            gbl.cursor_bkup = new DaxBlock(0, 1, 1, 8);
            gbl.cursor = new DaxBlock(0, 1, 1, 8);

            seg051.FillChar(0xf, gbl.cursor.bpp, gbl.cursor.data);

            gbl.symbol_8x8_set = new DaxBlock[5];
            gbl.symbol_8x8_set[0] = null;
            gbl.symbol_8x8_set[1] = null;
            gbl.symbol_8x8_set[2] = null;
            gbl.symbol_8x8_set[3] = null;
            gbl.symbol_8x8_set[4] = null;

            //gbl.primary_dax24x24Set = null;
            //gbl.secondary_dax24x24Set = null;

            //gbl.primary_dax24x24Set = new DaxBlock(0, 0x80, 3, 24);

            //gbl.area_ptr.Clear();

            //gbl.area_ptr.inDungeon = 1;
            //gbl.area_ptr.LastEclBlockId = 0;

            //gbl.area2_ptr.Clear();

            //gbl.stru_1B2CA.Clear();
            //gbl.ecl_ptr.Clear();


            gbl.combat_icons = new CombatIcon[29];
            for (int i = 0; i < 29; i++)
            {
                gbl.combat_icons[i] = new CombatIcon();
            }

            gbl.byte_1AD44 = 2;
            gbl.current_head_id = 0xff;
            gbl.current_body_id = 0xff;
            gbl.headX_dax = null;
            gbl.bodyX_dax = null;

            gbl.byte_1D556 = new DaxArray();

            gbl.bigpic_dax = null;
            gbl.items_pointer = new System.Collections.Generic.List<Item>();

            //gbl.mapPosX = 0;
            //gbl.mapPosY = 0;
            //gbl.mapDirection = 0;
            //gbl.mapWallType = 0;
            //gbl.mapWallRoof = 0;

            //gbl.mapPosX = 7;
            //gbl.mapPosY = 0x0D;
            //gbl.mapDirection = 0;

            gbl.can_bash_door = true;
            gbl.can_pick_door = true;
            gbl.can_knock_door = true;

            gbl.byte_1AD44 = 3;

            //gbl.saveData.setBlocks[0] = new SetBlock(1, 0);
            //gbl.saveData.setBlocks[1] = new SetBlock();
            //gbl.saveData.setBlocks[2] = new SetBlock();

            //gbl.AnimationsOn = true;
            //gbl.PicsOn = true;
            gbl.DelayBetweenCharacters = true;
            gbl.reload_ecl_and_pictures = false;
            gbl.rest_incounter_count = 0;

            gbl.TeamList.Clear();
            gbl.SelectedPlayer = null;

            gbl.ecl_offset = 0x8000;
            gbl.vm_mem0_offset = 0x4B00;
            gbl.vm_mem0_size = 0x0400;
            gbl.vm_mem1_offset = 0x7C00;
            gbl.vm_mem1_size = 0x0400;
            gbl.vm_mem2_offset = 0x7A00;
            gbl.vm_mem2_size = 0x0200;
            gbl.initial_ecl_offset = 0x8000;
            gbl.game_speed_var = 4;
            gbl.inDemo = false;
            //gbl.game_area = 1;
            gbl.game_area_backup = 1;
            gbl.mapAreaDisplay = false;
            //gbl.area2_ptr.party_size = 0;
            gbl.menuScreenIndex = 1;
            gbl.combat_type = CombatType.normal;
            gbl.displayPlayerStatusLine18 = false;
            gbl.search_flag_bkup = 0;
            gbl.spriteChanged = false;
            gbl.party_killed = false;
            gbl.byte_1AB0B = false;
            gbl.byte_1BF12 = 1;
            gbl.displayPlayerSprite = false;
            gbl.lastDaxFile = string.Empty;
            gbl.byte_1D5AB = string.Empty;
            gbl.lastDaxBlockId = 0x0FF;
            gbl.byte_1D5B5 = 0x0FF;
            gbl.gameSaved = false;
            gbl.byte_1EE95 = false;
            gbl.focusCombatAreaOnPlayer = true;
            gbl.bigpic_block_id = 0x0FF;
            gbl.silent_training = false;
            gbl.menuSelectedWord = 0;
            //gbl.game_state = GameState.DungeonMap;
            //gbl.last_game_state = 0;
            gbl.applyItemAffect = false;
            gbl.sky_dax_250 = null;
            gbl.sky_dax_251 = null;
            gbl.sky_dax_252 = null;
            gbl.gameWon = false;
            gbl.worldIcon = 0;
            await seg041.Load8x8Tiles();
            ovr027.ClearPromptArea();
            seg041.displayString("Loading...Please Wait", 0, 10, 0x18, 0);

            await ThreeD.Load8x8D(1, 4, 202);
            await ThreeD.Load8x8D(1, 0, 203);

            gbl.primary_dax24x24Set = await seg040.LoadDax(0, 0, 1, "SQRPACI");
            gbl.secondary_dax24x24Set = await seg040.LoadDax(0, 0, 2, "SQRPACI");

            for (gbl.byte_1AD44 = 0; gbl.byte_1AD44 <= 0x0b; gbl.byte_1AD44++)
            {
                await ovr034.chead_cbody_comspr_icon((byte)(gbl.byte_1AD44 + 0x0D), gbl.byte_1AD44, "COMSPR");
            }

            await ovr034.chead_cbody_comspr_icon(0x19, 0x19, "COMSPR");

            gbl.ItemDataTable = new ItemDataTable("ITEMS");

            Affects.Spells.Setup();
            Affects.Effect.Setup();

            return true;
        }


        static void InitAgain() /* sub_396E5 */
        {
            gbl.area_ptr.Clear();
            gbl.area_ptr.inDungeon = 1;
            gbl.area_ptr.LastEclBlockId = 0;
            gbl.area2_ptr.Clear();
            gbl.stru_1B2CA.Clear();
            gbl.ecl_ptr.Clear();

            gbl.mapPosX = 0;
            gbl.mapPosY = 0;
            gbl.mapDirection = 0;
            gbl.mapWallType = 0;
            gbl.mapWallRoof = 0;

            gbl.mapPosX = 7;
            gbl.mapPosY = 0x0D;
            gbl.mapDirection = 2;

            gbl.can_bash_door = true;
            gbl.can_pick_door = true;
            gbl.can_knock_door = true;

            gbl.byte_1AD44 = 3;

            gbl.setBlocks[0].blockId = 0;
            gbl.setBlocks[0].setId = 1;
            gbl.setBlocks[1].Reset();
            gbl.setBlocks[2].Reset();

            gbl.DelayBetweenCharacters = true;
            gbl.reload_ecl_and_pictures = false;
            gbl.rest_incounter_count = 0;

            gbl.TeamList.Clear();
            gbl.SelectedPlayer = null;

            gbl.ecl_offset = 0x8000;
            gbl.vm_mem0_offset = 0x4B00;
            gbl.vm_mem0_size = 0x0400;
            gbl.vm_mem1_offset = 0x7C00;
            gbl.vm_mem1_size = 0x0400;
            gbl.vm_mem2_offset = 0x7A00;
            gbl.vm_mem2_size = 0x0200;
            gbl.initial_ecl_offset = 0x8000;
            gbl.game_speed_var = 4;
            gbl.game_area = 1;
            gbl.game_area_backup = 1;
            gbl.mapAreaDisplay = false;
            gbl.area2_ptr.party_size = 0;
            gbl.menuScreenIndex = 1;
            gbl.combat_type = CombatType.normal;
            gbl.displayPlayerStatusLine18 = false;
            gbl.search_flag_bkup = 0;
            gbl.spriteChanged = false;
            gbl.party_killed = false;
            gbl.byte_1AB0B = false;
            gbl.byte_1BF12 = 1;
            gbl.displayPlayerSprite = false;
            gbl.lastDaxFile = string.Empty;
            gbl.byte_1D5AB = string.Empty;
            gbl.lastDaxBlockId = 0x0FF;
            gbl.byte_1D5B5 = 0x0FF;
            gbl.gameSaved = false;
            gbl.byte_1EE95 = false;
            gbl.focusCombatAreaOnPlayer = true;
            gbl.bigpic_block_id = 0x0FF;
            gbl.silent_training = false;
            ovr027.ClearPromptArea();
            gbl.menuSelectedWord = 0;
            gbl.game_state = GameState.DungeonMap;
            gbl.last_game_state = 0;
            gbl.applyItemAffect = false;
            gbl.gameWon = false;
            gbl.worldIcon = 0;
        }
    }
}
