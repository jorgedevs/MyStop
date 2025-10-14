![MyStop GitHub Banner](/Assets/Images/jorgedevs-mystop.jpg)

# MyStop

This .NET MAUI app is intended to interact with Vancouver, BC's public system API (TransLink), providing real-time data on all their busses across the entire city.

## Important Note!

As of December 2024, TransLink replaced their Real Time Transit Information (RTTI) Web API with General Transit Feed Specification (GTFS), which means the current implementation needs work to adapt to the new common format for public transportation.

You can read more about it on the [App Developer Resources](https://www.translink.ca/about-us/doing-business-with-translink/app-developer-resources).

## Software Stack

## How it works

On every bus stop, there's a sign with a 5 digit code that you will enter on the app, and it'll query the bus arrival times on that specific stop, polling data every 10 seconds.

![MyStop GitHub Banner](/Assets/Images/jorgedevs-mystop-screens.png)

## Build 

## Support

Finding bugs or weird behaviors? File an [issue](https://github.com/jorgedevs/MyStop/issues) with repro steps.