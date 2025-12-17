using Moq;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Services.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommomTestUtilities.OpenAI;
public class GenerateRecipeAIBuilder
{
    public static IGenerateRecipeAI Build(GeneratedRecipeDto dto) 
    {
        var mock = new Mock<IGenerateRecipeAI>();

        mock.Setup(service => service.Generate(It.IsAny<List<string>>())).ReturnsAsync(dto);

        return mock.Object;
    }
}
