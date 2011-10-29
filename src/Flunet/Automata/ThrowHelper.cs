using System;

internal static class ThrowHelper
{
    private const string NO_STATE_AVAILABLE = "No states available. An automata needs to be initialized with at least one state before this method can be used.";

    public static void ThrowNoStatesAvailable()
    {
        throw new InvalidOperationException(NO_STATE_AVAILABLE);
    }
}