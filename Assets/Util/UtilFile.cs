using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

using LitJson;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Util
{
    public class UtilFile
    {
        //搜索路径
        private List<string> m_searchPathArray = new List<string>();

        private static UtilFile instance;

        public static UtilFile Instance{
            get {
                if (instance == null)
                {
                    instance = new UtilFile();
                }
                return instance;
            }
        }

        protected void FileUtils()
        {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX 
            m_searchPathArray.Add(Application.dataPath);
#else
            m_searchPathArray.Add(Application.streamingAssetsPath);
#endif
        }


        public string  GetDataFromFile(string path)
        {
            /*string strText = "";
            string strPath = Application.streamingAssetsPath + path;
#if UNITY_EDITOR
        strText = File.ReadAllText(strPath);
#elif UNITY_ANDROID
        WWW www = new WWW(strPath);
        while (!www.isDone) { }
        string str = www.text;
        strText = Encoding.UTF8.GetString(www.bytes,3, www.bytes.Length -3);
#endif
            return strText;*/
            TextAsset go = (TextAsset)Resources.Load(path);
            string strText = "";
#if UNITY_EDITOR
            strText = go.text;
#elif UNITY_ANDROID
            strText = go.text;//Encoding.UTF8.GetString(go.bytes, 0, go.bytes.Length);
#endif
            return strText;
        }

        /*
        private void PritFullName(string strPath)
        {
            foreach (string d in Directory.GetFileSystemEntries(strPath))
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                Debug.Log(strPath + "  :  " + d1.Name);
                PritFullName(d1.FullName);
                //CleanDirectory(d1.FullName);////递归删除子文件夹
                //Directory.Delete(d);
            }
        }

        
        public string FullPathForFilename(string path)
        {
            if (path == null)
                return "";

            path = path.Replace("\\", "/");
            //string outPath = "";
            //if (m_cachedEntries.TryGetValue(path, out outPath))
            //    return outPath;

            bool isRoot = Path.IsPathRooted(path) || path.Contains(Application.streamingAssetsPath);
            if (isRoot)
            {
                bool isExists = IsFileInSearchPath(path);
                if (isExists)
                {
                    //m_cachedEntries[path] = path;
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
                bool isExists = IsFileInSearchPath(fullPath);
                if (isExists)
                {
                    //Debug.LogFormat("{0}", fullPath);
                    //m_cachedEntries[path] = fullPath;
                    return fullPath;
                }
            }

            return "";
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
        }*/
    }
}
