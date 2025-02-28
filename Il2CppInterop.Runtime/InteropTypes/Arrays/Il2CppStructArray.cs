using System;
using System.Diagnostics.CodeAnalysis;

namespace Il2CppInterop.Runtime.InteropTypes.Arrays;

public class Il2CppStructArray<T> : Il2CppArrayBase<T> where T : unmanaged
{
    static Il2CppStructArray()
    {
        StaticCtorBody(typeof(Il2CppStructArray<T>));
    }

    public Il2CppStructArray(IntPtr nativeObject) : base(nativeObject)
    {
    }

    public Il2CppStructArray(long size) : base(AllocateArray(size))
    {
    }

    public Il2CppStructArray(T[] arr) : base(AllocateArray(arr.Length))
    {
        for (int i = 0; i < arr.Length; i++)
        {
            this[i] = arr[i];
        }
    }

    public override T this[int index]
    {
        get => GetElementPointer(index);
        set => GetElementPointer(index) = value;
    }

    private unsafe ref T GetElementPointer(int index)
    {
        ThrowIfIndexOutOfRange(index);
        return ref ((T*)ArrayStartPointer.ToPointer())[index];
    }

    [return: NotNullIfNotNull(nameof(arr))]
    public static implicit operator Il2CppStructArray<T>?(T[]? arr)
    {
        if (arr == null) return null;

        return new Il2CppStructArray<T>(arr);
    }

    //public static implicit operator Span<T>(Il2CppStructArray<T>? il2CppArray)
    //{
    //    return il2CppArray is not null ? il2CppArray.AsSpan() : default;
    //}

    //public static implicit operator ReadOnlySpan<T>(Il2CppStructArray<T>? il2CppArray)
    //{
    //    return il2CppArray is not null ? il2CppArray.AsSpan() : default;
    //}

    private static IntPtr AllocateArray(long size)
    {
        if (size < 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Array size must not be negative");

        var elementTypeClassPointer = Il2CppClassPointerStore<T>.NativeClassPtr;
        if (elementTypeClassPointer == IntPtr.Zero)
            throw new ArgumentException(
                $"{nameof(Il2CppStructArray<T>)} requires an Il2Cpp reference type, which {typeof(T)} isn't");
        return IL2CPP.il2cpp_array_new(elementTypeClassPointer, (ulong)size);
    }
}
