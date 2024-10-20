namespace Classes
{
    public abstract class Game
    {
        public abstract void Load();
        public abstract GameState GameState(byte data);
        public abstract byte GameState(GameState data);
        public abstract GameState LoadGameState { get; }
        public abstract Logging.Game Name { get; }
        public abstract ImportSource ImportFrom { get; }
        public abstract int InitialExp { get; }
        public abstract MoneySet InitialMoney { get; }
        public abstract string DemoString { get; }
        public abstract string SavePlayerExt { get; }
        public abstract string SaveItemExt { get; }
        public abstract string SaveAffectExt { get; }
        public abstract byte DemoGameArea { get; }
        public abstract int DemoGameSpeed { get; }
        public abstract byte DemoEclBlockId { get; }
        public abstract byte InitialGameArea { get; }
        public abstract byte Tile8x8D201 { get; }
        public abstract void SetupBackgroundTiles();
        public abstract Tile[] BackgroundTiles { get; }
        public abstract ushort InitialVmMem0Offset { get; }
        public abstract ushort InitialVmMem0Size { get; }
        public abstract ushort InitialVmMem1Offset { get; }
        public abstract ushort InitialVmMem1Size { get; }
        public abstract ushort InitialVmMem2Offset { get; }
        public abstract ushort InitialVmMem2Size { get; }
        public abstract ushort InitialEclOffset { get; }
        public abstract byte InitialEclBlockId { get; }
        public abstract SpellBook InitialMUSpells { get; }
        public abstract byte CampingImage { get; }
        public abstract byte TreasureImage { get; }
        public abstract byte WildernessImage { get; }
        public abstract byte EndGameImage { get; }
        public abstract byte BigpicImage { get; }
        public abstract ushort CallRedraw { get; }
        public abstract ushort CallDuelPlayer { get; }
        public abstract ushort CallDuelMonster { get; }
        public abstract ushort CallSound { get; }
        public abstract ushort CallMove { get; }
        public abstract ushort CallWall { get; }
        public abstract ushort CallDemo { get; }
        public abstract void DrawFrame_Outer();
        public abstract void DrawCredits();
        public abstract void DrawFrame_Dungeon();
        public abstract void DrawFrame_Wilderness();
        public abstract void DrawFrame_Memorize();
        public abstract void DrawFrame_Combat();
        public abstract void DrawFrame_List();
        public abstract void DrawFrame_Portrait();
        public abstract void DrawFrame_Temple();
        public abstract int ShopBuy();
        public abstract bool Portrait { get; }
        public abstract bool GameWonGameOver { get; }
        public abstract bool SetBlocksInArea1 { get; }
        public abstract int EclClockArguments { get; }
        public abstract bool StoreEclBlock { get; }
        public abstract byte[] PortraitBody { get; }
        public abstract byte[] PortraitHead { get; }
        public abstract string[] TempleSpells { get; }
        public abstract Player LoadPlayer(System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream);
        public abstract Player LoadPlayer(byte[] player_data, byte[] item_data, ushort item_len, byte[] affect_data, ushort affect_len);
        public abstract void SavePlayer(Player player, System.IO.Stream player_stream, System.IO.Stream? item_stream, System.IO.Stream? affect_stream);
        public abstract void DrawProtection();
        public abstract (string, string) CheckProtection();
    }
}
