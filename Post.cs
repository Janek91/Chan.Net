using System;
using System.Collections.Generic;

namespace Chan.Net
{
    public class Post : IQuotable
    {
        public Thread Thread { get; set; }
        public uint PostNumber { get; set; }

        public string Message { get; set; }
        public string Name { get; set; }
        public PostedImage Image { get; set; }

        public DateTime TimeCreated { get; set; }
        public long? EpochTimeCreated { get; set; }
        public string Trip { get; set; }
        public string Country { get; set; }
        public string CountryName { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }

        public event ReplyEventDelegate Quoted;

        public void Update(IEnumerable<Post> newPosts)
        {
            foreach (var newReply in newPosts)
            {
                if (newReply.Message.Contains(">>" + PostNumber))
                {
                    Quoted?.Invoke(this, newReply);
                }
            }
        }
    }
}