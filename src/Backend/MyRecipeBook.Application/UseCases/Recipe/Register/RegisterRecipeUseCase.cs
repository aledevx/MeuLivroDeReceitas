using AutoMapper;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using MyRecipeBook.Application.Extensions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Exception;
using MyRecipeBook.Exception.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;
public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    public RegisterRecipeUseCase(IRecipeWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser,
        IMapper mapper,
        IBlobStorageService blobStorageService
        )
    {
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }
    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRegisterRecipeFormData request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.User();

        var recipeMapped = _mapper.Map<Domain.Entities.Recipe>(request);
        recipeMapped.UserId = loggedUser.Id;

        var instructions = request.Instructions.OrderBy(i => i.Step).ToList();
        for (var index = 0; index < instructions.Count; index++ ) 
        {
            instructions[index].Step = index + 1;
        }

        recipeMapped.Instructions = _mapper.Map<IList<Domain.Entities.Instruction>>(instructions);

        if (request.Image is not null) 
        {
            var fileStream = request.Image.OpenReadStream();

            (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

            if (isValidImage.IsFalse()) 
            {
                throw new ErrorOnValidationException([ResourceMessagesException.ONLY_IMAGES_ACCEPTED]);
            }

            recipeMapped.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            await _blobStorageService.Upload(loggedUser, fileStream, recipeMapped.ImageIdentifier);
        }

        await _writeOnlyRepository.Add(recipeMapped);

        await _unitOfWork.CommitAsync();

        return _mapper.Map<ResponseRegisteredRecipeJson>(recipeMapped);

    }
    private static void Validate(RequestRegisterRecipeFormData request) 
    {
        var validator = new RecipeValidator().Validate(request);

        if (validator.IsValid.IsFalse())
        {
            var errorMessages = validator.Errors.Select(e => e.ErrorMessage).Distinct().ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
