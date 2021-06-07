using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.V1.UseCase.Interfaces
{
    public interface IAddResidentUseCase
    {
        ResidentInformation Execute(AddResidentRequest addResident);
    }
}
