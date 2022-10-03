using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : StatBase
{
    [SerializeField] float _walkSpeed = 2;
    [SerializeField] float _runSpeed = 4;
    [SerializeField] float _rotAngle = 120;
    [SerializeField] Transform _posHand;
    
    // 참조형
    WeaponObj _weapon;
    Animator _aniController;
    CharacterController _charController;
    UIMiniStatusBox _statusBox;
    UIMiniAlertBox _alertBox;
    VirtualStickBox _stickInput;

    // 정보형
    DefineDevHelper.eActionType _nowActType;
    float _movSpeed;
    bool _isFire;
    int _storageBulletCapacity = 20;
    int _remainBulletCount = 0;
    int _reloadBulletCount;
    bool _isReloading = false;
    float _fullReloadTime = 4;
    float _reloadTime;
    float _timeCount = 0;


    // 파라미터 정보
    int _level;
    int _nowHP;
    int _remainExp;
    Dictionary<DefineDevHelper.eItemPos, ItemInfo> _equipment;
    List<ItemInfo> _bag;

    // 임시
    float mz = 0;
    float rx = 0;
    Vector3 _moveV;
    DefineDevHelper.stAdditionalStat _startUpStat;
    Dictionary<int, DefineDevHelper.stAdditionalStat> _addStatLvlUpContainer;
    // =========

    public override int _finalDamage
    {
        get
        {
            int add = 0;
            foreach(ItemInfo info in _equipment.Values)
            {
                if(info != null)
                    add += info[DefineDevHelper.eItemStat.Att];
            }
            return _att + add;
        }
    }

    public override int _finalDefence
    {
        get
        {
            int add = 0;
            foreach(ItemInfo info in _equipment.Values)
            {
                if(info != null)
                    add += info[DefineDevHelper.eItemStat.Def];
            }

            return _def + add;
        }
    }

    public float _rateHP
    {
        get { return (float)_nowHP / _maxHP; }
    }

    void Awake()
    {
        _aniController = GetComponent<Animator>();
        _charController = GetComponent<CharacterController>();
        _statusBox = GameObject.FindGameObjectWithTag("UIMiniStatusBox").GetComponent<UIMiniStatusBox>();
        _alertBox = GameObject.FindGameObjectWithTag("UIAlertBox").GetComponent<UIMiniAlertBox>();
        _stickInput = GameObject.FindGameObjectWithTag("UIStickInput").GetComponent<VirtualStickBox>();
        
    }

    void Start()
    {
        // 임시
        _addStatLvlUpContainer = new Dictionary<int, DefineDevHelper.stAdditionalStat>();
        ItemInfo info = new ItemInfo("긴 불막대기", DefineDevHelper.eItemType.Equipment, DefineDevHelper.eEquipKind.Weapon, 7, 0, 5, DefineDevHelper.ePrefabKind.Weapon_Rifle);
        DummyDatas();
        _stickInput.InitDataSet(this);
        InitSetData("홍길동", _startUpStat._att, _startUpStat._def, _startUpStat._hp, _startUpStat._targetLevel);
        WeaponMounting(info);
        // =======
    }

    void Update()
    {
        if (_isDead)
            return;
        if (_stickInput._isVirtual)
        {
            mz = _stickInput._vertcVal;
            rx = _stickInput._horizVal;
        }
        else
        {
            //_stickInput.transform.gameObject.SetActive(false);
            mz = Input.GetAxisRaw("Vertical");
            rx = Input.GetAxisRaw("Horizontal");
        }
        
        if (!_isFire)
        {
            _moveV = transform.forward * mz * _movSpeed;
            /*
            if (_charController.isGrounded)                                     // 땅에 있을 때만 점프 가능 : 버그가 많아 잘 사용하지 않는 기능
            {
                //_moveV = transform.forward * mz * _runSpeed;
                if (Input.GetButtonDown("Jump"))
                    _moveV.y += 5;
            }

            //Move() : 컨트롤 해야하는 값이 많으나 그만큼 제어할 수 있는 범위가 넓음
            _moveV.y += Physics.gravity.y * Time.deltaTime;
            _charController.Move(_moveV * Time.deltaTime);
            */

            //SimpleMove() : 컨트롤 해야하는 값이 적으나 그만큼 제어할 수 있는 범위가 좁음
            _charController.SimpleMove(_moveV);
            //점프 안할거면 심플이 나은듯

            transform.Rotate(Vector3.up * rx * _rotAngle * Time.deltaTime);

            if (mz > 0)
                ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
            else if (mz < 0)
                ChangeAniFromAction(DefineDevHelper.eActionType.BACKWALK);
            else
                ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (!_stickInput._isVirtual)
                FireStart();
        }
        if (Input.GetButtonDown("Reload"))
        {
            if (!_stickInput._isVirtual)
                ReloadStart();
        }
        //Reload
        if (_isReloading)
        {

            _timeCount += Time.deltaTime;

            if (_remainBulletCount == _storageBulletCapacity)
            {
                _isReloading = false;
                _timeCount = 0;
                _alertBox.ActivateAlert(false);
                //Debug.Log("리로딩 끝");
            }
            else if (_timeCount >= _reloadTime / _reloadBulletCount)
            {
                _remainBulletCount++;
                _statusBox.SetBullet(_remainBulletCount, (float)_remainBulletCount /_storageBulletCapacity);
                _timeCount = 0;
            }
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1"))
            {
                _alertBox.ActivateAlert(true);
                _alertBox.SetAlert("Reloading");
            }
        }
    }

    public void InitSetData(string n, int a, int d, int hp, int l)
    {
        _name = n;
        _att = a;
        _def = d;
        _nowHP = _hp = hp;
        _level = l;
        for (int m = 2; m <= _level; m++)       //레벨에 따른 스탯 증가량 합산
        {
            DefineDevHelper.stAdditionalStat add = _addStatLvlUpContainer[m];
            _att += add._att;
            _def += add._def;
            _nowHP = _hp += add._hp;
        }
        _remainExp = _addStatLvlUpContainer[_level + 1]._requireExp;

        //스테이터스 창 동기화
        _remainBulletCount = _storageBulletCapacity;
        _statusBox.InitStatusBox(_name, _rateHP);

        _bag = new List<ItemInfo>();
        _equipment = new Dictionary<DefineDevHelper.eItemPos, ItemInfo>();
        //null : 가지고 있지 않은 아이템을 호출했을 때 오류가 나지 않으면서도 가지고 있지 않다고 체크하기 위함
        _equipment.Add(DefineDevHelper.eItemPos.Weapon, null);
        _equipment.Add(DefineDevHelper.eItemPos.Armor, null);
    }

    public void WeaponMounting(ItemInfo info)            //모델(프리팹) 장착, 장비 스탯은 ItemInfo에서 관리
    {
        // info에서 가져와서 프리팹 만들기
        GameObject prefab = ResPoolManager._instance.GetPrefabsFrom(DefineDevHelper.ePrefabKind.Weapon_Rifle);
        GameObject go = Instantiate(prefab, _posHand);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        _weapon = go.GetComponent<WeaponObj>();
        
        //총알 생성
        //prefab = ResPoolManager._instance.GetPrefabsFrom(DefineDevHelper.ePrefabKind.BulletObject);
        //_weapon.InitDataSet(prefab, this);

        _weapon.InitDataSet(info, this);
        _equipment[DefineDevHelper.eItemPos.Weapon] = info;
        _nowHP = _hp += info[DefineDevHelper.eItemStat.HP];
    }

    

    public void ChangeAniFromAction(DefineDevHelper.eActionType type)
    {
        if (_isDead)
            return;
        if (type == DefineDevHelper.eActionType.IDLE)
        {
            if (_nowActType == DefineDevHelper.eActionType.ATTACK || type == _nowActType)
                return;
        }
        switch (type)
        {
            case DefineDevHelper.eActionType.IDLE:
                if(Random.Range(0, 2) == 0)                                         // 50% 확률로 Idle 변경
                    _aniController.SetBool("IsIdle1", true);
                else
                    _aniController.SetBool("IsIdle1", false);
                break;
            case DefineDevHelper.eActionType.WALK:
                _movSpeed = _walkSpeed;
                break;
            case DefineDevHelper.eActionType.BACKWALK:
                _movSpeed = _walkSpeed / 2;
                break;
            case DefineDevHelper.eActionType.RUN:
                _movSpeed = _runSpeed;
                break;
            case DefineDevHelper.eActionType.ATTACK:
                _isFire = true;
                _aniController.SetTrigger("Fire");
                break;
            case DefineDevHelper.eActionType.DEATH:
                _isDead = true;
                _aniController.SetTrigger("isDead");
                break;


        }
        _aniController.SetInteger("BaseAniState", (int)type);
        _nowActType = type;
    }

    public override void HittingMe(int finishDam)
    {
        finishDam -= _finalDefence;
        finishDam = finishDam < 1 ? 1 : finishDam;
        _nowHP -= finishDam;

        if (_nowHP <= 0 && !_isDead)
        {
            ChangeAniFromAction(DefineDevHelper.eActionType.DEATH);
            Debug.Log(transform.name + "이(가) 죽었습니다.");
        }
        else
            Debug.Log(transform.name + "의 체력이 " + _nowHP + "남았습니다.");
    }

    public void InstantiateBullet()
    {
        GameObject go = ResPoolManager._instance.GetPrefabsFrom(DefineDevHelper.ePrefabKind.BulletObject);
        GameObject bullet = Instantiate(go, _weapon._firePosition.position, _weapon._firePosition.rotation);
        BulletObj obj = bullet.GetComponent<BulletObj>();
        obj._damage = _finalDamage;
        go = ResPoolManager._instance.GetPrefabsFrom(DefineDevHelper.ePrefabKind.RocketTrail);
        Instantiate(go, _weapon._firePosition.position, _weapon._firePosition.rotation);
        _weapon._muzzle.Play();
        _statusBox.SetBullet(--_remainBulletCount, (float)_remainBulletCount/_storageBulletCapacity);
    }

    //public void FireStart()
    //{
    //    if (!_miniStatusBox._reloadding)
    //    {
    //        if (_remainBulletCount > 0)
    //            ChangeAniFromAction(DefineDevHelper.eActionType.ATTACK);
    //        else
    //        {
    //            _miniStatusBox.StartReload(_remainBulletCount, _storageBulletCapacity);
    //            _remainBulletCount = _storageBulletCapacity;
    //        }
    //    }
    //}

    public void FireStart()
    {
        if (_remainBulletCount == 0)
        {
            //Debug.Log("리로딩 시작");
            _reloadTime = _fullReloadTime;
            _reloadBulletCount = _storageBulletCapacity - _remainBulletCount;
            _isReloading = true;
        }
        else if (!_isReloading)
            ChangeAniFromAction(DefineDevHelper.eActionType.ATTACK);
    }

    public void ReloadStart()
    {
        _reloadBulletCount = _storageBulletCapacity - _remainBulletCount;
        _reloadTime = (float)_reloadBulletCount / _storageBulletCapacity * _fullReloadTime;
        _isReloading = true;
    }

    //public void ReloadStart()
    //{
    //    if(!_miniStatusBox._reloading && _remainBulletCount < _storageBulletCapacity)
    //    {
    //        _miniStatusBox.StartReload(_remainBulletCount, _storageBulletCapacity);
    //        _remainBulletCount = _storageBulletCapacity;
    //    }
    //}

    void EndIdleBehaviour()
    {
        _nowActType = DefineDevHelper.eActionType.WALK;
        ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
    }

    //총알 생성
    //void ShotFired()
    //{
    //    InstantiateBullet(_finalDamage);
    //}

    //발사 이후 이동
    void EndFired()
    {
        _isFire = false;
    }

    void LevelUpPractice()
    {
        DefineDevHelper.stAdditionalStat add = _addStatLvlUpContainer[_level + 1];
        _att += add._att;
        _def += add._def;
        _nowHP = _hp += add._hp;
        int leftExp = _remainExp - _addStatLvlUpContainer[_level]._requireExp;
        _remainExp = add._requireExp - leftExp;
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("MonDam_Nor1"))
        {
            //MonsterObj mon = other.transform.parent.GetComponent<MonsterObj>();
            DamageZoneObj zone = other.GetComponent<DamageZoneObj>();
            HittingMe(zone._finalDamage);
            _statusBox.SetHpRate(_rateHP);

        }
        else if (other.CompareTag("MonDam_Nor2"))
        {
            DamageZoneObj zone = other.GetComponent<DamageZoneObj>();
            HittingMe(zone._finalDamage);
            _statusBox.SetHpRate(_rateHP);
        }
        else if (other.CompareTag("MonDam_SP"))
        {
            DamageZoneObj zone = other.GetComponent<DamageZoneObj>();
            HittingMe(zone._finalDamage);
            _statusBox.SetHpRate(_rateHP);
        }
        
    }
    

    #region [OnGUI 함수]
    //void OnGUI()
    //{
    //    if(GUI.Button(new Rect(0, 0, 200, 65), "IDLE1"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
    //        _aniController.SetBool("IsIdle1", true);
    //    }
    //    if (GUI.Button(new Rect(0, 70, 200, 65), "IDLE2"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.IDLE);
    //        _aniController.SetBool("IsIdle1", false);
    //    }
    //    if (GUI.Button(new Rect(0, 140, 200, 65), "RUN"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.RUN);
    //    }
    //    if (GUI.Button(new Rect(0, 210, 200, 65), "BACKWALK"))
    //    {
    //        ChangeAniFromAction(DefineDevHelper.eActionType.BACKWALK);
    //    }
    //}
    #endregion

    void DummyDatas()
    {
        _startUpStat = new DefineDevHelper.stAdditionalStat(1, 10, 2, 125, 0);

        DefineDevHelper.stAdditionalStat add = new DefineDevHelper.stAdditionalStat(2, 2, 1, 5, 35);
        _addStatLvlUpContainer.Add(add._targetLevel, add);
        add = new DefineDevHelper.stAdditionalStat(3, 2, 0, 10, 73);
        _addStatLvlUpContainer.Add(add._targetLevel, add);
        add = new DefineDevHelper.stAdditionalStat(4, 2, 1, 10, 228);
        _addStatLvlUpContainer.Add(add._targetLevel, add);
        add = new DefineDevHelper.stAdditionalStat(5, 3, 0, 20, 931);
        _addStatLvlUpContainer.Add(add._targetLevel, add);
    }
}

