
using Microsoft.AspNetCore.Mvc;

namespace MosaicResidentInformationApi.V1.Boundary.Requests
{
    public class ResidentQueryParam
    {
        /// <example>
        /// Ciasom
        /// </example>
        /// Databind to first_name
        [FromQuery(Name = "first_name")]
        public string FirstName { get; set; }
        /// <example>
        /// Tessellate
        /// </example>
        /// Databind to last_name
        [FromQuery(Name = "last_name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
    }
}
