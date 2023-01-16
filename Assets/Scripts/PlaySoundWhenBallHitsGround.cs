using System;
using UnityEngine;

public class PlaySoundWhenBallHitsGround : MonoBehaviour
{
    [Tooltip("The time (in seconds) to wait before playing the sound again since the last time the ball hit the floor.")]
    [SerializeField]
    private float _soundCooldownTime;

    private DateTime? _lastFloorCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var floor = collision.otherCollider.gameObject.GetComponent<Floor>();
        if (floor == null) return;

        if(SfxCooldownPassed())
            SoundManager.PlayBallHitGroundSound();
    }

    private bool SfxCooldownPassed()
    {
        return _lastFloorCollision == null ||
               (DateTime.Now - _lastFloorCollision).Value.TotalSeconds >= _soundCooldownTime;
    }
}
