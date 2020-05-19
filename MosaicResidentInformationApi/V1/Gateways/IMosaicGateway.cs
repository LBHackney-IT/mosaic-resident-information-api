using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Domain;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public interface IMosaicGateway
    {
        ResidentInformation GetEntityById(int id);
        ResidentInformationList GetAllResidentsSelect(ResidentQueryParam rqp);
    }
}
