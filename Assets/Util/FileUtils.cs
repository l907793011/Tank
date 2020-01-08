using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Scx
{
    public class FileUtils 
    {
        //静态变量
        protected static FileUtils s_instance = null;

        //和搜索路径有关的
        private Dictionary<string, string> m_cachedEntries = new Dictionary<string, string>();
        private List<string> m_searchPathArray = new List<string>();

        public static FileUtils Instance
        {
            get{
                if (null == s_instance)
                {
                    s_instance = new FileUtils();
                }
                return s_instance;
            }
        }

        protected FileUtils()
        {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX 
            m_searchPathArray.Add(Application.dataPath);
            //m_searchPathArray.Add(Path.Combine(Application.dataPath, "Game"));
#else
            m_searchPathArray.Add(Application.streamingAssetsPath);
#endif
        }

        public string GetDataFromFile(string path)
        {
            string newPath = this.FullPathForFilename(path);
            if (string.IsNullOrEmpty(newPath))
                return null;
            byte[] ctn = null;
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX || UNITY_IOS || UNITY_STANDALONE
            ctn = File.ReadAllBytes(newPath);
#elif UNITY_ANDROID
            int indexOfAssets = newPath.IndexOf(Application.streamingAssetsPath) ;
            if (indexOfAssets == 0)
            {
                ctn = ApkReader.Instance.ReadAll(newPath);
            }
            else
            {
                ctn = File.ReadAllBytes(newPath);
            }  
#endif
            string strJson = System.Text.Encoding.UTF8.GetString(ctn, 3, ctn.Length - 3);
            return strJson;
        }

        public void AddSearchPath(string path, bool front = true)
        {
            if (front)
            {
                if(m_searchPathArray.Count != 0)
                { 
                    string frontString = m_searchPathArray[0];
                    if (frontString.Equals(path))
                        return;
                }
                m_searchPathArray.Insert(0, path);
            }
            else
            {
                if (m_searchPathArray.Count != 0)
                {
                    string backString = m_searchPathArray[m_searchPathArray.Count - 1];
                    if (backString.Equals(path))
                        return;
                }

                m_searchPathArray.Add(path);
            }
        }

        public string FullPathForFilename(string path)
        {
            if (path == null)
                return "";

            path = path.Replace("\\", "/");
            string outPath = "";
            if (m_cachedEntries.TryGetValue(path, out outPath))
                return outPath;
            //Path.IsPathRooted:是不是根目录
            //path.Contains:是否包含对应的字符串
            bool bIsRoot = Path.IsPathRooted(path);
            bool bHave = path.Contains(Application.streamingAssetsPath);
            bool isRoot = bIsRoot || bHave;
            if (isRoot)
            {
                bool isExists = IsFileInSearchPath(path);
                if (isExists)
                { 
                    m_cachedEntries[path] = path;
                    return path;
                }
                else
                {
                    return "";
                }
            }

            

            for (int i = 0; i < m_searchPathArray.Count; i++)
            {
                string searchPath = m_searchPathArray[i];
                string fullPath = Path.Combine(searchPath, path);
                //Debug.LogFormat("FullPathForFilename 101 {0}", fullPath);
                fullPath = fullPath.Replace("\\", "/");
                bool isExists = IsFileInSearchPath(fullPath);
                if (isExists)
                {
                    //Debug.LogFormat("{0}", fullPath);
                    m_cachedEntries[path] = fullPath;
                    return fullPath;
                }
            }

            return "";
        }

        public bool IsFileExists(string fileName)
        {
            string fullPath = FullPathForFilename(fileName);
            return !string.IsNullOrEmpty(fullPath);
        }

        public bool IsDirectoryExists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }
        
        public void PurgeCachedEntries()
        {
            m_cachedEntries.Clear();
        }

        public static void Copy(string srcPath, string destPath)
        {
            if (srcPath == null || destPath == null)
            {
                Debug.LogFormat("Source path or destination path can not null when copy");
                return;
            }

            if (srcPath.Equals(destPath))
            {
                Debug.LogFormat(String.Format("Source path and destination path can not same when copy : {0}", srcPath));
                return;
            }

            string fullDestPath = destPath;
            int destIndex = destPath.IndexOf(Application.persistentDataPath);
            if (destIndex == -1)
            {
                fullDestPath = Application.persistentDataPath + "/" + destPath;
            }

            Stream srcStream = null;
            int srcIndex = srcPath.IndexOf(Application.persistentDataPath);
            if (srcIndex == 0)
            {//这边表示在源文件在persistentDataPath中，也就是沙盒路径
                if (!File.Exists(srcPath))
                {
                    Debug.LogFormat(String.Format("Source file not exists persistentDataPath : {0}", srcPath));
                    return;
                }

                srcStream = File.Open(srcPath, FileMode.Open);
            }
            else
            {//这边表示源路径在streamingAssetsPath中，也就是安装路径
                #if UNITY_ANDROID
                srcStream = ApkReader.Instance.GetInputStream(srcPath);
                #else
                if (!File.Exists(srcPath))
                {
                    Debug.LogFormat(String.Format("Source file not exists at streamingAssetsPath : {0}", srcPath));
                    return;
                }
                srcStream = File.Open(srcPath, FileMode.Open);
                #endif
            }

            if (null == srcStream)
                return;
            byte[] buffer = new byte[8192];
            Stream destStream = File.Open(fullDestPath, FileMode.Create);
            while (true)
            {
                int readSize = srcStream.Read(buffer, 0, 8192);
                if (readSize <= 0)
                    break;
                destStream.Write(buffer, 0, readSize);
            }
            srcStream.Close();
            destStream.Close();
        }

        protected string SubAndroidAssetsPath(string fullPath)
        {
            return fullPath.Substring(Application.streamingAssetsPath.Length);
        }

        protected bool IsFileInSearchPath(string fullPath)
        {
            bool isExists = false;
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX || UNITY_IOS || UNITY_STANDALONE
            isExists = File.Exists(fullPath);
#elif UNITY_ANDROID
            if(fullPath.IndexOf(Application.persistentDataPath) == 0)
            {
                isExists = File.Exists(fullPath); 
            }
            else
            {
                isExists = ApkReader.Instance.Contains(fullPath);
            }
#endif
            return isExists;
        }

        public void CreateDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string GetWritablePath()
        {
            return Application.persistentDataPath;
        }

        public List<string> SearchPathArray
        {
            get { return m_searchPathArray; }
        }

        public void CleanDirectory(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    CleanDirectory(d1.FullName);////递归删除子文件夹
                    Directory.Delete(d);
                }
            }
        }

        public void DeleteDirectory(string dir)
        {
            Directory.Delete(dir, true);
        }

        public void Rename(string oldName, string newName)
        {
            File.Move(oldName, newName);
        }
    }
}

