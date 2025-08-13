# Fruit Recognition ML - Setup Guide

## Quick Setup

### 1. Create Required Folders
```
data/
├── training/    # Your training images
│   ├── apple/   # Create folders for each fruit type
│   ├── banana/
│   └── orange/
└── test/        # Images for prediction
models/          # Trained models (auto-created)
```

### 2. Add Your Images
- Place training images in `data/training/[fruit-name]/`
- Place test images in `data/test/`
- Supported formats: .jpg, .jpeg, .png, .bmp, .gif, .tiff, .webp

### 3. Train Your Model
```bash
# Double-click or run from command line
./Fruit.Recognition.MachineLearning.Training.exe
```

### 4. Test Predictions
```bash
# Double-click or run from command line  
./Fruit.Recognition.MachineLearning.Testing.exe
```

## Configuration

Edit `appsettings.json` to customize:
- Image resolution
- GPU settings
- Training parameters

## System Requirements
- Windows 10/11 x64 or Linux x64
- 4GB RAM minimum
- NVIDIA GPU with CUDA 12.x (optional)

## Support
- Issues: https://github.com/luangrezende/fruit-recognition-net-ml/issues
- Documentation: https://github.com/luangrezende/fruit-recognition-net-ml
