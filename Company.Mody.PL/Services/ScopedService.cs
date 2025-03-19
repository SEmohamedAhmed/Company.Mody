namespace Company.Mody.PL.Services
{
    public class ScopedService:IScopedService
    {
        public Guid Guid { get; }

        public ScopedService()
        {
            Guid = Guid.NewGuid();
        }
    }
}
