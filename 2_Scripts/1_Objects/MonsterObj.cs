using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterObj : StatBase
{
    [SerializeField] DefineDevHelper.eMonPersonality _personal;
    [SerializeField] DefineDevHelper.eRoamWay _roamWay;
    // Idle : 최소 시간 & 최대 시간(5 ~ 15초)
    // Walk & Run : 목적지 도달 시 판단
    [SerializeField] SightRangeObj _sightCS;
    [SerializeField] BoxCollider[] _damageZone;
    [SerializeField] GameObject _miniUI;

    // 파라미터 정보
    DefineDevHelper.eMonGrade _grade;
    float _sightRange = 20;
    float _attackDistance = 3.5f;

    // 참조 변수
    Animator _aniController;
    NavMeshAgent _navAgent;
    PlayerObj _targetObj;
    UIMonMiniWnd _uiMini;

    // 정보 변수
    float _limitRangeLnR = 14;
    float _limitRangeFnB = 11;
    Vector3 _originPos;

    DefineDevHelper.eActionType _nowActType;
    DefineDevHelper.eMonPersonality _nowMonPersonal;

    float _walkSpeed = 2;
    float _runSpeed = 5;

    float _nowWaitTime = 0;
    float _randWaitTime = 0;
    float _waitTimeMin = 1;
    float _waitTimeMax = 6;

    //int _idleRate = 0;
    //int _walkRate = 0;
    //bool _isSelectAI = false;
    List<Vector3> _roammingPointList;
    int _listNum = 0;
    bool _isDownCount = false;
    // int _nextIdx = -1;
    // bool _isBack = false;

    int _paramAttackAni = 0;
    bool[] _isAttack3 = new bool[9];
    //int _isAttack3Count = 9;
    int _checkRate = 9;
    bool _isEndAni = true;

    int _addAtt;
    int _addDef;
    int _addHP;
    int _nowHP;

    public override int _finalDamage { get { return _att + _addAtt; } }
    public override int _finalDefence { get { return _def + _addDef; } }
    public override int _maxHP { get { return _hp + _addHP; } }
    public float _rateHP
    {
        get { return (float)_nowHP / _maxHP; }
    }


    void Awake()
    {
        _aniController = GetComponent<Animator>();
        _navAgent = GetComponent<NavMeshAgent>();
        _uiMini = _miniUI.GetComponent<UIMonMiniWnd>();
        _originPos = transform.position;
        

        // 임시
        //SettingAISelectRate();
        Transform tfList = GameObject.Find("RoammingList").transform;
        SettingRoammingList(tfList);
        InitDataSet("거인A", 4, 1, 15, DefineDevHelper.eMonGrade.Boss);
        //==========

        for (int i = 0; i < _isAttack3.Length; i++)
            _isAttack3[i] = false;
    }

    void Update()
    {
        switch (_nowActType)
        {
            case DefineDevHelper.eActionType.IDLE:
                _nowWaitTime += Time.deltaTime;
                if (_nowWaitTime >= _randWaitTime)
                {
                    _nowWaitTime = 0;
                    ChangeAniFromPersonality(_personal);
                }
                break;
            case DefineDevHelper.eActionType.WALK:
            case DefineDevHelper.eActionType.RUN:
                if (_targetObj != null)
                {
                    if (Vector3.Distance(_targetObj.transform.position, transform.position) <= _attackDistance)
                        ChangeAniFromAction(DefineDevHelper.eActionType.ATTACK);
                    else
                        _navAgent.destination = _targetObj.transform.position;
                }
                else
                    if (_navAgent.remainingDistance <= 0)
                        ChangeAniFromPersonality(_personal);
                break;
            case DefineDevHelper.eActionType.SHOUT:                 //이펙트
                if (Vector3.Distance(_targetObj.transform.position, transform.position) <= _attackDistance)
                    ChangeAniFromAction(DefineDevHelper.eActionType.ATTACK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
            case DefineDevHelper.eActionType.ATTACK:
                if (Vector3.Distance(_targetObj.transform.position, transform.position) > _attackDistance)
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                else
                    transform.LookAt(_targetObj.transform);         //거리만 체크하면 유저가 움직였을 때 방향이 맞지 않게될 가능성이 높기 때문에 방향 조절
                break;
        }
    }

    public void InitDataSet(string n, int a, int d, int hp, DefineDevHelper.eMonGrade g)
    {
        switch (g)      //SetAddStat()
        {
            case DefineDevHelper.eMonGrade.Normal:
                break;
            case DefineDevHelper.eMonGrade.Advanced:
                a = (int)(a * 1.3f);
                d = (int)(d * 1.5f);
                hp = (int)(hp * 1.3f);
                break;
            case DefineDevHelper.eMonGrade.Rare:
                a = (int)(a * 1.7f);
                d = (int)(d * 2.1f);
                hp = (int)(hp * 1.9f);
                break;
            case DefineDevHelper.eMonGrade.Elite:
                a = (int)(a * 2.5f);
                d = (int)(d * 3.2f);
                hp = (int)(hp * 2.8f);
                break;
            case DefineDevHelper.eMonGrade.Named:
                a = (int)(a * 3.1f);
                d *= 4;
                hp = (int)(hp * 3.7f);
                break;
            case DefineDevHelper.eMonGrade.Boss:
                a *= 5;
                d = (int)(d * 6.4f);
                hp *= 6;
                break;
        }

        _name = n;
        _att = a;
        _def = d;
        _nowHP = _hp = hp;
        _grade = g;

        for (int i = 0; i < _damageZone.Length; i++)
        {
            _damageZone[i].GetComponent<DamageZoneObj>().initDataSet(this);
            _damageZone[i].enabled = false;
        }

        _sightCS.InitSet(this);
        _uiMini.OpenWindow(_name, _rateHP);
    }

    public void ChangeAniFromAction(DefineDevHelper.eActionType type)
    {
        if (_isDead || !_isEndAni)
            return;

        switch (type)
        {
            case DefineDevHelper.eActionType.IDLE:
                _navAgent.isStopped = true;
                break;
            case DefineDevHelper.eActionType.WALK:
                _navAgent.isStopped = false;
                _navAgent.stoppingDistance = 0;
                _navAgent.speed = _walkSpeed;
                break;
            case DefineDevHelper.eActionType.RUN:
                _navAgent.isStopped = false;
                if (_targetObj != null)
                    _navAgent.stoppingDistance = _attackDistance;               // player 위치까지 이동 : 겹쳐서 공격 = stoppingDistance 조정
                else
                    _navAgent.stoppingDistance = 0;
                _navAgent.speed = _runSpeed;
                break;
            case DefineDevHelper.eActionType.SHOUT:
                _navAgent.isStopped = true;
                _isEndAni = false;
                break;
            case DefineDevHelper.eActionType.ATTACK:
                _navAgent.isStopped = true;
                _aniController.SetInteger("AttackParam", _paramAttackAni);
                _isEndAni = false;
                break;
            case DefineDevHelper.eActionType.DEATH:
                _navAgent.isStopped = true;
                _aniController.SetTrigger("isDead");
                _isDead = true;
                break;
        }

        _aniController.SetInteger("AniState", (int)type);
        _nowActType = type;
    }

    public void ChangeAniFromPersonality(DefineDevHelper.eMonPersonality type)
    {
        _nowMonPersonal = type;
        int i = Random.Range(0, 100);
        switch (type)
        {
            case DefineDevHelper.eMonPersonality.Lazy:
                if (i < 80)
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                else if (i < 98)
                    ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
            case DefineDevHelper.eMonPersonality.Easygoing:
                if (i < 65)
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                else if (i < 90)
                    ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
            case DefineDevHelper.eMonPersonality.Ordinary:
                if (i < 50)
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                else if (i < 85)
                    ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
            case DefineDevHelper.eMonPersonality.Impatient:
                if (i < 30)
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                else if (i < 80)
                    ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
            case DefineDevHelper.eMonPersonality.Distracted:
                if (i < 5)
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                else if (i < 60)
                    ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
                else
                    ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
                break;
        }
        switch (_nowActType)
        {
            case DefineDevHelper.eActionType.IDLE:
                _randWaitTime = Random.Range(_waitTimeMin, _waitTimeMax);
                Debug.Log(_nowActType + " 선택 : " + _randWaitTime + "초 대기");
                break;
            case DefineDevHelper.eActionType.WALK:
            case DefineDevHelper.eActionType.RUN:
                AddRoammingWayList(_roamWay);
                //Debug.Log(_nowActType + " 선택 : " + _navAgent.destination + "로 이동");
                //Debug.Log(_nowActType + " 선택 : " + randNum + "번째 로밍포인트로 이동");
                break;
        }
    }
    void AddRoammingWayList(DefineDevHelper.eRoamWay type)
    {
        switch(type)
        {
            case DefineDevHelper.eRoamWay.FieldRandom:
                _navAgent.destination = NextGoalPosition();
                return;
            case DefineDevHelper.eRoamWay.PointInOrder:
                if (_listNum == _roammingPointList.Count)
                {
                    ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
                    return;
                }
                _navAgent.destination = NextGoalPosition(_listNum++);
                return;
            case DefineDevHelper.eRoamWay.PointBnF:
                if (_listNum == _roammingPointList.Count)
                    _isDownCount = true;
                else if (_listNum == 0)
                    _isDownCount = false;
                if (_isDownCount)
                    _listNum--;
                if (!_isDownCount)
                    _listNum++;
                _navAgent.destination = NextGoalPosition(_listNum);
                Debug.Log(_nowActType + " 선택 : " + _listNum + "번째 로밍포인트로 이동");
                return;
            case DefineDevHelper.eRoamWay.PointRandom:
                int randNum = Random.Range(0, _roammingPointList.Count);
                _navAgent.destination = NextGoalPosition(randNum);
                Debug.Log(_nowActType + " 선택 : " + randNum + "번째 로밍포인트로 이동");
                return;
        }
    }

    public void SettingRoammingList(Transform list)
    {
        _roammingPointList = new List<Vector3>();
        for (int n = 0; n < list.childCount; n++)
            _roammingPointList.Add(list.GetChild(n).position);
    }
    public override void HittingMe(int finishDam)       //이펙트 추가
    {
        finishDam -= _finalDefence;
        finishDam = finishDam < 1 ? 1 : finishDam;
        _nowHP -= finishDam;
        if (_nowHP <= 0 && !_isDead)
        {
            _nowHP = 0;
            ChangeAniFromAction(DefineDevHelper.eActionType.DEATH);
            GetComponent<CapsuleCollider>().enabled = false;
            Debug.Log(transform.name + "이(가) 죽었습니다.");
        }
        else
            Debug.Log(transform.name + "의 체력이 " + _nowHP + "남았습니다.");
    }
    public override bool SightOn(StatBase target)
    {
        if (_targetObj == target)
            return true;
        _targetObj = (PlayerObj)target;
        ChangeAniFromAction(DefineDevHelper.eActionType.SHOUT);
        transform.LookAt(_targetObj.transform);
        return false;
    }



    /// <summary>
    /// 다음 이동 위치를 받는 함수.
    /// </summary>
    /// <param name="index">위치 순서 -1이면 랜덤 위치</param>
    /// <returns></returns>
    Vector3 NextGoalPosition(int index = -1)
    {
        Vector3 goal = _originPos;
        if (index == -1)
        {
            float px = Random.Range(-_limitRangeLnR, _limitRangeLnR);
            float pz = Random.Range(-_limitRangeFnB, _limitRangeFnB);
            goal += new Vector3(px, 0, pz);
        }
        else
            goal = _roammingPointList[index];

        return goal;
    }

    //int GetNextIndexFromWay(int nowIdx = 0){ ... }

    void SetAttackStateFromRate(float rate)
    {
        // 1, 2 비율
        // hp가 30% 이하면 50:50
        // hp가 50% 이하면 90:10
        // hp가 50% 초과면 65:35
        // hp가 10% 단위마다 3번 액션 발동
        int check = (int)(rate * 100);
        int percent = Random.Range(0, 100);
        if (_checkRate >= check / 10 && check % 10 == 0)
        {
            _checkRate = check / 10;
            _paramAttackAni = 2;
        }
        else if (check < 30)
        {
            if (percent < 50)
                _paramAttackAni = 0;
            else
                _paramAttackAni = 1;
        }else if(check < 50)
        {
            if (percent < 90)
                _paramAttackAni = 0;
            else
                _paramAttackAni = 1;
        }
        else
        {
            if (percent < 65)
                _paramAttackAni = 0;
            else
                _paramAttackAni = 1;
        }

        /*
        if (check / 10 <= _isAttack3Count)
        {
            if (!_isAttack3[9 - _isAttack3Count])
            {
                _isAttack3[9 - _isAttack3Count] = true;
                _paramAttackAni = 2;
                _isAttack3Count--;
            }
        }
        */

        
    }
    void EnableZone(int zoneNum)
    {
        _damageZone[zoneNum].enabled = true;
    }
    void DisableZone(int zoneNum)
    {
        _damageZone[zoneNum].enabled = false;
    }

    void SetEndAni()
    {
        _isEndAni = true;
        SetAttackStateFromRate(_rateHP);
    }
    // void SettingAISelectRate() { ... }
    // void SelectAIProcess() { ... }

    void SetAddStat(DefineDevHelper.eMonGrade mg)
    {
        float magniAtt = 0;
        float magniDef = 0;
        float magniHP = 0;
        switch (mg)
        {
            case DefineDevHelper.eMonGrade.Normal:
                break;
            case DefineDevHelper.eMonGrade.Advanced:
                magniAtt = 0.3f;
                magniDef = 0.5f;
                magniHP = 0.3f;
                break;
            case DefineDevHelper.eMonGrade.Rare:
                magniAtt = 0.7f;
                magniDef = 1.1f;
                magniHP = 0.9f;
                break;
            case DefineDevHelper.eMonGrade.Elite:
                magniAtt = 1.5f;
                magniDef = 2.2f;
                magniHP = 1.8f;
                break;
            case DefineDevHelper.eMonGrade.Named:
                magniAtt = 2.1f;
                magniDef = 3.0f;
                magniHP = 2.7f;
                break;
            case DefineDevHelper.eMonGrade.Boss:
                magniAtt = 4.0f;
                magniDef = 5.4f;
                magniHP = 5.0f;
                break;
        }

        _addAtt = (int)(_att * magniAtt);
        _addDef = (int)(_def * magniDef);
        _addHP = (int)(_hp * magniHP);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            _miniUI.SetActive(true);
            _hitTime = _miniUI.GetComponent<UIMonMiniWnd>()._visibleTimer;
            BulletObj bbj = other.GetComponent<BulletObj>();
            HittingMe(bbj._damage);

            GameObject prefab = ResPoolManager._instance.GetFXObjectsFrom(DefineDevHelper.eFxEffectKind.Bleeding);
            Quaternion dir = Quaternion.LookRotation(transform.TransformDirection(Vector3.forward));

            if (prefab != null)
            {
                Destroy(Instantiate(prefab, bbj.transform.position, dir), 5);
            }
            else
                Debug.Log("이펙트가 없습니다.");


            if (_targetObj == null)
            {
                SightOn(bbj._myOwner);
            }

            //_uiMini.SetHpRate(_rateHP, transform);
            Destroy(bbj.gameObject);
        }
            
    }

    #region [GUI 함수]
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 200, 65), "IDLE"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
    //    }
    //    if (GUI.Button(new Rect(0, 70, 200, 65), "WALK"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.WALK);
    //    }
    //    if (GUI.Button(new Rect(0, 140, 200, 65), "RUN"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
    //    }
    //    if (GUI.Button(new Rect(0, 210, 200, 65), "SHOUT"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.SHOUT);
    //    }
    //}
    #endregion
}
