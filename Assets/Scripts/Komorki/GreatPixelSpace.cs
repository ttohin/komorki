using System.Collections;
using System.Collections.Generic;
using Komorki.Basic;
using PixelPos = System.Int32;

namespace Komorki {
  public class GreatPixelSpace {
    public GreatPixel[, , ] Space;
    public PixelPos Width;
    public PixelPos Height;
    public PixelPos Depth;
    public GreatPixelSpace (PixelPos width, PixelPos height, PixelPos depth) {
      Space = new GreatPixel[width, height, depth];
      this.Width = width;
      this.Height = height;
      this.Depth = depth;
      for (int i = 0; i < width; i++) {
        for (int j = 0; j < height; j++) {
          for (int z = 0; z < depth; z++) { 
            Space[i, j, z] = new GreatPixel(new Vec3(i, j, z));
          }
        }
      }

      for (int z = 0; z < depth; z++) {
        for (int i = 0; i < width; i++) {
          for (int j = 0; j < height; j++) {

            GreatPixel pixel = Space[i, j, z];

            // connect neighbours with each other
            for (int di = -1; di <= 1; ++di) {
              for (int dj = -1; dj <= 1; ++dj) {
                if (i + di >= width || i + di < 0) {
                  continue;
                }

                if (j + dj >= height || j + dj < 0) {
                  continue;
                }

                if (di == 0 && dj == 0) {
                  continue;
                }

                GreatPixel neighbour = Space[i + di, j + dj, z];
                pixel.SetNeighbour (di, dj, neighbour);
              }
            }

            if (z + 1 < depth) {
              pixel.SetFront (Space[i, j, z + 1]);
            }

            if (z - 1 >= 0) {
              pixel.SetBack (Space[i, j, z - 1]);
            }
          }
        }
      }
    }
  }
}