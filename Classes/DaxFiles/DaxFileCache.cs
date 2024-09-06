using Logging;
using System.Collections.Generic;

namespace Classes.DaxFiles
{
    class DaxFileCache
    {
        Dictionary<int, byte[]> entries;
        List<string> files;

        internal DaxFileCache(string filename, string filenum)
        {
            entries = new Dictionary<int, byte[]>();
            files = new List<string>();

            LoadFile(filename, filenum);
            files.Add(filenum);
        }
        internal DaxFileCache(string filename, byte filenum) : this(filename, filenum.ToString()) { }

        public bool Add(string filename, string filenum)
        {
            if (files.Contains(filenum))
            {
                return false;
            }
            else
            {
                LoadFile(filename, filenum);
                files.Add(filenum);
                return true;
            }
        }
        public bool Add(string filename, byte filenum) {
            return Add(filename, filenum.ToString());
        }

        private async void LoadFile(string filename, string filenum)
        {
            string name = string.Format("{0}{1}.DAX", filename, filenum);
            var path = Config.DataPath;
            System.IO.BinaryReader fileA;

            try
            {
                var fsA = await gbl.file.Open(path, name);

                fileA = new System.IO.BinaryReader(fsA);
            }
            catch (System.ApplicationException)
            {
                return;
            }

            int dataOffset = fileA.ReadInt16() + 2;

            List<DaxHeaderEntry> headers = new List<DaxHeaderEntry>();

            const int headerEntrySize = 9;

            for (int i = 0; i < ((dataOffset - 2) / headerEntrySize); i++)
            {
                DaxHeaderEntry dhe = new DaxHeaderEntry();
                dhe.id = fileA.ReadByte();
                dhe.offset = fileA.ReadInt32();
                dhe.rawSize = fileA.ReadUInt16();
                dhe.compSize = fileA.ReadUInt16();

                headers.Add(dhe);
            }

            foreach (DaxHeaderEntry dhe in headers)
            {
                byte[] comp = new byte[dhe.compSize];
                byte[] raw = new byte[dhe.rawSize];

                fileA.BaseStream.Seek(dataOffset + dhe.offset, System.IO.SeekOrigin.Begin);

                comp = fileA.ReadBytes(dhe.compSize);

                Decode(dhe.rawSize, dhe.compSize, raw, comp);

                entries.Add(dhe.id, raw);
            }

            fileA.Close();
        }

        void Decode(int decodeSize, int dataLength, byte[] output_ptr, byte[] input_ptr)
        {
            sbyte run_length;
            int output_index;
            int input_index;

            input_index = 0;
            output_index = 0;

            do
            {
                run_length = (sbyte)input_ptr[input_index];

                if (run_length >= 0)
                {
                    for (int i = 0; i <= run_length; i++)
                    {
                        output_ptr[output_index + i] = input_ptr[input_index + i + 1];
                    }

                    input_index += run_length + 2;
                    output_index += run_length + 1;
                }
                else
                {
                    run_length = (sbyte)(-run_length);

                    for (int i = 0; i < run_length; i++)
                    {
                        output_ptr[output_index + i] = input_ptr[input_index + 1];
                    }

                    input_index += 2;
                    output_index += run_length;
                }
            } while (input_index < dataLength);
        }

        internal byte[] GetData(int block_id)
        {
            byte[] orig;
            if (entries.TryGetValue(block_id, out orig) == false)
            {
                return null;
            }

            return (byte[])orig.Clone();
        }
    }

}
