
namespace Company.Mody.PL.Services
{
    public class SingletonService : ISingletonService
    {
        public Guid Guid { get; }

        public SingletonService() 
        { 
            Guid = Guid.NewGuid();
        }
    }
}
