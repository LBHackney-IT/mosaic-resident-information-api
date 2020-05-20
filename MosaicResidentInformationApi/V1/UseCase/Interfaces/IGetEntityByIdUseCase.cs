using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public interface IGetEntityByIdUseCase
    {
        ResidentInformation Execute(int id);
    }
}
