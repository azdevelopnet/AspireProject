using System;

namespace Aspire.ApiServices.Models
{
    public class Appointment: MongoDbbase
    {
        public string PatientId { get; set; }
        public string ProviderId { get; set; }
        public DateTime Date { get; set; }
        public AppointmentBlock Block { get; set; }
        public string PatientNotes { get; set; }
        public string ProviderNotes { get; set; }
    }
}
