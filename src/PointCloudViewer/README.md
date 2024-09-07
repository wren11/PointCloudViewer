# Point Cloud Viewer with Sphere-like Points

## Overview

This project is a 3D point cloud viewer implemented using C# and the Silk.NET library. It visualizes a set of 3D points, rendering them as sphere-like points in a window. The application supports basic camera controls, including rotation, panning, and zooming, to navigate through the point cloud.

## Features

- **3D Point Cloud Visualization**: Renders a set of 3D points as sphere-like points.
- **Camera Controls**: Supports rotation, panning, and zooming using mouse interactions.
- **Octree Partitioning**: Efficiently partitions the point cloud data using an octree structure for optimized rendering.
- **Shader-based Rendering**: Utilizes vertex and fragment shaders for rendering points with a smooth circle effect.

## Dependencies

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Silk.NET](https://github.com/dotnet/Silk.NET) (Input, Maths, OpenGL, Windowing)

## Project Structure

- **Program.cs**: The main entry point of the application. Initializes the window, sets up event handlers, and manages the rendering loop.
  ```csharp:Program.cs
  startLine: 1
  endLine: 320
  ```

- **PointCloudData.cs**: Represents the point cloud data, including a list of points and a bounding box.
  ```csharp:PointCloudData.cs
  startLine: 1
  endLine: 32
  ```

- **BoundingBox.cs**: Defines a bounding box structure used for spatial partitioning.
  ```csharp:BoundingBox.cs
  startLine: 1
  endLine: 7
  ```

- **Shader.cs**: Manages the creation, compilation, and usage of OpenGL shaders.
  ```csharp:Shader.cs
  startLine: 1
  endLine: 44
  ```

- **Camera.cs**: Handles the camera's view and projection matrices, and frustum culling.
  ```csharp:Camera.cs
  startLine: 1
  endLine: 49
  ```

- **Point3D.cs**: Defines a 3D point structure.
  ```csharp:Point3D.cs
  startLine: 1
  endLine: 8
  ```

- **OctreeNode.cs**: Implements an octree node for spatial partitioning of the point cloud.
  ```csharp:OctreeNode.cs
  startLine: 1
  endLine: 60
  ```

- **Plane.cs**: Represents a plane in 3D space, used for frustum culling.
  ```csharp:Plane.cs
  startLine: 1
  endLine: 26
  ```

## Usage

1. **Build the Project**: Ensure you have .NET 8.0 installed. Navigate to the project directory and run:
   ```sh
   dotnet build
   ```

2. **Run the Application**: After building, run the application using:
   ```sh
   dotnet run
   ```

3. **Interact with the Viewer**:
   - **Rotate**: Click and drag with the left mouse button.
   - **Pan**: Click and drag with the right mouse button.
   - **Zoom**: Scroll the mouse wheel.

## Future Enhancements

- **Point Cloud Loading**: Add functionality to load point cloud data from external files.
- **Advanced Shading**: Implement more advanced shading techniques for better visualization.
- **Performance Optimization**: Further optimize the rendering and partitioning algorithms for larger datasets.
