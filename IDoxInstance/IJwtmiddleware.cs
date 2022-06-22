using IDoxInstance.Entities;

namespace IDoxInstance
{
    public interface IJwtmiddleware 
    {
        public Task<string> JSONToken(User user);
    }
}
