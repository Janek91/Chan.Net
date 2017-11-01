using System.Collections.Generic;

namespace Chan.Net
{
    public interface IQuotable
    {
        uint PostNumber { get; }
        void Update(IEnumerable<Post> newPosts);
    }
}