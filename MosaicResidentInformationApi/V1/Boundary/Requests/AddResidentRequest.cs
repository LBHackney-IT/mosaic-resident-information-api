using Microsoft.AspNetCore.Mvc;

namespace MosaicResidentInformationApi.V1.Boundary.Requests
{
    public class AddResidentRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
