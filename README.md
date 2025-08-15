# Touch Gestures

The goal of this project is to build a foundation on how gestures should be handled accross all versions of OpenTabletDriver, until touch is officially supported.

This gesture system is in no way final, there is so much that could be improved and optimized.
Unfortunately, i do not have the necessary knowledge to properly do so, so if you have any suggestions, feel free to open an issue or a pull request.

## What is OpenTabletDriver?

OpenTabletDriver is an Cross-platform & Open-source Tablet driver that support a wide range of graphic tablets. It was made to replace
existing manufacturer drivers and provide better configurability than most. If a feature is missing, it can be added through plugins.

## What is Touch Gestures?

Touch Gesture is one of those plugins, it allows user with touch tablets to use gestures and perform actions.
A project of such scale would be difficult to integrate & maintain in the main repository, which is why this is part of an extension.

## What Gestures are Supported?

Any of the following gestures are supported:

![Supported Gestures](https://mrcubix.github.io/Touch-Gestures-Doc/_images/gesture_selection_screen.png)

## How to Install / Use

Check out the step-by-step guide on how to install & setup everything on the User Documentation!

[Install & Setup Guide](https://mrcubix.github.io/Touch-Gestures-Doc/installation_guide/)

Don't hesitate to also check out the FAQ there, and if any of your questions are not answered, feel free to open an issue.
I may be able to help you out & your question may help others in the future by being added to the FAQ.

## I am a Developer, how can i help?

- Packaging for Linux & MacOS (Binary Tar for each platforms + .deb?)

- ~~Currently i'm having issues dealing with both versions at the same time, mostly because of an ongoing bug in .NET project handling.  
It seems to be currently impossible to have have a multi-target project & have dependencies change depending on the target framework.  
What i'm noticing is that, the dependency with the highest version is always used, even if it's not compatible with the framework that is being targeted.~~

- You can't use multiple gestures at the same time, as gestures with higher requirements will cancel out the lower ones.

- The gesture system is not very optimized, and could be improved.

## What about a documentation of the current plugin system?

A developer documentation is currently being worked on, though i can't promise that it will be complete anytime soon.

## Why some of the file in the 0.6 version have `Bulletproof` in their name?

TLDR: `Bulletproof` is the codename for the 0.6 version.

Long version:

I initially thought of using `Bulletproof` as a codename for the version, this came from an inside joke in the OTD community, where Ghost claimed that the updater was bulletproof (It was not, it wiped people's data). It is still broken to this day.

![Bulletproof Updater](img/image.png)