using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public float RespawnDelay = 5;  //재생성주기
    public float MaxHp;             //최대체력
    public float AttackDamage;      //공격력
    public float AttackRange;       //공격가능 거리
    public float AttackDelay;       //공격대기시간
}
