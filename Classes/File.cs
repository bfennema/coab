using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Classes
{
    public interface File
    {
        public IAsyncEnumerable<(string,Stream)> OpenAll(string path, string filter = "*");
        public Task<Stream?> Open(string path, string filename);
        public Task<Stream?> Create(string path, string filename);
        public void Delete(string path, string filename);
        public Task<bool> Find(string path, string filename);

        public void Reset(Stream stream)
        {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        public void Rewrite(Stream stream)
        {
            stream.SetLength(0);
        }

        public void Close(Stream stream)
        {
            stream.Close();
        }

        public int BlockRead(int count, byte[] data, Stream stream)
        {
            return stream.Read(data, 0, count);
        }

        public void BlockWrite(int count, byte[] data, Stream stream)
        {
            stream.Write(data, 0, count);
        }
    }
}
