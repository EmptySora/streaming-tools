"use strict";
/**
 * @file Produces an animation that vaguely resembles rain falling upwards.
 * @author EmptySora_
 * @version 2.1.7.22
 * @license CC-BY 4.0
 * This work is licensed under the Creative Commons Attribution 4.0
 * International License. To view a copy of this license, visit
 * http://creativecommons.org/licenses/by/4.0/ or send a letter to Creative
 * Commons, PO Box 1866, Mountain View, CA 94042, USA.
 */
const VERSION = "2.1.7.22";

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
 * Img: {@link https://learnui.design/blog/img/hsb/hsb-cone-and-hsl-dicone.png}
 *
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
 * {@link onlinemathlearning.com/image-files/transformation-trig-graphs.png}
 * You can also play with the graphs of sine functions on
 * [graph.tk]{@link graph.tk} as well. In the upper right is a "?" button,
 * click there if you need help, as that article explains how to type in the pi
 * symbol.
 *
 * @typedef {undefined} Sinusoid
 */
/**
 *
 * @typedef {Object} KeyBindingInfo
 * @property {Key} key
 *     The name of the key being pressed, if this is a letter or value that
 *     changes with the shift key, the actual value here depends on whether or
 *     not the shift key is pressed.
 * @property {KeyBindingCondition[]} conditions
 *     An Array of {@link KeyBindingCondition} objects that describe
 *     conditionals required to trigger the binding. Only one of the conditions
 *     needs to be met.
 * @property {Function} handler
 *     The event handler for the key binding.
 */
/**
 * Conditions that must be met in order to run the key binding. All specified
 * properties must have their respective keys set to the specified state.
 * @typedef {Object} KeyBindingCondition
 * @property {boolean} [ctrl]
 *     Required state of the CTRL key.
 * @property {boolean} [alt]
 *     Required state of the ALT key.
 * @property {boolean} [meta]
 *     Required state of the META key.
 * @property {boolean} [shift]
 *     Required state of the SHIFT key.
 */
/**
 * Represents the delegate that is called to invoke the "value" property.
 * @callback StatusElementPropertiesValueCallback
 * @returns {object|object[]}
 *     The value of the StatusElement, or an array of such values if necessary.
 */
/**
 * Represents the properties of a particular status element.
 * @typedef {Object} StatusElementProperties
 * @property {string} name
 *     The text that is displayed on the HUD for this StatusElement. This
 *     property is required.
 * @property {string} type
 *     The type of StatusElement this is. The valid types are below:
 *     + string
 *         Indicates that the value is a textual value.
 *     + range.*
 *         Indicates that the value is a range between two values. The asterisk
 *         is another valid type value.
 *     + header
 *         Indicates that the row is a header row that doesn't have a value.
 *         All properties aside from {@link StatusElementProperties#show} and
 *         {@link StatusElementProperties#name} will be ignored.
 *     + number.[integer|decimal|percentage]
 *         Indicates that the value is a numeric value.
 *         - The "integer" subtype indicates that only whole numbers are valid.
 *         - The "decimal" subtype indicates that all numbers are valid.
 *         - The "percentage" subtype indicates that the value will be displayed
 *           as a percentage.
 *     + color.[rgb|rgba|hsl|hsla|red|blue|green|hue|sat|luma|alpha]
 *         Indicates that the value is a color.
 *         The subtypes are as follows:
 *         - rgb / rgba
 *             Specifies that color is an array of 3 to 4 numbers:
 *             [red, green, blue, alpha]
 *         - hsl / hsla
 *             Specifies that color is an array of 3 to 4 numbers:
 *             [hue, saturation, luminosity, alpha]
 *         - red / green / blue / hue / sat / luma
 *             Specifies that the color is a single number for the specified
 *             RGBA or HSLA component.
 *     + flag
 *         Indicates that the value is a boolean value that can be either true
 *         or false.
 *     + custom
 *         Indicates that the value is a custom value that has no set format.
 *         Generally, this will be treated like "string", except the value is
 *         not necessarily a string value.
 * @property {string|string[]} [unit]
 *     Either a single string value or an array of string values that is
 *     displayed beside the value of the StatusElement and acts as the units.
 *     Using an array of two strings and having the first string be empty is a
 *     valid use case (eg: a range of pixels. Instead of ## pixels ## pixels,
 *     you can use ["","pixels"] with the {@link StatusElementProperties#sep}
 *     property to get like ## x ## pixels).
 * @property {StatusElementPropertiesValueCallback} [value]
 *     A callback function that is used to retrieve the value of the property.
 * @property {boolean} [show]
 *     Gets whether or not the property should be shown. This defaults to true,
 *     obviously.
 *     As such, this property is generally omitted.
 * @property {boolean} [verbose]
 *     Gets whether or not the property is considered to provide verbose
 *     information. Defaults to false.
 * @property {string} [sep]
 *     A string value that is used to separate the values in a range. Defaults
 *     to an empty string.
 * @see {@link StatusElement}
 */
/**
 * Represents the settings for a particular {@link StatusElementCollection}.
 * @typedef {Object} StatusCollectionSettings
 * @property {string} [title]
 *     The text that is displayed as the title for the collection. EG:
 *     "Status".
 * @property {string} [itemText]
 *     The custom text that is displayed for the header of the "Item" column.
 *     This defaults to "Item".
 * @property {string} [valueText]
 *     The custom text that is displayed for the header of the "Value" column.
 *     This defaults to "Value".
 * @property {StatusElementProperties[]} rows
 *     The StatusElementProperties that make up this collection.
 * @property {boolean} [enableUpdate]
 *     Whether or not the application should periodically update the values of
 *     this collection. This defaults to "true".
 * @property {string} customCSS
 *     Custom CSS that is applied to the "status-info-container" element upon
 *     creation. You should supply one out of each of the following pairs of
 *     CSS properties:
 *       - top / bottom
 *       - left / right
 * @see {@link StatusElement}
 * @see {@link StatusElementCollection}
 */
/**
 * Represents the properties allowed to be passed to
 * {@link StatusElementCollection~createElement}.
 * @typedef {Object} CreateElementProperties
 * @property {string[]} [classes]
 *     The list of classes to apply to the element.
 * @property {Object} [attributes]
 *     An object containing the named attributes to apply to the element.
 * @property {string} [text]
 *     The textual content to apply to the element.
 * @property {string} [style]
 *     A custom CSS string to apply to the element.
 * @property {Node[]} [children]
 *     A list of nodes to append to the element.
 */
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
 *   Success - The message contains "valid" audio data.
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
/**
 * The object that represents the settings for a particular key-binding.
 * @typedef {Object} KeyBinding
 * @property {string} key
 *     The key code of the key required to activate this key-binding.
 *     This should match the value supplied by the
 *     {@link KeyboardEvent#key} property. For typical keys, this will be
 *     the value of the key if you pressed it under those conditions. Eg: for
 *     the "A" key, this will be "a". and for the "7"/"&" key, this will be
 *     "7".
 *     To explicitly match "&" in the last example, you would specify the
 *     key to be "&" and ensure all "condition" objects specified contain a
 *     key that indicates that the shift-key is held down.
 *     Go to the following link to see a list of all the special key code
 *     values:
 *     https://preview.tinyurl.com/s3kwy7fa (links to MDN)
 *     (link was originally too long to fit on one line... sadface)
 *
 *     Here's a quick list (capitalization included):
 *     Shift, Control, OS, " ", Enter, Tab, F[1-12], Insert, Home, PageUp,
 *     PageDown, Delete, End, NumLock, CapsLock, Escape, ScrollLock, Pause,
 *     AudioVolumeMute, AudioVolumeDown, AudioVolumeUp, ContextMenu
 * @property {boolean} [enabled]
 *     A property that determines whether or not the key-binding is enabled
 *     and can be used by the user. This property will default to "true" if
 *     it is not present.
 * @property {KeyBindingCallback} handler
 *     A callback that is run if the conditions of this key-binding are
 *     met.
 * @property {KeyBindingCondition[]} [conditions]
 *     The optional array of conditions to be additionally evaluated when
 *     determining if this key-binding has been triggered. If present, and
 *     there is at least one element, the application will evaluate each
 *     element individually and, if at least one condition matches, then
 *     the key-binding will be triggered.
 *     IE: only one of the specified conditions needs to be met, not all.
 * @property {string} [description]
 *     A textual description shown on-screen in the Help overlay that describes
 *     this key-binding.
 * @property {string} [group]
 *     A textual group used to categorize key-bindings on the Help overlay.
 * @property {boolean} [display]
 *     A bolean value that indicates whether or not the key-binding is
 *     displayed on the Help overlay. Defaults to "true".
 * @since 2.1.7.18
 */
/**
 * The object that represents the additional constraints required to
 * trigger a key-binding.
 * @typedef {Object} KeyBindingCondition
 * @property {boolean} ctrl
 *     If present, specifies the mandatory state of the Ctrl key during the
 *     key-press. (Omitting this will specify that the binding will activate
 *     regardless if the Ctrl key is held during the key-press).
 * @property {boolean} alt
 *     If present, specifies the mandatory state of the Alt key during the
 *     key-press. (Omitting this will specify that the binding will activate
 *     regardless if the Alt key is held during the key-press).
 * @property {boolean} shift
 *     If present, specifies the mandatory state of the Shift key during the
 *     key-press. (Omitting this will specify that the binding will activate
 *     regardless if the Shift key is held during the key-press).
 * @property {boolean} meta
 *     If present, specifies the mandatory state of the Meta key during the
 *     key-press. (Omitting this will specify that the binding will activate
 *     regardless if the Meta key is held during the key-press).
 *     The "meta" key is the "Windows" key on Windows computers, and the
 *     "⌘" key on Macintosh computers.
 * @since 2.1.7.18
 */
/**
 * Represents the delegate that is called to invoke a particular
 * key-binding when its conditions are satisfied.
 * @callback KeyBindingCallback
 * @since 2.1.7.18
 */
/**
 * The object that represents the settings for the application
 * @typedef {Object} SettingsObject
 * @property {SettingsTrailObject} trail
 *     An object that describes the various settings related to the trail of
 *     the individual dots in the animation.
 * @property {SettingsSpeedObject} speed
 *     An object that describes the minimum and maximum speed of the dots in
 *     the animation.
 * @property {SettingsAccelObject} accel
 *     An object that describes the minimum and maximum acceleration of the
 *     dots in the animation.
 * @property {SettingsLumObject} luminosity
 *     An object that describes the various settings related to the luminosity
 *     of the individual dots in the animation.
 * @property {SettingsBgObject} background
 *     An object that describes the various settings related to the background
 *     color and fade rate of the animation.
 * @property {number} fps
 *     A number that determines how many frames-per-second the animation runs
 *     at. Values lower than 20 will generally cause stuttering while values
 *     larger than 30 may result in computational lag depending on the other
 *     settings.
 *
 *     DEFAULT: 30
 *     RECOMMENDED: 20, 30, 60
 * @property {SettingsLineWidthObject} lineWidth
 *     An object that describes the various settings related to the line width
 *     of the individual dots in the animation.
 * @property {SettingsPeaksObject} peaks
 *     An object that describes the various settings related to the AudioPeaks
 *     subsystem of the animation.
 * @property {SettingsDotsObject} dots
 *     An object that describes the general settings related to the dots in the
 *     animation.
 * @property {boolean} resize
 *     A boolean value that determines whether or not the canvas is
 *     automatically resized whenever the window is resized.
 *     While the canvas *is* stretched to the full size of the window, the
 *     actual size of the canvas (internally) is set when the page first loads.
 *     As a result, if you were to resize the window after the page loads, this
 *     ends up causing the canvas to scale up or down to the new window size.
 *     This is generally more noticable the more extreme the window size is
 *     upon load and when changed.
 *     Setting this to true causes the application to update the internal
 *     canvas size whenever the window resizes.
 *     To really see this issue, load the animation when the window is really
 *     tiny and then reload, resizing the window to be very large.
 *
 *     DEFAULT: true
 * @since 2.1.7.19
 */
/**
 * An object that describes the various settings related to the trail of
 * the individual dots in the animation.
 * @typedef {Object} SettingsTrailObject
 * @property {SettingsTrailSatObject} saturation
 *     A collection of properties that describes the bounds for the saturation
 *     of the trail of the individual dots.
 * @property {SettingsTrailLumObject} luminosity
 *     A collection of properties that describes the bounds for the luminosity
 *     of the trail of the individual dots.
 * @property {Opacity} opacity
 *     A number ranging from 0.0 to 1.0 that represents the opacity of the
 *     trails the dots leave.
 *
 *     DEFAULT: 1.0
 * @property {SettingsTrailHueObject} hue
 *     A collection of properties that describes the initial hue range of the
 *     dots as well as how far that range drifts per dot.
 * @since 2.1.7.19
 */
/**
 * An object that describes the minimum and maximum speed of the dots in
 * the animation.
 * @typedef {Object} SettingsSpeedObject
 * @property {number} min
 *     The minimum speed in pixels-per-frame in which the dots move.
 *
 *     DEFAULT: 0.1
 * @property {number} max
 *     The maximum speed in pixels-per-frame in which the dots move.
 *
 *     DEFAULT: 2.0
 * @since 2.1.7.19
 */
/**
 * An object that describes the minimum and maximum acceleration of the
 * dots in the animation.
 * @typedef {Object} SettingsAccelObject
 * @property {number} min
 *     The minimum acceleration in pixels-per-frame-per-frame in which the dots
 *     move.
 *
 *     DEFAULT: 0.01
 * @property {number} max
 *     The maximum acceleration in pixels-per-frame-per-frame in which the dots
 *     move.
 *
 *     DEFAULT: 0.50
 * @since 2.1.7.19
 */
/**
 * An object that describes the various settings related to the luminosity
 * of the individual dots in the animation.
 * @typedef {Object} SettingsLumObject
 * @property {SettingsLumOscObject} oscillation
 *     A collection of properties that describes the parameters that oscillate
 *     the luminosities of every individual dot in the animation.
 * @since 2.1.7.19
 */
/**
 * An object that describes the various settings related to the background
 * color and fade rate of the animation.
 * @typedef {Object} SettingsBgObject
 * @property {ColorRGB} color
 *     The background color of the canvas.
 *
 *     DEFAULT: [0, 0, 0]
 * @property {Opacity} opacity
 *     The opacity at which active trails are erased.
 *     A lower value here will result in longer trails and vice versa.
 *     As a side-effect of this method, whenever this value is not set to 1.0
 *     (indicating that the trails instantly vanish), the path the trails will
 *     travel will never truly be set to the background color ever again.
 *     This is because the average of any number less than 1.0 will always
 *     remain less than 1.0. You might notice this as the dots begin to trail
 *     the canvas. They will seem to leave a persisten trail whose color is
 *     very slightly brighter or darker than the background color.
 *
 *     DEFAULT: 0.2
 * @since 2.1.7.19
 */
/**
 * An object that describes the various settings related to the line width
 * of the individual dots in the animation.
 * @typedef {Object} SettingsLineWidthObject
 * @property {number} min
 *     The minimum width of the trails the dots produce. This is effectively
 *     like the radius of a circle, meaning that the produced trail extends,
 *     both, to the left and right by this many pixels.
 *     IE: a value of 1.0 would actually take up 2-3 pixels, in reality.
 *
 *     DEFAULT: 0.5
 * @property {number} max
 *     The maximum width of the trails the dots produce. This is effectively
 *     like the radius of a circle, meaning that the produced trail extends,
 *     both, to the left and right by this many pixels.
 *     IE: a value of 1.0 would actually take up 2-3 pixels, in reality.
 *
 *     DEFAULT: 3.0
 * @property {SettingsLineWidthOscObject} oscillation
 *     A collection of properties that describes the parameters that oscillate
 *     the line widths of every individual dot in the animation.
 * @since 2.1.7.19
 */
/**
 * An object that describes the various settings related to the AudioPeaks
 * subsystem of the animation.
 * @typedef {Object} SettingsPeaksObject
 * @property {number} port
 *     The port on the server running WinAudioLevels.exe that is listening for
 *     WebSocket connections.
 *
 *     DEFAULT: 8069
 * @property {number} domain
 *     The domain of the server running WinAudioLevels.exe. If you are
 *     selfhosting, set this to one of: "127.0.0.1", "localhost", or "::1"
 *
 *     DEFAULT: "localhost"
 * @property {number} secure
 *     Whether or not the server running WinAudioLevels.exe only accepts secure
 *     connections.
 *     If you are selfhosting, most likely, this will be set to false.
 *     Otherwise, this must be set to true due to JS security restrictions.
 *
 *     DEFAULT: false
 * @property {number} enabled
 *     Whether or not to try connection to the audio peaks server at all. Set
 *     this to false if you are not running an audio peaks server.
 *
 *     DEFAULT: true
 * @property {SettingsPeaksReconnObject} reconnect
 *     The collection of properties that specify how long the application will
 *     wait before reconnecting to the AudioPeaks server.
 * @property {SettingsPeaksVarMulObject} varianceMultiplier
 *     The collection of properties that specify how much the audio peaks
 *     affect the animation.
 * @since 2.1.7.19
 */
/**
 * An object that describes the general settings related to the dots in the
 * animation.
 * @typedef {Object} SettingsDotsObject
 * @property {number} rate
 *     The rate at which new dots are added to the animation.
 *     Setting this value to a value higher than
 *     {@link SettingsObject.dots.max} will not pose any issues.
 *
 *     DEFAULT: 2
 * @property {number} max
 *     The maximum number of dots that can be concurrently active at any one
 *     time during the animation. If you set this to a high value, your
 *     processor and/or GPU might have trouble keeping up with the physics of
 *     each particle.
 *
 *     DEFAULT: 250
 * @since 2.1.7.19
 */
/**
 * A collection of properties that describes the bounds for the saturation
 * of the trail of the individual dots.
 * @typedef {Object} SettingsTrailSatObject
 * @property {Saturation} min
 *     The minimum saturation allowed for trail components.
 *
 *     DEFAULT: 100.0
 * @property {Saturation} max
 *     The maximum saturation allowed for trail components.
 *
 *     DEFAULT: 100.0
 * @since 2.1.7.19
 */
/**
 * A collection of properties that describes the bounds for the luminosity
 * of the trail of the individual dots.
 * @typedef {Object} SettingsTrailLumObject
 * @property {Luminosity} min
 *     The minimum luminosity allowed for trail components.
 *
 *     DEFAULT: 25.0
 * @property {Luminosity} max
 *     The maximum luminosity allowed for trail components.
 *
 *     DEFAULT: 75.0
 * @since 2.1.7.19
 */
/**
 * A collection of properties that describes the initial hue range of the
 * dots as well as how far that range drifts per dot.
 * @typedef {Object} SettingsTrailHueObject
 * @property {number} drift
 *     The rate at which the average hue of the dots shifts around the color
 *     wheel.
 *     This value has a period of 360, meaning that, if the value is over 360,
 *     it will effectively shift it by "trail.hue.drift MOD 360".
 *     Eg: setting this to 475 is identical to setting this to 115.
 *     (since 475 - 360 = 115)
 *     This value *can* be negative (to indicate shifting color backwards).
 *
 *     DEFAULT: 0.1
 * @property {Hue} start
 *     Represents the initial minimum hue, in degrees, of the dots in the
 *     animation as rotated around the color wheel.
 *     The larger the difference between {@link SettingsObject.trail.hue.start}
 *     and {@link SettingsObject.trail.hue.end}, the more variation in color
 *     the dots will display.
 *     Setting the range to anything larger than or equal to 360 will
 *     effectively prevent the colors from cycling at all (and all hues will
 *     be available.)
 *
 *     DEFAULT: 180.0
 * @property {Hue} end
 *     Represents the initial maximum hue, in degrees, of the dots in the
 *     animation as rotated around the color wheel.
 *     The larger the difference between {@link SettingsObject.trail.hue.start}
 *     and {@link SettingsObject.trail.hue.end}, the more variation in color
 *     the dots will display.
 *     Setting the range to anything larger than or equal to 360 will
 *     effectively prevent the colors from cycling at all (and all hues will
 *     be available.)
 *
 *     DEFAULT: 240.0
 * @since 2.1.7.19
 */
/**
 * A collection of properties that describes the parameters that oscillate
 * the luminosities of every individual dot in the animation.
 * @typedef {Object} SettingsLumOscObject
 * @property {SettingsLumOscAmpObject} amplitude
 *     The collection of properties that specify the range in which the
 *     amplitude of the luminosity oscillation can vary by.
 * @property {number} phaseShift
 *     The phase shift of the luminosity oscillation relative to the start of
 *     the oscillation.
 *     This value will likely be converted to a "MIN"/"MAX" so that it may be
 *     randomized, as having this static does basically nothing.
 *
 *     DEFAULT: 0
 * @property {SettingsLumOscPerFactObject} periodFactor
 *     The collection of properties that specify the range in which the
 *     period of the luminosity oscillation can vary by (in conjunction with
 *     the current FPS).
 * @since 2.1.7.19
 */
/**
 * A collection of properties that describes the parameters that oscillate
 * the line widths of every individual dot in the animation.
 * @typedef {Object} SettingsLineWidthOscObject
 * @property {SettingsLineWidthOscAmpObject} amplitude
 *     The collection of properties that specify the range in which the
 *     amplitude of the line width oscillation can vary by.
 * @property {number} phaseShift
 *     The phase shift of the line width oscillation relative to the start of
 *     the oscillation.
 *     This value will likely be converted to a "MIN"/"MAX" so that it may be
 *     randomized, as having this static does basically nothing.
 *
 *     DEFAULT: 0
 * @property {SettingsLineWidthOscPerFactObject} periodFactor
 *     The collection of properties that specify the range in which the
 *     period of the line width oscillation can vary by (in conjunction with
 *     the current FPS).
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify how long the application will
 * wait before reconnecting to the AudioPeaks server.
 * @typedef {Object} SettingsPeaksReconnObject
 * @property {number} normal
 *     How long to wait, in milliseconds, before attempting to reconnect to the
 *     audio peaks server after a disconnection occurs.
 *
 *     DEFAULT: 5000
 * @property {number} error
 *     How long to wait, in milliseconds, before attempting to reconnect to the
 *     audio peaks server after an error occurs.
 *
 *     DEFAULT: 30000
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify how much the audio peaks
 * affect the animation.
 * @typedef {Object} SettingsPeaksVarMulObject
 * @property {number} min
 *     The minimum multiplier the audio levels on the computer can affect the
 *     animation by. This should be between 0.0 and 1.0.
 *     A value of 0.0 would specify that dead silence would effectively halt
 *     the animation, while a value of 1.0 would indicate that the audio peaks
 *     would only be able to speed up the animation, if anything.
 *
 *     The peak is converted to a percentage between 0% and 100%. Values less
 *     than 50% will slow down the animation, while values above that will
 *     speed up the animation.
 *     So, the range between MIN and MAX is not evenly spread out. MIN to 1,
 *     and 1 to MAX are spread out evenly.
 *
 *     DEFAULT: 0.125
 * @property {number} max
 *     The maximum multiplier the audio levels on the computer can affect the
 *     animation by. This should be greater than or equal to 1.0.
 *     A value of 1.0 would indicate that the audio levels would only be able
 *     to slow down the animation, if anything, while a value of anything
 *     higher than that would indicate that the audio levels would speed things
 *     up.
 *
 *     The peak is converted to a percentage between 0% and 100%. Values less
 *     than 50% will slow down the animation, while values above that will
 *     speed up the animation.
 *     So, the range between MIN and MAX is not evenly spread out. MIN to 1,
 *     and 1 to MAX are spread out evenly.
 *
 *     DEFAULT: 8.000
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify the range in which the
 * amplitude of the luminosity oscillation can vary by.
 * @typedef {Object} SettingsLumOscAmpObject
 * @property {number} min
 *     The minimum variation in luminosity the dot should oscillate to/from.
 *     This value is relative to the luminosity of the dot.
 *
 *     DEFAULT: 0.1
 * @property {number} max
 *     The maximum variation in luminosity the dot should oscillate to/from.
 *     This value is relative to the luminosity of the dot.
 *
 *     DEFAULT: 25.0
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify the range in which the
 * period of the luminosity oscillation can vary by (in conjunction with
 * the current FPS).
 * @typedef {Object} SettingsLumOscPerFactObject
 * @property {number} min
 *     The default min luminosity oscillation period factor that is used to
 *     determine the allowed oscillation period range based on the current FPS.
 *
 *     DEFAULT: 0.5
 * @property {number} max
 *     The default max luminosity oscillation period factor that is used to
 *     determine the allowed oscillation period range based on the current FPS.
 *
 *     DEFAULT: 1.0
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify the range in which the
 * amplitude of the line width oscillation can vary by.
 * @typedef {Object} SettingsLineWidthOscAmpObject
 * @property {number} min
 *     The minimum variation in line width the dot should oscillate to/from.
 *     This value is relative to the line width of the dot.
 *
 *     DEFAULT: 0.1
 * @property {number} max
 *     The maximum variation in line width the dot should oscillate to/from.
 *     This value is relative to the line width of the dot.
 *
 *     DEFAULT: 2.0
 * @since 2.1.7.19
 */
/**
 * The collection of properties that specify the range in which the
 * period of the line width oscillation can vary by (in conjunction with
 * the current FPS).
 * @typedef {Object} SettingsLineWidthOscPerFactObject
 * @property {number} min
 *     The default min line width oscillation period factor that is used to
 *     determine the allowed oscillation period range based on the current FPS.
 *
 *     DEFAULT: 0.5
 * @property {number} max
 *     The default max line width oscillation period factor that is used to
 *     determine the allowed oscillation period range based on the current FPS.
 *
 *     DEFAULT: 1.0
 * @since 2.1.7.19
 */

/**
 * Represents an individual settings element and provides methods and properties
 * for updating the HUD on such elements.
 */
class StatusElement {
    /**
     * The original {@link StatusElementProperties} object from which this
     * {@link StatusElement} is created from.
     * @type {StatusElementProperties}
     * @private
     */
    __original;
    /**
     * A double-array of string objects that represent the types and
     * subtypes of this {@link StatusElement}.
     * @type {string[][]}
     * @private
     */
    pType;
    /**
     * The {@link HTMLElement} that this {@link StatusElement} is
     * displayed via.
     * @type {HTMLElement}
     * @private
     */
    owner;
    /**
     * The first of the two parameter value Elements that the value of this
     * {@link StatusElement} is displayed via.
     * @type {HTMLElement}
     * @private
     */
    param0;
    /**
     * The second of the two parameter value Elements that the value of this
     * {@link StatusElement} is displayed via.
     * This property may not always be present
     * @type {HTMLElement}
     * @private
     */
    param1;
    /**
     * @type {boolean}
     * @private
     */
    isRange = false;
    /**
     * @type {boolean}
     * @private
     */
    isHeader = false;
    /**
     * @type {boolean}
     * @private
     */
    isVisible = false;
    /**
     * Creates a new {@link StatusElement} from the HTML node containing it and
     * the settings configuration information it is based upon.
     * @param {HTMLElement} tbody
     *     The element (not necessarily a TBODY element) that will contain this
     *     status element.
     * @param {StatusElementProperties} statrow
     *     An object with properties that describe the various aspects of the
     *     status element.
     */
    constructor(tbody, statrow) {
        this.__original = statrow;
        this.isVisible = typeof statrow.show === "undefined" || statrow.show;
        if (!this.isVisible) {
            return;
        }
        const trow = tbody.insertRow(-1);
        trow.classList.add("status-info-row");
        if (statrow.verbose) {
            trow.classList.add("status-info-row-verbose");
        }
        let c = trow.insertCell(-1);
        c.textContent = statrow.name;
        const type = statrow.type.split(/\./g).map((t) => t.split(/,/g));
        this.pType = type;
        if (statrow.type === "header") {
            this.isHeader = true;
            trow.classList.add("header");
            c.setAttribute("colspan", "2");
        } else {
            c = trow.insertCell(-1);
            this.owner = c;
            if (type[0][0] === "range") {
                this.isRange = true;
                this.pType = type.slice(1);
            }
            this.craftStatusElement(this.pType[0][0], 0);
            if (this.isRange) {
                c = this.owner.appendChild(document.createElement("SPAN"));
                c.classList.add("range-to");
                c.appendChild(
                    document.createTextNode(this.__original.sep || " to ")
                );
                this.craftStatusElement(this.pType[0][0], 1);
            }
        }
    }
    /**
     * Gets the value of the {@link StatusElement}.
     * @returns {Object} The value of the {@link StatusElement}.
     */
    get value() {
        return this.isRange
            ? this.__original.value()
            : [this.__original.value()];
    }

    /**
     * An internal method that is called to update the value of this
     * {@link StatusElement}.
     * @param {string[][]} types
     *     A double-array of string objects that represent the types and
     *     subtypes of this {@link StatusElement}.
     * @param {number} parameter
     *     A number that indicates which value (if there are multiple) is being
     *     updated.
     *     If there are not multiple values, this should be set to 0.
     */
    __updateInternal(types, parameter) {
        let value = this.value[parameter];
        const widget = this[`param${parameter}`];
        /* eslint-disable-next-line prefer-destructuring */
        const main_type = types[0][0];
        const subs = types.slice(1);
        const sub_param = (subs[0] || [])[parameter] || (subs[0] || [])[0];
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
                widget.title = `${value}%`;
                break;
            case "luma":
                widget.style.backgroundColor = `hsl(0,100%,${value}%)`;
                widget.title = `${value}%`;
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
                widget.textContent = `${Math.round(value * 10000) / 100}%`;
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

    /**
     * An internal method that is called to create the HTML necessary to render
     * this {@link StatusElement} in the HUD.
     * @param {string} type
     *     A string containing the types and subtypes of this
     *     {@link StatusElement}.
     * @param {number} parameter
     *     A number that indicates which value (if there are multiple) is being
     *     updated.
     *     If there are not multiple values, this should be set to 0.
     */
    craftStatusElement(type, parameter) {
        let widget = document.createElement("DIV");
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
        widget.classList.add("status-widget-parameter");

        this[`param${parameter}`] = widget;
        const units = this.__original.unit instanceof Array
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

    /**
     * Updates the value of this {@link StatusElement} and updates the HUD.
     */
    update() {
        if (!this.isVisible) {
            return;
        }
        if (!this.isHeader) {
            this.__updateInternal(this.pType, 0);
            if (this.isRange) {
                this.__updateInternal(this.pType, 1);
            }
        }
    }
}

/**
 * Represents a collection of settings elements and provides methods and
 * properties for updating the HUD on such collections of elements.
 */
class StatusElementCollection {
    /**
     * @type {boolean}
     * @private
     */
    __disposed = false;
    /**
     * @type {boolean}
     * @private
     */
    __enabled;
    /**
     * @type {number}
     * @private
     */
    __interval;
    /**
     * @type {HTMLElement}
     * @private
     */
    __container;
    /**
     * The list of {@link StatusElement} objects in this
     * {@link StatusElementCollection}.
     * @type {StatusElement[]}
     */
    rows;

    /**
     * Creates a new {@link StatusElementCollection} from the specified
     * settings object provided.
     * @param {StatusCollectionSettings} settings
     *     The settings used to initialize this collection.
     */
    constructor(settings) {
        /**
         * Creates an {@link HTMLElement} given its tag name, and the
         * list of properties provided
         * @param {string} name
         *     The tag name of the element to create.
         * @param {CreateElementProperties} obj
         *     The property list to attribute to the element.
         */
        function createElement(name, obj) {
            const element = document.createElement(name);
            const oKeys = Object.keys(obj);
            let keys;
            let i;
            if (oKeys.includes("classes")) {
                obj.classes.forEach((className) => {
                    element.classList.add(className);
                });
            }
            if (oKeys.includes("attributes")) {
                keys = Object.keys(obj.attributes);
                for (i = 0; i < keys.length; i += 1) {
                    element.setAttribute(
                        keys[i],
                        obj.attributes[keys[i]]
                    );
                }
            }
            if (oKeys.includes("text")) {
                element.textContent = obj.text;
            }
            if (oKeys.includes("style")) {
                element.style = obj.style;
            }
            if (oKeys.includes("children")) {
                obj.children.forEach((child) => {
                    element.appendChild(child);
                });
            }
            return element;
        }
        const {body} = document;
        const container = body.appendChild(createElement("DIV", {
            classes: ["status-info-container", "status-hide"],
            style: settings.customCSS || "",
            children: [
                createElement("CENTER", {
                    text: settings.title || "Status"
                }),
                createElement("HR", {
                    attributes: {
                        size: "+0",
                        color: "white"
                    }
                })
            ]
        }));
        const element = container
            .appendChild(document.createElement("TABLE"));
        element.classList.add("status-table");
        element
            .appendChild(document.createElement("THEAD"))
            .appendChild(document.createElement("TR"))
            .append(
                createElement("TH", {
                    text: settings.itemText || "Item"
                }),
                createElement("TH", {
                    text: settings.valueText || "Value"
                })
            );
        const output = element
            .appendChild(document.createElement("TBODY"));
        output.classList.add("status-table-body");

        console.info("creating rows!");
        const nrows = [];
        settings.rows.forEach((row) => {
            nrows.push(new StatusElement(output, row));
        });

        this.rows = nrows;
        this.__container = container;
        this.__enabled = !Object.keys(settings).includes("enableUpdate")
            || settings.enableUpdate;
        this.displayed = false;
        this.showVerbose = false;
    }

    /**
     * Updates the values of this {@link StatusElementCollection} and updates
     * the HUD.
     * @throws {Error} Object has been disposed already.
     */
    update() {
        if (this.__disposed) {
            throw new Error("Object disposed already!");
        }
        if (!this.displayed) {
            return; //Don't update, save the frames; kill the animals.
        }
        this.rows.forEach((row) => {
            try {
                row.update();
            } catch (e) {
                console.error("Error during update: ", e);
                //This will be run if AudioPeaks is not enabled (most likely).
            }
        });
    }

    /**
     * Gets whether or not the HUD for this {@link StatusElementCollection} is
     * displayed.
     * @throws {Error} Object has been disposed already.
     */
    get displayed() {
        if (this.__disposed) {
            throw new Error("Object disposed already!");
        }
        return !this.__container.classList.contains("status-hide");
    }
    /**
     * Sets whether or not the HUD for this {@link StatusElementCollection} is
     * displayed.
     * @throws {Error} Object has been disposed already.
     */
    set displayed(value) {
        if (this.__disposed) {
            throw new Error("Object disposed already!");
        }
        if (this.displayed === value) {
            return;
        }
        if (value) {
            this.__container.classList.remove("status-hide");
            if (this.__enabled) {
                this.__interval = window.setInterval(() => this.update(), 10);
            }
        } else {
            this.__container.classList.add("status-hide");
            if (this.__interval && this.__enabled) {
                window.clearInterval(this.__interval, 10);
                this.__interval = null;
            }
        }
    }
    /**
     * Gets whether or not the HUD for this {@link StatusElementCollection}
     * shows verbose information.
     * @throws {Error} Object has been disposed already.
     */
    get showVerbose() {
        if (this.__disposed) {
            throw new Error("Object disposed already!");
        }
        return !this.__container.classList.contains("status-hide-verbose");
    }
    /**
     * Sets whether or not the HUD for this {@link StatusElementCollection}
     * shows verbose information.
     * @throws {Error} Object has been disposed already.
     */
    set showVerbose(value) {
        if (this.__disposed) {
            throw new Error("Object disposed already!");
        }
        if (this.showVerbose === value) {
            return;
        }
        if (value) {
            this.__container.classList.remove("status-hide-verbose");
        } else {
            this.__container.classList.add("status-hide-verbose");
        }
    }
    /**
     * Gets rid of this {@link StatusElementCollection} and disposes of any
     * resources used.
     * @since 2.1.7.3
     */
    dispose() {
        if (this.__disposed) {
            return;
        }
        this.displayed = false;
        this.__container.parentElement.removeChild(this.__container);
    }
    /*
     * This is intended to be used such that we can remove an overlay
     * and recreate it (namely so we can modify keybindings on the fly)
     */
}

/**
 * Represents all of the properties that describe any individual dot used in
 * the animation.
 */
class Dot {
    /**
     * The x-coordinate of this {@link Dot}.
     *
     * Default: A random value between 0 and the size of the canvas.
     * @type {number}
     */
    x;
    /**
     * The y-coordinate of this {@link Dot}.
     *
     * Default: The very bottom of the canvas.
     * @type {number}
     */
    y;
    /**
     * The speed of this {@link Dot}.
     *
     * Default: A random value between {@link MIN_SPEED} and
     * {@link MAX_SPEED}.
     * @type {number}
     */
    s;
    /**
     * The acceleration of this {@link Dot}.
     *
     * Default: A random value between {@link MIN_ACCEL} and
     * {@link MAX_ACCEL}.
     * @type {number}
     */
    a;
    /**
     * The hue of this {@link Dot}.
     *
     * Default: A random value between {@link TRAIL_HSL_START} and
     * {@link TRAIL_HSL_END}.
     * @type {number}
     */
    c;
    /**
     * The luminosity of this {@link Dot}.
     *
     * Default: A random value between {@link TRAIL_LUMINOSITY_MIN} and
     * {@link TRAIL_LUMINOSITY_MAX}.
     * @type {number}
     */
    l;
    /**
     * The saturation of this {@link Dot}.
     *
     * Default: A random value between {@link TRAIL_SATURATION_MIN} and
     * {@link TRAIL_SATURATION_MAX}.
     * @type {number}
     */
    sa;
    /**
     * The frame this {@link Dot} was created on.
     *
     * Default: The value of {@link Ani.frameCount} at the time of creation.
     * @type {number}
     */
    f;
    /**
     * The amplitude of the sine wave that oscillates the luminosity of
     * this {@link Dot}.
     *
     * Default: A random value between
     * {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MIN} and
     * {@link LUMINOSITY_OSCILLATION_AMPLITUDE_MAX}.
     * @type {number}
     */
    pa;
    /**
     * The frequency of the sine wave that oscillates the luminosity of
     * this {@link Dot}.
     *
     * Default: The frequency, as calculated based on {@link Dot#pp}.
     * @type {number}
     */
    pb;
    /**
     * The period of the sine wave that oscillates the luminosity of this
     * {@link Dot}.
     *
     * Default: A random value between
     * {@link LUMINOSITY_OSCILLATION_PERIOD_MIN} and
     * {@link LUMINOSITY_OSCILLATION_PERIOD_MAX}.
     * @type {number}
     */
    pp;
    /**
     * The original period of the sine wave that oscillates the luminosity
     * of this {@link Dot}.
     *
     * Default: A random value between
     * {@link LUMINOSITY_OSCILLATION_PERIOD_MIN} and
     * {@link LUMINOSITY_OSCILLATION_PERIOD_MAX}.
     * @type {number}
     */
    opp;
    /**
     * The phase shift of the sine wave that oscillates the luminosity of
     * this {@link Dot}.
     *
     * Default: The sum of {@link Ani.frameCount} and
     * {@link LUMINOSITY_OSCILLATION_PHASE_SHIFT}.
     * @type {number}
     */
    pc;
    /**
     * The amplitude of the sine wave that oscillates the line width of
     * this {@link Dot}.
     *
     * Default: A random value between
     * {@link LINE_WIDTH_OSCILLATION_AMPLITUDE_MIN} and
     * {@link LINE_WIDTH_OSCILLATION_AMPLITUDE_MAX}.
     * @type {number}
     */
    bpa;
    /**
     * The frequency of the sine wave that oscillates the line width of
     * this {@link Dot}.
     *
     * Default: The frequency, as calculated based on {@link Dot#bpp}.
     * @type {number}
     */
    bpb;
    /**
     * The period of the sine wave that oscillates the line width of this
     * {@link Dot}.
     *
     * Default: A random value between
     * {@link LINE_WIDTH_OSCILLATION_PERIOD_MIN} and
     * {@link LINE_WIDTH_OSCILLATION_PERIOD_MAX}.
     * @type {number}
     */
    bpp;
    /**
     * The original period of the sine wave that oscillates the line width
     * of this {@link Dot}.
     *
     * Default: A random value between
     * {@link LINE_WIDTH_OSCILLATION_PERIOD_MIN} and
     * {@link LINE_WIDTH_OSCILLATION_PERIOD_MAX}.
     * @type {number}
     */
    obpp;
    /**
     * The phase shift of the sine wave that oscillates the line width of
     * this {@link Dot}.
     *
     * Default: The sum of {@link Ani.frameCount} and
     * {@link LINE_WIDTH_OSCILLATION_PHASE_SHIFT}.
     * @type {number}
     */
    bpc;
    /**
     * The line width of this {@link Dot}.
     *
     * Default: A random value between {@link LINE_WIDTH_MIN} and
     * {@link LINE_WIDTH_MAX}.
     * @type {number}
     */
    w;
    /**
     * A helper value that helps keep track of the frame number for the
     * purposes of oscillating the luminosity of this {@link Dot}.
     *
     * Default: The value of {@link Ani.frameCount} at the time of creation.
     * @type {number}
     * @protected
     */
    pfx;
    /**
     * A helper value that helps keep track of the frame number for the
     * purposes of oscillating the line width of this {@link Dot}.
     *
     * Default: The value of {@link Ani.frameCount} at the time of creation.
     * @type {number}
     * @protected
     */
    bpfx;
    /**
     * A helper value that helps keep track of the last
     * AUDIO_PEAK_MULTIPLIER value fro the purposes of modifying the speed
     * of the animation of this {@link Dot}.
     *
     * @default 1
     * @type {number}
     * @protected
     */
    oapm = 1;
    /**
     * The y-coordinate of the spot this {@link Dot} was on in the previous
     * frame.
     * @type {number}
     */
    py = null;
    /**
     * The y-coordinate of the spot this {@link Dot} was on in the frame
     * before the previous frame.
     * @type {number}
     */
    ppy = null;
    /**
     * The x-coordinate of the spot this {@link Dot} was on in the previous
     * frame.
     * @type {number}
     */
    px = null;
    /**
     * The x-coordinate of the spot this {@link Dot} was on in the frame
     * before the previous frame.
     * @type {number}
     */
    ppx = null;

    /**
     * Creates a new {@link Dot}.
     */
    constructor() {
        const vpb = Dot.rand(Ani.sObj.opnLum, Ani.sObj.opxLum);
        const vbpb = Dot.rand(Ani.sObj.opnwLine, Ani.sObj.opxwLine);

        this.x = Dot.rand(0, Ani.size.width);
        this.y = Ani.size.height;
        this.s = Dot.rand(Ani.sObj.nSpeed, Ani.sObj.xSpeed);
        this.a = Dot.rand(Ani.sObj.nAccel, Ani.sObj.xAccel);
        this.c = Dot.rand(Ani.hsTrail, Ani.heTrail);
        this.l = Dot.rand(Ani.sObj.lnTrail, Ani.sObj.lxTrail);
        this.sa = Dot.rand(Ani.sObj.snTrail, Ani.sObj.sxTrail);
        this.f = Ani.frameCount;
        this.pa = Dot.rand(Ani.sObj.oanLum, Ani.sObj.oaxLum);
        this.pb = Dot.getFrequency(vpb);
        this.pp = vpb;
        this.opp = vpb;
        this.pc = Ani.frameCount + Ani.sObj.opsLum;
        this.bpa = Dot.rand(Ani.sObj.oanwLine, Ani.sObj.oaxwLine);
        this.bpb = Dot.getFrequency(vbpb);
        this.bpp = vbpb;
        this.obpp = vbpb;
        this.bpc = Ani.frameCount + Ani.sObj.opswLine;
        this.w = Dot.rand(Ani.sObj.wnLine, Ani.sObj.wxLine);
        this.pfx = Ani.frameCount;
        this.bpfx = Ani.frameCount;

        Ani.heTrail += Ani.sObj.hDrift;
        Ani.hsTrail += Ani.sObj.hDrift;
    }
    /**
     * Shifts the reference point coordinates for this {@link Dot}.
     */
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
    /**
     * Updates the phase shifts of this {@link Dot} object so that modulating
     * the frequency of the sine waves does not cause jittering during the
     * animation.
     */
    updatePhaseShifts() {
        if (Ani.apMul !== this.oapm) {
            const np = this.opp / Ani.apMul;
            const nbp = this.obpp / Ani.apMul;
            this.pc = Dot.getNewShift(this.opp, np, this.pc, this.pfx);
            this.pb = Dot.getFrequency(np);
            this.pp = np;
            this.bpc = Dot.getNewShift(this.obpp, nbp, this.bpc, this.bpfx);
            this.bpb = Dot.getFrequency(nbp);
            this.bpp = nbp;
        }
        /*
         * We need to...
         * keep the original period and the last period/phaseshift
         * new period is the multiplier*original
         */
    }
    /**
     * Draws the {@link Dot} to the canvas.
     * @param {CanvasRenderingContext2D} context
     *     The context in which to draw to.
     */
    draw(context) {
        //Skip 1st 2 frames to retrieve last 2 pts necessary for animation
        if (this.py === null || this.ppy === null) {
            this.shiftRefPoints();
            return;
        }

        //Calc: sin x = 2pi    b/2pi=period
        this.pfx = (this.pfx + 1) % this.pp;
        this.bpfx = (this.bpfx + 1) % this.bpp;

        //Set the line width
        context.lineWidth = this.currentLineWidth;

        //Clear all preivous paths
        context.beginPath();

        //Set the stroke and fill styles to the color of the current dot.
        context.strokeStyle = this.colorHSL;
        context.fillStyle = this.colorHSL;

        //Move to the oldest of the 3 reference points of the dot.
        context.moveTo(Math.round(this.ppx), Math.round(this.ppy));
        //Make a line to the second oldest of the 3 reference points of the dot.
        context.lineTo(Math.round(this.px), Math.round(this.py));
        //Draw the line.
        context.stroke();

        //Make a line to the first (newest) of the 3 reference points of the dot
        context.lineTo(Math.round(this.x), Math.round(this.y));
        //Draw the line.
        context.stroke();

        //Shift the ref points and update the speed, position, and phase shifts.
        this.shiftRefPoints();

        //Move the dot upwards based on the dot's speed.
        this.y -= this.s * Ani.apMul;
        //Increase the dot's speed based on its acceleration.
        this.s += this.a;// * AUDIO_PEAK_MULTIPLIER;
        /* eslint-disable-next-line capitalized-comments */
        //this.updatePhaseShifts(); //BUGGED

        //Reset the line width
        context.lineWidth = 1;
    }

    /**
     * Gets the current luminosity of this {@link Dot} based on the current
     * frame count.
     * @returns {number}
     *     The luminosity of this {@link Dot}.
     */
    get currentLuminosity() {
        return Dot.sinusoidal(
            this.pa,
            this.pb,
            this.pc,
            this.l,
            Ani.frameCount
        );
    }
    /**
     * Gets the current line width of this {@link Dot} based on the current
     * frame count.
     * @returns {number}
     *     The line width of this {@link Dot}.
     */
    get currentLineWidth() {
        return Dot.sinusoidal(
            this.bpa,
            this.bpb,
            this.bpc,
            this.w,
            Ani.frameCount
        );
    }
    /**
     * Gets the color of this {@link Dot} as a valid CSS color tag.
     * @returns {string}
     *     The color of this {@link Dot} as a valid CSS color tag.
     */
    get colorHSL() {
        return `hsla(${this.c},${this.sa}%,`
            + `${this.currentLuminosity}%,${Ani.sObj.oTrail})`;
    }
    /**
     * Gets whether or not this {@link Dot} is off-screen.
     * @returns {boolean}
     *     Whether or not this {@link Dot} is off-screen.
     */
    get offScreen() {
        return this.ppy < 0;
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
     * from 20==>15==>20 (frame 17.5), it's -2.5 (that's at 0% to looping, so)
     *
     *  +   2.5,   2.5  + -12.5, -12.5 (or -12.5) (bc 1/2) (f 10 0.5) 20>15 = 5
     *  + - 5.0, - 2.5  +  15.0,   2.5 (or +15) (f 17.5 0.0/1.0) 15>20
     *  +   7.5,   5.0  + - 7.5, - 5.0 (or -7.5) (f 27.5 0.5) 20>15 17.5%15 =2.5
     *  + -10.0, -10.0           (or +10) (frame 35 0.0/1.0) 15>20
     *
     *  12.5 17.5 25.0 35
     *
     *  if we mod by new period then subtract the phase shift
     * @param {number} oP
     *     The old period of the sine wave.
     * @param {number} nP
     *     The new period of the sine wave.
     * @param {number} oPS
     *     The old phase shift of the sine wave.
     * @param {number} x
     *     The current frame.
     */
    static getNewShift(oP, nP, oPS, x) {
        /* eslint-disable-next-line no-extra-parens */
        return (((oP - nP) * ((x - oPS) % oP / oP)) + oPS) % nP;
        //Fuck this calculation...
    }

    /**
     * Calculates the value of a sinusoid equation given the four possible
     * transformations that can be applied to it. (see {@link Sinusoid} for more
     * details about each parameter.)
     * The function uses the current [frame count]{@link FRAME_COUNT} as the
     * value of the "x" parameter.
     * @see {@link Sinusoid}
     * @param {number} a
     *     The amplitude of the function.
     * @param {number} b
     *     The frequency of the function.
     * @param {number} c
     *     The phase-shift of the function.
     * @param {number} d
     *     The vertical-shift of the function.
     * @param {number} x
     *     The x-value.
     * @returns {number}
     *     The value of y in the equation y = A * sin(B * (x - C)) + D where x
     *     is equal to the the current [frame count]{@link FRAME_COUNT}.
     */
    static sinusoidal(a, b, c, d, x) {
        /* eslint-disable-next-line no-extra-parens */
        return (a * Math.sin(b * (x - c))) + d;
    }

    /**
     * Generates a random floating-point number between "min" and "max".
     * If you need an integer instead, call the {@link randInt} function.
     * @param {number} min
     *     The inclusive lower bound of the random number.
     * @param {number} max
     *     The exclusive upper bound of the random number.
     * @returns {number}
     *     The pseudorandom number that was generated.
     */
    static rand(min, max) {
        /* eslint-disable-next-line no-extra-parens */
        return (Math.random() * (max - min)) + min;
    }

    /**
     * Determines the frequency of a sinusoidal equation based on a given
     * period.
     * @see {@link Sinusoid}
     * @param {number} period
     *     The period of the sinusoidal function.
     * @returns {number}
     *     The frequency of the sinusoidal function.
     */
    static getFrequency(period) {
        return 2 * Math.PI / period;
    }
}

/**
 * A static class that contains methods and properties that can be used
 * to manage the animation being run.
 */
class Ani {
    /*
     * ... It took me 2hrs to realize how to document static properties like
     * this. I feel dead inside...
     */

    /**
     * An object used to obtain application settings.
     * @type {SettingsDB}
     * @private
     */
    static settingsFactory;
    /**
     * The current application settings.
     * @type {Settings}
     */
    static sObj;
    /**
     * An array of all of the dots in the animation.
     * @type {Dot[]}
     */
    static dots = [];
    /**
     * A number that is used as a multiplier for modulating the animation
     * based on system audio volume.
     * @type {number}
     */
    static apMul = 1;
    /**
     * An object that is used to update the status HUD screen.
     * @type {StatusElementCollection}
     */
    static status;
    /**
     * An object that is used to display help/keybindings on the screen.
     * @type {StatusElementCollection}
     */
    static statusHelp;
    /**
     * The number of frames that have been rendered for the animation.
     * @type {number}
     */
    static frameCount = 0;
    /**
     * The CANVAS element being rendered to.
     * @type {HTMLCanvasElement}
     */
    static canvas;
    /**
     * The rendering context for the CANVAS element being rendered to.
     * @type {CanvasRenderingContext2D}
     */
    static context;
    /**
     * The bounding rectangle that the canvas is displaying in.
     * @type {DOMRect}
     */
    static size;
    /**
     * @type {number}
     * @private
     */
    static the;
    /**
     * @type {number}
     * @private
     */
    static ths;
    /**
     * The AudioPeaks client that retrieves the system audio peaks.
     * @type {AudioPeaks}
     */
    static peaks;
    /**
     * The time in which the animation was first started.
     * Note: this is the number of milliseconds since 1970-01-01T00:00.
     * @type {number}
     */
    static startTime;
    /**
     * A value that determines whether or not the animation is currently
     * running.
     * @type {boolean}
     * @since 2.1.7.9
     * @private
     */
    static started = false;
    /**
     * The number of "threads" open (used to synchronize stopping the
     * animation).
     * @type {number}
     * @since 2.1.7.9
     * @private
     */
    static threadCount = 0;
    /**
     * Whether or not the animation is being stopped.
     * @type {boolean}
     * @since 2.1.7.9
     * @private
     */
    static stopping = false;
    /**
     * The status settings object.
     * @type {StatusCollectionSettings}
     * @since 2.1.7.9
     * @private
     */
    static statusSettings = {
        "rows": [
            {
                "name": "General",
                "type": "header"
            }, {
                "name": "Canvas Size",
                "type": "range.number.integer",
                "unit": ["", "pixels"],
                "sep": " x ",
                "value": () => [Ani.width, Ani.height]
            }, {
                "name": "Version",
                "type": "string",
                "value": () => VERSION
            }, {
                "name": "Auto Resize",
                "type": "flag",
                "value": () => Ani.sObj.resize
            }, {
                "name": "Background",
                "type": "color.rgb",
                "value": () => Ani.sObj.cBackground
            }, {
                "name": "Frame Statistics",
                "type": "header"
            }, {
                "name": "Target FPS",
                "type": "string",
                "unit": "frames/second",
                "value": () => Ani.sObj.fps.toFixed(2)
            }, {
                "name": "Achieved FPS",
                "type": "string",
                "unit": "frames/second",
                "value": () => (
                    Math.round(Ani.frameCount / (
                        (new Date().getTime() - Ani.startTime)
                        / 1000
                    ) * 100) / 100)
                    .toFixed(2)
            }, {
                "name": "Frame Count",
                "type": "number.integer",
                "unit": "frames",
                "value": () => Ani.frameCount
            }, {
                "name": "Dot Statistics",
                "type": "header"
            }, {
                "name": "Speed",
                "type": "range.number.decimal",
                "unit": ["", "pixels/second"],
                "value": () => [Ani.sObj.nSpeed, Ani.sObj.xSpeed]
            }, {
                "name": "Acceleration",
                "type": "range.number.decimal",
                "unit": ["", "pixels/second"],
                "value": () => [Ani.sObj.nAccel, Ani.sObj.xAccel]
            }, {
                "name": "Active Dots",
                "type": "range.number.integer",
                "unit": ["", "dots"],
                "sep": " of ",
                "value": () => [Ani.dots.length, Ani.sObj.xDots]
            }, {
                "name": "Dot Rate",
                "type": "range.number.integer",
                "unit": ["dot(s)", "ms"],
                "sep": " per ",
                "value": () => [
                    Ani.sObj.rDot,
                    Math.round(Ani.sObj.iFrame * (1 / Ani.apMul))
                ]
            }, {
                "name": "Trail Opacity",
                "type": "color.alpha",
                "value": () => Ani.sObj.oTrail
            }, {
                "name": "Trail Saturation",
                "type": "range.color.sat",
                "value": () => [Ani.sObj.snTrail, Ani.sObj.sxTrail]
            }, {
                "name": "Trail Luminosity",
                "type": "range.color.luma",
                "value": () => [Ani.sObj.lnTrail, Ani.sObj.lxTrail]
            }, {
                "name": "Fade Opacity",
                "type": "number.percentage",
                "value": () => Ani.sObj.oFade
            }, {
                "name": "Line Width",
                "type": "range.number.decimal",
                "unit": ["", "pixels"],
                "value": () => [Ani.sObj.wnLine, Ani.sObj.wxLine]
            }, {
                "name": "Trail Hue Range",
                "type": "header"
            }, {
                "name": "Hue Drift",
                "type": "number.decimal",
                "unit": "degrees",
                "value": () => Ani.sObj.hDrift
            }, {
                "name": "Current",
                "type": "range.color.hue",
                "value": () => [Ani.hsTrail, Ani.heTrail]
            }, {
                "name": "Default",
                "type": "range.color.hue",
                "value": () => [
                    Ani.defaults.trail.hue.start,
                    Ani.defaults.trail.hue.end
                ]
            }, {
                "name": "Luma Oscillation",
                "type": "header"
            }, {
                "name": "Period",
                "type": "range.string",
                "unit": ["", "seconds"],
                "value": () => [
                    Ani.sObj.opnLum.toFixed(2),
                    Ani.sObj.opxLum.toFixed(2)
                ]
            }, {
                "name": "Amplitude",
                "type": "range.color.luma",
                "value": () => [Ani.sObj.oanLum, Ani.sObj.oaxLum]
            }, {
                "name": "Phase Shift",
                "type": "string",
                "unit": "seconds",
                "value": () => Ani.sObj.opsLum.toFixed(2)
            }, {
                "name": "Line Width Oscillation",
                "type": "header"
            }, {
                "name": "Period",
                "type": "range.string",
                "unit": ["", "seconds"],
                "value": () => [
                    Ani.sObj.opnwLine.toFixed(2),
                    Ani.sObj.opxwLine.toFixed(2)
                ]
            }, {
                "name": "Amplitude",
                "type": "range.number.decimal",
                "unit": ["", "pixels"],
                "value": () => [Ani.sObj.oanwLine, Ani.sObj.oaxwLine]
            }, {
                "name": "Phase Shift",
                "type": "string",
                "unit": "seconds",
                "value": () => Ani.sObj.opswLine.toFixed(2)
            }, {
                "name": "Audio Peaks",
                "type": "header"
            }, {
                "name": "Variance (Low)",
                "type": "number.decimal",
                "value": () => Ani.sObj.vnPeaks
            }, {
                "name": "Variance (High)",
                "type": "number.decimal",
                "value": () => Ani.sObj.vxPeaks
            }, {
                "name": "Reconnect Wait",
                "type": "number.decimal",
                "unit": "ms",
                "value": () => Ani.sObj.wPeaks
            }, {
                "name": "Reconnect Wait (Err)",
                "type": "number.decimal",
                "unit": "ms",
                "value": () => Ani.sObj.ewPeaks
            }, {
                "name": "Enabled",
                "type": "flag",
                "value": () => Ani.sObj.ePeaks
            }, {
                "name": "Secure",
                "type": "flag",
                "value": () => Ani.sObj.sPeaks
            }, {
                "name": "Endpoint",
                "type": "string",
                "value": () => AudioPeaks.url
            }
        ],
        "customCSS": "top: 0; left: 0;"
    };
    /**
     * An object that contains the default settings for the application.
     * @type {SettingsObject}
     * @since 2.1.7.19
     * @private
     */
    static defaults;

    /**
     * A static constructor of sorts that is run when starting the animation.
     * @private
     */
    static __constructor() {
        Bindings.bind();
        Ani.loadSettings();
    }
    /**
     * A static method of sorts that is used to start the animation.
     * @private
     */
    static start() {
        if (Ani.started) {
            return; //Duplicate call.
        }
        Ani.started = true;

        //Load overlays.
        Ani.status = new StatusElementCollection(Ani.statusSettings);
        Ani.statusHelp = new StatusElementCollection(
            Bindings.generateHelpSettings()
        );
        Ani.the = Ani.defaults.trail.hue.end;
        Ani.ths = Ani.defaults.trail.hue.start;

        Ani.frameCount = 0;
        //Retrieve the CANVAS element
        Ani.canvas = document.querySelector("canvas");

        //Get a 2D drawing context for the canvas
        Ani.context = Ani.canvas.getContext("2d");

        //Get canvas size, which should be stretched to full size of window.
        Ani.size = Ani.canvas.getBoundingClientRect();
        //Set w&h of canvas internally so canvas has a 1:1 ratio to the screen.
        Ani.canvas.setAttribute("width", Ani.width);
        Ani.canvas.setAttribute("height", Ani.height);

        //Clear all prior paths.
        Ani.context.beginPath();

        //Set the fill and stroke styles to the bg color at full opacity.
        Ani.context.fillStyle = `rgba(${Ani.sObj.cBackground.join(",")},1)`;
        Ani.context.strokeStyle = `rgba(${Ani.sObj.cBackground.join(",")},1)`;

        //Fill the entire canvas with the current fill style.
        Ani.context.fillRect(0, 0, Ani.width, Ani.height);

        //Create a timer to start the animation.
        Ani.threadCount = 2;
        Ani.startTime = new Date().getTime();
        window.setTimeout(Ani.animate, Ani.sObj.iFrame);
        window.setTimeout(Ani.addDots, Ani.sObj.iFrame);
        Ani.status.update();

        try {
            Ani.peaks = new AudioPeaks();
        } catch (e) {
            console.log("Failed to load peaks app: ", e);
        }
    }
    /**
     * Renders the next animation frame.
     * @protected
     */
    static animate() {
        if (!Ani.started) {
            Ani.threadCount -= 1; //Decrement "thread" count
            return; //Terminate animation if stopping
        }

        //Increment the frame counter.
        Ani.frameCount += 1;
        try {
            Ani.animateInternal(Ani.context);
        } catch (e) {
            console.error(e);
        }
        //Set a timer to rerun this, when we need to animate the next frame.
        window.setTimeout(Ani.animate, Ani.sObj.iFrame);
    }
    /**
     * Adds new dots to the animation.
     * @since 2.1.7.4
     * @protected
     */
    static addDots() {
        if (!Ani.started) {
            Ani.threadCount -= 1; //Decrement "thread" count
            return; //Terminate animation if stopping
        }
        let i;
        for (i = 0; i < Ani.sObj.rDot; i += 1) {
            if (Ani.dots.length >= Ani.sObj.xDots) {
                break; //Can't add more dots.
            }
            //Add another dot and add it to the list.
            Ani.dots.push(new Dot());
        }
        window.setTimeout(
            Ani.addDots,
            Math.round(Ani.sObj.iFrame * (1 / Ani.apMul))
        );
    }
    /**
     * Updates the size of the canvas the animation is rendering to.
     * This would usually be called if the user resizes the window.
     */
    static updateSize() {
        if (Ani.stopping) {
            return;
        }
        //Initialize variables.
        let i;
        const osize = Ani.size;

        //Verify that resize is actually enabled
        if (Ani.sObj.resize) {
            //Get canvas size, which should be stretched to full window size
            Ani.size = Ani.canvas.getBoundingClientRect();

            //Set w&h of canvas internally, so canvas has 1:1 ratio to screen
            Ani.canvas.setAttribute("width", Ani.width);
            Ani.canvas.setAttribute("height", Ani.height);

            //Check to see if the canvas was made smaller, if not, don't check.
            if (osize.width > Ani.size.width) {
                //Check all dots to see if they're still in bounds.
                for (i = 0; i < Ani.dots.length; i += 1) {
                    if (Ani.dots[i].x > Ani.size.width) {
                        /* eslint-disable-next-line no-plusplus */
                        Ani.dots.splice(i--, 1);
                        //Dot is out of bounds, remove it.
                    }
                }
            }

            //Set the fill and stroke styles to the bg color at full opacity.
            Ani.context.fillStyle
                = `rgba(${Ani.sObj.cBackground.join(",")},1)`;
            Ani.context.strokeStyle
                = `rgba(${Ani.sObj.cBackground.join(",")},1)`;

            //Fill the entire canvas with the current fill style.
            Ani.context.fillRect(0, 0, Ani.width, Ani.height);
        }
    }
    /**
     * A function that represents the computation required to complete a single
     * frame in the animation.
     * @private
     * @param {CanvasRenderingContext2D} context
     *     The context in which to render to.
     */
    static animateInternal(context) {
        let i;
        //Clear the canvas to make the dot trails appear to fade away.
        context.beginPath();
        context.fillStyle
            = `rgba(${Ani.sObj.cBackground.join(",")},${Ani.sObj.oFade})`;
        context.strokeStyle
            = `rgba(${Ani.sObj.cBackground.join(",")},${Ani.sObj.oFade})`;
        context.moveTo(0, 0);
        context.rect(0, 0, Ani.width, Ani.height);
        context.fill();

        //Draw and move all dots.
        Ani.dots.forEach((d) => d.draw(context));

        //Remove dots if they're off-screen
        for (i = 0; i < Ani.dots.length; i += 1) {
            if (Ani.dots[i].offScreen) {
                /* eslint-disable-next-line no-plusplus */
                Ani.dots.splice(i--, 1);
            }
        }
    }
    /**
     * Loads the saved application settings.
     * @private
     */
    static loadSettings() {
        Ani.defaults = Settings.loadSettingsFile();
        Ani.settingsFactory = new SettingsDB();
        Ani.settingsFactory.addEventListener("open", () => {
            Ani.settingsFactory.loadSettings().then((s) => {
                Ani.sObj = s;
                Ani.start();
            }).catch(() => {
                console.error("Failed to load/parse IDB settings... :(");
                Ani.sObj = new Settings(Ani.settingsFactory, "{}");
                Ani.sObj.save();
                Ani.start();
            });
        });
        Ani.settingsFactory.addEventListener("error", () => {
            console.error("Failed to load IDB settings... :(");
            Ani.sObj = new Settings(null, "{}");
            Ani.start();
        });
        Ani.settingsFactory.open();
    }
    /**
     * Steps the FPS up by 5 frames per second.
     */
    static upFPS() {
        if (Ani.stopping) {
            return;
        }
        console.info(`Now targeting ${Ani.sObj.fps += 5} frames per second.`);
    }
    /**
     * Steps the FPS down by 5 frames per second.
     */
    static downFPS() {
        if (Ani.stopping) {
            return;
        }
        const old_fps = Ani.sObj.fps;
        Ani.sObj.fps -= 5;
        console.info(old_fps === Ani.sObj.fps
            ? "Cannot reduce the framerate any lower than 5 FPS."
            : `Now targeting ${Ani.sObj.fps} frames per second.`);
    }
    /**
     * Resets the animation.
     */
    static reset() {
        if (Ani.stopping) {
            return;
        }
        Ani.startTime = new Date().getTime();
        Ani.dots = [];
        Ani.frameCount = 0;
        Ani.heTrail = Ani.defaults.trail.hue.end;
        Ani.hsTrail = Ani.defaults.trail.hue.start;
        //Clear all prior paths.
        Ani.context.beginPath();

        //Set the fill and stroke styles to the bg color at full opacity.
        Ani.context.fillStyle = `rgba(${Ani.sObj.cBackground.join(",")},1)`;
        Ani.context.strokeStyle = `rgba(${Ani.sObj.cBackground.join(",")},1)`;

        //Fill the entire canvas with the current fill style.
        Ani.context.fillRect(0, 0, Ani.width, Ani.height);
    }
    /**
     * Toggles whether or not the status overlay is enabled and displayed.
     */
    static toggleStatus() {
        if (Ani.stopping) {
            return;
        }
        /* eslint-disable-next-line no-multi-assign */
        const d = Ani.status.displayed = !Ani.status.displayed;
        console.info(
            `Turned ${d ? "on" : "off"} the status overlay.`
        );
    }
    /**
     * Toggles whether or not verbose information is displayed in the status
     * overlay.
     */
    static toggleVerboseStatus() {
        if (Ani.stopping) {
            return;
        }
        /* eslint-disable-next-line no-multi-assign */
        const d = Ani.status.showVerbose = !Ani.status.showVerbose;
        console.info(`${d ? "Show" : "Hid"}ing verbose info on the overlay.`);
    }
    /**
     * Outputs help information to the console.
     */
    static toggleHelp() {
        if (Ani.stopping) {
            return;
        }
        /* eslint-disable-next-line no-multi-assign */
        const d = Ani.statusHelp.displayed = !Ani.statusHelp.displayed;
        console.info(`Turned ${d ? "on" : "off"} the help overlay.`);
    }
    /**
     * Toggles whether or not the audioPeaks subsystem is enabled.
     * @since 2.1.7.6
     */
    static togglePeaks() {
        if (Ani.stopping) {
            return;
        }
        /* eslint-disable-next-line no-multi-assign */
        const d = Ani.sObj.ePeaks = !Ani.sObj.ePeaks;
        console.info(`${d ? "En" : "Dis"}abled the AudioPeaks subsystem.`);
    }
    /**
     * Resets the default settings to their values as defined in this script
     * file (the DEFAULT_* constants)
     * @since 2.1.7.9
     * @returns {Promise<void>}
     *     A promise that returns once the default settings have been restored.
     */
    static resetDefaultSettings() {
        return new Promise((resolve, reject) => {
            if (Ani.stopping) {
                reject();
            }
            Ani.stop(true)
                .then(() => Ani.sObj.deleteSettings())
                .then(() => {
                    Ani.sObj = null;
                    Ani.settingsFactory = null;
                    Ani.__constructor(); //RELOAD
                    resolve();
                });
        });
    }
    /**
     * Stops the animation.
     * @since 2.1.7.9
     * @param {boolean} leaveSettings
     *     A value that indicates whether or not the Settings objects should
     *     also be disposed of.
     * @returns {Promise<void>}
     *     A promise that returns when the application has been stopped.
     */
    static stop(leaveSettings = false) {
        //Ugh, this was an ass to implement.
        return new Promise((resolve, reject) => {
            if (Ani.stopping) {
                reject();
            }
            if (!Ani.started) {
                reject();
            }
            //This will prevent bugs if the user hits a keybind while stopping
            Ani.stopping = true;

            //Remove overlays from doc.
            Ani.status.dispose();
            Ani.statusHelp.dispose();
            Ani.started = false;

            new Promise((innerResolve) => {
                const interval = window.setInterval(() => {
                    if (Ani.threadCount <= 0) {
                        window.clearInterval(interval);
                        innerResolve();
                        //Wait until both ticks die.
                    }
                }, 10);
            }).then(() => new Promise((innerResolve) => {
                //Disconnect peaks server
                if (Ani.peaks && Ani.peaks.connected) {
                    Ani.peaks.addEventListener("close", innerResolve);
                    Ani.peaks.disconnect();
                } else {
                    innerResolve();
                }
            })).then(() => {
                if (!leaveSettings) {
                    Ani.sObj = null;
                    Ani.settingsFactory = null;
                }
                Bindings.unbind();
                Ani.stopping = false;
                Ani.started = false;
                resolve();
            });
        });
    }
    /**
     * Toggles the animation state.
     * @since 2.1.7.9
     */
    static toggleAnimation() {
        if (Ani.started) {
            Ani.stop();
            //Stop unbinds the event handlers...
        } else {
            Ani.__constructor();
        }
    }
    /**
     * A function that is run when the debugging keybind is pressed.
     * This is used to perform various debuggging tasks with ease.
     * @since 2.1.7.18
     */
    static debug() {
        const settings = Bindings.generateHelpSettings();
        console.log(JSON.stringify(settings));
    }

    /**
     * Gets the current maximum hue the next dot's trail can be.
     * @type {number}
     */
    static get heTrail() {
        return Ani.the;
    }
    /**
     * Gets the current minimum hue the next dot's trail can be.
     * @type {number}
     */
    static get hsTrail() {
        return Ani.ths;
    }
    /**
     * Gets the rounded width of the animation canvas.
     * @returns {number} The rounded width of the animation canvas.
     */
    static get width() {
        return Math.round(Ani.size.width);
    }
    /**
     * Gets the rounded height of the animation canvas.
     * @returns {number} The rounded height of the animation canvas.
     */
    static get height() {
        return Math.round(Ani.size.height);
    }
    /**
     * Sets the current maximum hue the next dot's trail can be.
     * @type {number}
     */
    static set heTrail(value) {
        let xvalue = value;
        while (xvalue < 0) {
            xvalue += 360;
        }
        xvalue %= 360;
        Ani.the = value;
    }
    /**
     * Sets the current minimum hue the next dot's trail can be.
     * @type {number}
     */
    static set hsTrail(value) {
        let xvalue = value;
        while (xvalue < 0) {
            xvalue += 360;
        }
        xvalue %= 360;
        Ani.ths = value;
        if (Ani.ths > Ani.the) {
            Ani.ths -= 360;
        }
    }
    /*
     * The extra junk in heTrail/hsTrail is...
     * Bounds checking, make sure HSL_START/END are between 0 and 360.
     * doing this prevents the application from randomly failing when either
     * gets too large or too small.
     * The application should only break when the precision of FRAME_COUNT
     * becomes too small to keep track of each new frame, or when we overflow
     * FRAME_COUNT into NaN
     */
}
/**
 * A class that uses IndexedDB to store and retrieve application settings.
 */
class SettingsDB extends EventTarget {
    /**
     * Fired when the settings database is successfully opened.
     * @event SettingsDB#event:open
     * @type {Object}
     */
    /**
     * Fired when an error occurs while trying to open the settings database.
     * @event SettingsDB#event:error
     * @type {Object}
     */
    /**
     * @type {string}
     * @private
     */
    __state = "closed";
    /**
     * The opened settings database.
     * @type {IDBDatabase}
     * @private
     */
    __db;
    /**
     * Indicates the state of the settings database. It's one of the
     * following values:
     * + closed
     * + open
     * + opening
     */
    get state() {
        return this.__state;
    }

    /**
     * Opens the settings database.
     *
     * @fires SettingsDB#event:open
     * @fires SettingsDB#event:error
     * @returns {boolean}
     *     "true" if the database is actually being opened.
     *     "false" if the state isn't "closed".
     */
    open() {
        if (this.__state !== "closed") {
            return false;
        }
        this.__state = "opening";
        const self = this;
        new Promise((resolve, reject) => {
            function create(db) {
                try {
                    db.createObjectStore("settings", {"keyPath": "id"});
                    return true;
                } catch (e) {
                    return false;
                }
            }
            const request = indexedDB.open("settingsDB", SettingsDB.version);

            request.addEventListener("success", (e) => {
                const db = e.target.result;
                db.addEventListener("error", reject);
                if (db.setVersion) {
                    if (db.version === SettingsDB.version) {
                        resolve(db);
                    } else {
                        const sv = db.setVersion(SettingsDB.version);
                        sv.addEventListener("success", () => {
                            if (create(db)) {
                                resolve(db);
                            } else {
                                reject();
                            }
                        });
                    }
                } else {
                    resolve(db);
                }
            });
            request.addEventListener("upgradeneeded", (e) => {
                const db = e.target.result;
                if (create(db)) {
                    resolve(db);
                } else {
                    reject();
                }
            });
        }).then((db) => {
            self.__db = db;
            self.__state = "open";
            self.dispatchEvent(new CustomEvent("open"));
        }).catch(() => {
            self.__state = "closed";
            self.dispatchEvent(new CustomEvent("error"));
        });
        return true;
    }
    /**
     * Opens the specified settings store.
     *
     * @param {string} id
     *     The ID used to save the settings. This will be "(default)"
     *     for the default settings.
     *
     *     Passing any non-string value or an empty string will result in the
     *     application using "(default)" instead.
     *
     *     You can use this property to save multiple settings presets.
     * @returns {Promise<Settings>}
     *     The Settings object obtained, or "null" if the application was
     *     unable to attempt this;
     */
    loadSettings(id = "(default)") {
        if (this.__state !== "open") {
            return null;
        }
        const self = this;
        return new Promise((resolve, reject) => {
            const t = self.__db.transaction(["settings"], "readwrite");
            t.addEventListener("error", reject);
            const store = t.objectStore("settings");
            const tr = store.get(id);
            tr.addEventListener("error", reject);
            tr.addEventListener("success", (e) => {
                const {result} = e.target;
                let sett;
                if (result) {
                    try {
                        sett = new Settings(self, result.data, id);
                    } catch (ex) {
                        reject();
                    }
                    resolve(sett);
                } else {
                    reject();
                }
            });
        });
    }

    /**
     * Saves the specified settings store.
     *
     * @param {string} data
     *     The settings data
     * @param {string} id
     *     The ID used to save the settings. This will be "(default)"
     *     for the default settings.
     *
     *     Passing any non-string value or an empty string will result in the
     *     application using "(default)" instead.
     *
     *     You can use this property to save multiple settings presets.
     * @returns {Promise<void>}
     *     The Settings object obtained, or "null" if the application was
     *     unable to attempt this;
     * @protected
     */
    saveSettings(data, id = "(default)") {
        const self = this;
        if (this.__state !== "open") {
            return null;
        }
        return new Promise((resolve, reject) => {
            const t = self.__db.transaction(["settings"], "readwrite");
            t.addEventListener("error", reject);
            t.addEventListener("complete", resolve);
            const store = t.objectStore("settings");
            store.put({id, data});
            t.commit();
        });
    }
    /**
     * Gets the current database version. Used to upgrade IDB objects in the
     * event the database structure is changed.
     *
     * This is currently: 2
     */
    static get version() {
        return 2;
    }
}

/**
 * A class that represents the settings of the application.
 */
class Settings {
    /**
     * @type {SettingsDB}
     * @private
     */
    __db;
    /**
     * @type {string}
     * @private
     */
    __id;
    /**
     * @type {Object}
     * @private
     */
    __data;
    /**
     * @type {string[]}
     * @private
     */
    __keys;

    /**
     * Creates a new {@link Settings} object from a {@link SettingsDB} object,
     * the id of the settings, and the JSON string containing the raw settings.
     * @protected
     * @param {SettingsDB} db
     *     The database backing-store for these settings, or null.
     * @param {string} data
     *     A JSON string containing the unparsed settings data.
     * @param {string} id
     *     The id of the settings for DB use.
     */
    constructor(db, data, id = "(default)") {
        this.__db = db;
        this.__id = id;
        this.__data = JSON.parse(data);
        this.__keys = Object.keys(this.__data);
    }
    /**
     * Saves these settings to the {@link SettingsDB}.
     * @returns {Promise<void>}
     *     A promise object that determines if the data was saved successfully
     *     or null, if the data cannot be saved.
     */
    save() {
        if (this.__db === null) {
            return null;
        }
        return this.__db.saveSettings(
            JSON.stringify(this.__data),
            this.__id
        );
    }
    /**
     * Refreshes the key cache of the settings object.
     * @private
     */
    __refreshKeys() {
        this.__keys = Object.keys(this.__data);
    }
    /**
     * Renames this settings store and overwrites the database settings.
     * @param {string} newID
     *     The new ID to save the settings under;
     * @returns {?Promise<void>}
     *     A promise that resolves once the settings have been renamed or null
     *     if there's not anything to be renamed.
     */
    rename(newID) {
        const self = this;
        return new Promise((resolve, reject) => {
            if (!(newID instanceof String)) {
                reject();
            }
            if (this.__db === null) {
                reject();
            }
            if (this.__id === newID) {
                reject();
            }
            const id = self.__id;
            self.__id = newID;
            self.__db
                .saveSettings(JSON.stringify(self.__data), self.__id)
                .then(() => self.__db.saveSettings("", id))
                .then(resolve)
                .catch(reject);
            //Ahhh, I love stringing promises together.
        });
    }

    /**
     * Deletes this settings store from the storage database. This
     * {@link Settings} object will be unusable after calling this method.
     * @since 2.1.7.9
     * @returns {?Promise<void>}
     *     A promise that returns when the settings have been deleted or null
     *     if there is nothing to delete.
     */
    deleteSettings() {
        if (this.__db === null) {
            return null;
        }
        const id = this.__id;
        this.__id = null;
        this.__data = {};
        this.__refreshKeys();
        const promise = this.__db.saveSettings("", id);
        this.__db = null;
        return promise;
    }

    get cBackground() {
        return this.__keys.includes("bg")
            ? this.__data.bg
            : Ani.defaults.background.color;
    }
    get oTrail() {
        return this.__keys.includes("to")
            ? this.__data.to
            : Ani.defaults.trail.opacity;
    }
    get snTrail() {
        return this.__keys.includes("tsn")
            ? this.__data.tsn
            : Ani.defaults.trail.saturation.min;
    }
    get sxTrail() {
        return this.__keys.includes("tsx")
            ? this.__data.tsx
            : Ani.defaults.trail.saturation.max;
    }
    get lnTrail() {
        return this.__keys.includes("tln")
            ? this.__data.tln
            : Ani.defaults.trail.luminosity.min;
    }
    get lxTrail() {
        return this.__keys.includes("tlx")
            ? this.__data.tlx
            : Ani.defaults.trail.luminosity.max;
    }
    get hDrift() {
        return this.__keys.includes("hd")
            ? this.__data.hd
            : Ani.defaults.trail.hue.drift;
    }
    get nSpeed() {
        return this.__keys.includes("ns")
            ? this.__data.ns
            : Ani.defaults.speed.min;
    }
    get xSpeed() {
        return this.__keys.includes("xs")
            ? this.__data.xs
            : Ani.defaults.speed.max;
    }
    get nAccel() {
        return this.__keys.includes("na")
            ? this.__data.na
            : Ani.defaults.accel.min;
    }
    get xAccel() {
        return this.__keys.includes("xa")
            ? this.__data.xa
            : Ani.defaults.accel.max;
    }
    get xDots() {
        return this.__keys.includes("xd")
            ? this.__data.xd
            : Ani.defaults.dots.max;
    }
    get rDot() {
        return this.__keys.includes("dr")
            ? this.__data.dr
            : Ani.defaults.dots.rate;
    }
    get oFade() {
        return this.__keys.includes("fo")
            ? this.__data.fo
            : Ani.defaults.background.opacity;
    }
    get fps() {
        return this.__keys.includes("f")
            ? this.__data.f
            : Ani.defaults.fps;
    }
    get wnLine() {
        return this.__keys.includes("lwn")
            ? this.__data.lwn
            : Ani.defaults.lineWidth.min;
    }
    get wxLine() {
        return this.__keys.includes("lwx")
            ? this.__data.lwx
            : Ani.defaults.lineWidth.max;
    }
    get oanLum() {
        return this.__keys.includes("loan")
            ? this.__data.loan
            : Ani.defaults.luminosity.oscillation.amplitude.min;
    }
    get oaxLum() {
        return this.__keys.includes("loax")
            ? this.__data.loax
            : Ani.defaults.luminosity.oscillation.amplitude.max;
    }
    get opsLum() {
        return this.__keys.includes("lops")
            ? this.__data.lops
            : Ani.defaults.luminosity.oscillation.phaseShift;
    }
    get oanwLine() {
        return this.__keys.includes("lwoan")
            ? this.__data.lwoan
            : Ani.defaults.lineWidth.oscillation.amplitude.min;
    }
    get oaxwLine() {
        return this.__keys.includes("lwoax")
            ? this.__data.lwoax
            : Ani.defaults.lineWidth.oscillation.amplitude.max;
    }
    get opswLine() {
        return this.__keys.includes("lwops")
            ? this.__data.lwops
            : Ani.defaults.lineWidth.oscillation.phaseShift;
    }
    get resize() {
        return this.__keys.includes("r")
            ? this.__data.r
            : Ani.defaults.resize;
    }
    get opnfLum() {
        return this.__keys.includes("lopnf")
            ? this.__data.lopnf
            : Ani.defaults.luminosity.oscillation.periodFactor.min;
    }
    get opxfLum() {
        return this.__keys.includes("lopxf")
            ? this.__data.lopxf
            : Ani.defaults.luminosity.oscillation.periodFactor.max;
    }
    get opnfwLine() {
        return this.__keys.includes("lwopnf")
            ? this.__data.lwopnf
            : Ani.defaults.lineWidth.oscillation.periodFactor.min;
    }
    get opxfwLine() {
        return this.__keys.includes("lwopxf")
            ? this.__data.lwopxf
            : Ani.defaults.lineWidth.oscillation.periodFactor.max;
    }
    get pPeaks() {
        return this.__keys.includes("pp")
            ? this.__data.pp
            : Ani.defaults.peaks.port;
    }
    get dPeaks() {
        return this.__keys.includes("pd")
            ? this.__data.pd
            : Ani.defaults.peaks.domain;
    }
    get sPeaks() {
        return this.__keys.includes("ps")
            ? this.__data.ps
            : Ani.defaults.peaks.secure;
    }
    get ePeaks() {
        return this.__keys.includes("pe")
            ? this.__data.pe
            : Ani.defaults.peaks.enabled;
    }
    get ewPeaks() {
        return this.__keys.includes("pew")
            ? this.__data.pew
            : Ani.defaults.peaks.reconnect.error;
    }
    get wPeaks() {
        return this.__keys.includes("pw")
            ? this.__data.pw
            : Ani.defaults.peaks.reconnect.normal;
    }
    get vnPeaks() {
        return this.__keys.includes("pvn")
            ? this.__data.pvn
            : Ani.defaults.peaks.varianceMultiplier.min;
    }
    get vxPeaks() {
        return this.__keys.includes("pvx")
            ? this.__data.pvx
            : Ani.defaults.peaks.varianceMultiplier.max;
    }

    get opnLum() {
        return this.fps * this.opnfLum;
    } //NO SETTER
    get opxLum() {
        return this.fps * this.opxfLum;
    } //NO SETTER
    get opnwLine() {
        return this.fps * this.opnfwLine;
    } //NO SETTER
    get opxwLine() {
        return this.fps * this.opxfwLine;
    } //NO SETTER
    get iFrame() {
        return 1000 / this.fps;
    } //NO SETTER

    set cBackground(value) {
        this.__data.bg = value;
        this.__refreshKeys();
        this.save();
    }
    set oTrail(value) {
        this.__data.to = value;
        this.__refreshKeys();
        this.save();
    }
    set snTrail(value) {
        this.__data.tsn = value;
        this.__refreshKeys();
        this.save();
    }
    set sxTrail(value) {
        this.__data.tsx = value;
        this.__refreshKeys();
        this.save();
    }
    set lnTrail(value) {
        this.__data.tln = value;
        this.__refreshKeys();
        this.save();
    }
    set lxTrail(value) {
        this.__data.tlx = value;
        this.__refreshKeys();
        this.save();
    }
    set hDrift(value) {
        this.__data.hd = value;
        this.__refreshKeys();
        this.save();
    }
    set nSpeed(value) {
        this.__data.ns = value;
        this.__refreshKeys();
        this.save();
    }
    set xSpeed(value) {
        this.__data.xs = value;
        this.__refreshKeys();
        this.save();
    }
    set nAccel(value) {
        this.__data.na = value;
        this.__refreshKeys();
        this.save();
    }
    set xAccel(value) {
        this.__data.xa = value;
        this.__refreshKeys();
        this.save();
    }
    set xDots(value) {
        this.__data.xd = value;
        this.__refreshKeys();
        this.save();
    }
    set rDot(value) {
        this.__data.dr = value;
        this.__refreshKeys();
        this.save();
    }
    set oFade(value) {
        this.__data.fo = value;
        this.__refreshKeys();
        this.save();
    }
    set fps(value) {
        this.__data.f = Math.max(5, value);
        this.__refreshKeys();
        this.save();
    }
    set wnLine(value) {
        this.__data.lwn = value;
        this.__refreshKeys();
        this.save();
    }
    set wxLine(value) {
        this.__data.lwx = value;
        this.__refreshKeys();
        this.save();
    }
    set oanLum(value) {
        this.__data.loan = value;
        this.__refreshKeys();
        this.save();
    }
    set oaxLum(value) {
        this.__data.loax = value;
        this.__refreshKeys();
        this.save();
    }
    set opsLum(value) {
        this.__data.lops = value;
        this.__refreshKeys();
        this.save();
    }
    set oanwLine(value) {
        this.__data.lwoan = value;
        this.__refreshKeys();
        this.save();
    }
    set oaxwLine(value) {
        this.__data.lwoax = value;
        this.__refreshKeys();
        this.save();
    }
    set opswLine(value) {
        this.__data.lwops = value;
        this.__refreshKeys();
        this.save();
    }
    set resize(value) {
        this.__data.r = value;
        this.__refreshKeys();
        this.save();
    }
    set opnwfLine(value) {
        this.__data.lwopnf = value;
        this.__refreshKeys();
        this.save();
    }
    set opxwfLine(value) {
        this.__data.lwopxf = value;
        this.__refreshKeys();
        this.save();
    }
    set opnfLum(value) {
        this.__data.lopnf = value;
        this.__refreshKeys();
        this.save();
    }
    set opxfLum(value) {
        this.__data.lopxf = value;
        this.__refreshKeys();
        this.save();
    }
    set pPeaks(value) {
        this.__data.pp = value;
        this.__refreshKeys();
        this.save();
        Ani.peaks.reconnect();
    }
    set dPeaks(value) {
        this.__data.pd = value;
        this.__refreshKeys();
        this.save();
        Ani.peaks.reconnect();
    }
    set sPeaks(value) {
        this.__data.ps = value;
        this.__refreshKeys();
        this.save();
        Ani.peaks.reconnect();
    }
    set ePeaks(value) {
        this.__data.pe = value;
        this.__refreshKeys();
        this.save();
        Ani.peaks.reconnect();
    }
    set ewPeaks(value) {
        this.__data.pew = value;
        this.__refreshKeys();
        this.save();
    }
    set wPeaks(value) {
        this.__data.pw = value;
        this.__refreshKeys();
        this.save();
    }
    set vnPeaks(value) {
        this.__data.pvn = value;
        this.__refreshKeys();
        this.save();
    }
    set vxPeaks(value) {
        this.__data.pvx = value;
        this.__refreshKeys();
        this.save();
    }

    /**
     * A method that returns an object containing the default properties for
     * the animation.
     * @returns {SettingsObject}
     *     An object that contains the default settings for the animation.
     * @since 2.1.7.19
     */
    static loadSettingsFile() {
        const defaultDefaultSettings = {
            "trail": {
                "saturation": {
                    "min": 100.0,
                    "max": 100.0
                },
                "luminosity": {
                    "min": 25.0,
                    "max": 75.0
                },
                "opacity": 1.0,
                "hue": {
                    "drift": 0.1,
                    "start": 180.0,
                    "end": 240.0
                }
            },
            "speed": {
                "min": 0.1,
                "max": 2.0
            },
            "accel": {
                "min": 0.01,
                "max": 0.50
            },
            "luminosity": {
                "oscillation": {
                    "amplitude": {
                        "min": 0.1,
                        "max": 25
                    },
                    "phaseShift": 0,
                    "periodFactor": {
                        "min": 0.5,
                        "max": 1.0
                    }
                }
            },
            "background": {
                "color": [0, 0, 0],
                "opacity": 0.2
            },
            "fps": 30,
            "lineWidth": {
                "min": 0.5,
                "max": 3.0,
                "oscillation": {
                    "periodFactor": {
                        "min": 0.5,
                        "max": 1.0
                    },
                    "amplitude": {
                        "min": 0.1,
                        "max": 2.0
                    },
                    "phaseShift": 0
                }
            },
            "peaks": {
                "port": 8069,
                "domain": "localhost",
                "secure": false,
                "enabled": true,
                "reconnect": {
                    "normal": 5000,
                    "error": 30000
                },
                "varianceMultiplier": {
                    "min": 0.125,
                    "max": 8.000
                }
            },
            "dots": {
                "rate": 2,
                "max": 250
            },
            "resize": false
        };
        try {
            return Object.assign(
                {},
                defaultDefaultSettings,
                /* eslint-disable-next-line no-undef */
                DefaultSettings
            );
        } catch (e) {
            console.error(
                "ERROR WHILE LOADING SETTINGS",
                e
            );
            return Object.assign(
                {},
                defaultDefaultSettings
            );
        }
    }
    /*
     * Shorthands
     * a = amplitude, c = color, e = end, f = factor, h = height/hsl,
     * i = interval, l = luminosity, o = opacity, p = period, ps = phase shift,
     * r = rate, s = saturation / start, w = width
     * -n = min, -x = max
     */
}

/**
 * A class that enables connection to an AudioPeaks server and dynamically
 * alter aspects of the animation based on the volume.
 */
class AudioPeaks extends EventTarget {
    /**
     * @type {boolean}
     * @private
     */
    __connecting = false;
    /**
     * @type {boolean}
     * @private
     */
    __connected = false;
    /**
     * @type {boolean}
     * @private
     */
    __reconnect = false;
    /**
     * @type {WebSocket}
     * @private
     */
    __socket;

    /**
     * Creates a new {@link AudioPeaks} object.
     */
    constructor() {
        super();
        if (Ani.sObj.ePeaks) {
            this.connect();
        }
    }

    /**
     * @private
     */
    __open() {
        console.info("Connected to the Audio peaks sever successfully.");
        this.__connected = true;
        this.dispatchEvent(new CustomEvent("open"));
    }
    /**
     * @private
     * @param {Event} e
     */
    __error(e) {
        console.error("Failed to connect to the audio peaks server: ", e);
        if (!this.__reconnect) {
            console.info(
                `Retrying in ${Ani.sObj.ewPeaks / 1000}s.`
            );
            window.setTimeout(() => this.reconnect(), Ani.sObj.ewPeaks);
            this.__reconnect = true;
        }
        Ani.apMul = 1;
        this.dispatchEvent(new CustomEvent("error", {detail: e}));
    }
    /**
     * @private
     */
    __close() {
        console.info("Lost the connection to the audio peaks server.");
        if (!this.__reconnect) {
            console.info(`Retrying in ${Ani.sObj.wPeaks / 1000}s.`);
            window.setTimeout(() => this.reconnect(), Ani.sObj.wPeaks);
            this.__reconnect = true;
        }
        Ani.apMul = 1;
        this.__connected = false;
        this.dispatchEvent(new CustomEvent("close"));
    }
    /**
     * @private
     * @param {Event} e
     */
    static message(e) {
        const message = JSON.parse(e.data);
        let peak;
        switch (message.status) {
        case "Success":
            peak = message.data.max;
            if (peak === 0.5) {
                Ani.apMul = 1;
            } else if (peak < 0.5) {
                Ani.apMul
                    /* eslint-disable-next-line no-extra-parens */
                    = 1 - ((0.5 - peak) * 2 * (1 - Ani.sObj.vnPeaks));
            } else { // > 0.5
                Ani.apMul
                    /* eslint-disable-next-line no-extra-parens */
                    = 1 + ((peak - 0.5) * 2 * (Ani.sObj.vxPeaks - 1));
            }
            /* eslint-disable-next-line no-extra-parens */
            Ani.apMul = ((Ani.sObj.vxPeaks - Ani.sObj.vnPeaks) * peak)
                + Ani.sObj.vnPeaks;
            break;
        case "Error":
            console.error("AudioPeaks Error: ", message.data);
            break;
        }
    }

    /**
     * Gets the full URL being connected to.
     * @type {string}
     */
    static get url() {
        return `ws${Ani.sObj.sPeaks ? "s" : ""}://`
            + `${Ani.sObj.dPeaks}:${Ani.sObj.pPeaks}/AudioPeaks`;
    }

    /**
     * Gets whether or not the AudioPeaks system is connected.
     * @since 2.1.7.9
     */
    get connected() {
        return this.__connected;
    }

    /**
     * Disconnects the current AudioPeaks connection and reconnects.
     */
    reconnect() {
        this.__reconnect = true;
        if (this.__socket) {
            this.__socket.addEventListener("close", () => {
                this.__connecting = false;
                this.connect();
                this.__socket = null;
                //Required so we don't have two at once.
            });
            this.__socket.close();
        } else {
            this.__connecting = false;
            this.connect();
        }
    }
    /**
     * Attempts to connect to the audio peaks server.
     */
    connect() {
        if (this.__connecting) {
            return;
        }
        if (!Ani.sObj.ePeaks) {
            return;
        }
        this.__connecting = true;
        this.__reconnect = false;
        this.__socket = new WebSocket(AudioPeaks.url);
        this.__socket.addEventListener("open", () => this.__open());
        this.__socket.addEventListener("error", (e) => this.__error(e));
        this.__socket.addEventListener("close", () => this.__close());
        this.__socket.addEventListener("message",
            (e) => AudioPeaks.message(e));
    }

    /**
     * Attempts to disconnect from the audio peaks server.
     * @since 2.1.7.9
     */
    disconnect() {
        if (!this.__connected) {
            return;
        }
        this.__reconnect = true;
        if (this.__socket) {
            this.__socket.addEventListener("close", () => {
                this.__connecting = false;
                this.__socket = null;
                //Required so we don't have two at once.
            });
            this.__socket.close();
        } else {
            this.__connecting = false;
        }
    }
}

/**
 * A static class that provides methods to bind and unbind event listeners
 * the the document for the purposes of this animation.
 * @since 2.1.7.18
 */
class Bindings {
    /**
     * The list of keybindings for the application.
     * @type {KeyBinding[]}
     * @since 2.1.7.18
     * @private
     */
    static keybinds = [
        {
            "key": "e",
            "conditions": [{
                "ctrl": false
            }],
            "handler": () => window.location.reload(),
            "description": "Reloads the page.",
            "group": "Animation"
        }, {
            "key": "s",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.toggleStatus,
            "description": "Toggles the status overlay on/off.",
            "group": "Overlays"
        }, {
            "key": "v",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.toggleVerboseStatus,
            "description": "Toggles verbose info in the status overlay.",
            "group": "Overlays"
        }, {
            "key": "r",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.reset,
            "description": "Resets the animation.",
            "group": "Animation"
        }, {
            "key": "h",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.toggleHelp,
            "description": "Toggles the help overlay on/off.",
            "group": "Overlays"
        }, {
            "key": "+",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.upFPS,
            "description": "Ups the FPS by 5.",
            "group": "Animation"
        }, {
            "key": "-",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.downFPS,
            "description": "Downs the FPS by 5.",
            "group": "Animation"
        }, {
            "key": "_",
            "conditions": [{
                "ctrl": false,
                "shift": true
            }],
            "handler": Ani.downFPS,
            "display": false
        }, {
            "key": "=",
            "conditions": [{
                "ctrl": false,
                "shift": false
            }],
            "handler": Ani.upFPS,
            "display": false
        }, {
            "key": "a",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.togglePeaks,
            "description": "Toggles the AudioPeaks subsystem.",
            "group": "Audio Peaks"
        }, {
            "key": "q",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.toggleAnimation,
            "description": "Toggles the animation on/off.",
            "group": "Animation"
        }, {
            "key": "t",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.resetDefaultSettings,
            "description": "Resets the default settings.",
            "group": "Animation"
        }, {
            "key": "z",
            "conditions": [{
                "ctrl": false
            }],
            "handler": Ani.debug,
            "display": false,
            "enabled": false
        }
        /*
         * Keybinds
         * a - toggle peaks
         * e - reload
         * h - toggle help
         * q - toggle animation
         * r - reset
         * s - toggle status
         * t - reset default settings
         * v - toggle verbose
         * z - debug binding
         * + - up fps
         * - - down fps
         */
    ];
    /**
     * The timeout number that handles hiding the mouse.
     * @type {number}
     * @since 2.1.7.18
     * @private
     */
    static timeout;
    /**
     * Whether or not the bindings have been bound to the document.
     * @type {boolean}
     * @since 2.1.7.18
     * @private
     */
    static bound;

    /**
     * A function that handles the processing for keystrokes.
     * @param {Event} e
     *     The event data for the KeyUp event.
     * @since 2.1.7.18
     * @private
     */
    static keyhandle(e) {
        if (Ani.stopping) {
            return; //Stopping animation, do not process keystrokes.
        }
        if (!Ani.started) {
            return; //Animation isn't loaded. do not process keystrokes.
        }
        const modifiers = ["ctrl", "alt", "shift", "meta"];
        Bindings.keybinds.forEach((binding) => {
            if (binding.key !== e.key) {
                return;
            }
            let keys = Object.keys(binding);
            if (keys.includes("enabled") && !binding.enabled) {
                return;
            }
            if (keys.includes("conditions") && binding.conditions.length > 0) {
                if (binding.conditions.some((cond) => {
                    keys = Object.keys(cond);
                    return !modifiers.some((mod) => keys.indexOf(mod) !== -1
                        && e[`${mod}Key`] === cond[mod]);
                })) {
                    return;
                }
            }

            binding.handler(e);
        });
    }
    /**
     * A function that handles the processing for hiding the mouse.
     * @since 2.1.7.18
     * @private
     */
    static mousehandle() {
        document.body.style.cursor = "default";
        if (!Bindings.timeout) {
            window.clearTimeout(Bindings.timeout);
        }
        Bindings.timeout = window.setTimeout(() => {
            Bindings.timeout = null;
            document.body.style.cursor = "none";
        }, 1000);
    }

    /**
     * Attaches the event listeners necessary to run the animation to their
     * targets.
     * @since 2.1.7.18
     */
    static bind() {
        if (Bindings.bound) {
            return;
        }
        Bindings.bound = true;
        document.body.addEventListener(
            "mousemove",
            Bindings.mousehandle
        );
        window.addEventListener(
            "keyup",
            Bindings.keyhandle
        );
        window.addEventListener(
            "resize",
            Ani.updateSize
        );
    }
    /**
     * Detaches the event listeners necessary to run the animation to their
     * targets.
     * @since 2.1.7.18
     */
    static unbind() {
        if (!Bindings.bound) {
            return;
        }
        Bindings.bound = false;
        document.body.removeEventListener(
            "mousemove",
            Bindings.mousehandle
        );
        window.removeEventListener(
            "keyup",
            Bindings.keyhandle
        );
        window.removeEventListener(
            "resize",
            Ani.updateSize
        );
    }

    /**
     * Generates a {@link StatusCollectionSettings} object from the
     * currently registered key-bindings.
     * @since 2.1.7.18
     * @returns {StatusCollectionSettings}
     *     The collection of settings that can be used to create a new Help
     *     overlay to detail the list of key-bindings in use by the
     *     application.
     */
    static generateHelpSettings() {
        const settings = {
            "title": "Key Bindings",
            "itemText": "Command",
            "valueText": "Explanation",
            "enableUpdate": false,
            "customCSS": "top: 0; right: 0",
            "rows": []
        };
        const groups = {};
        const noGroup = [];
        const groupList = [];
        Bindings.keybinds.forEach((obj) => {
            const keys = Object.keys(obj);
            if (keys.includes("display") && !obj.display) {
                return;
            }
            const row = {
                "type": "string",
                "value": () => ""
            };
            if (keys.includes("conditions") && obj.conditions.length > 0) {
                const cmap = obj.conditions.map((cond) => {
                    let str = Object
                        .keys(cond)
                    /* eslint-disable-next-line no-extra-parens */
                        .map((key) => (cond[key]
                            ? key.substring(0, 1).toUpperCase()
                            + key.substring(1).toLowerCase()
                            : null))
                        .filter((value) => value !== null)
                        .join(" + ");
                    if (str.length > 0) {
                        str += " + ";
                    }
                    return `${str}${obj.key}`;
                });
                row.name = cmap.length > 0
                    ? `(${cmap.join(",")})`
                    : `(${obj.key})`;
            } else {
                row.name = `(${obj.key})`;
            }
            if (
                keys.includes("description")
                && obj.description.trim().length > 0) {
                row.unit = obj.description.trim();
            }
            //Evaluate and generate object
            if (keys.includes("group") && obj.group.trim().length > 0) {
                if (!groupList.includes(obj.group.trim())) {
                    groupList.push(obj.group.trim());
                    groups[obj.group.trim()] = [];
                }
                groups[obj.group.trim()].push(row);
            } else {
                noGroup.push(row);
            }
        });

        noGroup.forEach((row) => {
            settings.rows.push(row);
        });
        groupList.forEach((group) => {
            settings.rows.push({
                "type": "header",
                "name": group
            });
            groups[group].forEach((row) => {
                settings.rows.push(row);
            });
        });
        return settings;
    }
}

if (document.readyState === "complete") {
    Ani.__constructor();
} else {
    window.addEventListener("load", () => Ani.__constructor());
}

/**
 * @todo Fix obsolete references and other issues in documentation and also
 *       complete documentation. (WORKING ON)
 * @todo Check github commit history to retroactively add the at since tags for
 *       all the objects missing them...
 * @todo Fix the buggy phaseshift code.
 * @todo add ability to modify keybindings (though I'm not sure why we would
 *       need this...)
 * @todo Open firefox and use the dev console to check GC for potential issues
 *       With the slowdown from peaks and a target fps of 30, we *are*
 *       averaging 29fps (which is really good given we have to do the
 *       calculations synchronously)
 *       See if there's any potential GC issues and fix them.
 * @todo (Note below from top):
 *       The only other modification that I can think of is Collision detection,
 *       ie: particles that bump other particles from behind randomly shift to
 *       the left or right
 * @todo Change how changing the FPS works (add in BASE_FPS and use that with
 *       current FPS to multiply the speed/acceleration/frequencies)
 *       This should effectively nullify the speed-up/down caused by changing
 *       the FPS. Why do this? This will increase the smoothness of the
 *       animation.
 *       We might have to move the background wipe to another "thread" and have
 *       that run at BASE_FPS so that the trails don't shrink/grow from
 *       changing the FPS.
 *       We need to debug the phaseshift code so that we can use it to detect
 *       the proper phaseshift in conjunction with this. (That will prevent
 *       jittering)
 *       One last note, the acceleration change might be more tricky than just
 *       multiplying by the ratio of the BASE_FPS and FPS.
 * @todo the "Q" (toggle animation) key-bind ultimately disables keybinds...
 * REMEMBER: Document AT SINCE for all new properties and objects.
 *
 * Unavailable eslint rules in my version of VisualStudio right now:
 *     no-loss-of-precision: 2
 *     no-promise-executor-return: 2
 *     no-unreachable-loop: 2
 *     no-unsafe-optional-chaining: 2
 *     no-useless-backreference: 2
 *     no-nonoctal-decimal-escape: 2
 */