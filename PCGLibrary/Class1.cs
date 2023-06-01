namespace PCGLibrary;

public class PCG : Random
{
    private ulong _seed;
    public ulong Seed
    {
        get => _seed; 
        set => Initialization(value);
    }
    public ulong State { get; set; }

    private const byte HighOrderBitsCount = 5;
    private const byte AmountOfRotationUpToHighOrderBitsCountForInt64 = 64 - HighOrderBitsCount;
    private const byte AmountOfRotationUpToHighOrderBitsCountForInt32 = 32 - HighOrderBitsCount;
    private const byte AmountOfRotationForXorShift = (64 - AmountOfRotationUpToHighOrderBitsCountForInt32) / 2;
    
    private const ulong Multiplier = 6364136223846793005ul;
    private const ulong Increment = 1442695040888963407ul | 1;
    private const double DoubleConvertMultiplier = 1.0d / Multiplier;

    public PCG() : this((ulong)Environment.TickCount64) { }
    
    public PCG(ulong seed) => Initialization(seed);

    public override int Next() => (int)(NextUInt() >>> 1);

    public override int Next(int maxValue) => (int)NextUInt((uint)maxValue);
    
    public override int Next(int minValue, int maxValue) => (int)NextUInt((uint)minValue, (uint)maxValue);

    public uint NextUInt()
    {
        var oldState = State;
        State = LCG(State);
        
        var xorShift = (uint)(XorShift(oldState) >>> AmountOfRotationUpToHighOrderBitsCountForInt32);
        var rotationCount = (byte)(oldState >>> AmountOfRotationUpToHighOrderBitsCountForInt64);
        return RandomRotation(xorShift, rotationCount);
    }
    
    public uint NextUInt(uint maxValue)
    {
        var randomNumber = NextUInt();
        var result = (ulong)maxValue * randomNumber;
        result >>= 32;
        return (uint)result;
    }
    
    public uint NextUInt(uint minValue, uint maxValue)
    {
        if(minValue >= maxValue)
            throw new ArgumentException("MaxValue must be larger than MinValue");

        var offsetMaxValue = maxValue - minValue;
        var result = NextUInt(offsetMaxValue);
        return result + minValue;
    }

    public uint FastNextUInt()
    {
        var oldState = State;
        State *= Multiplier;

        oldState ^= oldState >>> 22;
        var rotationCount = (byte)(oldState >>> 61);
        return (uint)(oldState >> (22 + rotationCount));
    }

    public byte NextByte() => (byte)(NextUInt() >>> 24);

    public bool NextBool() => NextUInt() % 2 == 1;

    public override float NextSingle() => (float)Sample();
    
    public override double NextDouble() => Sample();

    protected override double Sample() => NextUInt() * DoubleConvertMultiplier;

    private static ulong LCG(ulong x) => Multiplier * x + Increment;

    private static ulong XorShift(ulong x) => x ^ x >>> AmountOfRotationForXorShift;
    
    private static uint RandomRotation(uint x, byte shift) => x >>> shift | x << (-shift & 31);

    private void Initialization(ulong seed)
    {
        _seed = seed;
        State = Seed + Increment;
        Next();
    }
}