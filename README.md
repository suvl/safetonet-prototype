# Recipe Search Prototype

This is an early prototype for a webservice + webapp that allows the user to search for 
recipes based on ingredients by speech. This is part of a technical test.

## Requirements

A simple application that searches and displays recipes. The recipes can be displayed in a list. A recipe
details screen should display the picture, the list of ingredients, and a link to display the Instructions page.

The goal of the application is to take commands via a text and voice interface as input, interpret what the
user wants and display the results as the output, using Web APIs:

* A recipe aggregator such as Food2Fork: https://food2fork.com/about/api
* A natural language platform for developers such as Wit.ai: https://wit.ai

Here is a list of sample commands:
* “What’s popular?”
* “Find recipes containing pork and rice ”
* “Display recipes named moussaka”
* “Show me a burger recipe”

Choose the best design and workflow to display the list, details, and instructions within the app while
providing seamless use to the command interface.

## What has been implemented

I'd like to start this topic by admiting that choosing a pre-release SDK was not the best choice, even so when
I used Fedora to work on it and Microsoft clearly only loves Debian/Ubuntu. I spent quite a few time trying to 
make it work then I quit and spinned off a windows VM. But then again, tried to work on linux as much as I could.

The app implements few commands with ingredients, successfully listing them with thumbnails. The backend implements
everything it needs voice and text search, as well as obtaining a list of recipes and a recipe detail. 

On the frontend there was quite a struggle with the WebRTC audio recording. The audio is not recorded and an empty
46 byte WAV file is always sent to the backend, resulting in a bad request from Wit.Ai. Testing with

` curl -vvv -F "audio=@/home/jm/sample.wav;type=audio/wav" http://localhost:5005/api/Search `

with a correctly recorded file yields the expected NLP result:

```javascript
{
    "_text" : "tomato and rice",
    "entities" : {
        "search_query" : [ {
        "suggested" : true,
        "confidence" : 0.938,
        "value" : "tomato",
        "type" : "value"
        }, {
        "suggested" : true,
        "confidence" : 0.93804,
        "value" : "rice",
        "type" : "value"
        } ],
        "user_intent" : [ {
        "confidence" : 0.68560915157093,
        "value" : "recipe"
        } ]
    },
    "msg_id" : "0qRGcqJvtEBE2vZsB"
}
```

and thus the correct json recipe listing. 

The SPA support native to aspnetcore is broken on v2.1, resulting in nodejs thinking the port chosen to run
is already being used (in development mode) or not shipping the `packages.json` file (in production mode). 
In a real-world environment, I'd say the FE and BE apps would run separately, so I've set the docker image
to launch them separately and use nginx to proxy to them. From this to actually separating it onto two images
is straightforward.

As I've been away from the FE development for two years now, I struggled with that quite more than the BE part. 
As expected, the design and maybe some arch decisions might be wrong. 

# Build

## Requirements

* dotnet sdk 2.1 preview2 
* nodejs 8.x or higher

## with docker

The Dockerfile is a multistage with build on top of it. So you just need to run

`docker build -t prototype .`

## native

Build the BE with `dotnet build`, it will restore the required nugets and build the app. Build the FE with `react-scripts build`
if you want the production-grade code.

# Run

If using docker, just run it like this:

`docker run --rm -it -p 80:80 -e WitAiConfiguration__ApiKey=$WITAPIKEY -e Food2ForkConfiguration__ApiKey=$FOODtoFORKAPI prototype` 

if you need logging, set `ASPNETCORE_ENVIRONMENT=Development` too.

If not using docker, just cd to `SafeToNet.Prototype.Api` and do `dotnet run` or `dotnet watch run`, changing the values at 
appsettings.json or with env variables. Then as the nodejs server might not work, cd to `ClientApp` and run `react-scripts start`.

As I've published the image to Dockerhub, you can also simply do this:

`docker run --rm -it -p 80:80 -e WitAiConfiguration__ApiKey=$WITAPIKEY -e Food2ForkConfiguration__ApiKey=$FOODtoFORKAPI suvl/stn-prototype` 