using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Scx
{

    public class ApkReader
    {
        public delegate void OnCopyProgress (int curCount,int totalCount);

        //public delegate void OnCopyFinish (Error error);

        private static ApkReader s_apkReader = null;
        private ZipFile m_zipFile = null;
        private Dictionary<string, long> m_nameIndex = new Dictionary<string, long> ();
        private Dictionary<string, string> m_pathCache = new Dictionary<string, string>();
        private string m_assetsPath = "";

        public static ApkReader Instance
        {
            get
            {
                if (null == s_apkReader)
                {
                    s_apkReader = new ApkReader();
                }
                return s_apkReader;
            }
        }

        private ApkReader ()
        {
            DateTime t1 = DateTime.Now;
            string apkPath = Application.dataPath;
            m_assetsPath = apkPath + "/assets";
            m_zipFile = new ZipFile (apkPath);
            foreach (ZipEntry entry in m_zipFile) {
                if (entry.Name.IndexOf ("assets/") != -1) {
                    m_nameIndex.Add (entry.Name, entry.ZipFileIndex);
                }
            }
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2.Subtract(t1).Duration();
            Debug.LogFormat("apkreader {0}", ts.TotalSeconds);
        }

        public string AssetsPath
        {
            get { return m_assetsPath; }
        }

        public Stream GetInputStream (string pathOrName)
        {
            ZipEntry entry = this.GetEntry (pathOrName);
            if (null == entry) {
                return null;
            }
		
            return m_zipFile.GetInputStream (entry.ZipFileIndex);
        }

        public bool Contains(string pathOrName)
        {
            string pathWithAssets = PathTransform(pathOrName);
            bool hasContains = m_nameIndex.ContainsKey(pathWithAssets);
            return hasContains;
        }

        public ZipEntry GetEntry(string pathOrName)
        {
            string pathWithAssets = PathTransform(pathOrName);
            long nameIndex = -1;
            if (!m_nameIndex.TryGetValue(pathWithAssets, out nameIndex))
            {
                return null;
            }

            ZipEntry entry = m_zipFile[(int)nameIndex];
            return entry;
        }

        protected string PathTransform(string pathOrName)
        {
            //Debug.LogFormat("PathTransform 91 {0}", pathOrName);
            //jar:file:///data/app/com.daiei.gap-1.apk!/assets
            string pathWithAssets = "";
            if (m_pathCache.TryGetValue(pathOrName, out pathWithAssets))
            {
                return pathWithAssets;
            }
            string newPathOrName = pathOrName.Replace("\\", "/");
            int index = newPathOrName.IndexOf(Application.streamingAssetsPath);
            if (-1 == index)
            {
                pathWithAssets = "assets/" + newPathOrName;
            }
            else
            {
                int assestIndex = newPathOrName.IndexOf("/assets");
                pathWithAssets = newPathOrName.Substring(assestIndex + 1);
            }
            //Debug.LogFormat("ApkReader PathTransform {0} platwithassets {1}", pathOrName, pathWithAssets);
            m_pathCache[pathOrName] = pathWithAssets;
            return pathWithAssets;
        }

        public byte[] ReadAll (string pathOrName)
        {
            ZipEntry zipEntry =	this.GetEntry (pathOrName);
            if (null == zipEntry)
                return null;
            long size = zipEntry.Size;
            byte[] allCtn = new byte[size];
            Stream input = m_zipFile.GetInputStream (zipEntry.ZipFileIndex);
            input.Read (allCtn, 0, (int)size);
            input.Close ();
            return allCtn;
        }

        public bool CopyFile (string srcName, string dest)
        {
            ZipEntry zipEntry = this.GetEntry (srcName);
            if (null == zipEntry)
                return false;
            
            if (zipEntry.IsDirectory) {
                return false; 
            } 

            using (Stream srcStream = m_zipFile.GetInputStream (zipEntry.ZipFileIndex)) {
                string fullDestPath = dest;
                int destIndex = dest.IndexOf (Application.persistentDataPath);
                if (destIndex == -1) {
                    fullDestPath = Application.persistentDataPath + "/" + dest;
                }


                byte[] buffer = new byte[81920];
                Stream destStream = File.Open (fullDestPath, FileMode.Create);
                while (true) {
                    int readSize = srcStream.Read (buffer, 0, 81920);
                    if (readSize <= 0)
                        break;
                    destStream.Write (buffer, 0, readSize);
                }
                srcStream.Close ();
                destStream.Close ();
            }
            return true;
        }

        public bool CopyDirectory (string subPath, string dest)
        {
            if (string.IsNullOrEmpty (subPath))
                return false;

            char lastChar = subPath [subPath.Length - 1];
            if (lastChar != '/')
                subPath += "/";

            ZipEntry zipEntry = this.GetEntry (subPath);
            if (null == zipEntry)
                return false;

            if (!zipEntry.IsDirectory)
                return false;
           

            int destIndex = dest.IndexOf (Application.persistentDataPath);
            if (destIndex == -1) {
                dest = Application.persistentDataPath + "/" + dest;
            }

            int count = (int)m_zipFile.Count;
            int index = (int)zipEntry.ZipFileIndex;
            for (int i = (int)index; i < count; i++) {
                ZipEntry tmp = m_zipFile [i];
                if (null == tmp)
                    break;
                string name = tmp.Name;
                if (name.IndexOf (subPath) != 0)
                    break;

                string fullDestPath = Path.Combine (dest, name);
                if (Directory.Exists (fullDestPath)) {
                    Directory.CreateDirectory (fullDestPath);
                } else {
                    Stream srcStream = m_zipFile.GetInputStream (i);
                    byte[] buffer = new byte[8192];
                    Stream destStream = File.Open (fullDestPath, FileMode.Create);
                    while (true) {
                        int readSize = srcStream.Read (buffer, 0, 8192);
                        if (readSize <= 0)
                            break;
                        destStream.Write (buffer, 0, readSize);
                    }
                    srcStream.Close ();
                    destStream.Close ();
                }
            }
            return true;
        }

        /*class ApkCopyEvent : RunloopObserver
        {
            public static int Progress = 1;
            public static int Finish = 2;
           
            public int curCount;
            public int totalCount;
            public int eventType;
            public AysnCopyTask task = null;

            public override void Operate ()
            {
                if (eventType == ApkCopyEvent.Progress) {
                    if (null != task.copyProgress)
                        task.copyProgress (curCount, totalCount);
                } else {
                    if (null != task.copyFinish)
                        task.copyFinish (null);
                }
            }

        }

        class AysnCopyTask
        {
            public AysnCopyTask ()
            {
            }

            public void Start ()
            {
                ThreadPool.QueueUserWorkItem (new WaitCallback (Run));
            }

            private void Run (object state)
            {
                int copyCount = 0;
                var keys = this.files;
                foreach (string path in keys) {
                    int parentIndex = path.LastIndexOf ("/");
                    if (-1 != parentIndex) {
                        string parentPath = path.Substring (0, parentIndex);
                        string fullParentPath = Path.Combine (dataPath, parentPath);
                        if (!Directory.Exists (fullParentPath)) {
                            Directory.CreateDirectory (fullParentPath);
                        }
                    }

                    string fullFilePath = Path.Combine (dataPath, path);
                    ApkReader.Instance.CopyFile (path, fullFilePath);
                    copyCount++;
                    int progress = (int)Math.Ceiling ((copyCount * 1.0 / keys.Count) * 100);
                    if (progress != this.progress) {
                        this.progress = progress;
                        ApkCopyEvent copyEvent = new ApkCopyEvent ();
                        copyEvent.task = this;
                        copyEvent.eventType = ApkCopyEvent.Progress;
                        copyEvent.curCount = (int)copyCount;
                        copyEvent.totalCount = (int)keys.Count;
                        Runloop.Add (copyEvent);
                    }
                }

                if (copyFinish != null) {
                    ApkCopyEvent copyEvent = new ApkCopyEvent ();
                    copyEvent.task = this;
                    copyEvent.eventType = ApkCopyEvent.Finish;
                    Runloop.Add (copyEvent);
                }
            }

            public int progress;
            public string dataPath;
            public OnCopyFinish copyFinish;
            public OnCopyProgress copyProgress;
            public ICollection<string> files;
        }

        public void AsynCopyFiles (ICollection<string> files, string dataPath, OnCopyProgress copyProgress, OnCopyFinish copyFinish)
        {
            AysnCopyTask task = new AysnCopyTask ();
            task.files = files;
            task.copyFinish = copyFinish;
            task.copyProgress = copyProgress;
            task.dataPath = dataPath;
            task.Start ();
        }*/
    }
}