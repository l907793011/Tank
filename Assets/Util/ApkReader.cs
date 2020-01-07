using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
//using UnityEditor.PackageManager;

namespace Util
{
    public class ApkReader
    {
       // public delegate void OnCopyProgress(int curCount, int totalCount);

        //public delegate void OnCopyFinish(Error error);

        private ZipFile m_zipFile = null;
        private Dictionary<string, long> m_nameIndex = new Dictionary<string, long>();
        private Dictionary<string, string> m_pathCache = new Dictionary<string, string>();
        private string m_assetsPath = "";

        private static ApkReader s_apkReader = null;
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

        private ApkReader()
        {
            DateTime t1 = DateTime.Now;
            string apkPath = Application.dataPath;
            m_assetsPath = apkPath + "/assets";
            m_zipFile = new ZipFile(apkPath);
            foreach (ZipEntry entry in m_zipFile)
            {
                if (entry.Name.IndexOf("assets/") != -1)
                {
                    m_nameIndex.Add(entry.Name, entry.ZipFileIndex);
                }
            }
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2.Subtract(t1).Duration();
            Debug.LogFormat("apkreader {0}", ts.TotalSeconds);
        }

        public byte[] ReadAll(string pathOrName)
        {
            ZipEntry zipEntry = this.GetEntry(pathOrName);
            if (null == zipEntry)
                return null;
            long size = zipEntry.Size;
            byte[] allCtn = new byte[size];
            Stream input = m_zipFile.GetInputStream(zipEntry.ZipFileIndex);
            input.Read(allCtn, 0, (int)size);
            input.Close();
            return allCtn;
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

        public bool Contains(string pathOrName)
        {
            string pathWithAssets = PathTransform(pathOrName);
            bool hasContains = m_nameIndex.ContainsKey(pathWithAssets);
            return hasContains;
        }
    }
}
