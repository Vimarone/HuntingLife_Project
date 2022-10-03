using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatBase : MonoBehaviour
{
    protected bool _isDead;
    protected string _name;
    protected int _att;
    protected int _def;
    protected int _hp;
    protected float _hitTime;

    public abstract int _finalDamage { get; }
    public abstract int _finalDefence { get; }
    public abstract void HittingMe(int finishDam);
    public virtual int _maxHP { get { return _hp; } }
    public virtual float _hitTimer { get { return _hitTime; } set { _hitTime = value; } }
    public virtual bool SightOn(StatBase target) { return false; }
}
