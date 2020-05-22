using MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.V1.UseCase.Interfaces
{
    public interface IGetAllResidentsUseCase
    {
        ResidentInformationList Execute();
    }
}
