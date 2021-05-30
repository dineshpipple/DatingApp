using System.Threading.Tasks;

namespace API.Interface
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository {get;}
        IMessageRepository MessageRepository {get;}
        ILikeRepository LikeRepository {get;}

        Task<bool> Complete();
        bool HasChange();
    }
}