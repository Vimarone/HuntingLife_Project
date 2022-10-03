using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo
{
    int _att;
    int _def;
    int _hp;
    DefineDevHelper.ePrefabKind _model;

    public string _name
    {
        get; set;
    }

    public DefineDevHelper.eItemType _iType
    {
        get; set;
    }
    public DefineDevHelper.eEquipKind _eKind
    {
        get; set;
    }
    public int this[DefineDevHelper.eItemStat stat]
    {
        get
        {
            int re = 0;
            switch (stat)
            {
                case DefineDevHelper.eItemStat.Att:
                    re = _att;
                    break;
                case DefineDevHelper.eItemStat.Def:
                    re = _def;
                    break;
                case DefineDevHelper.eItemStat.HP:
                    re = _hp;
                    break;
            }
            return re;
        }
    }public DefineDevHelper.ePrefabKind _modelName
    {
        get { return _model; }
    }


    public ItemInfo(string n, DefineDevHelper.eItemType it, DefineDevHelper.eEquipKind ek, int a, int d, int hp, DefineDevHelper.ePrefabKind m)
    {
        _name = n;
        _iType = it;
        _eKind = ek;
        _att = a;
        _def = d;
        _hp = hp;
        _model = m;
    }
    
}
