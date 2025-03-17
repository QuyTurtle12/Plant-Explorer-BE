using Google.Cloud.Firestore;

namespace Plant_Explorer.Contract.Repositories.ModelViews.ImageModel
{
    [FirestoreData]
    public class ImageRecord
    {
        [FirestoreProperty]
        public string Name { get; set; }

        // Store the image as a Base64 encoded string.
        [FirestoreProperty]
        public string ImageData { get; set; }

        // Alternatively, you can store raw bytes:
        // [FirestoreProperty]
        // public byte[] ImageData { get; set; }
    }
}
