using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public bool CanWalk { get; private set; }
    public bool CanRotate { get; private set; }
    public bool CanJump { get; private set; }
    public bool Enabled { get; private set; }

    public bool CanNormalAttack { get; private set; }

    public bool CanSpecialAttack { get; private set; }

    public bool SlowedDown { get; private set; }

    public bool AffectedByGravity { get; private set; }

    public bool KnockedOut { get; private set; }

    public void ApplyProfile(MoveProfile profile)
    {
        CanWalk = profile.canWalk;
        CanRotate = profile.canRotate;
        CanJump = profile.canJump;
        Enabled = profile.enabled;
        CanSpecialAttack = profile.canSpecialAttack;
        CanNormalAttack = profile.canNormalAttack;
        KnockedOut = profile.knockedOut;
        SlowedDown = profile.slowDown;
        AffectedByGravity = profile.affectedByGravity;
    }
}
