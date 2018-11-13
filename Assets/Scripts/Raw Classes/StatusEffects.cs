using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatusEffects{

    public float damage;
    public float attackSpeed;


    public void SetAttackSpeed(float mult) { this.attackSpeed = mult; }
    public float GetAttackSpeed() { return attackSpeed; }

    public void SetDamage(float mult) { this.damage = mult; }
    public float GetDamage() { return damage; }

    public StatusEffects()
    {
        damage = 1;
        attackSpeed = 1;
    }
}
