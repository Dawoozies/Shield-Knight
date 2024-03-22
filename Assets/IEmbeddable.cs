using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEmbeddable
{
    public void TryEmbed(ShieldThrow shieldThrow, Vector3 embeddingVelocity);
    public void TryRemoveEmbed(ShieldThrow shieldThrow, Vector3 recallVelocity);

}
