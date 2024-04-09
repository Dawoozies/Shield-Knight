using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Physics2DFunctions
{
    public static class Cast
    {
        public static RaycastHit2D BoxCast(BoxCastInfo castInfo)
        {
            return Physics2D.BoxCast(castInfo.Origin, castInfo.Size, castInfo.Angle, castInfo.Direction,
                castInfo.Distance, castInfo.Layers);
        }
        public static RaycastHit2D[] BoxCastAll(BoxCastInfo castInfo)
        {
            return Physics2D.BoxCastAll(castInfo.Origin, castInfo.Size, castInfo.Angle, castInfo.Direction,
                castInfo.Distance, castInfo.Layers);
        }
    }
}