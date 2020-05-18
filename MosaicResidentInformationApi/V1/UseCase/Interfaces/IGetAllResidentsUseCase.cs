using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public interface IGetAllResidentsUseCase
    {
        ResidentInformationList Execute(ResidentQueryParam rqp);
    }
}
