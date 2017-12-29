using System;

namespace Komorki.Common {
  public class Buffer<T> {
    private T[, ] items;
    public int width = 0;
    public int height = 0;
    public Buffer () { }

    public Buffer (int width, int height) {
      this.items = new T[width, height];
      this.width = width;
      this.height = height;
    }

    public Buffer (Buffer<T> buffer, int width, int height) {
      items = new T[width, height];
      this.width = width;
      this.height = height;

      for (int i = 0; i < width; ++i) {
        for (int j = 0; j < height; ++j) {
          items[i, j] = buffer.Get (i, j);
        }
      }
    }

    public T Get (int x, int y) {
      return items[x, y];
    }

    public T Get (int x, int y, T defaultValue) {
      if (!IsInside (x, y)) {
        return defaultValue;
      }
      return items[x, y];
    }

    public bool Get (int x, int y, out T value) {
      value = default (T);
      if (IsInside (x, y)) {
        value = items[x, y];
        return true;
      }

      return false;
    }

    public bool Set (T value, int x, int y) {
      if (!IsInside (x, y)) {
        return false;
      }
      items[x, y] = value;
      return true;
    }

    public void ForEach (Action<T, int, int> func) {
      for (int i = 0; i < width; ++i) {
        for (int j = 0; j < height; ++j) {
          func (Get (i, j), i, j);
        }
      }
    }

    public void Fill (T value) {
      for (int i = 0; i < width; ++i) {
        for (int j = 0; j < height; ++j) {
          items[i, j] = value;
        }
      }
    }

    public bool IsInside (int x, int y) {
      if (x < 0 || x >= width || y < 0 || y >= height) {
        return false;
      }

      return true;
    }
  }
}