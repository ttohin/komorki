using System.Collections;
using System.Collections.Generic;
using Komorki.Basic;

namespace Komorki {
  public enum ShapeType {
    SinglePixel,
    Rectangle,
    StaticShape
  }
  public abstract class IShape {
    public abstract ShapeType GetShapeType ();
    public abstract bool CanBeHere (GreatPixel position);
  }

  public class SinglePixelShape : IShape {
    public GreatPixel Position;
    public override ShapeType GetShapeType () {
      return ShapeType.SinglePixel;
    }
    public override bool CanBeHere (GreatPixel position) {
      return position.Komorka == null && position.Type == GreatPixelType.Empty;
    }
  }
}