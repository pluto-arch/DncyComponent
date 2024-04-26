using System.Security.Cryptography;

namespace Dotnetydd.QuartzHost.Utils;

internal static class TokenGenerator
{
    public static string GenerateToken()
    {
        // Generate a 128-bit entropy token 
        var tokenBytes = GenerateEntropyToken(size: 16); // 16 bytes = 128 bits 

        string tokenHex;
#if NET9_0_OR_GREATER
        tokenHex = Convert.ToHexStringLower(tokenBytes); 
#else
        tokenHex = Convert.ToHexString(tokenBytes).ToLowerInvariant();
#endif

        return tokenHex;
    }

    private static byte[] GenerateEntropyToken(int size)
    {
#if NET6_0_OR_GREATER
        return RandomNumberGenerator.GetBytes(size);
#else
        using (var rng = new RNGCryptoServiceProvider()) 
        { 
            byte[] token = new byte[size]; 
            rng.GetBytes(token); 
            return token; 
        } 
#endif
    }
}
