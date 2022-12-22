// To use this shader you'll need an Nvidia GPU
#version 400 core
#extension GL_NV_gpu_shader_fp64 : enable
// Disable precision optimizations for better zoom results
#pragma optionNV(fastmath off)
#pragma optionNV(fastprecision off)

// Uniform variables are values that are passed to the shader from the calling program
// `windowAspect` is the aspect ratio of the window
// it is used to convert from screen coordinates to the actual coordinates of the Mandelbrot set
uniform double windowAspect;

// `scale` is used to zoom in and out of the Mandelbrot set
uniform float scale;

// `maxIterations` is the maximum number of iterations to run the Mandelbrot calculation for each pixel
uniform int maxIterations;

// `centerX` and `centerY` specify the center of the viewport in the Mandelbrot coordinate system
uniform double centerX;
uniform double centerY;

// `pixelPosition` is a varying variable, which means it will have different values for each pixel
varying vec2 pixelPosition;

// `fragColor` is the output variable for the shader
// it specifies the final color of the pixel
out vec4 fragColor;

// Map the iteration count value to a color
vec4 mapColor(float mcol)
{
	// Use a combination of cosine and linear functions to generate a color
	// It goes something like this:
	// Vec4(Gama + Contrast * Cos(Tone + mcol * brightness + RGB), Alpha
	return vec4(0.5 + 0.5 * cos(2.5 + mcol * 30.0 + vec3(1.0, 0.5, 0.0)), 1.0);
}

// Perform the complex multiplication of the two 2D vectors
dvec2 complexMult(dvec2 a, dvec2 b)
{
	return dvec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
}

// Iterate the Mandelbrot calculation for a given coordinate
float iterateMandelbrot(dvec2 coord)
{
	// `currentPoint` is the current coordinate being processed
	dvec2 currentPoint = dvec2(0, 0);

	// Repeate the calculations until we reach `maxIterations`
	for (int i = 0; i < maxIterations; i++)
	{
		// Update `currentPoint` using the Mandelbrot equation: currentPoint = currentPoint^2 + coord
		currentPoint = complexMult(currentPoint, currentPoint) + coord;

		// `ndot` is the squared magnitude of currentPoint
		double ndot = dot(currentPoint, currentPoint);

		// If `ndot` exceeds 7, the point is considered to be outside the Mandelbrot set
		if (ndot > 7.0)
		{
			// Apply a logarithmic smoothing formula to create a more visually pleasing result
			float sl = i - log2(log2(float(ndot))) + 4.0;

			// return the scaled color value 
			return sl * 0.0025;
		}
	}

	// If it's inside the set return 0.0 (black)
	return 0.0;
}

void main()
{
	// `fragment` holds the coordinate of the current pixel in the Mandelbrot coordinate system
	dvec2 fragment = dvec2(
		windowAspect * pixelPosition.x * scale + centerX,
		pixelPosition.y * scale + centerY
	);

	// Get the fragColor
	fragColor = mapColor(iterateMandelbrot(fragment));
}