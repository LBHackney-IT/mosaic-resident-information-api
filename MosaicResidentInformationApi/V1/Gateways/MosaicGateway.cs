using MosaicResidentInformationApi.V1.Boundary.Responses;
using MosaicResidentInformationApi.V1.Boundary.Requests;
using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Factories;
using MosaicResidentInformationApi.V1.Infrastructure;
using System.Linq;

namespace MosaicResidentInformationApi.V1.Gateways
{
    public class MosaicGateway : IMosaicGateway
    {
        private readonly IMosaicContext _mosaicContext;
        private readonly EntityFactory _entityFactory;

        public MosaicGateway(IMosaicContext mosaicContext)
        {
            _mosaicContext = mosaicContext;
            _entityFactory = new EntityFactory();
        }

        public ResidentInformationList GetAllResidentsSelect(ResidentQueryParam rqp)
        {
            var results = _mosaicContext.ResidentDatabaseEntities
                            .Where(res => res.FirstName.Equals(rqp.FirstName) || res.LastName.Equals(rqp.LastName))
                            .ToList();

            return new ResidentInformationList() { Residents = results };
        }

        public ResidentInformation GetEntityById(int id)
        {
            var result = _mosaicContext.DatabaseEntities.Find(id);

            return (result != null) ?
                _entityFactory.ToDomain(result) :
                null;
        }


    }
}
