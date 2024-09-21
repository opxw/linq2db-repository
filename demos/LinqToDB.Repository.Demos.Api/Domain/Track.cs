using LinqToDB.Mapping;

namespace LinqToDB.Repository.Demos.Api.Domain
{
    public class Track
    {
        [PrimaryKey, Identity]
        public int TrackId { get; set; }
        public string Name { get; set; }
        public int AlbumId { get; set; }
        public int MediaTypeId { get; set; }
        public int GenreId { get; set; }
        public string Composer { get; set; }
        public double UnitPrice { get; set; }
    }
}