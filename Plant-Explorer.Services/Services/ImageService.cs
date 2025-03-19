using Google.Cloud.Firestore;
using Plant_Explorer.Contract.Repositories.ModelViews.ImageModel;
using Plant_Explorer.Contract.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.Services
{
    public class ImageService : IImageService
    {
        private readonly FirestoreDb _firestoreDb;

        public ImageService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<string> UploadImageAsync(string imagePath)
        {
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
            string base64Image = Convert.ToBase64String(imageBytes);

            ImageRecord record = new ImageRecord
            {
                Name = Path.GetFileName(imagePath),
                ImageData = base64Image
            };

            CollectionReference imagesCollection = _firestoreDb.Collection("images");
            DocumentReference docRef = imagesCollection.Document();
            await docRef.SetAsync(record);

            // Return the generated document ID.
            return docRef.Id;
        }

        public async Task<ImageRecord> GetImageAsync(string documentId)
        {
            DocumentReference docRef = _firestoreDb.Collection("images").Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                ImageRecord record = snapshot.ConvertTo<ImageRecord>();
                return record;
            }
            throw new Exception("Image not found");
        }
    }

}
