using System;
using UnityEngine;

public class PlaySoundWhenBallHitsGround : MonoBehaviour
{
    public float SoundCooldownTime;

    private DateTime? _lastFloorCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var floor = collision.gameObject.GetComponent<Floor>();
        if (floor == null) return;

        if (SfxCooldownPassed())
        {
            SoundManager.PlayBallHitGroundSound();
            _lastFloorCollision = DateTime.Now;
        }
    }

    private bool SfxCooldownPassed()
    {
        return _lastFloorCollision == null ||
               (DateTime.Now - _lastFloorCollision).Value.TotalSeconds >= SoundCooldownTime;
    }
}
