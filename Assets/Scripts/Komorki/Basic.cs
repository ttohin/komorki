using System;
using System.Collections;
using System.Collections.Generic;
using PixelPos = System.Int32;

namespace Komorki.Basic {
  public class Vec2 {
    public static Vec2 Zero = new Vec2 (0, 0);
    public PixelPos x = 0;
    public PixelPos y = 0;

    public Vec2 (PixelPos x, PixelPos y) {
      this.x = x;
      this.y = y;
    }
    public Vec2 Normalize () {
      var max = Math.Max (Math.Abs (x), Math.Abs (y));
      if (max == 0) {
        return Vec2.Zero;
      }

      var res = new Vec2 (
        (PixelPos) Math.Round ((float) x / (float) max),
        (PixelPos) Math.Round ((float) y / (float) max)
      );
      return res;
    }

    public override string ToString () {
      return "[" + x.ToString () + "," + y.ToString () + "]";
    }
  }

  public class Vec3 {

    public PixelPos x = 0;
    public PixelPos y = 0;
    public PixelPos z = 0;

    public Vec3 (PixelPos x, PixelPos y, PixelPos z) {
      this.x = x;
      this.y = y;
      this.z = z;
    }

    public override string ToString () {
      return "[" + x.ToString () + ", " + y.ToString () + ", " + z.ToString() + "]";
    }
  }

}