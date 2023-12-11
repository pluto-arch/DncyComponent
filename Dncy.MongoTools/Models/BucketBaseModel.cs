using MongoDB.Bson;

namespace Dncy.MongoTools.Models
{

    public class BucketBaseModel
    {
        public ObjectId ObjectId { get; set; }

        public int BucketId { get; set; }

        public DateTime Start { get; set; }

        public DateTime end { get; set; }
    }


    public class BucketBaseModel<T> : BucketBaseModel
    {
        public List<T> Records { get; set; }

        public int Count { get; set; }
    }
}

