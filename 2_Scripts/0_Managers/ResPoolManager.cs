using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResPoolManager : MonoBehaviour
{
    static ResPoolManager _uniqueInstance;

    // 이펜트 저장소
    [SerializeField] GameObject[] _fxObjects;

    // 프리팹 저장소
    Dictionary<DefineDevHelper.ePrefabKind, GameObject> _dicPrefabs;
    
    

    public static ResPoolManager _instance
    {
        get { return _uniqueInstance; }
    }

    void Awake()
    {
        _uniqueInstance = this;
        DontDestroyOnLoad(gameObject);

        _dicPrefabs = new Dictionary<DefineDevHelper.ePrefabKind, GameObject>();

        LoadPrefabs();
    }

    void LoadPrefabs()
    {
        string path = "ModelData/Characters/" + DefineDevHelper.ePrefabKind.Char_Dizon.ToString();
        GameObject go = Resources.Load(path) as GameObject;
        _dicPrefabs.Add(DefineDevHelper.ePrefabKind.Char_Dizon, go);
        path = "ModelData/Weapons/" + DefineDevHelper.ePrefabKind.Weapon_Rifle.ToString();
        go = Resources.Load(path) as GameObject;
        _dicPrefabs.Add(DefineDevHelper.ePrefabKind.Weapon_Rifle, go);
        path = "ModelData/Weapons/" + DefineDevHelper.ePrefabKind.BulletObject.ToString();
        go = Resources.Load(path) as GameObject;
        _dicPrefabs.Add(DefineDevHelper.ePrefabKind.BulletObject, go);
        path = "ModelData/Weapons/" + DefineDevHelper.ePrefabKind.RocketTrail.ToString();
        go = Resources.Load(path) as GameObject;
        _dicPrefabs.Add(DefineDevHelper.ePrefabKind.RocketTrail, go);
    }

    public GameObject GetPrefabsFrom(DefineDevHelper.ePrefabKind kind)
    {
        if (_dicPrefabs.ContainsKey(kind))
            return _dicPrefabs[kind];
        else
            return null;
    }

    public GameObject GetFXObjectsFrom(DefineDevHelper.eFxEffectKind kind)
    {
        return _fxObjects[(int)kind];
    }
}
