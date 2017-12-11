using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using NUnit.Framework;

namespace Komorki.Generation {
  [TestFixture]
  public class ShapeGenerationTest {
    [Test]
    public void HorizontalMirror () {
      // 0001
      // 0001
      // 0100
      // 1001
      var buffer = new Buffer<bool> (width: 4, height: 4);
      buffer.Set (true, 0, 0);
      buffer.Set (true, 1, 1);
      buffer.Set (true, 0, 3);
      buffer.Set (true, 2, 3);
      buffer.Set (true, 3, 3);

      // 1001
      // 0100
      // 0100
      // 1001
      var expectedResult = new Buffer<bool> (width: 4, height: 4);
      expectedResult.Set (true, 0, 0);
      expectedResult.Set (true, 1, 1);
      expectedResult.Set (true, 0, 3);
      expectedResult.Set (true, 1, 2);
      expectedResult.Set (true, 3, 0);
      expectedResult.Set (true, 3, 3);

      buffer.ForEach ((value, i, j) => {
        Assert.That (expectedResult.Get (i, j), Is.EqualTo (value),
          string.Format ("value ({2}) at index {0},{1} should be equal to value ({3})", i, j, value, expectedResult.Get (i, j)));
      });
    }
  }
}