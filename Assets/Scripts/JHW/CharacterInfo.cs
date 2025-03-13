using UnityEngine;

public enum State
{
    Neutral, //기본
    Slow, //둔화
    Root, //속박
    Stun, //기절
    Airborne, //에어본
    Unstoppable, //저지불가
    Invincible//무적
}

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/CharacterInfo", order = int.MaxValue)]
public class Character : ScriptableObject
{
    #region 변수
    [SerializeField]
    float _HP; //체력
    float _curHP; //현재 체력
    [SerializeField]
    float _HPRegen; //체젠
    [SerializeField]
    float _atk; //공격력
    int _lethality; //물관
    int _armorPen; //방깎 (35%면 0.35f 이렇게 되게 할거)
    [SerializeField]
    float _def; //방어력
    [SerializeField]
    int _moveSpeed; //이속
    [SerializeField]
    float _atkSpeed; //공속 (얘도 0.00f)
    float _baseAS; //기본공속 (얘도 0.00f)
    float _additionAS; //추가공속 (얘도 0.00f)
    float _critChance; //치확 (얘도 0.00f)
    float _critDamage; //치뎀 (얘는 1.30f 이렇게 할듯)
    [SerializeField]
    int _range; //사거리
    int _abilityHaste; //스킬가속
    float _lifeSteal; //피흡 (얘도 0.00f)
    State _state; //상태(이상)
    float _damageResist; //받피감 (얘도 0.00f);

    [SerializeField]
    float qCoolDown;
    float qCurCool;
    [SerializeField]
    float wCoolDown;
    float wCurCool;
    [SerializeField]
    float eCoolDown;
    float eCurCool;
    [SerializeField]
    float rCoolDown;
    float rCurCool;

    bool canRush;
    bool canFlash;

    #endregion

    #region 프로퍼티
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
    public float HpRegen
    {
        get
        {
            return _HPRegen;
        }
    }

    public float ATK
    {
        get
        {
            return _atk;
        }
    }

    public int Lethality
    {
        get
        {
            return _lethality;
        }
    }

    public int ArmorPenetration
    {
        get
        {
            return _armorPen;
        }
    }

    public float DEF
    {
        get
        {
            return _def;
        }
    }

    public int MoveSpeed
    {
        get
        {
            return _moveSpeed;
        }
    }

    public float AttackSpeed
    {
        get
        {
            return _atkSpeed;
        }
    }

    public float CritChance
    {
        get
        {
            return _critChance;
        }
    }

    public float CritDamage
    {
        get
        {
            return _critDamage;
        }
    }

    public int Range
    {
        get
        {
            return _range;
        }
    }

    public int AbilityHaste
    {
        get
        {
            return _abilityHaste;
        }
    }

    public float LifeSteal
    {
        get
        {
            return _lifeSteal;
        }
    }

    public float CurQCool
    {
        get
        {
            return qCurCool;
        }
    }

    public float CurWCool
    {
        get
        {
            return wCurCool;
        }
    }
    public float CurECool
    {
        get
        {
            return eCurCool;
        }
    }
    public float CurRCool
    {
        get
        {
            return rCurCool;
        }
    }
    public State CurState
    {
        get
        {
            return _state;
        }
    }

    public float DamageResist
    {
        get
        {
            return _damageResist;
        }
    }

    public bool CanRush
    {
        get
        {
            return canRush;
        }
    }

    public bool CanFlash
    {
        get
        {
            return canFlash;
        }
    }
    #endregion

    public void InitCharacter()
    {
        _curHP = _HP;
        _lethality = 0;
        _armorPen = 0;
        _baseAS = _atkSpeed;
        _additionAS = 0;
        _critChance = 0;
        _critDamage = 1.7f;
        _abilityHaste = 0;
        _lifeSteal = 0;
        _state = State.Neutral;

        qCurCool = 0;
        wCurCool = 0;
        eCurCool = 0;
        rCurCool = 0;

        canFlash = true;
        canRush = true;
    }

    /*public Character(float HP, float hPRegen, float atk, float def, int moveSpeed, float baseAS, int range, float qCoolDown, float wCoolDown, float eCoolDown, float rCoolDown)
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

        qCurCool = 0;
        wCurCool = 0;
        eCurCool = 0;
        rCurCool = 0;
    }*/

    //라운드 시작할때
    public void ResetState()
    {
        _curHP = _HP;

        qCurCool = 0;
        wCurCool = 0;
        eCurCool = 0;
        rCurCool = 0;

        canRush = true;
        canFlash = true;
    }

    public void TakeDamage(float damage, bool isTrueDmg, int lethal, float armorPen)
    {
        if (isTrueDmg)
        {
            _curHP -= damage;
        }
        else
        {
            damage -= damage * _damageResist;
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

    void Die()
    {
        //죽으셈
    }

    public void Heal(float heal)
    {
        _curHP += heal;
        if (_curHP >= _HP)
        {
            _curHP = _HP;
        }
    }

    public void AdjustHP(float hp)
    {
        _HP += hp;
    }

    public void AdjustATK(float atk)
    {
        _atk += atk;
    }

    public void AdjustLethality(int lethal)
    {
        _lethality += lethal;
    }

    public void AdjustArmorPen(int aP)
    {
        _armorPen += aP;
    }

    public void AdjustDef(float def)
    {
        _def += def;
    }

    public void AdjustMoveSpeed(int moveSpeed)
    {
        _moveSpeed += moveSpeed;
    }

    public void AdjustAtkSpeed(float addSpeed)
    {
        _additionAS += addSpeed;
        _atkSpeed = _baseAS + (_baseAS * _additionAS);
    }

    public void AdjustCritChance(float cc)
    {
        _critChance += cc;
    }


    public void AdjustCritDmg(float cd)
    {
        _critDamage += cd;
    }

    public void AdjustRange(int range)
    {
        _range += range;
    }

    public void AdjustAbilityHaste(int ah)
    {
        _abilityHaste += ah;
    }

    public void AdjustLifeSteal(float ls)
    {
        _lifeSteal += ls;
    }

    public void SetState(State state)
    {
        if (_state == State.Unstoppable)
        {
            if (state == State.Neutral)
            {
                _state = state;
            }
        }
        else
        {
            _state = state;
        }
    }

    public void SetQCooldown()
    {
        qCurCool = 100 / (100 + _abilityHaste) * qCoolDown;
    }

    public void SetWCooldown()
    {
        wCurCool = 100 / (100 + _abilityHaste) * wCoolDown;
    }

    public void SetECooldown()
    {
        eCurCool = 100 / (100 + _abilityHaste) * eCoolDown;
    }

    public void SetRCooldown()
    {
        rCurCool = 100 / (100 + _abilityHaste) * rCoolDown;
    }

    public void SetCanRush(bool state)
    {
        canRush = state;
    }

    public void SetCanFlash(bool state)
    {
        canFlash = state;
    }
}
