using System.Collections.Generic;

namespace Classes.PoolRad
{
    public class Game : Classes.Game
    {
        static Game()
        {
            gbl.import_func[(int)ImportSource.Pool] = Player.LoadPlayer;
        }
        readonly static internal byte[] portraitHead = { 0, 8, 9, 13, 16, 18, 22, 34, 45, 51, 53, 57, 67, 68 };
        readonly static internal byte[] portraitBody = { 1, 2, 3, 4, 7, 8, 18, 24, 26, 33, 35, 37 };
        readonly static string[] templeSpells = { "Cure Blindness", "Cure Disease", "Cure Light Wounds", "Cure Serious Wounds", "Cure Critical Wounds", "Neutralize Poison", "Raise Dead", "Remove Curse", "Stone to Flesh", "Exit" };
        public override void Load() { }
        public override Logging.Game Name { get => Logging.Game.PoolOfRadiance; }
        public override ImportSource ImportFrom { get => Classes.ImportSource.Pool; }
        public override int InitialExp { get => 0; }
        public override MoneySet InitialMoney
        {
            get
            {
                return new MoneySet();
            }
        }
        public override string DemoString { get => null; }
        public override string SavePlayerExt { get => "CHA"; }
        public override string SaveItemExt { get => "ITM"; }
        public override string SaveAffectExt { get => "SPC"; }
        public override byte DemoGameArea { get => 0; }
        public override int DemoGameSpeed { get => 0; }
        public override byte DemoEclBlockId { get => 0; }
        public override byte InitialGameArea { get => 3; }
        public override ushort InitialVmMem0Offset { get => 0x4900; }
        public override ushort InitialVmMem0Size { get => 0x0400; }
        public override ushort InitialVmMem1Offset { get => 0x6B00; }
        public override ushort InitialVmMem1Size { get => 0x0400; }
        public override ushort InitialVmMem2Offset { get => 0x9800; }
        public override ushort InitialVmMem2Size { get => 0x0100; }
        public override ushort InitialEclOffset { get => 0x9900; }
        public override byte InitialEclBlockId { get => 0; }
        public override SpellBook InitialMUSpells { get => new SpellBook(new List<Spells>() { Spells.detect_magic_MU, Spells.read_magic, Spells.shield, Spells.sleep }); }
        public override byte CampingImage { get => 29; }
        public override byte TreasureImage { get => 1; }
        public override ushort CallRedraw { get => 0x2C90; }
        public override ushort CallDuelPlayer { get => 0x8000; }
        public override ushort CallDuelMonster { get => 0x8001; }
        public override ushort CallSound { get => 0xBA03; }
        public override ushort CallMove { get => 0xC01E; }
        public override ushort CallWall { get => 0xC018; }
        public override ushort CallDemo { get => 0x6803; }
        public override void DrawFrame_Outer() { Draw.Frame_Outer(); }
        public override void DrawCredits() { Draw.Credits(); }
        public override void DrawFrame_Dungeon() { Draw.Frame_Dungeon(); }
        public override void DrawFrame_Wilderness() { Draw.Frame_Wilderness(); }
        public override void DrawFrame_Memorize() { Draw.Frame_Memorize(); }
        public override void DrawFrame_Combat() { Draw.Frame_Combat(); }
        public override void DrawFrame_List() { Draw.Frame_List(); }
        public override void DrawFrame_Portrait() { Draw.Frame_Portrait(); }
        public override void DrawFrame_Temple() { Draw.Frame_Temple(); }
        public override int ShopBuy() { gbl.displayString("Shop", 0, 15, 1, 17); return 4; }
        public override bool Portrait { get => true; }
        public override byte[] PortraitBody { get => portraitBody; }
        public override byte[] PortraitHead { get => portraitHead; }
        public override string[] TempleSpells { get => templeSpells; }
        public override Classes.Player LoadPlayer(System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream) { return Player.LoadPlayer(player_stream, item_stream, affect_stream); }
        public override Classes.Player LoadPlayer(byte[] player_data, byte[] item_data, ushort item_len, byte[] affect_data, ushort affect_len) { return Player.LoadPlayer(player_data, item_data, item_len, affect_data, affect_len); }
        public override void SavePlayer(Classes.Player player, System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream) { Player.SavePlayer(player, player_stream, item_stream, affect_stream); }
        public override void DrawProtection() { CopyProtection.DrawProtection(); }
        public override (string, string) CheckProtection() {  return CopyProtection.CheckProtection(); }
    }
}
