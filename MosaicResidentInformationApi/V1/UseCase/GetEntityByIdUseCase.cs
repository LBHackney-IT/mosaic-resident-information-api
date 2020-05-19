using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Gateways;

namespace MosaicResidentInformationApi.V1.UseCase
{
    public class GetEntityByIdUseCase
    {
        private IMosaicGateway _iMosaicGateway;
        public GetEntityByIdUseCase(IMosaicGateway iMosaicGateway)
        {
            _iMosaicGateway = iMosaicGateway;
        }

        public Entity Execute(int id)
        {
            return _iMosaicGateway.GetEntityById(id);
        }
    }
}
