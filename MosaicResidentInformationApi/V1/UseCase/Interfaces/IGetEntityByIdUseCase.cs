using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public interface IGetResidentInformationByPersonIdUseCase
    {
        ResidentInformation Execute(int id);
    }
}
