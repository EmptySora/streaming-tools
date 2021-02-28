"use strict";
/* eslint-disable-next-line no-unused-vars */
const DefaultSettings = {
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
                "max": 25.0
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
    "resize": true
};
/*
 * Seriously...? CORS makes my life hell sometimes.
 * XHR won't work due to users potentially being tricked into opening
 * a local file that can then XHRs their entire file system and send arbitrary
 * files from your computer to a remote server...
 * JS Modules won't work (likely for a similar reason, stupidly enough)
 *
 * The whole point of XHR/Modules was to avoid polluting the global scope...
 * Guess we can't do that! So pollute the global scope we must... ugh...
 */
//Export {settings};