using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetAllResidentsUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        public GetAllResidentsUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }

        public ResidentInformationList Execute(ResidentQueryParam rqp)
        {
            return _iMosaicGateway.GetAllResidentsSelect(rqp);
        }
    }
}
