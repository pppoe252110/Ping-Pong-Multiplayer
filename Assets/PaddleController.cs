using Fusion;
using UnityEngine;

public class PaddleController : NetworkBehaviour
{
    public float MinBounds = -1.5f;
    public float MaxBounds = 1.5f;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector2 targetPos = transform.position;
            targetPos.x = Mathf.Clamp(data.position, MinBounds, MaxBounds);
            transform.position = targetPos;
        }
    }
}
