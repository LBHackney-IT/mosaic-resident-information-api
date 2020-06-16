namespace MosaicResidentInformationApi.V1.UseCase
{
    public interface IValidatePostcode
    {
        bool Execute(string postcode);
    }
}
