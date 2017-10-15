using UnityEngine;

namespace MeshGeneration {
  public class SquareGrid {
    public Square[, ] grid;
    public int width;
    public int height;
    public SquareGrid (int width, int height, MutableMesh mesh) {
      grid = new Square[width, height];
      this.width = width;
      this.height = height;

      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          grid[i, j] = new Square (mesh, new Vector3 (2 * i,  2 * j, 0), null);
        }
      }

      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          var square = grid[i, j];
          if (i != width - 1)
            square.JoinWithSquare (grid[i + 1, j], SquarePoint.RightCenter);
        }
      }
      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          var square = grid[i, j];
          if (j != height - 1)
            square.JoinWithSquare (grid[i, j + 1], SquarePoint.TopCenter);
        }
      }
    }

    public void Build()
    {
      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          grid[i, j].Build();
        }
      }
    }
  }
}