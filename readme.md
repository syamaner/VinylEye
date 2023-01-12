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
Alternatively the demo can be run using Docker as following:

The Dockerfile is built using the following command:

`docker buildx build --push --platform linux/amd64,linux/arm64/v8,linux/arm/v7 --tag syamaner/vinyleye:0 .`

then run as from within  "VinylEye\src\backend\VinylEye.Cli" directory:

`docker run -v $PWD/images:/output:0`

Above command will trigger the match task and output the images to  `$PWD/images` directory which is  "VinylEye\src\backend\VinylEye.Cli\images" directory.

 