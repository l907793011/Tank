using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTextureManager : MonoBehaviour
{
    private static GameObject m_pMainObject;
    private static PPTextureManager m_pContainer = null;
    private Dictionary<string, Object[]> m_pAtlasDic; //图集的集合

    private static PPTextureManager instance;
    public static PPTextureManager Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public static PPTextureManager getIstance()
    {
        if (m_pContainer == null)
        {
            m_pContainer = m_pMainObject.GetComponent<PPTextureManager>();
        }
        return m_pContainer;
    }

    private void Awake()
    {
        instance = this;
        InitData();
    }

    private void InitData()
    {
        PPTextureManager.m_pMainObject = gameObject;
        m_pAtlasDic = new Dictionary<string, Object[]>();
    }

    //加载图集上的精灵
    public Sprite LoadAtlasSprite(string _strAtlasPath,string _strSpriteName)
    {
        Sprite _sprite = FindSpriteFormBuffer(_strAtlasPath, _strSpriteName);
        if (_sprite == null)
        {
            Object[] _atlas = Resources.LoadAll(_strAtlasPath);
            m_pAtlasDic.Add(_strAtlasPath, _atlas);
            _sprite = SpriteFormAtlas(_atlas, _strSpriteName);
        }

        return _sprite;
    }

    private Sprite FindSpriteFormBuffer(string _strAtlasPath, string _strSpriteName)
    {
        if (m_pAtlasDic.ContainsKey(_strAtlasPath))
        {
            Object[] _atlas = m_pAtlasDic[_strAtlasPath];
            Sprite _sprite = SpriteFormAtlas(_atlas, _strSpriteName);
            return _sprite;
        }
        return null;
    }

    //图集中获取精灵
    private Sprite SpriteFormAtlas(Object[] _atlas,string _strSpriteName)
    {
        for (int i =0;i<_atlas.Length;i++)
        {
            if (_atlas[i].GetType() == typeof(UnityEngine.Sprite))
            {
                if (_atlas[i].name == _strSpriteName)
                {
                    return (Sprite)_atlas[i];
                }

            }
        }
        return null;
    }

    //删除图集
    public void DeleteAtlas(string _strAtlasPath)
    {
        if (m_pAtlasDic.ContainsKey(_strAtlasPath))
        {
            m_pAtlasDic.Remove(_strAtlasPath);
        }
    }


}
