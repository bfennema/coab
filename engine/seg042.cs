using Classes;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace engine
{
    class seg042
    {
        static void debug_display(string text)
        {
            Logging.Logger.Log(text);
            seg043.GetInputKey();
        }

        internal static async System.Threading.Tasks.Task<System.IO.Stream> find_and_open_file(bool noError, string dir_path, string file_name)
        {
            System.IO.Stream file_ptr;

            if (dir_path.Length == 0)
            {
                dir_path = gbl.exe_path;
            }

            file_ptr = await gbl.file.Open(dir_path, file_name);

            if (file_ptr == null && noError == false)
            {
                debug_display("Couldn't find " + file_name + ". Check install.");
            }

            return file_ptr;
        }

        static bool setupDaxFiles(out System.IO.BinaryReader fileA, out System.IO.BinaryReader fileB, out short arg_8, string file_name)
        {
            fileA = null;
            fileB = null;
            arg_8 = 0;

            if (System.IO.File.Exists(file_name) == false)
            {
                /*TODO Add message about missing file here.*/
                return false;
            }

            try
            {
                System.IO.FileStream fsA = new System.IO.FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                System.IO.FileStream fsB = new System.IO.FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                fileA = new System.IO.BinaryReader(fsA);
                fileB = new System.IO.BinaryReader(fsB);
            }
            catch (System.ApplicationException)
            {
                /*TODO Add message about error here.*/
                return false;
            }

            arg_8 = fileA.ReadInt16();
            arg_8 += 2;

            fileB.BaseStream.Seek(arg_8, System.IO.SeekOrigin.Begin);
            return true;
        }


        internal static async Task<(byte[], ushort)> load_decode_dax(int block_id, string filename, byte filenum)
        {
            //seg044.PlaySound(Sound.sound_0);

            var out_data = await Classes.DaxFiles.DaxCache.LoadDax(filename, filenum, block_id);
            var decodeSize = out_data == null ? (ushort)0 : (ushort)out_data.Length;

            return (out_data, decodeSize);
        }

        internal static async Task<(byte[], ushort)> load_decode_dax(int block_id, string filename)
        {
            //seg044.PlaySound(Sound.sound_0);

            var out_data = await Classes.DaxFiles.DaxCache.LoadDax(filename, block_id);
            var decodeSize = out_data == null ? (ushort)0 : (ushort)out_data.Length;

            return (out_data, decodeSize);
        }


        internal static void set_game_area(byte arg_0)
        {
            gbl.game_area_backup = gbl.game_area;
            gbl.game_area = arg_0;
        }


        internal static void restore_game_area()
        {
            gbl.game_area = gbl.game_area_backup;
        }
    }
}
