using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class ThreeD
    {
        public static async Task<bool> Load8x8D(int symbolSet, int block_id)
        {
            if (symbolSet >= 0 && symbolSet < 5)
            {
                var data = await Classes.DaxFiles.DaxCache.LoadDax("8X8D", gbl.game_area, block_id);

                if (data is not null)
                {
                    gbl.symbol_8x8_set[symbolSet] = new DaxBlock(data, 1, 13);

                    if (gbl.symbol_8x8_set[symbolSet] == null)
                    {
                        Logging.Logger.LogAndExit($"Unable to load {block_id} from 8x8D{gbl.game_area}");
                    }

                    Input.ClearKeyboard();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> LoadWalldef(short symbolSet, short block_id)
        {
            if (symbolSet >= 1 && symbolSet < 4)
            {
                var data = await DaxFiles.DaxCache.LoadDax("WALLDEF", gbl.game_area, block_id);

                if (data is not null && ((data.Length / 0x30C) + symbolSet) <= 4)
                {
                    int var_A = gbl.symbol_set_fix[symbolSet] - gbl.symbol_set_fix[1];

                    gbl.wallDef.LoadData(symbolSet, data);

                    int blockCount = data.Length / 0x30C;

                    for (int block = 0; block < blockCount; block++)
                    {
                        int idx = symbolSet + block;
                        if (idx >= 1 && idx <= 3)
                        {
                            gbl.setBlocks[idx - 1].Reset();

                            gbl.wallDef.BlockOffset(idx, var_A);

                            if (blockCount > 1)
                            {
                                if (block_id == 0)
                                {
                                    await Load8x8D(idx, (10 * 10) + block + 1);
                                }
                                else
                                {
                                    await Load8x8D(idx, (block_id * 10) + block + 1);
                                }
                            }
                            else
                            {
                                await Load8x8D(idx, block_id);
                            }
                        }
                    }

                    gbl.setBlocks[symbolSet - 1].blockId = block_id;
                    gbl.setBlocks[symbolSet - 1].setId = symbolSet;

                    return true;
                }
                else
                {
                    Logging.Logger.LogAndExit("Unable to load {0} from WALLDEF{1}.DAX", block_id, gbl.game_area);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
