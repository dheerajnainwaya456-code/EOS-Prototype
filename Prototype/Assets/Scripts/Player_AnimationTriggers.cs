using UnityEngine;

public class Player_AnimationTriggers : MonoBehaviour
{
    Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    private void currentStateTrigger()
    {
        player.CallAnimationTrigger();
    }
}
