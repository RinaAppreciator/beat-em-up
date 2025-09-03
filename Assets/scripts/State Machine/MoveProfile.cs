using UnityEngine;

[CreateAssetMenu(fileName = "MoveProfile", menuName = "Scriptable Objects/MoveProfile")]
public class MoveProfile : ScriptableObject
{
    public bool canWalk ;
    public bool canRotate ;
    public bool canJump ;
    public bool canNormalAttack ;
    public bool canSpecialAttack;
    public bool enabled ;
    public bool knockedOut;
    public bool slowDown;
    public bool affectedByGravity;
}
