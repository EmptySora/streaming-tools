/**
 * @file Produces an animation that vaguely resembles rain falling upwards.
 * @author EmptySora_
 * @version 1.0.0.0
 * @license CC-BY 4.0
 * This work is licensed under the Creative Commons Attribution 4.0
 * International License. To view a copy of this license, visit
 * http://creativecommons.org/licenses/by/4.0/ or send a letter to Creative
 * Commons, PO Box 1866, Mountain View, CA 94042, USA.
 */

/*
 * Animation consists of white dots travelling up at varying
 * speeds and accelerations.
 * 
 * When we draw the dots, we draw B color over the previous
 * coordinates, and A color over the new coordinate,
 *
 * In between each frame we fill the canvas with C opacity bg color
 *
 *
 * The only other modification that I can think of is Collision detection,
 * ie: particles that bump other particles from behind randomly shift to the
 * left or right
 */

/**
 * Represents a color using Red, Green, and Blue components in that order.
 * Each value should range from 0 - 255, or 0x00 - 0xFF.
 * 0 represents a full lack of that color, and 255 represents a full occurrence
 * of that color.
 * @typedef {number[]} ColorRGB
 */

/**
 * Represents a color using Red, Green, Blue, and Alpha components in that
 * order.
 * The Red, Green, and Blue values should range from 0 - 255, or 0x00 - 0xFF.
 * 0 represents a full lack of that color, and 255 represents a full occurrence
 * of that color.
 * The Alpha value should range from 0.0 - 1.0.
 * 0.0 represents full transparancy, 1.0 represents full opacity.
 * @typedef {number[]} ColorRGBA
 * @see {@link Opacity}
 */

/**
 * Represents the opacity of a color ranging from 0.0 to 1.0.
 * 0.0 represents full transparancy, 1.0 represents full opacity.
 * @typedef {number} Opacity
 */

/**
 * Represents the saturation of a color, or how "vibrant" the color appears,
 * ranging from 0.0 to 100.0.
 * 0.0 represents 0% saturation, or grayscale.
 * 100.0 represents 100% saturation, or complete vibrancy.
 *
 * Even though this value represents a percentage, you must express the value
 * as a number ranging from 0 to 100.
 * @typedef {number} Saturation
 */
 
/**
 * Represents the luminosity of a color, or how much light the color gives off,
 * ranging from 0.0 to 100.0.
 * 0.0 represents 0% luminosity, or black.
 * 50.0 represents 50% luminosity, or "regular".
 * 100.0 represents 100% luminosity, or white.
 *
 * Luminosity, while related to the "Value" component in HSV, is not the same
 * thing as "Value". A "Value" of 100%, would be equal to a Luminosity of 50%.
 * Any Luminosity over 50% would still be equivalent to a "Value" of 100%, the
 * Saturation would instead decrease.
 * Refer to conical models of the HSL and HSV color spaces. HSV looks like an
 * upside-down cone, while HSL looks like a di-cone.
 * [(Image)]{@link https://learnui.design/blog/img/hsb/hsb-cone-and-hsl-dicone.png}
 
 * Even though this value represents a percentage, you must express the value
 * as a number ranging from 0 to 100.
 * @typedef {number} Luminosity
 */

/**
 * Represents the Hue of a color, as a measure of degrees (not radians), around
 * the color wheel.
 * The color wheel is split into 60° sections in the following order:
 * 330° -  30°  Red          (  0°/360° Completely Red)
 *  30° -  90°  Yellow       ( 60° Completely Red & Green (Yellow))
 *  90° - 150°  Green (Lime) (120° Completely Green)
 * 150° - 210°  Cyan         (180° Completely Green & Blue (Cyan))
 * 210° - 270°  Blue         (240° Completely Blue)
 * 270° - 330°  Magenta      (300° Completely Blue & Red (Magenta))
 * 
 * If you are familiar with the Additive (RGB) and Subtractive (CMY) color
 * models, you may notice that their respective components form the color wheel:
 * R > Y > G > C > B > M > R ...
 * As, RED + BLUE = MAGENTA, RED + GREEN = YELLOW, GREEN + BLUE = CYAN
 * and MAGENTA + CYAN = BLUE, CYAN + YELLOW = GREEN, YELLOW + MAGENTA = RED
 * @typedef {number} Hue
 */

/**
 * This isn't an actual typedef. It's meant to be a pointer to common notes
 * regarding the calculation of sinusoidal functions, or functions that use
 * trigonometric functions like sine, cosine, or tangent (hence SINusoid).
 * 
 * Sinusoidal equations generally take the following forms:
 * F(x) = y = A x sin(B (x - C)) + D
 * G(x) = y = A x cos(B (x - C)) + D
 * where with F(x = 0) = 0 (sine)
 * and with G(x = 0) = 1 (sine)
 * Note that both F(x) and G(x) can be equated depending on their phase shifts
 * (the C)
 *
 * A = Amplitude
 * B = Frequency
 * C = Phase Shift
 * D = Vertical Shift
 * To calculate the period, or when the function repeats use: (2 x PI) / B
 *
 * Amplitude describes where the peaks of the function reach, for example, with
 * a Vertical Shift of 0 (no shift), and an amplitude of 3
 * (y = 3sin(1(x-0))+0 = 3sin(x) ), the value of "y" should range from -3 to 3
 * In other words, the Range of the function is [-|A|,+|A|]
 *
 * Frequency describes how quickly the function repeats itself. It's basically
 * used to modify the periodicity of the function. Sinusoid functions normally
 * have a period of 2 x PI. Using the B parameter you can change that.
 * To have the function repeat every 2 units, or to set the period equal to 2,
 * you would have to determine the value of B: 
 * B = (2 x PI) / Period
 * In this case: B = (2 x PI) / 2, since two is in both the numerator and
 * denominator, they cancel out leaving: B = PI, so setting B to "PI" makes
 * the function repeat every two units.
 *
 * Phase shift, or horizontal shift, describes how far the function is shifted
 * horizontally, through the first phase of the function (before it repeats).
 * Positive values for C, will shift the function to the right C units.
 * Negative values for C, will shift the function to the left C units.
 * If you use a phase shift that is equal to the period of the function, the
 * function is not affected by the phase shift. Using the previous example:
 * y = sin(pi * x)   which has a period of 2,
 * if we modified this to add a phase shift of positive 2:
 * y = sin(pi * (x - 2))
 * the function will produce the same results as the original, ie:
 * y = sin(pi * x) = sin(pi * (x - 2))
 * this holds true for all positive and negative multiples of the period as
 * well.
 * 
 * Vertical shift, describes how far the function is shifted vertically.
 * Positive values shift the function upwards D units.
 * Negative values shift the function downwards D units.
 *
 * Since trigonometry can be a bit confusing, feel free to google terms like:
 * "sine function formula" or "cosine function formula"
 * Or you can use this link: 
 * [image]{@link https://www.onlinemathlearning.com/image-files/transformation-trig-graphs.png}
 * You can also play with the graphs of sine functions on [graph.tk]{@link graph.tk}
 * as well. In the upper right is a "?" button, click there if you need help, as that
 * article explains how to type in the pi symbol.
 *
 * @typedef {undefined} Sinusoid
 */

/**
 * Represents the components that make up the internal state of a dot.
 * most of the property names are heavily abbreviated to make other pieces of
 * code more readable, at the expense of understanding what each property means.
 * Refer to the {@link newDot} function for information on the default values
 * of the following properties.
 * @typedef {Object} Dot
 * @property {number} x - The current x coordinate of the dot.
 * @property {number} y - The current y coordinate of the dot.
 * @property {number} s - The speed of the dot, in pixels per frame.
 * @property {number} a - The acceleration of the dot, in pixels per frame.
 * @property {number} c - The average hue of the dot.
 * @property {number} l - The average luminosity of the dot.
 * @property {number} sa - The average saturation of the dot.
 * @property {ColorRGB} c2 - The original RGB color of the dot, before the HSL
 * change. This property is unused. It's included for compatibility.
 * @property {number} f - The frame that the dot was created on, this is used
 * to phase shift the sinusoid function so that the oscillation starts on the
 * frame the dot was created, instead of where the oscillation would be at if
 * the dot was created on the first frame
 * @property {number} pa - The amplitude at which the luminosity of the dot
 * fluctuates.
 * @property {number} pb - The frequency at which the luminosity of the dot
 * fluctuates.
 * @property {number} pc - The phase-shift at which the luminosity of the dot
 * fluctuates.
 * @property {number} bpa - The amplitude at which the line width of the dot
 * fluctuates.
 * @property {number} bpb - The frequency at which the line width of the dot
 * fluctuates.
 * @property {number} bpc - The phase-shift at which the line width of the dot
 * fluctuates.
 * @property {number} w - The thickness of the trail left by the dot.
 * @see {@link Hue}
 * @see {@link Luminosity}
 * @see {@link Saturation}
 * @see {@link Sinusoid}
 * @see {@link newDot}
 */










/**
 * The background color of the canvas.
 * @constant {ColorRGB}
 * @default [0,0,0]
 */
const BACKGROUND = [0,0,0];

/**
 * The color of the leading trail all dots leave.
 * This value is currently not used.
 * @constant {ColorRGBA}
 * @default [255,255,255,1.0]
 */
const DOT_COLOR = [255,255,255,1.0];

/**
 * A number ranging from 0.0 - 1.0 that represents the opacity of the trails
 * the dots leave.
 * @see {@link Opacity}
 * @constant {Opacity}
 * @default 1.0
 */
const TRAIL_OPACITY = 1.0;

/**
 * The color of the secondary trail all dots leave.
 * This value is currently not used.
 * @constant {ColorRGBA}
 * @default [88,0,133,TRAIL_OPACITY]
 */
const TRAIL_COLOR = [88, 0, 133,TRAIL_OPACITY];

/**
 * The minimum saturation allowed for trail components.
 * @see {@link Saturation}
 * @see {@link TRAIL_SATURATION_MAX}
 * @constant {Saturation}
 * @default 100.0
 */
const TRAIL_SATURATION_MIN = 100.0;

/**
 * The maximum saturation allowed for trail components.
 * @see {@link Saturation}
 * @see {@link TRAIL_SATURATION_MIN}
 * @constant {Saturation}
 * @default 100.0
 */
const TRAIL_SATURATION_MAX = 100.0;

/**
 * The minimum luminosity allowed for trail components.
 * @see {@link Luminosity}
 * @see {@link TRAIL_LUMINOSITY_MAX}
 * @constant {Luminosity}
 * @default 25.0
 */
const TRAIL_LUMINOSITY_MIN = 25.0;

/**
 * The maximum luminosity allowed for trail components.
 * @see {@link Luminosity}
 * @see {@link TRAIL_LUMINOSITY_MIN}
 * @constant {Luminosity}
 * @default 75.0
 */
const TRAIL_LUMINOSITY_MAX = 75.0;

/**
 * The rate at which the average hue of the dots shifts around the color wheel.
 * This value has a period of 360, meaning that if this value is over 360, it
 * will effectively shift it by "HSL_DRIFT MOD 360".
 * Eg: Setting this to 475 is the same as setting this to 115. (since 475-360=115)
 * This value can also be negative.
 * @see {@link Hue}
 * @constant {number}
 * @default 0.1
 */
const HSL_DRIFT = 0.1;

/**
 * The minimum speed in pixels per frame the dots move.
 * To calculate the approximate minimum number of pixels per second, use the
 * following formula: {@link FPS} x {@link MIN_SPEED}
 * @see {@link MAX_SPEED}
 * @constant {number}
 * @default 0.1
 */
const MIN_SPEED = 0.1;

/**
 * The maximum speed in pixels per frame the dots move.
 * To calculate the approximate maximum number of pixels per second, use the
 * following formula: {@link FPS} x {@link MAX_SPEED}
 * @see {@link MIN_SPEED}
 * @constant {number}
 * @default 2.0
 */
const MAX_SPEED = 2.0;

/**
 * The minimum acceleration in pixels per frame the dots move.
 * To calculate the approximate minimum acceleration number of pixels per
 * second, use the following formula: {@link FPS} x {@link MIN_ACCEL}
 * @see {@link MAX_ACCEL}
 * @constant {number}
 * @default 0.01
 */
const MIN_ACCEL = 0.01;

/**
 * The maximum acceleration in pixels per frame the dots move.
 * To calculate the approximate maximum acceleration number of pixels per
 * second, use the following formula: {@link FPS} x {@link MAX_ACCEL}
 * @see {@link MIN_ACCEL}
 * @constant {number}
 * @default 0.50
 */
const MAX_ACCEL = 0.50;

/**
 * The maximum number of dots that can concurrently be active at one time.
 * If you set this to a high value, your processor and/or GPU might have trouble
 * keeping up with the physics of each particle.
 * @constant {number}
 * @default 250
 */
const MAX_DOTS = 250;

/**
 * The rate at which new dots are added to the simulation/animation. They are
 * effectively added at a rate of {@link DOT_RATE} dots per frame.
 * Setting this value to a value higher than {@link MAX_DOTS} will not pose
 * any issues.
 * @constant {number}
 * @default 2
 */
const DOT_RATE = 2;

/**
 * The opacity at which active trails are erased. A lower value here will make
 * longer trails, while a larger value produces shorter trails.
 * A small caveat of this method is that unless this is set to 1.0 (where 
 * trails instantly vanish), the path the trails travel will never truly be set
 * to the background color ever again as the average of any numbers less than
 * 1.0 will always remain under 1.0. You might notice this effect as dots trail
 * accross the canvas. They seem to leave a persistent trail whose color is
 * very slightly brighter or darker than the background color.
 * @see {@link Opacity}
 * @constant {Opacity}
 * @default 2
 */
const FADE_OPACITY = 0.2;

/**
 * The total number of frames to animate per second.
 * Recommended values: 20, 30
 * Values lower than 20 will result in stuttering.
 * Values greater than 30 will result in computational lag depending on other
 * settings.
 * @constant {number}
 * @default 20
 */
const FPS = 20;

/**
 * DO NOT MODIFY THIS CONSTANT!!!
 * This constant holds the number of milliseconds between each frame. It is used
 * internally to cooperate with [window.setTimeout]{@link Window.setTimeout} to
 * schedule when the next frame should occur.
 * You should have no reason to modify this value, unless, for some reason, the
 * laws of physics have changed and a second no longer consists of exactly 1000
 * milliseconds.
 * @constant {number}
 * @default
 */
const FRAME_INTERVAL = 1000 / FPS;


/**
 * The minimum width of the trails the dots produce. This value is effectively
 * like the radius of a circle, meaning that the produced trail extends both to
 * the left and right {@link LINE_WIDTH_MIN} pixels.
 * In other words, a value of 1.0 would actually take up 2-3 pixels.
 * @constant {number}
 * @default 0.5
 */
const LINE_WIDTH_MIN = 0.5;

/**
 * The maximum width of the trails the dots produce. This value is effectively
 * like the radius of a circle, meaning that the produced trail extends both to
 * the left and right {@link LINE_WIDTH_MAX} pixels.
 * In other words, a value of 1.0 would actually take up 2-3 pixels.
 * @constant {number}
 * @default 3.0
 */
const LINE_WIDTH_MAX = 3.0;


/**
 * The minimum amount of time before the luminosity of a dot, finishes an
 * oscillation. This value should be in the form "{@link FPS} * [number of seconds]"
 * where "[number of seconds]" is how many seconds it should take to loop.
 * @see {@link Sinusoid}
 * @see {@link FPS}
 * @see {@link LUMINOSITY_OSCILLATION_PERIOD_MAX}
 * @see {@link Luminosity}
 * @constant {number}
 * @default
 */
const LUMINOSITY_OSCILLATION_PERIOD_MIN = FPS * 0.5;

/**
 * The maximum amount of time before the luminosity of a dot, finishes an
 * oscillation. This value should be in the form "{@link FPS} * [number of seconds]"
 * where "[number of seconds]" is how many seconds it should take to loop.
 * @see {@link Sinusoid}
 * @see {@link FPS}
 * @see {@link LUMINOSITY_OSCILLATION_PERIOD_MIN}
 * @see {@link Luminosity}
 * @constant {number}
 * @default
 */
const LUMINOSITY_OSCILLATION_PERIOD_MAX = FPS * 1;

/**
 * The minimum variation in luminosity the dot should oscillate to/from.
 * This value is relative to the luminosity of the dot.
 * @see {@link Sinusoid}
 * @see {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MAX}
 * @see {@link Luminosity}
 * @constant {number}
 * @default 0.1
 */
const LUMINOSITY_OSCILLATION_AMPLITUDE_MIN = 0.1;

/**
 * The maximum variation in luminosity the dot should oscillate to/from.
 * This value is relative to the luminosity of the dot.
 * @see {@link Sinusoid}
 * @see {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MIN}
 * @see {@link Luminosity}
 * @constant {number}
 * @default 25
 */
const LUMINOSITY_OSCILLATION_AMPLITUDE_MAX = 25;

/**
 * The phase shift of the luminosity oscillation, relative to the start of the,
 * oscillation.
 * This value will likely be converted to a "MIN"/"MAX" so that it may be
 * randomized, as having this static does basically nothing.
 * @see {@link Sinusoid}
 * @see {@link Luminosity}
 * @constant {number}
 * @default 0
 */
const LUMINOSITY_OSCILLATION_PHASE_SHIFT = 0;


/**
 * The minimum amount of time before the line width of a dot, finishes an
 * oscillation. This value should be in the form "{@link FPS} * [number of seconds]"
 * where "[number of seconds]" is how many seconds it should take to loop.
 * @see {@link Sinusoid}
 * @see {@link FPS}
 * @see {@link LINE_WIDTH_OSCILLATION_PERIOD_MAX}
 * @see {@link LINE_WIDTH_MIN}
 * @see {@link LINE_WIDTH_MAX}
 * @constant {number}
 * @default
 */
const LINE_WIDTH_OSCILLATION_PERIOD_MIN = FPS * 0.5;

/**
 * The maximum amount of time before the line width of a dot, finishes an
 * oscillation. This value should be in the form "{@link FPS} * [number of seconds]"
 * where "[number of seconds]" is how many seconds it should take to loop.
 * @see {@link Sinusoid}
 * @see {@link FPS}
 * @see {@link LINE_WIDTH_OSCILLATION_PERIOD_MIN}
 * @see {@link LINE_WIDTH_MIN}
 * @see {@link LINE_WIDTH_MAX}
 * @constant {number}
 * @default
 */
const LINE_WIDTH_OSCILLATION_PERIOD_MAX = FPS * 1;

/**
 * The minimum variation in line width the dot should oscillate to/from.
 * This value is relative to the line width of the dot.
 * @see {@link Sinusoid}
 * @see {@link LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX}
 * @see {@link LINE_WIDTH_MIN}
 * @see {@link LINE_WIDTH_MAX}
 * @constant {number}
 * @default 0.1
 */
const LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN = 0.1;

/**
 * The maximum variation in line width the dot should oscillate to/from.
 * This value is relative to the line width of the dot.
 * @see {@link Sinusoid}
 * @see {@link LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN}
 * @see {@link LINE_WIDTH_MIN}
 * @see {@link LINE_WIDTH_MAX}
 * @constant {number}
 * @default 2.0
 */
const LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX = 2.0;

/**
 * The phase shift of the line width oscillation, relative to the start of the,
 * oscillation.
 * This value will likely be converted to a "MIN"/"MAX" so that it may be
 * randomized, as having this static does basically nothing.
 * @see {@link Sinusoid}
 * @see {@link LINE_WIDTH_MIN}
 * @see {@link LINE_WIDTH_MAX}
 * @constant {number}
 * @default 0
 */
const LINE_WIDTH_OSCILLATION_PHASE_SHIFT = 0;

/**
 * A flag that determines whether or not the canvas is resized whenever the
 * document or window is resized.
 * While the canvas *IS* stretched to the full size of the window, the actaul
 * size of the canvas itself, internally, is set when the page first loads.
 * It's set to the initial window size of the page. If you resize the window
 * after this point, the canvas is stretched or skewed to fit in the new bounds,
 * it's not actually resized.
 * Setting this to true will resize the canvas automatically when the page size
 * is changed.
 *
 * To see this effect, set this value to false, load the page with the window
 * really small, and then maximize the window, the contents of the canvas will
 * be stretched to the point that they blur really badly.
 * @constant {boolean}
 * @default false
 *
 * @todo Implement this
 */
const RESIZE_CANVAS_ON_WINDOW_RESIZE = false;
//vertical shift (D) is the l parameter of the dot
//2PI / LUM_OSC_PER = B

/**
 * Represents the initial minimum hue of the dots that are created in degrees
 * rotated around the color wheel.
 * The larger the difference between {@link TRAIL_HSL_START} and
 * {@link TRAIL_HSL_END}, the more variation in color the dots will display.
 * Setting the range to anything larger than or equal to 360 will effectively
 * Eliminate the cycling of color.
 * @see {@link Hue}
 * @see {@link TRAIL_HSL_END}
 * @var {number}
 * @default 180.0
 */
var TRAIL_HSL_START = 180.0;

/**
 * Represents the initial maximum hue of the dots that are created in degrees
 * rotated around the color wheel.
 * The larger the difference between {@link TRAIL_HSL_START} and
 * {@link TRAIL_HSL_END}, the more variation in color the dots will display.
 * Setting the range to anything larger than or equal to 360 will effectively
 * Eliminate the cycling of color.
 * @see {@link Hue}
 * @see {@link TRAIL_HSL_START}
 * @var {number}
 * @default 180.0
 */
var TRAIL_HSL_END = 240.0;




/*********************************
 * END OF CONFIGURATION SETTINGS *
 *********************************/




/**
 * A variable that holds a reference to the HTML canvas element
 * (selector: "#canvas").
 * This value is filled in when the page finishes loading.
 * @var {HTMLCanvasElement}
 * @readonly
 */
var canvas;

/**
 * A variable that holds a reference to the 2D Rendering context of the
 * {@link canvas} element. This context provides methods and properties that
 * are used to draw to the {@link canvas}.
 * This value is filled in when the page finishes loading.
 * @var {CanvasRenderingContext2D}
 * @readonly
 */
var context;

/**
 * A variable that holds the last known
 * (or initial if {@link RESIZE_CANVAS_ON_WINDOW_RESIZE} is false)
 * size of the viewport window. The size of the {@link canvas} is set to the
 * [width]{@link DOMRect.width} and [height]{@link DOMRect.height} properties of
 * this variable.
 * @var {DOMRect}
 */
var size;

/**
 * A variable that contains a list of all the currently active dots being
 * animated. The maximum number of elements in this array is {@link MAX_DOTS}.
 * @see {@link Dot)
 * @var {Dot[]}
 */
var dots = [];

/**
 * A variable that stores the total number of frames that have been rendered
 * already. This value is used when computing the value of the sinusoidal
 * functions
 * @var {number}
 */
var FRAME_COUNT = 0;

//Asin(B(x-C))+D
//Amplitude, Frequency, H-Shift, V-Shift


/**
 * Generates a random floating-point number between "min" and "max".
 * If you need an integer instead, call {@link Math.floor} on the result of this
 * function.
 * @function
 * @param {number} min - The inclusive lower bound of the random number.
 * @param {number} max - The exclusive upper bound of the random number.
 * @returns {number} The pseudorandom number that was generated.
 */
function rand(min, max) {
    return (Math.random() * (max - min)) + min;
}

/**
 * Determines the frequency of a sinusoidal equation based on a given period.
 * @see {@link Sinusoid}
 * @function
 * @param {number} period - The period of the sinusoidal function.
 * @returns {number} The frequency of the sinusoidal function.
 */
function getB(period) {
    return (2 * Math.PI) / period;
}

/**
 * Calculates the value of a sinusoid equation given the four possible
 * transformations that can be applied to it. (see {@link Sinusoid} for more
 * details about each parameter.)
 * The function uses the current [frame count]{@link FRAME_COUNT} as the value
 * of the "x" parameter.
 * @see {@link Sinusoid}
 * @function
 * @param {number} a - The amplitude of the function.
 * @param {number} b - The frequency of the function.
 * @param {number} c - The phase-shift of the function.
 * @param {number} d - The vertical-shift of the function.
 * @returns {number} The value of y in the equation y = A * sin(B * (x - C)) + D
 * where x is equal to the the current [frame count]{@link FRAME_COUNT}
 */
function sinusoidal(a,b,c,d) {
    return a * Math.sin(b * (FRAME_COUNT - c)) + d;
}

/**
 * A function that represents the computation required to complete a single
 * frame in the animation.
 * @function
 */
function animation() {
    //Increment the frame counter.
    FRAME_COUNT += 1;

    //Erase all previously recorded paths.
    context.beginPath();

    //Set the fill style and stroke style to the background color, at the
    //fade opacity
    context.fillStyle = `rgba(${BACKGROUND.join(",")},${FADE_OPACITY})`;
    context.strokeStyle = `rgba(${BACKGROUND.join(",")},${FADE_OPACITY})`;

    //Move the path to the origin of the canvas, (0,0), or the upper left corner
    //of the canvas.
    context.moveTo(0,0);

    //Create a rectangle, offset by (0,0), the size of the entire canvas.
    context.rect(0,0,Math.round(size.width),Math.round(size.height));

    //Fill the current path.
    context.fill();


    //Add new dots to the animation, given we are able to.
    var i;
    for (i = 0; i < DOT_RATE; i += 1) {
        if (dots.length >= MAX_DOTS) {
            //We currently have the maximum number of dots allowed,
            //don't add any more.
            break;
        }
        //Add another dot.
        newDot();
    }

    //Move all dots.
    moveDots();

    //set a timer to run this same function, when we need to animate the next
    //frame.
    window.setTimeout(animation,FRAME_INTERVAL);
}


/**
 * A setup function, called when the page loads. It sets up the initial values
 * of the {@link canvas}, {@link context}, and {@link size} variables; sets up
 * the canvas, and begins the animation.
 * @function
 */
function start() {
    //Retrieve the CANVAS element
    canvas = document.querySelector("canvas");

    //Get a 2D drawing context for the canvas
    context = canvas.getContext("2d");

    //Get the size of the canvas, which should be stretched to the full size of
    //the window.
    size = canvas.getBoundingClientRect();

    //Set the width and height of the canvas internally, so that the canvas has
    //a 1 to 1 ratio between itself and the screen.
    canvas.setAttribute("width",Math.round(size.width));
    canvas.setAttribute("height",Math.round(size.height));

    //Clear all prior paths.
    context.beginPath();

    //Set the fill and stroke styles to the background color at full opacity.
    context.fillStyle = `rgba(${BACKGROUND.join(",")},1)`;
    context.strokeStyle = `rgba(${BACKGROUND.join(",")},1)`;

    //Fill the entire canvas with the current fill style.
    context.fillRect(0,0,Math.round(size.width),Math.round(size.height));

    //Create a timer to start the animation.
    window.setTimeout(animation,FRAME_INTERVAL);

    //hook into the resize event.
    window.addEventListener("resize",resizeHandler);
}

/**
 * An event handler used to handle the {@link Window.onresize} event.
 * @function
 * @listens event:resize
 */
function resizeHandler() {
    //Initialize variables.
    var i;
    var osize = size;

    //verify that resize is actually enabled
    if (RESIZE_CANVAS_ON_WINDOW_RESIZE) {
        //Get the size of the canvas, which should be stretched to the full size
        //of the window.
        size = canvas.getBoundingClientRect();

        //Set the width and height of the canvas internally, so that the canvas
        //has a 1 to 1 ratio between itself and the screen.
        canvas.setAttribute("width",Math.round(size.width));
        canvas.setAttribute("height",Math.round(size.height));

        //check to see if the canvas was made smaller, if not, don't check.
        if(osize.width > size.width) {
            //check all dots to see if they're still in bounds.
            for (i = 0; i < dots.length; i += 1) {
                if (dots[i].x > size.width) {
                    //dot is out of bounds, remove it.
                    dots.splice(i,1);
                    i -= 1;
                }
            }
        }
    }
}

/**
 * A helper function used to move all the dots.
 * @function
 */
function moveDots() {
    //Initialize all variables used in the for loop below to undefined.
    //Initializing them outside of the loop is beneficial as it prevents
    //the javascript engine from reallocating each variable every iteration
    //of the for loop. This reduces the amount of data needed in garbage
    //collection cycles.
    var i;
    var d; 
    var py;
    var px;
    var l;
    var w;

    //Iterate over every dot.
    for (i = 0; i < dots.length; i += 1) {
        //get the current dot.
        //JavaScript objects are always passed **by value**
        //meaning that even if we modify the "d" variable, it will still
        //affect "dots[i]", ie: d.y = 1 is the same as dots[i].y = 1.
        d = dots[i];

        //Skip the first frame of the animation to retrieve the second of
        //three points necessary for the animation
        if (d.py === undefined) {
            d.px = d.x;
            d.py = d.y;
            continue;
        }

        //Skip the second frame of the animation to retrieve the third of
        //three points necessary for the animation
        if (d.ppy === undefined) {
            d.ppx = d.px;
            d.ppy = d.py;
            d.px = d.x;
            d.py = d.y
            continue;
        }
        // 3 2 p 1 n
        // 1 1 1 1 2 =>
        // 1 1 2 2 3 =>
        // 1 2 3 3 4
        //store the old coordinates of the dot.
        py = d.y;
        px = d.x;
        
        //Determine the current effective luminosity of the dot based on the
        //current frame count
        l = sinusoidal(d.pa,d.pb,d.pc,d.l);
        //Determine the current effective line width of the dot based on the
        //current frame count
        w = sinusoidal(d.bpa,d.bpb,d.bpc,d.w);

        //Set the line width
        context.lineWidth = w;

        //Clear all preivous paths
        context.beginPath();

        //Set the stroke and fill styles to the hue, saturation, luminosity,
        //and opacity of the current dot.
        context.strokeStyle = `hsla(${d.c},${d.sa}%,${l}%,${TRAIL_OPACITY})`;
        context.fillStyle = `hsla(${d.c},${d.sa}%,${l}%,${TRAIL_OPACITY})`;

        //Move to the oldest of the three reference points of the dot.
        context.moveTo(Math.round(d.ppx),Math.round(d.ppy));
        //Make a line to the second oldest of the three reference points of the
        //dot.
        context.lineTo(Math.round(d.px),Math.round(d.py));
        //Draw the line.
        context.stroke();

        //sets the stroke and fill styles to the lead color of the dot.
        //context.strokeStyle = `rgba(${DOT_COLOR.join(",")})`;
        //context.fillStyle = `rgba(${DOT_COLOR.join(",")})`;

        //Make a line to the first (newest) of the three reference points of the
        //dot
        context.lineTo(Math.round(d.x),Math.round(d.y));
        //Draw the line.
        context.stroke();

        //Shift the second reference point into the third slot.
        d.ppx = d.px;
        d.ppy = d.py;
        //Shift the first reference point into the second slot.
        d.px = d.x;
        d.py = d.y;
        
        //Move the dot upwards based on the dot's speed.
        d.y -= d.s;
        //Increase the dot's speed based on its acceleration.
        d.s += d.a;

        //Remove the current dot if it's off-screen
        if (d.ppy < 0) {
            dots.splice(i,1);
            i -= 1;
        }
        
        //Reset the line width
        context.lineWidth = 1;
    }
}

/**
 * A helper function used to generate a single new dot.
 * See {@link Dot} for more info about the structure of the dot.
 * @see {@link Dot}
 * @function
 */
function newDot() {
    //push the new dot to the end to the list of all active dots.
    //See the typedef of Dot for explanations of the following.
    dots.push({
        //X-Coord: a random value between 0 and the size of the canvas.
        x: rand(0,size.width),
        //Y-Coord: the very bottom of the canvas.
        y: size.height,
        //Speed: A random value between MIN/MAX SPEED.
        s: rand(MIN_SPEED,MAX_SPEED),
        //Acceleration: A random value between MIN/MAX ACCELERATION.
        a: rand(MIN_ACCEL,MAX_ACCEL),
        //Hue: A random value between TRAIL HSL START/END.
        c: rand(TRAIL_HSL_START,TRAIL_HSL_END),
        //Luminosity: A random value between TRAIL LUMINOSITY START/END.
        l: rand(TRAIL_LUMINOSITY_MIN,TRAIL_LUMINOSITY_MAX),
        //Saturation: A random value between TRAIL SATURATION START/END.
        sa: rand(TRAIL_SATURATION_MIN,TRAIL_SATURATION_MAX),
        //Legacy RGB color, deprecated in favor of HSL's greater control of color.
        c2: [Math.floor(rand(0,255)),Math.floor(rand(0,255)),Math.floor(rand(0,255))],
        //Creation Frame: current frame count.
        f: FRAME_COUNT,
        //Luminosity Oscillation Amplitude: random value in between MIN/MAX.
        pa: rand(LUMINOSITY_OSCILLATION_AMPLITUDE_MIN,LUMINOSITY_OSCILLATION_AMPLITUDE_MAX),
        //Luminosity Oscillation Frequency: the frequency when the period is a
        //random value in between MIN/MAX.
        pb: getB(rand(LUMINOSITY_OSCILLATION_PERIOD_MIN,LUMINOSITY_OSCILLATION_PERIOD_MAX)),
        //Luminosity Oscillation Phase Shift: creation frame number + PHASE SHIFT
        pc: FRAME_COUNT + LUMINOSITY_OSCILLATION_PHASE_SHIFT,
        //Line Width Oscillation Amplitude: random value in between MIN/MAX.
        bpa: rand(LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN,LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX),
        //Line Width Oscillation Frequency: the frequency when the period is a
        //random value in between MIN/MAX.
        bpb: getB(rand(LINE_WIDTH_OSCILLATION_PERIOD_MIN,LINE_WIDTH_OSCILLATION_PERIOD_MAX)),
        //Line Width Oscillation Phase Shift: creation frame number + PHASE SHIFT
        bpc: FRAME_COUNT + LINE_WIDTH_OSCILLATION_PHASE_SHIFT,
        //Line Width: a random value between MIN/MAX.
        w: rand(LINE_WIDTH_MIN,LINE_WIDTH_MAX)
    });

    //Drift the Hue range, by HSL_DRIFT
    TRAIL_HSL_START += HSL_DRIFT;
    TRAIL_HSL_END += HSL_DRIFT;

    //Bounds checking, make sure HSL_START/END are between 0 and 360.
    //doing this prevents the application from randomly failing when either
    //gets too large or too small.
    //The application should only break when the precision of FRAME_COUNT becomes
    //too small to keep track of each new frame, or when we overflow FRAME_COUNT into NaN
    if(TRAIL_HSL_START < 0) {
        TRAIL_HSL_START += 360;
    }
    if(TRAIL_HSL_END < 0) {
        TRAIL_HSL_END += 360;
    }
    TRAIL_HSL_START %= 360;
    TRAIL_HSL_END %= 360;
    if(TRAIL_HSL_START > TRAIL_HSL_END) {
        TRAIL_HSL_START -= 360;
    }
}


if (document.readyState !== "complete") {
    //If the page isn't done loading, wait for it to finish loading, then run
    //start.
    window.addEventListener("load",start);
} else {
    //The page has already finished loading, run start now.
    start();
}
