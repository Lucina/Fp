using NUnit.Framework;

namespace Fp.Tests;

public class BitUtil_NumberTools
{
    public static (int value, int align, int expected)[] AlignUp_ValidInputs = new[]
    {
        (0, 0, 0), //
        (1, 0, 1), //
        (0, 1, 0), //
        (1, 1, 1), //
        (0, 3, 0), //
        (1, 3, 3), //
        (2, 3, 3), //
        (3, 3, 3), //
    };

    public static (int value, int align, int expected)[] AlignDown_ValidInputs = new[]
    {
        (0, 0, 0), //
        (1, 0, 1), //
        (0, 1, 0), //
        (1, 1, 1), //
        (0, 3, 0), //
        (1, 3, 0), //
        (2, 3, 0), //
        (3, 3, 3), //
    };

    public static (int value, int expected)[] GetBitsForBytes_ValidInputs = new[]
    {
        (0, 0), //
        (1, 8), //
        (2, 16), //
        (3, 24), //
    };

    public static (int value, int expected)[] GetBytesForBits_ValidInputs = new[]
    {
        (0, 0), //
        (1, 1), //
        (8, 1), //
        (9, 2), //
        (16, 2), //
        (17, 3), //
    };

    [Test]
    public void AlignUp_ValidInputs_Correct([ValueSource("AlignUp_ValidInputs")] (int value, int align, int expected) input)
    {
        Assert.That(BitUtil.AlignUp(input.value, input.align), Is.EqualTo(input.expected));
    }

    [Test]
    public void AlignDown_ValidInputs_Correct([ValueSource("AlignDown_ValidInputs")] (int value, int align, int expected) input)
    {
        Assert.That(BitUtil.AlignDown(input.value, input.align), Is.EqualTo(input.expected));
    }

    [Test]
    public void GetBytesForBits_ValidInputs_Correct([ValueSource("GetBytesForBits_ValidInputs")] (int value, int expected) input)
    {
        Assert.That(BitUtil.GetBytesForBits(input.value), Is.EqualTo(input.expected));
    }

    [Test]
    public void GetBitsForBytes_ValidInputs_Correct([ValueSource("GetBitsForBytes_ValidInputs")] (int value, int expected) input)
    {
        Assert.That(BitUtil.GetBitsForBytes(input.value), Is.EqualTo(input.expected));
    }
}
