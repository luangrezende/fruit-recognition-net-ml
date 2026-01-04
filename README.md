# Fruit Recognition ML

![Build](https://img.shields.io/github/actions/workflow/status/luangrezende/fruit-recognition-net-ml/ci-cd.yml)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![ML.NET](https://img.shields.io/badge/ML.NET-4.0-FF6F00)
![CUDA](https://img.shields.io/badge/CUDA-12.x-76B900?logo=nvidia)
![License](https://img.shields.io/badge/License-MIT-green)

A **GPU-accelerated image classification system** built with **.NET 8 and ML.NET**, designed for **high-throughput training and inference** using structured datasets.

The project focuses on **performance, reproducibility, and clean separation between training and inference**, avoiding experimental abstractions and unnecessary framework complexity.

---

## Overview

This repository contains a complete pipeline for **image-based classification**, covering:
- Dataset ingestion
- Model training
- Evaluation
- Batch inference

The solution is split into **independent executables** for training and testing, allowing deterministic runs and simplified deployment.

The implementation targets **practical ML workloads** rather than academic experimentation.

---

## Key Characteristics

- GPU-accelerated training and inference (CUDA)
- ML.NET 4.0 with SDCA-based classification
- Deterministic, repeatable runs via JSON configuration
- No runtime dependencies beyond the generated binaries
- Cross-platform support (Windows / Linux)

---

## Project Structure

```
fruit-recognition-net-ml/
├── src/
│   ├── Fruit.Recognition.MachineLearning.Domain/     # ML domain and data models
│   ├── Fruit.Recognition.MachineLearning.Training/   # Training application
│   ├── Fruit.Recognition.MachineLearning.Testing/    # Inference application
│   ├── data/
│   │   ├── training/
│   │   └── test/
│   └── models/                                       # Trained model artifacts
└── .github/                                          # CI/CD workflows
```

---

## Dataset Model

- Images are loaded recursively from directories
- Folder names define class labels
- Dataset structure is not fixed
- Multiple image formats supported

This allows the same pipeline to be reused for **custom classification problems** without code changes.

---

## Configuration

All runtime behavior is driven by **JSON configuration files**.

### Training Configuration (Example)

```json
{
  "ModelConfiguration": {
    "ImageWidth": 128,
    "ImageHeight": 128,
    "TestFraction": 0.3,
    "ValidationFraction": 0.2,
    "UseGpu": true,
    "FallbackToCpu": false
  },
  "PathConfiguration": {
    "DatasetPath": "../data/training",
    "ModelOutputPath": "../models/model.zip"
  }
}
```

No parameters are hardcoded; all training and evaluation behavior is externally configurable.

---

## Usage

### Training

```bash
dotnet run --project src/Fruit.Recognition.MachineLearning.Training
```

This process:
- Scans the dataset
- Trains a GPU-backed classification model
- Outputs evaluation metrics
- Persists the trained model

---

### Inference

```bash
dotnet run --project src/Fruit.Recognition.MachineLearning.Testing
```

This process:
- Loads a trained model
- Runs batch predictions on test images
- Outputs classification results with confidence scores

---

## Performance (Reference Hardware)

| Metric | Value |
|------|------|
| Accuracy | ~95% |
| Training time | ~12s |
| Inference throughput | ~390 images/sec |
| Dataset size | ~4.6k images |
| GPU | RTX 4090 |

Performance varies based on hardware, dataset quality, and configuration.

---

## Build & CI

The repository uses **GitHub Actions** for:
- Continuous integration
- Cross-platform builds
- Release packaging

Releases are versioned via Git tags.

---

## Scope and Intent

This project is not intended as:
- A general-purpose AutoML framework
- A deep learning research platform
- A plug-and-play production ML service

It is intended as:
- A high-performance ML.NET reference
- A reproducible training/inference pipeline
- A practical example of GPU-backed ML in .NET

---

## License

MIT License.
