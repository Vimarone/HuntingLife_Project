using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefineDevHelper
{
    #region [Manager Define]

    //프리팹 관리용
    public enum ePrefabKind
    {
        Char_Dizon,
        Weapon_Rifle,
        BulletObject,
        RocketTrail
    }

    public enum eFxEffectKind
    {
        LandHit_Nor,
        LandHit_Fire,
        WallHit,
        ObstacleHit,
        ObstacleExplode,
        Bleeding
    }

    #endregion




    #region [Character Define]

    //몬스터 등급 : 등급에 따라 
    public enum eMonGrade
    {
        Normal,
        Advanced,
        Rare,
        Elite,
        Named,
        Boss
    }

    //몬스터 성향(Idle/Walk/Run 확률)
    public enum eMonPersonality
    {
        Lazy,                                       // 80, 18, 2
        Easygoing,                                  // 65, 25, 10
        Ordinary,                                   // 50, 35, 15
        Impatient,                                  // 30, 50, 20
        Distracted                                  // 5 , 55, 40
    }

    //
    public enum eRoamWay
    {
        FieldRandom,
        PointRandom,
        PointInOrder,   //만든 순서대로 갔다가 돌아왔다가
        PointBnF        
    }

    //애니메이션 컨트롤용 액션 타입
    public enum eActionType
    {
        IDLE,
        WALK,
        BACKWALK,
        RUN,
        ATTACK,
        VICTORY,
        SHOUT,


        DEATH                       = 99
    }

    //소지&장비 아이템
    public enum eItemPos
    {
        Weapon,
        Armor,
        Belongs
    }
    
    //레벨에 따른 추가 스탯 설정
    public struct stAdditionalStat
    {
        public int _targetLevel;
        public int _att;
        public int _def;
        public int _hp;
        public int _requireExp;

        public stAdditionalStat(int l, int a, int d, int hp, int exp)
        {
            _targetLevel = l;   //몇 레벨에 어느 정도의 스탯이 올라가는지
            _att = a;
            _def = d;
            _hp = hp;
            _requireExp = exp;
        }
    }

    #endregion




    #region [Item Define]

    //아이템 스탯 종류
    public enum eItemStat
    {
        Att,
        Def,
        HP
    }

    //아이템 종류
    public enum eItemType
    {
        Use,
        Equipment,
        Quest,
        ETC
    }

    //장비 종류
    public enum eEquipKind
    {
        Weapon,
        Armor,
        Accessory
    }

    #endregion




    #region [Map Define]

    // 맵상 콜라이더 종류
    public enum eHittingType
    {
        LAND,
        WALL,
        OBSTACLE_1,
        OBSTACLE_2,
        OBSTACLE_3,
        MONSTER
    }

    #endregion
}
