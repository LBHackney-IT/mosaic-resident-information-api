using MosaicResidentInformationApi.V1.Domain;
using MosaicResidentInformationApi.V1.Infrastructure;

namespace MosaicResidentInformationApi.V1.Factories
{
    public class EntityFactory : AbstractEntityFactory
    {
        public override ResidentInformation ToDomain(DatabaseEntity databaseEntity)
        {
            return new ResidentInformation()
            {
            };
        }
    }
}
