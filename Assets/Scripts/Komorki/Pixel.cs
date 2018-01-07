using Komorki.Basic;
using PixelPos = System.Int32;

namespace Komorki {

  public enum GreatPixelType {
    Komorka,
    Terrain,
    Empty
  }
  public class GreatPixel {
    public GreatPixel lt;
    public GreatPixel lc;
    public GreatPixel lb;
    public GreatPixel ct;
    public GreatPixel cb;
    public GreatPixel rt;
    public GreatPixel rc;
    public GreatPixel rb;
    public GreatPixel Front;
    public GreatPixel Back;
    public GreatPixel[] directions = new GreatPixel[8];
    public Komorka Komorka;
    public GreatPixelType Type = GreatPixelType.Empty;
    public Vec3 Position = new Vec3(0, 0, 0);
    public GreatPixel (Vec3 position) {
      directions[0] = lb;
      directions[1] = cb;
      directions[2] = rb;
      directions[3] = rc;
      directions[4] = rt;
      directions[5] = ct;
      directions[6] = lt;
      directions[7] = lc;
      this.Position = position;
    }
    public void SetNeighbour (PixelPos x, PixelPos y, GreatPixel pixel) {
      if (x == -1 && y == 1) lt = pixel;
      if (x == -1 && y == 0) lc = pixel;
      if (x == -1 && y == -1) lb = pixel;
      if (x == 0 && y == 1) ct = pixel;
      if (x == 0 && y == -1) cb = pixel;
      if (x == 1 && y == 1) rt = pixel;
      if (x == 1 && y == 0) rc = pixel;
      if (x == 1 && y == -1) rb = pixel;
    }

    public GreatPixel GetNeighbour(PixelPos x, PixelPos y)
    {
      if (x == -1 && y == 1) return lt;
      if (x == -1 && y == 0) return lc;
      if (x == -1 && y == -1) return lb;
      if (x == 0 && y == 1) return ct;
      if (x == 0 && y == -1) return cb;
      if (x == 1 && y == 1) return rt;
      if (x == 1 && y == 0) return rc;
      if (x == 1 && y == -1) return rb;
      return null;
    }

    public void SetFront(GreatPixel pixel) {
      Front = pixel;
    }

    public void SetBack(GreatPixel pixel) {
      Back = pixel;
    }

  }
}