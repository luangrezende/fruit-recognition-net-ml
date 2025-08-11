# Fruit Recognition ## Quick Start

### Option 1: Download Pre-b## Supported Fruits (Expandable)

- **Apple## Data Organization** - Red, Green, Yellow varieties
- **Bananas** - Various ripeness stages  
- **Oranges** - Multiple sizes and types
- **Custom fruits** - Train with y## Support

- **Bug Reports**: [GitHub Issues](https://github.com/luangrezende/fruit-recognition-net-ml/issues)
- **Feature Requests**: [GitHub Discussions](https://github.com/luangrezende/fruit-recognition-net-ml/discussions)
- **Documentation**: Check the [Wiki](https://github.com/luangrezende/fruit-recognition-net-ml/wiki)wn images

## Configurationease
1. Go to [Releases](https://github.com/luangrezende/fruit-recognition-net-ml/releases)
2. Download the appropriate package for your OS
3. Extract and run following the included README

### Option 2: Build from Source
```bash
git clone https://github.com/luangrezende/fruit-recognition-net-ml.git
cd fruit-recognition-net-ml/src
dotnet restore
dotnet build --configuration Release
```

## Project Structuretatus](https://github.com/luangrezende/fruit-recognition-net-ml/workflows/Fruit%20Recognition%20ML%20-%20CI%2FCD/badge.svg)](https://github.com/luangrezende/fruit-recognition-net-ml/actions)
[![Release](https://img.shields.io/github/v/release/luangrezende/fruit-recognition-net-ml)](https://github.com/luangrezende/fruit-recognition-net-ml/releases)
[![License](https://img.shields.io/github/license/luangrezende/fruit-recognition-net-ml)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)

High-performance GPU-accelerated machine learning system for fruit classification using ML.NET and advanced computer vision.

## Key Features

- **GPU Acceleration** - NVIDIA RTX optimized training with CUDA support  
- **High Accuracy** - 95%+ accuracy achieved on balanced datasets  
- **Flexible Input** - Supports 8 image formats (.jpg, .png, .bmp, .gif, .tiff, .webp)  
- **Smart Loading** - Recursive folder scanning with any directory structure  
- **Cross-Platform** - Windows and Linux support  
- **Production Ready** - Self-contained executables, no dependencies  
- **Batch Processing** - Process multiple images simultaneously  
- **Anti-Overfitting** - Advanced regularization and validation

## � Quick Start

### Option 1: Download Pre-built Release
1. Go to [Releases](https://github.com/luangrezende/fruit-recognition-net-ml/releases)
2. Download the appropriate package for your OS
3. Extract and run following the included README

### Option 2: Build from Source
```bash
git clone https://github.com/luangrezende/fruit-recognition-net-ml.git
cd fruit-recognition-net-ml/src
dotnet restore
dotnet build --configuration Release
```

## 🏗️ Project Structure

```
src/
├── Fruit.Recognition.MachineLearning.Domain/    # Core domain models and services
│   ├── Models/                                  # ML data models
│   │   ├── ImageData.cs                        # Training data structure
│   │   └── ImagePrediction.cs                  # Prediction output model
│   └── Services/                               # Core ML services
├── Fruit.Recognition.MachineLearning.Training/ # Training application
│   ├── Program.cs                              # Training entry point
│   ├── Services/                               # Training orchestration
│   └── appsettings.json                       # Training configuration
└── Fruit.Recognition.MachineLearning.Testing/  # Testing/Prediction application
    ├── Program.cs                              # Testing entry point
    └── appsettings.json                       # Testing configuration
```

## � Supported Fruits (Expandable)

- 🍎 **Apples** - Red, Green, Yellow varieties
- 🍌 **Bananas** - Various ripeness stages  
- 🍊 **Oranges** - Multiple sizes and types
- **Custom fruits** - Train with your own images!

## 🔧 Configuration

### Training Settings (`Fruit.Recognition.MachineLearning.Training/appsettings.json`)
```json
{
  "ModelConfiguration": {
    "ImageWidth": 128,
    "ImageHeight": 128,
    "TestFraction": 0.3,
    "ValidationFraction": 0.2,
    "UseGpu": true,
    "FallbackToCpu": false,
    "Architecture": "Anti-Overfitting-SDCA"
  },
  "PathConfiguration": {
    "DatasetPath": "..\\data\\training",
    "ModelOutputPath": "..\\models\\fruit-recognition-model.zip"
  }
}
```

### Testing Settings (`Fruit.Recognition.MachineLearning.Testing/appsettings.json`)
```json
{
  "PathConfiguration": {
    "ModelPath": "..\\models\\fruit-recognition-model.zip",
    "TestImagesPath": "..\\data\\test"
  }
}
```

## � Data Organization

The system supports flexible folder structures:

```
data/
├── training/
│   ├── apple/
│   │   ├── red_apples/
│   │   │   ├── apple001.jpg
│   │   │   └── apple002.png
│   │   └── green_apples/
│   │       └── green001.bmp
│   ├── banana/
│   │   ├── ripe/
│   │   └── unripe/
│   └── orange/
└── test/
    ├── test_image1.jpg
    └── test_image2.png
```

## Usage

### Training a Model
```bash
cd src/Fruit.Recognition.MachineLearning.Training
dotnet run
```

The training application will:
1. Scan your training data directory recursively
2. Load and validate images from all subdirectories  
3. Train GPU-optimized model with anti-overfitting measures
4. Save trained model for prediction use
5. Display accuracy metrics and performance stats

### Making Predictions
```bash
cd src/Fruit.Recognition.MachineLearning.Testing
dotnet run
```

The testing application will:
1. Load the trained model
2. Process all images in the test directory
3. Classify each fruit with confidence scores
4. Display batch processing results and statistics

## Performance Metrics

**Tested Configuration:**
- **Hardware**: NVIDIA RTX 4090, Intel i9, 32GB RAM
- **Dataset**: 4,637 images (2,392 apples, 2,245 bananas)
- **Training Time**: ~12 seconds
- **Accuracy**: 95.4%
- **Inference Speed**: 394+ images/second

## System Requirements

### Minimum
- Windows 10/11 x64 or Linux x64
- 4GB RAM
- 1GB free disk space
- .NET 8.0 Runtime (included in self-contained builds)

### Recommended for GPU Training
- NVIDIA RTX 3060 or better
- 8GB+ RAM  
- CUDA 12.x toolkit
- SSD storage for faster image loading

## Development

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- NVIDIA CUDA 12.x (for GPU training)

### Build Commands
```bash
# Restore dependencies
dotnet restore src/Fruit.Recognition.MachineLearning.sln

# Build solution
dotnet build src/Fruit.Recognition.MachineLearning.sln --configuration Release

# Run training
dotnet run --project src/Fruit.Recognition.MachineLearning.Training

# Run testing  
dotnet run --project src/Fruit.Recognition.MachineLearning.Testing

# Create release packages
dotnet publish src/Fruit.Recognition.MachineLearning.Training --runtime win-x64 --self-contained
```

## Machine Learning Details

**Algorithm**: Enhanced SDCA (Stochastic Dual Coordinate Ascent) with:
- L1/L2 regularization for overfitting prevention
- Feature normalization for better convergence
- Cross-validation for robust model selection
- GPU-optimized matrix operations

**Features**: 
- RGB pixel extraction at configurable resolution
- Min-max normalization
- Concatenated feature vectors
- Anti-overfitting validation pipeline

## Performance Optimization Tips

1. **More Data = Better Results**: Use 100+ images per fruit type
2. **Balanced Dataset**: Similar quantities for each fruit class
3. **Image Quality**: Clear, well-lit photos work best
4. **Variety**: Different angles, lighting, backgrounds
5. **GPU Usage**: Enable for 10-100x training speedup
6. **Resolution**: Start with 128x128, increase if needed

## Troubleshooting

### GPU Issues
```bash
# Check CUDA installation
nvcc --version
nvidia-smi

# Enable CPU fallback if needed
"FallbackToCpu": true
```

### Low Accuracy
- Add more training images
- Balance dataset between classes
- Check for mislabeled images
- Increase image resolution
- Add data augmentation

### Linux Permissions
```bash
chmod +x Fruit.Recognition.MachineLearning.Training
chmod +x Fruit.Recognition.MachineLearning.Testing
```

## CI/CD Pipeline

This project uses GitHub Actions for automated:
- **Continuous Integration** - Build and test on every push
- **Cross-Platform Builds** - Windows, Linux, macOS
- **Automated Releases** - Self-contained executables
- **Documentation Updates** - Auto-generated build reports

### Triggering a Release
```bash
# Create and push a version tag
git tag v1.0.0
git push origin v1.0.0
```

Or use the manual workflow dispatch in GitHub Actions.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## � Support

- **🐛 Bug Reports**: [GitHub Issues](https://github.com/luangrezende/fruit-recognition-net-ml/issues)
- **💡 Feature Requests**: [GitHub Discussions](https://github.com/luangrezende/fruit-recognition-net-ml/discussions)
- **📖 Documentation**: Check the [Wiki](https://github.com/luangrezende/fruit-recognition-net-ml/wiki)

## Acknowledgments

- **ML.NET Team** - For the amazing machine learning framework
- **Microsoft** - For .NET 8 and cross-platform support  
- **NVIDIA** - For CUDA and GPU acceleration capabilities
- **Community** - For testing and feedback

---

**Built with ML.NET 4.0 and .NET 8**

*If this project helped you, please consider giving it a star!*
