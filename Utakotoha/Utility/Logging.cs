using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Threading;

namespace Utakotoha
{
    public static class Logging
    {
        const string FileName = "__Logging__";
        static object lockgate = new Object();

        /// <summary>write log data.</summary>
        [Conditional("DEBUG")]
        public static void DebugWrite(params object[] objects)
        {
            lock (lockgate)
            {
                foreach (var item in objects)
                {
                    var format = DateTime.Now + "|" + item;
                    Debug.WriteLine(format);
                    using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                    using (var stream = file.OpenFile(FileName, FileMode.OpenOrCreate))
                    {
                        stream.Seek(0, SeekOrigin.End);
                        using (var sw = new StreamWriter(stream, Encoding.UTF8))
                        {
                            sw.WriteLine(format);
                        }
                    }
                }
            }
        }

        /// <summary>return log string, if file doesn't exist then return null.</summary>
        public static string ReadData()
        {
            lock (lockgate)
            {
                using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!file.FileExists(FileName)) return null;

                    using (var stream = file.OpenFile(FileName, FileMode.Open))
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>delete log data.</summary>
        public static void DeleteData()
        {
            lock (lockgate)
            {
                using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (file.FileExists(FileName))
                    {
                        file.DeleteFile(FileName);
                    }
                }
            }
        }
    }
}
