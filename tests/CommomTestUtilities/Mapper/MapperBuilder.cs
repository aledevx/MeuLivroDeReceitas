using AutoMapper;
using CommomTestUtilities.IdEncryption;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommomTestUtilities.Mapper;

public class MapperBuilder
{
    public static IMapper Build()
    {
        var idEncripter = IdEncripterBuilder.Build();
        return new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping(idEncripter));
        }).CreateMapper();
    }
}