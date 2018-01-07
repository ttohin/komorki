using System.Collections;
using System.Collections.Generic;

namespace Komorki {
  public class Komorka {
    public GreatPixel Position;
    public void MoveToPosition (GreatPixel newPosition) {
      if (newPosition == null)
      {
        return;
      }

      if (Position != null) {
        Position.Type = GreatPixelType.Empty;
        Position.Komorka = null;
      }

      this.Position = newPosition;
      this.Position.Type = GreatPixelType.Komorka;
      this.Position.Komorka = this;
    }
  }
}