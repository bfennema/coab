using System.Collections.Generic;
using System.Threading.Tasks;

namespace Classes.Champ
{
    public class Game : Classes.Game
    {
        static Game()
        {
            gbl.import_func[(int)ImportSource.Curse] = Player.LoadPlayer;
        }
        public override async Task<bool> Load()
        {
            byte[]? pic_data;
            pic_data = await DaxFiles.DaxCache.LoadDax("SKY", 250);
            if (pic_data != null && pic_data.Length > 0)
            {
                gbl.sky_dax_250 = new DaxBlock(pic_data, 1, 13);
            }
            pic_data = await DaxFiles.DaxCache.LoadDax("SKY", 251);
            if (pic_data != null && pic_data.Length > 0)
            {
                gbl.sky_dax_251 = new DaxBlock(pic_data, 1, 13);
            }
            pic_data = await DaxFiles.DaxCache.LoadDax("SKY", 252);
            if (pic_data != null && pic_data.Length > 0)
            {
                gbl.sky_dax_252 = new DaxBlock(pic_data, 1, 13);
            }

            return true;
        }
        public override GameState GameState(byte data)
        {
            return (GameState)data;
        }
        public override byte GameState(GameState data)
        {
            return (byte)data;
        }
        public override GameState LoadGameState { get => Classes.GameState.StartGameMenu; }
        public override Logging.Game Name { get => Logging.Game.ChampionsOfKrynn; }
        public override ImportSource ImportFrom { get => Classes.ImportSource.Champions; }
        public override string[]? ImportSources { get => null; }
        public override int InitialExp { get => 1251; }
        public override MoneySet? InitialMoney { get => new MoneySet(Money.Steel, 300); }
        public override string DemoString
        {
            get
            {
                gbl.displayInputSecondsToWait = 30;
                gbl.displayInputTimeoutValue = 'D';
                return "Champions of Krynn V1.2";
            }
        }
        public override string SavePlayerExt { get => "WHO"; }
        public override string SaveItemExt { get => "STF"; }
        public override string SaveAffectExt { get => "SFX"; }
        public override byte DemoGameArea { get => 2; }
        public override int DemoGameSpeed { get => 6; }
        public override byte DemoEclBlockId { get => 57; }
        public override byte InitialGameArea { get => 1; }
        public override byte Tile8x8D201 { get => 1; }
        public override void SetupBackgroundTiles() { Tiles.SetupBackground(); }
        public override Tile[] BackgroundTiles { get => Tiles.Background; }
        public override ushort InitialVmMem0Offset { get => 0x4B00; }
        public override ushort InitialVmMem0Size { get => 0x0400; }
        public override ushort InitialVmMem1Offset { get => 0x7C00; }
        public override ushort InitialVmMem1Size { get => 0x0400; }
        public override ushort InitialVmMem2Offset { get => 0x7A00; }
        public override ushort InitialVmMem2Size { get => 0x0200; }
        public override ushort InitialEclOffset { get => 0x8000; }
        public override byte InitialEclBlockId { get => 36; }
        public override byte InvalidEclBlockId { get => 0; }
        public override SpellBook InitialMUSpells { get => new SpellBook(new List<Spells>() { Spells.detect_magic_MU, Spells.read_magic, Spells.enlarge, Spells.sleep }); }
        public override byte CampingImage { get => 59; }
        public override byte TreasureImage { get => 60; }
        public override byte WildernessImage { get => 121; }
        public override byte EndGameImage { get => 120; }
        public override byte BigpicImage { get => 112; }
        public override ushort? CallRedraw { get => 0x2E10; }
        public override ushort? CallDuelPlayer { get => 0x8000; }
        public override ushort? CallDuelMonster { get => 0x8001; }
        public override ushort? CallSound { get => 0xB200; }
        public override ushort? CallMove { get => 0xC01E; }
        public override ushort? CallWall { get => 0xC018; }
        public override ushort? CallDemo { get => 0x6803; }
        public override void DrawFrame_Outer() { Draw.Frame_Outer(); }
        public override void DrawCredits() { Draw.Credits(); }
        public override void DrawFrame_Dungeon() { Draw.Frame_Dungeon(); }
        public override void DrawFrame_Wilderness() { Draw.Frame_Wilderness(); }
        public override void DrawFrame_Memorize() { Draw.Frame_Memorize(); }
        public override void DrawFrame_Combat() { Draw.Frame_Combat(); }
        public override void DrawFrame_List() { Draw.Frame_List(); }
        public override void DrawFrame_Portrait() { Draw.Frame_Outer(); }
        public override void DrawFrame_Temple() { Draw.Frame_Wilderness(); }
        public override int ShopBuy() { return 1; }
        public override bool Portrait { get => false; }
        public override byte CombatTransparentColor { get => 0; }
        public override bool GameWonGameOver { get => false; }
        public override bool SetBlocksInArea1 { get => false; }
        public override int EclClockArguments { get => 2; }
        public override bool StoreEclBlock { get => false; }
        public override bool HasLastGameState { get => true; }
        public override byte[] PortraitBody { get => []; }
        public override byte[] PortraitHead { get => []; }
        public override string[] TempleSpells { get => [ "Cure Blindness", "Cure Disease", "Cure Light Wounds", "Cure Serious Wounds", "Cure Critical Wounds", "Heal", "Neutralize Poison", "Raise Dead", "Remove Curse", "Stone to Flesh" ]; }
        public override Race[] AllowedRaces { get => [Race.silvanesti_elf, Race.qualinesti_elf, Race.half_elf, Race.mountain_dwarf, Race.hill_dwarf, Race.kender, Race.human]; }
        public override ClassId[] AllowedClasses
        {
            get
            {
                if (Cheats.allow_champions_paladin)
                {
                    return [ClassId.cleric, ClassId.fighter, ClassId.magic_user, ClassId.thief, ClassId.ranger, ClassId.knight, ClassId.paladin, ClassId.mc_c_f, ClassId.mc_c_r, ClassId.mc_c_f_m, ClassId.mc_c_mu, ClassId.mc_f_mu, ClassId.mc_f_t, ClassId.mc_f_mu_t, ClassId.mc_mu_t];
                }
                else
                {
                    return [ClassId.cleric, ClassId.fighter, ClassId.magic_user, ClassId.thief, ClassId.ranger, ClassId.knight, ClassId.mc_c_f, ClassId.mc_c_r, ClassId.mc_c_f_m, ClassId.mc_c_mu, ClassId.mc_f_mu, ClassId.mc_f_t, ClassId.mc_f_mu_t, ClassId.mc_mu_t];
                }
            }
        }
        public override Classes.Player LoadPlayer(System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream) { return Player.LoadPlayer(player_stream, item_stream, affect_stream); }
        public override Classes.Player LoadPlayer(byte[] player_data, byte[] item_data, ushort item_len, byte[] affect_data, ushort affect_len) { return Player.LoadPlayer(player_data, item_data, item_len, affect_data, affect_len); }
        public override void SavePlayer(Classes.Player player, System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream) { Player.SavePlayer(player, player_stream, item_stream, affect_stream); }
        public override void DrawProtection() { CopyProtection.DrawProtection(); }
        public override (string, string) CheckProtection() { return CopyProtection.CheckProtection(); }
    }
}
