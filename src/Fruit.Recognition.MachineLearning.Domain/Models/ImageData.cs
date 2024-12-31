using Microsoft.ML.Data;

namespace Fruit.Recognition.MachineLearning.Domain.Models
{
    public class ImageData
    {
        [ColumnName("ImagePath")]
        public string ImagePath { get; set; }

        [ColumnName("Label")]
        public string Label { get; set; }
    }
}
