# VinylEye

Repository supporting the post [Image Feature Detection and Matching using OpenCvSharp - First Steps Towards VinylEye](https://dev.to/syamaner/image-feature-detection-and-matching-using-opencvsharp-first-steps-towards-vinyleye-gd7)


## Running via Visual Studio / Rider

As long as `launchSettings.json` is configured to pass command line arguments, it is possible to run using the IDEs as usual.

```
{
  "profiles": {
    "VinylEye.Cli": {
      "commandName": "Project",
      "commandLineArgs": "perspective-correct --output-directory images"
    }
  }
}
```

## Running via dotnet cli

To run the application using .Net CLI please use the following within "VinylEye\src\backend\VinylEye.Cli" directory:

`dotnet run -- perspective-correct` to apply perspective correction

`dotnet run -- match` to demo matching positive and negative cases


## Running using Docker

The Dockerfile is updated to use [OPENCV_VERSION=4.7.0](https://github.com/opencv/opencv/releases/tag/4.7.0), [OPENCVSHARP_VERSION=4.7.0.20230114](https://github.com/shimat/opencvsharp/releases/tag/4.7.0.20230114)


The folliwing steps will demonstrate running the perspective correction command without cloning the repository:

- In a suitable location, create a directory called `images` (can be any directory you have permission to of course.)
- Without changing the directory, issue the docker run command mounting the directory you have created below:
  - PowerShell: `docker run -v $PWD/images:/output syamaner/vinyleye:1` (this is the PowerShell version)
  - macOs terminal: `docker run -v $(pwd)/images:/output syamaner/vinyleye:1`
- Now if you check the images directory, you will see the original user image, the training image and the perspective corrected image output.

### Build porocess:

The Dockerfile is built and pusched https://hub.docker.com/repository/docker/syamaner/vinyleye/tags using the following command:

`docker buildx build --push --platform linux/amd64,linux/arm64/v8,linux/arm/v7 --tag syamaner/vinyleye:1 .`


