using System.Collections.Generic;
using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Gateways;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetAllResidentsUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        private List<ResidentInformation> _residentInformation;

        public GetAllResidentsUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }        
        
        public ResidentInformationList Execute()
        {
            return _iMosaicGateway.GetAllResidentsSelect();
        }        
    }
}