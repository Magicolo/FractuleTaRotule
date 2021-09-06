using Melanchall.DryWetMidi.Common;

public enum Track : byte
{
    // Bells_1 = 0,
    Bells_2 = 1,
    SmoothLead = 2,
    // WeirdLead = 3,
    Pad1 = 4,
    // Pad2 = 5,
    Pad3 = 6,
    RythmicPad = 7,
    // NoisyPad = 8,
    GhostPad = 9,
    // FallingEffect = 10,
    // UpwardEffect = 11,
    Bass = 12,
    Drum = 13,
}

public static class TracksExtensions
{
    public static FourBitNumber Channel(this Track track) => new FourBitNumber((byte)track);
}