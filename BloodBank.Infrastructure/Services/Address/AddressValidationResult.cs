namespace BloodBank.Infrastructure.Services.Address;

public record AddressValidationResult(bool IsValid, AddressValidationError? Error = null);

public enum AddressValidationError
{
    InvalidZipcode,  
    ApiFailure,     
    UnknownError 
}