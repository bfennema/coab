using Avalonia.Controls.Platform.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static Eclh.EclhDecompilerProgram;

namespace ECLTools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Classes.gbl.file = new File();

            if (args.Length == 3)
            {
                var stats = new Eclh.EclhDecompilerProgram.CompoundAssignStats();
                Classes.gbl.DataPath = args[0];
                if (int.TryParse(args[2], out var block_id) == true)
                {
                    await ProcessFile(args[1], block_id, stats);
                }
                stats.Print(args[0]);
            }
            else
            {
                string[] games;
                if (args.Length == 1)
                {
                    games = [ args[0] ];
                }
                else
                {
                    games = [ "Poolrad", "Curse" ];
                }

                foreach (var game in games)
                {
                    var stats = new Eclh.EclhDecompilerProgram.CompoundAssignStats();
                    Classes.gbl.DataPath = game;
                    Classes.DaxFiles.DaxCache.ClearCache();
                    for (int j = 0; j <= 82; j++)
                    {
                        for (int i = 1; i <= 8; i++)
                        {
                            await ProcessFile($"ECL{i}", j, stats);
                        }
                    }
                    stats.Print(game);
                }
            }
        }

        static async Task<bool> ProcessFile(string filename, int block_id, Eclh.EclhDecompilerProgram.CompoundAssignStats stats)
        {
            var bytes = await Classes.DaxFiles.DaxCache.LoadDax(filename, block_id);
            if (bytes != null)
            {
                var input_hex = HexDump(bytes);
                System.IO.File.WriteAllText($"{Classes.gbl.DataPath}/input_{filename}_{block_id}.txt", input_hex);
                //string output_decompiler = Eclh.EclhDecompilerProgram.Run(bytes, 0x9900);
                //string output_decompiler = Eclh.EclhDecompilerProgram.Run(bytes, "pool_of_radiance");
                //string output_decompiler = Eclh.EclhDecompilerProgram.Run(bytes, "curse_of_azure_bonds");
                string output_decompiler;
                if (Classes.gbl.DataPath == "Poolrad")
                {
                    output_decompiler = Eclh.EclhDecompilerProgram.Run(bytes, "pool_of_radiance");
                    stats.Merge(Eclh.EclhDecompilerProgram.AnalyzeCompoundAssignPatterns(bytes, "pool_of_radiance"));
                }
                else
                {
                    output_decompiler = Eclh.EclhDecompilerProgram.Run(bytes, "curse_of_azure_bonds");
                    stats.Merge(Eclh.EclhDecompilerProgram.AnalyzeCompoundAssignPatterns(bytes, "curse_of_azure_bonds"));
                }
                System.IO.File.WriteAllText($"{Classes.gbl.DataPath}/output_decompiler_{Eclh.EclhDecompiler.Version}_{filename}_{block_id}.txt", output_decompiler);
                Eclh.Lexer lexer = new(output_decompiler);
                var list = lexer.Tokenize();
                Eclh.Parser parser = new(list);
                var unit = parser.ParseUnit();
                Eclh.EclhCompiler compiler = new(unit);
                //compiler.SetEngineFunctions(Eclh.EclhDecompiler.EngineFunctions);
                //compiler.SetHardwareRegisters(Eclh.EclhDecompiler.HardwareRegisters);
                byte[] output_compiler = compiler.Compile();
                output_compiler[0] = bytes[0];
                output_compiler[1] = bytes[1];
                System.IO.File.WriteAllBytes($"{Classes.gbl.DataPath}/output_compiler_{Eclh.EclhCompiler.Version}_{filename}_{block_id}.bin", output_compiler);

                var output_hex = HexDump(output_compiler);
                System.IO.File.WriteAllText($"{Classes.gbl.DataPath}/output_compiler_{Eclh.EclhCompiler.Version}_{filename}_{block_id}.txt", output_hex);
                if (bytes.Length == output_compiler.Length)
                {
                    if (ByteArrayEqual(bytes, output_compiler) == false)
                    {
                        Console.Write("Comparison Failed!\n");
                        for (var i = 0; i < output_compiler.Length; i++)
                        {
                            if (bytes[i] != output_compiler[i])
                            {
                                Console.Write("Failure at byte {i}");
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    Console.Write("Length failed!\n");
                    for (var i = 0; i < Math.Min(output_compiler.Length, bytes.Length); i++)
                    {
                        if (bytes[i] != output_compiler[i])
                        {
                            Console.Write("Failure at byte {i}");
                            return false;
                        }
                    }
                    return false;
                }
            }
            //Console.Write("Success\n");
            return true;
        }

        static bool ByteArrayEqual(ReadOnlySpan<byte> a1,  ReadOnlySpan<byte> a2)
        {
            return a1.SequenceEqual(a2);
        }

        static string HexDump(byte[] data)
        {
            string new_string = "        00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F\n\n";
            for (var i = 0; i < data.Length; i += 16)
            {
                new_string += $"{i:X4}:   ";
                for (var j = 0; j < 16 && i + j < data.Length; j++)
                {
                    new_string += $"{data[i + j]:X2} ";
                }
                new_string += "\n";
            }

            return new_string;
        }
    }
}
