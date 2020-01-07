using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

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
            StreamReader reader = new StreamReader(Application.persistentDataPath + path);
            string jsonData = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            return jsonData;
            //string json = "";
            //TextAsset text = Resources.Load<TextAsset>(path);
            //json = text.text;
            ////if (string.IsNullOrEmpty(json)) return null;
            //return json;

            //string strPath = "";
            //if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer ||
            //    Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            //{
            //    strPath = Application.dataPath + "/StreamingAssets/" + path;
            //}
            //else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    strPath = Application.dataPath + "/Raw/" + path;
            //}
            //else if (Application.platform == RuntimePlatform.Android)
            //{
            //    strPath = Application.persistentDataPath + path;
            //    strPath = "jar:file://" + Application.dataPath + "!/assets/";
            //    Debug.Log("GetDataFromFile  Android: " + strPath);
            //    PritFullName(Application.persistentDataPath);
            //}
            //else
            //{
            //    strPath = Application.dataPath + "/config/" + path;
            //}
            //Debug.Log("GetDataFromFile  strPath: " + strPath);
            //string strJson = "";
            //if (File.Exists(strPath))
            //{
            //    Debug.Log("GetDataFromFile : " + "1111111111111111");


            //    byte[] ctn = null;
                //#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX || UNITY_IOS || UNITY_STANDALONE
                //                ctn = File.ReadAllBytes(strPath);
                //#elif UNITY_ANDROID
                //            int indexOfAssets = newPath.IndexOf(Application.streamingAssetsPath) ;
                //            if (indexOfAssets == 0)
                //            {
                //                ctn = ApkReader.Instance.ReadAll(newPath);
                //            }
                //            else
                //            {
                //                ctn = File.ReadAllBytes(strPath);
                //            }  
                //#endif
                //                strJson = Encoding.ASCII.GetString(ctn);
                //                Debug.Log("GetDataFromFile strJson : " + strJson);
                //                2、
                //            string strJson = File.ReadAllText(strPath);
                //                Hashtable jd = JsonMapper.ToObject<Hashtable>(strJson);
                //                JsonData jd1 = jd["map"] as JsonData;
                //                for (int i = 0; i < jd1.Count; i++)
                //                {
                //                    Debug.Log(jd1[i]["id"]);
                //                    Debug.Log(jd1[i]["map"]);
                //                }

                //                ctn = File.ReadAllBytes(newPath);
                //                StreamReader sr = new StreamReader(strPath);
                //                strJson = sr.ReadToEnd();//获取json文件里面的字符串
                //            }
                //            Debug.Log("GetDataFromFile  strJson: " + strJson);

            //}
//                Debug.Log("path: "+ path);
//            string newPath = this.FullPathForFilename(path);
//            Debug.Log("newPath: " + newPath);
//            if (string.IsNullOrEmpty(newPath))
//                return null;
//            byte[] ctn = null;
//#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX || UNITY_IOS || UNITY_STANDALONE
//            Debug.Log("strJson: " + "3333333333333333333333333333");
//            ctn = File.ReadAllBytes(newPath);
//#elif UNITY_ANDROID
//            int indexOfAssets = newPath.IndexOf(Application.streamingAssetsPath) ;
//            if (indexOfAssets == 0)
//            {
//                Debug.Log("strJson: " + "1111111111111111111111111");
//                ctn = ApkReader.Instance.ReadAll(newPath);
//            }
//            else
//            {
//                Debug.Log("strJson: " + "222222222222222222222222222");
//                ctn = File.ReadAllBytes(newPath);
//            }  
//#endif
//            Debug.Log("strJson 11111:  ");
//            string strJson = strJson = Encoding.ASCII.GetString(ctn);
//            Debug.Log("strJson 22222: " + strJson);
            //return strJson;
        }

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
        }
    }
}
