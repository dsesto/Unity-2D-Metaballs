# 2D Metaballs in Unity

This is an experimental project I worked on in order to learn more about Metaballs and Unity Meshes.

It generates multiple 2D Metaballs that move around a closed space, being rendered with a procedural [Unity Mesh](https://docs.unity3d.com/ScriptReference/Mesh.html). The metaball effect is achieved using the Marching Squares algorithm, based on the work of [Jamie Wong](http://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/) in this article. I also used [Luke Holland](https://github.com/luke161/Unity-Metaballs-2D)'s Unity Project as a starting point, although our implementations differ in several structural aspects.

## Usage

Built with Unity 2019.4; in order to use the project, open it in Unity, load [Scenes/Main](master/Assets/Scenes/Main.unity) and hit the *Play* button. There are several options that you can customize out-of-the-box from the Inspector, under the *MetaballGenerator* script of the *MetaballGenerator* GameObject in the hierarchy:
* **Num Metaballs:** the amount of Metaballs that will be used for the experiment.
* **Grid Resolution:** the number of samples to be used for the Marching Squares algorithm.

![](master/RepositoryResources/Recordings/Low_Resolution_No_Smooth.gif)

* **Boundaries:** the size of the experiment space.
* **Metaball Speed**: the speed at which the Metaballs will move. It includes a small random offset so that not all balls move at the same speed.
* **Metaball Radius**: the size (radius) of each Metaball. It also includes a random offset to obtain balls of multiple sizes.
* **Smooth:** a toggle to determine whether to apply the interpolation phase of Jamie Wong's algorithm.

![](master/RepositoryResources/Recordings/Low_Resolution_Smooth.gif)

![](master/RepositoryResources/Recordings/High_Resolution.gif)

* **Include Rectangle:"** a toggle to determine if you want to include an additional rectangle to the experiment. I developed myself a "Metarectangle" that integrates with the experiment and merges with the Metaballs. When toggled, it will be added to the mesh, and you can move it around dragging it in the Unity *Scene* view.

![](master/RepositoryResources/Recordings/Rectangle.gif)

## About

I hope this project is as helpful to you as it has been to me. I've learned a lot both from Jamie and Luke's work, and I've added my own touch thinking on my next project.

Feel free to follow my indie videogames developer account on Twitter ([@digging_dinos](https://twitter.com/digging_dinos)), and check out our games in the [Play Store](https://play.google.com/store/apps/developer?id=Digging+Dinosaurs).