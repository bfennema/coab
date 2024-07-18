using System.Collections.Generic;

namespace Classes.Curse
{
    public class Game : Classes.Game
    {
        public override Logging.Game Name { get => Logging.Game.CurseOfTheAzureBonds; }
        public override ImportSource ImportFrom { get => Classes.ImportSource.Curse; }
        public override int InitialExp { get => 25000; }
        public override MoneySet InitialMoney { get => new MoneySet(Money.Platinum, 300); }
        public override string DemoString
        {
            get
            {
                gbl.displayInputSecondsToWait = 30;
                gbl.displayInputTimeoutValue = 'D';
                return "Curse of the Azure Bonds v1.3 ";
            }
        }
        public override string SaveItemExt { get => "SWG"; }
        public override string SaveAffectExt { get => "FX"; }

        public override byte DemoGameArea { get => 1; }
        public override int DemoGameSpeed { get => 6; }
        public override byte DemoEclBlockId { get => 82; }
        public override byte InitialGameArea { get => 2; }
        public override ushort InitialVmMem0Offset { get => 0x4B00; }
        public override ushort InitialVmMem0Size { get => 0x0400; }
        public override ushort InitialVmMem1Offset { get => 0x7C00; }
        public override ushort InitialVmMem1Size { get => 0x0400; }
        public override ushort InitialVmMem2Offset { get => 0x7A00; }
        public override ushort InitialVmMem2Size { get => 0x0200; }
        public override ushort InitialEclOffset { get => 0x8000; }
        public override byte InitialEclBlockId { get => 1; }
        public override SpellBook InitialMUSpells { get => new SpellBook(new List<Spells>() { Spells.detect_magic_MU, Spells.read_magic, Spells.enlarge, Spells.sleep }); }
        public override byte CampingImage { get => 29; }
        public override byte TreasureImage { get => 1; }
        public override ushort CallRedraw { get => 0x2E10; }
        public override ushort CallDuelPlayer { get => 0x8000; }
        public override ushort CallDuelMonster { get => 0x8001; }
        public override ushort CallSound { get => 0xB200; }
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
        public override void DrawFrame_Portrait() { Draw.Frame_Outer(); }
        public override bool Portrait { get => false; }
        public override byte[] PortraitBody { get => []; }
        public override byte[] PortraitHead { get => []; }
    }
}
