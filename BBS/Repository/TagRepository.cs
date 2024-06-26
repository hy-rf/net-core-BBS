using BBS.Data;
using BBS.IRepository;
using BBS.Models;

namespace BBS.Repository
{
    public class TagRepository(ForumContext context) : BaseRepository<Tag>(context), ITagRepository
    {
        public async Task<Tag> GetTagByNameAsync(string name)
        {
            var result = context.Tag.Single(t => t.Name == name);
            return await Task.FromResult(result);
        }
    }
}