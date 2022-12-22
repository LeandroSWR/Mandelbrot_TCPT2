# Mandelbrot_TCPT2

TCP assignment to parallelize the Mandelbrot set on the CPU.

## Author

- Leandro Brás a22100770

## Summary

This project was created as the second assignment for the class of
`Parallel Computing Techniques` with the objective of being a comprehensive
implementation of the Mandelbrot Set in C#, with a focus on efficient rendering
and the ability to display statistics about the rendering process.

The user has the option of running a CPU based Mandelbrot calculator to
determine the advantages of parallel computing when calculating the fractal
set, or as a bonus the user can also run an interactive view of the Mandelbrot
Set where it's possible to zoom in/out and change the number of iterations
dynamically.

## Instructions

**Note:** This project can only be used on `Windows`.

### Menu Options

- Run Benchmark:
  - Runs a calculation of the set 3 times using Linear and Parallel methods and
display the averaged result.
- Parallel Only:
  - Run only the Parallel calculation method and display the final image.
- Linear Only:
  - Run only the Linear calculation method and display the final image.
- GPU Mandelbrot:
  - Opens a new OpenGL window that displays the Mandelbrot set at interactive
rates and allows the user to move around, zoom and change the number of
iterations.
- Check Stats
  - Shows the averaged results of all calculations done so far.

### OpenGL Mandelbrot Controlls

- **Click** anywhere on the screen with `Middle Mouse Button` and **drag** the
`Mouse` to **move** around;
- Use the `Mouse Wheel` to **zoom in / out** on the `Mouse`'s current position;
- **Hold** `Shift + Mouse Wheel` to **increase/decrease** the number of
**iterations**;

## Architecture Description

### Resume

The file `Mandelbrot.cs`, contains a class called `Mandelbrot` that functions
like a brain that is responsible for generating the Mandelbrot Set image. This
class has several methods for rendering the image, including
`RenderMandelbrotSet`, `RenderMandelbrotSetParallel`, and
`RenderMandelbrotSetShader`.

The `MandelbrotCalc` and `MandelbrotCalcParallel` classes are used to calculate
the `Mandelbrot Set`. The `MandelbrotCalc` class calculates the Mandelbrot Set
sequentially, while the `MandelbrotCalcParallel` class calculates the
Mandelbrot Set in parallel using the `Parallel.For` method.

The `MandelbrotStats` class is used to collect statistics about the rendering
process, such as the time it takes to render the image and the number of
iterations that were performed.

The `Menu` class is responsible for displaying a menu when the program starts
allowing the user to select different options, such as the type of rendering
method to use.

The `Program` class is the entry point for the program and is responsible for
creating an instance of the `Menu` class, displaying it to the user.

The `StatsDisplay` class is used to display the statistics collected by the
MandelbrotStats class.

The most important class for this project is the MandelbrotCalc.

## MandelbrotCalc class

The `MandelbrotCalc` class is a base class for generating the Mandelbrot Set
image. It provides a framework for performing the calculations necessary to
determine whether each point in the complex plane is inside or outside of the
Mandelbrot Set, and for mapping the resulting iteration values to colors.

In this class you'll notice the `Calculate` method is `abstract` this was done
so we could have all the functionality in this base class and override just
the calculation for when we need to calculate the set in Parallel.

### Properties

- `width`: An `int` representing the width of the image in pixels.
- `height`: An `int` representing the height of the image in pixels.
- `maxIterations`: An `int` representing the maximum number of iterations to
perform when determining whether a point is inside or outside of the Mandelbrot
Set.
- `scale`: A `float` representing the scale of the image.
- `xMin`: A `float` representing the minimum value for the real component of the
complex plane.
- `xMax`: A `float` representing the maximum value for the real component of the
complex plane.
- `yMin`: A `float` representing the minimum value for the imaginary component of
the complex plane.
- `yMax`: A `float` representing the maximum value for the imaginary component of
the complex plane.

### Methods

#### MandelbrotCalc

```csharp
public MandelbrotCalc(int width, int height, int maxIterations, float scale)
```

This is a constructor that initializes the `width`, `height`, `maxIterations`,
and `scale` properties with the provided values.

#### RenderMandelbrotSet

```csharp
public void RenderMandelbrotSet(ref long time, ref Bitmap bitmap)
```

This method generates the Mandelbrot Set image and stores the elapsed time it
takes to do so in the `time` parameter. It takes a reference to a `long`
variable and a reference to a `Bitmap` object as input, and sets the pixels of
the Bitmap object to the calculated colors.

It begins by starting a new `Stopwatch` object to measure the time it
takes to calculate the set. It then calls the `Calculate` method to get an
array of iteration values for each point in the complex plane.

After the iteration values have been calculated, it stores the elapsed
time in the `time` parameter and stops the `Stopwatch` object. It then iterates
over each pixel in the `Bitmap` object and sets its color based on the
corresponding iteration value from the iteration array. The color is then
determined by the `MapColor` method. Finally, the current pixel of the `Bitmap`
object is set to the calculated color.

#### MapColor

```csharp
private Color MapColor(float iter)
```

This method maps an iteration value to a color. It takes a `float` value as
input that represents a value that was calculated from the total number of
iterations and returns a `Color` object.

It uses the cosine function and some constants to create a color gradient based
on the input iteration value. It calculates the red, green, and blue values of
the color by applying the cosine function to the iteration value and then
scaling and clamping the result to the range 0-255.

#### Calculate

```csharp
internal virtual float[,] Calculate()
```

This is an abstract method that can be overridden in derived classes. It
was built this way to avoid code duplication between the Linear and Parallel
calculations. It performs the calculations necessary to determine whether each
point in the complex plane is inside or outside of the Mandelbrot Set, and
returns an array of iteration values for each point.

It first initializes an array `iterations` to hold a value that was calculated
from the total number of iterations it took for each point in the set to
diverge, or 0 if it did not diverge within a given number of maximum
iterations. It then centers the set by adjusting the values of `xMax`,
`xMin`, `yMax`, and `yMin` based on the `scale`.

Next, it calculates the width and height of each pixel in the complex plane
using the range of complex numbers to be plotted and the width and height of
the image. It then uses a `For` loop to iterate over each pixel in the image.

For each pixel, it calculates the complex number `c` corresponding to the
pixel's position and initializes the real and imaginary parts of the `z` complex
number to 0. It then enters a loop that calculates the new value of `z` using the
formula `z = z^2 + c` and updates the values of `zReal` and `zImaginary` with the
new values. The loop continues until either the magnitude of `z` becomes greater
than 4 (which means the function has diverged) or the number of iterations
exceeds `maxIterations`.

If the square magnitude of `z` is greater than 4, the method calculates the number
of iterations it took for the function to diverge based on the squared magnitude
of `z` times a constant and sets the value of `iterations[x, y]` to this value. If
the magnitude of z is not greater than 4, the function did not diverge within
the given number of iterations and the value of `iterations[x, y]` is set to 0.

Finally, the method returns the `iterations` array.

The implementation of this method in the `MandelCalcParallel` class is exactly
the same but the `Parallel.For` method is used in the 1st loop `For` loop.

## Results

**Note:** For the best results run the project in `Release` mode.

The tests where done on my own computer with the following specs:

- MSI RTX 4090 Gaming X Trio
- Ryzen 9 5900X (12core 24thread)
- 32Gb (4x8Gb) 3600Mhz Cl16

These results where obtain by running the benchmark option wir a resolution of
4096x4096.

*(Note that these results will be different depending on the CPU
and the total number of cores)*.

### Average Time Table

In Milliseconds:

| Size      | Iterations |    Linear    |   Parallel   |
| :-------: | :--------: | :----------: | :----------: |
| 4096x4096 |    1000    |  22940.33ms  |  1858.00ms   |

In Seconds:

| Size      | Iterations |    Linear    |   Parallel   |
| :-------: | :--------: | :----------: | :----------: |
| 4096x4096 |    1000    |   22.94sec   |   1.86sec    |

From these results we can see there's a massive increase in performance,
being `12.35x` faster when calculating in `Parallel`. If we also take a quick
look at the GPU side of things we can see the same image now runs at an average
of `142fps` which means it's doing the same calculations in just `7.04ms` or
`3259x` faster than the Linear CPU calculations and `264x` faster than the
Parallel CPU calculations! Of curse lets not forget that CPUs and GPU function
in a very different way.

## Thanks

I'd like to thank Professor José Rogado for all the help and all the excellent
classes we've been having since the start of the semester! Parallel computing
has always been an interesting topic for me and i can't wait to dive a bit
more into GPU computing for the next assignment.
