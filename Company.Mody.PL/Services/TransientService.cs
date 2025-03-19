namespace Company.Mody.PL.Services
{
    public class TransientService:ITransientService
    {
        public Guid Guid { get; }

        public TransientService()
        {
            Guid = Guid.NewGuid();
        }
    }
}
