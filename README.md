# 🍎 Fruit Recognition - ML.NET Image Classification

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![ML.NET](https://img.shields.io/badge/ML.NET-4.0-FF6F00?style=for-the-badge&logo=microsoft)](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
[![SDCA](https://img.shields.io/badge/Algorithm-SDCA-00D4AA?style=for-the-badge)](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen?style=for-the-badge&logo=github-actions)](.)

A fruit recognition system using **ML.NET Image Classification** with **SDCA (Stochastic Dual Coordinate Ascent)** for reliable cross-platform performance.

## 🚀 Features

- **SDCA Algorithm**: Fast and reliable multiclass classification algorithm
- **Image Processing**: Advanced pixel extraction and resizing pipeline
- **Cross-Platform**: Works on Windows, macOS, and Linux without native dependencies
- **High Performance**: Optimized for speed and accuracy
- **Clean Architecture**: Clear separation between Core, Training and Prediction
- **Advanced Logging**: Detailed training process logs
- **Flexible Configuration**: Adjustable parameters via appsettings.json

## 📁 Project Structure

```
src/
├── FruitRecognition.Core/           # Core library
│   ├── Models/                      # Data models
│   ├── Services/                    # Business services
│   └── Configuration/               # Configurations
├── FruitRecognition.Training/       # Training application
└── FruitRecognition.Prediction/     # Prediction application
```

## 🛠️ Technologies Used

- **.NET 8.0**
- **ML.NET 4.0** with ImageAnalytics
- **SDCA Algorithm** for multiclass classification
- **Microsoft.Extensions** (DI, Logging, Configuration)

## 📋 Prerequisites

- .NET 8.0 SDK
- Windows, macOS or Linux
- Minimum 8GB RAM (recommended: 16GB+)
- Training images organized by fruit categories (minimum 20 per category)

## 🚀 How to Use

### 1. Clone the Repository
```bash
git clone https://github.com/luangrezende/fruit-recognition-net-ml.git
cd fruit-recognition-net-ml
```

### 2. Prepare the Dataset
Create the dataset structure and add fruit images:
```
data/training/
├── apple/       (add 20+ apple images)
│   ├── apple1.jpg
│   └── apple2.jpg
├── banana/      (add 20+ banana images)
│   ├── banana1.jpg
│   └── banana2.jpg
└── orange/      (add 20+ orange images)
    ├── orange1.jpg
    └── orange2.jpg
```

### 3. Build the Project
```bash
cd src
dotnet build
```

### 4. Train the Model
```bash
cd FruitRecognition.Training
dotnet run
```

### 5. Make Predictions
```bash
cd ../FruitRecognition.Prediction
dotnet run -- "../../data/test/fruit.jpg"
```

## ⚙️ SDCA Configuration

Edit `src/FruitRecognition.Training/appsettings.json`:

```json
{
  "ModelConfiguration": {
    "ImageWidth": 224,
    "ImageHeight": 224,
    "TreesCount": 500,
    "LeavesCount": 20,
    "LearningRate": 0.1,
    "TestFraction": 0.2,
    "ValidationFraction": 0.1,
    "FeatureExtractor": "ImagePixels"
  },
  "PathConfiguration": {
    "DatasetPath": "..\\..\\data\\training",
    "ModelPath": "..\\..\\models\\fruit-recognition-model.zip"
  }
}
```

## 📊 Expected Performance

With a well-balanced dataset, SDCA typically achieves:

- **Micro Accuracy**: 85-95%
- **Training Time**: 1-5 minutes (depending on dataset)
- **Model Size**: ~1-10MB (much smaller than deep learning models)
- **Cross-Platform**: Works on any .NET 8 supported platform

## 🔧 Troubleshooting

### Low Accuracy
- Increase the number of images per category (minimum: 20, recommended: 50+)
- Ensure image quality and variety
- Balance the dataset (similar number of images per fruit)
- Adjust `TreesCount` and `LeavesCount` in configuration

### Slow Performance
- Reduce image size (`ImageWidth` and `ImageHeight`)
- Use fewer training images initially for testing
- Ensure sufficient RAM is available

### Build Issues
- Make sure you have .NET 8.0 SDK installed
- Run `dotnet restore` in the src directory
- Check that all NuGet packages are restored properly

## 📚 Additional Documentation

- [ML.NET Documentation](https://docs.microsoft.com/en-us/dotnet/machine-learning/)
- [ResNet Paper](https://arxiv.org/abs/1603.05027)

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is under the MIT License. See the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Luan Rezende**
- GitHub: [@luangrezende](https://github.com/luangrezende)

---

⭐ If this project helped you, consider giving it a star!

## 🔄 Changelog

### v2.1.0 - SDCA Implementation
- Migration from ResNetV2101 to SDCA algorithm for better compatibility
- Removed TensorFlow dependencies (cross-platform friendly)
- Faster training and smaller model size
- Maintained clean architecture and professional logging

### v2.0.0 - ResNetV2101 Implementation (Legacy)
- Migration to ResNetV2101 with transfer learning
- Completely rewritten architecture
- Significant accuracy improvement
- Flexible JSON configuration
- Advanced logging with detailed metrics

### v1.0.0 - Initial Release
- Initial implementation with SDCA
- Basic project structure
