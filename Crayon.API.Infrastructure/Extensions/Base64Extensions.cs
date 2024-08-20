using System.Text;

namespace Crayon.API.Infrastructure.Extensions;

public static class Base64Extensions
{
    public static string EncodeStringAsB64String(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        return EncodeBytesAsB64String(bytes);
    }
    
    public static string EncodeBytesAsB64String(byte[] data)
    {
        return Convert.ToBase64String(data);
    }
    
    public static byte[] EncodeStringAsB64ByteArray(string data)
    {
        string b64string = EncodeStringAsB64String(data);
        return Encoding.UTF8.GetBytes(b64string);
    }

    public static byte[] EncodeBytesAsB64ByteArray(byte[] data)
    {
        string b64data = EncodeBytesAsB64String(data);
        return Encoding.UTF8.GetBytes(b64data);
    }
}