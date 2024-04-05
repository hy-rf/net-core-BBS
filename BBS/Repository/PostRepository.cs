
using System.Linq.Expressions;
using BBS.Data;
using BBS.IRepository;
using BBS.Models;

namespace BBS.Repository
{
    public class PostRepository(AppDbContext context) : BaseRepository<Post>(context), IPostRepository
    {
    }


}