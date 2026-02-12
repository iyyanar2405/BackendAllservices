using AuthProvider.Settings;
using OtpNet;
using QRCoder;
using System.Drawing;
using Windows.Media.Playback;

namespace AuthProvider.Providers;

public class TotpAuthProvider 
{
    public string GenerateSecret()
    {
        byte[] randomBytes = new byte[20];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
        }
        return Base32Encoding.ToString(randomBytes);
    }

    public string GenerateOtpUrl(string baseUrl, string secret, string userName, string companyName, int digits = 6)
    {

        return $"{baseUrl}/api/authorize/totp/{Uri.EscapeDataString(userName)}?secret={secret}&digits={digits}&issuer={Uri.EscapeDataString("Hadvida")}";
    }

    public bool ValidateToken(string secret, string token)
    {
        byte[] secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        return totp.VerifyTotp(token, out _);
    }

    public byte[] GenerateQrCode(string otpUrl)
    {
        QRCodeData qrCodeData;
        using (var qrGenerator = new QRCodeGenerator())
        {
            qrCodeData = qrGenerator.CreateQrCode(otpUrl, QRCodeGenerator.ECCLevel.Q);
        }
        PngByteQRCode qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
}

public class TfaRegisterModel
{
    public byte[] qrCodeImageUrl { get; set; }
    public string qrManualEntryKey { get; set; }
}
