using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetEntityByIdUseCase : IGetEntityByIdUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        public GetEntityByIdUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }

        public ResidentInformation Execute(int id)
        {
            return _iMosaicGateway.GetEntityById(id);
        }
    }
}
