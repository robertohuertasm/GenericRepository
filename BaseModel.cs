using System.Runtime.Serialization;

namespace GenericRepository
{
    [DataContract(IsReference = true)]
    public class BaseModel<TId>
    {
        [DataMember]
        public TId Id { get; set; }
    }
}
