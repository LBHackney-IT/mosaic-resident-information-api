using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using ResidentInformation = MosaicResidentInformationApi.V1.Domain.ResidentInformation;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public interface IMosaicGateway
    {
        ResidentInformation GetEntityById(int id);
        ResidentInformationList GetAllResidentsSelect(ResidentQueryParam rqp);
    }
}
