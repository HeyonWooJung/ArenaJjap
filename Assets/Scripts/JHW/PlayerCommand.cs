using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Neutral, //기본
    Slow, //둔화
    Root, //속박
    Stun, //기절
    Airborne, //에어본
    Invincible//무적
}
public class Character
{
    float _HP; //체력
    float _curHP; //현재 체력
    float _HPRegen; //체젠
    float _atk; //공격력
    int _lethality; //물관
    int _armorPen; //방깎 (35%면 0.35f 이렇게 되게 할거)
    float _def; //방어력
    int _moveSpeed; //이속
    float _atkSpeed; //공속 (얘도 0.00f)
    float _baseAS; //기본공속 (얘도 0.00f)
    float _additionAS; //추가공속 (얘도 0.00f)
    float _critChance; //치확 (얘도 0.00f)
    float _critDamage; //치뎀 (얘는 1.30f 이렇게 할듯)
    int _range; //사거리
    int _abilityHaste; //스킬가속
    float _lifeSteal; //피흡 (얘도 0.00f)
    State _state; //상태(이상)

    float qCoolDown;
    float qCurCool;
    float wCoolDown;
    float wCurCool;
    float eCoolDown;
    float eCurCool;
    float rCoolDown;
    float rCurCool;

    bool canRush;
    bool canFlash;

    public float HP
    {
        get
        {
            return _HP;
        }
    }

    public float CurHP
    {
        get
        {
            return _curHP;
        }
    }

    public float ATK
    {
        get
        {
            return _atk;
        }
    }

    public float DEF
    {
        get
        {
            return _def;
        }
    }

    public float LifeSteal
    {
        get
        {
            return _lifeSteal;
        }
    }

    public Character(float HP, float hPRegen, float atk, float def, int moveSpeed, float baseAS, int range, float qCoolDown, float wCoolDown, float eCoolDown, float rCoolDown)
    {
        _HP = HP;
        _curHP = _HP;
        _HPRegen = hPRegen;
        _atk = atk;
        _lethality = 0;
        _armorPen = 0;
        _def = def;
        _moveSpeed = moveSpeed;
        _atkSpeed = baseAS;
        _baseAS = baseAS;
        _additionAS = 0;
        _critChance = 0;
        _critDamage = 1.7f;
        _range = range;
        _abilityHaste = 0;
        _lifeSteal = 0;
        _state = State.Neutral;

        this.qCoolDown = qCoolDown;
        this.wCoolDown = wCoolDown;
        this.eCoolDown = eCoolDown;
        this.rCoolDown = rCoolDown;

        eCurCool = 0;
        wCurCool = 0;
        qCurCool = 0;
        rCurCool = 0;
    }

    public void Move(Transform transform)
    {
        //움직이다
    }

    public virtual void AutoAttack(Character target)
    {
        //평타
        float damage = _atk;

        if (_critChance >= Random.Range(0f, 1f)) //치명타
        {
            damage *= _critDamage;
        }

        target.TakeDamage(damage, false, _lethality, _armorPen);
    }

    public virtual void SkillQ(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (qCurCool >= 0)
        {
            //대충 스킬쓰기
            qCurCool = 100 / (100 + _abilityHaste) * qCoolDown;
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillW(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (wCurCool >= 0)
        {
            //대충 스킬쓰기
            wCurCool = 100 / (100 + _abilityHaste) * wCoolDown;
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillE(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (eCurCool >= 0)
        {
            //대충 스킬쓰기
            eCurCool = 100 / (100 + _abilityHaste) * eCoolDown;
        }
        else
        {
            //못쓴다하기
        }
    }

    public virtual void SkillR(bool isTargeting, bool isChanneling, Character target, Vector3 location)
    {
        if (rCurCool >= 0)
        {
            //대충 스킬쓰기
            rCurCool = 100 / (100 + _abilityHaste) * rCoolDown;
        }
        else
        {
            //못쓴다하기
        }
    }

    public void Rush()
    {
        //도주
        
    }

    IEnumerator DoRush()
    {
        int tempSpeed = (int)(_moveSpeed * 0.4f);
        _moveSpeed += tempSpeed;
        yield return new WaitForSeconds(2f);
        _moveSpeed -= tempSpeed;
    }

    public void Flash(Vector3 pos)
    {
        //점멸
    }

    public void TakeDamage(float damage, bool isTrueDmg, int lethal, float armorPen)
    {
        if (isTrueDmg)
        {
            _curHP -= damage;
        }
        else
        {
            float tempDef = _def - lethal; //물관 적용
            tempDef -= tempDef * armorPen; //방관 적용
            _curHP -= damage * (1 + tempDef * 0.01f);

        }

        if (_curHP <= 0)
        {
            _curHP = 0;
            Die();
        }
    }

    public void Heal(float heal)
    {
        _curHP += heal;
        if (_curHP >= _HP)
        {
            _curHP = _HP;
        }
    }

    void Die()
    {
        //죽으셈
    }
}
