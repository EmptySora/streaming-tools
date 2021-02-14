/**
 * @file Produces an animation that vaguely resembles rain falling upwards.
 * @author EmptySora_
 * @version 2.1.0.0
 * @license CC-BY 4.0
 * This work is licensed under the Creative Commons Attribution 4.0
 * International License. To view a copy of this license, visit
 * http://creativecommons.org/licenses/by/4.0/ or send a letter to Creative
 * Commons, PO Box 1866, Mountain View, CA 94042, USA.
 */
const VERSION = "2.1.0.0";

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
 * 
 * @typedef {Object} KeyBindingInfo
 * @property {Key} key - The name of the key being pressed, if this is a letter, or value that changes with the shift key, the actual value here depends on whether or not the shift key is pressed.
 * @property {KeyBindingCondition[]} conditions - An Array of {@link KeyBindingCondition} objects that describe conditionals required to trigger the binding. Only one of the conditions needs to be met.
 * @property {Function} handler - The event handler for the key binding.
 */

/**
 * Conditions that must be met in order to run the key binding. All specified
 * properties must have their respective keys set to the specified state.
 * @typedef {Object} KeyBindingCondition
 * @property {boolean} ctrl - Required state of the CTRL key.
 * @property {boolean} alt - Required state of the ALT key.
 * @property {boolean} meta - Required state of the META key.
 * @property {boolean} shift - Required state of the SHIFT key.
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
const DEFAULT_BACKGROUND = [0, 0, 0];

/**
 * The color of the leading trail all dots leave.
 * This value is currently not used.
 * @constant {ColorRGBA}
 * @default [255,255,255,1.0]
 */
const DEFAULT_DOT_COLOR = [255, 255, 255, 1.0];

/**
 * A number ranging from 0.0 - 1.0 that represents the opacity of the trails
 * the dots leave.
 * @see {@link Opacity}
 * @constant {Opacity}
 * @default 1.0
 */
const DEFAULT_TRAIL_OPACITY = 1.0;

/**
 * The color of the secondary trail all dots leave.
 * This value is currently not used.
 * @constant {ColorRGBA}
 * @default [88,0,133,TRAIL_OPACITY]
 */
const DEFAULT_TRAIL_COLOR = [88, 0, 133, DEFAULT_TRAIL_OPACITY];

/**
 * The minimum saturation allowed for trail components.
 * @see {@link Saturation}
 * @see {@link TRAIL_SATURATION_MAX}
 * @constant {Saturation}
 * @default 100.0
 */
const DEFAULT_TRAIL_SATURATION_MIN = 100.0;

/**
 * The maximum saturation allowed for trail components.
 * @see {@link Saturation}
 * @see {@link TRAIL_SATURATION_MIN}
 * @constant {Saturation}
 * @default 100.0
 */
const DEFAULT_TRAIL_SATURATION_MAX = 100.0;

/**
 * The minimum luminosity allowed for trail components.
 * @see {@link Luminosity}
 * @see {@link TRAIL_LUMINOSITY_MAX}
 * @constant {Luminosity}
 * @default 25.0
 */
const DEFAULT_LUMINOSITY_MIN = 25.0;

/**
 * The maximum luminosity allowed for trail components.
 * @see {@link Luminosity}
 * @see {@link TRAIL_LUMINOSITY_MIN}
 * @constant {Luminosity}
 * @default 75.0
 */
const DEFAULT_TRAIL_LUMINOSITY_MAX = 75.0;

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
const DEFAULT_HSL_DRIFT = 0.1;

/**
 * The minimum speed in pixels per frame the dots move.
 * To calculate the approximate minimum number of pixels per second, use the
 * following formula: {@link FPS} x {@link MIN_SPEED}
 * @see {@link MAX_SPEED}
 * @constant {number}
 * @default 0.1
 */
const DEFAULT_MIN_SPEED = 0.1;

/**
 * The maximum speed in pixels per frame the dots move.
 * To calculate the approximate maximum number of pixels per second, use the
 * following formula: {@link FPS} x {@link MAX_SPEED}
 * @see {@link MIN_SPEED}
 * @constant {number}
 * @default 2.0
 */
const DEFAULT_MAX_SPEED = 2.0;

/**
 * The minimum acceleration in pixels per frame the dots move.
 * To calculate the approximate minimum acceleration number of pixels per
 * second, use the following formula: {@link FPS} x {@link MIN_ACCEL}
 * @see {@link MAX_ACCEL}
 * @constant {number}
 * @default 0.01
 */
const DEFAULT_MIN_ACCEL = 0.01;

/**
 * The maximum acceleration in pixels per frame the dots move.
 * To calculate the approximate maximum acceleration number of pixels per
 * second, use the following formula: {@link FPS} x {@link MAX_ACCEL}
 * @see {@link MIN_ACCEL}
 * @constant {number}
 * @default 0.50
 */
const DEFAULT_MAX_ACCEL = 0.50;

/**
 * The maximum number of dots that can concurrently be active at one time.
 * If you set this to a high value, your processor and/or GPU might have trouble
 * keeping up with the physics of each particle.
 * @constant {number}
 * @default 250
 */
const DEFAULT_MAX_DOTS = 250;

/**
 * The rate at which new dots are added to the simulation/animation. They are
 * effectively added at a rate of {@link DOT_RATE} dots per frame.
 * Setting this value to a value higher than {@link MAX_DOTS} will not pose
 * any issues.
 * @constant {number}
 * @default 2
 */
const DEFAULT_DOT_RATE = 2;

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
const DEFAULT_FADE_OPACITY = 0.2;

/**
 * The total number of frames to animate per second.
 * Recommended values: 20, 30
 * Values lower than 20 will result in stuttering.
 * Values greater than 30 will result in computational lag depending on other
 * settings.
 * @constant {number}
 * @default 20
 */
const DEFAULT_FPS = 20;

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
const DEFAULT_FRAME_INTERVAL = 1000 / DEFAULT_FPS;


/**
 * The minimum width of the trails the dots produce. This value is effectively
 * like the radius of a circle, meaning that the produced trail extends both to
 * the left and right {@link LINE_WIDTH_MIN} pixels.
 * In other words, a value of 1.0 would actually take up 2-3 pixels.
 * @constant {number}
 * @default 0.5
 */
const DEFAULT_LINE_WIDTH_MIN = 0.5;

/**
 * The maximum width of the trails the dots produce. This value is effectively
 * like the radius of a circle, meaning that the produced trail extends both to
 * the left and right {@link LINE_WIDTH_MAX} pixels.
 * In other words, a value of 1.0 would actually take up 2-3 pixels.
 * @constant {number}
 * @default 3.0
 */
const DEFAULT_LINE_WIDTH_MAX = 3.0;


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
const DEFAULT_LUMINOSITY_OSCILLATION_PERIOD_MIN = DEFAULT_FPS * 0.5;

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
const DEFAULT_LUMINOSITY_OSCILLATION_PERIOD_MAX = DEFAULT_FPS * 1;

/**
 * The minimum variation in luminosity the dot should oscillate to/from.
 * This value is relative to the luminosity of the dot.
 * @see {@link Sinusoid}
 * @see {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MAX}
 * @see {@link Luminosity}
 * @constant {number}
 * @default 0.1
 */
const DEFAULT_LUMINOSITY_OSCILLATION_AMPLITUDE_MIN = 0.1;

/**
 * The maximum variation in luminosity the dot should oscillate to/from.
 * This value is relative to the luminosity of the dot.
 * @see {@link Sinusoid}
 * @see {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MIN}
 * @see {@link Luminosity}
 * @constant {number}
 * @default 25
 */
const DEFAULT_LUMINOSITY_OSCILLATION_AMPLITUDE_MAX = 25;

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
const DEFAULT_LUMINOSITY_OSCILLATION_PHASE_SHIFT = 0;


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
const DEFAULT_LINE_WIDTH_OSCILLATION_PERIOD_MIN = DEFAULT_FPS * 0.5;

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
const DEFAULT_LINE_WIDTH_OSCILLATION_PERIOD_MAX = DEFAULT_FPS * 1;

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
const DEFAULT_LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN = 0.1;

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
const DEFAULT_LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX = 2.0;

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
const DEFAULT_LINE_WIDTH_OSCILLATION_PHASE_SHIFT = 0;

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
const DEFAULT_RESIZE_CANVAS_ON_WINDOW_RESIZE = false;
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
const DEFAULT_TRAIL_HSL_START = TRAIL_HSL_START;

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
const DEFAULT_TRAIL_HSL_END = TRAIL_HSL_END;




/**
 * The port on the server running WinAudioLevels.exe that is listening
 * for WebSocket Connections.
 * @default 8069
 * @constant {number}
 */
const PEAKS_APP_PORT = 8069;
/**
 * The domain of the server running WinAudioLevels.exe. If you are selfhosting,
 * set this to one of: "127.0.0.1", "localhost", or "::1"
 * @default "localhost"
 * @constant {string}
 */
const PEAKS_APP_DOMAIN = "localhost";
/**
 * Whether or not the server running WinAudioLevels.exe only accepts secure connections.
 * If you are selfhosting, most likely, this will be set to false. Otherwise, this must
 * be set to true due to JS security restrictions.
 * @default false
 * @constant {boolean}
 */
const PEAKS_APP_SECURE = false;

/**
 * Whether or not to try connection to the audio peaks server at all. Set this to
 * false if you are not running an audio peaks server.
 * @default true
 * @constant {boolean}
 */
const PEAKS_APP_ENABLED = true;

/**
 * How long to wait, in milliseconds, before attempting to reconnect to the
 * audio peaks server after an error occurs.
 * @default 30000
 * @constant {number}
 */
const PEAKS_APP_ERROR_RECONNECT_WAIT = 30000;

/**
 * How long to wait, in milliseconds, before attempting to reconnect to the
 * audio peaks server after a disconnection occurs.
 * @default 5000
 * @constant {number}
 */
const PEAKS_APP_RECONNECT_WAIT = 5000;

/**
 * The minimum multiplier the audio levels on the computer can affect the
 * animation by. This should be between 0.0 and 1.0.
 * A value of 0.0 would specify that dead silence would effectively halt
 * the animation, while a value of 1.0 would indicate that the audio peaks
 * would only be able to speed up the animation, if anything.
 *
 * The peak is converted to a percentage between 0% and 100%. Values less than
 * 50% will slow down the animation, while values above that will speed up the
 * animation. The actual calculation is as follows:
 *     speed_up_range = MAX_VARIANCE - 1;
 *     slow_down_range = 1 - MIN_VARIANCE;
 *     if (peak === 50%) {
 *         multiplier = 1;
 *     } else if (peak < 50%) {
 *         multiplier = 1 - (peak * 2 * slow_down_range);
 *     } else { // > 50%
 *         multiplier = 1 + ((peak - 50%) * 2 * speed_up_range);
 *     }
 * So, the range between MIN and MAX is not evenly spread out. MIN to 1, and 1
 * to MAX are spread out evenly.
 * @default 0.5
 * @constant {number}
 * @see {AUDIO_PEAKS_MAX_VARIANCE_MULTIPLIER}
 */
const AUDIO_PEAKS_MIN_VARIANCE_MULTIPLIER = 0.125;

/**
 * The maximum multiplier the audio levels on the computer can affect the
 * animation by. This should be greater than or equal to 1.0.
 * A value of 1.0 would indicate that the audio levels would only be able
 * to slow down the animation, if anything, while a value of anything higher
 * than that would indicate that the audio levels would speed things up.
 * 
 * The peak is converted to a percentage between 0% and 100%. Values less than
 * 50% will slow down the animation, while values above that will speed up the
 * animation. The actual calculation is as follows:
 *     speed_up_range = MAX_VARIANCE - 1;
 *     slow_down_range = 1 - MIN_VARIANCE;
 *     if (peak === 50%) {
 *         multiplier = 1;
 *     } else if (peak < 50%) {
 *         multiplier = 1 - (peak * 2 * slow_down_range);
 *     } else { // > 50%
 *         multiplier = 1 + ((peak - 50%) * 2 * speed_up_range);
 *     }
 * So, the range between MIN and MAX is not evenly spread out. MIN to 1, and 1
 * to MAX are spread out evenly.
 * @default 2.0
 * @constant {number}
 * @see {AUDIO_PEAKS_MIN_VARIANCE_MULTIPLIER}
 */
const AUDIO_PEAKS_MAX_VARIANCE_MULTIPLIER = 8.0;

/*****************************************************************************
 *********************** END OF CONFIGURATION SETTINGS ***********************
 *****************************************************************************/

var BACKGROUND = [0, 0, 0];
var DOT_COLOR = [255, 255, 255, 1.0];
var TRAIL_OPACITY = 1.0;
var TRAIL_COLOR = [88, 0, 133, TRAIL_OPACITY];
var TRAIL_SATURATION_MIN = 100.0;
var TRAIL_SATURATION_MAX = 100.0;
var TRAIL_LUMINOSITY_MIN = 25.0;
var TRAIL_LUMINOSITY_MAX = 75.0;
var HSL_DRIFT = 0.1;
var MIN_SPEED = 0.1;
var MAX_SPEED = 2.0;
var MIN_ACCEL = 0.01;
var MAX_ACCEL = 0.50;
var MAX_DOTS = 250;
var DOT_RATE = 2;
var FADE_OPACITY = 0.2;
var FPS = 30;
var FRAME_INTERVAL = 1000 / FPS;
var FRAME_INTERVAL_ = () => 1000 / FPS;
var LINE_WIDTH_MIN = 0.5;
var LINE_WIDTH_MAX = 3.0;
var LUMINOSITY_OSCILLATION_PERIOD_MIN = FPS * 0.5;
var LUMINOSITY_OSCILLATION_PERIOD_MIN_ = () => FPS * 0.5;
var LUMINOSITY_OSCILLATION_PERIOD_MAX = FPS * 1;
var LUMINOSITY_OSCILLATION_PERIOD_MAX_ = () => FPS * 1;
var LUMINOSITY_OSCILLATION_AMPLITUDE_MIN = 0.1;
var LUMINOSITY_OSCILLATION_AMPLITUDE_MAX = 25;
var LUMINOSITY_OSCILLATION_PHASE_SHIFT = 0;
var LINE_WIDTH_OSCILLATION_PERIOD_MIN = FPS * 0.5;
var LINE_WIDTH_OSCILLATION_PERIOD_MIN_ = () => FPS * 0.5;
var LINE_WIDTH_OSCILLATION_PERIOD_MAX = FPS * 1;
var LINE_WIDTH_OSCILLATION_PERIOD_MAX_ = () => FPS * 1;
var LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN = 0.1;
var LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX = 2.0;
var LINE_WIDTH_OSCILLATION_PHASE_SHIFT = 0;
var TRAIL_HSL_START = 180.0;
var TRAIL_HSL_END = 240.0;
var RESIZE_CANVAS_ON_WINDOW_RESIZE = false;
var ani;

class StatusElement {
    constructor(tbody, statrow) {
        this.__original = statrow;
        if (statrow.show !== undefined && !statrow.show) {
            return;
        }
        var trow = tbody.insertRow(-1);
        trow.classList.add("status-info-row");
        var c = trow.insertCell(-1);
        c.textContent = statrow.name;
        var type = statrow.type.split(/\./g).map((t) => {
            return t.split(/,/g);
        });
        this.pType = type;
        if (statrow.type !== "header") {
            c = trow.insertCell(-1);
            this.widget = c;
            this.owner = c;
            if (type[0][0] === "range") {
                this.craftStatusElement(type.slice(1)[0][0], 0);
                c = document.createElement("SPAN");
                c.classList.add("range-to");
                c.appendChild(document.createTextNode(this.sep));
                this.owner.appendChild(c);
                this.craftStatusElement(type.slice(1)[0][0], 1);
            } else {
                this.craftStatusElement(type[0][0], 0);
            }
        } else {
            trow.classList.add("header");
            c.setAttribute("colspan", "2");
        }
    }
    get name() {
        return this.__original.name;
    }
    get type() {
        return this.__original.type;
    }
    get unit() {
        return this.__original.unit;
    }
    get value() {
        var fn = this.__original.value;
        if (fn instanceof Function) {
            return (this.pType[0][0] !== "range")
                ? [fn()]
                : fn();
        }
        return undefined;
    }
    get format() {
        var fn = this.__original.format;
        if (fn instanceof Function) {
            return fn();
        }
        return undefined;
    }
    get sep() {
        return this.__original.sep || " to ";
    }
    get visible() {
        return this.__original.show === undefined || this.__original.show;
    }
    __updateInternal(types, parameter) {
        var value = this.value[parameter];
        var widget = this[`param${parameter}`];
        var main_type = types[0][0];
        var sub_types = types.slice(1);
        var sub_param = ((sub_types[0] || [])[parameter] || (sub_types[0] || [])[0]);
        switch (main_type) {
        case "color":
            switch (sub_param) {
            case "rgb":
                widget.style.backgroundColor = `rgb(${value.join(",")})`;
                widget.title = value.join(",");
                break;
            case "rgba":
                widget.style.backgroundColor = `rgba(${value.join(",")})`;
                widget.title = value.join(",");
                break;
            case "hsl":
                widget.style.backgroundColor = `hsl(${value.join(",")})`;
                widget.title = value.join(",");
                break;
            case "hsla":
                widget.style.backgroundColor = `hsla(${value.join(",")})`;
                widget.title = value.join(",");
                break;
            case "red":
                widget.style.backgroundColor = `rgb(${value},0,0)`;
                widget.title = value;
                break;
            case "blue":
                widget.style.backgroundColor = `rgb(0,0,${value})`;
                widget.title = value;
                break;
            case "green":
                widget.style.backgroundColor = `rgb(0,${value},0)`;
                widget.title = value;
                break;
            case "hue":
                widget.style.backgroundColor = `hsl(${value},100%,50%)`;
                widget.title = value;
                break;
            case "sat":
                widget.style.backgroundColor = `hsl(0,${value}%,50%)`;
                widget.title = value + "%";
                break;
            case "luma":
                widget.style.backgroundColor = `hsl(0,100%,${value}%)`;
                widget.title = value + "%";
                break;
            case "alpha":
                widget.style.backgroundColor = `rgba(255,0,0,${value})`;
                widget.title = value;
                break;
            }
            break;
        case "string":
            widget.textContent = value;
            break;
        case "number":
            value = parseFloat(value);
            switch (sub_param) {
            case "integer":
                widget.textContent = Math.round(value);
                break;
            case "decimal":
                widget.textContent = value;
                break;
            case "percentage":
                widget.textContent = `${(Math.round(value * 100 * 100) / 100)}%`;
                break;
            }
            break;
        case "flag":
            widget.textContent = value ? "YES" : "NO";
            break;
        case "custom":
            widget.textContent = value;
            break;
        }
    }
    craftStatusElement(type, parameter) {
        var widget = document.createElement("DIV");
        switch (type) {
        case "color":
            widget.classList.add("status-widget");
            widget.classList.add("color");
            break;
        case "string":
        case "number":
        case "flag":
            widget = document.createElement("SPAN");
            break;
        }
        this.owner.appendChild(widget);
        this[`param${parameter}`] = widget;
        var units = (this.__original.unit instanceof Array)
            ? this.__original.unit[parameter]
            : this.__original.unit;
        if (units) {
            widget = document.createElement("DIV");
            this.owner.appendChild(widget);
            widget.classList.add("status-widget");
            widget.classList.add("units");
            widget.innerHTML = "&nbsp;";
            widget.textContent += units;
        }
        return;
    }

    update() {
        if (!this.visible) {
            return;
        }
        if (this.type !== "header") {
            if (this.pType[0][0] !== "range") {
                this.__updateInternal(this.pType, 0);
            } else {
                this.__updateInternal(this.pType.slice(1), 0);
                this.__updateInternal(this.pType.slice(1), 1);
            }
        }
    }
}

class StatusElementCollection {
    constructor(output, rows) {
        console.info("creating rows!");
        var nrows = [];
        rows.forEach((row) => {
            nrows.push(new StatusElement(output, row));
        });
        this.rows = nrows;
    }
    update() {
        if (!ani.statusEnabled) {
            return; //don't update, save the frames; kill the animals.
        }
        this.rows.forEach((row) => {
            row.update();
        });
    }
}

class Dot {
    constructor(ani) {
        var vpb = Dot.rand(LUMINOSITY_OSCILLATION_PERIOD_MIN, LUMINOSITY_OSCILLATION_PERIOD_MAX);
        var vbpb = Dot.rand(LINE_WIDTH_OSCILLATION_PERIOD_MIN, LINE_WIDTH_OSCILLATION_PERIOD_MAX);
        //See the typedef of Dot for explanations of the following.

        //X-Coord: a random value between 0 and the size of the canvas.
        //Y-Coord: the very bottom of the canvas.
        //Speed: A random value between MIN/MAX SPEED.
        //Acceleration: A random value between MIN/MAX ACCELERATION.
        //Hue: A random value between TRAIL HSL START/END.
        //Luminosity: A random value between TRAIL LUMINOSITY START/END.
        //Saturation: A random value between TRAIL SATURATION START/END.
        //Legacy RGB color, deprecated in favor of HSL's greater control of color.
        //Creation Frame: current frame count.
        //Luminosity Oscillation Amplitude: random value in between MIN/MAX.
        //Luminosity Oscillation Frequency: the frequency when the period is a random value in between MIN/MAX.
        //Luminosity Oscillation Period
        //Luminosity Original Oscillation Period
        //Luminosity Oscillation Phase Shift: creation frame number + PHASE SHIFT
        //Line Width Oscillation Amplitude: random value in between MIN/MAX.
        //Line Width Oscillation Frequency: the frequency when the period is a random value in between MIN/MAX.
        //Line Width Oscillation Period
        //Line Width Original Oscillation Period
        //Line Width Oscillation Phase Shift: creation frame number + PHASE SHIFT
        //Line Width: a random value between MIN/MAX.
        //Sinusoidal frame helper value.
        //Sinusoidal frame helper value.
        //Old AUDIO_PEAK_MULTIPLIER.
        this.x = Dot.rand(0, ani.size.width);
        this.y = ani.size.height;
        this.s = Dot.rand(MIN_SPEED, MAX_SPEED);
        this.a = Dot.rand(MIN_ACCEL, MAX_ACCEL);
        this.c = Dot.rand(TRAIL_HSL_START, TRAIL_HSL_END);
        this.l = Dot.rand(TRAIL_LUMINOSITY_MIN, TRAIL_LUMINOSITY_MAX);
        this.sa = Dot.rand(TRAIL_SATURATION_MIN, TRAIL_SATURATION_MAX);
        this.c2 = [Dot.randInt(0, 255), Dot.randInt(0, 255), Dot.randInt(0, 255)];
        this.f = ani.frameCount;
        this.pa = Dot.rand(LUMINOSITY_OSCILLATION_AMPLITUDE_MIN, LUMINOSITY_OSCILLATION_AMPLITUDE_MAX);
        this.pb = Dot.getB(vpb);
        this.pp = vpb;
        this.opp = vpb;
        this.pc = ani.frameCount + LUMINOSITY_OSCILLATION_PHASE_SHIFT;
        this.bpa = Dot.rand(LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN, LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX);
        this.bpb = Dot.getB(vbpb);
        this.bpp = vbpb;
        this.obpp = vbpb;
        this.bpc = ani.frameCount + LINE_WIDTH_OSCILLATION_PHASE_SHIFT;
        this.w = Dot.rand(LINE_WIDTH_MIN, LINE_WIDTH_MAX);
        this.pfx = ani.frameCount;
        this.bpfx = ani.frameCount;
        this.oapm = 1;
        this.py = null;
        this.ppy = null;
        this.px = null;
        this.ppx = null;
        Dot.UpdateTrailDrift();
    }
    //getter
    get blah() {
        return "blah";
    }

    shiftRefPoints() {
        if (this.py === null) {
            this.px = this.x;
            this.py = this.y;
            return;
        }
        this.ppx = this.px;
        this.ppy = this.py;
        this.px = this.x;
        this.py = this.y;
    }
    updatePhaseShifts() {
        if (ani.audioPeakMultiplier !== this.oapm) {
            var np = (1 / ani.audioPeakMultiplier) * this.opp;
            var nbp = (1 / ani.audioPeakMultiplier) * this.obpp;
            this.pc = Dot.getNewPhaseShift(this.opp, np, this.pc, this.pfx);
            this.pb = Dot.getB(np);
            this.pp = np;
            this.bpc = Dot.getNewPhaseShift(this.obpp, nbp, this.bpc, this.bpfx);
            this.bpb = Dot.getB(nbp);
            this.bpp = nbp;
        }
        //we need to...
        //keep the original period and the last period/phaseshift
        //new period is the multiplier*original
    }
    updateSpeedAndPosition() {
        //Move the dot upwards based on the dot's speed.
        this.y -= this.s * ani.audioPeakMultiplier;
        //Increase the dot's speed based on its acceleration.
        this.s += this.a;// * AUDIO_PEAK_MULTIPLIER;
    }
    draw() {
        //Skip the first and second frames of the animation to retrieve the
        //second and third of the three points necessary for the animation
        if (this.mustShift) {
            this.shiftRefPoints();
            return;
        }

        //sin x = 2pi    b/2pi=period
        this.pfx = (this.pfx + 1) % this.pp;
        this.bpfx = (this.bpfx + 1) % this.bpp;

        //Set the line width
        ani.context.lineWidth = this.currentLineWidth;

        //Clear all preivous paths
        ani.context.beginPath();

        //Set the stroke and fill styles to the color of the current dot.
        ani.context.strokeStyle =
            ani.context.fillStyle = this.colorHSL;

        //Move to the oldest of the three reference points of the dot.
        ani.context.moveTo(this.rppx, this.rppy);
        //Make a line to the second oldest of the three reference points of the dot.
        ani.context.lineTo(this.rpx, this.rpy);
        //Draw the line.
        ani.context.stroke();

        //Make a line to the first (newest) of the three reference points of the dot
        ani.context.lineTo(this.rx, this.ry);
        //Draw the line.
        ani.context.stroke();

        //Shift the reference points and update the speed, position, and phase shifts.
        this.shiftRefPoints();
        this.updateSpeedAndPosition();
        //this.updatePhaseShifts(); //BUGGED

        //Reset the line width
        ani.context.lineWidth = 1;
    }
    get mustShift() {
        return (this.py === null) || (this.ppy === null);
    }
    get currentLuminosity() {
        //Determine the current effective luminosity of the dot based on the
        //current frame count
        return Dot.sinusoidal(this.pa, this.pb, this.pc, this.l);
    }
    get currentLineWidth() {
        //Determine the current effective line width of the dot based on the
        //current frame count
        return Dot.sinusoidal(this.bpa, this.bpb, this.bpc, this.w);
    }
    get colorHSL() {
        return `hsla(${this.c},${this.sa}%,${this.currentLuminosity}%,${TRAIL_OPACITY})`;
    }
    get offScreen() {
        return this.ppy < 0;
    }
    get rpx() {
        return Math.round(this.px);
    }
    get rppx() {
        return Math.round(this.ppx);
    }
    get rpy() {
        return Math.round(this.py);
    }
    get rppy() {
        return Math.round(this.ppy);
    }
    get rx() {
        return Math.round(this.x);
    }
    get ry() {
        return Math.round(this.y);
    }

    static UpdateTrailDrift() {
        //Drift the Hue range, by HSL_DRIFT
        TRAIL_HSL_START += HSL_DRIFT;
        TRAIL_HSL_END += HSL_DRIFT;

        //Bounds checking, make sure HSL_START/END are between 0 and 360.
        //doing this prevents the application from randomly failing when either
        //gets too large or too small.
        //The application should only break when the precision of FRAME_COUNT becomes
        //too small to keep track of each new frame, or when we overflow FRAME_COUNT into NaN
        if (TRAIL_HSL_START < 0) {
            TRAIL_HSL_START += 360;
        }
        if (TRAIL_HSL_END < 0) {
            TRAIL_HSL_END += 360;
        }
        TRAIL_HSL_START %= 360;
        TRAIL_HSL_END %= 360;
        if (TRAIL_HSL_START > TRAIL_HSL_END) {
            TRAIL_HSL_START -= 360;
        }
    }

    /**
        * A helper function used to get the new phase shift when changing the
        * oscillation speed of the dots.
        * See the comments below for more
        * 
        *
        * To modify the trajectory of a sine wave, the following must occur:
        *   - We must know the old period (eg: 1)
        *   - We must know the new period (eg: 1.5)
        *   - We must know the old phase shift (eg: 0)
        *     - The phase shift is what will allow us to alter the frequency
        *       of the sinusoid without altering the current position along
        *       it. IE: it means that the last value was 1, it'll be 1--even
        *       after the change in period.
        *       This is necessary because what we are doing is effectively
        *       shrinking the graph horizontally. Our position along the
        *       X-axis isn't going to shrink with us, so, we shift the
        *       graph to compensate for that.
        *
        * The calculation is:
        *     new_phase_shift = ((old_period - new_period)
        *         * (((frame - old_phase_shift) % old_period) / old_period)
        *         + old_phase_shift) % new_period
        * 2.5   2.5   5
        * old_period = 15
        * new_period = 20
        * old_phase_shift = 2.5
        * frame = 17.5
        *
        * Eg: if we have a sinusoid that repeats every 20 frames, that means
        * that the period of that sinusoid is 20.
        * Say we want to speed it up by 50%. How do we do that?
        * Well, we know that to speed it up by 75% the new period has to be 15
        * frames. If we assume that we weren't shifting the animation at all
        * before this, that means the previous phase shift was 0 frames.
        * So, if we are currently on frame 10, that means that the value of
        * the sinusoid is 0, with the graph curving positive for 5 frames,
        * then back to zero for another five frames before repeating from the
        * start.
        * IE:
        *              _______
        * |           /       \|
        * |\---------/---------| (repeat) (dashed line is x-axis)
        * | \_______/          |
        *  ^ frame 0      ^ frame 15
        *       ^ frame 5      ^ frame 20 (repeat)
        *            ^ frame 10
        * Without phase shifting, the new graph would look like this:
        *           _____          _____
        * |        /     \|       /     \|
        * |\------/-------\------/-------| (repeat) (dashed line is x-axis)
        * | \____/        |\____/        |
        *  ^ frame 0      ^ frame 15
        *       ^ frame 5      ^ frame 20 (repeat)
        *            ^ frame 10
        *
        * Now, frame 0 suddenly goes from a value of 0 to nearing 1.
        * If we don't rectify this, the animation will appear jittery as the
        * change in function will cause drastic and frequent alterations.
        * To fix this, we just need to shift the graph over until frame 10 has
        * the right value and the right direction. IE:
        *
        *  __          _____          _____
        * |  \|       /     \|       /     \|
        * |---\------/-------\------/-------| (repeat) (dashed line is x-axis)
        * |   |\____/        |\____/        |
        *  ^ frame 0      ^ frame 15
        *       ^ frame 5      ^ frame 20 (repeat)
        *            ^ frame 10
        * So: if the old period was 20, the new one is 15, and phase shift was 0
        * shift it by "(20 - 15) + 0" = "5" (it's actually 2.5...?)
        *
        * so, from 20 ==> 15 (frame 10), it's 2.5 (frame 10 is 50% to looping
        *    so 5 * 50% = 2.5)
        * from 20 ==> 15 ==> 20 (frame 17.5), it's -2.5 (that's at 0% to looping, so)
        *
        *  +   2.5,   2.5  + -12.5, -12.5 (or -12.5) (because halfway) (frame 10 0.5) 20>15 = 5
        *  + - 5.0, - 2.5  +  15.0,   2.5 (or +15) (frame 17.5 0.0/1.0) 15>20
        *  +   7.5,   5.0  + - 7.5, - 5.0 (or -7.5) (frame 27.5 0.5) 20>15   17.5%15 =2.5
        *  + -10.0, -10.0           (or +10) (frame 35 0.0/1.0) 15>20
        *
        *  12.5 17.5 25.0 35
        *
        *  if we mod by new period then subtract the phase shift
        * @function
        */
    static getNewPhaseShift(oldPeriod, newPeriod, oldPhaseShift, x) {
        return ((oldPeriod - newPeriod) * (
            ((x - oldPhaseShift) % oldPeriod) / oldPeriod)
            + oldPhaseShift) % newPeriod;
        //fuck this calculation...
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
    static sinusoidal(a, b, c, d) {
        return Dot.sinusoidal2(a, b, c, d, ani.frameCount);
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
     * @param {number} x - The x-value.
     * @returns {number} The value of y in the equation y = A * sin(B * (x - C)) + D
     * where x is equal to the the current [frame count]{@link FRAME_COUNT}
     */
    static sinusoidal2(a, b, c, d, x) {
        return a * Math.sin(b * (x - c)) + d;
    }


    /**
     * Generates a random floating-point number between "min" and "max".
     * If you need an integer instead, call the {@link randInt} function.
     * @function
     * @param {number} min - The inclusive lower bound of the random number.
     * @param {number} max - The exclusive upper bound of the random number.
     * @returns {number} The pseudorandom number that was generated.
     */
    static rand(min, max) {
        return (Math.random() * (max - min)) + min;
    }

    /**
     * Generates a random integer number between "min" and "max".
     * @function
     * @param {number} min - The inclusive lower bound of the random number.
     * @param {number} max - The exclusive upper bound of the random number.
     * @returns {number} The pseudorandom number that was generated.
     */
    static randInt(min, max) {
        return Math.floor((Math.random() * (max - min)) + min);
    }

    /**
     * Determines the frequency of a sinusoidal equation based on a given period.
     * @see {@link Sinusoid}
     * @function
     * @param {number} period - The period of the sinusoidal function.
     * @returns {number} The frequency of the sinusoidal function.
     */
    static getB(period) {
        return (2 * Math.PI) / period;
    }
}

class Animation {
    constructor() {
        var status_rows = [
            {
                "name": "General",
                "type": "header"
            }, {
                "name": "Canvas Size",
                "type": "range.number.integer",
                "unit": ["", "pixels"],
                "sep": " x ",
                "value": () => [ani.size.width, ani.size.height]
            }, {
                "name": "Version",
                "type": "string",
                "value": () => VERSION
            }, {
                "name": "Auto Resize",
                "type": "flag",
                "value": () => RESIZE_CANVAS_ON_WINDOW_RESIZE
            }, {
                "name": "Background",
                "type": "color.rgb",
                "value": () => BACKGROUND
            }, {
                "name": "Dot Color",
                "type": "color.rgba",
                "show": false,
                "value": () => DOT_COLOR
            }, {
                "name": "Frame Statistics",
                "type": "header"
            }, {
                "name": "Target FPS",
                "type": "string",
                "unit": "frames/second",
                "value": () => FPS.toFixed(2)
            }, {
                "name": "Achieved FPS",
                "type": "string",
                "unit": "frames/second",
                "value": () => (Math.round((ani.frameCount / (((new Date()).getTime() - ani.startTime) / 1000)) * 100) / 100).toFixed(2)
            }, {
                "name": "Frame Count",
                "type": "number.integer",
                "unit": "frames",
                "value": () => ani.frameCount
            }, {
                "name": "Dot Statistics",
                "type": "header"
            }, {
                "name": "Speed",
                "type": "range.number.decimal",
                "unit": ["", "pixels/second"],
                "value": () => [MIN_SPEED, MAX_SPEED]
            }, {
                "name": "Acceleration",
                "type": "range.number.decimal",
                "unit": ["", "pixels/second"],
                "value": () => [MIN_ACCEL, MAX_ACCEL]
            }, {
                "name": "Active Dots",
                "type": "range.number.integer",
                "unit": ["", "dots"],
                "sep": " of ",
                "value": () => [ani.dots.length, MAX_DOTS]
            }, {
                "name": "Dot Rate",
                "type": "number.integer",
                "unit": "dots/frame",
                "value": () => DOT_RATE
            }, {
                "name": "Trail Opacity",
                "type": "color.alpha",
                "value": () => TRAIL_OPACITY
            }, {
                "name": "Trail Color",
                "type": "color.rgba",
                "show": false,
                "value": () => TRAIL_COLOR
            }, {
                "name": "Trail Saturation",
                "type": "range.color.sat",
                "value": () => [TRAIL_SATURATION_MIN, TRAIL_SATURATION_MAX]
            }, {
                "name": "Trail Luminosity",
                "type": "range.color.luma",
                "value": () => [TRAIL_LUMINOSITY_MIN, TRAIL_LUMINOSITY_MAX]
            }, {
                "name": "Fade Opacity",
                "type": "number.percentage",
                "value": () => FADE_OPACITY
            }, {
                "name": "Line Width",
                "type": "range.number.decimal",
                "unit": ["", "pixels"],
                "value": () => [LINE_WIDTH_MIN, LINE_WIDTH_MAX]
            }, {
                "name": "Trail Hue Range",
                "type": "header"
            }, {
                "name": "Hue Drift",
                "type": "number.decimal",
                "unit": "degrees",
                "value": () => HSL_DRIFT
            }, {
                "name": "Current",
                "type": "range.color.hue",
                "value": () => [TRAIL_HSL_START, TRAIL_HSL_END]
            }, {
                "name": "Default",
                "type": "range.color.hue",
                "value": () => [DEFAULT_TRAIL_HSL_START, DEFAULT_TRAIL_HSL_END]
            }, {
                "name": "Luma Oscillation",
                "type": "header"
            }, {
                "name": "Period",
                "type": "range.string",
                "unit": ["", "seconds"],
                "format": (time) => { },
                "value": () => [
                    LUMINOSITY_OSCILLATION_PERIOD_MIN.toFixed(2),
                    LUMINOSITY_OSCILLATION_PERIOD_MAX.toFixed(2)
                ]
            }, {
                "name": "Amplitude",
                "type": "range.color.luma",
                "value": () => [
                    LUMINOSITY_OSCILLATION_AMPLITUDE_MIN,
                    LUMINOSITY_OSCILLATION_PERIOD_MAX
                ]
            }, {
                "name": "Phase Shift",
                "type": "string",
                "unit": "seconds",
                "format": (time) => { },
                "value": () => LUMINOSITY_OSCILLATION_PHASE_SHIFT.toFixed(2)
            }, {
                "name": "Line Width Oscillation",
                "type": "header"
            }, {
                "name": "Period",
                "type": "range.string",
                "unit": ["", "seconds"],
                "format": (time) => { },
                "value": () => [
                    LINE_WIDTH_OSCILLATION_PERIOD_MIN.toFixed(2),
                    LINE_WIDTH_OSCILLATION_PERIOD_MAX.toFixed(2)
                ]
            }, {
                "name": "Amplitude",
                "type": "range.number.decimal",
                "unit": ["", "pixels"],
                "value": () => [
                    LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN,
                    LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX
                ]
            }, {
                "name": "Phase Shift",
                "type": "string",
                "unit": "seconds",
                "format": (time) => { },
                "value": () => LINE_WIDTH_OSCILLATION_PHASE_SHIFT.toFixed(2)
            }
        ];
        this.dots = [];
        this.audioPeakMultiplier = 1;

        this.status = new StatusElementCollection(
            document.getElementById("status-table-body"),
            status_rows);
        this.statusEnabled = false;
    }
    start() {
        //Retrieve the CANVAS element
        this.canvas = document.querySelector("canvas");

        //Get a 2D drawing context for the canvas
        this.context = this.canvas.getContext("2d");

        //Get the size of the canvas, which should be stretched to the full size of
        //the window.
        this.size = this.canvas.getBoundingClientRect();

        //Set the width and height of the canvas internally, so that the canvas has
        //a 1 to 1 ratio between itself and the screen.
        this.canvas.setAttribute("width", this.width);
        this.canvas.setAttribute("height", this.height);

        //Clear all prior paths.
        this.context.beginPath();

        //Set the fill and stroke styles to the background color at full opacity.
        this.context.fillStyle = `rgba(${BACKGROUND.join(",")},1)`;
        this.context.strokeStyle = `rgba(${BACKGROUND.join(",")},1)`;

        //Fill the entire canvas with the current fill style.
        this.context.fillRect(0, 0, this.width, this.height);

        //Create a timer to start the animation.
        window.setTimeout(Animation.animate, FRAME_INTERVAL);
        this.status.update();
    }
    get width() {
        return Math.round(this.size.width);
    }
    get height() {
        return Math.round(this.size.height);
    }
    animate() {
        try {
            this.__animateInternal();
        } catch (e) {
            console.error(e);
        }
        //set a timer to run this same function, when we need to animate the next
        //frame.
        window.setTimeout(Animation.animate, FRAME_INTERVAL);
    }
    updateSize() {
        //Initialize variables.
        var i;
        var osize = this.size;

        //verify that resize is actually enabled
        if (RESIZE_CANVAS_ON_WINDOW_RESIZE) {
            //Get the size of the canvas, which should be stretched to the full size
            //of the window.
            this.size = this.canvas.getBoundingClientRect();

            //Set the width and height of the canvas internally, so that the canvas
            //has a 1 to 1 ratio between itself and the screen.
            this.canvas.setAttribute("width", this.width);
            this.canvas.setAttribute("height", this.height);

            //check to see if the canvas was made smaller, if not, don't check.
            if (osize.width > this.size.width) {
                //check all dots to see if they're still in bounds.
                for (i = 0; i < this.dots.length; i += 1) {
                    if (this.dots[i].x > this.size.width) {
                        //dot is out of bounds, remove it.
                        this.dots.splice(i--, 1);
                    }
                }
            }
        }
    }

    /**
     * A function that represents the computation required to complete a single
     * frame in the animation.
     * @function
     */
    __animateInternal() {
        this.frameCount = 0;
        if (!this.startTime) {
            this.startTime = (new Date()).getTime();
        }
        //Increment the frame counter.
        this.frameCount += 1;

        //Erase all previously recorded paths.
        this.context.beginPath();

        //Set the fill style and stroke style to the background color, at the
        //fade opacity
        this.context.fillStyle = `rgba(${BACKGROUND.join(",")},${FADE_OPACITY})`;
        this.context.strokeStyle = `rgba(${BACKGROUND.join(",")},${FADE_OPACITY})`;

        //Move the path to the origin of the canvas, (0,0), or the upper left corner
        //of the canvas.
        this.context.moveTo(0, 0);

        //Create a rectangle, offset by (0,0), the size of the entire canvas.
        this.context.rect(0, 0, this.width, this.height);

        //Fill the current path.
        this.context.fill();

        //Add new dots and then move all dots.
        this.addNewDots();
        this.moveAllDots();
    }

    /**
     * A helper function used to move all the dots.
     * @function
     */
    moveAllDots() {
        var i;

        //Iterate over every dot.
        this.dots.forEach((d) => {
            d.draw();
        });
        for (i = 0; i < this.dots.length; i += 1) {
            //Remove the current dot if it's off-screen
            if (this.dots[i].offScreen) {
                this.dots.splice(i--, 1);
            }
        }
    }

    addNewDots() {
        var i;
        for (i = 0; i < DOT_RATE * this.audioPeakMultiplier; i += 1) {
            if (this.dots.length >= MAX_DOTS) {
                //Can't add more dots.
                break;
            }
            //Add another dot and add it to the list.
            this.dots.push(new Dot(this));
        }
    }

    static animate() {
        ani.animate();
    }

    /**
     * A setup function, called when the page loads. It sets up the initial values
     * of the {@link canvas}, {@link context}, and {@link size} variables; sets up
     * the canvas, and begins the animation.
     * @function
     */
    static start() {

        //Shift, Control, OS, " ", Enter, Tab, F[1-12], Insert, Home, PageUp, PageDown
        //Delete, End, NumLock, CapsLock, Escape, ScrollLock, Pause, AudioVolumeMute,
        //AudioVolumeDown, AudioVolumeUp, ContextMenu
        var keybinds = [
            {
                "key": "e",
                "conditions": [{ "ctrl": false }],
                "handler": () => window.location.reload()
            }, {
                "key": "s",
                "conditions": [{ "ctrl": false }],
                "handler": () => ani.toggleStatus()
            }, {
                "key": "v",
                "conditions": [{ "ctrl": false }],
                "handler": () => ani.toggleVerboseStatus()
            }, {
                "key": "d",
                "conditions": [{ "ctrl": false }],
                "handler": () => console.info(`${ani.dots.length} active dot(s).`)
            }, {
                "key": "r",
                "conditions": [{ "ctrl": false }],
                "handler": () => ani.reset()
            }, {
                "key": "h",
                "conditions": [{ "ctrl": false }],
                "handler": () => ani.help()
            }, {
                "key": "+",
                "conditions": [{ "ctrl": false }],
                "handler": Animation.upFPS
            }, {
                "key": "-",
                "conditions": [{ "ctrl": false }],
                "handler": Animation.downFPS
            }, {
                "key": "_",
                "conditions": [{ "ctrl": false, "shift": true }],
                "handler": Animation.downFPS
            }, {
                "key": "=",
                "conditions": [{ "ctrl": false, "shift": false }],
                "handler": Animation.upFPS
            }
        ];

        var timeout = null;
        ani = new Animation();
        document.body.addEventListener("mousemove", () => {
            document.body.style.cursor = "default";
            if (timeout !== null) {
                window.clearTimeout(timeout);
            }
            timeout = window.setTimeout(() => {
                timeout = null;
                document.body.style.cursor = "none";
            }, 1000);
        });
        window.addEventListener("keyup", (e) => {
            var modifiers = ["ctrl", "alt", "shift", "meta"];
            keybinds.forEach((binding) => {
                if (binding.key !== e.key) {
                    return;
                }
                var keys = Object.keys(binding);
                if (keys.indexOf("conditions") !== -1) {
                    if (binding.conditions.some((cond) => {
                        keys = Object.keys(cond);
                        return !modifiers.some((mod) => {
                            return (keys.indexOf(mod) !== -1) && (e[mod + "Key"] === cond[mod]);
                        });
                    })) {
                        return;
                    }
                }

                binding.handler(e);
            });
        });
        window.addEventListener("resize", () => {
            ani.updateSize();
        });
        //check if peaks app is enabled for more dynamic animations.
        if (PEAKS_APP_ENABLED) {
            try {
                ani.loadPeaksApp();
            } catch (e) {
                console.log("Failed to load peaks app: ", e);
            }
        }
        ani.start();
    }

    /**
     * A function that loads the audio peaks subsystem to enable dynamic animations
     * that respond to the audio levels on your computer.
     * @function
     */
    loadPeaksApp() {
        //peaks socket
        var socket;
        function connect() {
            var reconnect = false;
            socket = new WebSocket(`ws${PEAKS_APP_SECURE ? "s" : ""}://` +
                `${PEAKS_APP_DOMAIN}:${PEAKS_APP_PORT}/AudioPeaks`);
            socket.addEventListener("open", function () {
                console.info("Connected to the Audio peaks sever successfully.");
            });
            socket.addEventListener("error", function (e) {
                console.error("Failed to connect to the audio peaks server: ", e);
                if (!reconnect) {
                    console.info(`Trying again in ${PEAKS_APP_ERROR_RECONNECT_WAIT / 1000} seconds.`);
                    window.setTimeout(connect, PEAKS_APP_ERROR_RECONNECT_WAIT);
                    reconnect = true;
                }
                ani.audioPeakMultiplier = 1;
            });
            socket.addEventListener("close", function () {
                console.info("Lost the connection to the audio peaks server.");
                if (!reconnect) {
                    console.info(`Trying again in ${PEAKS_APP_RECONNECT_WAIT / 1000} seconds.`);
                    window.setTimeout(connect, PEAKS_APP_RECONNECT_WAIT);
                    reconnect = true;
                }
                ani.audioPeakMultiplier = 1;
            });
            socket.addEventListener("message", function (e) {
                var message = JSON.parse(e.data);
                switch (message.status) {
                    case "Success":
                        var peak = message.data.max;
                        if (peak === 0.5) {
                            ani.audioPeakMultiplier = 1;
                        } else if (peak < 0.5) {
                            ani.audioPeakMultiplier = 1 - ((0.5 - peak) * 2 * (1 -
                                AUDIO_PEAKS_MIN_VARIANCE_MULTIPLIER));
                        } else { // > 0.5
                            ani.audioPeakMultiplier = 1 + ((peak - 0.5) * 2 * (
                                AUDIO_PEAKS_MAX_VARIANCE_MULTIPLIER - 1));
                        }
                        break;
                    case "Error":
                        console.error("Audio peaks server encountered an error: ", message.data);
                        break;
                }
                /**
                 * The audio peaks message data structure. This is the format the
                 * audio peaks server sends data.
                 * @typedef {Object} AudioPeakMessage
                 * @property {string} status - 
                 *     Indicates the status of the message. See
                 *     {@link AudioPeakMessageStatus}, possible values of this
                 *     property.
                 * @property {AudioPeaks|Object} data -
                 *     The data of the message. This will either be a
                 *     {@link AudioPeaks} object if {@link AudioPeakMessage#status}
                 *     is "Success", or a C# Exception object if it is "Error".
                 */
                /**
                 * The audio peaks message data structure. This is the format the
                 * audio peaks server sends data. There are three defined statuses:
                 *   Success - THe message contains "valid" audio data.
                 *   Info - Currently unused.
                 *   Error - The server encountered an error.
                 * @typedef {string} AudioPeakMessageStatus
                 */
                /**
                 * The raw peak data from the WinAudioLevels.exe server.
                 * @typedef {Object} AudioPeaks
                 * @property {Number[]} peaks -
                 *     The collection of audio peaks detected. Each element is the
                 *     detected peak of a separate audio device.
                 * @property {Number} max - The largest audio peak detected.
                 * @property {Number} min - The smallest audio peak detected.
                 * @property {Number} avg -
                 *     The average of all the audio peaks detected.
                 */
            });
        }
        //connect
        connect();
    }

    static updateFPS(_fps) {
        FPS = _fps;
        FRAME_INTERVAL = FRAME_INTERVAL_();
        LUMINOSITY_OSCILLATION_PERIOD_MAX = LUMINOSITY_OSCILLATION_PERIOD_MAX_();
        LUMINOSITY_OSCILLATION_PERIOD_MIN = LUMINOSITY_OSCILLATION_PERIOD_MIN_();
        LINE_WIDTH_OSCILLATION_PERIOD_MAX = LINE_WIDTH_OSCILLATION_PERIOD_MAX_();
        LINE_WIDTH_OSCILLATION_PERIOD_MIN = LINE_WIDTH_OSCILLATION_PERIOD_MIN_();
    }
    static upFPS() {
        Animation.updateFPS(FPS + 5);
        console.info(`Now targeting ${FPS} frames per second.`);
    }
    static downFPS() {
        var old_fps = FPS;
        Animation.updateFPS(Math.max(FPS - 5, 5));
        if (old_fps !== FPS) {
            console.info(`Now targeting ${FPS} frames per second.`);
        } else {
            console.info(`Cannot reduce the framerate any lower than five frames per second.`);
        }
    }

    reset() {
        this.startTime = (new Date()).getTime();
        this.dots = [];
        this.frameCount = 0;
        TRAIL_HSL_END = DEFAULT_TRAIL_HSL_END;
        TRAIL_HSL_START = DEFAULT_TRAIL_HSL_START;
        //Clear all prior paths.
        this.context.beginPath();

        //Set the fill and stroke styles to the background color at full opacity.
        this.context.fillStyle = `rgba(${BACKGROUND.join(",")},1)`;
        this.context.strokeStyle = `rgba(${BACKGROUND.join(",")},1)`;

        //Fill the entire canvas with the current fill style.
        this.context.fillRect(0, 0, this.width, this.height);
    }
    toggleStatus() {
        this.statusEnabled = !this.statusEnabled;
        if (this.statusEnabled) {
            document.getElementById("status-info").style = "";
            console.info("Turned on the status info overlay.");
            this.statusInterval = window.setInterval(() => {
                this.status.update();
            }, 10);
        } else {
            document.getElementById("status-info").style.display = "none";
            console.info("Turned off the status info overlay.");
            if (this.statusInterval) {
                window.clearInterval(this.statusInterval, 10);
                window.statusInterval = undefined;
            }
        }
    }
    toggleVerboseStatus() {
        this.verbose = !this.verbose;
        console.info(`Now ${this.verbose ? "displaying" : "hiding"} verbose information on the status info overlay.`);

    }
    help() {
        console.clear();
        console.info("KEYBINDINGS:");
        console.info("r(e)fresh -- Refreshes the page.");
        console.info("(h)elp    -- Displays this mesage.");
        console.info("(s)tatus  -- Toggles the visibility of the status info overlay.");
        console.info("(v)erbose -- Toggles the verbosity of the status info overlay.");
        console.info("(r)eset   -- Resets the animation.");
        console.info("(+)       -- Increase the FPS by five frames per second.");
        console.info("(-)       -- Decrease the FPS by five frames per second.");
    }
}

if (document.readyState !== "complete") {
    window.addEventListener("load", Animation.start);
} else {
    Animation.start();
}

/*
class Settings {
    constructor(local_settings) {
        this.__internal__settings = local_settings;
    }
    get area() { }
    calcArea() { }
    set blah(value) { }
    static method() { }
    static __constructor() { }
}
Settings.__constructor(); //basically emulating static constructors
*/

/**
 * @todo Add settings rows for the audio peaks settings
 * @todo Add in keybinds to enable/disable audio peaks
 * @todo Store settings in localStorage or IDB to use later on (see commented out code at top)
 * @todo Complete documentation.
 */