using System;

namespace Komorki.Common {
  public class Part {
    public enum Type {
      Empty,
      Corner,
      Border,
      BorderTopToCorner,
      BorderBottomToCorner,
      BorderLeftToCorner,
      BorderRightToCorner,
      Bridge,
      Fill,
      InnnerCornter
    }

    public enum Position {
      Top,
      Left,
      Bottom,
      Right,
      Center,
      TopLeft,
      TopCenter,
      TopRight,
      CenterRight,
      BottomRight,
      BottomCenter,
      BottomLeft,
      CenterLeft
    }

    public Type type;
    public Position position;
  }

  public class CellGenerator {
    public CellGenerator () { }
  }

  public class ShapeAnalizer {
    public static int scale = 4;
    public ShapeAnalizer (Buffer<bool> inputBuffer) {
      internalBuffer = new Buffer<bool> (inputBuffer.width * scale, inputBuffer.height * scale);
      inputBuffer.Scale (scale, out internalBuffer);
      resultBuffer = new Buffer<Part> (internalBuffer.width, internalBuffer.height);

      internalBuffer.ForEach ((value, x, y) => {

        Part part = new Part ();
        if (value == false) {
          int nSquare = CountNeighborsSquare (x, y);
          int nDiamond = CountNeighborsDiamond (x, y);

          if (nSquare == 2 && nDiamond == 2) {
            AnalizeInnterCorner (x, y, out part);
          }
        } else {
          int nSquare = CountNeighborsSquare (x, y);
          int nDiamond = CountNeighborsDiamond (x, y);
          if (nSquare == 4) {
            part.type = Part.Type.Fill;
          } else if (nSquare == 2 && nDiamond == 1) {
            AnalizeCorner (x, y, out part);
          } else if (nSquare == 3 && nDiamond == 2) {
            AnalizeBorder (x, y, out part);
          } else if (nSquare == 3 && nDiamond == 3) {
            AnalizeBorderToCorner (x, y, out part);
          } else if (nSquare == 2 && nDiamond == 2) {
            AnalizeBridge (x, y, out part);
          }
        }

        if (part.type == Part.Type.Empty && value) {
          part.type = Part.Type.Fill;
        }

        resultBuffer.Set (part, x, y);
      });
    }

    int CountNeighborsSquare (int x, int y) {
      bool value;
      int count = 0;
      if (internalBuffer.Get (x + 1, y, out value) && value == true) count += 1;
      if (internalBuffer.Get (x - 1, y, out value) && value == true) count += 1;
      if (internalBuffer.Get (x, y + 1, out value) && value == true) count += 1;
      if (internalBuffer.Get (x, y - 1, out value) && value == true) count += 1;
      return count;
    }

    int CountNeighborsDiamond (int x, int y) {
      bool value;
      int count = 0;
      if (internalBuffer.Get (x + 1, y + 1, out value) && value == true) count += 1;
      if (internalBuffer.Get (x - 1, y - 1, out value) && value == true) count += 1;
      if (internalBuffer.Get (x - 1, y + 1, out value) && value == true) count += 1;
      if (internalBuffer.Get (x + 1, y - 1, out value) && value == true) count += 1;
      return count;
    }

    void AnalizeCorner (int x, int y, out Part result) {
      result = new Part ();
      bool value;
      if (internalBuffer.Get (x, y - 1, out value) && value) // left of right
      {
        if (internalBuffer.Get (x + 1, y, out value) && value)
          result.position = Part.Position.TopLeft;
        else
          result.position = Part.Position.TopRight;
      } else {
        if (internalBuffer.Get (x + 1, y, out value) && value)
          result.position = Part.Position.BottomLeft;
        else
          result.position = Part.Position.BottomRight;
      }

      result.type = Part.Type.Corner;
    }

    void AnalizeBorder (int x, int y, out Part result) {
      result = new Part ();
      bool value;
      if (!internalBuffer.Get (x, y + 1, out value) || value == false)
        result.position = Part.Position.Top;
      else if (!internalBuffer.Get (x, y - 1, out value) || value == false)
        result.position = Part.Position.Bottom;
      else if (!internalBuffer.Get (x + 1, y, out value) || value == false)
        result.position = Part.Position.Right;
      else if (!internalBuffer.Get (x - 1, y, out value) || value == false)
        result.position = Part.Position.Left;

      result.type = Part.Type.Border;
    }

    void AnalizeBridge (int x, int y, out Part result) {
      result = new Part ();
      bool value;
      if ((!internalBuffer.Get (x, y + 1, out value) || value == false) &&
        (!internalBuffer.Get (x + 1, y, out value) || value == false))
        result.position = Part.Position.TopRight;
      if ((!internalBuffer.Get (x, y - 1, out value) || value == false) &&
        (!internalBuffer.Get (x - 1, y, out value) || value == false))
        result.position = Part.Position.BottomLeft;
      if ((!internalBuffer.Get (x, y + 1, out value) || value == false) &&
        (!internalBuffer.Get (x - 1, y, out value) || value == false))
        result.position = Part.Position.TopLeft;
      if ((!internalBuffer.Get (x, y - 1, out value) || value == false) &&
        (!internalBuffer.Get (x + 1, y, out value) || value == false))
        result.position = Part.Position.BottomRight;

      result.type = Part.Type.Bridge;
    }

    void AnalizeBorderToCorner (int x, int y, out Part result) {
      result = new Part ();
      bool value;
      if (!internalBuffer.Get (x, y + 1, out value) || value == false) {
        if (!internalBuffer.Get (x - 1, y + 1, out value) || value == false) {
          result.position = Part.Position.Right;
          result.type = Part.Type.BorderTopToCorner;
        } else {
          result.position = Part.Position.Left;
          result.type = Part.Type.BorderTopToCorner;
        }
      } else if (!internalBuffer.Get (x, y - 1, out value) || value == false) {
        if (!internalBuffer.Get (x + 1, y - 1, out value) || value == false) {
          result.position = Part.Position.Left;
          result.type = Part.Type.BorderBottomToCorner;
        } else {
          result.position = Part.Position.Right;
          result.type = Part.Type.BorderBottomToCorner;
        }
      } else if (!internalBuffer.Get (x + 1, y, out value) || value == false) {
        if (!internalBuffer.Get (x + 1, y + 1, out value) || value == false) {
          result.position = Part.Position.Bottom;
          result.type = Part.Type.BorderRightToCorner;
        } else {
          result.position = Part.Position.Top;
          result.type = Part.Type.BorderRightToCorner;
        }
      } else if (!internalBuffer.Get (x - 1, y, out value) || value == false) {
        if (!internalBuffer.Get (x - 1, y - 1, out value) || value == false) {
          result.position = Part.Position.Top;
          result.type = Part.Type.BorderLeftToCorner;
        } else {
          result.position = Part.Position.Bottom;
          result.type = Part.Type.BorderLeftToCorner;
        }
      }
    }

    void AnalizeInnterCorner (int x, int y, out Part result) {
      result = new Part ();
      bool value;
      if (internalBuffer.Get (x, y - 1, out value) && value == true &&
        internalBuffer.Get (x + 1, y, out value) && value == true)
        result.position = Part.Position.BottomRight;
      if (internalBuffer.Get (x, y + 1, out value) && value == true &&
        internalBuffer.Get (x - 1, y, out value) && value == true)
        result.position = Part.Position.TopLeft;
      if (internalBuffer.Get (x, y - 1, out value) && value == true &&
        internalBuffer.Get (x - 1, y, out value) && value == true)
        result.position = Part.Position.BottomLeft;
      if (internalBuffer.Get (x, y + 1, out value) && value == true &&
        internalBuffer.Get (x + 1, y, out value) && value == true)
        result.position = Part.Position.TopRight;

      result.type = Part.Type.InnnerCornter;
    }

    public Buffer<Part> resultBuffer;
    private Buffer<bool> internalBuffer;
  }
}