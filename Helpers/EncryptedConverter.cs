using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NationalCardBookingSystemWithoutCleanArch.Helpers
{
    public class EncryptedConverter : ValueConverter<string, string>
    {
        public EncryptedConverter() :
            base(
                v => EncryptionHelper.Encrypt(v),
                v => EncryptionHelper.Decrypt(v))
        { }
    }
}
