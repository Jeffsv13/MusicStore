using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MusicStore.Dto.Validations;

public class FileSizeValidation : ValidationAttribute
{
    private readonly int maxSizeInMegaBytes;
    public FileSizeValidation(int MaxSizeInMegabytes)
    {
        maxSizeInMegaBytes = MaxSizeInMegabytes;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is null)
            return ValidationResult.Success;
        IFormFile? formFile = value as IFormFile;

        if (formFile is null)
            return ValidationResult.Success;

        if(formFile.Length > maxSizeInMegaBytes*1024*1024)
            return new ValidationResult($"File size must not exceed {maxSizeInMegaBytes} mb.");

        return ValidationResult.Success;
    }
}
