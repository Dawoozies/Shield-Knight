using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public static class BoxCast2DGizmo
{
    public static void BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance = Mathf.Infinity)
    {
        // clockwise going around from lower left
        Vector2[] starts = new Vector2[4];

        float w = size.x / 2;
        float h = size.y / 2;

        starts[0] = new Vector2(-w, -h);
        starts[1] = new Vector2(-w, +h);
        starts[2] = new Vector2(+w, +h);
        starts[3] = new Vector2(+w, -h);

        Quaternion q = Quaternion.Euler(0, 0, angle);

        // rotate and add offsets
        for (int i = 0; i < starts.Length; i++)
        {
            starts[i] = q * starts[i];

            starts[i] = starts[i] + origin;
        }

        // draw the origin box
        for (int i = 0; i < starts.Length; i++)
        {
            int j = (i + 1) % starts.Length;

            Gizmos.DrawLine(starts[i], starts[j]);
        }

        float castDistance = float.IsInfinity(distance) ? 1E+5f : distance;

        Vector2 offset = direction.normalized * castDistance;

        Vector2[] finishes = new Vector2[4];
        for (int i = 0; i < starts.Length; i++)
        {
            finishes[i] = starts[i] + offset;
        }

        // draw the destination box
        for (int i = 0; i < finishes.Length; i++)
        {
            int j = (i + 1) % finishes.Length;

            Gizmos.DrawLine(finishes[i], finishes[j]);
        }

        // streaks between them
        for (int i = 0; i < finishes.Length; i++)
        {
            Gizmos.DrawLine(starts[i], finishes[i]);
        }
    }
    public static void BoxCast(ShieldSystem.CastInfoDebug castInfo)
    {
        BoxCast(castInfo.origin, castInfo.size, castInfo.angle, castInfo.direction, castInfo.distance);
    }

    public static void BoxCast(BoxCastInfo castInfo)
    {
        BoxCast(castInfo.Origin, castInfo.Size, castInfo.Angle, castInfo.Direction, castInfo.Distance);
    }
}