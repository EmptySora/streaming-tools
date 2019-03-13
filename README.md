# streaming-tools
A collection of the tools I use when streaming in OBS.
See below for explanations about each tool.

## bot-storage
A C# application that runs in a service container. It was originally used to store most of the persistent data for the ruby version of my bot. After migrating to StreamLabs Chatbot, I phased this out.
It operates a REST API on localhost that can be used by a webserver or bot to access information.

## livesplit-remote
A simple front-end/back-end combo that works with the server plugin for Livesplit to create an easy to use mobile controller for Livesplit.
The idea was to use this to control livesplit since I tend to stream a bit far from my PC.
Then I discovered the joys of using a second keyboard, and I never used this again.

## old-chatbot
The original Ruby source code for my chat bot, built from the ground up, most of the code was migrated into a StreamLabs Chatbot Extension (see slchatbot-command-py).
It's slow-ish, and clunky but it did work, for what it's worth.

## splash-screens
Ever since OBS removed the ability to store HTML in a browser source, I moved such code to individual HTML files. In this directory are my End/Start/Paused splash screens.

## stream-timer
A timer that displays how long you have been streaming, self-explanatory. It'll display "NOT LIVE" if you are... you guessed it, not live. You must have a Twitch Client API Key to use this. You should also know your User ID, however, there's a function in main.js you can use to get it, refer to the source code.

## stream-titles-log
A PHP server application that uses MySQL to store a record of all the previously used titles/tags you've used while streaming. Please note the tool doesn't automatically pull such data, it just lets you record it. The "definitions.sql" file contains the MySQL database/table definition required to get it to work. Not 100% sure if it's valid sql, but if you use PHPMyAdmin or something, you could probably use the interface without any issues.

## streamers
An HTML5 application that uses the Canvas API, to animate what is similiar to rain flowing upwards. Probably the most heavily documented of these tools. A live demo is located [here](https://emptysora.github.io/streamers/main.html)

## tts-bot
An old C# based bot, that runs in a service container. it cooperates with the old-chatbot code to allow for low-ish latency text-to-speech. Unfortunately you can only choose the voice from the ones installed on your computer. [MS Documentation](https://docs.microsoft.com/en-us/previous-versions/office/developer/speech-technologies/dd167624(v%3Doffice.14))

## command-py
A Streamlabs chatbot script that allows flexibly formulating commands that require python scripting to be completed. (Eg: !wr to get the world record speedrun for the current game, requires speedrun.com api calls)
Not very well documented, but if you look at the already set-up code, you should get the structure of it.

## link-preivews
A streamlabs chatbot script that watches chat for users that post various links to sites, the bot will then query information about the link and display a preview in chat.
Currently, only YouTube is coded to work. Requires an API KEY from google to work, see the 
[api console](https://console.cloud.google.com/apis/dashboard).

