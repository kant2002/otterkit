using System.Runtime.InteropServices;
namespace Otterkit.Library;

/// <summary>
/// Otterkit DecimalHolder ref struct
/// <para>This ref struct holds an IEEE 754 128-bit Decimal Floating Point Number</para>
/// <para>Decimal operations are done by calling DecimalMath static methods</para>
/// </summary>
public readonly ref struct DecimalHolder
{
    public readonly ReadOnlySpan<byte> Bytes;
    public readonly bool IsNegative = false;

    public DecimalHolder(ReadOnlySpan<byte> bytes)
    {
        Bytes = bytes;
        if (bytes[0] == 45)
            IsNegative = true;
    }

    public static implicit operator DecimalHolder(ReadOnlySpan<byte> bytes)
    {
        return new DecimalHolder(bytes);
    }

    public static implicit operator DecimalHolder(Numeric numeric)
    {
        if (numeric.IsSigned && numeric.Bytes[0] == 43)
        {
            return new DecimalHolder(numeric.Bytes[1..]);
        }
        return new DecimalHolder(numeric.Bytes);
    }

    public static DecimalHolder operator +(DecimalHolder left, DecimalHolder right)
    {
        return new(DecimalMath.Add(left.Bytes, right.Bytes));
    }

    public static DecimalHolder operator +(DecimalHolder number)
    {
        return DecimalMath.Plus(number);
    }

    public static DecimalHolder operator ++(DecimalHolder number)
    {
        return new(DecimalMath.Add(number.Bytes, "1"u8));
    }

    public static DecimalHolder operator -(DecimalHolder left, DecimalHolder right)
    {
        return new(DecimalMath.Subtract(left.Bytes, right.Bytes));
    }

    public static DecimalHolder operator -(DecimalHolder number)
    {
        return DecimalMath.Minus(number);
    }

    public static DecimalHolder operator --(DecimalHolder number)
    {
        return new(DecimalMath.Subtract(number.Bytes, "1"u8));
    }

    public static DecimalHolder operator *(DecimalHolder left, DecimalHolder right)
    {
        return new(DecimalMath.Multiply(left.Bytes, right.Bytes));
    }

    public static DecimalHolder operator /(DecimalHolder left, DecimalHolder right)
    {
        return new(DecimalMath.Divide(left.Bytes, right.Bytes));
    }

    public static DecimalHolder operator %(DecimalHolder left, DecimalHolder right)
    {
        return DecimalMath.Rem(left, right);
    }

    public static bool operator ==(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return result.Bytes[0] == 48;
    }

    public static bool operator !=(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return result.Bytes[0] != 48;
    }

    public static bool operator >(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return result.Bytes[0] == 49;
    }

    public static bool operator <(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return result.Bytes[0] == 45 && result.Bytes[1] == 49;
    }

    public static bool operator >=(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return result.Bytes[0] == 48 || result.Bytes[0] == 49;
    }

    public static bool operator <=(DecimalHolder left, DecimalHolder right)
    {
        DecimalHolder result = DecimalMath.Compare(left, right);
        return (result.Bytes[0] == 45 && result.Bytes[1] == 49) || result.Bytes[0] == 48;
    }

    public override bool Equals(object? obj)
    {
        throw new System.NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new System.NotImplementedException();
    }

    public string Display => System.Text.Encoding.UTF8.GetString(Bytes);
}

/// <summary>
/// Otterkit DecimalMath static class
/// <para>This static class contains methods to perform math operations on a DecimalHolder</para>
/// <para>Decimal operations are done by calling native C code from the mpdecimal library</para>
/// </summary>
public static class DecimalMath
{
    internal static unsafe ReadOnlySpan<byte> Arithmetic(ReadOnlySpan<byte> expression)
    {
        Span<byte> withNullTerminator = stackalloc byte[expression.Length + 1];

        expression.CopyTo(withNullTerminator);
        withNullTerminator[expression.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Arithmetic(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    internal static unsafe ReadOnlySpan<byte> Add(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        Span<byte> expression = stackalloc byte[left.Length + right.Length + 3];
        expression.Fill(32);

        left.CopyTo(expression);
        right.CopyTo(expression[(left.Length + 1)..]);
        expression[left.Length + right.Length + 2] = 43;

        ReadOnlySpan<byte> result = Arithmetic(expression);
        fixed (byte* Pointer = result)
        {
            return new ReadOnlySpan<byte>(Pointer, result.Length);
        }
    }

    internal static unsafe ReadOnlySpan<byte> Subtract(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        Span<byte> expression = stackalloc byte[left.Length + right.Length + 3];
        expression.Fill(32);

        left.CopyTo(expression);
        right.CopyTo(expression[(left.Length + 1)..]);
        expression[left.Length + right.Length + 2] = 45;

        ReadOnlySpan<byte> result = Arithmetic(expression);
        fixed (byte* Pointer = result)
        {
            return new ReadOnlySpan<byte>(Pointer, result.Length);
        }
    }

    internal static unsafe ReadOnlySpan<byte> Multiply(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        Span<byte> expression = stackalloc byte[left.Length + right.Length + 3];
        expression.Fill(32);

        left.CopyTo(expression);
        right.CopyTo(expression[(left.Length + 1)..]);
        expression[left.Length + right.Length + 2] = 42;

        ReadOnlySpan<byte> result = Arithmetic(expression);
        fixed (byte* Pointer = result)
        {
            return new ReadOnlySpan<byte>(Pointer, result.Length);
        }
    }

    internal static unsafe ReadOnlySpan<byte> Divide(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        Span<byte> expression = stackalloc byte[left.Length + right.Length + 3];
        expression.Fill(32);

        left.CopyTo(expression);
        right.CopyTo(expression[(left.Length + 1)..]);
        expression[left.Length + right.Length + 2] = 47;

        ReadOnlySpan<byte> result = Arithmetic(expression);
        fixed (byte* Pointer = result)
        {
            return new ReadOnlySpan<byte>(Pointer, result.Length);
        }
    }

    public static unsafe DecimalHolder Pow(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> expression = stackalloc byte[left.Bytes.Length + right.Bytes.Length + 3];
        expression.Fill(32);

        left.Bytes.CopyTo(expression);
        right.Bytes.CopyTo(expression[(left.Bytes.Length + 1)..]);
        expression[left.Bytes.Length + right.Bytes.Length + 2] = 94;

        ReadOnlySpan<byte> result = Arithmetic(expression);
        fixed (byte* Pointer = result)
        {
            return new ReadOnlySpan<byte>(Pointer, result.Length);
        }
    }

    public static unsafe DecimalHolder Exp(DecimalHolder exponent)
    {
        Span<byte> withNullTerminator = stackalloc byte[exponent.Bytes.Length + 1];

        exponent.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[exponent.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Exp(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Sqrt(DecimalHolder radicand)
    {
        Span<byte> withNullTerminator = stackalloc byte[radicand.Bytes.Length + 1];

        radicand.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[radicand.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Sqrt(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Ln(DecimalHolder argument)
    {
        Span<byte> withNullTerminator = stackalloc byte[argument.Bytes.Length + 1];

        argument.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[argument.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Ln(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Log10(DecimalHolder argument)
    {
        Span<byte> withNullTerminator = stackalloc byte[argument.Bytes.Length + 1];

        argument.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[argument.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Log10(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Abs(DecimalHolder number)
    {
        Span<byte> withNullTerminator = stackalloc byte[number.Bytes.Length + 1];

        number.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[number.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Abs(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Plus(DecimalHolder number)
    {
        Span<byte> withNullTerminator = stackalloc byte[number.Bytes.Length + 1];

        number.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[number.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Plus(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Minus(DecimalHolder number)
    {
        Span<byte> withNullTerminator = stackalloc byte[number.Bytes.Length + 1];

        number.Bytes.CopyTo(withNullTerminator);
        withNullTerminator[number.Bytes.Length] = 0;

        byte* result;

        fixed (byte* Pointer = withNullTerminator)
        {
            result = NativeDecimal128.Minus(Pointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Rem(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Rem(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Compare(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];
    
        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Compare(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Min(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Min(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Max(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Max(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Shift(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Shift(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder And(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.And(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Or(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Or(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

    public static unsafe DecimalHolder Xor(DecimalHolder left, DecimalHolder right)
    {
        Span<byte> leftWithNullTerminator = stackalloc byte[left.Bytes.Length + 1];
        Span<byte> rightWithNullTerminator = stackalloc byte[right.Bytes.Length + 1];

        left.Bytes.CopyTo(leftWithNullTerminator);
        leftWithNullTerminator[left.Bytes.Length] = 0;

        right.Bytes.CopyTo(rightWithNullTerminator);
        rightWithNullTerminator[right.Bytes.Length] = 0;

        byte* result;

        fixed (byte* LeftPointer = leftWithNullTerminator, RightPointer = rightWithNullTerminator)
        {
            result = NativeDecimal128.Xor(LeftPointer, RightPointer);
        }

        int length = 0;
        byte current = result[0];

        while (current != 0)
        {
            current = result[length];
            length++;
        }

        return new ReadOnlySpan<byte>(result, length - 1);
    }

}

internal partial class NativeDecimal128
{
    // Import native C code from libmpdec
    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "OtterkitArithmetic")]
    internal static unsafe partial byte* Arithmetic(byte* expression);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Exp")]
    internal static unsafe partial byte* Exp(byte* exponent);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Sqrt")]
    internal static unsafe partial byte* Sqrt(byte* radicand);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Ln")]
    internal static unsafe partial byte* Ln(byte* argument);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Log10")]
    internal static unsafe partial byte* Log10(byte* argument);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Abs")]
    internal static unsafe partial byte* Abs(byte* number);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Plus")]
    internal static unsafe partial byte* Plus(byte* number);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Minus")]
    internal static unsafe partial byte* Minus(byte* number);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Rem")]
    internal static unsafe partial byte* Rem(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Compare")]
    internal static unsafe partial byte* Compare(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Min")]
    internal static unsafe partial byte* Min(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Max")]
    internal static unsafe partial byte* Max(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Shift")]
    internal static unsafe partial byte* Shift(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128And")]
    internal static unsafe partial byte* And(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Or")]
    internal static unsafe partial byte* Or(byte* left, byte* right);

    [LibraryImport("OtterkitMath/Decimal128", EntryPoint = "Decimal128Xor")]
    internal static unsafe partial byte* Xor(byte* left, byte* right);
}
