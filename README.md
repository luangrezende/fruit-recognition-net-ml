<div align="center">

# 🍎 Fruit Recognition ML

[![Build Status](https://img.shields.io/github/actions/workflow/status/luangrezende/fruit-recognition-net-ml/ci-cd.yml?style=for-the-badge&logo=github&cacheSeconds=300)](https://github.com/luangrezende/fruit-recognition-net-ml/actions)
[![Release](https://img.shields.io/github/v/release/luangrezende/fruit-recognition-net-ml?style=for-the-badge&logo=github&color=green&cacheSeconds=300)](https://github.com/luangrezende/fruit-recognition-net-ml/releases)
[![Downloads](https://img.shields.io/github/downloads/luangrezende/fruit-recognition-net-ml/total?style=for-the-badge&logo=github&color=brightgreen&cacheSeconds=300)](https://github.com/luangrezende/fruit-recognition-net-ml/releases)

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ML.NET](https://img.shields.io/badge/ML.NET-4.0-FF6F00?style=for-the-badge&logo=microsoft)](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet)
[![GPU](https://img.shields.io/badge/GPU-CUDA%2012.x-76B900?style=for-the-badge&logo=nvidia)](https://developer.nvidia.com/cuda-downloads)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-lightgrey?style=for-the-badge)](https://github.com/luangrezende/fruit-recognition-net-ml)

**High-performance GPU-accelerated machine learning for fruit classification**

[Features](#-features) •
[Quick Start](#-quick-start) •
[Documentation](#-documentation) •
[Performance](#-performance) •
[Contributing](#-contributing)

</div>

## Features

<table>
<tr>
<td width="50%">

### Performance
- **95%+ accuracy** on balanced datasets
- **GPU acceleration** with CUDA support
- **Fast inference** - 394+ images/second
- **12-second training** on RTX 4090

### Technical
- **ML.NET 4.0** with SDCA algorithm
- **Anti-overfitting** measures
- **Cross-platform** (Windows/Linux)
- **Self-contained** executables

</td>
<td width="50%">

### Flexible Data
- **8 image formats** supported
- **Recursive folder** scanning
- **Any directory structure**
- **Automatic class** detection

### Easy to Use
- **No dependencies** required
- **JSON configuration**
- **Batch processing**
- **Professional logging**

</td>
</tr>
</table>

## Quick Start

### Option 1: Pre-built Releases
```bash
# 1. Download from releases
curl -L https://github.com/luangrezende/fruit-recognition-net-ml/releases/download/v1.0.0/fruit-recognition-training-win-x64.zip

# 2. Extract and run
./fruit-recognition-training    # Train your model
./fruit-recognition-testing     # Make predictions
```

### Option 2: Build from Source
```bash
git clone https://github.com/luangrezende/fruit-recognition-net-ml.git
cd fruit-recognition-net-ml/src
dotnet restore
dotnet build --configuration Release
```

## Project Structure

```
fruit-recognition-net-ml/
├── 📂 src/
│   ├── Fruit.Recognition.MachineLearning.Domain/     # Core ML models
│   ├── Fruit.Recognition.MachineLearning.Training/   # Training app
│   ├── Fruit.Recognition.MachineLearning.Testing/    # Testing app
│   ├── data/
│   │   ├── training/    # Your training images
│   │   └── test/        # Images for testing
│   └── models/       # Trained ML models
└── .github/         # CI/CD workflows
```

## Supported Fruits

| Fruit | Status | Notes |
|-------|--------|--------|
| 🍎 **Apples** | ✅ Built-in | Red, Green, Yellow varieties |
| 🍌 **Bananas** | ✅ Built-in | Various ripeness stages |
| 🍊 **Oranges** | ✅ Built-in | Multiple sizes and types |
| 🍇 **Custom** | ✅ Flexible | Add any fruit by creating folders |

## Configuration

<details>
<summary><strong>Training Configuration</strong> (click to expand)</summary>

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

</details>

<details>
<summary><strong>Testing Configuration</strong> (click to expand)</summary>

```json
{
  "PathConfiguration": {
    "ModelPath": "..\\models\\fruit-recognition-model.zip",
    "TestImagesPath": "..\\data\\test"
  }
}
```

</details>

## Usage

### Training a Model
```bash
cd src/Fruit.Recognition.MachineLearning.Training
dotnet run
```

**What happens:**
1.  Scans training directory recursively
2.  Loads and validates all images
3.  Trains GPU-optimized model
4.  Saves model for predictions
5.  Shows accuracy metrics

### Making Predictions
```bash
cd src/Fruit.Recognition.MachineLearning.Testing
dotnet run
```

**What happens:**
1.  Loads the trained model
2.  Processes test images
3.  Classifies with confidence scores
4.  Displays batch results

## Performance

| Metric | Value |
|--------|-------|
| **Accuracy** | 95.4% |
| **Training Time** | ~12 seconds |
| **Inference Speed** | 394+ images/sec |
| **Dataset Size** | 4,637 images |
| **Hardware** | RTX 4090, 32GB RAM |

## System Requirements

### Minimum
- Windows 10/11 x64 or Linux x64
- 4GB RAM
- 1GB free disk space

### Recommended (GPU Training)
- NVIDIA RTX 3060+
- 8GB+ RAM
- CUDA 12.x toolkit
- SSD storage

## Development

```bash
# Restore dependencies
dotnet restore src/Fruit.Recognition.MachineLearning.sln

# Build solution
dotnet build src/Fruit.Recognition.MachineLearning.sln --configuration Release

# Run training
dotnet run --project src/Fruit.Recognition.MachineLearning.Training

# Run testing
dotnet run --project src/Fruit.Recognition.MachineLearning.Testing
```

## Troubleshooting

<details>
<summary><strong>GPU Issues</strong></summary>

```bash
# Check CUDA installation
nvcc --version
nvidia-smi

# Enable CPU fallback
"FallbackToCpu": true
```

</details>

<details>
<summary><strong>Low Accuracy</strong></summary>

- Add more training images (100+ per fruit)
- Balance dataset between classes
- Increase image resolution
- Check for mislabeled images

</details>

## CI/CD Pipeline

This project uses **GitHub Actions** for:
-  Continuous Integration
-  Cross-platform builds
-  Automated releases
-  Documentation updates

### Create a Release
```bash
git tag v1.0.0
git push origin v1.0.0
```

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Contributing

1. **Fork** the repository
2. **Create** your feature branch
3. **Commit** your changes
4. **Push** to the branch
5. **Open** a Pull Request

## Support

<div align="center">

[![Issues](https://img.shields.io/badge/Issues-GitHub-red?style=for-the-badge&logo=github)](https://github.com/luangrezende/fruit-recognition-net-ml/issues)
[![Discussions](https://img.shields.io/badge/Discussions-GitHub-blue?style=for-the-badge&logo=github)](https://github.com/luangrezende/fruit-recognition-net-ml/discussions)
[![Wiki](https://img.shields.io/badge/Wiki-Documentation-green?style=for-the-badge&logo=github)](https://github.com/luangrezende/fruit-recognition-net-ml/wiki)

</div>

---

<div align="center">

**Built with ❤️ using ML.NET 4.0 and .NET 8**

⭐ **If this project helped you, please consider giving it a star!**

</div>
