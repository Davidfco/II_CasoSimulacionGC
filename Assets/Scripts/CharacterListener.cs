using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterListener : MonoBehaviour
{
    [SerializeField]
    AttackMode[] attackModes;
    
    private CharacterController2D _character;


    private void Awake()
    {
        _character = GetComponentInParent<CharacterController2D>();
    }

    private AttackMode GetAttackMode(string name) 
    {
        foreach (AttackMode attackMode in attackModes)
        {
            if (attackMode.getName().Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return attackMode;
            }
        }
        return null;
    }

    public void OnPunch()
    {
        AttackMode attackMode = GetAttackMode("OnPunch");
        _character.Punch(attackMode.getDamage(), attackMode.getIsPercentage());
    }
    
    public void OnSuper()
    {
        AttackMode attackMode = GetAttackMode("OnSuper");
        _character.Super(attackMode.getDamage(), attackMode.getIsPercentage());
    }

}
