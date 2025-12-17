using AutoMapper;
using Sqids;

namespace CommomTestUtilities.IdEncryption;
public class IdEncripterBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new SqidsOptions()
        {
            MinLength = 3,
            Alphabet = "VWdeDEFGHIJKfghijnopqr3456stuvwxyzABCLMNklmOPQRSTUXYZ0abc12789",
        });
    }
}
