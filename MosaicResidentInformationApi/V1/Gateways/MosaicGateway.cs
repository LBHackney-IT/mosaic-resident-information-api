using MosaicResidentInformationApi.V1.Boundary.Responses;
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

        public ResidentInformationList GetAllResidentsSelect(string FirstName, string LastName, string Address, string postCode)
        {
            var results = _mosaicContext.ResidentDatabaseEntities
                            .Where(res => res.FirstName.Equals(FirstName) || res.LastName.Equals(LastName));
            // res.AddressList || 
        }

        public Entity GetEntityById(int id)
        {
            var result = _mosaicContext.DatabaseEntities.Find(id);

            return (result != null) ?
                _entityFactory.ToDomain(result) :
                null;
        }


    }
}
