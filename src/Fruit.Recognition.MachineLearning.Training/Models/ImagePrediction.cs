using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fruit.Recognition.MachineLearning.Models
{
    public class ImagePrediction
    {
        public string PredictedLabel { get; set; }

        public float[] Score { get; set; }
    }
}
